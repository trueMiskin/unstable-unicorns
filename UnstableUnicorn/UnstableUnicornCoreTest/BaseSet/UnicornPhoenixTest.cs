/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
﻿using System.Collections.Generic;
using UnstableUnicornCore.BaseSet;
using UnstableUnicornCore;
using Xunit;

namespace UnstableUnicornCoreTest.BaseSet {
    public class UnicornPhoenixTest {
        [Fact]
        public void TestUnicornPhoenixCannotBeSaved() {
            SimplePlayerMockUp playerOne = new(), playerTwo = new();
            GameController controller = new GameController(new List<Card>(), new List<Card>(), new List<APlayer>() { playerOne, playerTwo }, shufflePlayers: false);

            // protection before shuffling
            Card unicornPoison = new UnicornPoison().GetCardTemplate().CreateCard();
            controller.Pile.Add(unicornPoison);
            Card unicornPhoenix = new UnicornPhoenix().GetCardTemplate().CreateCard();
            controller.Pile.Add(unicornPhoenix);
            
            controller.PlayerDrawCard(playerOne);
            controller.PlayerDrawCard(playerTwo);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 1, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 1, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            Assert.Equal(playerOne.Hand[0], unicornPhoenix);
            Assert.Equal(playerTwo.Hand[0], unicornPoison);

            controller.PlayCardAndResolveChainLink(unicornPhoenix, playerOne);

            controller.PlayCardAndResolveChainLink(unicornPoison, playerTwo);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);

            Assert.Equal(2, controller.DiscardPile.Count);
        }

        [Fact]
        public void TestActivateableTestWithValidTarget() {
            SimplePlayerMockUp playerOne = new(), playerTwo = new();
            GameController controller = new GameController(new List<Card>(), new List<Card>(), new List<APlayer>() { playerOne, playerTwo }, shufflePlayers: false);

            // protection before shuffling
            Card unicornPoison = new UnicornPoison().GetCardTemplate().CreateCard();
            controller.Pile.Add(unicornPoison);
            Card basicUnicorn = new BasicUnicorn().GetCardTemplate().CreateCard();
            controller.Pile.Add(basicUnicorn);
            Card unicornPhoenix = new UnicornPhoenix().GetCardTemplate().CreateCard();
            controller.Pile.Add(unicornPhoenix);

            controller.PlayerDrawCard(playerOne);
            controller.PlayerDrawCard(playerOne);
            controller.PlayerDrawCard(playerTwo);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 2, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 1, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            Assert.Equal(playerOne.Hand[0], unicornPhoenix);
            Assert.Equal(playerOne.Hand[1], basicUnicorn);
            Assert.Equal(playerTwo.Hand[0], unicornPoison);

            controller.PlayCardAndResolveChainLink(unicornPhoenix, playerOne);

            controller.PlayCardAndResolveChainLink(unicornPoison, playerTwo);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 0, stableSize: 1, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);

            Assert.Equal(unicornPhoenix, playerOne.Stable[0]);

            Assert.Equal(2, controller.DiscardPile.Count);
        }
    }
}
