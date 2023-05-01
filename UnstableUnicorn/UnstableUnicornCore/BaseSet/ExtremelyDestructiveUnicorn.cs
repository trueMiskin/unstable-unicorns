using UnstableUnicornCore.BasicEffects;

namespace UnstableUnicornCore.BaseSet {
    public class ExtremelyDestructiveUnicorn : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return EmptyCard
                .Name("Extremely Destructive Unicorn")
                .CardType(ECardType.MagicUnicorn)
                .Text("When this card enters your Stable, each player must SACRIFICE a Unicorn card.")
                .Cast(
                    (owningCard) => new SacrificeEffect(owningCard, 1, ECardTypeUtils.UnicornTarget, PlayerTargeting.EachPlayer)
                );
        }
    }
}
