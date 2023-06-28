/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
﻿using UnstableUnicornCore.BasicEffects;

namespace UnstableUnicornCore.BaseSet {
    public class MermaidUnicorn : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return EmptyCard
                .Name("Mermaid Unicorn")
                .CardType(ECardType.MagicUnicorn)
                .Text("When this card enters your Stable, you may choose any player. Return a card in that player's Stable to their hand.")
                .Cast((Card owningCard) => new ActivatableEffect(owningCard,
                        new ReturnEffect(owningCard, 1, ECardTypeUtils.CardTarget)
                    )
                );
        }
    }
}
