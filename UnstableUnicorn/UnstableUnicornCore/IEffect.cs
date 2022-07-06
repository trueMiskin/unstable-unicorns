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
        /// Number card to discard
        /// </summary>
        protected int _cardCount;

        /// <summary>
        /// Which cards are targets of affect
        /// </summary>
        public List<Card> CardTargets { get; protected set; } = new();

        /// <summary>
        /// Where will be card after effect
        /// </summary>
        public CardLocation TargetLocation { get; protected set; }

        /// <summary>
        /// Who will owner of card after effect
        /// </summary>
        public APlayer? TargetOwner { get; protected set; }

        public AEffect(Card owningCard, int cardCount) {
            OwningCard = owningCard;
            _cardCount = cardCount;
        }

        /// <summary>
        /// Choosing targets of effect
        /// 
        /// This method should check if targets are valid and is selected required
        /// number of cards if is possible to do it
        /// 
        /// This method should not append to list `cardsWhichAreTargeted`
        /// </summary>
        public abstract void ChooseTargets(GameController gameController);

        /// <summary>
        /// Check if card meets criteria to play
        /// 
        /// This method should be overrided only by if effect.
        /// Else this requirement will be required on every time when you want play
        /// card with this effect even if effect by itself have no condition.
        /// </summary>
        /// <param name="gameController"></param>
        /// <returns></returns>
        public virtual bool MeetsRequirementsToPlay(GameController gameController) => true;

        /// <summary>
        /// Requirement which will be checked in conditional effect.
        /// </summary>
        /// <param name="gameController"></param>
        /// <returns></returns>
        public abstract bool MeetsRequirementsToPlayInner(GameController gameController);
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
