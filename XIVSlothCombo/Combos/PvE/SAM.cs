using Dalamud.Game.ClientState.JobGauge.Enums;
using Dalamud.Game.ClientState.JobGauge.Types;
using System.Linq;
using XIVSlothCombo.Combos.JobHelpers;
using XIVSlothCombo.Combos.PvE.Content;
using XIVSlothCombo.CustomComboNS;
using XIVSlothCombo.CustomComboNS.Functions;
using XIVSlothCombo.Data;
using XIVSlothCombo.Extensions;

namespace XIVSlothCombo.Combos.PvE
{
    internal class SAM
    {
        public const byte JobID = 34;

        public static int NumSen(SAMGauge gauge)
        {
            bool ka = gauge.Sen.HasFlag(Sen.KA);
            bool getsu = gauge.Sen.HasFlag(Sen.GETSU);
            bool setsu = gauge.Sen.HasFlag(Sen.SETSU);
            return (ka ? 1 : 0) + (getsu ? 1 : 0) + (setsu ? 1 : 0);
        }

        public const uint
            Hakaze = 7477,
            Yukikaze = 7480,
            Gekko = 7481,
            Enpi = 7486,
            Jinpu = 7478,
            Kasha = 7482,
            Shifu = 7479,
            Mangetsu = 7484,
            Fuga = 7483,
            Oka = 7485,
            Higanbana = 7489,
            TenkaGoken = 7488,
            MidareSetsugekka = 7487,
            Shinten = 7490,
            Kyuten = 7491,
            Hagakure = 7495,
            Guren = 7496,
            Senei = 16481,
            MeikyoShisui = 7499,
            Seigan = 7501,
            ThirdEye = 7498,
            Iaijutsu = 7867,
            TsubameGaeshi = 16483,
            KaeshiHiganbana = 16484,
            Shoha = 16487,
            Ikishoten = 16482,
            Fuko = 25780,
            OgiNamikiri = 25781,
            KaeshiNamikiri = 25782,
            Yaten = 7493,
            Gyoten = 7492,
            KaeshiSetsugekka = 16486,
            TendoGoken = 36965,
            TendoKaeshiSetsugekka = 36968,
            Zanshin = 36964,
            TendoSetsugekka = 36966,
            Gyofu = 36963;

        public static class Buffs
        {
            public const ushort
                MeikyoShisui = 1233,
                EnhancedEnpi = 1236,
                EyesOpen = 1252,
                OgiNamikiriReady = 2959,
                Fuka = 1299,
                Fugetsu = 1298,
                KaeshiSetsugekkaReady = 4216,
                TendoKaeshiSetsugekkaReady = 4218,
                KaeshiGokenReady = 3852,
                TendoKaeshiGokenReady = 4217,
                ZanshinReady = 3855,
                Tendo = 3856;
        }

        public static class Debuffs
        {
            public const ushort
                Higanbana = 1228;
        }
        public static class Traits
        {
            public const ushort
                EnhancedHissatsu = 591,
                EnhancedMeikyoShishui2 = 593;
        }

        public static class Config
        {
            public static UserInt
                SAM_STSecondWindThreshold = new("SAM_STSecondWindThreshold", 25),
                SAM_STBloodbathThreshold = new("SAM_STBloodbathThreshold", 40),
                SAM_AoESecondWindThreshold = new("SAM_AoESecondWindThreshold", 25),
                SAM_AoEBloodbathThreshold = new("SAM_AoEBloodbathThreshold", 40),
                SAM_VariantCure = new("SAM_VariantCure");

            public static UserFloat
                SAM_ST_Higanbana_Threshold = new("SAM_ST_Higanbana_Threshold", 1),
                SAM_ST_KenkiOvercapAmount = new("SamKenkiOvercapAmount", 50),
                SAM_AoE_KenkiOvercapAmount = new("SamAOEKenkiOvercapAmount", 50),
                 SAM_ST_ExecuteThreshold = new("SAM_ST_ExecuteThreshold", 1);
        }
        internal class SAM_ST_YukikazeCombo : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SAM_ST_YukikazeCombo;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                SAMGauge? gauge = GetJobGauge<SAMGauge>();
                float SamKenkiOvercapAmount = Config.SAM_ST_KenkiOvercapAmount;

                if (actionID is Yukikaze)
                {
                    if (CanWeave(actionID))
                    {
                        if (gauge.Kenki >= SamKenkiOvercapAmount && LevelChecked(Shinten))
                            return Shinten;
                    }

                    if (HasEffect(Buffs.MeikyoShisui) && LevelChecked(Yukikaze))
                        return Yukikaze;

                    if (comboTime > 0)
                    {
                        if (lastComboMove is Hakaze && LevelChecked(Yukikaze))
                            return Yukikaze;
                    }
                    return Hakaze;
                }
                return actionID;
            }
        }

        internal class SAM_ST_KashaCombo : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SAM_ST_KashaCombo;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte levels)
            {
                SAMGauge? gauge = GetJobGauge<SAMGauge>();
                float SamKenkiOvercapAmount = Config.SAM_ST_KenkiOvercapAmount;

                if (actionID is Kasha)
                {
                    if (CanWeave(actionID))
                    {
                        if (gauge.Kenki >= SamKenkiOvercapAmount && LevelChecked(Shinten))
                            return Shinten;
                    }
                    if (HasEffect(Buffs.MeikyoShisui))
                        return Kasha;

                    if (comboTime > 0)
                    {
                        if (lastComboMove is Hakaze && LevelChecked(Shifu))
                            return Shifu;

                        if (lastComboMove is Shifu && LevelChecked(Kasha))
                            return Kasha;
                    }
                    return Hakaze;
                }
                return actionID;
            }
        }

        internal class SAM_ST_SimpleMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SAM_ST_SimpleMode;
            internal static SAMOpenerLogic SAMOpener = new();

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                SAMGauge? gauge = GetJobGauge<SAMGauge>();
                bool oneSeal = OriginalHook(Iaijutsu) is Higanbana;
                bool twoSeal = OriginalHook(Iaijutsu) is TenkaGoken or TendoGoken;
                bool threeSeal = OriginalHook(Iaijutsu) is MidareSetsugekka or TendoSetsugekka;
                float enemyHP = GetTargetHPPercent();
                bool trueNorthReady = TargetNeedsPositionals() && ActionReady(All.TrueNorth) && !HasEffect(All.Buffs.TrueNorth) && CanDelayedWeave(actionID);
                float meikyostacks = GetBuffStacks(Buffs.MeikyoShisui);

                if (actionID is Gekko)
                {
                    if (IsEnabled(CustomComboPreset.SAM_Variant_Cure) &&
                        IsEnabled(Variant.VariantCure) &&
                        PlayerHealthPercentageHp() <= Config.SAM_VariantCure)
                        return Variant.VariantCure;

                    if (IsEnabled(CustomComboPreset.SAM_Variant_Rampart) &&
                        IsEnabled(Variant.VariantRampart) &&
                        IsOffCooldown(Variant.VariantRampart) &&
                        CanSpellWeave(actionID))
                        return Variant.VariantRampart;

                    // Opener for SAM
                    if (SAMOpener.DoFullOpener(ref actionID))
                        return actionID;

                    //Meikyo to start before combat
                    if (!HasEffect(Buffs.MeikyoShisui) && ActionReady(MeikyoShisui) && !InCombat())
                        return MeikyoShisui;

                    //oGCDs
                    if (CanWeave(actionID))
                    {
                        //Meikyo Features
                        if (ActionReady(MeikyoShisui) && WasLastWeaponskill(MidareSetsugekka))
                            return MeikyoShisui;

                        //Ikishoten Features
                        if (LevelChecked(Ikishoten))
                        {
                            //Dumps Kenki in preparation for Ikishoten
                            if (gauge.Kenki > 50 && GetCooldownRemainingTime(Ikishoten) < 10)
                                return Shinten;

                            if (gauge.Kenki <= 50 && IsOffCooldown(Ikishoten))
                                return Ikishoten;
                        }

                        //Senei Features
                        if (gauge.Kenki >= 25 && ActionReady(Senei) && HasEffect(Buffs.MeikyoShisui) &&
                            HasEffect(Buffs.Fugetsu) && HasEffect(Buffs.Fuka))
                            return Senei;

                        //Zanshin Usage
                        if (LevelChecked(Zanshin) && gauge.Kenki >= 50 &&
                            CanWeave(actionID) && HasEffect(Buffs.ZanshinReady) &&
                            ((meikyostacks is 2 && WasLastWeaponskill(Gekko)) ||
                            GetBuffRemainingTime(Buffs.ZanshinReady) <= 6))
                            return OriginalHook(Ikishoten);

                        if (LevelChecked(Shoha) && gauge.MeditationStacks is 3)
                            return Shoha;

                        if (LevelChecked(Shinten) && gauge.Kenki > 50 &&
                            !HasEffect(Buffs.ZanshinReady) &&
                            ((gauge.Kenki >= 50) ||
                            (enemyHP <= 1)))
                            return Shinten;
                    }

                    if (LevelChecked(Enpi) && !InMeleeRange() && HasBattleTarget())
                        return Enpi;

                    if (HasEffect(Buffs.Fugetsu) && HasEffect(Buffs.Fuka))
                    {
                        //Ogi Namikiri Features
                        if (!IsMoving &&
                            LevelChecked(OgiNamikiri) &&
                            (gauge.Kaeshi == Kaeshi.NAMIKIRI ||
                            (HasEffect(Buffs.OgiNamikiriReady) && !HasEffect(Buffs.ZanshinReady) &&
                            ((meikyostacks is 2 && WasLastWeaponskill(Higanbana)) ||
                            GetBuffRemainingTime(Buffs.OgiNamikiriReady) <= 6))))
                            return OriginalHook(OgiNamikiri);

                        // Iaijutsu Features
                        if (LevelChecked(Iaijutsu))
                        {
                            if (UseTsubame())
                                return OriginalHook(TsubameGaeshi);

                            if (!IsMoving &&
                                ((oneSeal && GetDebuffRemainingTime(Debuffs.Higanbana) <= 10 && enemyHP > 1 && !LevelChecked(Traits.EnhancedMeikyoShishui2)) ||
                                (oneSeal && enemyHP > 1 && LevelChecked(Traits.EnhancedMeikyoShishui2) && !HasEffect(Buffs.Tendo) && WasLastWeaponskill(Gekko)) ||
                                (twoSeal && !LevelChecked(MidareSetsugekka)) ||
                                (threeSeal && LevelChecked(MidareSetsugekka)) ||
                                (threeSeal && LevelChecked(TendoSetsugekka) && !HasEffect(Buffs.KaeshiSetsugekkaReady))))
                                return OriginalHook(Iaijutsu);
                        }
                    }

                    if (HasEffect(Buffs.MeikyoShisui))
                    {
                        if (trueNorthReady)
                            return All.TrueNorth;

                        if (LevelChecked(Gekko) && (!HasEffect(Buffs.Fugetsu) || (!gauge.Sen.HasFlag(Sen.GETSU) && HasEffect(Buffs.Fuka))))
                            return Gekko;

                        if (LevelChecked(Kasha) && (!HasEffect(Buffs.Fuka) || (!gauge.Sen.HasFlag(Sen.KA) && HasEffect(Buffs.Fugetsu))))
                            return Kasha;

                        if (LevelChecked(Yukikaze) && !gauge.Sen.HasFlag(Sen.SETSU))
                            return Yukikaze;
                    }

                    // healing

                    if (PlayerHealthPercentageHp() <= 25 && ActionReady(All.SecondWind))
                        return All.SecondWind;

                    if (PlayerHealthPercentageHp() <= 40 && ActionReady(All.Bloodbath))
                        return All.Bloodbath;


                    if (comboTime > 0)
                    {
                        if (lastComboMove is Hakaze or Gyofu && LevelChecked(Jinpu))
                        {
                            if (!gauge.Sen.HasFlag(Sen.SETSU) && LevelChecked(Yukikaze) && HasEffect(Buffs.Fugetsu) && HasEffect(Buffs.Fuka))
                                return Yukikaze;

                            if ((!LevelChecked(Kasha) && ((GetBuffRemainingTime(Buffs.Fugetsu) < GetBuffRemainingTime(Buffs.Fuka)) || !HasEffect(Buffs.Fugetsu))) ||
                               (LevelChecked(Kasha) && (!HasEffect(Buffs.Fugetsu) || (HasEffect(Buffs.Fuka) && !gauge.Sen.HasFlag(Sen.GETSU)) || (threeSeal && (GetBuffRemainingTime(Buffs.Fugetsu) < GetBuffRemainingTime(Buffs.Fuka))))))
                                return Jinpu;

                            if (LevelChecked(Shifu) && ((!LevelChecked(Kasha) && ((GetBuffRemainingTime(Buffs.Fuka) < GetBuffRemainingTime(Buffs.Fugetsu)) || !HasEffect(Buffs.Fuka))) ||
                                (LevelChecked(Kasha) && (!HasEffect(Buffs.Fuka) || (HasEffect(Buffs.Fugetsu) && !gauge.Sen.HasFlag(Sen.KA)) || (threeSeal && (GetBuffRemainingTime(Buffs.Fuka) < GetBuffRemainingTime(Buffs.Fugetsu)))))))
                                return Shifu;
                        }

                        if (lastComboMove is Jinpu && LevelChecked(Gekko))
                            return Gekko;

                        if (lastComboMove is Shifu && LevelChecked(Kasha))
                            return Kasha;
                    }
                    return OriginalHook(Hakaze);
                }
                return actionID;
            }

            private static bool UseTsubame()
            {
                int MeikyoUsed = ActionWatching.CombatActions.Count(x => x == MeikyoShisui);

                if (LevelChecked(TsubameGaeshi))
                {
                    //Tendo
                    if (WasLastWeaponskill(TendoSetsugekka) && HasEffect(Buffs.TendoKaeshiSetsugekkaReady))
                        return true;

                    if (HasEffect(Buffs.KaeshiSetsugekkaReady))
                    {
                        //1 and 2 min
                        if ((MeikyoUsed is 0 or 1 or 2 or 3 or 4 or 9 or 10) &&
                            WasLastWeaponskill(MidareSetsugekka))
                            return true;

                        // 3 and 4 min
                        if ((MeikyoUsed is 5 or 6 or 11 or 12) &&
                            GetBuffStacks(Buffs.MeikyoShisui) is 2)
                            return true;

                        // 5 and 6 min
                        if ((MeikyoUsed is 7 or 8 or 13 or 14) &&
                            GetBuffStacks(Buffs.MeikyoShisui) is 1)
                            return true;
                    }
                }
                return false;
            }
        }

        internal class SAM_ST_AdvancedMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SAM_ST_AdvancedMode;
            internal static SAMOpenerLogic SAMOpener = new();

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                SAMGauge? gauge = GetJobGauge<SAMGauge>();
                bool oneSeal = OriginalHook(Iaijutsu) is Higanbana;
                bool twoSeal = OriginalHook(Iaijutsu) is TenkaGoken or TendoGoken;
                bool threeSeal = OriginalHook(Iaijutsu) is MidareSetsugekka or TendoSetsugekka;
                float enemyHP = GetTargetHPPercent();
                bool trueNorthReady = TargetNeedsPositionals() && ActionReady(All.TrueNorth) && !HasEffect(All.Buffs.TrueNorth) && CanDelayedWeave(actionID);
                float kenkiOvercap = Config.SAM_ST_KenkiOvercapAmount;
                float shintenTreshhold = Config.SAM_ST_ExecuteThreshold;
                float HiganbanaThreshold = Config.SAM_ST_Higanbana_Threshold;
                float meikyostacks = GetBuffStacks(Buffs.MeikyoShisui);

                if (actionID is Gekko)
                {
                    if (IsEnabled(CustomComboPreset.SAM_Variant_Cure) &&
                        IsEnabled(Variant.VariantCure) &&
                        PlayerHealthPercentageHp() <= Config.SAM_VariantCure)
                        return Variant.VariantCure;

                    if (IsEnabled(CustomComboPreset.SAM_Variant_Rampart) &&
                        IsEnabled(Variant.VariantRampart) &&
                        IsOffCooldown(Variant.VariantRampart) &&
                        CanSpellWeave(actionID))
                        return Variant.VariantRampart;

                    // Opener for SAM
                    if (IsEnabled(CustomComboPreset.SAM_ST_Opener))
                    {
                        if (SAMOpener.DoFullOpener(ref actionID))
                            return actionID;
                    }

                    //Meikyo to start before combat
                    if (IsEnabled(CustomComboPreset.SAM_ST_CDs) &&
                        IsEnabled(CustomComboPreset.SAM_ST_CDs_MeikyoShisui) &&
                        !HasEffect(Buffs.MeikyoShisui) && ActionReady(MeikyoShisui) && !InCombat())
                        return MeikyoShisui;

                    //oGCDs
                    if (CanWeave(actionID))
                    {
                        if (IsEnabled(CustomComboPreset.SAM_ST_CDs))
                        {
                            //Meikyo Features
                            if (IsEnabled(CustomComboPreset.SAM_ST_CDs_MeikyoShisui) &&
                               !HasEffect(Buffs.MeikyoShisui) &&
                               ActionReady(MeikyoShisui) && JustUsed(MidareSetsugekka, 1))
                                return MeikyoShisui;

                            //Ikishoten Features
                            if (IsEnabled(CustomComboPreset.SAM_ST_CDs_Ikishoten) && LevelChecked(Ikishoten))
                            {
                                //Dumps Kenki in preparation for Ikishoten
                                if (gauge.Kenki > 50 && GetCooldownRemainingTime(Ikishoten) < 10)
                                    return Shinten;

                                if (gauge.Kenki <= 50 && IsOffCooldown(Ikishoten))
                                    return Ikishoten;
                            }

                            //Senei Features
                            if (IsEnabled(CustomComboPreset.SAM_ST_CDs_Senei) &&
                                gauge.Kenki >= 25 && ActionReady(Senei) && HasEffect(Buffs.MeikyoShisui) &&
                                HasEffect(Buffs.Fugetsu) && HasEffect(Buffs.Fuka))
                                return Senei;

                            //Zanshin Usage
                            if (IsEnabled(CustomComboPreset.SAM_ST_CDs_Zanshin) &&
                                LevelChecked(Zanshin) && gauge.Kenki >= 50 &&
                                CanWeave(actionID) && HasEffect(Buffs.ZanshinReady) &&
                                ((meikyostacks is 2 && WasLastWeaponskill(Gekko)) ||
                                GetBuffRemainingTime(Buffs.ZanshinReady) <= 6))
                                return OriginalHook(Ikishoten);

                            if (IsEnabled(CustomComboPreset.SAM_ST_CDs_Shoha) &&
                                LevelChecked(Shoha) && gauge.MeditationStacks is 3)
                                return Shoha;
                        }

                        if (IsEnabled(CustomComboPreset.SAM_ST_Shinten) &&
                            LevelChecked(Shinten) && gauge.Kenki > 50 &&
                            !HasEffect(Buffs.ZanshinReady) &&
                            ((gauge.Kenki >= kenkiOvercap) ||
                            (enemyHP <= shintenTreshhold)))
                            return Shinten;
                    }

                    if (IsEnabled(CustomComboPreset.SAM_ST_RangedUptime) &&
                        LevelChecked(Enpi) && !InMeleeRange() && HasBattleTarget())
                        return Enpi;

                    if (IsEnabled(CustomComboPreset.SAM_ST_CDs) &&
                        HasEffect(Buffs.Fugetsu) && HasEffect(Buffs.Fuka))
                    {
                        //Ogi Namikiri Features
                        if (IsEnabled(CustomComboPreset.SAM_ST_CDs_OgiNamikiri) &&
                            (!IsEnabled(CustomComboPreset.SAM_ST_CDs_OgiNamikiri_Movement) ||
                            (IsEnabled(CustomComboPreset.SAM_ST_CDs_OgiNamikiri_Movement) && !IsMoving)) &&
                            LevelChecked(OgiNamikiri) &&
                            (gauge.Kaeshi == Kaeshi.NAMIKIRI ||
                            (HasEffect(Buffs.OgiNamikiriReady) && !HasEffect(Buffs.ZanshinReady) &&
                            ((meikyostacks is 2 && WasLastWeaponskill(Higanbana)) ||
                            GetBuffRemainingTime(Buffs.OgiNamikiriReady) <= 6))))
                            return OriginalHook(OgiNamikiri);

                        // Iaijutsu Features
                        if (IsEnabled(CustomComboPreset.SAM_ST_CDs_Iaijutsu) && LevelChecked(Iaijutsu))
                        {
                            if (UseTsubame())
                                return OriginalHook(TsubameGaeshi);

                            if ((!IsEnabled(CustomComboPreset.SAM_ST_CDs_Iaijutsu_Movement) ||
                                (IsEnabled(CustomComboPreset.SAM_ST_CDs_Iaijutsu_Movement) && !IsMoving)) &&
                                ((oneSeal && enemyHP > HiganbanaThreshold &&
                                ((!TraitLevelChecked(Traits.EnhancedMeikyoShishui2) && GetDebuffRemainingTime(Debuffs.Higanbana) <= 10) ||
                                (TraitLevelChecked(Traits.EnhancedMeikyoShishui2) && !HasEffect(Buffs.Tendo) && WasLastWeaponskill(Gekko)))) ||
                                (twoSeal && !LevelChecked(MidareSetsugekka)) ||
                                (threeSeal &&
                                (LevelChecked(MidareSetsugekka) ||
                                (LevelChecked(TendoSetsugekka) && !HasEffect(Buffs.KaeshiSetsugekkaReady))))))
                                return OriginalHook(Iaijutsu);
                        }
                    }

                    if (HasEffect(Buffs.MeikyoShisui))
                    {
                        if (IsEnabled(CustomComboPreset.SAM_ST_TrueNorth) &&
                            trueNorthReady)
                            return All.TrueNorth;

                        if (LevelChecked(Gekko) && (!HasEffect(Buffs.Fugetsu) || (!gauge.Sen.HasFlag(Sen.GETSU) && HasEffect(Buffs.Fuka))))
                            return Gekko;

                        if (IsEnabled(CustomComboPreset.SAM_ST_Kasha) &&
                            LevelChecked(Kasha) && (!HasEffect(Buffs.Fuka) || (!gauge.Sen.HasFlag(Sen.KA) && HasEffect(Buffs.Fugetsu))))
                            return Kasha;

                        if (IsEnabled(CustomComboPreset.SAM_ST_Yukikaze) &&
                            LevelChecked(Yukikaze) && !gauge.Sen.HasFlag(Sen.SETSU))
                            return Yukikaze;
                    }

                    // healing
                    if (IsEnabled(CustomComboPreset.SAM_ST_ComboHeals))
                    {
                        if (PlayerHealthPercentageHp() <= Config.SAM_STSecondWindThreshold && ActionReady(All.SecondWind))
                            return All.SecondWind;

                        if (PlayerHealthPercentageHp() <= Config.SAM_STBloodbathThreshold && ActionReady(All.Bloodbath))
                            return All.Bloodbath;
                    }

                    if (comboTime > 0)
                    {
                        if (lastComboMove is Hakaze or Gyofu && LevelChecked(Jinpu))
                        {
                            if (IsEnabled(CustomComboPreset.SAM_ST_Yukikaze) &&
                                !gauge.Sen.HasFlag(Sen.SETSU) && LevelChecked(Yukikaze) && HasEffect(Buffs.Fugetsu) && HasEffect(Buffs.Fuka))
                                return Yukikaze;

                            if ((!LevelChecked(Kasha) && ((GetBuffRemainingTime(Buffs.Fugetsu) < GetBuffRemainingTime(Buffs.Fuka)) || !HasEffect(Buffs.Fugetsu))) ||
                               (LevelChecked(Kasha) && (!HasEffect(Buffs.Fugetsu) || (HasEffect(Buffs.Fuka) && !gauge.Sen.HasFlag(Sen.GETSU)) || (threeSeal && (GetBuffRemainingTime(Buffs.Fugetsu) < GetBuffRemainingTime(Buffs.Fuka))))))
                                return Jinpu;

                            if (IsEnabled(CustomComboPreset.SAM_ST_Kasha) &&
                                LevelChecked(Shifu) && ((!LevelChecked(Kasha) && ((GetBuffRemainingTime(Buffs.Fuka) < GetBuffRemainingTime(Buffs.Fugetsu)) || !HasEffect(Buffs.Fuka))) ||
                                (LevelChecked(Kasha) && (!HasEffect(Buffs.Fuka) || (HasEffect(Buffs.Fugetsu) && !gauge.Sen.HasFlag(Sen.KA)) || (threeSeal && (GetBuffRemainingTime(Buffs.Fuka) < GetBuffRemainingTime(Buffs.Fugetsu)))))))
                                return Shifu;
                        }

                        if (lastComboMove is Jinpu && LevelChecked(Gekko))
                            return Gekko;

                        if (IsEnabled(CustomComboPreset.SAM_ST_Kasha) &&
                            lastComboMove is Shifu && LevelChecked(Kasha))
                            return Kasha;
                    }
                    return OriginalHook(Hakaze);
                }
                return actionID;
            }

            private static bool UseTsubame()
            {
                int MeikyoUsed = ActionWatching.CombatActions.Count(x => x == MeikyoShisui);

                if (IsEnabled(CustomComboPreset.SAM_ST_CDs_Iaijutsu) &&
                    LevelChecked(TsubameGaeshi))
                {
                    //Tendo
                    if (WasLastWeaponskill(TendoSetsugekka) && HasEffect(Buffs.TendoKaeshiSetsugekkaReady))
                        return true;

                    if (HasEffect(Buffs.KaeshiSetsugekkaReady))
                    {
                        //1 and 2 min
                        if ((MeikyoUsed is 0 or 1 or 2 or 3 or 4 or 9 or 10) &&
                            WasLastWeaponskill(MidareSetsugekka))
                            return true;

                        // 3 and 4 min
                        if ((MeikyoUsed is 5 or 6 or 11 or 12) &&
                            GetBuffStacks(Buffs.MeikyoShisui) is 2)
                            return true;

                        // 5 and 6 min
                        if ((MeikyoUsed is 7 or 8 or 13 or 14) &&
                            GetBuffStacks(Buffs.MeikyoShisui) is 1)
                            return true;
                    }
                }
                return false;
            }
        }

        internal class SAM_AoE_OkaCombo : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SAM_AoE_OkaCombo;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                SAMGauge? gauge = GetJobGauge<SAMGauge>();

                if (actionID is Oka)
                {
                    if (IsNotEnabled(CustomComboPreset.SAM_AoE_OkaCombo_TwoTarget) &&
                        gauge.Kenki >= Config.SAM_AoE_KenkiOvercapAmount && LevelChecked(Kyuten) && CanWeave(actionID))
                        return Kyuten;

                    if (IsNotEnabled(CustomComboPreset.SAM_AoE_OkaCombo_TwoTarget) &&
                        HasEffect(Buffs.MeikyoShisui))
                        return Oka;

                    //Two Target Rotation
                    if (IsEnabled(CustomComboPreset.SAM_AoE_OkaCombo_TwoTarget))
                    {
                        if (CanWeave(actionID))
                        {
                            if (!HasEffect(Buffs.MeikyoShisui) && ActionReady(MeikyoShisui))
                                return MeikyoShisui;

                            if (ActionReady(Senei) && gauge.Kenki >= 25)
                                return Senei;

                            if (LevelChecked(Shinten) && gauge.Kenki >= 25)
                                return Shinten;

                            if (LevelChecked(Shoha) && gauge.MeditationStacks is 3)
                                return Shoha;
                        }

                        if (HasEffect(Buffs.MeikyoShisui))
                        {
                            if (!gauge.Sen.HasFlag(Sen.SETSU) && Yukikaze.LevelChecked())
                                return Yukikaze;

                            if (!gauge.Sen.HasFlag(Sen.GETSU) && Gekko.LevelChecked())
                                return Gekko;

                            if (!gauge.Sen.HasFlag(Sen.KA) && Kasha.LevelChecked())
                                return Kasha;
                        }

                        if (ActionReady(TsubameGaeshi) && gauge.Kaeshi is Kaeshi.SETSUGEKKA)
                            return OriginalHook(TsubameGaeshi);

                        if (LevelChecked(MidareSetsugekka) && OriginalHook(Iaijutsu) is MidareSetsugekka)
                            return OriginalHook(Iaijutsu);

                        if (comboTime > 1f)
                        {
                            if (lastComboMove is Hakaze && LevelChecked(Yukikaze))
                                return Yukikaze;

                            if (lastComboMove is Fuko or Fuga && !gauge.Sen.HasFlag(Sen.GETSU) && LevelChecked(Mangetsu))
                                return Mangetsu;
                        }

                        if (!gauge.Sen.HasFlag(Sen.SETSU))
                            return Hakaze;
                    }

                    if (comboTime > 0 && LevelChecked(Oka))
                    {
                        if (lastComboMove is Fuko || lastComboMove is Fuga)
                            return Oka;
                    }
                    return OriginalHook(Fuko);
                }
                return actionID;
            }
        }

        internal class SAM_AoE_SimpleMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SAM_AoE_SimpleMode;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                SAMGauge? gauge = GetJobGauge<SAMGauge>();

                if (actionID is Fuga or Fuko)
                {
                    if (IsEnabled(CustomComboPreset.SAM_Variant_Cure) &&
                        IsEnabled(Variant.VariantCure) &&
                        PlayerHealthPercentageHp() <= Config.SAM_VariantCure)
                        return Variant.VariantCure;

                    if (IsEnabled(CustomComboPreset.SAM_Variant_Rampart) &&
                        IsEnabled(Variant.VariantRampart) &&
                        IsOffCooldown(Variant.VariantRampart) &&
                        CanWeave(actionID))
                        return Variant.VariantRampart;

                    //oGCD Features
                    if (CanWeave(actionID))
                    {
                        if (OriginalHook(Iaijutsu) is MidareSetsugekka && LevelChecked(Hagakure))
                            return Hagakure;

                        if (ActionReady(Guren) && gauge.Kenki >= 25)
                            return Guren;

                        if (LevelChecked(Ikishoten))
                        {
                            //Dumps Kenki in preparation for Ikishoten
                            if (gauge.Kenki > 50 && GetCooldownRemainingTime(Ikishoten) < 10)
                                return Kyuten;

                            if (gauge.Kenki <= 50 && IsOffCooldown(Ikishoten))
                                return Ikishoten;
                        }

                        if (Kyuten.LevelChecked() && gauge.Kenki >= 50 &&
                            IsOnCooldown(Guren) && LevelChecked(Guren))
                            return Kyuten;

                        if (ActionReady(Shoha) && gauge.MeditationStacks is 3)
                            return Shoha;

                        if (ActionReady(MeikyoShisui) && !HasEffect(Buffs.MeikyoShisui))
                            return MeikyoShisui;
                    }

                    if (LevelChecked(Zanshin) && HasEffect(Buffs.ZanshinReady) && gauge.Kenki >= 50)
                        return OriginalHook(Ikishoten);

                    if (LevelChecked(OgiNamikiri) && ((!IsMoving && HasEffect(Buffs.OgiNamikiriReady)) || gauge.Kaeshi is Kaeshi.NAMIKIRI))
                        return OriginalHook(OgiNamikiri);

                    if (LevelChecked(TenkaGoken))
                    {
                        if (!IsMoving && (OriginalHook(Iaijutsu) is TenkaGoken or TendoGoken))
                            return OriginalHook(Iaijutsu);

                        if (LevelChecked(TsubameGaeshi) && (HasEffect(Buffs.KaeshiGokenReady) || HasEffect(Buffs.TendoKaeshiGokenReady)))
                            return OriginalHook(TsubameGaeshi);
                    }

                    if (HasEffect(Buffs.MeikyoShisui))
                    {
                        if ((!gauge.Sen.HasFlag(Sen.GETSU) && HasEffect(Buffs.Fuka)) || !HasEffect(Buffs.Fugetsu))
                            return Mangetsu;

                        if ((!gauge.Sen.HasFlag(Sen.KA) && HasEffect(Buffs.Fugetsu)) || !HasEffect(Buffs.Fuka))
                            return Oka;
                    }

                    // healing - please move if not appropriate this high priority
                    if (PlayerHealthPercentageHp() <= 25 && ActionReady(All.SecondWind))
                        return All.SecondWind;

                    if (PlayerHealthPercentageHp() <= 40 && ActionReady(All.Bloodbath))
                        return All.Bloodbath;

                    if (comboTime > 0)
                    {
                        if (lastComboMove is Fuko or Fuga && LevelChecked(Mangetsu))
                        {
                            if (!gauge.Sen.HasFlag(Sen.GETSU) || GetBuffRemainingTime(Buffs.Fugetsu) < GetBuffRemainingTime(Buffs.Fuka) || !HasEffect(Buffs.Fugetsu))
                                return Mangetsu;

                            if (LevelChecked(Oka) &&
                                (!gauge.Sen.HasFlag(Sen.KA) || GetBuffRemainingTime(Buffs.Fuka) < GetBuffRemainingTime(Buffs.Fugetsu) || !HasEffect(Buffs.Fuka)))
                                return Oka;
                        }
                    }
                    return OriginalHook(Fuko);
                }
                return actionID;
            }
        }

        internal class SAM_AoE_AdvancedMode : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SAM_AoE_AdvancedMode;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                SAMGauge? gauge = GetJobGauge<SAMGauge>();
                float kenkiOvercap = Config.SAM_AoE_KenkiOvercapAmount;

                if (actionID is Fuga or Fuko)
                {
                    if (IsEnabled(CustomComboPreset.SAM_Variant_Cure) &&
                        IsEnabled(Variant.VariantCure) &&
                        PlayerHealthPercentageHp() <= Config.SAM_VariantCure)
                        return Variant.VariantCure;

                    if (IsEnabled(CustomComboPreset.SAM_Variant_Rampart) &&
                        IsEnabled(Variant.VariantRampart) &&
                        IsOffCooldown(Variant.VariantRampart) &&
                        CanWeave(actionID))
                        return Variant.VariantRampart;

                    //oGCD Features
                    if (CanWeave(actionID))
                    {
                        if (IsEnabled(CustomComboPreset.SAM_AoE_Hagakure) &&
                            OriginalHook(Iaijutsu) is MidareSetsugekka && LevelChecked(Hagakure))
                            return Hagakure;

                        if (IsEnabled(CustomComboPreset.SAM_AoE_Guren) &&
                            ActionReady(Guren) && gauge.Kenki >= 25)
                            return Guren;

                        if (IsEnabled(CustomComboPreset.SAM_AOE_CDs_Ikishoten) &&
                            LevelChecked(Ikishoten))
                        {
                            //Dumps Kenki in preparation for Ikishoten
                            if (gauge.Kenki > 50 && GetCooldownRemainingTime(Ikishoten) < 10)
                                return Kyuten;

                            if (gauge.Kenki <= 50 && IsOffCooldown(Ikishoten))
                                return Ikishoten;
                        }

                        if (IsEnabled(CustomComboPreset.SAM_AoE_Kyuten) &&
                            Kyuten.LevelChecked() && gauge.Kenki >= 50 &&
                            ((IsOnCooldown(Guren) && LevelChecked(Guren)) ||
                            (gauge.Kenki >= kenkiOvercap)))
                            return Kyuten;

                        if (IsEnabled(CustomComboPreset.SAM_AoE_Shoha) &&
                            ActionReady(Shoha) && gauge.MeditationStacks is 3)
                            return Shoha;

                        if (IsEnabled(CustomComboPreset.SAM_AoE_MeikyoShisui) &&
                            ActionReady(MeikyoShisui) && !HasEffect(Buffs.MeikyoShisui))
                            return MeikyoShisui;
                    }

                    if (IsEnabled(CustomComboPreset.SAM_AoE_Zanshin) &&
                        LevelChecked(Zanshin) && HasEffect(Buffs.ZanshinReady) && gauge.Kenki >= 50)
                        return OriginalHook(Ikishoten);

                    if (IsEnabled(CustomComboPreset.SAM_AoE_OgiNamikiri) &&
                        LevelChecked(OgiNamikiri) && ((!IsMoving && HasEffect(Buffs.OgiNamikiriReady)) || gauge.Kaeshi is Kaeshi.NAMIKIRI))
                        return OriginalHook(OgiNamikiri);

                    if (IsEnabled(CustomComboPreset.SAM_AoE_TenkaGoken) && LevelChecked(TenkaGoken))
                    {
                        if (!IsMoving && (OriginalHook(Iaijutsu) is TenkaGoken))
                            return OriginalHook(Iaijutsu);

                        if (LevelChecked(TsubameGaeshi) && (HasEffect(Buffs.KaeshiGokenReady) || HasEffect(Buffs.TendoKaeshiGokenReady)))
                            return OriginalHook(TsubameGaeshi);
                    }

                    if (HasEffect(Buffs.MeikyoShisui))
                    {
                        if ((!gauge.Sen.HasFlag(Sen.GETSU) && HasEffect(Buffs.Fuka)) || !HasEffect(Buffs.Fugetsu))
                            return Mangetsu;

                        if (IsEnabled(CustomComboPreset.SAM_AoE_Oka) &&
                            ((!gauge.Sen.HasFlag(Sen.KA) && HasEffect(Buffs.Fugetsu)) || !HasEffect(Buffs.Fuka)))
                            return Oka;
                    }

                    if (IsEnabled(CustomComboPreset.SAM_AoE_ComboHeals))
                    {
                        if (PlayerHealthPercentageHp() <= Config.SAM_AoESecondWindThreshold && ActionReady(All.SecondWind))
                            return All.SecondWind;

                        if (PlayerHealthPercentageHp() <= Config.SAM_AoEBloodbathThreshold && ActionReady(All.Bloodbath))
                            return All.Bloodbath;
                    }

                    if (comboTime > 0)
                    {
                        if (lastComboMove is Fuko or Fuga && LevelChecked(Mangetsu))
                        {
                            if (IsNotEnabled(CustomComboPreset.SAM_AoE_Oka) ||
                                !gauge.Sen.HasFlag(Sen.GETSU) || GetBuffRemainingTime(Buffs.Fugetsu) < GetBuffRemainingTime(Buffs.Fuka) || !HasEffect(Buffs.Fugetsu))
                                return Mangetsu;

                            if (IsEnabled(CustomComboPreset.SAM_AoE_Oka) &&
                                LevelChecked(Oka) &&
                                (!gauge.Sen.HasFlag(Sen.KA) || GetBuffRemainingTime(Buffs.Fuka) < GetBuffRemainingTime(Buffs.Fugetsu) || !HasEffect(Buffs.Fuka)))
                                return Oka;
                        }
                    }
                    return OriginalHook(Fuko);
                }
                return actionID;
            }
        }

        internal class SAM_JinpuShifu : CustomCombo
        {
            protected internal override CustomComboPreset Preset => CustomComboPreset.SAM_JinpuShifu;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                SAMGauge? gauge = GetJobGauge<SAMGauge>();

                if (actionID is MeikyoShisui)
                {
                    if (HasEffect(Buffs.MeikyoShisui))
                    {
                        if (!HasEffect(Buffs.Fugetsu) ||
                            !gauge.Sen.HasFlag(Sen.GETSU))
                            return Gekko;

                        if (!HasEffect(Buffs.Fuka) ||
                            !gauge.Sen.HasFlag(Sen.KA))
                            return Kasha;

                        if (!gauge.Sen.HasFlag(Sen.SETSU))
                            return Yukikaze;
                    }
                    return MeikyoShisui;
                }
                return actionID;
            }
        }

        internal class SAM_Iaijutsu : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SAM_Iaijutsu;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                SAMGauge? gauge = GetJobGauge<SAMGauge>();

                if (actionID is Iaijutsu)
                {
                    if (IsEnabled(CustomComboPreset.SAM_Iaijutsu_Shoha) &&
                        ActionReady(Shoha) && gauge.MeditationStacks is 3 && CanWeave(actionID))
                        return Shoha;

                    if (IsEnabled(CustomComboPreset.SAM_Iaijutsu_OgiNamikiri) &&
                        ActionReady(OgiNamikiri) && (gauge.Kaeshi is Kaeshi.NAMIKIRI || HasEffect(Buffs.OgiNamikiriReady)))
                        return OriginalHook(OgiNamikiri);

                    if (IsEnabled(CustomComboPreset.SAM_Iaijutsu_TsubameGaeshi) &&
                        ActionReady(TsubameGaeshi) && (gauge.Kaeshi != Kaeshi.NONE || WasLastWeaponskill(TendoSetsugekka)))
                        return OriginalHook(TsubameGaeshi);
                }
                return actionID;
            }
        }

        internal class SAM_Shinten_Shoha : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SAM_Shinten_Shoha;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                SAMGauge? gauge = GetJobGauge<SAMGauge>();

                if (actionID is Shinten)
                {
                    if (IsEnabled(CustomComboPreset.SAM_Shinten_Shoha_Senei) &&
                        ActionReady(Senei))
                        return Senei;

                    if (gauge.MeditationStacks is 3 && ActionReady(Shoha))
                        return Shoha;
                }
                return actionID;
            }
        }

        internal class SAM_Kyuten_Shoha_Guren : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SAM_Kyuten_Shoha;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                SAMGauge? gauge = GetJobGauge<SAMGauge>();

                if (actionID is Kyuten)
                {
                    if (IsEnabled(CustomComboPreset.SAM_Kyuten_Shoha_Guren) &&
                        ActionReady(Guren))
                        return Guren;

                    if (gauge.MeditationStacks is 3 && ActionReady(Shoha))
                        return Shoha;
                }

                return actionID;
            }
        }

        internal class SAM_Ikishoten_OgiNamikiri : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SAM_Ikishoten_OgiNamikiri;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                SAMGauge? gauge = GetJobGauge<SAMGauge>();

                if (actionID is Ikishoten)
                {
                    if ((LevelChecked(OgiNamikiri) && HasEffect(Buffs.OgiNamikiriReady)) || gauge.Kaeshi == Kaeshi.NAMIKIRI)
                        return OriginalHook(OgiNamikiri);
                }
                return actionID;
            }
        }

        internal class SAM_GyotenYaten : CustomCombo
        {
            protected internal override CustomComboPreset Preset { get; } = CustomComboPreset.SAM_GyotenYaten;

            protected override uint Invoke(uint actionID, uint lastComboMove, float comboTime, byte level)
            {
                if (actionID is Gyoten)
                {
                    SAMGauge? gauge = GetJobGauge<SAMGauge>();

                    if (gauge.Kenki >= 10)
                    {
                        if (InMeleeRange())
                            return Yaten;

                        if (!InMeleeRange())
                            return Gyoten;
                    }
                }

                return actionID;
            }
        }
    }
}
