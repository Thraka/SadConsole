using System;
using System.Collections.Generic;
using System.Linq;

namespace SadConsole.Tests
{
    /// <summary>
    /// Series of LINQ-like extension methods that can be useful for generating unit test data.
    /// </summary>
    public static class LinqExtensions
    {
        /// <summary>
        /// Yields the value given as a single-item enumerable.
        /// </summary>
        /// <param name="value">Value to yield</param>
        /// <typeparam name="T">Type of value being yielded</typeparam>
        /// <returns>An enumerable containing only the item given.</returns>
        public static IEnumerable<T> Yield<T>(this T value)
        {
            yield return value;
        }

        /// <summary>
        /// Creates a tuple of each possible pairing of elements in the enumerables.
        /// </summary>
        /// <typeparam name="T1" />
        /// <typeparam name="T2" />
        /// <param name="l1" />
        /// <param name="l2" />
        /// <returns>Tuples containing every possible (unique) pairing of elements between the two enumerables.</returns>
        public static IEnumerable<(T1, T2)> Combinate<T1, T2>(this IEnumerable<T1> l1, IEnumerable<T2> l2)
        {
            var l2List = l2.ToArray();
            foreach (var x in l1)
                foreach (var y in l2List)
                    yield return (x, y);
        }

        /// <summary>
        /// Creates a tuple for each pairing of the tuples with the elements from <paramref name="l2" />.
        /// </summary>
        /// <typeparam name="T1" />
        /// <typeparam name="T2" />
        /// <typeparam name="T3" />
        /// <param name="tuples" />
        /// <param name="l2" />
        /// <returns>
        /// An enumerable of 3-element tuples that collectively represent every unique pairing of the initial tuples with
        /// the new values.
        /// </returns>
        public static IEnumerable<(T1, T2, T3)> Combinate<T1, T2, T3>(this IEnumerable<(T1 i1, T2 i2)> tuples,
                                                                      IEnumerable<T3> l2)
        {
            var l2List = l2.ToArray();
            foreach (var (i1, i2) in tuples)
                foreach (var y in l2List)
                    yield return (i1, i2, y);
        }

        /// <summary>
        /// Creates a tuple for each pairing of the tuples with the elements from <paramref name="l2" />.
        /// </summary>
        /// <typeparam name="T1" />
        /// <typeparam name="T2" />
        /// <typeparam name="T3" />
        /// <typeparam name="T4" />
        /// <param name="tuples" />
        /// <param name="l2" />
        /// <returns>
        /// An enumerable of 4-element tuples that collectively represent every unique pairing of the initial tuples with
        /// the new values.
        /// </returns>
        public static IEnumerable<(T1, T2, T3, T4)> Combinate<T1, T2, T3, T4>(this IEnumerable<(T1 i1, T2 i2, T3 i3)> tuples,
                                                                              IEnumerable<T4> l2)
        {
            var l2List = l2.ToArray();
            foreach (var (i1, i2, i3) in tuples)
                foreach (var y in l2List)
                    yield return (i1, i2, i3, y);
        }

        /// <summary>
        /// Creates a tuple for each pairing of the tuples with the elements from <paramref name="l2" />.
        /// </summary>
        /// <typeparam name="T1" />
        /// <typeparam name="T2" />
        /// <typeparam name="T3" />
        /// <typeparam name="T4" />
        /// <typeparam name="T5" />
        /// <param name="tuples" />
        /// <param name="l2" />
        /// <returns>
        /// An enumerable of 5-element tuples that collectively represent every unique pairing of the initial tuples with
        /// the new values.
        /// </returns>
        public static IEnumerable<(T1, T2, T3, T4, T5)> Combinate<T1, T2, T3, T4, T5>(this IEnumerable<(T1 i1, T2 i2, T3 i3, T4)> tuples,
                                                                              IEnumerable<T5> l2)
        {
            var l2List = l2.ToArray();
            foreach (var (i1, i2, i3, i4) in tuples)
                foreach (var y in l2List)
                    yield return (i1, i2, i3, i4, y);
        }

        /// <summary>
        /// Flattens an enumerable of enumerables into a single enumerable.
        /// </summary>
        /// <param name="self"/>
        /// <typeparam name="T">Type of item in the lists.</typeparam>
        /// <returns>A enumerable with all (flattened) values.</returns>
        public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> self)
        {
            return self.SelectMany(enumerable => enumerable);
        }

        /// <summary>
        /// Gets all enum values from an enum.
        /// </summary>
        /// <typeparam name="T"/>
        /// <returns/>
        public static IEnumerable<T> GetEnumValues<T>() where T : Enum
            => (T[])Enum.GetValues(typeof(T));
    }
}
