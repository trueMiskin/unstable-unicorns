using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnstableUnicornCore {
    public sealed class StealEffect : AEffect {

        public StealEffect(Card owningCard, int cardCount) : base(owningCard, cardCount) {
            TargetOwner = owningCard.Player;
            TargetLocation = CardLocation.OnTable;
        }

        public override void ChooseTargets(GameController gameController) {
            
            // choosing card as target
            CardTargets = OwningCard.Player.WhichCardsToSacrifice(_cardCount);

            // TODO: Check if cards are not same
            // TODO: Check if card is not target of another affect
            foreach (var card in CardTargets)
                if (card.Player == OwningCard.Player || card.Location != CardLocation.OnTable)
                    throw new InvalidOperationException("Selected own card or card which is not on table");
        }

        public override void InvokeEffect(ETriggerSource triggerSource, AEffect? effect, GameController gameController) {
            foreach(var card in CardTargets)
                card.MoveCard(gameController, TargetOwner, TargetLocation);
        }

        public override bool MeetsRequirementsToPlayInner(GameController gameController) {
            APlayer player = OwningCard.Player;
            return player.Stable.Count +
                player.Upgrades.Count +
                player.Downgrades.Count
                >= _cardCount;
        }
    }
}
