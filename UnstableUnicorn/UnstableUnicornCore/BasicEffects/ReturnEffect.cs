using System;
using System.Collections.Generic;

namespace UnstableUnicornCore.BasicEffects {
    /// <summary>
    /// Return the choosen card to the owner's hand
    /// </summary>
    public class ReturnEffect : AEffect {
        // card types which can be targeted
        protected List<ECardType> _allowedCardTypes;

        public ReturnEffect(Card owningCard, int cardCount, List<ECardType> targetType) : base(owningCard, cardCount) {
            TargetLocation = CardLocation.InHand;
            _allowedCardTypes = targetType;
        }

        private int numberValidTargets(GameController gameController) {
            List<Card> cards = gameController.GetCardsOnTable();
            int numberValidTargets = 0;

            foreach (Card c in cards)
                if (_allowedCardTypes.Contains(c.CardType))
                    numberValidTargets++;

            return numberValidTargets;
        }

        public override void ChooseTargets(GameController gameController) {
            int numCardsOnTable = numberValidTargets(gameController);

            if (_cardCount > numCardsOnTable)
                _cardCount = numCardsOnTable;

            // owner choose which card should be destroyed
            CardTargets = OwningPlayer.WhichCardsToDestroy(numCardsOnTable, _allowedCardTypes);

            if (CardTargets.Count != _cardCount)
                throw new InvalidOperationException($"Not selected enough cards to destroy");

            foreach (var card in CardTargets) {
                if (card.Location != CardLocation.OnTable)
                    throw new InvalidOperationException("Selected a card which is not on table");
                if (!_allowedCardTypes.Contains(card.CardType))
                    throw new InvalidOperationException($"Card {card.Name} have not allowed card type");
                if (gameController.cardsWhichAreTargeted.Contains(card))
                    throw new InvalidOperationException($"Card {card.Name} is targeted by another effect");
            }
        }

        public override void InvokeEffect(GameController gameController) {
            foreach (var card in CardTargets)
                card.MoveCard(gameController, card.Player, TargetLocation);
        }

        public override bool MeetsRequirementsToPlayInner(GameController gameController) {
            return numberValidTargets(gameController) >= _cardCount;
        }
    }
}
