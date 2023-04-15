using UnstableUnicornCore.BasicContinuousEffects;

namespace UnstableUnicornCore.BaseSet {
    public class RainbowAura : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return EmptyCard
                .Name("Rainbow Aura")
                .CardType(ECardType.Upgrade)
                .Text("Your Unicorn cards cannot be destroyed.")
                .ContinuousFactory(
                    (Card owningCard) => new PlayerCardsCantBeDestroyed(owningCard, ECardTypeUtils.UnicornTarget)
                );
        }
    }
}
