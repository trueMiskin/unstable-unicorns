using System;
using System.Collections.Generic;

namespace UnstableUnicornCore.BasicEffects {
    public class BringCardFromSourceOnTable : AEffect {
        Predicate<Card> allowedCardsPredicate;
        CardLocation cardsourceLocation;
        public BringCardFromSourceOnTable(Card owningCard,
                                        int cardCount,
                                        Predicate<Card> allowedCardsPredicate,
                                        CardLocation cardsourceLocation = CardLocation.InHand) : base(owningCard, cardCount) 
        {
            this.allowedCardsPredicate = allowedCardsPredicate;
            this.cardsourceLocation = cardsourceLocation;
            TargetOwner = OwningPlayer;
            TargetLocation = CardLocation.OnTable;
        }

        public override void ChooseTargets(GameController gameController) {
            var cardsSource = cardsourceLocation switch {
                CardLocation.InHand => OwningPlayer.Hand,
                CardLocation.DiscardPile => gameController.DiscardPile,
                CardLocation.Nursery => gameController.Nursery,
                _ => throw new NotImplementedException()
            };

            var cards = cardsSource.FindAll(allowedCardsPredicate);
            cards = RemoveCardsWhichAreTargeted(cards, gameController);

            CardCount = Math.Min(CardCount, cards.Count);
            // maybe a little bit wrong used this method for selection
            CardTargets = OwningPlayer.WhichCardsToGet(CardCount, this, cards);

            ValidatePlayerSelection(CardCount, CardTargets, cards);
            CheckAndUpdateSelectionInActualLink(new List<Card>(), CardTargets, gameController);
        }

        public override void InvokeEffect(GameController gameController) {
            foreach (var card in CardTargets)
                card.MoveCard(gameController, TargetOwner, TargetLocation);
        }

        public override bool MeetsRequirementsToPlayInner(GameController gameController) => true;
    }
}
