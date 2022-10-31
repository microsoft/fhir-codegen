// <copyright file="FhirCanonicalResource.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

namespace Microsoft.Health.Fhir.CodeGenCommon.Models;

/// <summary>A FHIR canonical resource.</summary>
public class FhirCanonicalResource //: FhirTypeBase
{
    public string Id { get; }

    public string NarrativeText { get; }

    public string NarrativeStatus { get; }

    public string Url { get; }

    public string Version { get; }

    public string VersionAlgorithm { get; }

    public string Name { get; }

    public string Title { get; }

    public string Status { get; }

    public bool IsExperimental { get; }

    /// <summary>Gets the description, as markdown text.</summary>
    public string Description { get; }
}
