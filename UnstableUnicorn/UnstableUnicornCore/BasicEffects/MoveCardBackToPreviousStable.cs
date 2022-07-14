namespace UnstableUnicornCore.BasicEffects {
    public class MoveCardBackToPreviousStable : AEffect {
        /// <summary>
        /// Helper effect which return card back to previous stable
        /// 
        /// Used for effects which says: Steal unicorn then return this card back when....
        /// </summary>
        /// <param name="owningCard"></param>
        /// <param name="previousPlayer"></param>
        public MoveCardBackToPreviousStable(Card owningCard, APlayer previousPlayer) : base(owningCard, 0) {
            CardTargets.Add(owningCard);
            TargetOwner = previousPlayer;
            TargetLocation = CardLocation.OnTable;
        }
        public override void ChooseTargets(GameController gameController) { }

        public override void InvokeEffect(GameController gameController) {
            foreach (var card in CardTargets)
                card.MoveCard(gameController, TargetOwner, TargetLocation);
        }

        public override bool MeetsRequirementsToPlayInner(GameController gameController) => true;
    }
}
