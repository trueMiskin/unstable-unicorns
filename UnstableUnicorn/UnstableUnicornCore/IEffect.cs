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
        public Card OwningCard { get; init; }

        /// <summary>
        /// Player who owned this card in time, when this card
        /// was played
        /// 
        /// DON'T USE `OwningCard.Player` because when card is spell
        /// than this value will be resetted on null!
        /// </summary>
        public APlayer OwningPlayer { get; init; }

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

            if (owningCard.Player == null)
                throw new InvalidOperationException("When constructing effect player who owns card must be setted!");

            OwningPlayer = owningCard.Player;
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
        public abstract void InvokeEffect(GameController gameController);
    }

    public sealed class TriggerEffect {
        /// <summary>
        /// Which card belongs this affect
        /// </summary>
        public Card OwningCard { get; init; }

        /// <summary>
        /// Predicate which determines if effect is triggered or not
        /// </summary>
        /// <param name="affect">Affect which cause trigger</param>
        /// <param name="owningCard">Card which owns this trigger effect</param>
        /// <returns></returns>
        public delegate bool TriggerPredicate(AEffect affect);
        private TriggerPredicate triggerPredicate;

        private List<ETriggerSource> triggers;
        private Card.FactoryEffect factoryEffect;

        public TriggerEffect(Card owningCard, TriggerPredicate triggerPredicate, List<ETriggerSource> triggers, Card.FactoryEffect factoryEffect) {
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

        public void InvokeEffect(ETriggerSource triggerSource, AEffect effect, GameController gameController) {
            AEffect triggeredEffect = factoryEffect(OwningCard, gameController);
            if (triggerPredicate(effect) ) {
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

    public abstract class ContinuousEffect {
        public abstract bool IsEnabledTriggeringEffects(AEffect effect);
        public abstract bool IsCardPlayable(Card card);
    }

    public class ActivatableEffect /*: AEffect*/ {
        AEffect effect;

        /*public override void InvokeEffect(ETriggerSource triggerSource, AEffect? effect, GameController gameController) {
            throw new NotImplementedException();
        }*/

        void askOnActivate() { }   
    }
}
