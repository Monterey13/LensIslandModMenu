using BepInEx.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LensIslandModMenu.Cheats
{
    internal class BlackjackCheats
    {
        public static void DealPlayerAce(ManualLogSource Log)
        {
            PlayingCard card = new PlayingCard
            {
                number = 13, // Ace is usually represented as 1 or 13 in card games
                suit = CardSuit.Spade
            };

            Singleton<BlackjackController>.Instance.DealCard(HandOwner.Player, card);
        }
    }
}
