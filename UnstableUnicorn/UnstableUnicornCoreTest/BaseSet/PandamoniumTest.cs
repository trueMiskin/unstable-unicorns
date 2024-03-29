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
    public class PandamoniumTest {
        [Fact]
        public void TestCardType() {
            SimplePlayerMockUp playerOne = new();
            BasicUnicorn basicUnicornTemplate = new();
            GameController controller = new GameController(new List<Card>(), new List<Card>(), new List<APlayer>() { playerOne });

            // protection before shuffling
            Card basicUnicorn = basicUnicornTemplate.GetCardTemplate().CreateCard();
            controller.Pile.Add(basicUnicorn);
            Card pandamonium = new Pandamonium().GetCardTemplate().CreateCard();
            controller.Pile.Add(pandamonium);

            controller.PlayerDrawCard(playerOne);
            controller.PlayerDrawCard(playerOne);

            Assert.Equal(playerOne.Hand[0], pandamonium);
            Assert.Equal(playerOne.Hand[1], basicUnicorn);

            Assert.Equal(ECardType.BasicUnicorn, basicUnicorn.CardType);
            controller.PlayCardAndResolveChainLink(basicUnicorn, playerOne);
            Assert.Equal(ECardType.BasicUnicorn, basicUnicorn.CardType);
            controller.PlayCardAndResolveChainLink(pandamonium, playerOne);
            Assert.Equal(ECardType.Panda, basicUnicorn.CardType);

            Assert.Empty(playerOne.Hand);
            Assert.Single(playerOne.Stable);
            Assert.Empty(playerOne.Upgrades);
            Assert.Single(playerOne.Downgrades);

            Assert.Empty(controller.DiscardPile);
        }

        [Fact]
        public void TestInteractionWithUnicornPoison() {
            SimplePlayerMockUp playerOne = new(), playerTwo = new();
            BasicUnicorn basicUnicornTemplate = new();
            GameController controller = new GameController(new List<Card>(), new List<Card>(), new List<APlayer>() { playerOne, playerTwo }, shufflePlayers: false);

            // protection before shuffling
            Card basicUnicorn = basicUnicornTemplate.GetCardTemplate().CreateCard();
            controller.Pile.Add(basicUnicorn);
            Card pandamonium = new Pandamonium().GetCardTemplate().CreateCard();
            controller.Pile.Add(pandamonium);
            Card unicornPoison = new UnicornPoison().GetCardTemplate().CreateCard();
            controller.Pile.Add(unicornPoison);

            controller.PlayerDrawCard(playerTwo);
            controller.PlayerDrawCard(playerOne);
            controller.PlayerDrawCard(playerOne);

            Assert.Equal(playerOne.Hand[0], pandamonium);
            Assert.Equal(playerOne.Hand[1], basicUnicorn);
            Assert.Equal(playerTwo.Hand[0], unicornPoison);

            Assert.Equal(ECardType.BasicUnicorn, basicUnicorn.CardType);
            controller.PlayCardAndResolveChainLink(basicUnicorn, playerOne);
            Assert.Equal(ECardType.BasicUnicorn, basicUnicorn.CardType);
            controller.PlayCardAndResolveChainLink(pandamonium, playerOne);
            Assert.Equal(ECardType.Panda, basicUnicorn.CardType);

            // Unicorn poison can target unicorn, but players one unicorn are pandas!
            // so unicorn spell can't be played
            TestUtils.CardCantBePlayed(unicornPoison, playerTwo, controller);

            Assert.Empty(playerOne.Hand);
            Assert.Single(playerOne.Stable);
            Assert.Empty(playerOne.Upgrades);
            Assert.Single(playerOne.Downgrades);

            Assert.Single(playerTwo.Hand);
            Assert.Empty(playerTwo.Stable);
            Assert.Empty(playerTwo.Upgrades);
            Assert.Empty(playerTwo.Downgrades);

            Assert.Empty(controller.DiscardPile);
        }
    }
}
