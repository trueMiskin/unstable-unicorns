using System;
using System.Collections.Generic;
using System.Linq;
using UnstableUnicornCore.BasicEffects;

namespace UnstableUnicornCore.BaseSet {
    /// <summary>
    /// Used original (first print) version because the second print changed
    /// meaning of effect that you can't move card into stable without basic unicorn.
    /// But what if was applied steal and return effect and during returning the basic
    /// unicorn is not there??
    /// </summary>
    public class RainbowMane : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return Card
                .Name("Rainbow Mane")
                .CardType(ECardType.Upgrade)
                .Text("You must have a Basic Unicorn in your Stable in order to play this card. If this card is in your Stable at the beginning of your turn, you may bring a Basic Unicorn from your hand directly into your Stable.")
                .RequiredBasicUnicornInStableToPlay()
                .TriggerEffect(
                    TriggerPredicates.IsItInYourStableAtTheBeginningOfYourTurn,
                    new List<ETriggerSource> { ETriggerSource.BeginningTurn },
                    (Card owningCard) => new ActivatableEffect(owningCard,
                        new BringCardFromHandOnTable(owningCard, 1, card => card.CardType == ECardType.BasicUnicorn)
                    )
                );
        }
    }
}
