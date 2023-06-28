/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
﻿using UnstableUnicornCore.BasicEffects;

namespace UnstableUnicornCore.BaseSet {
    public class RainbowUnicorn : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return EmptyCard
                .Name("Rainbow Unicorn")
                .CardType(ECardType.MagicUnicorn)
                .Text("When this card enters your Stable, you may bring a Basic Unicorn card from your hand directly into your Stable.")
                .Cast(
                    (Card owningCard) => new ActivatableEffect(owningCard,
                        new BringCardFromSourceOnTable(owningCard, 1, card => card.CardType == ECardType.BasicUnicorn)
                    )
                );
        }
    }
}
