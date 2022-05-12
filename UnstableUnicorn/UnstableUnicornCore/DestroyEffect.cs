using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnstableUnicornCore {
    public sealed class DestroyEffect : AEffect {

        public DestroyEffect(Card owningCard, Card targetCard) {
            OwningCard = owningCard;

            TargetCard = targetCard;
            TargetOwner = null;
            TargetLocation = CardLocation.DiscardPile;
        }

        public override void InvokeEffect(ETriggerSource triggerSource, AEffect? effect, GameController gameController) {
            TargetCard.MoveCard(gameController, TargetOwner, TargetLocation);
        }
    }
}
