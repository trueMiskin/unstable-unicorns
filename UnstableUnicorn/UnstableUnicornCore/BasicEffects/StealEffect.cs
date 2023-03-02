using System;
using System.Collections.Generic;

namespace UnstableUnicornCore.BasicEffects {
    public class StealEffect : AEffect {
        private List<ECardType> _allowedCardTypes;
        public Func<Card, GameController, bool> CardPredicate;
        public StealEffect(Card owningCard, int cardCount, List<ECardType> targetType) : base(owningCard, cardCount) {
            _allowedCardTypes = targetType;
            TargetOwner = owningCard.Player;
            TargetLocation = CardLocation.OnTable;
            int owningPlayerIdx = OwningPlayer.PlayerIndex;
            CardPredicate = (card, controller) => _allowedCardTypes.Contains(card.CardType) && card.Player != controller.Players[owningPlayerIdx];
        }

        public StealEffect(Card owningCard, int cardCount, Func<Card, GameController, bool> predicate) : base(owningCard, cardCount) {
            _allowedCardTypes = new();
            TargetOwner = owningCard.Player;
            TargetLocation = CardLocation.OnTable;
            this.CardPredicate = predicate;
        }

        private List<Card> GetValidTargets(GameController gameController) {
            List<Card> cards = gameController.GetCardsOnTable();

            Predicate<Card> predicate = (card) => CardPredicate(card, gameController);
            return RemoveCardsWhichAreTargeted(cards.FindAll(predicate), gameController);
        }

        public override void ChooseTargets(GameController gameController) {
            var stealableCards = GetValidTargets(gameController);
            if (CardCount > stealableCards.Count)
                CardCount = stealableCards.Count;

            // owner choose target cards to steal
            var selection = OwningPlayer.WhichCardsToSteal(CardCount, this, stealableCards);


            ValidatePlayerSelection(CardCount, selection, stealableCards);
            CheckAndUpdateSelectionInActualLink(new List<Card>(), selection, gameController);

            CardTargets.AddRange(selection);
        }

        public override void InvokeEffect(GameController gameController) {
            foreach(var card in CardTargets)
                card.MoveCard(gameController, TargetOwner, TargetLocation);
        }

        public override bool MeetsRequirementsToPlayInner(GameController gameController) {
            return GetValidTargets(gameController).Count >= CardCount;
        }
    }
}
