// <copyright file="PackageCacheRecord.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using static Microsoft.Health.Fhir.CodeGenCommon.Packaging.FhirReleases;

namespace Microsoft.Health.Fhir.CodeGenCommon.Models;

/// <summary>A package cache entry.</summary>
/// <param name="fhirVersion">        The FHIR version.</param>
/// <param name="directory">          Pathname of the directory.</param>
/// <param name="resolvedDirective">  The resolved directive.</param>
/// <param name="resolvedName">       Name of the resolved.</param>
/// <param name="resolvedVersion">    The resolved version.</param>
/// <param name="umbrellaPackageName">Name of the umbrella package.</param>
public record struct PackageCacheEntry(
    FhirSequenceCodes fhirVersion,
    string directory,
    string resolvedDirective,
    string name,
    string version);
