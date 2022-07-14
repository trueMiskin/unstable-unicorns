namespace UnstableUnicornCore.BasicEffects {
    public sealed class ShuffleDeckEffect : AEffect {
        bool addDiscardPileToPile;
        public ShuffleDeckEffect(Card owningCard, bool addDiscardPileToPile = false) : base(owningCard, 0) {
            this.addDiscardPileToPile = addDiscardPileToPile;
        }

        public override void ChooseTargets(GameController gameController) {}

        public override void InvokeEffect(GameController gameController) {
            if (addDiscardPileToPile)
                gameController.AddDiscardPileToPile();

            gameController.ShuffleDeck();
        }

        public override bool MeetsRequirementsToPlayInner(GameController gameController) => true;
    }
}
