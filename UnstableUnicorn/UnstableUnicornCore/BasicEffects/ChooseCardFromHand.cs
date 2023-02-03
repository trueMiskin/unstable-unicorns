using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace UnstableUnicornCore.BasicEffects {
    public class ChooseCardFromHand : AEffect {
        bool canSelectMyself;
        public ChooseCardFromHand(Card owningCard, int cardCount, bool canSelectMyself) : base(owningCard, cardCount) {
            this.canSelectMyself = canSelectMyself;
            TargetLocation = CardLocation.InHand;
            TargetOwner = OwningPlayer;
        }

        private bool _playerSelected = false;
        private APlayer? player;
        public override void ChooseTargets(GameController gameController) {
            if (!_playerSelected) {
                var players = OwningPlayer.ChoosePlayers(1, canSelectMyself, this);
                if (players.Count != 1 || (players[0] == OwningPlayer && !canSelectMyself))
                    throw new InvalidOperationException("Selected wrong number of players or player select itself which is disallowed");

                player = players[0];
                _playerSelected = true;
            }

            Debug.Assert(player != null);
            _cardCount = Math.Min(_cardCount, player.Hand.Count);
            CardTargets = OwningPlayer.WhichCardsToGet(_cardCount, this, player.Hand);

            ValidatePlayerSelection(_cardCount, CardTargets, player.Hand);
        }

        public override void InvokeEffect(GameController gameController) {
            foreach (var card in CardTargets)
                card.MoveCard(gameController, TargetOwner, TargetLocation);
        }

        public override bool MeetsRequirementsToPlayInner(GameController gameController) => true;

        public override AEffect Clone(Dictionary<Card, Card> cardMapper, Dictionary<AEffect, AEffect> effectMapper, Dictionary<APlayer, APlayer> playerMapper) {
            ChooseCardFromHand effect = (ChooseCardFromHand)base.Clone(cardMapper, effectMapper, playerMapper);
            effect.player = player == null ? null : playerMapper[player];

            return effect;
        }
    }
}
