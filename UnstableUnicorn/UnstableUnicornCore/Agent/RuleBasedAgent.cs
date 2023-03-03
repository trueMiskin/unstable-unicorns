using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnstableUnicornCore.BaseSet;

namespace UnstableUnicornCore.Agent {
    /// <summary>
    /// Rule based agent is a simple agent which uses a set of rules to make decisions.
    /// In most time agent dicisides based on the card's strength.
    /// 
    /// The strength of cards is categorized based on Reddit post & commments:
    /// https://www.reddit.com/r/UnstableUnicorns/comments/himbmz/my_personal_tier_list_for_the_base_deck_first/
    /// </summary>
    public class RuleBasedAgent : APlayer {
        protected Dictionary<string, Tiers> _cardStrength;
        public RuleBasedAgent() {
            _cardStrength = CardStrength;
        }

        protected override bool ActivateEffectCore(AEffect effect) {
            return true;
        }

        protected override List<APlayer> ChoosePlayersCore(int number, AEffect effect, List<APlayer> playersWhichCanBeSelected) {
            List<APlayer> players = new();
            if (playersWhichCanBeSelected.Contains(this))
                players.Add(this);
            
            var availablePlayers = GetSortedPlayerByStableSize(playersWhichCanBeSelected);

            for(int i = 0; players.Count < number; i++){
                if (availablePlayers[i] != this)
                    players.Add(availablePlayers[i]);
            }

            return players;
        }

        protected override Card? PlayInstantOnStackCore(List<Card> stack, List<Card> availableInstantCards) {
            Debug.Assert(stack.Count > 0);
            Debug.Assert(stack[0].Player != null);
            APlayer player = stack[0].Player;
            Card card = stack[0];
            Tiers tier = GetCardTier(card);

            if (player == this){
                // i don't want react on my card
                if (stack.Count % 2 == 1)
                    return null;
                if (tier >= Tiers.AA)
                    return availableInstantCards.First();
            }else{
                // i want react only opponent card
                if (stack.Count % 2 == 0)
                    return null;
                if (tier >= Tiers.AA)
                    return availableInstantCards.First();
            }

            return null;
        }

        protected override APlayer WhereShouldBeCardPlayedCore(Card card) {
            if (ECardTypeUtils.UnicornTarget.Contains(card.CardType)
                    || card.CardType == ECardType.Instant 
                    || card.CardType == ECardType.Spell
                    || card.CardType == ECardType.Upgrade
            )
                return this;

            var players = GetSortedPlayerByStableSize(GameController.Players);

            if (players[0] == this)
                return players[1];
            return players[0];
        }

        protected override List<Card> WhichCardsToDestroyCore(int number, AEffect effect, List<Card> cardsWhichCanBeSelected) {
            // same as return card (WhichCardsToReturnCore)
            var cards = cardsWhichCanBeSelected.OrderBy(c => GetCardTier(c)
                + (c.Player == this ? 100 : 0)
                + (c.CardType == ECardType.Downgrade && c.Player != this ? 1000 : 0)
                + (c.CardType == ECardType.Downgrade && c.Player == this ? -1000 : 0)
            );
            
            return cards.Take(number).ToList();
        }

        protected override List<Card> WhichCardsToDiscardCore(int number, AEffect? effect, List<Card> cardsWhichCanBeSelected) {
            var cards = cardsWhichCanBeSelected.OrderByDescending(c => GetCardTier(c));
            
            return cards.Take(number).ToList();
        }

        protected override List<Card> WhichCardsToGetCore(int number, AEffect effect, List<Card> cardsWhichCanBeSelected) {
            var cards = cardsWhichCanBeSelected.OrderBy(c => GetCardTier(c));

            return cards.Take(number).ToList();
        }

        protected override List<Card> WhichCardsToMoveCore(int number, AEffect effect, List<Card> cardsWhichCanBeSelected) {
            // Not really good end point
            // var players = from c in cardsWhichCanBeSelected
            //             where c.Player != null
            //             group c by c.Player into g
            //             orderby g.Count() descending
            //             select g.Key;
            
            return RandomSelectionFromCards(number, cardsWhichCanBeSelected);
        }

        private List<Card> RandomSelectionFromCards(int number, List<Card> cardsWhichCanBeSelected) {
            HashSet<int> selectedCards = new();

            while(selectedCards.Count != number) {
                selectedCards.Add(GameController.Random.Next(cardsWhichCanBeSelected.Count));
            }

            return (from idx in selectedCards select cardsWhichCanBeSelected[idx]).ToList();
        }

        protected override List<Card> WhichCardsToReturnCore(int number, AEffect effect, List<Card> cardsWhichCanBeSelected) {
            // primarly dont return my cards except downgrades in my stable
            var cards = cardsWhichCanBeSelected.OrderBy(c => GetCardTier(c)
                + (c.Player == this ? 100 : 0)
                + (c.CardType == ECardType.Downgrade && c.Player != this ? 1000 : 0)
                + (c.CardType == ECardType.Downgrade && c.Player == this ? -1000 : 0)
            );
            
            return cards.Take(number).ToList();
        }

        protected override List<Card> WhichCardsToSacrificeCore(int number, AEffect effect, List<Card> cardsWhichCanBeSelected) {
            var cards = cardsWhichCanBeSelected.OrderByDescending(c => GetCardTier(c));

            return cards.Take(number).ToList();
        }

        protected override List<Card> WhichCardsToSaveCore(int number, AEffect effect, List<Card> cardsWhichCanBeSelected) {
            var cards = cardsWhichCanBeSelected.OrderBy(c => GetCardTier(c));

            return cards.Take(number).ToList();
        }

        protected override List<Card> WhichCardsToStealCore(int number, AEffect effect, List<Card> cardsWhichCanBeSelected) {
            var cards = cardsWhichCanBeSelected.OrderBy(c => GetCardTier(c));

            return cards.Take(number).ToList();
        }

        protected override Card? WhichCardToPlayCore() {
            var cards = from c in Hand
                        where ECardTypeUtils.UnicornTarget.Contains(c.CardType)
                            || c.CardType == ECardType.Spell
                            || c.CardType == ECardType.Upgrade
                            || c.CardType == ECardType.Downgrade
                        where c.CanBePlayed(this)
                        orderby GetCardTier(c) descending
                        select c;
            
            if (cards.Any())
                return cards.First();

            return null;
        }

        protected override AEffect WhichEffectToSelectCore(List<AEffect> effectsVariants) {
            return effectsVariants[GameController.Random.Next(effectsVariants.Count)];
        }

        private List<APlayer> GetSortedPlayerByStableSize(List<APlayer> players){
            var sortedPlayers = new List<APlayer>(players);
            sortedPlayers.Sort((a, b) => a.Stable.Count.CompareTo(b.Stable.Count));
            return sortedPlayers;
        }

        private Tiers GetCardTier(Card card, bool allowUnknown = false) {
            if (_cardStrength.TryGetValue(card.Name, out Tiers tier))
                return tier;
            
            if (allowUnknown)
                return Tiers.F;

            throw new InvalidOperationException("Card is not in the list of known cards.");
        }

        protected static readonly Dictionary<string, Tiers> CardStrength = new Dictionary<string, Tiers>(){
            {new BabyNarwhal().GetCardTemplate().CreateCard().Name, Tiers.F},
            {new BabyUnicorn().GetCardTemplate().CreateCard().Name, Tiers.F},

            {new BasicUnicorn().GetCardTemplate().CreateCard().Name, Tiers.F},
            {new Narwhal().GetCardTemplate().CreateCard().Name, Tiers.A},
            {new AlluringNarwhal().GetCardTemplate().CreateCard().Name, Tiers.AA},
            {new Americorn().GetCardTemplate().CreateCard().Name, Tiers.A},
            // angel unicorn
            {new AnnoyingFlyingUnicorn().GetCardTemplate().CreateCard().Name, Tiers.B},
            {new BlackKnightUnicorn().GetCardTemplate().CreateCard().Name, Tiers.D},
            {new ChainsawUnicorn().GetCardTemplate().CreateCard().Name, Tiers.C},
            {new ClassyNarwhal().GetCardTemplate().CreateCard().Name, Tiers.A},
            // extremely destructive
            // extremely fertile
            {new GinormousUnicorn().GetCardTemplate().CreateCard().Name, Tiers.E},
            {new GreedyFlyingUnicorn().GetCardTemplate().CreateCard().Name, Tiers.B},
            // Llamacorn
            {new MagicalFlyingUnicorn().GetCardTemplate().CreateCard().Name, Tiers.A},
            // magical kittencorn
            {new MajesticFlyingUnicorn().GetCardTemplate().CreateCard().Name, Tiers.AA},
            {new MermaidUnicorn().GetCardTemplate().CreateCard().Name, Tiers.A},
            // narwhal torpedo - A tier
            {new NarwhalTorpedo().GetCardTemplate().CreateCard().Name, Tiers.A},
            {new Puppicorn().GetCardTemplate().CreateCard().Name, Tiers.F},
            {new QueenBeeUnicorn().GetCardTemplate().CreateCard().Name, Tiers.AA},
            {new RainbowUnicorn().GetCardTemplate().CreateCard().Name, Tiers.A},
            {new Rhinocorn().GetCardTemplate().CreateCard().Name, Tiers.D},
            {new SeductiveUnicorn().GetCardTemplate().CreateCard().Name, Tiers.S},
            {new ShabbyTheNarwhal().GetCardTemplate().CreateCard().Name, Tiers.B},
            {new SharkWithAHorn().GetCardTemplate().CreateCard().Name, Tiers.E},
            {new StabbyTheUnicorn().GetCardTemplate().CreateCard().Name, Tiers.D},
            {new SwiftFlyingUnicorn().GetCardTemplate().CreateCard().Name, Tiers.AA},
            {new TheGreatNarwhal().GetCardTemplate().CreateCard().Name, Tiers.B},
            {new UnicornOnTheCob().GetCardTemplate().CreateCard().Name, Tiers.B},
            // unicorn phoenix
            // zombie unicorn

            //
            // Spells
            //
            {new BackKick().GetCardTemplate().CreateCard().Name, Tiers.B},
            {new BlatantThievery().GetCardTemplate().CreateCard().Name, Tiers.A},
            {new ChangeOfLuck().GetCardTemplate().CreateCard().Name, Tiers.D},
            {new GlitterTornado().GetCardTemplate().CreateCard().Name, Tiers.A},
            {new GoodDeal().GetCardTemplate().CreateCard().Name, Tiers.D},
            // ??????
            {new MysticalVortex().GetCardTemplate().CreateCard().Name, Tiers.AA},
            {new ReTarget().GetCardTemplate().CreateCard().Name, Tiers.D},
            {new ResetButton().GetCardTemplate().CreateCard().Name, Tiers.F},
            {new ShakeUp().GetCardTemplate().CreateCard().Name, Tiers.F},
            {new TargetedDestruction().GetCardTemplate().CreateCard().Name, Tiers.E},
            {new TwoForOne().GetCardTemplate().CreateCard().Name, Tiers.AA},
            {new UnfairBargain().GetCardTemplate().CreateCard().Name, Tiers.C},
            {new UnicornPoison().GetCardTemplate().CreateCard().Name, Tiers.D},
            {new UnicornShrinkray().GetCardTemplate().CreateCard().Name, Tiers.C},
            {new UnicornSwap().GetCardTemplate().CreateCard().Name, Tiers.C},
            //
            // Instants
            //
            {new Neigh().GetCardTemplate().CreateCard().Name, Tiers.AA},
            {new SuperNeigh().GetCardTemplate().CreateCard().Name, Tiers.S},
            //
            // Upgrades
            //
            {new DoubleDutch().GetCardTemplate().CreateCard().Name, Tiers.S},
            {new ExtraTail().GetCardTemplate().CreateCard().Name, Tiers.B},
            {new GlitterBomb().GetCardTemplate().CreateCard().Name, Tiers.AA},
            {new RainbowAura().GetCardTemplate().CreateCard().Name, Tiers.E},
            {new RainbowMane().GetCardTemplate().CreateCard().Name, Tiers.AA},
            {new SummoningRitual().GetCardTemplate().CreateCard().Name, Tiers.AA},
            // unicorn lasso - S tier
            {new UnicornLasso().GetCardTemplate().CreateCard().Name, Tiers.S},
            {new Yay().GetCardTemplate().CreateCard().Name, Tiers.S},
            //
            // Downgrades
            //
            {new BarbedWire().GetCardTemplate().CreateCard().Name, Tiers.A},
            {new BlindingLight().GetCardTemplate().CreateCard().Name, Tiers.E},
            {new BrokenStable().GetCardTemplate().CreateCard().Name, Tiers.F},
            // nany cam
            {new Pandamonium().GetCardTemplate().CreateCard().Name, Tiers.F},
            {new SadisticRitual().GetCardTemplate().CreateCard().Name, Tiers.C},
            {new Slowdown().GetCardTemplate().CreateCard().Name, Tiers.D},
            {new TinyStable().GetCardTemplate().CreateCard().Name, Tiers.AA},
        };
    }

    public enum Tiers{
        S, AA, A, B, C, D, E, F
    }

    // public static class CardStrength {
    //     public static Tiers GetTier(Card card) {
    //         return card switch {
    //             AlluringNarwhal a => Tiers.AA,
    //             _ => Tiers.F,
    //         };
    //     }
    // }
}