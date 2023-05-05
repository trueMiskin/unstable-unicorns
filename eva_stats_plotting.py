from collections import namedtuple
import glob
import argparse
import re

import pandas as pd
import numpy as np
import matplotlib.pyplot as plt

# a tuple of fitness value and objective value
FitObjPair = namedtuple('FitObjPair', ['fitness', 'objective'])

# can plot a single experiment from the statistical information
# Arguments:
#   evals - number of evalutions
#   lower - lower boundary of shaded area
#   mean - the main line in the plot
#   upper - upper boundary of shaded area
#   legend_name - name in the plot legend
def plot_experiment(evals, lower, mean, upper, legend_name=''):
    plt.plot(evals, mean, label=legend_name)
    plt.fill_between(evals, lower, upper, alpha=0.25)

# reads the run logs and computes experiment statisticts (those used for plots)
# Arguments:
#   prefix - directory with experiments
#   exp_id - name of the experiment
#   stat_type - either 'objective' or 'fitness', used to specify which values
#               to use for the stats
def get_experiment_stats(prefix, exp_id):
    data = []
    for fn in glob.glob(f'{prefix}/stats-{exp_id}-*'):
        evals, stats = read_run_file(fn)
        data.append(pd.Series([s.max for s in stats], index=evals))
    data_frame = pd.DataFrame(data)
    data_frame.fillna(method='ffill', inplace=True, axis=1)
    return (data_frame.columns.values, np.min(data_frame, axis=0),
            np.percentile(data_frame, q=25, axis=0),
            np.mean(data_frame, axis=0),
            np.percentile(data_frame, q=75, axis=0),
            np.max(data_frame, axis=0))

# the same as get_experiment_stats, but returns only some of the data
def get_plot_data(prefix, exp_id):
    evals, lower, q25, mean, q75, upper = get_experiment_stats(prefix, exp_id)
    return evals, q25, mean, q75

# a tuple for the stats about a single generation
GenStats = namedtuple('GenStats', ['min', 'max', 'mean'])

# reads one file with log of a single run
def read_run_file(filename):
    stats = []
    evals = []
    match = re.match('.*-ps=(\d+).*', filename)
    pop_size = int(match.group(1)) if match else 1

    with open(filename) as f:
        print(filename)
        for line in f.readlines():
            # Generation 1, best fitness 608, min 444, avg. 529, max 608 fitness
            line_segments = line.split()
            g, x, m, n = line_segments[1][:-1], line_segments[10], line_segments[8][:-1], line_segments[6][:-1]
            #evals.append(int(g) * pop_size)
            evals.append(int(g))
            stats.append(GenStats(min=float(n), max=float(x), mean=float(m)))

    return evals, stats

# plots a number of experiments
# Arguments:
#   prefix - directory with experimental results
#   exp_ids - list of experiments to plot
#   rename_dict - a mapping of exp_id -> legend name, can be used to rename
#                 entries in the plot legend
def plot_experiments(prefix, exp_ids, rename_dict=None):
    if not rename_dict:
        rename_dict = dict()
    for e in exp_ids:
        evals, lower, mean, upper = get_plot_data(prefix, e)
        plot_experiment(evals, lower, mean, upper, rename_dict.get(e, e))
    plt.legend()
    plt.xlabel('Generations')
    plt.ylabel('Fitness value')


if __name__ == '__main__':
    parser = argparse.ArgumentParser()
    parser.add_argument('directory', type=str, help='Directory with experiments')
    parser.add_argument('exp_ids', type=str, nargs='+', help='list of experiments to plot, a file name prefix can be specified')

    args = parser.parse_args()
    plt.figure(figsize=(12,8))
    plot_experiments(args.directory, args.exp_ids)
    plt.show()
