// <copyright file="FhirCacheService.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.IO;
using System.Net.Http;
using IniParser.Model;
using Microsoft.Health.Fhir.SpecManager.Manager;
using static Microsoft.Health.Fhir.SpecManager.Manager.FhirPackageCommon;

namespace Microsoft.Health.Fhir.SpecManager.PackageManager;

/// <summary>Manager for package caches.</summary>
public class FhirCacheService : IDisposable
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

    /// <summary>Pathname of the cache directory.</summary>
    private string _cacheDirectory;

    /// <summary>Pathname of the cache package directory.</summary>
    private string _cachePackageDirectory;

    /// <summary>Full pathname of the initialize file.</summary>
    private string _iniFilePath;

    /// <summary>The HTTP client.</summary>
    private HttpClient _httpClient;

    /// <summary>True to disposed value.</summary>
    private bool _disposedValue;

    /// <summary>The singleton.</summary>
    private static FhirCacheService _singleton;

    /// <summary>The package records, by directive.</summary>
    private Dictionary<string, PackageCacheRecord> _packagesByDirective;

    /// <summary>Package versions, by package name.</summary>
    private Dictionary<string, List<string>> _versionsByName;

    /// <summary>
    /// Prevents a default instance of the <see cref="FhirCacheService"/> class from being created.
    /// </summary>
    private FhirCacheService()
    {
    }

    /// <summary>Initializes a new instance of the <see cref="FhirCacheService"/> class.</summary>
    /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
    /// <param name="cacheDirectory">Pathname of the cache directory.</param>
    private FhirCacheService(string cacheDirectory)
    {
        if (string.IsNullOrEmpty(cacheDirectory))
        {
            throw new ArgumentNullException(nameof(cacheDirectory));
        }

        _cacheDirectory = cacheDirectory;
        _cachePackageDirectory = Path.Combine(cacheDirectory, "packages");
        _iniFilePath = Path.Combine(_cachePackageDirectory, "packages.ini");
        _packagesByDirective = new();
        _versionsByName = new();
        _httpClient = new();

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

    /// <summary>Occurs when On Changed.</summary>
    public event EventHandler<EventArgs> OnChanged;

    /// <summary>Gets the current singleton.</summary>
    public static FhirCacheService Current => _singleton;

    /// <summary>Gets the packages by directive.</summary>
    public Dictionary<string, PackageCacheRecord> PackagesByDirective => _packagesByDirective;

    /// <summary>Initializes the FhirCacheService.</summary>
    /// <param name="cacheDirectory">Pathname of the cache directory.</param>
    public static void Init(string cacheDirectory)
    {
        Console.WriteLine("Starting FhirCacheService...");
        Console.WriteLine($" <<< using cache directory: {cacheDirectory}");

        _singleton = new(cacheDirectory);
    }

    /// <summary>Searches for the first or download.</summary>
    /// <param name="directive">  The directive.</param>
    /// <param name="directory">  [out] Pathname of the directory.</param>
    /// <param name="offlineMode">True to enable offline mode, false to disable it.</param>
    /// <param name="branchName"> (Optional) Name of the branch.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public bool FindOrDownload(
        string directive,
        out string directory,
        bool offlineMode,
        string branchName = "")
    {
        string name;
        string version;
        bool isLocal = false;
        directory = string.Empty;

        if (directive.Contains('#'))
        {
            string[] components = directive.Split('#').Select(x => x.Trim()).ToArray();
            name = components[0];
            version = components[1];
        }
        else
        {
            name = directive;
            version = string.Empty;
        }

        name = GetPackageNameFromInput(name);

        if (version.Equals("dev", StringComparison.OrdinalIgnoreCase))
        {
            if (TryDownloadCoreViaCI(name, branchName, out directory))
            {
                return true;
            }

            // resolve dev (local only) version or fail
            if (!HasCachedVersion(name, version, out directory))
            {
                Console.WriteLine($"FindOrDownload <<< package: {directive} not found in cache!");
                directory = string.Empty;
                return false;
            }

            return true;
        }

        if (version.Equals("current", StringComparison.OrdinalIgnoreCase))
        {
            // resolve CI build (online or local) or fail
            return TryDownloadViaCI(name, branchName, out directory);
        }

        if (string.IsNullOrEmpty(version) || version.Equals("latest", StringComparison.OrdinalIgnoreCase))
        {
            TryGetHighestVersion(name, offlineMode, out version, out isLocal, out directory);
        }

        if (isLocal || HasCachedVersion(name, version, out directory))
        {
            return true;
        }

        if ((!isLocal) && offlineMode)
        {
            Console.WriteLine($"FindOrDownload <<< cannot resolve requested package while offline: {name}#{version}");
            directory = string.Empty;
            return false;
        }

        if (TryDownloadViaRegistry(name, version, out directory))
        {
            return true;
        }

        if (TryDownloadCoreViaPublication(name, version, out directory))
        {
            return true;
        }

        Console.WriteLine($"FindOrDownload <<< unable to resolve a directive: {directive}");
        directory = string.Empty;
        return false;
    }

    /// <summary>Attempts to download via registry a string from the given string.</summary>
    /// <param name="name">   The name.</param>
    /// <param name="version">[out] The version string (e.g., 4.0.1).</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    private bool TryDownloadViaRegistry(
        string name,
        string version,
        out string directory)
    {
        foreach (Uri registryUri in PackageRegistryUris)
        {
            Uri uri = new Uri(registryUri, $"{name}/{version}");
            directory = Path.Combine(_cachePackageDirectory, $"{name}#{version}");

            string directive = name + "#" + version;

            if (TryDownloadAndExtract(uri, directory, directive))
            {
                UpdatePackageCacheIndex(directive, directory);

                return true;
            }
        }

        directory = string.Empty;
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
        IniData iniData = null)
    {
        string[] components = directive.Split('#');
        string name = components[0];
        string directiveVersion = components.Length > 1 ? components[1] : "current";

        if (!Directory.Exists(directory))
        {
            if (iniData == null)
            {
                IniParser.FileIniDataParser parser = new();

                IniData data = parser.ReadFile(_iniFilePath);

                if (data["packages"].ContainsKey(directive))
                {
                    data["packages"].RemoveKey(directive);
                }

                if (data["package-sizes"].ContainsKey(directive))
                {
                    data["package-sizes"].RemoveKey(directive);
                }

                SaveIniData(_iniFilePath, data);
            }
            else
            {
                if (iniData["packages"].ContainsKey(directive))
                {
                    iniData["packages"].RemoveKey(directive);
                }

                if (iniData["package-sizes"].ContainsKey(directive))
                {
                    iniData["package-sizes"].RemoveKey(directive);
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
            NpmPackageDetails npmDetails = NpmPackageDetails.Load(npmJson);
            if (!string.IsNullOrEmpty(npmDetails.BuildDate))
            {
                packageDate = npmDetails.BuildDate;
            }

            PackageCacheRecord record = new(
                directive,
                PackageLoadStateEnum.NotLoaded,
                name,
                npmDetails.Version,
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
            IniParser.FileIniDataParser parser = new();

            IniData data = parser.ReadFile(_iniFilePath);

            if (data["packages"].ContainsKey(directive))
            {
                data["packages"].RemoveKey(directive);
            }

            data["packages"].AddKey(directive, packageDate);

            if (data["package-sizes"].ContainsKey(directive))
            {
                data["package-sizes"].RemoveKey(directive);
            }

            data["package-sizes"].AddKey(directive, size.ToString());

            SaveIniData(_iniFilePath, data);
        }
        else
        {
            if (iniData["packages"].ContainsKey(directive))
            {
                iniData["packages"].RemoveKey(directive);
            }

            iniData["packages"].AddKey(directive, packageDate);

            if (iniData["package-sizes"].ContainsKey(directive))
            {
                iniData["package-sizes"].RemoveKey(directive);
            }

            iniData["package-sizes"].AddKey(directive, size.ToString());
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

            //if (_packagesByDirective.ContainsKey(packageDirective))
            //{
            //    _packagesByDirective.Remove(packageDirective);
            //}

            //string[] components = packageDirective.Split('#');
            //string name = packageDirective.Split('#')[0];
            //string version = (components.Length > 1) ? components[1] : string.Empty;

            //if ((!string.IsNullOrEmpty(version)) &&
            //    _versionsByName.ContainsKey(name))
            //{
            //    _versionsByName[name] = _versionsByName[name].Where((v) => !v.Equals(version)).ToList();
            //}
        }
        catch (Exception ex)
        {
            Console.WriteLine($"DeletePackage <<< caught exception: {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($" <<< {ex.InnerException.Message}");
            }
        }
    }

    /// <summary>Attempts to download and extract a string from the given URI.</summary>
    /// <param name="uri">             URI of the resource.</param>
    /// <param name="directory">       [out] Pathname of the directory.</param>
    /// <param name="packageDirective">The package directive.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    private bool TryDownloadAndExtract(
        Uri uri,
        string directory,
        string packageDirective)
    {
        try
        {
            using (Stream fileStream = _httpClient.GetStreamAsync(uri).Result)
            using (Stream gzipStream = new ICSharpCode.SharpZipLib.GZip.GZipInputStream(fileStream))
            using (ICSharpCode.SharpZipLib.Tar.TarArchive tar =
                    ICSharpCode.SharpZipLib.Tar.TarArchive.CreateInputTarArchive(gzipStream, Encoding.ASCII))
            {
                // make sure our destination directory exists
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // extract
                tar.ExtractContents(directory, false);
            }

            UpdatePackageCacheIndex(packageDirective, directory);

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"TryDownloadAndExtract <<< exception downloading: {uri}, {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($" <<< inner: {ex.InnerException.Message}");
            }
        }

        return false;
    }

    /// <summary>Attempts to download via ci a string from the given string.</summary>
    /// <param name="name">      The name.</param>
    /// <param name="branchName">Name of the branch.</param>
    /// <param name="directory"> [out] Pathname of the directory.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public bool TryDownloadViaCI(
        string name,
        string branchName,
        out string directory)
    {
        string directive = name + "#current";

        if (FhirPackageCommon.PackageIsFhirCore(name))
        {
            if (TryDownloadCoreViaCI(name, branchName, out directory))
            {
                UpdatePackageCacheIndex(directive, directory);

                return true;
            }

            return false;
        }

        if (TryDownloadGuideViaCI(name, branchName, out directory))
        {
            UpdatePackageCacheIndex(directive, directory);

            return true;
        }

        return false;
    }

    /// <summary>Attempts to download core via ci a string from the given string.</summary>
    /// <param name="name">      The name.</param>
    /// <param name="branchName">Name of the branch.</param>
    /// <param name="directory"> [out] Pathname of the directory.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public bool TryDownloadCoreViaCI(
        string name,
        string branchName,
        out string directory)
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
                NpmPackageDetails cachedNpm = NpmPackageDetails.Load(localNpmFilename);

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
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"TryDownloadCoreViaCI <<< failed to compare local to CI, forcing download ({ex.Message})");
            }
        }

        Uri uri = new Uri(branchUri, $"{name}.tgz");

        return TryDownloadAndExtract(uri, directory, $"{name}#current");
    }

    /// <summary>Attempts to download guide via ci a string from the given string.</summary>
    /// <param name="branchName">Name of the branch.</param>
    /// <param name="name">      [out] The name.</param>
    /// <param name="directory"> [out] Pathname of the directory.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public bool TryDownloadGuideViaCI(
        string branchName,
        out string name,
        out string directory)
    {
        branchName = GetIgBranchFromInput(branchName);

        try
        {
            Uri versionInfoUri = new Uri(FhirCiUri, $"ig/{branchName}/package.manifest.json");
            string contents = _httpClient.GetStringAsync(versionInfoUri).Result;

            NpmPackageDetails ciNpm = NpmPackageDetails.Parse(contents);

            name = ciNpm.Name;
            directory = Path.Combine(_cachePackageDirectory, $"{ciNpm.Name}#current");

            string localNpmFilename = Path.Combine(directory, "package", "package.json");

            if (File.Exists(localNpmFilename))
            {
                NpmPackageDetails cachedNpm = NpmPackageDetails.Load(localNpmFilename);

                if (cachedNpm.BuildDate.CompareTo(ciNpm.BuildDate) <= 0)
                {
                    return true;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"TryDownloadCoreViaCI <<< failed to compare local to CI, forcing download ({ex.Message})");
            name = string.Empty;
            directory = string.Empty;
            return false;
        }

        Uri uri = new Uri(FhirCiUri, $"ig/{branchName}/package.tgz");

        return TryDownloadAndExtract(uri, directory, $"{name}#current");
    }

    /// <summary>
    /// Attempts to get guide ci package details the NpmPackageDetails from the given string.
    /// </summary>
    /// <param name="branchName">Name of the branch.</param>
    /// <param name="details">   [out] The details.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public bool TryGetGuideCiPackageDetails(string branchName, out NpmPackageDetails details)
    {
        branchName = GetIgBranchFromInput(branchName);

        try
        {
            Uri versionInfoUri = new Uri(FhirCiUri, $"ig/{branchName}/package.manifest.json");

            string contents = _httpClient.GetStringAsync(versionInfoUri).Result;

            details = NpmPackageDetails.Parse(contents);

            if (details.URL == null)
            {
                details.URL = new Uri(FhirCiUri, $"ig/{branchName}");
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
            Console.WriteLine($"TryGetGuideCiPackageDetails <<< failed to find CI IG ({ex.Message})");
        }

        details = null;
        return false;
    }

    /// <summary>Attempts to get core ci package details.</summary>
    /// <param name="branchName">Name of the branch.</param>
    /// <param name="details">   [out] The details.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public bool TryGetCoreCiPackageDetails(
        string branchName,
        out NpmPackageDetails details)
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

            FhirPackageCommon.FhirSequenceEnum sequence = FhirPackageCommon.MajorReleaseForVersion(version);

            string corePackageName = FhirPackageCommon.PackageBaseForRelease(sequence) + ".core";

            details = new NpmPackageDetails()
            {
                Name = corePackageName,
                Version = version,
                BuildDate = buildDate,
                FhirVersionList = new string[1] { version },
                FhirVersions = new string[1] { version },
                PackageType = "core",
                ToolsVersion = 3M,
                URL = branchUri,
                Title = $"FHIR {sequence}: {fhirVersion}",
                Description = $"CI Build from branch {branchName}, current as of: {DateTime.Now}",
                Dependencies = new(),
            };

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"TryDownloadCoreViaCI <<< failed to compare local to CI, forcing download ({ex.Message})");
        }

        details = null;
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
        out string directory)
    {
        directory = Path.Combine(_cachePackageDirectory, $"{name}#current");

        branchName = GetIgBranchFromInput(branchName);

        string localNpmFilename = Path.Combine(directory, "package", "package.json");
        if (File.Exists(localNpmFilename))
        {
            try
            {
                NpmPackageDetails cachedNpm = NpmPackageDetails.Load(localNpmFilename);

                Uri versionInfoUri = new Uri(FhirCiUri, $"ig/{branchName}/package.manifest.json");

                string contents = _httpClient.GetStringAsync(versionInfoUri).Result;

                NpmPackageDetails ciNpm = NpmPackageDetails.Parse(contents);

                if (cachedNpm.BuildDate.CompareTo(ciNpm.BuildDate) <= 0)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"TryDownloadCoreViaCI <<< failed to compare local to CI, forcing download ({ex.Message})");
                return false;
            }
        }

        Uri uri = new Uri(FhirCiUri, $"ig/{branchName}/package.tgz");

        return TryDownloadAndExtract(uri, directory, $"{name}#current");
    }

    /// <summary>Attempts to download core via publication a string from the given string.</summary>
    /// <param name="name">     The name.</param>
    /// <param name="version">  [out] The version string (e.g., 4.0.1).</param>
    /// <param name="directory">[out] Pathname of the directory.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    private bool TryDownloadCoreViaPublication(
        string name,
        string version,
        out string directory)
    {
        if (!FhirPackageCommon.TryGetRelativeBaseForVersion(version, out string relative))
        {
            directory = string.Empty;
            return false;
        }

        string directive = name + "#" + version;
        directory = Path.Combine(_cachePackageDirectory, directive);

        // most publication versions are named with correct package information
        Uri uri = new Uri(FhirPublishedUri, $"{relative}/{name}.tgz");
        if (TryDownloadAndExtract(uri, directory, directive))
        {
            UpdatePackageCacheIndex(directive, directory);
            return true;
        }

        // some ballot versions are published directly as CI versions
        uri = new Uri(FhirPublishedUri, $"{relative}/package.tgz");
        directory = Path.Combine(_cachePackageDirectory, $"hl7.fhir.core#{version}");
        if (TryDownloadAndExtract(uri, directory, directive))
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
    public IEnumerable<NpmPackageDetails> GetCachedPackages()
    {
        return _packagesByDirective.Select(kvp => kvp.Value.Details).ToArray();
    }

    /// <summary>Query if 'name' has cached version.</summary>
    /// <param name="name">     The name.</param>
    /// <param name="version">  [out] The version string (e.g., 4.0.1).</param>
    /// <param name="directory">[out] Pathname of the directory.</param>
    /// <returns>True if cached version, false if not.</returns>
    private bool HasCachedVersion(string name, string version, out string directory)
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

        if (TryGetReleaseByPackage(name, out FhirSequenceEnum seq))
        {
            foreach (PublishedReleaseInformation release in CoreVersions.Where(cv => cv.Major == seq).OrderByDescending(cv => cv.PublicationDate))
            {
                if (string.IsNullOrEmpty(highestOnline))
                {
                    highestOnline = release.Version;
                }

                if (HasCachedVersion(name, release.Version, out directory))
                {
                    highestCached = release.Version;
                    break;
                }
            }

            if (offlineMode)
            {
                version = highestCached;
                isCached = true;
                directory = Path.Combine(_cachePackageDirectory, $"{name}#{version}");
                return true;
            }

            version = highestOnline;
            isCached = highestOnline.Equals(highestCached);
            directory = Path.Combine(_cachePackageDirectory, $"{name}#{version}");
            return true;
        }

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
                    Console.WriteLine(
                        $"GetPackageVersionsAndUrls <<<" +
                        $" Failed to get package info: {response.StatusCode}" +
                        $" {requestUri.AbsoluteUri}");
                    continue;
                }

                string json = response.Content.ReadAsStringAsync().Result;

                RegistryPackageManifest info = RegistryPackageManifest.Parse(json);

                if (!info.Name.Equals(packageName, StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine(
                        $"GetPackageVersionsAndUrls <<<" +
                        $" Package information mismatch: requested {requestUri.AbsoluteUri}" +
                        $" received manifest for {info.Name}");
                    continue;
                }

                if ((info.Versions == null) || (info.Versions.Count == 0))
                {
                    Console.WriteLine(
                        $"GetPackageVersionsAndUrls <<<" +
                        $" Package {requestUri.AbsoluteUri}" +
                        $" contains NO versions");
                    continue;
                }

                manifestList.Add(info);
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"GetPackageVersionsAndUrls <<<" +
                    $" Server {registryUri.AbsoluteUri}" +
                    $" Package {packageName}" +
                    $" threw: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($" <<< {ex.InnerException.Message}");
                }
            }
        }

        if (manifestList.Any())
        {
            manifests = manifestList.AsEnumerable();
            return true;
        }

        manifests = null;
        return false;
    }

    /// <summary>Discover cached packages.</summary>
    private void SynchronizeCache()
    {
        bool modified = false;

        IniParser.FileIniDataParser parser = new();

        IniData data = parser.ReadFile(_iniFilePath);

        if (!data.Sections.ContainsSection("packages"))
        {
            _packagesByDirective.Clear();
            _versionsByName.Clear();
            return;
        }

        List<string> directivesToRemove = new();

        foreach (KeyData line in data["packages"])
        {
            ProcessSync(data, line.KeyName, line.Value, out bool shouldRemove);

            if (shouldRemove)
            {
                directivesToRemove.Add(line.KeyName);
            }
        }

        foreach (string directive in directivesToRemove)
        {
            Console.WriteLine($" <<< removing {directive} from ini");

            if (data["packages"].ContainsKey(directive))
            {
                data["packages"].RemoveKey(directive);
            }

            if (data["package-sizes"].ContainsKey(directive))
            {
                data["package-sizes"].RemoveKey(directive);
            }

            modified = true;
        }

        IEnumerable<string> directories = Directory.EnumerateDirectories(_cachePackageDirectory, "*", SearchOption.TopDirectoryOnly);

        foreach (string directory in directories)
        {
            string directive = Path.GetFileName(directory);

            if (!data["packages"].ContainsKey(directive))
            {
                Console.WriteLine($" <<< adding {directive} to ini");
                UpdatePackageCacheIndex(directive, directory, data);
                modified = true;
                ProcessSync(data, directive, data["packages"][directive], out _);
            }
        }

        if (modified)
        {
            SaveIniData(_iniFilePath, data);
        }

        Console.WriteLine($" << cache contains {_packagesByDirective.Count} packages");
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
            Console.WriteLine($" <<< unknown package directive: {directive}");
            shouldRemove = true;
            return;
        }

        if (!Directory.Exists(Path.Combine(_cachePackageDirectory, directive)))
        {
            Console.WriteLine($"SynchronizeCache <<< removing entry {directive}, directory not found!");
            shouldRemove = true;
            return;
        }

        string[] components = directive.Split('#').Select(x => x.Trim()).ToArray();
        if (components.Length != 2)
        {
            Console.WriteLine($"SynchronizeCache <<< unparseable package directive: {directive}");
            shouldRemove = true;
            return;
        }

        shouldRemove = false;

        string name = components[0];
        string version = components[1];
        long size = -1;
        NpmPackageDetails versionInfo;

        if (data.Sections.ContainsSection("package-sizes") &&
            data["package-sizes"].ContainsKey(directive))
        {
            _ = long.TryParse(data["package-sizes"][directive], out size);
        }

        try
        {
            versionInfo = NpmPackageDetails.Load(Path.Combine(_cachePackageDirectory, directive));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"DiscoverCachedPackages <<< skipping package: {directive} - {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($" <<< inner: {ex.InnerException.Message}");
            }

            return;
        }

        PackageCacheRecord record = new(
                directive,
                PackageLoadStateEnum.NotLoaded,
                name,
                version,
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

        data.Sections.Add(new SectionData("cache"));
        data["cache"].AddKey("version", "3");

        data.Sections.Add(new SectionData("urls"));

        data.Sections.Add(new SectionData("local"));

        data.Sections.Add(new SectionData("packages"));

        data.Sections.Add(new SectionData("package-sizes"));

        SaveIniData(_iniFilePath, data);
    }

    /// <summary>Saves an initialize data.</summary>
    /// <param name="destinationPath">Full pathname of the destination file.</param>
    /// <param name="data">           The data.</param>
    private void SaveIniData(string destinationPath, IniData data)
    {
        IniParser.FileIniDataParser parser = new();

        IniParser.Model.Configuration.IniParserConfiguration config = new()
        {
            NewLineStr = "\r\n",
        };

        IniParser.Model.Formatting.DefaultIniDataFormatter formatter = new(config);

        using (FileStream fs = new FileStream(destinationPath, FileMode.Create))
        using (StreamWriter writer = new StreamWriter(fs))
        {
            parser.WriteData(writer, data, formatter);
        }
    }

    /// <summary>
    /// Releases the unmanaged resources used by the <see cref="FhirCacheService"/>
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
        EventHandler<EventArgs> handler = OnChanged;

        if (handler != null)
        {
            handler(this, new());
        }
    }
}
