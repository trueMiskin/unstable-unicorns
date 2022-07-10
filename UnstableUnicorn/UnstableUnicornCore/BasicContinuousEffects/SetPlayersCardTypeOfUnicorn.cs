namespace UnstableUnicornCore.BasicContinuousEffects {
    public class SetPlayersCardTypeOfUnicorn : AContinuousEffect {
        private ECardType _newCardType;
        public SetPlayersCardTypeOfUnicorn(Card owningCard, ECardType newCardType) : base(owningCard) {
            _newCardType = newCardType;
        }

        public override ECardType GetCardType(ECardType actualCardType, APlayer playerOwner) {
            if (OwningPlayer != playerOwner)
                return actualCardType;

            if (ECardTypeUtils.UnicornTarget.Contains(actualCardType))
                return _newCardType;

            return actualCardType;
        }
    }
}
