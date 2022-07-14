﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnstableUnicornCore.BasicEffects;

namespace UnstableUnicornCore.BaseSet {
    public class MysticalVortex : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return Card
                .Name("Mystical Vortex")
                .CardType(ECardType.Spell)
                .Text("Each player must DISCARD a card. Shuffle the discard pile into the deck.")
                .Cast((Card owningCard) => new AndEffect(owningCard,
                        new DiscardEffect(owningCard, 1, ECardTypeUtils.CardTarget, PlayerTargeting.EachPlayer),
                        new ShuffleDeckEffect(owningCard, addDiscardPileToPile: true)
                    )
                );
        }
    }
}
