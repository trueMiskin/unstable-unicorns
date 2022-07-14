﻿using System.Collections.Generic;
using UnstableUnicornCore;
using UnstableUnicornCore.BaseSet;
using Xunit;

namespace UnstableUnicornCoreTest.BaseSet {
    public class PuppicornTest {
        [Fact]
        public void TestMovingCardToActivePlayerStable() {
            SimplePlayerMockUp playerOne = new(), playerTwo = new();

            GameController controller = new GameController(new List<Card>(), new List<Card>(), new List<APlayer>() { playerOne, playerTwo });

            // protection before shuffling
            Card puppicorn = new Puppicorn().GetCardTemplate().CreateCard();
            controller.Pile.Add(puppicorn);

            controller.PlayerDrawCard(playerOne);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 1, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            Assert.Equal(playerOne.Hand[0], puppicorn);
            Assert.Empty(controller.Pile);
            Assert.Empty(controller.DiscardPile);

            // player one playes unicorn to his own stable
            controller.PlayCardAndResolveChainLink(puppicorn, playerOne);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 0, stableSize: 1, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);

            controller.ActualPlayerOnTurn = playerTwo;
            controller.PublishEvent(ETriggerSource.BeginningTurn);
            controller.ResolveChainLink();

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 1, numUpgrades: 0, numDowngrades: 0);
            Assert.Equal(puppicorn, playerTwo.Stable[0]);
            Assert.Equal(puppicorn.Player, playerTwo);

            controller.ActualPlayerOnTurn = playerOne;
            controller.PublishEvent(ETriggerSource.BeginningTurn);
            controller.ResolveChainLink();

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 0, stableSize: 1, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            Assert.Equal(puppicorn, playerOne.Stable[0]);
            Assert.Equal(puppicorn.Player, playerOne);
        }

        [Fact]
        public void TestCantBeDestroyed() {
            SimplePlayerMockUp playerOne = new(), playerTwo = new();

            GameController controller = new GameController(new List<Card>(), new List<Card>(), new List<APlayer>() { playerOne, playerTwo });

            // protection before shuffling
            Card unicornPoison = new UnicornPoison().GetCardTemplate().CreateCard();
            controller.Pile.Add(unicornPoison);
            Card basicUnicorn = new BasicUnicorn().GetCardTemplate().CreateCard();
            controller.Pile.Add(basicUnicorn);
            Card puppicorn = new Puppicorn().GetCardTemplate().CreateCard();
            controller.Pile.Add(puppicorn);

            controller.PlayerDrawCard(playerOne);
            controller.PlayerDrawCard(playerTwo);
            controller.PlayerDrawCard(playerTwo);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 1, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 2, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            Assert.Equal(playerOne.Hand[0], puppicorn);
            Assert.Equal(playerTwo.Hand[0], basicUnicorn);
            Assert.Equal(playerTwo.Hand[1], unicornPoison);
            Assert.Empty(controller.Pile);
            Assert.Empty(controller.DiscardPile);

            // player one playes unicorn to his own stable
            controller.PlayCardAndResolveChainLink(puppicorn, playerOne);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 0, stableSize: 1, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 2, stableSize: 0, numUpgrades: 0, numDowngrades: 0);

            controller.ActualPlayerOnTurn = playerTwo;
            controller.PublishEvent(ETriggerSource.BeginningTurn);
            controller.ResolveChainLink();

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 2, stableSize: 1, numUpgrades: 0, numDowngrades: 0);
            Assert.Equal(puppicorn, playerTwo.Stable[0]);
            Assert.Equal(puppicorn.Player, playerTwo);

            controller.PlayCardAndResolveChainLink(basicUnicorn, playerTwo);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 1, stableSize: 2, numUpgrades: 0, numDowngrades: 0);

            controller.PlayCardAndResolveChainLink(unicornPoison, playerTwo);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 1, numUpgrades: 0, numDowngrades: 0);

            Assert.False(puppicorn.CanBeSacriced());
            Assert.False(puppicorn.CanBeDestroyed());
            Assert.Equal(puppicorn, playerTwo.Stable[0]);
        }
    }
}