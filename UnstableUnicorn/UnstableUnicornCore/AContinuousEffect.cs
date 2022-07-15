using System;
using UnstableUnicornCore.BaseSet;

namespace UnstableUnicornCore {
    public abstract class AContinuousEffect {
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

        protected AContinuousEffect(Card owningCard) {
            OwningCard = owningCard;

            if (owningCard.Player == null)
                throw new InvalidOperationException("When constructing continuous effect player who owns card must be setted!");

            OwningPlayer = owningCard.Player;
        }

        // public abstract bool IsEnabledTriggeringEffects(AEffect effect);

        /// <summary>
        /// If continuous effect allow or disallow play given card to given player stable
        /// </summary>
        /// <param name="card"></param>
        /// <param name="targetOwner">Who will own the card after it is played</param>
        /// <returns></returns>
        public virtual bool IsCardPlayable(Card card, APlayer targetOwner) => true;

        public virtual bool IsCardDestroyable(Card card) => true;

        /// <summary>
        /// Triggered effect are one time effects of unicorns and trigger effects
        /// of unicorns
        /// 
        /// There is one exception, this not effecting baby unicorns because they
        /// can be on <see cref="CardLocation.OnTable"/> or in <see cref="CardLocation.Nursery"/>
        /// <br/>
        /// The function needs card type. When a card enter the stable then
        /// it should not be triggered any effects even if you have <see cref="Pandamonium"/>.
        /// <see cref="Pandamonium"/> is aplicated on that card after casting
        /// trigger effects when this card enter the stable and one time unicorn affects.
        /// <br/>
        /// Note: Effect: When this card enter the stable is implemented as one time effect.
        /// </summary>
        /// <param name="card"></param>
        /// <param name="cardType"></param>
        /// <returns></returns>
        public virtual bool CanBeActivatedTriggerEffect(Card card, ECardType cardType) => true;

        public virtual bool CanBePlayedInstantCards(APlayer player) => true;

        public virtual bool IsCardNeighable(Card card) => true;
        public virtual ECardType GetCardType(ECardType actualCardType, APlayer playerOwner) => actualCardType; 
    }
}
