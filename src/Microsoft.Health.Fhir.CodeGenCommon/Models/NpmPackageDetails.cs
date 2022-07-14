// <copyright file="NpmPackageDetails.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.Text.Json;
using System.Text.Json.Nodes;
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

    /// <summary>Gets or sets the FHIR version.</summary>
    [JsonPropertyName("fhirVersion")]
    public string FhirVersion { get; set; }

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

        return Parse(packageContents);
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

        NpmPackageDetails details = null;

        // attempt to parse
        try
        {
            details = JsonSerializer.Deserialize<NpmPackageDetails>(contents, new JsonSerializerOptions()
            {
                AllowTrailingCommas = true,
            });
        }
        catch (JsonException jex)
        {
            Console.WriteLine($"NpmPackageDetails.Parse <<< caught JSON exception in typed parse: {jex.Message}");
            if (jex.InnerException != null)
            {
                Console.WriteLine($" <<< {jex.InnerException.Message}");
            }

            details = null;
        }

        if (details == null)
        {
            try
            {
                JsonNode node = JsonNode.Parse(contents);

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
                };

                string url = StringFromNode(node, "url");

                if (!string.IsNullOrEmpty(url))
                {
                    details.URL = new Uri(url);
                }
            }
            catch (JsonException jex)
            {
                Console.WriteLine($"NpmPackageDetails.Parse <<< caught JSON exception in untyped parse: {jex.Message}");
                if (jex.InnerException != null)
                {
                    Console.WriteLine($" <<< {jex.InnerException.Message}");
                }

                details = null;
            }
        }

        if ((details == null) || string.IsNullOrEmpty(details.Name))
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

        if (details.FhirVersionList == null)
        {
            if (details.FhirVersions != null)
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

        if (details.FhirVersions == null)
        {
            if (details.FhirVersionList != null)
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
    private static Dictionary<string, string> StringDictFromNode(JsonNode node, string prop)
    {
        if (node[prop] == null)
        {
            return null;
        }

        Dictionary<string, string> val = new();

        switch (node[prop])
        {
            case JsonObject obj:
                {
                    foreach ((string key, JsonNode objNode) in obj)
                    {
                        val.Add(key, StringFromNode(node[prop], key));
                    }
                }
                break;
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
    private static IEnumerable<string> EnumerableStringFromNode(JsonNode node, string prop)
    {
        if (node[prop] == null)
        {
            return null;
        }

        List<string> val = new();

        switch (node[prop])
        {
            case JsonArray ja:
                {
                    foreach (string item in ja)
                    {
                        val.Add(item);
                    }
                }
                break;

            default:
                val.Add(node[prop].ToString());
                break;
        }

        return val;
    }

    /// <summary>String from node.</summary>
    /// <param name="node">The node.</param>
    /// <param name="prop">The property.</param>
    /// <returns>A string.</returns>
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    private static string StringFromNode(JsonNode node, string prop)
    {
        if (node[prop] == null)
        {
            return string.Empty;
        }

        switch (node[prop])
        {
            case JsonArray ja:
                {
                    return ja[0].ToString();
                }
        }

        return node[prop].ToString();
    }
}
