using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnstableUnicornCore {
    public enum PlayerTargeting {
        PlayerOwner,
        AnyPlayer,
        EachPlayer,
        EachOtherPlayer,  // excludes player owner
    }
}
