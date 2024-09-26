// <copyright file="CodeGenCommonPolyfill.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

/* 
 * NOTE: This file uses `internal` access modifiers to avoid exporting the polyfill types to the assembly consumers.
 * Each project internally that wants to use this should add the file as a link to ensure consistency.
 */

#if NETSTANDARD2_0
// some functionality must be specified in CompilerServices to Polyfill without errors
namespace System.Runtime.CompilerServices
{
    internal static class RuntimeHelpers
    {
        // For a value of type System.Range to be used in an array element access expression, the following member must be present:
        public static T[] GetSubArray<T>(T[] array, System.Range range)
        {
            if (array == null)
            {
                throw new ArgumentNullException();
            }

            (int offset, int length) = range.GetOffsetAndLength(array.Length);

            if (default(T)! != null || typeof(T[]) == array.GetType()) // TODO-NULLABLE: default(T) == null warning (https://github.com/dotnet/roslyn/issues/34757)
            {
                if (length == 0)
                {
                    return Array.Empty<T>();
                }

                var dest = new T[length];
                Array.Copy(array, offset, dest, 0, length);
                return dest;
            }
            else
            {
                // The array is actually a U[] where U:T.
                T[] dest = (T[])Array.CreateInstance(array.GetType().GetElementType()!, length);
                Array.Copy(array, offset, dest, 0, length);
                return dest;
            }
        }
    }
}
#endif

namespace Microsoft.Health.Fhir.CodeGenCommon.Polyfill
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

    internal class FrozenDictionary<TKey, TValue> : Dictionary<TKey, TValue>
        where TKey : notnull
    {
        public FrozenDictionary(IEqualityComparer<TKey>? comparer = null) : base(comparer) { }
    }

    internal static class FrozenDictionaryExtensions
    {
        public static FrozenDictionary<TKey, TValue> ToFrozenDictionary<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, IEqualityComparer<TKey>? comparer = null)
            where TKey : notnull => new FrozenDictionary<TKey, TValue>(comparer);
    }

    internal static class StringExtensions
    {
        public static bool StartsWith(this string str, char value)
        {
            return string.IsNullOrEmpty(str)
                ? false
                : str[0] == value;
        }

        public static bool EndsWith(this string str, char value)
        {
            return string.IsNullOrEmpty(str)
                ? false
                : str[^1] == value;
        }

        public static bool Contains(this string str, string value, StringComparison _)
        {
            return str.Contains(value);
        }

        public static string Replace(this string str, string search, string replace, StringComparison _)
        {
            return str.Replace(search, replace);
        }

        public static string AsSpan(this string value, int start) => value.Substring(start);
    }

    internal static class StringArrayExtensions
    {
        public static string[] Split(this string str, char sep1, char sep2, StringSplitOptions options)
        {
            return str.Split(new[] { sep1, sep2 }, options);
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
