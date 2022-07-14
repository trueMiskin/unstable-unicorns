using System;
using System.Collections.Generic;

namespace UnstableUnicornCore.BasicEffects {
    public class ChooseEffect : AEffect {
        List<AEffect> effectVariants;
        public ChooseEffect(Card owningCard, List<AEffect> effectVariants) : base(owningCard, 0) {
            this.effectVariants = effectVariants;
        }

        public override void ChooseTargets(GameController gameController) {
            AEffect effect = OwningPlayer.WhichEffectToSelect(effectVariants);

            if (!effectVariants.Contains(effect))
                throw new InvalidOperationException("Selected unknown effect");

            gameController.AddEffectToActualChainLink(effect);
        }

        public override void InvokeEffect(GameController gameController) { }

        public override bool MeetsRequirementsToPlayInner(GameController gameController) {
            bool canBePlayed = false;
            foreach (var effect in effectVariants)
                canBePlayed |= effect.MeetsRequirementsToPlayInner(gameController);

            return canBePlayed;
        }
    }
}
