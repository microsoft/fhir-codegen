// <copyright file="NpmPackageDetails.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Microsoft.Health.Fhir.CodeGenCommon.Models;

/// <summary>Information about the fhir package.</summary>
public class NpmPackageDetails
{
    /// <summary>Gets or sets the name.</summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }

    /// <summary>Gets or sets the version.</summary>
    [JsonPropertyName("version")]
    public string Version { get; set; }

    /// <summary>Gets or sets the build date.</summary>
    [JsonPropertyName("date")]
    public string BuildDate { get; set; }

    /// <summary>Gets or sets a list of fhir versions.</summary>
    [JsonPropertyName("fhir-version-list")]
    public IEnumerable<string> FhirVersionList { get; set; }

    /// <summary>Gets or sets the fhir versions.</summary>
    [JsonPropertyName("fhirVersions")]
    public IEnumerable<string> FhirVersions { get; set; }

    /// <summary>Gets or sets the type of the package.</summary>
    [JsonPropertyName("type")]
    public string PackageType { get; set; }

    /// <summary>Gets or sets the tools version.</summary>
    [JsonPropertyName("tools-version")]
    public decimal ToolsVersion { get; set; }

    /// <summary>Gets or sets the canonical.</summary>
    [JsonPropertyName("canonical")]
    public string Canonical { get; set; }

    /// <summary>Gets or sets the homepage.</summary>
    [JsonPropertyName("homepage")]
    public string Homepage { get; set; }

    /// <summary>Gets or sets URL of the document.</summary>
    [JsonPropertyName("url")]
    public Uri URL { get; set; }

    /// <summary>Gets or sets the title.</summary>
    [JsonPropertyName("title")]
    public string Title { get; set; }

    /// <summary>Gets or sets the description.</summary>
    [JsonPropertyName("description")]
    public string Description { get; set; }

    /// <summary>Gets or sets the dependencies.</summary>
    [JsonPropertyName("dependencies")]
    public Dictionary<string, string> Dependencies { get; set; }

    /// <summary>Gets or sets the keywords.</summary>
    [JsonPropertyName("keywords")]
    public IEnumerable<string> Keywords { get; set; }

    /// <summary>Gets or sets the author.</summary>
    [JsonPropertyName("author")]
    public string Author { get; set; }

    /// <summary>Gets or sets the license.</summary>
    [JsonPropertyName("license")]
    public string License { get; set; }

    /// <summary>Gets or sets the directories.</summary>
    [JsonPropertyName("directories")]
    public Dictionary<string, string> Directories { get; set; }

    /// <summary>Gets or sets the original version.</summary>
    [JsonPropertyName("original-version")]
    public string OriginalVersion { get; set; }

    /// <summary>Gets or sets a value indicating whether this object is loaded.</summary>
    public bool IsLoaded { get; set; }

    /// <summary>Attempts to load FHIR NPM package information from the given directory.</summary>
    /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
    /// <exception cref="FileNotFoundException">Thrown when the requested file is not present.</exception>
    /// <exception cref="JsonException">        Thrown when a JSON error condition occurs.</exception>
    /// <param name="location">Pathname of the package directory.</param>
    /// <returns>The package information.</returns>
    public static NpmPackageDetails Load(string location)
    {
        if (string.IsNullOrEmpty(location))
        {
            throw new ArgumentNullException(nameof(location));
        }

        string packageFilename;

        if (location.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
        {
            packageFilename = location;
        }
        else if (File.Exists(Path.Combine(location, "package.json")))
        {
            packageFilename = Path.Combine(location, "package.json");
        }
        else if (File.Exists(Path.Combine(location, "package", "package.json")))
        {
            packageFilename = Path.Combine(location, "package", "package.json");
        }
        else
        {
            throw new FileNotFoundException($"Package file not found at location: {location}!");
        }

        // make sure our file exists
        if (!File.Exists(packageFilename))
        {
            throw new FileNotFoundException($"Package file not found at location: {location}!");
        }

        // load the file
        string packageContents = File.ReadAllText(packageFilename);

        // attempt to parse
        try
        {
            NpmPackageDetails details = JsonSerializer.Deserialize<NpmPackageDetails>(packageContents);

            if (details.FhirVersionList == null)
            {
                details.FhirVersionList = details.FhirVersions;
            }
            else if (details.FhirVersions == null)
            {
                details.FhirVersions = details.FhirVersionList;
            }

            return details;
        }
        catch (JsonException)
        {
            throw;
        }
    }

    /// <summary>Parses.</summary>
    /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
    /// <exception cref="JsonException">        Thrown when a JSON error condition occurs.</exception>
    /// <param name="contents">The contents.</param>
    /// <returns>The NpmPackageDetails.</returns>
    public static NpmPackageDetails Parse(string contents)
    {
        if (string.IsNullOrEmpty(contents))
        {
            throw new ArgumentNullException(nameof(contents));
        }

        // attempt to parse
        try
        {
            NpmPackageDetails details = JsonSerializer.Deserialize<NpmPackageDetails>(contents);

            if (details.FhirVersionList == null)
            {
                details.FhirVersionList = details.FhirVersions;
            }
            else if (details.FhirVersions == null)
            {
                details.FhirVersions = details.FhirVersionList;
            }

            return details;

        }
        catch (JsonException)
        {
            throw;
        }
    }
}
