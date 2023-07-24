// <copyright file="FhirElementMapping.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Microsoft.Health.Fhir.CodeGenCommon.Models;

namespace Microsoft.Health.Fhir.CodeGenCommon.Structural;

/// <summary>A FHIR element mapping.</summary>
public record class FhirElementMapping
{
    /// <summary>Gets the reference to mapping declaration.</summary>
    public required string Identity { get; init; }

    /// <summary>Gets the computable language of mapping.</summary>
    public string Language { get; init; } = string.Empty;

    /// <summary>Gets the details of the mapping.</summary>
    public required string Map { get; init; }

    /// <summary>Gets comments about the mapping or its use.</summary>
    public string Comment { get; init; } = string.Empty;
}
