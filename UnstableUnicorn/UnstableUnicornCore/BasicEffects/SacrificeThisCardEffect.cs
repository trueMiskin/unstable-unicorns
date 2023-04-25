using System.Collections.Generic;

namespace UnstableUnicornCore.BasicEffects {
    public sealed class SacrificeThisCardEffect : SacrificeEffect {
        public SacrificeThisCardEffect(Card owningCard) : base(owningCard, 1, ECardTypeUtils.CardTarget) {}

        public override void ChooseTargets(GameController gameController) {
            CardTargets.Add(OwningCard);
            CheckAndUpdateSelectionInActualLink(new List<Card>(), CardTargets, gameController);
        }
    }
}
