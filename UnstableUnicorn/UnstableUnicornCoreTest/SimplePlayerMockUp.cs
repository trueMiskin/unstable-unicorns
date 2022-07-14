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

        public override List<APlayer> ChoosePlayers(int number, bool canChooseMyself, AEffect effect) {
            List<APlayer> selectedPlayers = new();
            if (ChooseMyself)
                selectedPlayers.Add(this);

            foreach (var player in GameController.Players)
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

        public List<Card> SimpleSelection(int number) {
            List<Card> list = new();
            for (int i = 0; i < number; i++)
                list.Add(Hand[i]);
            return list;
        }

        public override List<Card> WhichCardsToDestroy(int number, List<ECardType> allowedCardTypes) {
            List<Card> selection = new();
            foreach(var card in GameController.GetCardsOnTable()) {
                if (selection.Count == number)
                    break;
                if( (card.CanBeDestroyed() || ChooseCardsWhichCantBeDestroy) && allowedCardTypes.Contains(card.CardType) ) {
                    selection.Add(card);
                }
            }
            return selection;
        }

        public override List<Card> WhichCardsToDiscard(int number, List<ECardType> allowedCardTypes) {
            return SimpleSelection(number);
        }

        public override List<Card> WhichCardsToGet(int number, AEffect effect, List<Card> cards) {
            return SimpleSelectionFromCards(number, cards);
        }

        public override List<Card> WhichCardsToSacrifice(int number, List<ECardType> allowedCardTypes) {
            List<Card> selection = new();
            foreach (var card in GameController.GetCardsOnTable()) {
                if (selection.Count == number)
                    break;
                if (( (card.CanBeSacriced() && card.Player == this) || ChooseCardsWhichCantBeSacrificed) 
                    && allowedCardTypes.Contains(card.CardType))
                {
                    selection.Add(card);
                }
            }
            return selection;
        }

        public override List<Card> WhichCardsToSave(int number, AEffect effect, List<Card> cardsWhichCanBeSelected) {
            return SimpleSelectionFromCards(number, cardsWhichCanBeSelected);
        }

        public override List<Card> WhichCardsToSteal(int number, List<ECardType> allowedCardTypes) {
            List<Card> selection = new();
            foreach (var card in GameController.GetCardsOnTable()) {
                if (selection.Count == number)
                    break;
                if ( (card.Player != this) && allowedCardTypes.Contains(card.CardType)) {
                    selection.Add(card);
                }
            }
            return selection;
        }

        public override Card WhichCardToPlay() {
            return Hand[0];
        }

        public override AEffect WhichEffectToSelect(List<AEffect> effectsVariants) {
            return effectsVariants[WhichEffectShouldBeSelected];
        }

        public override List<Card> WhichCardsToMove(int number, AEffect effect, List<Card> cards) {
            return SimpleSelectionFromCards(number, cards);
        }
    }
}
