/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
﻿using System.Collections.Generic;
using UnstableUnicornCore.BasicEffects;

namespace UnstableUnicornCore.BaseSet {
    public class GlitterBomb : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return EmptyCard
                .Name("Glitter Bomb")
                .CardType(ECardType.Upgrade)
                .Text("If this card is in your Stable at the beginning of your turn, you may SACRIFICE a card. If you do, DESTROY a card.")
                // this doesn't need to be in conditional effect - there is always target to sacrifice -> upgrade itself
                .TriggerEffect(
                    TriggerPredicates.IsItInYourStableAtTheBeginningOfYourTurn,
                    new List<ETriggerSource> { ETriggerSource.BeginningTurn },
                    (Card owningCard) => new ActivatableEffect(owningCard,
                        new ConditionalEffect(owningCard,
                            new SacrificeEffect(owningCard, 1, ECardTypeUtils.CardTarget),
                            new DestroyEffect(owningCard, 1, ECardTypeUtils.CardTarget)
                        )
                    )
                );
        }
    }
}
