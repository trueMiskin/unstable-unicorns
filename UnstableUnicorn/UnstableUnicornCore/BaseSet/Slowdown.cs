/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
﻿using UnstableUnicornCore.BasicContinuousEffects;

namespace UnstableUnicornCore.BaseSet {
    public class Slowdown : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return EmptyCard
                .Name("Slowdown")
                .CardType(ECardType.Downgrade)
                .Text("You cannot play Instant cards.")
                .ContinuousFactory(
                    (Card owningCard) => new PlayerCantPlayInstantCards(owningCard)
                );
        }
    }
}
