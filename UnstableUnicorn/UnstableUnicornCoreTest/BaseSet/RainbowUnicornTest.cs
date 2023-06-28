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
    public class RainbowUnicornTest {
        [Fact]
        public void TestBringEffectWithoutValidTarget() {
            SimplePlayerMockUp playerOne = new(), playerTwo = new();

            GameController controller = new GameController(new List<Card>(), new List<Card>(), new List<APlayer>() { playerOne, playerTwo }, shufflePlayers: false);

            // protection before shuffling
            Card americorn = new Americorn().GetCardTemplate().CreateCard();
            controller.Pile.Add(americorn);
            Card rainbowMane = new RainbowMane().GetCardTemplate().CreateCard();
            controller.Pile.Add(rainbowMane);
            Card brokenStable = new BrokenStable().GetCardTemplate().CreateCard();
            controller.Pile.Add(brokenStable);
            Card rainbowUnicorn = new RainbowUnicorn().GetCardTemplate().CreateCard();
            controller.Pile.Add(rainbowUnicorn);

            controller.PlayerDrawCard(playerOne);
            controller.PlayerDrawCard(playerOne);
            controller.PlayerDrawCard(playerOne);
            controller.PlayerDrawCard(playerOne);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 4, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);

            controller.PlayCardAndResolveChainLink(rainbowUnicorn, playerOne);

            // no valid target for rainbow unicorn
            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 3, stableSize: 1, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(5)]
        public void TestBringEffectWithValidTarget(int numberBasicUnicorns) {
            SimplePlayerMockUp playerOne = new(), playerTwo = new();

            GameController controller = new GameController(new List<Card>(), new List<Card>(), new List<APlayer>() { playerOne, playerTwo }, shufflePlayers: false);

            // protection before shuffling
            for (int i = 0; i < numberBasicUnicorns; i++) {
                Card basicUnicorn = new BasicUnicorn().GetCardTemplate().CreateCard();
                controller.Pile.Add(basicUnicorn);
                controller.PlayerDrawCard(playerOne);
            }

            Card americorn = new Americorn().GetCardTemplate().CreateCard();
            controller.Pile.Add(americorn);
            Card rainbowMane = new RainbowMane().GetCardTemplate().CreateCard();
            controller.Pile.Add(rainbowMane);
            Card brokenStable = new BrokenStable().GetCardTemplate().CreateCard();
            controller.Pile.Add(brokenStable);
            Card rainbowUnicorn = new RainbowUnicorn().GetCardTemplate().CreateCard();
            controller.Pile.Add(rainbowUnicorn);

            controller.PlayerDrawCard(playerOne);
            controller.PlayerDrawCard(playerOne);
            controller.PlayerDrawCard(playerOne);
            controller.PlayerDrawCard(playerOne);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 4 + numberBasicUnicorns, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);

            controller.PlayCardAndResolveChainLink(rainbowUnicorn, playerOne);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 3 + numberBasicUnicorns - 1, stableSize: 2, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
        }
    }
}
