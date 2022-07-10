using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnstableUnicornCore {
    public sealed class DestroyEffect : AEffect {
        // card types which can be targeted
        List<ECardType> _allowedCardTypes;

        public DestroyEffect(Card owningCard, int cardCount, List<ECardType> targetType) : base(owningCard, cardCount) {
            TargetOwner = null;
            TargetLocation = CardLocation.DiscardPile;
            _allowedCardTypes = targetType;
        }

        public override void ChooseTargets(GameController gameController) {
            List<Card> cards = gameController.GetCardsOnTable();
            int numCardsOnTable = 0;
            
            foreach (Card c in cards)
                if (_allowedCardTypes.Contains(c.CardType) && c.CanBeDestroyed())
                    numCardsOnTable++;

            if (_cardCount > numCardsOnTable)
                _cardCount = numCardsOnTable;
            
            // owner choose which card should be destroyed
            CardTargets = OwningPlayer.WhichCardsToDestroy(numCardsOnTable, _allowedCardTypes);
            
            if (CardTargets.Count != _cardCount)
                throw new InvalidOperationException($"Not selected enough cards to destroy");

            foreach (var card in CardTargets) {
                if (card.Location != CardLocation.OnTable)
                    throw new InvalidOperationException("Selected a card which is not on table");
                if (!_allowedCardTypes.Contains(card.CardType) || !card.CanBeDestroyed())
                    throw new InvalidOperationException($"Card {card.Name} have not allowed card type or can't be destroyed");
                if (gameController.cardsWhichAreTargeted.Contains(card))
                    throw new InvalidOperationException($"Card {card.Name} is targeted by another effect");
            }
        }

        public override void InvokeEffect(GameController gameController) {
            foreach (var card in CardTargets)
                card.MoveCard(gameController, TargetOwner, TargetLocation);
        }

        public override bool MeetsRequirementsToPlayInner(GameController gameController) {
            return OwningPlayer.Hand.Count >= _cardCount;
        }
    }
}
