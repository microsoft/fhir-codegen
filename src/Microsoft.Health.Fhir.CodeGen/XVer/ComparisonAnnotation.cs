// <copyright file="ComparisonAnnotation.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Microsoft.Health.Fhir.CodeGen.XVer;


/// <summary>
/// Represents the failure codes for the comparison.
/// </summary>
public enum ComparisonFailureCodes
{
    UnresolvedTarget,
    CannotExpand,
}

/// <summary>
/// Represents the comparison details.
/// </summary>
public abstract record class ComparisonDetails<T>
{
    /// <summary>
    /// Gets or sets the target.
    /// </summary>
    public required T? Target { get; init; }

    /// <summary>
    /// Gets or sets the failure code for the comparison.
    /// </summary>
    /// <value>The failure code for the comparison.</value>
    public ComparisonFailureCodes? FailureCode { get; init; } = null;

    /// <summary>
    /// Gets or sets the failure message for the comparison.
    /// </summary>
    /// <value>The failure message for the comparison.</value>
    public string? FailureMessage { get; init; } = null;

    /// <summary>
    /// Gets or sets the explicit mapping source.
    /// </summary>
    public required string? ExplicitMappingSource { get; init; }

    /// <summary>
    /// Gets or sets the conceptual comparison for the concept domain.
    /// </summary>
    public required ConceptDomainComparison? ConceptDomain { get; init; }
}


/// <summary>
/// Represents the relationship codes for the concept domain.
/// </summary>
public enum ConceptDomainRelationshipCodes
{
    Unknown,
    Equivalent,
    SourceIsNew,
    SourceIsDeprecated,
    NotMapped,
    SourceIsNarrowerThanTarget,
    SourceIsBroaderThanTarget,
    Related,
    NotRelated,
}

/// <summary>
/// Represents the base class for domain comparisons.
/// </summary>
public abstract record class DomainComparison
{
    /// <summary>
    /// Gets or sets the messages.
    /// </summary>
    public List<string> Messages { get; init; } = [];
}

/// <summary>
/// Represents the comparison for the concept domain.
/// </summary>
public record class ConceptDomainComparison : DomainComparison
{
    /// <summary>
    /// Gets or sets the relationship code for the concept domain.
    /// </summary>
    public required ConceptDomainRelationshipCodes Relationship { get; init; }
}

/// <summary>
/// Represents the base class for value domain comparisons.
/// </summary>
public abstract record class ValueDomainComparison : DomainComparison
{
}


/// <summary>
/// Represents the flags for value set concept relationships.
/// </summary>
[Flags]
public enum ValueSetConceptRelationshipFlags : long
{
    None = 0,
    Equivalent = 1,
    Added = 2,
    Removed = 4,
    Renamed = 8,
    SystemChanged = 16,
}

/// <summary>
/// Represents the comparison for the value set domain.
/// </summary>
public record class ValueSetConceptValueDomain : ValueDomainComparison
{
    /// <summary>
    /// Gets or sets the concept relationship flags for the value set domain.
    /// </summary>
    public required ValueSetConceptRelationshipFlags ConceptRelationship { get; init; }
}

/// <summary>
/// Represents the comparison details for a value set concept.
/// </summary>
/// <typeparam name="T">The type of the target.</typeparam>
public record class ValueSetConceptComparisonDetails : ComparisonDetails<Hl7.Fhir.Model.ValueSet.ContainsComponent>
{
    /// <summary>
    /// Gets or sets the source value set concept.
    /// </summary>
    public required Hl7.Fhir.Model.ValueSet.ContainsComponent Source { get; init; }

    /// <summary>
    /// Gets or sets the value domain comparison for this ValueSet concept.
    /// </summary>
    public required ValueSetConceptValueDomain? ValueDomain { get; init; }
}

/// <summary>
/// Represents the comparison details for a value set.
/// </summary>
/// <typeparam name="T">The type of the target.</typeparam>
public record class ValueSetComparisonDetails : ComparisonDetails<Hl7.Fhir.Model.ValueSet>
{
    /// <summary>
    /// Gets or sets the dictionary of concept details for the value set.
    /// </summary>
    public required Dictionary<string, ValueSetConceptComparisonDetails[]>? ValueSetConcepts { get; init; }
}

// <summary>
// Represents a comparison annotation for a generic type.
// </summary>
/// <typeparam name="T">The type of the target.</typeparam>
public class ValueSetComparisonAnnotation
{
    /// <summary>
    /// Gets or sets the array of comparison details to the previous version.
    /// </summary>
    public List<ValueSetComparisonDetails> ToPrev { get; init; } = [];

    /// <summary>
    /// Gets or sets the array of comparison details to the next version.
    /// </summary>
    public List<ValueSetComparisonDetails> ToNext { get; init; } = [];

    /// <summary>
    /// Gets or sets the failure code for the comparison.
    /// </summary>
    /// <value>The failure code for the comparison.</value>
    public ComparisonFailureCodes? FailureCode { get; set; } = null;

    /// <summary>
    /// Gets or sets the failure message for the comparison.
    /// </summary>
    /// <value>The failure message for the comparison.</value>
    public string? FailureMessage { get; set; } = null;

    /// <summary>
    /// Gets or sets the escape valve codes.
    /// </summary>
    /// <value>The escape valve codes.</value>
    public List<string>? EscapeValveCodes { get; init; } = null;
}


/// <summary>
/// Represents the flags for structural relationships.
/// </summary>
[Flags]
public enum StructuralRelationshipFlags : long
{
    None = 0,
    Added = 1,
    Removed = 2,
    Renamed = 4,
    Moved = 8,
    ConvertedToBackbone = 16,
    ConvertedToSimpleProperty = 32,
    ConvertedToComplexProperty = 64,
    ConvertedToChoiceProperty = 128,
}

/// <summary>
/// Represents the flags for type relationships.
/// </summary>
[Flags]
public enum TypeRelationshipFlags : long
{
    None = 0,
    AddedType = 1,
    RemovedType = 2,
    ReplacedType = 4,
    AddedProfile = 8,
    RemovedProfile = 16,
    ReplacedProfile = 32,
}

/// <summary>
/// Represents the flags for cardinality relationships.
/// </summary>
[Flags]
public enum CardinalityRelationshipFlags : long
{
    None = 0,
    MadeRequired = 1,
    MadeOptional = 2,
    MadeArray = 4,
    MadeScalar = 8,
    MadeProhibited = 16,
    MadeAllowed = 32,
    ArrayLenReduced = 64,
    ArrayLenIncreased = 128,
}

/// <summary>
/// Represents the flags for binding relationships.
/// </summary>
[Flags]
public enum BindingRelationshipFlags : long
{
    None = 0,
    TargetContentsIncreased = 1,
    TargetContentsDecreased = 2,
    TargetContentsChanged = 4,
    StrengthIncreased = 8,
    StrengthDecreased = 16,
    UnresolvedChange = 32,
}

/// <summary>
/// Represents the comparison for the element definition domain.
/// </summary>
public record class ElementDefinitionValueDomain : ValueDomainComparison
{
    /// <summary>
    /// Gets or sets the structural relationship flags for the element definition domain.
    /// </summary>
    public required StructuralRelationshipFlags StructuralRelationship { get; init; }

    /// <summary>
    /// Gets or sets the type relationship flags for the element definition domain.
    /// </summary>
    public required TypeRelationshipFlags TypeRelationship { get; init; }

    /// <summary>
    /// Gets or sets the cardinality relationship flags for the element definition domain.
    /// </summary>
    public required CardinalityRelationshipFlags CardinalityRelationship { get; init; }

    /// <summary>
    /// Gets or sets the binding relationship flags for the element definition domain.
    /// </summary>
    public required BindingRelationshipFlags BindingRelationship { get; init; }
}

/// <summary>
/// Represents the comparison details for an element definition.
/// </summary>
/// <typeparam name="T">The type of the target.</typeparam>
public record class ElementDefinitionComparisonDetails : ComparisonDetails<Hl7.Fhir.Model.ElementDefinition>
{
    /// <summary>
    /// Gets or sets the value domain comparison for the element definition.
    /// </summary>
    public required ElementDefinitionValueDomain? ValueDomain { get; init; }
}

/// <summary>
/// Represents the comparison details for a structure definition.
/// </summary>
/// <typeparam name="T">The type of the target.</typeparam>
public record class StructureDefinitionComparisonDetails : ComparisonDetails<Hl7.Fhir.Model.StructureDefinition>
{
    /// <summary>
    /// Gets or sets the dictionary of element definition details for the structure definition.
    /// </summary>
    public required Dictionary<string, ElementDefinitionComparisonDetails> Elements { get; init; }
}


/// <summary>
/// Represents the comparison annotation for a structure definition.
/// </summary>
public record class StructureDefinitionComparisonAnnotation
{
    /// <summary>
    /// Gets or sets the array of comparison details to the previous version.
    /// </summary>
    public List<StructureDefinitionComparisonDetails> ToPrev { get; init; } = [];

    /// <summary>
    /// Gets or sets the array of comparison details to the next version.
    /// </summary>
    public List<StructureDefinitionComparisonDetails> ToNext { get; init; } = [];
}

public static class ComparisonExtensions
{
    public static ConceptDomainRelationshipCodes ToDomainRelationship(this Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship? cmr) => cmr switch
    {
        Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship.RelatedTo => ConceptDomainRelationshipCodes.Related,
        Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship.Equivalent => ConceptDomainRelationshipCodes.Equivalent,
        Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship.SourceIsNarrowerThanTarget => ConceptDomainRelationshipCodes.SourceIsNarrowerThanTarget,
        Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship.SourceIsBroaderThanTarget => ConceptDomainRelationshipCodes.SourceIsBroaderThanTarget,
        Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship.NotRelatedTo => ConceptDomainRelationshipCodes.Unknown,
        _ => ConceptDomainRelationshipCodes.Unknown
    };
}
