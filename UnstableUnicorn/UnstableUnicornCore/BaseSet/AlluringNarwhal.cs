using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnstableUnicornCore.BaseSet {
    public class AlluringNarwhal : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return Card
                .Name("Alluring Narwhal")
                .CardType(ECardType.MagicUnicorn)
                .Text("When this card enters your Stable, you may STEAL an Upgrade card.")
                .TriggerEffect((Card owningCard) =>
                    new TriggerEffect(owningCard,
                        (AEffect? affect, Card? card) => owningCard == card,
                        new List<ETriggerSource>() { ETriggerSource.CardEnteredStable },
                        (Card _) => new StealEffect(owningCard, 1, new List<ECardType>{ ECardType.Upgrade })
                    )
                );
        }
    }
}
