using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnstableUnicornCore.BasicEffects;

namespace UnstableUnicornCore.BaseSet {
    public class Americorn : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return Card
                .Name("Americorn")
                .CardType(ECardType.MagicUnicorn)
                .Text("When this card enters your Stable, choose any player. Pull a card from that player's hand.")
                .TriggerEffect(
                    (Card owningCard) =>
                    new TriggerEffect(
                        owningCard,
                        (AEffect? affect, Card? card) => owningCard == card,
                        new List<ETriggerSource>() { ETriggerSource.CardEnteredStable },
                        (Card _) => new PullOpponentsCardEffect(owningCard, cardCount: 1, numberSelectedPlayers: 1)
                    )
                );
        }
    }
}
