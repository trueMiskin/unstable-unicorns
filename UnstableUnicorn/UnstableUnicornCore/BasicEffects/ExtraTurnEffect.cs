/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
﻿namespace UnstableUnicornCore.BasicEffects {
    /// <summary>
    /// Actual player will take extra turn
    /// </summary>
    public sealed class ExtraTurnEffect : AEffect {
        public ExtraTurnEffect(Card owningCard) : base(owningCard, 0 /* For this effect is not needed*/) { }

        public override void ChooseTargets(GameController gameController) {}

        public override void InvokeEffect(GameController gameController) {
            gameController.ThisPlayerTakeExtraTurn();
        }

        public override bool MeetsRequirementsToPlayInner(GameController gameController) => true;
    }
}
