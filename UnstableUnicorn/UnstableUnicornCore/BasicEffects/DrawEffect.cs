namespace UnstableUnicornCore.BasicEffects {
    public sealed class DrawEffect : AEffect {
        public DrawEffect(Card owningCard, int cardCount) : base(owningCard, cardCount) {}

        public override void ChooseTargets(GameController gameController) {}

        public override void InvokeEffect(GameController gameController) {
            for (int i = 0; i < _cardCount; i++)
                gameController.PlayerDrawCard(OwningPlayer);
        }

        public override bool MeetsRequirementsToPlayInner(GameController gameController) => true;
    }
}
