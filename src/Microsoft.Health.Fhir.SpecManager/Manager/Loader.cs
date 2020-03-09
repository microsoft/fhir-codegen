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
        /// <param name="fhirSpecDirectory">    Pathname of the FHIR spec directory.</param>
        /// <param name="versionInfo">     Information describing the version.</param>
        /// <param name="versionDirectory">[out] Pathname of the version directory.</param>
        /// <returns>True if it succeeds, false if it fails.</returns>
        public static bool TryFindPackage(
            string fhirSpecDirectory,
            FhirVersionInfo versionInfo,
            out string versionDirectory)
        {
            versionDirectory = null;

            // sanity checks
            if (versionInfo == null)
            {
                throw new ArgumentNullException(nameof(versionInfo));
            }

            // check for finding the directory
            string packageDir = Path.Combine(
                fhirSpecDirectory,
                $"{versionInfo.PackageName}-{versionInfo.VersionString}",
                "package");

            if (!Directory.Exists(packageDir))
            {
                // check for NPM installed version
                packageDir = Path.Combine(fhirSpecDirectory, "node_modules", versionInfo.PackageName);

                if (!Directory.Exists(packageDir))
                {
                    Console.WriteLine($"Cannot find R{versionInfo.ReleaseName}-{versionInfo.PackageName} in {fhirSpecDirectory}");
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
        /// <param name="fhirSpecDirectory">   Pathname of the FHIR spec directory.</param>
        /// <param name="fhirVersionInfo">[in,out] Information describing the FHIR version.</param>
        public static void LoadPackage(string fhirSpecDirectory, ref FhirVersionInfo fhirVersionInfo)
        {
            // sanity checks
            if (fhirVersionInfo == null)
            {
                Console.WriteLine($"LoadPackage <<< invalid version info is NULL, cannot load {fhirSpecDirectory}");
                throw new ArgumentNullException(nameof(fhirVersionInfo));
            }

            // find the package
            if (!TryFindPackage(fhirSpecDirectory, fhirVersionInfo, out string packageDir))
            {
                Console.WriteLine($"LoadPackage <<< cannot find package for {fhirVersionInfo.ReleaseName}!");
                throw new FileNotFoundException($"Cannot find package for {fhirVersionInfo.ReleaseName}");
            }

            // load package info
            FhirPackageInfo packageInfo = FhirPackageInfo.Load(packageDir);

            // tell the user what's going on
            Console.WriteLine($"LoadPackage <<< Found: {packageInfo.Name} version: {packageInfo.Version}");

            // update our structure
            fhirVersionInfo.VersionString = packageInfo.Version;

            // process structure definitions (want types and resources)
            ProcessFileGroup(
                packageDir,
                "StructureDefinition",
                ref fhirVersionInfo);

            // process structure definitions for extensions
            ProcessFileGroup(
                packageDir,
                "StructureDefinition",
                ref fhirVersionInfo,
                processHint: "Extension");

            // process search parameters (adds to resources)
            ProcessFileGroup(packageDir, "SearchParameter", ref fhirVersionInfo);

            // process operations (adds to resources and version info (server level))
            ProcessFileGroup(packageDir, "OperationDefinition", ref fhirVersionInfo);

            // add version-specific "MAGIC" items
            AddSearchMagicParameters(ref fhirVersionInfo);

            // make sure we cleared the last line
            Console.WriteLine($"LoadPackage <<< Loaded and Parsed FHIR {fhirVersionInfo.ReleaseName}{new string(' ', 100)}");
        }

        /// <summary>Adds the search magic parameters.</summary>
        /// <param name="info">[in,out] The information.</param>
        private static void AddSearchMagicParameters(ref FhirVersionInfo info)
        {
            switch (info.MajorVersion)
            {
                case 2:
                    info.AddVersionedParam(FhirVersionInfo.SearchMagicParameter.Global, "_content", "string");
                    info.AddVersionedParam(FhirVersionInfo.SearchMagicParameter.Global, "_list", "string");

                    info.AddVersionedParam(FhirVersionInfo.SearchMagicParameter.Result, "_sort", "string");
                    info.AddVersionedParam(FhirVersionInfo.SearchMagicParameter.Result, "_count", "string");
                    info.AddVersionedParam(FhirVersionInfo.SearchMagicParameter.Result, "_include", "string");
                    info.AddVersionedParam(FhirVersionInfo.SearchMagicParameter.Result, "_revinclude", "string");
                    info.AddVersionedParam(FhirVersionInfo.SearchMagicParameter.Result, "_summary", "string");
                    info.AddVersionedParam(FhirVersionInfo.SearchMagicParameter.Result, "_elements", "string");
                    info.AddVersionedParam(FhirVersionInfo.SearchMagicParameter.Result, "_contained", "string");
                    info.AddVersionedParam(FhirVersionInfo.SearchMagicParameter.Result, "_containedType", "string");

                    info.AddVersionedParam(FhirVersionInfo.SearchMagicParameter.Interaction, "_format", "string");
                    break;

                case 3:
                    info.AddVersionedParam(FhirVersionInfo.SearchMagicParameter.Global, "_text", "string");
                    info.AddVersionedParam(FhirVersionInfo.SearchMagicParameter.Global, "_content", "string");
                    info.AddVersionedParam(FhirVersionInfo.SearchMagicParameter.Global, "_list", "string");
                    info.AddVersionedParam(FhirVersionInfo.SearchMagicParameter.Global, "_has", "string");
                    info.AddVersionedParam(FhirVersionInfo.SearchMagicParameter.Global, "_type", "string");

                    info.AddVersionedParam(FhirVersionInfo.SearchMagicParameter.Result, "_sort", "string");
                    info.AddVersionedParam(FhirVersionInfo.SearchMagicParameter.Result, "_count", "string");
                    info.AddVersionedParam(FhirVersionInfo.SearchMagicParameter.Result, "_include", "string");
                    info.AddVersionedParam(FhirVersionInfo.SearchMagicParameter.Result, "_revinclude", "string");
                    info.AddVersionedParam(FhirVersionInfo.SearchMagicParameter.Result, "_summary", "string");
                    info.AddVersionedParam(FhirVersionInfo.SearchMagicParameter.Result, "_elements", "string");
                    info.AddVersionedParam(FhirVersionInfo.SearchMagicParameter.Result, "_contained", "string");
                    info.AddVersionedParam(FhirVersionInfo.SearchMagicParameter.Result, "_containedType", "string");

                    info.AddVersionedParam(FhirVersionInfo.SearchMagicParameter.Interaction, "_format", "string");

                    break;

                case 4:
                default:
                    info.AddVersionedParam(FhirVersionInfo.SearchMagicParameter.Global, "_text", "string");
                    info.AddVersionedParam(FhirVersionInfo.SearchMagicParameter.Global, "_content", "string");
                    info.AddVersionedParam(FhirVersionInfo.SearchMagicParameter.Global, "_list", "string");
                    info.AddVersionedParam(FhirVersionInfo.SearchMagicParameter.Global, "_has", "string");
                    info.AddVersionedParam(FhirVersionInfo.SearchMagicParameter.Global, "_type", "string");

                    info.AddVersionedParam(FhirVersionInfo.SearchMagicParameter.Result, "_sort", "string");
                    info.AddVersionedParam(FhirVersionInfo.SearchMagicParameter.Result, "_count", "string");
                    info.AddVersionedParam(FhirVersionInfo.SearchMagicParameter.Result, "_include", "string");
                    info.AddVersionedParam(FhirVersionInfo.SearchMagicParameter.Result, "_revinclude", "string");
                    info.AddVersionedParam(FhirVersionInfo.SearchMagicParameter.Result, "_summary", "string");
                    info.AddVersionedParam(FhirVersionInfo.SearchMagicParameter.Result, "_total", "string");
                    info.AddVersionedParam(FhirVersionInfo.SearchMagicParameter.Result, "_elements", "string");
                    info.AddVersionedParam(FhirVersionInfo.SearchMagicParameter.Result, "_contained", "string");
                    info.AddVersionedParam(FhirVersionInfo.SearchMagicParameter.Result, "_containedType", "string");

                    info.AddVersionedParam(FhirVersionInfo.SearchMagicParameter.Interaction, "_format", "string");
                    info.AddVersionedParam(FhirVersionInfo.SearchMagicParameter.Interaction, "_pretty", "string");

                    break;
            }
        }

        /// <summary>
        /// Process a file group, specified by the file prefix (e.g., StructureDefinition).
        /// </summary>
        /// <param name="packageDir">     The package dir.</param>
        /// <param name="prefix">         The prefix.</param>
        /// <param name="fhirVersionInfo">[in,out] Information describing the fhir version.</param>
        /// <param name="processHint">    (Optional) Additional inclusion criteria.</param>
        private static void ProcessFileGroup(
            string packageDir,
            string prefix,
            ref FhirVersionInfo fhirVersionInfo,
            string processHint = "")
        {
            // get the files in this directory
            string[] files = Directory.GetFiles(packageDir, $"{prefix}*.json", SearchOption.TopDirectoryOnly);

            // process these files
            ProcessPackageFiles(files, ref fhirVersionInfo, processHint);
        }

        /// <summary>Process the package files.</summary>
        /// <exception cref="InvalidDataException">Thrown when an Invalid Data error condition occurs.</exception>
        /// <param name="files">          The files.</param>
        /// <param name="fhirVersionInfo">[in,out] Information describing the fhir version.</param>
        /// <param name="processHint">    (Optional) Additional inclusion criteria.</param>
        private static void ProcessPackageFiles(
            string[] files,
            ref FhirVersionInfo fhirVersionInfo,
            string processHint = "")
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
                string[] components = shortName.Split('-');
                string resourceHint = components[0];
                string resourceName = shortName.Substring(resourceHint.Length + 1);

                // attempt to load this file
                try
                {
                    Console.Write($"v{fhirVersionInfo.MajorVersion}: {shortName,-85}\r");

                    // TODO: this feels hacky - figure out a better way
                    // check for SDs to skip based on number of components + extensions flag
                    if (resourceHint.Equals("StructureDefinition", StringComparison.Ordinal))
                    {
                        if (string.IsNullOrEmpty(processHint) && (components.Length > 2))
                        {
                            continue;
                        }

                        if ((components.Length == 2) &&
                            (processHint == "Extension") &&
                            (fhirVersionInfo.ComplexTypes.ContainsKey(components[1]) ||
                             fhirVersionInfo.Resources.ContainsKey(components[1])))
                        {
                            continue;
                        }
                    }

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
                    fhirVersionInfo.ProcessResource(resource, processHint);
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
