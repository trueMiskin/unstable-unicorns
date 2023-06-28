/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
﻿using UnstableUnicornCore.BasicEffects;

namespace UnstableUnicornCore.BaseSet {
    public class SuperNeigh : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return EmptyCard
                .Name("Super Neigh")
                .CardType(ECardType.Instant)
                .Text("Play this card when any other player tries to play a card. Stop that player's card from being played and send it to the discard pile. This card cannot be Neigh'd.")
                .CantBeNeigh()
                .Cast(
                    (Card owningCard) => new NeighEffect(owningCard)
                );
        }
    }
}
