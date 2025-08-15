using System;
using BepInEx;
using BepInEx.Logging;
using LensIslandModMenu.Cheats;
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

        private bool _menuOpen;
        private Rect _menuRect = new Rect(20, 20, 300, 260);

        private void Awake()
        {
            Log = Logger;
            Log.LogInfo($"{PluginName} v{PluginVersion} loaded. goActive={gameObject.activeInHierarchy}, compEnabled={enabled}");

            // Create a persistent driver so Update runs no matter what.
            var go = new GameObject("Midnight_ModMenu_Driver");
            go.hideFlags = HideFlags.HideAndDontSave;
            GameObject.DontDestroyOnLoad(go);

            var driver = go.AddComponent<UpdateDriver>();
            driver.OnUpdate += PluginUpdate;
            driver.OnGUIEvent += PluginOnGUI;
        }

        // Our "Update" logic lives here, driven by UpdateDriver
        private void PluginUpdate()
        {
            // Add a heartbeat if you want to verify:
            // if (Time.frameCount % 120 == 0) Log.LogInfo("PluginUpdate tick");

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
                    if (GUILayout.Button("Increase Backpack Level"))
                    {
                        Log.LogInfo("Increase Backpack Level pressed.");
                        PlayerCheats.IncreaseBackpackLevel(Log);
                    }
                    if (GUILayout.Button("Kill Player"))
                    {
                        Log.LogInfo("Kill Player pressed.");
                        PlayerCheats.KillPlayer(Log);
                    }
                    GUILayout.Space(8);
                    GUILayout.Label("Toggle: F1");
                    GUILayout.EndVertical();
                    GUI.DragWindow(new Rect(0, 0, 10000, 20));
                },
                "Midnight Mod Menu"
            );
        }
    }

    // Lives in the same assembly; no extra DLLs required.
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
