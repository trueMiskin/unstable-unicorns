/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
﻿using System;
using System.Collections.Generic;
using UnstableUnicornCore.BaseSet;

namespace UnstableUnicornCore {
    public abstract class AContinuousEffect {
        private Card _owningCard;
        private APlayer _owningPlayer;

        /// <summary>
        /// Which card belongs this affect
        /// </summary>
        public Card OwningCard { get => _owningCard; }

        /// <summary>
        /// Player who owned this card in time, when this card
        /// was played
        /// 
        /// DON'T USE `OwningCard.Player` because when card is spell
        /// than this value will be resetted on null!
        /// </summary>
        public APlayer OwningPlayer { get => _owningPlayer; }

        protected AContinuousEffect(Card owningCard) {
            _owningCard = owningCard;

            if (owningCard.Player == null)
                throw new InvalidOperationException("When constructing continuous effect player who owns card must be setted!");

            _owningPlayer = owningCard.Player;
        }

        // public abstract bool IsEnabledTriggeringEffects(AEffect effect);

        /// <summary>
        /// If continuous effect allow or disallow play given card to given player stable
        /// </summary>
        /// <param name="card"></param>
        /// <param name="targetOwner">Who will own the card after it is played</param>
        /// <returns></returns>
        public virtual bool IsCardPlayable(Card card, APlayer targetOwner) => true;

        /// <summary>
        /// If continuous effect allow destroy the owning card.
        /// </summary>
        /// <param name="card"></param>
        /// <param name="byEffect"></param>
        /// <returns></returns>
        public virtual bool IsCardDestroyable(Card card, AEffect? byEffect) => true;

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

        /// <summary>
        /// Can given player play an instant card?
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public virtual bool CanBePlayedInstantCards(APlayer player) => true;

        /// <summary>
        /// If continuous effect allow neigh a card.
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public virtual bool IsCardNeighable(Card card) => true;

        /// <summary>
        /// This method transforms the card type on another card type
        /// </summary>
        /// <param name="actualCardType"></param>
        /// <param name="playerOwner"></param>
        /// <returns></returns>
        public virtual ECardType GetCardType(ECardType actualCardType, APlayer playerOwner) => actualCardType;

        /// <summary>
        /// Deep copy continuous effect
        /// 
        /// - resetting owningCard and player
        /// </summary>
        /// <param name="newOwningCard"></param>
        /// <returns></returns>
        public virtual AContinuousEffect Clone(Card newOwningCard, Dictionary<APlayer, APlayer> playerMapper) {
            AContinuousEffect newEffect = (AContinuousEffect)MemberwiseClone();

            newEffect._owningCard = newOwningCard;
            newEffect._owningPlayer = playerMapper[_owningPlayer];

            return newEffect;
        }
    }
}
