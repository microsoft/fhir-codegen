using System;
using System.Collections.Generic;
using System.Formats.Tar;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Microsoft.Health.Fhir.PackageCacheManager;

/// <summary>
/// 
/// </summary>
public partial class PackageClient : IDisposable
{
    /// <summary>(Immutable) The package registry uris.</summary>
    private static readonly Uri[] PackageRegistryUris =
    {
        new("http://packages.fhir.org/"),
        new("http://packages2.fhir.org/packages/"),
    };

    /// <summary>(Immutable) URI of the FHIR published server.</summary>
    private static readonly Uri FhirPublishedUri = new("http://hl7.org/fhir/");

    /// <summary>(Immutable) URI of the FHIR CI server.</summary>
    private static readonly Uri FhirCiUri = new("http://build.fhir.org/");

    /// <summary>The logger.</summary>
    private ILogger _logger;

    /// <summary>True if this service is available.</summary>
    private bool _hasCacheDirectory = false;

    /// <summary>True if is initialized, false if not.</summary>
    private bool _isInitialized = false;

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

    /// <summary>Initializes a new instance of the <see cref="FhirPackageService"/> class.</summary>
    /// <param name="logger">             The logger.</param>
    /// <param name="serverConfiguration">The server configuration.</param>
    public FhirPackageService(
        ILogger<PackageClient> logger,
        string fhirCachePath)
    {
        _logger = logger;
        _singleton = this;

        if (string.IsNullOrEmpty(fhirCachePath))
        {
            _hasCacheDirectory = false;
            return;
        }

        _cacheDirectory = fhirCachePath;
        _hasCacheDirectory = true;
    }

    /// <summary>Gets the current singleton.</summary>
    public static FhirPackageService Current => _singleton;

    /// <summary>Gets the packages by directive.</summary>
    public Dictionary<string, PackageCacheRecord> PackagesByDirective => _packagesByDirective;

    /// <summary>Gets a value indicating whether this object is available.</summary>
    public bool IsConfigured => _hasCacheDirectory;

    /// <summary>Gets a value indicating whether the package service is ready.</summary>
    public bool IsReady => _isInitialized;

    /// <summary>The completed requests.</summary>
    private HashSet<string> _processed = new();

    /// <summary>Initializes this object.</summary>
    public void Init()
    {
        if (_isInitialized)
        {
            return;
        }

        if (!_hasCacheDirectory)
        {
            _logger.LogInformation("Disabling FhirPackageService, --fhir-package-cache set to empty.");
            return;
        }

        _logger.LogInformation($"Initializing FhirPackageService with cache: {_cacheDirectory}");
        _isInitialized = true;

        _cachePackageDirectory = Path.Combine(_cacheDirectory, "packages");
        _iniFilePath = Path.Combine(_cachePackageDirectory, "packages.ini");

        if (!Directory.Exists(_cacheDirectory))
        {
            Directory.CreateDirectory(_cacheDirectory);
        }

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

    /// <summary>Triggered when the application host is ready to start the service.</summary>
    /// <param name="cancellationToken">Indicates that the start process has been aborted.</param>
    /// <returns>An asynchronous result.</returns>
    Task IHostedService.StartAsync(CancellationToken cancellationToken)
    {
        if (!_hasCacheDirectory)
        {
            _logger.LogInformation("Disabling FhirPackageService, --fhir-package-cache set to empty.");
            return Task.CompletedTask;
        }

        _logger.LogInformation($"Starting FhirPackageService...");

        Init();

        return Task.CompletedTask;
    }

    /// <summary>Triggered when the application host is performing a graceful shutdown.</summary>
    /// <param name="cancellationToken">Indicates that the shutdown process should no longer be
    ///  graceful.</param>
    /// <returns>An asynchronous result.</returns>
    Task IHostedService.StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    /// <summary>A package cache entry.</summary>
    /// <param name="fhirVersion">        The FHIR version.</param>
    /// <param name="directory">          Pathname of the directory.</param>
    /// <param name="resolvedDirective">  The resolved directive.</param>
    /// <param name="resolvedName">       Name of the resolved.</param>
    /// <param name="resolvedVersion">    The resolved version.</param>
    /// <param name="umbrellaPackageName">Name of the umbrella package.</param>
    public record struct PackageCacheEntry(
        FhirSequenceEnum fhirVersion,
        string directory,
        string resolvedDirective,
        string name,
        string version,
        string umbrellaPackageName);

    /// <summary>Attempts to find locally or download a given package.</summary>
    /// <param name="directive">        The directive.</param>
    /// <param name="branchName">       Name of the branch.</param>
    /// <param name="directory">        [out] Pathname of the directory.</param>
    /// <param name="fhirVersion">      [out] The FHIR version.</param>
    /// <param name="resolvedDirective">[out] The branch directive.</param>
    /// <param name="offlineMode">      True to enable offline mode, false to disable it.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public bool FindOrDownload(
        string directive,
        string branchName,
        out IEnumerable<PackageCacheEntry> packages,
        bool offlineMode = false)
    {
        if (string.IsNullOrEmpty(branchName))
        {
            _logger.LogInformation($"FhirPackageService <<< attempting to load: {directive}");
        }
        else
        {
            _logger.LogInformation($"FhirPackageService <<< attempting to load branch: {branchName}");
        }

        if (!_hasCacheDirectory)
        {
            _logger.LogInformation($"FhirPackageService <<< Package service is unavailable, package will NOT be loaded!");
            packages = Enumerable.Empty<PackageCacheEntry>();
            return false;
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
        FhirSequenceEnum fhirVersion;
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

            // resolve dev (local only) version or fail
            if (string.IsNullOrEmpty(name) ||
                (!HasCachedVersion(name, version, out directory)))
            {
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

            packageList.Add(new()
            {
                fhirVersion = SequenceForVersion(_packagesByDirective[directive].Details.FhirVersion),
                directory = directory,
                resolvedDirective = directive,
                name = name,
                version = directive.Contains('#') ? directive.Split('#')[1] : version,
                umbrellaPackageName = name,
            });

            packages = packageList;
            return true;
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

        Dictionary<FhirSequenceEnum, string> sequencesToTest = new();

        if (lastComponent.TryFhirEnum(out FhirSequenceEnum seq))
        {
            sequencesToTest.Add(seq, string.Empty);
        }
        else
        {
            sequencesToTest = Enum.GetValues(typeof(FhirSequenceEnum))
                .Cast<FhirSequenceEnum>()
                .ToDictionary(x => x, x => LiteralForSequence(x).ToLowerInvariant());
        }

        bool foundLocally = false;

        // want to check for fhir-version named packages
        foreach ((FhirSequenceEnum sequence, string trailer) in sequencesToTest)
        {
            if ((sequence == FhirSequenceEnum.DSTU2) ||
                (sequence == FhirSequenceEnum.STU3))
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
        out FhirSequenceEnum fhirVersion,
        out string resolvedDirective)
    {
        foreach (Uri registryUri in PackageRegistryUris)
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
        fhirVersion = FhirSequenceEnum.Unknown;
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
                PackageLoadStateEnum.NotLoaded,
                name,
                npmDetails.Version,
                SequenceForVersion(npmDetails.FhirVersion),
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
        if (!_hasCacheDirectory) { return; }

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
        out FhirSequenceEnum fhirVersion,
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
                fhirVersion = FhirSequenceEnum.Unknown;
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
                    fhirVersion = FhirSequenceEnum.Unknown;
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

        fhirVersion = FhirSequenceEnum.Unknown;
        resolvedDirective = string.Empty;
        return false;
    }

    /// <summary>Package is FHIR core.</summary>
    /// <param name="packageName">Name of the package.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public static bool PackageIsFhirCore(string packageName)
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
    public bool TryDownloadCoreViaCI(
        string name,
        string branchName,
        out string directory,
        out FhirSequenceEnum fhirVersion,
        out string resolvedDirective)
    {
        branchName = GetCoreBranchFromInput(branchName);

        Uri branchUri;

        switch (branchName.ToLowerInvariant())
        {
            case "master":
            case "main":
                branchUri = FhirCiUri;
                break;

            case null:
            case "":
                if (name.Contains("r4b", StringComparison.OrdinalIgnoreCase))
                {
                    branchUri = new Uri(FhirCiUri, $"branches/R4B/");
                }
                else
                {
                    branchUri = FhirCiUri;
                }

                break;

            default:
                branchUri = new Uri(FhirCiUri, $"branches/{branchName}/");
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
                    fhirVersion = SequenceForVersion(ciFhirVersion);
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
    public bool TryDownloadGuideViaCI(
        string branchName,
        out string name,
        out string directory,
        out FhirSequenceEnum fhirVersion,
        out string resolvedDirective)
    {
        branchName = GetIgBranchFromInput(branchName);

        try
        {
            Uri versionInfoUri = new Uri(FhirCiUri, $"ig/{branchName}/package.manifest.json");
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
                    fhirVersion = SequenceForVersion(cachedNpm.FhirVersion);
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
            fhirVersion = FhirSequenceEnum.Unknown;
            resolvedDirective = string.Empty;
            return false;
        }

        Uri uri = new Uri(FhirCiUri, $"ig/{branchName}/package.tgz");

        return TryDownloadAndExtract(uri, directory, $"{name}#current", out fhirVersion, out resolvedDirective);
    }

    /// <summary>
    /// Attempts to get guide ci package details the NpmPackageDetails from the given string.
    /// </summary>
    /// <param name="branchName">Name of the branch.</param>
    /// <param name="details">   [out] The details.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public bool TryGetGuideCiPackageDetails(string branchName, out FhirNpmPackageDetails details)
    {
        branchName = GetIgBranchFromInput(branchName);

        try
        {
            Uri versionInfoUri = new Uri(FhirCiUri, $"ig/{branchName}/package.manifest.json");

            string contents = _httpClient.GetStringAsync(versionInfoUri).Result;

            details = FhirNpmPackageDetails.Parse(contents);

            if (details.Url == null)
            {
                details.Url = new Uri(FhirCiUri, $"ig/{branchName}").ToString();
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

    /// <summary>Sequence for version.</summary>
    /// <param name="version">[out] The version string (e.g., 4.0.1).</param>
    /// <returns>A FhirSequenceEnum.</returns>
    public static FhirSequenceEnum SequenceForVersion(string version)
    {
        if (string.IsNullOrEmpty(version))
        {
            return FhirSequenceEnum.Unknown;
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
        switch (val.ToUpperInvariant())
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

        return FhirSequenceEnum.Unknown;
    }

    /// <summary>Package base for sequence.</summary>
    /// <param name="seq">The sequence.</param>
    /// <returns>A string.</returns>
    public static string PackageBaseForSequence(FhirSequenceEnum seq) => seq switch
    {
        FhirSequenceEnum.R4B => "hl7.fhir.r4b",
        _ => $"hl7.fhir.r{(int)seq}",
    };

    /// <summary>Literal for sequence.</summary>
    /// <param name="seq">The sequence.</param>
    /// <returns>A string.</returns>
    public static string LiteralForSequence(FhirSequenceEnum seq) => seq.ToLiteral();

    /// <summary>Attempts to get core ci package details.</summary>
    /// <param name="branchName">Name of the branch.</param>
    /// <param name="details">   [out] The details.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public bool TryGetCoreCiPackageDetails(
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
                branchUri = FhirCiUri;
                break;

            case "r4b":
                branchUri = new Uri(FhirCiUri, $"branches/R4B/");
                break;

            default:
                branchUri = new Uri(FhirCiUri, $"branches/{branchName}/");
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

            FhirSequenceEnum sequence = SequenceForVersion(version);

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
    public bool TryDownloadGuideViaCI(
        string name,
        string branchName,
        out string directory,
        out FhirSequenceEnum fhirVersion,
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

                Uri versionInfoUri = new Uri(FhirCiUri, $"ig/{branchName}/package.manifest.json");

                string contents = _httpClient.GetStringAsync(versionInfoUri).Result;

                FhirNpmPackageDetails ciNpm = FhirNpmPackageDetails.Parse(contents);

                if (cachedNpm.BuildDate.CompareTo(ciNpm.BuildDate) <= 0)
                {
                    fhirVersion = SequenceForVersion(ciNpm.FhirVersion);
                    resolvedDirective = $"{ciNpm.Name}#{ciNpm.Version}";
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"TryDownloadCoreViaCI <<< failed to compare local to CI, forcing download ({ex.Message})");
                fhirVersion = FhirSequenceEnum.Unknown;
                resolvedDirective = string.Empty;
                return false;
            }
        }

        Uri uri = new Uri(FhirCiUri, $"ig/{branchName}/package.tgz");

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
    public static bool TryGetRelativeBaseForVersion(string version, out string relative)
    {
        // versions with a dash are not promoted to their version name root
        if (version.Contains('-'))
        {
            relative = version;
            return true;
        }

        FhirSequenceEnum sequence = SequenceForVersion(version);

        if (sequence == FhirSequenceEnum.Unknown)
        {
            relative = string.Empty;
            return false;
        }

        // major releases are promoted to their version name root
        relative = LiteralForSequence(sequence);
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
        out FhirSequenceEnum fhirVersion,
        out string resolvedDirective)
    {
        if (!TryGetRelativeBaseForVersion(version, out string relative))
        {
            directory = string.Empty;
            fhirVersion = FhirSequenceEnum.Unknown;
            resolvedDirective = string.Empty;
            return false;
        }

        string directive = name + "#" + version;
        directory = Path.Combine(_cachePackageDirectory, directive);

        // most publication versions are named with correct package information
        Uri uri = new Uri(FhirPublishedUri, $"{relative}/{name}.tgz");
        if (TryDownloadAndExtract(uri, directory, directive, out fhirVersion, out resolvedDirective))
        {
            UpdatePackageCacheIndex(directive, directory);
            return true;
        }

        // some ballot versions are published directly as CI versions
        uri = new Uri(FhirPublishedUri, $"{relative}/package.tgz");
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

    /// <summary>Gets the cached packages in this collection.</summary>
    /// <returns>
    /// An enumerator that allows foreach to be used to process the cached packages in this
    /// collection.
    /// </returns>
    public IEnumerable<FhirNpmPackageDetails> GetCachedPackages()
    {
        return _packagesByDirective.Select(kvp => kvp.Value.Details).ToArray();
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
    public bool TryGetPackageManifests(string packageName, out IEnumerable<RegistryPackageManifest> manifests)
    {
        List<RegistryPackageManifest> manifestList = new();

        packageName = GetPackageNameFromInput(packageName);

        foreach (Uri registryUri in PackageRegistryUris)
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
    public void UpdatePackageState(
        string directive,
        string resolvedName,
        string resolvedVersion,
        PackageLoadStateEnum toState)
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
    public bool TryGetPackageState(string directive, out PackageLoadStateEnum state)
    {
        if (!_packagesByDirective.ContainsKey(directive))
        {
            state = PackageLoadStateEnum.Unknown;
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
                PackageLoadStateEnum.NotLoaded,
                name,
                version,
                SequenceForVersion(versionInfo.FhirVersion),
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
    /// Releases the unmanaged resources used by the <see cref="FhirPackageService"/>
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
