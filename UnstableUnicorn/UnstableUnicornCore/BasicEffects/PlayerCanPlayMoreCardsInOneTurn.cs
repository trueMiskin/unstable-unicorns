/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
﻿using System;

namespace UnstableUnicornCore.BasicEffects {
    public class PlayerCanPlayMoreCardsInOneTurn : AEffect {
        int _maxPlayableCards;
        public PlayerCanPlayMoreCardsInOneTurn(Card owningCard, int maxPlayableCards) : base(owningCard, 0) {
            _maxPlayableCards = maxPlayableCards;
        }

        public override void ChooseTargets(GameController gameController) {}

        public override void InvokeEffect(GameController gameController) {
            gameController.MaxCardsToPlayInOneTurn = Math.Max(gameController.MaxCardsToPlayInOneTurn,
                _maxPlayableCards);
        }

        public override bool MeetsRequirementsToPlayInner(GameController gameController) => true;
    }
}
