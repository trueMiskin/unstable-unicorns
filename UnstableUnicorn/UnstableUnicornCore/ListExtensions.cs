/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */
﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace UnstableUnicornCore {
    public static class ListExtensions {
        /// <summary>
        /// Implementation of Fisher–Yates shuffle as helper method for lists
        /// 
        /// This implementation doesn't not shuffle inplace
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="rng">Random generator to be used for shuffle</param>
        /// <returns>Shuffled list</returns>
        public static List<T> Shuffle<T>(this IList<T> list, Random rng) {
            List<T> result = new List<T>( list );
            for (int n = result.Count - 1; n > 0; n--) {
                int k = rng.Next(n);
                T value = result[k];
                result[k] = result[n];
                result[n] = value;
            }
            return result;
        }

        /// <summary>
        /// Generate subsets of given length
        /// 
        /// Slightly edited: https://stackoverflow.com/questions/36328825/c-sharp-get-all-combinations-of-defined-length-or-less-from-a-list-of-object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objects"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static IEnumerable<List<T>> Subsets<T>(this List<T> objects, int length) {
            if (objects == null || length <= 0)
                yield break;
            var stack = new Stack<int>(length);
            int i = 0;
            while (stack.Count > 0 || i < objects.Count) {
                if (i < objects.Count) {
                    if (stack.Count == length)
                        i = stack.Pop() + 1;
                    stack.Push(i++);

                    if (stack.Count == length)
                        yield return (from index in stack.Reverse()
                                      select objects[index]).ToList();
                } else {
                    i = stack.Pop() + 1;
                    if (stack.Count > 0)
                        i = stack.Pop() + 1;
                }
            }
        }

        /// <summary>
        /// Returns a specified number of randomly selected elements from the list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="availableSelection"></param>
        /// <param name="rng">Random generator to be used for selection</param>
        /// <param name="count">Number of selected elements</param>
        /// <returns></returns>
        public static List<T> RandomSelection<T>(this List<T> availableSelection, Random rng, int count) {
            return Enumerable.Range(0, availableSelection.Count)
                .ToList().Shuffle(rng)
                .Take(count)
                .Select(i => availableSelection[i])
                .ToList();
        }
    }
}
