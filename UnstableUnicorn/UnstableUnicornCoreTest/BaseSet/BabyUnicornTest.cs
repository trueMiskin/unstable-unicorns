using System.Collections.Generic;
using UnstableUnicornCore;
using UnstableUnicornCore.BaseSet;
using Xunit;

namespace UnstableUnicornCoreTest.BaseSet {
    public class BabyUnicornTest {
        [Fact]
        public void TestPlayerGotBabyUnicorn() {
            Card babyUnicorn = new BabyUnicorn().GetCardTemplate().CreateCard();

            SimplePlayerMockUp playerOne = new(), playerTwo = new();
            GameController controller = new GameController(new List<Card>(), new List<Card> { babyUnicorn }, new List<APlayer>() { playerOne, playerTwo });

            // protection before shuffling
            controller.PlayerGetBabyUnicornOnTable(playerTwo);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 1, numUpgrades: 0, numDowngrades: 0);

            Assert.Empty(controller.Pile);
            Assert.Empty(controller.DiscardPile);
        }

        [Fact]
        public void TestInteractionWithUnicornPoison() {
            Card babyUnicorn = new BabyUnicorn().GetCardTemplate().CreateCard();

            SimplePlayerMockUp playerOne = new(), playerTwo = new();
            GameController controller = new GameController(new List<Card>(), new List<Card> { babyUnicorn }, new List<APlayer>() { playerOne, playerTwo });

            // protection before shuffling
            Card unicornPoison = new UnicornPoison().GetCardTemplate().CreateCard();
            controller.Pile.Add(unicornPoison);

            controller.PlayerDrawCard(playerOne);
            controller.PlayerGetBabyUnicornOnTable(playerTwo);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 1, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 1, numUpgrades: 0, numDowngrades: 0);
            Assert.Equal(playerOne.Hand[0], unicornPoison);

            controller.PlayCardAndResolveChainLink(unicornPoison, playerOne);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);

            Assert.Single(controller.Nursery);
            Assert.Equal(babyUnicorn, controller.Nursery[0]);

            Assert.Single(controller.DiscardPile);
            Assert.Equal(unicornPoison, controller.DiscardPile[0]);
        }

        [Fact]
        public void TestInteractionWithBlindingLightAndUnicornPoison() {
            Card babyUnicorn = new BabyUnicorn().GetCardTemplate().CreateCard();

            SimplePlayerMockUp playerOne = new(), playerTwo = new();
            GameController controller = new GameController(new List<Card>(), new List<Card> { babyUnicorn }, new List<APlayer>() { playerOne, playerTwo });

            // protection before shuffling
            Card unicornPoison = new UnicornPoison().GetCardTemplate().CreateCard();
            controller.Pile.Add(unicornPoison);
            Card blindingLight = new BlindingLight().GetCardTemplate().CreateCard();
            controller.Pile.Add(blindingLight);

            controller.PlayerDrawCard(playerOne);
            controller.PlayerDrawCard(playerOne);
            controller.PlayerGetBabyUnicornOnTable(playerTwo);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 2, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 1, numUpgrades: 0, numDowngrades: 0);

            // blinding light is played to player's two stable
            controller.PlayCardAndResolveChainLink(blindingLight, playerTwo);
            controller.PlayCardAndResolveChainLink(unicornPoison, playerOne);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 1);

            Assert.Single(controller.Nursery);
            Assert.Equal(babyUnicorn, controller.Nursery[0]);

            Assert.Single(controller.DiscardPile);
            Assert.Equal(unicornPoison, controller.DiscardPile[0]);
        }

        [Fact]
        public void TestInteractionWithReturnEffect() {
            Card babyUnicorn = new BabyUnicorn().GetCardTemplate().CreateCard();

            SimplePlayerMockUp playerOne = new(), playerTwo = new();
            GameController controller = new GameController(new List<Card>(), new List<Card> { babyUnicorn }, new List<APlayer>() { playerOne, playerTwo });

            // protection before shuffling
            Card backKick = new BackKick().GetCardTemplate().CreateCard();
            controller.Pile.Add(backKick);

            controller.PlayerDrawCard(playerOne);
            controller.PlayerGetBabyUnicornOnTable(playerTwo);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 1, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 1, numUpgrades: 0, numDowngrades: 0);

            controller.PlayCardAndResolveChainLink(backKick, playerOne);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);

            Assert.Single(controller.Nursery);
            Assert.Equal(babyUnicorn, controller.Nursery[0]);

            Assert.Single(controller.DiscardPile);
            Assert.Equal(backKick, controller.DiscardPile[0]);
        }

        [Fact]
        public void TestInteractionWithSacrificeEffect() {
            Card babyUnicorn = new BabyUnicorn().GetCardTemplate().CreateCard();

            SimplePlayerMockUp playerOne = new(), playerTwo = new();
            GameController controller = new GameController(new List<Card>(), new List<Card> { babyUnicorn }, new List<APlayer>() { playerOne, playerTwo });

            // protection before shuffling
            Card twoForOne = new TwoForOne().GetCardTemplate().CreateCard();
            controller.Pile.Add(twoForOne);

            controller.PlayerDrawCard(playerOne);
            controller.PlayerGetBabyUnicornOnTable(playerOne);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 1, stableSize: 1, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);

            controller.PlayCardAndResolveChainLink(twoForOne, playerOne);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);

            Assert.Single(controller.Nursery);
            Assert.Equal(babyUnicorn, controller.Nursery[0]);

            Assert.Single(controller.DiscardPile);
            Assert.Equal(twoForOne, controller.DiscardPile[0]);
        }
    }
}
