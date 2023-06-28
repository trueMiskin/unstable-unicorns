/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
﻿namespace UnstableUnicornCore.BasicContinuousEffects {
    public class CardCannotBeDestroyedByMagicCard : AContinuousEffect {
        public CardCannotBeDestroyedByMagicCard(Card owningCard) : base(owningCard) {}

        public override bool IsCardDestroyable(Card card, AEffect? byEffect) {
            if (byEffect == null)
                return true;

            if (byEffect.OwningCard.CardType == ECardType.Spell)
                return false;

            return true;
        }
    }
}
