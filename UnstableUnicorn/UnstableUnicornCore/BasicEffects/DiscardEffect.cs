using System;
using System.Collections.Generic;
using System.Linq;

namespace UnstableUnicornCore.BasicEffects {
    public class DiscardEffect : AEffect {
        
        PlayerTargeting _playerTargeting;

        // card types which can be targeted
        List<ECardType> _allowedCardTypes;
        
        public DiscardEffect(Card owningCard, int cardCount, List<ECardType> targetType, PlayerTargeting playerTargeting) 
            : base(owningCard, cardCount)
        {
            _allowedCardTypes = targetType;
            TargetOwner = null;
            TargetLocation = CardLocation.DiscardPile;
            _playerTargeting = playerTargeting;
        }

        List<APlayer>? players;
        public override void ChooseTargets(GameController gameController) {
            players = _playerTargeting switch {
                PlayerTargeting.PlayerOwner => new List<APlayer> { OwningPlayer },
                PlayerTargeting.AnyPlayer => OwningPlayer.ChoosePlayers(1, true, this),
                PlayerTargeting.EachPlayer => gameController.Players,
                PlayerTargeting.EachOtherPlayer => gameController.Players.Except( new List<APlayer>{ OwningPlayer }).ToList(),
                _ => throw new NotImplementedException(),
            };
        }

        private void ChooseTargetForPlayer(GameController gameController, APlayer player) {
            List<Card> availableSelection = validTargets(gameController, player);

            int numberCardsToSelect = Math.Min(CardCount, availableSelection.Count);

            var selectedCards = player.WhichCardsToDiscard(numberCardsToSelect, this, availableSelection);

            ValidatePlayerSelection(numberCardsToSelect, selectedCards, availableSelection);
            CheckAndUpdateSelectionInActualLink(new List<Card>(), selectedCards, gameController);

            CardTargets.AddRange(selectedCards);
        }

        private List<Card> validTargets(GameController gameController, APlayer player, bool checkPrePlayConditions = false) {
            List<Card> validtargets = new();

            foreach (var card in player.Hand) {
                // if this card is not played - only checking requirements, then
                // don't count this card as valid target, but when this is presented
                // on hand during choosing target then this card can be valid target
                // this can be used like in ReturnThenDiscardCard when we need choosen
                // player to discard a card
                if (card == OwningCard && checkPrePlayConditions)
                    continue;
                if (_allowedCardTypes.Contains(card.CardType) && !gameController.CardsWhichAreTargeted.ContainsKey(card))
                    validtargets.Add(card);
            }

            // TODO: does this effect need to call RemoveCardsWhichAreTargeted?
            return validtargets;
        }

        private int _playerIdx = 0;
        public override void InvokeEffect(GameController gameController) {
            if (players == null)
                throw new NullReferenceException();

            // choosing what should be discarded right before perfoming this action
            for (; _playerIdx < players.Count; _playerIdx++) {
                var player = players[_playerIdx];
                ChooseTargetForPlayer(gameController, player);
            }
            foreach (var card in CardTargets)
                card.MoveCard(gameController, TargetOwner, TargetLocation);
        }

        public override bool MeetsRequirementsToPlay(GameController gameController) => MeetsRequirementsToPlayInner(gameController);

        public override bool MeetsRequirementsToPlayInner(GameController gameController) {
            if (_playerTargeting != PlayerTargeting.EachPlayer
                && _playerTargeting != PlayerTargeting.PlayerOwner)
                // requirements are always met, if player targeting don't include owner of card
                return true;

            return validTargets(gameController, OwningPlayer, true).Count >= CardCount;
        }
        
        public override AEffect Clone(Dictionary<Card, Card> cardMapper,
                                      Dictionary<AEffect, AEffect> effectMapper,
                                      Dictionary<APlayer, APlayer> playerMapper) {
            var newEffect = (DiscardEffect)base.Clone(cardMapper, effectMapper, playerMapper);
            newEffect.players = players == null ? null : players.ConvertAll(p => playerMapper[p]);

            return newEffect;
        }
    }
}
