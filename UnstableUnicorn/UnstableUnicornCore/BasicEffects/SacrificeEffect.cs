using System;
using System.Collections.Generic;

namespace UnstableUnicornCore.BasicEffects {
    public sealed class SacrificeEffect : AEffect {
        // card types which can be targeted
        List<ECardType> _allowedCardTypes;

        public SacrificeEffect(Card owningCard, int cardCount, List<ECardType> targetType) : base(owningCard, cardCount) {
            OwningCard = owningCard;

            _allowedCardTypes = targetType;
            TargetOwner = null;
            TargetLocation = CardLocation.DiscardPile;
        }

        private int numberValidTargets(GameController gameController) {
            int numberValidTargets = 0;

            List<Card> cards = gameController.GetCardsOnTable();
            foreach (Card c in cards)
                if (_allowedCardTypes.Contains(c.CardType) && c.CanBeSacriced() && c.Player == OwningPlayer)
                    numberValidTargets++;

            return numberValidTargets;
        }

        public override void ChooseTargets(GameController gameController) {
            int numCardsSacrifice = numberValidTargets(gameController);

            if (_cardCount > numCardsSacrifice)
                _cardCount = numCardsSacrifice;

            CardTargets = OwningPlayer.WhichCardsToSacrifice(_cardCount, _allowedCardTypes);

            if (CardTargets.Count != _cardCount)
                throw new InvalidOperationException($"Not selected enough cards to discard");

            foreach (var card in CardTargets) {
                if (card.Player != OwningPlayer || card.Location != CardLocation.OnTable)
                    throw new InvalidOperationException("Selected other player's card or card which is not on table");
                if (!_allowedCardTypes.Contains(card.CardType) || !card.CanBeSacriced())
                    throw new InvalidOperationException($"Card {card.Name} have not allowed card type or can't be sacrificed");
                if (gameController.cardsWhichAreTargeted.Contains(card))
                    throw new InvalidOperationException($"Card {card.Name} is targeted by another effect");
            }
        }

        public override void InvokeEffect(GameController gameController) {
            foreach (var card in CardTargets)
                card.MoveCard(gameController, TargetOwner, TargetLocation);
        }

        public override bool MeetsRequirementsToPlayInner(GameController gameController) {
            return numberValidTargets(gameController) >= _cardCount;
        }
    }
}
