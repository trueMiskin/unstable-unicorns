using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnstableUnicornCore {
    
    public abstract class APlayer {
        public List<Card> Hand       = new();
        public List<Card> Stable     = new();
        public List<Card> Upgrades   = new();
        public List<Card> Downgrades = new();
        public GameController GameController { get; set; }

        /// <summary>
        /// Which card from should be played
        /// 
        /// If function return null then no card will be played
        /// </summary>
        /// <returns>Card to play or null</returns>
        public abstract Card? WhichCardToPlay();
        public abstract List<Card> WhichCardsToSacrifice(int number, List<ECardType> allowedCardTypes);
        public abstract List<Card> WhichCardsToDestroy(int number, List<ECardType> allowedCardTypes);

        public abstract List<Card> WhichCardsToReturn(int number, AEffect effect, List<Card> cardsWhichCanBeSelected);
        public abstract List<Card> WhichCardsToSteal(int number, AEffect effect, List<Card> cardsWhichCanBeSelected);
        public abstract List<Card> WhichCardsToDiscard(int number, List<ECardType> allowedCardTypes);

        public abstract List<Card> WhichCardsToSave(int number, AEffect effect, List<Card> cardsWhichCanBeSelected);
        public abstract List<APlayer> ChoosePlayers(int number, bool canChooseMyself, AEffect effect);
        public abstract List<Card> WhichCardsToGet(int number, AEffect effect, List<Card> cardsWhichCanBeSelected);
        public abstract List<Card> WhichCardsToMove(int number, AEffect effect, List<Card> cardsWhichCanBeSelected);
        public abstract AEffect WhichEffectToSelect(List<AEffect> effectsVariants);
        public abstract bool ActivateEffect(AEffect effect);
        
        /// <summary>
        /// Enter card on table of current player
        /// </summary>
        /// <param name="effect"></param>
        /// <param name="effectedCard"></param>
        public void CardEnterOnTable(AEffect effect, Card effectedCard) {

        }
    }
}
