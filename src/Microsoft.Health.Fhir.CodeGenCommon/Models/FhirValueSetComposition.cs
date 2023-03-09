// <copyright file="FhirValueSetComposition.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Health.Fhir.CodeGenCommon.Models;

/// <summary>A fhir value set composition.</summary>
public class FhirValueSetComposition : ICloneable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FhirValueSetComposition"/> class.
    /// </summary>
    /// <param name="system">           The system.</param>
    /// <param name="version">          The version.</param>
    /// <param name="concepts">         The concepts.</param>
    /// <param name="filters">          The filters.</param>
    /// <param name="linkedValueSets">  The linked ValueSet URLs.</param>
    public FhirValueSetComposition(
        string system,
        string version,
        List<FhirConcept> concepts,
        List<FhirValueSetFilter> filters,
        List<string> linkedValueSets)
    {
        System = system;
        Version = version;
        Concepts = concepts;
        Filters = filters;
        LinkedValueSets = linkedValueSets;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FhirValueSetComposition"/> class.
    /// </summary>
    /// <param name="source">Source for the.</param>
    public FhirValueSetComposition(FhirValueSetComposition source)
    {
        System = source.System;
        Version = source.Version;
        Concepts = source.Concepts?.Select(c => new FhirConcept(c)).ToList() ?? new();
        Filters = source.Filters?.Select(f => new FhirValueSetFilter(f)).ToList() ?? new();
        LinkedValueSets = source.LinkedValueSets?.Select(s => s).ToList() ?? new();
    }

    /// <summary>Values that represent composition types.</summary>
    public enum CompositionType
    {
        /// <summary>An enum constant representing the inclusion option.</summary>
        Inclusion,

        /// <summary>An enum constant representing the exclusion option.</summary>
        Exclusion,
    }

    /// <summary>Gets the system.</summary>
    /// <value>The system.</value>
    public string System { get; }

    /// <summary>Gets the version.</summary>
    /// <value>The version.</value>
    public string Version { get; }

    /// <summary>Gets the concepts.</summary>
    /// <value>The concepts.</value>
    public List<FhirConcept> Concepts { get; }

    /// <summary>Gets the filters.</summary>
    /// <value>The filters.</value>
    public List<FhirValueSetFilter> Filters { get; }

    /// <summary>Gets the sets the linked value belongs to.</summary>
    /// <value>The linked value sets.</value>
    public List<string> LinkedValueSets { get; }

    /// <summary>Creates a new object that is a copy of the current instance.</summary>
    /// <returns>A new object that is a copy of this instance.</returns>
    public object Clone()
    {
        return new FhirValueSetComposition(this);
    }
}
