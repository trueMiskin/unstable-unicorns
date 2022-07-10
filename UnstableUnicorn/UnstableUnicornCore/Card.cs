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
    public sealed class Card {
        public delegate AEffect FactoryEffect(Card owningCard, GameController gameController);
        public delegate TriggerEffect TriggerFactoryEffect(Card owningCard);
        public delegate AContinuousEffect ContinuousFactoryEffect(Card owningCard);

        internal readonly ECardType _cardType;
        private List<FactoryEffect> oneTimeFactoryEffects;
        private List<TriggerEffect> triggerEffects;
        private List<ContinuousFactoryEffect> _continuousFactoryEffects;
        private List<AContinuousEffect> continuousEffects;
        public CardLocation Location { get; private set; }
        public string Name { get; init; }
        private bool _canBeNeigh = true, _canBeDestroyed = true;

        public Card(String name, ECardType cardType, List<FactoryEffect> oneTimeFactoryEffects,
            List<TriggerFactoryEffect> triggerFactoryEffects, List<ContinuousFactoryEffect> continuousFactoryEffect) {
            this.Name = name;
            this._cardType = cardType;
            this.oneTimeFactoryEffects = oneTimeFactoryEffects;
            this.triggerEffects = new();
            foreach (var f in triggerFactoryEffects)
                this.triggerEffects.Add(f(this));

            this.continuousEffects = new();
            this._continuousFactoryEffects = continuousFactoryEffect;

            this.Location = CardLocation.Pile;
        }

        public bool CanBeNeigh() { return false; }
        public bool CanBeSacriced() { return false; }
        public bool CanBeDestroyed() {
            if (Location != CardLocation.OnTable)
                throw new InvalidOperationException("Invalid calling method, this card is not on table!");
            if (Player == null)
                throw new InvalidOperationException("Card on table but player is not set!");

            bool ret = _canBeDestroyed;
            foreach (AContinuousEffect effect in Player.GameController.ContinuousEffects)
                ret &= effect.IsCardDestroyable(this);
            return ret;
        }
        public bool CanBePlayed() {
            if (Location != CardLocation.InHand)
                throw new InvalidOperationException("Card is not in hand. Card cannot be played.");
            if (Player == null)
                throw new InvalidOperationException("Card should be presented in hand but `Player` is not set.");

            bool ret = true;
            GameController gameController = Player.GameController;
            foreach (var effectFactory in oneTimeFactoryEffects)
                ret &= effectFactory(this, gameController).MeetsRequirementsToPlay(gameController);
            return ret;
        }

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

        internal void RegisterAllEffects() {
            if (Player == null)
                throw new InvalidOperationException("Can't register effects without knowing who played card");

            // spells
            foreach (var factoryEffect in oneTimeFactoryEffects)
                Player.GameController.AddNewEffectToChainLink(factoryEffect(this, Player.GameController));

            foreach (var effect in triggerEffects)
                effect.SubscribeToEvent(Player.GameController);

            if (this.continuousEffects.Count != 0)
                throw new InvalidOperationException("Previous continous effects of this card was not unregistered!");
            // Creating new continuous effect for given payer
            foreach (var f in _continuousFactoryEffects)
                this.continuousEffects.Add(f(this));

            foreach (var effect in continuousEffects)
                Player.GameController.AddContinuousEffect(effect);
        }
        internal void UnregisterAllEffects() {
            if (Player == null)
                throw new InvalidOperationException("Can't unregister effects without knowing who owning card");

            foreach (var effect in triggerEffects)
                effect.SubscribeToEvent(Player.GameController);

            foreach (var effect in continuousEffects)
                Player.GameController.RemoveContinuousEffect(effect);
            
            this.continuousEffects.Clear();
        }

        public void PlayedInstant(APlayer? player, List<Card> chainLink) {
            // todo: how play instant card
        }

        public void CardPlayed(GameController gameController, APlayer newCardOwner) {
            if (_cardType == ECardType.Instant)
                throw new InvalidOperationException("Instant card cannot be used by calling CardPlayed");
            if (!CanBePlayed())
                throw new InvalidOperationException("This card cannot be played. Requirements are not met.");

            // player who will own card
            // - in case of spell, owner will be player who played this spell
            // - in other case, owner will be player who own stable where this card will be
            Player = newCardOwner;
            // register all trigger and continuous effects
            // fire one time effects (or spell effects)
            RegisterAllEffects();

            if (_cardType == ECardType.Spell) {
                MoveCard(gameController, null, CardLocation.DiscardPile);
            } else {
                MoveCard(gameController, newCardOwner, CardLocation.OnTable);
                gameController.PublishEvent(ETriggerSource.CardEnteredStable, this);
            }
        }

        /// <summary>
        /// Warning: This function doesn't register or unregister card in pile, because
        /// -> when you draw card, caller can immediately remove this card from pile
        /// -- -> use `PlayerDrawCard` in game controller
        /// -> on other hand when card should be moved to pile, where should be this card placed
        /// 
        /// In other locations function sets new owning player and location and register/unregister on desired locations
        /// from data structures
        /// </summary>
        /// <param name="gameController"></param>
        /// <param name="newPlayer"></param>
        /// <param name="newCardLocation"></param>
        /// <exception cref="InvalidOperationException"></exception>
        internal void MoveCard(GameController gameController, APlayer? newPlayer, CardLocation newCardLocation) {
            if( (Location == CardLocation.InHand || Location == CardLocation.OnTable) && Player == null)
                throw new InvalidOperationException($"On location `{Location}` variable Player should be setted!");
            switch (Location) {
                case CardLocation.Pile:
                case CardLocation.DiscardPile:
                    break;
                case CardLocation.InHand:
                    Player.Hand.Remove(this);
                    break;
                case CardLocation.OnTable:
                    if (_cardType == ECardType.Unicorn)
                        Player.Stable.Remove(this);
                    else if (_cardType == ECardType.Upgrade)
                        Player.Upgrades.Remove(this);
                    else if (_cardType == ECardType.Downgrade)
                        Player.Downgrades.Remove(this);
                    else
                        throw new InvalidOperationException($"Card type {_cardType} can't be on Table!");
                    break;
                default:
                    throw new InvalidOperationException($"Uknown location {Location}");
            }

            Player = newPlayer;
            Location = newCardLocation;

            if ((Location == CardLocation.Pile || Location == CardLocation.DiscardPile) && Player != null)
                throw new InvalidOperationException($"Player can't be set on location `{Location}`");
            if ((Location == CardLocation.InHand || Location == CardLocation.OnTable) && Player == null)
                throw new InvalidOperationException($"Player must be set on location `{Location}`");

            switch (Location) {
                case CardLocation.Pile:
                    break;
                case CardLocation.DiscardPile:
                    gameController.DiscardPile.Add(this);
                    break;
                case CardLocation.InHand:
                    Player.Hand.Add(this);
                    break;
                case CardLocation.OnTable:
                    if (_cardType == ECardType.Unicorn)
                        Player.Stable.Add(this);
                    else if (_cardType == ECardType.Upgrade)
                        Player.Upgrades.Add(this);
                    else if (_cardType == ECardType.Downgrade)
                        Player.Downgrades.Add(this);
                    else
                        throw new InvalidOperationException($"Card type {_cardType} can't be on Table!");
                    break;
                default:
                    throw new InvalidOperationException($"Uknown location {Location}");
            }
        }
    }
}
