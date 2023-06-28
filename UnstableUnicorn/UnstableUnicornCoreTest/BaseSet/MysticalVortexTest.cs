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
    public class MysticalVortexTest {
        [Fact]
        public void TestOwnerPlayerDontHaveAnythingToDiscard() {
            SimplePlayerMockUp playerOne = new(), playerTwo = new();

            GameController controller = new GameController(new List<Card>(), new List<Card>(), new List<APlayer>() { playerOne, playerTwo }, shufflePlayers: false);

            // protection before shuffling
            Card annoyingFlyingUnicorn = new AnnoyingFlyingUnicorn().GetCardTemplate().CreateCard();
            controller.Pile.Add(annoyingFlyingUnicorn);
            Card mysticalVortex = new MysticalVortex().GetCardTemplate().CreateCard();
            controller.Pile.Add(mysticalVortex);

            controller.PlayerDrawCard(playerOne);
            controller.PlayerDrawCard(playerTwo);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 1, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 1, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            Assert.Equal(playerOne.Hand[0], mysticalVortex);
            Assert.Equal(playerTwo.Hand[0], annoyingFlyingUnicorn);
            Assert.Empty(controller.Pile);
            Assert.Empty(controller.DiscardPile);

            TestUtils.CardCantBePlayed(mysticalVortex, playerOne, controller);
        }

        [Fact]
        public void TestDiscardAndShuffleFunctionality() {
            SimplePlayerMockUp playerOne = new(), playerTwo = new();

            GameController controller = new GameController(new List<Card>(), new List<Card>(), new List<APlayer>() { playerOne, playerTwo }, shufflePlayers: false);

            // protection before shuffling
            CardTemplate basicUnicorn = new BasicUnicorn().GetCardTemplate();
            controller.Pile.Add(basicUnicorn.CreateCard());
            controller.Pile.Add(basicUnicorn.CreateCard());
            Card mysticalVortex = new MysticalVortex().GetCardTemplate().CreateCard();
            controller.Pile.Add(mysticalVortex);

            controller.PlayerDrawCard(playerOne);
            controller.PlayerDrawCard(playerOne);
            controller.PlayerDrawCard(playerTwo);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 2, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 1, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            Assert.Equal(playerOne.Hand[0], mysticalVortex);
            Assert.Empty(controller.Pile);
            Assert.Empty(controller.DiscardPile);

            Assert.True(mysticalVortex.CanBePlayed(playerOne));
            // player one playes unicorn to his own stable
            controller.PlayCardAndResolveChainLink(mysticalVortex, playerOne);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            Assert.Equal(3, controller.Pile.Count);
            Assert.Empty(controller.DiscardPile);

            foreach (var card in controller.Pile) {
                Assert.Equal(CardLocation.Pile, card.Location);
                Assert.Null(card.Player);
            }
        }
    }
}
