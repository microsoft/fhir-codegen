// <copyright file="FhirModelBase.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;

namespace Microsoft.Health.Fhir.CodeGenCommon.Models;

/// <summary>A FHIR canonical resource.</summary>
public abstract class FhirModelBase : FhirTypeBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FhirModelBase"/> class.
    /// </summary>
    /// <param name="artifactClass"> Type of artifact this complex object represents at its root.</param>
    /// <param name="id">               The identifier.</param>
    /// <param name="name">             The name.</param>
    /// <param name="path">             Path to this definition, if applicable.</param>
    /// <param name="baseTypeName">     Base type of this definition, if applicable.</param>
    /// <param name="baseTypeCanonical">Canonical url of the base definition, if applicable.</param>
    /// <param name="version">          Version of this definition.</param>
    /// <param name="url">              URL of the resource.</param>
    /// <param name="publicationStatus">The publication status.</param>
    /// <param name="standardStatus">   The standard status.</param>
    /// <param name="fmmLevel">         The fmm level.</param>
    /// <param name="isExperimental">   A value indicating whether this object is experimental.</param>
    /// <param name="shortDescription"> Information describing the short.</param>
    /// <param name="purpose">          The purpose.</param>
    /// <param name="comment">          The comment.</param>
    /// <param name="validationRegEx">  Validation regex pattern for this definition.</param>
    public FhirModelBase(
        FhirArtifactClassEnum artifactClass,
        string id,
        string name,
        string path,
        string baseTypeName,
        string baseTypeCanonical,
        string version,
        Uri url,
        string publicationStatus,
        string standardStatus,
        int? fmmLevel,
        bool isExperimental,
        string shortDescription,
        string purpose,
        string comment,
        string validationRegEx,
        string narrative,
        string narrativeStatus,
        string fhirVersion)
        : base(
            id,
            name,
            path,
            baseTypeName,
            baseTypeCanonical,
            url,
            shortDescription,
            purpose,
            comment,
            validationRegEx)
    {
        ArtifactClass = artifactClass;
        Url = url?.ToString() ?? string.Empty;
        PublicationStatus = publicationStatus;
        StandardStatus = standardStatus;
        FhirMaturityLevel = fmmLevel;
        IsExperimental = isExperimental;
        Version = version;
        NarrativeText = narrative;
        NarrativeStatus = narrativeStatus;
        FhirVersion = fhirVersion;
    }

    /// <summary>Initializes a new instance of the <see cref="FhirModelBase"/> class.</summary>
    /// <param name="source">Source to copy from.</param>
    public FhirModelBase(FhirModelBase source)
        : base(
            source.Id,
            source.Name,
            source.Path,
            source.BaseTypeName,
            source.BaseTypeCanonical,
            source.URL,
            source.ShortDescription,
            source.Purpose,
            source.Comment,
            source.ValidationRegEx)
    {
        ArtifactClass = source.ArtifactClass;
        Url = source.Url;
        PublicationStatus = source.PublicationStatus;
        StandardStatus = source.StandardStatus;
        FhirMaturityLevel = source.FhirMaturityLevel;
        IsExperimental = source.IsExperimental;
        Version = source.Version;
        NarrativeText = source.NarrativeText;
        NarrativeStatus = source.NarrativeStatus;
        FhirVersion = source.FhirVersion;
    }

    /// <summary>Gets the publication status.</summary>
    public string PublicationStatus { get; }

    /// <summary>
    /// Gets status of this type in the standards process
    /// see: http://hl7.org/fhir/valueset-standards-status.html.
    /// </summary>
    /// <value>The standard status.</value>
    public string StandardStatus { get; }

    /// <summary>Gets the FHIR maturity level.</summary>
    public int? FhirMaturityLevel { get; }

    /// <summary>Gets a value indicating whether this object is experimental.</summary>
    public bool IsExperimental { get; }

    /// <summary>Gets the artifact class of this definition.</summary>
    public FhirArtifactClassEnum ArtifactClass { get; }

    /// <summary>Gets the listed FHIR version.</summary>
    public string FhirVersion { get; }


    // **** NEW ****

    public string NarrativeText { get; }

    public string NarrativeStatus { get; }

    public string Url { get; }

    public string Version { get; }

    public string VersionAlgorithm { get; }

    /// <summary>Gets the title.</summary>
    public string Title => ShortDescription;

    /// <summary>Gets the description (markdown).</summary>
    public string Description => Purpose;
}
