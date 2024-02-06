// <copyright file="RegistryPackageManifest.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.Health.Fhir.PackageManager.Models;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Health.Fhir.CodeGenCommon.Packaging;

namespace Microsoft.Health.Fhir.PackageManager.Models;

/// <summary>FHIR Registry package manifest record.</summary>
internal record class RegistryPackageManifest
{
    /// <summary>Initializes a new instance of the <see cref="RegistryPackageManifest"/> class.</summary>
    public RegistryPackageManifest() { }

    /// <summary>Initializes a new instance of the <see cref="RegistryPackageManifest"/> class.</summary>
    /// <param name="other">The instance to copy from.</param>
    [SetsRequiredMembers]
    protected RegistryPackageManifest(RegistryPackageManifest other)
    {
        Id = other.Id;
        Name = other.Name;
        Description = other.Description;
        DistributionTags = other.DistributionTags.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        Versions = other.Versions.ToDictionary(kvp => kvp.Key, kvp => kvp.Value with { });
    }

    /// <summary>Gets or sets the identifier.</summary>
    [JsonPropertyName("_id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>Gets or sets the name.</summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets the description.</summary>
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    /// <summary>Gets or sets the distribution tags.</summary>
    [JsonPropertyName("dist-tags")]
    public Dictionary<string, string> DistributionTags { get; set; } = new();

    /// <summary>Gets or sets the versions.</summary>
    [JsonPropertyName("versions")]
    public Dictionary<string, FhirPackageVersionInfo> Versions { get; set; } = new();

    /// <summary>Parses.</summary>
    /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
    /// <exception cref="JsonException">        Thrown when a JSON error condition occurs.</exception>
    /// <param name="json">The JSON.</param>
    /// <returns>A RegistryPackageInfo.</returns>
    public static RegistryPackageManifest? Parse(string json)
    {
        if (string.IsNullOrEmpty(json))
        {
            throw new ArgumentNullException(nameof(json));
        }

        // attempt to parse
        try
        {
            RegistryPackageManifest? manifest = JsonSerializer.Deserialize<RegistryPackageManifest>(json, new JsonSerializerOptions()
            {
                AllowTrailingCommas = true,
            });

            if (manifest?.Versions == null)
            {
                return null;
            }

            List<string> keysToRemove = new();

            foreach (string key in manifest.Versions.Keys)
            {
                FhirReleases.FhirSequenceCodes sequence = FhirReleases.FhirVersionToSequence(key);
                bool remove = false;
                string name = manifest.Versions[key].Name;

                if (string.IsNullOrEmpty(manifest.Versions[key].PackageKind) ||
                    (manifest.Versions[key].PackageKind == "??"))
                {
                    if (FhirCache.PackageIsFhirCore(name))
                    {
                        manifest.Versions[key].PackageKind = "Core";
                    }
                    else
                    {
                        remove = true;
                    }
                }

                if (string.IsNullOrEmpty(manifest.Versions[key].FhirVersion) ||
                    (manifest.Versions[key].FhirVersion == "??"))
                {
                    if (manifest.Versions[key].PackageKind.Equals("core", StringComparison.OrdinalIgnoreCase) &&
                        (sequence != FhirReleases.FhirSequenceCodes.Unknown))
                    {
                        manifest.Versions[key].FhirVersion = sequence.ToLiteral();
                    }
                    else
                    {
                        remove = true;
                    }
                }

                if (manifest.Versions[key].PackageKind.Equals("core", StringComparison.OrdinalIgnoreCase))
                {
                    manifest.Versions[key].FhirVersion = sequence.ToLiteral();
                }

                if (remove)
                {
                    keysToRemove.Add(key);
                }
            }

            keysToRemove.ForEach((key) => manifest.Versions.Remove(key));

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

        string[] versionSplitF = first.Split('-');
        string[] versionSplitS = second.Split('-');

        string[] componentsF = versionSplitF[0].Split('.');
        string[] componentsS = versionSplitS[0].Split('.');

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

        // two equal versions, check for known dev tags: http://hl7.org/fhir/versions.html
        //  no tag - release version
        //  "snapshotN" - a frozen release of a specification for connectathon, ballot dependencies or other reasons
        //  "ballotN" - a frozen release used in the ballot process
        //  "draftN" - a frozen release put out for non - ballot review or QA.
        //  "cibuild" - a 'special' release label that refers to a non - stable release that changes with each commit.

        if (versionSplitF.Length == 1)
        {
            return true;
        }

        if (versionSplitS.Length == 1)
        {
            return false;
        }

        // check for final draft
        if ((versionSplitF.Length > 2) &&
            (versionSplitF[2].Equals("final", StringComparison.Ordinal)))
        {
            return true;
        }

        // check for final draft
        if ((versionSplitS.Length > 2) &&
            (versionSplitS[2].Equals("final", StringComparison.Ordinal)))
        {
            return false;
        }

        if (versionSplitF[1].Equals("cibuild", StringComparison.Ordinal))
        {
            // cibuild is always 'lower'
            return false;
        }

        if (versionSplitS[1].Equals("cibuild", StringComparison.Ordinal))
        {
            // cibuild is always 'lower'
            return true;
        }

        int hashF = BuildHashForTag(versionSplitF[1]);
        int hashS = BuildHashForTag(versionSplitS[1]);

        if (hashF != hashS)
        {
            return hashF > hashS;
        }

        // if both are equal as far as they can, the longer one is higher
        return componentsF.Length > componentsS.Length;
    }

    /// <summary>Builds hash for tag.</summary>
    /// <param name="tag">The tag.</param>
    /// <returns>An int.</returns>
    private static int BuildHashForTag(string tag)
    {
        if (string.IsNullOrEmpty(tag))
        {
            return 0;
        }

        int hash;

        // "snapshotN" - a frozen release of a specification for connectathon, ballot dependencies or other reasons
        // "ballotN" - a frozen release used in the ballot process
        // "draftN" - a frozen release put out for non - ballot review or QA.
        // "cibuild" - a 'special' release label that refers to a non - stable release that changes with each commit.
        // "draft-final" - a frozen release put out for QA.

        switch (tag[0])
        {
            case 's':
                hash = 200;
                break;

            case 'b':
                hash = 300;
                break;

            case 'd':
                if (tag.Contains("final", StringComparison.Ordinal))
                {
                    hash = 400;
                }
                else
                {
                    hash = 100;
                }
                break;

            case 'c':
            default:
                hash = 0;
                break;
        }

        if (int.TryParse(tag.Substring(tag.Length - 1), out int sequence))
        {
            hash += sequence;
        }

        return hash;
    }

    /// <summary>Gets the highest version.</summary>
    /// <param name="versionRange">(Optional) The version range.</param>
    /// <returns>The highest version for this package.</returns>
    public string HighestVersion(string versionRange = "")
    {
        string highestVersion = string.Empty;
        string highestDate = string.Empty;
        string highestVersionByDate = string.Empty;

        string filter = versionRange.Replace(".x", "", StringComparison.OrdinalIgnoreCase);

        // check all versions we know about
        foreach (FhirPackageVersionInfo version in Versions.Values)
        {
            // if there was a version filter (root), skip if it does not match
            if (!string.IsNullOrEmpty(filter) &&
                !version.Version.StartsWith(filter, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            // always attempt to compare version strings
            if (IsFirstHigherVersion(version.Version, highestVersion))
            {
                highestVersion = version.Version;
            }

            // if both versions have a date compare with it
            if ((!string.IsNullOrEmpty(highestDate) && !string.IsNullOrEmpty(version.Date)) &&
                string.Compare(version.Date, highestDate, StringComparison.Ordinal) > 0)
            {
                highestDate = version.Date;
                highestVersionByDate = version.Version;
            }
        }

        // if we have a highest version by date, use it
        if (string.IsNullOrEmpty(highestVersionByDate))
        {
            return highestVersion;
        }

        return highestVersionByDate;
    }
}
