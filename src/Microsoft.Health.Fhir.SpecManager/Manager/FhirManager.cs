// <copyright file="FhirManager.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Microsoft.Health.Fhir.SpecManager.PackageManager;

namespace Microsoft.Health.Fhir.SpecManager.Manager;

/// <summary>FHIR version manager.</summary>
public class FhirManager : IDisposable
{
    /// <summary>Local development version (if present).</summary>
    private FhirVersionInfo _localVersion;

    /// <summary>True to disposed value.</summary>
    private bool _disposedValue;

    /// <summary>The singleton.</summary>
    private static FhirManager _singleton;

    /// <summary>
    /// Prevents a default instance of the <see cref="FhirManager"/> class from being created.
    /// </summary>
    private FhirManager()
    {
        // set locals
        _localVersion = null;
    }

    /// <summary>Gets the current.</summary>
    public static FhirManager Current => _singleton;

    /// <summary>Initializes the FhirManager.</summary>
    public static void Init()
    {
        Console.WriteLine("Starting FhirManager...");

        _singleton = new FhirManager();
    }

    /// <summary>Loads FHIR core packages.</summary>
    /// <param name="sequence">              The sequence.</param>
    /// <param name="version">               The specific version of FHIR to load, or 'latest' for
    ///  highest known version.</param>
    /// <param name="offlineMode">           True to allow, false to suppress the download.</param>
    /// <param name="officialExpansionsOnly">True to official expansions only.</param>
    /// <param name="ciBranch">              (Optional) The CI branch.</param>
    /// <returns>The FHIR core.</returns>
    public FhirVersionInfo LoadFhirCore(
        FhirPackageCommon.FhirSequence sequence,
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

        // grab the correct basic version info
        //FhirVersionInfo info = new FhirVersionInfo(version, sequence, ciBranch, string.Empty)
        FhirVersionInfo info = new FhirVersionInfo(corePackageDirectory)
        {
            IsOnDisk = true,
        };

        // load the package
        FhirSpecificationLoader.Load(
            info,
            corePackageDirectory,
            expansionsDirectory,
            officialExpansionsOnly);

        return info;
    }

    /// <summary>Loads the packages.</summary>
    /// <param name="packageDirectives">The package directives.</param>
    /// <param name="offlineMode">      True to allow, false to suppress the download.</param>
    /// <returns>The packages.</returns>
    public IEnumerable<FhirVersionInfo> LoadPackages(
        IEnumerable<string> packageDirectives,
        bool offlineMode)
    {
        List<FhirVersionInfo> packages = new List<FhirVersionInfo>();

        foreach (string directive in packageDirectives)
        {
            FhirCacheService.Current.FindOrDownload(
                directive,
                out string directory,
                offlineMode);

            FhirPackageLoader.Load(directive, directory, out FhirVersionInfo info);

            if (info != null)
            {
                Console.WriteLine($"Loaded {info.PackageName}...");
                packages.Add(info);
            }
        }

        return packages.AsEnumerable();
    }

    /// <summary>Loads a published version of FHIR.</summary>
    /// <exception cref="ArgumentException">Thrown when one or more arguments have unsupported or
    ///  illegal values.</exception>
    /// <param name="majorRelease">          The release of FHIR to load (e.g., DSTU2, R4B).</param>
    /// <param name="version">               The specific version of FHIR to load, or 'latest' for
    ///  highest known version.</param>
    /// <param name="offlineMode">           (Optional) True to allow, false to suppress the download.</param>
    /// <param name="officialExpansionsOnly">(Optional) True to official expansions only.</param>
    /// <returns>The loaded FHIR fhir specification.</returns>
    public FhirVersionInfo LoadPublished(
        FhirPackageCommon.FhirSequence majorRelease,
        string version,
        bool offlineMode = false,
        bool officialExpansionsOnly = false)
    {
        string versionToLoad = string.Empty;

        if (string.IsNullOrEmpty(version) ||
            version.Equals("latest", StringComparison.OrdinalIgnoreCase))
        {
            versionToLoad = FhirPackageCommon.LatestVersionForRelease(majorRelease);
        }
        else if (FhirPackageCommon.IsKnownVersion(version))
        {
            versionToLoad = version;
        }

        if (string.IsNullOrEmpty(versionToLoad))
        {
            throw new ArgumentException($"Invalid version directive: {version}", nameof(version));
        }

        // grab the correct basic version info
        FhirVersionInfo info = new FhirVersionInfo(versionToLoad);

        //// grab the packages we need
        //FindOrDownload(
        //    info.ReleaseName,
        //    info.BallotPrefix,
        //    info.PackageName,
        //    info.VersionString,
        //    offlineMode);

        //FindOrDownload(
        //    info.ReleaseName,
        //    info.BallotPrefix,
        //    info.ExpansionsPackageName,
        //    info.VersionString,
        //    offlineMode);

        //FindOrDownload(
        //    info.ReleaseName,
        //    info.BallotPrefix,
        //    info.ExamplesPackageName,
        //    info.VersionString,
        //    offlineMode,
        //    true);

        //// load the package
        //FhirSpecificationLoader.LoadPackage(_fhirSpecDirectory, ref info, officialExpansionsOnly);

        //// update our version information
        //_publishedVersionDict[versionToLoad] = info;

        // return this record
        return info;
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
}
