namespace UnstableUnicornCore.BasicEffects {
    public sealed class ShuffleDeckEffect : AEffect {
        public ShuffleDeckEffect(Card owningCard) : base(owningCard, 0) {}

        public override void ChooseTargets(GameController gameController) {}

        public override void InvokeEffect(GameController gameController) => gameController.ShuffleDeck();

        public override bool MeetsRequirementsToPlayInner(GameController gameController) => true;
    }
}
