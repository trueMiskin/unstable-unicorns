# Comparing AI agents

Firstly, we will compare the Rule-based agent with the Mcts agents with a different number of playouts. In each game will be 4 random agents and 2 agents of the same type. The reason is that the random agents play totally randomly and they can end the game quickly because they can play to a specific stable and meet the win condition. In the first rounds, this will not be frequent but in the late game when some players have 3-4 unicorns in their stable, then this chance is a lot higher. For eliminating this effect, we changed the ratio of agent types. The win of a specific agent type is counted if either one of the agents of this type wins. All Mcts agents used the rule-based agent as a default policy. For each agent type, we played TODO:XX games and the results are shown in the table below.

TODO: add table with results and how many games were played? 1000 games enough?? more than 10_000 will take a lot of time

| Agent type | Win rate |
|------------|----------|
| Rule-based | XX %     |
| Mcts 100   | XX %     |
| Mcts 200   | XX %     |
| Mcts 400   | XX %     |

TODO: comment on the results

Next, we will train the different evolutionary agents with different parameters.
From the previous chapter, we know that it is better to have a bigger population size and a smaller number of generations. We will use the population size TODO:20-25-30?? and 5 generations. The number of generations is quite small but it should be enough according to the results from the previous chapter. The first parameter is the agent types used during the fitness evaluation. The second parameter is the number of games during the fitness evaluation.
We will make these evolutions with these agents (mcts agents use the rule-based agent as a default policy):

- 5 random agents vs 1 evolutionary agent
- 5 rule-based agents vs 1 evolutionary agent
- 5 mcts agents with 100 playout vs 1 evolutionary agent
- 5 mcts agents with 200 playout vs 1 evolutionary agent
- 5 best agents from the previous generation (in the first generation it will be a rule-based one) vs 1 evolutionary agent

TODO: how to compare the agents? with random agents?

Then we will take the best $k$ (TODO: maybe 10-20?) agents from each previous experiment and put them together as an initial population.

TODO: some other experiments??