using System;
using System.Collections.Generic;
using UnstableUnicornCore;
using UnstableUnicornCore.BaseSet;
using Xunit;

namespace UnstableUnicornCoreTest.BaseSet {
    public class UnicornLassoTest {
        [Fact]
        public void TestStealAndReturnEffectWithoutTarget() {
            SimplePlayerMockUp playerOne = new(), playerTwo = new();
            GameController controller = new GameController(new List<Card>(), new List<Card>(), new List<APlayer>() { playerOne, playerTwo });

            // protection before shuffling
            Card unicornLasso = new UnicornLasso().GetCardTemplate().CreateCard();
            controller.Pile.Add(unicornLasso);

            controller.PlayerDrawCard(playerOne);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 1, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            Assert.Equal(playerOne.Hand[0], unicornLasso);

            controller.PlayCardAndResolveChainLink(unicornLasso, playerOne);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 0, stableSize: 0, numUpgrades: 1, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            Assert.Equal(playerOne.Upgrades[0], unicornLasso);
            
            // actual player ot turn is by default the first player
            controller.PublishEvent(ETriggerSource.BeginningTurn);
            // now event should be added to chain link
            Assert.Empty(controller.ActualChainLink);
            Assert.Single(controller.NextChainLink);
            Assert.Equal(unicornLasso, controller.NextChainLink[0].OwningCard);

            controller.ResolveChainLink();
            // should not fail to resolve
            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 0, stableSize: 0, numUpgrades: 1, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            Assert.Equal(playerOne.Upgrades[0], unicornLasso);
        }

        [Fact]
        public void TestStealAndReturnEffectWithTarget() {
            SimplePlayerMockUp playerOne = new(), playerTwo = new();
            GameController controller = new GameController(new List<Card>(), new List<Card>(), new List<APlayer>() { playerOne, playerTwo });

            // protection before shuffling
            Card basicUnicorn = new BasicUnicorn().GetCardTemplate().CreateCard();
            controller.Pile.Add(basicUnicorn);
            Card unicornLasso = new UnicornLasso().GetCardTemplate().CreateCard();
            controller.Pile.Add(unicornLasso);

            controller.PlayerDrawCard(playerOne);
            controller.PlayerDrawCard(playerTwo);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 1, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 1, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            Assert.Equal(playerOne.Hand[0], unicornLasso);
            Assert.Equal(playerTwo.Hand[0], basicUnicorn);

            controller.PlayCardAndResolveChainLink(unicornLasso, playerOne);
            controller.PlayCardAndResolveChainLink(basicUnicorn, playerTwo);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 0, stableSize: 0, numUpgrades: 1, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 1, numUpgrades: 0, numDowngrades: 0);
            Assert.Equal(playerOne.Upgrades[0], unicornLasso);
            Assert.Equal(playerTwo.Stable[0], basicUnicorn);

            // actual player ot turn is by default the first player
            controller.PublishEvent(ETriggerSource.BeginningTurn);
            // now event should be added to chain link
            Assert.Empty(controller.ActualChainLink);
            Assert.Single(controller.NextChainLink);
            Assert.Equal(unicornLasso, controller.NextChainLink[0].OwningCard);

            controller.ResolveChainLink();
            // should not fail to resolve
            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 0, stableSize: 1, numUpgrades: 1, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            Assert.Equal(playerOne.Upgrades[0], unicornLasso);
            Assert.Equal(playerOne.Stable[0], basicUnicorn);

            Assert.Single(basicUnicorn.OneTimeTriggerEffects);

            controller.PublishEvent(ETriggerSource.EndTurn);
            controller.ResolveChainLink();

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 0, stableSize: 0, numUpgrades: 1, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 1, numUpgrades: 0, numDowngrades: 0);
            Assert.Equal(playerOne.Upgrades[0], unicornLasso);
            Assert.Equal(playerTwo.Stable[0], basicUnicorn);

            Assert.Empty(basicUnicorn.OneTimeTriggerEffects);
        }
    }
}
