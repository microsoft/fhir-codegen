// <copyright file="PackageElementModels.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Microsoft.Health.Fhir.CodeGenCommon.Models;

namespace FhirCodeGenBlazor.Models;

/// <summary>A package element models.</summary>
public abstract class PackageElementModels
{
    /// <summary>Information about the element.</summary>
    public readonly record struct ElementRecord(
        FhirArtifactClassEnum DefinedByClass,
        FhirComplex RootComplex,
        FhirElement Element);
}
