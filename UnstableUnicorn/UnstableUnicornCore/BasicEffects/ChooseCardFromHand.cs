using System;

namespace UnstableUnicornCore.BasicEffects {
    public class ChooseCardFromHand : AEffect {
        bool canSelectMyself;
        public ChooseCardFromHand(Card owningCard, int cardCount, bool canSelectMyself) : base(owningCard, cardCount) {
            this.canSelectMyself = canSelectMyself;
            TargetLocation = CardLocation.InHand;
            TargetOwner = OwningPlayer;
        }

        public override void ChooseTargets(GameController gameController) {
            var players = OwningPlayer.ChoosePlayers(1, canSelectMyself, this);
            if (players.Count != 1 || (players[0] == OwningPlayer && !canSelectMyself))
                throw new InvalidOperationException("Selected wrong number of players or player select itself which is disallowed");

            APlayer player = players[0];
            _cardCount = Math.Min(_cardCount, player.Hand.Count);
            CardTargets = OwningPlayer.WhichCardsToGet(_cardCount, this, player.Hand);
        }

        public override void InvokeEffect(GameController gameController) {
            foreach (var card in CardTargets)
                card.MoveCard(gameController, TargetOwner, TargetLocation);
        }

        public override bool MeetsRequirementsToPlayInner(GameController gameController) => true;
    }
}
