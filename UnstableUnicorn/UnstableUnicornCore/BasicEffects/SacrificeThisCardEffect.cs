/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
﻿using System.Collections.Generic;

namespace UnstableUnicornCore.BasicEffects {
    public sealed class SacrificeThisCardEffect : SacrificeEffect {
        public SacrificeThisCardEffect(Card owningCard) : base(owningCard, 1, ECardTypeUtils.CardTarget) {}

        public override void ChooseTargets(GameController gameController) {
            CardTargets.Add(OwningCard);
            CheckAndUpdateSelectionInActualLink(new List<Card>(), CardTargets, gameController);
        }

        public override bool MeetsRequirementsToPlayInner(GameController gameController) {
            return !gameController.CardsWhichAreTargeted.ContainsKey(OwningCard);
        }
    }
}
