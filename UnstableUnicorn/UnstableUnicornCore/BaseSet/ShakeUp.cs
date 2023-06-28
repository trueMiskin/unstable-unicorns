/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
﻿using System;
using UnstableUnicornCore.BasicEffects;

namespace UnstableUnicornCore.BaseSet {
    public class ShakeUp : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return EmptyCard
                .Name("Shake Up")
                .CardType(ECardType.Spell)
                .Text("Shuffle this card, your hand, and the discard pile into the deck. DRAW 5 cards.")
                .Cast((Card owningCard) => new AndEffect(owningCard,
                        new DiscardEffect(owningCard, Int32.MaxValue, ECardTypeUtils.CardTarget, PlayerTargeting.PlayerOwner),
                        new ShuffleDeckEffect(owningCard, addDiscardPileToPile: true),
                        new DrawEffect(owningCard, 5)
                    )
                );
        }
    }
}
