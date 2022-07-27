using System;
using System.Collections.Generic;

namespace UnstableUnicornCore {
    public abstract class Deck {
        public abstract IEnumerable<(CardTemplateSource card, int count)> BabyUnicorns();

        public abstract IEnumerable<(CardTemplateSource card, int count)> OtherCards();
    }
}
