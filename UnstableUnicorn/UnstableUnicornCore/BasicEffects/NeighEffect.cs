using System;

namespace UnstableUnicornCore.BasicEffects {
    public class NeighEffect : AEffect {
        public NeighEffect(Card owningCard) : base(owningCard, 0) {}

        public override void ChooseTargets(GameController gameController) {
            throw new NotImplementedException("This should be never called.");
        }

        public override void InvokeEffect(GameController gameController) {
            for (int i = 0; i < 2; i++) {
                var topCard = gameController.Stack[^1];
                topCard.MoveCard(gameController, null, CardLocation.DiscardPile);
                gameController.Stack.Remove(topCard);
            }
        }

        public override bool MeetsRequirementsToPlayInner(GameController gameController) => true;
    }
}
