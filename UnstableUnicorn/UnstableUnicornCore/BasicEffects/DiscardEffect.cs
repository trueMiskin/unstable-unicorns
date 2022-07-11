using System;
using System.Collections.Generic;
using System.Linq;

namespace UnstableUnicornCore.BasicEffects {
    public sealed class DiscardEffect : AEffect {
        
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

        public override void ChooseTargets(GameController gameController) {
            List<APlayer> players = _playerTargeting switch {
                PlayerTargeting.PlayerOwner => new List<APlayer> { OwningPlayer },
                PlayerTargeting.AnyPlayer => OwningPlayer.ChoosePlayers(1, true, this),
                PlayerTargeting.EachPlayer => gameController.Players,
                PlayerTargeting.EachOtherPlayer => gameController.Players.Except( new List<APlayer>{ OwningPlayer }).ToList(),
                _ => throw new NotImplementedException(),
            };

            foreach (APlayer player in players)
                ChooseTargetForPlayer(gameController, player);
        }

        private void ChooseTargetForPlayer(GameController gameController, APlayer player) {
            int validTargets = numberValidTarget(gameController, player);

            if (_cardCount > validTargets)
                _cardCount = validTargets;

            CardTargets = player.WhichCardsToDiscard(_cardCount, _allowedCardTypes);

            if (CardTargets.Count != _cardCount)
                throw new InvalidOperationException($"Not selected enough cards to discard");

            var copyPlayerHand = new List<Card>(player.Hand);
            foreach (var card in CardTargets) {
                if (card == null)
                    throw new InvalidOperationException($"Card was not selected");
                if (!_allowedCardTypes.Contains(card.CardType))
                    throw new InvalidOperationException($"Card {card.Name} have not allowed card type");
                if (!copyPlayerHand.Remove(card))
                    throw new InvalidOperationException($"Card {card.Name} not in player hand!");
                if (gameController.cardsWhichAreTargeted.Contains(card))
                    throw new InvalidOperationException($"Card {card.Name} is targeted by another effect");
            }
        }

        private int numberValidTarget(GameController gameController, APlayer player, bool checkPrePlayConditions = false) {
            int validTargets = 0;
            foreach (var card in player.Hand) {
                // if this card is not played - only checking requirements, then
                // don't count this card as valid target, but when this is presented
                // on hand during choosing target then this card can be valid target
                // this can be used like in ReturnThenDiscardCard when we need choosen
                // player to discard a card
                if (card == OwningCard && checkPrePlayConditions)
                    continue;
                if (_allowedCardTypes.Contains(card.CardType) && !gameController.cardsWhichAreTargeted.Contains(card))
                    validTargets++;
            }
            return validTargets;
        }

        public override void InvokeEffect(GameController gameController) {
            foreach (var card in CardTargets)
                card.MoveCard(gameController, TargetOwner, TargetLocation);
        }

        public override bool MeetsRequirementsToPlayInner(GameController gameController) {
            return numberValidTarget(gameController, OwningPlayer, true) >= _cardCount;
        }
    }
}
