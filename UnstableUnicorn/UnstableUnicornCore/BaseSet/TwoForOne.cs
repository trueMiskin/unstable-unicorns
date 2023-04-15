using UnstableUnicornCore.BasicEffects;

namespace UnstableUnicornCore.BaseSet {
    public class TwoForOne : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return EmptyCard
                .Name("Two-for-one")
                .CardType(ECardType.Spell)
                .Text("SACRIFICE a card, then DESTROY 2 cards.")
                .Cast((Card owningCard) => new ConditionalEffect(owningCard,
                        new SacrificeEffect(owningCard, 1, ECardTypeUtils.CardTarget),
                        new DestroyEffect(owningCard, 2, ECardTypeUtils.CardTarget)
                    )
                );
        }
    }
}
