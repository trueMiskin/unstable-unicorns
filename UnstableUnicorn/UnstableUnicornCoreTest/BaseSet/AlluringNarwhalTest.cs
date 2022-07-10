using System.Collections.Generic;
using UnstableUnicornCore;
using UnstableUnicornCore.BaseSet;
using Xunit;

namespace UnstableUnicornCoreTest.BaseSet {
    public class AlluringNarwhalTest {
        [Fact]
        public void testWithoutValidTarget() {
            SimplePlayerMockUp playerOne = new(), playerTwo = new();
            GameController controller = new GameController(new List<Card>(), new List<Card>(), new List<APlayer>() { playerOne, playerTwo });

            // protection before shuffling
            Card basicUnicorn = new BasicUnicorn().GetCardTemplate().CreateCard();
            controller.Pile.Add(basicUnicorn);
            Card alluringNarwhal = new AlluringNarwhal().GetCardTemplate().CreateCard();
            controller.Pile.Add(alluringNarwhal);

            controller.PlayerDrawCard(playerTwo);
            controller.PlayerDrawCard(playerOne);

            Assert.Equal(playerOne.Hand[0], basicUnicorn);
            Assert.Equal(playerTwo.Hand[0], alluringNarwhal);

            controller.PlayCardAndResolveChainLink(basicUnicorn, playerOne);
            controller.PlayCardAndResolveChainLink(alluringNarwhal, playerTwo);

            Assert.Empty(playerOne.Hand);
            Assert.Single(playerOne.Stable);
            Assert.Empty(playerOne.Upgrades);
            Assert.Empty(playerOne.Downgrades);

            Assert.Empty(playerTwo.Hand);
            Assert.Single(playerTwo.Stable);
            Assert.Empty(playerTwo.Upgrades);
            Assert.Empty(playerTwo.Downgrades);
        }

        [Fact]
        public void testWithValidTarget() {
            SimplePlayerMockUp playerOne = new(), playerTwo = new();
            GameController controller = new GameController(new List<Card>(), new List<Card>(), new List<APlayer>() { playerOne, playerTwo });

            // protection before shuffling
            Card rainbowAura = new RainbowAura().GetCardTemplate().CreateCard();
            controller.Pile.Add(rainbowAura);
            Card alluringNarwhal = new AlluringNarwhal().GetCardTemplate().CreateCard();
            controller.Pile.Add(alluringNarwhal);

            controller.PlayerDrawCard(playerTwo);
            controller.PlayerDrawCard(playerOne);

            Assert.Equal(playerOne.Hand[0], rainbowAura);
            Assert.Equal(playerTwo.Hand[0], alluringNarwhal);

            controller.PlayCardAndResolveChainLink(rainbowAura, playerOne);
            controller.PlayCardAndResolveChainLink(alluringNarwhal, playerTwo);

            Assert.Empty(playerOne.Hand);
            Assert.Empty(playerOne.Stable);
            Assert.Empty(playerOne.Upgrades);
            Assert.Empty(playerOne.Downgrades);

            Assert.Empty(playerTwo.Hand);
            Assert.Single(playerTwo.Stable);
            Assert.Single(playerTwo.Upgrades);
            Assert.Empty(playerTwo.Downgrades);
        }
    }
}
