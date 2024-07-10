﻿// <copyright file="FhirPackageUtils.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Microsoft.Health.Fhir.CodeGenCommon.Packaging;

public static partial class FhirPackageUtils
{

    /// <summary>Test if a name matches known core packages.</summary>
    /// <returns>A RegEx.</returns>
    [GeneratedRegex("^hl7\\.fhir\\.r\\d+[A-Za-z]?\\.(core|expansions|examples|search|elements|corexml)$")]
    public static partial Regex MatchFhirReleasePackageNames();

    /// <summary>
    /// Gets the regular expression for matching known core package names.
    /// </summary>
    /// <returns>A regular expression.</returns>
    private static Regex _matchFhirReleasePackageNames = MatchFhirReleasePackageNames();

    [GeneratedRegex("^hl7\\.fhir\\.r\\d+[A-Za-z]?\\.core$")]
    public static partial Regex MatchCorePackageOnly();

    /// <summary>
    /// Gets the regular expression for matching known core package names.
    /// </summary>
    /// <returns>A regular expression.</returns>
    private static Regex _matchCorePackageOnly = MatchCorePackageOnly();


    public static bool PackageIsFhirRelease(string packageName)
    {
        string name = packageName;

        if (name.StartsWith('@'))
        {
            name = name[1..];
        }

        name = name.Contains('@')
            ? name.Split('@')[0]
            : name.Contains('#')
            ? name.Split('#')[0]
            : name;

        return _matchFhirReleasePackageNames.IsMatch(name);
    }

    public static bool PackageIsFhirCore(string packageName)
    {
        return _matchCorePackageOnly.IsMatch(packageName);
    }
}
