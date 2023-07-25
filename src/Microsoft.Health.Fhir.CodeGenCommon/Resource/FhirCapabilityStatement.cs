// <copyright file="FhirCapabilityStatement.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.Diagnostics.CodeAnalysis;
using Microsoft.Health.Fhir.CodeGenCommon.Extensions;
using Microsoft.Health.Fhir.CodeGenCommon.Structural;

namespace Microsoft.Health.Fhir.CodeGenCommon.Resource;

/// <summary>A FHIR capability statement.</summary>
public record class FhirCapabilityStatement : FhirCanonicalBase, ICloneable
{
    /// <summary>A capability operation.</summary>
    public record class CapabilityOperation : IWithExpectations, ICloneable
    {
        /// <summary>Initializes a new instance of the CapabilityOperation class.</summary>
        public CapabilityOperation() { }

        /// <summary>Initializes a new instance of the CapabilityOperation class.</summary>
        /// <param name="other">The other.</param>
        [SetsRequiredMembers]
        protected CapabilityOperation(CapabilityOperation other)
        {
            Name = other.Name;
            DefinitionCanonical = other.DefinitionCanonical;
            Documentation = other.Documentation;
            AdditionalDefinitions = other.AdditionalDefinitions.Select(v => v);
            ObligationsByActor = other.ObligationsByActor.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Select(v => v with { }));
        }

        /// <summary>Gets the name.</summary>
        public required string Name { get; init; }

        /// <summary>Gets the definition canonical.</summary>
        public required string DefinitionCanonical { get; init; }

        /// <summary>Gets the documentation (markdown).</summary>
        public string Documentation { get; init; } = string.Empty;

        /// <summary>Gets the additional definitions.</summary>
        public IEnumerable<string> AdditionalDefinitions { get; init; } = Enumerable.Empty<string>();

        /// <summary>Gets the obligations by actor.</summary>
        public Dictionary<string, IEnumerable<FhirObligation>> ObligationsByActor { get; init; } = new();

        /// <summary>Convert this object into a string representation.</summary>
        /// <returns>A string that represents this object.</returns>
        public override string ToString()
        {
            if (!ObligationsByActor.Any())
            {
                return Name;
            }

            return Name + ": " + string.Join("; ", ObligationsByActor.Select(kvp => (string.IsNullOrEmpty(kvp.Key) ? "" : $"{kvp.Key}: ") + string.Join(", ", kvp.Value)));
        }

        /// <summary>Makes a deep copy of this object.</summary>
        /// <returns>A copy of this object.</returns>
        object ICloneable.Clone() => this with { };
    }

    /// <summary>A capability search parameter.</summary>
    public record class CapabilitySearchParam : IWithExpectations, ICloneable
    {
        private string _fhirSearchType = string.Empty;
        private readonly FhirSearchParam.SearchParameterTypeCodes _searchType;

        /// <summary>Initializes a new instance of the CapabilitySearchParam class.</summary>
        public CapabilitySearchParam() { }

        /// <summary>Initializes a new instance of the CapabilitySearchParam class.</summary>
        /// <param name="other">The other.</param>
        [SetsRequiredMembers]
        public CapabilitySearchParam(CapabilitySearchParam other)
        {
            Name = other.Name;
            DefinitionCanonical = other.DefinitionCanonical;
            Documentation = other.Documentation;
            FhirSearchType = other.FhirSearchType;
        }

        /// <summary>Gets or initializes the name.</summary>
        public required string Name { get; init; }

        /// <summary>Gets or initializes the definition canonical.</summary>
        public string DefinitionCanonical { get; init; } = string.Empty;

        /// <summary>Gets or initializes the documentation (markdown).</summary>
        public string Documentation { get; init; } = string.Empty;

        /// <summary>Gets the type of the search parameter input.</summary>
        public FhirSearchParam.SearchParameterTypeCodes SearchType => _searchType;

        /// <summary>Gets the FHIR Literal search parameter type.</summary>
        public required string FhirSearchType
        {
            get => _fhirSearchType;
            init
            {
                _fhirSearchType = value;
                if (_fhirSearchType.TryFhirEnum(out FhirSearchParam.SearchParameterTypeCodes v))
                {
                    _searchType = v;
                }
            }
        }

        /// <summary>Gets the obligations by actor.</summary>
        public Dictionary<string, IEnumerable<FhirObligation>> ObligationsByActor { get; init; } = new();

        /// <summary>Convert this object into a string representation.</summary>
        /// <returns>A string that represents this object.</returns>
        public override string ToString()
        {
            if (!ObligationsByActor.Any())
            {
                return Name;
            }

            return Name + ": " + string.Join("; ", ObligationsByActor.Select(kvp => (string.IsNullOrEmpty(kvp.Key) ? "" : $"{kvp.Key}: ") + string.Join(", ", kvp.Value)));
        }

        /// <summary>Makes a deep copy of this object.</summary>
        /// <returns>A copy of this object.</returns>
        object ICloneable.Clone() => this with { };
    }

    /// <summary>Makes a deep copy of this object.</summary>
    /// <returns>A copy of this object.</returns>
    object ICloneable.Clone() => this with { };
}
