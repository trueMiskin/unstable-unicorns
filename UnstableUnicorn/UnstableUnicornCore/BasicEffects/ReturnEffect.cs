/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
﻿using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace UnstableUnicornCore.BasicEffects {
    /// <summary>
    /// Return the choosen card to the owner's hand
    /// </summary>
    public class ReturnEffect : AEffect {
        // card types which can be targeted
        delegate bool Predicate(Card card, APlayer player);

        protected List<ECardType> _allowedCardTypes;
        PlayerTargeting _playerTargeting;

        public ReturnEffect(Card owningCard, int cardCount, List<ECardType> targetType,
                PlayerTargeting playerTargeting = PlayerTargeting.AnyPlayer) : base(owningCard, cardCount)
        {
            this._playerTargeting = playerTargeting;

            TargetLocation = CardLocation.InHand;
            _allowedCardTypes = targetType;
        }

        private List<Card> validTargets(GameController gameController, APlayer player, Predicate predicate) {
            List<Card> cards = gameController.GetCardsOnTable();
            List<Card> validtargets = new();

            foreach (Card card in cards)
                if (predicate(card, player))
                    validtargets.Add(card);

            return RemoveCardsWhichAreTargeted(validtargets, gameController);
        }

        private int _playerIdx = 0;
        public override void ChooseTargets(GameController gameController) {
            List<APlayer> players;
            Predicate predicate;
            switch(_playerTargeting){
                case PlayerTargeting.AnyPlayer:
                    players = new List<APlayer> { OwningPlayer };
                    predicate = AnyPlayerPredicate;
                    break;
                case PlayerTargeting.EachPlayer:
                    players = gameController.Players;
                    predicate = EachPlayerPredicate;
                    break;
                default:
                    throw new NotImplementedException();
            };

            for (; _playerIdx < players.Count; _playerIdx++) {
                APlayer player = players[_playerIdx];
                ChooseTargetForPlayer(gameController, player, predicate);
            }
        }

        private void ChooseTargetForPlayer(GameController gameController, APlayer player, Predicate predicate) {
            var targets = validTargets(gameController, player, predicate);

            int validTargetsToReturn = Math.Min(CardCount, targets.Count);

            // owner choose which card should be returned
            var cardsToReturn = player.WhichCardsToReturn(validTargetsToReturn, this, targets);

            ValidatePlayerSelection(validTargetsToReturn, cardsToReturn, targets);
            CheckAndUpdateSelectionInActualLink(new List<Card>(), cardsToReturn, gameController);

            CardTargets.AddRange(cardsToReturn);
        }

        public override void InvokeEffect(GameController gameController) {
            foreach (var card in CardTargets) {
                card.MoveCard(gameController, card.Player, TargetLocation);

                Debug.Assert(card.Player != null);
                gameController.CardVisibilityTracker.AllPlayersSawPlayerCard(card.Player, card);
            }
        }

        public bool AnyPlayerPredicate(Card card, APlayer player) => _allowedCardTypes.Contains(card.CardType);

        public bool EachPlayerPredicate(Card card, APlayer player) => _allowedCardTypes.Contains(card.CardType) && card.Player == player;

        public override bool MeetsRequirementsToPlayInner(GameController gameController) {
            var targets = _playerTargeting switch{
                PlayerTargeting.AnyPlayer => validTargets(gameController, OwningPlayer, AnyPlayerPredicate),
                PlayerTargeting.EachPlayer => validTargets(gameController, OwningPlayer, EachPlayerPredicate),
                _ => throw new NotImplementedException(),
            };
            return targets.Count >= CardCount;
        }
    }
}
