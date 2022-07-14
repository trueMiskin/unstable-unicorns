using System.Collections.Generic;

namespace UnstableUnicornCore.BasicEffects {
    /// <summary>
    /// This effect must extend sacrifice effect
    /// for good interaction with other effect.
    /// <br/>
    /// What if this card will have another effect if this
    /// card was sacrificed than do... (could be a little bit OP but why not)
    /// 
    /// This effect should be called on trigger <see cref="ETriggerSource.ChangeTargeting"/>
    /// but can be used also for self kill card (without using reaction part)
    /// </summary>
    public class SacrificeThisCardInsteadOtherCard : SacrificeEffect {
        public SacrificeThisCardInsteadOtherCard(Card owningCard, List<ECardType> targetType) : base(owningCard, 1, targetType) {
            CardTargets = new List<Card> { OwningCard };
        }

        public override void ChooseTargets(GameController gameController) { }

        public override void InvokeEffect(GameController gameController) {
            base.InvokeEffect(gameController);
        }

        public override void InvokeReactionEffect(GameController gameController, AEffect effect) {
            List<Card> cardsCanBeSelected = new();
            foreach (var card in effect.CardTargets)
                if (_allowedCardTypes.Contains(card.CardType) && card.Player == OwningPlayer)
                    cardsCanBeSelected.Add(card);

            var savedCards = OwningPlayer.WhichCardsToSave(1, effect, cardsCanBeSelected);

            // removed saved cards from target of effect
            foreach (var card in savedCards)
                effect.CardTargets.Remove(card);

            gameController.AddEffectToActualChainLink(this);
        }

        public override bool MeetsRequirementsToPlayInner(GameController gameController) => true;
    }
}
