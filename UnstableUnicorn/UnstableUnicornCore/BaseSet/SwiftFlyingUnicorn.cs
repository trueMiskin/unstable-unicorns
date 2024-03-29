/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
﻿using System.Collections.Generic;
using UnstableUnicornCore.BasicEffects;

namespace UnstableUnicornCore.BaseSet {
    public class SwiftFlyingUnicorn : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return EmptyCard
                .Name("Swift Flying Unicorn")
                .CardType(ECardType.MagicUnicorn)
                .Text("When this card enters your Stable, you may choose an Instant card from the discard pile and add it to your hand. If this card would be sacrificed or destroyed, return it to your hand instead.")
                .Cast((Card owningCard) => new ActivatableEffect(owningCard,
                        new SearchDeckEffect(owningCard, 1, CardLocation.DiscardPile, (Card card) => card.CardType == ECardType.Instant)
                    )
                )
                .TriggerEffect(
                    TriggerPredicates.IfThisCardWouldBeSacrificedOrDestroyed,
                    new List<ETriggerSource> { ETriggerSource.ChangeLocationOfCard },
                    (Card owningCard) => new ReturnThisCardToLocation(owningCard)
                );
        }
    }
}
