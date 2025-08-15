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
        private int _xpAmount = 500; // Default XP amount
        private int _rsAmount = 1; // Default resource amount
        private bool _showResourceList = false;
        private ResourceTypes _resourceType = ResourceTypes.GoldCoins; // Default resource type
        private bool _godModeEnabled = false;

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
                    GUILayout.Space(5);
                    _xpAmount = (int)GUILayout.HorizontalSlider(_xpAmount, 0, 10000);
                    if (GUILayout.Button($"Give {_xpAmount} XP"))
                    {
                        Log.LogInfo($"Give XP pressed... Amount: {_xpAmount}");
                        PlayerCheats.GiveXP(Log, _xpAmount);
                    }
                    if (GUILayout.Button($"God Mode: {(_godModeEnabled ? "ON" : "OFF")}"))
                    {
                        Log.LogInfo("TGM Pressed...");
                        _godModeEnabled = PlayerCheats.ToggleGodMode(Log);
                    }
                    // Dropdown toggle button
                    if (GUILayout.Button($"Resource: {_resourceType}"))
                    {
                        _showResourceList = !_showResourceList;
                    }

                    // Show dropdown list if open
                    if (_showResourceList)
                    {
                        foreach (ResourceTypes rt in Enum.GetValues(typeof(ResourceTypes)))
                        {
                            if (GUILayout.Button(rt.ToString()))
                            {
                                _resourceType = rt;
                                _showResourceList = false;
                            }
                        }
                    }

                    // Amount slider
                    _rsAmount = (int)GUILayout.HorizontalSlider(_rsAmount, 1, 250);

                    // Spawn button
                    if (GUILayout.Button($"Spawn {_rsAmount}x {_resourceType}"))
                    {
                        Log.LogInfo("SpawnItem Pressed...");
                        ItemCheats.SpawnItem(Log, _rsAmount, _resourceType);
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
