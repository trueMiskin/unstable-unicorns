using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnstableUnicornCore {
    public sealed class DestroyEffect : AEffect {

        public DestroyEffect(Card owningCard, int cardCount) : base(owningCard, cardCount) {
            TargetOwner = null;
            TargetLocation = CardLocation.DiscardPile;
        }

        public override void ChooseTargets(GameController gameController) {
            int numCardsInHand = OwningPlayer.Hand.Count;
            if (_cardCount > numCardsInHand)
                _cardCount = numCardsInHand;
            
            CardTargets = OwningPlayer.WhichCardsToDestroy(_cardCount);
            
            if (CardTargets.Count != _cardCount)
                throw new InvalidOperationException($"Not selected enough cards to discard");

            foreach (var card in CardTargets) {
                if (card.Player == OwningPlayer || card.Location != CardLocation.OnTable)
                    throw new InvalidOperationException("Selected own card or card which is not on table");
                if (gameController.cardsWhichAreTargeted.Contains(card))
                    throw new InvalidOperationException($"Card {card.Name} is targeted by another effect");
            }
        }

        public override void InvokeEffect(ETriggerSource triggerSource, AEffect? effect, GameController gameController) {
            foreach (var card in CardTargets)
                card.MoveCard(gameController, TargetOwner, TargetLocation);
        }

        public override bool MeetsRequirementsToPlayInner(GameController gameController) {
            return OwningPlayer.Hand.Count >= _cardCount;
        }
    }
}
