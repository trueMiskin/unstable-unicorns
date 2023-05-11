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
        PreCardLeftStable,
        CardLeftStable,
        EndTurn,
    }
}
