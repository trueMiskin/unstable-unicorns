using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnstableUnicornCore {
    public enum ECardType {
        BabyUnicorn,
        BasicUnicorn,
        MagicUnicorn,
        Spell,
        Upgrade,
        Downgrade,
        Instant,
        Panda
    }

    public static class ECardTypeUtils {
        public static readonly List<ECardType> UnicornTarget = new List<ECardType>() {
            ECardType.BabyUnicorn, ECardType.BasicUnicorn, ECardType.MagicUnicorn
        };
        public static readonly List<ECardType> CardTarget = new List<ECardType>() {
            ECardType.BabyUnicorn, ECardType.BasicUnicorn, ECardType.MagicUnicorn,
            ECardType.Spell, ECardType.Upgrade, ECardType.Downgrade, ECardType.Instant
        };
    }

    public enum CardLocation {
        Pile,
        InHand,
        OnTable,
        DiscardPile
    }

    public sealed class Card {
        public delegate AEffect FactoryEffect(Card owningCard);
        public delegate TriggerEffect TriggerFactoryEffect(Card owningCard);
        public delegate AContinuousEffect ContinuousFactoryEffect(Card owningCard);

        internal readonly ECardType _cardType;
        private List<FactoryEffect> oneTimeFactoryEffects;
        private List<TriggerEffect> triggerEffects;
        private List<TriggerEffect> _oneTimeTriggerEffects;
        private List<ContinuousFactoryEffect> _continuousFactoryEffects;
        private List<AContinuousEffect> continuousEffects;
        public CardLocation Location { get; private set; }
        public List<TriggerEffect> OneTimeTriggerEffects => _oneTimeTriggerEffects;

        public static string CardOnTablePlayerNull = "Card on table but player is not set!";
        public static string CardCannotBePlayed = "This card cannot be played. Requirements are not met.";

        public string Name { get; init; }
        private bool _canBeNeigh = true, _canBeDestroyed = true, _canBeSacrificed = true;

        public Card(String name, ECardType cardType, List<FactoryEffect> oneTimeFactoryEffects,
            List<TriggerFactoryEffect> triggerFactoryEffects, List<ContinuousFactoryEffect> continuousFactoryEffect) {
            this.Name = name;
            this._cardType = cardType;
            this.oneTimeFactoryEffects = oneTimeFactoryEffects;
            this.triggerEffects = new();
            foreach (var f in triggerFactoryEffects)
                this.triggerEffects.Add(f(this));

            this.continuousEffects = new();
            this._oneTimeTriggerEffects = new();
            this._continuousFactoryEffects = continuousFactoryEffect;

            this.Location = CardLocation.Pile;
        }

        public bool CanBeNeigh() { return _canBeNeigh; }
        public bool CanBeSacriced() { return _canBeSacrificed; }
        public bool CanBeDestroyed() {
            if (Location != CardLocation.OnTable)
                throw new InvalidOperationException("Invalid calling method, this card is not on table!");
            if (Player == null)
                throw new InvalidOperationException(CardOnTablePlayerNull);

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
                ret &= effectFactory(this).MeetsRequirementsToPlay(gameController);
            foreach (var effect in Player.GameController.ContinuousEffects)
                ret &= effect.IsCardPlayable(Player, this);
            return ret;
        }

        /// <summary>
        /// Player which own this card or null if it is in pile or discard pile
        /// </summary>
        public APlayer? Player { get; set; }

        public ECardType CardType {
            get {
                if (Location != CardLocation.OnTable)
                    return _cardType;
                if (Player == null)
                    throw new InvalidOperationException(CardOnTablePlayerNull);

                ECardType type = _cardType;
                foreach (var continuousEffect in Player.GameController.ContinuousEffects)
                    type = continuousEffect.GetCardType(type, Player);

                return type;
            }
        }

        internal void RegisterAllEffects() {
            if (Player == null)
                throw new InvalidOperationException("Can't register effects without knowing who played card");

            // spells
            foreach (var factoryEffect in oneTimeFactoryEffects)
                Player.GameController.AddNewEffectToChainLink(factoryEffect(this));

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
                effect.UnsubscribeToEvent(Player.GameController);

            foreach (var effect in _oneTimeTriggerEffects)
                effect.UnsubscribeToEvent(Player.GameController);

            _oneTimeTriggerEffects.Clear();

            foreach (var effect in continuousEffects)
                Player.GameController.RemoveContinuousEffect(effect);
            
            this.continuousEffects.Clear();
        }

        public void AddOneTimeTriggerEffect(TriggerEffect triggerEffect) {
            _oneTimeTriggerEffects.Add(triggerEffect);
        }

        public void RemoveOneTimeTriggerEffect(TriggerEffect triggerEffect) {
            _oneTimeTriggerEffects.Remove(triggerEffect);
        }

        public void PlayedInstant(APlayer? player, List<Card> chainLink) {
            // todo: how play instant card
        }

        public void CardPlayed(GameController gameController, APlayer newCardOwner) {
            if (_cardType == ECardType.Instant)
                throw new InvalidOperationException("Instant card cannot be used by calling CardPlayed");
            if (!CanBePlayed())
                throw new InvalidOperationException(CardCannotBePlayed);
            if (_cardType == ECardType.Spell && newCardOwner != Player)
                throw new InvalidOperationException("A spell cannot have a new card owner while the spell is being cast.");

            if (_cardType == ECardType.Spell) {
                // set player for trigger effect, then cast spell
                Player = newCardOwner;
                // trigger one time effect
                RegisterAllEffects();
                MoveCard(gameController, null, CardLocation.DiscardPile);
            } else {
                MoveCard(gameController, newCardOwner, CardLocation.OnTable);
                // register all trigger and continuous effects
                // fire one time effects (or spell effects
                RegisterAllEffects();
                gameController.PublishEvent(ETriggerSource.CardEnteredStable, this);
            }
        }

        /// <summary>
        /// Warning: This function does not add or remove cards to <see cref="CardLocation.Pile"/>
        /// for performence 
        /// <br/>
        /// In other locations will be card set to desired data structures
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
                    break;
                case CardLocation.DiscardPile:
                    gameController.DiscardPile.Remove(this);
                    break;
                case CardLocation.InHand:
                    Player.Hand.Remove(this);
                    break;
                case CardLocation.OnTable:
                    if (ECardTypeUtils.UnicornTarget.Contains(_cardType))
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
                    if (ECardTypeUtils.UnicornTarget.Contains(_cardType))
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
