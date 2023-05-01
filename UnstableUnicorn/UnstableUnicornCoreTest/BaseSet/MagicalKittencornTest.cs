using System.Collections.Generic;
using UnstableUnicornCore.BaseSet;
using UnstableUnicornCore;
using Xunit;

namespace UnstableUnicornCoreTest.BaseSet {
    public class MagicalKittencornTest {
        [Fact]
        public void TestMagicalKittencornCannotByDestroyedBySpell() {
            SimplePlayerMockUp playerOne = new(), playerTwo = new();
            GameController controller = new GameController(new List<Card>(), new List<Card>(), new List<APlayer>() { playerOne, playerTwo }, shufflePlayers: false);

            // protection before shuffling
            Card unicornPoison = new UnicornPoison().GetCardTemplate().CreateCard();
            controller.Pile.Add(unicornPoison);
            Card magicalKittencorn = new MagicalKittencorn().GetCardTemplate().CreateCard();
            controller.Pile.Add(magicalKittencorn);

            controller.PlayerDrawCard(playerOne);
            controller.PlayerDrawCard(playerTwo);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 1, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 1, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            Assert.Equal(playerOne.Hand[0], magicalKittencorn);
            Assert.Equal(playerTwo.Hand[0], unicornPoison);

            controller.PlayCardAndResolveChainLink(magicalKittencorn, playerOne);

            controller.PlayCardAndResolveChainLink(unicornPoison, playerTwo);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 0, stableSize: 1, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);

            Assert.Equal(magicalKittencorn, playerOne.Stable[0]);

            Assert.Single(controller.DiscardPile);
        }

        [Fact]
        public void TestDestroyedByUnicornEffect() {
            SimplePlayerMockUp playerOne = new(), playerTwo = new();
            GameController controller = new GameController(new List<Card>(), new List<Card>(), new List<APlayer>() { playerOne, playerTwo }, shufflePlayers: false);

            // protection before shuffling
            Card magicalKittencorn = new MagicalKittencorn().GetCardTemplate().CreateCard();
            controller.Pile.Add(magicalKittencorn);
            Card rhinocorn = new Rhinocorn().GetCardTemplate().CreateCard();
            controller.Pile.Add(rhinocorn);

            controller.PlayerDrawCard(playerOne);
            controller.PlayerDrawCard(playerTwo);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 1, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 1, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            Assert.Equal(playerOne.Hand[0], rhinocorn);
            Assert.Equal(playerTwo.Hand[0], magicalKittencorn);

            controller.PlayCardAndResolveChainLink(rhinocorn, playerOne);
            controller.PlayCardAndResolveChainLink(magicalKittencorn, playerTwo);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 0, stableSize: 1, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 1, numUpgrades: 0, numDowngrades: 0);

            controller.PublishEvent(ETriggerSource.BeginningTurn);
            controller.ResolveChainLink();

            // Rhinocorn destroyed the Magical Kittencorn card
            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 0, stableSize: 1, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);

            Assert.Equal(rhinocorn, playerOne.Stable[0]);

            Assert.Single(controller.DiscardPile);
        }
    }
}
