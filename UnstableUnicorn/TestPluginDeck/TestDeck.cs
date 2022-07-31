﻿using System;
using System.Collections.Generic;
using UnstableUnicornCore;
using UnstableUnicornCore.BaseSet;

namespace TestPluginDeck {
    /// <summary>
    /// Test deck - smaller deck than <see cref="SecondPrintDeck"/>
    /// </summary>
    public class TestDeck : Deck {
        public override string Name => "Test plugin deck";

        public override IEnumerable<(CardTemplateSource card, int count)> BabyUnicorns() {
            yield return (new BabyNarwhal(), 1);
            yield return (new BabyUnicorn(), 12);
        }

        public override IEnumerable<(CardTemplateSource card, int count)> OtherCards() {
            yield return (new BasicUnicorn(), 7);
            yield return (new Narwhal(), 1);
            yield return (new AlluringNarwhal(), 1);
            yield return (new Americorn(), 1);

            yield return (new AnnoyingFlyingUnicorn(), 1);
            yield return (new BlackKnightUnicorn(), 1);
            yield return (new ChainsawUnicorn(), 1);
            yield return (new ClassyNarwhal(), 1);

            yield return (new GreedyFlyingUnicorn(), 1);
            yield return (new MagicalFlyingUnicorn(), 1);
            yield return (new MajesticFlyingUnicorn(), 1);

            yield return (new MermaidUnicorn(), 1);
            yield return (new NarwhalTorpedo(), 1);
            yield return (new RainbowUnicorn(), 1);
            yield return (new Rhinocorn(), 1);

            yield return (new SeductiveUnicorn(), 1);
            yield return (new ShabbyTheNarwhal(), 1);
            yield return (new SharkWithAHorn(), 1);
            yield return (new StabbyTheUnicorn(), 1);
            yield return (new SwiftFlyingUnicorn(), 1);
            yield return (new TheGreatNarwhal(), 1);
            yield return (new UnicornOnTheCob(), 1);

            //
            // Spells
            //
            yield return (new BackKick(), 3);
            yield return (new BlatantThievery(), 1);
            yield return (new ChangeOfLuck(), 2);
            yield return (new GlitterTornado(), 2);
            yield return (new GoodDeal(), 1);
            yield return (new MysticalVortex(), 1);
            yield return (new ReTarget(), 2);
            yield return (new ResetButton(), 1);
            yield return (new ShakeUp(), 1);
            yield return (new TargetedDestruction(), 1);
            yield return (new TwoForOne(), 2);
            yield return (new UnicornPoison(), 3);
            yield return (new UnicornSwap(), 2);
            //
            // Instants
            //
            yield return (new Neigh(), 14);
            yield return (new SuperNeigh(), 1);
            //
            // Upgrades
            //
            yield return (new DoubleDutch(), 1);
            yield return (new ExtraTail(), 3);
            yield return (new GlitterBomb(), 2);
            yield return (new RainbowAura(), 1);
            yield return (new RainbowMane(), 3);
            yield return (new SummoningRitual(), 1);
            yield return (new UnicornLasso(), 1);
            yield return (new Yay(), 2);
            //
            // Downgrades
            //
            yield return (new BarbedWire(), 1);
            yield return (new BlindingLight(), 1);
            yield return (new BrokenStable(), 1);
            // nany cam
            yield return (new Pandamonium(), 1);
            yield return (new SadisticRitual(), 1);
            yield return (new Slowdown(), 1);
            yield return (new TinyStable(), 1);
        }
    }
}
