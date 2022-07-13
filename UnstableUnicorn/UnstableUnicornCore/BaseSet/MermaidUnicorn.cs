using UnstableUnicornCore.BasicEffects;

namespace UnstableUnicornCore.BaseSet {
    public class MermaidUnicorn : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return Card
                .Name("Mermaid Unicorn")
                .CardType(ECardType.MagicUnicorn)
                .Text("When this card enters your Stable, you may choose any player. Return a card in that player's Stable to their hand.")
                .Cast((Card owningCard) => new ActivatableEffect(owningCard,
                        (_) => new ReturnEffect(owningCard, 1, ECardTypeUtils.CardTarget)
                    )
                );
        }
    }
}
