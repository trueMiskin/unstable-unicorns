//#define DEBUG_PRINT
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnstableUnicornCore.BasicEffects;

namespace UnstableUnicornCore
{
    public interface IGameController {
        void SimulateGame();
    }

    public class GameController : IGameController, IPublisher {
        public List<TurnLog> GameLog = new();
        private TurnLog? _turnLog;
        public VerbosityLevel Verbosity { get; private set; }
        public EGameState State { get; internal set; } = EGameState.NotStarted;
        private List<GameResult>? _gameResults;
        public List<GameResult> GameResults {
            get {
                if (State != EGameState.Ended)
                    throw new InvalidOperationException("Game doesn't ended!!!");
                if (_gameResults == null)
                    throw new InvalidOperationException("Game ended but no game result???");

                return _gameResults;
            }
        }

        /// <summary>
        /// All cards in game - helpful when you want reference specific card but
        /// without storing specific reference in effect -> be state cloneable
        /// </summary>
        internal List<Card> _allCards = new();
        public Random Random { get; set; }
        public List<Card> Pile;
        public List<Card> DiscardPile = new();
        public List<Card> Nursery;
        public List<APlayer> Players;
        public List<AContinuousEffect> ContinuousEffects = new();

        public CardVisibilityTracker CardVisibilityTracker { get; private set; }
        private Dictionary<ETriggerSource, List<TriggerEffect>> EventsPool = new();
        private List<TriggerEffect> triggersToRemove = new();

        /// <summary>
        /// Actual chain link is link which is already in proccess of resolving
        /// 
        /// Next chain link is link which will follow in next iteration of resolving
        /// </summary>
        private List<AEffect> _actualChainLink = new();
        private List<AEffect> _nextChainLink = new();

        /// <summary>
        /// Stack for resolving instant cards
        /// </summary>
        public List<Card> Stack = new();
        public List<AEffect> ActualChainLink => _actualChainLink;
        public List<AEffect> NextChainLink => _nextChainLink;

        public APlayer ActualPlayerOnTurn { get; set; }
        public int MaxCardsToPlayInOneTurn { get; set; } = 1;
        public int DrawExtraCards { get; set; } = 0;
        public bool SkipToEndTurnPhase { get; set; } = false;

        public Dictionary<Card, AEffect> CardsWhichAreTargeted { get; set; } = new();

        private bool _willTakeExtraTurn = false;
        private int _playerOnTurn = 0, _turnNumber = 1;
        public int TurnNumber => _turnNumber;
        private int _debugIndentation = 0;
        public GameController(List<Card> pile,
                              List<Card> nursery,
                              List<APlayer> players,
                              int seed = 42,
                              VerbosityLevel verbosity = VerbosityLevel.None,
                              bool shufflePlayers = true) {
            Random = new Random(seed);
            Verbosity = verbosity;

            _allCards.AddRange(pile);
            _allCards.AddRange(nursery);
            
            Pile = pile.Shuffle(Random);
            Nursery = new List<Card>( nursery );
            CardVisibilityTracker = new(players);

            foreach (var babyUnicorn in Nursery) {
                babyUnicorn.Player = null;
                babyUnicorn.Location = CardLocation.Nursery;
            }

            foreach(APlayer p in players)
                p.GameController = this;

            if (shufflePlayers)
                Players = new List<APlayer>( players ).Shuffle(Random);
            else
                Players = new List<APlayer>( players );

            // setting for unit tests
            ActualPlayerOnTurn = Players[0];
        }

        [Conditional("DEBUG_PRINT")]
        private void DebugPrint(String text) {
            Console.Write(String.Join("", Enumerable.Repeat("*", _debugIndentation)));
            Console.WriteLine(text);
        }

        public void SimulateGame() {
            if (State == EGameState.NotStarted) {
                // set up game
                foreach (var player in Players) {
                    PlayerGetBabyUnicornOnTable(player);
                    PlayerDrawCards(player, 5);
                }
                State = EGameState.Running;
            }

            // game
            while (State != EGameState.Ended) {
                DebugPrint($"Player on turn {_playerOnTurn}, actual turn: {_turnNumber}");
                DebugPrint("------> Start turn <-------");

                APlayer player = Players[_playerOnTurn];

                SimulateOneTurn(player);

                if (State == EGameState.Ended) break;
                
                if (_willTakeExtraTurn)
                    _willTakeExtraTurn = false;
                else
                    _playerOnTurn = (_playerOnTurn + 1) % Players.Count;
                _turnNumber++;

                DebugPrint("------> End turn <-------");
            }

            var scoreBoard = from player in Players
                                let unicornValue = player.Stable.Sum(card => card.UnicornValue)
                                let unicornLen = player.Stable.Sum(card => card.Name.Replace(" ", string.Empty).Length)
                                select (unicornValue, unicornLen, player)
                                ;
            _gameResults = scoreBoard
                .OrderByDescending(item => item.unicornValue)
                .ThenByDescending(item => item.unicornLen)
                //.OrderByDescending(item => item.unicornLen)
                .Select(item => new GameResult(Players.IndexOf(item.player), item.player, item.unicornValue, item.unicornLen))
                .ToList();
        }

        private int _cardIdx = 0, _playersIterIdx = 0;
        private bool _cardSelected = false, _targetPlayerSelected = false, _stackResolved = false;
        private bool _cardPlayed = false, _cardResolved = false;
        private bool _beforeTurnStarted = false, _publishedOnBeginningTurn = false, _beginningTurnResolved = false;
        private bool _publishedOnEndTurn = false, _endTurnResolved = false;
        private Card? _card;
        private APlayer? _targetPlayer;

        private PlayedCardLog? _playedCardLog;
        internal void SimulateOneTurn(APlayer playerOnTurn) {
            if (!_beforeTurnStarted) {
                SkipToEndTurnPhase = false;
                MaxCardsToPlayInOneTurn = 1;
                DrawExtraCards = 0;
                ActualPlayerOnTurn = playerOnTurn;

                _beforeTurnStarted = true;

                if (Verbosity == VerbosityLevel.All)
                    _turnLog = new TurnLog(TurnNumber, playerOnTurn);
            }

            OnBeginTurn(playerOnTurn);

            for (; _cardIdx < MaxCardsToPlayInOneTurn && State != EGameState.Ended; _cardIdx++) {
                if (SkipToEndTurnPhase)
                    break;

                if (!_cardSelected) {
                    _card = playerOnTurn.WhichCardToPlay();
                    _cardSelected = true;

                    if (_card != null) {
                        if (!playerOnTurn.Hand.Remove(_card))
                            throw new InvalidOperationException($"Card {_card.Name} not in player hand!");
                        if (_card.CardType == ECardType.Instant)
                            throw new InvalidOperationException("Instant card cannot be played this way.");
                    }
                }

                if (_card == null) {
                    if(_cardIdx == 0)
                        PlayerDrawCard(playerOnTurn);

                    if (Verbosity == VerbosityLevel.All) {
                        _playedCardLog = new PlayedCardLog(_card);
                    }

                    _cardIdx = int.MaxValue; _cardSelected = false;
                    break;
                } else {
                    // choose target player and check if card can be played
                    // before resolving the stack

                    if (!_targetPlayerSelected) {
                        _targetPlayer = playerOnTurn.WhereShouldBeCardPlayed(_card);

                        if (!_card.CanBePlayed(_targetPlayer))
                            throw new InvalidOperationException(Card.CardCannotBePlayed);
                        _targetPlayerSelected = true;

                        Stack = new List<Card> { _card };

                        if (Verbosity == VerbosityLevel.All)
                            _playedCardLog = new PlayedCardLog(_card, _targetPlayer);
                    }
                    
                    Debug.Assert(_targetPlayer != null);
                    
                    while (Stack.Count != 0 && !_stackResolved) {
                        Card topCard = Stack[Stack.Count - 1];
                        List<Card> instantCards = new();

                        // if card cannot be neigh -> resolve this card
                        if (topCard.CanBeNeigh()) {
                            // check if any player wants to play instant card
                            for (; _playersIterIdx < Players.Count; _playersIterIdx++) {
                                var player = Players[_playersIterIdx];
                                var cardToPlayOnStack = player.PlayInstantOnStack(Stack);

                                // TODO: move CanPlayInstantCards to player class
                                if (cardToPlayOnStack == null || !cardToPlayOnStack.CanPlayInstantCards())
                                    continue;
                                if (cardToPlayOnStack.CardType != ECardType.Instant || !player.Hand.Contains(cardToPlayOnStack))
                                    throw new InvalidOperationException("Selected none instant card or card is not in your hand.");

                                instantCards.Add(cardToPlayOnStack);
                            }
                            _playersIterIdx = 0;
                        }

                        if (instantCards.Count == 0) {
                            if (Verbosity == VerbosityLevel.All)
                                _playedCardLog!.StackResolve.Add(new StackLog(StackTypeLog.Resolved, Stack[^1]));

                            if (Stack.Count == 1)
                                break;

                            // resolve last card in chain link
                            topCard.ExecuteInstant(this);
                        } else {
                            // move card from hand
                            int firstPlayedCard = Random.Next(instantCards.Count);
                            instantCards[firstPlayedCard].PlayedInstant(Stack);

                            if (Verbosity == VerbosityLevel.All)
                                _playedCardLog!.StackResolve.Add(new StackLog(StackTypeLog.Reacted, Stack[^1]));
                        }
                    }
                    _stackResolved = true;
                    // stack chain resolve

                    // if card is played -> was not neigh
                    if (Stack.Count == 1) {
                        DebugPrint($"Played {_card.Name}");

                        if (!_cardPlayed) {
                            _card.CardPlayed(this, _targetPlayer);
                            
                            _cardPlayed = true;
                        }

                        if (!_cardResolved) {
                            ResolveChainLink(_playedCardLog?.CardResolve);
                            
                            _cardResolved = true;
                        }
                    }
                }

                if (Verbosity == VerbosityLevel.All)
                    _turnLog?.CardPlaying.Add(_playedCardLog!);

                _card = null; _targetPlayer = null;
                _cardSelected = _targetPlayerSelected = _stackResolved = false;
                _cardPlayed = _cardResolved = false;
            }

            if (State != EGameState.Ended)
                OnEndTurn(playerOnTurn);

            if (Verbosity == VerbosityLevel.All)
                GameLog.Add(_turnLog!);

            if (State == EGameState.Ended) return;

            // reset all state variables
            _cardIdx = 0;
            _beforeTurnStarted = _publishedOnBeginningTurn = _beginningTurnResolved = false;
            _publishedOnEndTurn = _endTurnResolved = false;
        }

        public void ThisPlayerTakeExtraTurn() => _willTakeExtraTurn = true;

        /// <summary>
        /// Warning: This method can be called during resolving chain link but
        /// do not called it during <see cref="AEffect.InvokeEffect(GameController)"/>
        /// 
        /// If you do then resolving will crash
        /// </summary>
        /// <param name="effect"></param>
        public void AddEffectToActualChainLink(AEffect effect) => _actualChainLink.Add(effect);
        public void AddNewEffectToChainLink(AEffect effect) => _nextChainLink.Add(effect);

        private int _chooseTargetIdx = -1, _changeTargetingIdx = 0, _changeCardLocation = 0;
        private int _preCardLeft = 0, _preCardLeftPerEffectIdx = 0, _invokeEffectsIdx = 0;
        private bool _effectsUnregistered = false;
        private void ResolveChainLink(List<ChainLinkLog>? linksLog) {
            for (int chainNumber = 1; _actualChainLink.Count > 0 || _nextChainLink.Count > 0; chainNumber++) {
                if (_chooseTargetIdx == -1) {
                    DebugPrint($"-- Resolving chain link {chainNumber}");

                    _actualChainLink = _nextChainLink;
                    _nextChainLink = new List<AEffect>();

                    _chooseTargetIdx++;
                }

                // choosing targets of effects
                for (; _chooseTargetIdx < _actualChainLink.Count; _chooseTargetIdx++) {
                    var effect = _actualChainLink[_chooseTargetIdx];
                    effect.ChooseTargets(this);
                }
                _chooseTargetIdx = int.MaxValue;

                // trigger all ChangeTargeting
                for (; _changeTargetingIdx < _actualChainLink.Count; _changeTargetingIdx++) {
                    var effect = _actualChainLink[_changeTargetingIdx];
                    PublishEvent(ETriggerSource.ChangeTargeting, effect.OwningCard, effect);
                }
                _changeTargetingIdx = int.MaxValue;

                // trigger change card location
                // actual redirection work in this way
                // -> it is created a new affect which will be called after a card is moved
                //    for example: when card is destroyed and card have text:
                //    -  If this card would be sacrificed or destroyed, return it to your hand instead.
                //    then card will be moved on discard pile and then will be moved back to owner's hand
                //    in same chain link
                for (; _changeCardLocation < _actualChainLink.Count; _changeCardLocation++) {
                    var effect = _actualChainLink[_changeCardLocation];
                    PublishEvent(ETriggerSource.ChangeLocationOfCard, effect.OwningCard, effect);
                }
                _changeCardLocation = int.MaxValue;

                // effect which trigger on leaving but they not change targeting or location
                // this event is primary used for cards which can't be triggered by CardLeftStable
                // because in this time are triggers unregistered
                for (; _preCardLeft < _actualChainLink.Count; _preCardLeft++) {
                    var effect = _actualChainLink[_preCardLeft];
                    for (; _preCardLeftPerEffectIdx < effect.CardTargets.Count; _preCardLeftPerEffectIdx++) {
                        var card = effect.CardTargets[_preCardLeftPerEffectIdx];
                        PublishEvent(ETriggerSource.PreCardLeftStable, card, effect);
                    }
                    _preCardLeftPerEffectIdx = 0;
                }
                _preCardLeft = int.MaxValue;

                if (!_effectsUnregistered) {
                    ChainLinkLog chainLinkLog = new();

                    int cardsDrawn = 0;
                    foreach (var effect in _actualChainLink) {
                        DebugPrint($"{effect}, owner {effect.OwningCard.Name} and targets {string.Join(",", effect.CardTargets.Select(card => card.Name))}");

                        if (Verbosity == VerbosityLevel.All) {
                            // can be probably in method `ChooseTargets` in DrawEffects
                            // but for now it is a hotfix for logging DrawEffect targets
                            if (effect is DrawEffect)
                                for (int i = 0; i < effect.CardCount; i++) {
                                    if (cardsDrawn + 1 >= Pile.Count)
                                        break;

                                    effect.CardTargets.Add(Pile[Pile.Count - ++cardsDrawn]);
                                }

                            chainLinkLog.Effects.Add(new EffectLog(effect, effect.OwningCard, effect.CardTargets));
                        }
                    }

                    if (Verbosity == VerbosityLevel.All)
                        linksLog!.Add(chainLinkLog);

                    // unregister affects of targets cards then moves
                    // the cards which published card leave and card enter events
                    // for example barbed wire could cause trouble
                    foreach (var effect in _actualChainLink)
                        foreach (var card in effect.CardTargets)
                            if (card.Location == CardLocation.OnTable)
                                card.UnregisterAllEffects(this);

                    _effectsUnregistered = true;
                }

                // move cards to new location, trigger leave card and
                for(; _invokeEffectsIdx < _actualChainLink.Count; _invokeEffectsIdx++) {
                    var effect = _actualChainLink[_invokeEffectsIdx];
                    effect.InvokeEffect(this);
                }
                _invokeEffectsIdx = int.MaxValue;

                // delay card enter to new stable to next chain link - a.k.a. add to chainLink
                // --> this i dont need to solve, nearly all trigger effects are by default delayed
                foreach (var effect in _actualChainLink) {
                    foreach (var card in effect.CardTargets)
                        if (card.Location == CardLocation.OnTable) {
                            // register all effects and trigger one time effects
                            // this will cast even cards which says when this card enter stable
                            // because this effects are not trigger effects but one time effects
                            // -> this will prevent interaction of cards in same chain link
                            card.RegisterAllEffects();
                        } else if (card.OneTimeTriggerEffects.Count > 0){
                            // sometimes more cards can target same card and some effects add to card
                            // one time effects which must be removed 
                            card.UnregisterAllEffects(this);
                        }
                }

                _chooseTargetIdx = -1; _changeTargetingIdx = _changeCardLocation = 0;
                _preCardLeft = _preCardLeftPerEffectIdx = _invokeEffectsIdx = 0;
                _effectsUnregistered = false;
                CardsWhichAreTargeted = new Dictionary<Card, AEffect>();
            }

            CheckIfSomeoneWinGame();
        }

        public void CheckIfSomeoneWinGame() {
            foreach (var player in Players) {
                int value = player.Stable.Sum(card => card.UnicornValue);
                if (value >= 6) {
                    State = EGameState.Ended;
                    return;
                }
            }
        }

        public void PlayerDrawCard(APlayer player) {
            if (Pile.Count == 0) {
                DebugPrint("Pile empty -> ending game");

                State = EGameState.Ended;
                return;
            }

            Card topCard = Pile[^1];
            Pile.RemoveAt(Pile.Count - 1);
            topCard.MoveCard(this, player, CardLocation.InHand);
        }

        private void PlayerDrawCards(APlayer player, int numberCards) {
            for (int i = 0; i < numberCards; i++)
                PlayerDrawCard(player);
        }

        public void PlayerGetBabyUnicornOnTable(APlayer player) {
            // when no baby unicorn in Nursery -> player got nothing
            if (Nursery.Count == 0)
                return;

            Card babyUnicron = Nursery[^1];
            babyUnicron.MoveCard(this, player, CardLocation.OnTable);
            babyUnicron.RegisterAllEffects();
        }

        public void PlayerGetBabyUnicornsOnTable(APlayer player, int numberUnicorns) {
            for (int i = 0; i < numberUnicorns; i++)
                PlayerGetBabyUnicornOnTable(player);
        }

        private void OnBeginTurn(APlayer player) {
            // Trigger on begin turn effects
            if (!_publishedOnBeginningTurn) {
                PublishEvent(ETriggerSource.BeginningTurn);
                _publishedOnBeginningTurn = true;
            }

            if (!_beginningTurnResolved) {
                // resolve chain link
                ResolveChainLink(_turnLog?.BeginningOfTurn);

                if (!SkipToEndTurnPhase)
                    PlayerDrawCards(player, 1 + DrawExtraCards);
                
                if (Verbosity == VerbosityLevel.All)
                    foreach (var p in Players)
                        _turnLog!.PlayerCardsAfterBot.Add(new PlayerCards(p));

                _beginningTurnResolved = true;
            }
        }

        private void OnEndTurn(APlayer player) {
            // Trigger on end turn effects
            if (!_publishedOnEndTurn) {
                DebugPrint("Publishing end turn");
                PublishEvent(ETriggerSource.EndTurn);

                _publishedOnEndTurn = true;
            }

            if (!_endTurnResolved) {
                // resolve chain link
                ResolveChainLink(_turnLog?.EndOfTurn);

                _endTurnResolved = true;
            }

            if (player.Hand.Count > 7) {
                List<Card> cards = player.WhichCardsToDiscard(player.Hand.Count - 7, null, player.Hand);
                foreach (var card in cards) {
                    if (!player.Hand.Remove(card))
                        throw new InvalidOperationException($"Card {card.Name} not in player hand!");
                    card.MoveCard(this, null, CardLocation.DiscardPile);
                }
            }
        }

        private int _triggerListIdx = 0;
        public void PublishEvent(ETriggerSource _event, Card? card = null, AEffect? effect = null) {
            if (!EventsPool.TryGetValue(_event, out List<TriggerEffect>? triggerList))
                return;
            for (; _triggerListIdx < triggerList.Count; _triggerListIdx++) {
                var trigger = triggerList[_triggerListIdx];
                trigger.InvokeEffect(_event, card, effect, this);
            }
            _triggerListIdx = 0;

            foreach (var trigger in triggersToRemove)
                trigger.UnsubscribeToEvent(this);
            
            triggersToRemove.Clear();
        }

        public void AddContinuousEffect(AContinuousEffect effect) => ContinuousEffects.Add(effect);

        public void RemoveContinuousEffect(AContinuousEffect effect) => ContinuousEffects.Remove(effect);

        public void SubscribeEvent(ETriggerSource _event, TriggerEffect effect) {
            if (!EventsPool.TryGetValue(_event, out List<TriggerEffect>? triggerList)) {
                triggerList = new List<TriggerEffect> { };
                EventsPool.Add(_event, triggerList);
            }

            if (triggerList.Contains(effect))
                throw new InvalidOperationException("Request for add duplicate trigger effect!");

            triggerList.Add(effect);
        }

        public void UnsubscribeEvent(ETriggerSource _event, TriggerEffect effect) {
            if (!EventsPool.TryGetValue(_event, out List<TriggerEffect>? triggerList))
                return; // Effect was not event registered, nothing to unregister -> OK state
            if (!triggerList.Remove(effect))
                return; // Effect was not found, nothing to unregister -> OK state
        }

        public void UnsubscribeEventAfterEndOfPublishing(TriggerEffect triggerEffect)
            => triggersToRemove.Add(triggerEffect);

        public List<Card> GetCardsOnTable() {
            List<Card> cards = new();
            foreach(var player in Players) {
                cards.AddRange(player.Stable);
                cards.AddRange(player.Upgrades);
                cards.AddRange(player.Downgrades);
            }
            return cards;
        }

        public void ShuffleDeck() => Pile = Pile.Shuffle(Random);

        /// <summary>
        /// Move all cards from discard pile to pile
        /// 
        /// Warning: Must be shuffled deck after this operation
        /// </summary>
        public void AddDiscardPileToPile() {
            var listCopy = new List<Card>(DiscardPile);
            foreach (var card in listCopy)
                card.MoveCard(this, null, CardLocation.Pile);
            
            Pile.AddRange(listCopy);
        }

        /// <summary>
        /// Clone Gamecontroller / game state with player perspective
        /// 
        /// If <paramref name="player"/> is null then is copied actual gamecontroller / game state
        /// </summary>
        /// <param name="player"></param>
        /// <param name="playerMapper">Dictionary which maps all players to new players</param>
        /// <returns></returns>
        public GameController Clone(APlayer? player, Dictionary<APlayer, APlayer> playerMapper) {
            GameController newGameController = (GameController)MemberwiseClone();
            newGameController.GameLog = new();
            newGameController._turnLog = null;
            newGameController._playedCardLog = null;
            newGameController.Verbosity = VerbosityLevel.None;

            newGameController._debugIndentation += 1;

            Dictionary<TriggerEffect, TriggerEffect> triggerEffectMapper = new();
            Dictionary<AContinuousEffect, AContinuousEffect> continuousEffectMapper = new();
            Dictionary<Card, Card> cardMapper = new();
            foreach (var card in _allCards)
                cardMapper.Add(card, card.Clone(triggerEffectMapper, continuousEffectMapper, playerMapper));

            Dictionary<AEffect, AEffect> effectMapper = new();
            foreach (var effect in _actualChainLink)
                if (!effectMapper.ContainsKey(effect))
                    effectMapper.Add(effect, effect.Clone(cardMapper, effectMapper, playerMapper));
            foreach (var effect in _nextChainLink)
                if (!effectMapper.ContainsKey(effect))
                    effectMapper.Add(effect, effect.Clone(cardMapper, effectMapper, playerMapper));

            newGameController._allCards = cardMapper.Values.ToList();

            newGameController.Random = new Random(42);

            // shuffle pile to avoid cheating
            newGameController.Pile = Pile.ConvertAll(x => cardMapper[x]).Shuffle(newGameController.Random);
            newGameController.DiscardPile = DiscardPile.ConvertAll(x => cardMapper[x]);
            newGameController.Nursery = Nursery.ConvertAll(x => cardMapper[x]);

            newGameController.Players = Players.ConvertAll(p => playerMapper[p]);
            foreach (var (oldP, newP) in playerMapper) {
                newP.Hand = oldP.Hand.ConvertAll(x => cardMapper[x]);
                newP.Stable = oldP.Stable.ConvertAll(x => cardMapper[x]);
                newP.Upgrades = oldP.Upgrades.ConvertAll(x => cardMapper[x]);
                newP.Downgrades = oldP.Downgrades.ConvertAll(x => cardMapper[x]);
                newP.GameController = newGameController;
            }

            newGameController.ContinuousEffects = ContinuousEffects.ConvertAll(x => continuousEffectMapper[x]);
            newGameController.CardVisibilityTracker = CardVisibilityTracker.Clone(cardMapper, playerMapper);

            newGameController.EventsPool = new();
            foreach (var (key, value) in EventsPool) {
                var events = value.ConvertAll(x => triggerEffectMapper[x]);

                newGameController.EventsPool.Add(key, events);
            }
            newGameController.triggersToRemove = triggersToRemove.ConvertAll(x => triggerEffectMapper[x]);

            newGameController._actualChainLink = _actualChainLink.ConvertAll(x => effectMapper[x]);
            newGameController._nextChainLink = _nextChainLink.ConvertAll(x => effectMapper[x]);
            newGameController.Stack = Stack.ConvertAll(x => cardMapper[x]);

            newGameController.ActualPlayerOnTurn = playerMapper[ActualPlayerOnTurn];

            newGameController.CardsWhichAreTargeted = new();
            foreach (var (card, effect) in CardsWhichAreTargeted) {
                newGameController.CardsWhichAreTargeted.Add(cardMapper[card], effectMapper[effect]);
            }

            newGameController._card = _card == null ? null : cardMapper[_card];
            newGameController._targetPlayer = _targetPlayer == null ? null : playerMapper[_targetPlayer];

            if (player == null)
                return newGameController;

            Dictionary<Card, Card> redirections;
            if (!newGameController.CardVisibilityTracker.IsPlayerSeePile(playerMapper[player]))
                ChangeUnknownOppenentsCardWithPile(player, playerMapper, newGameController, out redirections);
            else
                ChangeUnknownOppenentsCardBetweenPlayers(player, playerMapper, newGameController, out redirections);

            foreach (var effect in newGameController._actualChainLink) {
                for (int i = 0; i < effect.CardTargets.Count; i++) {
                    if (redirections.TryGetValue(effect.CardTargets[i], out var newTarget))
                        effect.CardTargets[i] = newTarget;
                }
            }

            return newGameController;

            static void ChangeUnknownOppenentsCardWithPile(
                APlayer player,
                Dictionary<APlayer, APlayer> playerMapper,
                GameController newGameController,
                out Dictionary<Card, Card> redirections)
            {
                // leave opponents cards in hand which are known and rest of them randomly replace
                var playerKnowledge = newGameController.CardVisibilityTracker.GetKnownPlayerCardsOfAllPlayers(playerMapper[player]);
                Dictionary<APlayer, List<Card>> cardsLost = newGameController.Players.ToDictionary(x => x, x => new List<Card>());

                foreach (var p in newGameController.Players) {
                    var knowledgeAboutP = playerKnowledge[p];
                    for (int idx = 0; idx < p.Hand.Count; idx++) {
                        var card = p.Hand[idx];
                        if (!knowledgeAboutP.Contains(card)) {
                            cardsLost[p].Add(card);
                            card.MoveCard(newGameController, null, CardLocation.Pile);
                            newGameController.Pile.Add(card);
                            idx--;
                        }
                    }
                }

                newGameController.Pile = newGameController.Pile.Shuffle(newGameController.Random);

                // effects like discard effect can reference unknown cards
                redirections = new Dictionary<Card, Card>();
                foreach (var (p, playerLostCards) in cardsLost) {
                    newGameController.PlayerDrawCards(p, playerLostCards.Count);

                    for (int i = p.Hand.Count - playerLostCards.Count; i < p.Hand.Count; i++) {
                        redirections.Add(playerLostCards[i - p.Hand.Count + playerLostCards.Count], p.Hand[i]);
                    }
                }
            }

            static void ChangeUnknownOppenentsCardBetweenPlayers(
                APlayer player,
                Dictionary<APlayer, APlayer> playerMapper,
                GameController newGameController,
                out Dictionary<Card, Card> redirections)
            {
                // leave opponents cards in hand which are known and rest of them randomly replace
                var playerKnowledge = newGameController.CardVisibilityTracker.GetKnownPlayerCardsOfAllPlayers(playerMapper[player]);
                List<Card> unknownCards = new();

                for (int playerIdx = 0; playerIdx < newGameController.Players.Count; playerIdx++) {
                    var p = newGameController.Players[playerIdx];
                    var knowledgeAboutP = playerKnowledge[p];
                    for (int cardIdx = 0; cardIdx < p.Hand.Count; cardIdx++) {
                        var card = p.Hand[cardIdx];
                        if (!knowledgeAboutP.Contains(card)) {
                            unknownCards.Add(card);
                        }
                    }
                }

                unknownCards = unknownCards.Shuffle(newGameController.Random);

                // effects like discard effect can reference unknown cards
                redirections = new Dictionary<Card, Card>();

                for (int i = 0; i + 1 < unknownCards.Count; i+=2) {
                    var card1 = unknownCards[i];
                    var card2 = unknownCards[i+1];

                    var player1 = card1.Player;
                    var player2 = card2.Player;

                    card1.MoveCard(newGameController, player2, CardLocation.InHand);
                    card2.MoveCard(newGameController, player1, CardLocation.InHand);

                    redirections.Add(card1, card2);
                    redirections.Add(card2, card1);
                }
            }
        }
    }
}
