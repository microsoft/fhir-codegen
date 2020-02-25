// -------------------------------------------------------------------------------------------------
// <copyright file="Loader.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Health.Fhir.SpecManager.Models;

namespace Microsoft.Health.Fhir.SpecManager.Manager
{
    /// <summary>A FHIR Specification loader.</summary>
    public abstract class Loader
    {
        /// <summary>Searches for the currently specified package.</summary>
        ///
        /// <param name="npmDirectory">    Pathname of the npm directory.</param>
        /// <param name="versionInfo">     Information describing the version.</param>
        /// <param name="versionDirectory">[out] Pathname of the version directory.</param>
        ///
        /// <returns>True if it succeeds, false if it fails.</returns>
        public static bool TryFindPackage(string npmDirectory, FhirVersionInfo versionInfo, out string versionDirectory)
        {
            versionDirectory = null;

            // check for manual download first
            string packageDir = Path.Combine(npmDirectory, versionInfo.PackageName, "package");

            if (!Directory.Exists(packageDir))
            {
                // check for npm install directory
                packageDir = Path.Combine(npmDirectory, "node_modules", versionInfo.PackageName);

                if (!Directory.Exists(packageDir))
                {
                    Console.WriteLine($"TryFindPackage <<< cannot find FHIR Package: {versionInfo.ReleaseName}, {versionInfo.PackageName}!");
                    return false;
                }
            }

            // set our directory
            versionDirectory = packageDir;
            return true;
        }

        /// <summary>Loads a package.</summary>
        ///
        /// <param name="npmDirectory">   Pathname of the npm directory.</param>
        /// <param name="fhirVersionInfo">[in,out] Information describing the fhir version.</param>
        ///
        /// <returns>True if it succeeds, false if it fails.</returns>
        public static bool LoadPackage(string npmDirectory, ref FhirVersionInfo fhirVersionInfo)
        {
            // sanity checks
            if (fhirVersionInfo == null)
            {
                Console.WriteLine($"LoadPackage <<< invalid version info is NULL, cannot load {npmDirectory}");
                return false;
            }

            // find the package
            if (!TryFindPackage(npmDirectory, fhirVersionInfo, out string packageDir))
            {
                Console.WriteLine($"LoadPackage <<< cannot find package for {fhirVersionInfo.ReleaseName}!");
                return false;
            }

            // load package info
            if (!FhirPackageInfo.TryLoadPackageInfo(packageDir, out FhirPackageInfo packageInfo))
            {
                Console.WriteLine($"LoadPackage <<<" +
                                    $" Failed to load version {fhirVersionInfo.MajorVersion}" +
                                    $" package info, dir: {packageDir}");
                return false;
            }

            // tell the user what's going on
            Console.WriteLine($"LoadPackage <<< Found: {packageInfo.Name} version: {packageInfo.Version}");

            // get the files in this directory
            string[] files = Directory.GetFiles(packageDir, "*.json", SearchOption.TopDirectoryOnly);

            if (!ProcessPackageFiles(files, ref fhirVersionInfo))
            {
                Console.WriteLine($"LoadPackage <<< failed to process package files in {npmDirectory}!");
                return false;
            }

            // success
            return true;
        }

        /// <summary>Process the package files.</summary>
        ///
        /// <param name="files">          The files.</param>
        /// <param name="fhirVersionInfo">[in,out] Information describing the fhir version.</param>
        ///
        /// <returns>True if it succeeds, false if it fails.</returns>
        private static bool ProcessPackageFiles(string[] files, ref FhirVersionInfo fhirVersionInfo)
        {
            // traverse the files
            foreach (string filename in files)
            {
                // check for skipping file
                if (FhirVersionInfo.ShouldSkipFile(Path.GetFileName(filename)))
                {
                    // skip this file
                    continue;
                }

                // parse the name into parts we want
                string shortName = Path.GetFileNameWithoutExtension(filename);
                string resourceHint = shortName.Split('-')[0];
                string resourceName = shortName.Substring(resourceHint.Length + 1);

                // attempt to load this file
                try
                {
                    Console.Write($"v{fhirVersionInfo.MajorVersion}: {shortName,-85}\r");

                    // check for ignored types
                    if (fhirVersionInfo.ShouldIgnoreResource(resourceHint))
                    {
                        // skip
                        continue;
                    }

                    // this should be listed in process types (validation check)
                    if (!fhirVersionInfo.ShouldProcessResource(resourceHint))
                    {
                        // type not found
                        Console.WriteLine($"\nProcessPackageFiles <<< Unhandled type: {shortName}");
                        return false;
                    }

                    // read the file
                    string contents = File.ReadAllText(filename);

                    // parse the file
                    if (!fhirVersionInfo.TryParseResource(contents, out var obj))
                    {
                        Console.WriteLine($"\nProcessPackageFiles <<<" +
                            $" failed to parse resource: {shortName}");
                        return false;
                    }

                    // check type matching
                    if (!obj.GetType().Name.Equals(resourceHint, StringComparison.Ordinal))
                    {
                        // type not found
                        Console.WriteLine($"\nProcessPackageFiles <<<" +
                            $" Mismatched type: {shortName}," +
                            $" should be {resourceHint} parsed to:{obj.GetType().Name}");
                        return false;
                    }

                    // process this resource
                    if (!fhirVersionInfo.TryProcessResource(obj))
                    {
                        Console.WriteLine($"\nProcessPackageFiles <<<" +
                            $" failed to process resource: {shortName}");
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(string.Empty);
                    Console.WriteLine($"LoadPackage <<< Failed to process file: {filename}: \n{ex}\n--------------");
                    return false;
                }
            }

            // make sure we cleared the last line
            Console.WriteLine($"LoadPackage <<< Loaded and Parsed FHIR {fhirVersionInfo.ReleaseName}!{new string(' ', 100)}");

            // still here means success
            return true;
        }
    }
}
