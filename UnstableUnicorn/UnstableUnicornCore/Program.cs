using System;
using System.Collections.Generic;
using UnstableUnicornCore.BaseSet;

namespace UnstableUnicornCore {

    public class Program {
        public static List<Card> CreateCardFromGenerator(IEnumerable<(CardTemplateSource card, int count)> enumerator) {
            List<Card> output = new();
            foreach (var (cardTmpSrc, value) in enumerator) {
                for (int i = 0; i < value; i++)
                    output.Add(cardTmpSrc.GetCardTemplate().CreateCard());
            }
            return output;
        }

        public static GameController CreateGame(Deck deck, List<APlayer> playes, int seed = 42) {
            List<Card> nursery = CreateCardFromGenerator(deck.BabyUnicorns());
            List<Card> pile = CreateCardFromGenerator(deck.OtherCards());

            Console.WriteLine($"Card in set: {nursery.Count + pile.Count}");
            return new GameController(pile, nursery, playes, seed);
        }

        public static void Main(string[] args) {
            for (int id = 347516; ; id++) {
                Console.WriteLine($"---------> Starting game {id+1} <---------");
                List<APlayer> players = new();
                for (int x = 0; x < 6; x++) {
                    players.Add(new RandomPlayer());
                }
                players.Add(new DefaultConsolePlayer());

                var game = CreateGame(new SecondPrintDeck(), players, id);

                game.SimulateGame();
            }
        }
    }
}