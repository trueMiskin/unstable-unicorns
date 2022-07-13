using UnstableUnicornCore.BasicContinuousEffects;

namespace UnstableUnicornCore.BaseSet {
    public class BlindingLight : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return Card
                .Name("Blinding Light")
                .CardType(ECardType.Downgrade)
                .Text("Triggered effects of your Unicorn cards do not activate.")
                .ContinuousFactory((Card owningCard) => new TriggerEffectsCantBeActivated(owningCard));
        }
    }
}
