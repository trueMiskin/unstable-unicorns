/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
﻿using System;
using System.Collections.Generic;
using UnstableUnicornCore.BasicEffects;

namespace UnstableUnicornCore.BaseSet {
    public class NarwhalTorpedo : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return EmptyCard
                .Name("Narwhal Torpedo")
                .CardType(ECardType.MagicUnicorn)
                .Text("When this card enters your Stable, SACRIFICE all Downgrade cards.")
                .Cast((Card owningCard) =>
                    new SacrificeEffect(owningCard, Int32.MaxValue, new List<ECardType>() { ECardType.Downgrade })
                );
        }
    }
}
