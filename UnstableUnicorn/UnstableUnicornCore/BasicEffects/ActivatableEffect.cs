namespace UnstableUnicornCore.BasicEffects {
    public sealed class ActivatableEffect : AEffect {
        private Card.FactoryEffect _effectFactory;

        public ActivatableEffect(Card owningCard, Card.FactoryEffect effectFactory) : base(owningCard, 0) {
            _effectFactory = effectFactory;
        }

        public override void ChooseTargets(GameController gameController) {
            AEffect effect = _effectFactory(OwningCard);
            if (OwningPlayer.ActivateEffect(effect)) {
                // choosing targets of added effect will be called too
                gameController.AddEffectToActualChainLink(effect);
            }
        }

        public override void InvokeEffect(GameController gameController) {}

        public override bool MeetsRequirementsToPlayInner(GameController gameController) => true;
    }
}
