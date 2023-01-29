using System;
using UnstableUnicornCore;
using Xunit;

namespace UnstableUnicornCoreTest {
    public static class TestUtils {
        public static void CheckPlayerPileSizes(APlayer player, int handSize, int stableSize, int numUpgrades,
                                                int numDowngrades) {
            Assert.Equal(handSize, player.Hand.Count);
            Assert.Equal(stableSize, player.Stable.Count);
            Assert.Equal(numUpgrades, player.Upgrades.Count);
            Assert.Equal(numDowngrades, player.Downgrades.Count);
        }

        public static void CardCantBePlayed(Card card, APlayer targetOwner, GameController controller) {
            Assert.False(card.CanBePlayed(targetOwner));
        }

        public static void CheckKnownPlayerCardsOfTarget(GameController controller,
                                                         APlayer player,
                                                         APlayer targetPlayer,
                                                         params Card[] expectedValues) {
            var visibilityTracker = controller.CardVisibilityTracker;
            var knowledgeAboutPlayer = visibilityTracker.GetKnownPlayerCardsOfTarget(player, targetPlayer);
            Assert.Equal(expectedValues.Length, knowledgeAboutPlayer.Count);
            
            foreach (var card in expectedValues)
                Assert.Contains(card, knowledgeAboutPlayer);
        }
    }
}
