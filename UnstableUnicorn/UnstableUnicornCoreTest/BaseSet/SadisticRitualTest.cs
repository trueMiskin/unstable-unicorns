using System.Collections.Generic;
using UnstableUnicornCore;
using UnstableUnicornCore.BaseSet;
using Xunit;

namespace UnstableUnicornCoreTest.BaseSet {
    public class SadisticRitualTest {
        [Fact]
        public void TestNoUnicornCardToSacrifice() {
            SimplePlayerMockUp playerOne = new(), playerTwo = new();

            GameController controller = new GameController(new List<Card>(), new List<Card>(), new List<APlayer>() { playerOne, playerTwo });

            // protection before shuffling
            Card basicUnicorn = new BasicUnicorn().GetCardTemplate().CreateCard();
            controller.Pile.Add(basicUnicorn);
            Card sadisticRitual = new SadisticRitual().GetCardTemplate().CreateCard();
            controller.Pile.Add(sadisticRitual);

            controller.PlayerDrawCard(playerOne);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 1, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            Assert.Equal(playerOne.Hand[0], sadisticRitual);
            Assert.Single(controller.Pile);
            Assert.Empty(controller.DiscardPile);

            controller.PlayCardAndResolveChainLink(sadisticRitual, playerOne);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 1);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);

            controller.PublishEvent(ETriggerSource.BeginningTurn);
            Assert.Single(controller.NextChainLink);
            controller.ResolveChainLink();

            // no valid target for sacrifice -> nothing is happend, no cards are drawn
            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 1);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);

            Assert.Single(controller.Pile);
            Assert.Empty(controller.DiscardPile);
        }

        [Fact]
        public void TestValidTargetForSacrifice() {
            SimplePlayerMockUp playerOne = new(), playerTwo = new();

            GameController controller = new GameController(new List<Card>(), new List<Card>(), new List<APlayer>() { playerOne, playerTwo });

            // protection before shuffling
            Card secondBasicUnicorn = new BasicUnicorn().GetCardTemplate().CreateCard();
            controller.Pile.Add(secondBasicUnicorn);
            Card basicUnicorn = new BasicUnicorn().GetCardTemplate().CreateCard();
            controller.Pile.Add(basicUnicorn);
            Card sadisticRitual = new SadisticRitual().GetCardTemplate().CreateCard();
            controller.Pile.Add(sadisticRitual);

            controller.PlayerDrawCard(playerOne);
            controller.PlayerDrawCard(playerOne);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 2, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            Assert.Equal(playerOne.Hand[0], sadisticRitual);
            Assert.Single(controller.Pile);
            Assert.Empty(controller.DiscardPile);

            controller.PlayCardAndResolveChainLink(sadisticRitual, playerOne);
            controller.PlayCardAndResolveChainLink(basicUnicorn, playerOne);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 0, stableSize: 1, numUpgrades: 0, numDowngrades: 1);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);

            controller.PublishEvent(ETriggerSource.BeginningTurn);
            Assert.Single(controller.NextChainLink);
            controller.ResolveChainLink();

            // sadistic ritual is performed
            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 1, stableSize: 0, numUpgrades: 0, numDowngrades: 1);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);

            Assert.Empty(controller.Pile);
            Assert.Single(controller.DiscardPile);
        }
    }
}
