/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
﻿using UnstableUnicornCore.BasicEffects;

namespace UnstableUnicornCore.BaseSet {
    public class GoodDeal : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return EmptyCard
                .Name("Good Deal")
                .CardType(ECardType.Spell)
                .Text("DRAW 3 cards and DISCARD a card.")
                .Cast((Card owningCard) => new AndEffect(owningCard,
                        new DrawEffect(owningCard, 3),
                        new DiscardEffect(owningCard, 1, ECardTypeUtils.CardTarget, PlayerTargeting.PlayerOwner)
                    )
                );
        }
    }
}
