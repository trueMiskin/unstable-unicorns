/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
﻿using UnstableUnicornCore.BasicContinuousEffects;

namespace UnstableUnicornCore.BaseSet {
    public class QueenBeeUnicorn : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return EmptyCard
                .Name("Queen Bee Unicorn")
                .CardType(ECardType.MagicUnicorn)
                .Text("Basic Unicorn cards cannot enter any other player's Stable.")
                .ContinuousFactory((Card owning) =>
                    new AnyOtherPlayerCantPlayBasicUnicorn(owning)
                );
        }
    }
}
