using System;
using System.Collections.Generic;
using UnstableUnicornCore.BasicEffects;

namespace UnstableUnicornCore.BaseSet {
    public class ResetButton : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return Card
                .Name("Reset Button")
                .CardType(ECardType.Spell)
                .Text("Each player must SACRIFICE all Upgrade and Downgrade cards. Shuffle the discard pile into the deck.")
                .Cast((Card owningCard) => new AndEffect(owningCard,
                        new SacrificeEffect(owningCard, Int32.MaxValue, new List<ECardType> { ECardType.Upgrade, ECardType.Downgrade}, PlayerTargeting.EachPlayer),
                        new ShuffleDeckEffect(owningCard, addDiscardPileToPile: true)
                    )
                );
        }
    }
}
