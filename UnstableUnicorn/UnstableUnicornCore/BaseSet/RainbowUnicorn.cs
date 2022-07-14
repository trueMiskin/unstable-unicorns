using UnstableUnicornCore.BasicEffects;

namespace UnstableUnicornCore.BaseSet {
    public class RainbowUnicorn : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return Card
                .Name("Rainbow Unicorn")
                .CardType(ECardType.MagicUnicorn)
                .Text("When this card enters your Stable, you may bring a Basic Unicorn card from your hand directly into your Stable.")
                .Cast(
                    (Card owningCard) => new ActivatableEffect(owningCard,
                        new BringCardFromHandOnTable(owningCard, 1, card => card.CardType == ECardType.BasicUnicorn)
                    )
                );
        }
    }
}
