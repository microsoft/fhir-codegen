// <copyright file="ComparisonAnnotation.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Hl7.Fhir.Model;
using Microsoft.Health.Fhir.CodeGen.FhirExtensions;
using Microsoft.Health.Fhir.CodeGen.Models;
using Microsoft.Health.Fhir.CodeGenCommon.Extensions;

namespace Microsoft.Health.Fhir.Comparison.XVer;

public enum ComparisonDirection
{
    Up,
    Down,
}


/// <summary>
/// Represents the failure codes for the comparison.
/// </summary>
public enum ComparisonFailureCodes
{
    UnresolvedTarget,
    CannotExpand,
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
