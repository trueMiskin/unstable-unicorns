using System.Collections.Generic;
using UnstableUnicornCore;
using UnstableUnicornCore.BaseSet;
using Xunit;

namespace UnstableUnicornCoreTest.BaseSet {
    public class UnicornShrinkrayTest {
        [Fact]
        public void TestPlayedOnEmptyBoad() {
            Card babyUnicorn = new BabyUnicorn().GetCardTemplate().CreateCard();
            Card secondBabyUnicorn = new BabyUnicorn().GetCardTemplate().CreateCard();

            SimplePlayerMockUp playerOne = new(), playerTwo = new();
            GameController controller = new GameController(new List<Card>(),
                new List<Card> { babyUnicorn, secondBabyUnicorn }, new List<APlayer>() { playerOne, playerTwo });

            // protection before shuffling
            Card unicronShrinkray = new UnicornShrinkray().GetCardTemplate().CreateCard();
            controller.Pile.Add(unicronShrinkray);

            controller.PlayerDrawCard(playerTwo);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 1, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            Assert.Equal(playerTwo.Hand[0], unicronShrinkray);

            controller.PlayCardAndResolveChainLink(unicronShrinkray, playerTwo);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);

            Assert.Equal(2, controller.Nursery.Count);
            Assert.Empty(controller.Pile);
            Assert.Single(controller.DiscardPile);
        }

        [Fact]
        public void TestWithBabyUnicorn() {
            Card babyUnicorn = new BabyUnicorn().GetCardTemplate().CreateCard();
            Card secondBabyUnicorn = new BabyUnicorn().GetCardTemplate().CreateCard();

            SimplePlayerMockUp playerOne = new(), playerTwo = new();
            GameController controller = new GameController(new List<Card>(),
                new List<Card> { babyUnicorn, secondBabyUnicorn }, new List<APlayer>() { playerOne, playerTwo });

            // protection before shuffling
            Card unicronShrinkray = new UnicornShrinkray().GetCardTemplate().CreateCard();
            controller.Pile.Add(unicronShrinkray);

            controller.PlayerDrawCard(playerTwo);
            controller.PlayerGetBabyUnicornOnTable(playerOne);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 0, stableSize: 1, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 1, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            Assert.Equal(playerTwo.Hand[0], unicronShrinkray);

            controller.PlayCardAndResolveChainLink(unicronShrinkray, playerTwo);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 0, stableSize: 1, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);

            Assert.Single(controller.Nursery);
            Assert.Empty(controller.Pile);
            Assert.Single(controller.DiscardPile);
        }

        [Fact]
        public void TestWithBarbedWireAndBabyUnicorn() {
            Card babyUnicorn = new BabyUnicorn().GetCardTemplate().CreateCard();

            SimplePlayerMockUp playerOne = new(), playerTwo = new();
            GameController controller = new GameController(new List<Card>(),
                new List<Card> { babyUnicorn }, new List<APlayer>() { playerOne, playerTwo });

            // protection before shuffling
            Card basicUnicorn = new BasicUnicorn().GetCardTemplate().CreateCard();
            controller.Pile.Add(basicUnicorn);
            Card secondBasicUnicorn = new BasicUnicorn().GetCardTemplate().CreateCard();
            controller.Pile.Add(secondBasicUnicorn);

            Card barbedWire = new BarbedWire().GetCardTemplate().CreateCard();
            controller.Pile.Add(barbedWire);
            Card unicronShrinkray = new UnicornShrinkray().GetCardTemplate().CreateCard();
            controller.Pile.Add(unicronShrinkray);

            controller.PlayerDrawCard(playerTwo);
            controller.PlayerDrawCard(playerTwo);

            controller.PlayerDrawCard(playerOne);
            controller.PlayerDrawCard(playerOne);
            controller.PlayerGetBabyUnicornOnTable(playerOne);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 2, stableSize: 1, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 2, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            Assert.Equal(playerTwo.Hand[0], unicronShrinkray);

            controller.PlayCardAndResolveChainLink(barbedWire, playerOne);
            controller.PlayCardAndResolveChainLink(unicronShrinkray, playerTwo);

            // baby unicorn leave stable -> discard card
            // baby unicorn enter stable -> discard card
            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 0, stableSize: 1, numUpgrades: 0, numDowngrades: 1);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);

            Assert.Empty(controller.Nursery);
            Assert.Empty(controller.Pile);
            Assert.Equal(3, controller.DiscardPile.Count);
        }

        [Fact]
        public void TestWithPuppicorn() {
            Card babyUnicorn = new BabyUnicorn().GetCardTemplate().CreateCard();
            Card secondBabyUnicorn = new BabyUnicorn().GetCardTemplate().CreateCard();

            SimplePlayerMockUp playerOne = new(), playerTwo = new();
            GameController controller = new GameController(new List<Card>(),
                new List<Card> { babyUnicorn, secondBabyUnicorn }, new List<APlayer>() { playerOne, playerTwo });

            // protection before shuffling
            Card pupiccorn = new Puppicorn().GetCardTemplate().CreateCard();
            controller.Pile.Add(pupiccorn);
            Card unicronShrinkray = new UnicornShrinkray().GetCardTemplate().CreateCard();
            controller.Pile.Add(unicronShrinkray);

            controller.PlayerDrawCard(playerTwo);
            controller.PlayerDrawCard(playerOne);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 1, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 1, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            Assert.Equal(playerTwo.Hand[0], unicronShrinkray);

            controller.PlayCardAndResolveChainLink(pupiccorn, playerOne);
            controller.PlayCardAndResolveChainLink(unicronShrinkray, playerTwo);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 0, stableSize: 1, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);

            Assert.Equal(playerOne.Stable[0], secondBabyUnicorn);

            Assert.Single(controller.Nursery);
            Assert.Empty(controller.Pile);
            Assert.Equal(2, controller.DiscardPile.Count);
        }

        [Fact]
        public void TestNotTriggeringUnicornsEffects() {
            Card babyUnicorn = new BabyUnicorn().GetCardTemplate().CreateCard();
            Card secondBabyUnicorn = new BabyUnicorn().GetCardTemplate().CreateCard();

            SimplePlayerMockUp playerOne = new(), playerTwo = new();
            GameController controller = new GameController(new List<Card>(),
                new List<Card> { babyUnicorn, secondBabyUnicorn }, new List<APlayer>() { playerOne, playerTwo });

            // protection before shuffling
            Card basicUnicorn = new BasicUnicorn().GetCardTemplate().CreateCard();
            controller.Pile.Add(basicUnicorn);
            Card greedyFlyingUnicorn = new GreedyFlyingUnicorn().GetCardTemplate().CreateCard();
            controller.Pile.Add(greedyFlyingUnicorn);
            Card unicronShrinkray = new UnicornShrinkray().GetCardTemplate().CreateCard();
            controller.Pile.Add(unicronShrinkray);

            controller.PlayerDrawCard(playerTwo);
            controller.PlayerDrawCard(playerOne);

            controller.PlayCardAndResolveChainLink(greedyFlyingUnicorn, playerOne);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 1, stableSize: 1, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 1, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            Assert.Equal(playerTwo.Hand[0], unicronShrinkray);

            controller.PlayCardAndResolveChainLink(unicronShrinkray, playerTwo);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 1, stableSize: 1, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);

            Assert.Equal(playerOne.Stable[0], secondBabyUnicorn);

            Assert.Single(controller.Nursery);
            Assert.Empty(controller.Pile);
            Assert.Equal(2, controller.DiscardPile.Count);
        }
    }
}
