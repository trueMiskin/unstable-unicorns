using System;
using System.Collections.Generic;
using System.Linq;
using UnstableUnicornCore.BasicEffects;

namespace UnstableUnicornCore.BaseSet {
    public class Americorn : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return Card
                .Name("Americorn")
                .CardType(ECardType.MagicUnicorn)
                .Text("When this card enters your Stable, choose any player. Pull a card from that player's hand.")
                .Cast(
                    (Card owningCard) => new PullOpponentsCardEffect(owningCard, cardCount: 1, numberSelectedPlayers: 1)
                );
        }
    }
}
