using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace UnstableUnicornCore {

    public abstract class APlayer {
        public List<Card> Hand = new();
        public List<Card> Stable = new();
        public List<Card> Upgrades = new();
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

        private int playerIndex = -1;
        public int PlayerIndex {
            get {
                if (playerIndex == -1)
                    playerIndex = GameController.Players.IndexOf(this);
                return playerIndex;
            }
        }

        Stopwatch stopwatch = new Stopwatch();
        private long thinkTimeInMilis = 0;
        private int numberCalls = 0;
        public long AvarageResponse => thinkTimeInMilis / numberCalls;

        private void StopwatchStart() => stopwatch.Restart();
        private void StopWatchStop() {
            stopwatch.Stop();
            thinkTimeInMilis += stopwatch.ElapsedMilliseconds;
            numberCalls++;
        }

        /// <summary>
        /// Which card from should be played
        /// 
        /// If function return null then no card will be played
        /// </summary>
        /// <returns>Card to play or null</returns>
        public Card? WhichCardToPlay() {
            StopwatchStart();
            
            var ans = WhichCardToPlayCore();
            
            StopWatchStop();
            return ans;
        }
        protected abstract Card? WhichCardToPlayCore();

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
        public APlayer WhereShouldBeCardPlayed(Card card) {
            StopwatchStart();
            
            var ans = WhereShouldBeCardPlayedCore(card);
            
            StopWatchStop();
            return ans;
        }
        protected abstract APlayer WhereShouldBeCardPlayedCore(Card card);

        /// <summary>
        /// Return null if you don't want play instant card on stack
        /// or return instant card from your hand
        /// </summary>
        /// <param name="stack"></param>
        /// <returns></returns>
        public Card? PlayInstantOnStack(List<Card> stack) {
            StopwatchStart();
            
            var instants = Hand.FindAll(card => card.CardType == ECardType.Instant);

            Card? ans;
            // any instant card in hand?
            if (instants.Count == 0)
                 ans = null;
            else
                ans = PlayInstantOnStackCore(stack, instants);

            StopWatchStop();
            return ans;
        }
        protected abstract Card? PlayInstantOnStackCore(List<Card> stack, List<Card> availableInstantCards);

        public List<Card> WhichCardsToSacrifice(int number, AEffect effect, List<Card> cardsWhichCanBeSelected) {
            StopwatchStart();

            List<Card> ans;
            if (cardsWhichCanBeSelected.Count == 0)
                ans = new List<Card>();
            else
                ans = WhichCardsToSacrificeCore(number, effect, cardsWhichCanBeSelected);

            StopWatchStop();
            return ans;
        }
        protected abstract List<Card> WhichCardsToSacrificeCore(int number, AEffect effect, List<Card> cardsWhichCanBeSelected);

        public List<Card> WhichCardsToDestroy(int number, AEffect effect, List<Card> cardsWhichCanBeSelected) {
            StopwatchStart();

            List<Card> ans;
            if (cardsWhichCanBeSelected.Count == 0)
                ans = new List<Card>();
            else
                ans = WhichCardsToDestroyCore(number, effect, cardsWhichCanBeSelected);

            StopWatchStop();
            return ans;
        }
        protected abstract List<Card> WhichCardsToDestroyCore(int number, AEffect effect, List<Card> cardsWhichCanBeSelected);

        public List<Card> WhichCardsToReturn(int number, AEffect effect, List<Card> cardsWhichCanBeSelected) {
            StopwatchStart();

            List<Card> ans;
            if (cardsWhichCanBeSelected.Count == 0)
                ans = new List<Card>();
            else
                ans = WhichCardsToReturnCore(number, effect, cardsWhichCanBeSelected);

            StopWatchStop();
            return ans;
        }
        protected abstract List<Card> WhichCardsToReturnCore(int number, AEffect effect, List<Card> cardsWhichCanBeSelected);

        public List<Card> WhichCardsToSteal(int number, AEffect effect, List<Card> cardsWhichCanBeSelected) {
            StopwatchStart();

            List<Card> ans;
            if (cardsWhichCanBeSelected.Count == 0)
                ans = new List<Card>();
            else
                ans = WhichCardsToStealCore(number, effect, cardsWhichCanBeSelected);

            StopWatchStop();
            return ans;
        }
        protected abstract List<Card> WhichCardsToStealCore(int number, AEffect effect, List<Card> cardsWhichCanBeSelected);

        /// <summary>
        /// Which card should be discarded, effect could be null when this method is called from <see cref="GameController"/>
        /// when you have more cards on hand on end of your turn
        /// </summary>
        /// <param name="number"></param>
        /// <param name="effect"></param>
        /// <param name="cardsWhichCanBeSelected"></param>
        /// <returns></returns>
        public List<Card> WhichCardsToDiscard(int number, AEffect? effect, List<Card> cardsWhichCanBeSelected) {
            StopwatchStart();

            List<Card> ans;
            if (cardsWhichCanBeSelected.Count == 0)
                ans = new List<Card>();
            else
                ans = WhichCardsToDiscardCore(number, effect, cardsWhichCanBeSelected);

            StopWatchStop();
            return ans;
        }
        protected abstract List<Card> WhichCardsToDiscardCore(int number, AEffect? effect, List<Card> cardsWhichCanBeSelected);

        public List<Card> WhichCardsToSave(int number, AEffect effect, List<Card> cardsWhichCanBeSelected) {
            StopwatchStart();

            var ans = WhichCardsToSaveCore(number, effect, cardsWhichCanBeSelected);

            StopWatchStop();
            return ans;
        }
        protected abstract List<Card> WhichCardsToSaveCore(int number, AEffect effect, List<Card> cardsWhichCanBeSelected);

        public List<APlayer> ChoosePlayers(int number, bool canChooseMyself, AEffect effect) {
            var players = new List<APlayer>(GameController.Players);
            if (!canChooseMyself)
                players.Remove(this);
            return ChoosePlayers(number, effect, players);
        }
        public List<APlayer> ChoosePlayers(int number, AEffect effect, List<APlayer> playersWhichCanBeSelected) {
            StopwatchStart();

            var ans = ChoosePlayersCore(number, effect, playersWhichCanBeSelected);

            StopWatchStop();
            return ans;
        }
        protected abstract List<APlayer> ChoosePlayersCore(int number, AEffect effect, List<APlayer> playersWhichCanBeSelected);

        public List<Card> WhichCardsToGet(int number, AEffect effect, List<Card> cardsWhichCanBeSelected) {
            StopwatchStart();

            List<Card> ans;
            if (cardsWhichCanBeSelected.Count == 0)
                ans = new List<Card>();
            else
                ans = WhichCardsToGetCore(number, effect, cardsWhichCanBeSelected);

            StopWatchStop();
            return ans;
        }
        protected abstract List<Card> WhichCardsToGetCore(int number, AEffect effect, List<Card> cardsWhichCanBeSelected);

        public List<Card> WhichCardsToMove(int number, AEffect effect, List<Card> cardsWhichCanBeSelected) {
            StopwatchStart();

            List<Card> ans = WhichCardsToMoveCore(number, effect, cardsWhichCanBeSelected);

            StopWatchStop();
            return ans;
        }
        protected abstract List<Card> WhichCardsToMoveCore(int number, AEffect effect, List<Card> cardsWhichCanBeSelected);

        public AEffect WhichEffectToSelect(List<AEffect> effectsVariants) {
            StopwatchStart();

            var ans = WhichEffectToSelectCore(effectsVariants);

            StopWatchStop();
            return ans;
        }
        protected abstract AEffect WhichEffectToSelectCore(List<AEffect> effectsVariants);

        public bool ActivateEffect(AEffect effect) {
            StopwatchStart();

            var ans = ActivateEffectCore(effect);

            StopWatchStop();
            return ans;
        }
        protected abstract bool ActivateEffectCore(AEffect effect);
    }
}
