using UnstableUnicornCore.BasicEffects;

namespace UnstableUnicornCore.BaseSet {
    public class GoodDeal : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return EmptyCard
                .Name("Good Deal")
                .CardType(ECardType.Spell)
                .Text("DRAW 3 cards and DISCARD a card.")
                .Cast((Card owningCard) => new AndEffect(owningCard,
                        new DrawEffect(owningCard, 3),
                        new DiscardEffect(owningCard, 1, ECardTypeUtils.CardTarget, PlayerTargeting.PlayerOwner)
                    )
                );
        }
    }
}
