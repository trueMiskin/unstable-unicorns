using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnstableUnicornCore.BasicEffects {
    public class DrawEffect : AEffect {
        public DrawEffect(Card owningCard, int cardCount) : base(owningCard, cardCount) {}

        public override void ChooseTargets(GameController gameController) {}

        public override void InvokeEffect(ETriggerSource triggerSource, AEffect? effect, GameController gameController) {
            for (int i = 0; i < _cardCount; i++)
                gameController.PlayerDrawCard(OwningPlayer);
        }

        public override bool MeetsRequirementsToPlayInner(GameController gameController) => true;
    }
}
