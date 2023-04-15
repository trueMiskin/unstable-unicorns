using UnstableUnicornCore.BasicContinuousEffects;

namespace UnstableUnicornCore.BaseSet {
    public class Pandamonium : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return EmptyCard
                .Name("Pandamonium")
                .CardType(ECardType.Downgrade)
                .Text("All of your Unicorns are considered Pandas. Cards that affect Unicorn cards do not affect your Pandas.")
                .ContinuousFactory((Card owningCard) =>
                    new SetPlayersCardTypeOfUnicorn(owningCard, ECardType.Panda)
                );
        }
    }
}
