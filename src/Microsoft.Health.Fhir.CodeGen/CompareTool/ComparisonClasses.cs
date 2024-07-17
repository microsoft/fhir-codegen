// <copyright file="ComparisonClasses.cs" company="Microsoft Corporation">
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
using Hl7.Fhir.Model;
using Microsoft.Health.Fhir.CodeGen.Utils;
using Microsoft.Health.Fhir.CodeGenCommon.Models;
using Microsoft.Health.Fhir.CodeGenCommon.Utils;
using CMR = Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship;

namespace Microsoft.Health.Fhir.CodeGen.CompareTool;

internal static class ComparisonUtils
{
    internal static string ForName(this string value) => FhirSanitizationUtils.SanitizeForProperty(value);
    internal static string ForVersion(this string value) => value.Replace('.', '_').Replace('-', '_');
    internal static string ForMdTable(this string value) => string.IsNullOrEmpty(value) ? string.Empty : value.Replace("|", "\\|").Replace("\n", "<br/>").Replace("\r", "<br/>");
}

public record class ComparisonBase
{
    public required CMR? Relationship { get; init; }
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
        if (ConceptComparisons.Count == 0)
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
        if (string.IsNullOrEmpty(TargetTypeLiteral))
        {
            return "DoesNotExistInTarget";
        }

        return Relationship?.ToString() ?? "-";
    }
}

public record class StructureComparison : ComparisonTopLevelBase<StructureInfoRec>
{
    public required Dictionary<string, ElementComparison> ElementComparisons { get; init; }

    public override string GetStatusString() => ElementComparisons.Count == 0 ? "DoesNotExistInTarget" : Relationship?.ToString() ?? "-"; 
}

public record class ElementComparison : ComparisonBase
{
    public required ElementInfoRec Source { get; init; }
    public required List<ElementComparisonDetails> TargetMappings { get; init; }
    public override string GetStatusString() => TargetMappings.Count == 0 ? "DoesNotExistInTarget" : Relationship?.ToString() ?? "-";
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
    public required string Code { get; init; }
    public required string Description { get; init; }
}

public record class ValueSetInfoRec
{
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
