using System;

namespace UnstableUnicornCore.BasicEffects {
    public class BringCardFromHandOnTable : AEffect {
        Predicate<Card> allowedCardsPredicate;
        public BringCardFromHandOnTable(Card owningCard, int cardCount, Predicate<Card> allowedCardsPredicate) : base(owningCard, cardCount) {
            this.allowedCardsPredicate = allowedCardsPredicate;
            TargetOwner = OwningPlayer;
            TargetLocation = CardLocation.OnTable;
        }

        public override void ChooseTargets(GameController gameController) {
            var cards = OwningPlayer.Hand.FindAll(allowedCardsPredicate);

            _cardCount = Math.Min(_cardCount, cards.Count);
            // maybe a little bit wrong used this method for selection
            CardTargets = OwningPlayer.WhichCardsToGet(_cardCount, this, cards);

            foreach (var card in CardTargets)
                if (!cards.Contains(card))
                    throw new InvalidOperationException("Selected unknown card");
        }

        public override void InvokeEffect(GameController gameController) {
            foreach (var card in CardTargets)
                card.MoveCard(gameController, TargetOwner, TargetLocation);
        }

        public override bool MeetsRequirementsToPlayInner(GameController gameController) => true;
    }
}
