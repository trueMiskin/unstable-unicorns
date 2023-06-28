/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
﻿using UnstableUnicornCore.BasicEffects;

namespace UnstableUnicornCore.BaseSet {
    public class UnicornPoison : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return EmptyCard
                .Name("Unicorn Poison")
                .CardType(ECardType.Spell)
                .Text("DESTROY a Unicorn card.")
                .Cast((Card owningCard) =>
                    new DestroyEffect(owningCard, 1, ECardTypeUtils.UnicornTarget)
                );
        }
    }
}
