// <copyright file="FhirImplementationGuide.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

namespace Microsoft.Health.Fhir.CodeGenCommon.Models;

/// <summary>
/// Class representing normalized ImplementationGuide resources.
/// </summary>
public class FhirImplementationGuide : FhirTypeBase, ICloneable
{
    public record struct IgDependsOn(
        string IgUri,
        string PackageId,
        string Version);

    private Dictionary<string, IgDependsOn> _dependsOn = new();

    public FhirImplementationGuide(
        string name,
        string id,
        Uri url,
        string version,
        string publicationStatus,
        string standardStatus,
        int? fmmLevel,
        bool isExperimental,
        string shortDescription,
        string purpose,
        string comment,
        string packageId,
        string fhirVersion,
        Dictionary<string, IgDependsOn> dependsOn)
        : base(
            id,
            name,
            url,
            publicationStatus,
            standardStatus,
            fmmLevel,
            isExperimental,
            shortDescription,
            purpose,
            comment,
            string.Empty)
    {
        Version = version;
        PackageId = packageId;
        FhirVersion = fhirVersion;
        _dependsOn = dependsOn;
    }

    public string PackageId { get; }

    public string FhirVersion { get; }

    public string Version { get; }

    public Dictionary<string, IgDependsOn> DependsOn => _dependsOn;

    /// <summary>Makes a deep copy of this object.</summary>
    /// <returns>A copy of this object.</returns>
    public object Clone()
    {
        return new FhirImplementationGuide(
            Name,
            Id,
            URL,
            Version,
            PublicationStatus,
            StandardStatus,
            FhirMaturityLevel,
            IsExperimental,
            ShortDescription,
            Purpose,
            Comment,
            PackageId,
            FhirVersion,
            DependsOn);
    }
}
