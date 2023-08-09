using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Health.Fhir.CodeGenCommon;

internal static class Shim
{
    public static bool StartsWith(this string value, char ch)
    {
        return value.AsSpan()[0] == ch;
    }

    public static int IndexOf(this string value, char ch, StringComparison stringComparison)
    {
        return value.IndexOf(ch);
    }

    public static bool Contains(this string value, char ch, StringComparison stringComparison)
    {
        return value.Contains(ch);
    }

    public static bool Contains(this string value, string substring, StringComparison stringComparison)
    {
        return value.Contains(substring);
    }

    public static string Replace(this string value, string oldSubstring, string replacement, StringComparison stringComparison)
    {
        return value.Replace(oldSubstring, replacement);
    }

    public static string AsSpan(this string value, int start)
        => value.Substring(start);

    public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source)
        => new HashSet<T>(source);

    public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> pair, out TKey key, out TValue value)
    {
        key = pair.Key;
        value = pair.Value;
    }
}
