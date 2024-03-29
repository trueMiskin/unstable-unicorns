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
    public class ResetButtonTest {
        [Fact]
        public void TestSarificeAllUpgradesDowngrades() {
            SimplePlayerMockUp playerOne = new(), playerTwo = new();

            GameController controller = new GameController(new List<Card>(), new List<Card>(), new List<APlayer>() { playerOne, playerTwo }, shufflePlayers: false);

            // protection before shuffling
            Card rainbowAura = new RainbowAura().GetCardTemplate().CreateCard();
            controller.Pile.Add(rainbowAura);
            Card blindingLight = new BlindingLight().GetCardTemplate().CreateCard();
            controller.Pile.Add(blindingLight);
            Card unicornLasso = new UnicornLasso().GetCardTemplate().CreateCard();
            controller.Pile.Add(unicornLasso);
            Card resetButton = new ResetButton().GetCardTemplate().CreateCard();
            controller.Pile.Add(resetButton);

            controller.PlayerDrawCard(playerOne);
            controller.PlayerDrawCard(playerOne);
            controller.PlayerDrawCard(playerOne);
            controller.PlayerDrawCard(playerTwo);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 3, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 1, stableSize: 0, numUpgrades: 0, numDowngrades: 0);

            controller.PlayCardAndResolveChainLink(blindingLight, playerTwo);
            controller.PlayCardAndResolveChainLink(unicornLasso, playerOne);

            controller.PlayCardAndResolveChainLink(rainbowAura, playerTwo);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 1, stableSize: 0, numUpgrades: 1, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 0, numUpgrades: 1, numDowngrades: 1);

            Assert.True(resetButton.CanBePlayed(playerOne));
            controller.PlayCardAndResolveChainLink(resetButton, playerOne);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);

            Assert.Equal(4, controller.Pile.Count);
            Assert.Empty(controller.DiscardPile);
        }
    }
}
