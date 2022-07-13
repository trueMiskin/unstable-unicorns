using System;
using System.Collections.Generic;

namespace UnstableUnicornCore.BasicEffects {
    public class ChooseEffect : AEffect {
        List<Card.FactoryEffect> effectVariants;
        public ChooseEffect(Card owningCard, List<Card.FactoryEffect> effectVariants) : base(owningCard, 0) {
            this.effectVariants = effectVariants;
        }

        public override void ChooseTargets(GameController gameController) {
            List<AEffect> effects = new();
            foreach (var factory in effectVariants)
                effects.Add(factory(OwningCard));

            AEffect effect = OwningPlayer.WhichEffectToSelect(effects);

            if (!effects.Contains(effect))
                throw new InvalidOperationException("Selected unknown effect");

            gameController.AddEffectToActualChainLink(effect);
        }

        public override void InvokeEffect(GameController gameController) { }

        public override bool MeetsRequirementsToPlayInner(GameController gameController) => true;
    }
}
