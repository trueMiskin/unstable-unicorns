using System;
using System.Collections.Generic;
using UnstableUnicornCore.BasicEffects;

namespace UnstableUnicornCore {
    public sealed class TriggerEffect {
        /// <summary>
        /// Which card belongs this affect
        /// </summary>
        public Card OwningCard { get; init; }

        /// <summary>
        /// Predicate which determines if effect is triggered or not.
        /// <br/>
        /// API has both affect and card, because for some triggers like
        /// <see cref="ETriggerSource.CardEnteredStable" /> affect is null,
        /// but card is set.
        /// Sometimes even card can be null, for example: <see cref="ETriggerSource.BeginningTurn"/>
        /// <br/>
        /// For now <see cref="TriggerPredicate"/> don't have <see cref="ETriggerSource"/>
        /// as param because I think it should not depend on it.
        /// </summary>
        /// <param name="effect">Affect which cause trigger</param>
        /// <param name="causedCard">Card which belongs effect</param>
        /// <param name="owningCard">Card which owns this predicate/trigger effect</param>
        /// <param name="controller"></param>
        /// <returns>If affect should be triggered</returns>
        public delegate bool TriggerPredicate(AEffect? effect, Card? causedCard, Card owningCard, GameController controller);
        private TriggerPredicate triggerPredicate;

        private List<ETriggerSource> triggers;
        private Card.FactoryEffect factoryEffect;
        private bool oneTimeUseEffect;

        public TriggerEffect(Card owningCard, TriggerPredicate triggerPredicate, List<ETriggerSource> triggers,
                             Card.FactoryEffect factoryEffect, bool oneTimeUseEffect = false) {
            OwningCard = owningCard;
            this.triggerPredicate = triggerPredicate;
            this.triggers = triggers;
            this.factoryEffect = factoryEffect;
            this.oneTimeUseEffect = oneTimeUseEffect;
        }

        public void SubscribeToEvent(GameController gameController) {
            foreach(var trigger in triggers)
                gameController.SubscribeEvent( trigger, this );

            if (oneTimeUseEffect)
                OwningCard.AddOneTimeTriggerEffect(this);
        }

        public void UnsubscribeToEvent(GameController gameController) {
            foreach (var trigger in triggers)
                gameController.UnsubscribeEvent(trigger, this);

            if (oneTimeUseEffect)
                OwningCard.RemoveOneTimeTriggerEffect(this);
        }

        public void InvokeEffect(ETriggerSource triggerSource, Card? cardWhichTriggerEffect,
                                 AEffect? effectWhichTriggerEffect, GameController gameController) {
            AEffect triggeredEffect = factoryEffect(OwningCard);
            if (triggerPredicate(effectWhichTriggerEffect, cardWhichTriggerEffect, OwningCard, gameController) ) {
                // execute `ChangeTargeting` and `ChangeLocationOfCard` immediately because this event should be used
                // only on effects which saving unicorns from leaving stable (for example: to discard pile)
                if (triggerSource == ETriggerSource.ChangeTargeting)
                    triggeredEffect.InvokeEffect(gameController);
                if (triggerSource == ETriggerSource.ChangeLocationOfCard)
                    gameController.AddEffectToActualChainLink(triggeredEffect);
                else
                    // TOD: add info about effect to effectToTrigger
                    gameController.AddNewEffectToChainLink(triggeredEffect);

                if (oneTimeUseEffect)
                    // self destruction of trigger effect
                    gameController.UnsubscribeEventAfterEndOfPublishing(this);
            }
        }
    }

    public static class TriggerPredicates {
        private static readonly Type destroyEffectType = typeof(DestroyEffect);
        private static readonly Type sacrificeEffectType = typeof(SacrificeEffect);

        public static readonly TriggerEffect.TriggerPredicate IsItInYourStableAtTheBeginningOfYourTurn =
            (AEffect? effect, Card? causedCard, Card owningCard, GameController controller) =>
                owningCard.Player == controller.ActualPlayerOnTurn;

        public static readonly TriggerEffect.TriggerPredicate WhenThisCardEntersYourStable =
            (AEffect? effect, Card? causedCard, Card owningCard, GameController controller) => causedCard == owningCard;

        public static readonly TriggerEffect.TriggerPredicate IfThisCardWouldBeSacrificedOrDestroyed =
            (AEffect? effect, Card? causedCard, Card owningCard, GameController controller) => {
                if (effect == null)
                    throw new InvalidOperationException("Effect is disallowed to be null");

                if (!destroyEffectType.IsInstanceOfType(effect) && !sacrificeEffectType.IsInstanceOfType(effect))
                    return false;

                foreach (var card in effect.CardTargets) {
                    if (card == owningCard)
                        return true;
                }
                return false;
            };

        public static readonly TriggerEffect.TriggerPredicate CardEnterLeaveYourStable =
            (AEffect? effect, Card? causedCard, Card owningCard, GameController controller) => {
                if (causedCard == null)
                    throw new InvalidOperationException("Card is disallowed to be null");

                if (WhenThisCardEntersYourStable(effect, causedCard, owningCard, controller))
                    return false;

                return owningCard.Player == causedCard.Player;
            };
    }
}
