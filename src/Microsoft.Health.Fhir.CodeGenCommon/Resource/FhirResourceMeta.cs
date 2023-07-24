// <copyright file="FhirResourceMeta.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

namespace Microsoft.Health.Fhir.CodeGenCommon.Resource;

/// <summary>
/// FHIR resource meta.
/// </summary>
public class FhirResourceMeta : ICloneable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FhirResourceMeta"/> class.
    /// </summary>
    public FhirResourceMeta() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="FhirResourceMeta"/> class.
    /// </summary>
    /// <param name="other">The other.</param>
    public FhirResourceMeta(FhirResourceMeta other)
    {
        VersionId = other.VersionId;
        LastUpdated = other.LastUpdated;
        Source = other.Source;
        Profiles = other.Profiles.Any() ? other.Profiles.Select(v => v) : Enumerable.Empty<string>();
        SecurityLabels = other.SecurityLabels.Any() ? other.SecurityLabels.Select(v => v) : Enumerable.Empty<string>();
        Tags = other.Tags.Any() ? other.Tags.Select(v => v) : Enumerable.Empty<string>();
    }

    /// <summary>
    /// Gets or sets the version specific identifier.
    /// </summary>
    public string VersionId { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets when the resource version last changed.
    /// </summary>
    public string LastUpdated { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets where the resource comes from.
    /// </summary>
    public string Source { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the profiles this resource claims conformance to.
    /// </summary>
    public IEnumerable<string> Profiles { get; init; } = Enumerable.Empty<string>();

    /// <summary>
    /// Gets or sets the security labels applied to this resource.
    /// </summary>
    public IEnumerable<string> SecurityLabels { get; init; } = Enumerable.Empty<string>();

    /// <summary>
    /// Gets or sets the tags applied to this resource.
    /// </summary>
    public IEnumerable<string> Tags { get; init; } = Enumerable.Empty<string>();

    /// <summary>Creates a new object that is a copy of the current instance.</summary>
    /// <returns>A new object that is a copy of this instance.</returns>
    object ICloneable.Clone() => new FhirResourceMeta(this);
}
