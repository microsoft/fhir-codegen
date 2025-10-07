using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Fhir.CodeGen.Packages.Models;

public record class PackageIndex
{
    public record class IndexFile
    {
        /// <summary>
        /// The name of the file
        /// </summary>
        /// <remarks>
        /// This appears to be the bare filename, without any path information
        /// </remarks>
        [JsonPropertyName("filename")]
        public string? Filename { get; init; } = null;

        /// <summary>
        /// The type of the resource
        /// </summary>
        [JsonPropertyName("resourceType")]
        public string? ResourceType { get; init; } = null;

        /// <summary>
        /// The id assigned to the resources.
        /// </summary>
        /// <remarks>
        /// Note: resources SHOULD have an id, but in some workflows, none is assigned in the package
        /// </remarks>
        [JsonPropertyName("id")]
        public string? Id { get; init; } = null;

        /// <summary>
        /// The canonical url, if the resource has one (e.g. a property "url" which is a primitive)
        /// </summary>
        [JsonPropertyName("url")]
        public string? Url { get; init; } = null;

        /// <summary>
        /// The business version, if the resource has one (e.g. a property "version" which is a primitive)
        /// </summary>
        [JsonPropertyName("version")]
        public string? Version { get; init; } = null;

        /// <summary>
        /// The value of a the "kind" property in the resource, if it has one and it's a primitive
        /// </summary>
        [JsonPropertyName("kind")]
        public string? DeclaredKind { get; init; } = null;

        /// <summary>
        /// The value of a the "type" property in the resource, if it has one and it's a primitive
        /// </summary>
        [JsonPropertyName("type")]
        public string? DeclaredType { get; init; } = null;

        /// <summary>
        /// The value of the "content" property in the resource, if it has one
        /// </summary>
        [JsonPropertyName("content")]
        public string? DeclaredContent { get; init; } = null;
    }

    [JsonPropertyName("index-version")]
    public int? IndexVersion { get; init; } = null;

    [JsonPropertyName("files")]
    public List<IndexFile>? Files { get; init; } = null;
}
