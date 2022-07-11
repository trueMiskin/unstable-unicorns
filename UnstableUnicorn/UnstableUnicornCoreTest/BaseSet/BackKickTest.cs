﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnstableUnicornCore;
using UnstableUnicornCore.BaseSet;
using Xunit;

namespace UnstableUnicornCoreTest.BaseSet {
    public class BackKickTest {
        [Fact]
        public void TestReturnThenDiscardCardWithoutValidTarget() {
            SimplePlayerMockUp playerOne = new(), playerTwo = new();
            GameController controller = new GameController(new List<Card>(), new List<Card>(), new List<APlayer>() { playerOne, playerTwo });

            // protection before shuffling
            Card backKick = new BackKick().GetCardTemplate().CreateCard();
            controller.Pile.Add(backKick);

            controller.PlayerDrawCard(playerOne);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 1, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            Assert.Equal(playerOne.Hand[0], backKick);

            Assert.False(backKick.CanBePlayed());

            Action act = () => controller.PlayCardAndResolveChainLink(backKick, playerOne);
            var exception = Assert.Throws<InvalidOperationException>(act);
            Assert.Equal(Card.CardCannotBePlayed, exception.Message);
        }

        [Fact]
        public void TestReturnThenDiscardCardWithValidTarget() {
            SimplePlayerMockUp playerOne = new(), playerTwo = new();
            GameController controller = new GameController(new List<Card>(), new List<Card>(), new List<APlayer>() { playerOne, playerTwo });

            // protection before shuffling
            Card backKick = new BackKick().GetCardTemplate().CreateCard();
            controller.Pile.Add(backKick);
            Card basicUnicorn = new BasicUnicorn().GetCardTemplate().CreateCard();
            controller.Pile.Add(basicUnicorn);

            controller.PlayerDrawCard(playerOne);
            controller.PlayerDrawCard(playerTwo);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 1, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 1, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            
            Assert.Equal(playerOne.Hand[0], basicUnicorn);
            Assert.Equal(playerTwo.Hand[0], backKick);

            controller.PlayCardAndResolveChainLink(basicUnicorn, playerOne);
            Assert.True(backKick.CanBePlayed());
            controller.PlayCardAndResolveChainLink(backKick, playerTwo);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);

            Assert.Equal(2, controller.DiscardPile.Count);
        }
    }
}
