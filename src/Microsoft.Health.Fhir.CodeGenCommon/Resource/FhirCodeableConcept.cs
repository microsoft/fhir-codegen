// <copyright file="FhirCodeableConcept.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

namespace Microsoft.Health.Fhir.CodeGenCommon.Resource;

/// <summary>A FHIR codeable concept.</summary>
public record class FhirCodeableConcept
{
    /// <summary>Gets or initializes the code defined by a terminology system.</summary>
    public required IEnumerable<FhirCoding> Coding { get; init; } = Enumerable.Empty<FhirCoding>();

    /// <summary>Gets or initializes the plain text representation of the concept.</summary>
    public string Text { get; init; } = string.Empty;
}
