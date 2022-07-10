using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnstableUnicornCore.BasicContinuousEffects {
    public class AnyOtherPlayerCantPlayBasicUnicorn : AContinuousEffect {
        public AnyOtherPlayerCantPlayBasicUnicorn(Card owningCard) : base(owningCard) {}

        public override bool IsCardPlayable(APlayer player, Card card) {
            if (card.CardType == ECardType.BasicUnicorn && player != OwningPlayer)
                return false;
            return true;
        }
    }
}
