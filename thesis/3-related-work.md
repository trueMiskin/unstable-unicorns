# Related work

## Properties of task environments

Before we can compare our game with other similar games we must analyze the properties of the game environment. The properties of the environment are
well described in this book [@j2010artificial] in the second chapter.
In the following section, I will briefly describe the properties of the Unstable Unicorns game.

The game is partially observable. Players can see their cards, the
discard pile and the board state (unicorns/upgrades/downgrades in each stable).
They normally cannot see other players' cards in hand and the order of cards in the draw pile.

The game is a multi-agent environment. The game can be played with 2-8 players.

The game is stochastic. The cards themselves are deterministic -- all effects 
have no random components. However, the order of cards in the draw pile is random.

The game is sequential. The next game state is determined by the previous game
state and the action of the players.

The game is dynamic. When a player announces he wants to play a given card other
players can react to this card. Each player independently decides whether to react
or not.

The game is discrete. In the game, there is a finite number of cards and
each time an integer number of cards can be played. It is not possible to play
a fraction of a card. This is also true for the effects of cards.

The game theoretically can be infinite, but in practice, the game ends after
at most hundreds of turns.

## Artificial intelligence in similar games

A very popular and similar game is Magic: The Gathering[@mtg]. Magic: The Gathering is a
collectible card game where players buy booster packs, build their decks and play
against each other. The game rules are not too hard but the game complexity
grows with the different effects of cards and the combinations of cards.
After years of existence, there are a lot of decks that break the game balance
(for instance infinite combo decks[^infinite]). Every year, the game developers
release new sets of cards, and it is impossible to balance the game.
Therefore, there exists a lot of formats that allow using only
selected sets of cards.
Sometimes, some cards are too strong and are banned for the format.
The game has a lot of competitive tournaments for big prizes.

[^infinite]: [https://tappedout.net/mtg-decks/selesnya-infinite-elf-tokens/](https://tappedout.net/mtg-decks/selesnya-infinite-elf-tokens/)

In [-@2009mctsmtg], Monte Carlo Tree Search was applied to use to play the game [@2009mctsmtg].
Results showed that the algorithm can beat the strong rule-based agent that
was designed by an expert player of Magic: The Gathering.

Another well-known card game is Poker. Poker is a game for multiple players
where each player knows their two cards and the cards on the table. The
goal is to have the best combination of cards. This game is based on evaluating the probability of the opponent's card strength combination and
the probability of the player's card strength combination. Another aspect
of the game is psychological. The players can bluff and bet to confuse
the opponent. Some people can think that the game is just gambling and
the game is based on luck. However, a strategy was developed that wins the
game on average [@moravvcik2017deepstack] (not all games can be won).
This is a proof that the game is not just gambling.

The last game with slightly different environmental properties is Go game. Go is a game for two players, turn-based, deterministic with perfect
information. The game is challenging because of the huge search space.
In [-@silver2016mastering], the DeepMind team developed an algorithm[@silver2016mastering]
based on Monte Carlo Tree Search with deep neural networks, which became
very successful in Go. The algorithm was able to beat the
best human players.

## Evolutionary algorithms

Evolutionary algorithms[@eiben2015introduction] are nature-inspired algorithms. The idea is
to mimic Darwin's theory of evolution. The individuals compete to reproduce
their genes for the next generation.

\begin{algorithm}
\begin{algorithmic}
\Function{EvolutionaryAlgorithm}{}
    \State Initialize the population
    \State Evaluate the fitness of each individual
    \While{Until the stopping criterion is not met}
        \State Select the parents
        \State Crossover
        \State Mutation
        \State Evaluate the fitness of each individual
        \State Offspring is the new population
    \EndWhile
\EndFunction
\end{algorithmic}
\caption{Pseudocode of the evolutionary algorithm}
\label{alg:w}
\end{algorithm}

The whole algorithm is based on individuals in the population.
The individual is the solution to the problem.
In this section, the individual is a bit string. Other representations
of the individual will be used later.

Most of the time, the initial population is randomly generated.
It is possible to use some smart initialization but we must do it
very carefully, otherwise, we can get stuck in a local optimum.

The next step is to evaluate the fitness of each individual. The fitness function evaluates how good is the individual as a solution to the problem. The fitness function depends on the problem and the computing of the fitness function should be as quick as possible. Most of the time, the bottleneck of the algorithm is the fitness function.

The next step is selecting the parents. The selection can be done in many ways. The most common approaches are roulette wheel selection and tournament selection. In the roulette wheel, selection each individual is assigned an arc on the wheel. The arc is proportional to the fitness of the individual in the population. Then we are just spinning the wheel and selecting the individual. The probability of the individual $x$ is:
$$
\frac{f(x)}{\sum_i{f(i)}}
$$
where sum iterates over whole population and $f$ is fitness function.
In the tournament selection, we select randomly $k$ individuals and the best individual is selected as a parent. Both approaches have their advantages and disadvantages. The roulette wheel can have stronger selection pressure when the fitnesses are more different but when the fitnesses are similar, the roulette wheel performs as random selection. The tournament has the same selection pressure during the whole evolution. Another benefit is that the tournament selection can be easily switched to minimization instead maximization of the problem.

Another step is crossover. The crossover combines the genes of the parents to create a new individual. The crossover can be done in many ways too. The most common crossovers are one-point crossover and uniform crossover. The one-point crossover combines two parents and produces two offspring. The crossover randomly selects a point and the first offspring gets the genes from the first parent up to the point and the genes from the second parent after the point. The second offspring gets the other part of the genes. The uniform crossover randomly chooses for every gene from which parent it will be taken.

The last step is mutation. The mutation randomly changes offspring's genes. The mutation is mostly for the exploration of the search space and escaping from local optima. The most common mutation is bit-flip and uniform mutation. The bit-flip mutation randomly decides whether the gene will be flipped. The uniform mutation is mostly same as bit-flip. The only difference is that the uniform mutation randomly generates values from the allowed subset or interval of the gene.

Then offspring are evaluated and the new population replaces the old population.
This process is repeated until we reach the stopping criterion. For instance: the maximum number of generations reached, the best individual has lower fitness than some constant, or the fitnesses did not improve after some number of generations.

## Monte Carlo Tree Search

Monte Carlo Tree Search[@chaslot2010monte] is a search technique that combines a tree search algorithm and machine learning principles (the algorithm tries to exploit information that it detained so far). It solves the problem of exploration and exploitation. The algorithm expands a node that is most promising (best-first search).

The algorithm itself is quite simple and has four main steps: Selection, Expansion, Simulation and Backpropagation.

- Selection: The algorithm starts from the root node and traverses the tree until it reaches a leaf node. A leaf node is a node that has a child node that is not expanded yet. The selection of the next child node (successor) is done by UCT formula (Upper Confidence Bound applied to Trees) which is based on the UCB1 formula.
The UCT formula is following:
$$
\text{successor}
=
{\arg\max}_{s \in \text{successors}} \left(
    \frac{w_s}{n_s} + c \cdot\sqrt{\frac{\log n_{\text{parent}}}{n_s}}
\right)
$$
where $w_s$ is the number of wins after the successor state, $n_s$ is the number of playouts after the successor state, $n_{\text{parent}}$ is the number of playouts after the parent node of successors and $c$ is a constant that controls the exploration-exploitation trade-off. The constant $c$ is usually set to $\sqrt{2} \approx 1.41$.
- Expansion: The algorithm randomly selects an unexpanded successor of a selected leaf node and expands it.
- Simulation (Playout): The algorithm plays one game from the expanded node state until the end of the game. Moves in simulation are chosen by a given base strategy (random, greedy, etc.).
- Backpropagation: The algorithm updates the statistics of the expanded node and its ancestors.

The algorithm is repeated for a given number of playouts. The next move (the successor of the root node) is the one with the highest number of playouts (not the most win rate).
