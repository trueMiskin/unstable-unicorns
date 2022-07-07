using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnstableUnicornCore {
    public sealed class SacrificeEffect : AEffect {

        public SacrificeEffect(Card owningCard, int cardCount) : base(owningCard, cardCount) {
            OwningCard = owningCard;

            TargetOwner = null;
            TargetLocation = CardLocation.DiscardPile;
        }

        public override void ChooseTargets(GameController gameController) {
            // TODO: valide to sacrificy max number of cards on board

            CardTargets = OwningPlayer.WhichCardsToSacrifice(_cardCount);

            if (CardTargets.Count != _cardCount)
                throw new InvalidOperationException($"Not selected enough cards to discard");

            foreach (var card in CardTargets) {
                if (card.Player != OwningPlayer || card.Location != CardLocation.OnTable)
                    throw new InvalidOperationException("Selected other player's card or card which is not on table");
                if (gameController.cardsWhichAreTargeted.Contains(card))
                    throw new InvalidOperationException($"Card {card.Name} is targeted by another effect");
            }
        }

        public override void InvokeEffect(ETriggerSource triggerSource, AEffect? effect, GameController gameController) {
            foreach (var card in CardTargets)
                card.MoveCard(gameController, TargetOwner, TargetLocation);
        }

        public override bool MeetsRequirementsToPlayInner(GameController gameController) {
            return OwningPlayer.Stable.Count +
                OwningPlayer.Upgrades.Count +
                OwningPlayer.Downgrades.Count
                >= _cardCount;
        }
    }
}
