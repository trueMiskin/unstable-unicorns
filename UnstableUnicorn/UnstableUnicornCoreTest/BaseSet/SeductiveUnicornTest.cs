using System.Collections.Generic;
using UnstableUnicornCore;
using UnstableUnicornCore.BaseSet;
using Xunit;

namespace UnstableUnicornCoreTest.BaseSet {
    public class SeductiveUnicornTest {
        [Fact]
        public void TestReturnEffectAfterLeave() {
            SimplePlayerMockUp playerOne = new(), playerTwo = new();

            GameController controller = new GameController(new List<Card>(), new List<Card>(), new List<APlayer>() { playerOne, playerTwo }, shufflePlayers: false);

            // protection before shuffling
            Card basicUnicorn = new BasicUnicorn().GetCardTemplate().CreateCard();
            controller.Pile.Add(basicUnicorn);
            Card seductiveUnicorn = new SeductiveUnicorn().GetCardTemplate().CreateCard();
            controller.Pile.Add(seductiveUnicorn);
            controller._allCards.AddRange(controller.Pile);

            controller.PlayerDrawCard(playerOne);
            controller.PlayerDrawCard(playerOne);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 2, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            Assert.Equal(playerOne.Hand[0], seductiveUnicorn);

            controller.PlayCardAndResolveChainLink(basicUnicorn, playerTwo);
            controller.PlayCardAndResolveChainLink(seductiveUnicorn, playerOne);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 0, stableSize: 2, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);

            controller.PublishEvent(ETriggerSource.BeginningTurn);
            controller.ResolveChainLink();
            controller.PublishEvent(ETriggerSource.EndTurn);
            controller.ResolveChainLink();
            controller.PublishEvent(ETriggerSource.BeginningTurn);
            controller.ResolveChainLink();

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 0, stableSize: 2, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);

            Card unicornPoison = new UnicornPoison().GetCardTemplate().CreateCard();
            controller.Pile.Add(unicornPoison);
            controller.PlayerDrawCard(playerTwo);

            // simple player will destroy first unicorn -> Seductive Unicorn
            controller.PlayCardAndResolveChainLink(unicornPoison, playerTwo);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 1, numUpgrades: 0, numDowngrades: 0);

            Assert.Empty(controller.Pile);
            Assert.Equal(2, controller.DiscardPile.Count);
        }

        [Fact]
        public void TestSeductiveUnicornAndStealedUnicornDestroyedInSameChainLink() {
            SimplePlayerMockUp playerOne = new(), playerTwo = new();

            GameController controller = new GameController(new List<Card>(), new List<Card>(), new List<APlayer>() { playerOne, playerTwo }, shufflePlayers: false);

            // protection before shuffling
            Card seductiveUnicorn = new SeductiveUnicorn().GetCardTemplate().CreateCard();
            controller.Pile.Add(seductiveUnicorn);

            Card basicUnicorn = new BasicUnicorn().GetCardTemplate().CreateCard();
            controller.Pile.Add(basicUnicorn);
            Card secondBasicUnicorn = new BasicUnicorn().GetCardTemplate().CreateCard();
            controller.Pile.Add(secondBasicUnicorn);
            Card twoForOne = new TwoForOne().GetCardTemplate().CreateCard();
            controller.Pile.Add(twoForOne);
            controller._allCards.AddRange(controller.Pile);

            controller.PlayerDrawCard(playerOne);
            controller.PlayerDrawCard(playerOne);
            controller.PlayerDrawCard(playerOne);
            controller.PlayerDrawCard(playerTwo);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 3, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 1, stableSize: 0, numUpgrades: 0, numDowngrades: 0);

            controller.PlayCardAndResolveChainLink(basicUnicorn, playerOne);
            controller.PlayCardAndResolveChainLink(seductiveUnicorn, playerTwo);
            controller.PlayCardAndResolveChainLink(secondBasicUnicorn, playerOne);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 1, stableSize: 1, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 2, numUpgrades: 0, numDowngrades: 0);

            // two for one -> sacrifice own and destroy 2 unicorns -> on table should be nothing
            controller.PlayCardAndResolveChainLink(twoForOne, playerOne);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);

            Assert.Empty(controller.Pile);
            Assert.Equal(4, controller.DiscardPile.Count);
        }

        /// <summary>
        /// Test situation when some unicorn should be stolen by unicorn lasso but
        /// in same chain link is destroyed seductive unicorn -> so stolen unicorn
        /// is moved to player's stable where is unicorn lasso and !!! don't register effects !!!
        /// and after that is immediatly moved to original player's stable (before stealing by
        /// seductive unicorn)
        /// </summary>
        [Fact]
        public void TestUnicornLassoAndRhinocorn() {
            SimplePlayerMockUp playerOne = new(), playerTwo = new(), playerThree = new();

            GameController controller = new GameController(new List<Card>(), new List<Card>(), new List<APlayer>() { playerOne, playerTwo, playerThree }, shufflePlayers: false);

            // protection before shuffling
            Card seductiveUnicorn = new SeductiveUnicorn().GetCardTemplate().CreateCard();
            controller.Pile.Add(seductiveUnicorn);

            Card unicornLasso = new UnicornLasso().GetCardTemplate().CreateCard();
            controller.Pile.Add(unicornLasso);
            Card rhinocorn = new Rhinocorn().GetCardTemplate().CreateCard();
            controller.Pile.Add(rhinocorn);
            Card magicalFlyingUnicorn = new MagicalFlyingUnicorn().GetCardTemplate().CreateCard();
            controller.Pile.Add(magicalFlyingUnicorn);
            controller._allCards.AddRange(controller.Pile);

            controller.PlayerDrawCard(playerThree);
            controller.PlayerDrawCard(playerTwo);
            controller.PlayerDrawCard(playerTwo);
            controller.PlayerDrawCard(playerOne);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 1, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 2, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerThree, handSize: 1, stableSize: 0, numUpgrades: 0, numDowngrades: 0);

            controller.PlayCardAndResolveChainLink(magicalFlyingUnicorn, playerThree);
            controller.PlayCardAndResolveChainLink(seductiveUnicorn, playerOne);
            controller.PlayCardAndResolveChainLink(rhinocorn, playerTwo);
            controller.PlayCardAndResolveChainLink(unicornLasso, playerTwo);

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 0, stableSize: 2, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 1, numUpgrades: 1, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerThree, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);

            // rhinocorn will target seductive unicorn
            // unicorn lasso will target basic unicorn
            // but when seductive unicorn is destroyed then
            // basic unicorn is moved to previous owner so magical flying unicorn
            // will be in player's three stable not in player's two stable
            controller.ActualPlayerOnTurn = playerTwo;
            controller.PublishEvent(ETriggerSource.BeginningTurn);
            // little bit hack -> we know that unicorn lasso will choose targets early (implementation detail)
            playerTwo.PrimarilyStealThisCard = magicalFlyingUnicorn.Name;
            controller.ResolveChainLink();

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 1, numUpgrades: 1, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerThree, handSize: 0, stableSize: 1, numUpgrades: 0, numDowngrades: 0);

            Assert.Empty(controller.Pile);
            Assert.Single(controller.DiscardPile);

            // unicorn lasso effect is still active, so magical flying unicorn is returned to player's one stable
            controller.PublishEvent(ETriggerSource.EndTurn);
            controller.ResolveChainLink();

            TestUtils.CheckPlayerPileSizes(playerOne, handSize: 0, stableSize: 1, numUpgrades: 0, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerTwo, handSize: 0, stableSize: 1, numUpgrades: 1, numDowngrades: 0);
            TestUtils.CheckPlayerPileSizes(playerThree, handSize: 0, stableSize: 0, numUpgrades: 0, numDowngrades: 0);

            Assert.Empty(controller.Pile);
            Assert.Single(controller.DiscardPile);
        }
    }
}
