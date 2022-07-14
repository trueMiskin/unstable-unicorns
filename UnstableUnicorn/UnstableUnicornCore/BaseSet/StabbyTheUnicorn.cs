using System.Collections.Generic;
using UnstableUnicornCore.BasicEffects;

namespace UnstableUnicornCore.BaseSet {
    public class StabbyTheUnicorn : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return Card
                .Name("Stabby The Unicorn")
                .CardType(ECardType.MagicUnicorn)
                .Text("If this card is sacrificed or destroyed, you may DESTROY a Unicorn card.")
                .TriggerEffect(
                    TriggerPredicates.IfThisCardWouldBeSacrificedOrDestroyed,
                    new List<ETriggerSource> { ETriggerSource.PreCardLeftStable },
                    (Card owningCard) => new ActivatableEffect(owningCard,
                        new DestroyEffect(owningCard, 1, ECardTypeUtils.UnicornTarget)
                    )
                );
        }
    }
}
