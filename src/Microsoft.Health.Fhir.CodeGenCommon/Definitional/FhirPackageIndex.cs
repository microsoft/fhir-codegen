// <copyright file="FhirPackageIndex.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.Text.Json;
using System.Text.Json.Serialization;
using static Microsoft.Health.Fhir.CodeGenCommon.Models.FhirPackageIndex;

namespace Microsoft.Health.Fhir.CodeGenCommon.Definitional;

public record class FhirPackageIndex : ICloneable
{
    /// <summary>A package index file.</summary>
    public record class IndexFile : ICloneable
    {
        /// <summary>Gets or sets the filename of the file.</summary>
        [JsonPropertyName("filename")]
        public string Filename { get; set; } = string.Empty;

        /// <summary>Gets or sets the type of the resource.</summary>
        [JsonPropertyName("resourceType")]
        public string ResourceType { get; set; } = string.Empty;

        /// <summary>Gets or sets the identifier.</summary>
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        /// <summary>Gets or sets URL of the document.</summary>
        [JsonPropertyName("url")]
        public string Url { get; set; } = string.Empty;

        /// <summary>Gets or sets the version.</summary>
        [JsonPropertyName("version")]
        public string Version { get; set; } = string.Empty;

        /// <summary>Gets or sets the kind.</summary>
        [JsonPropertyName("kind")]
        public string Kind { get; set; } = string.Empty;

        /// <summary>Gets or sets the 'type' property (context dependant).</summary>
        [JsonPropertyName("type")]
        public string TypeHint { get; set; } = string.Empty;

        /// <summary>Makes a deep copy of this object.</summary>
        /// <returns>A copy of this object.</returns>
        object ICloneable.Clone() => this with { };
    }

    /// <summary>Gets or sets the index version.</summary>
    [JsonPropertyName("index-version")]
    public int IndexVersion { get; set; } = 0;

    /// <summary>Gets or sets the files.</summary>
    [JsonPropertyName("files")]
    public IEnumerable<PackageIndexFile> Files { get; set; } = Enumerable.Empty<PackageIndexFile>();

    /// <summary>Gets or sets the type of the files by resource.</summary>
    [JsonIgnore]
    public Dictionary<string, List<PackageIndexFile>> FilesByResourceType { get; set; } = new();


    /// <summary>Loads.</summary>
    /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
    /// <exception cref="FileNotFoundException">Thrown when the requested file is not present.</exception>
    /// <param name="location">The location to load.</param>
    /// <returns>A FhirPackageIndex.</returns>
    public static FhirPackageIndex Load(string location)
    {
        if (string.IsNullOrEmpty(location))
        {
            throw new ArgumentNullException(nameof(location));
        }

        string filename;

        if (location.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
        {
            filename = location;
        }
        else if (File.Exists(Path.Combine(location, ".index.json")))
        {
            filename = Path.Combine(location, ".index.json");
        }
        else if (File.Exists(Path.Combine(location, "package", ".index.json")))
        {
            filename = Path.Combine(location, "package", ".index.json");
        }
        else
        {
            throw new FileNotFoundException($"Package Index file not found at location: {location}!");
        }

        // make sure our file exists
        if (!File.Exists(filename))
        {
            throw new FileNotFoundException($"Package Index file not found at location: {location}!");
        }

        // load the file
        string contents = File.ReadAllText(filename);

        return Parse(contents);
    }

    /// <summary>Parses.</summary>
    /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
    /// <exception cref="Exception">            Thrown when an exception error condition occurs.</exception>
    /// <param name="contents">The contents.</param>
    /// <returns>A FhirPackageIndex.</returns>
    public static FhirPackageIndex Parse(string contents)
    {
        if (string.IsNullOrEmpty(contents))
        {
            throw new ArgumentNullException(nameof(contents));
        }

        FhirPackageIndex? packageIndex = null;

        // attempt to parse
        try
        {
            packageIndex = JsonSerializer.Deserialize<FhirPackageIndex>(contents, new JsonSerializerOptions()
            {
                AllowTrailingCommas = true,
            });

            if (packageIndex == null)
            {
                throw new Exception("Invalid FHIR Package Index");
            }

            foreach (PackageIndexFile pif in packageIndex.Files)
            {
                if (!packageIndex.FilesByResourceType.ContainsKey(pif.ResourceType))
                {
                    packageIndex.FilesByResourceType.Add(pif.ResourceType, new());
                }

                packageIndex.FilesByResourceType[pif.ResourceType].Add(pif);
            }
        }
        catch (JsonException jex)
        {
            Console.WriteLine($"FhirPackageIndex.Parse <<< caught JSON exception in typed parse: {jex.Message}");
            if (jex.InnerException != null)
            {
                Console.WriteLine($" <<< {jex.InnerException.Message}");
            }

            packageIndex = null;
        }

        if (packageIndex?.IndexVersion == null)
        {
            throw new Exception("Invalid FHIR Package Index");
        }

        return packageIndex;
    }

    /// <summary>Makes a deep copy of this object.</summary>
    /// <returns>A copy of this object.</returns>
    object ICloneable.Clone() => this with { };
}
