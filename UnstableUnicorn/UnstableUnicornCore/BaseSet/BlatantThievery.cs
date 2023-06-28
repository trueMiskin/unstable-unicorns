/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
﻿using UnstableUnicornCore.BasicEffects;

namespace UnstableUnicornCore.BaseSet {
    public class BlatantThievery : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return EmptyCard
                .Name("Blatant Thievery")
                .CardType(ECardType.Spell)
                .Text("Choose any player and look at that player's hand. Choose a card from that player's hand and add it to your hand.")
                // the player can choose himself even if it is nonsense
                .Cast((Card owningCard) => new ChooseCardFromHand(owningCard, 1, true));
        }
    }
}
