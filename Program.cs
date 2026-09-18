using System;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Threading;
using Microsoft.Win32;

namespace WT_SightRandomizer
{
    class Program
    {
        // ================= НАСТРОЙКИ ПУТЕЙ (ВСТАВЬТЕ СВОИ) =================
        private const string GlobalBlkPath = @"C:\Users\da\Documents\My Games\WarThunder\Saves\174346250\production\global.blk";
        private const string UserSightsPath = @"C:\Users\da\Documents\My Games\WarThunder\Saves\174346250\production\UserSights";
        
        private const string RegistryKeyName = "WTSightRandomizer";

        // ================= ВАША СТРАТЕГИЯ (ЧТО ВСТАВЛЯТЬ В БЛОК ТАНКА) =================
        // Вы можете менять эти строки как вам нужно. Программа сама запишет их внутрь каждого танка.
        private static readonly string[] MyStrategyTemplate = new string[]
        {
            "    crosshairColor:c=0, 0, 0, 255",
            "    crosshairLightColor:c=255, 36, 0, 255",
            "    crosshairNvColor:c=255, 36, 0, 255",
            "    crosshairThermalColor:c=0, 255, 60, 255",
            "    shotDistScaleOffset:i=0",
            "",
            "    rangefinder{",
            "      visible:b=no",
            "      textColor:c=0, 255, 60, 255",
            "      bgColor:c=0, 0, 0, 255",
            "    }"
        };

        static void Main(string[] args)
        {
            // 1. Проверяем, запущены ли мы скрытно вместе с Windows
            if (args.Contains("--silent"))
            {
                WaitForGameAndRun();
                return;
            }

            // 2. Иначе — запускаем обычное интерактивное меню в консоли
            ShowMenu();
        }

        // --- РЕЖИМ 1: ОБЫЧНОЕ МЕНЮ ---
        static void ShowMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("=== War Thunder Sight Randomizer ===");
                Console.ResetColor();
                Console.WriteLine("1. Перегенерировать прицелы прямо сейчас");
                
                // Проверяем статус автозапуска для красивого отображения
                bool isAutostartOn = CheckAutostartStatus();
                Console.Write("2. Автозапуск вместе с Windows: ");
                if (isAutostartOn) { Console.ForegroundColor = ConsoleColor.Green; Console.WriteLine("[ВКЛЮЧЕН]"); }
                else { Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine("[ВЫКЛЮЧЕН]"); }
                Console.ResetColor();

                Console.WriteLine("0. Выход");
                Console.Write("\nВыберите действие: ");

                string? choice = Console.ReadLine();
                if (choice == "1")
                {
                    Console.WriteLine("\nЗапуск рандомизации...");
                    ProcessSightsAndConfig();
                    Console.WriteLine("\nНажмите любую клавишу для возврата в меню...");
                    Console.ReadKey();
                }
                else if (choice == "2")
                {
                    ToggleAutostart(!isAutostartOn);
                }
                else if (choice == "0")
                {
                    break;
                }
            }
        }

        // --- РЕЖИМ 2: СКРЫТНЫЙ ФОНОВЫЙ МОНИТОРИНГ ---
        static void WaitForGameAndRun()
        {
            // Программа засыпает и раз в 5 секунд проверяет, запустилась ли игра.
            // Процесс лаунчера или самой игры в диспетчере обычно называется "aces" или "warthunder"
            while (true)
            {
                var processes = Process.GetProcessesByName("aces")
                    .Concat(Process.GetProcessesByName("warthunder"));

                if (processes.Any())
                {
                    // Игра запустилась! Меняем прицелы
                    ProcessSightsAndConfig();
                    break; // Завершаем скрытый процесс программы, она выполнила свою задачу
                }

                Thread.Sleep(5000); // Ждем 5 секунд перед следующей проверкой
            }
        }

        // --- СЕРДЦЕ ПРОГРАММЫ: АЛГОРИТМ ОПТИМИЗИРОВАННОЙ ЗАМЕНЫ СТРОК ---
        static void ProcessSightsAndConfig()
        {
            try
            {
                if (!File.Exists(GlobalBlkPath))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Ошибка: Файл не найден по пути {GlobalBlkPath}");
                    Console.ResetColor();
                    return;
                }

                if (!Directory.Exists(UserSightsPath))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Ошибка: Папка прицелов не найдена по пути {UserSightsPath}");
                    Console.ResetColor();
                    return;
                }

                // 1. Получаем случайный прицел из папки
                var blkFiles = Directory.GetFiles(UserSightsPath, "*.blk", SearchOption.AllDirectories);
                if (blkFiles.Length == 0)
                {
                    Console.WriteLine("В папке UserSights нет файлов .blk!");
                    return;
                }
                var random = new Random();
                string chosenSight = Path.GetFileNameWithoutExtension(blkFiles[random.Next(blkFiles.Length)]);

                // 2. Читаем файл игры
                string rawText = File.ReadAllText(GlobalBlkPath);

                // Быстрая проверка: если блок tankSightSettings вообще отсутствует, нам нечего редактировать
                if (!rawText.Contains("tankSightSettings{"))
                {
                    Console.WriteLine("Блок tankSightSettings не найден в файле global.blk.");
                    return;
                }

                // Разделяем текст на строки для построчной сборки
                string[] lines = File.ReadAllLines(GlobalBlkPath);
                using (StreamWriter writer = new StreamWriter(GlobalBlkPath, false))
                {
                    bool insideTankSettings = false;
                    bool insideSpecificTank = false;
                    int braceCount = 0;

                    for (int i = 0; i < lines.Length; i++)
                    {
                        string trimmed = lines[i].Trim();

                        if (trimmed.StartsWith("tankSightSettings{"))
                        {
                            insideTankSettings = true;
                            writer.WriteLine(lines[i]);
                            continue;
                        }

                        if (insideTankSettings)
                        {
                            // Считаем фигурные скобки, чтобы знать, когда выйдем из tankSightSettings
                            if (trimmed.Contains("{")) braceCount++;
                            if (trimmed.Contains("}")) braceCount--;

                            if (braceCount == -1) // Вышли из tankSightSettings
                            {
                                insideTankSettings = false;
                                writer.WriteLine(lines[i]);
                                continue;
                            }

                            // Мы внутри tankSightSettings. Ищем блоки конкретных танков (например, germ_vk_3002m{)
                            if (trimmed.EndsWith("{") && braceCount == 1)
                            {
                                insideSpecificTank = true;
                                writer.WriteLine(lines[i]); // Пишем имя танка, например germ_vk_3002m{
                                
                                // МГНОВЕННО вставляем имя случайного прицела и вашу стратегию!
                                writer.WriteLine($"    crosshair:t=\"{chosenSight}\"");
                                foreach (var strategyLine in MyStrategyTemplate)
                                {
                                    writer.WriteLine(strategyLine);
                                }
                                continue;
                            }

                            if (insideSpecificTank)
                            {
                                // Пропускаем (стираем) все старые настройки внутри танка, 
                                // так как мы уже записали туда новую стратегию и прицел
                                if (trimmed.Contains("}"))
                                {
                                    // Если дошли до закрытия блока танка, пишем скобку и выходим из режима стирания
                                    writer.WriteLine(lines[i]);
                                    insideSpecificTank = false;
                                }
                                continue; 
                            }
                        }

                        // Все остальные строки файла, не связанные с прицелами танков, пишем как есть
                        writer.WriteLine(lines[i]);
                    }
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Успех! Установлен прицел: {chosenSight}");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Произошла ошибка: {ex.Message}");
            }
        }

        // --- УПРАВЛЕНИЕ РЕЕСТРОМ WINDOWS ДЛЯ АВТОЗАПУСКА ---
        static bool CheckAutostartStatus()
        {
            using (var rk = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", false))
            {
                return rk?.GetValue(RegistryKeyName) != null;
            }
        }

        static void ToggleAutostart(bool enable)
        {
            using (var rk = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true))
            {
                if (rk == null) return;
                if (enable)
                {
                    string? exePath = Environment.ProcessPath; // <-- Вот здесь должна быть точка с запятой
                    if (exePath != null)
                    {
                        rk.SetValue(RegistryKeyName, $"\"{exePath}\" --silent");
                    }
                }
                else
                {
                    rk.DeleteValue(RegistryKeyName, false);
                }
            }
        }
    }
}
