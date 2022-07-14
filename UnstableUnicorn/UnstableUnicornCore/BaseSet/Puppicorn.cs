using System.Collections.Generic;
using UnstableUnicornCore.BasicEffects;

namespace UnstableUnicornCore.BaseSet {
    public class Puppicorn : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return Card
                .Name("Puppicorn")
                .CardType(ECardType.MagicUnicorn)
                .Text("Each time any player begins their turn, move this card to that player's Stable. This card cannot be sacrificed or destroyed.")
                .TriggerEffect(
                    (_, _, _, _) => true,
                    new List<ETriggerSource> { ETriggerSource.BeginningTurn },
                    (Card owningCard) => new MoveCardToActivePlayerStable(owningCard)
                )
                .CantBeSacrificed()
                .CantBeDestroyed();
        }
    }
}
