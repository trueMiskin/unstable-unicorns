using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnstableUnicornCore.BaseSet {
    public class NarwhalTorpedo : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return Card
                .Name("Narwhal Torpedo")
                .CardType(ECardType.MagicUnicorn)
                .Text("When this card enters your Stable, SACRIFICE all Downgrade cards.")
                .TriggerEffect((Card owningCard) =>
                    new TriggerEffect(
                        owningCard,
                        (AEffect? affect, Card? card) => owningCard == card,
                        new List<ETriggerSource>() { ETriggerSource.CardEnteredStable },
                        (Card _, GameController gameController) => new SacrificeEffect(owningCard, Int32.MaxValue, new List<ECardType>() { ECardType.Downgrade })
                ));
        }
    }
}
