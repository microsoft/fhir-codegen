// <copyright file="FhirPackageLoader.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Health.Fhir.SpecManager.Models;
using Microsoft.Health.Fhir.SpecManager.PackageManager;

namespace Microsoft.Health.Fhir.SpecManager.Manager;

/// <summary>A FHIR package loader (e.g., hl7.fhir.us.core).</summary>
public static class FhirPackageLoader
{
    /// <summary>(Immutable) The definitional resource types to load.</summary>
    public static readonly string[] DefinitionalResourceTypesToLoad = new string[]
    {
        "CodeSystem",
        "ValueSet",
        "StructureDefinition",
        "SearchParameter",
        "OperationDefinition",
    };

    /// <summary>Loads.</summary>
    /// <exception cref="ArgumentNullException">     Thrown when one or more required arguments are
    ///  null.</exception>
    /// <exception cref="DirectoryNotFoundException">Thrown when the requested directory is not
    ///  present.</exception>
    /// <param name="directive">  The directive.</param>
    /// <param name="directory">  Pathname of the directory.</param>
    /// <param name="packageInfo">[out] The information.</param>
    public static void Load(
        string directive,
        string directory,
        out FhirVersionInfo packageInfo)
    {
        if (string.IsNullOrEmpty(directive))
        {
            throw new ArgumentNullException(nameof(directive));
        }

        if (string.IsNullOrEmpty(directory))
        {
            throw new ArgumentNullException(nameof(directory));
        }

        string contentDirectory = Path.Combine(directory, "package");

        if (!Directory.Exists(contentDirectory))
        {
            throw new DirectoryNotFoundException($"Missing package content directory: {contentDirectory}");
        }

        packageInfo = new FhirVersionInfo(directory);

        HashSet<string> processedFiles = new HashSet<string>();

        bool checkUnescaped = false;

        if (packageInfo.VersionString.Equals("3.5.0", StringComparison.Ordinal))
        {
            checkUnescaped = true;
        }

        // process Code Systems
        ProcessFileGroup(contentDirectory, "CodeSystem", packageInfo, processedFiles, checkUnescaped);

        // process Value Set expansions
        ProcessFileGroup(contentDirectory, "ValueSet", packageInfo, processedFiles, checkUnescaped);

        // process structure definitions
        ProcessFileGroup(contentDirectory, "StructureDefinition", packageInfo, processedFiles, checkUnescaped);

        // process search parameters (adds to resources)
        ProcessFileGroup(contentDirectory, "SearchParameter", packageInfo, processedFiles, checkUnescaped);

        // process operations (adds to resources and version info (server level))
        ProcessFileGroup(contentDirectory, "OperationDefinition", packageInfo, processedFiles, checkUnescaped);

        if (packageInfo.ConverterHasIssues(out int errorCount, out int warningCount))
        {
            // make sure we cleared the last line
            Console.WriteLine($"LoadCached <<< Loaded and Parsed {directive}" +
                $" with {errorCount} errors" +
                $" and {warningCount} warnings" +
                $"{new string(' ', 100)}");
            packageInfo.DisplayConverterIssues();
        }
        else
        {
            // make sure we cleared the last line
            Console.WriteLine($"LoadCached <<< Loaded and Parsed {directive}{new string(' ', 100)}");
        }
    }

    /// <summary>
    /// Process a file group, specified by the file prefix (e.g., StructureDefinition).
    /// </summary>
    /// <param name="packageDir">    The package dir.</param>
    /// <param name="prefix">        The prefix.</param>
    /// <param name="fhirCoreInfo">  Information describing the fhir version.</param>
    /// <param name="processedFiles">The processed files.</param>
    /// <param name="checkUnescaped">True if check unescaped.</param>
    private static void ProcessFileGroup(
        string packageDir,
        string prefix,
        IPackageImportable fhirCoreInfo,
        HashSet<string> processedFiles,
        bool checkUnescaped)
    {
        // get the files in this directory
        string[] files = Directory.GetFiles(packageDir, $"{prefix}*.json", SearchOption.TopDirectoryOnly);

        // process these files
        ProcessPackageFiles(files, fhirCoreInfo, processedFiles, checkUnescaped);
    }

    /// <summary>Process the package files.</summary>
    /// <exception cref="InvalidDataException">Thrown when an Invalid Data error condition occurs.</exception>
    /// <param name="files">         The files.</param>
    /// <param name="fhirInfo">      FHIR information structure.</param>
    /// <param name="processedFiles">The processed files.</param>
    /// <param name="checkUnescaped">True if check unescaped.</param>
    private static void ProcessPackageFiles(
        string[] files,
        IPackageImportable fhirInfo,
        HashSet<string> processedFiles,
        bool checkUnescaped)
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
                Console.Write($"{fhirInfo.FhirSequence}: {shortName,-85}\r");

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

                if (checkUnescaped)
                {
                    Regex regex = new Regex("[\\s]");
                    contents = regex.Replace(contents, " ");
                }

                // parse the file - note: using var here is siginificantly more performant than object
                var resource = fhirInfo.ParseResource(contents);

                // check type matching
                //if (!resource.GetType().Name.Equals(resourceHint, StringComparison.Ordinal))
                //{
                //    // type not found
                //    Console.WriteLine($"\nProcessPackageFiles <<<" +
                //        $" Mismatched type: {shortName}," +
                //        $" should be {resourceHint} parsed to:{resource.GetType().Name}");
                //    throw new InvalidDataException($"Mismatched type: {shortName}: {resourceHint} != {resource.GetType().Name}");
                //}

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
