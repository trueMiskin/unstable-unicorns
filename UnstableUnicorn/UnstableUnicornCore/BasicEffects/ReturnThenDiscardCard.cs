using System.Collections.Generic;

namespace UnstableUnicornCore.BasicEffects {
    /// <summary>
    /// Text of effect: Choose any player. Return a card in that player's Stable to their hand. That player must DISCARD a card.
    /// 
    /// From wiki, this effect have requirement to select valid target (card which can be returned)
    /// </summary>
    public class ReturnThenDiscardCard : ReturnEffect {
        public ReturnThenDiscardCard(Card owningCard, List<ECardType> targetType) : base(owningCard, 1, targetType) {}

        public override void InvokeEffect(GameController gameController) {
            foreach (var card in CardTargets) {
                card.MoveCard(gameController, card.Player, TargetLocation);
                // card owner must discard a card
                gameController.AddNewEffectToChainLink(new DiscardEffect(card, 1, _allowedCardTypes, PlayerTargeting.PlayerOwner));
            }
        }

        public override bool MeetsRequirementsToPlay(GameController gameController) {
            return MeetsRequirementsToPlayInner(gameController);
        }
    }
}
