using System;
using System.Collections.Generic;
using System.Linq;

namespace ChainBehaviors
{
    public static class ThresholdUtils
    {
        public static bool IsSortedAscending<T>(IEnumerable<T> source) where T : IComparable<T>
        {
            if (source == null)
            {
                return false;
            }

            bool isInit = false;
            T prevValue = default;

            foreach (T item in source)
            {
                if (!isInit)
                {
                    isInit = true;
                }
                else if (item.CompareTo(prevValue) < 0)
                {
                    return false;
                }
                prevValue = item;
            }
            return true;
        }

        public static T[] Sort<T>(IEnumerable<T> source, Func<T> keySelector = null)
            => source.OrderBy((v) => v).ToArray();

        public static TSource[] Sort<TSource, TKey>(IEnumerable<TSource> source, Func<TSource, TKey> keySelector = null)
            => source.OrderBy(keySelector).ToArray();
    }
}
