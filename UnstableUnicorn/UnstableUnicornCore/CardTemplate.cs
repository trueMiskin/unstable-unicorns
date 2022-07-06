using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnstableUnicornCore {
    public class CardTemplate {
        private String name;
        private ECardType _cardType;
        private List<Card.FactoryEffect> oneTimeFactoryEffects = new();
        private List<Card.TriggerFactoryEffect> triggerFactoryEffects = new();
        private List<Card.ContinuousFactoryEffect> continuousFactoryEffects = new();

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

        public CardTemplate Cast(Card.FactoryEffect factoryEffect) {
            oneTimeFactoryEffects.Add(factoryEffect);
            return this;
        }

        public CardTemplate TriggerEffect(Card.TriggerFactoryEffect triggerFactoryEffect) {
            triggerFactoryEffects.Add(triggerFactoryEffect);
            return this;
        }

        public CardTemplate ContinuousFactory(Card.ContinuousFactoryEffect continuousFactoryEffect) {
            continuousFactoryEffects.Add(continuousFactoryEffect);
            return this;
        }

        public Card CreateCard() {
            return new Card(name, _cardType, oneTimeFactoryEffects, triggerFactoryEffects, continuousFactoryEffects);
        }
    }
}
