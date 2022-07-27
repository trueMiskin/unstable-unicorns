using System;
using System.Collections.Generic;

namespace UnstableUnicornCore.BasicEffects {
    public class StealEffect : AEffect {
        private List<ECardType> _allowedCardTypes;
        public Predicate<Card> CardPredicate;
        public StealEffect(Card owningCard, int cardCount, List<ECardType> targetType) : base(owningCard, cardCount) {
            _allowedCardTypes = targetType;
            TargetOwner = owningCard.Player;
            TargetLocation = CardLocation.OnTable;
            CardPredicate = card => _allowedCardTypes.Contains(card.CardType) && card.Player != OwningPlayer;
        }

        public StealEffect(Card owningCard, int cardCount, Predicate<Card> predicate) : base(owningCard, cardCount) {
            _allowedCardTypes = new();
            TargetOwner = owningCard.Player;
            TargetLocation = CardLocation.OnTable;
            this.CardPredicate = predicate;
        }

        private List<Card> GetValidTargets(GameController gameController) {
            List<Card> cards = gameController.GetCardsOnTable();

            return RemoveCardsWhichAreTargeted(cards.FindAll(CardPredicate), gameController);
        }

        public override void ChooseTargets(GameController gameController) {
            var stealableCards = GetValidTargets(gameController);
            if (_cardCount > stealableCards.Count)
                _cardCount = stealableCards.Count;

            // owner choose target cards to steal
            var selection = OwningPlayer.WhichCardsToSteal(_cardCount, this, stealableCards);


            ValidatePlayerSelection(_cardCount, selection, stealableCards);
            CheckAndUpdateSelectionInActualLink(new List<Card>(), selection, gameController);

            CardTargets.AddRange(selection);
        }

        public override void InvokeEffect(GameController gameController) {
            foreach(var card in CardTargets)
                card.MoveCard(gameController, TargetOwner, TargetLocation);
        }

        public override bool MeetsRequirementsToPlayInner(GameController gameController) {
            return GetValidTargets(gameController).Count >= _cardCount;
        }
    }
}
