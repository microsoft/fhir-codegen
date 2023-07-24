// <copyright file="FhirCanonicalBase.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Microsoft.Health.Fhir.CodeGenCommon.Extensions;

namespace Microsoft.Health.Fhir.CodeGenCommon.Resource;

/// <summary>A FHIR canonical base.</summary>
public abstract class FhirCanonicalBase : FhirResourceBase
{
    private PublicationStatusCodes _publicationStatus = PublicationStatusCodes.Unknown;
    private string _fhirPublicationStatus = string.Empty;

    public enum PublicationStatusCodes
    {
        /// <summary>This resource is still under development and is not yet considered to be ready for normal use.</summary>
        [FhirLiteral("draft")]
        Draft,

        /// <summary>This resource is ready for normal use.</summary>
        [FhirLiteral("active")]
        Active,

        /// <summary>This resource has been withdrawn or superseded and should no longer be used.</summary>
        [FhirLiteral("retired")]
        Retired,

        /// <summary>
        /// The authoring system does not know which of the status values currently applies for this resource.
        /// Note: This concept is not to be used for "other" - one of the listed statuses is presumed to apply,
        /// it's just not known which one.
        /// </summary>
        [FhirLiteral("unknown")]
        Unknown,
    }

    public IEnumerable<FhirIdentifier> Identifiers { get; init; } = Enumerable.Empty<FhirIdentifier>();

    public string Version { get; init; } = string.Empty;

    public string VersionAlgorithmString { get; init; } = string.Empty;

    public FhirCoding? VersionAlgorithmCoding { get; init; } = null;

    public string Title { get; init; } = string.Empty;

    /// <summary>Gets or initializes the publication status.</summary>
    public PublicationStatusCodes PublicationStatus { get => _publicationStatus; }

    /// <summary>Gets or initializes the FHIR publication status.</summary>
    public required string FhirPublicationStatus
    {
        get => _fhirPublicationStatus;
        init
        {
            _fhirPublicationStatus = value;
            if (_fhirPublicationStatus.TryFhirEnum(out PublicationStatusCodes v))
            {
                _publicationStatus = v;
            }
        }
    }

    /// <summary>Gets or initializes whether this content is experimental.</summary>
    public bool IsExperimental { get; init; } = false;

    /// <summary>Gets or initializes the date this content was last changed.</summary>
    public string LastChanged { get; init; } = string.Empty;

    /// <summary>Gets or initializes the name of the publisher/steward (organization or individual).</summary>
    public string Publisher { get;init; } = string.Empty;

    /// <summary>Gets or initializes the natural language description of this content (Markdown).</summary>
    public string Description { get; init; } = string.Empty;

    /// <summary>Gets or initializes the context that the content is intended to support.</summary>
    public FhirUsageContext? UseContext { get; init; } = null;

    /// <summary>Gets or initializes the intended jurisdiction for this resource.</summary>
    public IEnumerable<FhirCodeableConcept> Jurisdictions { get; init; } = Enumerable.Empty<FhirCodeableConcept>();

    /// <summary>Gets or initializes the copyright (Markdown).</summary>
    public string Copyright { get; init; } = string.Empty;

    /// <summary>Gets or initializes the copyright label.</summary>
    public string CopyrightLabel { get; init; } = string.Empty;
}
