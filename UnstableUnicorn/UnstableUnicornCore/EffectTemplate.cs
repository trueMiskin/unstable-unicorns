using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnstableUnicornCore {
    public abstract class EffectTemplate {
        public Card OwningCard { get; set; }
        public List<ECardType> TargetTypes { get; set; }
        public int NumberActivations;

        public void Activate(GameController gameController) {
            if (!IsRequirementsMet(gameController))
                throw new InvalidOperationException("Requirements are not met for activating this effects");
            
            ActivationImplementation(gameController);
        }

        protected abstract void ActivationImplementation(GameController gameController);

        public abstract bool IsRequirementsMet(GameController gameController);
    }

    public class DestroyEffectTemplate : EffectTemplate {
        public DestroyEffectTemplate(int numberActivations, Card owningCard) {
            NumberActivations = numberActivations;
            OwningCard = owningCard;
        }

        protected override void ActivationImplementation(GameController gameController) {
            for (int i = 0; i < NumberActivations; i++) {
                // choosing card as target
                Card card = OwningCard.Player.WhichCardToDestroy();
                if (!IsValidTarget(gameController, card))
                    throw new InvalidOperationException("Selected own card or card which is not on table");

                // TODO: Check chain link, if card is not selected

                gameController.AddNewEffectToChainLink( new DestroyEffect(OwningCard, card) );
            }
        }

        private bool IsValidTarget(GameController gameController, Card card) {
            if (!TargetTypes.Contains(card.CardType))
                return false;

            if (card.Player == null)
                return false;

            if (card.Player == OwningCard.Player || card.Location != CardLocation.OnTable)
                return false;

            foreach (var i in gameController.ContinuousEffects)
                if (!i.AreUnicornsDestroyable(card.Player))
                    return false;
            
            return true;    
        }

        public override bool IsRequirementsMet(GameController gameController) {
            int num = 0;
            foreach(APlayer player in gameController.Players) {
                if (OwningCard.Player == player)
                    continue;

                foreach (Card c in player.Stable)
                    if (IsValidTarget(gameController, c))
                        num++;
                foreach (Card c in player.Upgrades)
                    if (IsValidTarget(gameController, c))
                        num++;
                foreach (Card c in player.Downgrades)
                    if (IsValidTarget(gameController, c))
                        num++;

                if (num >= NumberActivations)
                    break;
            }
            return num >= NumberActivations;
        }
    }
}
