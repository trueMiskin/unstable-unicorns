namespace UnstableUnicornCore.BasicContinuousEffects {
    public class CardCannotBeDestroyedByMagicCard : AContinuousEffect {
        public CardCannotBeDestroyedByMagicCard(Card owningCard) : base(owningCard) {}

        public override bool IsCardDestroyable(Card card, AEffect? byEffect) {
            if (byEffect == null)
                return true;

            if (byEffect.OwningCard.CardType == ECardType.Spell)
                return false;

            return true;
        }
    }
}
