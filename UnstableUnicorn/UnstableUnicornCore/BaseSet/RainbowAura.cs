using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnstableUnicornCore.BasicContinuousEffects;

namespace UnstableUnicornCore.BaseSet {
    public class RainbowAura : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return Card
                .Name("Rainbow Aura")
                .CardType(ECardType.Upgrade)
                .Text("Your Unicorn cards cannot be destroyed.")
                .ContinuousFactory(
                    (Card owningCard) => new PlayerCardsCantBeDestroyed(owningCard, new List<ECardType>() { ECardType.Unicorn })
                );
        }
    }
}
