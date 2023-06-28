/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
﻿using UnstableUnicornCore.BasicEffects;

namespace UnstableUnicornCore.BaseSet {
    public class SeductiveUnicorn : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return EmptyCard
                .Name("Seductive Unicorn")
                .CardType(ECardType.MagicUnicorn)
                .Text("When this card enters your Stable, STEAL a Unicorn card. If this card leaves your Stable, return that Unicorn card to the Stable from which you stole it.")
                .Cast((Card owningCard) => new ActivatableEffect(owningCard,
                    new StealReturnCardWhenThiefCardLeaves(owningCard, 1, ECardTypeUtils.UnicornTarget))
                );
        }
    }
}
