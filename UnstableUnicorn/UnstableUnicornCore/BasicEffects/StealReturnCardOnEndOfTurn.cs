using System.Collections.Generic;

namespace UnstableUnicornCore.BasicEffects {
    public class StealReturnCardOnEndOfTurn : StealEffect {
        public StealReturnCardOnEndOfTurn(Card owningCard, int cardCount, List<ECardType> targetType) : base(owningCard, cardCount, targetType) { }

        public override void InvokeEffect(GameController gameController) {
            foreach (var card in CardTargets) {
                var previousOwner = card.Player;
                card.MoveCard(gameController, TargetOwner, TargetLocation);
                new TriggerEffect(
                    card,
                    (_, _, _, _) => true,
                    new List<ETriggerSource> { ETriggerSource.EndTurn },
                    (Card _) => new ReturnEffectCardOnEndOfTurn(card, previousOwner),
                    oneTimeUseEffect: true
                ).SubscribeToEvent(gameController);
            }
        }

        public class ReturnEffectCardOnEndOfTurn : AEffect {
            public ReturnEffectCardOnEndOfTurn(Card owningCard, APlayer previousPlayer) : base(owningCard, 0) {
                CardTargets.Add(owningCard);
                TargetOwner = previousPlayer;
                TargetLocation = CardLocation.OnTable;
            }
            public override void ChooseTargets(GameController gameController) {}

            public override void InvokeEffect(GameController gameController) {
                foreach (var card in CardTargets)
                    card.MoveCard(gameController, TargetOwner, TargetLocation);
            }

            public override bool MeetsRequirementsToPlayInner(GameController gameController) => true;
        }
    }
}
