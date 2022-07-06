using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnstableUnicornCore {
    public abstract class CardTemplateSource {
        public CardTemplate Card { get { return new CardTemplate(); } }

        public abstract CardTemplate GetCardTemplate();
    }
}
