/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
﻿using System.Collections.Generic;
using System.Diagnostics;

namespace UnstableUnicornCore.BasicEffects {
    public class StealReturnCardOnEndOfTurn : StealEffect {
        public StealReturnCardOnEndOfTurn(Card owningCard, int cardCount, List<ECardType> targetType) : base(owningCard, cardCount, targetType) { }

        public override void InvokeEffect(GameController gameController) {
            foreach (var card in CardTargets) {
                Debug.Assert(card.Player != null);
                var previousOwnerIndex = card.Player.PlayerIndex;
                int cardIndex = card.CardIndex(gameController);

                card.MoveCard(gameController, TargetOwner, TargetLocation);

                new TriggerEffect(
                    card,
                    (_, _, _, _) => true,
                    new List<ETriggerSource> { ETriggerSource.EndTurn },
                    (Card _, GameController controller) => new MoveCardBackToPreviousStable(
                        controller._allCards[cardIndex],
                        controller.Players[previousOwnerIndex]
                    ),
                    oneTimeUseEffect: true
                ).SubscribeToEvent(gameController);
            }
        }
    }
}
