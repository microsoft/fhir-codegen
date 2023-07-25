// <copyright file="FhirOperation.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.Diagnostics.CodeAnalysis;
using Microsoft.Health.Fhir.CodeGenCommon.Extensions;

namespace Microsoft.Health.Fhir.CodeGenCommon.Structural;

/// <summary>A FHIR operation.</summary>
public record class FhirOperation : FhirModelBase, ICloneable
{
    /// <summary>Values that represent operation kind codes.</summary>
    public enum OperationKindCodes
    {
        /// <summary>An executable operation.</summary>
        [FhirLiteral("operation")]
        Operation,

        /// <summary>A named query.</summary>
        [FhirLiteral("query")]
        Query,
    }

    /// <summary>An operation parameter.</summary>
    public record class OperationParameter : ICloneable
    {
        private int _cardinalityMax = -1;

        /// <summary>Initializes a new instance of the OperationParameter class.</summary>
        public OperationParameter() { }

        /// <summary>Initializes a new instance of the OperationParameter class.</summary>
        /// <param name="other">The other.</param>
        [SetsRequiredMembers]
        protected OperationParameter(OperationParameter other)
        {
            Name = other.Name;
            Use = other.Use;
            Scopes = other.Scopes.Select(v => v);
            CardinalityMin = other.CardinalityMin;
            CardinalityMaxString = other.CardinalityMaxString;
            Documentation = other.Documentation;
            ParameterType = other.ParameterType;
            AllowedSubTypes = other.AllowedSubTypes.Select(v => v);
            TargetProfiles = other.TargetProfiles.Select(v => v);
            SearchType = other.SearchType;
            FieldOrder = other.FieldOrder;
        }

        /// <summary>Gets the name.</summary>
        public required string Name { get; init; }

        /// <summary>Gets the use.</summary>
        public required string Use { get; init; }

        /// <summary>Gets the scopes.</summary>
        public IEnumerable<string> Scopes { get; init; } = Enumerable.Empty<string>();

        /// <summary>Gets the cardinality minimum.</summary>
        public required int CardinalityMin { get; init; }

        /// <summary>Gets the cardinality maximum, -1 for unbounded (e.g., *).</summary>
        /// <value>The cardinality maximum.</value>
        public int CardinalityMax { get => _cardinalityMax; }

        /// <summary>Gets the cardinality maximum string.</summary>
        /// <value>The cardinality maximum string.</value>
        public required string CardinalityMaxString
        {
            get => _cardinalityMax switch
            {
                -1 => "*",
                _ => _cardinalityMax.ToString(),
            };

            init
            {
                if (value == "*")
                {
                    _cardinalityMax = -1;
                }
                else if (int.TryParse(value, out int result))
                {
                    _cardinalityMax = result;
                }
                else
                {
                    throw new ArgumentException($"Invalid cardinality max value: {value}");
                }
            }
        }

        /// <summary>Gets the FHIR cardinality string: min..max.</summary>
        public string FhirCardinality => $"{CardinalityMin}..{CardinalityMaxString}";

        /// <summary>Gets the documentation.</summary>
        public string Documentation { get; init; } = string.Empty;

        /// <summary>Gets the type of this parameter.</summary>
        public string ParameterType { get; init; } = string.Empty;

        /// <summary>Gets the allowed sub-type this parameter can have (if type is abstract).</summary>
        public IEnumerable<string> AllowedSubTypes { get; init; } = Enumerable.Empty<string>();

        /// <summary>Gets target profiles.</summary>
        public IEnumerable<string> TargetProfiles { get; init; } = Enumerable.Empty<string>();

        /// <summary>Gets the search type, if this is a search parameter.</summary>
        public string SearchType { get; init; } = string.Empty;

        /// <summary>Gets the field order.</summary>
        public required int FieldOrder { get; init; }

        /// <summary>Makes a deep copy of this object.</summary>
        /// <returns>A copy of this object.</returns>
        object ICloneable.Clone() => this with { };
    }

    /// <summary>Initializes a new instance of the FhirOperation class.</summary>
    public FhirOperation() { }

    /// <summary>Initializes a new instance of the FhirOperation class.</summary>
    /// <param name="other">The other.</param>
    [SetsRequiredMembers]
    protected FhirOperation(FhirOperation other)
        : base(other)
    {
            AffectsState = other.AffectsState;
            DefinedOnSystem = other.DefinedOnSystem;
            DefinedOnType = other.DefinedOnType;
            DefinedOnInstance = other.DefinedOnInstance;
            Code = other.Code;
            BaseDefinition = other.BaseDefinition;
            ResourceTypes = other.ResourceTypes.Select(v => v);
            Parameters = other.Parameters.Select(v => v with { });
            Kind = other.Kind;
    }

    /// <summary>Gets a value indicating whether the affects state.</summary>
    public bool? AffectsState { get; init; } = null;

    /// <summary>Gets a value indicating whether the defined on system.</summary>
    public required bool DefinedOnSystem { get; init; }

    /// <summary>Gets a value indicating whether the defined on type.</summary>
    public required bool DefinedOnType { get; init; }

    /// <summary>Gets a value indicating whether the defined on instance.</summary>
    public required bool DefinedOnInstance { get; init; }

    /// <summary>Gets the code.</summary>
    public required string Code { get; init; }

    /// <summary>Gets the base definition.</summary>
    public string BaseDefinition { get; init; } = string.Empty;

    /// <summary>Gets a list of types of the resources.</summary>
    public IEnumerable<string> ResourceTypes { get; init; } = Enumerable.Empty<string>();

    /// <summary>Gets allowed parameters to this operation.</summary>
    public IEnumerable<OperationParameter> Parameters { get; init; } = Enumerable.Empty<OperationParameter>();

    /// <summary>Gets the operation kind.</summary>
    public required OperationKindCodes Kind { get; init; }

    /// <summary>Makes a deep copy of this object.</summary>
    /// <returns>A copy of this object.</returns>
    object ICloneable.Clone() => this with { };
}
