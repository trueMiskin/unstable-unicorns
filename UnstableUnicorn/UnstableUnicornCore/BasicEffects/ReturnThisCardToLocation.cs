﻿using System;

namespace UnstableUnicornCore.BasicEffects {
    /// <summary>
    /// Specialized effect of return effect
    /// 
    /// This effect get a card which should be return to:
    /// - owner's hand
    /// - nursery
    /// <br/>
    /// Since I wrote Black knight unicorn card, this effect move cards
    /// from table to hand without intermediate step moving to discard pile
    /// 
    /// This effect should be called on trigger <see cref="ETriggerSource.ChangeLocationOfCard"/>
    /// </summary>
    public class ReturnThisCardToLocation : ReturnEffect {
        public ReturnThisCardToLocation(Card owningCard, CardLocation cardLocation = CardLocation.InHand) : base(owningCard, 0, ECardTypeUtils.CardTarget) {
            if (owningCard.Player == null)
                throw new InvalidOperationException("Card should not have null player");

            CardTargets.Add(owningCard);

            switch (cardLocation) {
                case CardLocation.InHand:
                    TargetOwner = owningCard.Player;
                    TargetLocation = CardLocation.InHand;
                    break;
                case CardLocation.Nursery:
                    TargetOwner = null;
                    TargetLocation = CardLocation.Nursery;
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public override void ChooseTargets(GameController gameController) {
            /* No selection required */
        }

        public override void InvokeReactionEffect(GameController gameController, AEffect effect) {
            // break infinite recursion when baby unicorn is returned to nursery
            if (effect.TargetOwner == TargetOwner && effect.TargetLocation == TargetLocation)
                return;

            effect.CardTargets.Remove(OwningCard);
            gameController.AddEffectToActualChainLink(this);
        }

        public override bool MeetsRequirementsToPlayInner(GameController gameController) => true;
    }
}