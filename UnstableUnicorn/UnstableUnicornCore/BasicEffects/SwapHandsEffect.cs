using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace UnstableUnicornCore.BasicEffects {
    public class SwapHandsEffect : AEffect {
        APlayer? targetPlayerOfSwap;
        public SwapHandsEffect(Card owningCard) : base(owningCard, 0) {
            TargetLocation = CardLocation.InHand;
        }

        public override void ChooseTargets(GameController gameController) {
            var players = OwningPlayer.ChoosePlayers(1, false, this);

            if (players.Count != 1 || players[0] == OwningPlayer)
                throw new InvalidOperationException("Invalid player selection.");

            targetPlayerOfSwap = players[0];
        }

        public override void InvokeEffect(GameController gameController) {
            if (targetPlayerOfSwap == null)
                throw new InvalidOperationException($"Not called {nameof(ChooseTargets)}");

            var opponentsCards = new List<Card>(targetPlayerOfSwap.Hand);
            var playersHand = new List<Card>(OwningPlayer.Hand);

            gameController.CardVisibilityTracker.SwapPlayerKnownCards(OwningPlayer, targetPlayerOfSwap);

            foreach (var card in opponentsCards) {
                card.MoveCard(gameController, OwningPlayer, TargetLocation);
                gameController.CardVisibilityTracker.AddSeenCardToPlayerKnowledge(targetPlayerOfSwap, OwningPlayer, card);
            }

            foreach (var card in playersHand) {
                card.MoveCard(gameController, targetPlayerOfSwap, TargetLocation);
                gameController.CardVisibilityTracker.AddSeenCardToPlayerKnowledge(OwningPlayer, targetPlayerOfSwap, card);
            }
        }

        public override bool MeetsRequirementsToPlayInner(GameController gameController) => true;
    }
}
