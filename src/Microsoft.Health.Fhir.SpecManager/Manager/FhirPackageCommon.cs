// <copyright file="FhirPackageCommon.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

namespace Microsoft.Health.Fhir.SpecManager.Manager;

/// <summary>Information about the FHIR core package.</summary>
public static class FhirPackageCommon
{
        /// <summary>Extension URL for JSON type information.</summary>
    public const string UrlJsonType = "http://hl7.org/fhir/StructureDefinition/structuredefinition-json-type";

    /// <summary>Extension URL for XML type information.</summary>
    public const string UrlXmlType = "http://hl7.org/fhir/StructureDefinition/structuredefinition-xml-type";

    /// <summary>Extension URL for FHIR type information (added R4).</summary>
    public const string UrlFhirType = "http://hl7.org/fhir/StructureDefinition/structuredefinition-fhir-type";

    /// <summary>Values that represent FHIR package types.</summary>
    public enum FhirPackageTypeEnum
    {
        /// <summary>An enum constant representing an unknown package type.</summary>
        Unknown,

        /// <summary>An enum constant representing a FHIR core package.</summary>
        Core,

        /// <summary>An enum constant representing a FHIR Implementation Guide.</summary>
        IG,
    }

    /// <summary>Values that represent FHIR major releases.</summary>
    public enum FhirSequenceEnum : int
    {
        /// <summary>An enum constant representing the DSTU2 option.</summary>
        DSTU2,

        /// <summary>An enum constant representing the STU3 option.</summary>
        STU3,

        /// <summary>An enum constant representing the R4 option.</summary>
        R4,

        /// <summary>An enum constant representing the R4B option.</summary>
        R4B,

        /// <summary>An enum constant representing the R5 option.</summary>
        R5,
    }

    /// <summary>Information about the published release.</summary>
    private readonly record struct PublishedReleaseInformation(
        FhirSequenceEnum Major,
        DateOnly PublicationDate,
        string Version,
        string Description,
        string BallotPrefix = null);

    /// <summary>The FHIR releases.</summary>
    private static List<PublishedReleaseInformation> _fhirReleases = new()
    {
        new (FhirSequenceEnum.DSTU2, new DateOnly(2015, 10, 24), "1.0.2",           "DSTU2 Release with 1 technical errata"),
        new (FhirSequenceEnum.STU3,  new DateOnly(2019, 10, 24), "3.0.2",           "STU3 Release with 2 technical errata"),
        new (FhirSequenceEnum.R4,    new DateOnly(2019, 10, 30), "4.0.1",           "R4 Release with 1 technical errata"),
        new (FhirSequenceEnum.R4B,   new DateOnly(2021, 03, 11), "4.1.0",           "R4B Ballot #1", "2021Mar"),
        new (FhirSequenceEnum.R4B,   new DateOnly(2021, 12, 20), "4.3.0-snapshot1", "R4B January 2022 Connectathon"),
        new (FhirSequenceEnum.R5,    new DateOnly(2019, 12, 31), "4.2.0",           "R5 Preview #1", "2020Feb"),
        new (FhirSequenceEnum.R5,    new DateOnly(2020, 05, 04), "4.4.0",           "R5 Preview #2", "2020May"),
        new (FhirSequenceEnum.R5,    new DateOnly(2020, 08, 20), "4.5.0",           "R5 Preview #3", "2020Sep"),
        new (FhirSequenceEnum.R5,    new DateOnly(2021, 04, 15), "4.6.0",           "R5 Draft Ballot", "2021May"),
        new (FhirSequenceEnum.R5,    new DateOnly(2021, 12, 19), "5.0.0-snapshot1", "R5 January 2022 Connectathon"),
    };

    /// <summary>(Immutable) The ballot URL changeover date.</summary>
    private static readonly DateOnly _semverUrlChangeDate = new DateOnly(2021, 12, 01);

    /// <summary>The FHIR release by version.</summary>
    private static Dictionary<string, PublishedReleaseInformation> _fhirReleasesByVersion;

    /// <summary>The latest version by release.</summary>
    private static Dictionary<FhirSequenceEnum, string> _latestVersionByRelease;

    /// <summary>The package base by release.</summary>
    private static Dictionary<FhirSequenceEnum, string> _packageBaseByRelease;

    /// <summary>List of names of the core packages.</summary>
    private static Dictionary<string, FhirSequenceEnum> _corePackagesAndReleases;

    /// <summary>Types of resources to process, by FHIR version.</summary>
    private static Dictionary<FhirSequenceEnum, HashSet<string>> _versionResourcesToProcess = new()
    {
        {
            FhirSequenceEnum.DSTU2,
            new()
            {
                "OperationDefinition",
                "SearchParameter",
                "StructureDefinition",
                "ValueSet",
            }
        },
        {
            FhirSequenceEnum.STU3,
            new()
            {
                "CapabilityStatement",
                "CodeSystem",
                "NamingSystem",
                "OperationDefinition",
                "SearchParameter",
                "StructureDefinition",
                "ValueSet",
            }
        },
        {
            FhirSequenceEnum.R4,
            new()
            {
                "CapabilityStatement",
                "CodeSystem",
                "NamingSystem",
                "OperationDefinition",
                "SearchParameter",
                "StructureDefinition",
                "ValueSet",
            }
        },
        {
            FhirSequenceEnum.R4B,
            new()
            {
                "CapabilityStatement",
                "CodeSystem",
                "NamingSystem",
                "OperationDefinition",
                "SearchParameter",
                "StructureDefinition",
                "ValueSet",
            }
        },
        {
            FhirSequenceEnum.R5,
            new()
            {
                "CapabilityStatement",
                "CodeSystem",
                "NamingSystem",
                "OperationDefinition",
                "SearchParameter",
                "StructureDefinition",
                "ValueSet",
            }
        },
    };

    /// <summary>Types of resources to ignore, by FHIR version.</summary>
    private static Dictionary<FhirSequenceEnum, HashSet<string>> _versionResourcesToIgnore = new()
    {
        {
            FhirSequenceEnum.DSTU2,
            new()
            {
                "Conformance",
                "NamingSystem",
                "ConceptMap",
                "ImplementationGuide",
            }
        },
        {
            FhirSequenceEnum.STU3,
            new()
            {
                "CompartmentDefinition",
                "ConceptMap",
                "ImplementationGuide",
                "StructureMap",
            }
        },
        {
            FhirSequenceEnum.R4,
            new()
            {
                "CompartmentDefinition",
                "ConceptMap",
                "StructureMap",
            }
        },
        {
            FhirSequenceEnum.R4B,
            new()
            {
                "CompartmentDefinition",
                "ConceptMap",
                "StructureMap",
            }
        },
        {
            FhirSequenceEnum.R5,
            new()
            {
                "CompartmentDefinition",
                "ConceptMap",
                "StructureMap",
            }
        },
    };

    /// <summary>The version files to ignore.</summary>
    private static Dictionary<FhirSequenceEnum, HashSet<string>> _versionFilesToIgnore = new()
    {
        {
            FhirSequenceEnum.DSTU2,
            new()
        },
        {
            FhirSequenceEnum.STU3,
            new()
        },
        {
            FhirSequenceEnum.R4,
            new()
        },
        {
            FhirSequenceEnum.R4B,
            new()
        },
        {
            FhirSequenceEnum.R5,
            new()
        },
    };

    /// <summary>
    /// Initializes static members of the <see cref="FhirPackageCommon"/> class.
    /// </summary>
    static FhirPackageCommon()
    {
        _fhirReleasesByVersion = new();
        _latestVersionByRelease = new();

        foreach (PublishedReleaseInformation release in _fhirReleases)
        {
            _fhirReleasesByVersion.Add(release.Version, release);

            if (!_latestVersionByRelease.ContainsKey(release.Major))
            {
                _latestVersionByRelease.Add(release.Major, release.Version);
            }
            else if (release.PublicationDate.CompareTo(
                    _fhirReleasesByVersion[_latestVersionByRelease[release.Major]].PublicationDate) > 0)
            {
                _latestVersionByRelease[release.Major] = release.Version;
            }
        }

        _packageBaseByRelease = new();
        _corePackagesAndReleases = new();

        foreach (FhirSequenceEnum sequence in (FhirSequenceEnum[])Enum.GetValues(typeof(FhirSequenceEnum)))
        {
            switch (sequence)
            {
                case FhirSequenceEnum.DSTU2:
                    _packageBaseByRelease.Add(sequence, "hl7.fhir.r2");
                    _corePackagesAndReleases.Add("hl7.fhir.r2.core", sequence);
                    _corePackagesAndReleases.Add("hl7.fhir.r2.expansions", sequence);
                    break;

                case FhirSequenceEnum.STU3:
                    _packageBaseByRelease.Add(sequence, "hl7.fhir.r3");
                    _corePackagesAndReleases.Add("hl7.fhir.r3.core", sequence);
                    _corePackagesAndReleases.Add("hl7.fhir.r3.expansions", sequence);
                    break;

                default:
                    _packageBaseByRelease.Add(sequence, "hl7.fhir." + sequence.ToString().ToLowerInvariant());
                    _corePackagesAndReleases.Add(
                        $"hl7.fhir.{sequence.ToString().ToLowerInvariant()}.core",
                        sequence);
                    _corePackagesAndReleases.Add(
                        $"hl7.fhir.{sequence.ToString().ToLowerInvariant()}.expansions",
                        sequence);
                    break;
            }
        }
    }

    /// <summary>
    /// Attempts to get relative URL for version a string from the given string.
    /// </summary>
    /// <param name="version"> The version string.</param>
    /// <param name="relative">[out] The relative.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public static bool TryGetRelativeBaseForVersion(string version, out string relative)
    {
        if (!_fhirReleasesByVersion.ContainsKey(version))
        {
            relative = string.Empty;
            return false;
        }

        PublishedReleaseInformation info = _fhirReleasesByVersion[version];

        if (info.PublicationDate < _semverUrlChangeDate)
        {
            if (string.IsNullOrEmpty(info.BallotPrefix))
            {
                relative = $"{info.Major}";
                return true;
            }

            relative = $"{info.BallotPrefix}";
            return true;
        }

        relative = $"{version}";
        return true;
    }

    /// <summary>Package is FHIR core.</summary>
    /// <param name="packageName">Name of the package.</param>
    /// <returns>True if the package represents a FHIR Core package, false if it is not.</returns>
    public static bool PackageIsFhirCore(string packageName)
    {
        if (packageName.Contains('#'))
        {
            string name = packageName.Substring(0, packageName.IndexOf('#'));
            return _corePackagesAndReleases.ContainsKey(name);
        }

        return _corePackagesAndReleases.ContainsKey(packageName);
    }

    /// <summary>
    /// Attempts to get release by package a FhirSequence from the given string.
    /// </summary>
    /// <param name="packageName">Name of the package.</param>
    /// <param name="sequence">   [out] The sequence.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public static bool TryGetReleaseByPackage(string packageName, out FhirSequenceEnum sequence)
    {
        string name;

        if (packageName.Contains('#'))
        {
            name = packageName.Substring(0, packageName.IndexOf('#'));
        }
        else
        {
            name = packageName;
        }

        name = name.ToLowerInvariant();

        if (!name.EndsWith(".core", StringComparison.Ordinal))
        {
            name = name + ".core";
        }

        if (_corePackagesAndReleases.ContainsKey(name))
        {
            sequence = _corePackagesAndReleases[name];
            return true;
        }

        sequence = FhirSequenceEnum.DSTU2;
        return false;
    }

    /// <summary>Package base for release.</summary>
    /// <param name="major">The major.</param>
    /// <returns>A string.</returns>
    public static string PackageBaseForRelease(FhirSequenceEnum major)
    {
        return _packageBaseByRelease[major];
    }

    /// <summary>Latest version for release.</summary>
    /// <param name="major">The major.</param>
    /// <returns>The current highest release number for a sequence.</returns>
    public static string LatestVersionForRelease(FhirSequenceEnum major)
    {
        return _latestVersionByRelease[major];
    }

    /// <summary>Query if 'version' is known version.</summary>
    /// <param name="version">The version string.</param>
    /// <returns>True if the version string represents a known version of FHIR, false if not.</returns>
    public static bool IsKnownVersion(string version)
    {
        if (_fhirReleasesByVersion.ContainsKey(version))
        {
            return true;
        }

        return false;
    }

    /// <summary>Ballot prefix for version.</summary>
    /// <param name="version">The version string.</param>
    /// <returns>A string.</returns>
    public static string BallotPrefixForVersion(string version)
    {
        return _fhirReleasesByVersion[version].BallotPrefix;
    }

    /// <summary>Major for release.</summary>
    /// <param name="release">The release.</param>
    /// <returns>The sequence release number for a FHIR release (e.g., 2 for DSTU2).</returns>
    public static int MajorIntForVersion(FhirSequenceEnum release)
    {
        switch (release)
        {
            case FhirSequenceEnum.DSTU2:
                return 2;

            case FhirSequenceEnum.STU3:
                return 3;

            case FhirSequenceEnum.R4:
            case FhirSequenceEnum.R4B:
                return 4;

            case FhirSequenceEnum.R5:
                return 5;
        }

        return 0;
    }

    /// <summary>Major release for version.</summary>
    /// <exception cref="ArgumentNullException">      Thrown when one or more required arguments are
    ///  null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when one or more arguments are outside the
    ///  required range.</exception>
    /// <param name="version">The version string.</param>
    /// <returns>The FhirMajorRelease that should be used for the specified version.</returns>
    public static FhirSequenceEnum MajorReleaseForVersion(string version)
    {
        if (string.IsNullOrEmpty(version))
        {
            throw new ArgumentNullException(nameof(version));
        }

        if (_fhirReleasesByVersion.ContainsKey(version))
        {
            return _fhirReleasesByVersion[version].Major;
        }

        string val;

        if (version.Contains('.'))
        {
            val = version.Length > 2
                ? version.Substring(0, 3)
                : version.Substring(0, 1);
        }
        else
        {
            val = version;
        }

        // fallback to guessing
        switch (val)
        {
            case "DSTU2":
            case "STU2":
            case "R2":
            case "1.0":
            case "1":
            case "2.0":
            case "2":
                return FhirSequenceEnum.DSTU2;

            case "STU3":
            case "R3":
            case "3.0":
            case "3":
                return FhirSequenceEnum.STU3;

            case "R4":
            case "4":
            case "4.0":
                return FhirSequenceEnum.R4;

            case "R4B":
            case "4B":
            case "4.1":
            case "4.3":
                return FhirSequenceEnum.R4B;

            case "R5":
            case "4.2":
            case "4.4":
            case "4.5":
            case "4.6":
            case "5.0":
            case "5":
                return FhirSequenceEnum.R5;
        }

        throw new ArgumentOutOfRangeException($"Unknown FHIR version: {version}");
    }

    /// <summary>Determine if we should process resource.</summary>
    /// <param name="release">     The release.</param>
    /// <param name="resourceName">Name of the resource.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public static bool ShouldProcessResource(FhirSequenceEnum release, string resourceName)
    {
        if (_versionResourcesToProcess[release].Contains(resourceName))
        {
            return true;
        }

        return false;
    }

    /// <summary>Determine if we should ignore resource.</summary>
    /// <param name="release">     The release.</param>
    /// <param name="resourceName">Name of the resource.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public static bool ShouldIgnoreResource(FhirSequenceEnum release, string resourceName)
    {
        if (_versionResourcesToIgnore[release].Contains(resourceName))
        {
            return true;
        }

        return false;
    }

    /// <summary>Determine if we should skip file.</summary>
    /// <param name="release"> The release.</param>
    /// <param name="filename">Filename of the file.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public static bool ShouldSkipFile(FhirSequenceEnum release, string filename)
    {
        if (_versionFilesToIgnore[release].Contains(filename))
        {
            return true;
        }

        return false;
    }
}
