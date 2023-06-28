/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
﻿using System.Collections.Generic;
using UnstableUnicornCore.BasicEffects;

namespace UnstableUnicornCore.BaseSet {
    public class SadisticRitual : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return EmptyCard
                .Name("Sadistic Ritual")
                .CardType(ECardType.Downgrade)
                .Text("If this card is in your Stable at the beginning of your turn, SACRIFICE a Unicorn card, then DRAW a card.")
                .TriggerEffect(
                    TriggerPredicates.IsItInYourStableAtTheBeginningOfYourTurn,
                    new List<ETriggerSource> { ETriggerSource.BeginningTurn },
                    (Card owningCard) =>
                        new SacrificeThenDrawEffect(owningCard, 1, ECardTypeUtils.UnicornTarget, 1)
                );
        }
    }
}
