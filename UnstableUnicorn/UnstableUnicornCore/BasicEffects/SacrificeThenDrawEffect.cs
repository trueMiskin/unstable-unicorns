using System.Collections.Generic;

namespace UnstableUnicornCore.BasicEffects {
    /// <summary>
    /// If at least one card is not sacrificed then no card is drawn
    /// </summary>
    public class SacrificeThenDrawEffect : SacrificeEffect {
        int _numCardsToDraw;
        public SacrificeThenDrawEffect(Card owningCard,
                                       int cardCount,
                                       List<ECardType> targetType,
                                       int numCardsToDraw
            ) : base(owningCard, cardCount, targetType, PlayerTargeting.PlayerOwner) {
            _numCardsToDraw = numCardsToDraw;
        }

        public override void InvokeEffect(GameController gameController) {
            base.InvokeEffect(gameController);

            if (CardTargets.Count > 0) {
                for (int i = 0; i < _numCardsToDraw; i++) {
                    gameController.PlayerDrawCard(OwningPlayer);
                }
            }
        }
    }
}
