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
