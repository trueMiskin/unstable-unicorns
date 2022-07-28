using System.Collections.Generic;
using UnstableUnicornCore;
using UnstableUnicornCore.BaseSet;
using Xunit;

namespace UnstableUnicornCoreTest.BaseSet {
    public class MagicalFlyingUnicornTest {
        [Fact]
        public void TestChoosingMagicalCardFromDiscardPile() {
            SimplePlayerMockUp playerOne = new(), playerTwo = new();

            GameController controller = new GameController(new List<Card>(), new List<Card>(), new List<APlayer>() { playerOne, playerTwo });

            // protection before shuffling
            Card unicornPoison = new UnicornPoison().GetCardTemplate().CreateCard();
            // move magical unicorn directly to discard pile
            unicornPoison.MoveCard(controller, null, CardLocation.DiscardPile);

            Card magicalFlyingUnicorn = new MagicalFlyingUnicorn().GetCardTemplate().CreateCard();
            controller.Pile.Add(magicalFlyingUnicorn);
            
            controller.PlayerDrawCard(playerOne);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 1, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            Assert.Equal(playerOne.Hand[0], magicalFlyingUnicorn);
            Assert.Empty(controller.Pile);
            Assert.Single(controller.DiscardPile);

            // player one playes unicorn to his own stable
            controller.PlayCardAndResolveChainLink(magicalFlyingUnicorn, playerOne);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 1, stableSize: 1, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);

            Assert.Equal(unicornPoison, playerOne.Hand[0]);

            Assert.Empty(controller.Pile);
            Assert.Empty(controller.DiscardPile);
        }
    }
}
