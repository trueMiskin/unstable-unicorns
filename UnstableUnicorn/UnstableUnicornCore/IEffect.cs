using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnstableUnicornCore {
    public abstract class AEffect {
        /// <summary>
        /// Which card belongs this affect
        /// </summary>
        public Card OwningCard { get; protected set; }
        
        /// <summary>
        /// Which card is target of affect
        /// </summary>
        public Card TargetCard { get; protected set; }

        public CardLocation TargetLocation { get; protected set; }

        /// <summary>
        /// Who will own card after effect
        /// </summary>
        public APlayer? TargetOwner { get; protected set; }

        public abstract void InvokeEffect(ETriggerSource triggerSource, AEffect? effect, GameController gameController);
    }

    public class TriggerEffect : AEffect {
        /// <summary>
        /// Predicate which determines if effect is triggered or not
        /// </summary>
        /// <param name="affect">Affect which cause trigger</param>
        /// <param name="owningCard">Card which owns this trigger effect</param>
        /// <returns></returns>
        delegate bool TriggerPredicate(AEffect? affect, Card owningCard);
        TriggerPredicate triggerPredicate = (_, _) => false;

        List<ETriggerSource> triggers;
        AEffect effectToTrigger;
        
        public void SubscribeToEvent(GameController gameController) {
            foreach(var trigger in triggers)
                gameController.SubscribeEvent( trigger, this );
        }

        public void UnSubscribeToEvent(GameController gameController) {
            foreach (var trigger in triggers)
                gameController.UnsubscribeEvent(trigger, this);
        }

        public override void InvokeEffect(ETriggerSource triggerSource, AEffect? effect, GameController gameController) {
            if (triggerPredicate(effect, OwningCard) ) {
                // execute `PreLeaveCardFromTable` immediately because this event should be used
                // only on effects which saving unicorns from leaving stable (for example: to discard pile)
                if (triggerSource == ETriggerSource.ChangeTargeting || triggerSource == ETriggerSource.ChangeLocationOfCard)
                    effectToTrigger.InvokeEffect(triggerSource, effect, gameController);
                else
                    // TOD: add info about effect to effectToTrigger
                    gameController.AddNewEffectToChainLink(effectToTrigger);
            }
        }
    }

    public abstract class ContinuousEffect {
        public abstract bool IsEnabledTriggeringEffects(AEffect effect);
        public abstract bool IsCardPlayable(Card card);
    }

    public class ActivatableEffect : AEffect {
        AEffect effect;

        public override void InvokeEffect(ETriggerSource triggerSource, AEffect? effect, GameController gameController) {
            throw new NotImplementedException();
        }

        void askOnActivate() { }   
    }
}
