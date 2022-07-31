using System;
using System.Collections.Generic;

namespace UnstableUnicornCore {
    
    public abstract class APlayer {
        public List<Card> Hand       = new();
        public List<Card> Stable     = new();
        public List<Card> Upgrades   = new();
        public List<Card> Downgrades = new();

        private GameController? gameController;
        public GameController GameController {
            get {
                if (gameController == null)
                    throw new InvalidOperationException("Internal error: GameController should set this variable in constructor!");
                return gameController;
            }
            set => gameController = value;
        }

        /// <summary>
        /// Which card from should be played
        /// 
        /// If function return null then no card will be played
        /// </summary>
        /// <returns>Card to play or null</returns>
        public abstract Card? WhichCardToPlay();

        /// <summary>
        /// When you decide to play a card, then this method will be called
        /// <br/>
        /// Cards can be played to any player's stable, but if card is spell,
        /// then "target" owner must be player who own this card (you)
        /// <br/>
        /// In <see cref="WhichCardToPlay"/> you could decide what and where to play
        /// and then here return cached data.
        /// To this method is passed card which you decided to play,
        /// but to method will be always passed last card which you returned from
        /// <see cref="WhichCardToPlay"/>
        /// </summary>
        /// <param name="card">Card which you decided to play</param>
        /// <returns>Target owner</returns>
        public abstract APlayer WhereShouldBeCardPlayed(Card card);

        /// <summary>
        /// Return null if you don't want play instant card on stack
        /// or return instant card from your hand
        /// </summary>
        /// <param name="stack"></param>
        /// <returns></returns>
        public abstract Card? PlayInstantOnStack(List<Card> stack);
        public abstract List<Card> WhichCardsToSacrifice(int number, AEffect effect, List<Card> cardsWhichCanBeSelected);
        public abstract List<Card> WhichCardsToDestroy(int number, AEffect effect, List<Card> cardsWhichCanBeSelected);

        public abstract List<Card> WhichCardsToReturn(int number, AEffect effect, List<Card> cardsWhichCanBeSelected);
        public abstract List<Card> WhichCardsToSteal(int number, AEffect effect, List<Card> cardsWhichCanBeSelected);
        public abstract List<Card> WhichCardsToDiscard(int number, AEffect effect, List<Card> cardsWhichCanBeSelected);

        public abstract List<Card> WhichCardsToSave(int number, AEffect effect, List<Card> cardsWhichCanBeSelected);

        public List<APlayer> ChoosePlayers(int number, bool canChooseMyself, AEffect effect) {
            var players = new List<APlayer>(GameController.Players);
            if (!canChooseMyself)
                players.Remove(this);
            return ChoosePlayers(number, effect, players);
        }

        public abstract List<APlayer> ChoosePlayers(int number, AEffect effect, List<APlayer> playersWhichCanBeSelected);
        public abstract List<Card> WhichCardsToGet(int number, AEffect effect, List<Card> cardsWhichCanBeSelected);
        public abstract List<Card> WhichCardsToMove(int number, AEffect effect, List<Card> cardsWhichCanBeSelected);
        public abstract AEffect WhichEffectToSelect(List<AEffect> effectsVariants);
        public abstract bool ActivateEffect(AEffect effect);
    }
}
