using UnstableUnicornCore.BasicEffects;

namespace UnstableUnicornCore.BaseSet {
    public class UnicornSwap : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return EmptyCard
                .Name("Unicorn Swap")
                .CardType(ECardType.Spell)
                .Text("Move a Unicorn card in your Stable to any other player's Stable, then STEAL a Unicorn card from that player's Stable.")
                .Cast((Card owningCard) => new MoveUnicornCardThenStealCard(owningCard, 1))
                ;
        }
    }
}
