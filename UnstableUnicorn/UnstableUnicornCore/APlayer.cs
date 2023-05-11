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
        /// <summary>
        /// Returns the permanent player index in the game
        /// </summary>
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

        /// <summary>
        /// Return avarage response time in ms
        /// </summary>
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

        /// <summary>
        /// When you need to decide which card should be sacrificed, then this method will be called
        /// </summary>
        /// <param name="number">Number cards to select</param>
        /// <param name="effect">The effect that needs the targets</param>
        /// <param name="cardsWhichCanBeSelected">List of available cards to select</param>
        /// <returns></returns>
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

        /// <summary>
        /// When you need to decide which card should be destroyed, then this method will be called
        /// </summary>
        /// <param name="number">Number cards to select</param>
        /// <param name="effect">The effect that needs the targets</param>
        /// <param name="cardsWhichCanBeSelected">List of available cards to select</param>
        /// <returns></returns>
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

        /// <summary>
        /// When you need to decide which card should be returned to owner's hand, then this method will be called
        /// </summary>
        /// <param name="number">Number cards to select</param>
        /// <param name="effect">The effect that needs the targets</param>
        /// <param name="cardsWhichCanBeSelected">List of available cards to select</param>
        /// <returns></returns>
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

        /// <summary>
        /// When you need to decide which card should be stolen, then this method will be called
        /// </summary>
        /// <param name="number">Number cards to select</param>
        /// <param name="effect">The effect that needs the targets</param>
        /// <param name="cardsWhichCanBeSelected">List of available cards to select</param>
        /// <returns></returns>
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
        /// <param name="number">Number cards to select</param>
        /// <param name="effect">The effect that needs the targets</param>
        /// <param name="cardsWhichCanBeSelected">List of available cards to select</param>
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

        /// <summary>
        /// When you need to decide which card should be saved, then this method will be called.
        /// 
        /// The method is called, for example, by <see cref="BaseSet.BlackKnightUnicorn"/> card
        /// </summary>
        /// <param name="number">Number cards to select</param>
        /// <param name="effect">The effect that needs the targets</param>
        /// <param name="cardsWhichCanBeSelected">List of available cards to select</param>
        /// <returns></returns>
        public List<Card> WhichCardsToSave(int number, AEffect effect, List<Card> cardsWhichCanBeSelected) {
            StopwatchStart();

            var ans = WhichCardsToSaveCore(number, effect, cardsWhichCanBeSelected);

            StopWatchStop();
            return ans;
        }
        protected abstract List<Card> WhichCardsToSaveCore(int number, AEffect effect, List<Card> cardsWhichCanBeSelected);

        /// <summary>
        /// When you need to decide when you need choose a player, then this method will be called
        /// </summary>
        /// <param name="number">Number players to select</param>
        /// <param name="effect">The effect that needs the targets</param>
        /// <param name="canChooseMyself"></param>
        /// <returns></returns>
        public List<APlayer> ChoosePlayers(int number, bool canChooseMyself, AEffect effect) {
            var players = new List<APlayer>(GameController.Players);
            if (!canChooseMyself)
                players.Remove(this);
            return ChoosePlayers(number, effect, players);
        }

        /// <summary>
        /// When you need to decide when you need choose a player, then this method will be called.
        /// </summary>
        /// <param name="number">Number players to select</param>
        /// <param name="effect">The effect that needs the targets</param>
        /// <param name="playersWhichCanBeSelected">List of available players to select</param>
        /// <returns></returns>
        public List<APlayer> ChoosePlayers(int number, AEffect effect, List<APlayer> playersWhichCanBeSelected) {
            StopwatchStart();

            var ans = ChoosePlayersCore(number, effect, playersWhichCanBeSelected);

            StopWatchStop();
            return ans;
        }
        protected abstract List<APlayer> ChoosePlayersCore(int number, AEffect effect, List<APlayer> playersWhichCanBeSelected);

        /// <summary>
        /// When you need to decide which card should you get, then this method will be called.
        /// 
        /// The method is called, for example, by the <see cref="BasicEffects.BringCardFromSourceOnTable"/> effect
        /// </summary>
        /// <param name="number">Number cards to select</param>
        /// <param name="effect">The effect that needs the targets</param>
        /// <param name="cardsWhichCanBeSelected">List of available cards to select</param>
        /// <returns></returns>
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

        /// <summary>
        /// When you need to decide which card should be moved, then this method will be called
        /// 
        /// The method is called, for example, by the <see cref="BasicEffects.MoveCardBetweenStables"/> effect
        /// </summary>
        /// <param name="number">Number cards to select</param>
        /// <param name="effect">The effect that needs the targets</param>
        /// <param name="cardsWhichCanBeSelected">List of available cards to select</param>
        /// <returns></returns>
        public List<Card> WhichCardsToMove(int number, AEffect effect, List<Card> cardsWhichCanBeSelected) {
            StopwatchStart();

            List<Card> ans = WhichCardsToMoveCore(number, effect, cardsWhichCanBeSelected);

            StopWatchStop();
            return ans;
        }
        protected abstract List<Card> WhichCardsToMoveCore(int number, AEffect effect, List<Card> cardsWhichCanBeSelected);

        /// <summary>
        /// When you need to decide which effect should be activated, then this method will be called
        /// </summary>
        /// <param name="effectsVariants">List of effects that can be activated</param>
        /// <returns></returns>
        public AEffect WhichEffectToSelect(List<AEffect> effectsVariants) {
            StopwatchStart();

            var ans = WhichEffectToSelectCore(effectsVariants);

            StopWatchStop();
            return ans;
        }
        protected abstract AEffect WhichEffectToSelectCore(List<AEffect> effectsVariants);

        /// <summary>
        /// When you need to decide if the effect should be activated, then this method will be called
        /// </summary>
        /// <param name="effect">The effect object that can be activated</param>
        /// <returns></returns>
        public bool ActivateEffect(AEffect effect) {
            StopwatchStart();

            var ans = ActivateEffectCore(effect);

            StopWatchStop();
            return ans;
        }
        protected abstract bool ActivateEffectCore(AEffect effect);
    }
}
