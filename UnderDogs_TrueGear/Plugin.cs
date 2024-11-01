using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using MyTrueGear;
using System.Collections.Generic;
using System.Numerics;
using System;

namespace UnderDogs_TrueGear
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BasePlugin
    {
        internal static new ManualLogSource Log;

        private static TrueGearMod _TrueGear = null;
        private static bool isHeartBeat = false;
        private static bool onSection = true;

        public override void Load()
        {
            // Plugin startup logic
            Log = base.Log;

            HarmonyLib.Harmony.CreateAndPatchAll(typeof(Plugin));
            _TrueGear = new TrueGearMod();
            _TrueGear.Play("HeartBeat");

            Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        }

        //private static KeyValuePair<float, float> GetAngle(Vector3 player, Vector3 hitPoint)
        //{
        //    Vector3 hitPos = hitPoint - player;
        //    float hitAngle = Mathf.Atan2(hitPos.x, hitPos.z) * Mathf.Rad2Deg;
        //    hitAngle = hitAngle % 360f;
        //    if (hitAngle < 0f)
        //    {
        //        hitAngle += 360f;
        //    }
        //    hitAngle += 100f;
        //    if (hitAngle > 360f)
        //    {
        //        hitAngle -= 360f;
        //    }

        //    float verticalDifference = hitPoint.y - player.y;
        //    verticalDifference = verticalDifference + 3f;

        //    return new KeyValuePair<float, float>(hitAngle, verticalDifference);
        //}

        [HarmonyPostfix, HarmonyPatch(typeof(PlayerMech), "ShakeCockpit", new Type[] { typeof(float), typeof(float) })]
        private static void PlayerMech_ShakeCockpit_Postfix(PlayerMech __instance,float frequency, float amplitude)
        {
            Log.LogInfo("------------------------------------");
            Log.LogInfo("MechDamage1");
            _TrueGear.Play("PoisonDamage");
        }


        [HarmonyPostfix, HarmonyPatch(typeof(PlayerMech), "ShakeCockpit", new Type[] { typeof(UnityEngine.Vector3), typeof(float), typeof(float) })]
        private static void PlayerMech_ShakeCockpit2_Postfix(PlayerMech __instance, UnityEngine.Vector3 shakeRotationAxisWS, float frequency, float amplitude)
        {
            //var angle = GetAngle(__instance.centralPosition, shakeRotationAxisWS);
            Log.LogInfo("------------------------------------");
            Log.LogInfo($"MechDamage2");
            //_TrueGear.PlayAngle("DefaultDamage",angle.Key,angle.Value);
            _TrueGear.Play("PoisonDamage");
        }

        [HarmonyPostfix, HarmonyPatch(typeof(PlayerMech), "DetachLeft")]
        private static void PlayerMech_DetachLeft_Postfix()
        {
            Log.LogInfo("------------------------------------");
            Log.LogInfo("DetachLeft");
            _TrueGear.Play("DetachLeft");
        }

        [HarmonyPostfix, HarmonyPatch(typeof(PlayerMech), "DetachRight")]
        private static void PlayerMech_DetachRight_Postfix()
        {
            Log.LogInfo("------------------------------------");
            Log.LogInfo("DetachRight");
            _TrueGear.Play("DetachRight");
        }

        [HarmonyPostfix, HarmonyPatch(typeof(PlayerMech), "OnChassisHealthChange", new Type[] { typeof(IHealthComponent), typeof(int), typeof(IEntityBehavior) })]
        private static void PlayerMech_OnChassisHealthChange_Postfix(PlayerMech __instance, IHealthComponent healthcomponent)
        {
            if ((float)healthcomponent.health <= (float)healthcomponent.maxHealth * 0.33f)
            {
                Log.LogInfo("------------------------------------");
                Log.LogInfo("StartHeartBeat");
                _TrueGear.StartHeartBeat();
                isHeartBeat = true;
            }
            else
            {
                Log.LogInfo("------------------------------------");
                Log.LogInfo("StopHeartBeat");
                _TrueGear.StopHeartBeat();
                isHeartBeat = false;
            }
            if (healthcomponent.health == 0)
            {
                Log.LogInfo("------------------------------------");
                Log.LogInfo("PlayerDeath1");
                Log.LogInfo("StopHeartBeat");
                _TrueGear.Play("PlayerDeath");
                _TrueGear.StopHeartBeat();
                isHeartBeat = false;
            }
        }

        //[HarmonyPostfix, HarmonyPatch(typeof(PlayerMech), "KillPlayerMech")]
        //private static void PlayerMech_KillPlayerMech_Postfix()
        //{
        //    Log.LogInfo("------------------------------------");
        //    Log.LogInfo("PlayerDeath");
        //}

        [HarmonyPostfix, HarmonyPatch(typeof(PlayerPunchBehavior), "OnPunchResolved", new Type[] { typeof(EntityInteraction) })]
        private static void PlayerPunchBehavior_OnPunchResolved_Postfix(PlayerPunchBehavior __instance, EntityInteraction punchInteraction)
        {
            if (__instance.mechArm.side == VRSide.Left)
            {
                Log.LogInfo("------------------------------------");
                Log.LogInfo("LeftHandMeleeMajorHit");
                _TrueGear.Play("LeftHandMeleeMajorHit");
            }
            else
            {
                Log.LogInfo("------------------------------------");
                Log.LogInfo("RightHandMeleeMajorHit");
                _TrueGear.Play("RightHandMeleeMajorHit");
            }
        }

        [HarmonyPostfix, HarmonyPatch(typeof(BashGuardEntity), "OnBashResolved", new Type[] { typeof(EntityInteraction) })]
        private static void BashGuardEntity_OnBashResolved_Postfix(BashGuardEntity __instance, EntityInteraction bashInteraction)
        {
            //if (bashInteraction.totalHealthDamage > 0f)
            //{
            //    Log.LogInfo("------------------------------------");
            //    Log.LogInfo("totalHealthDamage");
            //}
            //if (bashInteraction.hasImpact)
            //{
            //    Log.LogInfo("------------------------------------");
            //    Log.LogInfo("hasImpact");
            //}
            if (bashInteraction.result.totalImpact > 0f)
            {
                Log.LogInfo("------------------------------------");
                Log.LogInfo("totalImpact");
                _TrueGear.Play("Impact");
            }
        }

        [HarmonyPostfix, HarmonyPatch(typeof(Bomb), "Explode")]
        private static void Bomb_Explode_Postfix()
        {
            Log.LogInfo("------------------------------------");
            Log.LogInfo("Explode");
            _TrueGear.Play("Explode");

        }

        [HarmonyPostfix, HarmonyPatch(typeof(PauseMenuManager), "Pause")]
        private static void PauseMenuManager_Pause_Postfix()
        {
            Log.LogInfo("------------------------------------");
            Log.LogInfo("Pause");
            Log.LogInfo("StopHeartBeat");
            _TrueGear.StopHeartBeat();
        }


        [HarmonyPostfix, HarmonyPatch(typeof(PauseMenuManager), "UnPause")]
        private static void PauseMenuManager_UnPause_Postfix()
        {
            Log.LogInfo("------------------------------------");
            Log.LogInfo("UnPause");
            if (isHeartBeat && onSection)
            {
                Log.LogInfo("StartHeartBeat");
                _TrueGear.StartHeartBeat();
            }
        }

        [HarmonyPostfix, HarmonyPatch(typeof(PauseMenuManager), "OnExitToMainMenu")]
        private static void PauseMenuManager_OnExitToMainMenu_Postfix()
        {
            Log.LogInfo("------------------------------------");
            Log.LogInfo("OnExitToMainMenu");
            isHeartBeat = false;
        }

        [HarmonyPostfix, HarmonyPatch(typeof(MechArm), "ApplyHaptics")]
        private static void MechArm_ApplyHaptics_Postfix(MechArm __instance, int totalQuality)
        {
            if (__instance.side == VRSide.Left)
            {
                Log.LogInfo("------------------------------------");
                Log.LogInfo("LeftHandMeleeMajorHit");
                _TrueGear.Play("LeftHandMeleeMajorHit");
            }
            else
            {
                Log.LogInfo("------------------------------------");
                Log.LogInfo("RightHandMeleeMajorHit");
                _TrueGear.Play("RightHandMeleeMajorHit");
            }
            Log.LogInfo("ApplyHaptics");
            Log.LogInfo(totalQuality);
            Log.LogInfo(__instance.side);
        }


        [HarmonyPostfix, HarmonyPatch(typeof(LevelController), "OnSectionEnded")]
        private static void LevelController_OnSectionEnded_Postfix(LevelController __instance)
        {
            Log.LogInfo("------------------------------------");
            Log.LogInfo("OnSectionEnded");
            Log.LogInfo("StopHeartBeat");
            onSection = false;
            _TrueGear.StopHeartBeat();
        }

        [HarmonyPostfix, HarmonyPatch(typeof(LevelController), "OnSectionStarted")]
        private static void LevelController_OnSectionStarted_Postfix(LevelController __instance)
        {
            Log.LogInfo("------------------------------------");
            Log.LogInfo("OnSectionStarted");
            onSection = true;
            if (isHeartBeat)
            {
                Log.LogInfo("StartHeartBeat");
                _TrueGear.StartHeartBeat();
            }
        }
    }
}
