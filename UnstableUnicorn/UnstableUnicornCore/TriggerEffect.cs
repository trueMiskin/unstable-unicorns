using System.Collections.Generic;

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
        /// <param name="affect">Affect which cause trigger</param>
        /// <param name="causedCard">Card which belongs effect</param>
        /// <param name="owningCard">Card which owns this predicate/trigger effect</param>
        /// <returns>If affect should be triggered</returns>
        public delegate bool TriggerPredicate(AEffect? affect, Card? causedCard, Card owningCard);
        private TriggerPredicate triggerPredicate;

        private List<ETriggerSource> triggers;
        private Card.FactoryEffect factoryEffect;

        public TriggerEffect(Card owningCard, TriggerPredicate triggerPredicate, List<ETriggerSource> triggers,
                             Card.FactoryEffect factoryEffect) {
            OwningCard = owningCard;
            this.triggerPredicate = triggerPredicate;
            this.triggers = triggers;
            this.factoryEffect = factoryEffect;
        }

        public void SubscribeToEvent(GameController gameController) {
            foreach(var trigger in triggers)
                gameController.SubscribeEvent( trigger, this );
        }

        public void UnSubscribeToEvent(GameController gameController) {
            foreach (var trigger in triggers)
                gameController.UnsubscribeEvent(trigger, this);
        }

        public void InvokeEffect(ETriggerSource triggerSource, Card? cardWhichTriggerEffect,
                                 AEffect? effectWhichTriggerEffect, GameController gameController) {
            AEffect triggeredEffect = factoryEffect(OwningCard);
            if (triggerPredicate(effectWhichTriggerEffect, cardWhichTriggerEffect, OwningCard) ) {
                // execute `ChangeTargeting` and `ChangeLocationOfCard` immediately because this event should be used
                // only on effects which saving unicorns from leaving stable (for example: to discard pile)
                if (triggerSource == ETriggerSource.ChangeTargeting || triggerSource == ETriggerSource.ChangeLocationOfCard)
                    triggeredEffect.InvokeEffect(gameController);
                else
                    // TOD: add info about effect to effectToTrigger
                    gameController.AddNewEffectToChainLink(triggeredEffect);
            }
        }
    }

    public static class TriggerPredicates {
        public static readonly TriggerEffect.TriggerPredicate WhenThisCardEntersYourStable =
            (AEffect? affect, Card? causedCard, Card owningCard) => causedCard == owningCard;
    }
}
