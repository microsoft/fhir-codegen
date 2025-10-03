using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace Fhir.CodeGen.Packages.Models;

/// <summary>FHIR QA record from the CI server.</summary>
public record class FhirCiQaRecord
{
    [JsonPropertyName("url")]
    public string? Url { get; init; } = null;

    [JsonPropertyName("name")]
    public string? Name { get; init; } = null;

    [JsonPropertyName("title")]
    public string? Title { get; init; } = null;

    [JsonPropertyName("description")]
    public string? Description { get; init; } = null;

    [JsonPropertyName("status")]
    public string? Status { get; init; } = null;

    [JsonPropertyName("package-id")]
    public string? PackageId { get; init; } = null;

    [JsonPropertyName("ig-ver")]
    public string? PackageVersion { get; init; } = null;

    [JsonPropertyName("date")]
    [JsonConverter(typeof(Converters.FhriCiBuildDateConverter))]
    public DateTimeOffset? BuildDate { get; init; } = null;

    [JsonPropertyName("dateISO8601")]
    public DateTimeOffset? BuildDateIso { get; init; } = null;

    [JsonPropertyName("errs")]
    public int? ErrorCount { get; init; } = null;

    [JsonPropertyName("warnings")]
    public int? WarningCount { get; init; } = null;

    [JsonPropertyName("hints")]
    public int? HintCount { get; init; } = null;

    [JsonPropertyName("suppressed-hints")]
    public int? SuppressedHintCount { get; init; } = null;

    [JsonPropertyName("suppressed-warnings")]
    public int? SuppressedWarningCount { get; init; } = null;

    [JsonPropertyName("version")]
    public string? FhirVersion { get; init; } = null;

    [JsonPropertyName("tool")]
    public string? ToolingVersion { get; init; } = null;

    [JsonPropertyName("maxMemory")]
    public long? MaxMemoryUsedToBuild { get; init; } = null;

    [JsonPropertyName("repo")]
    public string? RepositoryUrl { get; init; } = null;


    public (string org, string project, string? branch)? ParseGitHubRepo()
    {
        if (string.IsNullOrEmpty(RepositoryUrl))
        {
            return null;
        }

        int orgProjectSepLoc = RepositoryUrl.IndexOf('/');
        if (orgProjectSepLoc == -1)
        {
            return null;
        }

        int orgStartLoc = 0;
        int orgEndLoc = orgProjectSepLoc;

        int projectStartLoc = orgProjectSepLoc + 1;
        int projectEndLoc;

        int projectEndSepLoc = RepositoryUrl.IndexOf('/', projectStartLoc);
        if (projectEndSepLoc == -1)
        {
            projectEndLoc = RepositoryUrl.Length;
        }
        else
        {
            projectEndLoc = projectEndSepLoc;
        }

        int branchLiteralLoc = RepositoryUrl.IndexOf("/branches/", StringComparison.Ordinal);
        if (branchLiteralLoc == -1)
        {
            return (
                RepositoryUrl[orgStartLoc..orgEndLoc],
                RepositoryUrl[projectStartLoc..projectEndLoc],
                null);
        }

        int branchStartLoc = branchLiteralLoc + 10;
        int branchEndLoc;

        if (RepositoryUrl.EndsWith("/qa.json", StringComparison.Ordinal))
        {
            branchEndLoc = RepositoryUrl.Length - 8;
        }
        else if (RepositoryUrl.EndsWith('/'))
        {
            branchEndLoc= RepositoryUrl.Length - 1;
        }
        else
        {
            branchEndLoc = RepositoryUrl.Length;
        }

        return (
            RepositoryUrl[orgStartLoc..orgEndLoc],
            RepositoryUrl[projectStartLoc..projectEndLoc],
            RepositoryUrl[branchStartLoc..branchEndLoc]);
    }
}
