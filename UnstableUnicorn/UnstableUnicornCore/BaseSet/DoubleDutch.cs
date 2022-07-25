using System.Collections.Generic;
using UnstableUnicornCore.BasicEffects;

namespace UnstableUnicornCore.BaseSet {
    public class DoubleDutch : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return Card
                .Name("Double Dutch")
                .CardType(ECardType.Upgrade)
                .Text("If this card is in your Stable at the beginning of your turn, you may play 2 cards during your Action phase.")
                .TriggerEffect(
                    TriggerPredicates.IsItInYourStableAtTheBeginningOfYourTurn,
                    new List<ETriggerSource> { ETriggerSource.BeginningTurn },
                    (Card owningCard) => new PlayerCanPlayMoreCardsInOneTurn(owningCard, 2)
                );
        }
    }
}
