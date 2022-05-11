using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnstableUnicornCore {
    
    public abstract class APlayer {
        List<Func<ECardType, ECardType>> cardTypeTranformers = new();

        public List<Card> Hand       = new();
        public List<Card> Stable     = new();
        public List<Card> Upgrades   = new();
        public List<Card> Downgrades = new();
        internal GameController GameController { get; set; }

        /// <summary>
        /// Which card from should be played
        /// 
        /// If function return null then no card will be played
        /// </summary>
        /// <returns>Card to play or null</returns>
        public abstract Card? WhichCardToPlay();
        public abstract Card SacrificeCard();
        public abstract Card DestroyCard();
        public abstract Card StealCard();
        public abstract Card WhichCardDiscard();

        public void DrawCardHandler(Card drewCard) {
            drewCard.MoveCard(this, CardLocation.InHand);
            Hand.Add(drewCard);
        }

        /// <summary>
        /// Add card type transformer into list of transformers
        /// </summary>
        /// <param name="transformer"></param>
        public void AddCardTypeTransformer(Func<ECardType, ECardType> transformer)
            => cardTypeTranformers.Add(transformer);
        public bool RemoveCardTypeTransformer(Func<ECardType, ECardType> transformer)
            => cardTypeTranformers.Remove(transformer);
        public ICollection<Func<ECardType, ECardType>> CardTypeTransformers => cardTypeTranformers;

        /// <summary>
        /// Enter card on table of current player
        /// </summary>
        /// <param name="effect"></param>
        /// <param name="effectedCard"></param>
        public void CardEnterOnTable(AEffect effect, Card effectedCard) {

        }

        internal void CardPlayed(Card card) {
            if (!Hand.Remove(card))
                throw new InvalidOperationException($"Card {card.Name} not in player hand!");
            card.CardPlayed(this);
        }

        internal void CardDiscarded(Card card) {
            if(!Hand.Remove(card))
                throw new InvalidOperationException($"Card {card.Name} not in player hand!");
            card.MoveCard(null, CardLocation.DiscardPile);
        }
    }
}
