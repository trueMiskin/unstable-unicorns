/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
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
        /// <summary>
        /// Delegate should create 5 players and evolution agent based on cardStrength
        /// </summary>
        /// <param name="cardStrength"></param>
        /// <returns>The all players and the evolution agent instance based on cardStrength</returns>
        public delegate (List<APlayer>, EvolutionAgent) CreatePlayers(Dictionary<string, double> cardStrength);

        internal CreatePlayers createPlayers;

        /// <summary>
        /// Init game seed for fitness evaluation
        /// 
        /// This seed should be changed over generations, because if we use the same seed
        /// then overfitting can occur.
        /// </summary>
        public int InitGameSeed = 0;
        int numberGames;

        public MyProblemFitness(CreatePlayers createPlayers, int numberGames)
        {
            this.createPlayers = createPlayers;
            this.numberGames = numberGames;
        }

        public double Evaluate(IChromosome chromosome) {
            List<double> weights = chromosome.GetGenes().ToList().ConvertAll(g => (double) g.Value);
            var cardStrength = EvolutionAgent.GetCardStrength(weights);

            double fitness = 0;
            int gameSeedShift = 0;
            Console.WriteLine("Fitness evaluate started");
            for (int gameNum = 0; gameNum < numberGames; gameNum++) {
                (List<APlayer> players, var evaPlayer) = createPlayers(cardStrength);

                Stopwatch stopWatch = Stopwatch.StartNew();
                var game = Program.CreateGame(new List<Deck> { new SecondPrintDeck() },
                                              players,
                                              InitGameSeed + gameNum + gameSeedShift,
                                              VerbosityLevel.None);
                try {
                    game.SimulateGame();
                } catch (Exception e) {
                    // when game crashes, we print the error and try again with a different seed
                    Console.Error.WriteLine(e);
                    gameNum--; gameSeedShift++;
                    continue;
                }
                stopWatch.Stop();

                Console.WriteLine("Evaluate {0} ms", stopWatch.ElapsedMilliseconds);

                var evaResultIdx = game.GameResults.FindIndex(result => result.Player == evaPlayer);

                fitness += Math.Pow(2, 6 - evaResultIdx);
            }
            return fitness;
        }
    }

    // public class MyProblemChromosome : ChromosomeBase {
    //     int _length;
    //     public MyProblemChromosome(int length) : base(length) {
    //         _length = length;
    //         CreateGenes();
    //     }

    //     public override Gene GenerateGene(int geneIndex) {
    //         return new Gene(RandomizationProvider.Current.GetInt((int)Tiers.S, (int)Tiers.F));
    //     }

    //     public override IChromosome CreateNew() {
    //         return new MyProblemChromosome(_length);
    //     }
    // }

    public class FloatingPointChromosome : ChromosomeBase {
        public FloatingPointChromosome(int length) : base(length) {
            CreateGenes();
        }

        public FloatingPointChromosome(int length, EvolutionAgent agent) : base(length){
            var weights = agent.GetWeights();
            for (int i = 0; i < Length; i++) {
                ReplaceGene(i, new Gene(weights[i]));
            }
        }

        public override Gene GenerateGene(int geneIndex) {
            return new Gene(RandomizationProvider.Current.GetDouble(0, 1));
        }

        public override IChromosome CreateNew() {
            return new FloatingPointChromosome(Length);
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
                var gene1 = (double)parent1.GetGene(i).Value;
                var gene2 = (double)parent2.GetGene(i).Value;

                double newGene = alpha * gene1 + (1 - alpha) * gene2;
                child1.ReplaceGene(i, new Gene(newGene));
                child2.ReplaceGene(i, new Gene(gene1 + gene2 - newGene));
            }

            return new List<IChromosome> { child1, child2 };
        }
    }

    public class CopyElitistReinsertion : ElitistReinsertion {
        protected override IList<IChromosome> PerformSelectChromosomes(IPopulation population, IList<IChromosome> offspring, IList<IChromosome> parents) {
            var diff = population.MinSize - offspring.Count;

            if (diff > 0)
            {
                var bestParents = parents.OrderByDescending(p => p.Fitness).Take(diff).ToList();

                for (int i = 0; i < bestParents.Count; i++)
                {
                    var copy = bestParents[i].CreateNew();
                    copy.ReplaceGenes(0, bestParents[i].GetGenes());
                    offspring.Add(copy);
                }
            }
            return offspring;
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

                    int index = i;
                    threads[i] = new Thread(() => { Tasks[index](); semaphore.Release();});
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

    public class DefinedInitPopulation : Population {
        List<IChromosome> _initialPopulation;
        public DefinedInitPopulation(int minSize, int maxSize, IChromosome adamChromosome, List<IChromosome> initialPopulation) : base(minSize, maxSize, adamChromosome) {
            _initialPopulation = initialPopulation;
        }

        public override void CreateInitialGeneration() {
            Generations = new List<Generation>();
            GenerationsNumber = 0;

            var chromosomes = new List<IChromosome>(_initialPopulation);
            
            CreateNewGeneration(chromosomes);
        }
    }

    public class EvolutionAgent : RuleBasedAgent {

        Dictionary<string, double> _evaCardStrength;

        public EvolutionAgent(string file) {
            if (!File.Exists(file)) {
                RunEvolution(file);
            }
            loadBestIndividual(file);
        }

        public EvolutionAgent(Dictionary<string, double> cardStrength) => _evaCardStrength = cardStrength;

        private static List<double> parseWeights(string line) => line.Split(';').ToList().ConvertAll(str => double.Parse(str));
        private void loadBestIndividual(string file) {
            List<double> weights;
            using (var reader = new StreamReader(file)) {
                string? line = reader.ReadLine();
                if (line == null)
                    throw new InvalidOperationException("File does not contains any line");

                weights = parseWeights(line);
            }

            _evaCardStrength = GetCardStrength(weights);
        }

        public static List<EvolutionAgent> LoadIndividuals(string file) {
            List<EvolutionAgent> agents = new();

            using (var reader = new StreamReader(file)) {
                string? line;
                while((line = reader.ReadLine()) != null){
                    List<double> weights = parseWeights(line);
                    agents.Add(new EvolutionAgent(GetCardStrength(weights)));
                }
            }

            return agents;
        }

        public EvolutionAgent CopyNew() => new EvolutionAgent(new Dictionary<string, double>(_evaCardStrength));

        public List<double> GetWeights() {
            var keys = _evaCardStrength.Keys.ToList();
            keys.Sort();
            var weights = new List<double>();
            for (int i = 0; i < keys.Count; i++) {
                weights.Add(_evaCardStrength[keys[i]]);
            }
            return weights;
        }

        public static Dictionary<string, double> GetCardStrength(List<double> weights) {
            var keys = CardStrength.Keys.ToList();
            keys.Sort();

            Dictionary<string, double> cardStrength = new();
            for (int i = 0; i < keys.Count; i++) {
                cardStrength.Add(keys[i], weights[i]);
            }

            return cardStrength;
        }

        public static void RunEvolution(string file) {
            RunEvolution(file, (cardStrength) => {
                List<APlayer> players = new();
                players.Add(new RandomPlayer());
                players.Add(new RuleBasedAgent());
                players.Add(new MctsAgent(200, () => new RandomPlayer()));
                players.Add(new MctsAgent(200, () => new RuleBasedAgent()));
                var evaPlayer = new EvolutionAgent(cardStrength);
                players.Add(evaPlayer);
                players.Add(new MctsAgent(200, () => new EvolutionAgent(cardStrength)));
                return (players, evaPlayer);
            });
        }

        public static void RunEvolution(string file,
                                        MyProblemFitness.CreatePlayers createPlayers,
                                        int populationSize = 24,
                                        int maxGenerations = 10,
                                        int numberGames = 10,
                                        bool makeFitnessFunctionsFromLastGeneration = false,
                                        List<EvolutionAgent>? firstPopulation = null) {
            var selection = new TournamentSelection();

            var crossover = new ArithmeticCrossover();
            var mutation = new UniformMutation(allGenesMutable: true);
            var fitness = new MyProblemFitness(createPlayers, numberGames);
            var chromosome = new FloatingPointChromosome(CardStrength.Keys.Count);
            Population population;
            if (firstPopulation == null)
                population = new Population(populationSize, populationSize, chromosome);
            else {
                if (firstPopulation.Count != populationSize)
                    throw new InvalidOperationException($"List {nameof(firstPopulation)} has different length from {nameof(populationSize)}={populationSize}");
                var initPopulation = firstPopulation.Select(agent => new FloatingPointChromosome(chromosome.Length, agent));
                population = new DefinedInitPopulation(populationSize, populationSize, chromosome, initPopulation.Cast<IChromosome>().ToList());
            }

            var reinsertion = new CopyElitistReinsertion();

            var ga = new GeneticAlgorithm(population, fitness, selection, crossover, mutation){
                Reinsertion = reinsertion
            };

            // var executor = new ThreadExecutor(maxThreads: 8);
            var executor = new ParallelTaskExecutor();
            ga.TaskExecutor = executor;

            var stats = new StreamWriter($"eva_logs/stats-{file}");
            ga.GenerationRan += delegate {
                // generate new game seed for fitness function
                fitness.InitGameSeed = RandomizationProvider.Current.GetInt(0, 100_000);

                string statText = string.Format("Generation {0}, best fitness {1}, min {2}, avg. {3}, max {4} fitness",
                    ga.GenerationsNumber,
                    ga.BestChromosome.Fitness,
                    ga.Population.CurrentGeneration.Chromosomes.Min(c => c.Fitness),
                    ga.Population.CurrentGeneration.Chromosomes.Average(c => c.Fitness),
                    ga.Population.CurrentGeneration.Chromosomes.Max(c => c.Fitness)
                );
                Console.WriteLine(statText);
                stats.WriteLine(statText);
                stats.Flush();

                // after each generation is set new opponents in the fitness functions
                if (makeFitnessFunctionsFromLastGeneration)
                    ((MyProblemFitness)ga.Fitness).createPlayers = (cardStrength) => {
                        List<APlayer> players = new();
                        var chomosomes = ga.Population.CurrentGeneration.Chromosomes.Take(5);
                        foreach (var chromosome in chomosomes) {
                            List<double> weights = chromosome.GetGenes().ToList().ConvertAll(g => (double)g.Value);
                            var _cardStrength = GetCardStrength(weights);
                            players.Add(new EvolutionAgent(_cardStrength));
                        }
                        var evolutionAgent = new EvolutionAgent(cardStrength);
                        players.Add(evolutionAgent);
                        return (players, evolutionAgent);
                    };
            };

            ga.Termination = new GenerationNumberTermination(maxGenerations);

            Console.WriteLine("GA running...");
            ga.Start();

            Console.WriteLine("Best solution found has {0} fitness.", ga.BestChromosome.Fitness);
            var weights = ga.BestChromosome.GetGenes().ToList().ConvertAll(g => (double)g.Value);
            var values = string.Join(';', weights);
            Console.WriteLine("Values: {0}", values);

            stats.Close();
            using (var writer = new StreamWriter($"eva_logs/{file}"))
                writer.WriteLine(values);

            using (var writer = new StreamWriter($"eva_logs/last-pop-{file}"))
                foreach (var individual in ga.Population.CurrentGeneration.Chromosomes)
                    writer.WriteLine(string.Join(';', individual.GetGenes().ToList().ConvertAll(g => (double)g.Value)));
        }

        protected override Card? PlayInstantOnStackCore(List<Card> stack, List<Card> availableInstantCards) {
            return PlayInstantOnStackInner(stack, availableInstantCards, 0.2);
        }

        protected override double GetCardTier(Card card, bool allowUnknown = false) {
            if (_evaCardStrength.TryGetValue(card.Name, out double tier))
                return tier;

            if (allowUnknown)
                return 1;

            throw new InvalidOperationException("Card is not in the list of known cards.");
        }
    }
}
