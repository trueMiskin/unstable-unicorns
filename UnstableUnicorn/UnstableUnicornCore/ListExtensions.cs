using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
