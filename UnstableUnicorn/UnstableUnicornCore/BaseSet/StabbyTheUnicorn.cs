/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
﻿using System.Collections.Generic;
using UnstableUnicornCore.BasicEffects;

namespace UnstableUnicornCore.BaseSet {
    public class StabbyTheUnicorn : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return EmptyCard
                .Name("Stabby The Unicorn")
                .CardType(ECardType.MagicUnicorn)
                .Text("If this card is sacrificed or destroyed, you may DESTROY a Unicorn card.")
                .TriggerEffect(
                    TriggerPredicates.IfThisCardWouldBeSacrificedOrDestroyed,
                    new List<ETriggerSource> { ETriggerSource.PreCardLeftStable },
                    (Card owningCard) => new ActivatableEffect(owningCard,
                        new DestroyEffect(owningCard, 1, ECardTypeUtils.UnicornTarget)
                    )
                );
        }
    }
}
