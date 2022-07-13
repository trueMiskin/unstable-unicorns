using System;

namespace UnstableUnicornCore.BasicEffects {
    public class SearchDeckEffect : AEffect {
        Predicate<Card> searchPredicate;
        public SearchDeckEffect(Card owningCard, int cardCount, Predicate<Card> predicate) : base(owningCard, cardCount) {
            searchPredicate = predicate;

            TargetLocation = CardLocation.InHand;
            TargetOwner = OwningPlayer;
        }

        public override void ChooseTargets(GameController gameController) {}

        public override void InvokeEffect(GameController gameController) {
            // selecting cards are not in choose targets because what if there will be also
            // effects which will draw and draw a card this effect want???
            var cards = gameController.Pile.FindAll(searchPredicate);

            _cardCount = Math.Min(_cardCount, cards.Count);
            CardTargets = OwningPlayer.WhichCardsToGet(_cardCount, this, cards);

            foreach (var card in CardTargets)
                card.MoveCard(gameController, TargetOwner, TargetLocation);
        }

        public override bool MeetsRequirementsToPlayInner(GameController gameController) => true;
    }
}
