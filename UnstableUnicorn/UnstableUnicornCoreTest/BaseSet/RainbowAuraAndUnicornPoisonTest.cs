using System;
using System.Collections.Generic;
using UnstableUnicornCore;
using UnstableUnicornCore.BaseSet;
using Xunit;

namespace UnstableUnicornCoreTest.BaseSet {
    public class RainbowAuraAndUnicornPoisonTest {
        [Fact]
        public void TestUnicornPoison_TestDestroyEffect() {
            SimplePlayerMockUp playerOne = new(), playerTwo = new();
            BasicUnicorn basicUnicornTemplate = new();
            GameController controller = new GameController(new List<Card>(), new List<Card>(), new List<APlayer>() { playerOne, playerTwo });

            // protection before shuffling
            Card basicUnicorn = basicUnicornTemplate.GetCardTemplate().CreateCard();
            controller.Pile.Add(basicUnicorn);
            Card unicornPoison = new UnicornPoison().GetCardTemplate().CreateCard();
            controller.Pile.Add(unicornPoison);

            controller.PlayerDrawCard(playerTwo);
            controller.PlayerDrawCard(playerOne);

            Assert.Equal(playerOne.Hand[0], basicUnicorn);
            Assert.Equal(playerTwo.Hand[0], unicornPoison);

            controller.PlayCardAndResolveChainLink(basicUnicorn, playerOne);
            controller.PlayCardAndResolveChainLink(unicornPoison, playerTwo);

            Assert.Empty(playerOne.Hand);
            Assert.Empty(playerTwo.Hand);
            Assert.Empty(playerOne.Stable);
            Assert.Empty(playerTwo.Stable);
            Assert.Equal(2, controller.DiscardPile.Count);
        }

        [Fact]
        public void TestFunctionalityBothCardsNoValidTarget() {
            SimplePlayerMockUp playerOne = new(), playerTwo = new();
            BasicUnicorn basicUnicornTemplate = new();
            GameController controller = new GameController(new List<Card>(), new List<Card>(), new List<APlayer>() { playerOne, playerTwo });

            // protection before shuffling
            Card basicUnicorn = basicUnicornTemplate.GetCardTemplate().CreateCard();
            controller.Pile.Add(basicUnicorn);
            Card rainbowAura = new RainbowAura().GetCardTemplate().CreateCard();
            controller.Pile.Add(rainbowAura);
            Card unicornPoison = new UnicornPoison().GetCardTemplate().CreateCard();
            controller.Pile.Add(unicornPoison);

            controller.PlayerDrawCard(playerTwo);
            controller.PlayerDrawCard(playerOne);
            controller.PlayerDrawCard(playerOne);

            Assert.Equal(playerOne.Hand[0], rainbowAura);
            Assert.Equal(playerOne.Hand[1], basicUnicorn);
            Assert.Equal(playerTwo.Hand[0], unicornPoison);

            controller.PlayCardAndResolveChainLink(basicUnicorn, playerOne);
            controller.PlayCardAndResolveChainLink(rainbowAura, playerOne);
            controller.PlayCardAndResolveChainLink(unicornPoison, playerTwo);

            // After unicorn poison is played no valid target can be selected, so
            // nothing is happend
            // can't be selected rainbowAura because it is not unicorn card
            // can't be selected basicUnicorn because it it protected by rainbowAura

            Assert.Empty(playerOne.Hand);
            Assert.Empty(playerTwo.Hand);

            Assert.Single(playerOne.Stable);
            Assert.Equal(basicUnicorn, playerOne.Stable[0]);
            Assert.Single(playerOne.Upgrades);
            Assert.Equal(rainbowAura, playerOne.Upgrades[0]);
            Assert.Empty(playerOne.Downgrades);

            Assert.Empty(playerTwo.Stable);
            Assert.Empty(playerTwo.Upgrades);
            Assert.Empty(playerTwo.Downgrades);

            Assert.Single(controller.DiscardPile);
        }

        [Fact]
        public void TestFunctionalityBothCardsOneValidTarget() {
            SimplePlayerMockUp playerOne, playerTwo;
            GameController controller;
            Card basicUnicorn, rainbowAura, basicUnicornSecond, unicornPoison;
            prepareData(out playerOne, out playerTwo, out controller, out basicUnicorn, out rainbowAura, out basicUnicornSecond, out unicornPoison);

            controller.PlayCardAndResolveChainLink(basicUnicorn, playerOne);
            controller.PlayCardAndResolveChainLink(rainbowAura, playerOne);
            controller.PlayCardAndResolveChainLink(basicUnicornSecond, playerTwo);
            controller.PlayCardAndResolveChainLink(unicornPoison, playerTwo);

            // After unicorn poison is played one valid target can be selected, so
            // can be selected only his own basic unicorn
            // can't be selected rainbowAura because it is not unicorn card
            // can't be selected basicUnicorn because it it protected by rainbowAura

            Assert.Empty(playerOne.Hand);
            Assert.Empty(playerTwo.Hand);

            Assert.Single(playerOne.Stable);
            Assert.Equal(basicUnicorn, playerOne.Stable[0]);
            Assert.Single(playerOne.Upgrades);
            Assert.Equal(rainbowAura, playerOne.Upgrades[0]);
            Assert.Empty(playerOne.Downgrades);

            Assert.Empty(playerTwo.Stable);
            Assert.Empty(playerTwo.Upgrades);
            Assert.Empty(playerTwo.Downgrades);

            Assert.Equal(2, controller.DiscardPile.Count);
        }

        [Fact]
        public void TestFunctionalityBothCardsOneValidTargetInvalidSelect() {
            SimplePlayerMockUp playerOne, playerTwo;
            GameController controller;
            Card basicUnicorn, rainbowAura, basicUnicornSecond, unicornPoison;
            prepareData(out playerOne, out playerTwo, out controller, out basicUnicorn, out rainbowAura, out basicUnicornSecond, out unicornPoison);
            playerTwo.ChooseCardsWhichCantBeDestroy = true;

            controller.PlayCardAndResolveChainLink(basicUnicorn, playerOne);
            controller.PlayCardAndResolveChainLink(rainbowAura, playerOne);
            controller.PlayCardAndResolveChainLink(basicUnicornSecond, playerTwo);
            Action act = () => controller.PlayCardAndResolveChainLink(unicornPoison, playerTwo);

            // After unicorn poison is played one valid target can be selected, so
            // can be selected only his own basic unicorn
            // but player althought select player's one unicorn which cannot be selected
            var exception = Assert.Throws<InvalidOperationException>(act);
            Assert.Equal($"Card {basicUnicornSecond.Name} have not allowed card type or can't be destroyed", exception.Message);
        }

        private static void prepareData(out SimplePlayerMockUp playerOne, out SimplePlayerMockUp playerTwo, out GameController controller, out Card basicUnicorn, out Card rainbowAura, out Card basicUnicornSecond, out Card unicornPoison) {
            playerOne = new();
            playerTwo = new();
            BasicUnicorn basicUnicornTemplate = new();
            controller = new GameController(new List<Card>(), new List<Card>(), new List<APlayer>() { playerOne, playerTwo });

            // protection before shuffling
            basicUnicorn = basicUnicornTemplate.GetCardTemplate().CreateCard();
            controller.Pile.Add(basicUnicorn);
            rainbowAura = new RainbowAura().GetCardTemplate().CreateCard();
            controller.Pile.Add(rainbowAura);
            basicUnicornSecond = basicUnicornTemplate.GetCardTemplate().CreateCard();
            controller.Pile.Add(basicUnicornSecond);
            unicornPoison = new UnicornPoison().GetCardTemplate().CreateCard();
            controller.Pile.Add(unicornPoison);

            controller.PlayerDrawCard(playerTwo);
            controller.PlayerDrawCard(playerTwo);
            controller.PlayerDrawCard(playerOne);
            controller.PlayerDrawCard(playerOne);

            Assert.Equal(playerOne.Hand[0], rainbowAura);
            Assert.Equal(playerOne.Hand[1], basicUnicorn);
            Assert.Equal(playerTwo.Hand[0], unicornPoison);
            Assert.Equal(playerTwo.Hand[1], basicUnicornSecond);
        }
    }
}
