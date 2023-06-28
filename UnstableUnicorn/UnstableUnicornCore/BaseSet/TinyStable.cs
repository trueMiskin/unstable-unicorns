/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
﻿using System.Collections.Generic;
using UnstableUnicornCore.BasicEffects;

namespace UnstableUnicornCore.BaseSet {
    public class TinyStable : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return EmptyCard
                .Name("Tiny Stable")
                .CardType(ECardType.Downgrade)
                .Text("If at any time you have more than 5 Unicorns in your Stable, SACRIFICE a Unicorn card.")
                .TriggerEffect(
                    TriggerPredicates.WhenYourStableHaveMoreThanFiveUnicorns,
                    // max unicorns is 7 -> end game, so this card can be played
                    // when stable have max 6 unicorns
                    new List<ETriggerSource> { ETriggerSource.CardEnteredStable },
                    (Card owningCard) => new SacrificeEffect(owningCard, 1, ECardTypeUtils.UnicornTarget)
                );
        }
    }
}
