// <copyright file="FhirModelBase.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Microsoft.Health.Fhir.CodeGenCommon.Models;

namespace Microsoft.Health.Fhir.CodeGenCommon.Structural;

/// <summary>
/// A FHIR model base - the root class for complex objects such as Resources, DataTypes, etc.
/// </summary>
public abstract record class FhirModelBase : FhirDefinitionBase
{
    /// <summary>Initializes a new instance of the FhirModelBase class.</summary>
    /// <param name="other">The other.</param>
    protected FhirModelBase(FhirModelBase other)
        : base(other)
    {
        PublicationStatus = other.PublicationStatus;
        StandardStatus = other.StandardStatus;
        FhirMaturityLevel = other.FhirMaturityLevel;
        IsExperimental = other.IsExperimental;
        ArtifactClass = other.ArtifactClass;
        FhirVersion = other.FhirVersion;
        NarrativeText = other.NarrativeText;
        NarrativeStatus = other.NarrativeStatus;
        Version = other.Version;
        VersionAlgorithm = other.VersionAlgorithm;
    }

    /// <summary>Gets the publication status.</summary>
    public string PublicationStatus { get; init; } = string.Empty;

    /// <summary>
    /// Gets status of this type in the standards process
    /// see: http://hl7.org/fhir/valueset-standards-status.html.
    /// </summary>
    /// <value>The standard status.</value>
    public string StandardStatus { get; init; } = string.Empty;

    /// <summary>Gets the FHIR maturity level.</summary>
    public int? FhirMaturityLevel { get; init; } = null;

    /// <summary>Gets a value indicating whether this object is experimental.</summary>
    public bool? IsExperimental { get; init; } = null;

    /// <summary>Gets the artifact class of this definition.</summary>
    public required FhirArtifactClassEnum ArtifactClass { get; init; }

    /// <summary>Gets the listed FHIR version.</summary>
    public required string FhirVersion { get; init; }

    /// <summary>Gets or initializes the narrative text.</summary>
    public string NarrativeText { get; init; } = string.Empty;

    /// <summary>Gets or initializes the narrative status.</summary>
    public string NarrativeStatus { get; init; } = string.Empty;

    /// <summary>Gets or initializes the version.</summary>
    public string Version { get; init; } = string.Empty;

    /// <summary>Gets or initializes the version algorithm.</summary>
    public string VersionAlgorithm { get; init; } = string.Empty;

    /// <summary>Gets the title.</summary>
    public string Title => ShortDescription;

    /// <summary>Gets the description (markdown).</summary>
    public string Description => Purpose;
}
