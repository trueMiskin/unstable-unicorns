using System;
using System.Collections.Generic;
using UnstableUnicornCore.BasicEffects;

namespace UnstableUnicornCore {
    public sealed class TriggerEffect {
        /// <summary>
        /// Which card belongs this affect
        /// </summary>
        public Card OwningCard { get => _owningCard; init => _owningCard = value; }

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
        private Card.FactoryEffectForTriggerEffects factoryEffect;
        private bool oneTimeUseEffect;

        /// <summary>
        /// Sometimes is needed execute effect in actual chain link but beware in which
        /// state <see cref="ETriggerSource"/> is this effect triggered!!!
        /// </summary>
        private bool executeEffectInActualChainLink;
        private Card _owningCard;

        public TriggerEffect(Card owningCard, TriggerPredicate triggerPredicate, List<ETriggerSource> triggers,
                             Card.FactoryEffectForTriggerEffects factoryEffect, bool oneTimeUseEffect = false, bool executeEffectInActualChainLink = false) {
            this._owningCard = owningCard;
            this.triggerPredicate = triggerPredicate;
            this.triggers = triggers;
            this.factoryEffect = factoryEffect;
            this.oneTimeUseEffect = oneTimeUseEffect;
            this.executeEffectInActualChainLink = executeEffectInActualChainLink;
        }

        /// <summary>
        /// Subscribe/register trigger effect on one or more events
        /// </summary>
        /// <param name="gameController"></param>
        public void SubscribeToEvent(GameController gameController) {
            foreach (var trigger in triggers)
                gameController.SubscribeEvent(trigger, this);

            if (oneTimeUseEffect)
                OwningCard.AddOneTimeTriggerEffect(this);
        }

        /// <summary>
        /// Unsubscribe/unregister trigger effect
        /// </summary>
        /// <param name="gameController"></param>
        public void UnsubscribeToEvent(GameController gameController) {
            foreach (var trigger in triggers)
                gameController.UnsubscribeEvent(trigger, this);

            if (oneTimeUseEffect)
                OwningCard.RemoveOneTimeTriggerEffect(this);
        }

        /// <summary>
        /// Notify trigger effect that some event happened
        /// </summary>
        /// <param name="triggerSource"></param>
        /// <param name="cardWhichTriggerEffect"></param>
        /// <param name="effectWhichTriggerEffect"></param>
        /// <param name="gameController"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void InvokeEffect(ETriggerSource triggerSource, Card? cardWhichTriggerEffect,
                                 AEffect? effectWhichTriggerEffect, GameController gameController) {
            // if effect, which cause trigger, is blocking triggering of unicorns cards
            if (effectWhichTriggerEffect != null &&
                effectWhichTriggerEffect.IsBlockedTriggeringUnicornCards(OwningCard, OwningCard.CardType))
                return;

            if (!OwningCard.CanBeActivatedTriggerEffect(OwningCard.CardType))
                return;

            AEffect triggeredEffect = factoryEffect(OwningCard, gameController);
            if (triggerPredicate(effectWhichTriggerEffect, cardWhichTriggerEffect, OwningCard, gameController)) {
                // execute `ChangeTargeting` and `ChangeLocationOfCard` immediately because this event should be used
                // only on effects which saving unicorns from leaving stable (for example: to discard pile)
                if (triggerSource == ETriggerSource.ChangeTargeting || triggerSource == ETriggerSource.ChangeLocationOfCard) {
                    // if reaction effect can not be played - skip it
                    if (!triggeredEffect.MeetsRequirementsToPlayInner(gameController))
                        return;

                    if (effectWhichTriggerEffect == null)
                        throw new InvalidOperationException($"{triggerSource} must have not null effect");

                    triggeredEffect.InvokeReactionEffect(gameController, effectWhichTriggerEffect);
                } else {
                    if (executeEffectInActualChainLink)
                        gameController.AddEffectToCurrentChainLink(triggeredEffect);
                    else
                        gameController.AddNewEffectToChainLink(triggeredEffect);
                }

                if (oneTimeUseEffect)
                    // self destruction of trigger effect
                    gameController.UnsubscribeEventAfterEndOfPublishing(this);
            }
        }

        /// <summary>
        /// Deep copy and resetting the owning card
        /// 
        /// Actually it is shallow copy but anything didn't need to be deep copied
        /// </summary>
        /// <param name="owningCard"></param>
        /// <returns></returns>
        public TriggerEffect Clone(Card owningCard) {
            TriggerEffect newTriggerEffect = (TriggerEffect)MemberwiseClone();

            newTriggerEffect._owningCard = owningCard;

            return newTriggerEffect;
        }
    }

    /// <summary>
    /// Helper class that implements most frequent trigger predicates
    /// </summary>
    public static class TriggerPredicates {
        private static readonly Type destroyEffectType = typeof(DestroyEffect);
        private static readonly Type sacrificeEffectType = typeof(SacrificeEffect);
        private static readonly Type returnEffectType = typeof(ReturnEffect);

        public static readonly TriggerEffect.TriggerPredicate IsItInYourStableAtTheBeginningOfYourTurn =
            (AEffect? effect, Card? causedCard, Card owningCard, GameController controller) =>
                owningCard.Player == controller.ActualPlayerOnTurn;

        public static readonly TriggerEffect.TriggerPredicate WhenThisCardEntersYourStable =
            (AEffect? effect, Card? causedCard, Card owningCard, GameController controller) => causedCard == owningCard;

        public static readonly TriggerEffect.TriggerPredicate WhenYourStableHaveMoreThanFiveUnicorns =
            (AEffect? effect, Card? causedCard, Card owningCard, GameController controller) => {
                if (owningCard.Player == null)
                    throw new InvalidOperationException("Player of the card must be non null");

                return owningCard.Player.Stable.Count > 5;
            };

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

        public static readonly TriggerEffect.TriggerPredicate IfThisCardWouldBeSacrificedOrDestroyedOrRetuned =
            (AEffect? effect, Card? causedCard, Card owningCard, GameController controller) => {
                if (effect == null)
                    throw new InvalidOperationException("Effect is disallowed to be null");

                if (!destroyEffectType.IsInstanceOfType(effect) && !sacrificeEffectType.IsInstanceOfType(effect) &&
                    !returnEffectType.IsInstanceOfType(effect))
                    return false;

                foreach (var card in effect.CardTargets) {
                    if (card == owningCard)
                        return true;
                }
                return false;
            };

        public static readonly TriggerEffect.TriggerPredicate IfUnicornInYourStableWouldBeDestroyd =
            (AEffect? effect, Card? causedCard, Card owningCard, GameController controller) => {
                if (effect == null)
                    throw new InvalidOperationException("Effect is disallowed to be null");

                if (!destroyEffectType.IsInstanceOfType(effect))
                    return false;

                foreach (var card in effect.CardTargets) {
                    if (card.Player == owningCard.Player && ECardTypeUtils.UnicornTarget.Contains(card.CardType))
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
