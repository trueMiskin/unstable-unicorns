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
    public class ChangeOfLuck : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return EmptyCard
                .Name("Change of Luck")
                .CardType(ECardType.Spell)
                .Text("DRAW 2 cards and DISCARD 3 cards, then take another turn.")
                .Cast((Card owningCard) => {
                    return new ConditionalEffect(owningCard,
                        new AndEffect(owningCard,
                            new DiscardEffect(owningCard, 3, ECardTypeUtils.CardTarget, PlayerTargeting.PlayerOwner),
                            new DrawEffect(owningCard, 2)
                        ),
                        new ExtraTurnEffect(owningCard)
                        );
                    }
                );
        }
    }
}
