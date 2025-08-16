using BepInEx.Logging;
using MonoMod.RuntimeDetour;
using System;
using System.Reflection;
using UnityEngine;

namespace LensIslandModMenu.MonoMod
{
    internal static class Detour_BlackjackControllerWithinDistance
    {
        private static Hook _bjcWd;

        private delegate bool WithinDistanceOrig(BlackjackController self, Transform t);

        private static bool WithinDistance_Detour(WithinDistanceOrig orig, BlackjackController self, Transform t)
        {
            if (ModMenuPlugin.IsPlayingBlackjack)
                return true;

            return orig(self, t);
        }

        public static void Apply(ManualLogSource log)
        {
            try
            {
                var target = typeof(BlackjackController).GetMethod(
                    "WithinDistance",
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                    binder: null,
                    types: new[] { typeof(Transform) },
                    modifiers: null
                );

                if (target == null)
                {
                    log.LogWarning("WithinDistance(Transform) not found on BlackjackController.");
                    return;
                }

                var detourMI = typeof(Detour_BlackjackControllerWithinDistance)
                    .GetMethod(nameof(WithinDistance_Detour), BindingFlags.Static | BindingFlags.NonPublic);

                _bjcWd = new Hook(target, detourMI);
                log.LogInfo("Detoured BlackjackController.WithinDistance(Transform).");
            }
            catch (Exception ex)
            {
                log.LogError($"Failed to apply Detour_BlackjackControllerWithinDistance: {ex}");
            }
        }

        public static void Dispose()
        {
            try { _bjcWd?.Dispose(); }
            catch { /* ignore */ }
            _bjcWd = null;
        }
    }
}
