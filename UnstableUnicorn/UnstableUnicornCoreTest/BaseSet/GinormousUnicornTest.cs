using System.Collections.Generic;
using UnstableUnicornCore;
using UnstableUnicornCore.BaseSet;
using Xunit;

namespace UnstableUnicornCoreTest.BaseSet {
    public class GinormousUnicornTest {
        [Theory]
        [MemberData(nameof(Data))]
        public void TestUnicornValue(CardTemplateSource cardTemplateSource, int expectedUnicornValue) {
            SimplePlayerMockUp playerOne = new(), playerTwo = new();

            GameController controller = new GameController(new List<Card>(), new List<Card>(), new List<APlayer>() { playerOne, playerTwo }, shufflePlayers: false);

            // protection before shuffling
            Card card = cardTemplateSource.GetCardTemplate().CreateCard();
            controller.Pile.Add(card);

            controller.PlayerDrawCard(playerOne);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 1, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);

            Assert.Equal(expectedUnicornValue, card.UnicornValue);

            controller.PlayCardAndResolveChainLink(card, playerOne);
            
            Assert.Equal(expectedUnicornValue, card.UnicornValue);
        }

        public static IEnumerable<object[]> Data() {
            // unicorns
            yield return new object[] { new GinormousUnicorn(), 2 };
            yield return new object[] { new AlluringNarwhal(), 1 };
            yield return new object[] { new ClassyNarwhal(), 1 };
            yield return new object[] { new NarwhalTorpedo(), 1 };
            yield return new object[] { new ShabbyTheNarwhal(), 1 };
            yield return new object[] { new TheGreatNarwhal(), 1 };
            yield return new object[] { new Americorn(), 1 };
            yield return new object[] { new QueenBeeUnicorn(), 1 };
            // downgrades
            yield return new object[] { new BarbedWire(), 0 };
            yield return new object[] { new BrokenStable(), 0 };
            // upgrades
            yield return new object[] { new SummoningRitual(), 0 };
            yield return new object[] { new UnicornLasso(), 0 };
        }
    }
}
