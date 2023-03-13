﻿using System.Collections.Generic;
using UnstableUnicornCore;
using UnstableUnicornCore.BaseSet;
using Xunit;

namespace UnstableUnicornCoreTest.BaseSet {
    public class BlindingLightTest {

        [Fact]
        public void TestDontEffectingSpells() {
            SimplePlayerMockUp playerOne = new(), playerTwo = new();

            GameController controller = new GameController(new List<Card>(), new List<Card>(), new List<APlayer>() { playerOne, playerTwo }, shufflePlayers: false);

            // protection before shuffling
            Card basicUnicorn = new BasicUnicorn().GetCardTemplate().CreateCard();
            controller.Pile.Add(basicUnicorn);
            Card blindingLight = new BlindingLight().GetCardTemplate().CreateCard();
            controller.Pile.Add(blindingLight);
            Card unicornPoison = new UnicornPoison().GetCardTemplate().CreateCard();
            controller.Pile.Add(unicornPoison);

            controller.PlayerDrawCard(playerOne);
            controller.PlayerDrawCard(playerOne);
            controller.PlayerDrawCard(playerTwo);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 2, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 1, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            Assert.Equal(playerOne.Hand[0], unicornPoison);
            Assert.Equal(playerOne.Hand[1], blindingLight);
            Assert.Equal(playerTwo.Hand[0], basicUnicorn);

            // player one play downgrade to his own stable
            controller.PlayCardAndResolveChainLink(blindingLight, playerOne);
            controller.PlayCardAndResolveChainLink(basicUnicorn, playerTwo);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 1, stableSize: 0, numUpgrades: 0, numDowngrades: 1);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 1, numUpgrades: 0, numDowngrades: 0);

            controller.PlayCardAndResolveChainLink(unicornPoison, playerOne);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 1);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
        }

        [Fact]
        public void TestUnicornsAreEffectedByCard() {
            SimplePlayerMockUp playerOne = new(), playerTwo = new();

            GameController controller = new GameController(new List<Card>(), new List<Card>(), new List<APlayer>() { playerOne, playerTwo }, shufflePlayers: false);

            // protection before shuffling
            Card annoyingFlyingUnicorn = new AnnoyingFlyingUnicorn().GetCardTemplate().CreateCard();
            controller.Pile.Add(annoyingFlyingUnicorn);
            Card blindingLight = new BlindingLight().GetCardTemplate().CreateCard();
            controller.Pile.Add(blindingLight);
            Card unicornPoison = new UnicornPoison().GetCardTemplate().CreateCard();
            controller.Pile.Add(unicornPoison);

            controller.PlayerDrawCard(playerOne);
            controller.PlayerDrawCard(playerOne);
            controller.PlayerDrawCard(playerTwo);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 2, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 1, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            Assert.Equal(playerOne.Hand[0], unicornPoison);
            Assert.Equal(playerOne.Hand[1], blindingLight);
            Assert.Equal(playerTwo.Hand[0], annoyingFlyingUnicorn);

            // player one play blinding light into player's two stable
            controller.PlayCardAndResolveChainLink(blindingLight, playerTwo);
            // ability of flying unicorn is not triggered
            controller.PlayCardAndResolveChainLink(annoyingFlyingUnicorn, playerTwo);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 1, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 1, numUpgrades: 0, numDowngrades: 1);

            // second ability is not triggered
            controller.PlayCardAndResolveChainLink(unicornPoison, playerOne);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 1);
        }

        // TODO: This test dont work with unicorn poison because it can only destroy unicorns...
        /*[Fact]
        public void TestInteractionWithPandamonium() {
            SimplePlayerMockUp playerOne = new(), playerTwo = new();

            GameController controller = new GameController(new List<Card>(), new List<Card>(), new List<APlayer>() { playerOne, playerTwo });

            // protection before shuffling
            Card annoyingFlyingUnicorn = new AnnoyingFlyingUnicorn().GetCardTemplate().CreateCard();
            controller.Pile.Add(annoyingFlyingUnicorn);
            Card pandamonium = new Pandamonium().GetCardTemplate().CreateCard();
            controller.Pile.Add(pandamonium);
            Card blindingLight = new BlindingLight().GetCardTemplate().CreateCard();
            controller.Pile.Add(blindingLight);
            Card unicornPoison = new UnicornPoison().GetCardTemplate().CreateCard();
            controller.Pile.Add(unicornPoison);

            controller.PlayerDrawCard(playerOne);
            controller.PlayerDrawCard(playerOne);
            controller.PlayerDrawCard(playerOne);
            controller.PlayerDrawCard(playerTwo);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 3, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 1, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            Assert.Equal(playerOne.Hand[0], unicornPoison);
            Assert.Equal(playerOne.Hand[1], blindingLight);
            Assert.Equal(playerOne.Hand[2], pandamonium);
            Assert.Equal(playerTwo.Hand[0], annoyingFlyingUnicorn);

            // player one play blinding light into player's two stable
            controller.PlayCardAndResolveChainLink(blindingLight, playerTwo);
            // player one play pandamonium into player's two stable
            controller.PlayCardAndResolveChainLink(pandamonium, playerTwo);
            // ability of flying unicorn is not triggered
            controller.PlayCardAndResolveChainLink(annoyingFlyingUnicorn, playerTwo);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 1, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 1, numUpgrades: 0, numDowngrades: 2);

            // second ability should be triggered
            controller.PlayCardAndResolveChainLink(unicornPoison, playerOne);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 1, stableSize: 0, numUpgrades: 0, numDowngrades: 2);
        }*/
    }
}
