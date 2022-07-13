using UnstableUnicornCore.BasicContinuousEffects;

namespace UnstableUnicornCore.BaseSet {
    public class BrokenStable : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return Card
                .Name("Broken Stable")
                .CardType(ECardType.Downgrade)
                .Text("You cannot play Upgrade cards.")
                .ContinuousFactory((Card owningCard) => new PlayerCantPlayUpgrades(owningCard));
        }
    }
}
