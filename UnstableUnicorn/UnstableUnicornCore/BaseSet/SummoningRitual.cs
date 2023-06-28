/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
﻿using System.Collections.Generic;
using UnstableUnicornCore.BasicEffects;

namespace UnstableUnicornCore.BaseSet {
    public class SummoningRitual : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return EmptyCard
                .Name("Summoning Ritual")
                .CardType(ECardType.Upgrade)
                .Text("If this card is in your Stable at the beginning of your turn, you may DISCARD 2 Unicorn cards. If you do, bring a Unicorn card directly from the discard pile into your Stable.")
                .TriggerEffect(
                    TriggerPredicates.IsItInYourStableAtTheBeginningOfYourTurn,
                    new List<ETriggerSource> { ETriggerSource.BeginningTurn },
                    (Card owningCard) => new ActivatableEffect(owningCard,
                        new ConditionalEffect(owningCard,
                            new DiscardEffect(owningCard, 2, ECardTypeUtils.UnicornTarget, PlayerTargeting.PlayerOwner),
                            new BringCardFromSourceOnTable(owningCard, 1, card => ECardTypeUtils.UnicornTarget.Contains(card.CardType), CardLocation.DiscardPile)
                        )
                    )
                );
        }
    }
}
