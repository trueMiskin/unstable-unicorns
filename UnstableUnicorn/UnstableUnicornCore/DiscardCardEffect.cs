using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnstableUnicornCore {
    public sealed class DiscardCardEffect : AEffect {
        // card types which can be targeted
        List<ECardType> _allowedCardTypes;
        
        // number card to discard
        int _cardCount;
        public DiscardCardEffect(int cardCount, List<ECardType> targetType, Card owningCard) {
            _cardCount = cardCount;
            _allowedCardTypes = targetType;
            OwningCard = owningCard;
            TargetLocation = CardLocation.DiscardPile;
        }

        public override void InvokeEffect(ETriggerSource triggerSource, AEffect? effect, GameController gameController) {
            APlayer player = OwningCard.Player;
            for (int i = 0; i < _cardCount; i++) {
                Card card = player.WhichCardDiscard(_allowedCardTypes);
                if (card == null)
                    throw new InvalidOperationException($"Card was not selected");
                if (_allowedCardTypes.Contains(card.CardType))
                    throw new InvalidOperationException($"Card {card.Name} does not have allowed card type");
                if (!player.Hand.Remove(card))
                    throw new InvalidOperationException($"Card {card.Name} not in player hand!");
                card.MoveCard(gameController, null, TargetLocation);
            }
        }
    }
}
