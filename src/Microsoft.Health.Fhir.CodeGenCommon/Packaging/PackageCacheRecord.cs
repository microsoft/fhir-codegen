﻿// <copyright file="PackageCacheRecord.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using static Microsoft.Health.Fhir.CodeGenCommon.Packaging.FhirReleases;

namespace Microsoft.Health.Fhir.CodeGenCommon.Models;

/// <summary>A package cache entry.</summary>
/// <param name="FhirVersion">      The FHIR version this package is for.</param>
/// <param name="Directory">        Pathname of the directory where the package can be found.</param>
/// <param name="ResolvedDirective">The resolved directive in '[name]#[version or literal]' style.</param>
/// <param name="Name">             Name of the resolved package.</param>
/// <param name="Version">          The the resolved version (version string, not literal such as 'latest').</param>
public record struct PackageCacheEntry(
    FhirSequenceCodes FhirVersion,
    string Directory,
    string ResolvedDirective,
    string Name,
    string Version);
