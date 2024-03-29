/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
﻿using System;
using System.Diagnostics;

namespace UnstableUnicornCore.BasicEffects {
    public class SearchDeckEffect : AEffect {
        Predicate<Card> searchPredicate;
        CardLocation whereToSearchCards;
        public SearchDeckEffect(Card owningCard,
                                int cardCount,
                                CardLocation whereToSearchCards,
                                Predicate<Card> predicate) : base(owningCard, cardCount) {
            searchPredicate = predicate;
            if (whereToSearchCards != CardLocation.Pile && whereToSearchCards != CardLocation.DiscardPile)
                throw new InvalidOperationException($"Invalid location {whereToSearchCards} where to search for cards");

            this.whereToSearchCards = whereToSearchCards;
            TargetLocation = CardLocation.InHand;
            TargetOwner = OwningPlayer;
        }

        public override void ChooseTargets(GameController gameController) {}

        public override void InvokeEffect(GameController gameController) {
            // selecting cards are not in choose targets because what if there will be also
            // effects which will draw and draw a card this effect want???
            var correctPile = whereToSearchCards switch {
                CardLocation.Pile => gameController.Pile,
                CardLocation.DiscardPile => gameController.DiscardPile,
                _ => throw new NotImplementedException()
            };

            var cards = correctPile.FindAll(searchPredicate);

            CardCount = Math.Min(CardCount, cards.Count);

            if (whereToSearchCards == CardLocation.Pile)
                gameController.CardVisibilityTracker.AddPlayerSeePile(OwningPlayer);
            
            CardTargets = OwningPlayer.WhichCardsToGet(CardCount, this, cards);

            if (whereToSearchCards == CardLocation.Pile)
                gameController.CardVisibilityTracker.RemovePlayerSeePile(OwningPlayer);

            ValidatePlayerSelection(CardCount, CardTargets, cards);

            foreach (var card in CardTargets) {
                card.MoveCard(gameController, TargetOwner, TargetLocation);

                if(whereToSearchCards == CardLocation.Pile)
                    correctPile.Remove(card);

                Debug.Assert(TargetOwner != null);
                gameController.CardVisibilityTracker.AllPlayersSawPlayerCard(TargetOwner, card);
            }
        }

        public override bool MeetsRequirementsToPlayInner(GameController gameController) => true;
    }
}
