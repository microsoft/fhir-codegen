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
    private static readonly char[] _wordDelimiters = new char[] { ' ', '.', '_', '-' };

    /// <summary>(Immutable) Options for controlling the word split.</summary>
    private static readonly StringSplitOptions _wordSplitOptions = StringSplitOptions.RemoveEmptyEntries & StringSplitOptions.TrimEntries;

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
    }

    /// <summary>A string extension method that converts a word to PascalCase.</summary>
    /// <param name="word">            The word to act on.</param>
    /// <param name="removeDelimiters">(Optional) True to remove delimiters.</param>
    /// <param name="joinDelimiter">   (Optional) The word delimiter to use when joining.</param>
    /// <returns>Word as a string.</returns>
    public static string ToPascalCase(this string word, bool removeDelimiters = true, string joinDelimiter = "")
    {
        if (string.IsNullOrEmpty(word))
        {
            return string.Empty;
        }

        if (removeDelimiters)
        {
            return string.Join(joinDelimiter, word.Split(_wordDelimiters, _wordSplitOptions).Select(w => w.ToPascalCase(false)));
        }

        return string.Concat(word.Substring(0, 1).ToUpperInvariant(), word.Substring(1));
    }

    /// <summary>An extension method that converts an array of words each to PascalCase.</summary>
    /// <param name="words">           The words.</param>
    /// <param name="removeDelimiters">(Optional) True to remove delimiters.</param>
    /// <param name="joinDelimiter">   (Optional) The word delimiter to use when joining.</param>
    /// <returns>Word as a string.</returns>
    public static string[] ToPascalCase(this string[] words, bool removeDelimiters = true, string joinDelimiter = "")
    {
        if (!(words?.Any() ?? false))
        {
            return Array.Empty<string>();
        }

        string[] output = new string[words.Length];

        for (int i = 0; i < words.Length; i++)
        {
            output[i] = ToPascalCase(words[i], removeDelimiters, joinDelimiter);
        }

        return output;
    }

    /// <summary>
    /// An IEnumerable&lt;string&gt; extension method that converts the words to a PascalWord.
    /// </summary>
    /// <param name="words">           The words.</param>
    /// <param name="removeDelimiters">(Optional) True to remove delimiters.</param>
    /// <param name="joinDelimiter">   (Optional) The word delimiter to use when joining.</param>
    /// <returns>Words as a string.</returns>
    public static string ToPascalCaseWord(this IEnumerable<string> words, bool removeDelimiters = true, string joinDelimiter = "")
    {
        if (!(words?.Any() ?? false))
        {
            return string.Empty;
        }

        return string.Join(joinDelimiter ?? string.Empty, words.Select(w => w.ToPascalCase(removeDelimiters, joinDelimiter)));
    }

    /// <summary>A string extension method that converts a word to a camelCase.</summary>
    /// <param name="word">            The word to act on.</param>
    /// <param name="removeDelimiters">(Optional) True to remove delimiters.</param>
    /// <param name="joinDelimiter">   (Optional) The word delimiter to use when joining.</param>
    /// <returns>Word as a string.</returns>
    public static string ToCamelCase(this string word, bool removeDelimiters = true, string joinDelimiter = "")
    {
        if (string.IsNullOrEmpty(word))
        {
            return string.Empty;
        }

        if (removeDelimiters)
        {
            // converting to pascal and changing the initial letter is faster than accumulating here
            string pc = word.ToPascalCase(removeDelimiters, joinDelimiter);
            return string.Concat(pc.Substring(0, 1).ToLowerInvariant(), pc.Substring(1));
        }

        return string.Concat(word.Substring(0, 1).ToLowerInvariant(), word.Substring(1));
    }

    /// <summary>An extension method that converts an array of words each to camelCase.</summary>
    /// <param name="words">           The words.</param>
    /// <param name="removeDelimiters">(Optional) True to remove delimiters.</param>
    /// <param name="joinDelimiter">   (Optional) The word delimiter to use when joining.</param>
    /// <returns>Word as a string.</returns>
    public static string[] ToCamelCase(this string[] words, bool removeDelimiters = true, string joinDelimiter = "")
    {
        if (!(words?.Any() ?? false))
        {
            return Array.Empty<string>();
        }

        string[] output = new string[words.Length];

        if (words.ForEach((word, index) =>
            {
                output[index] = word.ToCamelCase(removeDelimiters, joinDelimiter);
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
    /// <param name="words">           The words.</param>
    /// <param name="removeDelimiters">(Optional) True to remove delimiters.</param>
    /// <param name="joinDelimiter">   (Optional) The word delimiter to use when joining.</param>
    /// <returns>Words as a string.</returns>
    public static string ToCamelCaseWord(this IEnumerable<string> words, bool removeDelimiters = true, string joinDelimiter = "")
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
                    sb.Append(word.ToCamelCase(removeDelimiters));
                }
                else
                {
                    sb.Append(joinDelimiter);
                    sb.Append(word.ToPascalCase(removeDelimiters));
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
    public static string ToUpperCase(this string word, bool removeDelimiters = true, string joinDelimiter = "_")
    {
        if (string.IsNullOrEmpty(word))
        {
            return string.Empty;
        }

        if (removeDelimiters)
        {
            return string.Join(joinDelimiter, word.Split(_wordDelimiters, _wordSplitOptions).Select(w => w.ToUpperInvariant()));
        }

        return word.ToUpperInvariant();
    }

    /// <summary>
    /// An extension method that converts an array of words each to UPPER INVARIANT.
    /// </summary>
    /// <param name="words">           The words.</param>
    /// <param name="removeDelimiters">(Optional) True to remove delimiters.</param>
    /// <param name="joinDelimiter">   (Optional) The word delimiter to use when joining.</param>
    /// <returns>Words as a string[].</returns>
    public static string[] ToUpperCase(this string[] words, bool removeDelimiters = true, string joinDelimiter = "_")
    {
        if (!(words?.Any() ?? false))
        {
            return Array.Empty<string>();
        }

        string[] output = new string[words.Length];

        if (removeDelimiters)
        {
            if (words.ForEach((word, index) =>
            {
                output[index] = word.ToUpperCase(removeDelimiters, joinDelimiter);
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

        return words.Select(w => w.ToUpperInvariant()).ToArray() ?? Array.Empty<string>();
    }

    /// <summary>
    /// An IEnumerable&lt;string&gt; extension method that converts words a single UPPER_INVARIANT
    /// word.
    /// </summary>
    /// <param name="words">           The words.</param>
    /// <param name="removeDelimiters">(Optional) True to remove delimiters.</param>
    /// <param name="joinDelimiter">   (Optional) The word delimiter to use when joining.</param>
    /// <returns>The given data converted to a string.</returns>
    public static string ToUpperCaseWord(this IEnumerable<string> words, bool removeDelimiters = true, string joinDelimiter = "")
    {
        if (!(words?.Any() ?? false))
        {
            return string.Empty;
        }

        return string.Join(joinDelimiter ?? string.Empty, words.Select(w => w.ToUpperCase(removeDelimiters, joinDelimiter)));
    }

    /// <summary>A string extension method that converts this object to a lower case.</summary>
    /// <param name="word">            The word to act on.</param>
    /// <param name="removeDelimiters">(Optional) True to remove delimiters.</param>
    /// <param name="joinDelimiter">   (Optional) The word delimiter to use when joining.</param>
    /// <returns>The given data converted to a string.</returns>
    public static string ToLowerCase(this string word, bool removeDelimiters = true, string joinDelimiter = "_")
    {
        if (string.IsNullOrEmpty(word))
        {
            return string.Empty;
        }

        if (removeDelimiters)
        {
            return string.Join(joinDelimiter, word.Split(_wordDelimiters, _wordSplitOptions).Select(w => w.ToLowerInvariant()));
        }

        return word.ToLowerInvariant();
    }

    /// <summary>
    /// An extension method that converts an array of words each to lower_invariant.
    /// </summary>
    /// <param name="words">           The words.</param>
    /// <param name="removeDelimiters">(Optional) True to remove delimiters.</param>
    /// <param name="joinDelimiter">   (Optional) The word delimiter to use when joining.</param>
    /// <returns>Words as a string[].</returns>
    public static string[] ToLowerCase(this string[] words, bool removeDelimiters = true, string joinDelimiter = "_")
    {
        if (!(words?.Any() ?? false))
        {
            return Array.Empty<string>();
        }

        string[] output = new string[words.Length];

        if (removeDelimiters)
        {
            if (words.ForEach((word, index) =>
            {
                output[index] = word.ToLowerCase(removeDelimiters, joinDelimiter);
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

        return words.Select(w => w.ToLowerInvariant()).ToArray() ?? Array.Empty<string>();
    }

    /// <summary>
    /// An IEnumerable&lt;string&gt; extension method that converts words a single lower_invariant
    /// word.
    /// </summary>
    /// <param name="words">           The words.</param>
    /// <param name="removeDelimiters">(Optional) True to remove delimiters.</param>
    /// <param name="joinDelimiter">   (Optional) The word delimiter to use when joining.</param>
    /// <returns>The given data converted to a string.</returns>
    public static string ToLowerCaseWord(this IEnumerable<string> words, bool removeDelimiters = true, string joinDelimiter = "")
    {
        if (!(words?.Any() ?? false))
        {
            return string.Empty;
        }

        return string.Join(joinDelimiter ?? string.Empty, words.Select(w => w.ToLowerCase(removeDelimiters, joinDelimiter)));
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

        return string.Join('.', word.Split(_wordDelimiters, _wordSplitOptions).Select(w => w.ToPascalCase(false)));
    }

    /// <summary>A string extension method that converts this object to a pascal dot case.</summary>
    /// <param name="words">           The words.</param>
    /// <returns>The given data converted to a string.</returns>
    public static string[] ToPascalDotCase(this string[] words)
    {
        if (!(words?.Any() ?? false))
        {
            return Array.Empty<string>();
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

        return string.Join('.', words.Select(w => w.ToPascalDotCase()));
    }
}
