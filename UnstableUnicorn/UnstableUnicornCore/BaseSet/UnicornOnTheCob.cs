/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
﻿using System.Collections.Generic;
using UnstableUnicornCore.BasicEffects;

namespace UnstableUnicornCore.BaseSet {
    public class UnicornOnTheCob : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return EmptyCard
                .Name("Unicorn On The Cob")
                .CardType(ECardType.MagicUnicorn)
                .Text("When this card enters your Stable, DRAW 2 cards and DISCARD a card.")
                .TriggerEffect(
                    TriggerPredicates.WhenThisCardEntersYourStable,
                    new List<ETriggerSource> { ETriggerSource.CardEnteredStable },
                    (Card owningCard) => new AndEffect(owningCard,
                        new DrawEffect(owningCard, 2),
                        new DiscardEffect(owningCard, 1, ECardTypeUtils.CardTarget, PlayerTargeting.PlayerOwner)
                    )
                );
        }
    }
}
