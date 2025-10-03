using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace Fhir.CodeGen.Packages.Models;

/// <summary>Information about a CI branch, as returned from a branch query to the server.</summary>
public record class FhirCiBranchRecord
{
    /// <summary>The relative name for this record.</summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; } = null;

    /// <summary>The size of the directory or file.</summary>
    [JsonPropertyName("size")]
    public long? Size { get; init; } = null;

    /// <summary>URL of the resource, relative to the current URL.</summary>
    [JsonPropertyName("url")]
    public string? Url { get; init; } = null;

    /// <summary>The file/directory mode.</summary>
    /// <remarks>This looks like a flag, but I cannot find documentation on values.</remarks>
    [JsonPropertyName("mode")]
    public long? ModeFlag { get; init; } = null;

    /// <summary>True if is directory, false if not.</summary>
    [JsonPropertyName("is_dir")]
    public bool? IsDirectory { get; init; } = null;

    /// <summary>True if is symbolic link, false if not.</summary>
    [JsonPropertyName("is_symlink")]
    public bool? IsSymbolicLink { get; init; } = null;
}
