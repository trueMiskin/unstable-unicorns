using System.Collections.Generic;

namespace UnstableUnicornCore.BasicEffects {
    public sealed class AndEffect : AEffect {
        private List<AEffect> _effects;
        public AndEffect(Card owningCard, AEffect firstEffect, AEffect secondEffect)
            : base(owningCard, 0 /* For this effect is not needed*/) {
            _effects = new List<AEffect> { firstEffect, secondEffect };
        }

        public AndEffect(Card owningCard, params AEffect[] effects)
            : base(owningCard, 0 /* For this effect is not needed*/) {
            _effects = new List<AEffect>(effects);
        }

        public override void ChooseTargets(GameController gameController) {
            foreach(var effect in _effects)
                effect.ChooseTargets(gameController);
        }

        public override void InvokeEffect(GameController gameController) {
            foreach (var effect in _effects)
                effect.InvokeEffect(gameController);
        }

        public override bool MeetsRequirementsToPlay(GameController gameController) {
            bool ret = true;
            foreach (var effect in _effects)
                ret &= effect.MeetsRequirementsToPlay(gameController);
            return ret;
        }

        public override bool MeetsRequirementsToPlayInner(GameController gameController) {
            bool ret = true;
            foreach (var effect in _effects)
                ret &= effect.MeetsRequirementsToPlayInner(gameController);
            return ret;
        }
    }
}
