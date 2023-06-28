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
    public class NarwhalTorpedoTest {
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(10)]
        public void TestSacrificeEffect(int numberDowngrades) {
            SimplePlayerMockUp playerOne = new(), playerTwo = new();
            Pandamonium pandamoniumTemplate = new();
            GameController controller = new GameController(new List<Card>(), new List<Card>(), new List<APlayer>() { playerOne, playerTwo }, shufflePlayers: false);

            // protection before shuffling
            Card pandamoniumModel = pandamoniumTemplate.GetCardTemplate().CreateCard();
            
            for (int i = 0; i < numberDowngrades; i++)
                controller.Pile.Add(pandamoniumTemplate.GetCardTemplate().CreateCard());
            Card narwhalTorpedo = new NarwhalTorpedo().GetCardTemplate().CreateCard();
            controller.Pile.Add(narwhalTorpedo);
            
            controller.PlayerDrawCard(playerTwo);
            for (int i = 0; i < numberDowngrades; i++)
                controller.PlayerDrawCard(playerOne);

            Assert.Equal(playerTwo.Hand[0], narwhalTorpedo);
            Assert.Equal(numberDowngrades, playerOne.Hand.Count);
            for (int i = 0; i < numberDowngrades; i++)
                Assert.Equal(playerOne.Hand[0].Name, pandamoniumModel.Name);

            for (int i = 0; i < numberDowngrades; i++)
                controller.PlayCardAndResolveChainLink(playerOne.Hand[0], playerTwo);
            controller.PlayCardAndResolveChainLink(narwhalTorpedo, playerTwo);

            Assert.Empty(playerOne.Hand);
            Assert.Empty(playerOne.Stable);
            Assert.Empty(playerOne.Upgrades);
            Assert.Empty(playerOne.Downgrades);

            Assert.Empty(playerTwo.Hand);
            Assert.Single(playerTwo.Stable);
            Assert.Equal(narwhalTorpedo, playerTwo.Stable[0]);
            Assert.Empty(playerTwo.Upgrades);
            Assert.Empty(playerTwo.Downgrades);

            Assert.Equal(numberDowngrades, controller.DiscardPile.Count);
        }
    }
}
