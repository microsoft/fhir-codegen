// <copyright file="FhirConcept.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

namespace Microsoft.Health.Fhir.CodeGen.Models;

public record class FhirConcept
{
    public record class ConceptProperty
    {
        public required string Code { get; init; }

        public required string Value { get; init; }
    }

    /// <summary>Gets or initializes the system.</summary>
    public required string System { get; init; }

    /// <summary>Gets or initializes the code.</summary>
    public required string Code { get; init; }

    /// <summary>Gets or initializes the display.</summary>
    public required string Display { get; init; }

    public bool? IsAbstract { get; init; } = null;

    public bool? IsInactive { get; init; } = null;

    public string Version { get; init; } = string.Empty;

    public ConceptProperty[] Properties { get; init; } = Array.Empty<ConceptProperty>();

    /// <summary>Query if 'code' has property.</summary>
    /// <param name="code"> The code.</param>
    /// <param name="value">(Optional) The value.</param>
    /// <returns>True if property, false if not.</returns>
    public bool HasProperty(string code, string? value = null) => Properties.Any(p => p.Code == code && (value == null || p.Value == value));
}
