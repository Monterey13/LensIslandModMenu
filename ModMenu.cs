using BepInEx;
using BepInEx.Logging;
using UnityEngine;

namespace LensIslandModMenu
{
    public class ModMenu
    {
        [BepInPlugin("com.Midnight.LensIslandModMenu", "Lens Island Mod Menu", "0.1.0")]
        public sealed class ModMenuPlugin : BaseUnityPlugin
        {
            public const string PluginGuid = "midnight.lensisland.modmenu";
            public const string PluginName = "Lens Island Mod Menu";
            public const string PluginVersion = "0.1.0";

            internal static ManualLogSource Log;
            private bool _menuOpen;

            private void Awake()
            {
                Log = Logger;
                Log.LogInfo($"{PluginName} v{PluginVersion} loaded.");
            }
            private void Update()
            {
                if (Input.GetKeyDown(KeyCode.Insert))
                    _menuOpen = !_menuOpen;
            }

            private void OnGUI()
            {
                if (!_menuOpen) return;

                GUILayout.BeginArea(new Rect(20, 20, 260, 260), GUI.skin.window);
                GUILayout.Label("Midnight Mod Menu", GUI.skin.label);

                //TODO: Add mod menu function calls here.

                GUILayout.EndArea();
            }
        }
    }
}
