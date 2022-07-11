using UnstableUnicornCore.BasicEffects;

namespace UnstableUnicornCore.BaseSet {
    public class BackKick : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return Card
                .Name("Back Kick")
                .CardType(ECardType.Spell)
                .Text("Choose any player. Return a card in that player's Stable to their hand. That player must DISCARD a card.")
                .Cast((Card owningCard) => new ReturnThenDiscardCard(owningCard, ECardTypeUtils.CardTarget));
        }
    }
}
