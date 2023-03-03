using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using UnstableUnicornCore.Agent;
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

        public static GameController CreateGame(List<Deck> decks, List<APlayer> playes, int gameSeed, VerbosityLevel verbosity) {
            List<Card> nursery = new();
            List<Card> pile = new();

            foreach (Deck deck in decks) {
                nursery.AddRange(CreateCardFromGenerator(deck.BabyUnicorns()));
                pile.AddRange(CreateCardFromGenerator(deck.OtherCards()));
            }

            Console.WriteLine($"Card in set: {nursery.Count + pile.Count}");
            return new GameController(pile, nursery, playes, gameSeed, verbosity);
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
                try {
                    foreach (Type type in assem.GetTypes()
                        .Where(type => type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(Deck)))) {
                        object? newDeck = Activator.CreateInstance(type);
                        Console.WriteLine("Found new deck: {0}", newDeck);
                        if (newDeck != null)
                            decks.Add((Deck)newDeck);
                    }
                } catch (ReflectionTypeLoadException ex) {
                    Console.WriteLine("Cannot load assembly: {0}", assem.FullName);
                    Console.WriteLine("Error: {0}", ex);
                }
            };
            return decks;
        }

        /// <summary>
        /// Select a number from to inclusive
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static int selectNumber(string msg, int from, int to) {
            while (true) {
                Console.WriteLine(msg);
                Console.WriteLine("Choose number from {0} to {1} (inclusive)", from, to);
                Console.Write("Your choose: ");
                string? answer = Console.ReadLine();
                if (Int32.TryParse(answer, out int selectedNumber)) {
                    if (selectedNumber >= from && selectedNumber <= to)
                        return selectedNumber;
                    else {
                        Console.WriteLine("Number not in available range!");
                    }
                } else {
                    Console.WriteLine("This is not a number!");
                }
            }
        }

        public static bool yesNoQuestion(string msg) {
            string? ans = "";
            while (ans == "") {
                Console.WriteLine(msg);
                Console.Write("Your answer? Y/N: ");
                ans = Console.ReadLine()?.ToLower();

                switch (ans) {
                    case "y":
                    case "yes":
                        ans = "y";
                        break;
                    case "n":
                    case "no":
                        ans = "n";
                        break;
                    default:
                        Console.WriteLine("Unknow answer! Try again.");
                        ans = "";
                        break;
                }
            }

            return ans == "y";
        }

        public static List<Deck> selectDecks(List<Deck> decks) {
            List<bool> selected = new();
            foreach (Deck _ in decks)
                selected.Add(false);

            while (true) {
                Console.WriteLine("Select decks which you want to include in the game:");
                Console.WriteLine("Available decks:");
                for (int idx = 0; idx < decks.Count; idx++) {
                    Deck deck = decks[idx];
                    Console.WriteLine("{0} - {2}deck {1}", idx + 1, deck.Name, selected[idx] ? "(selected) " : "");
                }
                Console.WriteLine("{0} - End deck selection", decks.Count + 1);
                Console.Write("Selection: ");

                string? answer = Console.ReadLine();
                if (Int32.TryParse(answer, out int selectedNumber)) {
                    if (selectedNumber >= 1 && selectedNumber <= decks.Count + 1)
                        if (selectedNumber == decks.Count + 1) {
                            if (selected.Any(value => value))
                                break;
                            Console.WriteLine("You don't select any deck!!");
                        } else
                            selected[selectedNumber - 1] = !selected[selectedNumber - 1];
                    else {
                        Console.WriteLine("Number not in available range!");
                    }
                } else {
                    Console.WriteLine("This is not a number!");
                }
            }

            List<Deck> ans = new();
            for (int i = 0; i < decks.Count; i++)
                if (selected[i])
                    ans.Add(decks[i]);
            return ans;
        }

        public static void handleGameInteractively() {
            var deck = findAllDecks();

            // TODO: change win condition for less players: 3-5 -> 7 unicorns
            // for 6-8 players win condition is to have 6 unicorns in stable
            int numberOfPlayers = selectNumber("Choose number of players", 6, 8);
            int seed = new Random().Next();

            bool onlyBots = !yesNoQuestion("Do you want control on of the player (not bot only game)?");

            Console.WriteLine($"---------> Starting game, game seed {seed} <---------");
            List<APlayer> players = new();
            for (int x = 0; x < numberOfPlayers; x++) {
                if (x == 0 && !onlyBots)
                    players.Add(new DefaultConsolePlayer());
                else
                    players.Add(new RandomPlayer());
            }

            var game = CreateGame(selectDecks(deck), players, seed, VerbosityLevel.None);

            game.SimulateGame();
        }

        public static void Main(string[] args) {
            /*
            handleGameInteractively();

            // For bot testing
            // 347516
            // 2938278
            // 8704718
            /*/
            //for (int id = 8704718; ; id++) {
            //    Console.WriteLine($"---------> Starting game {id+1} <---------");
            //    List<APlayer> players = new();
            //    for (int x = 0; x < 6; x++) {
            //        players.Add(new RandomPlayer());
            //    }

            //    var game = CreateGame(new List<Deck> { new SecondPrintDeck() }, players, id);

            //    game.SimulateGame();
            //}

            string test_name = "test4";
            int maxTurns = 1;
            int ruleBasedAgentWins = 0, mctsAgentWins = 0;
            for (int id = 0; id < maxTurns; id++) {
                Console.WriteLine($"---------> Starting game {id + 1} <---------");
                List<APlayer> players = new();
                for (int x = 0; x < 4; x++) {
                    players.Add(new RandomPlayer());
                }
                for (int x = 0; x < 2; x++) {
                    //players.Add(new RuleBasedAgent());
                    //players.Add(new MctsAgent(200, () => new RuleBasedAgent()));
                    players.Add(new EvolutionAgent("vahy.txt"));
                }

                var game = CreateGame(new List<Deck> { new SecondPrintDeck() }, players, id, VerbosityLevel.All);

                game.SimulateGame();

                Console.WriteLine($"Game ended after {game.TurnNumber} turns");
                foreach (var result in game.GameResults)
                    Console.WriteLine($"Player id: {result.PlayerId}, value: {result.NumUnicorns}, len: {result.SumUnicornNames}");

                if (game.GameResults.First().Player is RuleBasedAgent)
                    ruleBasedAgentWins++;
                if (game.GameResults.First().Player is MctsAgent)
                    mctsAgentWins++;

                var toLog = new GameRecord(gameSeed: id, gameLength: game.TurnNumber, game.GameResults, game.GameLog);

                using var stream = File.Create(test_name + "-seed=" + id + ".json");

                JsonSerializer.Serialize(stream, toLog, new JsonSerializerOptions {
                    WriteIndented= true,
                });
            }

            Console.WriteLine($"RuleBasedAgent won {ruleBasedAgentWins} times from {maxTurns} games.");
            Console.WriteLine($"MctsAgent won {mctsAgentWins} times from {maxTurns} games.");
            /**/
        }
    }
}