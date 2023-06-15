// <copyright file="FhirElementMapping.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Microsoft.Health.Fhir.CodeGenCommon.Models;

namespace Microsoft.Health.Fhir.CodeGenCommon.Refactor;

/// <summary>A FHIR element mapping.</summary>
public record class FhirElementMapping : ICloneable
{
    /// <summary>Gets the reference to mapping declaration.</summary>
    public required string Identity { get; init; }

    /// <summary>Gets the computable language of mapping.</summary>
    public string Language { get; init; } = string.Empty;

    /// <summary>Gets the details of the mapping.</summary>
    public required string Map { get; init; }

    /// <summary>Gets comments about the mapping or its use.</summary>
    public string Comment { get; init; } = string.Empty;

    /// <summary>Creates a new object that is a copy of the current instance.</summary>
    /// <returns>A new object that is a copy of this instance.</returns>
    object ICloneable.Clone() => this with { };

    /// <summary>Deep copy.</summary>
    /// <returns>A FhirElementProfile.</returns>
    public FhirElementMapping DeepCopy() => this with { };
}
