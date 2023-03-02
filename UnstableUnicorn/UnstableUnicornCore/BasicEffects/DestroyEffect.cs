using System;
using System.Collections.Generic;

namespace UnstableUnicornCore.BasicEffects {
    public class DestroyEffect : AEffect {
        // card types which can be targeted
        List<ECardType> _allowedCardTypes;

        public DestroyEffect(Card owningCard, int cardCount, List<ECardType> targetType) : base(owningCard, cardCount) {
            TargetOwner = null;
            TargetLocation = CardLocation.DiscardPile;
            _allowedCardTypes = targetType;
        }

        private List<Card> validTargets(GameController gameController) {
            List<Card> cards = gameController.GetCardsOnTable();
            List<Card> validtargets = new();

            foreach (Card card in cards)
                if (_allowedCardTypes.Contains(card.CardType) && card.CanBeDestroyed())
                    validtargets.Add(card);

            return RemoveCardsWhichAreTargeted(validtargets, gameController);
        }

        public override void ChooseTargets(GameController gameController) {
            List<Card> availableSelection = validTargets(gameController);

            int numberCardsToSelect = Math.Min(CardCount, availableSelection.Count);
            
            // owner choose which card should be destroyed
            var selectedCards = OwningPlayer.WhichCardsToDestroy(numberCardsToSelect, this, availableSelection);


            ValidatePlayerSelection(numberCardsToSelect, selectedCards, availableSelection);

            var old = new List<Card>(CardTargets);
            CardTargets = selectedCards;

            CheckAndUpdateSelectionInActualLink(old, CardTargets, gameController);
        }

        public override void InvokeEffect(GameController gameController) {
            foreach (var card in CardTargets)
                card.MoveCard(gameController, TargetOwner, TargetLocation);
        }

        public override bool MeetsRequirementsToPlayInner(GameController gameController) {
            return validTargets(gameController).Count >= CardCount;
        }
    }
}
