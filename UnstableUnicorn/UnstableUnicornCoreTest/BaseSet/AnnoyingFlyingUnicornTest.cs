using System.Collections.Generic;
using UnstableUnicornCore;
using UnstableUnicornCore.BaseSet;
using Xunit;

namespace UnstableUnicornCoreTest.BaseSet {
    public class AnnoyingFlyingUnicornTest {
        [Fact]
        public void TestActivateableTestNoValidTarget() {
            SimplePlayerMockUp playerOne = new(), playerTwo = new();
            GameController controller = new GameController(new List<Card>(), new List<Card>(), new List<APlayer>() { playerOne, playerTwo }, shufflePlayers: false);

            // protection before shuffling
            Card annoyingUnicorn = new AnnoyingFlyingUnicorn().GetCardTemplate().CreateCard();
            controller.Pile.Add(annoyingUnicorn);

            controller.PlayerDrawCard(playerOne);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 1, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            Assert.Equal(playerOne.Hand[0], annoyingUnicorn);

            controller.PlayCardAndResolveChainLink(annoyingUnicorn, playerOne);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 0, stableSize: 1, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);

            Assert.Equal(annoyingUnicorn, playerOne.Stable[0]);
            
            Assert.Empty(controller.DiscardPile);
        }

        [Fact]
        public void TestActivateableTestWithValidTarget() {
            SimplePlayerMockUp playerOne = new(), playerTwo = new();
            GameController controller = new GameController(new List<Card>(), new List<Card>(), new List<APlayer>() { playerOne, playerTwo }, shufflePlayers: false);

            // protection before shuffling
            Card basicUnicorn = new BasicUnicorn().GetCardTemplate().CreateCard();
            controller.Pile.Add(basicUnicorn);
            Card annoyingUnicorn = new AnnoyingFlyingUnicorn().GetCardTemplate().CreateCard();
            controller.Pile.Add(annoyingUnicorn);

            controller.PlayerDrawCard(playerOne);
            controller.PlayerDrawCard(playerTwo);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 1, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 1, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            Assert.Equal(playerOne.Hand[0], annoyingUnicorn);
            Assert.Equal(playerTwo.Hand[0], basicUnicorn);

            controller.PlayCardAndResolveChainLink(annoyingUnicorn, playerOne);

            // After resolving annoying unicorn, basic unicorn in other hand should be discarded

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 0, stableSize: 1, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);

            Assert.Equal(annoyingUnicorn, playerOne.Stable[0]);

            Assert.Single(controller.DiscardPile);
        }

        [Fact]
        public void TestReturningCardAfterDestroying() {
            SimplePlayerMockUp playerOne = new(), playerTwo = new();
            GameController controller = new GameController(new List<Card>(), new List<Card>(), new List<APlayer>() { playerOne, playerTwo }, shufflePlayers: false);

            // protection before shuffling
            Card unicornPoison = new UnicornPoison().GetCardTemplate().CreateCard();
            controller.Pile.Add(unicornPoison);
            Card annoyingUnicorn = new AnnoyingFlyingUnicorn().GetCardTemplate().CreateCard();
            controller.Pile.Add(annoyingUnicorn);

            controller.PlayerDrawCard(playerOne);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 1, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            Assert.Equal(playerOne.Hand[0], annoyingUnicorn);

            controller.PlayCardAndResolveChainLink(annoyingUnicorn, playerOne);
            // draw spell after playing unicorn else this spell will be discarded
            controller.PlayerDrawCard(playerTwo);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 0, stableSize: 1, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 1, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            Assert.Equal(playerTwo.Hand[0], unicornPoison);

            controller.PlayCardAndResolveChainLink(unicornPoison, playerTwo);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 1, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);

            Assert.Equal(annoyingUnicorn, playerOne.Hand[0]);

            Assert.Single(controller.DiscardPile);
        }

        [Fact]
        public void TestReturningCardAfterDestroyingWithCardVisibility() {
            SimplePlayerMockUp playerOne = new(), playerTwo = new(), playerThree = new();
            GameController controller = new GameController(new List<Card>(), new List<Card>(), new List<APlayer>() { playerOne, playerTwo, playerThree }, shufflePlayers: false);

            // protection before shuffling
            Card unicornPoison = new UnicornPoison().GetCardTemplate().CreateCard();
            controller.Pile.Add(unicornPoison);
            Card annoyingUnicorn = new AnnoyingFlyingUnicorn().GetCardTemplate().CreateCard();
            controller.Pile.Add(annoyingUnicorn);

            controller.PlayerDrawCard(playerOne);

            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerOne, playerOne, annoyingUnicorn);
            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerOne, playerTwo);
            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerOne, playerThree);

            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerTwo, playerOne);
            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerTwo, playerTwo);
            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerTwo, playerThree);

            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerThree, playerOne);
            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerThree, playerTwo);
            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerThree, playerThree);

            controller.PlayCardAndResolveChainLink(annoyingUnicorn, playerOne);
            // draw spell after playing unicorn else this spell will be discarded
            controller.PlayerDrawCard(playerTwo);

            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerOne, playerOne);
            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerOne, playerTwo);
            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerOne, playerThree);

            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerTwo, playerOne);
            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerTwo, playerTwo, unicornPoison);
            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerTwo, playerThree);

            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerThree, playerOne);
            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerThree, playerTwo);
            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerThree, playerThree);

            controller.PlayCardAndResolveChainLink(unicornPoison, playerTwo);

            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerOne, playerOne, annoyingUnicorn);
            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerOne, playerTwo);
            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerOne, playerThree);

            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerTwo, playerOne, annoyingUnicorn);
            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerTwo, playerTwo);
            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerTwo, playerThree);

            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerThree, playerOne, annoyingUnicorn);
            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerThree, playerTwo);
            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerThree, playerThree);
        }
    }
}
