using UnstableUnicornCore.BasicEffects;

namespace UnstableUnicornCore.BaseSet {
    public class UnicornPoison : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return Card
                .Name("Unicorn Poison")
                .CardType(ECardType.Spell)
                .Text("DESTROY a Unicorn card.")
                .Cast((Card owningCard) =>
                    new DestroyEffect(owningCard, 1, ECardTypeUtils.UnicornTarget)
                );
        }
    }
}
