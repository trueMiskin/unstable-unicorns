namespace UnstableUnicornCore.BasicEffects {
    public sealed class ConditionalEffect : AEffect {
        private AEffect _condition;
        private AEffect _thenEffect;
        public ConditionalEffect(Card owningCard, AEffect condition, AEffect thenEffect)
            : base(owningCard, 0 /* For this effect is not needed*/) {
            _condition = condition;
            _thenEffect = thenEffect;
        }

        public override void ChooseTargets(GameController gameController) {
            gameController.AddEffectToActualChainLink(_condition);
        }

        public override void InvokeEffect(GameController gameController) {
            gameController.AddNewEffectToChainLink(_thenEffect);
        }

        public override bool MeetsRequirementsToPlay(GameController gameController) {
            return _condition.MeetsRequirementsToPlayInner(gameController);
        }

        public override bool MeetsRequirementsToPlayInner(GameController gameController) => MeetsRequirementsToPlay(gameController);
    }
}
