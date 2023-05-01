using System.Collections.Generic;
using UnstableUnicornCore.BasicEffects;

namespace UnstableUnicornCore.BaseSet {
    /// <summary>
    /// Used 2nd edition text but effect is same as second print
    /// </summary>
    public class ZombieUnicorn : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return EmptyCard
                .Name("Zombie Unicorn")
                .CardType(ECardType.MagicUnicorn)
                .Text("If this card is in your Stable at the beginning of your turn, you may DISCARD a Unicorn card, then bring a Unicorn card from the discard pile into your Stable. If you do immediately end your turn.")
                .TriggerEffect(
                    TriggerPredicates.IsItInYourStableAtTheBeginningOfYourTurn,
                    new List<ETriggerSource> { ETriggerSource.BeginningTurn },
                    (Card owningCard) =>
                        new ActivatableEffect(owningCard,
                            new ConditionalEffect(owningCard,
                                new DiscardEffect(owningCard, 1, ECardTypeUtils.UnicornTarget, PlayerTargeting.PlayerOwner),
                                new AndEffect(owningCard,
                                    new BringCardFromSourceOnTable(owningCard, 1, card => ECardTypeUtils.UnicornTarget.Contains(card.CardType), CardLocation.DiscardPile),
                                    new SkipToEndTurnPhaseEffect(owningCard)
                                )
                            )
                        )
                );
        }
    }
}
