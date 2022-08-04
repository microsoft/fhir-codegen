// <copyright file="FhirManager.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.IO;
using Microsoft.Health.Fhir.SpecManager.PackageManager;

namespace Microsoft.Health.Fhir.SpecManager.Manager;

/// <summary>FHIR version manager.</summary>
public class FhirManager : IDisposable
{
    /// <summary>True to disposed value.</summary>
    private bool _disposedValue;

    /// <summary>The singleton.</summary>
    private static FhirManager _singleton;

    /// <summary>The loaded information by version.</summary>
    private Dictionary<string, FhirVersionInfo> _loadedInfoByDirective;

    /// <summary>List of types of the directive packages.</summary>
    private Dictionary<string, FhirPackageCommon.FhirPackageTypeEnum> _directivePackageTypes;

    /// <summary>The packages loaded by request.</summary>
    private HashSet<string> _loadedByRequest;

    /// <summary>The packages loaded as dependency.</summary>
    private HashSet<string> _loadedAsDependency;

    /// <summary>The load directive to version.</summary>
    private Dictionary<string, string> _loadDirectiveToVersion;

    /// <summary>Manager for load lock object.</summary>
    private object _managerLoadLockObject;

    /// <summary>
    /// Prevents a default instance of the <see cref="FhirManager"/> class from being created.
    /// </summary>
    private FhirManager()
    {
        _disposedValue = false;
        _loadedInfoByDirective = new();
        _loadedByRequest = new();
        _loadedAsDependency = new();
        _directivePackageTypes = new();
        _loadDirectiveToVersion = new();
        _managerLoadLockObject = new();
    }

    /// <summary>Gets the current.</summary>
    public static FhirManager Current => _singleton;

    /// <summary>Gets the information by directive.</summary>
    public Dictionary<string, FhirVersionInfo> InfoByDirective => _loadedInfoByDirective;

    /// <summary>Initializes the FhirManager.</summary>
    public static void Init()
    {
        Console.WriteLine("Starting FhirManager...");

        _singleton = new FhirManager();
    }

    /// <summary>Gets the loaded core packages in this collection.</summary>
    /// <returns>
    /// An enumerator that allows foreach to be used to process the loaded core packages in this
    /// collection.
    /// </returns>
    public List<FhirVersionInfo> GetLoadedCorePackages()
    {
        List<FhirVersionInfo> packages = new();

        foreach ((string directive, FhirPackageCommon.FhirPackageTypeEnum type) in _directivePackageTypes)
        {
            if (type != FhirPackageCommon.FhirPackageTypeEnum.Core)
            {
                continue;
            }

            if (_loadedInfoByDirective.ContainsKey(directive))
            {
                packages.Add(_loadedInfoByDirective[directive]);
            }
            else if (directive.Contains(".core", StringComparison.OrdinalIgnoreCase))
            {
                string t = directive.Replace(".core", string.Empty);
                if (_loadedInfoByDirective.ContainsKey(t))
                {
                    packages.Add(_loadedInfoByDirective[t]);
                }
            }
        }

        return packages;
    }

    /// <summary>Gets the loaded core packages in this collection.</summary>
    /// <returns>
    /// An enumerator that allows foreach to be used to process the loaded core packages in this
    /// collection.
    /// </returns>
    public List<FhirVersionInfo> GetLoadedPackages()
    {
        List<FhirVersionInfo> packages = new();

        foreach ((string directive, FhirPackageCommon.FhirPackageTypeEnum type) in _directivePackageTypes)
        {
            packages.Add(_loadedInfoByDirective[directive]);
        }

        return packages;
    }

    /// <summary>Attempts to get loaded a FhirVersionInfo from the given string.</summary>
    /// <param name="directive">The directive.</param>
    /// <param name="info">     [out] The information.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public bool TryGetLoaded(string directive, out FhirVersionInfo info)
    {
        if (!_loadedInfoByDirective.ContainsKey(directive))
        {
            info = null;
            return false;
        }

        info = _loadedInfoByDirective[directive];
        return true;
    }

    /// <summary>Query if 'directive' is loaded.</summary>
    /// <param name="directive">The directive.</param>
    /// <returns>True if loaded, false if not.</returns>
    public bool IsLoaded(string directive)
    {
        return _directivePackageTypes.ContainsKey(directive) || _loadDirectiveToVersion.ContainsKey(directive);
    }

    /// <summary>Unload package.</summary>
    /// <param name="directive">The directive.</param>
    public void UnloadPackage(string directive)
    {
        if (_loadedInfoByDirective.ContainsKey(directive))
        {
            _loadedInfoByDirective.Remove(directive);
        }

        if (_directivePackageTypes.ContainsKey(directive))
        {
            _directivePackageTypes.Remove(directive);
        }

        if (_loadDirectiveToVersion.ContainsKey(directive))
        {
            _loadDirectiveToVersion.Remove(directive);
        }
    }

    /// <summary>Loads FHIR core packages.</summary>
    /// <param name="sequence">              The sequence.</param>
    /// <param name="version">               The specific version of FHIR to load, or 'latest' for
    ///  highest known version.</param>
    /// <param name="offlineMode">           True to allow, false to suppress the download.</param>
    /// <param name="officialExpansionsOnly">True to official expansions only.</param>
    /// <param name="ciBranch">              (Optional) The CI branch.</param>
    /// <returns>The FHIR core.</returns>
    private FhirVersionInfo LoadFhirCore(
        FhirPackageCommon.FhirSequenceEnum sequence,
        string version,
        bool offlineMode,
        bool officialExpansionsOnly,
        string ciBranch = "")
    {
        string packageBase = FhirPackageCommon.PackageBaseForRelease(sequence);

        if (!string.IsNullOrEmpty(ciBranch))
        {
            version = "current";
        }

        string corePackage = packageBase + ".core#" + version;
        string expansionsPackage = packageBase + ".expansions#" + version;

        if (!FhirCacheService.Current.FindOrDownload(
            corePackage,
            out string corePackageDirectory,
            offlineMode,
            ciBranch))
        {
            throw new Exception($"Failed to retrieve {corePackage}");
        }

        if (!FhirCacheService.Current.FindOrDownload(
            expansionsPackage,
            out string expansionsDirectory,
            offlineMode,
            ciBranch))
        {
            throw new Exception($"Failed to retrieve {expansionsPackage}");
        }

        if (_loadedInfoByDirective.ContainsKey(corePackage))
        {
            return _loadedInfoByDirective[corePackage];
        }

        // grab the correct basic version info
        FhirVersionInfo info = new FhirVersionInfo(corePackageDirectory);

        // load the package
        lock (_managerLoadLockObject)
        {
            FhirSpecificationLoader.Load(
                info,
                corePackageDirectory,
                expansionsDirectory,
                officialExpansionsOnly);
        }

        return info;
    }

    /// <summary>Track loaded package.</summary>
    /// <param name="loadDirective">The load directive.</param>
    /// <param name="info">         The information.</param>
    /// <param name="wasRequested"> True if requested.</param>
    /// <param name="isCore">       True if is core, false if not.</param>
    private void TrackLoadedPackage(string loadDirective, FhirVersionInfo info, bool wasRequested, bool isCore)
    {
        string directive = info.PackageName + "#" + info.VersionString;

        if (_loadedInfoByDirective.ContainsKey(loadDirective))
        {
            Console.WriteLine($"WARNING: Attempt to load already loaded package: {loadDirective}");
            return;
        }

        _loadedInfoByDirective.Add(loadDirective, info);

        if (wasRequested)
        {
            _loadedByRequest.Add(loadDirective);
            _loadDirectiveToVersion.Add(loadDirective, directive);
        }
        else
        {
            _loadedAsDependency.Add(loadDirective);
        }

        if (isCore)
        {
            string coreDirective;

            if (loadDirective.Contains('#'))
            {
                coreDirective = info.PackageName + "#" + loadDirective.Split('#')[1];
            }
            else
            {
                coreDirective = loadDirective;
            }

            _directivePackageTypes.Add(coreDirective, FhirPackageCommon.FhirPackageTypeEnum.Core);
            _directivePackageTypes.Add(coreDirective.Replace(".core", ".expansions"), FhirPackageCommon.FhirPackageTypeEnum.Core);

            FhirCacheService.Current.UpdatePackageState(
                coreDirective,
                info.PackageName,
                info.VersionString,
                PackageLoadStateEnum.Loaded);

            FhirCacheService.Current.UpdatePackageState(
                coreDirective.Replace(".core", ".expansions"),
                info.PackageName,
                info.VersionString,
                PackageLoadStateEnum.Loaded);
        }
        else
        {
            _directivePackageTypes.Add(loadDirective, FhirPackageCommon.FhirPackageTypeEnum.IG);

            FhirCacheService.Current.UpdatePackageState(
                loadDirective,
                info.PackageName,
                info.VersionString,
                PackageLoadStateEnum.Loaded);
        }
    }

    /// <summary>Loads the packages.</summary>
    /// <param name="packageDirectives">     The package directives.</param>
    /// <param name="offlineMode">           True to allow, false to suppress the download.</param>
    /// <param name="officialExpansionsOnly">True to official expansions only.</param>
    /// <param name="loadDependencies">      True to load dependencies.</param>
    /// <param name="skipFhirCore">          True to skip loading FHIR core packages.</param>
    /// <param name="ciBranch">              The CI branch.</param>
    /// <param name="failedPackages">        [out] The failed packages.</param>
    /// <param name="areDependencies">       (Optional) True if are dependencies.</param>
    public void LoadPackages(
        IEnumerable<string> packageDirectives,
        bool offlineMode,
        bool officialExpansionsOnly,
        bool loadDependencies,
        bool skipFhirCore,
        string ciBranch,
        out List<string> failedPackages,
        bool areDependencies = false)
    {
        failedPackages = new();

        foreach (string directive in packageDirectives)
        {
            if (_loadedInfoByDirective.ContainsKey(directive))
            {
                continue;
            }

            FhirCacheService.Current.UpdatePackageState(directive, string.Empty, string.Empty, PackageLoadStateEnum.InProgress);

            FhirVersionInfo info;
            bool isCore = FhirPackageCommon.TryGetReleaseByPackage(
                directive,
                out FhirPackageCommon.FhirSequenceEnum release);

            if (isCore)
            {
                if (skipFhirCore)
                {
                    continue;
                }

                string[] components = directive.Split('#');

                info = LoadFhirCore(
                    release,
                    components[1],
                    offlineMode,
                    officialExpansionsOnly,
                    ciBranch);
            }
            else
            {
                FhirCacheService.Current.FindOrDownload(
                    directive,
                    out string directory,
                    offlineMode,
                    ciBranch);

                string resolvedDirective = Path.GetFileName(directory);

                if (_loadedInfoByDirective.ContainsKey(resolvedDirective))
                {
                    continue;
                }

                lock (_managerLoadLockObject)
                {
                    FhirPackageLoader.Load(directive, directory, out info);
                }
            }

            if (info == null)
            {
                failedPackages.Add(directive);
            }
            else
            {
                Console.WriteLine($"Loaded {directive} as {info.PackageName} version {info.VersionString}");
                TrackLoadedPackage(directive, info, true, isCore);

                if (loadDependencies &&
                    (info.PackageDetails != null) &&
                    (info.PackageDetails.Dependencies != null) &&
                    info.PackageDetails.Dependencies.Any())
                {
                    List<string> dependencyDirectives = new();

                    foreach ((string name, string version) in info.PackageDetails.Dependencies)
                    {
                        dependencyDirectives.Add(name + "#" + version);
                    }

                    LoadPackages(
                        dependencyDirectives,
                        offlineMode,
                        officialExpansionsOnly,
                        loadDependencies,
                        skipFhirCore,
                        ciBranch,
                        out List<string> failedDependencies,
                        areDependencies: true);

                    failedPackages.AddRange(failedDependencies);
                }
            }
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
                _loadedInfoByDirective.Clear();
            }

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
}
