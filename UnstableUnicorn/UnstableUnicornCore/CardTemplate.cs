using System;
using System.Collections.Generic;

namespace UnstableUnicornCore {
    /// <summary>
    /// Helper class for the creation of the cards. This class provides a fluent syntax for setting the card properties.
    /// </summary>
    public class CardTemplate {
        private String name;
        private ECardType _cardType;
        private int _extraUnicornValue;
        private List<Card.FactoryEffect> oneTimeFactoryEffects = new();
        private List<Card.TriggerFactoryEffect> triggerFactoryEffects = new();
        private List<Card.ContinuousFactoryEffect> continuousFactoryEffects = new();
        private bool canBeNeigh = true, canBeSacrificed = true, canBeDestroyed = true;
        private bool requiresBasicUnicornInStableToPlay = false;

        /// <summary>
        /// Set the card name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public CardTemplate Name(string name) {
            this.name = name;
            return this;
        }

        /// <summary>
        /// Set the card type
        /// </summary>
        /// <param name="cardType"></param>
        /// <returns></returns>
        public CardTemplate CardType(ECardType cardType) {
            _cardType = cardType;
            return this;
        }

        /// <summary>
        /// Set the effect of the card
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public CardTemplate Text(string text) {
            return this;
        }

        /// <summary>
        /// Does this unicorn count as more unicorns?
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public CardTemplate ExtraUnicornValue(int value) {
            _extraUnicornValue = value;
            return this;
        }

        /// <summary>
        /// Add a new one-time effect to the card
        /// </summary>
        /// <param name="factoryEffect"></param>
        /// <returns></returns>
        public CardTemplate Cast(Card.FactoryEffect factoryEffect) {
            oneTimeFactoryEffects.Add(factoryEffect);
            return this;
        }

        /// <summary>
        /// Add a new trigger effect to the card
        /// </summary>
        /// <param name="triggerPredicate"></param>
        /// <param name="triggers"></param>
        /// <param name="factoryEffect"></param>
        /// <returns></returns>
        public CardTemplate TriggerEffect(TriggerEffect.TriggerPredicate triggerPredicate, List<ETriggerSource> triggers, Card.FactoryEffect factoryEffect) {
            triggerFactoryEffects.Add((Card owningCard) => new TriggerEffect(
                owningCard,
                triggerPredicate,
                triggers,
                (Card card, GameController _) => factoryEffect(card)));
            return this;
        }

        /// <summary>
        /// Add a continuous effect to the card
        /// </summary>
        /// <param name="continuousFactoryEffect"></param>
        /// <returns></returns>
        public CardTemplate ContinuousFactory(Card.ContinuousFactoryEffect continuousFactoryEffect) {
            continuousFactoryEffects.Add(continuousFactoryEffect);
            return this;
        }

        /// <summary>
        /// A card cannot be neigh during the playing
        /// </summary>
        /// <returns></returns>
        public CardTemplate CantBeNeigh() {
            canBeNeigh = false;
            return this;
        }

        /// <summary>
        /// A card cannot be sacrificed
        /// </summary>
        /// <returns></returns>
        public CardTemplate CantBeSacrificed() {
            canBeSacrificed = false;
            return this;
        }

        /// <summary>
        /// A card cannot be destroyed
        /// </summary>
        /// <returns></returns>
        public CardTemplate CantBeDestroyed() {
            canBeDestroyed = false;
            return this;
        }

        /// <summary>
        /// Must be the card played when at least one basic unicorn is in the owner's stable?
        /// </summary>
        /// <returns></returns>
        public CardTemplate RequiredBasicUnicornInStableToPlay() {
            requiresBasicUnicornInStableToPlay = true;
            return this;
        }

        public Card CreateCard() {
            return new Card(name, _cardType, oneTimeFactoryEffects, triggerFactoryEffects, continuousFactoryEffects,
                canBeNeigh, canBeSacrificed, canBeDestroyed, requiresBasicUnicornInStableToPlay, _extraUnicornValue);
        }
    }
}
