using System.Collections.Generic;
using UnstableUnicornCore.BasicEffects;

namespace UnstableUnicornCore.BaseSet {
    public class AnnoyingFlyingUnicorn : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return EmptyCard
                .Name("Annoying Flying Unicorn")
                .CardType(ECardType.MagicUnicorn)
                .Text("When this card enters your Stable, you may choose any player. That player must DISCARD a card. If this card would be sacrificed or destroyed, return it to your hand instead.")
                .Cast((Card owningCard) => new ActivatableEffect(owningCard,
                        new DiscardEffect(owningCard, 1, ECardTypeUtils.CardTarget, PlayerTargeting.AnyPlayer)
                    )
                )
                .TriggerEffect(
                    TriggerPredicates.IfThisCardWouldBeSacrificedOrDestroyed,
                    new List<ETriggerSource> { ETriggerSource.ChangeLocationOfCard },
                    (Card owningCard) => new ReturnThisCardToLocation(owningCard)
                );
        }
    }
}
