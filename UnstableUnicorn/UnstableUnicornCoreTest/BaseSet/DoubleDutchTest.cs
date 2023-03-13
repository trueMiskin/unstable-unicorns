using System.Collections.Generic;
using UnstableUnicornCore;
using UnstableUnicornCore.BaseSet;
using Xunit;

namespace UnstableUnicornCoreTest.BaseSet {
    public class DoubleDutchTest {
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void TestMaxCardsWhichCanBePlayed(bool upgradeOwnerOnTurn) {
            SimplePlayerMockUp playerOne = new(), playerTwo = new();

            GameController controller = new GameController(new List<Card>(), new List<Card>(), new List<APlayer>() { playerOne, playerTwo }, shufflePlayers: false);

            // protection before shuffling
            Card doubleDutch = new DoubleDutch().GetCardTemplate().CreateCard();
            controller.Pile.Add(doubleDutch);

            controller.PlayerDrawCard(playerOne);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 1, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            Assert.Equal(playerOne.Hand[0], doubleDutch);

            controller.PlayCardAndResolveChainLink(doubleDutch, playerOne);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 0, stableSize: 0, numUpgrades: 1, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);

            if (upgradeOwnerOnTurn) {
                controller.ActualPlayerOnTurn = playerOne;
                controller.PublishEvent(ETriggerSource.BeginningTurn);
                controller.ResolveChainLink();
                Assert.Equal(2, controller.MaxCardsToPlayInOneTurn);
            } else {
                controller.ActualPlayerOnTurn = playerTwo;
                controller.PublishEvent(ETriggerSource.BeginningTurn);
                controller.ResolveChainLink();
                Assert.Equal(1, controller.MaxCardsToPlayInOneTurn);
            }
        }
    }
}
