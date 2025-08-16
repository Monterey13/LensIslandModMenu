using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;

namespace LensIslandModMenu.MonoMod
{
    [HarmonyPatch(typeof(BlackjackController))]
    [HarmonyPatch(nameof(BlackjackController.WithinDistance), new[] { typeof(TransformData) })]
    public static class Patch_BlackJackController_WithinDistance
    {
        [HarmonyPrefix]
        public static bool Prefix(BlackjackController __instance, TransformData t, ref bool __result)
        {
            if (ModMenuPlugin.IsPlayingBlackjack == true)
            {
                __result = true;
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
