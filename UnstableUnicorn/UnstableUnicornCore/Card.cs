using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnstableUnicornCore {
    public enum ECardType {
        Unicorn,
        Spell,
        Upgrade,
        Downgrade,
        Instant,
        Panda
    }

    public enum CardLocation {
        Pile,
        InHand,
        OnTable,
        DiscardPile
    }
    public class Card {
        internal readonly ECardType _cardType;
        private List<AEffect> oneTimeEffects;
        private List<TriggerEffect> triggerEffects;
        private List<ContinuousEffect> continuousEffects;
        public CardLocation Location { get; set; }
        public string Name { get; init; }


        public bool CanBeNeigh() { return false; }
        public bool CanBeSacriced() { return false; }
        public bool CanBeDestroyed() { return false; }

        /// <summary>
        /// Player which own this card or null if it is in pile or discard pile
        /// </summary>
        public APlayer? Player { get; set; }

        public ECardType CardType {
            get {
                ECardType type = _cardType;
                foreach (var cardTypeTranformer in Player?.CardTypeTransformers ?? Enumerable.Empty<Func<ECardType, ECardType>>())
                    type = cardTypeTranformer( type );

                return type;
            }
        }

        private void RegisterAllEffects() {
            if (Player == null)
                throw new InvalidOperationException("Can't register effects without knowing who played card");

            foreach (var effect in oneTimeEffects)
                Player.GameController.AddNewEffectToLink(effect);

            foreach (var effect in triggerEffects)
                effect.SubscribeToEvent(Player.GameController);

            foreach (var effect in continuousEffects)
                Player.GameController.AddContinuousEffect(effect);
        }
        internal void UnregisterAllEffects() {
            if (Player == null)
                throw new InvalidOperationException("Can't unregister effects without knowing who owning card");

            foreach (var effect in triggerEffects)
                effect.SubscribeToEvent(Player.GameController);

            foreach (var effect in triggerEffects)
                effect.UnSubscribeToEvent(Player.GameController);
        }

        public void PlayedInstant(APlayer? player, List<Card> chainLink) {
            // todo: how play instant card
        }

        public void CardPlayed(APlayer player) {
            if (_cardType == ECardType.Instant)
                throw new InvalidOperationException("Instant card cannot be used by calling CardPlayed");

            // player who played card
            Player = player;
            // register all trigger and continuous effects
            // fire one time effects (or spell effects)
            RegisterAllEffects();

            if (_cardType == ECardType.Spell) {
                MoveCard(null, CardLocation.DiscardPile);
            } else
                MoveCard(player, CardLocation.OnTable);
        }

        public void MoveCard(APlayer? newPlayer, CardLocation newCardLocation) {
            if(Location == CardLocation.OnTable)
                UnregisterAllEffects();

            APlayer? oldPlayer = Player;
            CardLocation oldLocation = Location;
            Player = newPlayer;
            Location = newCardLocation;

            if (Location != oldLocation && Location == CardLocation.OnTable) {
                // TODO: fire unconditional effects
            }

            switch (newCardLocation) {
                case CardLocation.OnTable:
                    // TODO: register effects
                    break;
                case CardLocation.DiscardPile:
                    if (oldPlayer == null)
                        throw new InvalidOperationException($"Card moved from {oldLocation} and player wasn't set!");
                    oldPlayer.GameController.DiscardPile.Add(this);
                    break;
                default:
                    throw new InvalidOperationException("More location is not know supported");
            }
        }
    }
}
