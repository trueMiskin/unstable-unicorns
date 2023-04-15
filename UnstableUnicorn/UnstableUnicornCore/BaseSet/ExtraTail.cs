using System.Collections.Generic;
using UnstableUnicornCore.BasicEffects;

namespace UnstableUnicornCore.BaseSet {
    /// <summary>
    /// Used original (first print) version card effect
    /// </summary>
    public class ExtraTail : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return EmptyCard
                .Name("Extra Tail")
                .CardType(ECardType.Upgrade)
                .Text("You must have a Basic Unicorn in your Stable in order to play this card. If this card is in your Stable at the beginning of your turn, you may DRAW an extra card.")
                .RequiredBasicUnicornInStableToPlay()
                .TriggerEffect(
                    TriggerPredicates.IsItInYourStableAtTheBeginningOfYourTurn,
                    new List<ETriggerSource> { ETriggerSource.BeginningTurn },
                    (Card owningCard) => new ActivatableEffect(owningCard, new DrawExtraCards(owningCard, 1))
                );
        }
    }
}
