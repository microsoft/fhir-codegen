// <copyright file="ConformanceAnnotatedBase.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Health.Fhir.CodeGenCommon.Expectations;

/// <summary>A conformance annotated base.</summary>
public abstract record class ConformanceAnnotatedBase : IConformanceAnnotated, ICloneable
{
    /// <summary>Initializes a new instance of the ConformanceAnnotatedBase class.</summary>
    public ConformanceAnnotatedBase() { }

    /// <summary>Initializes a new instance of the ConformanceAnnotatedBase class.</summary>
    /// <param name="other">The other.</param>
    [SetsRequiredMembers]
    protected ConformanceAnnotatedBase(ConformanceAnnotatedBase other)
    {
        ConformanceExpectation = other.ConformanceExpectation with { };
        ObligationsByActor = other.ObligationsByActor.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Select(v => v with { }));
    }

    /// <summary>Gets or initializes the conformance expectation.</summary>
    public FhirExpectation ConformanceExpectation { get; init; } = new();

    /// <summary>Gets the obligations by actor.</summary>
    public Dictionary<string, IEnumerable<FhirObligation>> ObligationsByActor { get; init; } = new();

    /// <summary>Makes a deep copy of this object.</summary>
    /// <returns>A copy of this object.</returns>
    object ICloneable.Clone() => this with { };
}
