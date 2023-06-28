/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
﻿using System.Collections.Generic;
using UnstableUnicornCore.BasicEffects;

namespace UnstableUnicornCore.BaseSet {
    public class AngelUnicorn : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return EmptyCard
                .Name("Angel Unicorn")
                .CardType(ECardType.MagicUnicorn)
                .Text("If this card is in your Stable at the beginning of your turn, you may SACRIFICE this card. If you do, choose a Unicorn card from the discard pile and bring it directly into your Stable.")
                .TriggerEffect(
                    TriggerPredicates.IsItInYourStableAtTheBeginningOfYourTurn,
                    new List<ETriggerSource> { ETriggerSource.BeginningTurn },
                    (Card owningCard) => new ActivatableEffect(owningCard,
                        new ConditionalEffect(owningCard,
                            new SacrificeThisCardEffect(owningCard),
                            new BringCardFromSourceOnTable(owningCard, 1, card => ECardTypeUtils.UnicornTarget.Contains(card.CardType), CardLocation.DiscardPile)
                        )
                    )
                );
        }
    }
}
