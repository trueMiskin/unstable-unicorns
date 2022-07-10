using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnstableUnicornCore.BaseSet {
    public class UnicornPoison : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return Card
                .Name("Unicorn Poison")
                .CardType(ECardType.Spell)
                .Text("DESTROY a Unicorn card.")
                .Cast((Card owningCard, GameController gameController) =>
                    new DestroyEffect(owningCard, 1, ECardTypeUtils.UnicornTarget)
                );
        }
    }
}
