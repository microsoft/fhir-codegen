// <copyright file="FhirCompartment.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.Diagnostics.CodeAnalysis;
using Microsoft.Health.Fhir.CodeGenCommon.Extensions;

namespace Microsoft.Health.Fhir.CodeGenCommon.Instance;

/// <summary>A FHIR compartment.</summary>
public record class FhirCompartment : FhirCanonicalBase, ICloneable
{
    /// <summary>Values that represent compartment type codes.</summary>
    public enum CompartmentTypeCodes
    {
        /// <summary>Patient compartment.</summary>
        [FhirLiteral("Patient")]
        Patient,

        /// <summary>Encounter compartment.</summary>
        [FhirLiteral("Encounter")]
        Encounter,

        /// <summary>RelatedPerson compartment.</summary>
        [FhirLiteral("RelatedPerson")]
        RelatedPerson,

        /// <summary>Practitioner compartment.</summary>
        [FhirLiteral("Practitioner")]
        Practitioner,

        /// <summary>Device compartment.</summary>
        [FhirLiteral("Device")]
        Device,

        /// <summary>An enum constant representing the unkown option.</summary>
        [FhirLiteral("")]
        Unkown,
    }

    /// <summary>A compartment resource.</summary>
    public record class CompartmentResource : FhirElementBase, ICloneable
    {
        public CompartmentResource() : base() { }

        [SetsRequiredMembers]
        protected CompartmentResource(CompartmentResource other)
            : base(other)
        {
            ResourceType = other.ResourceType;
            Parameters = other.Parameters.Select(v => v);
            Documentation = other.Documentation;
            StartParam = other.StartParam;
            EndParam = other.EndParam;
        }

        /// <summary>Gets or initializes the type of the resource.</summary>
        public required string ResourceType { get; init; }

        /// <summary>Gets or initializes options for controlling the operation.</summary>
        public IEnumerable<string> Parameters { get; init; } = Enumerable.Empty<string>();

        /// <summary>Gets or initializes the documentation.</summary>
        public string Documentation { get; init; } = string.Empty;

        /// <summary>Gets the Search Param for interpreting $everything.start.</summary>
        public string StartParam { get; init; } = string.Empty;

        /// <summary>Gets the Search Param for interpreting $everything.end.</summary>
        public string EndParam { get; init; } = string.Empty;
    }

    private CompartmentTypeCodes _compartmentType = CompartmentTypeCodes.Unkown;
    private string _compartmentTypeLiteral = string.Empty;

    /// <summary>Initializes a new instance of the FhirCompartment class.</summary>
    public FhirCompartment() : base() { }

    /// <summary>Initializes a new instance of the FhirCompartment class.</summary>
    /// <param name="other">The other.</param>
    [SetsRequiredMembers]
    public FhirCompartment(FhirCompartment other)
        : base(other)
    {
        CompartmentTypeLiteral = other.CompartmentTypeLiteral;
        SupportsSearch = other.SupportsSearch;
        Resources = other.Resources.Select(v => v with { });
    }

    /// <summary>Gets the type of the compartment.</summary>
    public CompartmentTypeCodes CompartmentType { get => _compartmentType; }

    /// <summary>Gets or initializes the compartment type literal.</summary>
    public required string CompartmentTypeLiteral
    {
        get => _compartmentTypeLiteral;
        init
        {
            _compartmentTypeLiteral = value;
            _compartmentType = value.ToEnum<CompartmentTypeCodes>() ?? CompartmentTypeCodes.Unkown;
        }
    }

    /// <summary>Gets or initializes a value indicating whether the supports search.</summary>
    public required bool SupportsSearch { get; init; }

    /// <summary>Gets or initializes the resources.</summary>
    public IEnumerable<CompartmentResource> Resources { get; init; } = Enumerable.Empty<CompartmentResource>();

    /// <summary>Makes a deep copy of this object.</summary>
    /// <returns>A copy of this object.</returns>
    object ICloneable.Clone() => this with { };
}
