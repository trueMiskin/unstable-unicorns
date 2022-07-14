using System;
using System.Collections.Generic;

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

            foreach (var card in opponentsCards)
                card.MoveCard(gameController, OwningPlayer, TargetLocation);

            foreach (var card in playersHand)
                card.MoveCard(gameController, targetPlayerOfSwap, TargetLocation);
        }

        public override bool MeetsRequirementsToPlayInner(GameController gameController) => true;
    }
}
