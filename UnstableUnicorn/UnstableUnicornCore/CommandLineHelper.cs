using CommandLine;
using System.Collections.Generic;

namespace UnstableUnicornCore {
    [Verb("game", isDefault: true, HelpText = "Run game")]
    public class GameOptions { }
    [Verb("variance-benchmark", HelpText = "Run multiple games with different seeds.")]
    public class VarianceBenchmark { }
    [Verb("mcts-agent-tests", HelpText = "Compare Mcts agents with different numbers of playouts.")]
    public class MctsAgentTestOptions { }

    [Verb("evolution", aliases: new[] { "evo" }, HelpText = "The command for running different evolutions.")]
    class Evolution { }
    public class EvoMctsOptions {
        [Value(0, Required = true, HelpText = "PC name.")]
        public string PcName { get; set; } = "";
    }

    [Verb("mcts_random", HelpText = "Evolution where 5 agents are mcts agents with a default policy of a random agent.")]
    public class EvoMctsRandomOptions : EvoMctsOptions { }
    [Verb("mcts_rule_based", HelpText = "Evolution where 5 agents are mcts agents with a default policy of a rule based agent.")]
    public class EvoMctsRuleBasedOptions : EvoMctsOptions { }
    [Verb("parametric", HelpText = "Evolution where 5 agents are mcts agents with a default policy of a rule based agent.")]
    public class EvoParametricOptions {
        [Value(0, Required = true, HelpText = "PC name.")]
        public string PcName { get; set; } = "";

        [Value(1, Required = true, HelpText = "Population size.")]
        public int PopulationSize { get; set; }

        [Value(2, Required = true, HelpText = "Max generations.")]
        public int MaxGenerations { get; set; }

        [Value(3, Required = true, HelpText = "Number games.")]
        public int NumGames { get; set; }
    }

    public static class CommandLineHelper {
        public static ParserResult<object> ParseInput(string[] args) {
            return Parser.Default
                .ParseSetArguments<GameOptions, VarianceBenchmark, MctsAgentTestOptions, Evolution>(args, parseSubverbs);
        }
        private static ParserResult<object> parseSubverbs(Parser parser,
        Parsed<object> parsed,
        IEnumerable<string> argsToParse,
        bool containedHelpOrVersion) {
            if (parsed.Value is not Evolution) {
                return parsed;
            }
            return parsed.MapResult(
                // parsing subcommands/subverbs
                (Evolution _) => parser.ParseArguments<EvoMctsRandomOptions, EvoMctsRuleBasedOptions, EvoParametricOptions>(argsToParse),
                (_) => parsed
            );
        }
    }
}
