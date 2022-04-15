// <copyright file="FhirUtils.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

using static Microsoft.Health.Fhir.CodeGenCommon.Models.FhirTypeBase;

namespace Microsoft.Health.Fhir.CodeGenCommon.Models;

/// <summary>A fhir utilities.</summary>
public abstract class FhirUtils
{
    /// <summary>The RegEx remove duplicate lines.</summary>
    private const string _regexRemoveDuplicateLinesDefinition = "__+";

    /// <summary>The RegEx remove duplicate lines.</summary>
    private static Regex _regexRemoveDuplicateLines = new Regex(_regexRemoveDuplicateLinesDefinition);

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
        string value = name;

        if (concatenatePath && (!string.IsNullOrEmpty(path)))
        {
            value = path;
        }

        if (string.IsNullOrEmpty(value))
        {
            throw new ArgumentNullException(nameof(name));
        }

        switch (convention)
        {
            case NamingConvention.FhirDotNotation:
                {
                    if ((reservedWords != null) && reservedWords.Contains(value))
                    {
                        int i = value.LastIndexOf('.');
                        if (i == -1)
                        {
                            value = "Fhir" + value;
                        }
                        else
                        {
                            value = value.Substring(0, i + 1) + "Fhir" + value.Substring(i + 1);
                        }
                    }

                    return value;
                }

            case NamingConvention.PascalDotNotation:
                {
                    string[] components = ToPascal(value.Split('.'));
                    value = string.Join(".", components);

                    if ((reservedWords != null) &&
                        reservedWords.Contains(value))
                    {
                        components[components.Length - 1] =
                            "Fhir" + components[components.Length - 1];

                        return string.Join(".", components);
                    }

                    return value;
                }

            case NamingConvention.PascalCase:
                {
                    string[] components = ToPascal(value.Split('.'));
                    value = string.Join(concatenationDelimiter, components);

                    if ((reservedWords != null) &&
                        reservedWords.Contains(value))
                    {
                        components[components.Length - 1] =
                            "Fhir" + components[components.Length - 1];

                        return string.Join(concatenationDelimiter, components);
                    }

                    return value;
                }

            case NamingConvention.CamelCase:
                {
                    string[] components = ToCamel(value.Split('.'));
                    value = string.Join(concatenationDelimiter, components);

                    if ((reservedWords != null) &&
                        reservedWords.Contains(value))
                    {
                        components[components.Length - 1] =
                            "fhir" + ToPascal(components[components.Length - 1]);

                        return string.Join(concatenationDelimiter, components);
                    }

                    return value;
                }

            case NamingConvention.UpperCase:
                {
                    string[] components = ToUpperInvariant(value.Split('.'));
                    value = string.Join(concatenationDelimiter, components);

                    if ((reservedWords != null) &&
                        reservedWords.Contains(value))
                    {
                        components[components.Length - 1] = "FHIR" + components[components.Length - 1];

                        return string.Join(concatenationDelimiter, components);
                    }

                    return value;
                }

            case NamingConvention.LowerCase:
                {
                    string[] components = ToLowerInvariant(value.Split('.'));
                    value = string.Join(concatenationDelimiter, components);

                    if ((reservedWords != null) &&
                        reservedWords.Contains(value))
                    {
                        components[components.Length - 1] = "fhir" + components[components.Length - 1];

                        return string.Join(concatenationDelimiter, components);
                    }

                    return value;
                }

            case NamingConvention.None:
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
                return sanitized.Replace('_', '.');

            case NamingConvention.PascalDotNotation:
                {
                    string[] components = ToPascal(sanitized.Split('_'));
                    return string.Join(".", components);
                }

            case NamingConvention.PascalCase:
                {
                    string[] components = ToPascal(sanitized.Split('_'));
                    return string.Join(string.Empty, components);
                }

            case NamingConvention.CamelCase:
                {
                    string[] components = ToCamel(sanitized.Split('_'));
                    return string.Join(string.Empty, components);
                }

            case NamingConvention.UpperCase:
                {
                    return sanitized.ToUpperInvariant();
                }

            case NamingConvention.LowerCase:
                {
                    return sanitized.ToLowerInvariant();
                }

            case NamingConvention.None:
            default:
                throw new ArgumentException($"Invalid Naming Convention: {convention}");
        }
    }

    /// <summary>Sanitize for quoted.</summary>
    /// <param name="input">The input.</param>
    /// <returns>A string.</returns>
    public static string SanitizeForQuoted(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return string.Empty;
        }

        input = input.Replace("\"", "\\\"");
        input = input.Replace("\r", "\\r");
        input = input.Replace("\n", "\\n");

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

        if (name.Contains(" "))
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
        if (string.IsNullOrEmpty(input))
        {
            return string.Empty;
        }

        string value = input.Trim();
        value = value.Replace("\"", "\\\"");

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
        string pattern = "[^ -~]+";
        Regex reg_exp = new Regex(pattern);
        return reg_exp.Replace(value, string.Empty);
    }

    /// <summary>Sanitize for property.</summary>
    /// <param name="value">        The value.</param>
    /// <param name="reservedWords">The reserved words.</param>
    /// <returns>A string.</returns>
    public static string SanitizeForProperty(
        string value,
        HashSet<string> reservedWords)
    {
        if (string.IsNullOrEmpty(value))
        {
            return "NONE";
        }

        if (value.StartsWith("http://hl7.org/fhir/", StringComparison.Ordinal))
        {
            value = value.Substring(20);
        }

        char[] chars = value.Normalize(NormalizationForm.FormD).ToCharArray();

        StringBuilder sb = new StringBuilder();

        int charsLen = chars.Length;

        for (int i = 0; i < charsLen; i++)
        {
            char ch = chars[i];
            char second = (i + 1 < charsLen) ? chars[i + 1] : '\0';
            char third = (i + 2 < charsLen) ? chars[i + 2] : '\0';

            switch (ch)
            {
                case '.':
                    if ((second == '.') &&
                        (third == '.'))
                    {
                        sb.Append("_ellipsis_");
                        i += 2;
                        continue;
                    }

                    sb.Append('_');
                    break;

                case '’':
                case '\'':
                    if (second == '\'')
                    {
                        sb.Append("_double_quote_");
                        i++;
                        continue;
                    }

                    sb.Append("_quote_");

                    break;

                case '=':
                    if (second == '=')
                    {
                        sb.Append("_double_equals_");
                        i++;
                        continue;
                    }

                    sb.Append("_equals_");

                    break;

                case '!':
                    if (second == '=')
                    {
                        sb.Append("_not_equals_");
                        i++;
                        continue;
                    }

                    sb.Append("_not_");

                    break;

                case '<':
                    if (second == '=')
                    {
                        sb.Append("_less_than_or_equals_");
                        i++;
                        continue;
                    }

                    sb.Append("_less_than_");

                    break;

                case '>':
                    if (second == '=')
                    {
                        sb.Append("_greater_than_or_equals_");
                        i++;
                        continue;
                    }

                    sb.Append("_greater_than_");

                    break;

                case '…':
                    sb.Append("_ellipsis_unicode_");
                    break;

                case '*':
                    sb.Append("_asterisk_");
                    break;

                case '^':
                    sb.Append("_power_");
                    break;

                case '#':
                    sb.Append("_number_");
                    break;

                case '$':
                    sb.Append("_dollar_");
                    break;

                case '%':
                    sb.Append("_percent_");
                    break;

                case '&':
                    sb.Append("_and_");
                    break;

                case '@':
                    sb.Append("_at_");
                    break;

                case '+':
                    sb.Append("_plus_");
                    break;

                case '{':
                    sb.Append("_");     // sb.Append("_open_brace_");
                    break;

                case '}':
                    sb.Append("_");     // sb.Append("_close_brace_");
                    break;

                case '[':
                    sb.Append("_");     // sb.Append("_open_bracket_");
                    break;

                case ']':
                    sb.Append("_");     // sb.Append("_close_bracket_");
                    break;

                case '(':
                    sb.Append("_");     // sb.Append("_open_parenthesis_");
                    break;

                case ')':
                    sb.Append("_");     // sb.Append("_close_parenthesis_");
                    break;

                case '\\':
                    sb.Append("_");     // sb.Append("_backslash_");
                    break;

                case '/':
                    if (i == 0)
                    {
                        sb.Append("Per");
                    }
                    else
                    {
                        sb.Append("_");     // sb.Append("_slash_");
                    }

                    break;

                case '|':
                    sb.Append("_pipe_");
                    break;

                case ':':
                    sb.Append("_");     // sb.Append("_colon_");
                    break;

                case ';':
                    sb.Append("_");     // sb.Append("_semicolon_");
                    break;

                case ',':
                    sb.Append("_");     // sb.Append("_comma_");
                    break;

                case '°':
                    sb.Append("_degrees_");
                    break;

                case '?':
                    sb.Append("_question_");
                    break;

                case '"':
                case '“':
                case '”':
                    sb.Append("_quotation_");
                    break;

                case ' ':
                case '-':
                case '–':
                case '—':
                case '_':
                    if (sb.Length != 0)
                    {
                        sb.Append('_');
                    }

                    break;

                case '\r':
                case '\n':
                    break;

                default:
                    UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(ch);

                    switch (uc)
                    {
                        case UnicodeCategory.UppercaseLetter:
                        case UnicodeCategory.LowercaseLetter:
                        case UnicodeCategory.TitlecaseLetter:
                            sb.Append(ch);
                            break;
                        case UnicodeCategory.ModifierLetter:
                            break;
                        case UnicodeCategory.OtherLetter:
                            break;
                        case UnicodeCategory.NonSpacingMark:
                            break;
                        case UnicodeCategory.SpacingCombiningMark:
                            break;
                        case UnicodeCategory.EnclosingMark:
                            break;
                        case UnicodeCategory.DecimalDigitNumber:
                            sb.Append(ch);
                            break;
                        case UnicodeCategory.LetterNumber:
                            break;
                        case UnicodeCategory.OtherNumber:
                            break;
                        case UnicodeCategory.SpaceSeparator:
                            break;
                        case UnicodeCategory.LineSeparator:
                            break;
                        case UnicodeCategory.ParagraphSeparator:
                            break;
                        case UnicodeCategory.Control:
                            break;
                        case UnicodeCategory.Format:
                            break;
                        case UnicodeCategory.Surrogate:
                            break;
                        case UnicodeCategory.PrivateUse:
                            break;
                        case UnicodeCategory.ConnectorPunctuation:
                            break;
                        case UnicodeCategory.DashPunctuation:
                            break;
                        case UnicodeCategory.OpenPunctuation:
                            break;
                        case UnicodeCategory.ClosePunctuation:
                            break;
                        case UnicodeCategory.InitialQuotePunctuation:
                            break;
                        case UnicodeCategory.FinalQuotePunctuation:
                            break;
                        case UnicodeCategory.OtherPunctuation:
                            break;
                        case UnicodeCategory.MathSymbol:
                            break;
                        case UnicodeCategory.CurrencySymbol:
                            break;
                        case UnicodeCategory.ModifierSymbol:
                            break;
                        case UnicodeCategory.OtherSymbol:
                            break;
                        case UnicodeCategory.OtherNotAssigned:
                            break;
                        default:
                            break;
                    }

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

        // need to check for all digits or underscores, or reserved word
        if (RequiresAlpha(value) ||
            ((reservedWords != null) && reservedWords.Contains(value)))
        {
            if (value[0] == '_')
            {
                return $"VAL{value}";
            }

            return $"VAL_{value}";
        }

        return value;
    }
}
