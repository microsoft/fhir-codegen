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
using System.Text;

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
                { 5, new SortedSet<string>() { "4.2.0" } },
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
                        VersionString = "4.0.1",
                        IsDevBuild = false,
                        IsLocalBuild = false,
                        IsOnDisk = false,
                    }
                },
                {
                    "4.2.0",
                    new FhirVersionInfo(5)
                    {
                        ReleaseName = "2020Feb",
                        PackageName = "hl7.fhir.r5.core",
                        VersionString = "4.2.0",
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
                        VersionString = "4.2.0",
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
        /// <exception cref="DirectoryNotFoundException"> Thrown when the requested directory is not
        ///  present.</exception>
        /// <param name="majorRelease">The release number of FHIR to load (e.g., 2 for DSTU2).</param>
        /// <param name="versions">    The specific version of FHIR to load, or 'latest' for highest known version.</param>
        /// <param name="offlineMode"> (Optional) True to allow, false to suppress the download.</param>
        /// <returns>True if it succeeds, false if it fails.</returns>
        public FhirVersionInfo LoadPublished(
            int majorRelease,
            string versions,
            bool offlineMode = false)
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
                versionsToLoad.Add(_knownVersions[majorRelease].ElementAt(0));
            }
            else
            {
                // NOTE: we'll have to support multiple versions of a release in the future, was easier to just put this in now
                string[] versionSplit = versions.Split(',');

                foreach (string version in versionSplit)
                {
                    if (version == "latest")
                    {
                        if (!versionsToLoad.Contains(_knownVersions[majorRelease].ElementAt(0)))
                        {
                            versionsToLoad.Add(_knownVersions[majorRelease].ElementAt(0));
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

            // check for local package
            if (!Loader.TryFindPackage(_fhirSpecDirectory, info, out _))
            {
                if (offlineMode)
                {
                    throw new DirectoryNotFoundException(
                        $"Failed to find FHIR R{info.MajorVersion}" +
                        $" ({info.ReleaseName})" +
                        $" in {_fhirSpecDirectory}");
                }

                Console.WriteLine($"FhirManager.Load <<< downloading R{info.MajorVersion}: {info.PackageName}-{info.VersionString}");

                // download from the package manager
                FhirPackageDownloader.DownloadPackage(
                    info.ReleaseName,
                    info.PackageName,
                    info.VersionString,
                    _fhirSpecDirectory);
            }

            // load the package
            Loader.LoadPackage(_fhirSpecDirectory, ref info);

            // update our version information
            _publishedVersionDict[versionToLoad] = info;

            // return this record
            return info;
        }
    }
}
