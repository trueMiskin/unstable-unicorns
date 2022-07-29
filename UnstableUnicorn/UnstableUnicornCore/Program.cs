using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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

        public static List<Deck> findAllDecks() {
            // load dll in plugin directory
            string directory = "plugin-decks";
            try {
                string[] assemblies = Directory.GetFiles(directory);

                foreach (string assembly in assemblies)
                    Assembly.LoadFrom(assembly);
            } catch (IOException) { }

            // find all class which extends deck
            List<Deck> decks = new List<Deck>();
            foreach (Assembly assem in AppDomain.CurrentDomain.GetAssemblies()) {
                foreach (Type type in assem.GetTypes()
                    .Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(Deck))))
                {
                    object? newDeck = Activator.CreateInstance(type);
                    Console.WriteLine("Found new deck: {0}", newDeck);
                    if (newDeck != null)
                        decks.Add((Deck) newDeck);
                }
            };
            return decks;
        }

        public static void Main(string[] args) {
            var deck = findAllDecks();
            Console.Write("Select deck (0 - {0}):", deck.Count - 1);
            int selected = Int32.Parse(Console.ReadLine());

            for (int id = 347516; ; id++) {
                Console.WriteLine($"---------> Starting game {id+1} <---------");
                List<APlayer> players = new();
                for (int x = 0; x < 6; x++) {
                    players.Add(new RandomPlayer());
                }
                players.Add(new DefaultConsolePlayer());

                var game = CreateGame(deck[selected], players, id);

                game.SimulateGame();
            }
        }
    }
}