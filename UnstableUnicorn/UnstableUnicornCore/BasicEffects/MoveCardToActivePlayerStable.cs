namespace UnstableUnicornCore.BasicEffects {
    /// <summary>
    /// This card is supposed to be triggered on <see cref="ETriggerSource.BeginningTurn"/>
    /// </summary>
    public class MoveCardToActivePlayerStable : AEffect {
        public MoveCardToActivePlayerStable(Card owningCard) : base(owningCard, 0) {
            TargetLocation = CardLocation.OnTable;
        }

        public override void ChooseTargets(GameController gameController) {}

        public override void InvokeEffect(GameController gameController) {
            OwningCard.MoveCard(gameController, gameController.ActualPlayerOnTurn, TargetLocation);
        }

        public override bool MeetsRequirementsToPlayInner(GameController gameController) => true;
    }
}
