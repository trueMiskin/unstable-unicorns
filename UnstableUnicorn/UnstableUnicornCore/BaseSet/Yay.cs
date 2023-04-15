using UnstableUnicornCore.BasicContinuousEffects;

namespace UnstableUnicornCore.BaseSet {
    public class Yay : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return EmptyCard
                .Name("Yay")
                .CardType(ECardType.Upgrade)
                .Text("Cards you play cannot be Neigh'd.")
                .ContinuousFactory((Card owningCard) => new PlayerCardsCantBeNeighd(owningCard))
                ;
        }
    }
}
