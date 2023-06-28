/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
﻿using UnstableUnicornCore.BasicEffects;

namespace UnstableUnicornCore.BaseSet {
    public class GlitterTornado : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return EmptyCard
                .Name("Glitter Tornado")
                .CardType(ECardType.Spell)
                .Text("Return a card in each player's Stable (including yours) to their hand.")
                .Cast(
                    (Card owningCard) => new ReturnEffect(owningCard, 1, ECardTypeUtils.CardTarget, PlayerTargeting.EachPlayer)
                );
        }
    }
}
