/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
﻿namespace UnstableUnicornCore.BasicEffects {
    public sealed class ShuffleDeckEffect : AEffect {
        bool addDiscardPileToPile;
        public ShuffleDeckEffect(Card owningCard, bool addDiscardPileToPile = false) : base(owningCard, 0) {
            this.addDiscardPileToPile = addDiscardPileToPile;
        }

        public override void ChooseTargets(GameController gameController) {}

        public override void InvokeEffect(GameController gameController) {
            if (addDiscardPileToPile)
                gameController.AddDiscardPileToPile();

            gameController.ShuffleDeck();
        }

        public override bool MeetsRequirementsToPlayInner(GameController gameController) => true;
    }
}
