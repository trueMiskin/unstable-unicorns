using System.Collections.Generic;
using UnstableUnicornCore.BasicEffects;

namespace UnstableUnicornCore.BaseSet {
    public class Rhinocorn : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return Card
                .Name("Rhinocorn")
                .CardType(ECardType.MagicUnicorn)
                .Text("If this card is in your Stable at the beginning of your turn, you may DESTROY a Unicorn card. If you do, immediately skip to your End of Turn phase.")
                .TriggerEffect(
                    TriggerPredicates.IsItInYourStableAtTheBeginningOfYourTurn,
                    new List<ETriggerSource> { ETriggerSource.BeginningTurn },
                    (Card owningCard) => new ActivatableEffect(owningCard,
                        (_) => new AndEffect(owningCard,
                            new DestroyEffect(owningCard, 1, ECardTypeUtils.UnicornTarget),
                            new SkipToEndTurnPhaseEffect(owningCard)
                        )
                    )
                );
        }
    }
}
