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
            this._playerTargeting = playerTargeting;

            _allowedCardTypes = targetType;
            TargetOwner = null;
            TargetLocation = CardLocation.DiscardPile;
        }

        private List<Card> validTargets(GameController gameController, APlayer player) {
            List<Card> cards = gameController.GetCardsOnTable();
            List<Card> validtargets = new();

            foreach (Card card in cards)
                if (_allowedCardTypes.Contains(card.CardType) && card.CanBeSacriced() && card.Player == player)
                    validtargets.Add(card);

            return RemoveCardsWhichAreTargeted(validtargets, gameController);
        }

        private int _playerIdx = 0;
        public override void ChooseTargets(GameController gameController) {
            List<APlayer> players = _playerTargeting switch {
                PlayerTargeting.PlayerOwner => new List<APlayer> { OwningPlayer },
                PlayerTargeting.EachPlayer => gameController.Players,
                _ => throw new NotImplementedException(),
            };

            for (; _playerIdx < players.Count; _playerIdx++) {
                APlayer player = players[_playerIdx];
                ChooseTargetForPlayer(gameController, player);
            }
        }

        private void ChooseTargetForPlayer(GameController gameController, APlayer player) {
            List<Card> availableSelection = validTargets(gameController, player);

            int numberCardsToSelect = Math.Min(CardCount, availableSelection.Count);

            var selectedCards = player.WhichCardsToSacrifice(numberCardsToSelect, this, availableSelection);


            ValidatePlayerSelection(numberCardsToSelect, selectedCards, availableSelection);
            CheckAndUpdateSelectionInActualLink(new List<Card>(), selectedCards, gameController);

            CardTargets.AddRange(selectedCards);
        }

        public override void InvokeEffect(GameController gameController) {
            foreach (var card in CardTargets)
                card.MoveCard(gameController, TargetOwner, TargetLocation);
        }

        public override bool MeetsRequirementsToPlayInner(GameController gameController) {
            return validTargets(gameController, OwningPlayer).Count >= CardCount
                || CardCount == Int32.MaxValue /* For sacrifice all */;
        }
    }
}
