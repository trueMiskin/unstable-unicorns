using System.Collections.Generic;
using UnstableUnicornCore;
using UnstableUnicornCore.BaseSet;
using Xunit;

namespace UnstableUnicornCoreTest.BaseSet {
    public class RhinocornTest {
        [Fact]
        public void TestAbilities() {
            SimplePlayerMockUp playerOne = new(), playerTwo = new();

            GameController controller = new GameController(new List<Card>(), new List<Card>(), new List<APlayer>() { playerOne, playerTwo });

            // protection before shuffling
            Card rhinocorn = new Rhinocorn().GetCardTemplate().CreateCard();
            controller.Pile.Add(rhinocorn);

            controller.PlayerDrawCard(playerOne);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 1, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            Assert.Equal(playerOne.Hand[0], rhinocorn);
            Assert.Empty(controller.Pile);
            Assert.Empty(controller.DiscardPile);

            controller.PlayCardAndResolveChainLink(rhinocorn, playerOne);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 0, stableSize: 1, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);

            controller.PublishEvent(ETriggerSource.BeginningTurn);
            Assert.Single(controller.NextChainLink);
            controller.ResolveChainLink();
            Assert.True(controller.SkipToEndTurnPhase);

            // rhinocorn executed himself -> only one valid target
            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
        }
    }
}
