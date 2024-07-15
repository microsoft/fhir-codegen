// <copyright file="FhirCache.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using IniParser.Configuration;
using IniParser;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Health.Fhir.CodeGenCommon.Packaging;
using Microsoft.Health.Fhir.PackageManager.Models;
using System.Collections.Concurrent;
using System.Formats.Tar;
using System.Globalization;
using System.IO.Compression;
using System.Net;
using System.Text.RegularExpressions;
using System.Text.Json;
using static Microsoft.Health.Fhir.CodeGenCommon.Packaging.FhirReleases;
using static Microsoft.Health.Fhir.PackageManager.Models.FhirDirective;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System;

namespace Microsoft.Health.Fhir.PackageManager;

/// <summary>A FHIR cache.</summary>
public partial class FhirCache : IFhirPackageClient, IDisposable
{
    private const string WarningMsgDefault = "{Function} <<< caught exception: {Message}, inner: {Inner}";
    private const string WarningMsgEx = "{Function} <<< {Info} caught exception: {Message}, inner: {Inner}";

    /// <summary>Information about a package in the cache.</summary>
    internal readonly record struct PackageCacheRecord(
        string CacheDirective,
        string PackageName,
        string Version,
        FhirSequenceCodes FhirVersion,
        string DownloadDateTime,
        long PackageSize,
        FhirNpmPackageDetails Details);

    /// <summary>(Immutable) The package registry URIs.</summary>
    private static readonly Uri[] _defaultRegistryUris =
    [
        new("http://packages.fhir.org/"),
        new("http://packages2.fhir.org/packages/"),
    ];

    /// <summary>The registry URIs.</summary>
    private static IEnumerable<Uri> _registryUris = _defaultRegistryUris;

    /// <summary>(Immutable) URI of the FHIR published server.</summary>
    private static readonly Uri _publicationUri = new("http://hl7.org/fhir/");

    /// <summary>(Immutable) URI of the FHIR CI server.</summary>
    private static readonly Uri _ciUri = new("http://build.fhir.org/");

    /// <summary>(Immutable) The ci URI using HTTPS.</summary>
    private static readonly Uri _ciUriS = new("https://build.fhir.org/");

    /// <summary>(Immutable) URI of the qas.</summary>
    private static readonly Uri _qasUri = new("https://build.fhir.org/ig/qas.json");

    /// <summary>(Immutable) URI of the ig list.</summary>
    private static readonly Uri _igListUri = new("https://github.com/FHIR/ig-registry/blob/master/fhir-ig-list.json");

    /// <summary>The logger.</summary>
    private ILogger _logger;

    /// <summary>Pathname of the cache directory.</summary>
    private string _cacheDirectory = string.Empty;

    /// <summary>Pathname of the cache package directory.</summary>
    private string _cachePackageDirectory = string.Empty;

    /// <summary>Full pathname of the initialize file.</summary>
    private string _iniFilePath = string.Empty;

    /// <summary>The initialize file lock.</summary>
    private static object _iniFileLock = new();

    /// <summary>True to enable offline mode, false to disable it.</summary>
    private bool _offlineMode = false;

    /// <summary>The HTTP client.</summary>
    /// <remarks>Internal so that tests can replace the message handler.</remarks>
    internal static HttpClient _httpClient = new();

    /// <summary>True to disposed value.</summary>
    private bool _disposedValue = false;

    /// <summary>The package records, by directive.</summary>
    private Dictionary<string, PackageCacheRecord> _packagesByDirective = [];

    /// <summary>Package versions, by package name.</summary>
    private Dictionary<string, List<string>> _versionsByName = [];

    /// <summary>Occurs when a package has been downloaded or deleted.</summary>
    public event EventHandler<EventArgs>? OnChanged = null;

    /// <summary>Test if a name is the correct root for a core package.</summary>
    /// <returns>A RegEx.</returns>
    [GeneratedRegex("^hl7\\.fhir\\.r\\d+[A-Za-z]?$")]
    internal static partial Regex MatchCorePackageRoot();

    /// <summary>Test if a name is the correct root for a core package.</summary>
    internal static Regex _matchCorePackageRoot = MatchCorePackageRoot();

    /// <summary>Test if a name has a FHIR version suffix.</summary>
    /// <returns>A RegEx.</returns>
    [GeneratedRegex("\\.r\\d+[A-Za-z]?$")]
    internal static partial Regex MatchFhirSuffix();

    /// <summary>Test if a name has a FHIR version suffix.</summary>
    internal static Regex _matchFhirSuffix = MatchFhirSuffix();

    /// <summary>Match file URL suffix.</summary>
    /// <returns>A RegEx.</returns>
    [GeneratedRegex("\\.(html|htm|json|xml|tgz|zip|txt)$")]
    internal static partial Regex MatchFileUrlSuffix();

    /// <summary>The match file URL suffix.</summary>
    internal static Regex _matchFileUrlSuffix = MatchFileUrlSuffix();

    /// <summary>The RegEx to test if a string is a Semver version.</summary>
    internal static Regex _matchSemver = MatchSemVer();

    /// <summary>A RegEx to test if a string is a semver version.</summary>
    /// <remarks>Copied from https://semver.org/#is-there-a-suggested-regular-expression-regex-to-check-a-semver-string</remarks>
    [GeneratedRegex("^(0|[1-9]\\d*)\\.(0|[1-9]\\d*)\\.(0|[1-9]\\d*)(?:-((?:0|[1-9]\\d*|\\d*[a-zA-Z-][0-9a-zA-Z-]*)(?:\\.(?:0|[1-9]\\d*|\\d*[a-zA-Z-][0-9a-zA-Z-]*))*))?(?:\\+([0-9a-zA-Z-]+(?:\\.[0-9a-zA-Z-]+)*))?$")]
    internal static partial Regex MatchSemVer();

    /// <summary>The is package literal.</summary>
    internal static Regex _matchPackageLiteral = MatchPackageLiteral();

    /// <summary>Is package literal.</summary>
    /// <returns>A RegEx.</returns>
    [GeneratedRegex("package(\\.r.+)?\\.tgz")]
    internal static partial Regex MatchPackageLiteral();

    /// <summary>The is FHIR version literal.</summary>
    internal static Regex _matchFhirVersionLiteral = MatchFhirVersionLiteral();

    /// <summary>Is FHIR version literal.</summary>
    /// <returns>A RegEx.</returns>
    [GeneratedRegex("DSTU2|STU3|R[0-9]+[A-Z]*")]
    internal static partial Regex MatchFhirVersionLiteral();

    /// <summary>The is ballot literal.</summary>
    internal static Regex _matchBallotLiteral = MatchBallotLiteral();

    /// <summary>Is ballot literal.</summary>
    /// <returns>A RegEx.</returns>
    [GeneratedRegex("[0-9]{4}[A-Z]{1}[a-z]{2}")]
    internal static partial Regex MatchBallotLiteral();

    /// <summary>Match file URL suffix.</summary>
    /// <returns>A RegEx.</returns>
    [GeneratedRegex("^(http|https)://(hl7\\.org|www\\.hl7\\.org)/fhir/(?'literal'.+).*$")]
    internal static partial Regex MatchProdCoreUrl();

    /// <summary>The match file URL suffix.</summary>
    internal static Regex _matchProdCoreUrl = MatchProdCoreUrl();

    /// <summary>Match product ig realm URL.</summary>
    /// <returns>A RegEx.</returns>
    [GeneratedRegex("^(http|https)://(hl7\\.org|www\\.hl7\\.org)/fhir/[a-z]{2}/(?'name'.+)$")]
    internal static partial Regex MatchProdIgRealmUrl();

    /// <summary>URL of the match product ig realm.</summary>
    internal static Regex _matchProdIgRealmUrl = MatchProdIgRealmUrl();

    /// <summary>Match product ig realm URL.</summary>
    /// <returns>A RegEx.</returns>
    [GeneratedRegex("^(http|https)://(hl7\\.org|www\\.hl7\\.org)/fhir/(?'name'.+)$")]
    internal static partial Regex MatchProdIgNoRealmUrl();

    /// <summary>URL of the match product ig realm.</summary>
    internal static Regex _matchProdIgNoRealmUrl = MatchProdIgNoRealmUrl();

    /// <summary>Match file URL suffix.</summary>
    /// <returns>A RegEx.</returns>
    [GeneratedRegex("^(http|https)://build\\.fhir\\.org/[^/]*$")]
    internal static partial Regex MatchCiCoreCurrentUrl();

    /// <summary>The match file URL suffix.</summary>
    internal static Regex _matchCiCoreCurrentUrl = MatchCiCoreCurrentUrl();

    /// <summary>Match ci core branch URL.</summary>
    /// <returns>A RegEx.</returns>
    [GeneratedRegex("^(http|https)://build\\.fhir\\.org/branches/(?'branch'[^/\\s]*)(/.*)*$")]
    internal static partial Regex MatchCiCoreBranchUrl();

    /// <summary>URL of the match ci core branch.</summary>
    internal static Regex _matchCiCoreBranchUrl = MatchCiCoreBranchUrl();

    /// <summary>Match ci ig branch URL.</summary>
    /// <returns>A RegEx.</returns>
    [GeneratedRegex("^(http|https)://build\\.fhir\\.org/ig/(?'org'[^/\\s]*)/(?'repo'[^/\\s]*)/branches/(?'branch'[^/\\s]*)(/.*)*$")]
    internal static partial Regex MatchCiIgBranchUrl();

    /// <summary>URL of the match ci ig branch.</summary>
    internal static Regex _matchCiIgBranchUrl = MatchCiIgBranchUrl();

    /// <summary>Match ci ig current URL.</summary>
    /// <returns>A RegEx.</returns>
    [GeneratedRegex("^(http|https)://build\\.fhir\\.org/ig/(?'org'[^/\\s]*)/(?'repo'[^/\\s]*)(/.*)*$")]
    internal static partial Regex MatchCiIgUrl();

    /// <summary>URL of the match ci ig current.</summary>
    internal static Regex _matchCiIgUrl = MatchCiIgUrl();

    /// <summary>The jso case insensitive.</summary>
    private static JsonSerializerOptions _jsoCaseInsensitive = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    /// <summary>The jso allow trailing commas.</summary>
    private static JsonSerializerOptions _jsoAllowTrailingCommas = new()
    {
        AllowTrailingCommas = true,
    };

    /// <summary>Gets the packages by directive.</summary>
    private Dictionary<string, PackageCacheRecord> PackagesByDirective => _packagesByDirective;

    /// <summary>The completed requests.</summary>
    private HashSet<string> _processed = [];

    /// <summary>Creates a new IFhirPackageClient.</summary>
    /// <param name="settings">(Optional) Options for controlling the operation.</param>
    /// <returns>An IFhirPackageClient.</returns>
    public static IFhirPackageClient Create(FhirPackageClientSettings? settings = null)
    {
        string cachePath;

        if (string.IsNullOrEmpty(settings?.CachePath))
        {
            cachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".fhir");
        }
        else if (settings.CachePath.StartsWith('~'))
        {
            char second = settings.CachePath.Length > 2 ? settings.CachePath[1] : char.MinValue;

            if (second == '/' || second == '\\')
            {
                cachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), settings.CachePath[2..]);
            }
            else
            {
                cachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), settings.CachePath[1..]);
            }
        }
        else
        {
            cachePath = settings.CachePath;
        }

        FhirCache fhirCache = new(
            cachePath,
            settings?.OfflineMode ?? false,
            null,
            settings?.AdditionalFhirRegistryUrls ?? []);

        return fhirCache;
    }

    /// <summary>Adds a local package to the FHIR package cache.</summary>
    /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
    /// <exception cref="FileNotFoundException">Thrown when the requested file is not present.</exception>
    /// <param name="packageFilename">  Full pathname of the package file ([path][name].tgz).</param>
    /// <param name="cacheVersionAlias">(Optional) The cache version alias (e.g., 'dev').</param>
    /// <param name="cancellationToken">(Optional) A token that allows processing to be cancelled.</param>
    /// <returns>An asynchronous result that yields a PackageCacheEntry?</returns>
    public async Task<PackageCacheEntry?> AddLocalPackage(
        string packageFilename,
        string cacheVersionAlias = "",
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(packageFilename))
        {
            _logger.LogError("AddLocalPackage <<< packageFilename is required");
            throw new ArgumentNullException(nameof(packageFilename));
        }

        if (!File.Exists(packageFilename))
        {
            _logger.LogError($"AddLocalPackage <<< packageFilename `{packageFilename}` does not exist");
            throw new FileNotFoundException("packageFilename does not exist", packageFilename);
        }

        string packageDirective = string.Empty;
        string directory = string.Empty;
        string name = string.Empty;

        try
        {
            using (FileStream fs = new(packageFilename, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (Stream gzipStream = new GZipStream(fs, CompressionMode.Decompress))
            using (TarReader tarReader = new(gzipStream))
            {
                // since we do not know the contents of the package tar, we need to pull the package manifest
                TarEntry? entry = tarReader.GetNextEntry();
                while (entry != null)
                {
                    // skip any entries without a data stream
                    if (entry.DataStream == null)
                    {
                        entry = tarReader.GetNextEntry();
                        continue;
                    }

                    switch (entry.Name.ToLowerInvariant())
                    {
                        case "package/package.json":
                            {
                                if (entry.DataStream == null)
                                {
                                    continue;
                                }

                                using (StreamReader sr = new(entry.DataStream))
                                {
                                    string manifest = await sr.ReadToEndAsync(cancellationToken);

                                    CachePackageManifest? packageManifest = JsonSerializer.Deserialize<CachePackageManifest>(manifest);

                                    if (packageManifest == null)
                                    {
                                        _logger.LogError($"AddLocalPackage <<< packageFilename `{packageFilename}` does not contain a valid package manifest");
                                        return null;
                                    }

                                    // we have a valid package manifest, we can now create a cache directive
                                    name = packageManifest.Name;
                                    packageDirective = $"{packageManifest.Name}#{packageManifest.Version}";
                                }
                                break;
                            }
                    }

                    entry = tarReader.GetNextEntry();
                }

                // note that we cannot extract the contents of the package here because the GZipStream is forward-only
            }

            if (string.IsNullOrEmpty(packageDirective))
            {
                _logger.LogError($"AddLocalPackage <<< package file `{packageFilename}` does not contain a valid package manifest");
                return null;
            }

            if (string.IsNullOrEmpty(cacheVersionAlias))
            {
                directory = Path.Combine(_cachePackageDirectory, packageDirective);
            }
            else
            {
                directory = Path.Combine(_cachePackageDirectory, $"{name}#{cacheVersionAlias}");
            }

            using (FileStream fs = new(packageFilename, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (Stream gzipStream = new GZipStream(fs, CompressionMode.Decompress))
            {
                // make sure our destination directory exists
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                await TarFile.ExtractToDirectoryAsync(gzipStream, directory, true, cancellationToken);
            }

            UpdatePackageCacheIndex(packageDirective, directory);

            if (!_packagesByDirective.TryGetValue(packageDirective, out PackageCacheRecord pcr))
            {
                _logger.LogError($"AddLocalPackage <<< failed to extract package file `{packageFilename}`");
                return null;
            }

            // successful extraction
            return new PackageCacheEntry()
            {
                FhirVersion = pcr.FhirVersion,
                Directory = directory,
                ResolvedDirective = pcr.CacheDirective,
                Name = pcr.PackageName,
                Version = pcr.Version,
                ResolvedDependencies = [],
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning(WarningMsgEx, nameof(AddLocalPackage), "processing " + packageFilename, ex.Message, ex.InnerException?.Message ?? string.Empty);
        }

        // make sure to clean our directory so we don't load a partial package on next run
        try
        {
            if ((!string.IsNullOrEmpty(directory)) && Directory.Exists(directory))
            {
                Directory.Delete(directory, true);
            }
        }
        catch (Exception) { }

        return null;
    }

    /// <summary>
    /// Resolve a package directive, download the package if necessary, and return the local and
    /// extracted package information.
    /// </summary>
    /// <param name="directive">          The directive.</param>
    /// <param name="includeDependencies">(Optional) True to include, false to exclude the dependencies.</param>
    /// <param name="cancellationToken">  (Optional) A token that allows processing to be cancelled.</param>
    /// <returns>An asynchronous result that yields the package by directive.</returns>
    public async Task<PackageCacheEntry?> FindOrDownloadPackageByDirective(
        string directive,
        bool includeDependencies = false,
        CancellationToken cancellationToken = default)
    {
        PackageCacheEntry? package = await ResolveAndDownloadDirective(directive, cancellationToken: cancellationToken);

        if (package == null)
        {
            // cannot nest into dependencies
            return null;
        }

        if (includeDependencies)
        {
            List<PackageCacheEntry> dependencies = [];

            CachePackageManifest? manifest = GetManifest(package);

            if (manifest == null)
            {
                // cannot nest into dependencies
                return package;
            }

            foreach ((string name, string version) in manifest.Dependencies)
            {
                string dependencyDirective = $"{name}#{version}";

                PackageCacheEntry? dependencyPackage = await FindOrDownloadPackageByDirective(dependencyDirective, true, cancellationToken);

                if (dependencyPackage == null)
                {
                    // log the error, but continue
                    _logger.LogError($"Failed to resolve dependent package {dependencyDirective} requested by {package.ResolvedDirective}");
                    continue;
                }

                dependencies.Add(dependencyPackage);
            }

            package = package with { ResolvedDependencies = dependencies.ToArray() };
        }

        return package;
    }

    /// <summary>
    /// Resolve a package URL, download the package if necessary, and return the local and extracted
    /// package information.
    /// </summary>
    /// <param name="url">                URL of the package tgz or IG page URL.</param>
    /// <param name="includeDependencies">(Optional) True to include, false to exclude the dependencies.</param>
    /// <param name="cancellationToken">  (Optional) A token that allows processing to be cancelled.</param>
    /// <returns>An asynchronous result that yields the package by URL.</returns>
    public async Task<PackageCacheEntry?> FindOrDownloadPackageByUrl(
        string url,
        bool includeDependencies = false,
        CancellationToken cancellationToken = default)
    {
        if (!TryParseUrl(url, out FhirDirective? parsedUrl))
        {
            _logger.LogError($"Failed to resolve url {url} into a package directive");
            return null;
        }

        return await FindOrDownloadPackageByDirective(parsedUrl.Directive, includeDependencies, cancellationToken);
    }

    /// <summary>Gets local entries.</summary>
    /// <param name="name">          (Optional) Name of the package to search for.  By default, the
    ///  value is used as a 'starts-with' and case-insensitive comparison with local package names
    ///  (e.g., will return all local packages).</param>
    /// <param name="exactMatchOnly">(Optional) True to only return only exact name matches.</param>
    /// <returns>The local entries.</returns>
    public IEnumerable<PackageCacheEntry> LocalPackages(string name = "", bool exactMatchOnly = false)
    {
        return _packagesByDirective.Values.Select(pcr => new PackageCacheEntry()
        {
            FhirVersion = pcr.FhirVersion,
            Directory = Path.Combine(_cachePackageDirectory, pcr.CacheDirective),
            ResolvedDirective = pcr.CacheDirective,
            Name = pcr.PackageName,
            Version = pcr.Version,
        });
    }

    /// <summary>Gets a manifest.</summary>
    /// <param name="packageEntry">The package entry.</param>
    /// <returns>The manifest.</returns>
    public CachePackageManifest? GetManifest(PackageCacheEntry packageEntry)
    {
        if (string.IsNullOrEmpty(packageEntry.Directory) ||
            (!Directory.Exists(packageEntry.Directory)))
        {
            return null;
        }

        // default location is [directive]/package/package.json]
        string manifestPath = Path.Combine(packageEntry.Directory, "package", "package.json");

        if (!File.Exists(manifestPath))
        {
            // try removing the package directory literal
            manifestPath = Path.Combine(packageEntry.Directory, "package.json");

            if (!File.Exists(manifestPath))
            {
                return null;
            }
        }

        try
        {
            return JsonSerializer.Deserialize<CachePackageManifest>(File.ReadAllText(manifestPath));
        }
        catch (Exception ex)
        {
            _logger.LogWarning(WarningMsgDefault, nameof(GetManifest), ex.Message, ex.InnerException?.Message ?? string.Empty);
        }

        return null;
    }

    /// <summary>Gets the indexed contents (i.e., via parsing .index.json).</summary>
    /// <param name="packageEntry">The package entry.</param>
    /// <returns>The indexed contents.</returns>
    public PackageContents? GetIndexedContents(PackageCacheEntry packageEntry)
    {
        if (string.IsNullOrEmpty(packageEntry.Directory) ||
            (!Directory.Exists(packageEntry.Directory)))
        {
            return null;
        }

        // default location is [directive]/package/.index.json]
        string indexPath = Path.Combine(packageEntry.Directory, "package", ".index.json");

        if (!File.Exists(indexPath))
        {
            // try removing the package directory literal
            indexPath = Path.Combine(packageEntry.Directory, ".index.json");

            if (!File.Exists(indexPath))
            {
                return null;
            }
        }

        try
        {
            return JsonSerializer.Deserialize<PackageContents>(File.ReadAllText(indexPath), _jsoAllowTrailingCommas);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(WarningMsgDefault, nameof(GetIndexedContents), ex.Message, ex.InnerException?.Message ?? string.Empty);
        }

        return null;
    }

    /// <summary>Initializes a new instance of the <see cref="FhirCache"/> class.</summary>
    /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
    /// <exception cref="ArgumentException">    Thrown when one or more arguments have unsupported or
    ///  illegal values.</exception>
    /// <param name="fhirCachePath">         Pathname of the FHIR cache directory.</param>
    /// <param name="offlineMode">           True to enable offline mode, false to disable it.</param>
    /// <param name="logger">                The logger.</param>
    /// <param name="additionalRegistryUrls">The additional registry urls.</param>
    internal FhirCache(
        string fhirCachePath,
        bool offlineMode,
        ILogger<FhirCache>? logger,
        IEnumerable<string>? additionalRegistryUrls)
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

        if (additionalRegistryUrls?.Any() ?? false)
        {
            _registryUris = additionalRegistryUrls.Select(x => new Uri(x)).Concat(_defaultRegistryUris);
        }

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

    /// <summary>Query if 'input' is URL product core.</summary>
    /// <param name="input">    The input url.</param>
    /// <param name="segments"> The segments.</param>
    /// <param name="directive">[out] The parsed directive.</param>
    /// <returns>True if URL product core, false if not.</returns>
    private static bool IsUrlProdCore(
        string input,
        string[] segments,
        [NotNullWhen(true)] out FhirDirective? directive)
    {
        // core packages cannot have fewer than 4 segments
        if (segments.Length < 4)
        {
            directive = null;
            return false;
        }

        if (!_matchProdCoreUrl.IsMatch(input))
        {
            directive = null;
            return false;
        }

        // if there is a fifth segment, it could be a package name
        string possiblePackage = string.Empty;
        if ((segments.Length > 4) &&
            segments[4].EndsWith(".tgz", StringComparison.OrdinalIgnoreCase) &&
            !_matchPackageLiteral.IsMatch(segments[4]))
        {
            possiblePackage = segments[4].Substring(0, segments[4].Length - 4);
        }

        // check to see if we have a FHIR version literal
        if (_matchFhirVersionLiteral.IsMatch(segments[3]))
        {
            FhirSequenceCodes sequence = (FhirSequenceCodes)FhirReleases.FhirVersionToSequence(segments[3]);

            // if there was no package specified, we can still default if we know the FHIR version
            if (string.IsNullOrEmpty(possiblePackage) &&
                (sequence != FhirSequenceCodes.Unknown))
            {
                // use core package literal
                possiblePackage = $"hl7.fhir.{sequence.ToRLiteral().ToLowerInvariant()}.core";
            }

            // if we still do not have a package name, we are sunk
            if (string.IsNullOrEmpty(possiblePackage))
            {
                directive = null;
                return false;
            }

            // create a directive with 'latest', it will resolve the version later
            directive = new()
            {
                Directive = $"{possiblePackage}#latest",
                PackageId = possiblePackage,
                NameType = DirectiveNameTypeCodes.CoreFull,
                FhirRelease = sequence.ToRLiteral(),
                PackageVersion = string.Empty,
                VersionType = DirectiveVersionCodes.Latest,
                PublicationPackageUrl = $"{_publicationUri}{segments[2]}/{possiblePackage}.tgz",
            };

            return true;
        }

        // check to see if we have a FHIR ballot literal
        if (_matchBallotLiteral.IsMatch(segments[3]))
        {
            // check to see if we can resolve this ballot version
            IEnumerable<PublishedReleaseInformation> ballotMatches = FhirPublishedVersions.Values.Where(i => segments[3].Equals(i.BallotPrefix, StringComparison.Ordinal));

            if (ballotMatches.Any())
            {
                PublishedReleaseInformation ballot = ballotMatches.First();

                // if there was no package specified, we can default 
                if (string.IsNullOrEmpty(possiblePackage))
                {
                    // use core package literal
                    possiblePackage = $"hl7.fhir.{ballot.Sequence.ToRLiteral().ToLowerInvariant()}.core";
                }

                // create a directive with the ballot info
                directive = new()
                {
                    Directive = $"{possiblePackage}#{ballot.Version}",
                    PackageId = possiblePackage,
                    NameType = DirectiveNameTypeCodes.CoreFull,
                    FhirRelease = ballot.Sequence.ToRLiteral(),
                    PackageVersion = ballot.Version,
                    VersionType = DirectiveVersionCodes.Exact,
                    PublicationPackageUrl = $"{_publicationUri}{segments[2]}/{possiblePackage}.tgz",
                };

                return true;
            }

            // check to see if we have a package name we can use for the FHIR version
            if (!string.IsNullOrEmpty(possiblePackage))
            {
                string[] pnSegments = possiblePackage.Split('.');

                if (pnSegments.Length > 3)
                {
                    string rLit = pnSegments[2];

                    FhirSequenceCodes packageSequence = FhirVersionToSequence(rLit);

                    if (packageSequence != FhirSequenceCodes.Unknown)
                    {
                        // create a directive with 'latest', it will resolve the version later
                        directive = new()
                        {
                            Directive = $"{possiblePackage}#latest",
                            PackageId = possiblePackage,
                            NameType = DirectiveNameTypeCodes.CoreFull,
                            FhirRelease = packageSequence.ToRLiteral(),
                            PackageVersion = string.Empty,
                            VersionType = DirectiveVersionCodes.Latest,
                            PublicationPackageUrl = $"{_publicationUri}{segments[2]}/{possiblePackage}.tgz",
                        };

                        return true;
                    }
                }

                directive = null;
                return false;
            }
        }

        // check to see if we have a SemVer version
        if (_matchSemver.IsMatch(segments[3]))
        {
            FhirSequenceCodes sequence = FhirVersionToSequence(segments[3]);

            // if we know the version, we can default the package name
            if (string.IsNullOrEmpty(possiblePackage) &&
                (sequence != FhirSequenceCodes.Unknown))
            {
                // use core package literal
                possiblePackage = $"hl7.fhir.{sequence.ToRLiteral().ToLowerInvariant()}.core";
            }

            // if we still do not have a package name, we are sunk
            if (string.IsNullOrEmpty(possiblePackage))
            {
                directive = null;
                return false;
            }

            // create a directive with this version
            directive = new()
            {
                Directive = $"{possiblePackage}#{segments[3]}",
                PackageId = possiblePackage,
                NameType = DirectiveNameTypeCodes.CoreFull,
                FhirRelease = sequence.ToRLiteral(),
                PackageVersion = segments[3],
                VersionType = DirectiveVersionCodes.Exact,
                PublicationPackageUrl = $"{_publicationUri}{segments[2]}/{possiblePackage}.tgz",
            };

            return true;
        }

        // still here means that the URL is not really a core package
        directive = null;
        return false;
    }

    /// <summary>Query if 'input' is URL realm ig.</summary>
    /// <param name="input">    The input url.</param>
    /// <param name="segments"> The segments.</param>
    /// <param name="directive">[out] The parsed directive.</param>
    /// <returns>True if URL realm ig, false if not.</returns>
    private bool IsUrlRealmIg(
        string input,
        string[] segments,
        [NotNullWhen(true)] out FhirDirective? directive)
    {
        if (segments.Length < 5)
        {
            directive = null;
            return false;
        }

        // check for a URL that matches a realm URL
        if (!_matchProdIgRealmUrl.IsMatch(input))
        {
            directive = null;
            return false;
        }

        string realm = segments[3];
        string name = segments[4];

        string ballot = string.Empty;
        string possibleVersion = string.Empty;
        string possiblePackage = string.Empty;

        if (segments.Length >= 7)
        {
            if (segments[6].EndsWith(".tgz", StringComparison.OrdinalIgnoreCase))
            {
                possiblePackage = segments[6].Substring(0, segments[6].Length - 4);
            }

            if (_matchBallotLiteral.IsMatch(segments[5]))
            {
                ballot = segments[5];
            }
            else
            {
                possibleVersion = segments[5];
            }
        }
        else if (segments.Length >= 6)
        {
            if (segments[5].EndsWith(".tgz", StringComparison.OrdinalIgnoreCase))
            {
                possiblePackage = segments[5].Substring(0, segments[5].Length - 4);
            }
            else if (!_matchFileUrlSuffix.IsMatch(segments[5]))
            {
                if (_matchBallotLiteral.IsMatch(segments[5]))
                {
                    ballot = segments[5];
                }
                else
                {
                    possibleVersion = segments[5];
                }
            }
        }

        DirectiveNameTypeCodes nameType = DirectiveNameTypeCodes.GuideWithoutSuffix;
        string id;

        if (string.IsNullOrEmpty(possiblePackage))
        {
            id = $"hl7.fhir.{realm}.{name}";
            // default to package.tgz for IGs
            possiblePackage = "package";
        }
        else if (possiblePackage.StartsWith("package"))
        {
            if (possiblePackage.Length > 7)
            {
                id = $"hl7.fhir.{realm}.{name}{possiblePackage.Substring(7)}";
                nameType = DirectiveNameTypeCodes.GuideWithSuffix;
            }
            else
            {
                id = $"hl7.fhir.{realm}.{name}";
            }
        }
        else
        {
            id = possiblePackage;
            // TODO: should check for guide with/without suffix
        }

        // try to get a package manifest to round out our information
        string manifestUrl = string.IsNullOrEmpty(ballot)
            ? $"{_publicationUri}{realm}/{name}/{possiblePackage}.manifest.json"
            : $"{_publicationUri}{realm}/{name}/{ballot}/{possiblePackage}.manifest.json";

        if (TryFetchManifestInfo(manifestUrl, out FhirNpmPackageDetails? npmInfo) &&
            (npmInfo != null))
        {
            // create a directive with this version
            directive = new()
            {
                Directive = $"{npmInfo.Name}#{npmInfo.Version}",
                PackageId = npmInfo.Name,
                NameType = nameType,
                VersionType = DirectiveVersionCodes.Exact,
                PublicationPackageUrl = string.IsNullOrEmpty(ballot)
                    ? $"{_publicationUri}{realm}/{name}/{possiblePackage}.tgz"
                    : $"{_publicationUri}{realm}/{name}/{ballot}/{possiblePackage}.tgz",
            };

            return true;
        }

        // with no version, assume this is latest release
        if (string.IsNullOrEmpty(possibleVersion))
        {
            // create a directive with this version
            directive = new()
            {
                Directive = $"{id}#latest",
                PackageId = id,
                NameType = nameType,
                VersionType = DirectiveVersionCodes.Latest,
                PublicationPackageUrl = string.IsNullOrEmpty(ballot)
                    ? $"{_publicationUri}{realm}/{name}/{possiblePackage}.tgz"
                    : $"{_publicationUri}{realm}/{name}/{ballot}/{possiblePackage}.tgz",
            };

            return true;
        }

        // create a directive with this version
        directive = new()
        {
            Directive = $"{id}#latest",
            PackageId = id,
            NameType = nameType,
            VersionType = DirectiveVersionCodes.Latest,
            PublicationPackageUrl = $"{_publicationUri}{realm}/{name}/{possibleVersion}/{possiblePackage}.tgz",
        };

        return true;
    }

    /// <summary>Query if 'input' is URL non realm ig.</summary>
    /// <param name="input">    The input url.</param>
    /// <param name="segments"> The segments.</param>
    /// <param name="directive">[out] The parsed directive.</param>
    /// <returns>True if URL non realm ig, false if not.</returns>
    private bool IsUrlNonRealmIg(
        string input,
        string[] segments,
        [NotNullWhen(true)] out FhirDirective? directive)
    {
        if (segments.Length < 4)
        {
            directive = null;
            return false;
        }

        // note that we need to check this late - it is a very broad match
        if (!_matchProdIgNoRealmUrl.IsMatch(input))
        {
            directive = null;
            return false;
        }

        string name = segments[3];

        string ballot = string.Empty;
        string possibleVersion = string.Empty;
        string possiblePackage = string.Empty;

        if (segments.Length >= 6)
        {
            if (segments[5].EndsWith(".tgz", StringComparison.OrdinalIgnoreCase))
            {
                possiblePackage = segments[5].Substring(0, segments[5].Length - 4);
            }

            if (_matchBallotLiteral.IsMatch(segments[4]))
            {
                ballot = segments[4];
            }
            else
            {
                possibleVersion = segments[4];
            }
        }
        else if (segments.Length >= 5)
        {
            if (segments[4].EndsWith(".tgz", StringComparison.OrdinalIgnoreCase))
            {
                possiblePackage = segments[4].Substring(0, segments[4].Length - 4);
            }
            else if (!_matchFileUrlSuffix.IsMatch(segments[4]))
            {
                if (_matchBallotLiteral.IsMatch(segments[4]))
                {
                    ballot = segments[4];
                }
                else
                {
                    possibleVersion = segments[4];
                }
            }
        }

        DirectiveNameTypeCodes nameType = DirectiveNameTypeCodes.GuideWithoutSuffix;
        string id;

        if (string.IsNullOrEmpty(possiblePackage))
        {
            // packages get the universal realm if they have no realm
            id = $"hl7.fhir.uv.{name}";
            // default to package.tgz for IGs
            possiblePackage = "package";
        }
        else if (possiblePackage.StartsWith("package"))
        {
            if (possiblePackage.Length > 7)
            {
                // packages get the universal realm if they have no realm
                id = $"hl7.fhir.uv.{name}{possiblePackage.Substring(7)}";
                nameType = DirectiveNameTypeCodes.GuideWithSuffix;
            }
            else
            {
                // packages get the universal realm if they have no realm
                id = $"hl7.fhir.uv.{name}";
            }
        }
        else
        {
            id = possiblePackage;
            // TODO: should check for guide with/without suffix
        }

        // try to get a package manifest to round out our information
        string manifestUrl = string.IsNullOrEmpty(ballot)
            ? $"{_publicationUri}{name}/{possiblePackage}.manifest.json"
            : $"{_publicationUri}{name}/{ballot}/{possiblePackage}.manifest.json";

        if (TryFetchManifestInfo(manifestUrl, out FhirNpmPackageDetails? npmInfo) &&
            (npmInfo != null))
        {
            // create a directive with this version
            directive = new()
            {
                Directive = $"{npmInfo.Name}#{npmInfo.Version}",
                PackageId = npmInfo.Name,
                NameType = nameType,
                VersionType = DirectiveVersionCodes.Exact,
                PublicationPackageUrl = string.IsNullOrEmpty(ballot)
                    ? $"{_publicationUri}{name}/{possiblePackage}.tgz"
                    : $"{_publicationUri}{name}/{ballot}/{possiblePackage}.tgz",
            };

            return true;
        }

        // with no version, assume this is latest release
        if (string.IsNullOrEmpty(possibleVersion))
        {
            // TODO: should check for guide with/without suffix
            // create a directive with this version
            directive = new()
            {
                Directive = $"{id}#latest",
                PackageId = id,
                NameType = DirectiveNameTypeCodes.GuideWithoutSuffix,
                VersionType = DirectiveVersionCodes.Latest,
                PublicationPackageUrl = string.IsNullOrEmpty(ballot)
                    ? $"{_publicationUri}{name}/{possiblePackage}.tgz"
                    : $"{_publicationUri}{name}/{ballot}/{possiblePackage}.tgz",
            };

            return true;
        }

        // TODO: should check for guide with/without suffix
        // create a directive with this version
        directive = new()
        {
            Directive = $"{id}#latest",
            PackageId = id,
            NameType = DirectiveNameTypeCodes.GuideWithoutSuffix,
            VersionType = DirectiveVersionCodes.Latest,
            PublicationPackageUrl = $"{_publicationUri}{name}/{possibleVersion}/{possiblePackage}.tgz",
        };

        return true;
    }

    /// <summary>Query if 'input' is URL ci core current.</summary>
    /// <param name="input">    The input url.</param>
    /// <param name="segments"> The segments.</param>
    /// <param name="directive">[out] The parsed directive.</param>
    /// <returns>True if URL ci core current, false if not.</returns>
    private static bool IsUrlCiCoreCurrent(
        string input,
        string[] segments,
        [NotNullWhen(true)] out FhirDirective? directive)
    {
        if (!_matchCiCoreCurrentUrl.IsMatch(input))
        {
            directive = null;
            return false;
        }

        // default to highest known release
        int highest = Enum.GetValues(typeof(FhirSequenceCodes)).Cast<int>().Max();

        string package;

        if ((segments.Length > 2) &&
            segments[2].EndsWith(".tgz", StringComparison.OrdinalIgnoreCase) &&
            !_matchPackageLiteral.IsMatch(segments[2]))
        {
            package = segments[2].Substring(0, segments[2].Length - 4);
        }
        else
        {
            package = $"hl7.fhir.r{highest}.core";
        }

        // create a directive with this version
        directive = new()
        {
            Directive = $"{package}#current",
            PackageId = package,
            NameType = DirectiveNameTypeCodes.CoreFull,
            VersionType = DirectiveVersionCodes.ContinuousIntegration,
            CiOrg = "hl7",
            CiUrl = $"{_ciUri}{package}.tgz",
        };

        return true;
    }

    /// <summary>Query if 'input' is URL ci core branch.</summary>
    /// <param name="input">    The input url.</param>
    /// <param name="segments"> The segments.</param>
    /// <param name="directive">[out] The parsed directive.</param>
    /// <returns>True if URL ci core branch, false if not.</returns>
    private static bool IsUrlCiCoreBranch(
        string input,
        string[] segments,
        [NotNullWhen(true)] out FhirDirective? directive)
    {
        if (segments.Length < 4)
        {
            directive = null;
            return false;
        }

        if (!_matchCiCoreBranchUrl.IsMatch(input))
        {
            directive = null;
            return false;
        }

        // default to highest known release
        int highest = Enum.GetValues(typeof(FhirSequenceCodes)).Cast<int>().Max();

        string branch = segments[3];
        string package;

        if ((segments.Length > 3) &&
            segments[3].EndsWith(".tgz", StringComparison.OrdinalIgnoreCase) &&
            !_matchPackageLiteral.IsMatch(segments[3]))
        {
            package = segments[3].Substring(0, segments[3].Length - 4);
        }
        else
        {
            package = $"hl7.fhir.r{highest}.core";
        }

        // create a directive with this version
        directive = new()
        {
            Directive = $"{package}#current${branch}",
            PackageId = package,
            NameType = DirectiveNameTypeCodes.CoreFull,
            VersionType = DirectiveVersionCodes.ContinuousIntegration,
            CiBranch = branch,
            CiUrl = $"{_ciUri}branches/{branch}/{package}.tgz",
        };

        return true;
    }

    /// <summary>Query if 'input' is URL ci ig branch.</summary>
    /// <param name="input">    The input url.</param>
    /// <param name="segments"> The segments.</param>
    /// <param name="directive">[out] The parsed directive.</param>
    /// <returns>True if URL ci ig branch, false if not.</returns>
    private bool IsUrlCiIgBranch(
        string input,
        string[] segments,
        [NotNullWhen(true)] out FhirDirective? directive)
    {
        if (segments.Length < 6)
        {
            directive = null;
            return false;
        }

        if (!_matchCiIgBranchUrl.IsMatch(input))
        {
            directive = null;
            return false;
        }

        // extract known segment values
        string org = segments[3];
        string repo = segments[4];
        string branch = segments[6];

        string package;

        if ((segments.Length > 7) &&
            segments[7].EndsWith(".tgz", StringComparison.OrdinalIgnoreCase))
        {
            package = segments[7].Substring(0, segments[7].Length - 4);
        }
        else
        {
            // default to non-version-specific package
            package = "package";
        }

        // cannot determine package from the URL, need to try and fetch the package.manifest.json
        string manifestUrl = $"{_ciUri}ig/{org}/{repo}/branches/{branch}/{package}.manifest.json";

        if (!TryFetchManifestInfo(manifestUrl, out FhirNpmPackageDetails? npmInfo) ||
            (npmInfo == null))
        {
            directive = null;
            return false;
        }

        // create a directive with this version
        directive = new()
        {
            Directive = $"{npmInfo.Name}#current${branch}",
            PackageId = npmInfo.Name,
            NameType = package.Length == 7 ? DirectiveNameTypeCodes.GuideWithoutSuffix : DirectiveNameTypeCodes.GuideWithSuffix,
            VersionType = DirectiveVersionCodes.ContinuousIntegration,
            CiOrg = org,
            CiBranch = branch,
            CiUrl = $"{_ciUri}ig/{org}/{repo}/branches/{branch}/{package}.tgz",
        };

        return true;
    }

    /// <summary>Query if 'input' is URL ci ig.</summary>
    /// <param name="input">    The input url.</param>
    /// <param name="segments"> The segments.</param>
    /// <param name="directive">[out] The parsed directive.</param>
    /// <returns>True if URL ci ig, false if not.</returns>
    private bool IsUrlCiIg(
        string input,
        string[] segments,
        [NotNullWhen(true)] out FhirDirective? directive)
    {
        if (segments.Length < 5)
        {
            directive = null;
            return false;
        }

        if (!_matchCiIgUrl.IsMatch(input))
        {
            directive = null;
            return false;
        }

        // extract known segment values
        string org = segments[3];
        string repo = segments[4];

        string package;

        if ((segments.Length > 5) &&
            segments[5].EndsWith(".tgz", StringComparison.OrdinalIgnoreCase))
        {
            package = segments[5].Substring(0, segments[5].Length - 4);
        }
        else
        {
            // default to non-version-specific package
            package = "package";
        }

        // cannot determine package from the URL, need to try and fetch the package.manifest.json
        string manifestUrl = $"{_ciUri}ig/{org}/{repo}/{package}.manifest.json";

        if (!TryFetchManifestInfo(manifestUrl, out FhirNpmPackageDetails? npmInfo) ||
            (npmInfo == null))
        {
            directive = null;
            return false;
        }

        // create a directive with this version
        directive = new()
        {
            Directive = $"{npmInfo.Name}#current",
            PackageId = npmInfo.Name,
            NameType = package.Length == 7 ? DirectiveNameTypeCodes.GuideWithoutSuffix : DirectiveNameTypeCodes.GuideWithSuffix,
            VersionType = DirectiveVersionCodes.ContinuousIntegration,
            CiOrg = org,
            CiUrl = $"{_ciUri}ig/{org}/{repo}/{package}.tgz",
        };

        return true;
    }

    /// <summary>Attempts to parse URL into a package directive.</summary>
    /// <param name="input">    The input url.</param>
    /// <param name="directive">[out] The parsed directive.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    internal bool TryParseUrl(
        string input,
        [NotNullWhen(true)] out FhirDirective? directive)
    {
        char[] splits = ['/', '?'];
        string[] segments = input.Split(splits, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        if (segments.Length < 1)
        {
            directive = null;
            return false;
        }

        if (IsUrlProdCore(input, segments, out directive))
        {
            return true;
        }

        if (IsUrlRealmIg(input, segments, out directive))
        {
            return true;
        }

        // this test will match *most* production URLs, so it should be last of the production url tests
        if (IsUrlNonRealmIg(input, segments, out directive))
        {
            return true;
        }

        if (IsUrlCiCoreCurrent(input, segments, out directive))
        {
            return true;
        }

        if (IsUrlCiCoreBranch(input, segments, out directive))
        {
            return true;
        }

        if (IsUrlCiIgBranch(input, segments, out directive))
        {
            return true;
        }

        if (IsUrlCiIg(input, segments, out directive))
        {
            return true;
        }

        // if we are still here, try to fetch a manifest file
        string manifestUrl;
        if (_matchFileUrlSuffix.IsMatch(input))
        {
            if (input.EndsWith(".tgz", StringComparison.OrdinalIgnoreCase))
            {
                manifestUrl = string.Concat(input.AsSpan(0, input.Length - 4), ".manifest.json");
            }
            else
            {
                int loc = input.LastIndexOf('/');
                manifestUrl = string.Concat(input.AsSpan(0, loc), "/package.manifest.json");
            }
        }
        else
        {
            if (input.EndsWith('/'))
            {
                manifestUrl = $"{input}package.manifest.json";
            }
            else
            {
                manifestUrl = $"{input}/package.manifest.json";
            }
        }

        // if we cannot get a manifest, we are sunk
        if (!TryFetchManifestInfo(manifestUrl, out FhirNpmPackageDetails? npmInfo) ||
            (npmInfo == null))
        {
            directive = null;
            return false;
        }

        // create a directive with this info
        directive = new()
        {
            Directive = $"{npmInfo.Name}#{npmInfo.Version}",
            PackageId = npmInfo.Name,
            NameType = DirectiveNameTypeCodes.GuideWithoutSuffix,
            VersionType = DirectiveVersionCodes.Unknown,
        };

        return true;
    }

    /// <summary>
    /// Attempts to fetch manifest information the FhirNpmPackageDetails from the given string.
    /// </summary>
    /// <param name="url">    URL of the resource.</param>
    /// <param name="details">[out] The details.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    private bool TryFetchManifestInfo(
        string url,
        out FhirNpmPackageDetails? details)
    {
        try
        {
            using HttpResponseMessage response = _httpClient.GetAsync(url).Result;

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogInformation($"TryFetchManifestInfo <<< get {url} returned {response.StatusCode}");
                details = null;
                return false;
            }

            string contents = response.Content.ReadAsStringAsync().Result;

            details = FhirNpmPackageDetails.Parse(contents);

            if (details == null)
            {
                // not found just means the build does not exist
                _logger.LogInformation($"TryFetchManifestInfo <<< failed to parse package.manifest.json at {url}");
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(WarningMsgDefault, nameof(TryFetchManifestInfo), ex.Message, ex.InnerException?.Message ?? string.Empty);
            details = null;
            return false;
        }
    }

    /// <summary>
    /// Attempts to fetch version information a BuildVersionInfo from the given string.
    /// </summary>
    /// <param name="url"> URL of the resource.</param>
    /// <param name="info">[out] The information.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    private bool TryFetchVersionInfo(
        string url,
        out BuildVersionInfo? info)
    {
        try
        {
            using HttpResponseMessage response = _httpClient.GetAsync(url).Result;

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogInformation($"TryFetchVersionInfo <<< get {url} returned {response.StatusCode}");
                info = null;
                return false;
            }

            string contents = response.Content.ReadAsStringAsync().Result;

            if ((!TryParseVersionInfo(contents, out info)) ||
                (info == null))
            {
                // not found just means the build does not exist
                _logger.LogInformation($"TryFetchVersionInfo <<< failed to parse version.info at {url}");
                info = null;
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(WarningMsgDefault, nameof(TryFetchVersionInfo), ex.Message, ex.InnerException?.Message ?? string.Empty);
            info = null;
            return false;
        }
    }

    /// <summary>
    /// Attempts to parse hl 7 product URL a FhirDirective from the given string[].
    /// </summary>
    /// <exception cref="Exception">Thrown when an exception error condition occurs.</exception>
    /// <param name="segments"> The segments.</param>
    /// <param name="directive">[out] The parsed directive.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    private bool TryParseHl7ProdUrl(
        string[] segments,
        out FhirDirective? directive)
    {
        // URLs need to have at least 3 segments
        if ((segments.Length < 3) ||
            (!segments[1].Equals("fhir", StringComparison.Ordinal)))
        {
            directive = null;
            return false;
        }

        string packageName = string.Empty;
        string versionInfoUrl = string.Empty;
        DirectiveNameTypeCodes nameType = DirectiveNameTypeCodes.Unknown;

        // core ballot packages: hl7.org/fhir/[YYYYMMM]/[package|url]
        // check if the third segment is a ballot marker, a four-year number (YYYY) and a three-letter month abbreviation (MMM)
        if ((segments[2].Length == 7) &&
            int.TryParse(segments[2].AsSpan(0, 4), out _) &&
            DateTimeFormatInfo.CurrentInfo.AbbreviatedMonthNames.Contains(segments[2].Substring(4)))
        {
            // try and pull a version.info file to determine what this URL contains
            versionInfoUrl = $"https://hl7.org/fhir/{segments[2]}/version.info";
            nameType = DirectiveNameTypeCodes.CoreFull;

            // check to see if the package name is in the fourth segment
            if ((segments.Length >= 4) &&
                segments[3].EndsWith(".tgz", StringComparison.OrdinalIgnoreCase))
            {
                packageName = segments[3];
            }
        }
        // fhir major release packages: hl7.org/fhir/[R#]/[package|url]
        // check if the third segment is an R-literal, the letter 'R' followed by a number
        else if ((segments[2].Length > 1) &&
            segments[2].StartsWith('R') &&
            int.TryParse(segments[2].AsSpan(1), out _))
        {
            FhirSequenceCodes sequence = (FhirSequenceCodes)FhirReleases.FhirVersionToSequence(segments[2]);

            if (sequence == FhirSequenceCodes.Unknown)
            {
                _logger.LogInformation($"TryParseHl7ProdUrl <<< found unknown FHIR major release version: {segments[2]}");
                directive = null;
                return false;
            }

            // try and pull a version.info file to determine what this URL contains
            versionInfoUrl = $"https://hl7.org/fhir/{segments[2]}/version.info";
            nameType = DirectiveNameTypeCodes.CoreFull;

            // check to see if the package name is in the fourth segment
            if ((segments.Length >= 4) &&
                segments[3].EndsWith(".tgz", StringComparison.OrdinalIgnoreCase))
            {
                packageName = segments[3];
            }
        }
        // ig packages: hl7.org/fhir/[realm]/[package name]/[ballot?]
        else if (segments[2].Length == 2)
        {
            throw new Exception("IGs do not get version.info!");
#if CAKE
            string potentialPackageName;

            // check to see if we have a fifth segment that is a ballot marker
            if ((segments.Length >= 5) &&
                (segments[4].Length == 7) &&
                int.TryParse(segments[4].Substring(0, 4), out _) &&
                DateTimeFormatInfo.CurrentInfo.AbbreviatedMonthNames.Contains(segments[4].Substring(4)))
            {
                // try and pull a version.info file to determine what this URL contains
                versionInfoUrl = $"https://hl7.org/fhir/{segments[2]}/{segments[3]}/{segments[4]}/version.info";
                potentialPackageName = segments.Length >= 6 ? segments[5] : string.Empty;
            }
            else
            {
                // try and pull a version.info file to determine what this URL contains
                versionInfoUrl = $"https://hl7.org/fhir/{segments[2]}/{segments[3]}/version.info";
                potentialPackageName = segments.Length >= 5 ? segments[4] : string.Empty;
            }

            if (string.IsNullOrEmpty(potentialPackageName))
            {
                nameType = DirectiveNameTypeCodes.GuideWithoutSuffix;
                packageName = $"hl7.fhir.{segments[2]}.{segments[3]}";
            }
            else if (potentialPackageName.EndsWith(".tgz", StringComparison.OrdinalIgnoreCase))
            {
                string[] pnSegments = potentialPackageName.Split('.');

                int len = pnSegments.Length;
                int versionSegment = len - 1;

                if ((pnSegments.Length > 1) &&
                    (pnSegments[versionSegment].Length > 1) &&
                    pnSegments[versionSegment].StartsWith("R", StringComparison.OrdinalIgnoreCase) &&
                    int.TryParse(pnSegments[versionSegment].Substring(1), out _))
                {
                    packageName = potentialPackageName;
                    nameType = DirectiveNameTypeCodes.GuideWithSuffix;
                }
                else
                {
                    packageName = potentialPackageName;
                    nameType = DirectiveNameTypeCodes.GuideWithoutSuffix;
                }
            }
            else
            {
                // assume it is a guide without a suffix
                nameType = DirectiveNameTypeCodes.GuideWithoutSuffix;
                packageName = $"hl7.fhir.{segments[2]}.{segments[3]}";
            }
#endif
        }

        // check for not determining the URL
        if (string.IsNullOrEmpty(versionInfoUrl))
        {
            _logger.LogInformation($"TryParseHl7ProdUrl <<< could not resolve download type!");
            directive = null;
            return false;
        }

        try
        {
            if ((!TryFetchVersionInfo(versionInfoUrl, out BuildVersionInfo? versionInfo)) ||
                (versionInfo == null))
            {
                _logger.LogInformation($"TryParseHl7ProdUrl <<< failed to parse version.info at {versionInfoUrl}");
                directive = null;
                return false;
            }

            if (string.IsNullOrEmpty(packageName))
            {
                // determine a package name from the version
                packageName = $"hl7.fhir.{FhirReleases.FhirVersionToRLiteral(versionInfo.FhirVersion).ToLowerInvariant()}.core";
            }
            else if (packageName.EndsWith(".tgz", StringComparison.OrdinalIgnoreCase))
            {
                // remove the .tgz suffix
                packageName = packageName.Substring(0, packageName.Length - 4);
            }

            directive = new()
            {
                Directive = $"{packageName}#{versionInfo.Version}",
                PackageId = packageName,
                NameType = nameType,
                FhirRelease = FhirReleases.FhirVersionToRLiteral(versionInfo.FhirVersion),
                PackageVersion = versionInfo.Version,
                VersionType = DirectiveVersionCodes.Exact,
                PublicationPackageUrl = $"http://hl7.org/fhir/{segments[2]}/{packageName}.tgz",
                BuildDate = versionInfo.BuildDate,
            };

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(WarningMsgDefault, nameof(TryParseHl7ProdUrl), ex.Message, ex.InnerException?.Message ?? string.Empty);
            directive = null;
            return false;
        }
    }

    /// <summary>Attempts to parse directive a ParsedDirective from the given string.</summary>
    /// <param name="input">The input directive: [package-name]#[version].</param>
    /// <param name="directive">   [out] The parsed directive.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    internal static bool TryParseDirective(
        string input,
        out FhirDirective? directive)
    {
        if (string.IsNullOrEmpty(input))
        {
            directive = null;
            return false;
        }

        string[] components = input.Split('#', StringSplitOptions.TrimEntries);

        FhirDirective current;

        switch (components.Length)
        {
            case 1:
                current = new()
                {
                    Directive = input,
                    PackageId = components[0],
                    NameType = DirectiveNameTypeCodes.Unknown,
                    FhirRelease = string.Empty,
                    PackageVersion = "latest",
                    VersionType = DirectiveVersionCodes.Latest,
                };
                break;

            case 2:
                current = new()
                {
                    Directive = input,
                    PackageId = components[0],
                    NameType = DirectiveNameTypeCodes.Unknown,
                    FhirRelease = string.Empty,
                    PackageVersion = components[1],
                    VersionType = DirectiveVersionCodes.Unknown,
                };
                break;

            default:
                {
                    directive = null;
                    return false;
                }
        }

        // determine type of package name
        if (FhirPackageUtils.PackageIsFhirRelease(current.PackageId))
        {
            current = current with
            {
                NameType = DirectiveNameTypeCodes.CoreFull,
                FhirRelease = current.PackageId.Split('.')[2],
            };
        }
        else if (_matchCorePackageRoot.IsMatch(current.PackageId))
        {
            current = current with
            {
                NameType = DirectiveNameTypeCodes.CorePartial,
                FhirRelease = current.PackageId.Split('.')[2],
            };
        }
        else if (_matchFhirSuffix.IsMatch(current.PackageId))
        {
            current = current with
            {
                NameType = DirectiveNameTypeCodes.GuideWithSuffix,
                FhirRelease = current.PackageId.Substring(current.PackageId.LastIndexOf('.') + 1),
            };
        }
        else
        {
            current = current with { NameType = DirectiveNameTypeCodes.GuideWithoutSuffix };
        }

        // determine type of version
        switch (current.PackageVersion.ToLowerInvariant())
        {
            case "latest":
                current = current with { VersionType = DirectiveVersionCodes.Latest };
                break;

            case "dev":
                current = current with { VersionType = DirectiveVersionCodes.Local };
                break;

            case "current":
                current = current with { VersionType = DirectiveVersionCodes.ContinuousIntegration };
                break;

            default:
                {
                    if (current.PackageVersion.StartsWith("current$", StringComparison.OrdinalIgnoreCase))
                    {
                        current = current with
                        {
                            VersionType = DirectiveVersionCodes.ContinuousIntegration,
                            CiBranch = current.PackageVersion.Substring(8),
                        };
                    }
                    else if (current.PackageVersion.EndsWith(".x", StringComparison.OrdinalIgnoreCase))
                    {
                        current = current with
                        {
                            VersionType = DirectiveVersionCodes.Partial,
                        };
                    }
                    else
                    {
                        // check for special case - partial SemVer on an HL7 package
                        int segments = current.PackageVersion.Split('.').Length;

                        if (current.PackageId.StartsWith("hl7", StringComparison.OrdinalIgnoreCase))
                        {
                            if (segments == 3)
                            {
                                if (current.PackageVersion.EndsWith(".x", StringComparison.OrdinalIgnoreCase))
                                {
                                    current = current with { VersionType = DirectiveVersionCodes.Partial, };
                                }
                                else if (_matchSemver.IsMatch(current.PackageVersion))
                                {
                                    current = current with { VersionType = DirectiveVersionCodes.Exact, };
                                }
                                else
                                {
                                    directive = null;
                                    return false;
                                }
                            }
                            else if (segments == 2)
                            {
                                if (_matchSemver.IsMatch(current.PackageVersion + ".0"))
                                {
                                    current = current with { VersionType = DirectiveVersionCodes.Partial, };
                                }
                                else
                                {
                                    directive = null;
                                    return false;
                                }
                            }
                            else
                            {
                                directive = null;
                                return false;
                            }
                        }
                        else if (current.PackageVersion.EndsWith(".x", StringComparison.OrdinalIgnoreCase))
                        {
                            // check for a semver if we swap out the last segment
                            if (_matchSemver.IsMatch(string.Concat(current.PackageVersion.AsSpan(0, current.PackageVersion.Length -2), "0")))
                            {
                                current = current with { VersionType = DirectiveVersionCodes.Partial, };
                            }
                            else
                            {
                                // assume it is a non-semver exact
                                current = current with { VersionType = DirectiveVersionCodes.NonSemVer, };
                            }
                        }
                        else if (_matchSemver.IsMatch(current.PackageVersion))
                        {
                            current = current with { VersionType = DirectiveVersionCodes.Exact, };
                        }
                        else
                        {
                            current = current with { VersionType = DirectiveVersionCodes.NonSemVer, };
                        }
                    }
                }
                break;
        }

        // all HL7 packages use SemVer
        if (current.PackageId.StartsWith("hl7", StringComparison.OrdinalIgnoreCase) &&
            (current.VersionType == DirectiveVersionCodes.NonSemVer))
        {
            directive = null;
            return false;
        }

        // check for HL7 packages with specific versions to add a publication URL
        if (current.PackageId.StartsWith("hl7.", StringComparison.OrdinalIgnoreCase) &&
            (current.VersionType == DirectiveVersionCodes.Exact))
        {
            // fill in a publication URL
            switch (current.NameType)
            {
                case DirectiveNameTypeCodes.CoreFull:
                case DirectiveNameTypeCodes.CorePartial:
                    {
                        string packageId = current.NameType == DirectiveNameTypeCodes.CorePartial
                            ? current.PackageId + ".core"
                            : current.PackageId;

                        // if we know about a published version, try to construct the download URL
                        if (FhirPublishedVersions.TryGetValue(current.PackageVersion, out PublishedReleaseInformation pi))
                        {
                            if (!string.IsNullOrEmpty(pi.BallotPrefix))
                            {
                                // ballot versions use their ballot prefix
                                current = current with
                                {
                                    PublicationPackageUrl = $"{_publicationUri}{pi.BallotPrefix}/{packageId}.tgz",
                                };
                            }
                            else if (current.PackageVersion.Equals(FhirVersionToLongVersion(current.PackageVersion)))
                            {
                                // current major releases use their R-Version
                                current = current with
                                {
                                    PublicationPackageUrl = $"{_publicationUri}{FhirVersionToRLiteral(current.PackageVersion)}/{packageId}.tgz",
                                };
                            }
                            else
                            {
                                // remaining versions use their version literal as the URL prefix
                                current = current with
                                {
                                    PublicationPackageUrl = $"{_publicationUri}{current.PackageVersion}/{packageId}.tgz",
                                };
                            }
                        }
                    }
                    break;
                case DirectiveNameTypeCodes.GuideWithSuffix:
                    {
                        string[] segments = current.PackageId.Split('.');

                        if (segments.Length >= 5)
                        {
                            // we are only attempting fallbacks on published versions of HL7 packages
                            current = current with
                            {
                                PublicationPackageUrl = $"{_publicationUri}{segments[2]}/{segments[3]}/package.{segments[4]}.tgz",
                            };
                        }
                    }
                    break;

                case DirectiveNameTypeCodes.GuideWithoutSuffix:
                    {
                        string[] segments = current.PackageId.Split('.');

                        if (segments.Length >= 4)
                        {
                            // we are only attempting fallbacks on published versions of HL7 packages
                            current = current with
                            {
                                PublicationPackageUrl = $"{_publicationUri}{segments[2]}/{segments[3]}/package.tgz",
                            };
                        }
                    }
                    break;
                case DirectiveNameTypeCodes.Unknown:
                default:
                    break;
            }
        }

        directive = current;
        return true;
    }

    /// <summary>Attempts to resolve ci.</summary>
    /// <param name="directive">[out] The parsed directive.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    internal bool TryResolveCi(ref FhirDirective directive)
    {
        if (string.IsNullOrEmpty(directive.PackageId))
        {
            return false;
        }

        switch (directive.NameType)
        {
            case DirectiveNameTypeCodes.CoreFull:
            case DirectiveNameTypeCodes.CorePartial:
                {
                    return TryResolveCiCore(ref directive);
                }

            case DirectiveNameTypeCodes.Unknown:
            case DirectiveNameTypeCodes.GuideWithSuffix:
            case DirectiveNameTypeCodes.GuideWithoutSuffix:
            default:
                {
                    return TryResolveCiIg(ref directive);
                }
        }
    }

    /// <summary>
    /// Attempts to parse version information a BuildVersionInfo from the given string.
    /// </summary>
    /// <param name="contents">   The contents.</param>
    /// <param name="versionInfo">[out] Information describing the version.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    private bool TryParseVersionInfo(string contents, out BuildVersionInfo? versionInfo)
    {
        IniDataParser parser = new();

        IniData data = parser.Parse(contents);

        if (!data.Sections.Contains("FHIR"))
        {
            // not a valid versions.info file
            _logger.LogError($"TryParseVersionInfo <<< does not contain valid contents (missing [FHIR])!");
            versionInfo = null;
            return false;
        }

        versionInfo = new()
        {
            FhirVersion = data["FHIR"].Contains("FhirVersion") ? data["FHIR"]["FhirVersion"] : string.Empty,
            Version = data["FHIR"].Contains("version") ? data["FHIR"]["version"] : string.Empty,
            BuildId = data["FHIR"].Contains("buildId") ? data["FHIR"]["buildId"] : string.Empty,
            BuildDate = data["FHIR"].Contains("date") ? data["FHIR"]["date"] : string.Empty,
        };

        return true;
    }

    /// <summary>Attempts to resolve a CI build of a core package.</summary>
    /// <param name="directive">[out] The parsed directive.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    internal bool TryResolveCiCore(ref FhirDirective directive)
    {
        try
        {
            string packageId = directive.NameType == DirectiveNameTypeCodes.CorePartial
                ? directive.PackageId + ".core"
                : directive.PackageId;
            string ciBranch = directive.CiBranch;

            // for CI builds of core packages, all we can do is test the URL - check for a version file
            string url = string.IsNullOrEmpty(ciBranch)
                ? $"{_ciUri}version.info"
                : $"{_ciUri}branches/{ciBranch}/version.info";

            string contents = _httpClient.GetStringAsync(url).Result;

            if ((!TryParseVersionInfo(contents, out BuildVersionInfo? ciVersion)) ||
                (ciVersion == null))
            {
                // not found just means the build does not exist
                _logger.LogWarning("{Function} <<< failed to parse version.info for directive {Directive}", nameof(TryResolveCi), directive.Directive);
                return false;
            }

            // ensure the FHIR version is at least a partial match
            if (ciVersion.FhirVersion.StartsWith(FhirVersionToShortVersion(directive.FhirRelease), StringComparison.OrdinalIgnoreCase))
            {
                string resolvedCiUrl = string.IsNullOrEmpty(ciBranch)
                    ? $"{_ciUri}"
                    : $"{_ciUri}branches/{ciBranch}/";

                directive = directive with
                {
                    //FhirRelease = ciVersion.FhirVersion,
                    PackageId = packageId,
                    NameType = (directive.NameType == DirectiveNameTypeCodes.CorePartial) ? DirectiveNameTypeCodes.CoreFull : directive.NameType,
                    PackageVersion = string.IsNullOrEmpty(ciBranch) ? "current" : "current$" + ciBranch,
                    CiUrl = resolvedCiUrl,
                    CiOrg = string.Empty,
                    CiBranch = ciBranch,
                    BuildDate = ciVersion.BuildDate,
                    ResolvedTarballUrl = $"{resolvedCiUrl}{packageId}.tgz",
                    PublicationPackageUrl = string.Empty,       // ci builds already use the website URL
                    ResolvedSha = string.Empty,
                };

                return true;
            }
        }
        catch (AggregateException s)
        {
            if (s.InnerException is HttpRequestException h)
            {
                if (h.StatusCode == HttpStatusCode.NotFound)
                {
                    // not found just means the build does not exist
                    _logger.LogWarning("{Function} <<< ci build of {PackageId}${CiBranch} does not exist", nameof(TryResolveCi), directive.PackageId, directive.CiBranch);
                    return false;
                }
            }

            _logger.LogWarning(WarningMsgDefault, nameof(TryResolveCi), s.Message, s.InnerException?.Message ?? string.Empty);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(WarningMsgDefault, nameof(TryResolveCi), ex.Message, ex.InnerException?.Message ?? string.Empty);
        }

        // still here means nothing was found successfully
        return false;
    }

    /// <summary>Attempts to resolve a CI build of an IG package.</summary>
    /// <param name="directive">[out] The parsed directive.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    internal bool TryResolveCiIg(ref FhirDirective directive)
    {
        try
        {
            string packageId = directive.PackageId;
            string ciBranch = directive.CiBranch;

            string contents = _httpClient.GetStringAsync(_qasUri).Result;

            IEnumerable<FhirQasRec> igs = JsonSerializer.Deserialize<List<FhirQasRec>>(contents) ?? [];

            // find records that match the package name and branch
            // note that we are just using 'main' and 'master' as default branches - not fully correct, but default branches are not identified
            IEnumerable<FhirQasRec> matching = string.IsNullOrEmpty(ciBranch)
                ? igs.Where(x => x.PackageId.Equals(packageId, StringComparison.OrdinalIgnoreCase) &&
                    (x.RepositoryUrl.EndsWith("/main/qa.json", StringComparison.OrdinalIgnoreCase) ||
                     x.RepositoryUrl.EndsWith("/master/qa.json", StringComparison.OrdinalIgnoreCase)))
                : igs.Where(x => x.PackageId.Equals(packageId, StringComparison.OrdinalIgnoreCase) &&
                    x.RepositoryUrl.EndsWith($"/{ciBranch}/qa.json", StringComparison.OrdinalIgnoreCase));

            if (!matching.Any())
            {
                // check for names we need to mangle
                switch (directive.NameType)
                {
                    case DirectiveNameTypeCodes.GuideWithSuffix:
                        {
                            // build shortened version of name
                            packageId = packageId.Substring(0, packageId.LastIndexOf('.'));

                            matching = string.IsNullOrEmpty(ciBranch)
                                ? igs.Where(x => x.PackageId.Equals(packageId, StringComparison.OrdinalIgnoreCase) &&
                                    (x.RepositoryUrl.EndsWith("/main/qa.json", StringComparison.OrdinalIgnoreCase) ||
                                     x.RepositoryUrl.EndsWith("/master/qa.json", StringComparison.OrdinalIgnoreCase)))
                                : igs.Where(x => x.PackageId.Equals(packageId, StringComparison.OrdinalIgnoreCase) &&
                                    x.RepositoryUrl.EndsWith($"/{ciBranch}/qa.json", StringComparison.OrdinalIgnoreCase));
                        }
                        break;
                    case DirectiveNameTypeCodes.GuideWithoutSuffix:
                        {
                            // build lengthened version of name
                            packageId = packageId + "." + FhirVersionToRLiteral(directive.FhirRelease);

                            matching = string.IsNullOrEmpty(ciBranch)
                                ? igs.Where(x => x.PackageId.Equals(packageId, StringComparison.OrdinalIgnoreCase) &&
                                    (x.RepositoryUrl.EndsWith("/main/qa.json", StringComparison.OrdinalIgnoreCase) ||
                                     x.RepositoryUrl.EndsWith("/master/qa.json", StringComparison.OrdinalIgnoreCase)))
                                : igs.Where(x => x.PackageId.Equals(packageId, StringComparison.OrdinalIgnoreCase) &&
                                    x.RepositoryUrl.EndsWith($"/{ciBranch}/qa.json", StringComparison.OrdinalIgnoreCase));
                        }
                        break;

                    default:
                        // ignore other types
                        break;
                }

                if (!matching.Any())
                {
                    return false;
                }
            }

            // filter by FHIR version, if specified
            if (!string.IsNullOrEmpty(directive.FhirRelease))
            {
                string fhirRelease = directive.FhirRelease;

                IEnumerable<FhirQasRec> filtered = matching.Where(x => x.FhirVersion.Equals(fhirRelease, StringComparison.OrdinalIgnoreCase));

                // if there is at least one filtered version, go with that
                if (!filtered.Any())
                {
                    // if filtered reduces to zero, we need to check for 'hidden' FHIR-version-specific packages
                    string rVersion = FhirVersionToRLiteral(directive.FhirRelease);

                    // sort our matching record
                    matching = matching.OrderByDescending(q => q.BuildDate);

                    foreach (FhirQasRec rec in matching)
                    {
                        FhirDirective d = DirectiveForQasRec(directive, rec);

                        if (d.ResolvedTarballUrl.EndsWith(rVersion + ".tgz", StringComparison.OrdinalIgnoreCase))
                        {
                            directive = d;
                            return true;
                        }

                        string url = d.ResolvedTarballUrl.Replace(".tgz", $".{rVersion.ToLowerInvariant()}.tgz", StringComparison.OrdinalIgnoreCase);

                        try
                        {
                            HttpRequestMessage req = new()
                            {
                                Method = HttpMethod.Head,
                                RequestUri = new Uri(url),
                            };

                            HttpResponseMessage response = _httpClient.SendAsync(req).Result;

                            if (!response.IsSuccessStatusCode)
                            {
                                // go to next loop
                                continue;
                            }

                            directive = d;
                            return true;
                        }
                        catch (Exception)
                        {
                            // just go to the next loop
                            continue;
                        }
                    }

                    // still here means we have a FHIR version that cannot be resolved
                    return false;
                }

                matching = filtered;
            }

            if (matching.Count() > 1)
            {
                _logger.LogWarning("{Function} <<< multiple matches for {PackageId}${CiBranch}", nameof(TryResolveCiIg), directive.PackageId, directive.CiBranch);
            }

            FhirQasRec match = matching
                .OrderByDescending(q => q.BuildDate)
                .First();

            directive = DirectiveForQasRec(directive, match);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(WarningMsgEx, nameof(TryResolveCi), $"processing {directive.PackageId}${directive.CiBranch}", ex.Message, ex.InnerException?.Message ?? string.Empty);
        }

        // still here means nothing was found successfully
        return false;

        FhirDirective DirectiveForQasRec(FhirDirective directive, FhirQasRec rec)
        {
            string ciUrl = $"{_ciUri}ig/{rec.RepositoryUrl.Substring(0, rec.RepositoryUrl.Length - 8)}";

            string tarUrl;
            string fhirRelease;

            switch (directive.NameType)
            {
                case DirectiveNameTypeCodes.GuideWithSuffix:
                    tarUrl = $"{ciUrl}/package.{FhirVersionToRLiteral(directive.FhirRelease).ToLowerInvariant()}.tgz";
                    fhirRelease = directive.FhirRelease;
                    break;
                case DirectiveNameTypeCodes.GuideWithoutSuffix:
                    tarUrl = $"{ciUrl}/package.tgz";
                    fhirRelease = rec.FhirVersion;
                    break;
                case DirectiveNameTypeCodes.Unknown:
                default:
                    // assume there is a package.tgz there
                    tarUrl = $"{ciUrl}/package.tgz";
                    fhirRelease = rec.FhirVersion;
                    break;
            }

            return directive with
            {
                FhirRelease = fhirRelease,
                PackageVersion = rec.GuideVersion,
                CiUrl = ciUrl,
                CiOrg = GetOrg(rec.RepositoryUrl),
                CiBranch = GetBranch(rec.RepositoryUrl),
                BuildDate = rec.BuildDate,
                ResolvedTarballUrl = tarUrl,
                ResolvedSha = string.Empty,
            };
        }

        string GetOrg(string url) => url.Split('/').First();

        string GetBranch(string url)
        {
            string[] components = url.Split('/');
            if (components.Length > 1)
            {
                return components[components.Length - 2];
            }

            return string.Empty;
        }
    }

    /// <summary>Attempts to resolve ci.</summary>
    /// <param name="inputUrl">Input CI URL or fragment (not a package).</param>
    /// <param name="resolved">[out] The resolved directive record.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    internal bool TryResolveCi(
        string inputUrl,
        out FhirDirective? resolved)
    {
        if (string.IsNullOrEmpty(inputUrl))
        {
            resolved = null;
            return false;
        }

        FhirQasRec[] igs = [];

        // check to see if this is a CI-Build URL fragment
        if (inputUrl.StartsWith("HL7/", StringComparison.OrdinalIgnoreCase))
        {
            // check to see if this URL appears in qas.json
            try
            {
                string contents = _httpClient.GetStringAsync(_qasUri).Result;
                igs = JsonSerializer.Deserialize<FhirQasRec[]>(contents) ?? [];

                string searchUrl = inputUrl.EndsWith("qa.json", StringComparison.OrdinalIgnoreCase)
                    ? inputUrl
                    : (inputUrl.EndsWith('/') ? inputUrl + "qa.json" : inputUrl + "qa.json");

                IEnumerable<FhirQasRec> matching = igs.Where(x => x.RepositoryUrl.Equals(searchUrl, StringComparison.OrdinalIgnoreCase));

                if (matching.Any())
                {
                    // grab the most recently built match
                    FhirQasRec rec = matching
                        .OrderByDescending(q => q.BuildDate)
                        .First();

                    resolved = new()
                    {
                        Directive = GetDirective(rec.PackageId, rec.RepositoryUrl),
                        PackageId = rec.PackageId,
                        NameType = DirectiveNameTypeCodes.GuideWithoutSuffix,
                        FhirRelease = rec.FhirVersion,
                        PackageVersion = rec.GuideVersion,
                        VersionType = DirectiveVersionCodes.ContinuousIntegration,
                        CiBranch = GetBranch(rec.RepositoryUrl),
                        CiUrl = $"{_ciUri}ig/{rec.RepositoryUrl.Substring(0, rec.RepositoryUrl.Length - 8)}",
                        CiOrg = GetOrg(rec.RepositoryUrl),
                        BuildDate = rec.BuildDate,
                        ResolvedTarballUrl = $"{_ciUri}ig/{rec.RepositoryUrl.Substring(0, rec.RepositoryUrl.Length - 8)}/package.tgz",
                        ResolvedSha = string.Empty,
                    };

                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"TryResolveCi <<< processing {inputUrl} (fragment check stage) - caught: {ex.Message}");
                if (ex.InnerException != null)
                {
                    _logger.LogError($" <<< {ex.InnerException.Message}");
                }
            }
        }

        // check to see if this is a CI-Build package URL
        if (inputUrl.StartsWith("http://hl7.org/fhir/", StringComparison.OrdinalIgnoreCase))
        {
            // check to see if this URL appears in qas.json
            try
            {
                if (igs.Length == 0)
                {
                    Uri uri = new(_ciUri, "ig/qas.json");

                    string contents = _httpClient.GetStringAsync(uri).Result;

                    igs = JsonSerializer.Deserialize<FhirQasRec[]>(contents) ?? [];
                }

                IEnumerable<FhirQasRec> matching = igs.Where(x => x.Url.Equals(inputUrl, StringComparison.OrdinalIgnoreCase));

                if (matching.Any())
                {
                    // grab the most recently built match
                    FhirQasRec rec = matching
                        .OrderByDescending(q => q.BuildDate)
                        .First();

                    resolved = new()
                    {
                        Directive = GetDirective(rec.PackageId, rec.RepositoryUrl),
                        PackageId = rec.PackageId,
                        NameType = DirectiveNameTypeCodes.GuideWithoutSuffix,
                        FhirRelease = rec.FhirVersion,
                        PackageVersion = rec.GuideVersion,
                        VersionType = DirectiveVersionCodes.ContinuousIntegration,
                        CiBranch = GetBranch(rec.RepositoryUrl),
                        CiUrl = $"{_ciUri}ig/{rec.RepositoryUrl.Substring(0, rec.RepositoryUrl.Length - 8)}",
                        CiOrg = GetOrg(rec.RepositoryUrl),
                        BuildDate = rec.BuildDate,
                        ResolvedTarballUrl = $"{_ciUri}ig/{rec.RepositoryUrl.Substring(0, rec.RepositoryUrl.Length - 8)}/package.tgz",
                        ResolvedSha = string.Empty,
                    };

                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"TryResolveCi <<< processing {inputUrl} (qas url check stage) - caught: {ex.Message}");
                if (ex.InnerException != null)
                {
                    _logger.LogError($" <<< {ex.InnerException.Message}");
                }
            }
        }

        // check to see if this is a URL we can test via HTTP
        if (inputUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
        {
            string root = _matchFileUrlSuffix.IsMatch(inputUrl.ToLowerInvariant())
                ? inputUrl.Substring(0, inputUrl.LastIndexOf('/'))
                : inputUrl;

            if (root.EndsWith('/'))
            {
                root = root.Substring(0, root.Length - 1);
            }

            // try to get and parse a package manifest
            try
            {
                Uri uri;
                string fhirVersion = string.Empty;

                if (inputUrl.EndsWith(".tgz", StringComparison.OrdinalIgnoreCase))
                {
                    string fragment = inputUrl.Substring(inputUrl.LastIndexOf('/') + 1).ToLowerInvariant();

                    switch (fragment)
                    {
                        case "package.r4.tgz":
                            uri = new(root + "/package.r4.manifest.json");
                            fhirVersion = FhirSequenceCodes.R4.ToLongVersion();
                            break;

                        case "package.r4b.tgz":
                            uri = new(root + "/package.r4b.manifest.json");
                            fhirVersion = FhirSequenceCodes.R4B.ToLongVersion();
                            break;

                        case "package.r5.tgz":
                            uri = new(root + "/package.r5.manifest.json");
                            fhirVersion = FhirSequenceCodes.R5.ToLongVersion();
                            break;

                        case "package.r6.tgz":
                            uri = new(root + "/package.r6.manifest.json");
                            fhirVersion = FhirSequenceCodes.R6.ToLongVersion();
                            break;

                        case "package.tgz":
                        default:
                            uri = new(root + "/package.manifest.json");
                            break;
                    }
                }
                else
                {
                    uri = new(root + "/package.manifest.json");
                }

                string json = _httpClient.GetStringAsync(uri).Result;

                FhirPackageVersionInfo? v = JsonSerializer.Deserialize<FhirPackageVersionInfo>(json);

                if (v != null)
                {
                    resolved = new()
                    {
                        Directive = v.Name + "#current",
                        PackageId = v.Name,
                        NameType = _matchFhirSuffix.IsMatch(v.Name) ? DirectiveNameTypeCodes.GuideWithSuffix : DirectiveNameTypeCodes.GuideWithoutSuffix,
                        FhirRelease = string.IsNullOrEmpty(fhirVersion) ? v.FhirVersion : fhirVersion,
                        PackageVersion = v.Version,
                        VersionType = DirectiveVersionCodes.ContinuousIntegration,
                        CiUrl = root,
                        BuildDate = v.Date,
                        ResolvedTarballUrl = inputUrl.EndsWith(".tgz", StringComparison.OrdinalIgnoreCase) ? inputUrl : new Uri(new Uri(root), "package.tgz").ToString(),
                        ResolvedSha = string.Empty,
                    };

                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"TryResolveCi <<< processing {inputUrl} (http check stage) - caught: {ex.Message}");
                if (ex.InnerException != null)
                {
                    _logger.LogError($" <<< {ex.InnerException.Message}");
                }
            }
        }

        resolved = null;
        return false;

        string GetOrg(string url) => url.Split('/').First();

        string GetBranch(string url)
        {
            string[] components = url.Split('/');
            if (components.Length > 1)
            {
                return components[components.Length - 2];
            }

            return string.Empty;
            //return url
            //    .Replace("https://build.fhir.org/ig/", string.Empty, StringComparison.Ordinal)
            //    .Replace("http://build.fhir.org/ig/", string.Empty, StringComparison.Ordinal)
            //    .Replace("/qa.json", string.Empty, StringComparison.Ordinal);
        }

        string GetDirective(string id, string url)
        {
            string branch = GetBranch(url);
            return branch switch
            {
                "main" => $"{id}#current",
                "master" => $"{id}#current",
                "" => $"{id}#current",
                _ => $"{id}#current${branch}"
            };
        }
    }

    /// <summary>Information about a package version.</summary>
    /// <param name="version">    The version.</param>
    /// <param name="date">       The date.</param>
    /// <param name="versionRoot">The version root.</param>
    /// <param name="versionTag"> The version tag.</param>
    internal record struct VersionInfo(
        string version,
        string date,
        string versionRoot,
        string versionTag);

    /// <summary>Attempts to resolve version range.</summary>
    /// <param name="directive">[out] The parsed.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    internal bool TryResolveVersionRange(ref FhirDirective directive)
    {
        if (directive.VersionType != DirectiveVersionCodes.Partial)
        {
            return false;
        }

        // get manifests from each registry if we have not done so
        if (directive.Manifests.Count == 0)
        {
            _ = TryGetRegistryManifests(ref directive);
        }

        Dictionary<Uri, HashSet<string>> knownVersions = [];
        Dictionary<Uri, string> latestVersions = [];
        Dictionary<Uri, string> highestVersions = [];

        // traverse our manifests and build a list of known versions per registry
        foreach ((Uri uri, RegistryPackageManifest manifest) in directive.Manifests)
        {
            if (manifest.Versions == null || manifest.Versions.Count == 0)
            {
                continue;
            }

            string highestVersion = manifest.HighestVersion(directive.PackageVersion);

            if (string.IsNullOrEmpty(highestVersion))
            {
                continue;
            }

            highestVersions.Add(uri, highestVersion);
            knownVersions.Add(uri, [.. manifest.Versions.Keys]);

            // check for a tagged latest
            if (manifest.DistributionTags.TryGetValue("latest", out string? latest))
            {
                latestVersions[uri] = latest;
            }
        }

        string versionRoot = directive.PackageVersion.EndsWith(".x", StringComparison.OrdinalIgnoreCase)
            ? directive.PackageVersion.Substring(0, directive.PackageVersion.Length - 2)
            : directive.PackageVersion;

        // traverse registries in order to see if one can resolve our version with a 'latest'
        foreach (Uri uri in _registryUris)
        {
            // check for a matching latest
            if (!latestVersions.TryGetValue(uri, out string? latest))
            {
                continue;
            }

            if (latest.StartsWith(versionRoot, StringComparison.OrdinalIgnoreCase))
            {
                directive = directive with
                {
                    PackageVersion = latest,
                    VersionType = DirectiveVersionCodes.Exact,
                    ResolvedTarballUrl = directive.Manifests[uri].Versions[latest].Distribution?.TarballUrl ?? string.Empty,
                    ResolvedSha = directive.Manifests[uri].Versions[latest].Distribution?.HashSHA ?? string.Empty,
                };
                return true;
            }
        }

        // traverse registries in order to see if one can resolve our version with any known version
        foreach (Uri uri in _registryUris)
        {
            // check for no version from this registry
            if (!knownVersions.TryGetValue(uri, out HashSet<string>? versions))
            {
                continue;
            }

            // if there is no 'highest' matching version, go to the next registry
            if ((!highestVersions.TryGetValue(uri, out string? version)) ||
                string.IsNullOrEmpty(version))
            {
                continue;
            }

            // grab the 'highest' sort version - best guess since we cannot guarantee SemVer
            directive = directive with
            {
                PackageVersion = version,
                VersionType = DirectiveVersionCodes.Exact,
                ResolvedTarballUrl = directive.Manifests[uri].Versions[version].Distribution?.TarballUrl ?? string.Empty,
                ResolvedSha = directive.Manifests[uri].Versions[version].Distribution?.HashSHA ?? string.Empty,
            };

            return true;
        }

        return false;
    }

    internal bool TryResolveVersionExact(ref FhirDirective directive)
    {
        if (directive.VersionType != DirectiveVersionCodes.Exact)
        {
            return false;
        }

        // get manifests from each registry if we have not done so
        if (directive.Manifests.Count == 0)
        {
            _ = TryGetRegistryManifests(ref directive);
        }

        Dictionary<Uri, HashSet<string>> knownVersions = [];
        Dictionary<Uri, string> matchingVersions = [];

        // traverse our manifests and build a list of known versions per registry
        foreach ((Uri uri, RegistryPackageManifest manifest) in directive.Manifests)
        {
            if (manifest.Versions == null || manifest.Versions.Count == 0)
            {
                continue;
            }

            if (manifest.Versions.TryGetValue(directive.PackageVersion, out FhirPackageVersionInfo? pi))
            {
                directive = directive with
                {
                    PackageVersion = directive.PackageVersion,
                    VersionType = DirectiveVersionCodes.Exact,
                    ResolvedTarballUrl = pi.Distribution?.TarballUrl ?? string.Empty,
                    ResolvedSha = pi.Distribution?.HashSHA ?? string.Empty,
                };
                return true;
            }
        }

        return false;
    }

    /// <summary>Attempts to resolve version latest.</summary>
    /// <param name="directive">[out] The parsed directive.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    internal bool TryResolveVersionLatest(ref FhirDirective directive)
    {
        if (directive.VersionType != DirectiveVersionCodes.Latest)
        {
            return false;
        }

        // get manifests from each registry if we have not done so
        if (directive.Manifests.Count == 0)
        {
            _ = TryGetRegistryManifests(ref directive);
        }

        Dictionary<Uri, HashSet<string>> knownVersions = [];
        Dictionary<Uri, string> latestVersion = [];

        // traverse our manifests and build a list of known versions per registry
        foreach ((Uri uri, RegistryPackageManifest manifest) in directive.Manifests)
        {
            if (manifest.Versions == null || manifest.Versions.Count == 0)
            {
                continue;
            }

            knownVersions[uri] = [.. manifest.Versions.Keys];

            // check for a tagged latest
            if (manifest.DistributionTags.TryGetValue("latest", out string? latest))
            {
                latestVersion[uri] = latest;
            }
        }

        // check to see if we have any 'latest' tagged versions
        if (latestVersion.Count != 0)
        {
            string testVal = latestVersion.First().Value;

            // check to see if they are all the same
            if (!latestVersion.Any(kvp => !kvp.Value.Equals(testVal)))
            {
                Uri uri = latestVersion.First().Key;

                directive = directive with
                {
                    PackageVersion = testVal,
                    VersionType = DirectiveVersionCodes.Exact,
                    ResolvedTarballUrl = directive.Manifests[uri].Versions[testVal].Distribution?.TarballUrl ?? string.Empty,
                    ResolvedSha = directive.Manifests[uri].Versions[testVal].Distribution?.HashSHA ?? string.Empty,
                };

                return true;
            }

            // check to see if there is a 'latest' that does not exist on other registries, but traverse in registry preference order
            foreach (Uri uri in _registryUris)
            {
                if (!latestVersion.TryGetValue(uri, out string? latest))
                {
                    continue;
                }

                bool found = true;
                foreach (HashSet<string> knowns in knownVersions.Values)
                {
                    if (!knowns.Contains(latest))
                    {
                        found = false;
                        break;
                    }
                }

                if (!found)
                {
                    // we found a 'latest' that is not known on all registries
                    directive = directive with
                    {
                        PackageVersion = latest,
                        VersionType = DirectiveVersionCodes.Exact,
                        ResolvedTarballUrl = directive.Manifests[uri].Versions[latest].Distribution?.TarballUrl ?? string.Empty,
                        ResolvedSha = directive.Manifests[uri].Versions[latest].Distribution?.HashSHA ?? string.Empty,
                    };
                    return true;
                }
            }

            // just use the 'latest' from the most-preferred registry
            foreach (Uri uri in _registryUris)
            {
                if (!latestVersion.TryGetValue(uri, out string? latest))
                {
                    continue;
                }

                directive = directive with
                {
                    PackageVersion = latest,
                    VersionType = DirectiveVersionCodes.Exact,
                    ResolvedTarballUrl = directive.Manifests[uri].Versions[latest].Distribution?.TarballUrl ?? string.Empty,
                    ResolvedSha = directive.Manifests[uri].Versions[latest].Distribution?.HashSHA ?? string.Empty,
                };
                return true;
            }
        }

        // traverse registries in order and use whatever we can find
        foreach (Uri uri in _registryUris)
        {
            // check for a matching latest
            if (!knownVersions.TryGetValue(uri, out HashSet<string>? versions) ||
                (versions == null) ||
                (versions.Count == 0))
            {
                continue;
            }

            string version = versions.OrderByDescending(v => v).First();

            // grab the 'highest' sort version - best guess since we cannot guarantee SemVer
            directive = directive with
            {
                PackageVersion = version,
                VersionType = DirectiveVersionCodes.Exact,
                ResolvedTarballUrl = directive.Manifests[uri].Versions[version].Distribution?.TarballUrl ?? string.Empty,
                ResolvedSha = directive.Manifests[uri].Versions[version].Distribution?.HashSHA ?? string.Empty,
            };

            return true;
        }

        return false;
    }

    /// <summary>Attempts to get registry manifests.</summary>
    /// <param name="directive">[out] The parsed.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    internal bool TryGetRegistryManifests(ref FhirDirective directive)
    {
        ConcurrentDictionary<Uri, RegistryPackageManifest> manifests = new();

        // core package lookups need to use a resolvable package name - `core` always exists
        string packageId = directive.NameType == DirectiveNameTypeCodes.CorePartial
            ? directive.PackageId + ".core"
            : directive.PackageId;

        Parallel.ForEach(_registryUris, registryUri =>
        {
            try
            {
                Uri requestUri = new(registryUri, packageId);

                HttpResponseMessage response = _httpClient.GetAsync(requestUri).Result;

                if (!response.IsSuccessStatusCode)
                {
                    //_logger.LogInformation(
                    //    $"TryGetRegistryManifests <<<" +
                    //    $" Failed to get package info: {response.StatusCode}" +
                    //    $" {requestUri.AbsoluteUri}");
                    return;
                }

                string json = response.Content.ReadAsStringAsync().Result;

                RegistryPackageManifest? info = JsonSerializer.Deserialize<RegistryPackageManifest>(json);

                if (!(info?.Name.Equals(packageId, StringComparison.OrdinalIgnoreCase) ?? false))
                {
                    //_logger.LogInformation(
                    //    $"TryGetRegistryManifests <<<" +
                    //    $" Package information mismatch: requested {requestUri.AbsoluteUri}" +
                    //    $" received manifest for {info?.Name}");
                    return;
                }

                if (info.Versions == null || info.Versions.Count == 0)
                {
                    //_logger.LogInformation(
                    //    $"TryGetRegistryManifests <<<" +
                    //    $" package {requestUri.AbsoluteUri}" +
                    //    $" contains NO versions");
                    return;
                }

                _ = manifests.TryAdd(registryUri, info);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(
                    $"TryGetRegistryManifests <<<" +
                    $" Server {registryUri.AbsoluteUri}" +
                    $" Package {packageId}" +
                    $" threw: {ex.Message}");
                if (ex.InnerException != null)
                {
                    _logger.LogInformation($" <<< {ex.InnerException.Message}");
                }
            }
        });

        directive.Manifests = manifests.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        return directive.Manifests.Count != 0;
    }

    /// <summary>Attempts to catalog search.</summary>
    /// <param name="directive">[out] The parsed.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    internal bool TryCatalogSearch(ref FhirDirective directive)
    {
        ConcurrentDictionary<Uri, Dictionary<string, FhirNpmPackageDetails>> catalog = new();

        string packageId = directive.PackageId;

        Parallel.ForEach(_registryUris, registryUri =>
        {
            try
            {
                Uri requestUri = new(
                    registryUri,
                    $"catalog?op=find&name={packageId}&pkgcanonical=&canonical=&fhirversion=");

                HttpResponseMessage response = _httpClient.GetAsync(requestUri).Result;

                if (!response.IsSuccessStatusCode)
                {
                    //_logger.LogInformation(
                    //    $"TryCatalogSearch <<<" +
                    //    $" Failed to get package info: {response.StatusCode}" +
                    //    $" {requestUri.AbsoluteUri}");
                    return;
                }

                string json = response.Content.ReadAsStringAsync().Result;

                // TODO: packages.fhir.org is currently returning PascalCase instead of CamelCase - remove this (performance) when fixed
                IEnumerable<FhirNpmPackageDetails>? entries = JsonSerializer.Deserialize<IEnumerable<FhirNpmPackageDetails>>(json, _jsoCaseInsensitive);

                if (!(entries?.Any() ?? false))
                {
                    return;
                }

                _ = catalog.TryAdd(registryUri, entries.ToDictionary(e => e.Name, e => e));
            }
            catch (Exception ex)
            {
                _logger.LogInformation(
                    $"TryCatalogSearch <<<" +
                    $" Server {registryUri.AbsoluteUri}" +
                    $" Package {packageId}" +
                    $" threw: {ex.Message}");
                if (ex.InnerException != null)
                {
                    _logger.LogInformation($" <<< {ex.InnerException.Message}");
                }
            }
        });

        directive = directive with
        {
            CatalogEntries = catalog.ToDictionary(kvp => kvp.Key, kvp => kvp.Value),
        };

        return directive.CatalogEntries.Count != 0;
    }

    /// <summary>
    /// Attempts to resolve ig name the FhirSequenceCodes from the given FhirDirective.
    /// </summary>
    /// <param name="directive">     [out] The parsed directive.</param>
    /// <param name="forFhirVersion">(Optional) FHIR version to restrict downloads to.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    internal bool TryResolveNameFromCatalog(
        ref FhirDirective directive,
        FhirSequenceCodes forFhirVersion = FhirSequenceCodes.Unknown)
    {
        if (directive.NameType != DirectiveNameTypeCodes.GuideWithoutSuffix)
        {
            // other package types are fine
            return true;
        }

        // perform a catalog search if we need to
        if ((directive.CatalogEntries.Count == 0) &&
            (!TryCatalogSearch(ref directive)))
        {
            _logger.LogWarning("{Function} <<< catalog search failed for package: {PackageId}", nameof(TryResolveNameFromCatalog), directive.PackageId);
            return false;
        }

        // traverse registries in preferred order
        foreach (Uri uri in _registryUris)
        {
            if ((!directive.CatalogEntries.TryGetValue(uri, out Dictionary<string, FhirNpmPackageDetails>? catalog)) ||
                (catalog == null))
            {
                continue;
            }
            
            // if we have a package that matches the requested FHIR version, promote it
            foreach (FhirNpmPackageDetails entry in catalog.Values)
            {
                // check for a matching FHIR version or the caller asking for any
                if ((forFhirVersion == FhirSequenceCodes.Unknown) ||
                    entry.FhirVersion.Equals(forFhirVersion.ToLiteral(), StringComparison.OrdinalIgnoreCase) ||
                    entry.FhirVersion.Equals(forFhirVersion.ToRLiteral(), StringComparison.OrdinalIgnoreCase))
                {
                    if (FhirPackageUtils.PackageIsFhirRelease(entry.Name))
                    {
                        directive = directive with
                        {
                            PackageId = entry.Name,
                            NameType = DirectiveNameTypeCodes.CoreFull,
                            FhirRelease = entry.FhirVersion,
                        };
                    }
                    else if (_matchFhirSuffix.IsMatch(entry.Name))
                    {
                        directive = directive with
                        {
                            PackageId = entry.Name,
                            NameType = DirectiveNameTypeCodes.GuideWithSuffix,
                            FhirRelease = entry.FhirVersion,
                        };
                    }
                    else
                    {
                        directive = directive with
                        {
                            PackageId = entry.Name,
                            NameType = DirectiveNameTypeCodes.GuideWithoutSuffix,
                            FhirRelease = entry.FhirVersion,
                        };
                    }

                    return true;
                }
            }
        }

        // no match here means we failed to resolve
        return false;
    }

    /// <summary>
    /// Attempts to get cache record a PackageCacheRecord from the given IEnumerable&lt;string&gt;
    /// </summary>
    /// <param name="directives">The directives.</param>
    /// <param name="cached">    [out] The cached.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    private bool TryGetCacheRec(IEnumerable<string> directives, out PackageCacheRecord cached)
    {
        foreach (string directive in directives)
        {
            if (_packagesByDirective.TryGetValue(directive, out cached))
            {
                return true;
            }
        }

        cached = default;
        return false;
    }

    /// <summary>Resolve directive and download contents if necessary.</summary>
    /// <param name="inputDirective">The input directive.</param>
    /// <param name="package">       [out] The package.</param>
    /// <param name="forFhirVersion">(Optional) FHIR version to restrict downloads to.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    internal async Task<PackageCacheEntry?> ResolveAndDownloadDirective(
        string inputDirective,
        FhirSequenceCodes forFhirVersion = FhirSequenceCodes.Unknown,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation($"Request to resolve: {inputDirective}");

        // parse the requested directive
        if ((!TryParseDirective(inputDirective, out FhirDirective? directive)) ||
            (directive == null))
        {
            _logger.LogError($"Failed to parse directive input: {inputDirective}");
            return null;
        }

        List<string> resolvableDirectives = UpdateDirectives(directive);

        PackageCacheRecord cached;

        // can only rely on the cache if we have an exact or non-semver version
        switch (directive.VersionType)
        {
            case DirectiveVersionCodes.Exact:
            case DirectiveVersionCodes.NonSemVer:
                {
                    // check for a local version of the package
                    if (TryGetCacheRec(resolvableDirectives, out cached))
                    {
                        return new()
                        {
                            FhirVersion = cached.FhirVersion,
                            Directory = Path.Combine(_cachePackageDirectory, cached.CacheDirective),
                            ResolvedDirective = cached.CacheDirective,
                            Name = cached.PackageName,
                            Version = cached.Version,
                        };
                    }
                }
                break;

            case DirectiveVersionCodes.Unknown:
            case DirectiveVersionCodes.Partial:
            case DirectiveVersionCodes.Latest:
            case DirectiveVersionCodes.Local:
            case DirectiveVersionCodes.ContinuousIntegration:
            default:
                break;
        }

        // check if we want to look for FHIR-versioned packages
        if ((directive.NameType == DirectiveNameTypeCodes.GuideWithoutSuffix) &&
            (forFhirVersion != FhirSequenceCodes.Unknown))
        {
            if (TryResolveNameFromCatalog(ref directive, forFhirVersion))
            {
                resolvableDirectives = UpdateDirectives(directive);
            }
        }

        // handle additional resolution based on version type
        switch (directive.VersionType)
        {
            case DirectiveVersionCodes.NonSemVer:
            case DirectiveVersionCodes.Exact:
                // we have an exact version, check for a local copy
                if (TryGetCacheRec(resolvableDirectives, out cached))
                {
                    return new()
                    {
                        FhirVersion = cached.FhirVersion,
                        Directory = Path.Combine(_cachePackageDirectory, cached.CacheDirective),
                        ResolvedDirective = cached.CacheDirective,
                        Name = cached.PackageName,
                        Version = cached.Version,
                    };
                }

                // attempt to resolve the exact version
                if (TryResolveVersionExact(ref directive))
                {
                    resolvableDirectives = UpdateDirectives(directive);

                    // we now have an exact version, check for a local copy
                    if (TryGetCacheRec(resolvableDirectives, out cached))
                    {
                        return new()
                        {
                            FhirVersion = cached.FhirVersion,
                            Directory = Path.Combine(_cachePackageDirectory, cached.CacheDirective),
                            ResolvedDirective = cached.CacheDirective,
                            Name = cached.PackageName,
                            Version = cached.Version,
                        };
                    }
                }

                break;
            case DirectiveVersionCodes.Partial:
                {
                    // attempt to resolve the version range
                    if (!TryResolveVersionRange(ref directive))
                    {
                        _logger.LogError($"Failed to resolve version range: {inputDirective}");
                        return null;
                    }

                    resolvableDirectives = UpdateDirectives(directive);

                    // we now have an exact version, check for a local copy
                    if (TryGetCacheRec(resolvableDirectives, out cached))
                    {
                        return new()
                        {
                            FhirVersion = cached.FhirVersion,
                            Directory = Path.Combine(_cachePackageDirectory, cached.CacheDirective),
                            ResolvedDirective = cached.CacheDirective,
                            Name = cached.PackageName,
                            Version = cached.Version,
                        };
                    }
                }
                break;
            case DirectiveVersionCodes.Latest:
                {
                    // attempt to resolve the latest version
                    if (!TryResolveVersionLatest(ref directive))
                    {
                        _logger.LogError($"Failed to resolve latest version: {inputDirective}");
                        return null;
                    }

                    resolvableDirectives = UpdateDirectives(directive);

                    // we now have an exact version, check for a local copy
                    if (TryGetCacheRec(resolvableDirectives, out cached))
                    {
                        return new()
                        {
                            FhirVersion = cached.FhirVersion,
                            Directory = Path.Combine(_cachePackageDirectory, cached.CacheDirective),
                            ResolvedDirective = cached.CacheDirective,
                            Name = cached.PackageName,
                            Version = cached.Version,
                        };
                    }
                }
                break;
            case DirectiveVersionCodes.Local:
                {
                    // check for local entry
                    if (TryGetCacheRec(resolvableDirectives, out cached))
                    {
                        return new()
                        {
                            FhirVersion = cached.FhirVersion,
                            Directory = Path.Combine(_cachePackageDirectory, cached.CacheDirective),
                            ResolvedDirective = cached.CacheDirective,
                            Name = cached.PackageName,
                            Version = cached.Version,
                        };
                    }

                    // if we do not have a local version, fall-back to CI
                    directive = directive with
                    {
                        Directive = $"{directive.PackageId}#current",
                        VersionType = DirectiveVersionCodes.ContinuousIntegration,
                    };
                    
                    // attempt to resolve via CI
                    if (!TryResolveCi(ref directive))
                    {
                        _logger.LogError($"Not found locally and failed to resolve via CI: {inputDirective}");
                        return null;
                    }

                    resolvableDirectives = UpdateDirectives(directive);

                    // we now have CI info, compare to local and accept if it was downloaded on or after the current CI build date
                    if (TryGetCacheRec(resolvableDirectives, out cached) &&
                        (cached.DownloadDateTime.CompareTo(directive.BuildDate) >= 0))
                    {
                        return new()
                        {
                            FhirVersion = cached.FhirVersion,
                            Directory = Path.Combine(_cachePackageDirectory, cached.CacheDirective),
                            ResolvedDirective = cached.CacheDirective,
                            Name = cached.PackageName,
                            Version = cached.Version,
                        };
                    }
                }
                break;
            case DirectiveVersionCodes.ContinuousIntegration:
                {
                    // check to see if we have a local copy
                    _ = TryGetCacheRec(resolvableDirectives, out cached);

                    // attempt to resolve via CI
                    if (TryResolveCi(ref directive))
                    {
                        resolvableDirectives = UpdateDirectives(directive);

                        //_logger.LogError($"Failed to resolve via CI: {inputDirective}");
                        //return null;
                    }

                    // we now have CI info, compare to local and accept if it was downloaded on or after the current CI build date
                    if (TryGetCacheRec(resolvableDirectives, out cached) &&
                        (cached.DownloadDateTime.CompareTo(directive.BuildDate) >= 0))
                    {
                        return new()
                        {
                            FhirVersion = cached.FhirVersion,
                            Directory = Path.Combine(_cachePackageDirectory, cached.CacheDirective),
                            ResolvedDirective = cached.CacheDirective,
                            Name = cached.PackageName,
                            Version = cached.Version,
                        };
                    }
                }
                break;

            case DirectiveVersionCodes.Unknown:
            default:
                {
                    _logger.LogError($"Directive could not be parsed (unknown version type): {inputDirective}");
                    return null;
                }
        }

        if (string.IsNullOrEmpty(directive.ResolvedTarballUrl) &&
            string.IsNullOrEmpty(directive.PublicationPackageUrl))
        {
            _logger.LogError($"Directive did not contain a tarball URL and could not determine a publication fallback: {inputDirective}");
            return null;
        }

        // check for cancellation before downloading
        cancellationToken.ThrowIfCancellationRequested();

        List<string> urls = [];

        if (!string.IsNullOrEmpty(directive.ResolvedTarballUrl))
        {
            urls.Add(directive.ResolvedTarballUrl);
        }

        if (!string.IsNullOrEmpty(directive.PublicationPackageUrl))
        {
            urls.Add(directive.PublicationPackageUrl);
        }

        string dlDir = Path.Combine(_cachePackageDirectory, directive.Directive);

        bool downloaded = false;
        FhirSequenceCodes resolvedFhirVersion = FhirSequenceCodes.Unknown;
        string resolvedDirective = string.Empty;

        foreach (string url in urls)
        {
            Uri dlUri = new(url);

            try
            {
                (downloaded, resolvedFhirVersion, resolvedDirective) = await DownloadAndExtract(
                    dlUri,
                    dlDir,
                    directive.Directive,
                    cancellationToken);

                if (downloaded)
                {
                    break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to download {inputDirective}: {ex.Message}{((ex.InnerException != null) ? ex.InnerException.Message : string.Empty)}");
            }
        }

        if (!downloaded)
        {
            _logger.LogError($"Failed to download {inputDirective}: attempted: {string.Join(", ", urls)}");
            return null;
        }

        UpdatePackageCacheIndex(resolvedDirective, dlDir);
        StateHasChanged();

        return new()
        {
            FhirVersion = resolvedFhirVersion,
            Directory = dlDir,
            ResolvedDirective = resolvedDirective,
            Name = directive.PackageId,
            Version = directive.PackageVersion,
        };

        List<string> UpdateDirectives(FhirDirective directive)
        {
            if (!string.IsNullOrEmpty(directive.PackageId) && !string.IsNullOrEmpty(directive.PackageVersion))
            {
                switch (directive.NameType)
                {
                    case DirectiveNameTypeCodes.CoreFull:
                        {
                            return
                            [
                                $"{directive.PackageId}#{directive.PackageVersion}",
                            ];
                        }
                    case DirectiveNameTypeCodes.CorePartial:
                        {
                            return
                            [
                                $"{directive.PackageId}#{directive.PackageVersion}",
                                $"{directive.PackageId}.core#{directive.PackageVersion}",
                            ];
                        }
                    case DirectiveNameTypeCodes.GuideWithSuffix:
                        {
                            return
                            [
                                $"{directive.PackageId}#{directive.PackageVersion}",
                            ];
                        }
                    case DirectiveNameTypeCodes.GuideWithoutSuffix:
                        {
                            if (string.IsNullOrEmpty(directive.FhirRelease))
                            {
                                return
                                [
                                    $"{directive.PackageId}#{directive.PackageVersion}",
                                ];
                            }

                            return
                            [
                                $"{directive.PackageId}#{directive.PackageVersion}",
                                $"{directive.PackageId}.{FhirVersionToRLiteral(directive.FhirRelease)}#{directive.PackageVersion}",
                            ];
                        }

                    default:
                        {
                            return
                            [
                                $"{directive.PackageId}#{directive.PackageVersion}",
                            ];
                        }
                }
            }

            switch (directive.NameType)
            {
                case DirectiveNameTypeCodes.CoreFull:
                    {
                        return
                        [
                            directive.Directive,
                        ];
                    }
                case DirectiveNameTypeCodes.CorePartial:
                    {
                        return
                        [
                            directive.Directive,
                            directive.Directive.Replace("#", ".core#"),
                        ];
                    }
                case DirectiveNameTypeCodes.GuideWithSuffix:
                    {
                        return
                        [
                            directive.Directive,
                        ];
                    }
                case DirectiveNameTypeCodes.GuideWithoutSuffix:
                    {
                        if (string.IsNullOrEmpty(directive.FhirRelease))
                        {
                            return
                            [
                                directive.Directive,
                            ];
                        }

                        return
                        [
                            directive.Directive,
                            directive.PackageId.Replace("#", $".{FhirVersionToRLiteral(directive.FhirRelease)}#"),
                        ];
                    }

                default:
                    {
                        return
                        [
                            directive.Directive,
                        ];
                    }
            }
        }
    }

    /// <summary>Gets directory size.</summary>
    /// <param name="directory">[out] Pathname of the directory.</param>
    /// <returns>The directory size.</returns>
    private static long GetDirectorySize(string directory)
    {
        DirectoryInfo dirInfo = new(directory);
        IEnumerable<FileInfo> fileInfos = dirInfo.EnumerateFiles("*.*", SearchOption.AllDirectories);

        return fileInfos.Sum(fi => fi.Length);
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

                IniData data;

                lock (_iniFileLock)
                {
                    // open the file in shared mode because we are only reading here
                    using (Stream s = new FileStream(_iniFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    using (StreamReader r = new(s))
                    {
                        data = parser.Parse(r.ReadToEnd());
                    }
                }

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

            _packagesByDirective.Remove(directive);

            if (_versionsByName.TryGetValue(name, out List<string>? value))
            {
                _versionsByName[name] = value.Where((v) => !v.Equals(directiveVersion)).ToList();
            }

            return;
        }

        long size = GetDirectorySize(directory);
        string packageDate = DateTime.Now.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture);

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
                name,
                npmDetails.Version,
                FhirVersionToSequence(npmDetails.FhirVersion),
                packageDate,
                size,
                npmDetails);

            _packagesByDirective[directive] = record;

            // update the versions list
            if (_versionsByName.TryGetValue(name, out List<string>? versions))
            {
                if (!versions.Contains(npmDetails.Version))
                {
                    versions.Add(npmDetails.Version);
                }
            }
            else
            {
                _ = _versionsByName.TryAdd(name, [ npmDetails.Version ]);
            }
        }

        if (iniData == null)
        {
            IniDataParser parser = new();

            IniData data = parser.Parse(File.ReadAllText(_iniFilePath));

            if (!data.Sections.Contains("packages"))
            {
                data.Sections.Add("packages");
            }

            if (data["packages"].Contains(directive))
            {
                data["packages"].Remove(directive);
            }

            data["packages"].Add(directive, packageDate);

            if (!data.Sections.Contains("package-sizes"))
            {
                data.Sections.Add("package-sizes");
            }

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

        StateHasChanged();
    }

    /// <summary>Attempts to download and extract a string from the given URI.</summary>
    /// <param name="uri">              URI of the resource.</param>
    /// <param name="directory">        Pathname of the directory.</param>
    /// <param name="packageDirective"> The package directive.</param>
    /// <param name="fhirVersion">      [out] The FHIR version.</param>
    /// <param name="resolvedDirective">[out] The resolved directive.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    private async Task<(bool success, FhirSequenceCodes fhirVersion, string resolvedDirective)> DownloadAndExtract(
        Uri uri,
        string directory,
        string packageDirective,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using (Stream rawStream = await _httpClient.GetStreamAsync(uri, cancellationToken))
            using (Stream gzipStream = new GZipStream(rawStream, CompressionMode.Decompress))
            {
                // make sure our destination directory exists
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                await TarFile.ExtractToDirectoryAsync(gzipStream, directory, true, cancellationToken);
            }

            UpdatePackageCacheIndex(packageDirective, directory);

            // successful download and extraction
            return (true, _packagesByDirective[packageDirective].FhirVersion, packageDirective);
        }
        catch (HttpRequestException hex)
        {
            // we have a lot of not found because of package nesting, this is reported elsewhere
            if (hex.StatusCode == HttpStatusCode.NotFound)
            {
                return (false, FhirSequenceCodes.Unknown, string.Empty);
            }

            _logger.LogInformation($"DownloadAndExtract <<< exception downloading {uri}: {hex.Message}");
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
                    return (false, FhirSequenceCodes.Unknown, string.Empty);
                }
            }

            _logger.LogInformation($"DownloadAndExtract <<< exception downloading: {uri}: {ex.Message}");
            if (ex.InnerException != null)
            {
                _logger.LogInformation($" <<< inner: {ex.InnerException.Message}");
            }
        }

        // make sure to clean our directory so we don't load a partial package on next run
        try
        {
            if (Directory.Exists(directory))
            {
                Directory.Delete(directory, true);
            }
        }
        catch(Exception) { }

        // return failure info
        return (false, FhirSequenceCodes.Unknown, string.Empty);
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

        FhirSequenceCodes sequence = FhirVersionToSequence(version);

        if (sequence == FhirSequenceCodes.Unknown)
        {
            relative = string.Empty;
            return false;
        }

        // major releases are promoted to their version name root
        relative = sequence.ToLiteral();
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

        bool success;
        string directive = name + "#" + version;
        directory = Path.Combine(_cachePackageDirectory, directive);

        // most publication versions are named with correct package information
        Uri uri = new(_publicationUri, $"{relative}/{name}.tgz");

        (success, fhirVersion, resolvedDirective) = DownloadAndExtract(uri, directory, directive).Result;
        if (success)
        {
            UpdatePackageCacheIndex(directive, directory);
            return true;
        }

        // some ballot versions are published directly as CI versions
        uri = new Uri(_publicationUri, $"{relative}/package.tgz");
        directory = Path.Combine(_cachePackageDirectory, $"hl7.fhir.core#{version}");

        (success, fhirVersion, resolvedDirective) = DownloadAndExtract(uri, directory, directive).Result;
        if (success)
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
    /// <param name="name">     The name.</param>
    /// <param name="version">  [out] The version string (e.g., 4.0.1).</param>
    /// <param name="isCached"> [out] True if is cached, false if not.</param>
    /// <param name="directory">[out] Pathname of the directory.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    private bool TryGetHighestVersion(
        string name,
        out string version,
        out bool isCached,
        out string directory)
    {
        string highestCached = string.Empty;
        string highestOnline = string.Empty;

        _ = TryGetHighestVersionOffline(name, out highestCached);

        if (!_offlineMode)
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
        if (!_versionsByName.TryGetValue(name, out List<string>? value))
        {
            version = string.Empty;
            return false;
        }

        string highestVersion = string.Empty;

        foreach (string cachedVersion in value)
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
        List<RegistryPackageManifest> manifestList = [];

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

        if (manifestList.Count != 0)
        {
            manifests = manifestList.AsEnumerable();
            return true;
        }

        //_logger.LogInformation(
        //    $"Package {packageName}" +
        //    $" was not found on any registry.");
        manifests = [];
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

        List<string> directivesToRemove = [];

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

    ///// <summary>Updates the package state.</summary>
    ///// <param name="directive">      The directive.</param>
    ///// <param name="resolvedName">   Name of the resolved.</param>
    ///// <param name="resolvedVersion">The resolved version.</param>
    ///// <param name="toState">        State of to.</param>
    //private void UpdatePackageState(
    //    string directive,
    //    string resolvedName,
    //    string resolvedVersion,
    //    PackageLoadStateCodes toState)
    //{
    //    if (!_packagesByDirective.ContainsKey(directive))
    //    {
    //        _packagesByDirective.Add(directive, new()
    //        {
    //            CacheDirective = directive,
    //            PackageState = toState,
    //        });
    //    }

    //    _packagesByDirective[directive] = _packagesByDirective[directive] with
    //    {
    //        PackageState = toState,
    //        PackageName = string.IsNullOrEmpty(resolvedName) ? _packagesByDirective[directive].PackageName : resolvedName,
    //        Version = string.IsNullOrEmpty(resolvedVersion) ? _packagesByDirective[directive].Version : resolvedVersion,
    //    };

    //    StateHasChanged();
    //}

    ///// <summary>
    ///// Attempts to get a package state, returning a default value rather than throwing an exception
    ///// if it fails.
    ///// </summary>
    ///// <param name="directive">The directive.</param>
    ///// <param name="state">    [out] The state.</param>
    ///// <returns>True if it succeeds, false if it fails.</returns>
    //private bool TryGetPackageState(string directive, out PackageLoadStateCodes state)
    //{
    //    if (!_packagesByDirective.ContainsKey(directive))
    //    {
    //        state = PackageLoadStateCodes.Unknown;
    //        return false;
    //    }

    //    state = _packagesByDirective[directive].PackageState;
    //    return true;
    //}

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
                //PackageLoadStateCodes.NotLoaded,
                name,
                version,
                FhirVersionToSequence(versionInfo.FhirVersion),
                packageDate,
                size,
                versionInfo);

        _packagesByDirective[directive] = record;

        if (_versionsByName.TryGetValue(name, out List<string>? value))
        {
            if (!value.Contains(version))
            {
                value.Add(version);
            }
        }
        else
        {
            _ = _versionsByName.TryAdd(name, [ version ]);
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

        lock (_iniFileLock)
        {
            File.WriteAllText(destinationPath, formatter.Format(data, formattingConfig));
        }
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

    /// <summary>A package has been downloaded or deleted.</summary>
    public void StateHasChanged()
    {
        OnChanged?.Invoke(this, new());
    }
}
