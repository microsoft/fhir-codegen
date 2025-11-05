// <copyright file="FhirNameConventionExtensions.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.Text;

namespace Microsoft.Health.Fhir.CodeGenCommon.Extensions;

/// <summary>A FHIR name convention extensions.</summary>
public static class FhirNameConventionExtensions
{
    /// <summary>(Immutable) The word delimiters.</summary>
    private static readonly char[] _wordDelimiters = [' ', '.', '_', '-'];

    /// <summary>(Immutable) Options for controlling the word split.</summary>
#if NETSTANDARD2_0
    private static readonly StringSplitOptions _wordSplitOptions = StringSplitOptions.RemoveEmptyEntries;
#else
    private static readonly StringSplitOptions _wordSplitOptions = StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries;
#endif

    /// <summary>Values that represent naming conventions for item types.</summary>
    public enum NamingConvention
    {
        /// <summary>This feature is not supported / used.</summary>
        None,

        /// <summary>An enum constant representing the language controlled option.</summary>
        LanguageControlled,

        /// <summary>Names are standard FHIR dot notation (e.g., path).</summary>
        FhirDotNotation,

        /// <summary>Names are dot notation, with each first letter capitalized.</summary>
        PascalDotNotation,

        /// <summary>Names are Pascal Case (first letter capitalized).</summary>
        PascalCase,

        /// <summary>Names are Camel Case (first letter lower case).</summary>
        CamelCase,

        /// <summary>Names are all upper case.</summary>
        UpperCase,

        /// <summary>Names are all lower case.</summary>
        LowerCase,

        /// <summary>Lower case, separated by hyphens.</summary>
        LowerKebab,

        /// <summary>Pascal case, separated by an arbitrary delimiter.</summary>
        PascalDelimited,
    }

    public static string ToConvention(this string word, NamingConvention convention, string delimiter = "") => convention switch
    {
        NamingConvention.None => word,
        NamingConvention.LanguageControlled => word,
        NamingConvention.FhirDotNotation => word.ToPascalDotCase(),
        NamingConvention.PascalDotNotation => word.ToPascalDotCase(),
        NamingConvention.PascalCase => word.ToPascalCase(),
        NamingConvention.CamelCase => word.ToCamelCase(),
        NamingConvention.UpperCase => word.ToUpperCase(),
        NamingConvention.LowerCase => word.ToLowerCase(),
        NamingConvention.LowerKebab => word.ToLowerKebabCase(),
        NamingConvention.PascalDelimited => word.ToPascalDelimited(delimiter),
        _ => word,
    };

    /// <summary>A string extension method that converts a word to PascalCase.</summary>
    /// <param name="word">            The word to act on.</param>
    /// <param name="removeDelimiters">(Optional) True to remove delimiters.</param>
    /// <param name="joinDelimiter">   (Optional) The word delimiter to use when joining.</param>
    /// <returns>Word as a string.</returns>
    public static string ToPascalCase(
        this string word,
        bool removeDelimiters = true,
        string joinDelimiter = "",
        char[]? delimitersToRemove = null)
    {
        if (string.IsNullOrEmpty(word))
        {
            return string.Empty;
        }

        if (removeDelimiters)
        {
            if (delimitersToRemove == null)
            {
                return string.Join(joinDelimiter, word.Split(_wordDelimiters, _wordSplitOptions).Select(w => w.ToPascalCase(false)));
            }

            return string.Join(joinDelimiter, word.Split(delimitersToRemove, _wordSplitOptions).Select(w => w.ToPascalCase(false)));
        }

        return string.Concat(word[..1].ToUpperInvariant(), word[1..]);
    }

    /// <summary>An extension method that converts an array of words each to PascalCase.</summary>
    /// <param name="words">           The words.</param>
    /// <param name="removeDelimiters">(Optional) True to remove delimiters.</param>
    /// <param name="joinDelimiter">   (Optional) The word delimiter to use when joining.</param>
    /// <returns>Word as a string.</returns>
    public static string[] ToPascalCase(this IEnumerable<string> words, bool removeDelimiters = true, string joinDelimiter = "")
    {
        if (!words.Any())
        {
            return [];
        }

        List<string> output = [];

        foreach (string word in words)
        {
            output.Add(ToPascalCase(word, removeDelimiters, joinDelimiter));
        }

        return output.ToArray();
    }

    /// <summary>
    /// An IEnumerable&lt;string&gt; extension method that converts the words to a PascalWord.
    /// </summary>
    /// <param name="words">             The words.</param>
    /// <param name="removeDelimiters">  (Optional) True to remove delimiters.</param>
    /// <param name="joinDelimiter">     (Optional) The word delimiter to use when joining.</param>
    /// <param name="delimitersToRemove">(Optional) The delimiters to remove.</param>
    /// <returns>Words as a string.</returns>
    public static string ToPascalCaseWord(
        this IEnumerable<string> words,
        bool removeDelimiters = true,
        string joinDelimiter = "",
        char[]? delimitersToRemove = null)
    {
        if (!words.Any())
        {
            return string.Empty;
        }

        return string.Join(joinDelimiter, words.Select(w => w.ToPascalCase(removeDelimiters, joinDelimiter, delimitersToRemove)));
    }

    /// <summary>A string extension method that converts a word to a camelCase.</summary>
    /// <param name="word">              The word to act on.</param>
    /// <param name="removeDelimiters">  (Optional) True to remove delimiters.</param>
    /// <param name="joinDelimiter">     (Optional) The word delimiter to use when joining.</param>
    /// <param name="delimitersToRemove">(Optional) The delimiters to remove.</param>
    /// <returns>Word as a string.</returns>
    public static string ToCamelCase(
        this string word,
        bool removeDelimiters = true,
        string joinDelimiter = "",
        char[]? delimitersToRemove = null)
    {
        if (string.IsNullOrEmpty(word))
        {
            return string.Empty;
        }

        if (removeDelimiters)
        {
            // converting to pascal and changing the initial letter is faster than accumulating here
            string pc = word.ToPascalCase(removeDelimiters, joinDelimiter, delimitersToRemove);
            return string.Concat(pc[..1].ToLowerInvariant(), pc[1..]);
        }

        return string.Concat(word[..1].ToLowerInvariant(), word[1..]);
    }

    /// <summary>An extension method that converts an array of words each to camelCase.</summary>
    /// <param name="words">           The words.</param>
    /// <param name="removeDelimiters">(Optional) True to remove delimiters.</param>
    /// <param name="joinDelimiter">   (Optional) The word delimiter to use when joining.</param>
    /// <returns>Word as a string.</returns>
    public static string[] ToCamelCase(
        this string[] words,
        bool removeDelimiters = true,
        string joinDelimiter = "",
        char[]? delimitersToRemove = null)
    {
        if (words.Length == 0)
        {
            return [];
        }

        string[] output = new string[words.Length];

        if (words.ForEach((word, index) =>
            {
                output[index] = word.ToCamelCase(removeDelimiters, joinDelimiter, delimitersToRemove);
                return true;
            }))
        {
            return output;
        }

        return words;
    }

    /// <summary>
    /// An IEnumerable&lt;string&gt; extension method that converts the words to a camelWord.
    /// </summary>
    /// <param name="words">             The words.</param>
    /// <param name="removeDelimiters">  (Optional) True to remove delimiters.</param>
    /// <param name="joinDelimiter">     (Optional) The word delimiter to use when joining.</param>
    /// <param name="delimitersToRemove">(Optional) The delimiters to remove.</param>
    /// <returns>Words as a string.</returns>
    public static string ToCamelCaseWord(
        this IEnumerable<string> words,
        bool removeDelimiters = true,
        string joinDelimiter = "",
        char[]? delimitersToRemove = null)
    {
        if (!(words?.Any() ?? false))
        {
            return string.Empty;
        }

        StringBuilder sb = new();

        if (words.ForEach((word, i) =>
            {
                if (i == 0)
                {
                    sb.Append(word.ToCamelCase(removeDelimiters, delimitersToRemove: delimitersToRemove));
                }
                else
                {
                    sb.Append(joinDelimiter);
                    sb.Append(word.ToPascalCase(removeDelimiters, delimitersToRemove: delimitersToRemove));
                }

                return true;
            }))
        {
            return sb.ToString();
        }

        return string.Join(joinDelimiter, words) ?? string.Empty;
    }

    /// <summary>
    /// An extension method that converts an array of words each to UPPER CASE.
    /// </summary>
    /// <param name="word">            The word to act on.</param>
    /// <param name="removeDelimiters">(Optional) True to remove delimiters.</param>
    /// <param name="joinDelimiter">   (Optional) The word delimiter to use when joining.</param>
    /// <returns>Words as a string[].</returns>
    public static string ToUpperCase(
        this string word,
        bool removeDelimiters = true,
        string joinDelimiter = "_",
        char[]? delimitersToRemove = null)
    {
        if (string.IsNullOrEmpty(word))
        {
            return string.Empty;
        }

        if (removeDelimiters)
        {
            if (delimitersToRemove == null)
            {
                return string.Join(joinDelimiter, word.Split(_wordDelimiters, _wordSplitOptions).Select(w => w.ToUpperInvariant()));
            }

            return string.Join(joinDelimiter, word.Split(delimitersToRemove, _wordSplitOptions).Select(w => w.ToUpperInvariant()));
        }

        return word.ToUpperInvariant();
    }

    /// <summary>
    /// An extension method that converts an array of words each to UPPER INVARIANT.
    /// </summary>
    /// <param name="words">             The words.</param>
    /// <param name="removeDelimiters">  (Optional) True to remove delimiters.</param>
    /// <param name="joinDelimiter">     (Optional) The word delimiter to use when joining.</param>
    /// <param name="delimitersToRemove">(Optional) The delimiters to remove.</param>
    /// <returns>Words as a string[].</returns>
    public static string[] ToUpperCase(
        this string[] words,
        bool removeDelimiters = true,
        string joinDelimiter = "_",
        char[]? delimitersToRemove = null)
    {
        if (words.Length == 0)
        {
            return [];
        }

        string[] output = new string[words.Length];

        if (removeDelimiters)
        {
            if (words.ForEach((word, index) =>
            {
                output[index] = word.ToUpperCase(removeDelimiters, joinDelimiter, delimitersToRemove);
                return true;
            }))
            {
                return output;
            }
        }
        else
        {
            if (words.ForEach((word, index) =>
            {
                output[index] = word.ToUpperInvariant();
                return true;
            }))
            {
                return output;
            }
        }

        return words.Select(w => w.ToUpperInvariant()).ToArray() ?? [];
    }

    /// <summary>
    /// An IEnumerable&lt;string&gt; extension method that converts words a single UPPER_INVARIANT
    /// word.
    /// </summary>
    /// <param name="words">             The words.</param>
    /// <param name="removeDelimiters">  (Optional) True to remove delimiters.</param>
    /// <param name="joinDelimiter">     (Optional) The word delimiter to use when joining.</param>
    /// <param name="delimitersToRemove">(Optional) The delimiters to remove.</param>
    /// <returns>The given data converted to a string.</returns>
    public static string ToUpperCaseWord(
        this IEnumerable<string> words,
        bool removeDelimiters = true,
        string joinDelimiter = "",
        char[]? delimitersToRemove = null)
    {
        if (!(words?.Any() ?? false))
        {
            return string.Empty;
        }

        return string.Join(joinDelimiter, words.Select(w => w.ToUpperCase(removeDelimiters, joinDelimiter, delimitersToRemove)));
    }

    /// <summary>A string extension method that converts this object to a lower case.</summary>
    /// <param name="word">              The word to act on.</param>
    /// <param name="removeDelimiters">  (Optional) True to remove delimiters.</param>
    /// <param name="joinDelimiter">     (Optional) The word delimiter to use when joining.</param>
    /// <param name="delimitersToRemove">(Optional) The delimiters to remove.</param>
    /// <returns>The given data converted to a string.</returns>
    public static string ToLowerCase(
        this string word,
        bool removeDelimiters = true,
        string joinDelimiter = "_",
        char[]? delimitersToRemove = null)
    {
        if (string.IsNullOrEmpty(word))
        {
            return string.Empty;
        }

        if (removeDelimiters)
        {
            if (delimitersToRemove == null)
            {
                return string.Join(joinDelimiter, word.Split(_wordDelimiters, _wordSplitOptions).Select(w => w.ToLowerInvariant()));
            }

            return string.Join(joinDelimiter, word.Split(delimitersToRemove, _wordSplitOptions).Select(w => w.ToLowerInvariant()));
        }

        return word.ToLowerInvariant();
    }

    /// <summary>
    /// An extension method that converts an array of words each to lower_invariant.
    /// </summary>
    /// <param name="words">             The words.</param>
    /// <param name="removeDelimiters">  (Optional) True to remove delimiters.</param>
    /// <param name="joinDelimiter">     (Optional) The word delimiter to use when joining.</param>
    /// <param name="delimitersToRemove">(Optional) The delimiters to remove.</param>
    /// <returns>Words as a string[].</returns>
    public static string[] ToLowerCase(
        this string[] words,
        bool removeDelimiters = true,
        string joinDelimiter = "_",
        char[]? delimitersToRemove = null)
    {
        if (words.Length == 0)
        {
            return [];
        }

        string[] output = new string[words.Length];

        if (removeDelimiters)
        {
            if (words.ForEach((word, index) =>
            {
                output[index] = word.ToLowerCase(removeDelimiters, joinDelimiter, delimitersToRemove);
                return true;
            }))
            {
                return output;
            }
        }
        else
        {
            if (words.ForEach((word, index) =>
            {
                output[index] = word.ToLowerInvariant();
                return true;
            }))
            {
                return output;
            }
        }

        return words.Select(w => w.ToLowerInvariant()).ToArray() ?? [];
    }

    /// <summary>
    /// An IEnumerable&lt;string&gt; extension method that converts words a single lower_invariant
    /// word.
    /// </summary>
    /// <param name="words">             The words.</param>
    /// <param name="removeDelimiters">  (Optional) True to remove delimiters.</param>
    /// <param name="joinDelimiter">     (Optional) The word delimiter to use when joining.</param>
    /// <param name="delimitersToRemove">(Optional) The delimiters to remove.</param>
    /// <returns>The given data converted to a string.</returns>
    public static string ToLowerCaseWord(
        this IEnumerable<string> words,
        bool removeDelimiters = true,
        string joinDelimiter = "",
        char[]? delimitersToRemove = null)
    {
        if (!(words?.Any() ?? false))
        {
            return string.Empty;
        }

        return string.Join(joinDelimiter, words.Select(w => w.ToLowerCase(removeDelimiters, joinDelimiter, delimitersToRemove)));
    }

    /// <summary>
    /// A string extension method that converts this object to a lower-kebab-case.
    /// </summary>
    /// <param name="word">            The word to act on.</param>
    /// <param name="removeDelimiters">(Optional) True to remove delimiters.</param>
    /// <returns>The given data converted to a string.</returns>
    public static string ToLowerKebabCase(this string word, bool removeDelimiters = true)
        => ToLowerCase(word, removeDelimiters, "-");

    /// <summary>A string extension method that converts this object to a lower-kebab-case.</summary>
    /// <param name="words">           The words.</param>
    /// <param name="removeDelimiters">(Optional) True to remove delimiters.</param>
    /// <returns>The given data converted to a string.</returns>
    public static string[] ToLowerKebabCase(this string[] words, bool removeDelimiters = true)
        => ToLowerCase(words, removeDelimiters, "-");

    /// <summary>
    /// An IEnumerable&lt;string&gt; extension method that converts this object to a lower-kebab-case
    /// word.
    /// </summary>
    /// <param name="words">           The words.</param>
    /// <param name="removeDelimiters">(Optional) True to remove delimiters.</param>
    /// <returns>The given data converted to a string.</returns>
    public static string ToLowerKebabCaseWord(this IEnumerable<string> words, bool removeDelimiters = true)
        => ToLowerCaseWord(words, removeDelimiters, "-");

    /// <summary>
    /// A string extension method that converts this object to a lower_snake_case.
    /// </summary>
    /// <param name="word">            The word to act on.</param>
    /// <param name="removeDelimiters">(Optional) True to remove delimiters.</param>
    /// <returns>The given data converted to a string.</returns>
    public static string ToLowerSnakeCase(this string word, bool removeDelimiters = true)
        => ToLowerCase(word, removeDelimiters, "_");

    /// <summary>A string extension method that converts this object to a lower_snake_case.</summary>
    /// <param name="words">           The words.</param>
    /// <param name="removeDelimiters">(Optional) True to remove delimiters.</param>
    /// <returns>The given data converted to a string.</returns>
    public static string[] ToLowerSnakeCase(this string[] words, bool removeDelimiters = true)
        => ToLowerCase(words, removeDelimiters, "_");

    /// <summary>
    /// An IEnumerable&lt;string&gt; extension method that converts this object to a lower_snake_case
    /// word.
    /// </summary>
    /// <param name="words">           The words.</param>
    /// <param name="removeDelimiters">(Optional) True to remove delimiters.</param>
    /// <returns>The given data converted to a string.</returns>
    public static string ToLowerSnakeCaseWord(this IEnumerable<string> words, bool removeDelimiters = true)
        => ToLowerCaseWord(words, removeDelimiters, "_");

    /// <summary>
    /// A string extension method that converts this object to a pascal dot case.
    /// </summary>
    /// <param name="word">            The word to act on.</param>
    /// <returns>The given data converted to a string.</returns>
    public static string ToPascalDotCase(this string word)
    {
        if (string.IsNullOrEmpty(word))
        {
            return string.Empty;
        }

        return string.Join(".", word.Split(_wordDelimiters, _wordSplitOptions).Select(w => w.ToPascalCase(false)));
    }

    /// <summary>A string extension method that converts this object to a pascal dot case.</summary>
    /// <param name="words">           The words.</param>
    /// <returns>The given data converted to a string.</returns>
    public static string[] ToPascalDotCase(this string[] words)
    {
        if (words.Length == 0)
        {
            return [];
        }

        string[] output = new string[words.Length];

        for (int i = 0; i < words.Length; i++)
        {
            output[i] = ToPascalDotCase(words[i]);
        }

        return output;
    }

    /// <summary>
    /// An IEnumerable&lt;string&gt; extension method that converts this object to a pascal dot case
    /// word.
    /// </summary>
    /// <param name="words">           The words.</param>
    /// <returns>The given data converted to a string.</returns>
    public static string ToPascalDotCaseWord(this IEnumerable<string> words)
    {
        if (!(words?.Any() ?? false))
        {
            return string.Empty;
        }

        return string.Join(".", words.Select(w => w.ToPascalDotCase()));
    }

    /// <summary>
    /// A string extension method that converts this object to a pascal dot case.
    /// </summary>
    /// <param name="word">            The word to act on.</param>
    /// <returns>The given data converted to a string.</returns>
    public static string ToPascalDelimited(this string word, string delimiter)
    {
        if (string.IsNullOrEmpty(word))
        {
            return string.Empty;
        }

        return string.Join(delimiter, word.Split(_wordDelimiters, _wordSplitOptions).Select(w => w.ToPascalCase(false)));
    }

    /// <summary>A string extension method that converts this object to a pascal dot case.</summary>
    /// <param name="words">           The words.</param>
    /// <returns>The given data converted to a string.</returns>
    public static string[] ToPascalDelimited(this string[] words, string delimiter)
    {
        if (words.Length == 0)
        {
            return [];
        }

        string[] output = new string[words.Length];

        for (int i = 0; i < words.Length; i++)
        {
            output[i] = ToPascalDelimited(words[i], delimiter);
        }

        return output;
    }

    /// <summary>
    /// An IEnumerable&lt;string&gt; extension method that converts this object to a pascal dot case
    /// word.
    /// </summary>
    /// <param name="words">           The words.</param>
    /// <returns>The given data converted to a string.</returns>
    public static string ToPascalDelimitedWord(this IEnumerable<string> words, string delimiter)
    {
        if (!(words?.Any() ?? false))
        {
            return string.Empty;
        }

        return string.Join(delimiter, words.Select(w => w.ToPascalDelimited(delimiter)));
    }

    /// <summary>
    /// A string extension method that converts this string to a FHIR dot case string.
    /// </summary>
    /// <param name="word">            The word to act on.</param>
    /// <returns>The given data converted to a string.</returns>
    public static string ToFhirDotCase(this string word)
    {
        if (string.IsNullOrEmpty(word))
        {
            return string.Empty;
        }

        string[] words = word.Split(_wordDelimiters, _wordSplitOptions);

        return string.Join(".", words.Take(1).Select(w => w.ToPascalCase(false)), words.Skip(1).Select(w => w.ToCamelCase()));
    }

    /// <summary>A string extension method that converts this array of strings to an array of FHIR dot-case strings.</summary>
    /// <param name="words">           The words.</param>
    /// <returns>The given data converted to a string.</returns>
    public static string[] ToFhirDotCase(this string[] words)
    {
        if (words.Length == 0)
        {
            return [];
        }

        string[] output = new string[words.Length];

        for (int i = 0; i < words.Length; i++)
        {
            output[i] = ToFhirDotCase(words[i]);
        }

        return output;
    }

    /// <summary>
    /// An IEnumerable&lt;string&gt; extension method that converts this object to a pascal dot case
    /// word.
    /// </summary>
    /// <param name="words">           The words.</param>
    /// <returns>The given data converted to a string.</returns>
    public static string ToFhirDotCaseWord(this IEnumerable<string> words)
    {
        if (!(words?.Any() ?? false))
        {
            return string.Empty;
        }

        return string.Join(".", words.Take(1).Select(w => w.ToPascalCase(false)), words.Skip(1).Select(w => w.ToCamelCase()));
    }
}
