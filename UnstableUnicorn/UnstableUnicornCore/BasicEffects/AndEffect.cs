using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnstableUnicornCore.BasicEffects {
    public class AndEffect : AEffect {
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

        public override void InvokeEffect(ETriggerSource triggerSource, AEffect? effect, GameController gameController) {
            _firstEffect.InvokeEffect(triggerSource, effect, gameController);
            _secondEffect.InvokeEffect(triggerSource, effect, gameController);
        }

        public override bool MeetsRequirementsToPlayInner(GameController gameController) {
            return _firstEffect.MeetsRequirementsToPlayInner(gameController) &&
                _secondEffect.MeetsRequirementsToPlayInner(gameController);
        }
    }
}
