/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
﻿using System.Collections.Generic;
using UnstableUnicornCore.BasicEffects;

namespace UnstableUnicornCore.BaseSet {
    /// <summary>
    /// Used text from 2nd edition text instead second print:
    /// When this card enters your Stable, you may SACRIFICE or DESTROY an Upgrade or Downgrade card.
    /// </summary>
    public class ChainsawUnicorn : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return EmptyCard
                .Name("Chainsaw Unicorn")
                .CardType(ECardType.MagicUnicorn)
                .Text("When this card enters your Stable, you may DESTROY an Upgrade card or SACRIFICE a Downgrade card.")
                .Cast( (Card owningCard) => new ActivatableEffect(owningCard,
                    new ChooseEffect(owningCard,
                        new List<AEffect> {
                            new DestroyEffect(owningCard, 1, new List<ECardType>{ECardType.Upgrade}),
                            new SacrificeEffect(owningCard, 1, new List<ECardType>{ECardType.Downgrade})
                        }
                    ))
                );
        }
    }
}
