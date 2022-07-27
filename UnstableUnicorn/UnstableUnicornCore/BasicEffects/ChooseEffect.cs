using System;
using System.Collections.Generic;

namespace UnstableUnicornCore.BasicEffects {
    public class ChooseEffect : AEffect {
        List<AEffect> effectVariants;
        public ChooseEffect(Card owningCard, List<AEffect> effectVariants) : base(owningCard, 0) {
            this.effectVariants = effectVariants;
        }

        public List<AEffect> GetAvailableEffects(GameController gameController) {
            List<AEffect> ret = new();
            foreach (var effect in effectVariants)
                if (effect.MeetsRequirementsToPlayInner(gameController))
                    ret.Add(effect);
            return ret;
        }

        public override void ChooseTargets(GameController gameController) {
            List<AEffect> availableEffectVariants = GetAvailableEffects(gameController);
            AEffect effect = OwningPlayer.WhichEffectToSelect(availableEffectVariants);

            if (!availableEffectVariants.Contains(effect))
                throw new InvalidOperationException("Selected unknown effect");

            gameController.AddEffectToActualChainLink(effect);
        }

        public override void InvokeEffect(GameController gameController) { }

        public override bool MeetsRequirementsToPlayInner(GameController gameController) {
            return GetAvailableEffects(gameController).Count > 0;
        }
    }
}
