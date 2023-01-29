//#define DEBUG_PRINT
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace UnstableUnicornCore
{
    public interface IGameController {
        void SimulateGame();
    }

    public class GameController : IGameController, IPublisher {
        public EGameState State { get; private set; } = EGameState.NotStarted;
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

        public GameController(List<Card> pile, List<Card> nursery, List<APlayer> players, int seed = 42) {
            Random = new Random(seed);
            Pile = pile.Shuffle(Random);
            Nursery = new List<Card>( nursery );
            CardVisibilityTracker = new(players);

            foreach (var babyUnicorn in Nursery) {
                babyUnicorn.Player = null;
                babyUnicorn.Location = CardLocation.Nursery;
            }

            foreach(APlayer p in players)
                p.GameController = this;
            Players = new List<APlayer>( players );

            // setting for unit tests
            ActualPlayerOnTurn = Players[0];
        }

        [Conditional("DEBUG_PRINT")]
        private void DebugPrint(String text) {
            Console.WriteLine(text);
        }

        public void SimulateGame() {
            if (State == EGameState.NotStarted) {
                // set up game
                foreach (var player in Players) {
                    PlayerGetBabyUnicornOnTable(player);
                    PlayerDrawCards(player, 5);
                }
            }

            // game
            State = EGameState.Running;
            while (State != EGameState.Ended) {
                DebugPrint($"Player on turn {_playerOnTurn}, actual turn: {_turnNumber}");
                DebugPrint("------> Start turn <-------");

                APlayer player = Players[_playerOnTurn];
                SimulateOneTurn(player);

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
            var finalScoreBoard = scoreBoard.ToList()
                .OrderByDescending(item => item.unicornValue)
                .OrderByDescending(item => item.unicornLen);

            Console.WriteLine($"Game ended after {_turnNumber} turns");
            foreach(var f in finalScoreBoard)
                    Console.WriteLine($"Player id: {Players.IndexOf(f.player)}, value: {f.unicornValue}, len: {f.unicornLen}");
        }

        private int _cardIdx = 0, _playersIterIdx = 0;
        private bool _cardSelected = false, _targetPlayerSelected = false, _stackResolved = false;
        private bool _cardPlayed = false, _cardResolved = false;
        private bool _beforeTurnStarted = false, _publishedOnBeginningTurn = false, _beginningTurnResolved = false;
        private bool _publishedOnEndTurn = false, _endTurnResolved = false;
        private Card? _card;
        private APlayer? targetPlayer;
        internal void SimulateOneTurn(APlayer playerOnTurn) {
            if (!_beforeTurnStarted) {
                SkipToEndTurnPhase = false;
                MaxCardsToPlayInOneTurn = 1;
                DrawExtraCards = 0;
                ActualPlayerOnTurn = playerOnTurn;

                _beforeTurnStarted = true;
            }

            OnBeginTurn(playerOnTurn);
            if (State == EGameState.Ended) return;

            for (; _cardIdx < MaxCardsToPlayInOneTurn; _cardIdx++) {
                if (SkipToEndTurnPhase)
                    break;

                if (!_cardSelected) {
                    _card = playerOnTurn.WhichCardToPlay();
                    _cardSelected = true;
                }

                if (_card == null) {
                    if(_cardIdx == 0)
                        PlayerDrawCard(playerOnTurn);

                    _cardIdx = int.MaxValue; _cardSelected = false;
                    break;
                } else {
                    if (!playerOnTurn.Hand.Remove(_card))
                        throw new InvalidOperationException($"Card {_card.Name} not in player hand!");
                    if (_card.CardType == ECardType.Instant)
                        throw new InvalidOperationException("Instant card cannot be played this way.");

                    // choose target player and check if card can be played
                    // before resolving the stack

                    if (!_targetPlayerSelected) {
                        targetPlayer = playerOnTurn.WhereShouldBeCardPlayed(_card);

                        if (!_card.CanBePlayed(targetPlayer))
                            throw new InvalidOperationException(Card.CardCannotBePlayed);
                        _targetPlayerSelected = true;

                        Stack = new List<Card> { _card };
                    }
                    
                    Debug.Assert(targetPlayer != null);
                    
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
                            if (Stack.Count == 1)
                                break;

                            // resolve last card in chain link
                            topCard.ExecuteInstant(this);
                        } else {
                            // move card from hand
                            int firstPlayedCard = Random.Next(instantCards.Count);
                            instantCards[firstPlayedCard].PlayedInstant(Stack);
                        }
                    }
                    _stackResolved = true;
                    // stack chain resolve

                    // if card is played -> was not neigh
                    if (Stack.Count == 1) {
                        DebugPrint($"Played {_card.Name}");

                        if (!_cardPlayed) {
                            _card.CardPlayed(this, targetPlayer);
                            
                            _cardPlayed = true;
                        }

                        if (!_cardResolved) {
                            ResolveChainLink();
                            
                            _cardResolved = true;
                        }
                        if (State == EGameState.Ended) return;
                    }
                }

                _card = null; targetPlayer = null;
                _cardSelected = _targetPlayerSelected = _stackResolved = false;
                _cardPlayed = _cardResolved = false;
            }

            OnEndTurn(playerOnTurn);

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
        private void ResolveChainLink() {
            for (int chainNumber = 1; _nextChainLink.Count > 0; chainNumber++) {
                if (_chooseTargetIdx == -1) {
                    DebugPrint($"-- Resolving chain link {chainNumber}");

                    CardsWhichAreTargeted = new Dictionary<Card, AEffect>();
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
                    foreach (var effect in _actualChainLink)
                        DebugPrint($"{effect}, owner {effect.OwningCard.Name} and targets {string.Join(",", effect.CardTargets.Select(card => card.Name))}");

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
            }

            CheckIfSomeoneWinGame();
        }

        public void CheckIfSomeoneWinGame() {
            foreach (var player in Players) {
                int value = player.Stable.Sum(card => card.UnicornValue);
                if (value == 6) {
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
                ResolveChainLink();

                if (!SkipToEndTurnPhase)
                    PlayerDrawCards(player, 1 + DrawExtraCards);
                
                _beginningTurnResolved = true;
            }
        }

        private void OnEndTurn(APlayer player) {
            // Trigger on end turn effects
            if (!_publishedOnEndTurn) {
                PublishEvent(ETriggerSource.EndTurn);

                _publishedOnEndTurn = true;
            }

            if (!_endTurnResolved) {
                // resolve chain link
                ResolveChainLink();

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
    }
}
