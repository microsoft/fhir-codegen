// <copyright file="FhirResourceMeta.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Health.Fhir.CodeGenCommon.Instance;

/// <summary>
/// FHIR resource meta.
/// </summary>
public record class FhirResourceMeta : ICloneable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FhirResourceMeta"/> class.
    /// </summary>
    public FhirResourceMeta() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="FhirResourceMeta"/> class.
    /// </summary>
    /// <param name="other">The other.</param>
    [SetsRequiredMembers]
    protected FhirResourceMeta(FhirResourceMeta other)
    {
        VersionId = other.VersionId;
        LastUpdated = other.LastUpdated;
        Source = other.Source;
        Profiles = other.Profiles.Select(v => v);
        SecurityLabels = other.SecurityLabels.Select(v => v);
        Tags = other.Tags.Select(v => v);
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

    /// <summary>Makes a deep copy of this object.</summary>
    /// <returns>A copy of this object.</returns>
    object ICloneable.Clone() => this with { };
}
