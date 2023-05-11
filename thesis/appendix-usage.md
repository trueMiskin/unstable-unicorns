# Usage

When we run the application, we must specify at least one command.
Some commands require additional arguments or subcommands. Of course, there
are some optional arguments. The application has a help command.

## Commands

- `game` - run the game. The game can be played by AI agents and one agent can be controlled by a human player.
- `variance-benchmark` - run the variance benchmark shown in chapter 5
- `mcts-agent-tests` - run the MCTS agent tests shown in chapter 5
    - all games (in json format) are stored in the `mcts_agent_tests` directory
- `evolution` or `evo` - run the evolution. This command has a lot of subcommands. All commands are described help of this command. The evolution stores results in the `eva_logs` directory. **This directory is not created automatically.** For every run, three files will be stored in the directory:
    - file starting with `stats` - contains statistics about the evolution
    - file starting with `last` - contains the last population
    - file that contains the best agent
- `evo-evaluate` - evaluate the evolution results and print sorted results

## Plotting evolution results

For plotting evolution results, we use the `eva_stats_plotting.py` script. This script requires the `matplotlib`, `numpy` and `pandas` packages. The script has two arguments:

- `directory` - directory with evolution results
- `exp_ids` - list of experiments to plot, a file name prefix can be specified

## View game log

For viewing the game log (json file), we use the `logs_viewer.html` html file. This log file is created, for example, when we run `mcts-agent-tests`. Then we select a log file and the log is shown
in the browser. By the left and right arrow keys, we can move between turns. By the enter, we can show/hide the game summary. 