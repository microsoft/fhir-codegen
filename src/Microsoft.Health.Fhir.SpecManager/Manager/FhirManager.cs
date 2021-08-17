// -------------------------------------------------------------------------------------------------
// <copyright file="FhirManager.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.Health.Fhir.SpecManager.Manager
{
    /// <summary>FHIR version manager.</summary>
    public class FhirManager
    {
        /// <summary>FHIR Manager singleton.</summary>
        private static FhirManager _instance;

        /// <summary>Map of known major versions and version strings.</summary>
        private Dictionary<int, SortedSet<string>> _knownVersions;

        /// <summary>Dictionary of published versions.</summary>
        private Dictionary<string, FhirVersionInfo> _publishedVersionDict;

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

            _knownVersions = new Dictionary<int, SortedSet<string>>()
            {
                { 2, new SortedSet<string>() { "1.0.2" } },
                { 3, new SortedSet<string>() { "3.0.2" } },
                { 4, new SortedSet<string>() { "4.0.1", "4.1.0" } },
                { 5, new SortedSet<string>() { "4.4.0", "4.5.0", "4.6.0" } },
            };

            // build the dictionary of published versions*
            _publishedVersionDict = new Dictionary<string, FhirVersionInfo>()
            {
                {
                    "1.0.2",
                    new FhirVersionInfo(2)
                    {
                        ReleaseName = "DSTU2",
                        BallotPrefix = string.Empty,
                        PackageName = "hl7.fhir.r2.core",
                        ExamplesPackageName = "hl7.fhir.r2.examples",
                        ExpansionsPackageName = "hl7.fhir.r2.expansions",
                        VersionString = "1.0.2",
                        IsDevBuild = false,
                        IsLocalBuild = false,
                        IsOnDisk = false,
                    }
                },
                {
                    "3.0.2",
                    new FhirVersionInfo(3)
                    {
                        ReleaseName = "STU3",
                        BallotPrefix = string.Empty,
                        PackageName = "hl7.fhir.r3.core",
                        ExamplesPackageName = "hl7.fhir.r3.examples",
                        ExpansionsPackageName = "hl7.fhir.r3.expansions",
                        VersionString = "3.0.2",
                        IsDevBuild = false,
                        IsLocalBuild = false,
                        IsOnDisk = false,
                    }
                },
                {
                    "4.0.1",
                    new FhirVersionInfo(4)
                    {
                        ReleaseName = "R4",
                        BallotPrefix = string.Empty,
                        PackageName = "hl7.fhir.r4.core",
                        ExamplesPackageName = "hl7.fhir.r4.examples",
                        ExpansionsPackageName = "hl7.fhir.r4.expansions",
                        VersionString = "4.0.1",
                        IsDevBuild = false,
                        IsLocalBuild = false,
                        IsOnDisk = false,
                    }
                },
                {
                    "4.1.0",
                    new FhirVersionInfo(4)
                    {
                        ReleaseName = "R4B",
                        BallotPrefix = "2021Mar",
                        PackageName = "hl7.fhir.r4b.core",
                        ExamplesPackageName = string.Empty,
                        ExpansionsPackageName = "hl7.fhir.r4b.expansions",
                        VersionString = "4.1.0",
                        IsDevBuild = false,
                        IsLocalBuild = false,
                        IsOnDisk = false,
                    }
                },
                {
                    "4.4.0",
                    new FhirVersionInfo(5)
                    {
                        ReleaseName = "R5",
                        BallotPrefix = "2020May",
                        PackageName = "hl7.fhir.r5.core",
                        ExamplesPackageName = string.Empty,                         // "hl7.fhir.r5.examples",
                        ExpansionsPackageName = "hl7.fhir.r5.expansions",
                        VersionString = "4.4.0",
                        IsDevBuild = false,
                        IsLocalBuild = false,
                        IsOnDisk = false,
                    }
                },
                {
                    "4.5.0",
                    new FhirVersionInfo(5)
                    {
                        ReleaseName = "R5",
                        BallotPrefix = "2020Sep",
                        PackageName = "hl7.fhir.r5.core",
                        ExamplesPackageName = string.Empty,                         // "hl7.fhir.r5.examples",
                        ExpansionsPackageName = "hl7.fhir.r5.expansions",
                        VersionString = "4.5.0",
                        IsDevBuild = false,
                        IsLocalBuild = false,
                        IsOnDisk = false,
                    }
                },
                {
                    "4.6.0",
                    new FhirVersionInfo(5)
                    {
                        ReleaseName = "R5",
                        BallotPrefix = "2021May",
                        PackageName = "hl7.fhir.r5.core",
                        ExamplesPackageName = "hl7.fhir.r5.examples",
                        ExpansionsPackageName = "hl7.fhir.r5.expansions",
                        VersionString = "4.6.0",
                        IsDevBuild = false,
                        IsLocalBuild = false,
                        IsOnDisk = false,
                    }
                },
            };
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

            GetLocalVersionInfo(
                out string fhirVersion,
                out string versionString,
                out string buildId,
                out string buildDate);

            int majorVersion = FhirVersionInfo.GetMajorVersion(versionString);

            if (majorVersion == 0)
            {
                throw new ArgumentOutOfRangeException($"Unknown FHIR version: {versionString}");
            }

            string unfortunateVersionNameAppend =
                (versionString == "4.1.0")
                ? "b"
                : string.Empty;

            _localVersion = new FhirVersionInfo(majorVersion)
            {
                ReleaseName = buildId,
                PackageName = $"hl7.fhir.r{majorVersion}{unfortunateVersionNameAppend}.core",
                ExamplesPackageName = string.Empty,
                ExpansionsPackageName = $"hl7.fhir.r{majorVersion}{unfortunateVersionNameAppend}.expansions",
                VersionString = versionString,
                IsDevBuild = true,
                DevBranch = string.Empty,
                IsLocalBuild = true,
                IsOnDisk = true,
            };

            // load the package
            FhirSpecificationLoader.LoadLocalBuild(_localPublishDirectory, _fhirSpecDirectory, ref _localVersion, localLoadType, officialExpansionsOnly);

            return _localVersion;
        }

        /// <summary>Gets local version information.</summary>
        /// <exception cref="FileNotFoundException">Thrown when the requested file is not present.</exception>
        /// <param name="fhirVersion">[out] The FHIR version.</param>
        /// <param name="version">    [out] The version string (e.g., 4.0.1).</param>
        /// <param name="buildId">    [out] Identifier for the build.</param>
        /// <param name="buildDate">  [out] The build date.</param>
        private void GetLocalVersionInfo(
            out string fhirVersion,
            out string version,
            out string buildId,
            out string buildDate)
        {
            string infoPath = Path.Combine(_localPublishDirectory, "version.info");

            if (!File.Exists(infoPath))
            {
#pragma warning disable CA1303 // Do not pass literals as localized parameters
                throw new FileNotFoundException("Incomplete FHIR build, cannot find version information", infoPath);
#pragma warning restore CA1303 // Do not pass literals as localized parameters
            }

            IEnumerable<string> lines = File.ReadLines(infoPath);

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

            string[] versionParts = components[1].Split('.');

            if (!int.TryParse(versionParts[0], out int majorRelease))
            {
                throw new ArgumentException($"Invalid FHIR version: {versionDirective}");
            }

            if ((!_knownVersions.ContainsKey(majorRelease)) ||
                (!_knownVersions[majorRelease].Contains(components[1])))
            {
                throw new ArgumentOutOfRangeException($"Unknown Published FHIR version: {majorRelease}");
            }

            string upper = components[0].ToUpperInvariant();
            string lower = components[0].ToLowerInvariant();

            // grab the correct basic version info
            FhirVersionInfo info = new FhirVersionInfo(majorRelease)
            {
                ReleaseName = upper,
                BallotPrefix = string.Empty,
                PackageName = $"hl7.fhir.{lower}.core",
                ExamplesPackageName = $"hl7.fhir.{lower}.examples",
                ExpansionsPackageName = $"hl7.fhir.{lower}.expansions",
                VersionString = components[1],
                IsDevBuild = false,
                IsLocalBuild = false,
                IsOnDisk = true,
            };

            // load the package
            FhirSpecificationLoader.LoadCached(ref info, officialExpansionsOnly);

            // return this record
            return info;
        }

        /// <summary>Loads a published version of FHIR.</summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when one or more arguments are outside the
        ///  required range.</exception>
        /// <exception cref="ArgumentException">          Thrown when one or more arguments have
        ///  unsupported or illegal values.</exception>
        /// <param name="majorRelease">          The release number of FHIR to load (e.g., 2 for DSTU2).</param>
        /// <param name="versions">              The specific version of FHIR to load, or 'latest' for
        ///  highest known version.</param>
        /// <param name="offlineMode">           (Optional) True to allow, false to suppress the download.</param>
        /// <param name="officialExpansionsOnly">(Optional) True to official expansions only.</param>
        /// <returns>True if it succeeds, false if it fails.</returns>
        public FhirVersionInfo LoadPublished(
            int majorRelease,
            string versions,
            bool offlineMode = false,
            bool officialExpansionsOnly = false)
        {
            HashSet<string> versionsToLoad = new HashSet<string>();

            // check major version
            if (!_knownVersions.ContainsKey(majorRelease))
            {
                throw new ArgumentOutOfRangeException($"Unknown Published FHIR version: {majorRelease}");
            }

            // figure out which version(s) we are loading
            if (string.IsNullOrEmpty(versions) || (versions == "latest"))
            {
                string bestMatchVersion = string.Empty;

                // known versions are sorted in ascending order
                foreach (string knownVersion in _knownVersions[majorRelease])
                {
                    if (string.IsNullOrEmpty(bestMatchVersion))
                    {
                        bestMatchVersion = knownVersion;
                        continue;
                    }

                    if (string.IsNullOrEmpty(_publishedVersionDict[knownVersion].BallotPrefix) ||
                        (!string.IsNullOrEmpty(_publishedVersionDict[bestMatchVersion].BallotPrefix)))
                    {
                        bestMatchVersion = knownVersion;
                    }
                }

                if (!versionsToLoad.Contains(bestMatchVersion))
                {
                    versionsToLoad.Add(bestMatchVersion);
                }
            }
            else
            {
                // NOTE: we'll have to support multiple versions of a release in the future, was easier to just put this in now
                string[] versionSplit = versions.Split(',');

                foreach (string version in versionSplit)
                {
                    if (version == "latest")
                    {
                        string bestMatchVersion = string.Empty;

                        // known versions are sorted in ascending order
                        foreach (string knownVersion in _knownVersions[majorRelease])
                        {
                            if (string.IsNullOrEmpty(bestMatchVersion))
                            {
                                bestMatchVersion = knownVersion;
                                continue;
                            }

                            if (string.IsNullOrEmpty(_publishedVersionDict[knownVersion].BallotPrefix))
                            {
                                bestMatchVersion = knownVersion;
                            }
                        }

                        if (!versionsToLoad.Contains(bestMatchVersion))
                        {
                            versionsToLoad.Add(bestMatchVersion);
                        }

                        continue;
                    }

                    if (!_knownVersions[majorRelease].Contains(version))
                    {
                        throw new ArgumentException($"Invalid version {version} for R{majorRelease}");
                    }

                    versionsToLoad.Add(version);
                }
            }

            if (versionsToLoad.Count == 0)
            {
                throw new ArgumentException($"Could not find version to load: R{majorRelease}, versions: {versions}");
            }

            // for now, only load the first version we encountered
            string versionToLoad = versionsToLoad.ElementAt(0);

            // grab the correct basic version info
            FhirVersionInfo info = _publishedVersionDict[versionToLoad];

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

            // update our version information
            _publishedVersionDict[versionToLoad] = info;

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
        private void FindOrDownload(
            string releaseName,
            string ballotPrefix,
            string packageName,
            string version,
            bool offlineMode,
            bool isOptional = false)
        {
            if (string.IsNullOrEmpty(packageName))
            {
                if (isOptional)
                {
                    return;
                }

                throw new ArgumentNullException(nameof(packageName));
            }

            if (!FhirSpecificationLoader.TryFindPackage(
                releaseName,
                packageName,
                version,
                _fhirSpecDirectory,
                out _))
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
                        _fhirSpecDirectory))
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
}
