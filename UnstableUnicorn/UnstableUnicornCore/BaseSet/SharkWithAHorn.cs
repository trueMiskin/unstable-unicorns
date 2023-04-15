using UnstableUnicornCore.BasicEffects;

namespace UnstableUnicornCore.BaseSet {
    public class SharkWithAHorn : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return EmptyCard
                .Name("Shark With A Horn")
                .CardType(ECardType.MagicUnicorn)
                .Text("When this card enters your Stable, you may SACRIFICE this card. If you do, DESTROY a Unicorn card.")
                .Cast((Card owningCard) => new ActivatableEffect(owningCard,
                        // if done sacrifice then destroy, TODO:  maybe change implementation of ConditionalEffect
                        new ConditionalEffect(owningCard,
                            new SacrificeThisCardInsteadOtherCard(owningCard, ECardTypeUtils.CardTarget /* */),
                            new DestroyEffect(owningCard, 1, ECardTypeUtils.UnicornTarget)
                        )
                    )
                );
        }
    }
}
