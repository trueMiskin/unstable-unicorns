namespace UnstableUnicornCore.BasicEffects {
    /// <summary>
    /// Draw Extra cards during draw phase
    /// 
    /// This effect doesn't extend draw effect because this
    /// effect doesn't do actual draw but only setting variable
    /// <see cref="GameController.DrawExtraCards"/>
    /// </summary>
    public class DrawExtraCards : AEffect {
        public DrawExtraCards(Card owningCard, int cardCount) : base(owningCard, cardCount) {}

        public override void ChooseTargets(GameController gameController) {}

        public override void InvokeEffect(GameController gameController) {
            gameController.DrawExtraCards += CardCount;
        }

        public override bool MeetsRequirementsToPlayInner(GameController gameController) => true;
    }
}
