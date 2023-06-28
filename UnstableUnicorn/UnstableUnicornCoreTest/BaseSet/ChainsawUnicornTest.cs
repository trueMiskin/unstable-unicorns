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
    public class ChainsawUnicornTest {
        [Fact]
        public void TestChooseDestroyUpgrade() {
            SimplePlayerMockUp playerOne = new(), playerTwo = new();
            playerTwo.WhichEffectShouldBeSelected = 0; // destroy a upgrade

            GameController controller = new GameController(new List<Card>(), new List<Card>(), new List<APlayer>() { playerOne, playerTwo }, shufflePlayers: false);

            // protection before shuffling
            Card chainsawUnicorn = new ChainsawUnicorn().GetCardTemplate().CreateCard();
            controller.Pile.Add(chainsawUnicorn);
            Card rainbowAura = new RainbowAura().GetCardTemplate().CreateCard();
            controller.Pile.Add(rainbowAura);

            controller.PlayerDrawCard(playerOne);
            controller.PlayerDrawCard(playerTwo);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 1, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 1, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            Assert.Equal(playerOne.Hand[0], rainbowAura);
            Assert.Equal(playerTwo.Hand[0], chainsawUnicorn);

            // player one playes upgrade to his own stable
            controller.PlayCardAndResolveChainLink(rainbowAura, playerOne);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 0, stableSize: 0, numUpgrades: 1, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 1, stableSize: 0, numUpgrades: 0, numDowngrades: 0);

            // player two play unicorn to his own stable
            controller.PlayCardAndResolveChainLink(chainsawUnicorn, playerTwo);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 1, numUpgrades: 0, numDowngrades: 0);
        }

        [Fact]
        public void TestChooseSacrificeDowngrade() {
            SimplePlayerMockUp playerOne = new(), playerTwo = new();
            
            // normally sacrifice downgrade is second effects but requirements for first effect is not met
            playerTwo.WhichEffectShouldBeSelected = 0;

            GameController controller = new GameController(new List<Card>(), new List<Card>(), new List<APlayer>() { playerOne, playerTwo }, shufflePlayers: false);

            // protection before shuffling
            Card chainsawUnicorn = new ChainsawUnicorn().GetCardTemplate().CreateCard();
            controller.Pile.Add(chainsawUnicorn);
            Card pandamonium = new Pandamonium().GetCardTemplate().CreateCard();
            controller.Pile.Add(pandamonium);

            controller.PlayerDrawCard(playerOne);
            controller.PlayerDrawCard(playerTwo);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 1, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 1, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            Assert.Equal(playerOne.Hand[0], pandamonium);
            Assert.Equal(playerTwo.Hand[0], chainsawUnicorn);

            // player one playes downgrade to opponent's stable
            controller.PlayCardAndResolveChainLink(pandamonium, playerTwo);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 1, stableSize: 0, numUpgrades: 0, numDowngrades: 1);

            // player two play unicorn to his own stable
            controller.PlayCardAndResolveChainLink(chainsawUnicorn, playerTwo);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 1, numUpgrades: 0, numDowngrades: 0);
        }
    }
}
