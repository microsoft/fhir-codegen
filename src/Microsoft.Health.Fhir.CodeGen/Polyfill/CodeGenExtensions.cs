using System;
using System.Collections.Generic;
using System.Text;
using Antlr4.Runtime.Atn;

namespace Microsoft.Health.Fhir.CodeGen.Polyfill
{
    internal static class LiftedExtensions
    {
        /// <summary>Indicates whether a character is categorized as an ASCII letter.</summary>
        /// <param name="c">The character to evaluate.</param>
        /// <returns>true if <paramref name="c"/> is an ASCII letter; otherwise, false.</returns>
        /// <remarks>
        /// This determines whether the character is in the range 'A' through 'Z', inclusive,
        /// or 'a' through 'z', inclusive.
        /// </remarks>
        public static bool IsAsciiLetter(this char c)
        {
            return (uint)((c | 0x20) - 'a') <= 'z' - 'a';
        }
    }

#if NETSTANDARD2_0
    internal static class KeyValuePairExtensions
    {
        public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> kvp, out TKey key, out TValue value)
        {
            key = kvp.Key;
            value = kvp.Value;
        }
    }

    internal static class EnumerableExtensions
    {
        public static IOrderedEnumerable<T> Order<T>(this IEnumerable<T> val)
        {
            return val.OrderBy(v => v);
        }

        public static IOrderedEnumerable<T> Order<T>(this IEnumerable<T> val, IComparer<T> comparer)
        {
            return val.OrderBy(v => v, comparer);
        }

        public static IOrderedEnumerable<T> OrderDescending<T>(this IEnumerable<T> val)
        {
            return val.OrderByDescending(v => v);
        }
    }

    internal static class StringExtensions
    {
        public static bool StartsWith(this string str, char value)
        {
            return str.StartsWith(value.ToString());
        }

        public static bool EndsWith(this string str, char value)
        {
            return str.EndsWith(value.ToString());
        }

        public static bool Contains(this string str, string value, StringComparison _)
        {
            return str.Contains(value);
        }

        public static string Replace(this string str, string search, string replace, StringComparison _)
        {
            return str.Replace(search, replace);
        }
    }

    internal static class CollectionExtensions
    {
        public static bool TryAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (dictionary.ContainsKey(key))
            {
                return false;
            }

            dictionary.Add(key, value);
            return true;
        }

        public static TValue? GetValueOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key) =>
            dictionary.GetValueOrDefault(key, default!);

        public static TValue GetValueOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
        {
            return dictionary.TryGetValue(key, out TValue? value) ? value : defaultValue;
        }
    }
#endif
}
