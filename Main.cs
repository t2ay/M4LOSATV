using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GHPC.AI;
using GHPC.Camera;
using GHPC.Player;
using GHPC.State;
using M4LOSATV;
using MelonLoader;
using NWH.VehiclePhysics;
using UnityEngine;

[assembly: MelonInfo(typeof(LOSATVMod), "M4 LOSATV", "1.2.0", "Cyance and T2")]
[assembly: MelonGame("Radian Simulations LLC", "GHPC")]

namespace M4LOSATV
{
    public class LOSATVMod : MelonMod
    {
        public static GameObject[] vic_gos;
        public static GameObject gameManager;
        public static CameraManager camManager;
        public static PlayerInput playerManager;

        public IEnumerator GetVics(GameState _)
        {
            vic_gos = GameObject.FindGameObjectsWithTag("Vehicle");

            yield break;
        }



        public override void OnSceneWasLoaded(int idx, string scene_name)
        {
            if (scene_name == "MainMenu2_Scene" || scene_name == "LOADER_MENU" || scene_name == "LOADER_INITIAL" || scene_name == "t64_menu" || scene_name == "MainMenu2-1_Scene") return;

            gameManager = GameObject.Find("_APP_GHPC_");
            camManager = gameManager.GetComponent<CameraManager>();
            playerManager = gameManager.GetComponent<PlayerInput>();

            StateController.RunOrDefer(GameState.GameReady, new GameStateEventHandler(GetVics), GameStatePriority.Medium);
            M4_LOSATV.Init();
        }
    }
}