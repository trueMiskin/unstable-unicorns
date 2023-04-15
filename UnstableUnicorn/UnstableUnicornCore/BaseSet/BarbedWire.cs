using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnstableUnicornCore.BasicEffects;

namespace UnstableUnicornCore.BaseSet {
    public class BarbedWire : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return EmptyCard
                .Name("Barbed Wire")
                .CardType(ECardType.Downgrade)
                .Text("Each time a Unicorn card enters or leaves your stable, DISCARD a card.")
                .TriggerEffect(
                    TriggerPredicates.CardEnterLeaveYourStable,
                    new List<ETriggerSource> { ETriggerSource.CardEnteredStable, ETriggerSource.CardLeftStable },
                    (Card owningCard) => new DiscardEffect(owningCard, 1, ECardTypeUtils.CardTarget, PlayerTargeting.PlayerOwner)
                );
        }
    }
}
