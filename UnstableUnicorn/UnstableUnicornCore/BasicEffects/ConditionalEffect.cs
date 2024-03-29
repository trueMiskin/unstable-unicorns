/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
﻿using System.Collections.Generic;

namespace UnstableUnicornCore.BasicEffects {
    public sealed class ConditionalEffect : AEffect {
        private AEffect _condition;
        private AEffect _thenEffect;
        public ConditionalEffect(Card owningCard, AEffect condition, AEffect thenEffect)
            : base(owningCard, 0 /* For this effect is not needed*/) {
            _condition = condition;
            _thenEffect = thenEffect;
        }

        public override void ChooseTargets(GameController gameController) {
            gameController.AddEffectAfterSelectedEffectToCurrentChainLink(_condition, this);
        }

        public override void InvokeEffect(GameController gameController) {
            gameController.AddNewEffectToChainLink(_thenEffect);
        }

        public override bool MeetsRequirementsToPlay(GameController gameController) {
            return _condition.MeetsRequirementsToPlayInner(gameController);
        }

        public override bool MeetsRequirementsToPlayInner(GameController gameController) => MeetsRequirementsToPlay(gameController);

        public override AEffect Clone(Dictionary<Card, Card> cardMapper,
                                      Dictionary<AEffect, AEffect> effectMapper,
                                      Dictionary<APlayer, APlayer> playerMapper) {
            var newEffect = (ConditionalEffect)base.Clone(cardMapper, effectMapper, playerMapper);
            newEffect._condition = _condition.Clone(cardMapper, effectMapper, playerMapper);
            effectMapper.Add(_condition, newEffect._condition);
            newEffect._thenEffect = _thenEffect.Clone(cardMapper, effectMapper, playerMapper);
            effectMapper.Add(_thenEffect, newEffect._thenEffect);

            return newEffect;
        }
    }
}
