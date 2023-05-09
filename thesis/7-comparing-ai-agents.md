# Comparing AI agents

First, we will compare the rule-based agent with the MCTS agents with a different number of playouts. Each game will have four random agents and two agents of the selected type (for instance, two MCTS with 100 playouts, two rule-based etc.). The reason is that the random agents play totally randomly and can end the game quickly because they can play to a specific stable and meet the win condition. This will not be frequent in the first rounds, but later in the game, when some players have 3-4 unicorns in their stable, this chance is much higher. To eliminate this effect, we changed the ratio of agent types. The win of a specific agent type is counted if either one of the agents of this type wins. All MCTS agents use the rule-based agent as a default policy. For each agent type, we played 100 games. It is a little bit small number for an accurate comparison, but more games with MCTS agents will take a lot more time. The results are shown in the table below.

| Agent type | Win rate |
|------------|----------|
| Rule-based | 93 %     |
| MCTS 100   | 69 %     |
| MCTS 200   | 75 %     |
| MCTS 400   | 76 %     |

Table: Win rate of agents. MCTS 100 means MCTS agent with 100 playouts.

Interestingly, the rule-based agent does so well. It seems to be a good opponent for testing how well the different evolutionary agents perform in the game.

Next, we will train the different evolutionary agents with different parameters.
From the previous chapter, we know it is better to have a bigger population size and a smaller number of generations. We will use the population size 20 and 200 generations if the type of agent is not MCTS. For the MCTS agents, we use only 100 generations. The number of generations should be enough, accordingly to the results from the previous chapter. The first parameter in evolution is the agent opponent type used during the fitness evaluation. The second parameter is the number of games during the fitness evaluation. This number is set to 100 for all agents except MCTS. For MCTS agents, we use ten games because the MCTS agent is much slower in evolution.
We will make these evolutions with these agent types:

- five random agents vs. one evolutionary agent
- five rule-based agents vs. one evolutionary agent
- five MCTS agents with 100 playout and random agent as default policy vs. one evolutionary agent
- five MCTS agents with 100 playout and rule-based agent as default policy vs. one evolutionary agent
- five best agents from the previous generation (for evaluation of initial population will be used rule-based agent) vs. one evolutionary agent

All these evolution runs will be repeated ten times.
For comparison, we will evaluate individuals from the last generation. We will not compare individuals by their fitnesses because it highly depends on the opponents (a random agent is a much easier opponent). For that reason, each individual will play 1000 games against the rule-based and random agent. In each game, there will be two same evolutionary individuals, and the rest will be random or rule-based agents. The reference value for two rule-based agents and four random agents is 90.9%. We will show only the best ten individuals for each test. The results are shown in the tables below.

| Agent name              | Win rate vs. rule-based |
|-------------------------|-------------------------|
| evo_rule_based-3:3      | 94.8 %                  |
| evo_random-1:2          | 94.7 %                  |
| evo_random-1:3          | 94.7 %                  |
| evo_random-1:4          | 94.7 %                  |
| evo_rule_based-4:7      | 94.5 %                  |
| evo_random-8:9          | 94.4 %                  |
| evo_random-4:14         | 94.4 %                  |
| evo_rule_based-5:9      | 94.3 %                  |
| evo_rule_based-3:13     | 94.3 %                  |
| evo_rule_based-4:2      | 94.2 %                  |

| Agent name              | Win rate vs. random |
|-------------------------|---------------------|
| evo_best_last_gen-7:1   | 46.0 %              |
| evo_rule_based-8:9      | 42.8 %              |
| evo_rule_based-5:14     | 42.7 %              |
| evo_best_last_gen-7:7   | 42.7 %              |
| evo_rule_based-5:1      | 42.6 %              |
| evo_rule_based-4:2      | 42.6 %              |
| evo_rule_based-4:3      | 42.6 %              |
| evo_rule_based-4:4      | 42.6 %              |
| evo_rule_based-4:5      | 42.6 %              |
| evo_best_last_gen-7:3   | 42.6 %              |

In the tables, the first column is the name of the agent. The name is composed of the type of agent, the evolution run number and the position of an individual in the last generation. In the population, the individuals are sorted by fitness. It also shows how accurately individuals are evaluated in the population.

From the tables above, we can see that the evolutionary agents are better than the rule-based agent. From the first table, we can see that the evolutionary agents are 3% better. This is no huge improvement, but the second table shows the improvement much better. The best evolutionary agent got a 46% win rate, and there were two individuals against four rule-based agents in the game. If the agents are equally strong, the win rate will be around 33%.

The final evolution takes the best 100 agents from previous experiments and sets them as an initial population. We will use only the last type of evolution with the five best agents from the previous generation. The population size is 100, and the number of generations is 200.

Results against random agents are improved on 95.7% (1% better). The first ten individuals have a win rate between 95%-95.7%. Results against rule-based agents are improved on 46.8% (~1% better). The spread between the first ten individuals is not high: 45.6%-46.8%.

## Conclusion

The evolutionary agents improved performance by changing the tier list of the rule-based agents. Additionally, the run speed is quick as a rule-based agent, which is a significant advantage over MCTS agents.

The second place has the rule-based agent. It is incredible that using a card tier list performs so well. Even MCTS agents have a much lower win rate. The increasing number of playouts helps, but the run time dramatically slows down. Even during the evolution, the MCTS agent does not train good individual. Maybe we would get better result with higher playout number, but the run time will be much slower.
