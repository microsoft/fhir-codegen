// <copyright file="DiffResults.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

namespace Microsoft.Health.Fhir.SpecManager.Manager;

/// <summary>A difference results.</summary>
public class DiffResults
{
    /// <summary>Values that represent Difference type enums.</summary>
    public enum DiffTypeEnum
    {
        /// <summary>Indicates an element was not present in 'A' and exists in 'B'.</summary>
        Added,

        /// <summary>Indicates an element was in 'A' and does not exist in 'B'.</summary>
        Removed,

        /// <summary>Indicates an element is optional in 'A' and required in 'B'.</summary>
        MadeRequired,

        /// <summary>Indicates an element is required in 'A' and optional in 'B'.</summary>
        MadeOptional,

        /// <summary>Indicates an element has a min cardinality change between two required values (e.g., 1-2).</summary>
        ChangedMinCardinality,

        /// <summary>Indicates an element has been changed from an array in 'A' to a scalar in 'B'.</summary>
        MadeScalar,

        /// <summary>Indicates an element has been changed from a scalar in 'A' to an array in 'B'.</summary>
        MadeArray,

        /// <summary>Indicates an element has a different maximum array length in 'A' and 'B'.</summary>
        ChangedMaxCardinality,

        /// <summary>Indicates an element in 'B' has additional types compared with the same element in 'A'.</summary>
        AddedType,

        /// <summary>Indicates an element in 'B' has fewer types compared with the same element in 'A'.</summary>
        RemovedType,

        /// <summary>Indicates an element has different types in 'A' and 'B' (not simple overlap +/-).</summary>
        ChangedType,

        /// <summary>Indicates a change in the search parameter type.</summary>
        ChangedSearchParameterType,

        /// <summary>Indicates an element has the same type in 'A' and 'B', but additional Target Profiles in 'B'.</summary>
        AddedTargetProfile,

        /// <summary>Indicates an element has the same type in 'A' and 'B', but fewer Target Profiles in 'B'.</summary>
        RemovedTargetProfile,

        /// <summary>Indicates an element has the same type in 'A' and 'B', but different Target Profiles (not simple overlap +/-).</summary>
        ChangedTargetProfile,

        /// <summary>Indicates an element has the same type in 'A' and 'B', but additional Type Profiles in 'B'.</summary>
        AddedTypeProfile,

        /// <summary>Indicates an element has the same type in 'A' and 'B', but fewer Type Profiles in 'B'.</summary>
        RemovedTypeProfile,

        /// <summary>Indicates an element has the same type in 'A' and 'B', but different Type Profiles (not simple overlap +/-).</summary>
        ChangedTypeProfile,

        /// <summary>Indicates an element has changes in the validation RegEx expressions between 'A' and 'B'.</summary>
        ChangedRegEx,

        /// <summary>Indicates an element is the same in 'A' and 'B', but has modified descriptive text.</summary>
        ChangedDescription,

        /// <summary>Indicates a backbone element has a different explicit name in 'A' and 'B'.</summary>
        ChangedExplicitName,

        /// <summary>Indicates an element is in a different order.</summary>
        ChangedOrder,

        /// <summary>Indicates a change in modifier status flag.</summary>
        ChangedModifierFlag,

        /// <summary>Indicates a change in the summary flag.</summary>
        ChangedSummaryFlag,

        /// <summary>Indicates a change in the Must Support flag.</summary>
        ChangedMustSupportFlag,

        /// <summary>Indicates a change in a ValueSet binding strength.</summary>
        ChangedBindingStrength,

        /// <summary>Indicates a change in which ValueSet an element is bound to.</summary>
        ChangedBindingTarget,

        /// <summary>Indicates a change in the ID of a canonical resource with the same URL.</summary>
        ChangedId,

        /// <summary>Indicates a change in the scope.</summary>
        ChangedScope,

        /// <summary>Indicates the 'code' of an object has changed.</summary>
        ChangedCode,

        /// <summary>Indicates a change in XPath literal and/or usage.</summary>
        ChangedXPath,

        /// <summary>Indicates a change in Expressions.</summary>
        ChangedExpression,

        /// <summary>Indicates an expansion was added to the package.</summary>
        AddedExpansion,

        /// <summary>Indicates an expansion was removed from the package.</summary>
        RemovedExpansion,

        /// <summary>Indicates some change to the properties of an expansion.</summary>
        ChangedExpansion,

        /// <summary>Indicates the element has a new representation.</summary>
        AddedRepresentation,

        /// <summary>Indicates the element has removed a representation.</summary>
        RemovedRepresentation,

        /// <summary>Indicates the element.representation has changed.</summary>
        ChangedRepresentation,

        /// <summary>Indicates a definition has changed in standard status.</summary>
        ChangedStandardStatus,

        /// <summary>Indicates a definition has changed in FMM level.</summary>
        ChangedFmmLevel,
    }

    public readonly record struct DiffRecord(
        string path,
        DiffTypeEnum diffType,
        string valueA,
        string valueB);

    private Dictionary<FhirArtifactClassEnum, Dictionary<string, List<DiffRecord>>> _diffsByKeyByArtifactClass = new();

    private static readonly HashSet<FhirArtifactClassEnum> _validArtifactClasses = new()
    {
        FhirArtifactClassEnum.PrimitiveType,
        FhirArtifactClassEnum.ComplexType,
        FhirArtifactClassEnum.Resource,
        FhirArtifactClassEnum.Extension,
        FhirArtifactClassEnum.Operation,
        FhirArtifactClassEnum.SearchParameter,
        FhirArtifactClassEnum.ValueSet,
        FhirArtifactClassEnum.Profile,
        FhirArtifactClassEnum.LogicalModel,
    };

    /// <summary>
    /// Initializes a new instance of the <see cref="DiffResults"/> class.
    /// </summary>
    public DiffResults()
    {
        foreach (FhirArtifactClassEnum artifactClass in _validArtifactClasses)
        {
            _diffsByKeyByArtifactClass.Add(artifactClass, new());
        }
    }

    /// <summary>Gets the diffs by key by artifact class.</summary>
    public Dictionary<FhirArtifactClassEnum, Dictionary<string, List<DiffRecord>>> DiffsByKeyByArtifactClass => _diffsByKeyByArtifactClass;

    /// <summary>Adds a difference.</summary>
    /// <param name="artifactClass">The artifact class.</param>
    /// <param name="artifactKey">  The artifact key.</param>
    /// <param name="path">         Path to the difference (e.g., FHIR Element Path).</param>
    /// <param name="diffType">     Type of the diff.</param>
    /// <param name="valueA">       The value a.</param>
    /// <param name="valueB">       The value b.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public bool AddDiff(
        FhirArtifactClassEnum artifactClass,
        string artifactKey,
        string path,
        DiffTypeEnum diffType,
        string valueA,
        string valueB)
    {
        if (!_validArtifactClasses.Contains(artifactClass))
        {
            return false;
        }

        if (!_diffsByKeyByArtifactClass[artifactClass].ContainsKey(artifactKey))
        {
            _diffsByKeyByArtifactClass[artifactClass].Add(artifactKey, new());
        }

        _diffsByKeyByArtifactClass[artifactClass][artifactKey].Add(new DiffRecord(
            path,
            diffType,
            valueA,
            valueB));

        return true;
    }
}
