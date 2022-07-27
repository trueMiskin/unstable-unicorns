using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnstableUnicornCore;

namespace UnstableUnicornCoreTest {
    public class SimplePlayerMockUp : APlayer {
        public bool ChooseCardsWhichCantBeDestroy { get; set; } = false;
        public bool ChooseCardsWhichCantBeSacrificed { get; set; } = false;
        public bool ChooseMyself { get; set; } = false;
        public int WhichEffectShouldBeSelected { get; set; } = 0;

        public override bool ActivateEffect(AEffect effect) => true;

        public override List<APlayer> ChoosePlayers(int number, AEffect effect, List<APlayer> playersWhichCanBeSelected) {
            List<APlayer> selectedPlayers = new();
            if (ChooseMyself)
                selectedPlayers.Add(this);

            foreach (var player in playersWhichCanBeSelected)
                if (player != this && selectedPlayers.Count < number)
                    selectedPlayers.Add(player);

            return selectedPlayers;
        }

        private static List<Card> SimpleSelectionFromCards(int number, List<Card> cardsWhichCanBeSelected) {
            List<Card> ret = new();
            for (int i = 0; i < number; i++)
                ret.Add(cardsWhichCanBeSelected[i]);

            return ret;
        }

        public override List<Card> WhichCardsToDestroy(int number, AEffect effect, List<Card> cardsWhichCanBeSelected) {
            var ret = SimpleSelectionFromCards(number, cardsWhichCanBeSelected);

            if (ChooseCardsWhichCantBeDestroy) {
                foreach (var card in GameController.GetCardsOnTable()) {
                    if (!card.CanBeDestroyed()) {
                        ret.Add(card);
                    }
                }
            }

            ret.Reverse();
            while (ret.Count > number)
                ret.RemoveAt(ret.Count - 1);
            ret.Reverse();

            return ret;
        }

        public override List<Card> WhichCardsToDiscard(int number, AEffect effect, List<Card> cardsWhichCanBeSelected) {
            return SimpleSelectionFromCards(number, cardsWhichCanBeSelected);
        }

        public override List<Card> WhichCardsToGet(int number, AEffect effect, List<Card> cardsWhichCanBeSelected) {
            return SimpleSelectionFromCards(number, cardsWhichCanBeSelected);
        }

        public override List<Card> WhichCardsToSacrifice(int number, AEffect effect, List<Card> cardsWhichCanBeSelected) {
            return SimpleSelectionFromCards(number, cardsWhichCanBeSelected);
        }

        public override List<Card> WhichCardsToSave(int number, AEffect effect, List<Card> cardsWhichCanBeSelected) {
            return SimpleSelectionFromCards(number, cardsWhichCanBeSelected);
        }

        public override List<Card> WhichCardsToSteal(int number, AEffect effect, List<Card> cardsWhichCanBeSelected) {
            return SimpleSelectionFromCards(number, cardsWhichCanBeSelected);
        }

        public override Card WhichCardToPlay() {
            return Hand[0];
        }

        public override AEffect WhichEffectToSelect(List<AEffect> effectsVariants) {
            return effectsVariants[WhichEffectShouldBeSelected];
        }

        public override List<Card> WhichCardsToMove(int number, AEffect effect, List<Card> cardsWhichCanBeSelected) {
            return SimpleSelectionFromCards(number, cardsWhichCanBeSelected);
        }

        public override List<Card> WhichCardsToReturn(int number, AEffect effect, List<Card> cardsWhichCanBeSelected) {
            return SimpleSelectionFromCards(number, cardsWhichCanBeSelected);
        }

        public override APlayer WhereShouldBeCardPlayed(Card card) {
            return this;
        }

        public override Card PlayInstantOnStack(List<Card> stack) {
            return null;
        }
    }
}
