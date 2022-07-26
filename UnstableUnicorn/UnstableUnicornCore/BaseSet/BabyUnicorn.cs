using System.Collections.Generic;
using UnstableUnicornCore.BasicEffects;

namespace UnstableUnicornCore.BaseSet {
    public class BabyUnicorn : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return Card
                .Name("Baby Unicorn")
                .CardType(ECardType.BabyUnicorn)
                .Text("If this card would be sacrificed, destroyed, or returned to your hand, return it to the Nursery instead.")
                .TriggerEffect(
                    TriggerPredicates.IfThisCardWouldBeSacrificedOrDestroyedOrRetuned,
                    new List<ETriggerSource> { ETriggerSource.ChangeLocationOfCard },
                    (Card owningCard) => new ReturnThisCardToLocation(owningCard, CardLocation.Nursery)
                );
        }
    }
}
