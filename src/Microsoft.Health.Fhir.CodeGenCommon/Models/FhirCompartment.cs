// <copyright file="FhirCompartment.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>


using Microsoft.Health.Fhir.CodeGenCommon.Extensions;

namespace Microsoft.Health.Fhir.CodeGenCommon.Models;

public class FhirCompartment : FhirModelBase, ICloneable
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
        Unkown,
    }

    /// <summary>How a resource is related to the compartment.</summary>
    /// <param name="ResourceName"> Name of the resource type.</param>
    /// <param name="Parameters">   Search Parameter Names, or chained parameters.</param>
    /// <param name="Documentation">Additional documentation about the resource and compartment.</param>
    /// <param name="StartParamUri">Search Param for interpreting $everything.start.</param>
    /// <param name="EndParamUri">  Search Param for interpreting $everything.end.</param>
    public readonly record struct CompartmentResource(
        string ResourceName,
        IEnumerable<string> Parameters,
        string Documentation,
        string StartParamUri,
        string EndParamUri);

    /// <summary>
    /// Initializes a new instance of the <see cref="FhirCompartment"/> class.
    /// </summary>
    /// <param name="id">               The identifier.</param>
    /// <param name="name">             The name.</param>
    /// <param name="title">            Title of this compartment.</param>
    /// <param name="version">          Version of this definition.</param>
    /// <param name="versionAlgorithm"> Algorithm for the version information, if present</param>
    /// <param name="url">              URL of the resource.</param>
    /// <param name="publicationStatus">The publication status.</param>
    /// <param name="standardStatus">   The standard status.</param>
    /// <param name="fmmLevel">         The fmm level.</param>
    /// <param name="isExperimental">   A value indicating whether this object is experimental.</param>
    /// <param name="purpose">          The purpose.</param>
    /// <param name="description">      Natural language description of the compartment definition.</param>
    /// <param name="narrative">        Narrative content for this resource.</param>
    /// <param name="narrativeStatus">  Narrative status for the narrative of this resource.</param>
    /// <param name="compartmentType">  CompartmentType code.</param>
    /// <param name="isSearchSupported">If search is supported in this compartment.</param>
    public FhirCompartment(
        string id,
        string name,
        string title,
        string version,
        FhirConcept versionAlgorithm,
        Uri url,
        string publicationStatus,
        string standardStatus,
        int? fmmLevel,
        bool isExperimental,
        string purpose,
        string description,
        string narrative,
        string narrativeStatus,
        string compartmentType,
        bool isSearchSupported,
        Dictionary<string, CompartmentResource> compartmentResources)
        : base(
            FhirArtifactClassEnum.Compartment,
            id,
            name,
            string.Empty,
            string.Empty,
            string.Empty,
            version,
            url,
            publicationStatus,
            standardStatus,
            fmmLevel,
            isExperimental,
            title,
            purpose,
            description,
            string.Empty,
            narrative,
            narrativeStatus,
            string.Empty)
    {
        //VersionAlgorithm = versionAlgorithm;

        CompartmentTypeLiteral = compartmentType;

        if (compartmentType.TryFhirEnum(out CompartmentTypeCodes ct))
        {
            CompartmentType = ct;
        }
        else
        {
            CompartmentType = CompartmentTypeCodes.Unkown;
        }

        IsSearchSupported = isSearchSupported;
        ResourcesByType = compartmentResources;
    }

    /// <summary>Initializes a new instance of the <see cref="FhirCompartment"/> class.</summary>
    /// <param name="source">Source top copy from.</param>
    public FhirCompartment(FhirCompartment source)
        : base(source)
    {
        //VersionAlgorithm = new FhirConcept(source.VersionAlgorithm);
        CompartmentType = source.CompartmentType;
        CompartmentTypeLiteral = source.CompartmentTypeLiteral;
        IsSearchSupported = source.IsSearchSupported;
        ResourcesByType = source.ResourcesByType.ShallowCopy();
    }

    /// <summary>Gets the type of the compartment.</summary>
    public CompartmentTypeCodes CompartmentType { get; }

    /// <summary>Gets the compartment type literal.</summary>
    public string CompartmentTypeLiteral { get; }

    /// <summary>Gets a value indicating whether this object is search supported.</summary>
    public bool IsSearchSupported { get; }

    /// <summary>Gets the compartment resource records, by resource type.</summary>
    public Dictionary<string, CompartmentResource> ResourcesByType { get; }

    /// <summary>Makes a deep copy of this object.</summary>
    /// <returns>A copy of this object.</returns>
    public object Clone()
    {
        return new FhirCompartment(this);
    }
}
