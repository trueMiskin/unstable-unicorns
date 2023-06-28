/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
﻿using UnstableUnicornCore.BasicEffects;

namespace UnstableUnicornCore.BaseSet {
    public class SharkWithAHorn : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return EmptyCard
                .Name("Shark With A Horn")
                .CardType(ECardType.MagicUnicorn)
                .Text("When this card enters your Stable, you may SACRIFICE this card. If you do, DESTROY a Unicorn card.")
                .Cast((Card owningCard) => new ActivatableEffect(owningCard,
                        new ConditionalEffect(owningCard,
                            new SacrificeThisCardEffect(owningCard),
                            new DestroyEffect(owningCard, 1, ECardTypeUtils.UnicornTarget)
                        )
                    )
                );
        }
    }
}
