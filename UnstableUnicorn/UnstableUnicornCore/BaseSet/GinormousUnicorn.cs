/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
﻿using UnstableUnicornCore.BasicContinuousEffects;

namespace UnstableUnicornCore.BaseSet {
    public class GinormousUnicorn : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return EmptyCard
                .Name("Ginormous Unicorn")
                .CardType(ECardType.MagicUnicorn)
                .Text("This card counts for 2 Unicorns. You cannot play any Instant cards.")
                .ExtraUnicornValue(1)
                .ContinuousFactory((Card owningCard) => new PlayerCantPlayInstantCards(owningCard))
                ;
        }
    }
}
