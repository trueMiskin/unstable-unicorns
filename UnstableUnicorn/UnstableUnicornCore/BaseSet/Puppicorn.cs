/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
﻿using System.Collections.Generic;
using UnstableUnicornCore.BasicEffects;

namespace UnstableUnicornCore.BaseSet {
    public class Puppicorn : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return EmptyCard
                .Name("Puppicorn")
                .CardType(ECardType.MagicUnicorn)
                .Text("Each time any player begins their turn, move this card to that player's Stable. This card cannot be sacrificed or destroyed.")
                .TriggerEffect(
                    (_, _, _, _) => true,
                    new List<ETriggerSource> { ETriggerSource.BeginningTurn },
                    (Card owningCard) => new MoveCardToActivePlayerStable(owningCard)
                )
                .CantBeSacrificed()
                .CantBeDestroyed();
        }
    }
}
