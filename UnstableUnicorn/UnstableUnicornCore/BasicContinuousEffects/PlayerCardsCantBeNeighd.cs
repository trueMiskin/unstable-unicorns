namespace UnstableUnicornCore.BasicContinuousEffects {
    public class PlayerCardsCantBeNeighd : AContinuousEffect {
        public PlayerCardsCantBeNeighd(Card owningCard) : base(owningCard) {}

        public override bool IsCardNeighable(Card card) => card.Player != OwningPlayer;
    }
}
