using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("UnstableUnicornCoreTest")]
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
        DiscardPile,
        Nursery
    }

    public sealed class Card {
        public delegate AEffect FactoryEffect(Card owningCard);
        public delegate AEffect FactoryEffectForTriggerEffects(Card owningCard, GameController controller);
        public delegate TriggerEffect TriggerFactoryEffect(Card owningCard);
        public delegate AContinuousEffect ContinuousFactoryEffect(Card owningCard);

        internal readonly ECardType _cardType;
        private List<FactoryEffect> oneTimeFactoryEffects;
        private List<TriggerEffect> triggerEffects;
        private List<TriggerEffect> _oneTimeTriggerEffects;
        private List<ContinuousFactoryEffect> _continuousFactoryEffects;
        private List<AContinuousEffect> continuousEffects;
        public CardLocation Location { get; internal set; }
        public List<TriggerEffect> OneTimeTriggerEffects => _oneTimeTriggerEffects;

        public static string CardNotInHand = "Card is not in hand.Card cannot be played.";
        public static string CardInHandPlayerNull = "Card should be presented in hand but `Player` is not set.";
        public static string CardOnTablePlayerNull = "Card on table but player is not set!";
        public static string CardCannotBePlayed = "This card cannot be played. Requirements are not met.";

        private int _cardIndex = -1;

        /// <summary>
        /// Return persistent card index in the game (even if the game state is copied)
        /// </summary>
        /// <param name="controller"></param>
        /// <returns></returns>
        public int CardIndex(GameController controller) {
            if (_cardIndex == -1)
                _cardIndex = controller._allCards.IndexOf(this);
            return _cardIndex;
        }

        /// <summary>
        /// Name of the card
        /// </summary>
        public string Name { get; init; }

        private int _extraUnicornValue;
        /// <summary>
        /// Unicorn value is number which says how many unicorns is represented by this card
        /// 
        /// Default value for unicorn cards is 1
        /// </summary>
        public int UnicornValue {
            get {
                // none unicorn cards have value 0
                if (!ECardTypeUtils.UnicornTarget.Contains(_cardType))
                    return 0;

                return 1 + _extraUnicornValue;
            }
        }

        private bool _canBeNeigh = true, _canBeDestroyed = true, _canBeSacrificed = true;
        private bool _requiresBasicUnicornInStableToPlay = false;

        public Card(String name, ECardType cardType, List<FactoryEffect> oneTimeFactoryEffects,
            List<TriggerFactoryEffect> triggerFactoryEffects, List<ContinuousFactoryEffect> continuousFactoryEffect,
            bool canBeNeigh, bool canBeSacrificed, bool canBeDestroyed, bool requiresBasicUnicornInStableToPlay,
            int extraUnicornValue) {
            this.Name = name;
            this._cardType = cardType;
            this._extraUnicornValue = extraUnicornValue;

            this._canBeNeigh = canBeNeigh;
            this._canBeSacrificed = canBeSacrificed;
            this._canBeDestroyed = canBeDestroyed;
            this._requiresBasicUnicornInStableToPlay = requiresBasicUnicornInStableToPlay;

            this.oneTimeFactoryEffects = oneTimeFactoryEffects;
            this.triggerEffects = new();
            foreach (var f in triggerFactoryEffects)
                this.triggerEffects.Add(f(this));

            this.continuousEffects = new();
            this._oneTimeTriggerEffects = new();
            this._continuousFactoryEffects = continuousFactoryEffect;

            this.Location = CardLocation.Pile;
        }

        /// <summary>
        /// Can be instant card played?
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">When an invalid game state happens</exception>
        public bool CanPlayInstantCards() {
            if (Player == null)
                throw new InvalidOperationException(CardInHandPlayerNull);

            bool ret = true;
            foreach (var effect in Player.GameController.ContinuousEffects)
                ret &= effect.CanBePlayedInstantCards(Player);
            return ret;
        }

        /// <summary>
        /// Can be card neigh?
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">When an invalid game state happens.</exception>
        public bool CanBeNeigh() {
            if (Player == null)
                throw new InvalidOperationException($"{nameof(Player)} should be not null.");

            bool ret = _canBeNeigh;
            foreach (AContinuousEffect effect in Player.GameController.ContinuousEffects)
                ret &= effect.IsCardNeighable(this);
            return ret;
        }

        /// <summary>
        /// Can be card sacrified?
        /// </summary>
        /// <returns></returns>
        public bool CanBeSacrificed() { return _canBeSacrificed; }

        /// <summary>
        /// Returns if a card can be destroyed by some effect.
        /// If the effect is null, it returns whether the card can be destroyed at all
        /// </summary>
        /// <param name="byEffect"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public bool CanBeDestroyed(AEffect? byEffect) {
            if (Location != CardLocation.OnTable)
                throw new InvalidOperationException("Invalid calling method, this card is not on table!");
            if (Player == null)
                throw new InvalidOperationException(CardOnTablePlayerNull);

            bool ret = _canBeDestroyed;
            foreach (AContinuousEffect effect in Player.GameController.ContinuousEffects)
                ret &= effect.IsCardDestroyable(this, byEffect);
            return ret;
        }

        /// <summary>
        /// Is card playable if card is played to targetOwner's stable
        /// 
        /// If card is spell, then set target owner to player who cast spell
        /// </summary>
        /// <param name="targetOwner"></param>
        /// <returns></returns>
        public bool CanBePlayed(APlayer targetOwner) {
            if (Location != CardLocation.InHand)
                throw new InvalidOperationException(CardNotInHand);
            if (Player == null)
                throw new InvalidOperationException(CardInHandPlayerNull);
            if (targetOwner == null)
                throw new InvalidOperationException($"{nameof(targetOwner)} should not be null");
            if (_cardType == ECardType.Instant)
                throw new InvalidOperationException("Instant cards cannot be played.");

            bool ret = true;
            GameController gameController = Player.GameController;
            if (_requiresBasicUnicornInStableToPlay)
                ret &= Player.Stable.Find(card => card.CardType == ECardType.BasicUnicorn) != null;
            foreach (var effectFactory in oneTimeFactoryEffects)
                if (_cardType != ECardType.Spell)
                    ret &= effectFactory(this).MeetsRequirementsToPlay(gameController);
                else
                    // for spells should be always be target
                    ret &= effectFactory(this).MeetsRequirementsToPlayInner(gameController);
            foreach (var effect in Player.GameController.ContinuousEffects)
                ret &= effect.IsCardPlayable(this, targetOwner);
            return ret;
        }

        /// <summary>
        /// <see cref="AContinuousEffect.CanBeActivatedTriggerEffect(Card, ECardType)"/>
        /// </summary>
        /// <param name="cardType"></param>
        /// <returns></returns>
        public bool CanBeActivatedTriggerEffect(ECardType cardType) {
            if (Player == null)
                throw new InvalidOperationException("Card should be presented on board but `Player` is not set.");

            bool ret = true;
            foreach (var effect in Player.GameController.ContinuousEffects)
                ret &= effect.CanBeActivatedTriggerEffect(this, cardType);
            return ret;
        }

        /// <summary>
        /// Player which own this card or null if it is in pile or discard pile
        /// </summary>
        public APlayer? Player { get; set; }

        /// <summary>
        /// Returns current card type 
        /// </summary>
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

            // spells or unicorns one time effects a.k.a when this card enter the stable
            // there must be passed real card type!!!
            if (CanBeActivatedTriggerEffect(_cardType)) {
                foreach (var factoryEffect in oneTimeFactoryEffects)
                    Player.GameController.AddNewEffectToChainLink(factoryEffect(this));
            }

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

        /// <summary>
        /// Unregister all (one time, trigger, continuous) card effect.
        /// 
        /// Calling <see cref="Card.UnregisterAllEffects"/> multiple times is a safe operation
        /// </summary>
        /// <param name="gameController"></param>
        internal void UnregisterAllEffects(GameController gameController) {
            foreach (var effect in triggerEffects)
                effect.UnsubscribeToEvent(gameController);

            foreach (var effect in new List<TriggerEffect>(_oneTimeTriggerEffects))
                effect.UnsubscribeToEvent(gameController);

            if (_oneTimeTriggerEffects.Count != 0)
                throw new InvalidOperationException("This should be every time empty - UnsubscribeToEvent removes one time effect");

            foreach (var effect in continuousEffects)
                gameController.RemoveContinuousEffect(effect);
            
            this.continuousEffects.Clear();
        }

        /// <summary>
        /// Add a one-time activatable trigger effect on a card.
        /// </summary>
        /// <param name="triggerEffect"></param>
        public void AddOneTimeTriggerEffect(TriggerEffect triggerEffect) {
            _oneTimeTriggerEffects.Add(triggerEffect);
        }

        /// <summary>
        /// Remove a one-time activatable trigger effect on a card.
        /// </summary>
        /// <param name="triggerEffect"></param>
        public void RemoveOneTimeTriggerEffect(TriggerEffect triggerEffect) {
            _oneTimeTriggerEffects.Remove(triggerEffect);
        }

        /// <summary>
        /// Check if card can be played, add this card on stack
        /// and remove this card from hand.
        /// Location is still set on <see cref="CardLocation.InHand"/>
        /// </summary>
        /// <param name="stack"></param>
        public void PlayedInstant(List<Card> stack) {
            if (Location != CardLocation.InHand || _cardType != ECardType.Instant)
                throw new InvalidOperationException("Card can't be played as instant because is not instant card.");
            if (Player == null)
                throw new InvalidOperationException(CardInHandPlayerNull);
            if (!Player.Hand.Remove(this))
                throw new InvalidOperationException("Played card is not located in hand.");
            stack.Add(this);
        }

        /// <summary>
        /// Execute instant card -> instant effect is special effect which
        /// all work is done in <see cref="AEffect.InvokeEffect(GameController)"/>
        /// </summary>
        /// <param name="gameController"></param>
        public void ExecuteInstant(GameController gameController) {
            if (Location != CardLocation.InHand || _cardType != ECardType.Instant)
                throw new InvalidOperationException("Card can't be played as instant because is not instant card.");
            foreach (var effectFactory in oneTimeFactoryEffects) {
                var effect = effectFactory(this);
                effect.InvokeEffect(gameController);
            }
        }

        /// <summary>
        /// Play card to given stable / cast given spell card
        /// 
        /// This method check validity of <paramref name="newCardOwner"/>
        /// but does not check if card can be played!!!
        /// 
        /// The check if card can be played, can pass before the stack is resolved,
        /// but not after resolution.
        /// </summary>
        /// <param name="gameController"></param>
        /// <param name="newCardOwner">To which stable will be card
        /// played or who cast spell card</param>
        public void CardPlayed(GameController gameController, APlayer newCardOwner) {
            if (_cardType == ECardType.Instant)
                throw new InvalidOperationException("Instant card cannot be used by calling CardPlayed");
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
            }
        }

        private List<(int turnNumber, CardLocation from, CardLocation to)> log = new();

        [Conditional("DEBUG")]
        private void AddToLog(GameController gameController, CardLocation newCardLocation) {
            log.Add((gameController.TurnNumber, Location, newCardLocation));
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
            AddToLog(gameController, newCardLocation);
            if ((Location == CardLocation.InHand || Location == CardLocation.OnTable) && Player == null)
                throw new InvalidOperationException($"On location `{Location}` variable Player should be setted!");

            // event must be published before moving card for triggers
            if (Location == CardLocation.OnTable)
                gameController.PublishEvent(ETriggerSource.CardLeftStable, this);

            switch (Location) {
                case CardLocation.Pile:
                    break;
                case CardLocation.DiscardPile:
                    gameController.DiscardPile.Remove(this);
                    break;
                case CardLocation.InHand:
                    Debug.Assert(Player != null);
                    Player.Hand.Remove(this);

                    gameController.CardVisibilityTracker.RemoveCardFromVisibility(Player, this);
                    break;
                case CardLocation.OnTable:
                    Debug.Assert(Player != null);
                    if (ECardTypeUtils.UnicornTarget.Contains(_cardType))
                        Player.Stable.Remove(this);
                    else if (_cardType == ECardType.Upgrade)
                        Player.Upgrades.Remove(this);
                    else if (_cardType == ECardType.Downgrade)
                        Player.Downgrades.Remove(this);
                    else
                        throw new InvalidOperationException($"Card type {_cardType} can't be on Table!");
                    break;
                case CardLocation.Nursery:
                    gameController.Nursery.Remove(this);
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
                    Debug.Assert(Player != null);
                    Player.Hand.Add(this);

                    gameController.CardVisibilityTracker.AddSeenCardToPlayerKnowledge(Player, Player, this);
                    break;
                case CardLocation.OnTable:
                    Debug.Assert(Player != null);
                    if (ECardTypeUtils.UnicornTarget.Contains(_cardType))
                        Player.Stable.Add(this);
                    else if (_cardType == ECardType.Upgrade)
                        Player.Upgrades.Add(this);
                    else if (_cardType == ECardType.Downgrade)
                        Player.Downgrades.Add(this);
                    else
                        throw new InvalidOperationException($"Card type {_cardType} can't be on Table!");
                    break;
                case CardLocation.Nursery:
                    gameController.Nursery.Add(this);
                    break;
                default:
                    throw new InvalidOperationException($"Uknown location {Location}");
            }

            if (Location == CardLocation.OnTable)
                gameController.PublishEvent(ETriggerSource.CardEnteredStable, this);
        }

        /// <summary>
        /// Deep copy card
        /// 
        /// - resetting player
        /// - deep copy trigger effects and continous effects
        /// </summary>
        /// <param name="triggerEffectMapper"></param>
        /// <param name="continuousEffectMapper"></param>
        /// <param name="playerMapper"></param>
        /// <returns></returns>
        public Card Clone(Dictionary<TriggerEffect, TriggerEffect> triggerEffectMapper,
                          Dictionary<AContinuousEffect, AContinuousEffect> continuousEffectMapper,
                          Dictionary<APlayer, APlayer> playerMapper) {
            Card newCard = (Card)MemberwiseClone();

            newCard.Player = Player == null ? null : playerMapper[Player];

            foreach (var effect in triggerEffects)
                triggerEffectMapper.Add(effect, effect.Clone(newCard));
            foreach (var effect in _oneTimeTriggerEffects)
                triggerEffectMapper.Add(effect, effect.Clone(newCard));
            foreach (var effect in continuousEffects)
                continuousEffectMapper.Add(effect, effect.Clone(newCard, playerMapper));

            newCard.triggerEffects = (from effect in triggerEffects select triggerEffectMapper[effect]).ToList();
            newCard._oneTimeTriggerEffects = (from effect in _oneTimeTriggerEffects select triggerEffectMapper[effect]).ToList();
            newCard.continuousEffects = (from effect in continuousEffects select continuousEffectMapper[effect]).ToList();
            newCard.log = new List<(int turnNumber, CardLocation from, CardLocation to)>(log);

            return newCard;
        }

        public override string? ToString() {
            return $"Card [{Name}]";
        }
    }
}
