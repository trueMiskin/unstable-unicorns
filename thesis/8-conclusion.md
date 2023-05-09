# Conclusion

In this work, we have analyzed the Unstable Unicorns game. This includes game rules, game mechanics and description of successful AI agents in similar games. We have developed a game simulator that can be easily extended with new effects, cards and deck. We have implemented and tested several AI agents. We have developed a successful rule-based agent, which was further improved by an evolutionary algorithm. The huge advantage of the evolutionary agent is run time. The MCTS agent probably can be the same or better than the evolutionary agent with a higher number of playouts, but the run time is much higher for better comparison. Combining the win rate and run time, the evolutionary agent is an absolute winner.

## Future work

One way of extending this work is to implement additional AI agents. No agents use the information about known cards in opponents hand and this could improve the win rate. This card tracking is already done by the simulator.

The game has a lot of expansions. It could be interesting to analyze how the agents will perform with the new cards from expansions. Some effects of new cards are unique. It changes how the game is played.

The last one, but most interesting for human players, is implementing a graphical interface for the game. The game can be played from a console but it is not user-friendly. The game control from the console is not bad but it is hard to track the game. I think a user interface in the console for this game is hard to make. One reason is that more effects perform simultaneously, which is hard to understand. The graphical interface helps a lot to solve this problem. Additionally, it improves the game experience and understanding of the game's progress.