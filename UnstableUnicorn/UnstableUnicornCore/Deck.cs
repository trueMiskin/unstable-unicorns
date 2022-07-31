using System;
using System.Collections.Generic;

namespace UnstableUnicornCore {
    public abstract class Deck {
        public abstract string Name { get; }

        public abstract IEnumerable<(CardTemplateSource card, int count)> BabyUnicorns();

        public abstract IEnumerable<(CardTemplateSource card, int count)> OtherCards();
    }
}
