/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
﻿using System.Collections.Generic;
using UnstableUnicornCore.BasicEffects;

namespace UnstableUnicornCore.BaseSet {
    public class ReTarget : CardTemplateSource {
        /// <summary>
        /// This card doesn't work 100% correctly when is 2 players
        /// -> edge case which doesn't matter
        /// </summary>
        /// <returns></returns>
        public override CardTemplate GetCardTemplate() {
            return EmptyCard
                .Name("Re-target")
                .CardType(ECardType.Spell)
                .Text("Move an Upgrade or Downgrade card from any player's Stable to any other player's Stable.")
                .Cast((Card owningCard) => new ChooseEffect(owningCard,
                    new List<AEffect> {
                        new MoveCardBetweenStables(owningCard, card => card.CardType == ECardType.Upgrade, false),
                        new MoveCardBetweenStables(owningCard, card => card.CardType == ECardType.Downgrade, false)
                    })
                );
        }
    }
}
