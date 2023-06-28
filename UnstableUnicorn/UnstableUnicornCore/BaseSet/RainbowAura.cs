/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
﻿using UnstableUnicornCore.BasicContinuousEffects;

namespace UnstableUnicornCore.BaseSet {
    public class RainbowAura : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return EmptyCard
                .Name("Rainbow Aura")
                .CardType(ECardType.Upgrade)
                .Text("Your Unicorn cards cannot be destroyed.")
                .ContinuousFactory(
                    (Card owningCard) => new PlayerCardsCantBeDestroyed(owningCard, ECardTypeUtils.UnicornTarget)
                );
        }
    }
}
