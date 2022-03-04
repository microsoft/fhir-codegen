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
    }

    /// <summary>Gets the current.</summary>
    public static FhirManager Current => _singleton;

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

            packages.Add(_loadedInfoByDirective[directive]);
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
        FhirSpecificationLoader.Load(
            info,
            corePackageDirectory,
            expansionsDirectory,
            officialExpansionsOnly);

        return info;
    }

    /// <summary>Track loaded package.</summary>
    /// <param name="info">     The information.</param>
    /// <param name="wasRequested">True if requested.</param>
    /// <param name="isCore">   True if is core, false if not.</param>
    private void TrackLoadedPackage(FhirVersionInfo info, bool wasRequested, bool isCore)
    {
        string directive = info.PackageName + "#" + info.VersionString;

        if (_loadedInfoByDirective.ContainsKey(directive))
        {
            Console.WriteLine($"WARNING: Attempt to load already loaded package: {directive}");
            return;
        }

        _loadedInfoByDirective.Add(directive, info);

        if (wasRequested)
        {
            _loadedByRequest.Add(directive);
        }
        else
        {
            _loadedAsDependency.Add(directive);
        }

        if (isCore)
        {
            _directivePackageTypes.Add(directive, FhirPackageCommon.FhirPackageTypeEnum.Core);
        }
        else
        {
            _directivePackageTypes.Add(directive, FhirPackageCommon.FhirPackageTypeEnum.IG);
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

            FhirVersionInfo info;
            bool isCore = FhirPackageCommon.TryGetReleaseByPackage(directive, out FhirPackageCommon.FhirSequenceEnum release);

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

                FhirPackageLoader.Load(directive, directory, out info);
            }

            if (info == null)
            {
                failedPackages.Add(directive);
            }
            else
            {
                Console.WriteLine($"Loaded {directive} as {info.PackageName} version {info.VersionString}");
                TrackLoadedPackage(info, true, isCore);

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
