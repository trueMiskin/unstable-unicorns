using UnstableUnicornCore.BasicEffects;

namespace UnstableUnicornCore.BaseSet {
    public class ClassyNarwhal : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return Card
                .Name("Classy Narwhal")
                .CardType(ECardType.MagicUnicorn)
                .Text("When this card enters your Stable, you may search the deck for an Upgrade card and add it to your hand. Shuffle the deck.")
                .Cast((Card owningCard) => new AndEffect(owningCard,
                        new SearchDeckEffect(owningCard, 1, CardLocation.Pile, (Card card) => card.CardType == ECardType.Upgrade),
                        new ShuffleDeckEffect(owningCard)
                    )
                );
        }
    }
}
