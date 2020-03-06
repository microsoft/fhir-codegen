﻿// -------------------------------------------------------------------------------------------------
// <copyright file="FhirManager.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Microsoft.Health.Fhir.SpecManager.Manager
{
    /// <summary>FHIR version manager.</summary>
    public class FhirManager
    {
        /// <summary>FHIR Manager singleton.</summary>
        private static FhirManager _instance;

        /// <summary>Dictionary of published versions.</summary>
        private Dictionary<int, FhirVersionInfo> _publishedVersionDict;

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

            // build the dictionary of published versions*
            _publishedVersionDict = new Dictionary<int, FhirVersionInfo>()
            {
                {
                    2,
                    new FhirVersionInfo(2)
                    {
                        ReleaseName = "DSTU2",
                        PackageName = "hl7.fhir.r2.core",
                        IsDevBuild = false,
                        IsLocalBuild = false,
                        IsOnDisk = false,
                    }
                },
                {
                    3,
                    new FhirVersionInfo(3)
                    {
                        ReleaseName = "STU3",
                        PackageName = "hl7.fhir.r3.core",
                        IsDevBuild = false,
                        IsLocalBuild = false,
                        IsOnDisk = false,
                    }
                },
                {
                    4,
                    new FhirVersionInfo(4)
                    {
                        ReleaseName = "R4",
                        PackageName = "hl7.fhir.r4.core",
                        IsDevBuild = false,
                        IsLocalBuild = false,
                        IsOnDisk = false,
                    }
                },
                {
                    5,
                    new FhirVersionInfo(5)
                    {
                        ReleaseName = "2020Feb",
                        PackageName = "hl7.fhir.r5.core",
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
        ///
        /// <param name="npmDirectory">Pathname of the npm directory.</param>
        ///
        /// <returns>True if it succeeds, false if it fails.</returns>
        public static bool Init(string npmDirectory)
        {
            // check to make sure we have a directory to work from
            if (string.IsNullOrEmpty(npmDirectory))
            {
                Console.WriteLine($"FhirManager.Init <<< Invalid NPM Directory: {npmDirectory}");
                return false;
            }

            // make sure the directory exists
            if (!Directory.Exists(npmDirectory))
            {
                Console.WriteLine($"FhirManager.Init <<< NPM directory not found: {npmDirectory}");
                return false;
            }

            // make our instance
            _instance = new FhirManager(npmDirectory);

            // here means success
            return true;
        }

        /// <summary>Loads a published version of FHIR.</summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when one or more arguments are outside the
        ///  required range.</exception>
        /// <param name="version">      The version.</param>
        /// <param name="offlineMode">(Optional) True to allow, false to suppress the download.</param>
        /// <returns>True if it succeeds, false if it fails.</returns>
        public FhirVersionInfo LoadPublished(int version, bool offlineMode = false)
        {
            // check version
            if (!_publishedVersionDict.ContainsKey(version))
            {
                throw new ArgumentOutOfRangeException($"Unknown Published FHIR version: {version}");
            }

            // grab the correct basic version info
            FhirVersionInfo info = _publishedVersionDict[version];

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

                Console.WriteLine($"FhirManager.Load <<< downloading version {info.MajorVersion} ({info.ReleaseName})");

                // download the published version
                FhirPackageDownloader.DownloadPublishedPackage(
                    info.ReleaseName,
                    info.PackageName,
                    _fhirSpecDirectory);
            }

            // load the package
            Loader.LoadPackage(_fhirSpecDirectory, ref info);

            // update our version information
            _publishedVersionDict[version] = info;

            // return this record
            return info;
        }

        /// <summary>Check local versions.</summary>
        private void CheckLocalVersions()
        {
            // traverse the dictionary of published versions
            foreach (KeyValuePair<int, FhirVersionInfo> kvp in _publishedVersionDict)
            {
                // TODO: check to see if this version is already downloaded
            }
        }
    }
}
