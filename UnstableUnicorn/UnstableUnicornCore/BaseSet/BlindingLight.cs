/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
﻿using UnstableUnicornCore.BasicContinuousEffects;

namespace UnstableUnicornCore.BaseSet {
    public class BlindingLight : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return EmptyCard
                .Name("Blinding Light")
                .CardType(ECardType.Downgrade)
                .Text("Triggered effects of your Unicorn cards do not activate.")
                .ContinuousFactory((Card owningCard) => new UnicornTriggerEffectsCantBeActivated(owningCard));
        }
    }
}
