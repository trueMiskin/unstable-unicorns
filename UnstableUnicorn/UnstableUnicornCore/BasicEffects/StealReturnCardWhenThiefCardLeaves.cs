/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
﻿using System.Collections.Generic;
using System.Diagnostics;

namespace UnstableUnicornCore.BasicEffects {
    class StealReturnCardWhenThiefCardLeaves : StealEffect {
        public StealReturnCardWhenThiefCardLeaves(Card owningCard, int cardCount, List<ECardType> targetType) : base(owningCard, cardCount, targetType) {}
        
        public override void InvokeEffect(GameController gameController) {
            foreach (var card in CardTargets) {
                Debug.Assert(card.Player != null);
                var previousOwnerIndex = card.Player.PlayerIndex;
                int cardIndex = card.CardIndex(gameController);

                card.MoveCard(gameController, TargetOwner, TargetLocation);

                new TriggerEffect(
                    card,
                    (AEffect? effect, Card? causedCard, Card owningCard, GameController controller) => causedCard == OwningCard,
                    new List<ETriggerSource> { ETriggerSource.PreCardLeftStable },
                    (Card _, GameController controller) => new MoveCardBackToPreviousStable(
                        controller._allCards[cardIndex],
                        controller.Players[previousOwnerIndex]
                    ),
                    oneTimeUseEffect: true,
                    executeEffectInActualChainLink: true
                ).SubscribeToEvent(gameController);
            }
        }
    }
}
