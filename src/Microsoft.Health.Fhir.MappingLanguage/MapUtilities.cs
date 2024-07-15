// <copyright file="MapUtilities.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>


using System.Text.RegularExpressions;
using Hl7.Fhir.Utility;

namespace Microsoft.Health.Fhir.MappingLanguage;

/// <summary>FHIR Mapping Language utilities.</summary>
internal static partial class MapUtilities
{
    private static readonly HashSet<string> _uriSchemes = new(StringComparer.Ordinal)
    {
        "http",
        "https",
        "urn",
    };

#if NET8_0_OR_GREATER
    /// <summary>Invalid token RegEx.</summary>
    /// <returns>A RegEx.</returns>
    [GeneratedRegex(@"[^a-zA-Z0-9_\[\]]")]
    private static partial Regex InvalidTokenRegex();
    private static readonly Regex _invalidTokenRegex = InvalidTokenRegex();
#else
    private static readonly Regex _invalidTokenRegex = new Regex(@"[^a-zA-Z0-9_\[\]]", RegexOptions.Compiled);
#endif

#if NET8_0_OR_GREATER
    [GeneratedRegex(@"[^a-z0-9_\[\]]")]
    private static partial Regex InvalidTokenRegexForLower();
    private static readonly Regex _invalidTokenRegexForLower = InvalidTokenRegexForLower();
#else
    private static readonly Regex _invalidTokenRegexForLower = new Regex(@"[^a-z0-9_\[\]]", RegexOptions.Compiled);
#endif

    /// <summary>Query if 'value' is token.</summary>
    /// <param name="value">The value.</param>
    /// <returns>True if token, false if not.</returns>
    internal static bool IsToken(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return false;
        }

        // tokens must start with a letter
        if (!char.IsLetter(value[0]))
        {
            return false;
        }

        // check for any invalid characters
        return !_invalidTokenRegex.IsMatch(value);
    }

    /// <summary>Query if 'value' is lower-case token.</summary>
    /// <param name="value">The value.</param>
    /// <returns>True if lower token, false if not.</returns>
    internal static bool IsLowerToken(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return false;
        }

        // tokens must start with a lower-case letter
        if (!char.IsLower(value[0]))
        {
            return false;
        }

        // check for any invalid characters
        return !_invalidTokenRegexForLower.IsMatch(value);
    }

    /// <summary>Query if 'url' is absolute URL, according to org.hl7.fhir.utilities.Utilities tests.</summary>
    /// <param name="url">URL of the resource.</param>
    /// <returns>True if absolute url, false if not.</returns>
    internal static bool IsAbsoluteUrl(string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            return false;
        }

        int firstColon = url.IndexOf(':');

        // ensure there is a colon and it is not the first or last character
        if ((firstColon == -1) || (firstColon == 0) || (firstColon == url.Length - 1))
        {
            return false;
        }

        string prefix = url.Substring(0, firstColon);

        if (_uriSchemes.Contains(prefix))
        {
            // if the prefix is a known scheme, we just need to check there is additional content and it does not contain a space
            return (url.Length > firstColon + 1) && !url.Contains(' ');
        }

        // if the prefix is not a known scheme, check if it is a lower-case token
        if (IsLowerToken(prefix))
        {
            // if the prefix is a lower-case token, we just need to check there is additional content and it does not contain a space
            return (url.Length > firstColon + 1) && !url.Contains(' ');
        }

        // still here means it is not a valid absolute URL
        return false;
    }
}
