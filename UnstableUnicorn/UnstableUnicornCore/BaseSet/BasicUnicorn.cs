namespace UnstableUnicornCore.BaseSet {
    public class BasicUnicorn : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return EmptyCard
                .Name("Basic Unicorn")
                .CardType(ECardType.BasicUnicorn)
                .Text("Just basic unicorn.");
        }
    }
}
