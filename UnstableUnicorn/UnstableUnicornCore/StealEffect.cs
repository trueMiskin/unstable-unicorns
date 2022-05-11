using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnstableUnicornCore {
    public sealed class StealEffect : AEffect {

        public StealEffect(Card owningCard) {
            OwningCard = owningCard;

            // choosing card as target
            Card card = owningCard.Player.WhichCardToSacrifice();
            if (card.Player == owningCard.Player || card.Location != CardLocation.OnTable)
                throw new InvalidOperationException("Selected own card or card which is not on table");

            TargetCard = card;
            TargetOwner = owningCard.Player;
            TargetLocation = CardLocation.OnTable;
        }

        public override void InvokeEffect(ETriggerSource triggerSource, AEffect? effect, GameController gameController) {
            TargetCard.MoveCard(gameController, TargetOwner, TargetLocation);
        }

    }
}
