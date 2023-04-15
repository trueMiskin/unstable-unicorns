using UnstableUnicornCore.BasicContinuousEffects;

namespace UnstableUnicornCore.BaseSet {
    public class QueenBeeUnicorn : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return EmptyCard
                .Name("Queen Bee Unicorn")
                .CardType(ECardType.MagicUnicorn)
                .Text("Basic Unicorn cards cannot enter any other player's Stable.")
                .ContinuousFactory((Card owning) =>
                    new AnyOtherPlayerCantPlayBasicUnicorn(owning)
                );
        }
    }
}
