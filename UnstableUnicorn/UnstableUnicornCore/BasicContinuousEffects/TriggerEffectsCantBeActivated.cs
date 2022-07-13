using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnstableUnicornCore.BasicContinuousEffects {
    public class TriggerEffectsCantBeActivated : AContinuousEffect {
        public TriggerEffectsCantBeActivated(Card owningCard) : base(owningCard) {}

        public override bool CanBeActivatedTriggerEffect(Card card, ECardType cardType) {
            if (card.Player != OwningPlayer)
                return true;

            if (!ECardTypeUtils.UnicornTarget.Contains(cardType))
                return true;

            if (cardType == ECardType.BabyUnicorn)
                return true;

            return false;
        }
    }
}
