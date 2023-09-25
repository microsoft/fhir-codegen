// <copyright file="FhirCache.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using IniParser.Configuration;
using IniParser;
using System.Formats.Tar;
using System.IO.Compression;
using System.Net;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.IO;
using System.Xml.Linq;
using System;
using System.Text.Json;

namespace Microsoft.Health.Fhir.PackageManager;

public partial class FhirCache : IDisposable
{
    /// <summary>Values that represent package load states.</summary>
    public enum PackageLoadStateCodes
    {
        /// <summary>The package is in an unknown state.</summary>
        Unknown,

        /// <summary>The package has not been loaded.</summary>
        NotLoaded,

        /// <summary>The package is queued for loading.</summary>
        Queued,

        /// <summary>The package is currently being loaded.</summary>
        InProgress,

        /// <summary>The package is currently loaded into memory.</summary>
        Loaded,

        /// <summary>The package has failed to load and cannot be used.</summary>
        Failed,

        /// <summary>The package has been parsed but not loaded into memory.</summary>
        Parsed,
    }

    /// <summary>Values that represent FHIR major releases.</summary>
    public enum FhirSequenceCodes : int
    {
        /// <summary>Unknown FHIR Version.</summary>
        Unknown = 0,

        /// <summary>FHIR DSTU2.</summary>
        DSTU2 = 2,

        /// <summary>FHIR STU3.</summary>
        STU3 = 3,

        /// <summary>FHIR R4.</summary>
        R4 = 4,

        /// <summary>FHIR R4B.</summary>
        R4B = -1,

        /// <summary>FHIR R5.</summary>
        R5 = 5,

        /// <summary>FHIR R5.</summary>
        R6 = 6,
    }

    /// <summary>Convert a FHIR sequence code to the literal for that version.</summary>
    /// <param name="sequence">The sequence.</param>
    /// <returns>A string.</returns>
    public static string ToLiteral(FhirSequenceCodes sequence) => sequence switch
    {
        FhirSequenceCodes.DSTU2 => "DSTU2",
        FhirSequenceCodes.STU3 => "STU3",
        FhirSequenceCodes.R4 => "R4",
        FhirSequenceCodes.R4B => "R4B",
        FhirSequenceCodes.R5 => "R5",
        FhirSequenceCodes.R6 => "R6",
        _ => "Unknown"
    };

    /// <summary>Converts a sequence to a short version.</summary>
    /// <param name="sequence">The sequence.</param>
    /// <returns>Sequence as a string.</returns>
    public static string ToShortVersion(FhirSequenceCodes sequence) => sequence switch
    {
        FhirSequenceCodes.DSTU2 => "1.0",
        FhirSequenceCodes.STU3 => "3.0",
        FhirSequenceCodes.R4 => "4.0",
        FhirSequenceCodes.R4B => "4.3",
        FhirSequenceCodes.R5 => "5.0",
        FhirSequenceCodes.R6 => "6.0",
        _ => "Unknown"
    };

    /// <summary>Converts a sequence to a long version.</summary>
    /// <param name="sequence">The sequence.</param>
    /// <returns>Sequence as a string.</returns>
    public static string ToLongVersion(FhirSequenceCodes sequence) => sequence switch
    {
        FhirSequenceCodes.DSTU2 => "1.0.2",
        FhirSequenceCodes.STU3 => "3.0.2",
        FhirSequenceCodes.R4 => "4.0.1",
        FhirSequenceCodes.R4B => "4.3.0",
        FhirSequenceCodes.R5 => "5.0.0",
        FhirSequenceCodes.R6 => "6.0.0",
        _ => "Unknown"
    };

    /// <summary>Converts a FHIR sequence literal or package number to a sequence code.</summary>
    /// <param name="literal">The literal.</param>
    /// <returns>Literal as the FhirSequenceCodes.</returns>
    public static FhirSequenceCodes ToSequence(string literal) => literal.ToUpperInvariant() switch
    {
        "R2" => FhirSequenceCodes.DSTU2,
        "DSTU2" => FhirSequenceCodes.DSTU2,
        "0.4.0" => FhirSequenceCodes.DSTU2,
        "0.4" => FhirSequenceCodes.DSTU2,
        "0.5.0" => FhirSequenceCodes.DSTU2,
        "0.5" => FhirSequenceCodes.DSTU2,
        "1.0.0" => FhirSequenceCodes.DSTU2,
        "1.0.1" => FhirSequenceCodes.DSTU2,
        "1.0.2" => FhirSequenceCodes.DSTU2,
        "1.0" => FhirSequenceCodes.DSTU2,
        "R3" => FhirSequenceCodes.STU3,
        "STU3" => FhirSequenceCodes.STU3,
        "1.1.0" => FhirSequenceCodes.STU3,
        "1.1" => FhirSequenceCodes.STU3,
        "1.2.0" => FhirSequenceCodes.STU3,
        "1.2" => FhirSequenceCodes.STU3,
        "1.4.0" => FhirSequenceCodes.STU3,
        "1.4" => FhirSequenceCodes.STU3,
        "1.6.0" => FhirSequenceCodes.STU3,
        "1.6" => FhirSequenceCodes.STU3,
        "1.8.0" => FhirSequenceCodes.STU3,
        "1.8" => FhirSequenceCodes.STU3,
        "3.0.0" => FhirSequenceCodes.STU3,
        "3.0.1" => FhirSequenceCodes.STU3,
        "3.0.2" => FhirSequenceCodes.STU3,
        "3.0" => FhirSequenceCodes.STU3,
        "R4" => FhirSequenceCodes.R4,
        "3.2.0" => FhirSequenceCodes.R4,
        "3.2" => FhirSequenceCodes.R4,
        "3.3.0" => FhirSequenceCodes.R4,
        "3.3" => FhirSequenceCodes.R4,
        "3.5.0" => FhirSequenceCodes.R4,
        "3.5" => FhirSequenceCodes.R4,
        "3.5a.0" => FhirSequenceCodes.R4,
        "4.0.0" => FhirSequenceCodes.R4,
        "4.0.1" => FhirSequenceCodes.R4,
        "4.0" => FhirSequenceCodes.R4,
        "R4B" => FhirSequenceCodes.R4B,
        "4.1.0" => FhirSequenceCodes.R4B,
        "4.1" => FhirSequenceCodes.R4B,
        "4.3.0-snapshot1" => FhirSequenceCodes.R4B,
        "4.3.0" => FhirSequenceCodes.R4B,
        "4.3" => FhirSequenceCodes.R4B,
        "R5" => FhirSequenceCodes.R5,
        "4.2.0" => FhirSequenceCodes.R5,
        "4.2" => FhirSequenceCodes.R5,
        "4.4.0" => FhirSequenceCodes.R5,
        "4.4" => FhirSequenceCodes.R5,
        "4.5.0" => FhirSequenceCodes.R5,
        "4.5" => FhirSequenceCodes.R5,
        "4.6.0" => FhirSequenceCodes.R5,
        "4.6" => FhirSequenceCodes.R5,
        "5.0.0-snapshot1" => FhirSequenceCodes.R5,
        "5.0.0-ballot" => FhirSequenceCodes.R5,
        "5.0.0-snapshot3" => FhirSequenceCodes.R5,
        "5.0.0-draft-final" => FhirSequenceCodes.R5,
        "5.0.0" => FhirSequenceCodes.R5,
        "5.0" => FhirSequenceCodes.R5,
        "5" => FhirSequenceCodes.R5,
        "R6" => FhirSequenceCodes.R6,
        "6.0.0" => FhirSequenceCodes.R6,
        "6.0" => FhirSequenceCodes.R6,
        "6" => FhirSequenceCodes.R6,
        _ => literal?.Length > 3 ? ToSequence(literal.Substring(0, 3)) : FhirSequenceCodes.Unknown,
    };

    /// <summary>Information about a package in the cache.</summary>
    internal readonly record struct PackageCacheRecord(
        string CacheDirective,
        PackageLoadStateCodes PackageState,
        string PackageName,
        string Version,
        FhirSequenceCodes FhirVersion,
        string DownloadDateTime,
        long PackageSize,
        FhirNpmPackageDetails Details);

    /// <summary>(Immutable) The package registry uris.</summary>
    private static readonly Uri[] _defaultRegistryUris =
    {
        new("http://packages.fhir.org/"),
        new("http://packages2.fhir.org/packages/"),
    };

    /// <summary>(Immutable) URI of the FHIR published server.</summary>
    private static readonly Uri _publicationUri = new("http://hl7.org/fhir/");

    /// <summary>(Immutable) URI of the FHIR CI server.</summary>
    private static readonly Uri _ciUri = new("http://build.fhir.org/");

    /// <summary>The logger.</summary>
    private ILogger _logger;

    /// <summary>Pathname of the cache directory.</summary>
    private string _cacheDirectory = string.Empty;

    /// <summary>Pathname of the cache package directory.</summary>
    private string _cachePackageDirectory = string.Empty;

    /// <summary>Full pathname of the initialize file.</summary>
    private string _iniFilePath = string.Empty;

    /// <summary>The HTTP client.</summary>
    private HttpClient _httpClient = new();

    /// <summary>True to disposed value.</summary>
    private bool _disposedValue = false;

    /// <summary>The package records, by directive.</summary>
    private Dictionary<string, PackageCacheRecord> _packagesByDirective = new();

    /// <summary>Package versions, by package name.</summary>
    private Dictionary<string, List<string>> _versionsByName = new();

    /// <summary>Occurs when On Changed.</summary>
    public event EventHandler<EventArgs>? OnChanged = null;

    /// <summary>Test if a name matches known core packages.</summary>
    /// <returns>A RegEx.</returns>
    [GeneratedRegex("^hl7.fhir.r\\d+[a-z]?.(core|expansions|examples|search|elements|corexml)$")]
    private static partial Regex MatchCorePackageNames();

    /// <summary>Test if a name matches known core packages.</summary>
    private static Regex _matchCorePackageNames = MatchCorePackageNames();

    /// <summary>Initializes a new instance of the <see cref="FhirCache"/> class.</summary>
    /// <param name="fhirCachePath">Pathname of the FHIR cache directory.</param>
    /// <param name="logger">        The logger.</param>
    public FhirCache(
        string fhirCachePath,
        ILogger<FhirCache>? logger)
    {
        if (string.IsNullOrEmpty(fhirCachePath))
        {
            throw new ArgumentNullException(nameof(fhirCachePath));
        }

        if (!Directory.Exists(fhirCachePath))
        {
            // if the directory does not exist, we will try to create it
            try
            {
                Directory.CreateDirectory(fhirCachePath);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Unable to create FHIR cache directory: {fhirCachePath}, {ex.Message}");
            }
        }

        _logger = logger ?? NullLoggerFactory.Instance.CreateLogger<FhirCache>();
        _logger.LogInformation($"Initializing a FhirCache at: {_cacheDirectory}");

        _cacheDirectory = fhirCachePath;
        _cachePackageDirectory = Path.Combine(_cacheDirectory, "packages");
        _iniFilePath = Path.Combine(_cachePackageDirectory, "packages.ini");

        if (!Directory.Exists(_cachePackageDirectory))
        {
            Directory.CreateDirectory(_cachePackageDirectory);
        }

        if (!File.Exists(_iniFilePath))
        {
            CreateEmptyCacheIni();
        }

        SynchronizeCache();
    }

    /// <summary>Gets the packages by directive.</summary>
    private Dictionary<string, PackageCacheRecord> PackagesByDirective => _packagesByDirective;

    /// <summary>The completed requests.</summary>
    private HashSet<string> _processed = new();

    /// <summary>A package cache entry.</summary>
    /// <param name="fhirVersion">        The FHIR version.</param>
    /// <param name="directory">          Pathname of the directory.</param>
    /// <param name="resolvedDirective">  The resolved directive.</param>
    /// <param name="resolvedName">       Name of the resolved.</param>
    /// <param name="resolvedVersion">    The resolved version.</param>
    /// <param name="umbrellaPackageName">Name of the umbrella package.</param>
    public record struct PackageCacheEntry(
        FhirSequenceCodes fhirVersion,
        string directory,
        string resolvedDirective,
        string name,
        string version,
        string umbrellaPackageName);

    /// <summary>Attempts to find locally or download a given package.</summary>
    /// <param name="directive">     The directive: [package-name]#[version].</param>
    /// <param name="branchName">    Name of the branch.</param>
    /// <param name="packages">      [out] Path to the directory with the extracted package.</param>
    /// <param name="forFhirVersion">(Optional) FHIR version to restrict downloads to.</param>
    /// <param name="offlineMode">   (Optional) True to enable offline mode, false to disable it.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public bool FindOrDownload(
        string directive,
        string branchName,
        out IEnumerable<PackageCacheEntry> packages,
        FhirSequenceCodes forFhirVersion = FhirSequenceCodes.Unknown,
        bool offlineMode = false)
    {
        if (string.IsNullOrEmpty(branchName))
        {
            _logger.LogInformation($"Attempting to load: {directive}");
        }
        else
        {
            _logger.LogInformation($"Attempting to load branch: {branchName}");
        }

        string key = $"{directive}|{branchName}";
        if (_processed.Contains(key))
        {
            // if we have already processed this once, force into offline for performance
            offlineMode = true;
        }

        _processed.Add(key);

        string name;
        string version;
        bool isLocal = false;
        List<PackageCacheEntry> packageList = new();

        string directory = string.Empty;
        FhirSequenceCodes fhirVersion;
        string resolvedDirective;

        if (directive.Contains('#'))
        {
            string[] components = directive.Split('#', StringSplitOptions.TrimEntries);
            name = components[0];
            version = components[1];
        }
        else
        {
            name = directive;
            version = string.Empty;
        }

        if (!string.IsNullOrEmpty(branchName))
        {
            branchName = GetIgBranchFromInput(branchName);
            if (string.IsNullOrEmpty(version))
            {
                version = "dev";
            }
        }

        string directiveVersion = version;

        name = GetPackageNameFromInput(name);

        if (version.Equals("dev", StringComparison.OrdinalIgnoreCase))
        {
            bool success = ProcessCiRequest(
                directive,
                name,
                version,
                branchName,
                packageList,
                out directory,
                out fhirVersion,
                out resolvedDirective);

            packages = success ? packageList : Enumerable.Empty<PackageCacheEntry>();
            return success;
        }

        if (version.Equals("current", StringComparison.OrdinalIgnoreCase))
        {
            if (PackageIsFhirCore(name))
            {
                if (TryDownloadCoreViaCI(name, branchName, out directory, out fhirVersion, out resolvedDirective))
                {
                    packageList.Add(new()
                    {
                        fhirVersion = fhirVersion,
                        directory = directory,
                        resolvedDirective = resolvedDirective,
                        name = name,
                        version = resolvedDirective.Contains('#') ? resolvedDirective.Split('#')[1] : version,
                        umbrellaPackageName = name,
                    });

                    packages = packageList;
                    return true;
                }
            }
            else
            {
                if (TryDownloadGuideViaCI(branchName, out name, out directory, out fhirVersion, out resolvedDirective))
                {
                    packageList.Add(new()
                    {
                        fhirVersion = fhirVersion,
                        directory = directory,
                        resolvedDirective = resolvedDirective,
                        name = name,
                        version = resolvedDirective.Contains('#') ? resolvedDirective.Split('#')[1] : version,
                        umbrellaPackageName = name,
                    });

                    packages = packageList;
                    return true;
                }
            }

            if (string.IsNullOrEmpty(branchName))
            {
                _logger.LogInformation($"FindOrDownload <<< package: {directive}, branch: {branchName} not accessible!");
            }
            else
            {
                _logger.LogInformation($"FindOrDownload <<< package: {directive} not accessible!");
            }

            packages = Enumerable.Empty<PackageCacheEntry>();
            return false;
        }

        // check to see if this package already has a version trailer
        string lastComponent = name.Split('.').Last();

        Dictionary<FhirSequenceCodes, string> downloadPackages = new();

        FhirSequenceCodes seq = ToSequence(lastComponent);
        if (seq != FhirSequenceCodes.Unknown)
        {
            downloadPackages.Add(seq, string.Empty);
        }
        else
        {
            // query the package registry to see what packages matching this root are available
            string contents = _httpClient.GetStringAsync($"").Result;

            downloadPackages = Enum.GetValues(typeof(FhirSequenceCodes))
                .Cast<FhirSequenceCodes>()
                .ToDictionary(x => x, x => ToLiteral(x));
        }

        bool foundLocally = false;

        // want to check for fhir-version named packages
        foreach ((FhirSequenceCodes sequence, string trailer) in downloadPackages)
        {
            // there are no versioned packages for DSTU2 or STU3
            if ((sequence == FhirSequenceCodes.DSTU2) ||
                (sequence == FhirSequenceCodes.STU3))
            {
                continue;
            }

            version = directiveVersion;
            isLocal = false;
            directory = string.Empty;

            string sequencedName = string.IsNullOrEmpty(trailer) ? name : $"{name}.{trailer}";

            if (string.IsNullOrEmpty(version) ||
                version.Equals("latest", StringComparison.OrdinalIgnoreCase))
            {
                TryGetHighestVersion(sequencedName, offlineMode, out version, out isLocal, out directory);
            }

            if ((isLocal && !string.IsNullOrEmpty(directory)) ||
                HasCachedVersion(sequencedName, version, out directory))
            {
                packageList.Add(new()
                {
                    fhirVersion = _packagesByDirective[$"{sequencedName}#{version}"].FhirVersion,
                    directory = directory,
                    resolvedDirective = $"{sequencedName}#{version}",
                    name = sequencedName,
                    version = version,
                    umbrellaPackageName = name,
                });

                foundLocally = true;
                continue;
            }

            // do not check online if we already have the package locally
            // note this can have an issue if we have a 'root' package and not a version-specific package
            // but that is a rare case and can be solved by cleaning the cache.
            if ((!isLocal && offlineMode) ||
                foundLocally)
            {
                continue;
            }

            if (TryDownloadViaRegistry(sequencedName, version, out directory, out fhirVersion, out resolvedDirective))
            {
                packageList.Add(new()
                {
                    fhirVersion = fhirVersion,
                    directory = directory,
                    resolvedDirective = resolvedDirective,
                    name = sequencedName,
                    version = version,
                    umbrellaPackageName = name,
                });

                continue;
            }

            if (TryDownloadCoreViaPublication(sequencedName, version, out directory, out fhirVersion, out resolvedDirective))
            {
                packageList.Add(new()
                {
                    fhirVersion = fhirVersion,
                    directory = directory,
                    resolvedDirective = resolvedDirective,
                    name = sequencedName,
                    version = version,
                    umbrellaPackageName = name,
                });

                continue;
            }
        }

        if (packageList.Any())
        {
            packages = packageList;
            return true;
        }

        _logger.LogInformation($"FindOrDownload <<< unable to resolve directive: {directive}");
        packages = Enumerable.Empty<PackageCacheEntry>();
        return false;
    }

    private void FindPackagesOnRegistries(
        string directive,
        string name,
        string version,
        List<PackageCacheEntry> packageList,
        FhirSequenceCodes forFhirVersion)
    {


        foreach (Uri registryUri in _defaultRegistryUris)
        {
            Uri uri = new Uri(registryUri, $"catalog?op=find&name={name}");

            try
            {
                string contents = _httpClient.GetStringAsync(uri).Result;

                List<FhirPackageVersionInfo>? versions = JsonSerializer.Deserialize<List<FhirPackageVersionInfo>>(contents);
            }
            catch (Exception ex)
            {
                _logger.LogDebug($"FindPackagesOnRegistries <<< Registry query: {uri} failed: {ex.Message}");
                if (ex.InnerException != null)
                {
                    _logger.LogDebug($" <<< inner: {ex.InnerException}");
                }
            }
        }
    }


    /// <summary>Process a CI download request.</summary>
    /// <param name="directive">        The directive: [package-name]#[version].</param>
    /// <param name="name">             The name.</param>
    /// <param name="version">          [out] The version string (e.g., 4.0.1).</param>
    /// <param name="branchName">       Name of the branch.</param>
    /// <param name="packageList">      List of packages.</param>
    /// <param name="directory">        [out] Pathname of the directory.</param>
    /// <param name="fhirVersion">      [out] The FHIR version.</param>
    /// <param name="resolvedDirective">[out] The resolved directive.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    private bool ProcessCiRequest(
        string directive,
        string name,
        string version,
        string branchName,
        List<PackageCacheEntry> packageList,
        out string directory,
        out FhirSequenceCodes fhirVersion,
        out string resolvedDirective)
    {
        if (PackageIsFhirCore(name))
        {
            if (TryDownloadCoreViaCI(name, branchName, out directory, out fhirVersion, out resolvedDirective))
            {
                packageList.Add(new()
                {
                    fhirVersion = fhirVersion,
                    directory = directory,
                    resolvedDirective = resolvedDirective,
                    name = name,
                    version = resolvedDirective.Contains('#') ? resolvedDirective.Split('#')[1] : version,
                    umbrellaPackageName = name,
                });

                return true;
            }
        }
        else
        {
            if (TryDownloadGuideViaCI(branchName, out name, out directory, out fhirVersion, out resolvedDirective))
            {
                packageList.Add(new()
                {
                    fhirVersion = fhirVersion,
                    directory = directory,
                    resolvedDirective = resolvedDirective,
                    name = name,
                    version = resolvedDirective.Contains('#') ? resolvedDirective.Split('#')[1] : version,
                    umbrellaPackageName = name,
                });

                return true;
            }
        }

        // resolve dev (local only) version or fail
        if (string.IsNullOrEmpty(name) ||
            (!HasCachedVersion(name, version, out directory)))
        {
            if (string.IsNullOrEmpty(branchName))
            {
                _logger.LogInformation($"ProcessCiRequest <<< Package: {directive}, branch: {branchName} is not accessible!");
            }
            else
            {
                _logger.LogInformation($"ProcessCiRequest <<< package: {directive} not accessible!");
            }

            return false;
        }

        packageList.Add(new()
        {
            fhirVersion = ToSequence(_packagesByDirective[directive].Details.FhirVersion),
            directory = directory,
            resolvedDirective = directive,
            name = name,
            version = directive.Contains('#') ? directive.Split('#')[1] : version,
            umbrellaPackageName = name,
        });

        return true;
    }

    /// <summary>Attempts to download via registry a string from the given string.</summary>
    /// <param name="name">       The name.</param>
    /// <param name="version">    [out] The version string (e.g., 4.0.1).</param>
    /// <param name="directory">  [out] Pathname of the directory.</param>
    /// <param name="fhirVersion">[out] The FHIR version.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    private bool TryDownloadViaRegistry(
        string name,
        string version,
        out string directory,
        out FhirSequenceCodes fhirVersion,
        out string resolvedDirective)
    {
        foreach (Uri registryUri in _defaultRegistryUris)
        {
            Uri uri = new Uri(registryUri, $"{name}/{version}");
            directory = Path.Combine(_cachePackageDirectory, $"{name}#{version}");

            string directive = name + "#" + version;

            if (TryDownloadAndExtract(uri, directory, directive, out fhirVersion, out resolvedDirective))
            {
                UpdatePackageCacheIndex(directive, directory);

                return true;
            }
        }

        directory = string.Empty;
        fhirVersion = FhirSequenceCodes.Unknown;
        resolvedDirective = string.Empty;
        return false;
    }

    /// <summary>Gets directory size.</summary>
    /// <param name="directory">[out] Pathname of the directory.</param>
    /// <returns>The directory size.</returns>
    private static long GetDirectorySize(string directory)
    {
        long size = 0;

        DirectoryInfo dirInfo = new(directory);
        var fileInfos = dirInfo.EnumerateFiles("*.*", SearchOption.AllDirectories);

        size = 0;

        foreach (FileInfo fileInfo in fileInfos)
        {
            size = size + fileInfo.Length;
        }

        return size;
    }

    /// <summary>Updates the cache package initialize.</summary>
    /// <param name="directive">The directive.</param>
    /// <param name="directory">[out] Pathname of the directory.</param>
    /// <param name="iniData">  (Optional) Information describing the initialize.</param>
    private void UpdatePackageCacheIndex(
        string directive,
        string directory,
        IniData? iniData = null)
    {
        string[] components = directive.Split('#');
        string name = components[0];
        string directiveVersion = components.Length > 1 ? components[1] : "current";

        if (!Directory.Exists(directory))
        {
            if (iniData == null)
            {
                IniDataParser parser = new();

                IniData data = parser.Parse(File.ReadAllText(_iniFilePath));

                if (data["packages"].Contains(directive))
                {
                    data["packages"].Remove(directive);
                }

                if (data["package-sizes"].Contains(directive))
                {
                    data["package-sizes"].Remove(directive);
                }

                SaveIniData(_iniFilePath, data);
            }
            else
            {
                if (iniData["packages"].Contains(directive))
                {
                    iniData["packages"].Remove(directive);
                }

                if (iniData["package-sizes"].Contains(directive))
                {
                    iniData["package-sizes"].Remove(directive);
                }
            }

            if (_packagesByDirective.ContainsKey(directive))
            {
                _packagesByDirective.Remove(directive);
            }

            if (_versionsByName.ContainsKey(name))
            {
                _versionsByName[name] = _versionsByName[name].Where((v) => !v.Equals(directiveVersion)).ToList();
            }

            return;
        }

        long size = GetDirectorySize(directory);
        string packageDate = DateTime.Now.ToString("yyyyMMddHHmmss");

        string npmJson = Path.Combine(directory, "package", "package.json");

        if (File.Exists(npmJson))
        {
            FhirNpmPackageDetails npmDetails = FhirNpmPackageDetails.Load(npmJson);
            if (!string.IsNullOrEmpty(npmDetails.BuildDate))
            {
                packageDate = npmDetails.BuildDate;
            }

            PackageCacheRecord record = new(
                directive,
                PackageLoadStateCodes.NotLoaded,
                name,
                npmDetails.Version,
                ToSequence(npmDetails.FhirVersion),
                packageDate,
                size,
                npmDetails);

            _packagesByDirective[directive] = record;

            if (!_versionsByName.ContainsKey(name))
            {
                _versionsByName.Add(name, new());
            }

            if (!_versionsByName[name].Contains(npmDetails.Version))
            {
                _versionsByName[name].Add(npmDetails.Version);
            }
        }

        if (iniData == null)
        {
            IniDataParser parser = new();

            IniData data = parser.Parse(File.ReadAllText(_iniFilePath));

            if (data["packages"].Contains(directive))
            {
                data["packages"].Remove(directive);
            }

            data["packages"].Add(directive, packageDate);

            if (data["package-sizes"].Contains(directive))
            {
                data["package-sizes"].Remove(directive);
            }

            data["package-sizes"].Add(directive, size.ToString());

            SaveIniData(_iniFilePath, data);
        }
        else
        {
            if (iniData["packages"].Contains(directive))
            {
                iniData["packages"].Remove(directive);
            }

            iniData["packages"].Add(directive, packageDate);

            if (iniData["package-sizes"].Contains(directive))
            {
                iniData["package-sizes"].Remove(directive);
            }

            iniData["package-sizes"].Add(directive, size.ToString());
        }
    }

    /// <summary>Deletes the package described by packageDirective.</summary>
    /// <param name="packageDirective">The package directive.</param>
    public void DeletePackage(string packageDirective)
    {
        string directory = Path.Combine(_cachePackageDirectory, packageDirective);

        if (!Directory.Exists(directory))
        {
            return;
        }

        try
        {
            Directory.Delete(directory, true);

            UpdatePackageCacheIndex(packageDirective, directory);
        }
        catch (Exception ex)
        {
            _logger.LogInformation($"DeletePackage <<< caught exception: {ex.Message}");
            if (ex.InnerException != null)
            {
                _logger.LogInformation($" <<< {ex.InnerException.Message}");
            }
        }
    }

    /// <summary>Attempts to download and extract a string from the given URI.</summary>
    /// <param name="uri">              URI of the resource.</param>
    /// <param name="directory">        Pathname of the directory.</param>
    /// <param name="packageDirective"> The package directive.</param>
    /// <param name="fhirVersion">      [out] The FHIR version.</param>
    /// <param name="resolvedDirective">[out] The resolved directive.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    private bool TryDownloadAndExtract(
        Uri uri,
        string directory,
        string packageDirective,
        out FhirSequenceCodes fhirVersion,
        out string resolvedDirective)
    {
        try
        {
            using (Stream rawStream = _httpClient.GetStreamAsync(uri).Result)
            using (Stream gzipStream = new GZipStream(rawStream, CompressionMode.Decompress))
            {
                // make sure our destination directory exists
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                TarFile.ExtractToDirectory(gzipStream, directory, true);
            }

            UpdatePackageCacheIndex(packageDirective, directory);

            fhirVersion = _packagesByDirective[packageDirective].FhirVersion;
            resolvedDirective = packageDirective;
            return true;
        }
        catch (HttpRequestException hex)
        {
            // we have a lot of not found because of package nesting, this is reported elsewhere
            if (hex.StatusCode == HttpStatusCode.NotFound)
            {
                fhirVersion = FhirSequenceCodes.Unknown;
                resolvedDirective = string.Empty;
                return false;
            }

            _logger.LogInformation($"TryDownloadAndExtract <<< exception downloading {uri}: {hex.Message}");
            if (hex.InnerException != null)
            {
                _logger.LogInformation($" <<< inner: {hex.InnerException.Message}");
            }
        }
        catch (Exception ex)
        {
            if ((ex.InnerException != null) &&
                (ex.InnerException is HttpRequestException hex))
            {
                // we have a lot of not found because of package nesting, this is reported elsewhere
                if (hex.StatusCode == HttpStatusCode.NotFound)
                {
                    fhirVersion = FhirSequenceCodes.Unknown;
                    resolvedDirective = string.Empty;
                    return false;
                }
            }

            _logger.LogInformation($"TryDownloadAndExtract <<< exception downloading: {uri}: {ex.Message}");
            if (ex.InnerException != null)
            {
                _logger.LogInformation($" <<< inner: {ex.InnerException.Message}");
            }
        }

        fhirVersion = FhirSequenceCodes.Unknown;
        resolvedDirective = string.Empty;
        return false;
    }

    /// <summary>Package is FHIR core.</summary>
    /// <param name="packageName">Name of the package.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    internal static bool PackageIsFhirCore(string packageName)
    {
        string name = packageName.Contains('#')
            ? packageName.Substring(0, packageName.IndexOf('#'))
            : packageName;

        return _matchCorePackageNames.IsMatch(name);
    }

    /// <summary>Attempts to download core via ci a string from the given string.</summary>
    /// <param name="name">             The name.</param>
    /// <param name="branchName">       Name of the branch.</param>
    /// <param name="directory">        [out] Pathname of the directory.</param>
    /// <param name="fhirVersion">      [out] The FHIR version.</param>
    /// <param name="resolvedDirective">[out] The resolved directive.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    private bool TryDownloadCoreViaCI(
        string name,
        string branchName,
        out string directory,
        out FhirSequenceCodes fhirVersion,
        out string resolvedDirective)
    {
        branchName = GetCoreBranchFromInput(branchName);

        Uri branchUri;

        switch (branchName.ToLowerInvariant())
        {
            case "master":
            case "main":
                branchUri = _ciUri;
                break;

            case null:
            case "":
                if (name.Contains("r4b", StringComparison.OrdinalIgnoreCase))
                {
                    branchUri = new Uri(_ciUri, $"branches/R4B/");
                }
                else
                {
                    branchUri = _ciUri;
                }

                break;

            default:
                branchUri = new Uri(_ciUri, $"branches/{branchName}/");
                break;
        }

        directory = Path.Combine(_cachePackageDirectory, $"{name}#current");

        string localNpmFilename = Path.Combine(directory, "package", "package.json");
        if (File.Exists(localNpmFilename))
        {
            try
            {
                FhirNpmPackageDetails cachedNpm = FhirNpmPackageDetails.Load(localNpmFilename);

                Uri versionInfoUri = new Uri(branchUri, "version.info");
                string contents = _httpClient.GetStringAsync(versionInfoUri).Result;

                ParseVersionInfoIni(
                    contents,
                    out string ciFhirVersion,
                    out string ciVersion,
                    out string ciBuildId,
                    out string ciBuildDate);

                if (cachedNpm.BuildDate.CompareTo(ciBuildDate) > 0)
                {
                    fhirVersion = ToSequence(ciFhirVersion);
                    resolvedDirective = $"{cachedNpm.Name}#{cachedNpm.Version}";
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"TryDownloadCoreViaCI <<< failed to compare local to CI, forcing download ({ex.Message})");
            }
        }

        Uri uri = new Uri(branchUri, $"{name}.tgz");

        return TryDownloadAndExtract(uri, directory, $"{name}#current", out fhirVersion, out resolvedDirective);
    }

    /// <summary>Attempts to download guide via ci a string from the given string.</summary>
    /// <param name="branchName">       Name of the branch.</param>
    /// <param name="name">             [out] The name.</param>
    /// <param name="directory">        [out] Pathname of the directory.</param>
    /// <param name="fhirVersion">      [out] The FHIR version.</param>
    /// <param name="resolvedDirective">[out] The resolved directive.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    private bool TryDownloadGuideViaCI(
        string branchName,
        out string name,
        out string directory,
        out FhirSequenceCodes fhirVersion,
        out string resolvedDirective)
    {
        branchName = GetIgBranchFromInput(branchName);

        try
        {
            Uri versionInfoUri = new Uri(_ciUri, $"ig/{branchName}/package.manifest.json");
            string contents = _httpClient.GetStringAsync(versionInfoUri).Result;

            FhirNpmPackageDetails ciNpm = FhirNpmPackageDetails.Parse(contents);

            name = ciNpm.Name;
            directory = Path.Combine(_cachePackageDirectory, $"{ciNpm.Name}#current");

            string localNpmFilename = Path.Combine(directory, "package", "package.json");

            if (File.Exists(localNpmFilename))
            {
                FhirNpmPackageDetails cachedNpm = FhirNpmPackageDetails.Load(localNpmFilename);

                if (cachedNpm.BuildDate.CompareTo(ciNpm.BuildDate) <= 0)
                {
                    fhirVersion = ToSequence(cachedNpm.FhirVersion);
                    resolvedDirective = $"{cachedNpm.Name}#{cachedNpm.Version}";
                    return true;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogInformation($"TryDownloadCoreViaCI <<< failed to compare local to CI, forcing download ({ex.Message})");
            name = string.Empty;
            directory = string.Empty;
            fhirVersion = FhirSequenceCodes.Unknown;
            resolvedDirective = string.Empty;
            return false;
        }

        Uri uri = new Uri(_ciUri, $"ig/{branchName}/package.tgz");

        return TryDownloadAndExtract(uri, directory, $"{name}#current", out fhirVersion, out resolvedDirective);
    }

    /// <summary>
    /// Attempts to get guide ci package details the NpmPackageDetails from the given string.
    /// </summary>
    /// <param name="branchName">Name of the branch.</param>
    /// <param name="details">   [out] The details.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    private bool TryGetGuideCiPackageDetails(string branchName, out FhirNpmPackageDetails details)
    {
        branchName = GetIgBranchFromInput(branchName);

        try
        {
            Uri versionInfoUri = new Uri(_ciUri, $"ig/{branchName}/package.manifest.json");

            string contents = _httpClient.GetStringAsync(versionInfoUri).Result;

            details = FhirNpmPackageDetails.Parse(contents);

            if (details.Url == null)
            {
                details.Url = new Uri(_ciUri, $"ig/{branchName}").ToString();
            }

            if (string.IsNullOrEmpty(details.Title))
            {
                details.Title = $"FHIR IG: {details.Name}";
            }

            if (string.IsNullOrEmpty(details.Description))
            {
                details.Description = $"CI Build from branch {branchName}, current as of: {DateTime.Now}";
            }

            if (string.IsNullOrEmpty(details.PackageType))
            {
                details.PackageType = "ig";
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogInformation($"TryGetGuideCiPackageDetails <<< failed to find CI IG ({ex.Message})");
        }

        details = null!;
        return false;
    }

    /// <summary>Package base for sequence.</summary>
    /// <param name="seq">The sequence.</param>
    /// <returns>A string.</returns>
    private static string PackageBaseForSequence(FhirSequenceCodes seq) => seq switch
    {
        FhirSequenceCodes.R4B => "hl7.fhir.r4b",
        _ => $"hl7.fhir.r{(int)seq}",
    };

    /// <summary>Attempts to get core ci package details.</summary>
    /// <param name="branchName">Name of the branch.</param>
    /// <param name="details">   [out] The details.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    private bool TryGetCoreCiPackageDetails(
        string branchName,
        out FhirNpmPackageDetails details)
    {
        branchName = GetCoreBranchFromInput(branchName);

        Uri branchUri;

        switch (branchName.ToLowerInvariant())
        {
            case "master":
            case "main":
            case "":
                branchUri = _ciUri;
                break;

            case "r4b":
                branchUri = new Uri(_ciUri, $"branches/R4B/");
                break;

            default:
                branchUri = new Uri(_ciUri, $"branches/{branchName}/");
                break;
        }

        try
        {
            Uri versionInfoUri = new Uri(branchUri, "version.info");
            string contents = _httpClient.GetStringAsync(versionInfoUri).Result;

            ParseVersionInfoIni(
                contents,
                out string fhirVersion,
                out string version,
                out string buildId,
                out string buildDate);

            FhirSequenceCodes sequence = ToSequence(version);

            string corePackageName = PackageBaseForSequence(sequence) + ".core";

            details = new FhirNpmPackageDetails()
            {
                Name = corePackageName,
                Version = version,
                BuildDate = buildDate,
                FhirVersionList = new string[1] { version },
                FhirVersions = new string[1] { version },
                PackageType = "core",
                ToolsVersion = 3M,
                Url = branchUri.ToString(),
                Title = $"FHIR {sequence}: {fhirVersion}",
                Description = $"CI Build from branch {branchName}, current as of: {DateTime.Now}",
                Dependencies = new(),
            };

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogInformation($"TryGetCoreCiPackageDetails <<< failed to get CI package details: {ex.Message}");
        }

        details = null!;
        return false;
    }

    /// <summary>Attempts to download guide via ci a string from the given string.</summary>
    /// <param name="name">      The name.</param>
    /// <param name="branchName">Name of the branch.</param>
    /// <param name="directory"> [out] Pathname of the directory.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    private bool TryDownloadGuideViaCI(
        string name,
        string branchName,
        out string directory,
        out FhirSequenceCodes fhirVersion,
        out string resolvedDirective)
    {
        directory = Path.Combine(_cachePackageDirectory, $"{name}#current");

        branchName = GetIgBranchFromInput(branchName);

        string localNpmFilename = Path.Combine(directory, "package", "package.json");
        if (File.Exists(localNpmFilename))
        {
            try
            {
                FhirNpmPackageDetails cachedNpm = FhirNpmPackageDetails.Load(localNpmFilename);

                Uri versionInfoUri = new Uri(_ciUri, $"ig/{branchName}/package.manifest.json");

                string contents = _httpClient.GetStringAsync(versionInfoUri).Result;

                FhirNpmPackageDetails ciNpm = FhirNpmPackageDetails.Parse(contents);

                if (cachedNpm.BuildDate.CompareTo(ciNpm.BuildDate) <= 0)
                {
                    fhirVersion = ToSequence(ciNpm.FhirVersion);
                    resolvedDirective = $"{ciNpm.Name}#{ciNpm.Version}";
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"TryDownloadCoreViaCI <<< failed to compare local to CI, forcing download ({ex.Message})");
                fhirVersion = FhirSequenceCodes.Unknown;
                resolvedDirective = string.Empty;
                return false;
            }
        }

        Uri uri = new Uri(_ciUri, $"ig/{branchName}/package.tgz");

        return TryDownloadAndExtract(uri, directory, $"{name}#current", out fhirVersion, out resolvedDirective);
    }

    /// <summary>
    /// Attempts to get relative base for version a string from the given string.
    /// Note that this does not work for ballot versions of core, but that requires
    /// tracking versions individually. As this is only a fallback for when the
    /// package servers are offline, it feels like a reasonable compromise.
    /// </summary>
    /// <param name="version"> [out] The version string (e.g., 4.0.1).</param>
    /// <param name="relative">[out] The relative.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    private static bool TryGetRelativeBaseForVersion(string version, out string relative)
    {
        // versions with a dash are not promoted to their version name root
        if (version.Contains('-'))
        {
            relative = version;
            return true;
        }

        FhirSequenceCodes sequence = ToSequence(version);

        if (sequence == FhirSequenceCodes.Unknown)
        {
            relative = string.Empty;
            return false;
        }

        // major releases are promoted to their version name root
        relative = ToLiteral(sequence);
        return true;
    }

    /// <summary>Attempts to download core via publication a string from the given string.</summary>
    /// <param name="name">             The name.</param>
    /// <param name="version">          [out] The version string (e.g., 4.0.1).</param>
    /// <param name="directory">        [out] Pathname of the directory.</param>
    /// <param name="fhirVersion">      [out] The FHIR version.</param>
    /// <param name="resolvedDirective">[out] The resolved directive.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    private bool TryDownloadCoreViaPublication(
        string name,
        string version,
        out string directory,
        out FhirSequenceCodes fhirVersion,
        out string resolvedDirective)
    {
        if (!TryGetRelativeBaseForVersion(version, out string relative))
        {
            directory = string.Empty;
            fhirVersion = FhirSequenceCodes.Unknown;
            resolvedDirective = string.Empty;
            return false;
        }

        string directive = name + "#" + version;
        directory = Path.Combine(_cachePackageDirectory, directive);

        // most publication versions are named with correct package information
        Uri uri = new Uri(_publicationUri, $"{relative}/{name}.tgz");
        if (TryDownloadAndExtract(uri, directory, directive, out fhirVersion, out resolvedDirective))
        {
            UpdatePackageCacheIndex(directive, directory);
            return true;
        }

        // some ballot versions are published directly as CI versions
        uri = new Uri(_publicationUri, $"{relative}/package.tgz");
        directory = Path.Combine(_cachePackageDirectory, $"hl7.fhir.core#{version}");
        if (TryDownloadAndExtract(uri, directory, directive, out fhirVersion, out resolvedDirective))
        {
            UpdatePackageCacheIndex(directive, directory);
            return true;
        }

        return false;
    }

    /// <summary>Gets package name from canonical.</summary>
    /// <param name="input">The input.</param>
    /// <returns>The package name from canonical.</returns>
    private static string GetPackageNameFromInput(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return string.Empty;
        }

        if (input.StartsWith("http", StringComparison.OrdinalIgnoreCase))
        {
            if (input.EndsWith('/'))
            {
                return input.Substring(0, input.Length - 1).Split('/').Last();
            }

            return input.Split('/').Last();
        }

        return input;
    }

    /// <summary>Gets ig branch from input.</summary>
    /// <param name="input">The input.</param>
    /// <returns>The ig branch from input.</returns>
    private static string GetIgBranchFromInput(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return string.Empty;
        }

        string branchName = input;

        if (branchName.StartsWith("http://build.fhir.org/ig/", StringComparison.OrdinalIgnoreCase))
        {
            branchName = branchName.Substring(25);
        }
        else if (branchName.StartsWith("https://build.fhir.org/ig/", StringComparison.OrdinalIgnoreCase))
        {
            branchName = branchName.Substring(26);
        }
        else if (branchName.StartsWith("ig/", StringComparison.OrdinalIgnoreCase))
        {
            branchName = branchName.Substring(3);
        }

        if (branchName.EndsWith(".html", StringComparison.OrdinalIgnoreCase))
        {
            int last = branchName.LastIndexOf('/');
            branchName = branchName.Substring(0, last);
        }

        return branchName;
    }

    /// <summary>Gets core branch from input.</summary>
    /// <param name="input">The input.</param>
    /// <returns>The core branch from input.</returns>
    private static string GetCoreBranchFromInput(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return string.Empty;
        }

        string branchName = input;

        if (branchName.StartsWith("http://build.fhir.org/branches/", StringComparison.OrdinalIgnoreCase))
        {
            branchName = branchName.Substring(31);
        }
        else if (branchName.StartsWith("https://build.fhir.org/branches/", StringComparison.OrdinalIgnoreCase))
        {
            branchName = branchName.Substring(32);
        }
        else if (branchName.StartsWith("branches/", StringComparison.OrdinalIgnoreCase))
        {
            branchName = branchName.Substring(9);
        }

        if (branchName.Contains('/'))
        {
            branchName = branchName.Split('/')[0];
        }

        return branchName;
    }

    /// <summary>Query if 'name' has cached version.</summary>
    /// <param name="name">     The name.</param>
    /// <param name="version">  [out] The version string (e.g., 4.0.1).</param>
    /// <param name="directory">[out] Pathname of the directory.</param>
    /// <returns>True if cached version, false if not.</returns>
    private bool HasCachedVersion(
        string name,
        string version,
        out string directory)
    {
        directory = Path.Combine(_cachePackageDirectory, $"{name}#{version}");

        if (Directory.Exists(directory))
        {
            return true;
        }

        directory = string.Empty;
        return false;
    }

    /// <summary>Attempts to get highest version.</summary>
    /// <param name="name">       The name.</param>
    /// <param name="offlineMode">True to enable offline mode, false to disable it.</param>
    /// <param name="version">    [out] The version string (e.g., 4.0.1).</param>
    /// <param name="isCached">   [out] True if is cached, false if not.</param>
    /// <param name="directory">  [out] Pathname of the directory.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    private bool TryGetHighestVersion(
        string name,
        bool offlineMode,
        out string version,
        out bool isCached,
        out string directory)
    {
        string highestCached = string.Empty;
        string highestOnline = string.Empty;

        _ = TryGetHighestVersionOffline(name, out highestCached);

        if (!offlineMode)
        {
            TryGetHighestVersionOnline(name, out highestOnline);
        }

        if (string.IsNullOrEmpty(highestCached) && string.IsNullOrEmpty(highestOnline))
        {
            version = string.Empty;
            isCached = false;
            directory = string.Empty;
            return false;
        }

        if (highestCached.Equals(highestOnline, StringComparison.Ordinal))
        {
            version = highestCached;
            isCached = true;
            directory = Path.Combine(_cachePackageDirectory, $"{name}#{version}");
            return true;
        }

        if (RegistryPackageManifest.IsFirstHigherVersion(highestCached, highestOnline))
        {
            version = highestCached;
            isCached = true;
            directory = Path.Combine(_cachePackageDirectory, $"{name}#{version}");
            return true;
        }

        version = highestOnline;
        isCached = false;
        directory = string.Empty;
        return true;
    }

    /// <summary>Attempts to get highest version of a package from the local cache.</summary>
    /// <param name="name">   The name.</param>
    /// <param name="version">[out] The version string (e.g., 4.0.1).</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    private bool TryGetHighestVersionOffline(
        string name,
        out string version)
    {
        if (!_versionsByName.ContainsKey(name))
        {
            version = string.Empty;
            return false;
        }

        string highestVersion = string.Empty;

        foreach (string cachedVersion in _versionsByName[name])
        {
            if (cachedVersion.Equals("dev", StringComparison.OrdinalIgnoreCase) ||
                cachedVersion.Equals("current", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (RegistryPackageManifest.IsFirstHigherVersion(cachedVersion, highestVersion))
            {
                highestVersion = cachedVersion;
            }
        }

        if (string.IsNullOrEmpty(highestVersion))
        {
            version = string.Empty;
            return false;
        }

        version = highestVersion;
        return true;
    }

    /// <summary>
    /// Attempts to get highest version of a package from the package registries online.
    /// </summary>
    /// <param name="name">   The name.</param>
    /// <param name="version">[out] The version string (e.g., 4.0.1).</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    private bool TryGetHighestVersionOnline(
        string name,
        out string version)
    {
        if (!TryGetPackageManifests(name, out IEnumerable<RegistryPackageManifest> manifests))
        {
            version = string.Empty;
            return false;
        }

        string highestVersion = string.Empty;

        foreach (RegistryPackageManifest manifest in manifests)
        {
            string manifestHighest = manifest.HighestVersion();

            if (RegistryPackageManifest.IsFirstHigherVersion(manifestHighest, highestVersion))
            {
                highestVersion = manifestHighest;
            }
        }

        version = highestVersion;
        return !string.IsNullOrEmpty(version);
    }

    /// <summary>Attempts to get a package manifest for a given package.</summary>
    /// <param name="packageName">Name of the package.</param>
    /// <param name="manifests">  [out] The manifest.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    private bool TryGetPackageManifests(string packageName, out IEnumerable<RegistryPackageManifest> manifests)
    {
        List<RegistryPackageManifest> manifestList = new();

        packageName = GetPackageNameFromInput(packageName);

        foreach (Uri registryUri in _defaultRegistryUris)
        {
            try
            {
                Uri requestUri = new(registryUri, packageName);

                HttpResponseMessage response = _httpClient.GetAsync(requestUri).Result;

                if (!response.IsSuccessStatusCode)
                {
                    //_logger.LogInformation(
                    //    $"GetPackageVersionsAndUrls <<<" +
                    //    $" Failed to get package info: {response.StatusCode}" +
                    //    $" {requestUri.AbsoluteUri}");
                    continue;
                }

                string json = response.Content.ReadAsStringAsync().Result;

                RegistryPackageManifest? info = RegistryPackageManifest.Parse(json);

                if (!(info?.Name.Equals(packageName, StringComparison.OrdinalIgnoreCase) ?? false))
                {
                    //_logger.LogInformation(
                    //    $"GetPackageVersionsAndUrls <<<" +
                    //    $" Package information mismatch: requested {requestUri.AbsoluteUri}" +
                    //    $" received manifest for {info?.Name}");
                    continue;
                }

                if (info.Versions == null || info.Versions.Count == 0)
                {
                    //_logger.LogInformation(
                    //    $"GetPackageVersionsAndUrls <<<" +
                    //    $" package {requestUri.AbsoluteUri}" +
                    //    $" contains NO versions");
                    continue;
                }

                manifestList.Add(info);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(
                    $"GetPackageVersionsAndUrls <<<" +
                    $" Server {registryUri.AbsoluteUri}" +
                    $" Package {packageName}" +
                    $" threw: {ex.Message}");
                if (ex.InnerException != null)
                {
                    _logger.LogInformation($" <<< {ex.InnerException.Message}");
                }
            }
        }

        if (manifestList.Any())
        {
            manifests = manifestList.AsEnumerable();
            return true;
        }

        //_logger.LogInformation(
        //    $"Package {packageName}" +
        //    $" was not found on any registry.");
        manifests = Enumerable.Empty<RegistryPackageManifest>();
        return false;
    }

    /// <summary>Discover cached packages.</summary>
    private void SynchronizeCache()
    {
        bool modified = false;

        IniParser.IniDataParser parser = new();

        IniData data = parser.Parse(File.ReadAllText(_iniFilePath));

        if (!data.Sections.Contains("packages"))
        {
            _packagesByDirective.Clear();
            _versionsByName.Clear();
            return;
        }

        List<string> directivesToRemove = new();

        foreach (IniParser.Model.Property? line in data["packages"])
        {
            if (line == null)
            {
                continue;
            }

            ProcessSync(data, line.Key, line.Value, out bool shouldRemove);

            if (shouldRemove)
            {
                directivesToRemove.Add(line.Key);
            }
        }

        foreach (string directive in directivesToRemove)
        {
            _logger.LogInformation($" <<< removing {directive} from ini");

            if (data["packages"].Contains(directive))
            {
                data["packages"].Remove(directive);
            }

            if (data["package-sizes"].Contains(directive))
            {
                data["package-sizes"].Remove(directive);
            }

            modified = true;
        }

        IEnumerable<string> directories = Directory.EnumerateDirectories(_cachePackageDirectory, "*", SearchOption.TopDirectoryOnly);

        foreach (string directory in directories)
        {
            string directive = Path.GetFileName(directory);

            if (!data["packages"].Contains(directive))
            {
                _logger.LogInformation($" <<< adding {directive} to ini");
                UpdatePackageCacheIndex(directive, directory, data);
                modified = true;
                ProcessSync(data, directive, data["packages"][directive], out _);
            }
        }

        if (modified)
        {
            SaveIniData(_iniFilePath, data);
        }

        _logger.LogInformation($" << cache contains {_packagesByDirective.Count} packages");
    }

    /// <summary>Updates the package state.</summary>
    /// <param name="directive">      The directive.</param>
    /// <param name="resolvedName">   Name of the resolved.</param>
    /// <param name="resolvedVersion">The resolved version.</param>
    /// <param name="toState">        State of to.</param>
    private void UpdatePackageState(
        string directive,
        string resolvedName,
        string resolvedVersion,
        PackageLoadStateCodes toState)
    {
        if (!_packagesByDirective.ContainsKey(directive))
        {
            _packagesByDirective.Add(directive, new()
            {
                CacheDirective = directive,
                PackageState = toState,
            });
        }

        _packagesByDirective[directive] = _packagesByDirective[directive] with
        {
            PackageState = toState,
            PackageName = string.IsNullOrEmpty(resolvedName) ? _packagesByDirective[directive].PackageName : resolvedName,
            Version = string.IsNullOrEmpty(resolvedVersion) ? _packagesByDirective[directive].Version : resolvedVersion,
        };

        StateHasChanged();
    }

    /// <summary>
    /// Attempts to get a package state, returning a default value rather than throwing an exception
    /// if it fails.
    /// </summary>
    /// <param name="directive">The directive.</param>
    /// <param name="state">    [out] The state.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    private bool TryGetPackageState(string directive, out PackageLoadStateCodes state)
    {
        if (!_packagesByDirective.ContainsKey(directive))
        {
            state = PackageLoadStateCodes.Unknown;
            return false;
        }

        state = _packagesByDirective[directive].PackageState;
        return true;
    }

    /// <summary>Process the synchronize.</summary>
    /// <param name="data">        The data.</param>
    /// <param name="directive">   The directive.</param>
    /// <param name="shouldRemove">[out] True if should remove.</param>
    private void ProcessSync(
        IniData data,
        string directive,
        string packageDate,
        out bool shouldRemove)
    {
        if (!directive.Contains('#'))
        {
            _logger.LogInformation($" <<< unknown package directive: {directive}");
            shouldRemove = true;
            return;
        }

        if (!Directory.Exists(Path.Combine(_cachePackageDirectory, directive)))
        {
            _logger.LogInformation($"SynchronizeCache <<< removing entry {directive}, directory not found!");
            shouldRemove = true;
            return;
        }

        string[] components = directive.Split('#', StringSplitOptions.TrimEntries);
        if (components.Length != 2)
        {
            _logger.LogInformation($"SynchronizeCache <<< unparseable package directive: {directive}");
            shouldRemove = true;
            return;
        }

        shouldRemove = false;

        string name = components[0];
        string version = components[1];
        long size = -1;
        FhirNpmPackageDetails versionInfo;

        if (data.Sections.Contains("package-sizes") &&
            data["package-sizes"].Contains(directive))
        {
            _ = long.TryParse(data["package-sizes"][directive], out size);
        }

        try
        {
            versionInfo = FhirNpmPackageDetails.Load(Path.Combine(_cachePackageDirectory, directive));
        }
        catch (Exception ex)
        {
            _logger.LogInformation($"DiscoverCachedPackages <<< skipping package: {directive} - {ex.Message}");
            if (ex.InnerException != null)
            {
                _logger.LogInformation($" <<< inner: {ex.InnerException.Message}");
            }

            return;
        }

        PackageCacheRecord record = new(
                directive,
                PackageLoadStateCodes.NotLoaded,
                name,
                version,
                ToSequence(versionInfo.FhirVersion),
                packageDate,
                size,
                versionInfo);

        _packagesByDirective[directive] = record;

        if (!_versionsByName.ContainsKey(name))
        {
            _versionsByName.Add(name, new());
        }

        _versionsByName[name].Add(version);
    }

    /// <summary>Gets local version information.</summary>
    /// <exception cref="FileNotFoundException">Thrown when the requested file is not present.</exception>
    /// <param name="contents">   The contents.</param>
    /// <param name="fhirVersion">[out] The FHIR version.</param>
    /// <param name="version">    [out] The version string (e.g., 4.0.1).</param>
    /// <param name="buildId">    [out] Identifier for the build.</param>
    /// <param name="buildDate">  [out] The build date.</param>
    private static void ParseVersionInfoIni(
        string contents,
        out string fhirVersion,
        out string version,
        out string buildId,
        out string buildDate)
    {
        IEnumerable<string> lines = contents.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

        fhirVersion = string.Empty;
        version = string.Empty;
        buildId = string.Empty;
        buildDate = string.Empty;

        foreach (string line in lines)
        {
            if (!line.Contains('=', StringComparison.Ordinal))
            {
                continue;
            }

            string[] kvp = line.Split('=');

            if (kvp.Length != 2)
            {
                continue;
            }

            switch (kvp[0])
            {
                case "FhirVersion":
                    fhirVersion = kvp[1];
                    break;

                case "version":
                    version = kvp[1];
                    break;

                case "buildId":
                    buildId = kvp[1];
                    break;

                case "date":
                    buildDate = kvp[1];
                    break;
            }
        }
    }

    /// <summary>Creates empty cache initialize.</summary>
    private void CreateEmptyCacheIni()
    {
        IniData data = new();

        data.Sections.Add(new IniParser.Model.Section("cache"));
        data["cache"].Add("version", "3");

        data.Sections.Add(new IniParser.Model.Section("urls"));

        data.Sections.Add(new IniParser.Model.Section("local"));

        data.Sections.Add(new IniParser.Model.Section("packages"));

        data.Sections.Add(new IniParser.Model.Section("package-sizes"));

        SaveIniData(_iniFilePath, data);
    }

    /// <summary>Saves an initialize data.</summary>
    /// <param name="destinationPath">Full pathname of the destination file.</param>
    /// <param name="data">           The data.</param>
    private void SaveIniData(string destinationPath, IniData data)
    {
        IniFormattingConfiguration formattingConfig = new()
        {
            NewLineType = IniParser.Configuration.IniFormattingConfiguration.ENewLine.Windows,
        };

        IniDataFormatter formatter = new();

        File.WriteAllText(destinationPath, formatter.Format(data, formattingConfig));
    }

    /// <summary>
    /// Releases the unmanaged resources used by the <see cref="FhirCache"/>
    /// and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">True to release both managed and unmanaged resources; false to
    ///  release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            _disposedValue = true;
        }
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
    /// resources.
    /// </summary>
    void IDisposable.Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>State has changed.</summary>
    public void StateHasChanged()
    {
        OnChanged?.Invoke(this, new());
    }
}
