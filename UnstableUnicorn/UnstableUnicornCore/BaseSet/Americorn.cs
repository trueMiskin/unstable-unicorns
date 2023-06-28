/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnstableUnicornCore.BasicEffects;

namespace UnstableUnicornCore.BaseSet {
    public class Americorn : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return EmptyCard
                .Name("Americorn")
                .CardType(ECardType.MagicUnicorn)
                .Text("When this card enters your Stable, choose any player. Pull a card from that player's hand.")
                .Cast(
                    (Card owningCard) => new PullOpponentsCardEffect(owningCard, cardCount: 1, numberSelectedPlayers: 1)
                );
        }
    }
}
