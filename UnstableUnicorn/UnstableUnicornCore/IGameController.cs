using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnstableUnicornCore {
    public interface IGameController {
        void SimulateGame();
    }

    public class GameController : IGameController, IPublisher {
        public List<Card> Pile;
        public List<Card> DiscardPile = new();
        public List<Card> Nursery;
        public List<APlayer> Players;
        public List<ContinuousEffect> ContinuousEffects = new();
        private Dictionary<ETriggerSource, List<TriggerEffect>> EventsPool = new();

        /// <summary>
        /// Actual chain link is link which is already in proccess of resolving
        /// 
        /// Next chain link is link which will follow in next iteration of resolving
        /// </summary>
        private List<AEffect> _actualChainLink = new();
        private List<AEffect> _nextChainLink = new();

        public GameController(List<Card> pile, List<Card> nursery, List<APlayer> players) {
            Pile = new List<Card>( pile );
            Nursery = new List<Card>( nursery );

            foreach(APlayer p in players)
                p.GameController = this;
            Players = new List<APlayer>( players );
        }

        public void SimulateGame() {
            int index = 0;
            while (true) {
                APlayer player = Players[index];

                OnBeginTurn(player);

                Card? card = player.WhichCardToPlay();
                if (card == null)
                    PlayerDrawCard(player);
                else {
                    // check instant
                    // interative resolving instant cards
                    // stack chain resolve

                    player.CardPlayed(card);
                }

                OnEndTurn(player);

                index = (index + 1) % Players.Count;
            }
        }

        public void AddNewEffectToLink(AEffect effect) {
            // TODO: enque to queue
        }

        private void ResolveChainLink() {
            while (_nextChainLink.Count > 0) {
                var oldChainLink = _nextChainLink;
                _nextChainLink = new List<AEffect>();

                foreach (var effect in oldChainLink) {
                    // trigger all ChangeTargeting (PreLeaveCard)
                    PublishEvent(ETriggerSource.ChangeTargeting, effect);
                }

                foreach (var effect in oldChainLink) {
                    // trigger change card location
                    PublishEvent(ETriggerSource.ChangeLocationOfCard, effect);
                }

                // unregister affects of targets cards

                // move cards to new location, trigger leave card and
                // delay card enter to new stable to next chain link - a.k.a. add to chainLink
                // --> this i dont need to solve, nearly all trigger effects are by default delayed
            }
        }

        private void PlayerDrawCard(APlayer player) {
            player.DrawCardHandler(Pile[^1]);
            Pile.RemoveAt(Pile.Count - 1);
        }

        private void OnBeginTurn(APlayer player) {
            PlayerDrawCard(player);

            PublishEvent(ETriggerSource.BeginningTurn);
            // --> Trigger on begin turn effects

            // resolve chain link
            ResolveChainLink();
        }

        private void OnEndTurn(APlayer player) {
            PublishEvent(ETriggerSource.EndTurn);
            // Trigger on end turn effects
            // resolve chain link
            ResolveChainLink();

            while (player.Hand.Count > 7)
                player.CardDiscarded(player.WhichCardDiscard());
        }

        public void PublishEvent(ETriggerSource _event, AEffect? effect = null) {
            if (!EventsPool.TryGetValue(_event, out List<TriggerEffect>? triggerList))
                return;
            foreach (var trigger in triggerList) {
                // TODO: co predavat
                trigger.InvokeEffect(_event, effect, this);
            }
        }

        public void SubscribeEvent(ETriggerSource _event, TriggerEffect effect) {
            if(!EventsPool.TryGetValue(_event, out List<TriggerEffect>? triggerList)) {
                triggerList = new List<TriggerEffect> {};
                EventsPool.Add(_event, triggerList);
            }
            triggerList.Add(effect);
        }

        public void AddContinuousEffect(ContinuousEffect effect) => ContinuousEffects.Add(effect);

        // TODO: maybe throw error on remove non-existent effect
        public void RemoveContinuousEffect(ContinuousEffect effect) => ContinuousEffects.Remove(effect);

        public void UnsubscribeEvent(ETriggerSource _event, TriggerEffect effect) {
            if (!EventsPool.TryGetValue(_event, out List<TriggerEffect>? triggerList))
                throw new InvalidOperationException($"Trying unsubscribe unknown effect!");
            if(!triggerList.Remove(effect))
                throw new InvalidOperationException($"Trying unsubscribe unknown effect!");
        }
    }
}
