using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnstableUnicornCore;

namespace UnstableUnicornCoreTest {
    public class SimplePlayerMockUp : APlayer {
        public override List<APlayer> ChoosePlayers(int number, bool canChooseMyself, AEffect effect) {
            List<APlayer> selectedPlayers = new();
            foreach (var player in GameController.Players)
                if (player != this && selectedPlayers.Count < number)
                selectedPlayers.Add(player);

            return selectedPlayers;
        }

        public List<Card> SimpleSelection(int number) {
            List<Card> list = new();
            for (int i = 0; i < number; i++)
                list.Add(Hand[i]);
            return list;
        }

        public override List<Card> WhichCardsToDestroy(int number) {
            return SimpleSelection(number);
        }

        public override List<Card> WhichCardsToDiscard(int number, List<ECardType> allowedCardTypes) {
            return SimpleSelection(number);
        }

        public override List<Card> WhichCardsToSacrifice(int number) {
            return SimpleSelection(number);
        }

        public override List<Card> WhichCardsToSteal(int number) {
            return SimpleSelection(number);
        }

        public override Card WhichCardToPlay() {
            return Hand[0];
        }
    }
}
