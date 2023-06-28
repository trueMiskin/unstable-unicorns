/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
﻿namespace UnstableUnicornCore.BasicContinuousEffects {
    public class SetPlayersCardTypeOfUnicorn : AContinuousEffect {
        private ECardType _newCardType;
        public SetPlayersCardTypeOfUnicorn(Card owningCard, ECardType newCardType) : base(owningCard) {
            _newCardType = newCardType;
        }

        public override ECardType GetCardType(ECardType actualCardType, APlayer playerOwner) {
            if (OwningPlayer != playerOwner)
                return actualCardType;

            if (ECardTypeUtils.UnicornTarget.Contains(actualCardType))
                return _newCardType;

            return actualCardType;
        }
    }
}
