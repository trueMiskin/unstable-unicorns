using UnstableUnicornCore.BasicContinuousEffects;

namespace UnstableUnicornCore.BaseSet {
    public class Yay : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return Card
                .Name("Yay")
                .CardType(ECardType.Upgrade)
                .Text("Cards you play cannot be Neigh'd.")
                .ContinuousFactory((Card owningCard) => new PlayerCardsCantBeNeighd(owningCard))
                ;
        }
    }
}
