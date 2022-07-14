namespace UnstableUnicornCore.BasicEffects {
    public class SkipToEndTurnPhaseEffect : AEffect {
        public SkipToEndTurnPhaseEffect(Card owningCard) : base(owningCard, 0) {}

        public override void ChooseTargets(GameController gameController) {}

        public override void InvokeEffect(GameController gameController) {
            gameController.SkipToEndTurnPhase = true;
        }

        public override bool MeetsRequirementsToPlayInner(GameController gameController) => true;
    }
}
