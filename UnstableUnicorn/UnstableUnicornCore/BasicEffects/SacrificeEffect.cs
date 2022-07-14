using System;
using System.Collections.Generic;

namespace UnstableUnicornCore.BasicEffects {
    public class SacrificeEffect : AEffect {
        // card types which can be targeted
        protected List<ECardType> _allowedCardTypes;
        PlayerTargeting _playerTargeting;

        public SacrificeEffect(Card owningCard,
                               int cardCount,
                               List<ECardType> targetType,
                               PlayerTargeting playerTargeting = PlayerTargeting.PlayerOwner) : base(owningCard, cardCount)
        {
            OwningCard = owningCard;
            this._playerTargeting = playerTargeting;

            _allowedCardTypes = targetType;
            TargetOwner = null;
            TargetLocation = CardLocation.DiscardPile;
        }

        private int numberValidTargets(GameController gameController, APlayer player) {
            int numberValidTargets = 0;

            List<Card> cards = gameController.GetCardsOnTable();
            foreach (Card c in cards)
                if (_allowedCardTypes.Contains(c.CardType) && c.CanBeSacriced() && c.Player == player)
                    numberValidTargets++;

            return numberValidTargets;
        }

        public override void ChooseTargets(GameController gameController) {
            List<APlayer> players = _playerTargeting switch {
                PlayerTargeting.PlayerOwner => new List<APlayer> { OwningPlayer },
                PlayerTargeting.EachPlayer => gameController.Players,
                _ => throw new NotImplementedException(),
            };

            foreach (APlayer player in players)
                ChooseTargetForPlayer(gameController, player);
        }

        private void ChooseTargetForPlayer(GameController gameController, APlayer player) {
            int numCardsSacrifice = numberValidTargets(gameController, player);

            int numberCardsToSelect = Math.Min(_cardCount, numCardsSacrifice);

            var selectedCards = player.WhichCardsToSacrifice(numberCardsToSelect, _allowedCardTypes);

            if (selectedCards.Count != numberCardsToSelect)
                throw new InvalidOperationException($"Not selected enough cards to discard");

            foreach (var card in CardTargets) {
                if (card.Player != OwningPlayer || card.Location != CardLocation.OnTable)
                    throw new InvalidOperationException("Selected other player's card or card which is not on table");
                if (!_allowedCardTypes.Contains(card.CardType) || !card.CanBeSacriced())
                    throw new InvalidOperationException($"Card {card.Name} have not allowed card type or can't be sacrificed");
                if (gameController.cardsWhichAreTargeted.Contains(card))
                    throw new InvalidOperationException($"Card {card.Name} is targeted by another effect");
            }
            CardTargets.AddRange(selectedCards);
        }

        public override void InvokeEffect(GameController gameController) {
            foreach (var card in CardTargets)
                card.MoveCard(gameController, TargetOwner, TargetLocation);
        }

        public override bool MeetsRequirementsToPlayInner(GameController gameController) {
            return numberValidTargets(gameController, OwningPlayer) >= _cardCount
                || _cardCount == Int32.MaxValue /* For sacrifice all */;
        }
    }
}
