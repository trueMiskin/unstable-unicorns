/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
﻿namespace UnstableUnicornCore.BasicContinuousEffects {
    public class PlayerCantPlayUpgrades : AContinuousEffect {
        public PlayerCantPlayUpgrades(Card owningCard) : base(owningCard) {}

        public override bool IsCardPlayable(Card card, APlayer targetOwner) {
            if (card.Player != OwningPlayer)
                return true;

            return card.CardType != ECardType.Upgrade;
        }
    }
}
