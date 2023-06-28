/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
﻿using UnstableUnicornCore.BasicContinuousEffects;

namespace UnstableUnicornCore.BaseSet {
    /// <summary>
    /// Used 2nd edition effect
    /// </summary>
    public class MagicalKittencorn : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return EmptyCard
                .Name("Magical Kittencorn")
                .CardType(ECardType.MagicUnicorn)
                .Text("This card cannot be destroyed by Magic cards.")
                .ContinuousFactory((Card owningCard) => new CardCannotBeDestroyedByMagicCard(owningCard))
                ;
        }
    }
}
