using GeneticSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnstableUnicornCore.BaseSet;

namespace UnstableUnicornCore.Agent {
    public class MyProblemFitness : IFitness {
        public double Evaluate(IChromosome chromosome) {
            List<Tiers> weights = chromosome.GetGenes().ToList().ConvertAll(g => (Tiers)(int)g.Value);
            double fitness = 0;
            for (int id = 0; id < 10; id++) {
                List<APlayer> players = new();
                players.Add(new RandomPlayer());
                players.Add(new RuleBasedAgent());
                players.Add(new MctsAgent(200, () => new RandomPlayer()));
                players.Add(new MctsAgent(200, () => new RuleBasedAgent()));
                var evaPlayer = new EvolutionAgent(weights);
                players.Add(evaPlayer);
                players.Add(new MctsAgent(200, () => new EvolutionAgent(weights)));

                var game = Program.CreateGame(new List<Deck> { new SecondPrintDeck() }, players, id, VerbosityLevel.None);
                game.SimulateGame();

                var evaResultIdx = game.GameResults.FindIndex(result => result.Player == evaPlayer);

                fitness += Math.Pow(6 - evaResultIdx, 3);
            }
            return fitness;
        }
    }

    public class MyProblemChromosome : ChromosomeBase {
        int _length;
        public MyProblemChromosome(int length) : base(length) {
            _length = length;
            CreateGenes();
        }

        public override Gene GenerateGene(int geneIndex) {
            return new Gene(RandomizationProvider.Current.GetInt((int)Tiers.S, (int)Tiers.F));
        }

        public override IChromosome CreateNew() {
            return new MyProblemChromosome(_length);
        }
    }

    public class EvolutionAgent : RuleBasedAgent {

        public EvolutionAgent(string file) {
            if (!File.Exists(file)) {
                runEvolution(file);
            }
            loadBestIndividual(file);
        }

        public EvolutionAgent(List<Tiers> weights) => setWeights(weights);

        private void loadBestIndividual(string file) {
            List<Tiers> weights;
            using (var reader = new StreamReader(file))
                weights = reader.ReadLine().Split(';').ToList().ConvertAll(str => (Tiers) int.Parse(str));

            setWeights(weights);
        }

        private void setWeights(List<Tiers> weights) {
            var keys = CardStrength.Keys.ToList();
            keys.Sort();

            _cardStrength = new();
            for (int i = 0; i < keys.Count; i++) {
                _cardStrength.Add(keys[i], weights[i]);
            }
        }

        private void runEvolution(string file) {
            var selection = new TournamentSelection();
            var crossover = new UniformCrossover();
            var mutation = new UniformMutation(allGenesMutable: true);
            var fitness = new MyProblemFitness();
            var chromosome = new MyProblemChromosome(CardStrength.Keys.Count);
            var population = new Population(50, 70, chromosome);

            var ga = new GeneticAlgorithm(population, fitness, selection, crossover, mutation);
            ga.Termination = new GenerationNumberTermination(100);

            Console.WriteLine("GA running...");
            ga.Start();

            Console.WriteLine("Best solution found has {0} fitness.", ga.BestChromosome.Fitness);
            var weights = ga.BestChromosome.GetGenes().ToList().ConvertAll(g => (int)g.Value);
            var values = string.Join(';', weights);
            Console.WriteLine("Values: {}", values);

            using (var writer = new StreamWriter(file))
                writer.WriteLine(values);
        }
    }
}
