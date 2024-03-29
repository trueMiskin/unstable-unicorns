/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using UnstableUnicornCore.BaseSet;
using UnstableUnicornCore;
using System.Reflection;

namespace UnstableUnicornCoreTest.BaseSet {
    public class ChangeOfLuckTest {
        [Theory]
        [InlineData(0, false)]
        [InlineData(1, false)]
        [InlineData(2, false)]
        [InlineData(3, true)]
        public void TestConditionalEffect(int numberOtherCardsInHand, bool expectedOutput) {
            SimplePlayerMockUp player = new();
            BasicUnicorn basicUnicornTemplate = new();
            List<Card> cards = new();
            for (int i = 0; i < numberOtherCardsInHand; i++)
                cards.Add(basicUnicornTemplate.GetCardTemplate().CreateCard());
            Card changeOfLuck = new ChangeOfLuck().GetCardTemplate().CreateCard();
            cards.Add(changeOfLuck);

            GameController controller = new GameController(cards, new List<Card>(), new List<APlayer>() { player });

            for (int i = 0; i < numberOtherCardsInHand+1; i++) 
                controller.PlayerDrawCard(player);

            Assert.Equal(expectedOutput, changeOfLuck.CanBePlayed(player));
        }

        [Fact]
        public void TestDrawAndDiscardEffect() {
            SimplePlayerMockUp player = new();
            BasicUnicorn basicUnicornTemplate = new();
            GameController controller = new GameController(new List<Card>(), new List<Card>(), new List<APlayer>() { player });

            // protection before shuffling
            for (int i = 0; i < 60; i++)
                controller.Pile.Add(basicUnicornTemplate.GetCardTemplate().CreateCard());
            Card changeOfLuck = new ChangeOfLuck().GetCardTemplate().CreateCard();
            controller.Pile.Add(changeOfLuck);

            int numberOtherCardsInHand = 3;
            for (int i = 0; i < numberOtherCardsInHand + 1; i++)
                controller.PlayerDrawCard(player);

            controller.PlayCardAndResolveChainLink(changeOfLuck, player);

            Assert.Equal(2, player.Hand.Count);
        }
    }
}
