namespace UnstableUnicornCore.BasicEffects {
    public sealed class AndEffect : AEffect {
        private AEffect _firstEffect, _secondEffect;
        public AndEffect(Card owningCard, AEffect firstEffect, AEffect secondEffect)
            : base(owningCard, 0 /* For this effect is not needed*/) {
            _firstEffect = firstEffect;
            _secondEffect = secondEffect;
        }

        public override void ChooseTargets(GameController gameController) {
            _firstEffect.ChooseTargets(gameController);
            _secondEffect.ChooseTargets(gameController);
        }

        public override void InvokeEffect(GameController gameController) {
            _firstEffect.InvokeEffect(gameController);
            _secondEffect.InvokeEffect(gameController);
        }

        public override bool MeetsRequirementsToPlay(GameController gameController) {
            return _firstEffect.MeetsRequirementsToPlay(gameController) &&
                _secondEffect.MeetsRequirementsToPlay(gameController);
        }

        public override bool MeetsRequirementsToPlayInner(GameController gameController) {
            return _firstEffect.MeetsRequirementsToPlayInner(gameController) &&
                _secondEffect.MeetsRequirementsToPlayInner(gameController);
        }
    }
}
