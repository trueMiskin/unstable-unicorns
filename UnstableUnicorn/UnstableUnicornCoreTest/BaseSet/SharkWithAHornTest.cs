using System.Collections.Generic;
using UnstableUnicornCore;
using UnstableUnicornCore.BaseSet;
using Xunit;

namespace UnstableUnicornCoreTest.BaseSet {
    public class SharkWithAHornTest {
        [Fact]
        public void TestWithoutValidEffectToDestroy() {
            SimplePlayerMockUp playerOne = new(), playerTwo = new();

            GameController controller = new GameController(new List<Card>(), new List<Card>(), new List<APlayer>() { playerOne, playerTwo }, shufflePlayers: false);

            // protection before shuffling
            Card sharkWithAHorn = new SharkWithAHorn().GetCardTemplate().CreateCard();
            controller.Pile.Add(sharkWithAHorn);

            controller.PlayerDrawCard(playerOne);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 1, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            Assert.Equal(playerOne.Hand[0], sharkWithAHorn);

            // self kill, but no valid target for destroy effect
            controller.PlayCardAndResolveChainLink(sharkWithAHorn, playerOne);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);

            Assert.Empty(controller.Pile);
            Assert.Single(controller.DiscardPile);
        }

        [Fact]
        public void TestWithValidEffectToDestroy() {
            SimplePlayerMockUp playerOne = new(), playerTwo = new();

            GameController controller = new GameController(new List<Card>(), new List<Card>(), new List<APlayer>() { playerOne, playerTwo }, shufflePlayers: false);

            // protection before shuffling
            Card basicUnicorn = new BasicUnicorn().GetCardTemplate().CreateCard();
            controller.Pile.Add(basicUnicorn);
            Card sharkWithAHorn = new SharkWithAHorn().GetCardTemplate().CreateCard();
            controller.Pile.Add(sharkWithAHorn);

            controller.PlayerDrawCard(playerOne);
            controller.PlayerDrawCard(playerTwo);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 1, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 1, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            Assert.Equal(playerOne.Hand[0], sharkWithAHorn);
            Assert.Equal(playerTwo.Hand[0], basicUnicorn);

            controller.PlayCardAndResolveChainLink(basicUnicorn, playerTwo);
            // shark will destroy basic unicorn
            controller.PlayCardAndResolveChainLink(sharkWithAHorn, playerOne);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);

            Assert.Empty(controller.Pile);
            Assert.Equal(2, controller.DiscardPile.Count);
        }
    }
}
