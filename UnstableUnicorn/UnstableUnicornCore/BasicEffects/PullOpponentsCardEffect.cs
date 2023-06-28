/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
﻿using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace UnstableUnicornCore.BasicEffects {
    public sealed class PullOpponentsCardEffect : AEffect {
        int _numberSelectPlayers;
        List<APlayer>? playerList;
        public PullOpponentsCardEffect(Card owningCard, int cardCount, int numberSelectedPlayers) : base(owningCard, cardCount) {
            TargetOwner = OwningPlayer;
            TargetLocation = CardLocation.InHand;
            _numberSelectPlayers = numberSelectedPlayers;
        }

        public override void ChooseTargets(GameController gameController) {
            playerList = OwningPlayer.ChoosePlayers(_numberSelectPlayers, false, this);

            var availableSelection = new List<APlayer>(gameController.Players);
            availableSelection.Remove(OwningPlayer);

            ValidatePlayerSelection(_numberSelectPlayers, playerList, availableSelection);
        }

        public override void InvokeEffect(GameController gameController) {
            if (playerList == null)
                throw new InvalidOperationException("Players was not selected (was not called `ChooseTargets`).");

            foreach (APlayer player in playerList) {
                int numberCardToSelect = Math.Min(CardCount, player.Hand.Count);
                for (int i = 0; i < numberCardToSelect; i++) {
                    int selectedIndex = player.GameController.Random.Next(player.Hand.Count);
                    Card selectedCard = player.Hand[selectedIndex];
                    selectedCard.MoveCard(gameController, TargetOwner, TargetLocation);

                    Debug.Assert(TargetOwner != null);
                    gameController.CardVisibilityTracker.RemoveAllSeenCardsOfPlayer(player, new List<APlayer> { TargetOwner });
                    gameController.CardVisibilityTracker.AddSeenCardToPlayerKnowledge(TargetOwner, TargetOwner, selectedCard);
                    gameController.CardVisibilityTracker.AddSeenCardToPlayerKnowledge(player, TargetOwner, selectedCard);
                }
            }
        }

        public override bool MeetsRequirementsToPlayInner(GameController gameController) => true;

        public override AEffect Clone(Dictionary<Card, Card> cardMapper,
                                      Dictionary<AEffect, AEffect> effectMapper,
                                      Dictionary<APlayer, APlayer> playerMapper) {
            var newEffect = (PullOpponentsCardEffect)base.Clone(cardMapper, effectMapper, playerMapper);
            newEffect.playerList = playerList == null ? null : playerList.ConvertAll(p => playerMapper[p]);

            return newEffect;
        }
    }
}
