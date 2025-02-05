// <copyright file="DifferenceClasses.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;
using fhir_codegen.SQLiteGenerator;
using Hl7.Fhir.Model;
using Microsoft.Health.Fhir.CodeGen.Utils;
using Microsoft.Health.Fhir.CodeGenCommon.Models;
using Microsoft.Health.Fhir.CodeGenCommon.Utils;

namespace Microsoft.Health.Fhir.Comparison.CompareTool;

internal static class ComparisonUtils
{
    internal static string ForName(this string value) => FhirSanitizationUtils.SanitizeForProperty(value);
    internal static string ForVersion(this string value) => value.Replace('.', '_').Replace('-', '_');
    internal static string ForMdTable(this string value) => string.IsNullOrEmpty(value) ? string.Empty : value.Replace("|", "\\|").Replace("\n", "<br/>").Replace("\r", "<br/>");
}

/// <summary>
/// Represents issue codes for comparisons.
/// </summary>
public enum ComparisonIssueCode
{
    ManuallyExcluded,
    NoTarget,
    CannotExpandSource,
    CannotExpandTarget,
    NoRequiredBindings,
    InvalidMap,
}

[CgSQLiteTable]
public partial class RelationshipLookup
{
    [CgSQLiteKey]
    public required Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship Relationship { get; set; }
    public required string Name { get; set; }
}

[CgSQLiteTable]
public partial class ComparisonMetadata
{
    [CgSQLiteKey]
    public required long Id { get; set; }

    public required string SourcePackageId { get; init; }
    public required string SourcePackageVersion { get; init; }

    public required string TargetPackageId { get; init; }
    public required string TargetPackageVersion { get; init; }

    public required string Name { get; init; }
    public required string PackageId { get; init; }
    public required string PackageVersion { get; init; }
    public required string CanonicalUrl { get; init; }
}

[CgSQLiteTable]
public partial class ValueSetMetadata
{
    [CgSQLiteKey]
    public required long Id { get; set; }
    public required string PackageId { get; set; }
    public required string PackageVersion { get; set; }

    public required string CanonicalUrl { get; set; }
    public required string Name { get; set; }
    public required string Version { get; set; }
    public required string TableName { get; set; }
    public required string Description { get; set; }
    public required bool CanExpand { get; set; }
    public required bool? HasEscapeValveCode { get; set; }
    public required string? Message { get; set; }
}

[CgSQLiteTable]
public partial class ValueSetContent
{
    [CgSQLiteKey]
    public required long Id { get; set; }
    [CgSQLiteForeignKey("ValueSetMetadata", "Id")]
    public required long ValueSetMetadataId { get; set; }

    public required string System { get; set; }
    public required string Code { get; set; }
    public required string? Display { get; set; }
}

/// <summary>
/// Represents a comparison between a source and target FHIR model element.
/// </summary>
/// <typeparam name="T">The type of the FHIR model element being compared.</typeparam>
/// <remarks>Used by FhirCoreComparer</remarks>
public interface PairComparison<T>
    where T : Hl7.Fhir.Model.Base
{
    long Id { get; set; }

    /// <summary>
    /// Gets or initializes the source element.
    /// </summary>
    T? Source { get; set; }

    string SourceCanonical { get; set; }
    string SourceName { get; set; }

    /// <summary>
    /// Gets or initializes the target element.
    /// </summary>
    T? Target { get; set; }

    string? TargetCanonical { get; set; }
    string? TargetName { get; set; }

    string CompositeName { get; set; }

    /// <summary>
    /// Gets or initializes the relationship between the source and target elements.
    /// </summary>
    Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship? Relationship { get; init; }

    /// <summary>
    /// Gets or initializes the issue code for the comparison.
    /// </summary>
    ComparisonIssueCode? IssueCode { get; init; }

    /// <summary>
    /// Gets or initializes the message describing the comparison.
    /// </summary>
    string Message { get; init; }

    /// <summary>
    /// Gets or initializes the concept map associated with the comparison.
    /// </summary>
    ConceptMap? Map { get; set; }

    string? LastReviewedBy { get; set; }
    DateTime? LastReviewedOn { get; set; }
}

[CgSQLiteTable]
public partial record class ValueSetPairComparison : PairComparison<ValueSet>
{
    [CgSQLiteKey]
    public required long Id { get; set; }

    /// <summary>
    /// Gets or initializes the source element.
    /// </summary>
    [CgSQLiteIgnore]
    public ValueSet? Source { get; set; } = null;

    public required string SourceCanonical { get; set; }
    public required string SourceName { get; set; }
    public required string? SourceVersion { get; set; }


    /// <summary>
    /// Gets or initializes the target element.
    /// </summary>
    [CgSQLiteIgnore]
    public ValueSet? Target { get; set; } = null;

    public required string? TargetCanonical { get; set; }
    public required string? TargetName { get; set; }
    public required string? TargetVersion { get; set; }

    public required string CompositeName { get; set; }
    public required string TableName { get; set; }

    /// <summary>
    /// Gets or initializes the relationship between the source and target elements.
    /// </summary>
    public required Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship? Relationship { get; init; }

    /// <summary>
    /// Gets or initializes the issue code for the comparison.
    /// </summary>
    public required ComparisonIssueCode? IssueCode { get; init; }

    /// <summary>
    /// Gets or initializes the message describing the comparison.
    /// </summary>
    public required string Message { get; init; }

    /// <summary>
    /// Gets or initializes the concept map associated with the comparison.
    /// </summary>
    [CgSQLiteIgnore]
    public ConceptMap? Map { get; set; } = null;

    public required string? LastReviewedBy { get; set; }
    public required DateTime? LastReviewedOn { get; set; }
}

/// <summary>
/// Represents a comparison record for a value set code.
/// </summary>
/// <remarks>Used by FhirCoreComparer</remarks>
[CgSQLiteTable]
public partial record class ValueSetCodeComparisonRec
{
    [CgSQLiteKey]
    public required long Id { get; set; }

    [CgSQLiteForeignKey("ValueSetPairComparison", "Id")]
    public required long ValueSetPairComparisonId { get; set; }

    /// <summary>
    /// Gets or initializes the source system.
    /// </summary>
    public required string SourceSystem { get; init; }

    /// <summary>
    /// Gets or initializes the source code.
    /// </summary>
    public required string SourceCode { get; init; }

    /// <summary>
    /// Gets or sets the source display.
    /// </summary>
    public required string? SourceDisplay { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether there is no map.
    /// </summary>
    public bool? NoMap { get; set; }

    /// <summary>
    /// Gets or initializes the target system.
    /// </summary>
    public string? TargetSystem { get; init; }

    /// <summary>
    /// Gets or initializes the target code.
    /// </summary>
    public string? TargetCode { get; init; }

    /// <summary>
    /// Gets or sets the target display.
    /// </summary>
    public string? TargetDisplay { get; set; }

    /// <summary>
    /// Gets or sets the relationship.
    /// </summary>
    public required Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship? Relationship { get; set; }

    /// <summary>
    /// Gets or sets the comment.
    /// </summary>
    public required string? Comment { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the record is generated.
    /// </summary>
    public required bool? IsGenerated { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the record needs review.
    /// </summary>
    public required bool? NeedsReview { get; set; }
}



public record class ComparisonBase
{
    public required Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship? Relationship { get; init; }
    public required string Message { get; init; }

    public virtual string GetStatusString() => Relationship?.ToString() ?? "-";
}

public record class ComparisonTopLevelBase<T> : ComparisonBase
{
    public required T Source { get; init; }
    public required T? Target { get; init; }
    public required string CompositeName { get; init; }
}

public record class ComparisonDetailsBase<T> : ComparisonBase
{
    public required T? Target { get; init; }
}

public record class ConceptComparisonDetails : ComparisonDetailsBase<ConceptInfoRec>
{
    /// <summary>
    /// Gets or initializes a value indicating whether this mapping is preferred.
    /// </summary>
    public bool IsPreferred { get; init; }
}

public record class ConceptComparison : ComparisonBase
{
    /// <summary>Gets or initializes the source concept in this comparison.</summary>
    public required ConceptInfoRec Source { get; init; }

    public required List<ConceptComparisonDetails> TargetMappings { get; init; }

    public override string GetStatusString() => TargetMappings.Count == 0 ? "DoesNotExistInTarget" : Relationship?.ToString() ?? "-";
}

public record class ValueSetComparison : ComparisonTopLevelBase<ValueSetInfoRec>
{
    /// <summary>Gets or initializes the concept comparisons, keyed by source concept.</summary>
    public required Dictionary<string, ConceptComparison> ConceptComparisons { get; init; }

    public override string GetStatusString()
    {
        if ((Target == null) || (ConceptComparisons.Count == 0))
        {
            return "DoesNotExistInTarget";
        }

        return Relationship?.ToString() ?? "-";
    }
}

public record class PrimitiveTypeComparison : ComparisonTopLevelBase<StructureInfoRec>
{
    public required string SourceTypeLiteral { get; init; }

    public required string TargetTypeLiteral { get; init; }

    public override string GetStatusString()
    {
        if (Target == null)
        {
            return "DoesNotExistInTarget";
        }

        return Relationship?.ToString() ?? "-";
    }
}

public record class StructureComparison : ComparisonTopLevelBase<StructureInfoRec>
{
    public required Dictionary<string, ElementComparison> ElementComparisons { get; init; }

    public override string GetStatusString()
    {
        if ((Target == null) || (ElementComparisons.Count == 0))
        {
            return "DoesNotExistInTarget";
        }

        return Relationship?.ToString() ?? "-";
    }
}

public record class ElementComparison : ComparisonBase
{
    public required ElementInfoRec Source { get; init; }
    public required List<ElementComparisonDetails> TargetMappings { get; init; }
    public override string GetStatusString()
    {
        if (TargetMappings.Count == 0)
        {
            return "DoesNotExistInTarget";
        }

        return Relationship?.ToString() ?? "-";
    }

}

public record class ElementComparisonDetails : ComparisonDetailsBase<ElementInfoRec>
{
    public required Dictionary<string, ElementTypeComparison> TypeComparisons { get; init; }
}

public record class ElementTypeComparison : ComparisonBase
{
    public required ElementTypeInfoRec Source { get; init; }
    public required List<ElementTypeComparisonDetails> TargetTypes { get; init; }
}

public record class ElementTypeComparisonDetails : ComparisonDetailsBase<ElementTypeInfoRec>
{
}


public record class ConceptInfoRec
{
    public required string System { get; init; }
    public required string Version { get; init; }
    public required string Code { get; init; }
    public required string Display { get; init; }
    public required string Description { get; init; }
}

public record class ValueSetInfoRec
{
    public required string Id { get; init; }
    public required string Url { get; init; }
    public required string Name { get; init; }
    public required string NamePascal { get; init; }
    public required string Title { get; init; }
    public required string Description { get; init; }
}

public record class ElementTypeInfoRec
{
    public required string Name { get; init; }
    public required List<string> Profiles { get; init; }
    public required List<string> TargetProfiles { get; init; }
}

public record class ElementInfoRec
{
    public required string Name { get; init; }
    public required string Path { get; init; }
    public required string Short { get; init; }
    public required string Definition { get; init; }
    public required int MinCardinality { get; init; }
    public required int MaxCardinality { get; init; }
    public required string MaxCardinalityString { get; init; }
    public required Hl7.Fhir.Model.BindingStrength? ValueSetBindingStrength { get; init; }
    public required string BindingValueSet { get; init; }
    public required Dictionary<string, ElementTypeInfoRec> Types { get; init; }
}

public record class StructureInfoRec
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Url { get; init; }
    public required string Title { get; init; }
    public required string Description { get; init; }
    public required string Purpose { get; init; }
    public required int SnapshotCount { get; init; }
    public required int DifferentialCount { get; init; }
}

public record class PackageComparison
{
    public required string LeftPackageId { get; init; }
    public required string LeftPackageVersion { get; init; }
    public required string RightPackageId { get; init; }
    public required string RightPackageVersion { get; init; }

    public required Dictionary<string, List<ValueSetComparison>> ValueSets { get; init; }
    public required Dictionary<string, List<PrimitiveTypeComparison>> PrimitiveTypes { get; init; }
    public required Dictionary<string, List<StructureComparison>> ComplexTypes { get; init; }
    public required Dictionary<string, List<StructureComparison>> Resources { get; init; }
    //public required Dictionary<string, ComparisonRecord<StructureInfoRec, ElementInfoRec, ElementTypeInfoRec>> LogicalModels { get; init; }
    public required Dictionary<string, List<StructureComparison>> Extensions { get; init; }
}
