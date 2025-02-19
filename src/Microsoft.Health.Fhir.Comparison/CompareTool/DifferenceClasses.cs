// <copyright file="DifferenceClasses.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;
using Hl7.Fhir.Model;
using Microsoft.EntityFrameworkCore;
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

public class RelationshipLookup
{
    public required Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship Relationship { get; set; }
    public required string Name { get; set; }
}

public class DbFhirPackage
{
    [Key]
    public int Key { get; set; }

    public required string Name { get; set; } = null!;
    public required string PackageId { get; set; } = null!;
    public required string PackageVersion { get; set; } = null!;
    public required string CanonicalUrl { get; set; } = null!;

    public ICollection<DbFhirPackageComparisonPair> SourceDiffs { get; init; } = null!;

    public ICollection<DbFhirPackageComparisonPair> TargetDiffs { get; init; } = null!;

    public ICollection<DbValueSet> ValueSets { get; init; } = null!;
}

public class DbFhirPackageComparisonPair
{
    [Key]
    public int Key { get; set; }

    public int FhirPackageKey { get; set; }
    public DbFhirPackage FhirPackage { get; init; } = null!;

    public int TargetPackageKey { get; set; }
    //[ForeignKey(nameof(TargetPackageKey))]
    public DbFhirPackage TargetPackage { get; init; } = null!;
}

public abstract class DbCanonicalResource
{
    [Key]
    public int Key { get; set; }

    public int FhirPackageKey { get; set; }
    public required DbFhirPackage FhirPackage { get; init; } = null!;

    public required string Id { get; set; } = null!;
    public required string Url { get; set; } = null!;
    public required string Name { get; set; } = null!;
    public required string Version { get; set; } = null!;
    public required Hl7.Fhir.Model.PublicationStatus? Status { get; set; } = null;
    public required string? Title { get; set; } = null;
    public required string? Description { get; set; } = null;
    public required string? Purpose { get; set; } = null;
}


public class DbValueSet : DbCanonicalResource
{
    public required bool CanExpand { get; set; }
    public required bool? HasEscapeValveCode { get; set; } = null;
    public required string? Message { get; set; } = null;

    public required int BindingCountCore { get; set; } = -1;
    public required Hl7.Fhir.Model.BindingStrength? StrongestBindingCore { get; set; } = null;
    public required Hl7.Fhir.Model.BindingStrength? StrongestBindingCoreCode { get; set; } = null;
    public required Hl7.Fhir.Model.BindingStrength? StrongestBindingCoreCoding { get; set; } = null;

    public required int BindingCountExtended { get; set; } = -1;
    public required Hl7.Fhir.Model.BindingStrength? StrongestBindingExtended { get; set; } = null;
    public required Hl7.Fhir.Model.BindingStrength? StrongestBindingExtendedCode { get; set; } = null;
    public required Hl7.Fhir.Model.BindingStrength? StrongestBindingExtendedCoding { get; set; } = null;

    public ICollection<DbValueSetConcept> Concepts { get; init; } = null!;

    public ICollection<ValueSetPairComparison> ComparisonsAsSource { get; init; } = null!;

    public ICollection<ValueSetPairComparison> ComparisonsAsTarget { get; init; } = null!;
}

public class DbValueSetConcept
{
    [Key]
    public int Key { get; set; }

    public int ValueSetKey { get; set; }
    public required DbValueSet ValueSet { get; set; } = null!;

    public int FhirPackageKey { get; set; }
    public required DbFhirPackage FhirPackage { get; init; } = null!;


    public required string System { get; set; } = null!;
    public required string Code { get; set; } = null!;
    public required string? Display { get; set; } = null;
}

public class ValueSetPairComparison : IPairComparison<ValueSet>
{
    [Key]
    public int Key { get; set; }

    /// <summary>
    /// Gets or initializes the source element.
    /// </summary>
    [NotMapped]
    public ValueSet? Source { get; set; } = null;

    public int SourceVsMetaKey { get; set; }
    public required DbValueSet SourceVsMeta { get; init; } = null!;
    public required string SourceCanonical { get; set; } = null!;
    public required string SourceName { get; set; } = null!;
    public required string? SourceVersion { get; set; } = null;


    /// <summary>
    /// Gets or initializes the target element.
    /// </summary>
    [NotMapped]
    public ValueSet? Target { get; set; } = null;

    public int TargetVsMetaKey { get; set; }
    public required DbValueSet TargetVsMeta { get; init; } = null!;
    public required string? TargetCanonical { get; set; } = null;
    public required string? TargetName { get; set; } = null;
    public required string? TargetVersion { get; set; } = null;

    public required string CompositeName { get; set; } = null!;
    public required string TableName { get; set; } = null!;

    /// <summary>
    /// Gets or initializes the relationship between the source and target elements.
    /// </summary>
    public required Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship? Relationship { get; set; } = null;

    /// <summary>
    /// Gets or initializes the issue code for the comparison.
    /// </summary>
    public required ComparisonIssueCode? IssueCode { get; set; } = null;

    /// <summary>
    /// Gets or initializes the message describing the comparison.
    /// </summary>
    public required string Message { get; set; } = null!;

    /// <summary>
    /// Gets or initializes the concept map associated with the comparison.
    /// </summary>
    [NotMapped]
    public ConceptMap? Map { get; set; } = null;

    public required string? LastReviewedBy { get; set; } = null;
    public required DateTime? LastReviewedOn { get; set; } = null;

    public ICollection<ValueSetCodeComparisonRec> CodeComparisons { get; init; } = null!;
}

public class ValueSetCodeComparisonRec
{
    [Key]
    public int Key { get; set; }

    public int VsPairComparisonKey { get; set; }

    public required ValueSetPairComparison VsPairComparison { get; init; } = null!;

    /// <summary>
    /// Gets or initializes the source system.
    /// </summary>
    public required string SourceSystem { get; set; } = null!;

    /// <summary>
    /// Gets or initializes the source code.
    /// </summary>
    public required string SourceCode { get; set; } = null!;

    /// <summary>
    /// Gets or sets the source display.
    /// </summary>
    public required string? SourceDisplay { get; set; } = null;

    /// <summary>
    /// Gets or sets a value indicating whether there is no map.
    /// </summary>
    public required bool? NoMap { get; set; } = null;

    /// <summary>
    /// Gets or initializes the target system.
    /// </summary>
    public required string? TargetSystem { get; set; } = null;

    /// <summary>
    /// Gets or initializes the target code.
    /// </summary>
    public required string? TargetCode { get; set; } = null;

    /// <summary>
    /// Gets or sets the target display.
    /// </summary>
    public required string? TargetDisplay { get; set; } = null;

    /// <summary>
    /// Gets or sets the relationship.
    /// </summary>
    public required Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship? Relationship { get; set; } = null;

    /// <summary>
    /// Gets or sets the comment.
    /// </summary>
    public required string? Comment { get; set; } = null;

    /// <summary>
    /// Gets or sets a value indicating whether the record is generated.
    /// </summary>
    public required bool? IsGenerated { get; set; } = null;

    /// <summary>
    /// Gets or sets a value indicating whether the record needs review.
    /// </summary>
    public required bool? NeedsReview { get; set; } = null;
}



public class DbStructureDefinition : DbCanonicalResource
{
    public required string? Comment { get; set; } = null;
    public required string? Message { get; set; } = null;

    public required FhirArtifactClassEnum ArtifactClass { get; set; } = FhirArtifactClassEnum.Unknown;

    public ICollection<DbElementDefinition> Elements { get; init; } = null!;

    //public ICollection<ValueSetPairComparison> ComparisonsAsSource { get; init; } = null!;

    //public ICollection<ValueSetPairComparison> ComparisonsAsTarget { get; init; } = null!;
}


public class DbElementDefinition
{
    [Key]
    public int Key { get; set; }

    public int StructureKey { get; set; }
    public required DbStructureDefinition Structure { get; init; } = null!;


    public required int FieldOrder { get; set; } = -1;
    public required string Id { get; set; } = null!;
    public required string Path { get; set; } = null!;
}






/// <summary>
/// Represents a comparison between a source and target FHIR model element.
/// </summary>
/// <typeparam name="T">The type of the FHIR model element being compared.</typeparam>
/// <remarks>Used by FhirCoreComparer</remarks>
public interface IPairComparison<T>
    where T : Hl7.Fhir.Model.Base
{
    int Key { get; set; }

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
    Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship? Relationship { get; set; }

    /// <summary>
    /// Gets or initializes the issue code for the comparison.
    /// </summary>
    ComparisonIssueCode? IssueCode { get; set; }

    /// <summary>
    /// Gets or initializes the message describing the comparison.
    /// </summary>
    string Message { get; set; }

    /// <summary>
    /// Gets or initializes the concept map associated with the comparison.
    /// </summary>
    ConceptMap? Map { get; set; }

    string? LastReviewedBy { get; set; }
    DateTime? LastReviewedOn { get; set; }
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
