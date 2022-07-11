using System;

namespace UnstableUnicornCore.BasicEffects {
    /// <summary>
    /// Specialized effect of return effect
    /// 
    /// This effect get a card which should be return to owner's hand
    /// Used for this effect as safe redirection from discard pile back
    /// to owner's hand
    /// </summary>
    public class ReturnThisCardToHand : ReturnEffect {
        public ReturnThisCardToHand(Card owningCard) : base(owningCard, 0, ECardTypeUtils.CardTarget) {
            if (owningCard.Player == null)
                throw new InvalidOperationException("Card should not have null player");
            
            TargetOwner = owningCard.Player;
            TargetLocation = CardLocation.InHand;
        }

        public override void ChooseTargets(GameController gameController) {
            /* No selection required */
        }

        public override void InvokeEffect(GameController gameController) {
            OwningCard.MoveCard(gameController, TargetOwner, TargetLocation);
        }

        public override bool MeetsRequirementsToPlayInner(GameController gameController) => true;
    }
}
