/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
﻿using UnstableUnicornCore.BasicEffects;

namespace UnstableUnicornCore.BaseSet {
    public class UnicornShrinkray : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return EmptyCard
                .Name("Unicorn Shrinkray")
                .CardType(ECardType.Spell)
                .Text("Choose any player. Move all of that player's Unicorn cards to the discard pile without triggering any of their effects, then bring the same number of Baby Unicorn cards from the Nursery directly into that player's Stable.")
                .Cast(
                    (Card owningCard) => new SwapUnicornsForBabyUnicorns(owningCard)
                );
        }
    }
}
