using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Fhir.CodeGen.Packages.Converters;

namespace Fhir.CodeGen.Packages.Models;

/// <summary>
/// A full package manifest as returned by registries.
/// Note that this is not the same as the package.json file in a package.
/// </summary>
public record class FullPackageManifest
{
    [JsonPropertyName("_id")]
    public string? Id { get; init; } = null;

    public string? Revision { get; init; } = null;

    [JsonPropertyName("name")]
    public string? Name { get; init; } = null;

    [JsonPropertyName("description")]
    public string? Description { get; init; } = null;

    /// <summary>
    /// Distribution tags, like "latest" or "beta".
    /// </summary>
    /// <remarks>
    /// Key-value pairs where keys are tag names and values are version strings.
    /// </remarks>
    [JsonPropertyName("dist-tags")]
    public Dictionary<string, string>? DistributionTags { get; init; } = null;

    /// <summary>
    /// Recoded time of publication of versions.
    /// </summary>
    /// <remarks>
    /// Key-value pairs where keys are version strings and values are ISO 8601 date-time strings.
    /// </remarks>
    [JsonPropertyName("time")]
    public Dictionary<string, string>? PublicationTimes { get; init; } = null;

    [JsonPropertyName("versions")]
    public Dictionary<string, PackageManifest>? Versions { get; init; } = null;

    [JsonPropertyName("maintainers")]
    [JsonConverter(typeof(NpmPersonRecordConverter))]
    public IReadOnlyList<NpmPersonRecord>? Maintainers { get; init; } = null;

    [JsonPropertyName("contributors")]
    [JsonConverter(typeof(NpmPersonRecordConverter))]
    public IReadOnlyList<NpmPersonRecord>? Contributors { get; init; } = null;

    [JsonPropertyName("readme")]
    public string? Readme { get; init; } = null;

    [JsonPropertyName("readmeFilename")]
    public string? ReadmeFilename { get; init; } = null;

    [JsonPropertyName("license")]
    public string? License { get; init; } = null;

    [JsonPropertyName("homepage")]
    public string? HomePage { get; init; } = null;

    [JsonConverter(typeof(NpmRepositoryConverter))]
    [JsonPropertyName("repository")]
    public IReadOnlyList<NpmRepositoryRecord>? Repositories { get; init; } = null;

    [JsonPropertyName("keywords")]
    public List<string>? Keywords { get; init; } = null;

    [JsonConverter(typeof(NpmPersonRecordConverter))]
    [JsonPropertyName("author")]
    public IReadOnlyList<NpmPersonRecord>? Authors { get; init; } = null;

}
