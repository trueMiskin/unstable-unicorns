using System;
using System.Collections.Generic;

namespace UnstableUnicornCore.BasicEffects {
    public sealed class StealEffect : AEffect {
        private List<ECardType> _allowedCardTypes;
        public StealEffect(Card owningCard, int cardCount, List<ECardType> targetType) : base(owningCard, cardCount) {
            _allowedCardTypes = targetType;
            TargetOwner = owningCard.Player;
            TargetLocation = CardLocation.OnTable;
        }
        private int numberValidTargets(GameController gameController) {
            List<Card> cards = gameController.GetCardsOnTable();
            int numberValidTargets = 0;

            foreach (Card c in cards)
                if (_allowedCardTypes.Contains(c.CardType) && c.Player != OwningPlayer)
                    numberValidTargets++;

            return numberValidTargets;
        }
        public override void ChooseTargets(GameController gameController) {
            int stealableCards = numberValidTargets(gameController);
            if (_cardCount > stealableCards)
                _cardCount = stealableCards;

            // owner choose target cards to steal
            CardTargets = OwningPlayer.WhichCardsToSteal(_cardCount, _allowedCardTypes);

            if (CardTargets.Count != _cardCount)
                throw new InvalidOperationException($"Not selected enough cards to steal");
            
            // TODO: Check if cards are not same
            // TODO: Check if card is not target of another affect
            foreach (var card in CardTargets) {
                if (card.Location != CardLocation.OnTable)
                    throw new InvalidOperationException("Selected a card which is not on table");
                if (card.Player == OwningPlayer)
                    throw new InvalidOperationException("Player selected own card.");
                if (!_allowedCardTypes.Contains(card.CardType))
                    throw new InvalidOperationException($"Card {card.Name} have not allowed card type to be stealed");
            }
        }

        public override void InvokeEffect(GameController gameController) {
            foreach(var card in CardTargets)
                card.MoveCard(gameController, TargetOwner, TargetLocation);
        }

        public override bool MeetsRequirementsToPlayInner(GameController gameController) {
            return numberValidTargets(gameController) >= _cardCount;
        }
    }
}
