using UnstableUnicornCore.BasicEffects;

namespace UnstableUnicornCore.BaseSet {
    public class UnfairBargain : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return EmptyCard
                .Name("Unfair Bargain")
                .CardType(ECardType.Spell)
                .Text("Trade hands with any other player.")
                .Cast((Card owningCard) => new SwapHandsEffect(owningCard)
                );
        }
    }
}
