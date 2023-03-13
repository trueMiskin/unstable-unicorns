using System.Collections.Generic;
using UnstableUnicornCore;
using UnstableUnicornCore.BaseSet;
using Xunit;

namespace UnstableUnicornCoreTest.BaseSet {
    public class GlitterTornadoTest {
        [Fact]
        public void TestRequirementsToPlay() {
            SimplePlayerMockUp playerOne = new(), playerTwo = new();

            GameController controller = new GameController(new List<Card>(), new List<Card>(), new List<APlayer>() { playerOne, playerTwo }, shufflePlayers: false);

            // protection before shuffling
            Card glitterTornado = new GlitterTornado().GetCardTemplate().CreateCard();
            controller.Pile.Add(glitterTornado);

            controller.PlayerDrawCard(playerOne);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 1, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);

            TestUtils.CardCantBePlayed(glitterTornado, playerOne, controller);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void TestWithValidTargetInOwnersStable(bool opponentHaveCardInStable) {
            SimplePlayerMockUp playerOne = new(), playerTwo = new(), playerThree = new();

            GameController controller = new GameController(new List<Card>(), new List<Card>(), new List<APlayer>() { playerOne, playerTwo, playerThree }, shufflePlayers: false);

            // protection before shuffling
            Card unicornLasso = new UnicornLasso().GetCardTemplate().CreateCard();
            controller.Pile.Add(unicornLasso);
            
            Card glitterTornado = new GlitterTornado().GetCardTemplate().CreateCard();
            controller.Pile.Add(glitterTornado);
            Card basicUnicorn = new BasicUnicorn().GetCardTemplate().CreateCard();
            controller.Pile.Add(basicUnicorn);

            controller.PlayerDrawCard(playerOne);
            controller.PlayerDrawCard(playerOne);
            controller.PlayerDrawCard(playerThree);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 2, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerThree, handSize: 1, stableSize: 0, numUpgrades: 0, numDowngrades: 0);

            controller.PlayCardAndResolveChainLink(basicUnicorn, playerOne);

            if (!opponentHaveCardInStable) {
                TestUtils.CheckPlayerPileSizes(playerOne, handSize: 1, stableSize: 1, numUpgrades: 0, numDowngrades: 0);
                TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
                TestUtils.CheckPlayerPileSizes(playerThree, handSize: 1, stableSize: 0, numUpgrades: 0, numDowngrades: 0);

                controller.PlayCardAndResolveChainLink(glitterTornado, playerOne);

                TestUtils.CheckPlayerPileSizes(playerOne, handSize: 1, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
                TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
                TestUtils.CheckPlayerPileSizes(playerThree, handSize: 1, stableSize: 0, numUpgrades: 0, numDowngrades: 0);

                TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerOne, playerThree);
                TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerTwo, playerThree);
            } else {
                controller.PlayCardAndResolveChainLink(unicornLasso, playerThree);

                TestUtils.CheckPlayerPileSizes(playerOne, handSize: 1, stableSize: 1, numUpgrades: 0, numDowngrades: 0);
                TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
                TestUtils.CheckPlayerPileSizes(playerThree, handSize: 0, stableSize: 0, numUpgrades: 1, numDowngrades: 0);

                controller.PlayCardAndResolveChainLink(glitterTornado, playerOne);

                TestUtils.CheckPlayerPileSizes(playerOne, handSize: 1, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
                TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
                TestUtils.CheckPlayerPileSizes(playerThree, handSize: 1, stableSize: 0, numUpgrades: 0, numDowngrades: 0);

                TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerOne, playerThree, unicornLasso);
                TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerTwo, playerThree, unicornLasso);
            }

            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerOne, playerOne, basicUnicorn);
            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerOne, playerTwo);

            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerTwo, playerOne, basicUnicorn);
            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerTwo, playerTwo);

            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerThree, playerOne, basicUnicorn);
            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerThree, playerTwo);
            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerThree, playerThree, unicornLasso);
        }
    }
}
