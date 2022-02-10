// -------------------------------------------------------------------------------------------------
// <copyright file="FhirVersionInfo.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>
// -------------------------------------------------------------------------------------------------

using Microsoft.Health.Fhir.SpecManager.Converters;
using Microsoft.Health.Fhir.SpecManager.Language;
using Microsoft.Health.Fhir.SpecManager.Models;

namespace Microsoft.Health.Fhir.SpecManager.Manager;

// TODO(ginoc): rename this class to FhirCoreInfo - need to change languages to use IFhirInfo instead of this object.

/// <summary>Information about a FHIR release.</summary>
public class FhirVersionInfo : FhirInfoBase, IFhirInfo
{
    /// <summary>Extension URL for JSON type information.</summary>
    public const string UrlJsonType = "http://hl7.org/fhir/StructureDefinition/structuredefinition-json-type";

    /// <summary>Extension URL for XML type information.</summary>
    public const string UrlXmlType = "http://hl7.org/fhir/StructureDefinition/structuredefinition-xml-type";

    /// <summary>Extension URL for FHIR type information (added R4).</summary>
    public const string UrlFhirType = "http://hl7.org/fhir/StructureDefinition/structuredefinition-fhir-type";

    /// <summary>Values that represent FHIR major releases.</summary>
    public enum FhirCoreVersion : int
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
        FhirCoreVersion Major,
        string PublicationDate,
        string Version,
        string Description,
        string BallotPrefix = null);

    /// <summary>The FHIR releases.</summary>
    private static List<PublishedReleaseInformation> _fhirReleases = new()
    {
        new (FhirCoreVersion.DSTU2, "2015-10-24", "1.0.2",           "DSTU2 Release with 1 technical errata"),
        new (FhirCoreVersion.STU3,  "2019-10-24", "3.0.2",           "STU3 Release with 2 technical errata"),
        new (FhirCoreVersion.R4,    "2019-10-30", "4.0.1",           "R4 Release with 1 technical errata"),
        new (FhirCoreVersion.R4B,   "2021-03-11", "4.1.0",           "R4B Ballot #1", "2021Mar"),
        new (FhirCoreVersion.R4B,   "2021-12-20", "4.3.0-snapshot1", "R4B January 2022 Connectathon"),
        new (FhirCoreVersion.R5,    "2019-12-31", "4.2.0",           "R5 Preview #1", "2020Feb"),
        new (FhirCoreVersion.R5,    "2020-05-04", "4.4.0",           "R5 Preview #2", "2020May"),
        new (FhirCoreVersion.R5,    "2020-08-20", "4.5.0",           "R5 Preview #3", "2020Sep"),
        new (FhirCoreVersion.R5,    "2021-04-15", "4.6.0",           "R5 Draft Ballot", "2021May"),
        new (FhirCoreVersion.R5,    "2021-12-19", "5.0.0-snapshot1", "R5 January 2022 Connectathon"),
    };

    /// <summary>The FHIR release by version.</summary>
    private static Dictionary<string, PublishedReleaseInformation> _fhirReleasesByVersion;

    /// <summary>The latest version by release.</summary>
    private static Dictionary<FhirCoreVersion, string> _latestVersionByRelease;

    /// <summary>Types of resources to process, by FHIR version.</summary>
    private static Dictionary<FhirCoreVersion, HashSet<string>> _versionResourcesToProcess = new ()
    {
        {
            FhirCoreVersion.DSTU2,
            new()
            {
                "OperationDefinition",
                "SearchParameter",
                "StructureDefinition",
                "ValueSet",
            }
        },
        {
            FhirCoreVersion.STU3,
            new ()
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
            FhirCoreVersion.R4,
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
            FhirCoreVersion.R4B,
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
            FhirCoreVersion.R5,
            new ()
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
    private static Dictionary<FhirCoreVersion, HashSet<string>> _versionResourcesToIgnore = new ()
    {
        {
            FhirCoreVersion.DSTU2,
            new ()
            {
                "Conformance",
                "NamingSystem",
                "ConceptMap",
                "ImplementationGuide",
            }
        },
        {
            FhirCoreVersion.STU3,
            new ()
            {
                "CompartmentDefinition",
                "ConceptMap",
                "ImplementationGuide",
                "StructureMap",
            }
        },
        {
            FhirCoreVersion.R4,
            new ()
            {
                "CompartmentDefinition",
                "ConceptMap",
                "StructureMap",
            }
        },
        {
            FhirCoreVersion.R4B,
            new()
            {
                "CompartmentDefinition",
                "ConceptMap",
                "StructureMap",
            }
        },
        {
            FhirCoreVersion.R5,
            new ()
            {
                "CompartmentDefinition",
                "ConceptMap",
                "StructureMap",
            }
        },
    };

    /// <summary>The version files to ignore.</summary>
    private static Dictionary<FhirCoreVersion, HashSet<string>> _versionFilesToIgnore = new ()
    {
        {
            FhirCoreVersion.DSTU2,
            new()
        },
        {
            FhirCoreVersion.STU3,
            new ()
        },
        {
            FhirCoreVersion.R4,
            new ()
            {
                //"ValueSet-cpt-all.json",
                //"ValueSet-example-filter.json",
            }
        },
        {
            FhirCoreVersion.R4B,
            new()
        },
        {
            FhirCoreVersion.R5,
            new ()
        },
    };

    private static HashSet<string> _npmFilesToIgnore = new ()
    {
        ".index.json",
        "package.json",
        "StructureDefinition-example.json",
    };

    private IFhirConverter _fhirConverter;
    private Dictionary<string, FhirPrimitive> _primitiveTypesByName;
    private Dictionary<string, FhirComplex> _complexTypesByName;
    private Dictionary<string, FhirComplex> _resourcesByName;
    private Dictionary<string, FhirComplex> _extensionsByUrl;
    private Dictionary<string, Dictionary<string, FhirComplex>> _extensionsByPath;
    private Dictionary<string, FhirComplex> _profilesById;
    private Dictionary<string, Dictionary<string, FhirComplex>> _profilesByBaseType;
    private Dictionary<string, FhirOperation> _systemOperations;
    private Dictionary<string, FhirSearchParam> _globalSearchParameters;
    private Dictionary<string, FhirSearchParam> _searchResultParameters;
    private Dictionary<string, FhirSearchParam> _allInteractionParameters;
    private Dictionary<string, FhirCodeSystem> _codeSystemsByUrl;
    private Dictionary<string, FhirValueSetCollection> _valueSetsByUrl;
    private Dictionary<string, FhirNodeInfo> _nodeInfoByPath;

    /// <summary>
    /// Initializes static members of the <see cref="FhirVersionInfo"/> class.
    /// </summary>
    static FhirVersionInfo()
    {
        _fhirReleasesByVersion = new ();

        foreach (PublishedReleaseInformation release in _fhirReleases)
        {
            _fhirReleasesByVersion.Add(release.Version, release);
        }

        _latestVersionByRelease = new ();

        foreach (PublishedReleaseInformation release in _fhirReleases)
        {
            if (!_latestVersionByRelease.ContainsKey(release.Major))
            {
                _latestVersionByRelease.Add(release.Major, release.Version);
                continue;
            }

            if (release.PublicationDate.CompareTo(
                    _fhirReleasesByVersion[_latestVersionByRelease[release.Major]].PublicationDate) > 0)
            {
                _latestVersionByRelease[release.Major] = release.Version;
                continue;
            }
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FhirVersionInfo"/> class.
    /// </summary>
    public FhirVersionInfo()
    {
        // create our info dictionaries
        _primitiveTypesByName = new Dictionary<string, FhirPrimitive>();
        _complexTypesByName = new Dictionary<string, FhirComplex>();
        _resourcesByName = new Dictionary<string, FhirComplex>();
        _extensionsByUrl = new Dictionary<string, FhirComplex>();
        _extensionsByPath = new Dictionary<string, Dictionary<string, FhirComplex>>();
        _profilesById = new Dictionary<string, FhirComplex>();
        _profilesByBaseType = new Dictionary<string, Dictionary<string, FhirComplex>>();
        _systemOperations = new Dictionary<string, FhirOperation>();
        _globalSearchParameters = new Dictionary<string, FhirSearchParam>();
        _searchResultParameters = new Dictionary<string, FhirSearchParam>();
        _allInteractionParameters = new Dictionary<string, FhirSearchParam>();
        _codeSystemsByUrl = new Dictionary<string, FhirCodeSystem>();
        _valueSetsByUrl = new Dictionary<string, FhirValueSetCollection>();
        _nodeInfoByPath = new Dictionary<string, FhirNodeInfo>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FhirVersionInfo"/> class. Require major version
    /// (release #) to validate it is supported.
    /// </summary>
    /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
    /// <param name="version">The version string.</param>
    public FhirVersionInfo(string version)
        : this()
    {
        if (!_fhirReleasesByVersion.ContainsKey(version))
        {
            throw new Exception($"Invalid FHIR version: {version}!");
        }

        PublishedReleaseInformation release = _fhirReleasesByVersion[version];

        MajorVersionEnum = release.Major;
#pragma warning disable CS0618 // Type or member is obsolete
        MajorVersion = MajorIntForVersion(release.Major);
#pragma warning restore CS0618 // Type or member is obsolete
        ReleaseName = release.Major.ToString();
        BallotPrefix = release.BallotPrefix;
        PackageName = $"hl7.fhir.{ReleaseName.ToLowerInvariant()}.core";
        ExamplesPackageName = $"hl7.fhir.{ReleaseName.ToLowerInvariant()}.examples";
        ExpansionsPackageName = $"hl7.fhir.{ReleaseName.ToLowerInvariant()}.expansions";
        VersionString = release.Version;
        IsDevBuild = false;
        IsLocalBuild = false;
        IsOnDisk = false;
        BuildId = string.Empty;

        _fhirConverter = ConverterHelper.ConverterForVersion(release.Major);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FhirVersionInfo"/> class. Require major version
    /// (release #) to validate it is supported.
    /// </summary>
    /// <param name="version">        The version string.</param>
    /// <param name="fhirCoreVersion">The FHIR core version.</param>
    /// <param name="ciBranchName">   Name of the ci branch.</param>
    /// <param name="buildId">        Identifier for the build.</param>
    public FhirVersionInfo(
        string version,
        FhirCoreVersion fhirCoreVersion,
        string ciBranchName,
        string buildId)
        : this()
    {
        MajorVersionEnum = fhirCoreVersion;
#pragma warning disable CS0618 // Type or member is obsolete
        MajorVersion = MajorIntForVersion(fhirCoreVersion);
#pragma warning restore CS0618 // Type or member is obsolete
        ReleaseName = fhirCoreVersion.ToString();
        BallotPrefix = string.Empty;
        PackageName = $"hl7.fhir.{ReleaseName.ToLowerInvariant()}.core";
        ExamplesPackageName = string.Empty;                 // ci builds do not have examples
        ExpansionsPackageName = $"hl7.fhir.{ReleaseName.ToLowerInvariant()}.expansions";
        VersionString = version;
        IsDevBuild = true;
        IsLocalBuild = false;
        IsOnDisk = false;
        DevBranch = ciBranchName;
        BuildId = buildId;

        _fhirConverter = ConverterHelper.ConverterForVersion(fhirCoreVersion);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FhirVersionInfo"/> class. Require major version
    /// (release #) to validate it is supported.
    /// </summary>
    /// <param name="source"> Source for the.</param>
    /// <param name="options">Options for controlling the operation.</param>
    public FhirVersionInfo(FhirVersionInfo source, PackageCopyOptions options)
        : base(source, options)
    {
        _fhirConverter = ConverterHelper.ConverterForVersion(source.MajorVersionEnum);
    }

    /// <summary>Determine if we should process resource.</summary>
    /// <param name="resourceName">Name of the resource.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public override bool ShouldProcessResource(string resourceName)
    {
        if (_versionResourcesToProcess[MajorVersionEnum].Contains(resourceName))
        {
            return true;
        }

        return false;
    }

    /// <summary>Determine if we should ignore resource.</summary>
    /// <param name="resourceName">Name of the resource.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public override bool ShouldIgnoreResource(string resourceName)
    {
        if (_versionResourcesToIgnore[MajorVersionEnum].Contains(resourceName))
        {
            return true;
        }

        return false;
    }

    /// <summary>Determine if we should skip file.</summary>
    /// <param name="filename">Filename of the file.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public override bool ShouldSkipFile(string filename)
    {
        if (_npmFilesToIgnore.Contains(filename))
        {
            return true;
        }

        if (_versionFilesToIgnore[MajorVersionEnum].Contains(filename))
        {
            return true;
        }

        return false;
    }

    /// <summary>Parses resource an object from the given string.</summary>
    /// <param name="json">The JSON.</param>
    /// <returns>A typed Resource object.</returns>
    public override object ParseResource(string json)
    {
        return _fhirConverter.ParseResource(json);
    }

    /// <summary>Attempts to process resource.</summary>
    /// <param name="resource">[out] The resource object.</param>
    public override void ProcessResource(object resource)
    {
        // process this per the correct FHIR version
        _fhirConverter.ProcessResource(resource, this);
    }

    /// <summary>Determines if we can converter has issues.</summary>
    /// <param name="errorCount">  [out] Number of errors.</param>
    /// <param name="warningCount">[out] Number of warnings.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public override bool ConverterHasIssues(out int errorCount, out int warningCount)
    {
        return _fhirConverter.HasIssues(out errorCount, out warningCount);
    }

    /// <summary>Displays the converter issues.</summary>
    public override void DisplayConverterIssues()
    {
        _fhirConverter.DisplayIssues();
    }

    /// <summary>Latest version for release.</summary>
    /// <param name="major">The major.</param>
    /// <returns>A string.</returns>
    internal static string LatestVersionForRelease(FhirCoreVersion major)
    {
        return _latestVersionByRelease[major];
    }

    /// <summary>Query if 'version' is known version.</summary>
    /// <param name="version">The version string.</param>
    /// <returns>True if known version, false if not.</returns>
    internal static bool IsKnownVersion(string version)
    {
        if (_fhirReleasesByVersion.ContainsKey(version))
        {
            return true;
        }

        return false;
    }

    /// <summary>Major for release.</summary>
    /// <param name="release">The release.</param>
    /// <returns>An int.</returns>
    internal static int MajorIntForVersion(FhirCoreVersion release)
    {
        switch (release)
        {
            case FhirCoreVersion.DSTU2:
                return 2;

            case FhirCoreVersion.STU3:
                return 3;

            case FhirCoreVersion.R4:
            case FhirCoreVersion.R4B:
                return 4;

            case FhirCoreVersion.R5:
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
    /// <returns>A FhirMajorRelease.</returns>
    internal static FhirCoreVersion MajorReleaseForVersion(string version)
    {
        if (string.IsNullOrEmpty(version))
        {
            throw new ArgumentNullException(nameof(version));
        }

        if (_fhirReleasesByVersion.ContainsKey(version))
        {
            return _fhirReleasesByVersion[version].Major;
        }

        string vShort = version.Length > 2
            ? version.Substring(0, 3)
            : version.Substring(0, 1);

        // fallback to guessing
        switch (vShort)
        {
            case "1.0":
            case "1":
            case "2.0":
            case "2":
                return FhirCoreVersion.DSTU2;

            case "3.0":
            case "3":
                return FhirCoreVersion.STU3;

            case "4":
            case "4.0":
                return FhirCoreVersion.R4;

            case "4.1":
            case "4.3":
                return FhirCoreVersion.R4B;

            case "4.2":
            case "4.4":
            case "4.5":
            case "4.6":
            case "5.0":
            case "5":
                return FhirCoreVersion.R5;
        }

        throw new ArgumentOutOfRangeException($"Unknown FHIR version: {version}");
    }
}
