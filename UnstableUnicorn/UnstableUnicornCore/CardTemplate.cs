using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnstableUnicornCore {
    public class CardTemplate {
        private String name;
        private ECardType _cardType;
        private int _extraUnicornValue;
        private List<Card.FactoryEffect> oneTimeFactoryEffects = new();
        private List<Card.TriggerFactoryEffect> triggerFactoryEffects = new();
        private List<Card.ContinuousFactoryEffect> continuousFactoryEffects = new();
        private bool canBeNeigh = true, canBeSacrificed = true, canBeDestroyed = true;
        private bool requiresBasicUnicornInStableToPlay = false;

        public CardTemplate Name(string name) {
            this.name = name;
            return this;
        }

        public CardTemplate CardType(ECardType cardType) {
            _cardType = cardType;
            return this;
        }

        public CardTemplate Text(string text) {
            return this;
        }

        public CardTemplate ExtraUnicornValue(int value) {
            _extraUnicornValue = value;
            return this;
        }

        public CardTemplate Cast(Card.FactoryEffect factoryEffect) {
            oneTimeFactoryEffects.Add(factoryEffect);
            return this;
        }

        public CardTemplate TriggerEffect(TriggerEffect.TriggerPredicate triggerPredicate, List<ETriggerSource> triggers, Card.FactoryEffect factoryEffect) {
            triggerFactoryEffects.Add((Card owningCard) => new TriggerEffect(
                owningCard,
                triggerPredicate,
                triggers,
                (Card card, GameController _) => factoryEffect(card)));
            return this;
        }

        public CardTemplate ContinuousFactory(Card.ContinuousFactoryEffect continuousFactoryEffect) {
            continuousFactoryEffects.Add(continuousFactoryEffect);
            return this;
        }

        public CardTemplate CantBeNeigh() {
            canBeNeigh = false;
            return this;
        }

        public CardTemplate CantBeSacrificed() {
            canBeSacrificed = false;
            return this;
        }
        
        public CardTemplate CantBeDestroyed() {
            canBeDestroyed = false;
            return this;
        }

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
