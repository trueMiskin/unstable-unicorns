using System.Collections.Generic;
using System.Linq;

namespace UnstableUnicornCore {
    public class RandomPlayer : APlayer {
        protected override bool ActivateEffectCore(AEffect effect) {
            return GameController.Random.Next(2) == 1;
        }

        protected override List<APlayer> ChoosePlayersCore(int number, AEffect effect, List<APlayer> playersWhichCanBeSelected) {
            HashSet<int> selectedPlayers = new();

            while (selectedPlayers.Count != number) {
                selectedPlayers.Add(GameController.Random.Next(playersWhichCanBeSelected.Count));
            }

            return (from idx in selectedPlayers select playersWhichCanBeSelected[idx]).ToList();
        }

        protected override Card? PlayInstantOnStackCore(List<Card> stack, List<Card> availableInstantCards) {
            if (GameController.Random.NextSingle() <= 0.5)
                return availableInstantCards.RandomSelection(GameController.Random, 1)[0];
            return null;
        }

        protected override APlayer WhereShouldBeCardPlayedCore(Card card) {
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

        protected override List<Card> WhichCardsToDestroyCore(int number, AEffect effect, List<Card> cardsWhichCanBeSelected) {
            return RandomSelectionFromCards(number, cardsWhichCanBeSelected);
        }

        protected override List<Card> WhichCardsToDiscardCore(int number, AEffect? effect, List<Card> cardsWhichCanBeSelected) {
            return RandomSelectionFromCards(number, cardsWhichCanBeSelected);
        }

        protected override List<Card> WhichCardsToGetCore(int number, AEffect effect, List<Card> cardsWhichCanBeSelected) {
            return RandomSelectionFromCards(number, cardsWhichCanBeSelected);
        }

        protected override List<Card> WhichCardsToSacrificeCore(int number, AEffect effect, List<Card> cardsWhichCanBeSelected) {
            return RandomSelectionFromCards(number, cardsWhichCanBeSelected);
        }

        protected override List<Card> WhichCardsToSaveCore(int number, AEffect effect, List<Card> cardsWhichCanBeSelected) {
            return RandomSelectionFromCards(number, cardsWhichCanBeSelected);
        }

        protected override List<Card> WhichCardsToStealCore(int number, AEffect effect, List<Card> cardsWhichCanBeSelected) {
            return RandomSelectionFromCards(number, cardsWhichCanBeSelected);
        }

        protected override Card? WhichCardToPlayCore() {
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

        protected override AEffect WhichEffectToSelectCore(List<AEffect> effectsVariants) {
            return effectsVariants[GameController.Random.Next(effectsVariants.Count)];
        }

        protected override List<Card> WhichCardsToMoveCore(int number, AEffect effect, List<Card> cardsWhichCanBeSelected) {
            return RandomSelectionFromCards(number, cardsWhichCanBeSelected);
        }

        protected override List<Card> WhichCardsToReturnCore(int number, AEffect effect, List<Card> cardsWhichCanBeSelected) {
            return RandomSelectionFromCards(number, cardsWhichCanBeSelected);
        }
    }
}
