using System.Collections.Generic;

namespace UnstableUnicornCore.BasicEffects {
    /// <summary>
    /// This effect must extend discard effect
    /// for good interaction with other effect.
    /// <br/>
    /// This effect should be called on trigger <see cref="ETriggerSource.ChangeTargeting"/>
    /// </summary>
    public class DiscardCardToProtectCard : DiscardEffect {
        public DiscardCardToProtectCard(Card owningCard, int cardCount, List<ECardType> targetType) : base(owningCard, cardCount, targetType, PlayerTargeting.PlayerOwner) {
        }

        public override void InvokeReactionEffect(GameController gameController, AEffect effect) {
            ChooseTargets(gameController);

            // removed saved card from target of effect
            effect.CardTargets.Remove(OwningCard);

            gameController.AddEffectToCurrentChainLink(this);
        }
    }
}
