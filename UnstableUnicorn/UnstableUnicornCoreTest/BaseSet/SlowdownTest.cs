using System.Collections.Generic;
using UnstableUnicornCore;
using UnstableUnicornCore.BaseSet;
using Xunit;

namespace UnstableUnicornCoreTest.BaseSet {
    public class SlowdownTest {
        [Fact]
        public void TestFunctionality() {
            SimplePlayerMockUp playerOne = new(), playerTwo = new();

            GameController controller = new GameController(new List<Card>(), new List<Card>(), new List<APlayer>() { playerOne, playerTwo }, shufflePlayers: false);

            // protection before shuffling
            Card basicUnicorn = new BasicUnicorn().GetCardTemplate().CreateCard();
            controller.Pile.Add(basicUnicorn);
            Card slowdown = new Slowdown().GetCardTemplate().CreateCard();
            controller.Pile.Add(slowdown);

            controller.PlayerDrawCard(playerOne);
            controller.PlayerDrawCard(playerTwo);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 1, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 1, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            Assert.Equal(playerOne.Hand[0], slowdown);
            Assert.Equal(playerTwo.Hand[0], basicUnicorn);

            Assert.True(slowdown.CanPlayInstantCards());
            Assert.True(basicUnicorn.CanPlayInstantCards());

            controller.PlayCardAndResolveChainLink(slowdown, playerOne);

            Assert.False(slowdown.CanPlayInstantCards());
            Assert.True(basicUnicorn.CanPlayInstantCards());

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 1);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 1, stableSize: 0, numUpgrades: 0, numDowngrades: 0);

            Assert.Empty(controller.Pile);
            Assert.Empty(controller.DiscardPile);
        }
    }
}
