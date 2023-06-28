/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
﻿using UnstableUnicornCore.BasicEffects;

namespace UnstableUnicornCore.BaseSet {
    public class UnfairBargain : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return EmptyCard
                .Name("Unfair Bargain")
                .CardType(ECardType.Spell)
                .Text("Trade hands with any other player.")
                .Cast((Card owningCard) => new SwapHandsEffect(owningCard)
                );
        }
    }
}
