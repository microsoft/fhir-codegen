// <copyright file="IFhirConverter.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Microsoft.Health.Fhir.SpecManager.Manager;
using Microsoft.Health.Fhir.SpecManager.Models;

namespace Microsoft.Health.Fhir.SpecManager.Converters;

/// <summary>Interface for converter.</summary>
public interface IFhirConverter
{
    /// <summary>Try to parse a JSON string into a resource object.</summary>
    /// <param name="json">        The JSON.</param>
    /// <param name="resource">    [out].</param>
    /// <param name="resourceType">[out] Type of the resource.</param>
    /// <returns>A typed Resource object.</returns>
    bool TryParseResource(string json, out object resource, out string resourceType);

    /// <summary>Attempts to get the first resource from a bundle.</summary>
    /// <param name="json">        The JSON.</param>
    /// <param name="resource">    [out].</param>
    /// <param name="resourceType">[out] Type of the resource.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    bool TryGetFirstFromBundle(string json, out object resource, out string resourceType);

    /// <summary>Attempts to process resource.</summary>
    /// <param name="resourceToParse">The resource object.</param>
    /// <param name="fhirVersionInfo">Information describing the FHIR version.</param>
    void ProcessResource(
        object resourceToParse,
        IPackageImportable fhirVersionInfo);

    /// <summary>Attempts to process resource.</summary>
    /// <param name="resourceToParse">The resource object.</param>
    /// <param name="fhirVersionInfo">Information describing the FHIR version.</param>
    /// <param name="resourceCanonical">Canonical URL of the processed resource, or string.Empty if not processed.</param>
    /// <param name="artifactClass">  Class of the resource parsed</param>
    void ProcessResource(
        object resourceToParse,
        IPackageImportable fhirVersionInfo,
        out string resourceCanonical,
        out FhirArtifactClassEnum artifactClass);

    /// <summary>
    /// Replace a value in a parsed but not-yet processed resource
    /// </summary>
    /// <param name="resource"></param>
    /// <param name="path"></param>
    /// <param name="value"></param>
    void ReplaceValue(
        object resource,
        string[] path,
        object value);

    /// <summary>Process a FHIR metadata resource into Server Information.</summary>
    /// <param name="metadata">    The metadata resource object (e.g., r4.CapabilitiesStatement).</param>
    /// <param name="serverUrl">   URL of the server.</param>
    /// <param name="smartConfig"> The smart configuration.</param>
    /// <param name="capabilities">[out] Capabilities of a server.</param>
    void ProcessMetadata(
        object metadata,
        string serverUrl,
        SmartConfiguration smartConfig,
        out FhirCapabiltyStatement capabilities);

    /// <summary>Query if 'errorCount' has issues.</summary>
    /// <param name="errorCount">  [out] Number of errors.</param>
    /// <param name="warningCount">[out] Number of warnings.</param>
    /// <returns>True if issues, false if not.</returns>
    bool HasIssues(
        out int errorCount,
        out int warningCount);

    /// <summary>Displays the issues.</summary>
    void DisplayIssues();
}
