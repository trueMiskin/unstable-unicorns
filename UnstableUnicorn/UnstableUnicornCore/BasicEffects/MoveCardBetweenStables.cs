using System;
using System.Collections.Generic;

namespace UnstableUnicornCore.BasicEffects {
    public class MoveCardBetweenStables : AEffect {
        Predicate<Card> whichCardMovePredicate;
        bool canOwningPlayerGetCard;
        public MoveCardBetweenStables(Card owningCard,
                                      Predicate<Card> whichCardMovePredicate,
                                      bool canOwningPlayerGetCard) : base(owningCard, 1)
        {
            this.whichCardMovePredicate = whichCardMovePredicate;
            this.canOwningPlayerGetCard = canOwningPlayerGetCard;

            TargetLocation = CardLocation.OnTable;
        }

        private List<Card> GetValidTargets(GameController gameController)
            => gameController.GetCardsOnTable().FindAll(whichCardMovePredicate);
        public override void ChooseTargets(GameController gameController) {
            var cards = GetValidTargets(gameController);

            CardTargets = OwningPlayer.WhichCardsToMove(_cardCount, this, cards);

            foreach (var card in CardTargets)
                if (!cards.Contains(card))
                    throw new InvalidOperationException("Selected unknown card");

            var players = OwningPlayer.ChoosePlayers(1, canOwningPlayerGetCard, this);

            if (players.Count != 1)
                throw new InvalidOperationException("Selected wrong number of players");
            
            APlayer player = players[0];
            if (player == OwningPlayer && !canOwningPlayerGetCard)
                throw new InvalidOperationException("Owning player can't get card");

            if (player == CardTargets[0].Player)
                throw new InvalidOperationException("Player owner of this card and target owner is same player!");

            TargetOwner = player;
        }

        public override void InvokeEffect(GameController gameController) {
            foreach (var card in CardTargets)
                card.MoveCard(gameController, TargetOwner, TargetLocation);
        }

        public override bool MeetsRequirementsToPlayInner(GameController gameController)
            => GetValidTargets(gameController).Count >= _cardCount;
    }
}
