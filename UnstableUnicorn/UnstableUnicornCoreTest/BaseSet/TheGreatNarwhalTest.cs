using System.Collections.Generic;
using UnstableUnicornCore;
using UnstableUnicornCore.BaseSet;
using Xunit;

namespace UnstableUnicornCoreTest.BaseSet {
    public class TheGreatNarwhalTest {

        [Theory]
        [MemberData(nameof(Data))]
        public void TestRegex(CardTemplateSource card, bool shouldMatch) {
            string name = card.GetCardTemplate().CreateCard().Name;
            Assert.Equal(shouldMatch, TheGreatNarwhal.narwhalRegex.IsMatch(name));
        }

        public static IEnumerable<object[]> Data() {
            yield return new object[] { new AlluringNarwhal(), true };
            yield return new object[] { new ClassyNarwhal(), true };
            yield return new object[] { new NarwhalTorpedo(), true };
            yield return new object[] { new ShabbyTheNarwhal(), true };
            yield return new object[] { new TheGreatNarwhal(), true };
            yield return new object[] { new Americorn(), false };
            yield return new object[] { new BrokenStable(), false };
            yield return new object[] { new QueenBeeUnicorn(), false };
            yield return new object[] { new SummoningRitual(), false };
        }
    }
}
