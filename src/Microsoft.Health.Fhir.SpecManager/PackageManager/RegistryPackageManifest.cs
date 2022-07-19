// <copyright file="RegistryPackageManifest.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.Text.Json;
using System.Text.Json.Serialization;

namespace Microsoft.Health.Fhir.SpecManager.PackageManager;

/// <summary>Information about the package version.</summary>
public class RegistryPackageManifest
{
    /// <summary>Gets or sets the identifier.</summary>
    [JsonPropertyName("_id")]
    public string Id { get; set; }

    /// <summary>Gets or sets the name.</summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }

    /// <summary>Gets or sets the description.</summary>
    [JsonPropertyName("description")]
    public string Description { get; set; }

    /// <summary>Gets or sets the distribution tags.</summary>
    [JsonPropertyName("dist-tags")]
    public Dictionary<string, string> DistributionTags { get; set; }

    /// <summary>Gets or sets the versions.</summary>
    [JsonPropertyName("versions")]
    public Dictionary<string, VersionInfo> Versions { get; set; }

    /// <summary>Parses.</summary>
    /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
    /// <exception cref="JsonException">        Thrown when a JSON error condition occurs.</exception>
    /// <param name="json">The JSON.</param>
    /// <returns>A RegistryPackageInfo.</returns>
    public static RegistryPackageManifest Parse(string json)
    {
        if (string.IsNullOrEmpty(json))
        {
            throw new ArgumentNullException(nameof(json));
        }

        // attempt to parse
        try
        {
            RegistryPackageManifest manifest = JsonSerializer.Deserialize<RegistryPackageManifest>(json);

            // filter for garbage (packages2.fhir.org)
            if ((manifest != null) &&
                (manifest.Versions != null))
            {
                List<string> keysToRemove = new();

                foreach ((string key, VersionInfo info) in manifest.Versions)
                {
                    if ((info.FhirVersion == "??") ||
                        (info.PackageKind == "??"))
                    {
                        keysToRemove.Add(key);
                    }
                }

                keysToRemove.ForEach((key) => manifest.Versions.Remove(key));
            }

            return manifest;
        }
        catch (JsonException)
        {
            throw;
        }
    }

    /// <summary>Query if 'first' is first higher version.</summary>
    /// <param name="first"> The first.</param>
    /// <param name="second">The second.</param>
    /// <returns>True if first higher version, false if not.</returns>
    public static bool IsFirstHigherVersion(string first, string second)
    {
        if (string.IsNullOrEmpty(second))
        {
            return true;
        }

        if (string.IsNullOrEmpty(first))
        {
            return false;
        }

        string[] componentsF = first.Split('.');
        string[] componentsS = second.Split('.');

        int stopAt = Math.Min(componentsF.Length, componentsS.Length);

        for (int i = 0; i < stopAt; i++)
        {
            if (int.TryParse(componentsF[i], out int f) &&
                int.TryParse(componentsS[i], out int s))
            {
                if (f > s)
                {
                    return true;
                }

                if (f < s)
                {
                    return false;
                }
            }
            else
            {
                int comp = string.CompareOrdinal(componentsF[i], componentsS[i]);

                if (comp > 0)
                {
                    return true;
                }

                if (comp < 0)
                {
                    return false;
                }
            }
        }

        // if both are equal as far as they can, the longer one is higher
        return componentsF.Length > componentsS.Length;
    }

    /// <summary>Gets the highest version.</summary>
    /// <returns>The highest version for this package.</returns>
    public string HighestVersion()
    {
        string highestVersion = string.Empty;

        foreach (string version in Versions.Keys)
        {
            if (IsFirstHigherVersion(version, highestVersion))
            {
                highestVersion = version;
            }
        }

        return highestVersion;
    }

    /// <summary>Information about the version.</summary>
    public class VersionInfo
    {
        /// <summary>Gets or sets the name.</summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>Gets or sets the date.</summary>
        [JsonPropertyName("date")]
        public string Date { get; set; }

        /// <summary>Gets or sets the version.</summary>
        [JsonPropertyName("version")]
        public string Version { get; set; }

        /// <summary>Gets or sets the description.</summary>
        [JsonPropertyName("description")]
        public string Description { get; set; }

        /// <summary>Gets or sets URL of the document.</summary>
        [JsonPropertyName("url")]
        public Uri URL { get; set; }

        /// <summary>Gets or sets the distribution.</summary>
        [JsonPropertyName("dist")]
        public DistributionInfo Distribution { get; set; }

        /// <summary>Gets or sets the FHIR version.</summary>
        [JsonPropertyName("fhirVersion")]
        public string FhirVersion { get; set; }

        /// <summary>Gets or sets the unlisted.</summary>
        [JsonPropertyName("unlisted")]
        public string Unlisted { get; set; }

        /// <summary>Gets or sets the canonical.</summary>
        [JsonPropertyName("canonical")]
        public Uri Canonical { get; set; }

        /// <summary>Gets or sets the kind.</summary>
        [JsonPropertyName("kind")]
        public string PackageKind { get; set; }

        /// <summary>Gets or sets the count. </summary>
        [JsonPropertyName("count")]
        public string Count { get; set; }

        /// <summary>Information about the distribution.</summary>
        public class DistributionInfo
        {
            /// <summary>Gets or sets the hash sha.</summary>
            [JsonPropertyName("shasum")]
            public string HashSHA { get; set; }

            /// <summary>Gets or sets URL of the tarball.</summary>
            [JsonPropertyName("tarball")]
            public string TarballUrl { get; set; }
        }
    }
}
