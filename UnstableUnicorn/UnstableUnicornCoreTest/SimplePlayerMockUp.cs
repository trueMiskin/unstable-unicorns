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

        public string PrimarilyStealThisCard = null;

        protected override bool ActivateEffectCore(AEffect effect) => true;

        protected override List<APlayer> ChoosePlayersCore(int number, AEffect effect, List<APlayer> playersWhichCanBeSelected) {
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

        protected override List<Card> WhichCardsToDestroyCore(int number, AEffect effect, List<Card> cardsWhichCanBeSelected) {
            var ret = SimpleSelectionFromCards(number, cardsWhichCanBeSelected);

            if (ChooseCardsWhichCantBeDestroy) {
                foreach (var card in GameController.GetCardsOnTable()) {
                    if (!card.CanBeDestroyed(effect)) {
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

        protected override List<Card> WhichCardsToDiscardCore(int number, AEffect effect, List<Card> cardsWhichCanBeSelected) {
            return SimpleSelectionFromCards(number, cardsWhichCanBeSelected);
        }

        protected override List<Card> WhichCardsToGetCore(int number, AEffect effect, List<Card> cardsWhichCanBeSelected) {
            return SimpleSelectionFromCards(number, cardsWhichCanBeSelected);
        }

        protected override List<Card> WhichCardsToSacrificeCore(int number, AEffect effect, List<Card> cardsWhichCanBeSelected) {
            return SimpleSelectionFromCards(number, cardsWhichCanBeSelected);
        }

        protected override List<Card> WhichCardsToSaveCore(int number, AEffect effect, List<Card> cardsWhichCanBeSelected) {
            return SimpleSelectionFromCards(number, cardsWhichCanBeSelected);
        }

        protected override List<Card> WhichCardsToStealCore(int number, AEffect effect, List<Card> cardsWhichCanBeSelected) {
            var ret = SimpleSelectionFromCards(number, cardsWhichCanBeSelected);

            var card = cardsWhichCanBeSelected.Find(card => card.Name == PrimarilyStealThisCard); ;
            if (card != null)
                ret[^1] = card;

            return ret;
        }

        protected override Card WhichCardToPlayCore() {
            return Hand[0];
        }

        protected override AEffect WhichEffectToSelectCore(List<AEffect> effectsVariants) {
            return effectsVariants[WhichEffectShouldBeSelected];
        }

        protected override List<Card> WhichCardsToMoveCore(int number, AEffect effect, List<Card> cardsWhichCanBeSelected) {
            return SimpleSelectionFromCards(number, cardsWhichCanBeSelected);
        }

        protected override List<Card> WhichCardsToReturnCore(int number, AEffect effect, List<Card> cardsWhichCanBeSelected) {
            return SimpleSelectionFromCards(number, cardsWhichCanBeSelected);
        }

        protected override APlayer WhereShouldBeCardPlayedCore(Card card) {
            return this;
        }

        protected override Card PlayInstantOnStackCore(List<Card> stack, List<Card> availableInstantCards) {
            return null;
        }
    }
}
