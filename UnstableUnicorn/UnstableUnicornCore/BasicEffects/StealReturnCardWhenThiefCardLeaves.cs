using System.Collections.Generic;

namespace UnstableUnicornCore.BasicEffects {
    class StealReturnCardWhenThiefCardLeaves : StealEffect {
        public StealReturnCardWhenThiefCardLeaves(Card owningCard, int cardCount, List<ECardType> targetType) : base(owningCard, cardCount, targetType) {}
        
        public override void InvokeEffect(GameController gameController) {
            foreach (var card in CardTargets) {
                var previousOwner = card.Player;
                card.MoveCard(gameController, TargetOwner, TargetLocation);
                new TriggerEffect(
                    card,
                    (AEffect? effect, Card? causedCard, Card owningCard, GameController controller) => causedCard == OwningCard,
                    new List<ETriggerSource> { ETriggerSource.CardLeftStable },
                    (Card _) => new MoveCardBackToPreviousStable(card, previousOwner),
                    oneTimeUseEffect: true
                ).SubscribeToEvent(gameController);
            }
        }
    }
}
