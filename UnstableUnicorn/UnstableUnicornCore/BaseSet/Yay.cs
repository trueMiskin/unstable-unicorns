/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
﻿using UnstableUnicornCore.BasicContinuousEffects;

namespace UnstableUnicornCore.BaseSet {
    public class Yay : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return EmptyCard
                .Name("Yay")
                .CardType(ECardType.Upgrade)
                .Text("Cards you play cannot be Neigh'd.")
                .ContinuousFactory((Card owningCard) => new PlayerCardsCantBeNeighd(owningCard))
                ;
        }
    }
}
