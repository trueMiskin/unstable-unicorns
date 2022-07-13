using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnstableUnicornCore.BasicContinuousEffects {
    public class AnyOtherPlayerCantPlayBasicUnicorn : AContinuousEffect {
        public AnyOtherPlayerCantPlayBasicUnicorn(Card owningCard) : base(owningCard) {}

        public override bool IsCardPlayable(Card card, APlayer targetOwner) {
            if (card.CardType == ECardType.BasicUnicorn && targetOwner != OwningPlayer)
                return false;
            return true;
        }
    }
}
