﻿using System.Collections.Generic;

namespace UnstableUnicornCore.BasicEffects {
    public class StealReturnCardOnEndOfTurn : StealEffect {
        public StealReturnCardOnEndOfTurn(Card owningCard, int cardCount, List<ECardType> targetType) : base(owningCard, cardCount, targetType) { }

        public override void InvokeEffect(GameController gameController) {
            foreach (var card in CardTargets) {
                var previousOwner = card.Player;
                card.MoveCard(gameController, TargetOwner, TargetLocation);
                new TriggerEffect(
                    card,
                    (_, _, _, _) => true,
                    new List<ETriggerSource> { ETriggerSource.EndTurn },
                    (Card _) => new MoveCardBackToPreviousStable(card, previousOwner),
                    oneTimeUseEffect: true
                ).SubscribeToEvent(gameController);
            }
        }
    }
}
