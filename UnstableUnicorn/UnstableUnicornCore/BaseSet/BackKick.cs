/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
﻿using UnstableUnicornCore.BasicEffects;

namespace UnstableUnicornCore.BaseSet {
    public class BackKick : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return EmptyCard
                .Name("Back Kick")
                .CardType(ECardType.Spell)
                .Text("Choose any player. Return a card in that player's Stable to their hand. That player must DISCARD a card.")
                .Cast((Card owningCard) => new ReturnThenDiscardCard(owningCard, ECardTypeUtils.CardTarget));
        }
    }
}
