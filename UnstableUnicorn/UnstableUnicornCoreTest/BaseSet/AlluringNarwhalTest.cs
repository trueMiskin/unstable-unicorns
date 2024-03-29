/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
﻿using System.Collections.Generic;
using UnstableUnicornCore;
using UnstableUnicornCore.BaseSet;
using Xunit;

namespace UnstableUnicornCoreTest.BaseSet {
    public class AlluringNarwhalTest {
        [Fact]
        public void TestStealEffectWithoutValidTarget() {
            SimplePlayerMockUp playerOne = new(), playerTwo = new();
            GameController controller = new GameController(new List<Card>(), new List<Card>(), new List<APlayer>() { playerOne, playerTwo }, shufflePlayers: false);

            // protection before shuffling
            Card basicUnicorn = new BasicUnicorn().GetCardTemplate().CreateCard();
            controller.Pile.Add(basicUnicorn);
            Card alluringNarwhal = new AlluringNarwhal().GetCardTemplate().CreateCard();
            controller.Pile.Add(alluringNarwhal);

            controller.PlayerDrawCard(playerTwo);
            controller.PlayerDrawCard(playerOne);

            Assert.Equal(playerOne.Hand[0], basicUnicorn);
            Assert.Equal(playerTwo.Hand[0], alluringNarwhal);

            controller.PlayCardAndResolveChainLink(basicUnicorn, playerOne);
            controller.PlayCardAndResolveChainLink(alluringNarwhal, playerTwo);

            Assert.Empty(playerOne.Hand);
            Assert.Single(playerOne.Stable);
            Assert.Empty(playerOne.Upgrades);
            Assert.Empty(playerOne.Downgrades);

            Assert.Empty(playerTwo.Hand);
            Assert.Single(playerTwo.Stable);
            Assert.Empty(playerTwo.Upgrades);
            Assert.Empty(playerTwo.Downgrades);
        }

        [Fact]
        public void TestStealEffectWithValidTarget() {
            SimplePlayerMockUp playerOne = new(), playerTwo = new();
            GameController controller = new GameController(new List<Card>(), new List<Card>(), new List<APlayer>() { playerOne, playerTwo }, shufflePlayers: false);

            // protection before shuffling
            Card rainbowAura = new RainbowAura().GetCardTemplate().CreateCard();
            controller.Pile.Add(rainbowAura);
            Card alluringNarwhal = new AlluringNarwhal().GetCardTemplate().CreateCard();
            controller.Pile.Add(alluringNarwhal);

            controller.PlayerDrawCard(playerTwo);
            controller.PlayerDrawCard(playerOne);

            Assert.Equal(playerOne.Hand[0], rainbowAura);
            Assert.Equal(playerTwo.Hand[0], alluringNarwhal);

            controller.PlayCardAndResolveChainLink(rainbowAura, playerOne);
            controller.PlayCardAndResolveChainLink(alluringNarwhal, playerTwo);

            Assert.Empty(playerOne.Hand);
            Assert.Empty(playerOne.Stable);
            Assert.Empty(playerOne.Upgrades);
            Assert.Empty(playerOne.Downgrades);

            Assert.Empty(playerTwo.Hand);
            Assert.Single(playerTwo.Stable);
            Assert.Single(playerTwo.Upgrades);
            Assert.Empty(playerTwo.Downgrades);
        }
    }
}
