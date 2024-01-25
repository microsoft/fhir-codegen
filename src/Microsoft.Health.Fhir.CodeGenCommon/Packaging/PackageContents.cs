// <copyright file="PackageContent.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.Text.Json.Serialization;

namespace Microsoft.Health.Fhir.CodeGenCommon.Packaging;

/// <summary>An index of the contents of a package.</summary>
public record class PackageContents
{
    /// <summary>A package content record.</summary>
    public record class PackageFile
    {
        /// <summary>Gets the filename for the file.</summary>
        [JsonPropertyName("filename")]
        public required string FileName { get; init; }

        /// <summary>Gets or initializes the type of the resource.</summary>
        [JsonPropertyName("resourceType")]
        public string ResourceType { get; init; } = string.Empty;

        /// <summary>Gets or initializes the identifier.</summary>
        /// <remarks>the id assigned to the resources. Note: resources SHOULD have an id, but in some workflows, none is assigned in the package</remarks>
        [JsonPropertyName("id")]
        public string Id { get; init; } = string.Empty;

        /// <summary>Gets or initializes URL of the canonical.</summary>
        /// <remarks>the canonical url, if the resource has one (e.g. a property "url" which is a primitive)</remarks>
        [JsonPropertyName("url")]
        public string Url { get; init; } = string.Empty;

        /// <summary>Gets the version.</summary>
        /// <remarks>the business version, if the resource has one (e.g. a property "version" which is a primitive)</remarks>
        [JsonPropertyName("version")]
        public string Version { get; init; } = string.Empty;

        /// <summary>Gets the kind.</summary>
        /// <remarks>the value of a the "kind" property in the resource, if it has one and it's a primitive</remarks>
        [JsonPropertyName("kind")]
        public string Kind { get; init; } = string.Empty;

        /// <summary>Gets the type.</summary>
        /// <remarks>the value of a the "type" property in the resource, if it has one and it's a primitive</remarks>
        [JsonPropertyName("type")]
        public string Type { get; init; } = string.Empty;
    }

    /// <summary>Gets or initializes the index version.</summary>
    [JsonPropertyName("index-version")]
    public required int IndexVersion { get; init; }

    /// <summary>Gets or initializes the files.</summary>
    [JsonPropertyName("files")]
    public IEnumerable<PackageFile> Files { get; init; } = Enumerable.Empty<PackageFile>();
}

