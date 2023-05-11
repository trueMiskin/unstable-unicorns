namespace UnstableUnicornCore {
    /// <summary>
    /// Helper class for easy creation of the card by the fluent syntax
    /// </summary>
    public abstract class CardTemplateSource {
        public CardTemplate EmptyCard { get { return new CardTemplate(); } }

        /// <summary>
        /// Returns implemented a CardTemplate
        /// </summary>
        /// <returns></returns>
        public abstract CardTemplate GetCardTemplate();
    }
}
