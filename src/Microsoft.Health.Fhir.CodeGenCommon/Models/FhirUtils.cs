// <copyright file="FhirUtils.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.Collections;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Health.Fhir.CodeGenCommon.Extensions;
using static Microsoft.Health.Fhir.CodeGenCommon.Models.FhirTypeBase;
using System;

namespace Microsoft.Health.Fhir.CodeGenCommon.Models;

/// <summary>A fhir utilities.</summary>
public abstract partial class FhirUtils
{
    ///// <summary>The RegEx remove duplicate lines.</summary>
    //private const string RegexRemoveDuplicateLinesDefinition = "__+";

    ///// <summary>(Immutable) The RegEx remove duplicate whitespace.</summary>
    //private const string RegexRemoveDuplicateWhitespaceDefinition = "\\s+";

    /// <summary>The RegEx remove duplicate lines.</summary>
    private static readonly Regex _regexRemoveDuplicateLines = RegexRemoveDuplicateLines();

    /// <summary>The RegEx remove duplicate whitespace.</summary>
    private static readonly Regex _regexRemoveDuplicateWhitespace = RegexRemoveDuplicateWhitespace();

    /// <summary>The RegEx ASCII escaping.</summary>
    private static readonly Regex _regexAsciiEscaping = RegexAsciiEscapingR();

    /// <summary>(Immutable) The underscore.</summary>
    public static readonly Dictionary<char[], string> ReplacementsWithUnderscores = new(ReplacementComparer.Default)
    {
        { new char[1] { '…' }, "_ellipsis_" },
        { new char[3] { '.', '.', '.' }, "_ellipsis_" },
        { new char[1] { '’' }, "_quote_" },
        { new char[2] { '’', '’' }, "_double_quote_" },
        { new char[3] { '’', '’', '’' }, "_triple_quote_" },
        { new char[1] { '\'' }, "_quote_" },
        { new char[2] { '\'', '\'' }, "_double_quote_" },
        { new char[3] { '\'', '\'', '\'' }, "_triple_quote_" },
        { new char[1] { '=' }, "_equals_" },
        { new char[2] { '=', '=' }, "_double_equals_" },
        { new char[3] { '=', '=', '=' }, "_triple_equals_" },
        { new char[2] { '!', '=' }, "_not_equal_" },
        { new char[2] { '<', '>' }, "_not_equal_" },
        { new char[2] { '<', '=' }, "_less_or_equal_" },
        { new char[1] { '<' }, "_less_than_" },
        { new char[2] { '>', '=' }, "_greater_or_equal_" },
        { new char[1] { '>' }, "_greater_than_" },
        { new char[1] { '!' }, "_not_" },
        { new char[1] { '*' }, "_asterisk_" },
        { new char[1] { '^' }, "_power_" },
        { new char[1] { '#' }, "_number_" },
        { new char[1] { '$' }, "_dollar_" },
        { new char[1] { '%' }, "_percent_" },
        { new char[1] { '&' }, "_and_" },
        { new char[1] { '@' }, "_at_" },
        { new char[1] { '+' }, "_plus_" },
        { new char[1] { '{' }, "_" },
        { new char[1] { '}' }, "_" },
        { new char[1] { '[' }, "_" },
        { new char[1] { ']' }, "_" },
        { new char[1] { '(' }, "_" },
        { new char[1] { ')' }, "_" },
        { new char[1] { '\\' }, "_" },
        { new char[1] { '/' }, "_" },
        { new char[1] { '|' }, "_or_" },
        { new char[2] { '|', '|' }, "_or_" },
        { new char[1] { ':' }, "_" },
        { new char[1] { ';' }, "_" },
        { new char[1] { ',' }, "_" },
        { new char[1] { '°' }, "_degrees_" },
        { new char[1] { '?' }, "_question_" },
        { new char[1] { '"' }, "_quotation_" },
        { new char[2] { '"', '"' }, "_double_quotation_" },
        { new char[1] { '“' }, "_quotation_" },
        { new char[2] { '“', '“' }, "_double_quotation_" },
        { new char[1] { '”' }, "_quotation_" },
        { new char[2] { '”', '”' }, "_double_quotation_" },
        { new char[1] { ' ' }, "_" },
        { new char[1] { '-' }, "_" },
        { new char[1] { '–' }, "_" },
        { new char[1] { '—' }, "_" },
        { new char[1] { '_' }, "_" },
        { new char[1] { '.' }, "_" },
        { new char[1] { '\r' }, "" },
        { new char[1] { '\n' }, "" },
        { new char[1] { '~' }, "_tilde_" },
    };

    /// <summary>(Immutable) The pascal.</summary>
    public static readonly Dictionary<char[], string> ReplacementsPascal = new(ReplacementComparer.Default)
    {
        { new char[1] { '…' }, "Ellipsis" },
        { new char[3] { '.', '.', '.' }, "Ellipsis" },
        { new char[1] { '’' }, "Quote" },
        { new char[2] { '’', '’' }, "DoubleQuote" },
        { new char[3] { '’', '’', '’' }, "TripleQuote" },
        { new char[1] { '\'' }, "Quote" },
        { new char[2] { '\'', '\'' }, "DoubleQuote" },
        { new char[3] { '\'', '\'', '\'' }, "TripleQuote" },
        { new char[1] { '=' }, "Equals" },
        { new char[2] { '=', '=' }, "DoubleEquals" },
        { new char[3] { '=', '=', '=' }, "TripleEquals" },
        { new char[2] { '!', '=' }, "NotEqual" },
        { new char[2] { '<', '>' }, "NotEqual" },
        { new char[2] { '<', '=' }, "LessOrEqual" },
        { new char[1] { '<' }, "LessThan" },
        { new char[2] { '>', '=' }, "GreaterOrEqual" },
        { new char[1] { '>' }, "GreaterThan" },
        { new char[1] { '!' }, "Not" },
        { new char[1] { '*' }, "Asterisk" },
        { new char[1] { '^' }, "Power" },
        { new char[1] { '#' }, "Number" },
        { new char[1] { '$' }, "Dollar" },
        { new char[1] { '%' }, "Percent" },
        { new char[1] { '&' }, "And" },
        { new char[1] { '@' }, "At" },
        { new char[1] { '+' }, "Plus" },
        { new char[1] { '{' }, "_" },
        { new char[1] { '}' }, "_" },
        { new char[1] { '[' }, "_" },
        { new char[1] { ']' }, "_" },
        { new char[1] { '(' }, "_" },
        { new char[1] { ')' }, "_" },
        { new char[1] { '\\' }, "_" },
        { new char[1] { '/' }, "_" },
        { new char[1] { '|' }, "Or" },
        { new char[2] { '|', '|' }, "Or" },
        { new char[1] { ':' }, "_" },
        { new char[1] { ';' }, "_" },
        { new char[1] { ',' }, "_" },
        { new char[1] { '°' }, "Degrees" },
        { new char[1] { '?' }, "Question" },
        { new char[1] { '"' }, "Quotation" },
        { new char[2] { '"', '"' }, "DoubleQuotation" },
        { new char[1] { '“' }, "Quotation" },
        { new char[2] { '“', '“' }, "DoubleQuotation" },
        { new char[1] { '”' }, "Quotation" },
        { new char[2] { '”', '”' }, "DoubleQuotation" },
        { new char[1] { ' ' }, "_" },
        { new char[1] { '-' }, "_" },
        { new char[1] { '–' }, "_" },
        { new char[1] { '—' }, "_" },
        { new char[1] { '_' }, "_" },
        { new char[1] { '.' }, "_" },
        { new char[1] { '\r' }, "" },
        { new char[1] { '\n' }, "" },
        { new char[1] { '~' }, "Tilde" },
    };

    public static readonly Dictionary<string, string> DefinitionalResourceNames = new(StringComparer.OrdinalIgnoreCase)
    {
        { "StructureDefinition", "StructureDefinition" },
        { "OperationDefinition", "OperationDefinition" },
        { "Operation", "OperationDefinition" },
        { "SearchParameter", "SearchParameter" },
        { "ImplementationGuide", "ImplementationGuide" },
        { "CapabilityStatement", "CapabilityStatement" },
        { "CompartmentDefinition", "CompartmentDefinition" },
        { "Compartment", "CompartmentDefinition" },
        { "ConceptMap", "ConceptMap" },
        { "NamingSystem", "NamingSystem" },
        { "StructureMap", "StructureMap" },
    };

    /// <summary>A replacement comparer.</summary>
    private class ReplacementComparer : IEqualityComparer<char[]>
    {
        /// <summary>Gets the default.</summary>
        public static ReplacementComparer Default { get; } = new();

        /// <summary>Determines whether the specified objects are equal.</summary>
        /// <param name="a">The first object of type <paramref name="T" /> to compare.</param>
        /// <param name="b">The second object of type <paramref name="T" /> to compare.</param>
        /// <returns>
        /// <see langword="true" /> if the specified objects are equal; otherwise, <see langword="false" />
        /// .
        /// </returns>
        public bool Equals(char[] a, char[] b)
        {
            return StructuralComparisons.StructuralEqualityComparer.Equals(a, b);
        }

        /// <summary>Returns a hash code for the specified object.</summary>
        /// <param name="obj">The <see cref="T:System.Object" /> for which a hash code is to be returned.</param>
        /// <returns>A hash code for the specified object.</returns>
        public int GetHashCode(char[] obj)
        {
            return StructuralComparisons.StructuralEqualityComparer.GetHashCode(obj);
        }
    }

    /// <summary>Converts this object to a convention.</summary>
    /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
    /// <exception cref="ArgumentException">    Thrown when one or more arguments have unsupported or
    ///  illegal values.</exception>
    /// <param name="name">                  The value.</param>
    /// <param name="path">                  Full pathname of the file.</param>
    /// <param name="convention">            The convention.</param>
    /// <param name="concatenatePath">       (Optional) True to concatenate path.</param>
    /// <param name="concatenationDelimiter">(Optional) The concatenation delimiter.</param>
    /// <param name="reservedWords">         (Optional) The reserved words.</param>
    /// <returns>The given data converted to a string.</returns>
    public static string ToConvention(
        string name,
        string path,
        NamingConvention convention,
        bool concatenatePath = false,
        string concatenationDelimiter = "",
        HashSet<string> reservedWords = null)
    {
        string value;

        if (concatenatePath && !string.IsNullOrEmpty(path))
        {
            value = path;
        }
        else
        {
            value = string.IsNullOrEmpty(name) ? path : name;
        }

        if (string.IsNullOrEmpty(value))
        {
            throw new ArgumentNullException(nameof(name));
        }

        if (reservedWords?.Contains(value) ?? false)
        {
            value = "Fhir" + concatenationDelimiter + value;
        }

        switch (convention)
        {
            case NamingConvention.FhirDotNotation:
                {
                    return value;
                }

            case NamingConvention.PascalDotNotation:
                {
                    return value.ToPascalDotCase();
                }

            case NamingConvention.PascalCase:
                {
                    return value.ToPascalCase(true);
                }

            case NamingConvention.CamelCase:
                {
                    return value.ToCamelCase(true, concatenationDelimiter);
                }

            case NamingConvention.UpperCase:
                {
                    return value.ToUpperCase(true, concatenationDelimiter);
                }

            case NamingConvention.LowerCase:
                {
                    return value.ToLowerCase(true, concatenationDelimiter);
                }

            case NamingConvention.LowerKebab:
                {
                    return value.ToLowerKebabCase(true);
                }

            case NamingConvention.None:
                {
                    return value;
                }

            default:
                throw new ArgumentException($"Invalid Naming Convention: {convention}");
        }
    }

    /// <summary>Sanitized to convention.</summary>
    /// <param name="sanitized"> The sanitized.</param>
    /// <param name="convention">The convention.</param>
    /// <returns>A string.</returns>
    public static string SanitizedToConvention(string sanitized, NamingConvention convention)
    {
        if (string.IsNullOrEmpty(sanitized))
        {
            throw new ArgumentNullException(nameof(sanitized));
        }

        switch (convention)
        {
            case NamingConvention.FhirDotNotation:
                {
                    return sanitized.Replace('_', '.');
                }

            case NamingConvention.PascalDotNotation:
                {
                    return sanitized.ToPascalDotCase();
                }

            case NamingConvention.PascalCase:
                {
                    return sanitized.ToPascalCase(false);
                }

            case NamingConvention.CamelCase:
                {
                    return sanitized.ToCamelCase(false);
                }

            case NamingConvention.UpperCase:
                {
                    return sanitized.ToUpperCase(false);
                }

            case NamingConvention.LowerCase:
                {
                    return sanitized.ToLowerCase(false);
                }

            case NamingConvention.LowerKebab:
                {
                    return sanitized.ToLowerKebabCase(false);
                }

            case NamingConvention.None:
            default:
                throw new ArgumentException($"Invalid Naming Convention: {convention}");
        }
    }

    /// <summary>Sanitize for quoted.</summary>
    /// <param name="escapeToHtml">      (Optional) The input.</param>
    /// <param name="condenseWhitespace">(Optional) True to condense whitespace.</param>
    /// <returns>A string.</returns>
    public static string SanitizeForQuoted(
        string input,
        bool escapeToHtml = false,
        bool condenseWhitespace = false)
    {
        if (string.IsNullOrEmpty(input))
        {
            return string.Empty;
        }

        if (escapeToHtml)
        {
            input = input.Replace("\"", "&quot;")
                .Replace("\r\n", "<br />")
                .Replace("\r", "<br />")
                .Replace("\n", "<br />");
        }
        else
        {
            input = input.Replace("\"", "\\\"")
                .Replace("\r", "\\r")
                .Replace("\n", "\\n");
        }

        if (condenseWhitespace)
        {
            input = _regexRemoveDuplicateWhitespace.Replace(input, " ");
        }

        return input;
    }

    /// <summary>Sanitize for code.</summary>
    /// <param name="input">        The input.</param>
    /// <param name="reservedWords">The reserved words.</param>
    /// <param name="name">         [out] The sanitized name for the code.</param>
    /// <param name="value">        [out] The sanitized value for the code.</param>
    public static void SanitizeForCode(
        string input,
        HashSet<string> reservedWords,
        out string name,
        out string value)
    {
        if (string.IsNullOrEmpty(input))
        {
            name = string.Empty;
            value = string.Empty;
            return;
        }

        name = input.Trim();

        if (name.Contains(' ', StringComparison.Ordinal))
        {
            name = name.Substring(0, name.IndexOf(' '));
        }

        value = name;
        name = SanitizeForProperty(name, reservedWords);
    }

    /// <summary>Sanitize for value.</summary>
    /// <param name="input">The input.</param>
    /// <returns>A string.</returns>
    public static string SanitizeForValue(string input)
    {
        // A very specific clean up for encoding bugs in the current R5 release
        input = input switch
        {
            "SC#o TomC) and PrC-ncipe dobra" => "São Tomé and Príncipe dobra",
            "Icelandic krC3na" => "Icelandic króna",
            "Mongolian tC6grC6g" => "Mongolian tögrög",
            "Nicaraguan cC3rdoba" => "Nicaraguan córdoba",
            "Polish zEoty" => "Polish złoty",
            "Paraguayan guaranC-" => "Paraguayan guaraní",
            "Salvadoran colC3n" => "Salvadoran colón",
            "Tongan paJ;anga" => "Tongan paʻanga",
            "Venezuelan bolC-var" => "Venezuelan bolívar",
            "Vietnamese D#a;%ng" => "Vietnamese dồng",
            _ => input
        };

        if (string.IsNullOrEmpty(input))
        {
            return string.Empty;
        }

        string value = input.Trim();
        value = value.Replace("\"", "\\\"");

        value = value.Replace("\n", string.Empty);

        return value;
    }

    /// <summary>Requires alpha.</summary>
    /// <param name="value">The value.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    private static bool RequiresAlpha(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return true;
        }

        if (value[0] == '_')
        {
            return true;
        }

        if (!char.IsLetter(value[0]))
        {
            return true;
        }

        return false;
    }

    /// <summary>Sanitize to Printable ASCII.</summary>
    /// <param name="value">The value.</param>
    /// <returns>A string.</returns>
    public static string SanitizeToAscii(string value)
    {
        return _regexAsciiEscaping.Replace(value, string.Empty);
    }

    /// <summary>Sanitize for property.</summary>
    /// <param name="value">        The value.</param>
    /// <param name="reservedWords">The reserved words.</param>
    /// <param name="toConvention"> (Optional) To convention.</param>
    /// <param name="replacements"> (Optional) The replacements.</param>
    /// <param name="checkForGmt">  (Optional) True to check for GMT.</param>
    /// <returns>A string.</returns>
    public static string SanitizeForProperty(
        string value,
        HashSet<string> reservedWords,
        NamingConvention convertToConvention = NamingConvention.None,
        Dictionary<char[], string> replacements = null,
        bool checkForGmt = false)
    {
        if (string.IsNullOrEmpty(value))
        {
            return ToConvention("None", string.Empty, convertToConvention);
        }

        if (value.StartsWith("http://hl7.org/fhir/", StringComparison.Ordinal))
        {
            value = string.Concat("FHIR_", value.AsSpan(20));
        }
        else if (value.StartsWith("http://hl7.org/fhirpath/", StringComparison.Ordinal))
        {
            value = string.Concat("FHIRPath_", value.AsSpan(24));
        }
        else if (value.StartsWith("http://terminology.hl7.org/", StringComparison.Ordinal))
        {
            value = string.Concat("THO_", value.AsSpan(27));
        }
        else if (value.StartsWith("http://hl7.org/", StringComparison.Ordinal))
        {
            value = string.Concat("HL7_", value.AsSpan(15));
        }
        else if (value.StartsWith("https://"))
        {
            value = value.Substring(8);
        }
        else if (value.StartsWith("http://"))
        {
            value = value.Substring(7);
        }
        else if (value.StartsWith("urn:oid:"))
        {
            value = string.Concat("OID_", value.AsSpan(8));
        }
        else if (value.StartsWith("urn:uuid:"))
        {
            value = string.Concat("UUID_", value.AsSpan(9));
        }
        else if (value.StartsWith('/'))
        {
            value = string.Concat("Per", value.AsSpan(1));
        }

        if (checkForGmt)
        {
            if (value.Contains("/GMT-", StringComparison.Ordinal))
            {
                value = value.Replace("/GMT-", "/GMTMinus", StringComparison.Ordinal);
            }
        }

        replacements ??= ReplacementsWithUnderscores;

        char[] chars = value.Normalize(NormalizationForm.FormD).ToCharArray();

        StringBuilder sb = new StringBuilder();

        int charsLen = chars.Length;

        for (int i = 0; i < charsLen; i++)
        {
            char[] v;
            char ch = chars[i];

            if (i + 2 < charsLen)
            {
                v = new char[3] { ch, chars[i + 1], chars[i + 2] };
                if (replacements.ContainsKey(v))
                {
                    sb.Append(replacements[v]);
                    i += 2;                 // skip two *additional* characters
                    continue;
                }
            }

            if (i + 1 < charsLen)
            {
                v = new char[2] { ch, chars[i + 1] };
                if (replacements.ContainsKey(v))
                {
                    sb.Append(replacements[v]);
                    i += 1;                // skip one *additional* character
                    continue;
                }
            }

            v = new char[1] { ch };
            if (replacements.ContainsKey(v))
            {
                sb.Append(replacements[v]);
                continue;
            }

            UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(ch);

            switch (uc)
            {
                case UnicodeCategory.UppercaseLetter:
                case UnicodeCategory.LowercaseLetter:
                case UnicodeCategory.TitlecaseLetter:
                case UnicodeCategory.DecimalDigitNumber:
                    sb.Append(ch);
                    break;

                //case UnicodeCategory.ModifierLetter:
                //case UnicodeCategory.OtherLetter:
                //case UnicodeCategory.NonSpacingMark:
                //case UnicodeCategory.SpacingCombiningMark:
                //case UnicodeCategory.EnclosingMark:
                //case UnicodeCategory.LetterNumber:
                //case UnicodeCategory.OtherNumber:
                //case UnicodeCategory.SpaceSeparator:
                //case UnicodeCategory.LineSeparator:
                //case UnicodeCategory.ParagraphSeparator:
                //case UnicodeCategory.Control:
                //case UnicodeCategory.Format:
                //case UnicodeCategory.Surrogate:
                //case UnicodeCategory.PrivateUse:
                //case UnicodeCategory.ConnectorPunctuation:
                //case UnicodeCategory.DashPunctuation:
                //case UnicodeCategory.OpenPunctuation:
                //case UnicodeCategory.ClosePunctuation:
                //case UnicodeCategory.InitialQuotePunctuation:
                //case UnicodeCategory.FinalQuotePunctuation:
                //case UnicodeCategory.OtherPunctuation:
                //case UnicodeCategory.MathSymbol:
                //case UnicodeCategory.CurrencySymbol:
                //case UnicodeCategory.ModifierSymbol:
                //case UnicodeCategory.OtherSymbol:
                //case UnicodeCategory.OtherNotAssigned:
                default:
                    break;
            }
        }

        value = sb.ToString();

        // remove duplicate underscores caused by prior replacements
        value = _regexRemoveDuplicateLines.Replace(value, "_");

        while (value.StartsWith("_", StringComparison.Ordinal))
        {
            value = value.Substring(1);
        }

        while (value.EndsWith("_", StringComparison.Ordinal))
        {
            value = value.Remove(value.Length - 1);
        }

        if (convertToConvention != NamingConvention.None)
        {
            value = ToConvention(value, string.Empty, convertToConvention);
        }

        // need to check for all digits or underscores, or reserved word
        if (RequiresAlpha(value) ||
            reservedWords != null && reservedWords.Contains(value))
        {
            if (value[0] == '_')
            {
                return $"VAL{value}";
            }

            return $"VAL_{value}";
        }

        return value;
    }

    [GeneratedRegex("__+")]
    private static partial Regex RegexRemoveDuplicateLines();

    [GeneratedRegex("\\s+")]
    private static partial Regex RegexRemoveDuplicateWhitespace();

    [GeneratedRegex("[^ -~]+")]
    private static partial Regex RegexAsciiEscapingR();
}
