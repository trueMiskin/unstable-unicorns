namespace UnstableUnicornCore.BasicContinuousEffects {
    public class PlayerCantPlayUpgrades : AContinuousEffect {
        public PlayerCantPlayUpgrades(Card owningCard) : base(owningCard) {}

        public override bool IsCardPlayable(Card card, APlayer targetOwner) {
            if (card.Player != OwningPlayer)
                return true;

            return card.CardType != ECardType.Upgrade;
        }
    }
}
