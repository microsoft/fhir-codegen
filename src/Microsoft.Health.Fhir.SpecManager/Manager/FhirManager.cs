// -------------------------------------------------------------------------------------------------
// <copyright file="FhirManager.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>
// -------------------------------------------------------------------------------------------------

using System.IO;

namespace Microsoft.Health.Fhir.SpecManager.Manager;

/// <summary>FHIR version manager.</summary>
public class FhirManager
{
    /// <summary>FHIR Manager singleton.</summary>
    private static FhirManager _instance;

    /// <summary>Local development version (if present).</summary>
    private FhirVersionInfo _localVersion;

    /// <summary>Pathname of the npm directory.</summary>
    private string _fhirSpecDirectory;

    /// <summary>Pathname of the local FHIR Publish directory.</summary>
    private string _localPublishDirectory;

    /// <summary>Pathname of the package directory.</summary>
    private string _packageDirectory;

    /// <summary>Initializes a new instance of the <see cref="FhirManager"/> class.</summary>
    /// <param name="fhirCoreSpecDirectory">  Pathname of the FHIR core specification directory (fhirVersions).</param>
    /// <param name="localFhirBuildDirectory">Pathname of the local FHIR build directory (publish).</param>
    /// <param name="packageDirectory">       Pathname of the package directory (fhirPackages).</param>
    private FhirManager(
        string fhirCoreSpecDirectory,
        string localFhirBuildDirectory,
        string packageDirectory)
    {
        // set locals
        _fhirSpecDirectory = fhirCoreSpecDirectory;
        _localPublishDirectory = localFhirBuildDirectory;
        _packageDirectory = packageDirectory;
        _localVersion = null;
    }

    /// <summary>Gets the current singleton.</summary>
    public static FhirManager Current => _instance;

    /// <summary>Gets the pathname of the FHIR core specifier directory.</summary>
    public string FhirCoreSpecDirectory => _fhirSpecDirectory;

    /// <summary>Gets the pathname of the FHIR local build directory.</summary>
    public string FhirLocalBuildDirectory => _localPublishDirectory;

    /// <summary>Gets the pathname of the FHIR package directory.</summary>
    public string FhirPackageDirectory => _packageDirectory;

    /// <summary>Initializes this object.</summary>
    /// <exception cref="ArgumentNullException">     Thrown when one or more required arguments are
    ///  null.</exception>
    /// <exception cref="DirectoryNotFoundException">Thrown when the requested directory is not
    ///  present.</exception>
    /// <param name="fhirSpecDirectory">   Pathname of the FHIR Spec directory.</param>
    /// <param name="fhirPublishDirectory">Pathname of the FHIR local publish directory.</param>
    /// <param name="packageDirectory">    Pathname of the package directory.</param>
    public static void Init(
        string fhirSpecDirectory,
        string fhirPublishDirectory,
        string packageDirectory)
    {
        // check to make sure we have a directory to work from
        if (string.IsNullOrEmpty(fhirSpecDirectory) && string.IsNullOrEmpty(fhirPublishDirectory))
        {
            throw new ArgumentNullException(nameof(fhirSpecDirectory));
        }

        string specDir = string.Empty;
        string publishDir = string.Empty;
        string packageDir = string.Empty;

        if (!string.IsNullOrEmpty(fhirSpecDirectory))
        {
            // check for rooted vs relative
            if (Path.IsPathRooted(fhirSpecDirectory))
            {
                specDir = Path.GetFullPath(fhirSpecDirectory);
            }
            else
            {
                specDir = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), fhirSpecDirectory));
            }

            // make sure the directory exists
            if (!Directory.Exists(specDir))
            {
                throw new DirectoryNotFoundException($"FHIR Specification Directory not found: {fhirSpecDirectory}");
            }
        }

        if (!string.IsNullOrEmpty(fhirPublishDirectory))
        {
            // check for rooted vs relative
            if (Path.IsPathRooted(fhirPublishDirectory))
            {
                publishDir = Path.GetFullPath(fhirPublishDirectory);
            }
            else
            {
                publishDir = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), fhirPublishDirectory));
            }

            // make sure the directory exists
            if (!Directory.Exists(publishDir))
            {
                throw new DirectoryNotFoundException($"FHIR Build Directory not found: {fhirPublishDirectory}");
            }
        }

        if (!string.IsNullOrEmpty(packageDirectory))
        {
            // check for rooted vs relative
            if (Path.IsPathRooted(packageDirectory))
            {
                packageDir = Path.GetFullPath(packageDirectory);
            }
            else
            {
                packageDir = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), packageDirectory));
            }

            // make sure the directory exists
            if (!Directory.Exists(packageDir))
            {
                throw new DirectoryNotFoundException($"FHIR Package Directory not found: {packageDirectory}");
            }
        }

        // make our instance
        _instance = new FhirManager(specDir, publishDir, packageDir);
    }

    /// <summary>Loads the local.</summary>
    /// <exception cref="ArgumentNullException">      Thrown when one or more required arguments are
    ///  null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when one or more arguments are outside the
    ///  required range.</exception>
    /// <param name="localLoadType">         Type of the local load.</param>
    /// <param name="officialExpansionsOnly">(Optional) True to official expansions only.</param>
    /// <returns>The local.</returns>
    public FhirVersionInfo LoadLocal(
        string localLoadType,
        bool officialExpansionsOnly = false)
    {
        if (string.IsNullOrEmpty(_localPublishDirectory))
        {
#pragma warning disable CA2208 // Instantiate argument exceptions correctly
            throw new ArgumentNullException($"Loading a local FHIR build requires a --fhir-publish-directory");
#pragma warning restore CA2208 // Instantiate argument exceptions correctly
        }

        string versionInfoPath = Path.Combine();

        if (!File.Exists(versionInfoPath))
        {
            throw new FileNotFoundException("Incomplete FHIR build, cannot find version information", versionInfoPath);
        }

        FhirPackageDownloader.ParseVersionInfoIni(
            File.ReadAllText(versionInfoPath),
            out string fhirVersion,
            out string versionString,
            out string buildId,
            out string buildDate);

        _localVersion = new FhirVersionInfo(versionString)
        {
            ReleaseName = buildId,
            ExamplesPackageName = string.Empty,
            VersionString = versionString,
            IsDevBuild = true,
            DevBranch = string.Empty,
            IsLocalBuild = true,
            IsOnDisk = true,
        };

        // load the package
        FhirSpecificationLoader.LoadLocalBuild(
            _localPublishDirectory,
            _fhirSpecDirectory,
            ref _localVersion,
            localLoadType,
            officialExpansionsOnly);

        return _localVersion;
    }

    /// <summary>Loads a cached.</summary>
    /// <exception cref="ArgumentNullException">      Thrown when one or more required arguments are
    ///  null.</exception>
    /// <exception cref="ArgumentException">          Thrown when one or more arguments have
    ///  unsupported or illegal values.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when one or more arguments are outside the
    ///  required range.</exception>
    /// <param name="versionDirective">      The version directive.</param>
    /// <param name="officialExpansionsOnly">(Optional) True to official expansions only.</param>
    /// <returns>The cached.</returns>
    public FhirVersionInfo LoadCached(
        string versionDirective,
        bool officialExpansionsOnly = false)
    {
        if (string.IsNullOrEmpty(versionDirective))
        {
            throw new ArgumentNullException(nameof(versionDirective));
        }

        if (!versionDirective.Contains('#', StringComparison.Ordinal))
        {
            throw new ArgumentException($"Invalid version directive: {versionDirective}");
        }

        string[] components = versionDirective.Split('#');

        if (components.Length != 2)
        {
            throw new ArgumentException($"Invalid version directive: {versionDirective}");
        }

        string version = components[1];

        // grab the correct basic version info
        FhirVersionInfo info = new FhirVersionInfo(version)
        {
            IsOnDisk = true,
        };

        // load the package
        FhirSpecificationLoader.LoadCached(ref info, officialExpansionsOnly);

        // return this record
        return info;
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
        FhirVersionInfo.FhirCoreVersion majorRelease,
        string version,
        bool offlineMode = false,
        bool officialExpansionsOnly = false)
    {
        string versionToLoad = string.Empty;

        if (string.IsNullOrEmpty(version) ||
            version.Equals("latest", StringComparison.OrdinalIgnoreCase))
        {
            versionToLoad = FhirVersionInfo.LatestVersionForRelease(majorRelease);
        }
        else if (FhirVersionInfo.IsKnownVersion(version))
        {
            versionToLoad = version;
        }

        if (string.IsNullOrEmpty(versionToLoad))
        {
            throw new ArgumentException($"Invalid version directive: {version}", nameof(version));
        }

        // grab the correct basic version info
        FhirVersionInfo info = new FhirVersionInfo(versionToLoad);

        // grab the packages we need
        FindOrDownload(
            info.ReleaseName,
            info.BallotPrefix,
            info.PackageName,
            info.VersionString,
            offlineMode);

        FindOrDownload(
            info.ReleaseName,
            info.BallotPrefix,
            info.ExpansionsPackageName,
            info.VersionString,
            offlineMode);

        FindOrDownload(
            info.ReleaseName,
            info.BallotPrefix,
            info.ExamplesPackageName,
            info.VersionString,
            offlineMode,
            true);

        // load the package
        FhirSpecificationLoader.LoadPackage(_fhirSpecDirectory, ref info, officialExpansionsOnly);

        //// update our version information
        //_publishedVersionDict[versionToLoad] = info;

        // return this record
        return info;
    }

    /// <summary>Loads a CI build of FHIR.</summary>
    /// <param name="branch">                The branch name.</param>
    /// <param name="offlineMode">           (Optional) True to allow, false to suppress the download.</param>
    /// <param name="officialExpansionsOnly">(Optional) True to official expansions only.</param>
    /// <returns>The ci.</returns>
    public FhirVersionInfo LoadCi(
        string branch,
        bool offlineMode = false,
        bool officialExpansionsOnly = false)
    {
        string packageVersionInfoIni;

        if (!FhirPackageDownloader.DownloadCiBuildInfo(
                branch,
                out FhirVersionInfo info,
                out string versionInfoIni))
        {
            throw new ArgumentException($"Invalid branch: {branch}", nameof(branch));
        }

        // grab the packages we need - note that CI builds do NOT have examples
        FindOrDownload(
            info.ReleaseName,
            info.BallotPrefix,
            info.PackageName,
            info.VersionString,
            offlineMode,
            ciBranch: branch,
            buildId: info.BuildId);

        packageVersionInfoIni = Path.Combine(
            _fhirSpecDirectory,
            $"{info.PackageName}-{info.VersionString}-version.info");

        File.WriteAllText(packageVersionInfoIni, versionInfoIni);

        FindOrDownload(
            info.ReleaseName,
            info.BallotPrefix,
            info.ExpansionsPackageName,
            info.VersionString,
            offlineMode,
            ciBranch: branch,
            buildId: info.BuildId);

        packageVersionInfoIni = Path.Combine(
            _fhirSpecDirectory,
            $"{info.ExpansionsPackageName}-{info.VersionString}-version.info");

        File.WriteAllText(packageVersionInfoIni, versionInfoIni);

        // load the package
        FhirSpecificationLoader.LoadPackage(_fhirSpecDirectory, ref info, officialExpansionsOnly);

        // return this record
        return info;
    }

    /// <summary>Searches for the first or download.</summary>
    /// <exception cref="ArgumentNullException">     Thrown when one or more required arguments are
    ///  null.</exception>
    /// <exception cref="DirectoryNotFoundException">Thrown when the requested directory is not
    ///  present.</exception>
    /// <exception cref="Exception">                 Thrown when an exception error condition occurs.</exception>
    /// <param name="releaseName"> The release name (e.g., R4, DSTU2).</param>
    /// <param name="ballotPrefix">The ballot prefix.</param>
    /// <param name="packageName"> Name of the package.</param>
    /// <param name="version">     The version string (e.g., 4.0.1).</param>
    /// <param name="offlineMode"> True to allow, false to suppress the download.</param>
    /// <param name="isOptional">  (Optional) True if is optional, false if not.</param>
    /// <param name="ciBranch">    (Optional) The ci branch.</param>
    /// <param name="buildId">     (Optional) Identifier for the build.</param>
    private void FindOrDownload(
        string releaseName,
        string ballotPrefix,
        string packageName,
        string version,
        bool offlineMode,
        bool isOptional = false,
        string ciBranch = "",
        string buildId = "")
    {
        if (string.IsNullOrEmpty(packageName))
        {
            if (isOptional)
            {
                return;
            }

            throw new ArgumentNullException(nameof(packageName));
        }

        bool found = FhirSpecificationLoader.TryFindPackage(
            releaseName,
            packageName,
            version,
            _fhirSpecDirectory,
            out string versionDirectory);

        if (found && (!string.IsNullOrEmpty(buildId)))
        {
            string cachedVersionPath = Path.Combine(
                _fhirSpecDirectory,
                $"{packageName}-{version}-version.info");

            if (!File.Exists(cachedVersionPath))
            {
                found = false;
            }
            else
            {
                FhirPackageDownloader.ParseVersionInfoIni(
                    File.ReadAllText(cachedVersionPath),
                    out _,
                    out _,
                    out string cachedBuildId,
                    out _);

                if (!cachedBuildId.Equals(buildId, StringComparison.OrdinalIgnoreCase))
                {
                    found = false;
                }
            }
        }

        if (!found)
        {
            if (offlineMode)
            {
                if (isOptional)
                {
                    Console.WriteLine(
                        $"Failed to find OPTIONAL {packageName}-{version}" +
                        $" in {_fhirSpecDirectory}");
                    return;
                }

                throw new DirectoryNotFoundException(
                    $"Failed to find FHIR {packageName}" +
                    $" ({releaseName})" +
                    $" in {_fhirSpecDirectory}");
            }

            Console.WriteLine($" <<< downloading {packageName}-{version}...");

            if (!FhirPackageDownloader.DownloadFhirSpecification(
                    releaseName,
                    ballotPrefix,
                    packageName,
                    version,
                    _fhirSpecDirectory,
                    ciBranch))
            {
                if (isOptional)
                {
                    Console.WriteLine(
                        $"Failed to download OPTIONAL {packageName}-{version}");
                    return;
                }

                throw new Exception($"Could not download: {packageName}-{version}");
            }
        }
    }
}
