﻿using GeneticSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using UnstableUnicornCore.BaseSet;

namespace UnstableUnicornCore.Agent {
    public class MyProblemFitness : IFitness {
        public double Evaluate(IChromosome chromosome) {
            List<Tiers> weights = chromosome.GetGenes().ToList().ConvertAll(g => (Tiers)(int)g.Value);
            var cardStrength = EvolutionAgent.GetCardStrength(weights);

            double fitness = 0;
            Console.WriteLine("Fitness evaluate started");
            for (int id = 0; id < 10; id++) {
                List<APlayer> players = new();
                players.Add(new RandomPlayer());
                players.Add(new RuleBasedAgent());
                players.Add(new MctsAgent(200, () => new RandomPlayer()));
                players.Add(new MctsAgent(200, () => new RuleBasedAgent()));
                var evaPlayer = new EvolutionAgent(cardStrength);
                players.Add(evaPlayer);
                players.Add(new MctsAgent(200, () => new EvolutionAgent(cardStrength)));

                Stopwatch stopWatch = Stopwatch.StartNew();
                var game = Program.CreateGame(new List<Deck> { new SecondPrintDeck() }, players, id, VerbosityLevel.None);
                game.SimulateGame();
                stopWatch.Stop();

                Console.WriteLine("Evaluate {0} ms", stopWatch.ElapsedMilliseconds);

                var evaResultIdx = game.GameResults.FindIndex(result => result.Player == evaPlayer);

                fitness += Math.Pow(2, 6 - evaResultIdx);
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

    public class ArithmeticCrossover : CrossoverBase {
        public ArithmeticCrossover() : base(2, 2) { }

        protected override IList<IChromosome> PerformCross(IList<IChromosome> parents) {
            var parent1 = parents[0];
            var parent2 = parents[1];

            var child1 = parent1.CreateNew();
            var child2 = parent2.CreateNew();

            var alpha = RandomizationProvider.Current.GetDouble();

            for (int i = 0; i < parent1.Length; i++) {
                var gene1 = (int)parent1.GetGene(i).Value;
                var gene2 = (int)parent2.GetGene(i).Value;

                child1.ReplaceGene(i, new Gene((int)(alpha * gene1 + (1 - alpha) * gene2)));
                child2.ReplaceGene(i, new Gene((int)((1 - alpha) * gene1 + alpha * gene2)));
            }

            return new List<IChromosome> { child1, child2 };
        }
    }

    public class ThreadExecutor : TaskExecutorBase {
        private Semaphore semaphore;

        public ThreadExecutor(int maxThreads)
        {
            semaphore = new Semaphore(maxThreads, maxThreads);
        }

        public override bool Start()
        {
            try
            {
                base.Start();
                var threads = new Thread[Tasks.Count];

                for (int i = 0; i < Tasks.Count; i++)
                {
                    semaphore.WaitOne();

                    threads[i] = new Thread(() => { Tasks[i](); semaphore.Release();});
                    threads[i].Start();
                }

                for (int i = 0; i < threads.Length; i++)
                {
                    threads[i].Join();
                }

                return true;
            }
            finally
            {
                IsRunning = false;
            }
        }
    }

    public class EvolutionAgent : RuleBasedAgent {

        public EvolutionAgent(string file) {
            if (!File.Exists(file)) {
                runEvolution(file);
            }
            loadBestIndividual(file);
        }

        public EvolutionAgent(Dictionary<string, Tiers> cardStrength) => _cardStrength = cardStrength;

        private void loadBestIndividual(string file) {
            List<Tiers> weights;
            using (var reader = new StreamReader(file))
                weights = reader.ReadLine().Split(';').ToList().ConvertAll(str => (Tiers) int.Parse(str));

            _cardStrength = GetCardStrength(weights);
        }

        public static Dictionary<string, Tiers> GetCardStrength(List<Tiers> weights) {
            var keys = CardStrength.Keys.ToList();
            keys.Sort();

            Dictionary<string, Tiers> cardStrength = new();
            for (int i = 0; i < keys.Count; i++) {
                cardStrength.Add(keys[i], weights[i]);
            }

            return cardStrength;
        }

        private void runEvolution(string file) {
            var selection = new TournamentSelection();

            var crossover = new ArithmeticCrossover();
            var mutation = new UniformMutation(allGenesMutable: true);
            var fitness = new MyProblemFitness();
            var chromosome = new MyProblemChromosome(CardStrength.Keys.Count);
            var population = new Population(24, 24, chromosome);

            var ga = new GeneticAlgorithm(population, fitness, selection, crossover, mutation);
            var executor = new ThreadExecutor(maxThreads: 8);
            ga.TaskExecutor = executor;

            var stats = new StreamWriter($"stats-{file}");
            ga.GenerationRan += delegate {
                string statText = string.Format("Generation {0}, best fitness {1}, avg. fitness {2}, max fitness {3}",
                    ga.GenerationsNumber,
                    ga.BestChromosome.Fitness,
                    ga.Population.CurrentGeneration.Chromosomes.Average(c => c.Fitness),
                    ga.Population.CurrentGeneration.Chromosomes.Max(c => c.Fitness)
                );
                Console.WriteLine(statText);
                stats.WriteLine(statText);
            };

            ga.Termination = new GenerationNumberTermination(10);

            Console.WriteLine("GA running...");
            ga.Start();

            Console.WriteLine("Best solution found has {0} fitness.", ga.BestChromosome.Fitness);
            var weights = ga.BestChromosome.GetGenes().ToList().ConvertAll(g => (int)g.Value);
            var values = string.Join(';', weights);
            Console.WriteLine("Values: {}", values);

            stats.Close();
            using (var writer = new StreamWriter(file))
                writer.WriteLine(values);

            using (var writer = new StreamWriter($"last-pop-{file}"))
                foreach (var individual in ga.Population.CurrentGeneration.Chromosomes)
                    writer.WriteLine(string.Join(';', individual.GetGenes().ToList().ConvertAll(g => (int)g.Value)));
        }
    }
}