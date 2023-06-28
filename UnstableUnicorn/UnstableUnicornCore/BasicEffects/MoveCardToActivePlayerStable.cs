/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
﻿namespace UnstableUnicornCore.BasicEffects {
    /// <summary>
    /// This card is supposed to be triggered on <see cref="ETriggerSource.BeginningTurn"/>
    /// </summary>
    public class MoveCardToActivePlayerStable : AEffect {
        public MoveCardToActivePlayerStable(Card owningCard) : base(owningCard, 0) {
            TargetLocation = CardLocation.OnTable;
        }

        public override void ChooseTargets(GameController gameController) {}

        public override void InvokeEffect(GameController gameController) {
            OwningCard.MoveCard(gameController, gameController.ActualPlayerOnTurn, TargetLocation);
        }

        public override bool MeetsRequirementsToPlayInner(GameController gameController) => true;
    }
}
