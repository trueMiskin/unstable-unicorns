/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
﻿using System.Collections.Generic;

namespace UnstableUnicornCore.BasicEffects {
    /// <summary>
    /// If at least one card is not sacrificed then no card is drawn
    /// </summary>
    public class SacrificeThenDrawEffect : SacrificeEffect {
        int _numCardsToDraw;
        public SacrificeThenDrawEffect(Card owningCard,
                                       int cardCount,
                                       List<ECardType> targetType,
                                       int numCardsToDraw
            ) : base(owningCard, cardCount, targetType, PlayerTargeting.PlayerOwner) {
            _numCardsToDraw = numCardsToDraw;
        }

        public override void InvokeEffect(GameController gameController) {
            base.InvokeEffect(gameController);

            if (CardTargets.Count > 0) {
                for (int i = 0; i < _numCardsToDraw; i++) {
                    gameController.PlayerDrawCard(OwningPlayer);
                }
            }
        }
    }
}
