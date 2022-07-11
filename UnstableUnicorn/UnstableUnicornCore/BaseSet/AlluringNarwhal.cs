using System.Collections.Generic;

namespace UnstableUnicornCore.BaseSet {
    public class AlluringNarwhal : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return Card
                .Name("Alluring Narwhal")
                .CardType(ECardType.MagicUnicorn)
                .Text("When this card enters your Stable, you may STEAL an Upgrade card.")
                .TriggerEffect(
                    TriggerPredicates.WhenThisCardEntersYourStable,
                    new List<ETriggerSource>() { ETriggerSource.CardEnteredStable },
                    (Card owningCard) => new StealEffect(owningCard, 1, new List<ECardType>{ ECardType.Upgrade })
                );
        }
    }
}
