// <copyright file="FhirPackageLoader.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Health.Fhir.SpecManager.Models;

namespace Microsoft.Health.Fhir.SpecManager.Manager
{
    /// <summary>A FHIR package loader (e.g., hl7.fhir.us.core).</summary>
    public abstract class FhirPackageLoader
    {
        /// <summary>Attempts to find a FHIRpackage.</summary>
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        /// <param name="packageName">     Name of the package.</param>
        /// <param name="version">         The version.</param>
        /// <param name="fhirVersion">     The FHIR version.</param>
        /// <param name="packageDirectory">Pathname of the package directory.</param>
        /// <param name="versionDirectory">[out] Pathname of the version directory.</param>
        /// <returns>True if it succeeds, false if it fails.</returns>
        public static bool TryFindPackage(
            string packageName,
            string version,
            string fhirVersion,
            string packageDirectory,
            out string versionDirectory)
        {
            versionDirectory = null;

            if (string.IsNullOrEmpty(packageDirectory))
            {
                throw new ArgumentNullException(nameof(packageDirectory));
            }

            if (string.IsNullOrEmpty(packageName))
            {
                throw new ArgumentNullException(nameof(packageName));
            }

            // TODO(ginoc): need to search for unversioned packages, check against fhir versions, etc.

            // check for finding the directory
            string packageDir = Path.Combine(
                packageDirectory,
                $"{packageName}-{version}",
                "package");

            if (!Directory.Exists(packageDir))
            {
                // check for NPM installed version
                packageDir = Path.Combine(packageDirectory, "node_modules", packageName);

                if (!Directory.Exists(packageDir))
                {
                    Console.WriteLine($"Cannot find {packageName} version {version} in {packageDirectory}");
                    return false;
                }
            }

            // set our directory
            versionDirectory = packageDir;
            return true;
        }

        /// <summary>Loads a FHIR package (e.g., hl7.fhir.us.core-4.0.0).</summary>
        /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
        /// <exception cref="FileNotFoundException">Thrown when the requested file is not present.</exception>
        /// <param name="packageName">     Name of the package.</param>
        /// <param name="packageVersion">         The version.</param>
        /// <param name="fhirVersion">     The FHIR version.</param>
        /// <param name="packageDirectory">Pathname of the package directory.</param>
        /// <param name="fhirVersionInfo"> [in,out] Information describing the fhir version.</param>
        public static void LoadPackage(
            string packageName,
            string packageVersion,
            string fhirVersion,
            string packageDirectory,
            ref FhirVersionInfo fhirVersionInfo)
        {
            // sanity checks
            if (fhirVersionInfo == null)
            {
                Console.WriteLine($"LoadPackage <<< invalid version info is NULL, cannot load {packageDirectory}");
                throw new ArgumentNullException(nameof(fhirVersionInfo));
            }

            // TODO(ginoc): need to sort out finding/downloading/loading logic

            if (!TryFindPackage(
                    packageName,
                    packageVersion,
                    fhirVersion,
                    packageDirectory,
                    out string dir))
            {
                Console.WriteLine($"LoadPackage <<< cannot find package for {fhirVersionInfo.ReleaseName}: {fhirVersionInfo.ExpansionsPackageName}!");
                throw new FileNotFoundException($"Cannot find package for {fhirVersionInfo.ReleaseName}: {fhirVersionInfo.ExpansionsPackageName}");
            }

            // load package info
            FhirPackageInfo packageInfo = FhirPackageInfo.Load(dir);

            // tell the user what's going on
            Console.WriteLine($"LoadPackage <<< Found: {packageInfo.Name} version: {packageInfo.Version}");

            // update our structure
            fhirVersionInfo.VersionString = packageInfo.Version;

            HashSet<string> processedFiles = new HashSet<string>();

            // process Code Systems
            ProcessFileGroup(dir, "CodeSystem", ref fhirVersionInfo, ref processedFiles);

            // process Value Set expansions
            ProcessFileGroup(dir, "ValueSet", ref fhirVersionInfo, ref processedFiles);

            // process structure definitions
            ProcessFileGroup(dir, "StructureDefinition", ref fhirVersionInfo, ref processedFiles);

            // process search parameters (adds to resources)
            ProcessFileGroup(dir, "SearchParameter", ref fhirVersionInfo, ref processedFiles);

            // process operations (adds to resources and version info (server level))
            ProcessFileGroup(dir, "OperationDefinition", ref fhirVersionInfo, ref processedFiles);

            if (fhirVersionInfo.ConverterHasIssues(out int errorCount, out int warningCount))
            {
                // make sure we cleared the last line
                Console.WriteLine($"LoadPackage <<< Loaded and Parsed {packageName}-{packageVersion}" +
                    $" with {errorCount} errors" +
                    $" and {warningCount} warnings" +
                    $"{new string(' ', 100)}");
                fhirVersionInfo.DisplayConverterIssues();
            }
            else
            {
                // make sure we cleared the last line
                Console.WriteLine($"LoadPackage <<< Loaded and Parsed {packageName}-{packageVersion}{new string(' ', 100)}");
            }
        }

        /// <summary>
        /// Process a file group, specified by the file prefix (e.g., StructureDefinition).
        /// </summary>
        /// <param name="packageDir">     The package dir.</param>
        /// <param name="prefix">         The prefix.</param>
        /// <param name="fhirVersionInfo">[in,out] Information describing the fhir version.</param>
        /// <param name="processedFiles"> [in,out] The processed files.</param>
        private static void ProcessFileGroup(
            string packageDir,
            string prefix,
            ref FhirVersionInfo fhirVersionInfo,
            ref HashSet<string> processedFiles)
        {
            // get the files in this directory
            string[] files = Directory.GetFiles(packageDir, $"{prefix}*.json", SearchOption.TopDirectoryOnly);

            // process these files
            ProcessPackageFiles(files, ref fhirVersionInfo, ref processedFiles);
        }

        /// <summary>Process the package files.</summary>
        /// <exception cref="InvalidDataException">Thrown when an Invalid Data error condition occurs.</exception>
        /// <param name="files">          The files.</param>
        /// <param name="fhirVersionInfo">[in,out] Information describing the fhir version.</param>
        /// <param name="processedFiles"> [in,out] The processed files.</param>
        private static void ProcessPackageFiles(
            string[] files,
            ref FhirVersionInfo fhirVersionInfo,
            ref HashSet<string> processedFiles)
        {
            // traverse the files
            foreach (string filename in files)
            {
                // check for skipping file
                if (fhirVersionInfo.ShouldSkipFile(Path.GetFileName(filename)))
                {
                    // skip this file
                    continue;
                }

                // parse the name into parts we want
                string shortName = Path.GetFileNameWithoutExtension(filename);
                string[] components = shortName.Split('-');
                string resourceHint = components[0];
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

                    if (processedFiles.Contains(shortName))
                    {
                        // skip
                        continue;
                    }

                    processedFiles.Add(shortName);

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
        }
    }
}
