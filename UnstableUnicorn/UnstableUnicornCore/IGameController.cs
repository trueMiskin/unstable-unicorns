﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnstableUnicornCore {
    public interface IGameController {
        void SimulateGame();
    }

    public class GameController : IGameController, IPublisher {
        public Random Random { get; set; }
        public List<Card> Pile;
        public List<Card> DiscardPile = new();
        public List<Card> Nursery;
        public List<APlayer> Players;
        public List<AContinuousEffect> ContinuousEffects = new();
        private Dictionary<ETriggerSource, List<TriggerEffect>> EventsPool = new();

        /// <summary>
        /// Actual chain link is link which is already in proccess of resolving
        /// 
        /// Next chain link is link which will follow in next iteration of resolving
        /// </summary>
        private List<AEffect> _actualChainLink = new();
        private List<AEffect> _nextChainLink = new();

        public HashSet<Card> cardsWhichAreTargeted { get; set; } = new();

        private bool _willTakeExtraTurn = false;

        public GameController(List<Card> pile, List<Card> nursery, List<APlayer> players, int seed = 42) {
            Random = new Random(seed);
            Pile = pile.Shuffle(Random);
            Nursery = new List<Card>( nursery );

            foreach(APlayer p in players)
                p.GameController = this;
            Players = new List<APlayer>( players );
        }

        public void SimulateGame() {
            int index = 0;
            while (true) {
                APlayer player = Players[index];
                SimulateOneTurn(player);

                if (_willTakeExtraTurn)
                    _willTakeExtraTurn = false;
                else
                    index = (index + 1) % Players.Count;
            }
        }

        internal void SimulateOneTurn(APlayer playerOnTurn) {
            OnBeginTurn(playerOnTurn);

            Card? card = playerOnTurn.WhichCardToPlay();
            if (card == null)
                PlayerDrawCard(playerOnTurn);
            else {
                if (!playerOnTurn.Hand.Remove(card))
                    throw new InvalidOperationException($"Card {card.Name} not in player hand!");

                // check instant
                // interative resolving instant cards
                // stack chain resolve

                card.CardPlayed(this, playerOnTurn);
            }

            OnEndTurn(playerOnTurn);
        }

        public void ThisPlayerTakeExtraTurn() => _willTakeExtraTurn = true;

        /// <summary>
        /// Warning: This method can be called at the lastest during calling <see cref="AEffect.ChooseTargets(GameController)"/>,
        /// later will crash resolving whole chain link
        /// </summary>
        /// <param name="effect"></param>
        public void AddEffectToActualChainLink(AEffect effect) => _actualChainLink.Add(effect);
        public void AddNewEffectToChainLink(AEffect effect) => _nextChainLink.Add(effect);

        private void ResolveChainLink() {
            while (_nextChainLink.Count > 0) {
                cardsWhichAreTargeted = new HashSet<Card>();

                _actualChainLink = _nextChainLink;
                _nextChainLink = new List<AEffect>();

                // choosing targets of effects
                for (int i = 0; i < _actualChainLink.Count; i++) {
                    var effect = _actualChainLink[i];
                    effect.ChooseTargets(this);
                }

                // trigger all ChangeTargeting
                foreach (var effect in _actualChainLink) {
                    PublishEvent(ETriggerSource.ChangeTargeting, effect.OwningCard, effect);
                }

                // trigger change card location
                foreach (var effect in _actualChainLink) {
                    PublishEvent(ETriggerSource.ChangeLocationOfCard, effect.OwningCard, effect);
                }

                // unregister affects of targets cards
                foreach (var effect in _actualChainLink)
                    foreach (var card in effect.CardTargets)
                        card.UnregisterAllEffects();

                // move cards to new location, trigger leave card and
                foreach (var effect in _actualChainLink)
                    effect.InvokeEffect(this);

                // TODO: Does it done in CardPlayed already?
                // delay card enter to new stable to next chain link - a.k.a. add to chainLink
                // --> this i dont need to solve, nearly all trigger effects are by default delayed
                /*foreach (var effect in _actualChainLink) {
                    foreach (var card in effect.CardTargets)
                        if (card.Location == CardLocation.OnTable) {
                            card.RegisterAllEffects();
                            PublishEvent(ETriggerSource.CardEnteredStable, effect.OwningCard, effect);
                        }
                }*/
            }
        }

        public void PlayerDrawCard(APlayer player) {
            Card topCard = Pile[^1];
            Pile.RemoveAt(Pile.Count - 1);
            topCard.MoveCard(this, player, CardLocation.InHand);
        }

        private void OnBeginTurn(APlayer player) {
            // Trigger on begin turn effects
            PublishEvent(ETriggerSource.BeginningTurn);

            // resolve chain link
            ResolveChainLink();

            PlayerDrawCard(player);
        }

        private void OnEndTurn(APlayer player) {
            // Trigger on end turn effects
            PublishEvent(ETriggerSource.EndTurn);

            // resolve chain link
            ResolveChainLink();

            if (player.Hand.Count > 7) {
                List<Card> cards = player.WhichCardsToDiscard(player.Hand.Count - 7, Enum.GetValues(typeof(ECardType)).Cast<ECardType>().ToList());
                foreach (var card in cards) {
                    if (!player.Hand.Remove(card))
                        throw new InvalidOperationException($"Card {card.Name} not in player hand!");
                    card.MoveCard(this, null, CardLocation.DiscardPile);
                }
            }
        }

        public void PublishEvent(ETriggerSource _event, Card? card = null, AEffect? effect = null) {
            if (!EventsPool.TryGetValue(_event, out List<TriggerEffect>? triggerList))
                return;
            foreach (var trigger in triggerList) {
                trigger.InvokeEffect(_event, card, effect, this);
            }
        }

        public void AddContinuousEffect(AContinuousEffect effect) => ContinuousEffects.Add(effect);

        // TODO: maybe throw error on remove non-existent effect
        public void RemoveContinuousEffect(AContinuousEffect effect) => ContinuousEffects.Remove(effect);

        public void SubscribeEvent(ETriggerSource _event, TriggerEffect effect) {
            if (!EventsPool.TryGetValue(_event, out List<TriggerEffect>? triggerList)) {
                triggerList = new List<TriggerEffect> { };
                EventsPool.Add(_event, triggerList);
            }
            triggerList.Add(effect);
        }

        public void UnsubscribeEvent(ETriggerSource _event, TriggerEffect effect) {
            if (!EventsPool.TryGetValue(_event, out List<TriggerEffect>? triggerList))
                throw new InvalidOperationException($"Trying unsubscribe unknown effect!");
            if(!triggerList.Remove(effect))
                throw new InvalidOperationException($"Trying unsubscribe unknown effect!");
        }

        public List<Card> GetCardsOnTable() {
            List<Card> cards = new();
            foreach(var player in Players) {
                cards.AddRange(player.Stable);
                cards.AddRange(player.Upgrades);
                cards.AddRange(player.Downgrades);
            }
            return cards;
        }
    }
}
