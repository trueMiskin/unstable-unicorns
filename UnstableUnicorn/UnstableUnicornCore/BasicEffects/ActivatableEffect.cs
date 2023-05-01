using System.Collections.Generic;

namespace UnstableUnicornCore.BasicEffects {
    public sealed class ActivatableEffect : AEffect {
        private AEffect _effect;

        public ActivatableEffect(Card owningCard, AEffect effect) : base(owningCard, 0) {
            _effect = effect;
        }

        public override void ChooseTargets(GameController gameController) {
            // if effect cannot be executed - skip it
            if (!_effect.MeetsRequirementsToPlayInner(gameController))
                return;

            if (OwningPlayer.ActivateEffect(_effect)) {
                // choosing targets of added effect will be called too
                gameController.AddEffectAfterSelectedEffectToCurrentChainLink(_effect, this);
            }
        }

        public override void InvokeEffect(GameController gameController) {}

        public override bool MeetsRequirementsToPlayInner(GameController gameController) => _effect.MeetsRequirementsToPlayInner(gameController);

        public override void InvokeReactionEffect(GameController gameController, AEffect effect) {
            if (OwningPlayer.ActivateEffect(_effect))
                _effect.InvokeReactionEffect(gameController, effect);
        }

        public override AEffect Clone(Dictionary<Card, Card> cardMapper,
                                      Dictionary<AEffect, AEffect> effectMapper,
                                      Dictionary<APlayer, APlayer> playerMapper) {
            var newEffect = (ActivatableEffect)base.Clone(cardMapper, effectMapper, playerMapper);
            newEffect._effect = _effect.Clone(cardMapper, effectMapper, playerMapper);
            effectMapper.Add(_effect, newEffect._effect);
            
            return newEffect;
        }
    }
}
