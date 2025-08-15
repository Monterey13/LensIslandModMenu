using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using LensIslandModMenu.Cheats;
using System;
using UnityEngine;

namespace LensIslandModMenu
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    public sealed class ModMenuPlugin : BaseUnityPlugin
    {
        public const string PluginGuid = "midnight.lensisland.modmenu";
        public const string PluginName = "Lens Island Mod Menu";
        public const string PluginVersion = "0.1.0";

        internal static ManualLogSource Log;
        private Harmony _harmony;

        private bool _menuOpen;
        private Rect _menuRect = new Rect(20, 20, 300, 260);

        private void Awake()
        {
            Log = Logger;
            Log.LogInfo($"{PluginName} v{PluginVersion} loaded. goActive={gameObject.activeInHierarchy}, compEnabled={enabled}");

            try
            {
                // Driver to keep Update/OnGUI alive
                var go = new GameObject("Midnight_ModMenu_Driver");
                go.hideFlags = HideFlags.HideAndDontSave;
                DontDestroyOnLoad(go);
                var driver = go.AddComponent<UpdateDriver>();
                driver.OnUpdate += PluginUpdate;
                driver.OnGUIEvent += PluginOnGUI;
                Log.LogInfo("Driver created & hooks attached.");
            }
            catch (Exception ex)
            {
                Log.LogError("Driver init FAILED:\n" + ex);
            }

            try
            {
                DlcDetours.Apply(Log);
                _harmony = new Harmony(PluginGuid);
                _harmony.PatchAll();
            }
            catch (Exception ex)
            {
                Log.LogError("Apply Patches FAILED:\n" + ex);
            }
        }

        private void PluginUpdate()
        {

            if (Input.GetKeyDown(KeyCode.F1))
                _menuOpen = !_menuOpen;
        }

        private void PluginOnGUI()
        {
            if (!_menuOpen) return;

            _menuRect = GUILayout.Window(
                GUIUtility.GetControlID(FocusType.Passive),
                _menuRect,
                id =>
                {
                    GUILayout.BeginVertical();
                    if (GUILayout.Button("Kill Player"))
                    {
                        Log.LogInfo("Kill Player pressed...");
                        PlayerCheats.KillPlayer(Log);
                    }
                    if (GUILayout.Button("Give 500 XP"))
                    {
                        Log.LogInfo("Give XP pressed...");
                        PlayerCheats.GiveXP(Log, 500);
                    }
                    GUILayout.Space(8);
                    GUILayout.Label("Toggle: F1");
                    GUILayout.EndVertical();
                    GUI.DragWindow(new Rect(0, 0, 10000, 20));
                },
                "Midnight Mod Menu"
            );
        }

        private void OnDestroy()
        {
            try
            {
                _harmony?.UnpatchSelf();
            }
            catch
            {
                /*ignore*/
            }
        }
    }

    public sealed class UpdateDriver : MonoBehaviour
    {
        public event Action OnUpdate;
        public event Action OnGUIEvent;

        private void Awake()
        {
            hideFlags = HideFlags.HideAndDontSave;
            DontDestroyOnLoad(gameObject);
        }

        private void Update() => OnUpdate?.Invoke();
        private void OnGUI() => OnGUIEvent?.Invoke();
    }
}
