using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;
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

            // Console.WriteLine($"Card in set: {nursery.Count + pile.Count}");
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

        public static void varianceBenchmark(){
            var initSeeds = new int[]{0, 2_000, 4_000, 8_000, 16_000, 32_000, 64_000, 128_000, 256_000, 512_000};
            var numGames = new int[]{1, 2, 3, 5, 10, 20, 50, 100, 200, 500, 1000};

            foreach (var num in numGames) {
                Console.WriteLine($"Number of games: {num}");
                foreach (var seed in initSeeds) {
                    var results = new List<int>();
                    for (int i = 0; i < num; i++){
                        var players = new List<APlayer>();
                        for (int x = 0; x < 3; x++) {
                            // players.Add(new RandomPlayer());
                            players.Add(new RuleBasedAgent());
                        }
                        for (int x = 0; x < 3; x++) {
                            players.Add(new EvolutionAgent("eva_logs/mcts_random-19"));
                            // players.Add(new RuleBasedAgent());
                        }

                        var game = CreateGame(new List<Deck> { new SecondPrintDeck() }, players, seed + i, VerbosityLevel.None);

                        game.SimulateGame();
                        results.Add(game.GameResults.First().Player.GetType() == typeof(EvolutionAgent) ? 1 : 0);
                    }

                    var avg = results.Average();
                    var variance = results.Select(x => Math.Pow(x - avg, 2)).Sum() / results.Count;
                    Console.WriteLine($"Seed: {seed}, avg: {avg}, variance: {variance}");
                }
            }
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

            if (args.Length == 2) {
                MyProblemFitness.CreatePlayers createPlayers;
                if(args[0] == "mcts_random") {
                    createPlayers = (cardStrength) => {
                        List<APlayer> players = new();
                        for (int x = 0; x < 5; x++) {
                            players.Add(new MctsAgent(200, () => new RandomPlayer()));
                        }
                        var evolutionAgent = new EvolutionAgent(cardStrength);
                        players.Add(evolutionAgent);
                        return (players, evolutionAgent);
                    };
                }else if (args[0] == "mcts_rule_based"){
                    createPlayers = (cardStrength) => {
                        List<APlayer> players = new();
                        for (int x = 0; x < 5; x++) {
                            players.Add(new MctsAgent(200, () => new RuleBasedAgent()));
                        }
                        var evolutionAgent = new EvolutionAgent(cardStrength);
                        players.Add(evolutionAgent);
                        return (players, evolutionAgent);
                    };
                }else {
                    System.Console.WriteLine("Unknown parameter: " + args[0]);
                    return;
                }
                EvolutionAgent.RunEvolution($"{args[0]}-{args[1]}", createPlayers);
                return;
            }
            if (args.Length == 5) {
                if (args[0] != "parametric_evolution") {
                    System.Console.WriteLine("Unknown parameter: " + args[0]);
                    return;
                }
                MyProblemFitness.CreatePlayers createPlayers = (cardStrength) => {
                    List<APlayer> players = new();
                    for (int x = 0; x < 5; x++) {
                        players.Add(new RandomPlayer());
                    }
                    var evolutionAgent = new EvolutionAgent(cardStrength);
                    players.Add(evolutionAgent);
                    return (players, evolutionAgent);
                };
                string computerNum = args[1];
                int populationSize = int.Parse(args[2]);
                int maxGenerations = int.Parse(args[3]);
                int numGames = int.Parse(args[4]);
                EvolutionAgent.RunEvolution($"{args[0]}-ps={populationSize}-mg={maxGenerations}-ng={numGames}-{computerNum}", createPlayers, populationSize, maxGenerations, numGames);
                return;
            }

            // varianceBenchmark();
            // return;

            string test_name = "test_rule_based_agent";
            int maxTurns = 100_000;

            Regex regex = new Regex("/random.*");
            var matches = Directory.EnumerateFiles("eva_logs").Where(f => regex.IsMatch(f));
            foreach (var match in matches) {
                var fileName = Path.GetFileName(match);
                System.Console.WriteLine("Testing: " + fileName);

                int ruleBasedAgentWins = 0, mctsAgentWins = 0, evolutionAgentWins = 0;
                for (int id = 0; id < maxTurns; id++) {
                    // Console.WriteLine($"---------> Starting game {id + 1} <---------");
                    List<APlayer> players = new();
                    for (int x = 0; x < 4; x++) {
                        players.Add(new RandomPlayer());
                        // players.Add(new RuleBasedAgent());
                        // players.Add(new MctsAgent(200, () => new RuleBasedAgent()));
                    }
                    for (int x = 0; x < 2; x++) {
                        players.Add(new RuleBasedAgent());
                        //players.Add(new MctsAgent(200, () => new RuleBasedAgent()));
                        // players.Add(new EvolutionAgent(match));
                    }

                    Stopwatch stopWatch = Stopwatch.StartNew();
                    var game = CreateGame(new List<Deck> { new SecondPrintDeck() }, players, id, VerbosityLevel.All);

                    game.SimulateGame();
                    stopWatch.Stop();
                    // Console.WriteLine($"Game ended after {stopWatch.ElapsedMilliseconds} ms");

                    // Console.WriteLine($"Game ended after {game.TurnNumber} turns");
                    // foreach (var result in game.GameResults)
                    //     Console.WriteLine($"Player id: {result.PlayerId}, value: {result.NumUnicorns}, len: {result.SumUnicornNames}");

                    if (game.GameResults.First().Player.GetType() == typeof(RuleBasedAgent))
                        ruleBasedAgentWins++;
                    if (game.GameResults.First().Player is MctsAgent)
                        mctsAgentWins++;
                    if (game.GameResults.First().Player is EvolutionAgent)
                        evolutionAgentWins++;

                    var toLog = new GameRecord(gameSeed: id, gameLength: game.TurnNumber, game.GameResults, game.GameLog);

                    using var stream = File.Create(test_name + "-seed=" + id + ".json");

                    JsonSerializer.Serialize(stream, toLog, new JsonSerializerOptions {
                        WriteIndented= true,
                    });
                }
                Console.WriteLine($"RuleBasedAgent won {ruleBasedAgentWins} times from {maxTurns} games.");
                Console.WriteLine($"MctsAgent won {mctsAgentWins} times from {maxTurns} games.");
                Console.WriteLine($"EvolutionAgent won {evolutionAgentWins} times from {maxTurns} games.");
            }

            /**/
        }
    }
}