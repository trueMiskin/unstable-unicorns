namespace UnstableUnicornCore.BasicContinuousEffects {
    public class PlayerCantPlayInstantCards : AContinuousEffect {
        public PlayerCantPlayInstantCards(Card owningCard) : base(owningCard) {}

        public override bool CanBePlayedInstantCards(APlayer player) => player != OwningPlayer;
    }
}
