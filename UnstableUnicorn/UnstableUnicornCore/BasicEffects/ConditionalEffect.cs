using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnstableUnicornCore.BasicEffects {
    class ConditionalEffect : AEffect {
        private AEffect _condition;
        private AEffect _thenEffect;
        public ConditionalEffect(Card owningCard, AEffect condition, AEffect thenEffect)
            : base(owningCard, 0 /* For this effect is not needed*/) {
            _condition = condition;
            _thenEffect = thenEffect;
        }

        public override void ChooseTargets(GameController gameController) {
            // choosing targets only for condition
            // the then effect will be resolved in next chain link
            _condition.ChooseTargets(gameController);
        }

        public override void InvokeEffect(ETriggerSource triggerSource, AEffect? effect, GameController gameController) {
            _condition.InvokeEffect(triggerSource, effect, gameController);
            gameController.AddNewEffectToChainLink(_thenEffect);
        }

        public override bool MeetsRequirementsToPlay(GameController gameController) {
            return _condition.MeetsRequirementsToPlayInner(gameController);
        }

        public override bool MeetsRequirementsToPlayInner(GameController gameController) => MeetsRequirementsToPlay(gameController);
    }
}
