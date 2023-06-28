/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
﻿namespace UnstableUnicornCore.BaseSet {
    public class BasicUnicorn : CardTemplateSource {
        public override CardTemplate GetCardTemplate() {
            return EmptyCard
                .Name("Basic Unicorn")
                .CardType(ECardType.BasicUnicorn)
                .Text("Just basic unicorn.");
        }
    }
}
