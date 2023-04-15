using UnstableUnicornCore.BasicContinuousEffects;

namespace UnstableUnicornCore.BaseSet {
    public class GinormousUnicorn : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return EmptyCard
                .Name("Ginormous Unicorn")
                .CardType(ECardType.MagicUnicorn)
                .Text("This card counts for 2 Unicorns. You cannot play any Instant cards.")
                .ExtraUnicornValue(1)
                .ContinuousFactory((Card owningCard) => new PlayerCantPlayInstantCards(owningCard))
                ;
        }
    }
}
