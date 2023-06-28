/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace UnstableUnicornCore.Agent {
    public class AgentUtils {
        public static List<Card> GetPlayableCardToPlayerStable(APlayer player) {
            var result = from c in player.Hand
                   where ECardTypeUtils.UnicornTarget.Contains(c.CardType)
                       || c.CardType == ECardType.Spell
                       || c.CardType == ECardType.Upgrade
                       || c.CardType == ECardType.Downgrade
                   where c.CanBePlayed(player)
                   select c;
            return result.ToList();
        }

        public static List<APlayer> WhereCouldBeCardPlayed(GameController controller, Card card) {
            Debug.Assert(card.Player != null);
            if (card.CardType == ECardType.Instant || card.CardType == ECardType.Spell)
                return new List<APlayer> { card.Player };

            return controller.Players.FindAll(p => card.CanBePlayed(p));
        }
    }
}
