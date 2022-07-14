using System;
using System.Collections.Generic;
using UnstableUnicornCore;
using UnstableUnicornCore.BaseSet;
using Xunit;

namespace UnstableUnicornCoreTest.BaseSet {
    public class ReTargetTest {
        [Fact]
        public void TestRequirementsToPlay() {
            SimplePlayerMockUp playerOne = new(), playerTwo = new();

            GameController controller = new GameController(new List<Card>(), new List<Card>(), new List<APlayer>() { playerOne, playerTwo });

            // protection before shuffling
            Card retarget = new ReTarget().GetCardTemplate().CreateCard();
            controller.Pile.Add(retarget);
            controller.PlayerDrawCard(playerOne);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 1, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            Assert.Equal(playerOne.Hand[0], retarget);
            Assert.Empty(controller.Pile);
            Assert.Empty(controller.DiscardPile);

            TestUtils.CardCantBePlayed(retarget, playerOne, controller);
        }

        [Fact]
        public void TestMovingDowngrade() {
            SimplePlayerMockUp playerOne = new(), playerTwo = new();
            playerOne.WhichEffectShouldBeSelected = 1; // move downgrade

            GameController controller = new GameController(new List<Card>(), new List<Card>(), new List<APlayer>() { playerOne, playerTwo });

            // protection before shuffling
            Card brokenStable = new BrokenStable().GetCardTemplate().CreateCard();
            controller.Pile.Add(brokenStable);
            Card retarget = new ReTarget().GetCardTemplate().CreateCard();
            controller.Pile.Add(retarget);
            
            controller.PlayerDrawCard(playerOne);
            controller.PlayerDrawCard(playerOne);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 2, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            Assert.Equal(playerOne.Hand[0], retarget);
            Assert.Equal(playerOne.Hand[1], brokenStable);
            Assert.Empty(controller.Pile);
            Assert.Empty(controller.DiscardPile);

            // playing downgrade to own stable
            controller.PlayCardAndResolveChainLink(brokenStable, playerOne);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 1, stableSize: 0, numUpgrades: 0, numDowngrades: 1);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);

            Assert.True(retarget.CanBePlayed(playerOne));
            controller.PlayCardAndResolveChainLink(retarget, playerOne);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 1);
        }

        [Fact]
        public void TestMovingUpgrade() {
            SimplePlayerMockUp playerOne = new(), playerTwo = new();
            playerOne.WhichEffectShouldBeSelected = 0; // move upgrade

            GameController controller = new GameController(new List<Card>(), new List<Card>(), new List<APlayer>() { playerOne, playerTwo });

            // protection before shuffling
            Card rainbowAura = new RainbowAura().GetCardTemplate().CreateCard();
            controller.Pile.Add(rainbowAura);
            Card retarget = new ReTarget().GetCardTemplate().CreateCard();
            controller.Pile.Add(retarget);

            controller.PlayerDrawCard(playerOne);
            controller.PlayerDrawCard(playerOne);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 2, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            Assert.Equal(playerOne.Hand[0], retarget);
            Assert.Equal(playerOne.Hand[1], rainbowAura);
            Assert.Empty(controller.Pile);
            Assert.Empty(controller.DiscardPile);

            // playing downgrade to own stable
            controller.PlayCardAndResolveChainLink(rainbowAura, playerOne);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 1, stableSize: 0, numUpgrades: 1, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);

            Assert.True(retarget.CanBePlayed(playerOne));
            controller.PlayCardAndResolveChainLink(retarget, playerOne);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 0, numUpgrades: 1, numDowngrades: 0);
        }
    }
}
