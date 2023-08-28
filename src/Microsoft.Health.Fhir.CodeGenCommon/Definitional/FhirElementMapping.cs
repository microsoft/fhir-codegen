// <copyright file="FhirElementMapping.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

namespace Microsoft.Health.Fhir.CodeGenCommon.Definitional;

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

    /// <summary>Makes a deep copy of this object.</summary>
    /// <returns>A copy of this object.</returns>
    object ICloneable.Clone() => this with { };
}
