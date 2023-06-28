/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace UnstableUnicornCore.Agent {
    public delegate APlayer BaseStrategyGenerator();

    public class MctsAgent : APlayer {
        int Playouts { get; init; }
        BaseStrategyGenerator BaseStrategy { get; init; }
        public MctsAgent(int playouts, BaseStrategyGenerator baseStrategy) {
            Playouts = playouts;
            BaseStrategy = baseStrategy;
        }
        private List<T> chooseAction<T>(List<T> availableActions, int count) {
            if (availableActions.Count == count)
                return availableActions;

            var actions = availableActions.Subsets(count).ToList();

            var mcts = new Mcts(this, GameController, BaseStrategy, Playouts);
            var selection = mcts.Action(Enumerable.Range(0, availableActions.Count).ToList().Subsets(count).ToList());
            
            List<T> result = new List<T>();
            foreach (var idx in selection)
                result.Add(availableActions[idx]);

            return result;
        }
        protected override bool ActivateEffectCore(AEffect effect) {
            return chooseAction(new List<bool> { true, false }, 1)[0];
        }

        protected override List<APlayer> ChoosePlayersCore(int number, AEffect effect, List<APlayer> playersWhichCanBeSelected) {
            return chooseAction(playersWhichCanBeSelected, number);
        }

        protected override Card? PlayInstantOnStackCore(List<Card> stack, List<Card> availableInstantCards) {
            List<Card?> instants = availableInstantCards.Cast<Card?>().ToList();
            instants.Add(null);
            return chooseAction(instants, 1)[0];
        }

        protected override APlayer WhereShouldBeCardPlayedCore(Card card) {
            return chooseAction(AgentUtils.WhereCouldBeCardPlayed(GameController, card), 1)[0];
        }

        protected override List<Card> WhichCardsToDestroyCore(int number, AEffect effect, List<Card> cardsWhichCanBeSelected) {
            return chooseAction(cardsWhichCanBeSelected, number);
        }

        protected override List<Card> WhichCardsToDiscardCore(int number, AEffect? effect, List<Card> cardsWhichCanBeSelected) {
            return chooseAction(cardsWhichCanBeSelected, number);
        }

        protected override List<Card> WhichCardsToGetCore(int number, AEffect effect, List<Card> cardsWhichCanBeSelected) {
            return chooseAction(cardsWhichCanBeSelected, number);
        }

        protected override List<Card> WhichCardsToMoveCore(int number, AEffect effect, List<Card> cardsWhichCanBeSelected) {
            return chooseAction(cardsWhichCanBeSelected, number);
        }

        protected override List<Card> WhichCardsToReturnCore(int number, AEffect effect, List<Card> cardsWhichCanBeSelected) {
            return chooseAction(cardsWhichCanBeSelected, number);
        }

        protected override List<Card> WhichCardsToSacrificeCore(int number, AEffect effect, List<Card> cardsWhichCanBeSelected) {
            return chooseAction(cardsWhichCanBeSelected, number);
        }

        protected override List<Card> WhichCardsToSaveCore(int number, AEffect effect, List<Card> cardsWhichCanBeSelected) {
            return chooseAction(cardsWhichCanBeSelected, number);
        }

        protected override List<Card> WhichCardsToStealCore(int number, AEffect effect, List<Card> cardsWhichCanBeSelected) {
            return chooseAction(cardsWhichCanBeSelected, number);
        }

        protected override Card? WhichCardToPlayCore() {
            List<Card?> availableCards = AgentUtils.GetPlayableCardToPlayerStable(this).Cast<Card?>().ToList();
            availableCards.Add(null);
            return chooseAction(availableCards, 1)[0];
        }

        protected override AEffect WhichEffectToSelectCore(List<AEffect> effectsVariants) {
            return chooseAction(effectsVariants, 1)[0];
        }
    }

    class Mcts {
        APlayer _player;
        GameController _controller;
        BaseStrategyGenerator _baseStrat;
        int _limit;

        public Mcts(APlayer player, GameController controller, BaseStrategyGenerator baseStrat, int limit) {
            _player = player;
            _controller = controller;
            _baseStrat = baseStrat;
            _limit = limit;
        }

        public List<int> Action(List<List<int>> actions) {
            if (_controller.State == EGameState.Ended)
                throw new Exception("Game is ended");
            var mapper = new Dictionary<APlayer, APlayer>();
            foreach (var p in _controller.Players)
                mapper.Add(p, new EmptyAgent());

            var rootState = _controller.Clone(_player, mapper);

            var rootNode = new MctsNode(rootState, null, actions, _controller.Players.IndexOf(_player), new Random(42));
            for (int _ = 0; _ < _limit; _++) {
                var node = rootNode;

                StringBuilder treePath = new StringBuilder();
                while (!node.IsLeaf()) {
                    // every x will be non null
                    Debug.Assert(node._children.Length != 0);
                    node = node._children.MaxBy(x => x!.Ucb())!;

                    Debug.Assert(node._parent != null);
                    treePath.Append(node._parent._children.ToList().IndexOf(node));
                }

                MctsNode unexploredChild = node.PickUnexploredChildren(treePath);

                int[] outcomes = unexploredChild.Playout(_baseStrat);
                unexploredChild.Backpropagate(outcomes);
            }

            return actions[rootNode.BestActionIdx()];
        }
    }

    class MctsNode {
        GameController _state;
        public MctsNode?[] _children;
        IReadOnlyList<IReadOnlyList<int>> _actions;

        public MctsNode? _parent;
        int _playerIdx;
        int visits = 0, wins = 0;
        int numberCreatedChildren = 0;

        bool _isGameEnded;
        Random _random;

        static readonly double c = Math.Sqrt(2);
        public MctsNode(GameController state, MctsNode? parent, IReadOnlyList<IReadOnlyList<int>> actions, int playerIdx, Random random) {
            _state = state;
            _actions = actions;
            _children = new MctsNode[actions.Count];

            _parent = parent;
            _playerIdx = playerIdx;

            _isGameEnded = state.State == EGameState.Ended;
            _random = random;
        }

        public double Ucb() {
            if (visits == 0)
                return 1;

            double parent_playes = _parent == null ? 1 : _parent.visits;

            return wins / (double) visits + c * Math.Sqrt(Math.Log(parent_playes) / visits);
        }

        public bool IsLeaf() => numberCreatedChildren != _children.Length || _isGameEnded;

        public MctsNode PickUnexploredChildren(StringBuilder treePath) {
            if (_isGameEnded)
                return this;
            
            var idx = _random.Next(_children.Length);
            while (_children[idx] != null) idx = _random.Next(_children.Length);

            treePath.Append(idx);
            // Console.WriteLine(sb.ToString());

            // Console.WriteLine("One step");
            var (newState, actions, playerIdx) = makeOneAction(_actions[idx], _state);
            // Console.WriteLine("One step ended");

            _children[idx] = new MctsNode(newState, this, actions, playerIdx, _random);
            numberCreatedChildren++;

            return _children[idx]!;
        }

        public int[] Playout(BaseStrategyGenerator baseStrat) {
            var mapper = new Dictionary<APlayer, APlayer>();
            foreach (var p in _state.Players) {
                mapper.Add(p, baseStrat());
            }

            var state = _state.Clone(null, mapper);

            // Console.WriteLine($"New Playout {state.State}");
            state.SimulateGame();
            // Console.WriteLine("End playout");

            int[] outcomes = new int[state.Players.Count];
            for (int idx = 0; idx < state.Players.Count; idx++) {
                outcomes[state.GameResults[idx].PlayerId] = state.Players.Count - idx - 1;
            }

            return outcomes;
        }

        public void Backpropagate(int[] outcomes) {
            var node = this;
            while (true) {
                node.visits += _state.Players.Count;

                if (node._parent == null)
                    break;

                node.wins += outcomes[node._parent._playerIdx];

                node = node._parent;
            }
        }

        public int BestActionIdx() {
            int bestAction = -1, bestValue = 0;
            for (int i = 0; i < _children.Length; i++) {
                var child = _children[i];
                if (child == null) continue;

                if (child.visits > bestValue) {
                    bestValue = child.visits;
                    bestAction = i;
                }
            }
            return bestAction;
        }

        /// <summary>
        /// Useful debug variable
        /// </summary>
        public List<object?>? RealActions;

        /// <summary>
        /// Make one defined action from given state
        /// 
        /// This method does not change given state controller
        /// </summary>
        /// <param name="action"></param>
        /// <param name="controller"></param>
        /// <returns>New state after making one action, actions available from next state and player on "turn"</returns>
        (GameController newState, List<List<int>> actions, int playerIdx) makeOneAction(IReadOnlyList<int> action, GameController controller) {
            var data = new DataOneActionAgent(action);
            var playerMapper = new Dictionary<APlayer, APlayer>();

            foreach (APlayer p in controller.Players)
                playerMapper.Add(p, new OneActionAgent(data));

            var helperController = controller.Clone(null, playerMapper);
            helperController.SimulateGame();

            RealActions = data.RealActions;
            if (data.StateAfterAction == null) {
                data.StateAfterAction = copyStateWithNewEmptyAgents(helperController);
                data.ListOfActionsAfterAction = new();
                // it doesn't matter that player idx is not set -> game ended and this value will be never used
            }

            Debug.Assert(data.ListOfActionsAfterAction != null);
            return (data.StateAfterAction, data.ListOfActionsAfterAction, data.PlayerIdx);
        }

        static GameController copyStateWithNewEmptyAgents(GameController gameController) {
            var mapper = new Dictionary<APlayer, APlayer>();
            foreach (var player in gameController.Players) {
                mapper.Add(player, new EmptyAgent());
            }
            return gameController.Clone(null, mapper);
        }

        class DataOneActionAgent {
            public int PlayerIdx;
            public IReadOnlyList<int> Action;
            public GameController? StateAfterAction;
            public List<object?>? RealActions;
            public List<List<int>>? ListOfActionsAfterAction;
            public bool ActionPerformed = false;

            public DataOneActionAgent(IReadOnlyList<int> action) {
                Action = action;
            }

        }

        class OneActionAgent : APlayer {
            public DataOneActionAgent Data;
            public OneActionAgent(DataOneActionAgent data) => Data = data;

            private List<T> makeAction<T>(List<T> availableActions, int count) {
                if (!Data.ActionPerformed) {
                    List<T> answer = new();
                    foreach (var idx in Data.Action)
                        answer.Add(availableActions[idx]);

                    Data.ActionPerformed = true;
                    // Console.WriteLine("Playing {0}:", String.Join(" ", answer));
                    return answer;
                }
                if (Data.StateAfterAction == null) {
                    Debug.Assert(availableActions.Count != 0);
                    Data.PlayerIdx = GameController.Players.IndexOf(this);
                    Data.StateAfterAction = copyStateWithNewEmptyAgents(GameController);
                    Data.RealActions = availableActions.ConvertAll(o => (object?)o).ToList();
                    Data.ListOfActionsAfterAction = Enumerable.Range(0, availableActions.Count).ToList().Subsets(count).ToList();
                    GameController.State = EGameState.Ended;
                    // Console.WriteLine("Actions {0}:", String.Join(" ", availableActions));
                    // Console.WriteLine("State saved");
                }

                return availableActions.RandomSelection(GameController.Random, count);
            }

            protected override bool ActivateEffectCore(AEffect effect) {
                return makeAction(new List<bool> { true, false }, 1)[0];
            }

            protected override List<APlayer> ChoosePlayersCore(int number, AEffect effect, List<APlayer> playersWhichCanBeSelected) {
                return makeAction(playersWhichCanBeSelected, number);
            }

            protected override Card? PlayInstantOnStackCore(List<Card> stack, List<Card> availableInstantCards) {
                List<Card?> instants = availableInstantCards.Cast<Card?>().ToList();
                instants.Add(null);
                return makeAction(instants, 1)[0];
            }

            protected override APlayer WhereShouldBeCardPlayedCore(Card card) {
                return makeAction(AgentUtils.WhereCouldBeCardPlayed(GameController, card), 1)[0];
            }

            protected override List<Card> WhichCardsToDestroyCore(int number, AEffect effect, List<Card> cardsWhichCanBeSelected) {
                return makeAction(cardsWhichCanBeSelected, number);
            }

            protected override List<Card> WhichCardsToDiscardCore(int number, AEffect? effect, List<Card> cardsWhichCanBeSelected) {
                return makeAction(cardsWhichCanBeSelected, number);
            }

            protected override List<Card> WhichCardsToGetCore(int number, AEffect effect, List<Card> cardsWhichCanBeSelected) {
                return makeAction(cardsWhichCanBeSelected, number);
            }

            protected override List<Card> WhichCardsToMoveCore(int number, AEffect effect, List<Card> cardsWhichCanBeSelected) {
                return makeAction(cardsWhichCanBeSelected, number);
            }

            protected override List<Card> WhichCardsToReturnCore(int number, AEffect effect, List<Card> cardsWhichCanBeSelected) {
                return makeAction(cardsWhichCanBeSelected, number);
            }

            protected override List<Card> WhichCardsToSacrificeCore(int number, AEffect effect, List<Card> cardsWhichCanBeSelected) {
                return makeAction(cardsWhichCanBeSelected, number);
            }

            protected override List<Card> WhichCardsToSaveCore(int number, AEffect effect, List<Card> cardsWhichCanBeSelected) {
                return makeAction(cardsWhichCanBeSelected, number);
            }

            protected override List<Card> WhichCardsToStealCore(int number, AEffect effect, List<Card> cardsWhichCanBeSelected) {
                return makeAction(cardsWhichCanBeSelected, number);
            }

            protected override Card? WhichCardToPlayCore() {
                List<Card?> availableCards = AgentUtils.GetPlayableCardToPlayerStable(this).Cast<Card?>().ToList();
                availableCards.Add(null);
                var ans = makeAction(availableCards, 1)[0];
                return ans;
            }

            protected override AEffect WhichEffectToSelectCore(List<AEffect> effectsVariants) {
                return makeAction(effectsVariants, 1)[0];
            }
        }
    }
    class EmptyAgent : APlayer {
        protected override bool ActivateEffectCore(AEffect effect) {
            throw new NotImplementedException();
        }

        protected override List<APlayer> ChoosePlayersCore(int number, AEffect effect, List<APlayer> playersWhichCanBeSelected) {
            throw new NotImplementedException();
        }

        protected override Card? PlayInstantOnStackCore(List<Card> stack, List<Card> availableInstantCards) {
            throw new NotImplementedException();
        }

        protected override APlayer WhereShouldBeCardPlayedCore(Card card) {
            throw new NotImplementedException();
        }

        protected override List<Card> WhichCardsToDestroyCore(int number, AEffect effect, List<Card> cardsWhichCanBeSelected) {
            throw new NotImplementedException();
        }

        protected override List<Card> WhichCardsToDiscardCore(int number, AEffect? effect, List<Card> cardsWhichCanBeSelected) {
            throw new NotImplementedException();
        }

        protected override List<Card> WhichCardsToGetCore(int number, AEffect effect, List<Card> cardsWhichCanBeSelected) {
            throw new NotImplementedException();
        }

        protected override List<Card> WhichCardsToMoveCore(int number, AEffect effect, List<Card> cardsWhichCanBeSelected) {
            throw new NotImplementedException();
        }

        protected override List<Card> WhichCardsToReturnCore(int number, AEffect effect, List<Card> cardsWhichCanBeSelected) {
            throw new NotImplementedException();
        }

        protected override List<Card> WhichCardsToSacrificeCore(int number, AEffect effect, List<Card> cardsWhichCanBeSelected) {
            throw new NotImplementedException();
        }

        protected override List<Card> WhichCardsToSaveCore(int number, AEffect effect, List<Card> cardsWhichCanBeSelected) {
            throw new NotImplementedException();
        }

        protected override List<Card> WhichCardsToStealCore(int number, AEffect effect, List<Card> cardsWhichCanBeSelected) {
            throw new NotImplementedException();
        }

        protected override Card? WhichCardToPlayCore() {
            throw new NotImplementedException();
        }

        protected override AEffect WhichEffectToSelectCore(List<AEffect> effectsVariants) {
            throw new NotImplementedException();
        }
    }
}
