/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
﻿using System.Collections.Generic;
using System.Linq;

namespace UnstableUnicornCore {
    /// <summary>
    /// Component managing knowledge of seen cards of other players
    /// 
    /// This is specialy needed when some agent needs to make some playouts before
    /// he decises what he want to play. Because this agent should not cheat then the
    /// initial state is based on his knowledge of seen cards.
    /// </summary>
    public struct CardVisibilityTracker {

        Dictionary<APlayer, Dictionary<APlayer, HashSet<Card>>> knownCardsInHand = new();
        HashSet<APlayer> playersSeePile = new();

        public CardVisibilityTracker(List<APlayer> players) {
            foreach (var p in players) {
                Dictionary<APlayer, HashSet<Card>> knownCardsInHandByPlayer = new();
                foreach (var p2 in players)
                    knownCardsInHandByPlayer.Add(p2, new());

                knownCardsInHand.Add(p, knownCardsInHandByPlayer);
            }
        }

        /// <summary>
        /// Returns full knowledge of player about seen cards (including himself)
        /// </summary>
        /// <param name="player">Which player knowledge to return</param>
        /// <returns></returns>
        public Dictionary<APlayer, HashSet<Card>> GetKnownPlayerCardsOfAllPlayers(APlayer player) {
            var knowledge = knownCardsInHand[player];
            return knowledge;
        }

        /// <summary>
        /// Returns knowledge of <paramref name="player"/> about specific <paramref name="targetPlayer"/>
        /// </summary>
        /// <param name="player"></param>
        /// <param name="targetPlayer"></param>
        /// <returns></returns>
        public HashSet<Card> GetKnownPlayerCardsOfTarget(APlayer player, APlayer targetPlayer) {
            var cardsOfPlayer = GetKnownPlayerCardsOfAllPlayers(player)[targetPlayer];
            return cardsOfPlayer;
        }

        /// <summary>
        /// Removes card belonging <paramref name="cardOwner"/> from knowledge of all players
        /// </summary>
        /// <param name="cardOwner"></param>
        /// <param name="card"></param>
        public void RemoveCardFromVisibility(APlayer cardOwner, Card card) {
            foreach (var knownCards in knownCardsInHand) {
                var cards = knownCards.Value[cardOwner];
                cards.Remove(card);
            }
        }

        /// <summary>
        /// Remove all seen cards of player from knowledge of other players except some players
        /// 
        /// The player <paramref name="player"/> will be always excluded
        /// </summary>
        /// <param name="player"></param>
        /// <param name="exceptPlayers"></param>
        public void RemoveAllSeenCardsOfPlayer(APlayer player, List<APlayer> exceptPlayers) {
            foreach (var knownCards in knownCardsInHand) {
                if (exceptPlayers.Contains(knownCards.Key))
                    continue;
                if (knownCards.Key == player)
                    continue;

                var seenCards = knownCards.Value[player];
                seenCards.Clear();
            }
        }

        /// <summary>
        /// Add <paramref name="card"/> belonging to <paramref name="cardOwner"/> to knowledge
        /// of player <paramref name="whoSawCard"/>
        /// </summary>
        /// <param name="whoSawCard"></param>
        /// <param name="cardOwner"></param>
        /// <param name="card"></param>
        public void AddSeenCardToPlayerKnowledge(APlayer whoSawCard, APlayer cardOwner, Card card) {
            var knowledge = GetKnownPlayerCardsOfAllPlayers(whoSawCard);
            var cards = knowledge[cardOwner];
            cards.Add(card);
        }

        /// <summary>
        /// Add a card to each player's knowledge
        /// </summary>
        /// <param name="cardOwner"></param>
        /// <param name="card"></param>
        public void AllPlayersSawPlayerCard(APlayer cardOwner, Card card) {
            foreach (var player in knownCardsInHand.Keys) {
                AddSeenCardToPlayerKnowledge(player, cardOwner, card);
            }
        }

        /// <summary>
        /// For every player is swapped knowledge of two players
        /// </summary>
        /// <param name="player1"></param>
        /// <param name="player2"></param>
        public void SwapPlayerKnownCards(APlayer player1, APlayer player2) {
            foreach (var knownCards in knownCardsInHand.Values) {
                var cards1 = knownCards[player1];
                var cards2 = knownCards[player2];

                knownCards[player1] = cards2;
                knownCards[player2] = cards1;
            }
        }

        /// <summary>
        /// Adds information that <paramref name="player"/> currently see pile content
        /// </summary>
        /// <param name="player"></param>
        public void AddPlayerSeePile(APlayer player) => playersSeePile.Add(player);

        /// <summary>
        /// Removes information that <paramref name="player"/> currently see pile content
        /// => player now doesn't see pile content
        /// </summary>
        /// <param name="player"></param>
        public void RemovePlayerSeePile(APlayer player) => playersSeePile.Remove(player);

        /// <summary>
        /// Does the given player see a draw pile? This information is needed when we copy (clone) the game state.
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public bool IsPlayerSeePile(APlayer player) => playersSeePile.Contains(player);

        /// <summary>
        /// Make a deep copy of a CardVisibilityTracker
        /// </summary>
        /// <param name="cardMapper"></param>
        /// <param name="playerMapper"></param>
        /// <returns></returns>
        public CardVisibilityTracker Clone(Dictionary<Card, Card> cardMapper, Dictionary<APlayer, APlayer> playerMapper) {
            var newCardVisibility = new CardVisibilityTracker(playerMapper.Values.ToList());
            
            foreach (var (knowledgeOfPlayerOld, knowledgeOfPlayerNew) in playerMapper)
                foreach (var (oldP, newP) in playerMapper) {
                    newCardVisibility.knownCardsInHand[knowledgeOfPlayerNew][newP] =
                        knownCardsInHand[knowledgeOfPlayerOld][oldP].ToList().ConvertAll(c => cardMapper[c]).ToHashSet();
                }

            foreach (var player in playersSeePile) {
                newCardVisibility.playersSeePile.Add(playerMapper[player]);
            }

            return newCardVisibility;
        }
    }
}
