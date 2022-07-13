using System;

namespace UnstableUnicornCore.BasicEffects {
    /// <summary>
    /// Specialized effect of return effect
    /// 
    /// This effect get a card which should be return to owner's hand
    /// <br/>
    /// Since I wrote Black knight unicorn card, this effect move cards
    /// from table to hand without intermediate step moving to discard pile
    /// 
    /// This effect should be called on trigger <see cref="ETriggerSource.ChangeLocationOfCard"/>
    /// </summary>
    public class ReturnThisCardToHand : ReturnEffect {
        public ReturnThisCardToHand(Card owningCard) : base(owningCard, 0, ECardTypeUtils.CardTarget) {
            if (owningCard.Player == null)
                throw new InvalidOperationException("Card should not have null player");

            CardTargets.Add(owningCard);
            TargetOwner = owningCard.Player;
            TargetLocation = CardLocation.InHand;
        }

        public override void ChooseTargets(GameController gameController) {
            /* No selection required */
        }

        public override void InvokeReactionEffect(GameController gameController, AEffect effect) {
            effect.CardTargets.Remove(OwningCard);
            gameController.AddEffectToActualChainLink(this);
        }

        public override bool MeetsRequirementsToPlayInner(GameController gameController) => true;
    }
}
