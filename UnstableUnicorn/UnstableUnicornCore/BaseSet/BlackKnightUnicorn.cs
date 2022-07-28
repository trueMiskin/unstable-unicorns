using System.Collections.Generic;
using UnstableUnicornCore.BasicEffects;

namespace UnstableUnicornCore.BaseSet {
    public class BlackKnightUnicorn : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return Card
                .Name("Black Knight Unicorn")
                .CardType(ECardType.MagicUnicorn)
                .Text("If 1 of your Unicorn cards would be destroyed, you may SACRIFICE this card instead.")
                .TriggerEffect(
                    (effect, causedCard, owningCard, controller) =>
                        TriggerPredicates.IfUnicornInYourStableWouldBeDestroyd(effect, causedCard, owningCard, controller) &&
                        !controller.CardsWhichAreTargeted.ContainsKey(owningCard),
                    new List<ETriggerSource> { ETriggerSource.ChangeTargeting },
                    (Card owningCard) => new ActivatableEffect(owningCard,
                        new SacrificeThisCardInsteadOtherCard(owningCard, ECardTypeUtils.UnicornTarget)
                    )
                );
        }
    }
}
