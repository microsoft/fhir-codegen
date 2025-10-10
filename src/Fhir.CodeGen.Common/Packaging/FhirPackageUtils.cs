// <copyright file="FhirPackageUtils.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using Fhir.CodeGen.Common.Extensions;

namespace Fhir.CodeGen.Common.Packaging;

public static partial class FhirPackageUtils
{
#if NET8_0_OR_GREATER
    /// <summary>Test if a name matches known core packages.</summary>
    /// <returns>A RegEx.</returns>
    [GeneratedRegex("^hl7\\.fhir\\.r\\d+[A-Za-z]?\\.(core|corexml|elements|examples|expansions|search)$")]
    public static partial Regex MatchFhirReleasePackageNames();

    /// <summary>
    /// Gets the regular expression for matching known core package names.
    /// </summary>
    /// <returns>A regular expression.</returns>
    private static Regex _matchFhirReleasePackageNames = MatchFhirReleasePackageNames();
#else
    /// <summary>
    /// Gets the regular expression for matching known core package names.
    /// </summary>
    /// <returns>A regular expression.</returns>
    private static readonly Regex _matchFhirReleasePackageNames =
        new Regex("^hl7\\.fhir\\.r\\d+[A-Za-z]?\\.(core|expansions|examples|search|elements|corexml)$", RegexOptions.Compiled);
#endif

#if NET8_0_OR_GREATER
    [GeneratedRegex("^hl7\\.fhir\\.r\\d+[A-Za-z]?$")]
    public static partial Regex MatchFhirReleasePartialNames();

    private static Regex _matchFhirReleasePartialNames = MatchFhirReleasePartialNames();
#else
    private static readonly Regex _matchFhirReleasePartialNames =
        new Regex("^hl7\\.fhir\\.r\\d+[A-Za-z]?$", RegexOptions.Compiled);
#endif

#if NET8_0_OR_GREATER
    [GeneratedRegex("^\\S*\\.r\\d+[A-Za-z]?$")]
    public static partial Regex MatchEndsInFhirRelease();

    private static Regex _matchEndsInFhirRelease = MatchEndsInFhirRelease();
#else
    private static readonly Regex _matchEndsInFhirRelease =
        new Regex("^\\S*\\.r\\d+[A-Za-z]?$", RegexOptions.Compiled);
#endif

#if NET8_0_OR_GREATER
    [GeneratedRegex("^hl7\\.fhir\\.r\\d+[A-Za-z]?\\.core$")]
    public static partial Regex MatchCorePackageOnly();

    /// <summary>
    /// Gets the regular expression for matching known core package names.
    /// </summary>
    /// <returns>A regular expression.</returns>
    private static Regex _matchCorePackageOnly = MatchCorePackageOnly();
#else
    /// <summary>
    /// Gets the regular expression for matching known core package names.
    /// </summary>
    /// <returns>A regular expression.</returns>
    private static readonly Regex _matchCorePackageOnly = new Regex("^hl7\\.fhir\\.r\\d+[A-Za-z]?\\.core$", RegexOptions.Compiled);
#endif

    public static bool PackageIsFhirRelease(string packageId)
    {
        if (_matchFhirReleasePackageNames.IsMatch(packageId))
        {
            return true;
        }

        int delimiterLoc = packageId.IndexOfAny(['#', '@']);
        if (delimiterLoc == -1)
        {
            return false;
        }

        return _matchFhirReleasePackageNames.IsMatch(packageId[0..(delimiterLoc-1)]);
    }

    public static bool PackageIsFhirCore(string packageId) => _matchCorePackageOnly.IsMatch(packageId);

    public static bool PackageIsFhirCorePartial(string packageId) => _matchFhirReleasePartialNames.IsMatch(packageId);

    public static bool PackageEndsWithFhirVersion(string packageId) => _matchEndsInFhirRelease.IsMatch(packageId);

    public static string GetShortName(string packageName, string packageVersion)
    {
        string version = packageVersion.Replace('.', '_');

        if (packageName.StartsWith("hl7.fhir.", StringComparison.OrdinalIgnoreCase))
        {
            return packageName["hl7.fhir.".Length..].ToPascalCase() + "_" + version;
        }

        return packageName.ToPascalCase() + "_" + version;
    }
}
