using System.Collections.Generic;
using System.Linq;

namespace UnstableUnicornCore {
    public class RandomPlayer : APlayer {
        public override bool ActivateEffect(AEffect effect) {
            return GameController.Random.Next(2) == 1;
        }

        public override List<APlayer> ChoosePlayers(int number, AEffect effect, List<APlayer> playersWhichCanBeSelected) {
            HashSet<int> selectedPlayers = new();

            while (selectedPlayers.Count != number) {
                selectedPlayers.Add(GameController.Random.Next(playersWhichCanBeSelected.Count));
            }

            return (from idx in selectedPlayers select playersWhichCanBeSelected[idx]).ToList();
        }

        public override Card? PlayInstantOnStack(List<Card> stack) {
            return null;
        }

        public override APlayer WhereShouldBeCardPlayed(Card card) {
            if (card.CardType == ECardType.Instant || card.CardType == ECardType.Spell)
                return this;

            while (true) {
                var player = GameController.Players[GameController.Random.Next(GameController.Players.Count)];
                if (card.CanBePlayed(player))
                    return player;
            }
        }

        private List<Card> RandomSelectionFromCards(int number, List<Card> cardsWhichCanBeSelected) {
            HashSet<int> selectedCards = new();

            while(selectedCards.Count != number) {
                selectedCards.Add(GameController.Random.Next(cardsWhichCanBeSelected.Count));
            }

            return (from idx in selectedCards select cardsWhichCanBeSelected[idx]).ToList();
        }

        public override List<Card> WhichCardsToDestroy(int number, AEffect effect, List<Card> cardsWhichCanBeSelected) {
            return RandomSelectionFromCards(number, cardsWhichCanBeSelected);
        }

        public override List<Card> WhichCardsToDiscard(int number, AEffect effect, List<Card> cardsWhichCanBeSelected) {
            return RandomSelectionFromCards(number, cardsWhichCanBeSelected);
        }

        public override List<Card> WhichCardsToGet(int number, AEffect effect, List<Card> cardsWhichCanBeSelected) {
            return RandomSelectionFromCards(number, cardsWhichCanBeSelected);
        }

        public override List<Card> WhichCardsToSacrifice(int number, AEffect effect, List<Card> cardsWhichCanBeSelected) {
            return RandomSelectionFromCards(number, cardsWhichCanBeSelected);
        }

        public override List<Card> WhichCardsToSave(int number, AEffect effect, List<Card> cardsWhichCanBeSelected) {
            return RandomSelectionFromCards(number, cardsWhichCanBeSelected);
        }

        public override List<Card> WhichCardsToSteal(int number, AEffect effect, List<Card> cardsWhichCanBeSelected) {
            return RandomSelectionFromCards(number, cardsWhichCanBeSelected);
        }

        public override Card? WhichCardToPlay() {
            if (Hand.Count == 0 || Hand.All(card => card.CardType == ECardType.Instant || !card.CanBePlayed(this) ))
                return null;

            Card? selectedCard = null;
            while (selectedCard == null) {
                selectedCard = Hand[GameController.Random.Next(Hand.Count)];
                if (selectedCard.CardType == ECardType.Instant || !selectedCard.CanBePlayed(this))
                    selectedCard = null;
            }
            return selectedCard;
        }

        public override AEffect WhichEffectToSelect(List<AEffect> effectsVariants) {
            return effectsVariants[GameController.Random.Next(effectsVariants.Count)];
        }

        public override List<Card> WhichCardsToMove(int number, AEffect effect, List<Card> cardsWhichCanBeSelected) {
            return RandomSelectionFromCards(number, cardsWhichCanBeSelected);
        }

        public override List<Card> WhichCardsToReturn(int number, AEffect effect, List<Card> cardsWhichCanBeSelected) {
            return RandomSelectionFromCards(number, cardsWhichCanBeSelected);
        }
    }
}
