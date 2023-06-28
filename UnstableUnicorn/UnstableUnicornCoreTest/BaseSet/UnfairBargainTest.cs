/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
﻿using System.Collections.Generic;
using UnstableUnicornCore;
using UnstableUnicornCore.BaseSet;
using Xunit;

namespace UnstableUnicornCoreTest.BaseSet {
    public class UnfairBargainTest {
        [Fact]
        public void TestSwapWithEmptyHand() {
            SimplePlayerMockUp playerOne = new(), playerTwo = new();

            GameController controller = new GameController(new List<Card>(), new List<Card>(), new List<APlayer>() { playerOne, playerTwo }, shufflePlayers: false);

            // protection before shuffling
            Card secondBasicUnicorn = new BasicUnicorn().GetCardTemplate().CreateCard();
            controller.Pile.Add(secondBasicUnicorn);
            Card basicUnicorn = new BasicUnicorn().GetCardTemplate().CreateCard();
            controller.Pile.Add(basicUnicorn);
            Card unfairBargain = new UnfairBargain().GetCardTemplate().CreateCard();
            controller.Pile.Add(unfairBargain);

            controller.PlayerDrawCard(playerOne);
            controller.PlayerDrawCard(playerTwo);
            controller.PlayerDrawCard(playerTwo);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 1, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 2, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            Assert.Equal(playerOne.Hand[0], unfairBargain);

            controller.PlayCardAndResolveChainLink(unfairBargain, playerOne);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 2, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
        }

        [Fact]
        public void TestSwapBothHandsNotEmpty() {
            SimplePlayerMockUp playerOne = new(), playerTwo = new();

            GameController controller = new GameController(new List<Card>(), new List<Card>(), new List<APlayer>() { playerOne, playerTwo }, shufflePlayers: false);

            // protection before shuffling
            Card secondBasicUnicorn = new BasicUnicorn().GetCardTemplate().CreateCard();
            controller.Pile.Add(secondBasicUnicorn);
            Card basicUnicorn = new BasicUnicorn().GetCardTemplate().CreateCard();
            controller.Pile.Add(basicUnicorn);
            Card thirdBasicUnicorn = new BasicUnicorn().GetCardTemplate().CreateCard();
            controller.Pile.Add(thirdBasicUnicorn);
            Card unfairBargain = new UnfairBargain().GetCardTemplate().CreateCard();
            controller.Pile.Add(unfairBargain);

            controller.PlayerDrawCard(playerOne);
            controller.PlayerDrawCard(playerOne);
            controller.PlayerDrawCard(playerTwo);
            controller.PlayerDrawCard(playerTwo);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 2, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 2, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            Assert.Equal(playerOne.Hand[0], unfairBargain);

            controller.PlayCardAndResolveChainLink(unfairBargain, playerOne);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 2, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 1, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
        }

        [Fact]
        public void TestSwapHandsWithCardsVisibility() {
            SimplePlayerMockUp playerOne = new(), playerTwo = new(), playerThree = new();

            GameController controller = new GameController(new List<Card>(), new List<Card>(), new List<APlayer>() { playerOne, playerTwo, playerThree }, shufflePlayers: false);
            // protection before shuffling
            Card secondBasicUnicorn = new BasicUnicorn().GetCardTemplate().CreateCard();
            controller.Pile.Add(secondBasicUnicorn);
            Card basicUnicorn = new BasicUnicorn().GetCardTemplate().CreateCard();
            controller.Pile.Add(basicUnicorn);
            
            Card thirdUnicorn = new BasicUnicorn().GetCardTemplate().CreateCard();
            controller.Pile.Add(thirdUnicorn);
            Card glitterTornado = new GlitterTornado().GetCardTemplate().CreateCard();
            controller.Pile.Add(glitterTornado);
            Card unfairBargain = new UnfairBargain().GetCardTemplate().CreateCard();
            controller.Pile.Add(unfairBargain);

            controller.PlayerDrawCard(playerOne);
            controller.PlayerDrawCard(playerOne);
            controller.PlayerDrawCard(playerOne);
            controller.PlayerDrawCard(playerTwo);
            controller.PlayerDrawCard(playerTwo);

            var visibilityTracker = controller.CardVisibilityTracker;
            Assert.Equal(3, visibilityTracker.GetKnownPlayerCardsOfTarget(playerOne, playerOne).Count);
            Assert.Equal(2, visibilityTracker.GetKnownPlayerCardsOfTarget(playerTwo, playerTwo).Count);

            controller.PlayCardAndResolveChainLink(basicUnicorn, playerTwo);
            // for playing glitter bomb player one must have unicorn in stable
            controller.PlayCardAndResolveChainLink(thirdUnicorn, playerOne);
            controller.PlayCardAndResolveChainLink(glitterTornado, playerOne);

            // Player 1 has: thirdUnicorn and unfairBargain
            // Player 2 has: basicUnicorn and secondBasicUnicorn
            // -> known card are basicUnicorn and thirdUnicorn for non-owning players

            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerOne, playerOne, thirdUnicorn, unfairBargain);
            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerOne, playerTwo, basicUnicorn);

            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerTwo, playerTwo, basicUnicorn, secondBasicUnicorn);
            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerTwo, playerOne, thirdUnicorn);

            // test from observer side
            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerThree, playerOne, thirdUnicorn);
            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerThree, playerTwo, basicUnicorn);

            controller.PlayCardAndResolveChainLink(unfairBargain, playerOne);

            // Swap cards between player one and player two
            // Player 1 knew his cards which give to player 2 and vice versa
            // Player 3 know only show cards 

            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerOne, playerOne, basicUnicorn, secondBasicUnicorn);
            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerOne, playerTwo, thirdUnicorn);

            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerTwo, playerTwo, thirdUnicorn);
            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerTwo, playerOne, basicUnicorn, secondBasicUnicorn);

            // test from observer side
            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerThree, playerOne, basicUnicorn);
            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerThree, playerTwo, thirdUnicorn);
        
            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 2, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 1, stableSize: 0, numUpgrades: 0, numDowngrades: 0);

        }
    }
}
