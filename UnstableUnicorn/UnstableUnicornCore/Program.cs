using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using UnstableUnicornCore.Agent;
using UnstableUnicornCore.BaseSet;
using static UnstableUnicornCore.Agent.MyProblemFitness;

namespace UnstableUnicornCore {

    public class Program {
        private static List<Card> CreateCardFromGenerator(IEnumerable<(CardTemplateSource card, int count)> enumerator) {
            List<Card> output = new();
            foreach (var (cardTmpSrc, value) in enumerator) {
                for (int i = 0; i < value; i++)
                    output.Add(cardTmpSrc.GetCardTemplate().CreateCard());
            }
            return output;
        }

        /// <summary>
        /// Create game from list of decks and list of players with given seed and verbosity level
        /// </summary>
        /// <param name="decks"></param>
        /// <param name="playes"></param>
        /// <param name="gameSeed"></param>
        /// <param name="verbosity"></param>
        /// <returns></returns>
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

        private static List<Deck> findAllDecks() {
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
        private static int selectNumber(string msg, int from, int to) {
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

        private static bool yesNoQuestion(string msg) {
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

        private static List<Deck> selectDecks(List<Deck> decks) {
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

        private static void handleGameInteractively() {
            var deck = findAllDecks();

            int numberOfPlayers = selectNumber("Choose number of players", 3, 8);
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

            Console.WriteLine($"Game ended after {game.TurnNumber} turns");
            foreach (var result in game.GameResults)
                Console.WriteLine($"Player id: {result.PlayerId}, value: {result.NumUnicorns}, len: {result.SumUnicornNames}");

        }

        private static void varianceBenchmark(){
            var initSeeds = new int[]{0, 4_000, 6_000, 9_000, 13_000, 32_000, 48_000, 96_000, 160_000, 320_000};
            var numGames = new int[]{1, 2, 3, 5, 10, 20, 50, 100, 200, 500, 1000};

            foreach (var num in numGames) {
                Console.WriteLine($"Number of games: {num}");
                foreach (var seed in initSeeds) {
                    var results = new List<int>();
                    for (int i = 0; i < num; i++){
                        var players = new List<APlayer>();
                        for (int x = 0; x < 3; x++) {
                            players.Add(new RandomPlayer());
                        }
                        for (int x = 0; x < 3; x++) {
                            players.Add(new RuleBasedAgent());
                        }

                        var game = CreateGame(new List<Deck> { new SecondPrintDeck() }, players, seed + i, VerbosityLevel.None);

                        game.SimulateGame();
                        results.Add(game.GameResults.First().Player.GetType() == typeof(RuleBasedAgent) ? 1 : 0);
                    }

                    var avg = results.Average();
                    var variance = results.Select(x => Math.Pow(x - avg, 2)).Sum() / results.Count;
                    Console.WriteLine($"Seed: {seed}, avg: {avg}, variance: {variance}");
                }
            }
        }

        const string RandomOption = "random";
        const string RuleBasedOption = "rule_based";
        public static void Main(string[] args) {
            var gameCommand = new Command("game", "Run game");
            gameCommand.SetHandler(() => handleGameInteractively());

            var varianceBenchmarkCommand = new Command("variance-benchmark", "Run multiple games with different seeds.");
            varianceBenchmarkCommand.SetHandler(() => varianceBenchmark());

            var mctsAgentTestsCommand = new Command("mcts-agent-tests", "Compare Mcts agents with different numbers of playouts.");
            mctsAgentTestsCommand.SetHandler(() => mctsAgentTests());

            var evolutionCommand = new Command("evolution", "The command for running different evolutions.");
            evolutionCommand.AddAlias("evo");
            var pcNameArgument = new Argument<string>("pc-name", "The name of the pc where the evolution is running.");
            var populationSize = new Option<int>(new[]{ "--population-size", "--ps" }, "The size of the population.");
            populationSize.SetDefaultValue(16);
            var numGames_random = new Option<int>(new[]{ "--num-games", "--ng" }, "The number of games in each fitness evaluation.");
            numGames_random.SetDefaultValue(100);
            var numGames_mcts = new Option<int>(new[]{ "--num-games", "--ng" }, "The number of games in each fitness evaluation.");
            numGames_mcts.SetDefaultValue(10);
            var maxGenerations = new Option<int>(new[]{ "--max-generations", "--mg" }, "The maximum number of generations.");
            maxGenerations.SetDefaultValue(200);

            var evoMctsCommand = new Command("mcts", "Evolution where 5 agents are mcts agents.");
            var defaultPolicy = new Argument<string>("default-policy", "Default policy for MCTS agents.")
                .FromAmong(RandomOption, RuleBasedOption);
            var playoutsNum = new Option<int>("--playouts-num", "The number of playouts for MCTS agents.");
            playoutsNum.AddAlias("--pn");
            playoutsNum.SetDefaultValue(100);

            evoMctsCommand.AddArgument(pcNameArgument);
            evoMctsCommand.AddArgument(defaultPolicy);
            evoMctsCommand.AddOption(populationSize);
            evoMctsCommand.AddOption(maxGenerations);
            evoMctsCommand.AddOption(numGames_mcts);
            evoMctsCommand.AddOption(playoutsNum);

            evoMctsCommand.SetHandler((pcName, defaultPolicy, populationSize, maxGenerations, numGames, playoutsNum) => {
                System.Console.WriteLine($"Running mcts-{defaultPolicy} evolution on pc {pcName}");
                CreatePlayers createPlayers = (cardStrength) => {
                    List<APlayer> players = new();
                    for (int x = 0; x < 5; x++) {
                        switch(defaultPolicy){
                            case RandomOption:
                                players.Add(new MctsAgent(playoutsNum, () => new RandomPlayer()));
                                break;
                            case RuleBasedOption:
                                players.Add(new MctsAgent(playoutsNum, () => new RuleBasedAgent()));
                                break;
                            default:
                                throw new InvalidProgramException();
                        }
                    }
                    var evolutionAgent = new EvolutionAgent(cardStrength);
                    players.Add(evolutionAgent);
                    return (players, evolutionAgent);
                };
                EvolutionAgent.RunEvolution($"mcts_{defaultPolicy}-ps={populationSize}-mg={maxGenerations}-pn={playoutsNum}-ng={numGames}-{pcName}", createPlayers, populationSize, maxGenerations, numGames);
            }, pcNameArgument, defaultPolicy, populationSize, maxGenerations, numGames_mcts, playoutsNum);

            var evoRandomCommand = new Command("random", "Evolution where 5 agents are random agents.");
            evoRandomCommand.AddArgument(pcNameArgument);
            evoRandomCommand.AddOption(populationSize);
            evoRandomCommand.AddOption(maxGenerations);
            evoRandomCommand.AddOption(numGames_random);

            evoRandomCommand.SetHandler((pcName, populationSize, maxGenerations, numGames) => {
                CreatePlayers createPlayers = (cardStrength) => {
                    List<APlayer> players = new();
                    for (int x = 0; x < 5; x++) {
                        players.Add(new RandomPlayer());
                    }
                    var evolutionAgent = new EvolutionAgent(cardStrength);
                    players.Add(evolutionAgent);
                    return (players, evolutionAgent);
                };
                EvolutionAgent.RunEvolution($"random-ps={populationSize}-mg={maxGenerations}-ng={numGames}-{pcName}", createPlayers, populationSize, maxGenerations, numGames);
            }, pcNameArgument, populationSize, maxGenerations, numGames_random);

            var evoRuleBasedCommand = new Command("rule_based", "Evolution where 5 agents are rule-based agents.");
            evoRuleBasedCommand.AddArgument(pcNameArgument);
            evoRuleBasedCommand.AddOption(populationSize);
            evoRuleBasedCommand.AddOption(maxGenerations);
            evoRuleBasedCommand.AddOption(numGames_random);

            evoRuleBasedCommand.SetHandler((pcName, populationSize, maxGenerations, numGames) => {
                CreatePlayers createPlayers = (cardStrength) => {
                    List<APlayer> players = new();
                    for (int x = 0; x < 5; x++) {
                        players.Add(new RuleBasedAgent());
                    }
                    var evolutionAgent = new EvolutionAgent(cardStrength);
                    players.Add(evolutionAgent);
                    return (players, evolutionAgent);
                };
                EvolutionAgent.RunEvolution($"rule_based-ps={populationSize}-mg={maxGenerations}-ng={numGames}-{pcName}", createPlayers, populationSize, maxGenerations, numGames);
            }, pcNameArgument, populationSize, maxGenerations, numGames_random);

            var evoBestLastGenCommand = new Command("best_last_gen", "Evolution where 5 agents are the best agents from the last generation.");
            var directoryOption = new Option<DirectoryInfo?>("--load", "Set the directory from which the program should load the weights of individuals. From the directory, it will be selected best <population_size> individuals.");
            evoBestLastGenCommand.AddArgument(pcNameArgument);
            evoBestLastGenCommand.AddOption(populationSize);
            evoBestLastGenCommand.AddOption(maxGenerations);
            evoBestLastGenCommand.AddOption(numGames_random);
            evoBestLastGenCommand.AddOption(directoryOption);

            evoBestLastGenCommand.SetHandler((pcName, populationSize, maxGenerations, numGames, directory) => {
                List<EvolutionAgent>? firstGen = null;
                if (directory != null) {
                    List<(EvolutionAgent agent, string description, int wonGames)> loadedIndividuals = new();
                    foreach (FileInfo fi in directory.GetFiles()) {
                        int lineNum = 1;
                        foreach (var agent in EvolutionAgent.LoadIndividuals(fi.FullName)) {
                            loadedIndividuals.Add((agent, $"{fi.Name}:{lineNum++}", 0));
                        }
                    }

                    for (int individualIdx = 0; individualIdx < loadedIndividuals.Count; individualIdx++) {
                        var individual = loadedIndividuals[individualIdx];
                        individual.wonGames = evaluateEvolutionaryAgent(individual.agent);
                        loadedIndividuals[individualIdx] = individual;
                    }

                    var selectedIndividual = loadedIndividuals.OrderByDescending(individual => individual.wonGames).Take(populationSize);

                    if (selectedIndividual.Count() != populationSize)
                        throw new InvalidOperationException("Not enough individuals in the directory. The number of individuals is lower than the population size.");

                    Console.WriteLine("Selected individuals:");
                    foreach (var agent in selectedIndividual)
                        Console.WriteLine($"Agent won {agent.wonGames} games from file {agent.description}");

                    firstGen = selectedIndividual.Select(individual => individual.agent).ToList();
                }

                // zero generation agents will be evaluated by rule-based agents
                CreatePlayers createPlayers = (cardStrength) => {
                    List<APlayer> players = new();
                    for (int x = 0; x < 5; x++) {
                        players.Add(new RuleBasedAgent());
                    }
                    var evolutionAgent = new EvolutionAgent(cardStrength);
                    players.Add(evolutionAgent);
                    return (players, evolutionAgent);
                };
                EvolutionAgent.RunEvolution($"best_last_gen-ps={populationSize}-mg={maxGenerations}-ng={numGames}-{pcName}", createPlayers, populationSize, maxGenerations, numGames,
                    makeFitnessFunctionsFromLastGeneration: true, firstPopulation: firstGen);
            }, pcNameArgument, populationSize, maxGenerations, numGames_random, directoryOption);


            evolutionCommand.AddCommand(evoMctsCommand);
            evolutionCommand.AddCommand(evoRandomCommand);
            evolutionCommand.AddCommand(evoRuleBasedCommand);
            evolutionCommand.AddCommand(evoBestLastGenCommand);

            var evoEvaluateCommand = new Command("evo-evaluate", "Evaluate evolution agents with selected opponents.");
            var opponentArgument = new Argument<string>("opponent", "Selected opponents.")
                .FromAmong(RandomOption, RuleBasedOption);
            var directoryEvoEvaluateArgument = new Argument<DirectoryInfo>("directory", "Which directory should be evaluated?");
            var numberPlayerOption = new Option<int>(new[] { "--num-players", "--np" }, "How many evolutionary agents will be in the game?");
            numberPlayerOption.SetDefaultValue(2);

            evoEvaluateCommand.AddArgument(opponentArgument);
            evoEvaluateCommand.AddArgument(directoryEvoEvaluateArgument);
            evoEvaluateCommand.AddOption(numberPlayerOption);

            evoEvaluateCommand.SetHandler((opponentType, directory, numPlayer) => {
                List<(EvolutionAgent agent, string description, int wonGames)> loadedIndividuals = new();
                foreach (FileInfo fi in directory.GetFiles()) {
                    int lineNum = 1;
                    foreach (var agent in EvolutionAgent.LoadIndividuals(fi.FullName)) {
                        loadedIndividuals.Add((agent, $"{fi.Name}:{lineNum++}", 0));
                    }
                }

                for (int individualIdx = 0; individualIdx < loadedIndividuals.Count; individualIdx++) {
                    var individual = loadedIndividuals[individualIdx];
                    individual.wonGames = evaluateEvolutionaryAgent(individual.agent, numPlayer, opponentType);
                    loadedIndividuals[individualIdx] = individual;
                }

                foreach (var agent in loadedIndividuals.OrderByDescending(individual => individual.wonGames))
                    Console.WriteLine($"Agent won {agent.wonGames} games from file {agent.description}");
            }, opponentArgument, directoryEvoEvaluateArgument, numberPlayerOption);

            var rootCommand = new RootCommand("Unstable Unicorns CLI");
            rootCommand.AddCommand(gameCommand);
            rootCommand.AddCommand(varianceBenchmarkCommand);
            rootCommand.AddCommand(mctsAgentTestsCommand);
            rootCommand.AddCommand(evolutionCommand);
            rootCommand.AddCommand(evoEvaluateCommand);

            rootCommand.Invoke(args);
        }

        private static int evaluateEvolutionaryAgent(EvolutionAgent agent, int numberAgents = 1, string opponentType = RuleBasedOption) {
            int wonGames = 0;
            for (int gameIdx = 0; gameIdx < 1000; gameIdx++) {
                List<APlayer> players = new();
                for (int i = 0; i < numberAgents; i++)
                    players.Add(agent.CopyNew());
                for (int x = 0; x < 6 - numberAgents; x++) {
                    APlayer newPlayer = opponentType switch {
                        RuleBasedOption => new RuleBasedAgent(),
                        RandomOption => new RandomPlayer(),
                        _ => throw new NotImplementedException(),
                    };
                    players.Add(newPlayer);
                }
                var game = CreateGame(new List<Deck> { new SecondPrintDeck() }, players, gameIdx, VerbosityLevel.None);
                game.SimulateGame();

                if (game.GameResults.First().Player.GetType() == typeof(EvolutionAgent))
                    wonGames += 1;
            }
            return wonGames;
        }

        private static void mctsAgentTests(){
            int maxTurns = 100;
            for (int playouts = 100; playouts <= 800; playouts *= 2) {
                string folderName = "mcts_agent_tests";
                if (!Directory.Exists(folderName)) {
                    Directory.CreateDirectory(folderName);
                }
                int mctsAgentWins = 0;
                for (int id = 0; id < maxTurns; id++) {
                    List<APlayer> players = new();
                    for (int x = 0; x < 4; x++) {
                        players.Add(new RandomPlayer());
                    }
                    for (int x = 0; x < 2; x++) {
                        players.Add(new MctsAgent(playouts, () => new RuleBasedAgent()));
                    }

                    Stopwatch stopWatch = Stopwatch.StartNew();
                    var game = CreateGame(new List<Deck> { new SecondPrintDeck() }, players, id, VerbosityLevel.All);

                    game.SimulateGame();
                    stopWatch.Stop();
                    Console.WriteLine($"Game ended after {stopWatch.ElapsedMilliseconds} ms");

                    if (game.GameResults.First().Player is MctsAgent)
                        mctsAgentWins++;

                    var toLog = new GameRecord(gameSeed: id, gameLength: game.TurnNumber, game.GameResults, game.GameLog);

                    using var stream = File.Create($"{folderName}/mcts-{playouts}" + "-seed=" + id + ".json");

                    JsonSerializer.Serialize(stream, toLog, new JsonSerializerOptions {
                        WriteIndented= true,
                    });
                }
                Console.WriteLine($"MctsAgent won {mctsAgentWins} times from {maxTurns} games.");
            }
        }
    }
}
