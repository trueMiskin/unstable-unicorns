namespace UnstableUnicornCore.BasicContinuousEffects {
    public class UnicornTriggerEffectsCantBeActivated : AContinuousEffect {
        public UnicornTriggerEffectsCantBeActivated(Card owningCard) : base(owningCard) {}

        public override bool CanBeActivatedTriggerEffect(Card card, ECardType cardType) {
            if (card.Player != OwningPlayer)
                return true;

            return !IsBlockedTriggeringUnicornCards(card, cardType);
        }

        public static bool IsBlockedTriggeringUnicornCards(Card card, ECardType cardType) {
            if (!ECardTypeUtils.UnicornTarget.Contains(cardType))
                return false;

            // baby unicorn must be in Nursery or on table
            if (card._cardType == ECardType.BabyUnicorn)
                return false;

            return true;
        }
    }
}
