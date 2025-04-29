// <copyright file="PrimitiveMappings.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Hl7.Fhir.Model;
using CMR = Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship;

namespace Microsoft.Health.Fhir.Comparison.CompareTool;

public class FhirTypeMappings
{
    /// <summary>
    /// Represents a mapping between source and target types for code generation.
    /// </summary>
    /// <param name="SourceType">The source type in the mapping.</param>
    /// <param name="TargetType">The target type in the mapping.</param>
    /// <param name="ConceptDomainRelationship">The relationship between the concept domains of the source and target types.</param>
    /// <param name="ValueDomainRelationship">The relationship between the value domains of the source and target types.</param>
    public readonly record struct CodeGenTypeMapping(
        string SourceType,
        string TargetType,
        CMR ConceptDomainRelationship,
        CMR ValueDomainRelationship)
    {
        /// <summary>
        /// Gets a comment describing the relationship between the concept domains of the source and target types.
        /// </summary>
        public string ConceptDomainComment => ConceptDomainRelationship switch
        {
            CMR.Equivalent => (SourceType == TargetType)
                ? "The type is the same"
                : $"{SourceType} and {TargetType} are conceptually interchangeable where appropriate",
            CMR.SourceIsNarrowerThanTarget => $"`{SourceType}` is considered a narrower concept domain than `{TargetType}`",
            CMR.SourceIsBroaderThanTarget => $"`{SourceType}` is considered a broader concept domain than `{TargetType}`",
            _ => $"Conceptually, `{SourceType}` and `{TargetType}` should be treated as `{ConceptDomainRelationship}`"
        };

        /// <summary>
        /// Gets a comment describing the relationship between the value domains of the source and target types.
        /// </summary>
        public string ValueDomainComment => ValueDomainRelationship switch
        {
            CMR.Equivalent => (SourceType == TargetType)
                ? "The type is the same"
                : $"`{SourceType}` and `{TargetType}` cover the same value domain",
            CMR.SourceIsNarrowerThanTarget => $"`{SourceType}` covers a smaller value domain than `{TargetType}`",
            CMR.SourceIsBroaderThanTarget => $"`{SourceType} covers a larger value domain than `{TargetType}`",
            _ => $"In value domain, `{SourceType}` and `{TargetType}` should be treated as `{ConceptDomainRelationship}`"
        };

        public string Comment => (SourceType == TargetType)
            ? "The types are the same"
            : $"Concept domain: {ConceptDomainComment}. Value domain: {ValueDomainComment}";

        public CMR Relationship => ConceptDomainRelationship switch
        {
            CMR.Equivalent => ValueDomainRelationship,
            _ => ConceptDomainRelationship
        };
    }

    /// <summary>
    /// An array of mappings between primitive FHIR types.
    /// Each mapping defines the relationship between a source type and a target type.
    /// </summary>
    /// <remarks>
    /// The mappings are used to determine how types are converted or compared during code generation.
    /// </remarks>
    internal static readonly CodeGenTypeMapping[] PrimitiveMappings = [
        new("base64Binary", "base64Binary", CMR.Equivalent, CMR.Equivalent),

        new("boolean", "boolean", CMR.Equivalent, CMR.Equivalent),

        new("canonical", "canonical", CMR.Equivalent, CMR.Equivalent),
        new("canonical", "string", CMR.Equivalent, CMR.SourceIsNarrowerThanTarget),
        new("canonical", "uri", CMR.Equivalent, CMR.Equivalent),

        new("code", "code", CMR.Equivalent, CMR.Equivalent),
        new("code", "id", CMR.Equivalent, CMR.SourceIsBroaderThanTarget),
        new("code", "string", CMR.Equivalent, CMR.Equivalent),
        new("code", "oid", CMR.Equivalent, CMR.SourceIsBroaderThanTarget),
        new("code", "uri", CMR.Equivalent, CMR.SourceIsBroaderThanTarget),

        new("date", "date", CMR.Equivalent, CMR.Equivalent),
        new("date", "dateTime", CMR.Equivalent, CMR.SourceIsNarrowerThanTarget),

        new("dateTime", "dateTime", CMR.Equivalent, CMR.Equivalent),
        new("dateTime", "date", CMR.Equivalent, CMR.SourceIsBroaderThanTarget),

        new("decimal", "decimal", CMR.Equivalent, CMR.Equivalent),
        new("decimal", "unsignedInt", CMR.Equivalent, CMR.SourceIsBroaderThanTarget),

        new("id", "id", CMR.Equivalent, CMR.Equivalent),
        new("id", "code", CMR.Equivalent, CMR.SourceIsNarrowerThanTarget),
        new("id", "oid", CMR.Equivalent, CMR.NotRelatedTo),
        new("id", "string", CMR.Equivalent, CMR.SourceIsNarrowerThanTarget),

        new("instant", "instant", CMR.Equivalent, CMR.Equivalent),

        new("integer", "integer", CMR.Equivalent, CMR.Equivalent),
        new("integer", "integer64", CMR.Equivalent, CMR.SourceIsNarrowerThanTarget),
        new("integer", "positiveInt", CMR.Equivalent, CMR.SourceIsBroaderThanTarget),
        new("integer", "unsignedInt", CMR.Equivalent, CMR.SourceIsBroaderThanTarget),

        new("integer64", "integer64", CMR.Equivalent, CMR.Equivalent),
        new("integer64", "integer", CMR.Equivalent, CMR.SourceIsBroaderThanTarget),
        new("integer64", "string", CMR.Equivalent, CMR.SourceIsNarrowerThanTarget),

        new("markdown", "markdown", CMR.Equivalent, CMR.Equivalent),
        new("markdown", "string", CMR.Equivalent, CMR.Equivalent),

        new("oid", "oid", CMR.Equivalent, CMR.Equivalent),
        new("oid", "code", CMR.Equivalent, CMR.SourceIsNarrowerThanTarget),
        new("oid", "id", CMR.Equivalent, CMR.SourceIsNarrowerThanTarget),
        new("oid", "string", CMR.Equivalent, CMR.SourceIsNarrowerThanTarget),

        new("positiveInt", "positiveInt", CMR.Equivalent, CMR.Equivalent),
        new("positiveInt", "integer", CMR.Equivalent, CMR.SourceIsNarrowerThanTarget),
        new("positiveInt", "unsignedInt", CMR.Equivalent, CMR.SourceIsNarrowerThanTarget),

        new("string", "string", CMR.Equivalent, CMR.Equivalent),
        new("string", "canonical", CMR.Equivalent, CMR.SourceIsBroaderThanTarget),
        new("string", "code", CMR.Equivalent, CMR.Equivalent),
        new("string", "id", CMR.Equivalent, CMR.SourceIsBroaderThanTarget),
        new("string", "integer64", CMR.Equivalent, CMR.SourceIsBroaderThanTarget),
        new("string", "markdown", CMR.Equivalent, CMR.Equivalent),
        new("string", "oid", CMR.Equivalent, CMR.SourceIsBroaderThanTarget),
        new("string", "uri", CMR.Equivalent, CMR.SourceIsBroaderThanTarget),

        new("time", "time", CMR.Equivalent, CMR.Equivalent),

        new("unsignedInt", "unsignedInt", CMR.Equivalent, CMR.Equivalent),
        new("unsignedInt", "decimal", CMR.Equivalent, CMR.SourceIsNarrowerThanTarget),
        new("unsignedInt", "integer", CMR.Equivalent, CMR.SourceIsNarrowerThanTarget),
        new("unsignedInt", "positiveInt", CMR.Equivalent, CMR.SourceIsBroaderThanTarget),

        new("uri", "uri", CMR.Equivalent, CMR.Equivalent),
        new("uri", "canonical", CMR.Equivalent, CMR.Equivalent),
        new("uri", "code", CMR.Equivalent, CMR.SourceIsNarrowerThanTarget),
        new("uri", "string", CMR.Equivalent, CMR.SourceIsNarrowerThanTarget),
        new("uri", "url", CMR.Equivalent, CMR.Equivalent),

        new("url", "url", CMR.Equivalent, CMR.Equivalent),
        new("url", "uri", CMR.Equivalent, CMR.Equivalent),

        new("uuid", "uuid", CMR.Equivalent, CMR.Equivalent),

        new("xhtml", "xhtml", CMR.Equivalent, CMR.Equivalent),
    ];

    static internal bool TryGetMapping(
        string sourceType,
        string targetType,
        [NotNullWhen(true)] out CodeGenTypeMapping? mapping)
    {
        foreach (CodeGenTypeMapping m in PrimitiveMappings)
        {
            if (m.SourceType == sourceType && m.TargetType == targetType)
            {
                mapping = m;
                return true;
            }
        }
        mapping = default;
        return false;
    }
}
