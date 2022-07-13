using System.Collections.Generic;
using UnstableUnicornCore.BasicEffects;

namespace UnstableUnicornCore.BaseSet {
    public class GreedyFlyingUnicorn : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return Card
                .Name("Greedy Flying Unicorn")
                .CardType(ECardType.MagicUnicorn)
                .Text("When this card enters your Stable, DRAW a card. If this card would be sacrificed or destroyed, return it to your hand instead.")
                .Cast((Card owningCard) => new DrawEffect(owningCard, 1))
                .TriggerEffect(
                    TriggerPredicates.IfThisCardWouldBeSacrificedOrDestroyed,
                    new List<ETriggerSource> { ETriggerSource.ChangeLocationOfCard },
                    (Card owningCard) => new ReturnThisCardToHand(owningCard)
                );
        }
    }
}
