using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnstableUnicornCore.BasicEffects;

namespace UnstableUnicornCore.BaseSet {
    public class BlackKnightUnicorn : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return Card
                .Name("Black Knight Unicorn")
                .CardType(ECardType.MagicUnicorn)
                .Text("If 1 of your Unicorn cards would be destroyed, you may SACRIFICE this card instead.")
                .TriggerEffect(
                    TriggerPredicates.IfUnicornInYourStableWouldBeDestroyd,
                    new List<ETriggerSource> { ETriggerSource.ChangeTargeting },
                    (Card owningCard) => new SacrificeThisCardInsteadOtherCard(owningCard, ECardTypeUtils.UnicornTarget)
                );
        }
    }
}
