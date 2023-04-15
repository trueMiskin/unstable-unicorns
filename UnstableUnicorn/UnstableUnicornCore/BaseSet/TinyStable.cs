using System.Collections.Generic;
using UnstableUnicornCore.BasicEffects;

namespace UnstableUnicornCore.BaseSet {
    public class TinyStable : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return EmptyCard
                .Name("Tiny Stable")
                .CardType(ECardType.Downgrade)
                .Text("If at any time you have more than 5 Unicorns in your Stable, SACRIFICE a Unicorn card.")
                .TriggerEffect(
                    TriggerPredicates.WhenYourStableHaveMoreThanFiveUnicorns,
                    // max unicorns is 7 -> end game, so this card can be played
                    // when stable have max 6 unicorns
                    new List<ETriggerSource> { ETriggerSource.CardEnteredStable },
                    (Card owningCard) => new SacrificeEffect(owningCard, 1, ECardTypeUtils.UnicornTarget)
                );
        }
    }
}
