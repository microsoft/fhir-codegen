// <copyright file="FhirPackageVersionInfo.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Microsoft.Health.Fhir.CodeGenCommon.Packaging;

namespace Microsoft.Health.Fhir.CodeGen._ForPackages;

/// <summary>FHIR Package version information.</summary>
public record class PackageManifest : CachePackageManifest
{
}
