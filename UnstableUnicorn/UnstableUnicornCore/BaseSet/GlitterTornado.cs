using UnstableUnicornCore.BasicEffects;

namespace UnstableUnicornCore.BaseSet {
    public class GlitterTornado : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return Card
                .Name("Glitter Tornado")
                .CardType(ECardType.Spell)
                .Text("Return a card in each player's Stable (including yours) to their hand.")
                .Cast(
                    (Card owningCard) => new ReturnEffect(owningCard, 1, ECardTypeUtils.CardTarget, PlayerTargeting.EachPlayer)
                );
        }
    }
}
