namespace UnstableUnicornCore.BasicEffects {
    /// <summary>
    /// Actual player will take extra turn
    /// </summary>
    public class ExtraTurnEffect : AEffect {
        public ExtraTurnEffect(Card owningCard) : base(owningCard, 0 /* For this effect is not needed*/) { }

        public override void ChooseTargets(GameController gameController) {}

        public override void InvokeEffect(GameController gameController) {
            gameController.ThisPlayerTakeExtraTurn();
        }

        public override bool MeetsRequirementsToPlayInner(GameController gameController) => true;
    }
}
