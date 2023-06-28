/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
﻿using UnstableUnicornCore.BasicEffects;

namespace UnstableUnicornCore.BaseSet {
    public class UnicornSwap : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return EmptyCard
                .Name("Unicorn Swap")
                .CardType(ECardType.Spell)
                .Text("Move a Unicorn card in your Stable to any other player's Stable, then STEAL a Unicorn card from that player's Stable.")
                .Cast((Card owningCard) => new MoveUnicornCardThenStealCard(owningCard, 1))
                ;
        }
    }
}
