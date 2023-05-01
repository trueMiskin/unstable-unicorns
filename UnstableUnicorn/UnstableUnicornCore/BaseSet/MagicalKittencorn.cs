using UnstableUnicornCore.BasicContinuousEffects;

namespace UnstableUnicornCore.BaseSet {
    /// <summary>
    /// Used 2nd edition effect
    /// </summary>
    public class MagicalKittencorn : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return EmptyCard
                .Name("Magical Kittencorn")
                .CardType(ECardType.MagicUnicorn)
                .Text("This card cannot be destroyed by Magic cards.")
                .ContinuousFactory((Card owningCard) => new CardCannotBeDestroyedByMagicCard(owningCard))
                ;
        }
    }
}
