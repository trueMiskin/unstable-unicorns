namespace UnstableUnicornCore.BaseSet {
    public class Narwhal : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return EmptyCard
                .Name("Narwhal")
                .CardType(ECardType.BasicUnicorn)
                .Text("Just basic unicorn.");
        }
    }
}
