using UnstableUnicornCore.BasicContinuousEffects;

namespace UnstableUnicornCore.BaseSet {
    public class Slowdown : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return Card
                .Name("Slowdown")
                .CardType(ECardType.Downgrade)
                .Text("You cannot play Instant cards.")
                .ContinuousFactory(
                    (Card owningCard) => new PlayerCantPlayInstantCards(owningCard)
                );
        }
    }
}
