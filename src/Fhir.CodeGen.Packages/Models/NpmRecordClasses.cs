using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Fhir.CodeGen.Packages.Models;

/// <summary>
/// NPM expanded funding record type, string entry conversion is handled in the converter.
/// </summary>
public record class NpmFundingRecord
{
    [JsonPropertyName("type")]
    public string? Type { get; init; } = null;

    [JsonPropertyName("url")]
    public string? Url { get; init; } = null;
}

public record class NpmPersonRecord
{
    [JsonPropertyName("username")]
    public string? Username { get; init; } = null;

    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("email")]
    public string? Email { get; init; } = null;

    [JsonPropertyName("url")]
    public string? Url { get; init; } = null;

    [JsonPropertyName("_avatar")]
    public string? Avatar { get; init; } = null;
}

public record class NpmBugReportingRecord
{
    [JsonPropertyName("url")]
    public string? Url { get; init; } = null;

    [JsonPropertyName("email")]
    public string? Email { get; init; } = null;
}

public record class NpmRepositoryRecord
{
    [JsonPropertyName("type")]
    public string? Type { get; init; } = null;

    [JsonPropertyName("url")]
    public string? Url { get; init; } = null;

    [JsonPropertyName("directory")]
    public string? Directory { get; init; } = null;
}

public record class NpmManifestDistRecord
{
    public record class SignatureRecord
    {
        [JsonPropertyName("keyid")]
        public string? KeyId { get; init; } = null;

        [JsonPropertyName("sig")]
        public string? Signature { get; init; } = null;
    }

    [JsonPropertyName("shasum")]
    public string? ShaSum { get; init; } = null;

    [JsonPropertyName("tarball")]
    public string? TarballUrl { get; init; } = null;

    [JsonPropertyName("signatures")]
    public List<SignatureRecord>? Signatures { get; init; } = null;

    [JsonPropertyName("fileCount")]
    public int? FileCount { get; init; } = null;

    [JsonPropertyName("integrity")]
    public string? Integrity { get; init; } = null;

    [JsonPropertyName("unpackedSize")]
    public int? UnpackedSize { get; init; } = null;

    [JsonPropertyName("npm-signature")]
    public string? NpmSignature { get; init; } = null;
}
