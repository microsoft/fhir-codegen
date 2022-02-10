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
    /// <summary>Searches for a specification package.</summary>
    /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
    /// <param name="releaseName">      The release name (e.g., R4, DSTU2).</param>
    /// <param name="packageName">      Name of the package.</param>
    /// <param name="version">          The version string (e.g., 4.0.1).</param>
    /// <param name="fhirSpecDirectory">Pathname of the FHIR spec directory.</param>
    /// <param name="versionDirectory"> [out] Pathname of the version directory.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public static bool TryFindPackage(
        string releaseName,
        string packageName,
        string version,
        string fhirSpecDirectory,
        out string versionDirectory)
    {
        versionDirectory = null;

        if (string.IsNullOrEmpty(fhirSpecDirectory))
        {
            throw new ArgumentNullException(nameof(fhirSpecDirectory));
        }

        if (string.IsNullOrEmpty(releaseName))
        {
            throw new ArgumentNullException(nameof(releaseName));
        }

        if (string.IsNullOrEmpty(packageName))
        {
            throw new ArgumentNullException(nameof(packageName));
        }

        if (string.IsNullOrEmpty(version))
        {
            throw new ArgumentNullException(nameof(version));
        }

        // check for finding the directory
        string packageDir = Path.Combine(
            fhirSpecDirectory,
            $"{packageName}-{version}",
            "package");

        if (!Directory.Exists(packageDir))
        {
            // check for NPM installed version
            packageDir = Path.Combine(fhirSpecDirectory, "node_modules", packageName);

            if (!Directory.Exists(packageDir))
            {
                Console.WriteLine($"Cannot find {releaseName}-{packageName} in {fhirSpecDirectory}");
                return false;
            }
        }

        // set our directory
        versionDirectory = packageDir;
        return true;
    }

    /// <summary>Loads a local build of the FHIR specification.</summary>
    /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
    /// <param name="localPublishDirectory"> Local FHIR Publish directory.</param>
    /// <param name="fhirSpecDirectory">     Pathname of the FHIR spec directory.</param>
    /// <param name="fhirCoreInfo">       [in,out] Information describing the FHIR version.</param>
    /// <param name="localLoadType">         Type of the local load.</param>
    /// <param name="officialExpansionsOnly">True to official expansions only.</param>
    public static void LoadLocalBuild(
        string localPublishDirectory,
        string fhirSpecDirectory,
        ref FhirVersionInfo fhirCoreInfo,
        string localLoadType,
        bool officialExpansionsOnly)
    {
        // sanity checks
        if (fhirCoreInfo == null)
        {
            Console.WriteLine($"LoadLocalBuild <<< invalid version info is NULL, cannot load {localPublishDirectory}");
            throw new ArgumentNullException(nameof(fhirCoreInfo));
        }

        // tell the user what's going on
        Console.WriteLine(
            $"LoadLocalBuild <<<" +
            $" Found: {fhirCoreInfo.PackageName}" +
            $" version: {fhirCoreInfo.VersionString}" +
            $" build: {fhirCoreInfo.ReleaseName}");

        string expansionDir;
        string packageDir;

        if (localLoadType == "latest")
        {
            FhirPackageDownloader.CopyAndExtract(
                localPublishDirectory,
                fhirCoreInfo.ExpansionsPackageName,
                fhirCoreInfo.VersionString,
                fhirSpecDirectory,
                out expansionDir);

            expansionDir = Path.Combine(expansionDir, "package");
        }
        else
        {
            expansionDir = Path.Combine(
                fhirSpecDirectory,
                $"local-{fhirCoreInfo.ExpansionsPackageName}-{fhirCoreInfo.VersionString}",
                "package");
        }

        // load package info
        FhirPackage expansionPackageInfo = FhirPackage.Load(expansionDir);

        // tell the user what's going on
        Console.WriteLine($"LoadLocalBuild <<< Found: {expansionPackageInfo.Name} version: {expansionPackageInfo.Version}");

        if (localLoadType == "latest")
        {
            FhirPackageDownloader.CopyAndExtract(
                localPublishDirectory,
                fhirCoreInfo.PackageName,
                fhirCoreInfo.VersionString,
                fhirSpecDirectory,
                out packageDir);

            packageDir = Path.Combine(packageDir, "package");
        }
        else
        {
            packageDir = Path.Combine(
                fhirSpecDirectory,
                $"local-{fhirCoreInfo.PackageName}-{fhirCoreInfo.VersionString}",
                "package");
        }

        // load package info
        FhirPackage packageInfo = FhirPackage.Load(packageDir);

        // tell the user what's going on
        Console.WriteLine($"LoadLocalBuild <<< Found: {packageInfo.Name} version: {packageInfo.Version}");

        // update our structure
        fhirCoreInfo.VersionString = packageInfo.Version;

        HashSet<string> processedFiles = new HashSet<string>();

        // process Code Systems
        ProcessFileGroup(packageDir, "CodeSystem", ref fhirCoreInfo, ref processedFiles);

        // process Value Set expansions
        ProcessFileGroup(expansionDir, "ValueSet", ref fhirCoreInfo, ref processedFiles);

        if (!officialExpansionsOnly)
        {
            ProcessFileGroup(packageDir, "ValueSet", ref fhirCoreInfo, ref processedFiles);
        }

        // process structure definitions
        ProcessFileGroup(packageDir, "StructureDefinition", ref fhirCoreInfo, ref processedFiles);

        // process search parameters (adds to resources)
        ProcessFileGroup(packageDir, "SearchParameter", ref fhirCoreInfo, ref processedFiles);

        // process operations (adds to resources and version info (server level))
        ProcessFileGroup(packageDir, "OperationDefinition", ref fhirCoreInfo, ref processedFiles);

        // add version-specific "MAGIC" items
        AddSearchMagicParameters(ref fhirCoreInfo);

        // make sure we cleared the last line
        Console.WriteLine($"LoadLocalBuild <<< Loaded and Parsed FHIR {fhirCoreInfo.ReleaseName}{new string(' ', 100)}");
    }

    /// <summary>Loads a cached version of FHIR.</summary>
    /// <exception cref="ArgumentNullException">     Thrown when one or more required arguments are
    ///  null.</exception>
    /// <exception cref="DirectoryNotFoundException">Thrown when the requested directory is not
    ///  present.</exception>
    /// <param name="fhirCoreInfo">       [in,out] Information describing the FHIR version.</param>
    /// <param name="officialExpansionsOnly">True to official expansions only.</param>
    public static void LoadCached(
        ref FhirVersionInfo fhirCoreInfo,
        bool officialExpansionsOnly)
    {
        if (fhirCoreInfo == null)
        {
            throw new ArgumentNullException(nameof(fhirCoreInfo));
        }

        string fhirCacheDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            ".fhir",
            "packages");

        if (!Directory.Exists(fhirCacheDir))
        {
            throw new DirectoryNotFoundException($"Could not find FHIR cache directory: {fhirCacheDir}.");
        }

        string core = fhirCoreInfo.PackageName + "#" + fhirCoreInfo.VersionString;
        string coreDir = Path.Combine(fhirCacheDir, core, "package");

        string expansion = fhirCoreInfo.ExpansionsPackageName + "#" + fhirCoreInfo.VersionString;
        string expansionDir = Path.Combine(fhirCacheDir, expansion, "package");

        if (!Directory.Exists(coreDir))
        {
            throw new DirectoryNotFoundException($"Could not find FHIR cached package: {coreDir}.");
        }

        if (!Directory.Exists(expansionDir))
        {
            throw new DirectoryNotFoundException($"Could not find FHIR cached package: {expansionDir}.");
        }

        HashSet<string> processedFiles = new HashSet<string>();

        // process Code Systems
        ProcessFileGroup(coreDir, "CodeSystem", ref fhirCoreInfo, ref processedFiles);

        // process Value Set expansions
        ProcessFileGroup(expansionDir, "ValueSet", ref fhirCoreInfo, ref processedFiles);

        // process other value set definitions (if requested)
        if (!officialExpansionsOnly)
        {
            ProcessFileGroup(coreDir, "ValueSet", ref fhirCoreInfo, ref processedFiles);
        }

        // process structure definitions
        ProcessFileGroup(coreDir, "StructureDefinition", ref fhirCoreInfo, ref processedFiles);

        // process search parameters (adds to resources)
        ProcessFileGroup(coreDir, "SearchParameter", ref fhirCoreInfo, ref processedFiles);

        // process operations (adds to resources and version info (server level))
        ProcessFileGroup(coreDir, "OperationDefinition", ref fhirCoreInfo, ref processedFiles);

        // add version-specific "MAGIC" items
        AddSearchMagicParameters(ref fhirCoreInfo);

        if (fhirCoreInfo.ConverterHasIssues(out int errorCount, out int warningCount))
        {
            // make sure we cleared the last line
            Console.WriteLine($"LoadCached <<< Loaded and Parsed FHIR {fhirCoreInfo.ReleaseName}" +
                $" with {errorCount} errors" +
                $" and {warningCount} warnings" +
                $"{new string(' ', 100)}");
            fhirCoreInfo.DisplayConverterIssues();
        }
        else
        {
            // make sure we cleared the last line
            Console.WriteLine($"LoadCached <<< Loaded and Parsed FHIR {fhirCoreInfo.ReleaseName}{new string(' ', 100)}");
        }
    }

    /// <summary>Loads a FHIR Specification package.</summary>
    /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
    /// <exception cref="FileNotFoundException">Thrown when the requested file is not present.</exception>
    /// <param name="fhirSpecDirectory">     Pathname of the FHIR spec directory.</param>
    /// <param name="fhirCoreInfo">       [in,out] Information describing the FHIR version.</param>
    /// <param name="officialExpansionsOnly">True to official expansions only.</param>
    public static void LoadPackage(
        string fhirSpecDirectory,
        ref FhirVersionInfo fhirCoreInfo,
        bool officialExpansionsOnly)
    {
        // sanity checks
        if (fhirCoreInfo == null)
        {
            Console.WriteLine($"LoadPackage <<< invalid version info is NULL, cannot load {fhirSpecDirectory}");
            throw new ArgumentNullException(nameof(fhirCoreInfo));
        }

        // first load value set expansions
        if (!TryFindPackage(
            fhirCoreInfo.ReleaseName,
            fhirCoreInfo.ExpansionsPackageName,
            fhirCoreInfo.VersionString,
            fhirSpecDirectory,
            out string expansionDir))
        {
            Console.WriteLine($"LoadPackage <<< cannot find package for {fhirCoreInfo.ReleaseName}: {fhirCoreInfo.ExpansionsPackageName}!");
            throw new FileNotFoundException($"Cannot find package for {fhirCoreInfo.ReleaseName}: {fhirCoreInfo.ExpansionsPackageName}");
        }

        // load package info
        FhirPackage expansionPackageInfo = FhirPackage.Load(expansionDir);

        // tell the user what's going on
        Console.WriteLine($"LoadPackage <<< Found: {expansionPackageInfo.Name} version: {expansionPackageInfo.Version}");

        // find the package
        if (!TryFindPackage(
                fhirCoreInfo.ReleaseName,
                fhirCoreInfo.PackageName,
                fhirCoreInfo.VersionString,
                fhirSpecDirectory,
                out string packageDir))
        {
            Console.WriteLine($"LoadPackage <<< cannot find package for {fhirCoreInfo.ReleaseName}: {fhirCoreInfo.PackageName}!");
            throw new FileNotFoundException($"Cannot find package for {fhirCoreInfo.ReleaseName}: {fhirCoreInfo.PackageName}");
        }

        // load package info
        FhirPackage packageInfo = FhirPackage.Load(packageDir);

        // tell the user what's going on
        Console.WriteLine($"LoadPackage <<< Found: {packageInfo.Name} version: {packageInfo.Version}");

        // update our structure
        fhirCoreInfo.VersionString = packageInfo.Version;

        HashSet<string> processedFiles = new HashSet<string>();

        // process Code Systems
        ProcessFileGroup(packageDir, "CodeSystem", ref fhirCoreInfo, ref processedFiles);

        // process Value Set expansions
        ProcessFileGroup(expansionDir, "ValueSet", ref fhirCoreInfo, ref processedFiles);

        // process other value set definitions (if requested)
        if (!officialExpansionsOnly)
        {
            ProcessFileGroup(packageDir, "ValueSet", ref fhirCoreInfo, ref processedFiles);
        }

        // process structure definitions
        ProcessFileGroup(packageDir, "StructureDefinition", ref fhirCoreInfo, ref processedFiles);

        // process search parameters (adds to resources)
        ProcessFileGroup(packageDir, "SearchParameter", ref fhirCoreInfo, ref processedFiles);

        // process operations (adds to resources and version info (server level))
        ProcessFileGroup(packageDir, "OperationDefinition", ref fhirCoreInfo, ref processedFiles);

        // add version-specific "MAGIC" items
        AddSearchMagicParameters(ref fhirCoreInfo);

        if (fhirCoreInfo.ConverterHasIssues(out int errorCount, out int warningCount))
        {
            // make sure we cleared the last line
            Console.WriteLine($"LoadPackage <<< Loaded and Parsed FHIR {fhirCoreInfo.ReleaseName}" +
                $" with {errorCount} errors" +
                $" and {warningCount} warnings" +
                $"{new string(' ', 100)}");
            fhirCoreInfo.DisplayConverterIssues();
        }
        else
        {
            // make sure we cleared the last line
            Console.WriteLine($"LoadPackage <<< Loaded and Parsed FHIR {fhirCoreInfo.ReleaseName}{new string(' ', 100)}");
        }
    }

    /// <summary>Adds the search magic parameters.</summary>
    /// <param name="info">[in,out] The information.</param>
    private static void AddSearchMagicParameters(ref FhirVersionInfo info)
    {
        switch (info.MajorVersionEnum)
        {
            case FhirVersionInfo.FhirCoreVersion.DSTU2:
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

            case FhirVersionInfo.FhirCoreVersion.STU3:
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

            case FhirVersionInfo.FhirCoreVersion.R4:
            case FhirVersionInfo.FhirCoreVersion.R4B:
            case FhirVersionInfo.FhirCoreVersion.R5:
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
    /// <param name="fhirCoreInfo">[in,out] Information describing the fhir version.</param>
    /// <param name="processedFiles"> [in,out] The processed files.</param>
    private static void ProcessFileGroup(
        string packageDir,
        string prefix,
        ref FhirVersionInfo fhirCoreInfo,
        ref HashSet<string> processedFiles)
    {
        // get the files in this directory
        string[] files = Directory.GetFiles(packageDir, $"{prefix}*.json", SearchOption.TopDirectoryOnly);

        // process these files
        ProcessPackageFiles(files, ref fhirCoreInfo, ref processedFiles);
    }

    /// <summary>Process the package files.</summary>
    /// <exception cref="InvalidDataException">Thrown when an Invalid Data error condition occurs.</exception>
    /// <param name="files">          The files.</param>
    /// <param name="fhirCoreInfo">[in,out] Information describing the fhir version.</param>
    /// <param name="processedFiles"> [in,out] The processed files.</param>
    private static void ProcessPackageFiles(
        string[] files,
        ref FhirVersionInfo fhirCoreInfo,
        ref HashSet<string> processedFiles)
    {
        // traverse the files
        foreach (string filename in files)
        {
            // check for skipping file
            if (fhirCoreInfo.ShouldSkipFile(Path.GetFileName(filename)))
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
                Console.Write($"{fhirCoreInfo.MajorVersionEnum}: {shortName,-85}\r");

                // check for ignored types
                if (fhirCoreInfo.ShouldIgnoreResource(resourceHint))
                {
                    // skip
                    continue;
                }

                // this should be listed in process types (validation check)
                if (!fhirCoreInfo.ShouldProcessResource(resourceHint))
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
                var resource = fhirCoreInfo.ParseResource(contents);

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
                fhirCoreInfo.ProcessResource(resource);
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
