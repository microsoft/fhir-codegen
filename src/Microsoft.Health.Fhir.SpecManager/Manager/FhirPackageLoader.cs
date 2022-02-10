// <copyright file="FhirPackageLoader.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.IO;
using Microsoft.Health.Fhir.SpecManager.Models;

namespace Microsoft.Health.Fhir.SpecManager.Manager;

/// <summary>A FHIR package loader (e.g., hl7.fhir.us.core).</summary>
public static class FhirPackageLoader
{
    /// <summary>Attempts to find a FHIRpackage.</summary>
    /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
    /// <param name="packageName">     Name of the package.</param>
    /// <param name="version">         The version.</param>
    /// <param name="packageDirectory">Pathname of the package directory.</param>
    /// <param name="versionDirectory">[out] Pathname of the version directory.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    private static bool TryFindPackage(
        string packageName,
        string version,
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

    /// <summary>A fhirCoreInfo extension method that attempts to load packages.</summary>
    /// <param name="fhirCoreInfo">  Information describing the fhir version.</param>
    /// <param name="packageDirectives">The package directives.</param>
    /// <param name="packagesLoaded">   [out] The packages loaded.</param>
    /// <param name="packagesFailed">   [out] The packages failed.</param>
    /// <returns>True if all packages loaded, false if one or more failed.</returns>
    public static bool TryLoadPackages(
        this FhirVersionInfo fhirCoreInfo,
        string[] packageDirectives,
        out List<FhirGuideInfo> packagesLoaded,
        out List<string> packagesFailed)
    {
        if (fhirCoreInfo == null)
        {
            Console.WriteLine($"LoadPackage <<< {nameof(fhirCoreInfo)} is NULL, cannot load packages: {packageDirectives}!");
            packagesLoaded = null;
            packagesFailed = null;
            return false;
        }

        packagesLoaded = new ();
        packagesFailed = new ();

        if ((packageDirectives == null) || (packageDirectives.Length == 0))
        {
            return true;
        }

        foreach (string packageDirective in packageDirectives)
        {
            if (TryLoadPackage(
                    fhirCoreInfo,
                    packageDirective,
                    out string loadedDirective,
                    out FhirGuideInfo ig))
            {
                packagesLoaded.Add(ig);
            }
            else
            {
                packagesFailed.Add(packageDirective);
            }
        }

        return true;
    }

    /// <summary>Loads a FHIR package (e.g., hl7.fhir.us.core-4.0.0).</summary>
    /// <param name="fhirCoreInfo">          Information describing the fhir version.</param>
    /// <param name="packageDirective">      Name of the package, may inlcude a version.</param>
    /// <param name="loadedPackageDirective">[out] The loaded package directive.</param>
    /// <param name="igInfo">                [out] Information describing the ig.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public static bool TryLoadPackage(
        this FhirVersionInfo fhirCoreInfo,
        string packageDirective,
        out string loadedPackageDirective,
        out FhirGuideInfo igInfo)
    {
        // sanity checks
        if (fhirCoreInfo == null)
        {
            Console.WriteLine($"LoadPackage <<< {nameof(fhirCoreInfo)} is NULL, cannot load {packageDirective}!");
            loadedPackageDirective = string.Empty;
            igInfo = null;
            return false;
        }

        if (string.IsNullOrEmpty(packageDirective))
        {
#pragma warning disable CA1303 // Do not pass literals as localized parameters
            Console.WriteLine($"LoadPackage <<< {nameof(packageDirective)} is required!");
#pragma warning restore CA1303 // Do not pass literals as localized parameters
            loadedPackageDirective = string.Empty;
            igInfo = null;
            return false;
        }

        string packageName = string.Empty;
        string packageVersion = string.Empty;

        if (packageDirective.Contains('-', StringComparison.Ordinal))
        {
            string[] parts = packageDirective.Split('-');

            if (parts.Length != 2)
            {
                Console.WriteLine($"LoadPackage <<< unable to parse package directive: {packageDirective}!");
                loadedPackageDirective = string.Empty;
                igInfo = null;
                return false;
            }

            packageName = parts[0];
            packageVersion = parts[1];
        }
        else
        {
            // need to get package info from the registry
            packageName = packageDirective;
        }

        string versionedPackageDirectory;

        // need to download if we don't have an explicit version that we have cached
        if (string.IsNullOrEmpty(packageVersion) ||
            (!TryFindPackage(
                packageName,
                packageVersion,
                FhirManager.Current.FhirPackageDirectory,
                out versionedPackageDirectory)))
        {
            if (!FhirPackageDownloader.DownloadFhirPackage(
                packageName,
                ref packageVersion,
                fhirCoreInfo.MajorVersion,
                FhirManager.Current.FhirPackageDirectory,
                out versionedPackageDirectory))
            {
                Console.WriteLine($"LoadPackage <<< cannot downlaod package for {fhirCoreInfo.ReleaseName}: {packageDirective}!");
                loadedPackageDirective = string.Empty;
                igInfo = null;
                return false;
            }
        }

        loadedPackageDirective = packageName + "-" + packageVersion;

        // load package info
        FhirPackage packageInfo = FhirPackage.Load(versionedPackageDirectory);

        // tell the user what's going on
        Console.WriteLine($"LoadPackage <<< Found: {packageInfo.Name} version: {packageInfo.Version}");

        // create our structure
        igInfo = new FhirGuideInfo(fhirCoreInfo, packageInfo);

        HashSet<string> processedFiles = new HashSet<string>();



        // process Code Systems
        ProcessFileGroup(versionedPackageDirectory, "CodeSystem", igInfo, processedFiles);

        // process Value Set expansions
        ProcessFileGroup(versionedPackageDirectory, "ValueSet", igInfo, processedFiles);

        // process structure definitions
        ProcessFileGroup(versionedPackageDirectory, "StructureDefinition", igInfo, processedFiles);

        // process search parameters (adds to resources)
        ProcessFileGroup(versionedPackageDirectory, "SearchParameter", igInfo, processedFiles);

        // process operations (adds to resources and version info (server level))
        ProcessFileGroup(versionedPackageDirectory, "OperationDefinition", igInfo, processedFiles);

        if (fhirCoreInfo.ConverterHasIssues(out int errorCount, out int warningCount))
        {
            // make sure we cleared the last line
            Console.WriteLine($"TryLoadPackage <<< Loaded and Parsed {packageName}-{packageVersion}" +
                $" with {errorCount} errors" +
                $" and {warningCount} warnings" +
                $"{new string(' ', 100)}");
            fhirCoreInfo.DisplayConverterIssues();
        }
        else
        {
            // make sure we cleared the last line
            Console.WriteLine($"TryLoadPackage <<< Loaded and Parsed {packageName}-{packageVersion}{new string(' ', 100)}");
        }

        return true;
    }

    /// <summary>
    /// Process a file group, specified by the file prefix (e.g., StructureDefinition).
    /// </summary>
    /// <param name="packageDir">    The package dir.</param>
    /// <param name="prefix">        The prefix.</param>
    /// <param name="fhirCoreInfo">  Information describing the fhir version.</param>
    /// <param name="processedFiles">The processed files.</param>
    private static void ProcessFileGroup(
        string packageDir,
        string prefix,
        IFhirInfo fhirCoreInfo,
        HashSet<string> processedFiles)
    {
        // get the files in this directory
        string[] files = Directory.GetFiles(packageDir, $"{prefix}*.json", SearchOption.TopDirectoryOnly);

        // process these files
        ProcessPackageFiles(files, fhirCoreInfo, processedFiles);
    }

    /// <summary>Process the package files.</summary>
    /// <exception cref="InvalidDataException">Thrown when an Invalid Data error condition occurs.</exception>
    /// <param name="files">         The files.</param>
    /// <param name="fhirInfo">      FHIR information structure.</param>
    /// <param name="processedFiles">The processed files.</param>
    private static void ProcessPackageFiles(
        string[] files,
        IFhirInfo fhirInfo,
        HashSet<string> processedFiles)
    {
        // traverse the files
        foreach (string filename in files)
        {
            // check for skipping file
            if (fhirInfo.ShouldSkipFile(Path.GetFileName(filename)))
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
                Console.Write($"{fhirInfo.MajorVersionEnum}: {shortName,-85}\r");

                // check for ignored types
                if (fhirInfo.ShouldIgnoreResource(resourceHint))
                {
                    // skip
                    continue;
                }

                // this should be listed in process types (validation check)
                if (!fhirInfo.ShouldProcessResource(resourceHint))
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
                var resource = fhirInfo.ParseResource(contents);

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
                fhirInfo.ProcessResource(resource);
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Empty);
                Console.WriteLine($"nProcessPackageFiles <<< Failed to process file: {filename}: \n{ex}\n--------------");
                throw;
            }
        }
    }
}
