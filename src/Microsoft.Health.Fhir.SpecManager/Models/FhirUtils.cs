// <copyright file="FhirUtils.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using static Microsoft.Health.Fhir.SpecManager.Models.FhirTypeBase;

namespace Microsoft.Health.Fhir.SpecManager.Models
{
    /// <summary>A fhir utilities.</summary>
    public abstract class FhirUtils
    {
        /// <summary>The RegEx sanitize for property definition.</summary>
        private const string _regexSanitizeForPropertyDefinition = "[\r\n\\.\\|\\- \\/\\(\\)]";

        /// <summary>The RegEx sanitize for property.</summary>
        private static Regex _regexSanitizeForProperty = new Regex(_regexSanitizeForPropertyDefinition);

        /// <summary>Converts this object to a convention.</summary>
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        /// <exception cref="ArgumentException">    Thrown when one or more arguments have unsupported or
        ///  illegal values.</exception>
        /// <param name="name">                  The value.</param>
        /// <param name="path">                  Full pathname of the file.</param>
        /// <param name="convention">            The convention.</param>
        /// <param name="concatenatePath">       (Optional) True to concatenate path.</param>
        /// <param name="concatenationDelimiter">(Optional) The concatenation delimiter.</param>
        /// <returns>The given data converted to a string.</returns>
        public static string ToConvention(
            string name,
            string path,
            NamingConvention convention,
            bool concatenatePath = false,
            string concatenationDelimiter = "")
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
                    return value;

                case NamingConvention.PascalDotNotation:
                    {
                        string[] components = ToPascal(value.Split('.'));
                        return string.Join(".", components);
                    }

                case NamingConvention.PascalCase:
                    {
                        string[] components = ToPascal(value.Split('.'));
                        return string.Join(concatenationDelimiter, components);
                    }

                case NamingConvention.CamelCase:
                    {
                        string[] components = ToCamel(value.Split('.'));
                        return string.Join(concatenationDelimiter, components);
                    }

                case NamingConvention.UpperCase:
                    {
                        string[] components = ToUpperInvariant(value.Split('.'));
                        return string.Join(concatenationDelimiter, components);
                    }

                case NamingConvention.LowerCase:
                    {
                        string[] components = ToLowerInvariant(value.Split('.'));
                        return string.Join(concatenationDelimiter, components);
                    }

                case NamingConvention.None:
                default:
                    throw new ArgumentException($"Invalid Naming Convention: {convention}");
            }
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

        /// <summary>Requires alpha.</summary>
        /// <param name="value">The value.</param>
        /// <returns>True if it succeeds, false if it fails.</returns>
        private static bool RequiresAlpha(string value)
        {
            foreach (char c in value)
            {
                if (((c < '0') || (c > '9')) && (c != '_'))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>Sanitize for property.</summary>
        /// <param name="value">        The value.</param>
        /// <param name="reservedWords">The reserved words.</param>
        /// <returns>A string.</returns>
        public static string SanitizeForProperty(string value, HashSet<string> reservedWords)
        {
            if (string.IsNullOrEmpty(value))
            {
                return "NONE";
            }

            if (value.Equals("...", StringComparison.Ordinal))
            {
                value = "NONE";
            }

            if (value.Contains("<="))
            {
                value = value.Replace("<=", "LESS_THAN_OR_EQUALS");
            }

            if (value.Contains("<"))
            {
                value = value.Replace("<", "LESS_THAN");
            }

            if (value.Contains(">="))
            {
                value = value.Replace(">=", "GREATER_THAN_OR_EQUALS");
            }

            if (value.Contains(">"))
            {
                value = value.Replace(">", "GREATER_THAN");
            }

            if (value.Contains("!="))
            {
                value = value.Replace("!=", "NOT_EQUALS");
            }

            if (value.Contains("=="))
            {
                value = value.Replace("==", "EQUALS");
            }

            if (value.Contains("="))
            {
                value = value.Replace("=", "EQUALS");
            }

            if (value.Contains("+"))
            {
                value = value.Replace("+", "PLUS");
            }

            if (char.IsDigit(value[0]))
            {
                value = "_" + value;
            }

            // make major substitutions
            value = _regexSanitizeForProperty.Replace(value, "_");

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
}
