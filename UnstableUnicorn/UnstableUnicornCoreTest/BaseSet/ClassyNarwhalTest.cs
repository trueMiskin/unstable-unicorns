using System;
using System.Collections.Generic;
using UnstableUnicornCore;
using UnstableUnicornCore.BaseSet;
using Xunit;

namespace UnstableUnicornCoreTest.BaseSet {
    public class ClassyNarwhalTest {
        [Fact]
        public void TestChoosingUpgradeCard() {
            SimplePlayerMockUp playerOne = new(), playerTwo = new();

            GameController controller = new GameController(new List<Card>(), new List<Card>(), new List<APlayer>() { playerOne, playerTwo });

            // protection before shuffling
            Card rainbowAura = new RainbowAura().GetCardTemplate().CreateCard();
            controller.Pile.Add(rainbowAura);
            Card classyNarwhal = new ClassyNarwhal().GetCardTemplate().CreateCard();
            controller.Pile.Add(classyNarwhal);

            controller.PlayerDrawCard(playerOne);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 1, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            Assert.Equal(playerOne.Hand[0], classyNarwhal);

            // player one playes unicorn to his own stable
            controller.PlayCardAndResolveChainLink(classyNarwhal, playerOne);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 1, stableSize: 1, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);

            Assert.Equal(rainbowAura, playerOne.Hand[0]);
        }

        [Fact]
        public void TestChoosingUpgradeCardNarwhalPlayedToDifferentStable() {
            SimplePlayerMockUp playerOne = new(), playerTwo = new();

            GameController controller = new GameController(new List<Card>(), new List<Card>(), new List<APlayer>() { playerOne, playerTwo });

            // protection before shuffling
            Card rainbowAura = new RainbowAura().GetCardTemplate().CreateCard();
            controller.Pile.Add(rainbowAura);
            Card classyNarwhal = new ClassyNarwhal().GetCardTemplate().CreateCard();
            controller.Pile.Add(classyNarwhal);

            controller.PlayerDrawCard(playerOne);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 1, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            Assert.Equal(playerOne.Hand[0], classyNarwhal);

            // player one playes unicorn to his opponent's stable
            controller.PlayCardAndResolveChainLink(classyNarwhal, playerTwo);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 1, stableSize: 1, numUpgrades: 0, numDowngrades: 0);

            Assert.Equal(rainbowAura, playerTwo.Hand[0]);
        }

        [Fact]
        public void TestShufflingDeck() {
            SimplePlayerMockUp playerOne = new(), playerTwo = new();

            GameController controller = new GameController(new List<Card>(), new List<Card>(), new List<APlayer>() { playerOne, playerTwo });

            // protection before shuffling
            CardTemplate basicUnicornTemplate = new BasicUnicorn().GetCardTemplate();
            for (int i = 0; i < 60; i++)
                controller.Pile.Add(basicUnicornTemplate.CreateCard());

            Card rainbowAura = new RainbowAura().GetCardTemplate().CreateCard();
            controller.Pile.Add(rainbowAura);
            Card classyNarwhal = new ClassyNarwhal().GetCardTemplate().CreateCard();
            controller.Pile.Add(classyNarwhal);

            controller.PlayerDrawCard(playerOne);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 1, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            Assert.Equal(playerOne.Hand[0], classyNarwhal);

            var copyList = new List<Card>(controller.Pile);
            var correctlyShuffled = copyList.Shuffle(new Random(42));
            controller.Random = new Random(42);

            // player one playes unicorn to his own stable
            controller.PlayCardAndResolveChainLink(classyNarwhal, playerOne);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 1, stableSize: 1, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);

            Assert.Equal(rainbowAura, playerOne.Hand[0]);

            for(int i = 0; i < 60; i++) {
                Assert.Equal(correctlyShuffled[i], controller.Pile[i]);
            }
        }
    }
}
