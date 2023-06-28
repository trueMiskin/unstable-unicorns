/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
﻿using System.Collections.Generic;
using UnstableUnicornCore.BasicEffects;

namespace UnstableUnicornCore.BaseSet {
    public class TargetedDestruction : CardTemplateSource {
        /// <summary>
        /// Same as Chainsaw Unicorn
        /// -> second edition text used
        /// </summary>
        /// <returns></returns>
        public override CardTemplate GetCardTemplate() {
            return EmptyCard
                .Name("Targeted Destruction")
                .CardType(ECardType.Spell)
                .Text("DESTROY an Upgrade card or SACRIFICE a Downgrade card.")
                .Cast((Card owningCard) => new ActivatableEffect(owningCard,
                   new ChooseEffect(owningCard,
                       new List<AEffect> {
                            new DestroyEffect(owningCard, 1, new List<ECardType>{ECardType.Upgrade}),
                            new SacrificeEffect(owningCard, 1, new List<ECardType>{ECardType.Downgrade})
                       }
                   ))
                );
        }
    }
}
