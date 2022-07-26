namespace UnstableUnicornCore.BaseSet {
    public class Narwhal : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return Card
                .Name("Narwhal")
                .CardType(ECardType.BasicUnicorn)
                .Text("Just basic unicorn.");
        }
    }
}
