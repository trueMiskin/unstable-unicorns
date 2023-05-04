# Artificial agents

In this thesis, we implemented four types of artificial agents for the game Unstable Unicorns. We will start with standard agent types such as random or rule-based. The random agent is a very nice baseline for other agents and comparison. It is very helpful for catching bugs during the development of the game simulator.

## Random agent

As the name suggests, the random agent plays random actions from the set of possible actions. It is very easy to implement because nearly every method provides a list of possible actions, and we can pick one of them randomly.

## Rule-based agent

The rule-based agent is a bit more complicated than the random agent. The reason is that there are a lot of different actions that we must implement. Which card should be selected to discard, and which one to destroy? Should we play a card, or should we activate an effect?

We developed a rule-based agent based on a card tier list. Nearly every card game has a tier list of cards to measure a card's power. A tier list on the Reddit website [@tierlist]. We interpret values in the S tier as zero, in the A+ tier as one and so no. Action selection is done by sorting cards by this tier list with additional changes. For example, when we need to choose a card to sacrifice then we want one of the downgrade cards first. This is easily done by changing the value of the card retrieved from the tier list. In the implementation, we used subtraction by 1000. On the other hand, we do not want to sacrifice upgrade cards, so we add 1000 to the card's value. A lot of functions are implemented similarly.

Two methods are implemented a little bit differently. The first method is a decision if we should use the optional effect of the card. This method always returns true because there are no bad optional effects. Some optional effects are not good to activate every time, but the decision of when the effect is bad or good is not so trivial.

The second method is a decision on which card to play. This method uses the tier list, but instead of playing the best card, it plays the worst card. The reason is that at the start of the game, other players will likely have some instant cards to disallow playing a good card.

All values are implemented as a float because the evolutionary agent will reuse the method implementation.

## Monte Carlo Tree Search (MCTS) agent

The Monte Carlo Tree Search (MCTS) agent is an implementation of the MCTS algorithm described earlier in this thesis. The algorithm is not complicated to implement, but integrating it into the game simulator is a little bit tricky. The main problem is making a specific single action and then storing the state after this action. The reason is that we cannot tell who will be the next player taking an action. Some agents can be skipped by the simulator because they cannot play any action. The solution is to make a helper agent to play the action and store the next state. This agent will be set to play for every player, and these agents will share information if they played the selected action and if they should store the state. There is one additional problem to mention. We cannot store directly the action that we want to play. The reason is that after the copy is made, all references in the state will be different. The solution is to store the index of the action from the list of possible actions.

The last problem was how to evaluate agents during the backpropagation step. I used the simple formula: the first player in the final game results (the best player) gets $n-1$ points where $n$ is the number of players. The second player gets $n-2$ points and so on.


Now, we show the performance of the MCTS agents that use the rule-based agents with different number of playouts as base playout strategy. In the game, there were two MCTS agents and four random agents. The win of MCTS agents was counted if one of the MCTS agents won. Each of the MCTS agents played 100 games. The results are shown in the table below.

| Number of playouts | Win rate of MCTS agents |
| ------------------ | ----------------------- |
| 100                | 69 %                    |
| 200                | 75 %                    |
| 400                | 76 %                    |
| 800                | 84 %                    |

As the table shows, the MCTS agent does better with increasing number of playouts, which is an expected behavior.

## Evolutionary agent

The last implemented agent is the evolutionary one. The core behavior of this agent is the same as the rule-based agent. The difference is that the values of the cards are not hardcoded based on some tier list. Instead, the values will be chosen by the evolutionary algorithm.

I choose the float representation of individuals. Each gene of an individual represents one card as a number between 0 and 1 (how good the specific card is). The fitness function will be evaluated this way: Each individual will play a specific number of games and the fitness of the individual is computed by this formula:
$$
\sum_{i=1}^{\text{\#games}} 2^{n - p + 1}
$$
where $n$ is the number of players and $p$ is the final position of the player in the game. The formula has this form because I want to prefer the higher positions to the lower ones. This formula says that the first place is twice as good as the second place and so on.

The selection is done by the tournament selection. The crossover is done by an arithmetic crossover. The arithmetic crossover is a simple crossover that takes the average of the parents. Finally, the mutation is done by the uniform mutation. The uniform mutation simply generates a new value for the gene.

I used the GeneticSharp library for the implementation of the evolutionary algorithm and there is a functionality that is a bit strange. If the crossover is not performed, both individuals are thrown away and they do not make children individuals. This means that the population size would be decreasing over time. However, there is a phase called `reinsertion` after a new population is finished. This reinsertion can be used to implement elitism. This reinsertion will take the best individuals from the old population and add them to the new population. This means if the chance for the crossover is 85% on average, there is 15% of elitism.

The GeneticSharp library does not evaluate the already evaluated individuals' fitness. In a typical case, this is good because it saves time but in our case, we want to evaluate the fitness of the individuals again and again. Some individuals can be lucky during the fitness evaluation and they get a good fitness. For instance, the individual can luckily win all games and then this individual will be copied to the next generation. If we evaluate all individuals no matter if they were evaluated before then the evolution will be slower but it will be more accurate for individual measurements. Theoretically, we will throw away the bad individuals and we will keep the good ones.

Then there are additional questions. The first one is how many games should be played by each individual during the fitness evaluation. This is an important question because the fitness function evaluation is the most time-consuming part of the algorithm and the more games are played the longer the algorithm will take. To find it out I run the benchmark with the different number of games and initial seeds. In the games, there were three random agents and three rule-based agents. The results are shown in the table below.

| Number of games | Initial seed | Win rate | Variance |
| --------------- | ------------ | -------- | -------- |
| 3               | 0            | 1        | 0        |
|                 | 4000         | 1        | 0        |
|                 | 6000         | 1        | 0        |
|                 | 9000         | 0.6667   | 0.2222   |
| 5               | 0            | 1        | 0        |
|                 | 4000         | 1        | 0        |
|                 | 6000         | 1        | 0        |
|                 | 9000         | 0.8      | 0.16     |
| 10              | 0            | 0.9      | 0.09     |
|                 | 4000         | 1        | 0        |
|                 | 6000         | 1        | 0        |
|                 | 9000         | 0.9      | 0.09     |
| 20              | 0            | 0.95     | 0.0475   |
|                 | 4000         | 1        | 0        |
|                 | 6000         | 1        | 0        |
|                 | 9000         | 0.95     | 0.0475   |
| 100             | 0            | 0.99     | 0.0099   |
|                 | 4000         | 0.98     | 0.0196   |
|                 | 6000         | 0.99     | 0.0099   |
|                 | 9000         | 0.99     | 0.0099   |
| 200             | 0            | 0.985    | 0.0148   |
|                 | 4000         | 0.965    | 0.0338   |
|                 | 6000         | 0.975    | 0.0244   |
|                 | 9000         | 0.99     | 0.0099   |
| 500             | 0            | 0.972    | 0.0272   |
|                 | 4000         | 0.98     | 0.0196   |
|                 | 6000         | 0.978    | 0.0215   |
|                 | 9000         | 0.98     | 0.0196   |

Table: Table shows the win rate and variance of the fitness evaluation with a different number of games and initial seeds.

It shows that with the increasing number of games, the win rate is more accurate but around 100 games, the win rate accuracy is good enough. The win rate after 100,000 games is around 98.3%. Unfortunately, this number of games will take ages with more complex agents as the MCTS agents with a lot of playouts. For this reason, I choose only ten games for the fitness evaluation with MCTS agents.

The second question, if it is better to have a smaller population and more generations or a larger population and fewer generations. The answer depends on the problem. I made tests with different population sizes. The results are shown in the figure below.

![The performance of the different population sizes. "ps" means the population size. The single line in the figure is the mean of the ten experiments. The lighter color shows the first and the third quartile. In all experiments, the evaluated individual played 200 games with five random agents.](img/population-size-and-max-generations.png){width=510px height=280px}

The figure shows that the bigger the population size and the fewer generations are better. The figure clearly shows that the population relatively quickly goes to the local optimum and then it does not change much.