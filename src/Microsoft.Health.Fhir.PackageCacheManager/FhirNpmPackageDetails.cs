// <copyright file="FhirNpmPackageDetails.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Microsoft.Health.Fhir.PackageCacheManager;

/// <summary>
/// FHIR-NPM Package data.
/// </summary>
public class FhirNpmPackageDetails
{
    /// <summary>Gets or sets the name.</summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets the version.</summary>
    [JsonPropertyName("version")]
    public string Version { get; set; } = string.Empty;

    /// <summary>Gets or sets the build date.</summary>
    [JsonPropertyName("date")]
    public string BuildDate { get; set; } = string.Empty;

    /// <summary>Gets or sets a list of fhir versions.</summary>
    [JsonPropertyName("fhir-version-list")]
    public IEnumerable<string> FhirVersionList { get; set; } = Enumerable.Empty<string>();

    /// <summary>Gets or sets the fhir versions.</summary>
    [JsonPropertyName("fhirVersions")]
    public IEnumerable<string> FhirVersions { get; set; } = Enumerable.Empty<string>();

    /// <summary>Gets or sets the FHIR version.</summary>
    [JsonPropertyName("fhirVersion")]
    public string FhirVersion { get; set; } = string.Empty;

    /// <summary>Gets or sets the type of the package.</summary>
    [JsonPropertyName("type")]
    public string PackageType { get; set; } = string.Empty;

    /// <summary>Gets or sets the tools version.</summary>
    [JsonPropertyName("tools-version")]
    public decimal ToolsVersion { get; set; } = 0;

    /// <summary>Gets or sets the canonical.</summary>
    [JsonPropertyName("canonical")]
    public string Canonical { get; set; } = string.Empty;

    /// <summary>Gets or sets the homepage.</summary>
    [JsonPropertyName("homepage")]
    public string Homepage { get; set; } = string.Empty;

    /// <summary>Gets or sets URL of the document.</summary>
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    /// <summary>Gets or sets the title.</summary>
    [JsonPropertyName("title")]
    public string Title { get; set; } = string.Empty;

    /// <summary>Gets or sets the description.</summary>
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    /// <summary>Gets or sets the dependencies.</summary>
    [JsonPropertyName("dependencies")]
    public Dictionary<string, string> Dependencies { get; set; } = new();

    /// <summary>Gets or sets the keywords.</summary>
    [JsonPropertyName("keywords")]
    public IEnumerable<string> Keywords { get; set; } = Enumerable.Empty<string>();

    /// <summary>Gets or sets the author.</summary>
    [JsonPropertyName("author")]
    public string Author { get; set; } = string.Empty;

    /// <summary>Gets or sets the license.</summary>
    [JsonPropertyName("license")]
    public string License { get; set; } = string.Empty;

    /// <summary>Gets or sets the directories.</summary>
    [JsonPropertyName("directories")]
    public Dictionary<string, string> Directories { get; set; } = new();

    /// <summary>Gets or sets the original version.</summary>
    [JsonPropertyName("original-version")]
    public string OriginalVersion { get; set; } = string.Empty;

    /// <summary>Attempts to load FHIR NPM package information from the given directory.</summary>
    /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
    /// <exception cref="FileNotFoundException">Thrown when the requested file is not present.</exception>
    /// <exception cref="JsonException">        Thrown when a JSON error condition occurs.</exception>
    /// <param name="location">Pathname of the package directory.</param>
    /// <returns>The package information.</returns>
    public static FhirNpmPackageDetails Load(string location)
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

        return Parse(packageContents);
    }

    /// <summary>Parses.</summary>
    /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
    /// <exception cref="JsonException">        Thrown when a JSON error condition occurs.</exception>
    /// <param name="contents">The contents.</param>
    /// <returns>The NpmPackageDetails.</returns>
    public static FhirNpmPackageDetails Parse(string contents)
    {
        if (string.IsNullOrEmpty(contents))
        {
            throw new ArgumentNullException(nameof(contents));
        }

        FhirNpmPackageDetails? details = null;

        try
        {
            JsonNode? node = JsonNode.Parse(contents);

            if (node == null)
            {
                details = null;
            }
            else
            {
                details = new()
                {
                    Name = StringFromNode(node, "name"),
                    Version = StringFromNode(node, "version"),
                    BuildDate = StringFromNode(node, "date"),
                    FhirVersionList = EnumerableStringFromNode(node, "fhir-version-list"),
                    FhirVersions = EnumerableStringFromNode(node, "fhirVersions"),
                    FhirVersion = StringFromNode(node, "fhirVersion"),
                    PackageType = StringFromNode(node, "type"),
                    Canonical = StringFromNode(node, "canonical"),
                    Homepage = StringFromNode(node, "homepage"),
                    Title = StringFromNode(node, "title"),
                    Description = StringFromNode(node, "description"),
                    Dependencies = StringDictFromNode(node, "dependencies"),
                    Keywords = EnumerableStringFromNode(node, "keywords"),
                    Author = StringFromNode(node, "author"),
                    License = StringFromNode(node, "license"),
                    Directories = StringDictFromNode(node, "directories"),
                    OriginalVersion = StringFromNode(node, "original-version"),
                    Url = StringFromNode(node, "url"),
                };
            }
        }
        catch (JsonException jex)
        {
            Console.WriteLine($"FhirNpmPackageDetails.Parse <<< caught JSON exception in untyped parse: {jex.Message}");
            if (jex.InnerException != null)
            {
                Console.WriteLine($" <<< {jex.InnerException.Message}");
            }

            details = null;
        }
        //}

        if (string.IsNullOrEmpty(details?.Name))
        {
            throw new Exception("Invalid NPM Package Manifest");
        }

        if (!string.IsNullOrEmpty(details.FhirVersion))
        {
            if (details.FhirVersion.StartsWith('['))
            {
                details.FhirVersion = details.FhirVersion.Substring(1, details.FhirVersion.Length - 2);
            }
        }

        if (!details.FhirVersionList.Any())
        {
            if (details.FhirVersions.Any())
            {
                details.FhirVersionList = details.FhirVersions;
            }
            else if (!string.IsNullOrEmpty(details.FhirVersion))
            {
                details.FhirVersionList = new string[1] { details.FhirVersion };
            }
            else
            {
                details.FhirVersionList = new string[0];
            }
        }

        if (!details.FhirVersions.Any())
        {
            if (details.FhirVersionList.Any())
            {
                details.FhirVersions = details.FhirVersionList;
            }
            else if (!string.IsNullOrEmpty(details.FhirVersion))
            {
                details.FhirVersions = new string[1] { details.FhirVersion };
            }
            else
            {
                details.FhirVersions = new string[0];
            }
        }

        if (string.IsNullOrEmpty(details.FhirVersion))
        {
            if (details.FhirVersionList.Any())
            {
                details.FhirVersion = details.FhirVersionList.First();
            }
            else if (details.FhirVersions.Any())
            {
                details.FhirVersion = details.FhirVersions.First();
            }
            else
            {
                details.FhirVersion = string.Empty;
            }
        }

        if (details.Dependencies == null)
        {
            details.Dependencies = new();
        }

        return details;
    }

    /// <summary>String dictionary from node.</summary>
    /// <param name="node">The node.</param>
    /// <param name="prop">The property.</param>
    /// <returns>A Dictionary&lt;string,string&gt;</returns>
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    private static Dictionary<string, string> StringDictFromNode(JsonNode? node, string prop)
    {
        if (node?[prop] == null)
        {
            return new();
        }

        Dictionary<string, string> val = new();

        if (node[prop] is JsonObject obj)
        {
            foreach ((string key, object? oNode) in obj)
            {
                if (oNode == null)
                {
                    continue;
                }

                if (oNode is not JsonObject)
                {
                    val.Add(key, oNode.ToString() ?? string.Empty);
                    continue;
                }

                if (oNode is JsonNode jn)
                {
                    val.Add(key, StringFromNode(jn, key));
                    continue;
                }
            }
        }

        return val;
    }

    /// <summary>Enumerates enumerable string from node in this collection.</summary>
    /// <param name="node">The node.</param>
    /// <param name="prop">The property.</param>
    /// <returns>
    /// An enumerator that allows foreach to be used to process enumerable string from node in this
    /// collection.
    /// </returns>
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    private static IEnumerable<string> EnumerableStringFromNode(JsonNode? node, string prop)
    {
        if (node?[prop] == null)
        {
            return Enumerable.Empty<string>();
        }

        List<string> val = new();

        switch (node[prop])
        {
            case JsonArray ja:
                {
                    foreach (string? item in ja)
                    {
                        if (string.IsNullOrEmpty(item))
                        {
                            continue;
                        }

                        val.Add(item);
                    }
                }
                break;

            default:
                val.Add(node?[prop]?.ToString() ?? string.Empty);
                break;
        }

        return val;
    }

    /// <summary>String from node.</summary>
    /// <param name="node">The node.</param>
    /// <param name="prop">The property.</param>
    /// <returns>A string.</returns>
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    private static string StringFromNode(JsonNode? node, string prop)
    {
        if (node?[prop] == null)
        {
            return string.Empty;
        }

        if ((node[prop] is JsonArray ja) && ja.Any())
        {
            return ja.First()!.ToString();
        }

        return node[prop]!.ToString();
    }
}
