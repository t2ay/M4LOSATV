using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BehaviorDesigner.Runtime.Tasks.Movement;
using M4LOSATV;
using GHPC.Camera;
using GHPC.Equipment.Optics;
using GHPC.Player;
using GHPC.Vehicle;
using GHPC.Weapons;
using MelonLoader;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using GHPC;
using NWH.VehiclePhysics;
using GHPC.Equipment;
using GHPC.State;
using GHPC.Utility;
using System.Collections;
using HarmonyLib;


namespace M4LOSATV
{
    public static class M4_LOSATV
    {

        public static AmmoClipCodexScriptable clip_codex_KEM;
        public static AmmoType.AmmoClip clip_KEM;
        public static AmmoCodexScriptable ammo_codex_KEM;
        public static AmmoType ammo_KEM;

        public static AmmoType ammo_I_TOW;

        public static IEnumerator Convert(GameState _)
        {
            foreach (GameObject vic_go in LOSATVMod.vic_gos)
            {
                Vehicle vic = vic_go.GetComponent<Vehicle>();

                if (vic == null) continue;
                if (vic.FriendlyName != "M2 Bradley") continue;
                if (vic.GetComponent<Util.AlreadyConvertedLOSATV>() != null) continue;

                vic._friendlyName = "M4 LOSATV";
                vic.gameObject.AddComponent<Util.AlreadyConvertedLOSATV>();


                WeaponsManager weaponsManager = vic.GetComponent<WeaponsManager>();
                WeaponSystemInfo towGunInfo = weaponsManager.Weapons[1];
                WeaponSystem towGun = towGunInfo.Weapon;

                towGunInfo.Name = "KEM Launcher";
                towGun.TriggerHoldTime = 0f;
                towGun.MaxSpeedToFire = 999f;
                towGun.MaxSpeedToDeploy = 999f;
                towGun.RecoilBlurMultiplier = 0.2f;
                towGun.FireWhileGuidingMissile = true;
                vic.AimablePlatforms[2].ForcedStowSpeed = 999f;

                GHPC.Weapons.AmmoRack towRack = towGun.Feed.ReadyRack;

                towRack.ClipTypes[0] = clip_KEM;

                towRack.StoredClips[0] = clip_KEM;
                towRack.StoredClips[1] = clip_KEM;
                towRack.StoredClips[2] = clip_KEM;

                LoadoutManager loadoutManager = vic.GetComponent<LoadoutManager>();

                loadoutManager.SpawnCurrentLoadout();

                PropertyInfo roundInBreech = typeof(AmmoFeed).GetProperty("AmmoTypeInBreech");

                roundInBreech.SetValue(towGun.Feed, null);

                MethodInfo refreshBreech = typeof(AmmoFeed).GetMethod("Start", BindingFlags.Instance | BindingFlags.NonPublic);
                refreshBreech.Invoke(towGun.Feed, new object[] { });

                towRack.AddInvisibleClip(clip_KEM);
            }
            yield break;
        }

        [HarmonyPatch(typeof(GHPC.Weapons.LiveRound), "Start")]

        public static void Init()
        {
            if (ammo_I_TOW == null)
            {
                foreach (AmmoCodexScriptable s in Resources.FindObjectsOfTypeAll(typeof(AmmoCodexScriptable)))
                {
                    if (s.AmmoType.Name == "BGM-71C I-TOW") ammo_I_TOW = s.AmmoType;
                }
                ammo_KEM = new AmmoType();
                Util.ShallowCopy(ammo_KEM, ammo_I_TOW);
                ammo_KEM.Name = "MGM-166 KEM";
                ammo_KEM.Caliber = 162;
                ammo_KEM.Category = AmmoType.AmmoCategory.Penetrator;
                ammo_KEM.RhaPenetration = 780;
                ammo_KEM.MuzzleVelocity = 1510f;
                ammo_KEM.Mass = 77f;
                ammo_KEM.SpallMultiplier = 1.5f;
                ammo_KEM.TurnSpeed = 2.5f;
                ammo_KEM.MaxSpallRha = 20;
                ammo_KEM.CertainRicochetAngle = 5;

                ammo_codex_KEM = ScriptableObject.CreateInstance<AmmoCodexScriptable>();
                ammo_codex_KEM.AmmoType = ammo_KEM;
                ammo_codex_KEM.name = "ammo_KEM";

                clip_KEM = new AmmoType.AmmoClip();
                clip_KEM.Capacity = 4;
                clip_KEM.Name = "MGM-166 KEM";
                clip_KEM.MinimalPattern = new AmmoCodexScriptable[1];
                clip_KEM.MinimalPattern[0] = ammo_codex_KEM;

                clip_codex_KEM = ScriptableObject.CreateInstance<AmmoClipCodexScriptable>();
                clip_codex_KEM.name = "clip_ADAT";
                clip_codex_KEM.ClipType = clip_KEM;
            }
            StateController.RunOrDefer(GameState.GameReady, new GameStateEventHandler(Convert), GameStatePriority.Lowest);
        }
    }
}