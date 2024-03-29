/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
﻿using System.Collections.Generic;
using UnstableUnicornCore.BasicEffects;

namespace UnstableUnicornCore.BaseSet {
    public class ExtremelyFertileUnicorn : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return EmptyCard
                .Name("Extremely Fertile Unicorn")
                .CardType(ECardType.MagicUnicorn)
                .Text("If this card is in your Stable at the beginning of your turn, you may DISCARD a card. If you do, bring a Baby Unicorn card from the Nursery directly into your Stable.")
                .TriggerEffect(
                    TriggerPredicates.IsItInYourStableAtTheBeginningOfYourTurn,
                    new List<ETriggerSource> { ETriggerSource.BeginningTurn },
                    (Card owningCard) => new ActivatableEffect(owningCard,
                        new ConditionalEffect(owningCard,
                            new DiscardEffect(owningCard, 1, ECardTypeUtils.CardTarget, PlayerTargeting.PlayerOwner),
                            new BringCardFromSourceOnTable(owningCard, 1, card => card.CardType == ECardType.BabyUnicorn, CardLocation.Nursery)
                        )
                    )
                );
        }
    }
}
