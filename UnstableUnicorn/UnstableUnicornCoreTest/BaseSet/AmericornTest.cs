/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
﻿using System;
using System.Collections.Generic;
using UnstableUnicornCore;
using UnstableUnicornCore.BaseSet;
using Xunit;

namespace UnstableUnicornCoreTest.BaseSet {
    public class AmericornTest {
        [Fact]
        public void TestPullOpponentCardEffect() {
            SimplePlayerMockUp playerOne = new(), playerTwo = new();
            BasicUnicorn basicUnicornTemplate = new();
            GameController controller = new GameController(new List<Card>(), new List<Card>(), new List<APlayer>() { playerOne, playerTwo }, shufflePlayers: false);

            // protection before shuffling
            Card basicUnicorn = basicUnicornTemplate.GetCardTemplate().CreateCard();
            controller.Pile.Add(basicUnicorn);
            Card americorn = new Americorn().GetCardTemplate().CreateCard();
            controller.Pile.Add(americorn);

            controller.PlayerDrawCard(playerOne);
            controller.PlayerDrawCard(playerTwo);

            Assert.Equal(playerOne.Hand[0], americorn);
            Assert.Equal(playerTwo.Hand[0], basicUnicorn);

            controller.PlayCardAndResolveChainLink(americorn, playerOne);
            
            Assert.Single(playerOne.Hand);
            Assert.Empty(playerTwo.Hand);
        }

        [Fact]
        public void TestCardVisibilityAfterPullEffect() {
            SimplePlayerMockUp playerOne = new(), playerTwo = new(), playerThree = new();
            BasicUnicorn basicUnicornTemplate = new();
            GameController controller = new GameController(new List<Card>(), new List<Card>(), new List<APlayer>() { playerOne, playerTwo, playerThree }, shufflePlayers: false);

            // protection before shuffling
            Card basicUnicorn = basicUnicornTemplate.GetCardTemplate().CreateCard();
            controller.Pile.Add(basicUnicorn);
            Card secondBasicUnicorn = basicUnicornTemplate.GetCardTemplate().CreateCard();
            controller.Pile.Add(secondBasicUnicorn);
            Card americorn = new Americorn().GetCardTemplate().CreateCard();
            controller.Pile.Add(americorn);

            controller.PlayerDrawCard(playerOne);
            controller.PlayerDrawCard(playerTwo);
            controller.PlayerDrawCard(playerTwo);

            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerOne, playerOne, americorn);
            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerOne, playerTwo);
            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerOne, playerThree);

            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerTwo, playerOne);
            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerTwo, playerTwo, basicUnicorn, secondBasicUnicorn);
            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerTwo, playerThree);

            // test from observer side
            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerThree, playerOne);
            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerThree, playerTwo);
            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerThree, playerThree);

            // with this seed - the pulled card will be basic unicorn
            controller.Random = new Random(42);
            controller.PlayCardAndResolveChainLink(americorn, playerOne);

            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerOne, playerOne, basicUnicorn);
            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerOne, playerTwo);
            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerOne, playerThree);

            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerTwo, playerOne, basicUnicorn);
            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerTwo, playerTwo, secondBasicUnicorn);
            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerTwo, playerThree);

            // test from observer side
            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerThree, playerOne);
            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerThree, playerTwo);
            TestUtils.CheckKnownPlayerCardsOfTarget(controller, playerThree, playerThree);
        }
    }
}
