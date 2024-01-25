// <copyright file="FhirPackageVersionInfo.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Microsoft.Health.Fhir.CodeGenCommon.Packaging;

namespace Microsoft.Health.Fhir.PackageManager.Models;

/// <summary>FHIR Package version information.</summary>
internal record class FhirPackageVersionInfo
{
    private string _packageKind = string.Empty;
    private string _fhirVersion = string.Empty;
    private string _name = string.Empty;

    /// <summary>Initializes a new instance of the <see cref="FhirPackageVersionInfo"/> class.</summary>
    public FhirPackageVersionInfo() { }

    /// <summary>Initializes a new instance of the <see cref="FhirPackageVersionInfo"/> class.</summary>
    /// <param name="other">The instance to copy from.</param>
    [SetsRequiredMembers]
    protected FhirPackageVersionInfo(FhirPackageVersionInfo other)
    {
        _name = other._name;
        Date = other.Date;
        Version = other.Version;
        Description = other.Description;
        URL = other.URL;
        Distribution = (other.Distribution == null) ? null : other.Distribution with { };
        _fhirVersion = other._fhirVersion;
        Unlisted = other.Unlisted;
        Canonical = other.Canonical;
        Security = other.Security;
        _packageKind = other._packageKind;
        Count = other.Count;
    }

    /// <summary>Gets or sets the name.</summary>
    [JsonPropertyName("name")]
    public string Name
    {
        get => _name;
        set
        {
            _name = value;

            // if we are a core package, set what info we can
            if (FhirCache.PackageIsFhirCore(_name))
            {
                // we are a core package, set the package kind
                if (string.IsNullOrEmpty(_packageKind))
                {
                    _packageKind = "Core";
                }

                // if we do not have a FHIR version, try to grab one from the name
                if (string.IsNullOrEmpty(_fhirVersion))
                {
                    _fhirVersion = FhirReleases.FhirVersionToLiteral(_name.Split('.')[2]);
                }
            }
            else
            {
                // not core means we are an IG
                if (string.IsNullOrEmpty(_packageKind))
                {
                    _packageKind = "IG";
                }
            }
        }
    }

    /// <summary>Gets or sets the date.</summary>
    [JsonPropertyName("date")]
    public string Date { get; set; } = string.Empty;

    /// <summary>Gets or sets the version.</summary>
    [JsonPropertyName("version")]
    public string Version { get; set; } = string.Empty;

    /// <summary>Gets or sets the description.</summary>
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    /// <summary>Gets or sets URL of the document.</summary>
    [JsonPropertyName("url")]
    public Uri? URL { get; set; } = null;

    /// <summary>Gets or sets the distribution.</summary>
    [JsonPropertyName("dist")]
    public DistributionInfo? Distribution { get; set; } = null;

    /// <summary>Gets or sets the FHIR version.</summary>
    [JsonPropertyName("fhirVersion")]
    [JsonConverter(typeof(FhirVersionConverter))]
    public string FhirVersion
    {
        get => _fhirVersion;
        set
        {
            // check for the unknown literal and see if we can fix
            if (string.IsNullOrEmpty(value) || value.Equals("??", StringComparison.Ordinal))
            {
                // check to see if the name is a core package name
                if (FhirCache.PackageIsFhirCore(_name))
                {
                    _fhirVersion = FhirReleases.FhirVersionToLiteral(_name.Split('.')[2]);
                    return;
                }

                // cannot guess if we are not a core package and do not have a FHIR version
                return;
            }

            string temp = value.Trim();

            if (temp.StartsWith('['))
            {
                temp = temp.Replace("[", string.Empty).Replace("]", string.Empty);

                string[] versions = temp.Split(',');

                _fhirVersion = versions[0];

                return;
            }

            _fhirVersion = value;
        }
    }

    /// <summary>Gets or sets the unlisted.</summary>
    [JsonPropertyName("unlisted")]
    public string Unlisted { get; set; } = string.Empty;

    /// <summary>Gets or sets the canonical.</summary>
    [JsonPropertyName("canonical")]
    public Uri? Canonical { get; set; } = null;

    /// <summary>Gets or sets the security.</summary>
    [JsonPropertyName("security")]
    public string Security { get; set; } = string.Empty;

    /// <summary>Gets or sets the kind.</summary>
    [JsonPropertyName("kind")]
    public string PackageKind
    {
        get => _packageKind;
        set
        {
            // check for the unknown literal and see if we can fix
            if (string.IsNullOrEmpty(value) || value.Equals("??", StringComparison.Ordinal))
            {
                // check to see if the name is a core package name
                if (FhirCache.PackageIsFhirCore(_name))
                {
                    _packageKind = "Core";
                    return;
                }

                // not a core means we are an IG
                _packageKind = "IG";
                return;
            }

            _packageKind = value;
        }
    }

    /// <summary>Gets or sets the count. </summary>
    [JsonPropertyName("count")]
    public string Count { get; set; } = string.Empty;

    /// <summary>Information about the distribution.</summary>
    public record class DistributionInfo
    {
        /// <summary>Gets or sets the hash sha.</summary>
        [JsonPropertyName("shasum")]
        public string HashSHA { get; set; } = string.Empty;

        /// <summary>Gets or sets URL of the tarball.</summary>
        [JsonPropertyName("tarball")]
        public string TarballUrl { get; set; } = string.Empty;
    }
}
