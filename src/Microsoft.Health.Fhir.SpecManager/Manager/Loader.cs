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
        /// <param name="npmDirectory">    Pathname of the npm directory.</param>
        /// <param name="versionInfo">     Information describing the version.</param>
        /// <param name="versionDirectory">[out] Pathname of the version directory.</param>
        /// <returns>True if it succeeds, false if it fails.</returns>
        public static bool TryFindPackage(string npmDirectory, FhirVersionInfo versionInfo, out string versionDirectory)
        {
            versionDirectory = null;

            // sanity checks
            if (versionInfo == null)
            {
                Console.WriteLine($"TryFindPackage <<< invalid versionInfo is NULL for {npmDirectory}!");
                return false;
            }

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
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        /// <exception cref="FileNotFoundException">Thrown when the requested file is not present.</exception>
        /// <exception cref="JsonException">Thrown when a JSON error condition occurs.</exception>
        /// <param name="npmDirectory">   Pathname of the npm directory.</param>
        /// <param name="fhirVersionInfo">[in,out] Information describing the fhir version.</param>
        public static void LoadPackage(string npmDirectory, ref FhirVersionInfo fhirVersionInfo)
        {
            // sanity checks
            if (fhirVersionInfo == null)
            {
                Console.WriteLine($"LoadPackage <<< invalid version info is NULL, cannot load {npmDirectory}");
                throw new ArgumentNullException(nameof(fhirVersionInfo));
            }

            // find the package
            if (!TryFindPackage(npmDirectory, fhirVersionInfo, out string packageDir))
            {
                Console.WriteLine($"LoadPackage <<< cannot find package for {fhirVersionInfo.ReleaseName}!");
                throw new FileNotFoundException($"Cannot find package for {fhirVersionInfo.ReleaseName}");
            }

            // load package info
            FhirPackageInfo packageInfo = FhirPackageInfo.Load(packageDir);

            // tell the user what's going on
            Console.WriteLine($"LoadPackage <<< Found: {packageInfo.Name} version: {packageInfo.Version}");

            // get the files in this directory
            // TODO: relax filter to *.json when more than structure defintions are being parsed
            string[] files = Directory.GetFiles(packageDir, "StructureDefinition*.json", SearchOption.TopDirectoryOnly);

            // process these files
            ProcessPackageFiles(files, ref fhirVersionInfo);
        }

        /// <summary>Process the package files.</summary>
        /// <exception cref="InvalidDataException">Thrown when an Invalid Data error condition occurs.</exception>
        /// <param name="files">          The files.</param>
        /// <param name="fhirVersionInfo">[in,out] Information describing the fhir version.</param>
        private static void ProcessPackageFiles(string[] files, ref FhirVersionInfo fhirVersionInfo)
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
                        throw new InvalidDataException($"Unhandled type: {shortName}");
                    }

                    // read the file
                    string contents = File.ReadAllText(filename);

                    // parse the file - note: using var here is siginificantly more performant than object
                    var resource = fhirVersionInfo.ParseResource(contents);

                    // check type matching
                    if (!resource.GetType().Name.Equals(resourceHint, StringComparison.Ordinal))
                    {
                        // type not found
                        Console.WriteLine($"\nProcessPackageFiles <<<" +
                            $" Mismatched type: {shortName}," +
                            $" should be {resourceHint} parsed to:{resource.GetType().Name}");
                        throw new InvalidDataException($"Mismatched type: {shortName}: {resourceHint} != {resource.GetType().Name}");
                    }

                    // process this resource
                    fhirVersionInfo.ProcessResource(resource);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(string.Empty);
                    Console.WriteLine($"LoadPackage <<< Failed to process file: {filename}: \n{ex}\n--------------");
                    throw;
                }
            }

            // make sure we cleared the last line
            Console.WriteLine($"LoadPackage <<< Loaded and Parsed FHIR {fhirVersionInfo.ReleaseName}!{new string(' ', 100)}");
        }
    }
}
