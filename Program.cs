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
        private const string RegistryKeyName = "WTSightRandomizer";
        private const string ConfigFileName = "wt_randomizer_config.txt";


        private static string _globalBlkPath = "";
        private static string _userSightsPath = "";

        private static readonly string[] MyTanksList = new string[]
        {
            "germ_vk_3002m", "gunReady", "germ_pzkpfw_VI_ausf_h1_tiger", "turret", "fov", "gunReady", "germ_sdkfz_251_21", "germ_pzkpfw_VI_ausf_e_tiger", "gunReady", "germ_sdkfz_9_flak37", "germ_pzkpfw_V_ausf_a_panther", "germ_pzkpfw_V_ausf_g_panther", "germ_pzkpfw_VI_ausf_b_tiger_IIp", "germ_marder_1a1", "germ_bmp_1_ddr", "germ_pzkpfw_VI_ausf_b_tiger_IIh", "germ_mkpz_m48a2c", "germ_leopard_I", "germ_leopard_I_a1", "reticle", "germ_thyssen_henschel_tam", "germ_leopard_1a5", "germ_mkpz_m48a2ga2", "germ_marder_1a3", "germ_kpz_70", "germ_kpz_t72m1", "germ_panzerjager_nashorn", "germ_pzkpfw_V_ausf_d_panther", "germ_flakpz_I_Gepard", "germ_begleitpanzer_57", "germ_leopard_2k", "germ_radpanzer_90", "germ_pzkpfw_III_ausf_J_L42", "germ_leopard_2a4", "germ_schutzenpanzer_puma", "germ_thyssen_henschel_tam_2c", "germ_flakpz_1a2_Gepard", "germ_leopard_2a6", "germ_leopard_2a5", "germ_leopard_2a5_pso", "ussr_t_50", "ussr_t_80", "ussr_kv_1_L_11", "ussr_t_34_1941_l_11", "ussr_t_34_1942", "ussr_t_34_85_d_5t", "ussr_t_34_1941_57", "ussr_pt_76b", "ussr_kv_1s", "ussr_t_44", "ussr_is_2_1943", "ussr_su_100p", "ussr_is_2_1944", "ussr_btr_152d", "ussr_su_85m", "us_m2a4", "ussr_zsu_57_2", "us_lvt_a_1", "us_m4_sherman_promo", "us_m10", "us_halftrack_m16", "ussr_btr_80a", "ussr_t_44_100", "ussr_t_10m", "ussr_bmp_1", "ussr_object_906", "ussr_t_55_amd_1", "germ_pzkpfw_IV_ausf_F2", "ussr_bmp_2", "ussr_t_62", "ussr_t_55a", "ussr_t_72b3_arena", "ussr_t_72a", "ussr_bmp_3", "ussr_bmp_2m", "ussr_t_64a_1971", "it_ab_41", "it_l6", "fr_amd_35", "fr_hotchkiss_h39", "jp_type_98_ke_ni", "jp_type_97_kai", "jp_type_89b_i_go_otsu", "jp_type_1_chi_he", "jp_navy_120mm_spg", "sw_vickers_mk_e_37", "sw_pvlvv_fm42", "sw_ikv_72", "sw_sav_m43_1944", "sw_strv_m38", "sw_strv_m31", "sw_strv_m42_eh", "sw_lago_1", "sw_tgdgb_m40_lv", "sw_strv_m41_s2", "sw_pvkv_iv", "sw_l_62_anti_II", "sw_sherman_3_4", "sw_m24_chaffee_dk", "sw_pzkpfw_IV_ausf_J", "sw_pvkv_II", "sw_t_34_1941", "sw_pvkv_m43_1946", "sw_ikv_103", "sw_kv_1_1942_fin", "sw_lvkv_42", "sw_strv_m42_delat_torn", "sw_pvkv_m43_1963", "sw_pt_76b", "sw_a_34_comet", "sw_landsverk_ush_204_gk", "sw_strv_74", "sw_t_34_85_zis_53", "sw_charioteer_mk_7", "sw_bkan_1c", "sw_kungstiger", "sw_pbv_501", "sw_zsu_57_2", "germ_leopard_2a7v", "ussr_object_435", "sw_pbv_301", "sw_pvkv_III", "uk_valentine_mk_1", "uk_crusader_aa_mk_1", "uk_daimler_mk_2", "uk_a_13_mk2", "uk_crusader_mk_2", "it_as_42_metropolitana", "ussr_kv_1_zis_5", "uk_crusader_mk_3", "uk_3_inch_gun_carrier", "uk_17_pdr_m10_achilles", "germ_sdkfz_234_2", "germ_pzkpfw_IV_ausf_J", "germ_pzkpfw_IV_ausf_H", "germ_pzsfl_IVa_dickermax", "germ_flakpanzer_IV_Ostwind", "cn_m8_greyhound", "cn_type_97_kai", "cn_gmc_cckw_353_m45_quad", "cn_m8_scott", "cn_m3a3_stuart", "cn_m10", "ussr_t_64_b_1984", "sw_strv_101", "sw_landsverk_ush_405", "sw_strv_103a", "sw_strv_81", "sw_veak_40", "sw_pvrbv_551", "sw_t_80u", "sw_strv_104", "turret", "gunReady", "sw_strv_103c", "sw_itpsv_90", "sw_cv_90105", "sw_strv_105", "sw_cv_9030_fin", "us_m4a1_1942_sherman", "us_m24_chaffee", "us_m4a1_76w_sherman", "germ_flakpanzer_IV_Kugelblitz", "germ_le_kpz_m41", "germ_pzkpfw_III_ausf_F", "germ_pzkpfw_III_ausf_E", "germ_pzkpfw_II_ausf_C_DAK", "ussr_zsu_37_2", "germ_flakpanzer_IV_Ostwind_2", "ussr_bmd_4m2", "uk_sp_17_pdr_valentine", "uk_t17e2", "uk_cruiser_ram_1", "country_usa_1", "country_usa_2", "country_usa_3", "us_m4_sherman_calliope", "country_usa_4", "country_usa_5", "country_usa_6", "country_usa_7", "country_usa_8", "country_germany_1", "country_germany_2", "country_germany_3", "country_germany_4", "country_germany_5", "country_germany_6", "country_germany_7", "country_germany_8", "country_ussr_8", "country_ussr_1", "country_ussr_2", "us_m8_greyhound", "us_m2a4_1st_armor_div", "us_lvt_a_1_trb", "us_m2a2", "us_m8_scott", "us_m2_medium", "us_halftrack_m15", "us_halftrack_m13", "us_halftrack_m3_75mm_gmc", "us_m22_locust", "us_m3_stuart", "us_m3a1_stuart_usmc", "us_m3a1_stuart", "us_lvt_4_zis_2", "us_m3_lee", "us_mk1_grant", "us_lvt_a1_m24", "us_m5a1_stuart", "us_m8a1", "us_m4a3_105_sherman", "us_t18_e2", "us_m4a5_ram_2", "us_m4_sherman", "us_t77e1", "us_m4a2_sherman", "us_m55", "us_m44", "us_m19", "us_m42_duster", "us_t14", "us_m6a1", "us_t1e1", "us_m36", "us_skink_aa_kit_3rank", "us_m4a2_76w_sherman", "us_m4a3e2_sherman_jumbo", "us_t1e1_90", "us_m4a3e2_sherman_jumbo_cobra_king", "us_m4a3e8_76w_sherman_kit_3rank", "us_m4a3e8_76w_sherman", "us_m36b2", "us_m18_hellcat_black_cat", "us_t86", "us_m18_hellcat_kit_3rank", "us_m18_hellcat", "us_skink_aa", "us_m4_t26", "us_m109a1", "us_t20", "us_m4a3e2_76w_sherman_jumbo", "us_t25", "us_m41_walker_bulldog", "us_t30", "us_m18_super_hellcat", "us_t26e5", "us_m6a2e1", "us_t34", "us_m26_t99", "us_m56_scorpion", "us_m50_ontos", "us_m26_pershing", "us_t26e4_superpershing", "us_t92", "us_m46_patton_73_armor_bat", "us_m46_patton", "us_t29", "us_m551_76", "us_m728", "us_t95", "us_t32", "us_m163_vulcan", "us_m47_patton_II", "us_t54e2", "us_m103", "us_t114", "us_m48a1_patton_III", "us_t32e1", "us_t54e1", "us_m551", "us_m60", "us_t58", "us_m901_itv", "us_t95e1", "us_m60a2", "us_m60a1", "us_m3_bradley", "us_m60a1_rise_passive_era", "us_xm_800t", "us_m60a1_rise_mod", "us_xm246", "us_mim_72_chaparral", "us_efv_p1", "us_m60a3_tts", "us_xm_803", "us_m1296_dragoon", "us_m247", "us_xm1_gm", "us_mbt_70", "us_stingray", "us_merkava_mk_1", "us_merkava_mk_2b_late", "us_xm_8", "us_m3a3_bradley", "us_ccvl", "us_m60a3_slep", "us_losat_ccv", "us_xm_975_roland", "us_lav_ad", "us_m60_120s", "us_m1128_wolfpack", "us_m1128_mgs", "us_m1_abrams", "us_m10_booker_late", "us_m1_abrams_kvt", "us_m1_ip_abrams", "us_merkava_mk_3d", "us_m1a1_abrams", "us_m1a2_abrams", "us_m1a1_aim_abrams", "us_m1a1_hc_usmc", "us_m1a1_hc_abrams", "us_ags_teledyne", "us_rdf_lt", "us_hstv_l", "us_m1a1_hc_usmc_sm", "us_adats_bradley", "us_sl_amraam_fcs", "us_m1a2_sep3_abrams", "us_m1a2_sep2_abrams", "us_nasams_fcs", "us_m1a2_sep_abrams", "us_m1a2_sep2_abrams_trophy", "germ_sdkfz_221_s_pz_b_41", "germ_pzkpfw_III_ausf_B", "germ_sdkfz_251_10", "germ_sturmpanzer_II", "germ_pzkpfw_35t", "germ_pzkpfw_IV_ausf_C", "germ_flakpanzer_I_ausf_A", "germ_panzerjager_1", "germ_pzkpfw_II_ausf_C", "germ_sdkfz_251_9", "germ_pzkpfw_38t_ausf_A", "germ_nbfz_VI", "germ_pzkpfw_II_ausf_F", "germ_flakpanzer_38t_Gepard", "germ_sdkfz_222", "germ_sdkfz_234_1", "germ_pzkpfw_38t_Aufklarungspanzer", "germ_sdkfz_234_3", "germ_pzkpfw_IV_ausf_E", "germ_pzkpfw_38t_Marder_III", "germ_pzkpfw_38t_na", "germ_stug_III_ausf_A", "germ_sdkfz_6_2_flak36", "germ_pzkpfw_38t_ausf_F", "germ_pzkpfw_IV_ausf_F", "germ_panzerwerfer_42", "germ_amd_35_kwk39", "germ_pzkpfw_38t_Marder_III_ausf_H", "germ_pzkpfw_III_ausf_J", "germ_stuh_III_ausf_G", "germ_pzkpfw_III_ausf_L", "germ_hummel", "germ_stug_III_ausf_F", "germ_pzkpfw_III_ausf_M", "germ_pzkpfw_III_ausf_N", "germ_pzkpfw_IV_ausf_G", "germ_sdkfz_234_2_mod", "germ_pzsflk40_sturer_emil", "germ_stug_III_ausf_G", "germ_m44", "germ_m55", "germ_kv_2_754r", "germ_flakpanzer_IV_Wirbelwind", "germ_panzerbefelhswagen_IV_ausf_J", "germ_infanterie_kampfpanzer_churchill", "germ_t_34_747", "germ_kv_1B_finland", "germ_jgdpz_38t", "germ_sturmpanzer_IV_brummbar", "germ_jgdpz_IV_L48", "germ_sdkfz_234_4", "germ_kv_1_kwk_40", "germ_pzkpfw_VI_tiger_P", "germ_flakpanzer_IV_Ostwind_2_net", "germ_pzkpfw_VI_ausf_h1_tiger_animal_version", "germ_pzkpfw_VI_ausf_h1_tiger_east", "germ_vsw_flak_41", "germ_waffentrager_krupp_steyr", "germ_pz_IV_L70", "germ_pzkpfw_VI_ausf_h1_tiger_west", "germ_panzerbefelhswagen_VI_P", "germ_flakpanzer_zerstorer_45", "germ_pzkpfw_V_ausf_f_panther", "germ_kanonenjagdpanzer", "germ_panzerjager_panther", "germ_panzerbefelhswagen_jagdpanther", "germ_panzerjager_tiger", "germ_panzerjager_tiger_P_elefant", "germ_pzkpfw_VI_ausf_b_tiger_IIh_sla", "germ_panzerjager_tiger_P_ferdinand", "germ_spz_12_3", "germ_ru251", "germ_sppz2_luchs_a2", "germ_mkpz_m47", "germ_wiesel_1_mk20", "germ_pzh_2000", "germ_pzkpfw_Maus", "germ_pzkpfw_e_100", "germ_spz_oerlikon_raketenautomat", "germ_marder_clovis", "germ_raketenjagdpanzer_2", "germ_marder_df_105", "germ_erprobungstrager_3_achs_turm", "germ_raketenjagdpanzer_2_hot", "germ_vt_1_2", "germ_jaguar_2", "germ_sk105_a2", "germ_th_800_bismark", "germ_leopard_c2_mexas", "germ_leopard_a1a1_120", "germ_mkpz_super_m48", "germ_thyssen_henschel_tam_2ip", "germ_th_400", "germ_wiesel_2_adwc", "germ_wiesel_1_tow", "germ_flarakpz_1", "germ_leopard_2av", "germ_9a35_m", "germ_boxer_3105", "germ_9a33bm3", "germ_vilkas", "germ_boxer_swatrinf", "germ_leopard_2a4_pzbtl_123", "germ_schutzenpanzer_puma_vjtf", "germ_leopard_2_pt14", "germ_leopard_2a4m_can", "germ_schutzenpanzer_puma_vjtf_mod", "germ_leopard_2a4m_can_sm", "germ_flarakrad", "germ_leopard_2pl", "germ_iris_slm_fcs", "ussr_gaz_dshk", "ussr_t_26_1940", "ussr_gaz_4m", "ussr_bt_5", "ussr_t_26_4", "ussr_su_5_1", "ussr_t_26_1940_1st_GvTBr", "ussr_t_60_1941", "ussr_t_28_1938", "ussr_bm_8_24", "ussr_t_26E", "ussr_ba_11", "ussr_rbt_5", "ussr_bt_7_1937", "ussr_t_35", "ussr_t_28", "ussr_t_70_1942", "ussr_su_76m_1943", "ussr_bt_7_m", "ussr_pzkpfw_III_ausf_J_L42", "ussr_su_57", "ussr_su_76m_5st_kav_corps", "ussr_gaz_mm_72k", "ussr_zis_30", "ussr_t_28E", "ussr_su_76d", "ussr_zut_37", "ussr_bm_13n", "ussr_m3c", "ussr_btr_152a", "ussr_su_57b", "ussr_su_122", "ussr_zis_43", "ussr_t_126sp", "ussr_a_12_mk_2_matilda_2A_F96", "ussr_zsu_37", "ussr_zsu_29k", "ussr_zis_12_94KM_1945", "ussr_smk", "ussr_kv_8", "ussr_kv_2_1939", "ussr_bt_7a_f32", "ussr_su_85a", "ussr_t_34_1941", "ussr_t_34E", "ussr_su_152", "ussr_kv_2_1940", "ussr_sdkfz_251_21_kit_3rank", "ussr_asu_57", "ussr_su_85_1943", "ussr_su_100Y", "ussr_isu_152", "ussr_t_34_57_1943", "ussr_kv_85", "ussr_kv_2_zis_6", "ussr_isu_122", "ussr_m4a2_76w_sherman", "ussr_isu_122s", "ussr_t_34_85_zis_53_kit_3rank", "ussr_t_34_85_zis_53", "ussr_t_34_85_zis_53_v80", "ussr_is_1", "ussr_is_1_kit_3rank", "ussr_kv_122", "ussr_2s1", "ussr_su_122P", "ussr_su_100_1945", "ussr_2s3m", "ussr_kv_220", "ussr_pzkpfw_V", "ussr_t_34_85_stp_s53", "ussr_asu_85", "ussr_m53_59", "ussr_object_248", "ussr_t_44_po", "ussr_t_34_100", "ussr_type_62", "ussr_su_122_54", "ussr_2s19_m1", "ussr_2s19_m2", "ussr_object_268", "ussr_zsu_23_4m2", "ussr_is_3", "ussr_t_54_1947", "ussr_is_6", "ussr_is_4m", "ussr_t_10a", "ussr_t_54_1951", "ussr_to_55", "ussr_object_120", "ussr_t_54_1949", "ussr_pt_76_57", "ussr_zsu_23_4", "ussr_is_7", "ussr_it_1", "ussr_9p149", "ussr_object_140", "ussr_object_685", "ussr_t_55_am", "ussr_t_62m1", "ussr_btr_82at", "ussr_object_279", "ussr_2s25", "ussr_zsu_23_4m4", "ussr_object_775", "ussr_bmd_4", "ussr_bmd_4m", "ussr_9p157", "ussr_2s25m", "ussr_9a33bm3", "ussr_9a35_m2", "ussr_t_72b_1989", "ussr_t_72av_turms", "ussr_t_72b", "ussr_t_72m2_moderna", "ussr_object_292", "ussr_t_80b", "ussr_t_80ud", "ussr_2s38", "ussr_t_90a", "ussr_zprk_2s6", "ussr_tor_m1", "ussr_t_72b3_2011", "ussr_bmpt_72", "ussr_bmpt", "ussr_t_80u", "ussr_t_80um2", "ussr_t_80uk", "ussr_t_80ue1_sm", "ussr_t_80ue1", "ussr_pantsyr_s1", "ussr_t_72b3_arena_m", "ussr_t_90m_2020", "ussr_t_90m_arena_m", "ussr_pantsyr_sm_sv", "ussr_buk_m3_fcs", "ussr_t_80bvm", "uk_a_13_mk1", "uk_vickers_mk_6_aa_mk_1", "uk_a17_mk_1_tetrarch", "uk_a_13_mk1_3rd_rtr", "uk_a1e1_independent", "uk_sarc_mk4_a", "uk_a_13_mk2_1939", "uk_alecto_mk_1", "uk_m3_stuart", "uk_m3a1_stuart", "uk_a25_mk_8", "uk_marmon_herrington_mk_6_2pdr", "uk_a_12_mk_2_matilda_2", "uk_mk1_grant", "uk_churchill_avre", "uk_crusader_mk_2_the_saint", "uk_matilda_hedgehog", "uk_valentine_mk_11", "uk_valentine_mk_9", "uk_armored_car_aec_mk_2", "uk_a_22_mk_1_churchill_1941", "uk_m4a5_ram_2", "uk_a27m_cromwell_1", "uk_marmon_herrington_mk_6_6pdr", "uk_sherman_II", "uk_armored_car_mk_2_aa", "uk_a27m_cromwell_5", "uk_a27m_cromwell_5_rp3", "uk_m44", "uk_crusader_aa_mk_2_kit_3rank", "uk_crusader_aa_mk_2", "uk_ystervark_spaa", "uk_a_22b_mk_3_churchill_1942", "uk_churchill_na75", "uk_a_33_excelsior", "uk_a30_sp_avenger", "uk_sherman_vc_firefly", "uk_sherman_ic_firefly", "uk_a30_sp_avenger_kit_3rank", "uk_sherman_vc_firefly_kit_3rank", "uk_concept3_ngac", "uk_a_22f_mk_7_churchill_crocodile", "uk_a_22f_mk_7_churchill_1944", "uk_a_34_comet", "uk_a30_challenger", "uk_ram_90mm_aa", "uk_bosvark", "uk_a_34_comet_iron_duke", "uk_ac4_thunderbolt", "uk_skink_aa", "uk_centurion_mk_1", "uk_m109a1", "uk_charioteer_mk_7", "uk_fv4005", "uk_g6_spg", "uk_ratel_90", "uk_a39_tortoise", "uk_centurion_mk_2", "uk_ratel_20", "uk_fv4004_conway", "uk_vickers_gbt_155", "uk_as_90_mk_2", "uk_fv4202", "uk_centurion_mk_3", "uk_eland_90_mk_7", "uk_centurion_action_x", "uk_centurion_mk_5_avre_era", "uk_fv107_scimitar", "uk_conqueror_mk_2", "uk_fv221_caernarvon", "uk_centurion_mk_5_raac", "uk_vickers_mbt_mk_1", "uk_centurion_mk_10", "uk_fv721_fox", "uk_fv438_swingfire", "uk_falcon", "uk_fv102_striker", "uk_ratel_zt3", "uk_fv107_scimitar_mk2", "uk_vickers_mbt_mk_3", "uk_fv510_isv", "uk_chieftain_marksman", "uk_olifant_mk_1a", "uk_chieftain_mk_3", "uk_rooikat_76", "uk_chieftain_mk_5", "uk_rooikat_za_35", "uk_vickers_mk_11", "uk_badger", "uk_olifant_mk_2", "uk_rooikat_mttd", "uk_vfm_5", "uk_khalid", "uk_chieftain_mk_10", "uk_chieftain_900", "uk_tracked_rapier", "uk_rooikat_105_td", "uk_shir_2", "uk_desert_warrior", "uk_ttd", "uk_stormer_air_defence", "uk_challenger_1", "uk_9a35_m", "uk_ajax", "uk_stormer_hvm", "uk_9a33bm2", "uk_boxer_crv_block2", "uk_t_90s_bheeshma", "uk_vickers_mk7", "uk_challenger_mk_3", "uk_challenger_1_mk_3_gulf", "uk_challenger_2_dorchester", "uk_challenger_II", "uk_challenger_2_megatron_sm", "uk_challenger_2_tes", "uk_challenger_2_megatron", "uk_m1a1_aim_abrams", "uk_adats_m113a3", "uk_challenger_2_lep", "uk_challenger_2_bn", "uk_sky_sabre_fcs", "uk_challenger_2e", "uk_m1a2_sep3_abrams", "jp_type_95_ha_go", "jp_type_2_ka_mi", "jp_type_95_ha_go_commander", "jp_type_94", "jp_type_97_chi_ha", "jp_type_95_heavy", "jp_type_4_ho_ro", "jp_hiro_sha", "jp_type_98_ta_se", "jp_type_2_ho_i", "jp_type_2_ho_ni_2", "jp_type_97_chi_ha_12cm", "jp_type_3_ho_ni_I", "jp_type_3_ho_ni_III", "jp_halftrack_m16", "jp_type_1_chi_he_5th_regiment", "jp_type_95_so_ki", "jp_type_5_na_to", "jp_type_3_chi_nu", "jp_m44", "jp_m42_duster", "jp_m19", "jp_m24_chaffee", "jp_type_3_chi_nu_75cm_type_5", "jp_type_4_chi_to_late", "jp_type_4_chi_to", "jp_sub_i_ii_20mm", "jp_m4a3e8_76w_sherman", "jp_type_5_chi_ri", "jp_m36b2_jgsdf", "jp_pzkpfw_VI_ausf_e_tiger", "jp_st_a1", "jp_m41_walker_bulldog", "jp_st_a2", "jp_type_75", "jp_type_61_mod", "jp_st_a3", "jp_type_61", "jp_type_60_sprg", "jp_type_60_atm", "jp_type_75_mlrs", "jp_m47_patton_II", "jp_m163_vulcan", "jp_type_99", "jp_type_87_rcv_prot", "jp_type_74_c", "jp_st_b1", "jp_type_74_red_star", "jp_type_87", "jp_btr_3e1", "jp_type_74_f", "jp_type_89", "jp_type_87_rcv", "jp_type_74", "jp_m60a3_tts", "jp_stingray", "country_japan_6", "jp_type_74_mod_g_kai", "jp_type_16_mcv_prot", "jp_type_16_mod", "country_japan_7", "jp_type_93", "jp_icv_prototype", "jp_type_16", "jp_icv_ifv_prototype", "jp_type_90", "jp_type_90b_sm", "jp_type_90b", "jp_type_81_tansam", "jp_type_90b_camo", "jp_type_10_prototype", "jp_type_81_tansam_fcs", "jp_oplot_t", "jp_leopard_2ri", "jp_type_03_chusam_fcs", "jp_type_10", "jp_tkx_prot", "cn_t_26_1940", "cn_t_26_no531", "cn_type_97_chi_ha", "cn_sdkfz_222_early", "cn_su_76m_1943", "country_china_2", "cn_lvt_4_zis_2", "cn_m3a3_stuart_1st_ptg", "cn_m5a1_stuart", "cn_m4a4_sherman_1st_ptg", "cn_m4a4_sherman", "cn_m55", "cn_m42_duster", "cn_t_34_1942", "cn_m4a1_76w_sherman", "cn_m24_chaffee", "cn_isu_152", "cn_object_211", "cn_type_65_aa", "cn_m36", "cn_isu_122", "cn_m41_a3_walker_bulldog", "cn_t_34_85_d_5t", "cn_type_58", "cn_t_34_85_zis_53_no215", "cn_zsd63_pg87", "cn_is_2_1943", "cn_is_2_1943_no402", "cn_type_64", "cn_su_100_1945", "cn_m18_hellcat", "cn_plz_83", "cn_zsl_92", "cn_type_63_I", "cn_is_2_1944", "cn_type_62", "cn_wz_141", "cn_plz_83_130", "cn_zts_63_1980", "cn_plz_05", "cn_m48a1_patton_III", "cn_type_86", "cn_pgz_88", "cn_ztz_59a", "cn_type_59", "cn_type_69", "cn_wz_305", "cn_m113a1_tow", "cn_m_41d", "cn_type_59d", "cn_object_122tm", "cn_cm_25", "cn_type_69_2g", "cn_hj_9", "cn_ztz_88b", "cn_ptz_89", "cn_ptl_02", "cn_wma_301", "cn_m60a3_tts", "cn_ztz_88a", "cn_pgz_09", "cn_pgz_04a", "cn_ztz_96", "cn_cm11", "cn_cm_34", "cn_ztl_11", "cn_qn506", "cn_antelope_tc_1l_ads", "cn_zbd_04a", "cn_ztz_96a_prot", "cn_ztz_96a", "cn_ztz_99_w", "cn_t_80ud_478be", "cn_al_khalid_1", "cn_mbt2000", "cn_vt_5", "cn_mbt2000_sm", "cn_ztz_96b", "cn_tor_m1", "cn_hq_17", "cn_pgz_625_fb10", "cn_ztz_99a", "cn_oplot_t", "cn_wz_1001", "cn_vt_4", "cn_m1a2t", "cn_hq_11", "cn_vt_4b", "it_l3_cc", "it_m11_39", "it_39m_csaba", "it_l6_leone", "it_semovente_l40", "it_lancia3ro_100", "it_m13_40_serie_1", "it_m14_41", "it_m13_40_serie_2", "it_m13_40_serie_3", "it_as_42_47", "it_semovente_m41_75_18", "it_semovente_m42_75_34", "it_semovente_m41_75_32", "it_m14_41_47_40", "it_40m_turan_1", "it_fiat_cm52", "it_40_43m_zrinyi_2", "it_m3a3_stuart", "it_sahariano", "it_ab_43", "it_m15_42", "it_semovente_m43_105", "it_p_40", "it_p_26_40", "it_pzkpfw_III_ausf_N", "it_semovente_m43_75_34", "it_semovente_m41m_90", "it_m44", "it_m4a4_sherman", "it_sherman_75_37", "it_m55", "it_m15_42_contraereo", "it_43m_turan_3", "it_pzkpfw_IV_ausf_G", "it_breda_52_autocannone", "it_stug_III_ausf_G", "it_sherman_VII", "it_semovente_m43_75_46", "it_semovente_breda_501", "it_sherman_vc_firefly", "it_2s1", "it_m109g", "it_m36b1", "it_leopard_bofors", "it_oto_r3_t20_fa", "it_m18_hellcat", "it_pzkpfw_VI_ausf_e_tiger", "it_m26a1_pershing", "it_palmaria", "it_m26_ariete", "it_fiat_6616_cockerill", "it_fiat_6614_106sr", "it_zsu_57_2", "it_btr_80a_hungary", "it_c13_t90", "it_pzh_2000_hu", "it_m47_105", "it_fiat_6614_firos", "it_m60a1_ariete", "it_aubl_74_60_70m", "it_zsu_23_4", "it_oto_r3_106sr", "it_leopard_1a2", "it_of_40_mk_1", "it_otobreda_sidam_25", "it_c13_tua", "it_m113a1_tow", "it_of_40_mk_2a", "it_leopard_1a5", "it_t_72m1", "it_of_40_mtca", "it_vcc_80_hitfist_60", "it_b1_centauro", "it_otobreda_sidam_25_mistral", "it_b1_centauro_romor", "it_vrcc_centauro", "it_vbc_pt2", "it_dardo_vcc", "it_vcc_80_hitfist_30", "it_9a33bm3", "it_9a35_m", "it_leopard_2a4", "it_freccia_hitfist_ows", "it_c1_ariete_preserie", "it_freccia", "it_kf_41", "it_otomatic", "it_c1_ariete_pso", "it_centauro_mgs_120", "it_centauro_rgo_120", "it_c1_ariete_certezza", "it_c1_ariete", "it_centauro_2", "it_ariete_amv_pt1", "it_samp_t_fcs", "it_leopard_2a7_hungary", "fr_hotchkiss_h35", "fr_amc_34", "fr_amr_35_zt3", "fr_fcm_36", "fr_hotchkiss_h39_cambronne", "fr_renault_r39", "fr_renault_d2", "fr_lorraine_37l", "fr_citroen_kegresse_p4t", "fr_somua_s35", "fr_amc_35", "fr_char_2c_bis", "fr_somua_sau40", "fr_amd_35_sa35", "fr_b1_bis", "fr_char_2c", "fr_cckw_353_bofors", "fr_crusader_mk_2", "fr_b1_ter", "fr_amd_35_kwk39", "fr_m3a3_stuart", "fr_amx_vtt_dca", "fr_m4a3_105_sherman", "fr_m10", "fr_m4a1_sherman", "fr_amx_13_chaffee", "fr_m4a4_sherman", "fr_arl_44_acl1", "fr_m44", "fr_m55", "fr_amx_13_fl_11", "fr_m4a4_cn_75_50", "fr_m4a1_sherman_fl_10", "fr_amx_13_dca_40", "fr_arl_44", "fr_panhard_ebr_1951", "fr_m36b2_cefeo", "fr_tpk_641_vpc", "fr_m4a3e2_sherman_jumbo", "fr_lorraine_100", "fr_pzkpfw_V_panther_dauphine", "fr_amx_elc_bis", "fr_amx_elc_901", "fr_amx_10p", "fr_amx_30_auf_1", "fr_m26_pershing", "fr_amx_13_75_ss11", "fr_amx_13_75", "fr_amx_m4", "fr_m46_patton", "country_france_5", "fr_amx_50_foch", "fr_amx_10m_acra", "fr_amx_50_surblinde", "fr_aml_90", "fr_amx_13_90", "fr_panhard_ebr_1963", "fr_amx_50_surbaisse", "fr_amx_50", "fr_lorraine_40t", "fr_somua_sm", "fr_bat_chat_25t", "fr_amx_30", "fr_marder_clovis", "fr_amx_30_1972", "fr_marder_df_105", "fr_amx_50_1950", "fr_mars_15", "fr_amx_30_ACRA", "fr_amx_10rc", "fr_amx_13_hot", "fr_amx_30_b2_brenus", "fr_amx_30_dca", "fr_sk105_a2", "fr_amx_30_b2", "fr_vcac_mephisto", "fr_amx_30_super", "fr_vab_santal", "fr_vbci", "fr_amx_32_105", "fr_leopard_1a5be", "fr_vbci2_mct30", "fr_amx_32", "fr_cv_9035_nl", "fr_amx_30_roland", "fr_amx_40", "fr_vextra_105", "fr_leopard_2a4nl", "fr_leopard_2a4nl_les", "fr_msc", "fr_jaguar_ebrc", "fr_crotale_ng", "fr_leopard_2a5nl", "fr_samp_t_fcs", "fr_leclerc_azur", "fr_leclerc_sxxi", "fr_leclerc_s1", "fr_leclerc_s2", "fr_leopard_2a6nl", "sw_vickers_mk_e_45", "sw_strv_m39", "sw_bt_42", "sw_t_28", "sw_strv_m40l", "sw_pbil_m40", "sw_stormpjas_fm43_44", "sw_strv_m41_s1", "sw_sav_m43_1946", "sw_t_50_fin", "sw_strv_81_rb52", "sw_t_54_1951", "sw_ikv_91", "sw_strv_103_0", "sw_k9_vidar", "sw_udes_33", "sw_pbv_302_bill", "sw_bmp_2md", "sw_t_55m", "sw_ikv_91_105", "sw_t_72m1", "sw_leopard_1a5no", "sw_cv_9035_dk", "sw_strf_90b", "sw_lvrbv_701", "sw_patria_amv_ctcv_105", "sw_asrad_r", "sw_strf_9056", "sw_cv_90105_tml", "sw_lvkv_90c", "sw_strf_90c", "sw_strv121b_christian2", "sw_strv_121", "sw_leopard_2a4_fin", "sw_cv_90_mk4", "country_sweden_8", "sw_eldenhet_98", "sw_cv_90120", "sw_crotale_ng", "sw_nomads", "sw_leopard_2a6nl", "sw_strv_122b_plss", "sw_strv_122", "sw_strv_122b_plus", "sw_nasams_fcs", "il_tcm_20", "il_m109", "il_m109a1", "il_m_51", "il_m_51_w", "il_ss_11_halftrack", "il_amx_13_75", "il_sholef", "il_m163_vulcan", "il_aml_90", "il_m48a1_patton_III", "il_centurion_mk_5_shot", "il_magach_2", "il_m113_hvms", "il_magach_6a", "il_magach_6", "il_magach_3_idf", "il_magach_5", "il_zsu_23_4", "il_magach_3", "il_tiran_4", "il_tiran_4_sh", "il_centurion_shot_kal_alef", "il_m113a1_tow", "il_magach_6b", "il_magach_6_rocket", "il_tiran_6", "il_centurion_shot_kal_gimel", "il_magach_6m", "il_centurion_shot_kal_d", "il_magach_6c", "il_machbet", "il_magach_7c", "il_merkava_mk_1", "il_magach_6b_gal", "il_magach_6b_gal_batash", "il_merkava_mk_2b_early", "il_merkava_mk_1b", "il_merkava_mk_2d", "il_mim_72_chaparral", "il_sabra_mk1", "il_hunter_afv", "il_merkava_mk_3_raam_segol", "il_eitan", "il_merkava_mk_3b", "il_namer_rcws_30", "il_merkava_mk_3c", "il_namer_tsrikhon", "il_merkava_mk_4b", "il_merkava_mk_4_lic", "il_merkava_mk_4m", "il_spyder_aio"
        };


        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern bool AllocConsole();

        static void Main(string[] args)
        {
            InitializePaths();

            if (args.Contains("--silent"))
            {
                WaitForGameAndRun();
                return;
            }

            AllocConsole();
            ShowMenu();
        }

        static void InitializePaths()
        {
            if (File.Exists(ConfigFileName))
            {
                var lines = File.ReadAllLines(ConfigFileName);
                if (lines.Length >= 2 && Directory.Exists(Path.GetDirectoryName(lines[0])) && Directory.Exists(lines[1]))
                {
                    _globalBlkPath = lines[0];
                    _userSightsPath = lines[1];
                    return;
                }
            }

            try
            {
                string myDocuments = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string wtSavesPath = Path.Combine(myDocuments, "My Games", "WarThunder", "Saves");

                if (Directory.Exists(wtSavesPath))
                {
                    var playerFolders = Directory.GetDirectories(wtSavesPath);
                    foreach (var folder in playerFolders)
                    {
                        string prodFolder = Path.Combine(folder, "production");
                        string testBlk = Path.Combine(prodFolder, "global.blk");
                        string testSights = Path.Combine(prodFolder, "UserSights");

                        if (File.Exists(testBlk) && Directory.Exists(testSights))
                        {
                            _globalBlkPath = testBlk;
                            _userSightsPath = testSights;
                            File.WriteAllLines(ConfigFileName, new string[] { _globalBlkPath, _userSightsPath });
                            return;
                        }
                    }
                }
            }
            catch { }

            if (string.IsNullOrEmpty(_globalBlkPath) && !Environment.GetCommandLineArgs().Contains("--silent"))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Auto-detection failed to locate the game folder. Please enter the full path to the 'production' directory manually.");
                Console.Write("Example (C:\\Users\\username\\Documents\\My Games\\WarThunder\\Saves\\12345\\production): ");
                Console.ResetColor();
                
                string? input = Console.ReadLine()?.Trim();
                if (!string.IsNullOrEmpty(input) && Directory.Exists(input))
                {
                    _globalBlkPath = Path.Combine(input, "global.blk");
                    _userSightsPath = Path.Combine(input, "UserSights");
                    File.WriteAllLines(ConfigFileName, new string[] { _globalBlkPath, _userSightsPath });
                }
            }
        }

        static void ShowMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("=== War Thunder Sight Randomizer ===");
                Console.ResetColor();

                if (!string.IsNullOrEmpty(_globalBlkPath))
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine($"Game file: {Path.GetFileName(_globalBlkPath)} (Path successfully resolved)");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("CRITICAL ERROR: Game paths are not set!");
                    Console.ResetColor();
                }

                Console.WriteLine("\n1. Regenerate custom sights right now");
                
                bool isAutostartOn = CheckAutostartStatus();
                Console.Write("2. Use auto-generate with start game: ");
                if (isAutostartOn) { Console.ForegroundColor = ConsoleColor.Green; Console.WriteLine("[On]"); }
                else { Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine("[Off]"); }
                Console.ResetColor();

                Console.WriteLine("0. Exit");
                Console.Write("\nSelect an option: ");

                string? choice = Console.ReadLine();
                if (choice == "1" && !string.IsNullOrEmpty(_globalBlkPath))
                {
                    Console.WriteLine("\nStarting randomization...");
                    ProcessSightsAndConfig();
                    Console.WriteLine("\nPress any key to return to the menu...");
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

        static void WaitForGameAndRun()
        {
            if (string.IsNullOrEmpty(_globalBlkPath)) return;

            while (true)
            {
                var processes = Process.GetProcessesByName("aces")
                    .Concat(Process.GetProcessesByName("warthunder"));

                if (processes.Any())
                {

                    Thread.Sleep(10000); 
                    
                    ProcessSightsAndConfig();

                    while (true)
                    {
                        var activeRunningProcesses = Process.GetProcessesByName("aces")
                            .Concat(Process.GetProcessesByName("warthunder"));

                        if (!activeRunningProcesses.Any())
                        {
                            break; 
                        }

                        Thread.Sleep(10000);
                    }
                }

                Thread.Sleep(5000);
            }
        }
        static void ProcessSightsAndConfig()
        {
            try
            {
                if (!File.Exists(_globalBlkPath) || !Directory.Exists(_userSightsPath)) return;

                var blkFiles = Directory.GetFiles(_userSightsPath, "*.blk", SearchOption.AllDirectories);
                if (blkFiles.Length == 0)
                {
                    Console.WriteLine("No .blk files found in the game's UserSights folder!");
                    return;
                }
                var random = new Random();


                string GetRandomSightName()
                {
                    string randomPath = blkFiles[random.Next(blkFiles.Length)];
                    return Path.GetFileNameWithoutExtension(randomPath);
                }


                string[] lines = File.ReadAllLines(_globalBlkPath);
                
                using (StreamWriter writer = new StreamWriter(_globalBlkPath, false))
                {
                    bool insideMainBlockToSkip = false;
                    int skipBraceCount = 0;

                    for (int i = 0; i < lines.Length; i++)
                    {
                        string trimmed = lines[i].Trim();

                        if (trimmed.StartsWith("tankSightSettings{"))
                        {
                            insideMainBlockToSkip = true;
                            skipBraceCount = 1;

                            writer.WriteLine("tankSightSettings{");
                            writer.WriteLine($"  crosshair:t=\"{GetRandomSightName()}\"");
                            writer.WriteLine(); 

                            foreach (var tank in MyTanksList)
                            {
                                writer.WriteLine($"  {tank}{{");
                                writer.WriteLine($"    crosshair:t=\"{GetRandomSightName()}\"");
                                writer.WriteLine("  }");
                                writer.WriteLine();
                            }

                            writer.WriteLine("}");
                            continue;
                        }

                        if (insideMainBlockToSkip)
                        {

                            foreach (char ch in lines[i])
                            {
                                if (ch == '{') skipBraceCount++;
                                if (ch == '}') skipBraceCount--;
                            }

                            if (skipBraceCount == 0) insideMainBlockToSkip = false;
                            continue; 
                        }

                        writer.WriteLine(lines[i]);
                    }
                }

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Success! Configuration has been successfully regenerated.");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error modifying file: {ex.Message}");
            }
        }

        //AutoStart with Windows
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
                    string? exePath = Environment.ProcessPath;
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