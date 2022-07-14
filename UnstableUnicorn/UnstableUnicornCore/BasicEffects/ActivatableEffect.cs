namespace UnstableUnicornCore.BasicEffects {
    public sealed class ActivatableEffect : AEffect {
        private AEffect _effect;

        public ActivatableEffect(Card owningCard, AEffect effect) : base(owningCard, 0) {
            _effect = effect;
        }

        public override void ChooseTargets(GameController gameController) {
            if (OwningPlayer.ActivateEffect(_effect)) {
                // choosing targets of added effect will be called too
                gameController.AddEffectToActualChainLink(_effect);
            }
        }

        public override void InvokeEffect(GameController gameController) {}

        public override bool MeetsRequirementsToPlayInner(GameController gameController) => true;
    }
}
