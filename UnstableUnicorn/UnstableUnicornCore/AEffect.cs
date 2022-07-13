using System;
using System.Collections.Generic;

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

        /// <summary>
        /// Some effect are reacting on other effects like card Black knight Unicorn
        /// which sacrifice himself instead other card
        /// <br/>
        /// This method call <see cref="TriggerEffect"/> in situation when is published
        /// <see cref="ETriggerSource.ChangeTargeting"/> or <see cref="ETriggerSource.ChangeLocationOfCard"/>
        /// <br/>
        /// DON'T forget to ADD new effect to <see cref="GameController.AddEffectToActualChainLink(AEffect)"/>
        /// This method should not do actual effect, only preparing. Real execution of effect should
        /// be in <see cref="InvokeEffect(GameController)"/>
        /// </summary>
        /// <param name="gameController"></param>
        /// <param name="effect"></param>
        public virtual void InvokeReactionEffect(GameController gameController, AEffect effect) {
            throw new NotImplementedException($"{this} is not reaction effect or is not implemented.");
        }
    }
}
