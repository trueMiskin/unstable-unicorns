/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
﻿using UnstableUnicornCore.BasicContinuousEffects;

namespace UnstableUnicornCore.BaseSet {
    public class Pandamonium : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return EmptyCard
                .Name("Pandamonium")
                .CardType(ECardType.Downgrade)
                .Text("All of your Unicorns are considered Pandas. Cards that affect Unicorn cards do not affect your Pandas.")
                .ContinuousFactory((Card owningCard) =>
                    new SetPlayersCardTypeOfUnicorn(owningCard, ECardType.Panda)
                );
        }
    }
}
