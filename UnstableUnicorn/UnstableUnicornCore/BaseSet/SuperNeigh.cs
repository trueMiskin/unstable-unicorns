﻿using UnstableUnicornCore.BasicEffects;

namespace UnstableUnicornCore.BaseSet {
    public class SuperNeigh : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return Card
                .Name("Super Neigh")
                .CardType(ECardType.Instant)
                .Text("Play this card when any other player tries to play a card. Stop that player's card from being played and send it to the discard pile. This card cannot be Neigh'd.")
                .CantBeNeigh()
                .Cast(
                    (Card owningCard) => new NeighEffect(owningCard)
                );
        }
    }
}