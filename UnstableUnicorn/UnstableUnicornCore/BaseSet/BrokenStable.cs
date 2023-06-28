/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
﻿using UnstableUnicornCore.BasicContinuousEffects;

namespace UnstableUnicornCore.BaseSet {
    public class BrokenStable : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return EmptyCard
                .Name("Broken Stable")
                .CardType(ECardType.Downgrade)
                .Text("You cannot play Upgrade cards.")
                .ContinuousFactory((Card owningCard) => new PlayerCantPlayUpgrades(owningCard));
        }
    }
}
