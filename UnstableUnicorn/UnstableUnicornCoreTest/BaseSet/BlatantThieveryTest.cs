using System.Collections.Generic;
using UnstableUnicornCore;
using UnstableUnicornCore.BaseSet;
using Xunit;

namespace UnstableUnicornCoreTest.BaseSet {
    public class BlatantThieveryTest {
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void TestStealingCardFromHand(bool selectMyself) {
            SimplePlayerMockUp playerOne = new(), playerTwo = new(), playerThree = new();
            playerOne.ChooseMyself = selectMyself;

            GameController controller = new GameController(new List<Card>(), new List<Card>(), new List<APlayer>() { playerOne, playerTwo, playerThree }, shufflePlayers: false);

            // protection before shuffling
            Card secondBasicUnicorn = new BasicUnicorn().GetCardTemplate().CreateCard();
            controller.Pile.Add(secondBasicUnicorn);
            Card basicUnicorn = new BasicUnicorn().GetCardTemplate().CreateCard();
            controller.Pile.Add(basicUnicorn);
            Card blatantThievery = new BlatantThievery().GetCardTemplate().CreateCard();
            controller.Pile.Add(blatantThievery);

            controller.PlayerDrawCard(playerOne);
            controller.PlayerDrawCard(playerTwo);
            controller.PlayerDrawCard(playerTwo);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 1, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 2, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            Assert.Equal(playerOne.Hand[0], blatantThievery);
            Assert.Equal(playerTwo.Hand[0], basicUnicorn);
            Assert.Equal(playerTwo.Hand[1], secondBasicUnicorn);

            controller.PlayCardAndResolveChainLink(blatantThievery, playerOne);

            if (selectMyself) {
                TestUtils.CheckPlayerPileSizes(playerOne, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
                TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 2, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            } else {
                TestUtils.CheckPlayerPileSizes(playerOne, handSize: 1, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
                TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 1, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
                Assert.Equal(playerTwo.Hand[0], secondBasicUnicorn);

                TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerTwo, playerOne);

                TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerOne, playerTwo, secondBasicUnicorn);

                TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerThree, playerOne);
                TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerThree, playerTwo);
            }
        }
    }
}
