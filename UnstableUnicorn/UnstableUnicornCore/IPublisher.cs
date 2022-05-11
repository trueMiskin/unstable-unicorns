using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnstableUnicornCore {
    public enum ETriggerSource {
        BeginningTurn,
        CardEnteredStable,
        ChangeTargeting,
        ChangeLocationOfCard,
        CardLeftStable,
        EndTurn,
    }
    public interface IPublisher {
        void SubscribeEvent(ETriggerSource _event, TriggerEffect effect);
        void UnsubscribeEvent(ETriggerSource _event, TriggerEffect effect);
    }

    // TODO: restriction on T
    public interface ISubscriber<T> {
    }
}
