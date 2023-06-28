/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
﻿using System.Collections.Generic;

namespace UnstableUnicornCore.BasicContinuousEffects {
    public class PlayerCardsCantBeDestroyed : AContinuousEffect {
        // card types which can be targeted
        List<ECardType> _protectedCardTypes;
        public PlayerCardsCantBeDestroyed(Card owningCard, List<ECardType> protectedCardTypes) : base(owningCard) {
            _protectedCardTypes = protectedCardTypes;
        }

        public override bool IsCardDestroyable(Card card, AEffect? byEffect) {
            return !(card.Player == OwningPlayer && _protectedCardTypes.Contains(card.CardType));
        }
    }
}
