using System.Collections.Generic;
using UnstableUnicornCore.BasicEffects;

namespace UnstableUnicornCore.BaseSet {
    public class AlluringNarwhal : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return Card
                .Name("Alluring Narwhal")
                .CardType(ECardType.MagicUnicorn)
                .Text("When this card enters your Stable, you may STEAL an Upgrade card.")
                .Cast(
                    (Card owningCard) => new StealEffect(owningCard, 1, new List<ECardType>{ ECardType.Upgrade })
                );
        }
    }
}
