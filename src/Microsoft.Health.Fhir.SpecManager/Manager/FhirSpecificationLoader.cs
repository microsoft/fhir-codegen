// <copyright file="FhirSpecificationLoader.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.IO;
using Microsoft.Health.Fhir.SpecManager.Models;

namespace Microsoft.Health.Fhir.SpecManager.Manager;

/// <summary>A FHIR Specification Package loader (e.g., R4).</summary>
public abstract class FhirSpecificationLoader
{
    /// <summary>Loads a core specification.</summary>
    /// <exception cref="ArgumentNullException">     Thrown when one or more required arguments are
    ///  null.</exception>
    /// <exception cref="DirectoryNotFoundException">Thrown when the requested directory is not
    ///  present.</exception>
    /// <param name="packageInfo">           [in,out] The package info.</param>
    /// <param name="coreDirectory">         Pathname of the core directory.</param>
    /// <param name="expansionDirectory">    Pathname of the expansion directory.</param>
    /// <param name="officialExpansionsOnly">True to official expansions only.</param>
    public static void Load(
        IPackageImportable packageInfo,
        string coreDirectory,
        string expansionDirectory,
        bool officialExpansionsOnly)
    {
        if (string.IsNullOrEmpty(coreDirectory))
        {
            throw new ArgumentNullException(nameof(coreDirectory));
        }

        string corePackageDir = Path.Combine(coreDirectory, "package");

        if (!Directory.Exists(corePackageDir))
        {
            throw new DirectoryNotFoundException($"Missing core package directory: {corePackageDir}");
        }

        if (string.IsNullOrEmpty(expansionDirectory))
        {
            throw new ArgumentNullException(nameof(expansionDirectory));
        }

        string expansionPackageDir = Path.Combine(expansionDirectory, "package");

        if (!Directory.Exists(expansionPackageDir))
        {
            throw new DirectoryNotFoundException($"Missing expansions package directory: {expansionPackageDir}");
        }

        HashSet<string> processedFiles = new HashSet<string>();

        // process Code Systems
        ProcessFileGroup(corePackageDir, "CodeSystem", packageInfo, processedFiles);

        // process Value Set expansions
        ProcessFileGroup(expansionPackageDir, "ValueSet", packageInfo, processedFiles);

        // process other value set definitions (if requested)
        if (!officialExpansionsOnly)
        {
            ProcessFileGroup(corePackageDir, "ValueSet", packageInfo, processedFiles);
        }

        // process structure definitions
        ProcessFileGroup(corePackageDir, "StructureDefinition", packageInfo, processedFiles);

        // process search parameters (adds to resources)
        ProcessFileGroup(corePackageDir, "SearchParameter", packageInfo, processedFiles);

        // process operations (adds to resources and version info (server level))
        ProcessFileGroup(corePackageDir, "OperationDefinition", packageInfo, processedFiles);

        // add version-specific "MAGIC" items
        AddSearchMagicParameters(packageInfo);

        if (packageInfo.ConverterHasIssues(out int errorCount, out int warningCount))
        {
            // make sure we cleared the last line
            Console.WriteLine($"LoadCached <<< Loaded and Parsed FHIR {packageInfo.ReleaseName}" +
                $" with {errorCount} errors" +
                $" and {warningCount} warnings" +
                $"{new string(' ', 100)}");
            packageInfo.DisplayConverterIssues();
        }
        else
        {
            // make sure we cleared the last line
            Console.WriteLine($"LoadCached <<< Loaded and Parsed FHIR {packageInfo.ReleaseName}{new string(' ', 100)}");
        }
    }

    /// <summary>Adds the search magic parameters.</summary>
    /// <param name="info">[in,out] The information.</param>
    private static void AddSearchMagicParameters(IPackageImportable info)
    {
        switch (info.FhirSequence)
        {
            case FhirPackageCommon.FhirSequenceEnum.DSTU2:
                info.AddVersionedParam(FhirSearchParam.ParameterGrouping.Global, "_content", "string");
                info.AddVersionedParam(FhirSearchParam.ParameterGrouping.Global, "_list", "string");

                info.AddVersionedParam(FhirSearchParam.ParameterGrouping.Result, "_sort", "string");
                info.AddVersionedParam(FhirSearchParam.ParameterGrouping.Result, "_count", "string");
                info.AddVersionedParam(FhirSearchParam.ParameterGrouping.Result, "_include", "string");
                info.AddVersionedParam(FhirSearchParam.ParameterGrouping.Result, "_revinclude", "string");
                info.AddVersionedParam(FhirSearchParam.ParameterGrouping.Result, "_summary", "string");
                info.AddVersionedParam(FhirSearchParam.ParameterGrouping.Result, "_elements", "string");
                info.AddVersionedParam(FhirSearchParam.ParameterGrouping.Result, "_contained", "string");
                info.AddVersionedParam(FhirSearchParam.ParameterGrouping.Result, "_containedType", "string");

                info.AddVersionedParam(FhirSearchParam.ParameterGrouping.Interaction, "_format", "string");
                break;

            case FhirPackageCommon.FhirSequenceEnum.STU3:
                info.AddVersionedParam(FhirSearchParam.ParameterGrouping.Global, "_text", "string");
                info.AddVersionedParam(FhirSearchParam.ParameterGrouping.Global, "_content", "string");
                info.AddVersionedParam(FhirSearchParam.ParameterGrouping.Global, "_list", "string");
                info.AddVersionedParam(FhirSearchParam.ParameterGrouping.Global, "_has", "string");
                info.AddVersionedParam(FhirSearchParam.ParameterGrouping.Global, "_type", "string");

                info.AddVersionedParam(FhirSearchParam.ParameterGrouping.Result, "_sort", "string");
                info.AddVersionedParam(FhirSearchParam.ParameterGrouping.Result, "_count", "string");
                info.AddVersionedParam(FhirSearchParam.ParameterGrouping.Result, "_include", "string");
                info.AddVersionedParam(FhirSearchParam.ParameterGrouping.Result, "_revinclude", "string");
                info.AddVersionedParam(FhirSearchParam.ParameterGrouping.Result, "_summary", "string");
                info.AddVersionedParam(FhirSearchParam.ParameterGrouping.Result, "_elements", "string");
                info.AddVersionedParam(FhirSearchParam.ParameterGrouping.Result, "_contained", "string");
                info.AddVersionedParam(FhirSearchParam.ParameterGrouping.Result, "_containedType", "string");

                info.AddVersionedParam(FhirSearchParam.ParameterGrouping.Interaction, "_format", "string");

                break;

            case FhirPackageCommon.FhirSequenceEnum.R4:
            case FhirPackageCommon.FhirSequenceEnum.R4B:
            case FhirPackageCommon.FhirSequenceEnum.R5:
            default:
                info.AddVersionedParam(FhirSearchParam.ParameterGrouping.Global, "_text", "string");
                info.AddVersionedParam(FhirSearchParam.ParameterGrouping.Global, "_content", "string");
                info.AddVersionedParam(FhirSearchParam.ParameterGrouping.Global, "_list", "string");
                info.AddVersionedParam(FhirSearchParam.ParameterGrouping.Global, "_has", "string");
                info.AddVersionedParam(FhirSearchParam.ParameterGrouping.Global, "_type", "string");

                info.AddVersionedParam(FhirSearchParam.ParameterGrouping.Result, "_sort", "string");
                info.AddVersionedParam(FhirSearchParam.ParameterGrouping.Result, "_count", "string");
                info.AddVersionedParam(FhirSearchParam.ParameterGrouping.Result, "_include", "string");
                info.AddVersionedParam(FhirSearchParam.ParameterGrouping.Result, "_revinclude", "string");
                info.AddVersionedParam(FhirSearchParam.ParameterGrouping.Result, "_summary", "string");
                info.AddVersionedParam(FhirSearchParam.ParameterGrouping.Result, "_total", "string");
                info.AddVersionedParam(FhirSearchParam.ParameterGrouping.Result, "_elements", "string");
                info.AddVersionedParam(FhirSearchParam.ParameterGrouping.Result, "_contained", "string");
                info.AddVersionedParam(FhirSearchParam.ParameterGrouping.Result, "_containedType", "string");

                info.AddVersionedParam(FhirSearchParam.ParameterGrouping.Interaction, "_format", "string");
                info.AddVersionedParam(FhirSearchParam.ParameterGrouping.Interaction, "_pretty", "string");

                break;
        }
    }

    /// <summary>
    /// Process a file group, specified by the file prefix (e.g., StructureDefinition).
    /// </summary>
    /// <param name="packageDir">    The package dir.</param>
    /// <param name="prefix">        The prefix.</param>
    /// <param name="packageInfo">   [in,out] The package info.</param>
    /// <param name="processedFiles">[in,out] The processed files.</param>
    private static void ProcessFileGroup(
        string packageDir,
        string prefix,
        IPackageImportable packageInfo,
        HashSet<string> processedFiles)
    {
        // get the files in this directory
        string[] files = Directory.GetFiles(packageDir, $"{prefix}*.json", SearchOption.TopDirectoryOnly);

        // process these files
        ProcessPackageFiles(files, packageInfo, processedFiles);
    }

    /// <summary>Process the package files.</summary>
    /// <exception cref="InvalidDataException">Thrown when an Invalid Data error condition occurs.</exception>
    /// <param name="files">         The files.</param>
    /// <param name="packageInfo">   [in,out] Information describing the fhir version.</param>
    /// <param name="processedFiles">[in,out] The processed files.</param>
    private static void ProcessPackageFiles(
        string[] files,
        IPackageImportable packageInfo,
        HashSet<string> processedFiles)
    {
        // traverse the files
        foreach (string filename in files)
        {
            // check for skipping file
            if (packageInfo.ShouldSkipFile(Path.GetFileName(filename)))
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
                Console.Write($"{packageInfo.FhirSequence}: {shortName,-85}\r");

                // check for ignored types
                if (packageInfo.ShouldIgnoreResource(resourceHint))
                {
                    // skip
                    continue;
                }

                // this should be listed in process types (validation check)
                if (!packageInfo.ShouldProcessResource(resourceHint))
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

                // Note: this is faster than creating a FileStream and passing that object through
                // and uses less total memory.
                // Also, don't use ReadAllBytes here, since some DSTU2 stuff is encoded differently
                // and the code needs to be modified to handle both cases.
                string contents = File.ReadAllText(filename);

                // parse the file - note: using var here is siginificantly more performant than object
                var resource = packageInfo.ParseResource(contents);

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
                packageInfo.ProcessResource(resource);
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
