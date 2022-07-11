using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnstableUnicornCore.BasicEffects;

namespace UnstableUnicornCore.BaseSet {
    public class UnicornLasso : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return Card
                .Name("Unicorn Lasso")
                .CardType(ECardType.Upgrade)
                .Text("If this card is in your Stable at the beginning of your turn, you may STEAL a Unicorn card. At the end of your turn, return that Unicorn card to the Stable from which you stole it.")
                .TriggerEffect(
                    TriggerPredicates.IsItInYourStableAtTheBeginningOfYourTurn,
                    new List<ETriggerSource> { ETriggerSource.BeginningTurn },
                    (Card owningCard) => new ActivatableEffect(
                        owningCard,
                        (Card _) => new StealReturnCardOnEndOfTurn(owningCard, 1, ECardTypeUtils.UnicornTarget)
                    )
                );
        }
    }
}
