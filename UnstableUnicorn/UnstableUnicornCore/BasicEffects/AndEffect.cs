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
            for (int i = _effects.Count - 1; i >= 0; i--) {
                gameController.AddEffectAfterSelectedEffectToCurrentChainLink(_effects[i], this);
            }
        }

        public override void InvokeEffect(GameController gameController) {}

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

        public override AEffect Clone(Dictionary<Card, Card> cardMapper,
                                      Dictionary<AEffect, AEffect> effectMapper,
                                      Dictionary<APlayer, APlayer> playerMapper) {
            var newEffect = (AndEffect)base.Clone(cardMapper, effectMapper, playerMapper);

            newEffect._effects = new();
            foreach(var effect in _effects) {
                var newEff = effect.Clone(cardMapper, effectMapper, playerMapper);
                newEffect._effects.Add(newEff);
                effectMapper.Add(effect, newEff);
            }

            return newEffect;
        }
    }
}
