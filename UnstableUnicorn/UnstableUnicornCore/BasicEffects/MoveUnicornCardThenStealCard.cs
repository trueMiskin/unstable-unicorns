/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
﻿using System;
using System.Collections.Generic;

namespace UnstableUnicornCore.BasicEffects {
    /// <summary>
    /// Move card from player's stable to other stable,
    /// then steal card from that stable
    /// </summary>
    public class MoveUnicornCardThenStealCard : AEffect {
        StealEffect thenEffect;
        public MoveUnicornCardThenStealCard(Card owningCard, int cardCount) : base(owningCard, cardCount) {
            thenEffect = new StealEffect(owningCard, 1, (_, _) => false);
        }

        private List<Card> GetValidTargets(GameController gameController) =>
            RemoveCardsWhichAreTargeted(
                gameController.GetCardsOnTable().FindAll(
                    card => card.Player == OwningPlayer && ECardTypeUtils.UnicornTarget.Contains(card.CardType)
                ),
                gameController
            );

        private bool _cardTargetsSelected = false;
        public override void ChooseTargets(GameController gameController) {
            if (!_cardTargetsSelected) {
                var cards = GetValidTargets(gameController);
                var selectedCards = OwningPlayer.WhichCardsToMove(1, this, cards);

                if (selectedCards.Count != 1 || !cards.Contains(selectedCards[0]))
                    throw new InvalidOperationException("Invalid card selection.");
                CardTargets = selectedCards;

                _cardTargetsSelected = true;
            }

            var players = OwningPlayer.ChoosePlayers(1, false, this);

            if (players.Count != 1 || players[0] == OwningPlayer)
                throw new InvalidOperationException("Invalid player selection.");

            TargetOwner = players[0];
            TargetLocation = CardLocation.OnTable;

            int targetOwnerIdx = TargetOwner.PlayerIndex;
            thenEffect.CardPredicate = (card, controller) 
                => card.Player == controller.Players[targetOwnerIdx] && ECardTypeUtils.UnicornTarget.Contains(card.CardType);
        }

        public override void InvokeEffect(GameController gameController) {
            foreach (var card in CardTargets)
                card.MoveCard(gameController, TargetOwner, TargetLocation);

            gameController.AddNewEffectToChainLink(thenEffect);
        }

        public override bool MeetsRequirementsToPlayInner(GameController gameController)
            => GetValidTargets(gameController).Count >= CardCount;

        public override AEffect Clone(Dictionary<Card, Card> cardMapper,
                                      Dictionary<AEffect, AEffect> effectMapper,
                                      Dictionary<APlayer, APlayer> playerMapper) {
            var newEffect = (MoveUnicornCardThenStealCard)base.Clone(cardMapper, effectMapper, playerMapper);
            newEffect.thenEffect = (StealEffect)thenEffect.Clone(cardMapper, effectMapper, playerMapper);
            effectMapper.Add(thenEffect, newEffect.thenEffect);

            return newEffect;
        }
    }
}
