using System.Collections.Generic;
using UnstableUnicornCore;
using UnstableUnicornCore.BaseSet;
using Xunit;

namespace UnstableUnicornCoreTest.BaseSet {
    public class BackKickTest {
        [Fact]
        public void TestReturnThenDiscardCardWithoutValidTarget() {
            SimplePlayerMockUp playerOne = new(), playerTwo = new();
            GameController controller = new GameController(new List<Card>(), new List<Card>(), new List<APlayer>() { playerOne, playerTwo }, shufflePlayers: false);

            // protection before shuffling
            Card backKick = new BackKick().GetCardTemplate().CreateCard();
            controller.Pile.Add(backKick);

            controller.PlayerDrawCard(playerOne);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 1, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            Assert.Equal(playerOne.Hand[0], backKick);

            TestUtils.CardCantBePlayed(backKick, playerOne, controller);
        }

        [Fact]
        public void TestReturnThenDiscardCardWithValidTarget() {
            SimplePlayerMockUp playerOne = new(), playerTwo = new();
            GameController controller = new GameController(new List<Card>(), new List<Card>(), new List<APlayer>() { playerOne, playerTwo }, shufflePlayers: false);

            // protection before shuffling
            Card backKick = new BackKick().GetCardTemplate().CreateCard();
            controller.Pile.Add(backKick);
            Card basicUnicorn = new BasicUnicorn().GetCardTemplate().CreateCard();
            controller.Pile.Add(basicUnicorn);

            controller.PlayerDrawCard(playerOne);
            controller.PlayerDrawCard(playerTwo);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 1, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 1, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            
            Assert.Equal(playerOne.Hand[0], basicUnicorn);
            Assert.Equal(playerTwo.Hand[0], backKick);

            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerOne, playerOne, basicUnicorn);
            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerOne, playerTwo);
            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerTwo, playerOne);
            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerTwo, playerTwo, backKick);

            controller.PlayCardAndResolveChainLink(basicUnicorn, playerOne);

            Assert.True(backKick.CanBePlayed(playerTwo));
            controller.PlayCardAndResolveChainLink(backKick, playerTwo);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);

            Assert.Equal(2, controller.DiscardPile.Count);

            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerOne, playerOne);
            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerOne, playerTwo);
            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerTwo, playerOne);
            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerTwo, playerTwo);
        }

        [Fact]
        public void TestReturnThenDiscardCardWithCardsVisibility() {
            SimplePlayerMockUp playerOne = new(), playerTwo = new(), playerThree = new();
            GameController controller = new GameController(new List<Card>(), new List<Card>(), new List<APlayer>() { playerOne, playerTwo, playerThree }, shufflePlayers: false);

            // protection before shuffling
            Card backKick = new BackKick().GetCardTemplate().CreateCard();
            controller.Pile.Add(backKick);
            Card basicUnicorn = new BasicUnicorn().GetCardTemplate().CreateCard();
            controller.Pile.Add(basicUnicorn);
            Card secondBasicUnicorn = new BasicUnicorn().GetCardTemplate().CreateCard();
            controller.Pile.Add(secondBasicUnicorn);

            controller.PlayerDrawCard(playerOne);
            controller.PlayerDrawCard(playerOne);
            controller.PlayerDrawCard(playerTwo);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 2, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 1, stableSize: 0, numUpgrades: 0, numDowngrades: 0);

            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerOne, playerOne, basicUnicorn, secondBasicUnicorn);
            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerOne, playerTwo);
            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerOne, playerThree);

            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerTwo, playerOne);
            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerTwo, playerTwo, backKick);
            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerTwo, playerThree);

            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerThree, playerOne);
            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerThree, playerTwo);
            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerThree, playerThree);

            controller.PlayCardAndResolveChainLink(basicUnicorn, playerOne);
            Assert.True(backKick.CanBePlayed(playerTwo));

            // second player will discard his first card -- second basic unicorn
            // -> each player knows that the second player has a basic unicorn in hand
            controller.PlayCardAndResolveChainLink(backKick, playerTwo);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 1, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);

            Assert.Equal(2, controller.DiscardPile.Count);

            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerOne, playerOne, basicUnicorn);
            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerOne, playerTwo);
            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerOne, playerThree);

            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerTwo, playerOne, basicUnicorn);
            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerTwo, playerTwo);
            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerTwo, playerThree);

            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerThree, playerOne, basicUnicorn);
            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerThree, playerTwo);
            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerThree, playerThree);
        }
    }
}
