using BepInEx.Logging;
using System;

namespace LensIslandModMenu.Cheats
{
    internal class PlayerCheats
    {
        public static void IncreaseBackpackLevel(ManualLogSource Log)
        {
            try
            {
                Log.LogInfo("Increasing backpack level...");
                int backPackLvl = Singleton<Player>.Instance.backPackLvl;
                Singleton<Player>.Instance.SetBackPack(backPackLvl + 1);
                Log.LogInfo($"Backpack level increased to {backPackLvl + 1}.");
            }
            catch (Exception ex)
            {
                Log.LogError($"Failed to increase backpack level: {ex.Message}");
            }
        }

        public static void KillPlayer(ManualLogSource Log)
        {
            Log.LogInfo("Killing player...");
            try
            {
                Singleton<Player>.Instance.Die();
            }
            catch (Exception ex)
            {
                Log.LogError($"Failed to kill player: {ex.Message}");
            }
        }

        public static void GiveXP(ManualLogSource Log, int amount)
        {
            Log.LogInfo($"Granting players {amount} XP.");
            try
            {
                global::EventHandler.OnGiveAllPlayersXP(amount);
                SkillSystem.Instance.GiveXP(amount);
                Log.LogInfo($"Granted {amount} XP to player.");
            }
            catch (Exception ex)
            {
                Log.LogError($"Failed to give XP ({amount}). Message: {ex.Message}");
            }
        }
    }
}
