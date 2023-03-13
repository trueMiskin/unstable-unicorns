using System.Collections.Generic;
using UnstableUnicornCore;
using UnstableUnicornCore.BaseSet;
using Xunit;

namespace UnstableUnicornCoreTest.BaseSet {
    public class QueenBeeUnicornTest {
        [Fact]
        public void TestOwnerPlayerCanPlayBasicUnicorn() {
            SimplePlayerMockUp playerOne = new();
            BasicUnicorn basicUnicornTemplate = new();
            GameController controller = new GameController(new List<Card>(), new List<Card>(), new List<APlayer>() { playerOne });

            // protection before shuffling
            Card basicUnicorn = basicUnicornTemplate.GetCardTemplate().CreateCard();
            controller.Pile.Add(basicUnicorn);
            Card queenBeeUnicorn = new QueenBeeUnicorn().GetCardTemplate().CreateCard();
            controller.Pile.Add(queenBeeUnicorn);

            controller.PlayerDrawCard(playerOne);
            controller.PlayerDrawCard(playerOne);

            Assert.Equal(playerOne.Hand[0], queenBeeUnicorn);
            Assert.Equal(playerOne.Hand[1], basicUnicorn);

            controller.PlayCardAndResolveChainLink(queenBeeUnicorn, playerOne);
            controller.PlayCardAndResolveChainLink(basicUnicorn, playerOne);

            Assert.Empty(playerOne.Hand);
            Assert.Equal(2, playerOne.Stable.Count);
            Assert.Empty(playerOne.Upgrades);
            Assert.Empty(playerOne.Downgrades);

            Assert.Empty(controller.DiscardPile);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void TestOtherPlayersCannotPlayBasicUnicorn(bool toHisOwnStable) {
            SimplePlayerMockUp playerOne = new(), playerTwo = new();
            BasicUnicorn basicUnicornTemplate = new();
            GameController controller = new GameController(new List<Card>(), new List<Card>(), new List<APlayer>() { playerOne, playerTwo }, shufflePlayers: false);

            // protection before shuffling
            Card basicUnicorn = basicUnicornTemplate.GetCardTemplate().CreateCard();
            controller.Pile.Add(basicUnicorn);
            Card queenBeeUnicorn = new QueenBeeUnicorn().GetCardTemplate().CreateCard();
            controller.Pile.Add(queenBeeUnicorn);

            controller.PlayerDrawCard(playerOne);
            controller.PlayerDrawCard(playerTwo);

            Assert.Equal(playerOne.Hand[0], queenBeeUnicorn);
            Assert.Equal(playerTwo.Hand[0], basicUnicorn);

            controller.PlayCardAndResolveChainLink(queenBeeUnicorn, playerOne);

            if (toHisOwnStable) {
                TestUtils.CardCantBePlayed(basicUnicorn, playerTwo, controller);
            } else {
                Assert.True(basicUnicorn.CanBePlayed(playerOne));
                // player two want player basic unicorn to player one stable
                // -> player can play basic unicorn to stable who owns Queen Bee Unicorn
                controller.PlayCardAndResolveChainLink(basicUnicorn, playerOne);

                TestUtils.CheckPlayerPileSizes(playerOne, handSize: 0, stableSize: 2, numUpgrades: 0, numDowngrades: 0);
                TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            }
        }
    }
}
