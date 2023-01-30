using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnstableUnicornCore.Agent {
    public class MctsAgent : APlayer {
        protected override bool ActivateEffectCore(AEffect effect) {
            throw new NotImplementedException();
        }

        protected override List<APlayer> ChoosePlayersCore(int number, AEffect effect, List<APlayer> playersWhichCanBeSelected) {
            throw new NotImplementedException();
        }

        protected override Card? PlayInstantOnStackCore(List<Card> stack) {
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

    class Mcts {
        GameController _controller;
        APlayer _baseStrat;
        int _limit;

        public Mcts(GameController controller, APlayer baseStrat, int limit) {
            _controller = controller;
            _baseStrat = baseStrat;
            _limit = limit;
        }

        public T Action<T> (List<T> actions) {
            throw new NotImplementedException();
        }
    }

    class MctsNode {
        GameController _state;
        MctsNode? _parent;
        int visits = 0, wins = 0;
        int numberCreatedChildren = 0;
        MctsNode?[] _children;
        bool _isGameEnded;
        Random _random;

        double c = Math.Sqrt(2);
        public MctsNode(GameController state, MctsNode? parent, List<object> actions, Random random) {
            _state = state;
            _parent = parent;

            _children = new MctsNode[actions.Count];
            _isGameEnded = state.State == EGameState.Ended;
            _random = random;
        }

        public double ucb() {
            if (visits == 1)
                return 1;

            double parent_playes = _parent == null ? 1 : _parent.visits;

            return wins / visits + c * Math.Sqrt(Math.Log(parent_playes) / visits);
        }

        public bool isLeaf() => numberCreatedChildren != _children.Length || _isGameEnded;

        public MctsNode pickUnexploredChildren() {
            if (_isGameEnded)
                return this;
            
            var idx = _random.Next(_children.Length);
            while (_children[idx] != null) idx = _random.Next(_children.Length);

            // TODO: kopirovani stavu
            var newState = _state.Clone(null, null);
            // TODO: aplikovat jednu akci a vrátit další stav

            _children[idx] = new MctsNode(newState, this, __actions__, random);
            numberCreatedChildren++;

            return _children[idx];
        }

        public void playout(Dictionary<APlayer, APlayer> baseStrat) {
            var state = _state.Clone(null, baseStrat);

            state.SimulateGame();

            // TODO: vratit vysledek
        }

        public void backpropagate(List<int> values) {

        }

        GameController makeOneAction(object action) {
            
            _state.Clone()
        }

        class DataOneActionAgent {
            public List<object> Action;
            public GameController? StateAfterAction;
            public List<List<object>>? ListOfActionsAfterAction;
            public Dictionary<APlayer, APlayer> ReverseMapping = new();
            public DataOneActionAgent(List<object> action) => Action = action;
        }
        class OneActionAgent : APlayer {
            public DataOneActionAgent Data;
            public OneActionAgent(DataOneActionAgent data) => Data = data;

            private List<T> makeAction<T>(List<T> availableActions, int number) {
                if (Data.Action.Count != 0) {
                    List<T> answer = Data.Action.Cast<T>().ToList();
                    Data.Action.Clear();
                    return answer;
                }
                if (Data.StateAfterAction == null) {
                    Data.StateAfterAction = GameController.Clone(null, Data.ReverseMapping);
                    Data.ListOfActionsAfterAction = availableActions.Subsets(number).Cast<List<object>>().ToList();
                    GameController.State = EGameState.Ended;
                }
            }

            protected override bool ActivateEffectCore(AEffect effect) {
                throw new NotImplementedException();
            }

            protected override List<APlayer> ChoosePlayersCore(int number, AEffect effect, List<APlayer> playersWhichCanBeSelected) {
                throw new NotImplementedException();
            }

            protected override Card? PlayInstantOnStackCore(List<Card> stack) {
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
}
