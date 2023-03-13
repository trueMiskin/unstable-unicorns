using System.Collections.Generic;
using UnstableUnicornCore;
using UnstableUnicornCore.BaseSet;
using Xunit;

namespace UnstableUnicornCoreTest.BaseSet {
    public class YayTest {
        [Fact]
        public void TestContinuousAbility() {
            SimplePlayerMockUp playerOne = new(), playerTwo = new();
            GameController controller = new GameController(new List<Card>(), new List<Card>(), new List<APlayer>() { playerOne, playerTwo }, shufflePlayers: false);

            // protection before shuffling
            Card basicUnicorn = new BasicUnicorn().GetCardTemplate().CreateCard();
            controller.Pile.Add(basicUnicorn);
            Card yay = new Yay().GetCardTemplate().CreateCard();
            controller.Pile.Add(yay);

            controller.PlayerDrawCard(playerOne);
            controller.PlayerDrawCard(playerOne);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 2, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            Assert.Equal(playerOne.Hand[0], yay);
            Assert.Equal(playerOne.Hand[1], basicUnicorn);

            Assert.True(basicUnicorn.CanBeNeigh());

            controller.PlayCardAndResolveChainLink(yay, playerOne);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 1, stableSize: 0, numUpgrades: 1, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            Assert.Equal(playerOne.Upgrades[0], yay);

            Assert.False(basicUnicorn.CanBeNeigh());
        }
    }
}
