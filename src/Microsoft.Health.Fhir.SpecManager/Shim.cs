using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Microsoft.Health.Fhir.SpecManager;

public record struct DateOnly : IComparable<DateOnly>
{
    private readonly int _dayNumber;

    public DateOnly(int year, int month, int day)
    {
        _dayNumber = DayNumberFromDateTime(new DateTime(year, month, day));
    }

    private static int DayNumberFromDateTime(DateTime dt) => (int)((ulong)dt.Ticks / TimeSpan.TicksPerDay);

    public int CompareTo(DateOnly other) => _dayNumber.CompareTo(other._dayNumber);
}

internal static class Shim
{
    public static bool StartsWith(this string value, char ch)
    {
        return value.AsSpan()[0] == ch;
    }
    public static bool EndsWith(this string value, char ch)
    {
        return value.AsSpan()[^1] == ch;
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

    public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source)
        => new HashSet<T>(source);

    public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> pair, out TKey key, out TValue value)
    {
        key = pair.Key;
        value = pair.Value;
    }

    public static bool TryAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
    {
        if (!dictionary.ContainsKey(key))
        {
            dictionary.Add(key, value);
            return true;
        }
        else
        {
            return false;
        }
    }

    public static TSource FirstOrDefault<TSource>(this IEnumerable<TSource> source, TSource defaultValue)
    {
        TSource? first = source.TryGetFirst(out bool found);
        return found ? first! : defaultValue;
    }

    public static TSource FirstOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate, TSource defaultValue)
    {
        TSource? first = source.TryGetFirst(predicate, out bool found);
        return found ? first! : defaultValue;
    }

    private static TSource? TryGetFirst<TSource>(this IEnumerable<TSource> source, out bool found)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        if (source is IList<TSource> list)
        {
            if (list.Count > 0)
            {
                found = true;
                return list[0];
            }
        }
        else
        {
            using (IEnumerator<TSource> e = source.GetEnumerator())
            {
                if (e.MoveNext())
                {
                    found = true;
                    return e.Current;
                }
            }
        }

        found = false;
        return default;
    }

    private static TSource? TryGetFirst<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate, out bool found)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }
        using (IEnumerator<TSource> e = source.GetEnumerator())
        {
            if (e.MoveNext() && predicate(e.Current))
            {
                found = true;
                return e.Current;
            }
        }

        found = false;
        return default;
    }
}

public static class PathShim
{
    public static string GetRelativePath(string relativeTo, string path)
    {
        relativeTo = Path.GetFullPath(relativeTo);
        path = Path.GetFullPath(path);

        // Need to check if the roots are different- if they are we need to return the "to" path.
        if (Path.GetPathRoot(relativeTo) == Path.GetPathRoot(path))
            return path;

        var index = 0;
        var path1Segments = relativeTo.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        var path2Segments = path.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        // if path1 does not end with / it is assumed the end is not a directory
        // we will assume that is isn't a directory by ignoring the last split
        var len1 = path1Segments.Length - 1;
        var len2 = path2Segments.Length;

        // find largest common absolute path between both paths
        var min = Math.Min(len1, len2);
        while (min > index)
        {
            if (!string.Equals(path1Segments[index], path2Segments[index]))
            {
                break;
            }
            // Handle scenarios where folder and file have same name (only if os supports same name for file and directory)
            // e.g. /file/name /file/name/app
            else if ((len1 == index && len2 > index + 1) || (len1 > index && len2 == index + 1))
            {
                break;
            }
            ++index;
        }

        var result = "";

        // check if path2 ends with a non-directory separator and if path1 has the same non-directory at the end
        if (len1 + 1 == len2 && !string.IsNullOrEmpty(path1Segments[index]) &&
            string.Equals(path1Segments[index], path2Segments[index]))
        {
            return result;
        }

        for (var i = index; len2 - 1 > i; ++i)
        {
            result += path2Segments[i] + Path.DirectorySeparatorChar;
        }
        // if path2 doesn't end with an empty string it means it ended with a non-directory name, so we add it back
        if (!string.IsNullOrEmpty(path2Segments[len2 - 1]))
        {
            result += path2Segments[len2 - 1];
        }

        return result;
    }

    public static string Combine(params string[] paths) => Path.Combine(paths);

    public static string GetFileNameWithoutExtension(string path) => Path.GetFileNameWithoutExtension(path);
}
