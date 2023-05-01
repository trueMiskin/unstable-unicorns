using UnstableUnicornCore.BasicEffects;

namespace UnstableUnicornCore.BaseSet {
    public class Llamacorn : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return EmptyCard
                .Name("Llamacorn")
                .CardType(ECardType.MagicUnicorn)
                .Text("When this card enters your Stable, each player must DISCARD a card. Shuffle the discard pile back into the deck.")
                .Cast((Card owningCard) => new AndEffect(owningCard,
                        new DiscardEffect(owningCard, 1, ECardTypeUtils.CardTarget, PlayerTargeting.EachPlayer),
                        new ShuffleDeckEffect(owningCard, addDiscardPileToPile: true)
                    )
                );
        }
    }
}
