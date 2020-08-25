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

        /// <summary>Dictionary of development versions.</summary>
        private Dictionary<string, FhirVersionInfo> _devVersionDict;

        /// <summary>Pathname of the npm directory.</summary>
        private string _fhirSpecDirectory;

        /// <summary>
        /// Initializes a new instance of the <see cref="FhirManager"/> class.
        /// </summary>
        ///
        /// <param name="npmDirectory">Pathname of the npm directory.</param>
        private FhirManager(string npmDirectory)
        {
            // set locals
            _fhirSpecDirectory = npmDirectory;

            _knownVersions = new Dictionary<int, SortedSet<string>>()
            {
                { 2, new SortedSet<string>() { "1.0.2" } },
                { 3, new SortedSet<string>() { "3.0.2" } },
                { 4, new SortedSet<string>() { "4.0.1" } },
                { 5, new SortedSet<string>() { "4.4.0", "4.5.0" } },
            };

            // build the dictionary of published versions*
            _publishedVersionDict = new Dictionary<string, FhirVersionInfo>()
            {
                {
                    "1.0.2",
                    new FhirVersionInfo(2)
                    {
                        ReleaseName = "DSTU2",
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
                    "4.4.0",
                    new FhirVersionInfo(5)
                    {
                        ReleaseName = "2020May",
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
                        ReleaseName = "2020Sep",
                        PackageName = "hl7.fhir.r5.core",
                        ExamplesPackageName = string.Empty,                         // "hl7.fhir.r5.examples",
                        ExpansionsPackageName = "hl7.fhir.r5.expansions",
                        VersionString = "4.5.0",
                        IsDevBuild = false,
                        IsLocalBuild = false,
                        IsOnDisk = false,
                    }
                },
            };

            // create a dictionary for dev builds
            _devVersionDict = new Dictionary<string, FhirVersionInfo>()
            {
                {
                    "master",
                    new FhirVersionInfo(5)
                    {
                        ReleaseName = string.Empty,
                        PackageName = "hl7.fhir.r5.core",
                        ExamplesPackageName = string.Empty,                         // "hl7.fhir.r5.examples",
                        ExpansionsPackageName = "hl7.fhir.r5.expansions",
                        VersionString = "4.4.0",
                        IsDevBuild = true,
                        DevBranch = string.Empty,
                        IsLocalBuild = false,
                        IsOnDisk = false,
                    }
                },
            };
        }

        /// <summary>Gets the current singleton.</summary>
        ///
        /// <value>The current FHIR Manager singleton.</value>
        public static FhirManager Current => _instance;

        /// <summary>Initializes this object.</summary>
        /// <exception cref="ArgumentNullException">     Thrown when one or more required arguments are
        ///  null.</exception>
        /// <exception cref="DirectoryNotFoundException">Thrown when the requested directory is not
        ///  present.</exception>
        /// <param name="fhirSpecDirectory">Pathname of the FHIR Spec directory.</param>
        public static void Init(string fhirSpecDirectory)
        {
            // check to make sure we have a directory to work from
            if (string.IsNullOrEmpty(fhirSpecDirectory))
            {
                throw new ArgumentNullException(nameof(fhirSpecDirectory));
            }

            string dir;

            // check for rooted vs relative
            if (Path.IsPathRooted(fhirSpecDirectory))
            {
                dir = fhirSpecDirectory;
            }
            else
            {
                dir = Path.Combine(Directory.GetCurrentDirectory(), fhirSpecDirectory);
            }

            // make sure the directory exists
            if (!Directory.Exists(dir))
            {
                throw new DirectoryNotFoundException($"FHIR Specification Directory not found: {fhirSpecDirectory}");
            }

            // make our instance
            _instance = new FhirManager(dir);
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
                versionsToLoad.Add(_knownVersions[majorRelease].Max);
            }
            else
            {
                // NOTE: we'll have to support multiple versions of a release in the future, was easier to just put this in now
                string[] versionSplit = versions.Split(',');

                foreach (string version in versionSplit)
                {
                    if (version == "latest")
                    {
                        if (!versionsToLoad.Contains(_knownVersions[majorRelease].Max))
                        {
                            versionsToLoad.Add(_knownVersions[majorRelease].Max);
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
                info.PackageName,
                info.VersionString,
                offlineMode);

            FindOrDownload(
                info.ReleaseName,
                info.ExpansionsPackageName,
                info.VersionString,
                offlineMode);

            FindOrDownload(
                info.ReleaseName,
                info.ExamplesPackageName,
                info.VersionString,
                offlineMode,
                true);

            // load the package
            Loader.LoadPackage(_fhirSpecDirectory, ref info, officialExpansionsOnly);

            // update our version information
            _publishedVersionDict[versionToLoad] = info;

            // return this record
            return info;
        }

        /// <summary>Searches for the first or download.</summary>
        /// <exception cref="DirectoryNotFoundException">Thrown when the requested directory is not
        ///  present.</exception>
        /// <exception cref="Exception">                 Thrown when an exception error condition occurs.</exception>
        /// <param name="releaseName">The release name (e.g., R4, DSTU2).</param>
        /// <param name="packageName">Name of the package.</param>
        /// <param name="version">    The version string (e.g., 4.0.1).</param>
        /// <param name="offlineMode">True to allow, false to suppress the download.</param>
        /// <param name="isOptional"> (Optional) True if is optional, false if not.</param>
        private void FindOrDownload(
            string releaseName,
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

            if (!Loader.TryFindPackage(
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

                if (!FhirPackageDownloader.Download(
                        releaseName,
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
