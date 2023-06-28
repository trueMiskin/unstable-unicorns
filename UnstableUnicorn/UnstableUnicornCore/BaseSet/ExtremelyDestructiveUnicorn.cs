/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
﻿using UnstableUnicornCore.BasicEffects;

namespace UnstableUnicornCore.BaseSet {
    public class ExtremelyDestructiveUnicorn : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return EmptyCard
                .Name("Extremely Destructive Unicorn")
                .CardType(ECardType.MagicUnicorn)
                .Text("When this card enters your Stable, each player must SACRIFICE a Unicorn card.")
                .Cast(
                    (owningCard) => new SacrificeEffect(owningCard, 1, ECardTypeUtils.UnicornTarget, PlayerTargeting.EachPlayer)
                );
        }
    }
}
