﻿using Dalamud.Game.ClientState.JobGauge.Types;
using ECommons.DalamudServices;
using System;
using System.Net.Quic;
using XIVSlothCombo.Combos.JobHelpers.Enums;
using XIVSlothCombo.Combos.PvE;
using XIVSlothCombo.CustomComboNS.Functions;
using XIVSlothCombo.Data;

namespace XIVSlothCombo.Combos.JobHelpers
{
    internal class RDMHelper : RDM
    {
        static bool HasEffect(ushort id) => CustomComboFunctions.HasEffect(id);
        static float GetBuffRemainingTime(ushort effectid) => CustomComboFunctions.GetBuffRemainingTime(effectid);
        static bool LevelChecked(uint id) => CustomComboFunctions.LevelChecked(id);
        static float GetActionCastTime(uint actionID) => CustomComboFunctions.GetActionCastTime(actionID);
        static uint GetRemainingCharges(uint actionID) => CustomComboFunctions.GetRemainingCharges(actionID);
        static float GetCooldownRemainingTime(uint actionID) => CustomComboFunctions.GetCooldownRemainingTime(actionID);
        static bool ActionReady(uint id) => CustomComboFunctions.ActionReady(id);
        static bool CanSpellWeave(uint id) => CustomComboFunctions.CanSpellWeave(id);
        static bool HasCharges(uint id) => CustomComboFunctions.HasCharges(id);
        static bool TraitLevelChecked(uint id) => CustomComboFunctions.TraitLevelChecked(id);
        static byte GetBuffStacks(ushort id) => CustomComboFunctions.GetBuffStacks(id);

        internal class RDMMana
        {
            private static RDMGauge Gauge => CustomComboFunctions.GetJobGauge<RDMGauge>();
            internal static int ManaStacks => Gauge.ManaStacks;
            internal static int Black => AdjustMana(Gauge.BlackMana);
            internal static int White => AdjustMana(Gauge.WhiteMana);
            internal static int Min => AdjustMana(Math.Min(Gauge.BlackMana, Gauge.WhiteMana));
            internal static int Max => AdjustMana(Math.Max(Gauge.BlackMana, Gauge.WhiteMana));
            private static int AdjustMana(byte mana)
            {
                if (LevelChecked(Manafication))
                {
                    byte magickedSword = GetBuffStacks(Buffs.MagickedSwordPlay);
                    byte magickedSwordMana = magickedSword switch
                    {
                        3 => 50,
                        2 => 30,
                        1 => 15,
                        _ => 0
                    };
                    return mana + magickedSwordMana;
                }
                else return mana;
            }

            public static (bool useFire, bool useStone, bool useThunder, bool useAero, bool useThunder2, bool useAero2) CheckBalance()
            {
                //SYSTEM_MANA_BALANCING_MACHINE
                //Machine to decide which ver spell should be used.
                //Rules:
                //1.Avoid perfect balancing [NOT DONE]
                //   - Jolt adds 2/2 mana
                //   - Scatter/Impact adds 3/3 mana
                //   - Verstone/Verfire add 5 mana
                //   - Veraero/Verthunder add 6 mana
                //   - Veraero2/Verthunder2 add 7 mana
                //   - Verholy/Verflare add 11 mana
                //   - Scorch adds 4/4 mana
                //   - Resolution adds 4/4 mana
                //2.Stay within difference limit [DONE]
                //3.Strive to achieve correct mana for double melee combo burst [DONE]
                int blackmana = Black;
                int whitemana = White;
                //Reset outputs
                bool useFire = false;
                bool useStone = false;
                bool useThunder = false;
                bool useAero = false;
                bool useThunder2 = false;
                bool useAero2 = false;

                //ST
                if (LevelChecked(Verthunder)
                    && (HasEffect(Buffs.Dualcast) || HasEffect(All.Buffs.Swiftcast) || HasEffect(Buffs.Acceleration)))
                {
                    if (blackmana <= whitemana || HasEffect(Buffs.VerstoneReady)) useThunder = true;
                    if (whitemana <= blackmana || HasEffect(Buffs.VerfireReady)) useAero = true;
                    if (!LevelChecked(Veraero)) useThunder = true;
                }
                if (!HasEffect(Buffs.Dualcast)
                    && !HasEffect(All.Buffs.Swiftcast)
                    && !HasEffect(Buffs.Acceleration))
                {
                    //Checking the time remaining instead of just the effect, to stop last second bad casts
                    bool VerFireReady = GetBuffRemainingTime(Buffs.VerfireReady) >= GetActionCastTime(Verfire);
                    bool VerStoneReady = GetBuffRemainingTime(Buffs.VerstoneReady) >= GetActionCastTime(Verstone);

                    //Prioritize mana balance
                    if (blackmana <= whitemana && VerFireReady) useFire = true;
                    if (whitemana <= blackmana && VerStoneReady) useStone = true;
                    //Else use the action if we can
                    if (!useFire && !useStone && VerFireReady) useFire = true;
                    if (!useFire && !useStone && VerStoneReady) useStone = true;
                }

                //AoE
                if (LevelChecked(Verthunder2)
                    && !HasEffect(Buffs.Dualcast)
                    && !HasEffect(All.Buffs.Swiftcast)
                    && !HasEffect(Buffs.Acceleration))
                {
                    if (blackmana <= whitemana || !LevelChecked(Veraero2)) useThunder2 = true;
                    else useAero2 = true;
                }
                //END_SYSTEM_MANA_BALANCING_MACHINE

                return (useFire, useStone, useThunder, useAero, useThunder2, useAero2);
            }
        }

        internal class MeleeFinisher
        {
            internal static bool CanUse(in uint lastComboMove, out uint actionID)
            {
                int blackmana = RDMMana.Black;
                int whitemana = RDMMana.White;

                if (RDMMana.ManaStacks >= 3)
                {
                    if (blackmana >= whitemana && LevelChecked(Verholy))
                    {
                        if ((!HasEffect(Buffs.Embolden) || GetBuffRemainingTime(Buffs.Embolden) < 10)
                            && !HasEffect(Buffs.VerfireReady)
                            && HasEffect(Buffs.VerstoneReady) && GetBuffRemainingTime(Buffs.VerstoneReady) >= 10
                            && (blackmana - whitemana <= 18))
                        {
                            actionID = Verflare;
                            return true;
                        }
                        actionID = Verholy;
                        return true;
                    }
                    else if (LevelChecked(Verflare))
                    {
                        if ((!HasEffect(Buffs.Embolden) || GetBuffRemainingTime(Buffs.Embolden) < 10)
                            && HasEffect(Buffs.VerfireReady) && GetBuffRemainingTime(Buffs.VerfireReady) >= 10
                            && !HasEffect(Buffs.VerstoneReady)
                            && LevelChecked(Verholy)
                            && (whitemana - blackmana <= 18))
                        {
                            actionID = Verholy;
                            return true;
                        }
                        actionID = Verflare;
                        return true;
                    }
                }
                if ((lastComboMove is Verflare or Verholy)
                    && LevelChecked(Scorch))
                {
                    actionID = Scorch;
                    return true;
                }

                if (lastComboMove is Scorch
                    && LevelChecked(Resolution))
                {
                    actionID = Resolution;
                    return true;
                }

                actionID = 0;
                return false;
            }
        }

        internal class OGCDHelper
        {
            internal static bool CanUse(in uint actionID, in bool SingleTarget, out uint newActionID)
            {
                var distance = CustomComboFunctions.GetTargetDistance();

                uint placeOGCD = 0;

                bool fleche = SingleTarget ? Config.RDM_ST_oGCD_Fleche : Config.RDM_AoE_oGCD_Fleche;
                bool contra = SingleTarget ? Config.RDM_ST_oGCD_ContraSixte : Config.RDM_AoE_oGCD_ContraSixte;
                bool engagement = SingleTarget ? Config.RDM_ST_oGCD_Engagement : Config.RDM_AoE_oGCD_Engagement;
                bool vice = SingleTarget ? Config.RDM_ST_oGCD_ViceOfThorns : Config.RDM_AoE_oGCD_ViceOfThorns;
                bool prefulg = SingleTarget ? Config.RDM_ST_oGCD_Prefulgence : Config.RDM_AoE_oGCD_Prefulgence;
                int engagementPool = (SingleTarget && Config.RDM_ST_oGCD_Engagement_Pooling) || (!SingleTarget && Config.RDM_AoE_oGCD_Engagement_Pooling) ? 1 : 0;

                bool corpacorps = SingleTarget ? Config.RDM_ST_oGCD_CorpACorps : Config.RDM_AoE_oGCD_CorpACorps;
                int corpsacorpsPool = (SingleTarget && Config.RDM_ST_oGCD_CorpACorps_Pooling) || (!SingleTarget && Config.RDM_ST_oGCD_CorpACorps_Pooling) ? 1 : 0;
                int corpacorpsRange = (SingleTarget && Config.RDM_ST_oGCD_CorpACorps_Melee) || (!SingleTarget && Config.RDM_ST_oGCD_CorpACorps_Melee) ? 3 : 25;


                //Grabs an oGCD to return based on radio options
                if (engagement
                    && (GetRemainingCharges(Engagement) > engagementPool
                        || (GetRemainingCharges(Engagement) == 1 && GetCooldownRemainingTime(Engagement) < 3))
                    && LevelChecked(Engagement)
                    && distance <= 3)
                    placeOGCD = Engagement;
                if (corpacorps
                    && (GetRemainingCharges(Corpsacorps) > corpsacorpsPool
                        || (GetRemainingCharges(Corpsacorps) == 1 && GetCooldownRemainingTime(Corpsacorps) < 3))
                    && ((GetRemainingCharges(Corpsacorps) >= GetRemainingCharges(Engagement)) || !LevelChecked(Engagement)) // Try to alternate between Corps-a-corps and Engagement
                    && LevelChecked(Corpsacorps)
                    && distance <= corpacorpsRange)
                    placeOGCD = Corpsacorps;

                if (contra
                    && ActionReady(ContreSixte))
                    placeOGCD = ContreSixte;
                if (fleche && ActionReady(Fleche))
                    placeOGCD = Fleche;

                if (vice &&
                    TraitLevelChecked(Traits.EnhancedEmbolden) &&
                    HasEffect(Buffs.ThornedFlourish))
                    placeOGCD = ViceOfThorns;

                if (prefulg &&
                    TraitLevelChecked(Traits.EnhancedManaficationIII) &&
                    HasEffect(Buffs.PrefulugenceReady))
                    placeOGCD = Prefulgence;

                if (CanSpellWeave(actionID) && placeOGCD != 0)
                {
                    newActionID = placeOGCD;
                    return true;
                }

                if (actionID is Fleche && placeOGCD == 0) // All actions are on cooldown, determine the lowest CD to display on Fleche.
                {
                    placeOGCD = Fleche;
                    if (contra
                        && LevelChecked(ContreSixte)
                        && GetCooldownRemainingTime(placeOGCD) > GetCooldownRemainingTime(ContreSixte))
                        placeOGCD = ContreSixte;
                    if (corpacorps
                        && LevelChecked(Corpsacorps)
                        && !HasCharges(Corpsacorps)
                        && GetCooldownRemainingTime(placeOGCD) > GetCooldownRemainingTime(Corpsacorps))
                        placeOGCD = Corpsacorps;
                    if (engagement
                        && LevelChecked(Engagement)
                        && GetCooldownRemainingTime(Engagement) == 0
                        && GetCooldownRemainingTime(placeOGCD) > GetCooldownRemainingTime(Engagement))
                        placeOGCD = Engagement;
                }
                if (actionID is Fleche)
                {
                    newActionID = placeOGCD;
                    return true;
                }

                newActionID = 0;
                return false;
            }
        }

        internal class RDMLucid
        {
            internal static bool SafetoUse(in uint lastComboMove)
            {
                return
                    !HasEffect(Buffs.Dualcast)
                    && lastComboMove != EnchantedRiposte
                    && lastComboMove != EnchantedZwerchhau
                    && lastComboMove != EnchantedRedoublement
                    && lastComboMove != Verflare
                    && lastComboMove != Verholy
                    && lastComboMove != Scorch; // Change abilities to Lucid Dreaming for entire weave window
            }
        }

        internal class RDMOpenerLogic
        {
            private static bool HasCooldowns()
            {
                if (CustomComboFunctions.GetRemainingCharges(Acceleration) < 2)
                    return false;

                if (CustomComboFunctions.GetRemainingCharges(Corpsacorps) < 2)
                    return false;

                if (CustomComboFunctions.GetRemainingCharges(Engagement) < 2)
                    return false;

                if (!CustomComboFunctions.ActionReady(Embolden))
                    return false;

                if (!CustomComboFunctions.ActionReady(Manafication))
                    return false;

                if (!CustomComboFunctions.ActionReady(Fleche))
                    return false;

                if (!CustomComboFunctions.ActionReady(ContreSixte))
                    return false;

                if (!CustomComboFunctions.ActionReady(All.Swiftcast))
                    return false;

                return true;
            }

            private static uint OpenerLevel => 100;

            public uint PrePullStep = 0;

            public uint OpenerStep = 0;

            public static bool LevelChecked => CustomComboFunctions.LocalPlayer.Level >= OpenerLevel;

            private static bool CanOpener => HasCooldowns() && LevelChecked;

            private OpenerState currentState = OpenerState.PrePull;

            public OpenerState CurrentState
            {
                get
                {
                    return currentState;
                }
                set
                {
                    if (value != currentState)
                    {
                        if (value == OpenerState.PrePull)
                        {
                            Svc.Log.Debug($"Entered PrePull Opener");
                        }
                        if (value == OpenerState.InOpener) OpenerStep = 1;
                        if (value == OpenerState.OpenerFinished || value == OpenerState.FailedOpener)
                        {
                            if (value == OpenerState.FailedOpener)
                                Svc.Log.Information($"Opener Failed at step {OpenerStep}");

                            ResetOpener();
                        }
                        if (value == OpenerState.OpenerFinished) Svc.Log.Information("Opener Finished");

                        currentState = value;
                    }
                }
            }

            private bool DoPrePullSteps(ref uint actionID)
            {
                if (!LevelChecked)
                    return false;

                if (CanOpener && PrePullStep == 0)
                {
                    PrePullStep = 1;
                }

                if (!HasCooldowns())
                {
                    PrePullStep = 0;
                }

                if (CurrentState == OpenerState.PrePull && PrePullStep > 0)
                {
                    if (CustomComboFunctions.LocalPlayer.CastActionId == Veraero3 && PrePullStep == 1) CurrentState = OpenerState.InOpener;
                    else if (PrePullStep == 1) actionID = Veraero3;

                    if (ActionWatching.CombatActions.Count > 2 && CustomComboFunctions.InCombat())
                        CurrentState = OpenerState.FailedOpener;

                    return true;
                }
                PrePullStep = 0;
                return false;
            }

            private bool DoOpener(ref uint actionID)
            {
                if (!LevelChecked)
                    return false;

                if (currentState == OpenerState.InOpener)
                {
                    if (CustomComboFunctions.WasLastAction(Verthunder3) && OpenerStep == 1) OpenerStep++;
                    else if (OpenerStep == 1) actionID = Verthunder3;

                    if (CustomComboFunctions.WasLastAction(All.Swiftcast) && OpenerStep == 2) OpenerStep++;
                    else if (OpenerStep == 2) actionID = All.Swiftcast;

                    if (CustomComboFunctions.WasLastAction(Verthunder3) && OpenerStep == 3) OpenerStep++;
                    else if (OpenerStep == 3) actionID = Verthunder3;

                    if (CustomComboFunctions.WasLastAction(Fleche) && OpenerStep == 4) OpenerStep++;
                    else if (OpenerStep == 4) actionID = Fleche;

                    if (CustomComboFunctions.WasLastAction(Acceleration) && OpenerStep == 5) OpenerStep++;
                    else if (OpenerStep == 5) actionID = Acceleration;

                    if (CustomComboFunctions.WasLastAction(Verthunder3) && OpenerStep == 6) OpenerStep++;
                    else if (OpenerStep == 6) actionID = Verthunder3;

                    if (CustomComboFunctions.WasLastAction(Embolden) && OpenerStep == 7) OpenerStep++;
                    else if (OpenerStep == 7) actionID = Embolden;

                    if (CustomComboFunctions.WasLastAction(Manafication) && OpenerStep == 8) OpenerStep++;
                    else if (OpenerStep == 8) actionID = Manafication;

                    if (CustomComboFunctions.WasLastAction(EnchantedRiposte) && OpenerStep == 9) OpenerStep++;
                    else if (OpenerStep == 9) actionID = EnchantedRiposte;

                    if (CustomComboFunctions.WasLastAction(ContreSixte) && OpenerStep == 10) OpenerStep++;
                    else if (OpenerStep == 10) actionID = ContreSixte;

                    if (CustomComboFunctions.WasLastAction(EnchantedZwerchhau) && OpenerStep == 11) OpenerStep++;
                    else if (OpenerStep == 11) actionID = EnchantedZwerchhau;

                    if (CustomComboFunctions.WasLastAction(Engagement) && OpenerStep == 12) OpenerStep++;
                    else if (OpenerStep == 12) actionID = Engagement;

                    if (CustomComboFunctions.WasLastAction(EnchantedRedoublement) && OpenerStep == 13) OpenerStep++;
                    else if (OpenerStep == 13) actionID = EnchantedRedoublement;

                    if (CustomComboFunctions.WasLastAction(Corpsacorps) && OpenerStep == 14) OpenerStep++;
                    else if (OpenerStep == 14) actionID = Corpsacorps;

                    if (CustomComboFunctions.WasLastAction(Verholy) && OpenerStep == 15) OpenerStep++;
                    else if (OpenerStep == 15) actionID = Verholy;

                    if (CustomComboFunctions.WasLastAction(ViceOfThorns) && OpenerStep == 16) OpenerStep++;
                    else if (OpenerStep == 16) actionID = ViceOfThorns;

                    if (CustomComboFunctions.WasLastAction(Scorch) && OpenerStep == 17) OpenerStep++;
                    else if (OpenerStep == 17) actionID = Scorch;

                    if (CustomComboFunctions.WasLastAction(Engagement) && OpenerStep == 18) OpenerStep++;
                    else if (OpenerStep == 18) actionID = Engagement;

                    if (CustomComboFunctions.WasLastAction(Corpsacorps) && OpenerStep == 19) OpenerStep++;
                    else if (OpenerStep == 19) actionID = Corpsacorps;

                    if (CustomComboFunctions.WasLastAction(Resolution) && OpenerStep == 20) OpenerStep++;
                    else if (OpenerStep == 20) actionID = Resolution;

                    if (CustomComboFunctions.WasLastAction(Prefulgence) && OpenerStep == 21) OpenerStep++;
                    else if (OpenerStep == 21) actionID = Prefulgence;

                    if (CustomComboFunctions.WasLastAction(GrandImpact) && OpenerStep == 22) OpenerStep++;
                    else if (OpenerStep == 22) actionID = GrandImpact;

                    if (CustomComboFunctions.WasLastAction(Acceleration) && OpenerStep == 23) OpenerStep++;
                    else if (OpenerStep == 23) actionID = Acceleration;

                    if (CustomComboFunctions.WasLastAction(Verfire) && OpenerStep == 24) OpenerStep++;
                    else if (OpenerStep == 24) actionID = Verfire;

                    if (CustomComboFunctions.WasLastAction(GrandImpact) && OpenerStep == 25) OpenerStep++;
                    else if (OpenerStep == 25) actionID = GrandImpact;

                    if (CustomComboFunctions.WasLastAction(Verthunder3) && OpenerStep == 26) OpenerStep++;
                    else if (OpenerStep == 26) actionID = Verthunder3;

                    if (CustomComboFunctions.WasLastAction(Fleche) && OpenerStep == 27) OpenerStep++;
                    else if (OpenerStep == 27) actionID = Fleche;

                    if (CustomComboFunctions.WasLastAction(Veraero3) && OpenerStep == 28) OpenerStep++;
                    else if (OpenerStep == 28) actionID = Veraero3;

                    if (CustomComboFunctions.WasLastAction(Verfire) && OpenerStep == 29) OpenerStep++;
                    else if (OpenerStep == 29) actionID = Verfire;

                    if (CustomComboFunctions.WasLastAction(Verthunder3) && OpenerStep == 30) OpenerStep++;
                    else if (OpenerStep == 30) actionID = Verthunder3;

                    if (CustomComboFunctions.WasLastAction(Verstone) && OpenerStep == 31) OpenerStep++;
                    else if (OpenerStep == 31) actionID = Verstone;

                    if (CustomComboFunctions.WasLastAction(Veraero3) && OpenerStep == 32) OpenerStep++;
                    else if (OpenerStep == 32) actionID = Veraero3;

                    if (CustomComboFunctions.WasLastAction(All.Swiftcast) && OpenerStep == 33) OpenerStep++;
                    else if (OpenerStep == 33) actionID = All.Swiftcast;

                    if (CustomComboFunctions.WasLastAction(Veraero3) && OpenerStep == 34) OpenerStep++;
                    else if (OpenerStep == 34) actionID = Veraero3;

                    if (CustomComboFunctions.WasLastAction(ContreSixte) && OpenerStep == 35) CurrentState = OpenerState.OpenerFinished;
                    else if (OpenerStep == 35) actionID = ContreSixte;

                    if (ActionWatching.TimeSinceLastAction.TotalSeconds >= 5)
                        CurrentState = OpenerState.FailedOpener;

                    if (((actionID == Embolden && CustomComboFunctions.IsOnCooldown(Embolden)) ||
                        (actionID == Manafication && CustomComboFunctions.IsOnCooldown(Manafication)) ||
                        (actionID == Fleche && CustomComboFunctions.IsOnCooldown(Fleche)) ||
                        (actionID == ContreSixte && CustomComboFunctions.IsOnCooldown(ContreSixte)) ||
                        (actionID == All.Swiftcast && CustomComboFunctions.IsOnCooldown(All.Swiftcast)) ||
                        (actionID == Acceleration && CustomComboFunctions.GetRemainingCharges(Acceleration) < 2) ||
                        (actionID == Corpsacorps && CustomComboFunctions.GetRemainingCharges(Corpsacorps) < 2) ||
                        (actionID == Engagement && CustomComboFunctions.GetRemainingCharges(Engagement) < 2)) && ActionWatching.TimeSinceLastAction.TotalSeconds >= 3)
                    {
                        CurrentState = OpenerState.FailedOpener;
                        return false;
                    }
                    return true;
                }
                return false;
            }

            private void ResetOpener()
            {
                PrePullStep = 0;
                OpenerStep = 0;
            }

            public bool DoFullOpener(ref uint actionID)
            {
                if (!LevelChecked)
                    return false;

                if (CurrentState == OpenerState.PrePull)
                    if (DoPrePullSteps(ref actionID))
                        return true;

                if (CurrentState == OpenerState.InOpener)
                {
                    if (DoOpener(ref actionID))
                        return true;
                }

                if (!CustomComboFunctions.InCombat())
                {
                    ResetOpener();
                    CurrentState = OpenerState.PrePull;
                }
                return false;
            }
        }
    }
}