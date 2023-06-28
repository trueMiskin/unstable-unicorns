/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
﻿namespace UnstableUnicornCore.BasicEffects {
    public sealed class DrawEffect : AEffect {
        public DrawEffect(Card owningCard, int cardCount) : base(owningCard, cardCount) {}

        public override void ChooseTargets(GameController gameController) {}

        public override void InvokeEffect(GameController gameController) {
            for (int i = 0; i < CardCount; i++)
                gameController.PlayerDrawCard(OwningPlayer);
        }

        public override bool MeetsRequirementsToPlayInner(GameController gameController) => true;
    }
}
