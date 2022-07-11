using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnstableUnicornCore;
using UnstableUnicornCore.BaseSet;
using Xunit;

namespace UnstableUnicornCoreTest.BaseSet {
    public class AmericornTest {
        [Fact]
        public void TestPullOpponentCardEffect() {
            SimplePlayerMockUp playerOne = new(), playerTwo = new();
            BasicUnicorn basicUnicornTemplate = new();
            GameController controller = new GameController(new List<Card>(), new List<Card>(), new List<APlayer>() { playerOne, playerTwo });

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
    }
}
