/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnstableUnicornCore.BasicEffects;

namespace UnstableUnicornCore.BaseSet {
    public class MysticalVortex : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return EmptyCard
                .Name("Mystical Vortex")
                .CardType(ECardType.Spell)
                .Text("Each player must DISCARD a card. Shuffle the discard pile into the deck.")
                .Cast((Card owningCard) => new AndEffect(owningCard,
                        new DiscardEffect(owningCard, 1, ECardTypeUtils.CardTarget, PlayerTargeting.EachPlayer),
                        new ShuffleDeckEffect(owningCard, addDiscardPileToPile: true)
                    )
                );
        }
    }
}
