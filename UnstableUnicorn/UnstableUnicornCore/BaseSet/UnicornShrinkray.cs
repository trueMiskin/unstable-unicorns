using UnstableUnicornCore.BasicEffects;

namespace UnstableUnicornCore.BaseSet {
    public class UnicornShrinkray : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return EmptyCard
                .Name("Unicorn Shrinkray")
                .CardType(ECardType.Spell)
                .Text("Choose any player. Move all of that player's Unicorn cards to the discard pile without triggering any of their effects, then bring the same number of Baby Unicorn cards from the Nursery directly into that player's Stable.")
                .Cast(
                    (Card owningCard) => new SwapUnicornsForBabyUnicorns(owningCard)
                );
        }
    }
}
