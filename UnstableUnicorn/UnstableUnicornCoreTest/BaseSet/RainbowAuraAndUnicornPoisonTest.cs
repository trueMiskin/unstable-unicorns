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
    public class RainbowAuraAndUnicornPoisonTest {
        [Fact]
        public void TestUnicornPoison_TestDestroyEffect() {
            SimplePlayerMockUp playerOne = new(), playerTwo = new();
            BasicUnicorn basicUnicornTemplate = new();
            GameController controller = new GameController(new List<Card>(), new List<Card>(), new List<APlayer>() { playerOne, playerTwo }, shufflePlayers: false);

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
            GameController controller = new GameController(new List<Card>(), new List<Card>(), new List<APlayer>() { playerOne, playerTwo }, shufflePlayers: false);

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

            TestUtils.CardCantBePlayed(unicornPoison, playerTwo, controller);

            // Unicorn poison can't be played -> no valid targets, so
            // nothing happend
            // can't be selected rainbowAura because it is not unicorn card
            // can't be selected basicUnicorn because it it protected by rainbowAura

            Assert.Empty(playerOne.Hand);
            Assert.Single(playerTwo.Hand);

            Assert.Single(playerOne.Stable);
            Assert.Equal(basicUnicorn, playerOne.Stable[0]);
            Assert.Single(playerOne.Upgrades);
            Assert.Equal(rainbowAura, playerOne.Upgrades[0]);
            Assert.Empty(playerOne.Downgrades);

            Assert.Empty(playerTwo.Stable);
            Assert.Empty(playerTwo.Upgrades);
            Assert.Empty(playerTwo.Downgrades);

            Assert.Empty(controller.DiscardPile);
        }

        [Fact]
        public void TestFunctionalityBothCardsOneValidTargetInvalidSelect() {
            SimplePlayerMockUp playerOne = new(), playerTwo = new(), playerThree = new();
            GameController controller;
            Card basicUnicorn, rainbowAura, basicUnicornSecond, unicornPoison;
            controller = new GameController(new List<Card>(), new List<Card>(), new List<APlayer>() { playerOne, playerTwo, playerThree }, shufflePlayers: false);

            BasicUnicorn basicUnicornTemplate = new();
            // protection before shuffling
            basicUnicorn = basicUnicornTemplate.GetCardTemplate().CreateCard();
            controller.Pile.Add(basicUnicorn);
            rainbowAura = new RainbowAura().GetCardTemplate().CreateCard();
            controller.Pile.Add(rainbowAura);
            basicUnicornSecond = basicUnicornTemplate.GetCardTemplate().CreateCard();
            controller.Pile.Add(basicUnicornSecond);
            unicornPoison = new UnicornPoison().GetCardTemplate().CreateCard();
            controller.Pile.Add(unicornPoison);

            controller.PlayerDrawCard(playerThree);
            controller.PlayerDrawCard(playerTwo);
            controller.PlayerDrawCard(playerOne);
            controller.PlayerDrawCard(playerOne);

            Assert.Equal(playerOne.Hand[0], rainbowAura);
            Assert.Equal(playerOne.Hand[1], basicUnicorn);
            Assert.Equal(playerTwo.Hand[0], basicUnicornSecond);
            Assert.Equal(playerThree.Hand[0], unicornPoison);
            playerThree.ChooseCardsWhichCantBeDestroy = true;

            controller.PlayCardAndResolveChainLink(basicUnicorn, playerOne);
            controller.PlayCardAndResolveChainLink(rainbowAura, playerOne);
            controller.PlayCardAndResolveChainLink(basicUnicornSecond, playerTwo);
            Action act = () => controller.PlayCardAndResolveChainLink(unicornPoison, playerThree);

            // After unicorn poison is played one valid target can be selected, so
            // can be selected only his own basic unicorn
            // but player althought select player's one unicorn which cannot be selected
            var exception = Assert.Throws<InvalidOperationException>(act);
            Assert.Equal("Selected item which is not from available selection.", exception.Message);
        }
    }
}
