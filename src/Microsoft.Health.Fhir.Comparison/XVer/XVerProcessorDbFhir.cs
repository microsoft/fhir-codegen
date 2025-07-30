using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.CommandLine;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Formats.Tar;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Linq;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Hl7.Fhir.Specification.Snapshot;
using Hl7.Fhir.Utility;
using Hl7.FhirPath.Sprache;
using Microsoft.Extensions.Logging;
using Microsoft.Health.Fhir.CodeGen.Configuration;
using Microsoft.Health.Fhir.CodeGen.FhirExtensions;
using Microsoft.Health.Fhir.CodeGen.Language;
using Microsoft.Health.Fhir.CodeGen.Models;
using Microsoft.Health.Fhir.CodeGenCommon.Extensions;
using Microsoft.Health.Fhir.CodeGenCommon.Models;
using Microsoft.Health.Fhir.CodeGenCommon.Packaging;
using Microsoft.Health.Fhir.CodeGenCommon.Utils;
using Microsoft.Health.Fhir.Comparison.CompareTool;
using Microsoft.Health.Fhir.Comparison.Models;
using Octokit;
using static System.Net.Mime.MediaTypeNames;
using CMR = Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship;
using Tasks = System.Threading.Tasks;

namespace Microsoft.Health.Fhir.Comparison.XVer;

public partial class XVerProcessor
{

    /// <summary>
    /// Represents index information for a cross-version FHIR package, including references to supporting structures and value sets.
    /// </summary>
    private class XverPackageIndexInfo
    {
        /// <summary>
        /// Gets or sets the source package support information.
        /// </summary>
        public required PackageXverSupport SourcePackageSupport { get; set; }

        /// <summary>
        /// Gets or sets the target package support information.
        /// </summary>
        public required PackageXverSupport TargetPackageSupport { get; set; }

        /// <summary>
        /// Gets or sets the unique package identifier for this cross-version package.
        /// </summary>
        public required string PackageId { get; set; }

        /// <summary>
        /// Gets or sets the list of JSON strings representing indexed structure definitions.
        /// </summary>
        public List<string> IndexStructureJsons { get; set; } = [];

        /// <summary>
        /// Gets or sets the list of JSON strings representing indexed value sets.
        /// </summary>
        public List<string> IndexValueSetJsons { get; set; } = [];

        /// <summary>
        /// Gets or sets the list of JSON strings for ImplementationGuide structure resources.
        /// </summary>
        public List<string> IgStructureJsons { get; set; } = [];

        /// <summary>
        /// Gets or sets the list of JSON strings for ImplementationGuide value set resources.
        /// </summary>
        public List<string> IgValueSetJsons { get; set; } = [];

        /// <summary>
        /// Gets or sets the list of ImplementationGuide structure resource components.
        /// </summary>
        public List<ImplementationGuide.ResourceComponent> IgStructures { get; set; } = [];

        /// <summary>
        /// Gets or sets the list of ImplementationGuide value set resource components.
        /// </summary>
        public List<ImplementationGuide.ResourceComponent> IgValueSets { get; set; } = [];
    }

    private static Dictionary<string, string> _publisherScripts = [];
    private static Lock _publisherScriptsLock = new();

    /// <summary>
    /// Generates cross-version FHIR artifacts from the loaded database, including ValueSets, StructureDefinitions, and ImplementationGuides.
    /// </summary>
    /// <param name="version">Optional artifact version to use; if null, uses the configured artifact version.</param>
    /// <param name="outputDir">Optional output directory; if null, uses the configured map source path.</param>
    /// <exception cref="Exception">
    /// Thrown if the database is not loaded or if the output directory is not specified.
    /// </exception>
    public void WriteFhirFromDatabase(string? version = null, string? outputDir = null)
    {
        // check for no database
        if (_db == null)
        {
            throw new Exception("Cannot generate FHIR artifacts without a loaded database!");
        }

        outputDir ??= _config.CrossVersionMapSourcePath;

        // check for no output location
        if (string.IsNullOrEmpty(outputDir))
        {
            throw new Exception("Cannot write FHIR artifacts without output or map source folder!");
        }

        string fhirDir = Path.Combine(outputDir, "fhir");
        if (Directory.Exists(fhirDir))
        {
            Directory.Delete(fhirDir, true);
        }

        Directory.CreateDirectory(fhirDir);

        if (string.IsNullOrEmpty(version))
        {
            _crossDefinitionVersion = _config.XverArtifactVersion;
        }
        else
        {
            _crossDefinitionVersion = version;
        }

        _logger.LogInformation($"Writing cross-version FHIR artifacts to {fhirDir} with version {_crossDefinitionVersion}");

        // grab the FHIR Packages we are processing
        List<DbFhirPackage> packages = DbFhirPackage.SelectList(_db.DbConnection, orderByProperties: [nameof(DbFhirPackage.ShortName)]);
        List<DbFhirPackageComparisonPair> packageComparisonPairs = DbFhirPackageComparisonPair.SelectList(
            _db.DbConnection,
            orderByProperties: [nameof(DbFhirPackageComparisonPair.SourcePackageKey), nameof(DbFhirPackageComparisonPair.TargetPackageKey)]);

        ConcurrentDictionary<int, string> differentialVsBySourceKey = [];

        Dictionary<int, HashSet<string>> basicElementPathsByPackageKey = [];
        Dictionary<(int, string), List<List<XverOutcome>>> xverOutcomes = [];

        List<PackageXverSupport> packageSupports = [];

        List<XverPackageIndexInfo> allIndexInfos = [];

        // iterate over the packages to build the Basic resource element paths
        foreach ((DbFhirPackage package, int index) in packages.Select((p, i) => (p, i)))
        {
            // need to create a definition collection with the matching core package so that we can build everything
            string packageDirective = $"{package.PackageId}#{package.PackageVersion}";
            
            //// create a loader because these are all different FHIR core versions
            //using CodeGen.Loader.PackageLoader loader = new(_config, new()
            //{
            //    JsonModel = CodeGen.Loader.LoaderOptions.JsonDeserializationModel.SystemTextJson,
            //});

            //DefinitionCollection coreDc = loader.LoadPackages([packageDirective]).Result
            //    ?? throw new Exception($"Could not load package: {packageDirective}");

            PackageXverSupport packageSupport = new()
            {
                PackageIndex = index,
                Package = package,
                BasicElements = [],
                //CoreDC = coreDc,
                //SnapshotGenerator = new(coreDc),
            };

            packageSupports.Add(packageSupport);

            // check for a basic structure
            DbStructureDefinition? basicResource = DbStructureDefinition.SelectSingle(
                _db.DbConnection,
                FhirPackageKey: package.Key,
                Name: "Basic",
                ArtifactClass: FhirArtifactClassEnum.Resource);

            if (basicResource != null)
            {
                // get the elements for this structure
                List<DbElement> basicElements = DbElement.SelectList(
                    _db.DbConnection,
                    StructureKey: basicResource.Key);

                // iterate over the elements
                foreach (DbElement element in basicElements)
                {
                    // skip root and elements with empty paths
                    if ((element.ResourceFieldOrder == 0) ||
                        string.IsNullOrEmpty(element.Path))
                    {
                        continue;
                    }

                    // add the path to the dictionary, but strip "Basic" from the front
                    packageSupport.BasicElements.Add(element.Path.Substring(5));
                }
            }

            // check for an extension structure
            DbStructureDefinition? extensionStructure = DbStructureDefinition.SelectSingle(
                _db.DbConnection,
                FhirPackageKey: package.Key,
                Name: "Extension",
                ArtifactClass: FhirArtifactClassEnum.ComplexType);

            if (extensionStructure != null)
            {
                // check for the value[x] element
                DbElement? extValueElement = DbElement.SelectSingle(
                    _db.DbConnection,
                    FhirPackageKey: package.Key,
                    StructureKey: extensionStructure.Key,
                    Id: "Extension.value[x]");

                if (extValueElement != null)
                {
                    // get the types for this element
                    List<DbElementType> extValueTypes = DbElementType.SelectList(
                        _db.DbConnection,
                        ElementKey: extValueElement.Key);

                    // iterate over the types
                    foreach (DbElementType extValueType in extValueTypes)
                    {
                        if (!string.IsNullOrEmpty(extValueType.TypeName))
                        {
                            packageSupport.AllowedExtensionTypes.Add(extValueType.TypeName);
                        }
                    }
                }
            }
        }

        // iterate over the list of packages
        for (int focusPackageIndex = 0; focusPackageIndex < packages.Count; focusPackageIndex++)
        {
            // ignore DSTU2 for now
            //if (packageSupports[focusPackageIndex].Package.DefinitionFhirSequence == FhirReleases.FhirSequenceCodes.DSTU2)
            //{
            //    continue;
            //}

            //if (focusPackageIndex != packages.Count - 1)
            //{
            //    continue;
            //}

            _logger.LogInformation($"Processing package {focusPackageIndex + 1} of {packages.Count}: {packages[focusPackageIndex].ShortName}");

            Dictionary<(int sourceVsKey, int targetPackageId), ValueSet> xverValueSets = buildXverValueSets(packages, focusPackageIndex);

            writeXverValueSets(packages, focusPackageIndex, xverValueSets, fhirDir);
            //writeFhirValueSets(packages, i, packageComparisonPairs, fhirDir, differentialVsBySourceKey);

            Dictionary<(int sourceElementKey, int targetPackageId), (StructureDefinition, DbExtensionSubstitution?)> xverExtensions = [];
            buildXverStructures(packageSupports, focusPackageIndex, xverValueSets, xverExtensions, xverOutcomes, FhirArtifactClassEnum.ComplexType);
            buildXverStructures(packageSupports, focusPackageIndex, xverValueSets, xverExtensions, xverOutcomes, FhirArtifactClassEnum.Resource);

            writeXverStructures(packageSupports, focusPackageIndex, xverExtensions, fhirDir);

            if (_config.XverExportForPublisher)
            {
                // write the publisher config files
                writePublisherSinglePackageConfig(packageSupports, focusPackageIndex, fhirDir);
            }
            else
            {
                List<XverPackageIndexInfo> focusedIndexInfos = writeXverSinglePackageSupportFiles(
                    packageSupports,
                    focusPackageIndex,
                    xverValueSets,
                    xverExtensions,
                    fhirDir);
                allIndexInfos.AddRange(focusedIndexInfos);
            }
        }

        // write our combined package support files
        if (_config.XverExportForPublisher)
        {
            writePublisherValidationPackageConfig(packageSupports, allIndexInfos, fhirDir);
        }
        else
        {
            writeXverValidationPackageSupportFiles(packageSupports, allIndexInfos, fhirDir);
        }

        // write all of our outcome lists
        writeXverOutcomes(packageSupports, xverOutcomes, outputDir);

        if ((_config.XverExportForPublisher == false) &&
            (_config.XverGenerateNpms == true))
        {
            // make the make package tgz files
            foreach (DbFhirPackage focusPackage in packages)
            {
                // TODO: until verified, only write R4 and later packages
                if ((focusPackage.ShortName == "R2") ||
                    (focusPackage.ShortName == "R3"))
                {
                    continue;
                }

                string validationPackageId = $"hl7.fhir.uv.xver.{focusPackage.ShortName.ToLowerInvariant()}";

                // create the validation package
                createTgzFromDirectory(
                    Path.Combine(fhirDir, focusPackage.ShortName),
                    Path.Combine(fhirDir, $"{validationPackageId}.{_crossDefinitionVersion}.tgz"));

                // look for all combination packages, using the focus as the target
                foreach (DbFhirPackage sourcePackage in packages)
                {
                    if (sourcePackage.Key == focusPackage.Key)
                    {
                        continue;
                    }

                    string packageId = $"hl7.fhir.uv.xver-{sourcePackage.ShortName.ToLowerInvariant()}.{focusPackage.ShortName.ToLowerInvariant()}";

                    // create the validation package
                    createTgzFromDirectory(
                        Path.Combine(fhirDir, $"{sourcePackage.ShortName}-for-{focusPackage.ShortName}"),
                        Path.Combine(fhirDir, $"{packageId}.{_crossDefinitionVersion}.tgz"));
                }
            }
        }
    }

    private static string getPackageId(DbFhirPackage? sourcePackage, DbFhirPackage targetPackage) => sourcePackage == null
        ? $"hl7.fhir.uv.xver.{targetPackage.ShortName.ToLowerInvariant()}"
        : $"hl7.fhir.uv.xver-{sourcePackage.ShortName.ToLowerInvariant()}.{targetPackage.ShortName.ToLowerInvariant()}";

    /// <summary>
    /// Determines whether a file should be skipped when copying from the GitHub repository.
    /// </summary>
    /// <param name="filePath">The relative file path from the repository root.</param>
    /// <returns>True if the file should be skipped, false otherwise.</returns>
    private static bool ShouldSkipFile(string filePath)
    {
        // Skip hidden files and directories
        if (filePath.StartsWith(".") || filePath.Contains("/."))
        {
            return true;
        }

        // skip repository README
        if (filePath.Equals("README.md", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        // Skip common binary file extensions
        string[] skipExtensions = { ".exe", ".dll", ".bin", ".jar", ".zip", ".tar", ".gz", ".7z", 
                                   ".png", ".jpg", ".jpeg", ".gif", ".bmp", ".ico", ".svg",
                                   ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx" };
        
        string extension = Path.GetExtension(filePath).ToLowerInvariant();
        if (skipExtensions.Contains(extension))
        {
            return true;
        }

        // Skip node_modules and other common directories
        string[] skipDirectories = { "node_modules/", ".git/", ".vs/", ".vscode/", "bin/", "obj/" };
        foreach (string skipDir in skipDirectories)
        {
            if (filePath.Contains(skipDir))
            {
                return true;
            }
        }

        // Skip very large files (> 1MB) - these are likely not useful for IG publishing
        // Note: We can't check file size here since we only have the path, 
        // but we'll handle this in the fetch method

        return false;
    }

    
    /// <summary>
    /// Fetches all files from the HL7 ig-publisher-scripts GitHub repository.
    /// </summary>
    /// <returns>A dictionary where keys are relative file paths and values are file contents.</returns>
    private Dictionary<string, string> getCurrentPublisherScripts()
    {
        const string owner = "HL7";
        const string repo = "ig-publisher-scripts";

        lock (_publisherScriptsLock)
        {
            if (_publisherScripts.Count > 0)
            {
                _logger.LogInformation("Using cached publisher scripts from GitHub repository: {Owner}/{Repo}", owner, repo);
                return _publisherScripts;
            }

            try
            {
                _logger.LogInformation("Fetching files from GitHub repository: {Owner}/{Repo}", owner, repo);

                GitHubClient client = new(new ProductHeaderValue("fhir-codegen"));

                // Get the repository contents recursively
                List<RepositoryContent> contents = getRepositoryContentsRecursive(client, owner, repo);

                foreach (RepositoryContent content in contents)
                {
                    if (content.Type == ContentType.File && !ShouldSkipFile(content.Path))
                    {
                        try
                        {
                            // Skip files larger than 1MB
                            if (content.Size > 1024 * 1024)
                            {
                                _logger.LogDebug("Skipping large file: {Path} ({Size} bytes)", content.Path, content.Size);
                                continue;
                            }

                            // Get the file content
                            byte[] fileContent = client.Repository.Content.GetRawContent(owner, repo, content.Path).Result;
                            string contentString = System.Text.Encoding.UTF8.GetString(fileContent);

                            _publisherScripts[content.Path] = contentString;
                            _logger.LogDebug("Fetched file: {Path}", content.Path);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Failed to fetch file content for: {Path}", content.Path);
                        }
                    }
                    else if (content.Type == ContentType.File)
                    {
                        _logger.LogDebug("Skipping file: {Path}", content.Path);
                    }
                }

                _logger.LogInformation("Successfully fetched {Count} files from GitHub repository", _publisherScripts.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch files from GitHub repository: {Owner}/{Repo}", owner, repo);
            }
        }
        return _publisherScripts;
    }

    /// <summary>
    /// Recursively gets all contents from a GitHub repository.
    /// </summary>
    private List<RepositoryContent> getRepositoryContentsRecursive(GitHubClient client, string owner, string repo, string? path = null)
    {
        List<RepositoryContent> allContents = [];
        
        try
        {
            IReadOnlyList<RepositoryContent> contents = path == null
                ? client.Repository.Content.GetAllContents(owner, repo).Result
                : client.Repository.Content.GetAllContents(owner, repo, path).Result;
            
            foreach (var content in contents)
            {
                if (content.Type == ContentType.File)
                {
                    allContents.Add(content);
                }
                else if (content.Type == ContentType.Dir && !ShouldSkipFile(content.Path))
                {
                    // Recursively get directory contents
                    List<RepositoryContent> subContents = getRepositoryContentsRecursive(client, owner, repo, content.Path);
                    allContents.AddRange(subContents);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get repository contents for path: {Path}", path);
        }
        
        return allContents;
    }

    private string createExportPackageDir(string fhirDir, DbFhirPackage? sourcePackage, DbFhirPackage targetPackage)
    {
        if (!Directory.Exists(fhirDir))
        {
            Directory.CreateDirectory(fhirDir);
        }

        string packageId = getPackageId(sourcePackage, targetPackage);

        string dir = Path.Combine(fhirDir, packageId);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        if (_config.XverExportForPublisher)
        {
            string inputDir = Path.Combine(dir, "input");
            if (!Directory.Exists(inputDir))
            {
                Directory.CreateDirectory(inputDir);
            }

            string pagesDir = Path.Combine(inputDir, "pagecontent");
            if (!Directory.Exists(pagesDir))
            {
                Directory.CreateDirectory(pagesDir);
            }

            string resourcesDir = Path.Combine(inputDir, "resources");
            if (!Directory.Exists(resourcesDir))
            {
                Directory.CreateDirectory(resourcesDir);
            }

            string vocabDir = Path.Combine(inputDir, "vocabulary");
            if (!Directory.Exists(vocabDir))
            {
                Directory.CreateDirectory(vocabDir);
            }
        }
        else
        {
            string packageSubDir = Path.Combine(dir, "package");
            if (!Directory.Exists(packageSubDir))
            {
                Directory.CreateDirectory(packageSubDir);
            }
        }

        return dir;
    }

    private void writePublisherValidationPackageConfig(
        List<PackageXverSupport> packageSupports,
        List<XverPackageIndexInfo> allPackageIndexInfos,
        string fhirDir)
    {
        // Fetch files from GitHub repository
        Dictionary<string, string> githubFiles = getCurrentPublisherScripts();

        // iterate over the support packages
        foreach (PackageXverSupport targetSupport in packageSupports)
        {
            DbFhirPackage targetPackage = targetSupport.Package;
            string packageId = getPackageId(null, targetPackage);
            string dir = createExportPackageDir(fhirDir, null, targetPackage);

            List<(string packageId, string packageVersion)> internalDependencies = [];
            foreach (PackageXverSupport sourcePackage in packageSupports)
            {
                if (sourcePackage.Package.Key == targetPackage.Key)
                {
                    continue;
                }

                internalDependencies.Add((
                    getPackageId(sourcePackage.Package, targetPackage),
                    _crossDefinitionVersion));
            }


            // write GitHub repository files to the output directory
            if (githubFiles.Count > 0)
            {
                _logger.LogInformation("Writing {Count} files from GitHub repository to {dir}", githubFiles.Count, dir);

                foreach ((string filePath, string fileContent) in githubFiles)
                {
                    try
                    {
                        string fullPath = Path.Combine(dir, filePath);
                        string? directory = Path.GetDirectoryName(fullPath);

                        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                        {
                            Directory.CreateDirectory(directory);
                        }

                        File.WriteAllText(fullPath, fileContent);
                        _logger.LogDebug("Wrote GitHub file to: {FullPath}", fullPath);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to write GitHub file: {FilePath}", filePath);
                    }
                }

                _logger.LogInformation("Successfully wrote GitHub repository files to {FhirDir}", fhirDir);
            }
            else
            {
                _logger.LogWarning("No files were fetched from GitHub repository");
            }

            {
                string filename = Path.Combine(dir, "ig.ini");
                string contents = $$$"""
                    [IG]
                    ig = fsh-generated/resources/ImplementationGuide-{{{packageId}}}.json
                    template = fhir.base.template#current
                    """;
                File.WriteAllText(filename, contents);
            }

            {
                string additionalDependencies = internalDependencies.Count == 0
                    ? string.Empty
                    : string.Join("\n\t", internalDependencies.Select(pi => $"{pi.packageId} : {pi.packageVersion}"));

                string filename = Path.Combine(dir, "sushi-config.yaml");
                string contents = $$$"""
                    # ╭─────────────────────────Commonly Used ImplementationGuide Properties───────────────────────────╮
                    # │  The properties below are used to create the ImplementationGuide resource. The most commonly   │
                    # │  used properties are included. For a list of all supported properties and their functions,     │
                    # │  see: https://fshschool.org/docs/sushi/configuration/.                                         │
                    # ╰────────────────────────────────────────────────────────────────────────────────────────────────╯
                    id: {{{packageId}}}
                    canonical: http://hl7.org/fhir/uv/xver
                    name: {{{packageId}}}
                    title: FHIR Cross-Version Extensions validation package for FHIR {{{targetPackage.ShortName}}}
                    description: All cross-version extensions available in FHIR {{{targetPackage.ShortName}}}
                    status: draft # draft | active | retired | unknown
                    version: {{{_crossDefinitionVersion}}}
                    fhirVersion: {{{targetPackage.PackageVersion}}} # https://www.hl7.org/fhir/valueset-FHIR-version.html
                    copyrightYear: 2025+
                    releaseLabel: trial-use
                    license: CC0-1.0 # https://www.hl7.org/fhir/valueset-spdx-license.html
                    jurisdiction: http://unstats.un.org/unsd/methods/m49/m49.htm#001 "World"
                    publisher:
                        name: HL7 International / FHIR Infrastructure
                        url: http://www.hl7.org/Special/committees/fiwg
                        # email: test@example.org

                    # The dependencies property corresponds to IG.dependsOn. The key is the
                    # package id and the value is the version (or dev/current). For advanced
                    # use cases, the value can be an object with keys for id, uri, and version.
                    #
                    dependencies:
                        {{{targetSupport.Package.PackageId}}} : {{{targetSupport.Package.PackageVersion}}}
                        hl7.terminology.{{{targetSupport.Package.ShortName.ToLowerInvariant()}}} : 6.3.0
                        hl7.fhir.uv.extensions.{{{targetSupport.Package.ShortName.ToLowerInvariant()}}} : 5.2.0
                        hl7.fhir.uv.tools.{{{targetSupport.Package.ShortName.ToLowerInvariant()}}} : current
                        {{{additionalDependencies}}}

                    #   hl7.fhir.us.core: 3.1.0
                    #   hl7.fhir.us.mcode:
                    #     id: mcode
                    #     uri: http://hl7.org/fhir/us/mcode/ImplementationGuide/hl7.fhir.us.mcode
                    #     version: 1.0.0
                    #
                    #
                    # The pages property corresponds to IG.definition.page. SUSHI can
                    # auto-generate the page list, but if the author includes pages in
                    # this file, it is assumed that the author will fully manage the
                    # pages section and SUSHI will not generate any page entries.
                    # The page file name is used as the key. If title is not provided,
                    # then the title will be generated from the file name.  If a
                    # generation value is not provided, it will be inferred from the
                    # file name extension.  Any subproperties that are valid filenames
                    # with supported extensions (e.g., .md/.xml) will be treated as
                    # sub-pages.
                    #
                    # pages:
                    #   index.md:
                    #     title: Example Home
                    #   implementation.xml:
                    #   examples.xml:
                    #     title: Examples Overview
                    #     simpleExamples.xml:
                    #     complexExamples.xml:
                    #
                    #
                    # The parameters property represents IG.definition.parameter. Rather
                    # than a list of code/value pairs (as in the ImplementationGuide
                    # resource), the code is the YAML key. If a parameter allows repeating
                    # values, the value in the YAML should be a sequence/array.
                    # For parameters defined by core FHIR see:
                    # http://build.fhir.org/codesystem-guide-parameter-code.html
                    # For parameters defined by the FHIR Tools IG see:
                    # http://build.fhir.org/ig/FHIR/fhir-tools-ig/branches/master/CodeSystem-ig-parameters.html
                    #
                    # parameters:
                    #   excludettl: true
                    #   validation: [allow-any-extensions, no-broken-links]
                    parameters:
                        apply-wg: true
                        default-wg: true
                        show-inherited-invariants: false
                        usage-stats-opt-out: true
                        shownav: 'true'

                    extension:
                        - url: http://hl7.org/fhir/StructureDefinition/structuredefinition-standards-status
                            valueCode: trial-use
                        - url: http://hl7.org/fhir/StructureDefinition/structuredefinition-wg
                            valueCode: fhir
                    #
                    # ╭────────────────────────────────────────────menu.xml────────────────────────────────────────────╮
                    # │ The menu property will be used to generate the input/menu.xml file. The menu is represented    │
                    # │ as a simple structure where the YAML key is the menu item name and the value is the URL.       │
                    # │ The IG publisher currently only supports one level deep on sub-menus. To provide a             │
                    # │ custom menu.xml file, do not include this property and include a `menu.xml` file in            │
                    # │ input/includes. To use a provided input/includes/menu.xml file, delete the "menu"              │
                    # │ property below.                                                                                │
                    # ╰────────────────────────────────────────────────────────────────────────────────────────────────╯
                    menu:
                      Home: index.html
                      Artifacts: artifacts.html

                    # ╭───────────────────────────Less Common Implementation Guide Properties──────────────────────────╮
                    # │  Uncomment the properties below to configure additional properties on the ImplementationGuide  │
                    # │  resource. These properties are less commonly needed than those above.                         │
                    # ╰────────────────────────────────────────────────────────────────────────────────────────────────╯
                    #
                    # Those who need more control or want to add additional details to the contact values can use
                    # contact directly and follow the format outlined in the ImplementationGuide resource and
                    # ContactDetail.
                    #
                    # contact:
                    #   - name: Bob Smith
                    #     telecom:
                    #       - system: email # phone | fax | email | pager | url | sms | other
                    #         value: bobsmith@example.org
                    #         use: work
                    #
                    #
                    # The global property corresponds to the IG.global property, but it
                    # uses the type as the YAML key and the profile as its value. Since
                    # FHIR does not explicitly disallow more than one profile per type,
                    # neither do we; the value can be a single profile URL or an array
                    # of profile URLs. If a value is an id or name, SUSHI will replace
                    # it with the correct canonical when generating the IG JSON.
                    #
                    # global:
                    #   Patient: http://example.org/fhir/StructureDefinition/my-patient-profile
                    #   Encounter: http://example.org/fhir/StructureDefinition/my-encounter-profile
                    #
                    #
                    # The resources property corresponds to IG.definition.resource.
                    # SUSHI can auto-generate all of the resource entries based on
                    # the FSH definitions and/or information in any user-provided
                    # JSON or XML resource files. If the generated entries are not
                    # sufficient or complete, however, the author can add entries
                    # here. If the reference matches a generated entry, it will
                    # replace the generated entry. If it doesn't match any generated
                    # entries, it will be added to the generated entries. The format
                    # follows IG.definition.resource with the following differences:
                    #   * use IG.definition.resource.reference.reference as the YAML key.
                    #   * if the key is an id or name, SUSHI will replace it with the
                    #     correct URL when generating the IG JSON.
                    #   * specify "omit" to omit a FSH-generated resource from the
                    #     resource list.
                    #   * if the exampleCanonical is an id or name, SUSHI will replace
                    #     it with the correct canonical when generating the IG JSON.
                    #   * groupingId can be used, but top-level groups syntax may be a
                    #     better option (see below).
                    # The following are simple examples to demonstrate what this might
                    # look like:
                    #
                    # resources:
                    #   Patient/my-example-patient:
                    #     name: My Example Patient
                    #     description: An example Patient
                    #     exampleBoolean: true
                    #   Patient/bad-example: omit
                    #
                    #
                    # Groups can control certain aspects of the IG generation.  The IG
                    # documentation recommends that authors use the default groups that
                    # are provided by the templating framework, but if authors want to
                    # use their own instead, they can use the mechanism below.  This will
                    # create IG.definition.grouping entries and associate the individual
                    # resource entries with the corresponding groupIds. If a resource
                    # is specified by id or name, SUSHI will replace it with the correct
                    # URL when generating the IG JSON.
                    #
                    # groups:
                    #   GroupA:
                    #     name: Group A
                    #     description: The Alpha Group
                    #     resources:
                    #     - StructureDefinition/animal-patient
                    #     - StructureDefinition/arm-procedure
                    #   GroupB:
                    #     name: Group B
                    #     description: The Beta Group
                    #     resources:
                    #     - StructureDefinition/bark-control
                    #     - StructureDefinition/bee-sting
                    #
                    #
                    # The ImplementationGuide resource defines several other properties
                    # not represented above. These properties can be used as-is and
                    # should follow the format defined in ImplementationGuide:
                    # * date
                    # * meta
                    # * implicitRules
                    # * language
                    # * text
                    # * contained
                    # * extension
                    # * modifierExtension
                    # * experimental
                    # * useContext
                    # * copyright
                    # * packageId
                    #
                    #
                    # ╭──────────────────────────────────────────SUSHI flags───────────────────────────────────────────╮
                    # │  The flags below configure aspects of how SUSHI processes FSH.                                 │
                    # ╰────────────────────────────────────────────────────────────────────────────────────────────────╯
                    # The FSHOnly flag indicates if only FSH resources should be exported.
                    # If set to true, no IG related content will be generated.
                    # The default value for this property is false.
                    #
                    # FSHOnly: false
                    #
                    #
                    # When set to true, the "short" and "definition" field on the root element of an Extension will
                    # be set to the "Title" and "Description" of that Extension. Default is true.
                    #
                    # applyExtensionMetadataToRoot: true
                    #
                    #
                    # The instanceOptions property is used to configure certain aspects of how SUSHI processes instances.
                    # See the individual option definitions below for more detail.
                    #
                    instanceOptions:
                        # When set to true, slices must be referred to by name and not only by a numeric index in order to be used
                        # in an Instance's assignment rule. All slices appear in the order in which they are specified in FSH rules.
                        # While SUSHI defaults to false for legacy reasons, manualSliceOrding is recommended for new projects.
                        manualSliceOrdering: true # true | false

                        # Determines for which types of Instances SUSHI will automatically set meta.profile
                        # if InstanceOf references a profile:
                        #
                        # setMetaProfile: always # always | never | inline-only | standalone-only
                        #
                        #
                        # Determines for which types of Instances SUSHI will automatically set id
                        # if InstanceOf references a profile:
                        #
                        # setId: always # always | standalone-only
                    """;
                File.WriteAllText(filename, contents);
            }
        }
    }

    private void writePublisherSinglePackageConfig(
        List<PackageXverSupport> packageSupports,
        int focusPackageIndex,
        string fhirDir)
    {
        DbFhirPackage sourcePackage = packageSupports[focusPackageIndex].Package;

        // Fetch files from GitHub repository
        Dictionary<string, string> githubFiles = getCurrentPublisherScripts();

        foreach (PackageXverSupport targetSupport in packageSupports)
        {
            DbFhirPackage targetPackage = targetSupport.Package;
            string packageId = getPackageId(sourcePackage, targetPackage);
            string dir = createExportPackageDir(fhirDir, sourcePackage, targetPackage);

            // write GitHub repository files to the output directory
            if (githubFiles.Count > 0)
            {
                _logger.LogInformation("Writing {Count} files from GitHub repository to {dir}", githubFiles.Count, dir);

                foreach ((string filePath, string fileContent) in githubFiles)
                {
                    try
                    {
                        string fullPath = Path.Combine(dir, filePath);
                        string? directory = Path.GetDirectoryName(fullPath);

                        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                        {
                            Directory.CreateDirectory(directory);
                        }

                        File.WriteAllText(fullPath, fileContent);
                        _logger.LogDebug("Wrote GitHub file to: {FullPath}", fullPath);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to write GitHub file: {FilePath}", filePath);
                    }
                }

                _logger.LogInformation("Successfully wrote GitHub repository files to {FhirDir}", fhirDir);
            }
            else
            {
                _logger.LogWarning("No files were fetched from GitHub repository");
            }

            {
                string filename = Path.Combine(dir, "ig.ini");
                string contents = $$$"""
                    [IG]
                    ig = fsh-generated/resources/ImplementationGuide-{{{packageId}}}.json
                    template = fhir.base.template#current
                    """;
                File.WriteAllText(filename, contents);
            }

            {
                string filename = Path.Combine(dir, "sushi-config.yaml");
                string contents = $$$"""
                    # ╭─────────────────────────Commonly Used ImplementationGuide Properties───────────────────────────╮
                    # │  The properties below are used to create the ImplementationGuide resource. The most commonly   │
                    # │  used properties are included. For a list of all supported properties and their functions,     │
                    # │  see: https://fshschool.org/docs/sushi/configuration/.                                         │
                    # ╰────────────────────────────────────────────────────────────────────────────────────────────────╯
                    id: {{{packageId}}}
                    canonical: http://hl7.org/fhir/{{{sourcePackage.FhirVersionShort}}}
                    name: {{{packageId}}}
                    title: FHIR Cross-Version Extensions package for FHIR {{{targetPackage.ShortName}}} from FHIR {{{sourcePackage.ShortName}}}
                    description: The cross-version extensions available in FHIR {{{targetPackage.ShortName}}} from FHIR {{{sourcePackage.ShortName}}}
                    status: draft # draft | active | retired | unknown
                    version: {{{_crossDefinitionVersion}}}
                    fhirVersion: {{{targetPackage.PackageVersion}}} # https://www.hl7.org/fhir/valueset-FHIR-version.html
                    copyrightYear: 2025+
                    releaseLabel: trial-use
                    license: CC0-1.0 # https://www.hl7.org/fhir/valueset-spdx-license.html
                    jurisdiction: http://unstats.un.org/unsd/methods/m49/m49.htm#001 "World"
                    publisher:
                        name: HL7 International / FHIR Infrastructure
                        url: http://www.hl7.org/Special/committees/fiwg
                        # email: test@example.org

                    # The dependencies property corresponds to IG.dependsOn. The key is the
                    # package id and the value is the version (or dev/current). For advanced
                    # use cases, the value can be an object with keys for id, uri, and version.
                    #
                    dependencies:
                        {{{targetSupport.Package.PackageId}}} : {{{targetSupport.Package.PackageVersion}}}
                        hl7.terminology.{{{targetSupport.Package.ShortName.ToLowerInvariant()}}} : 6.3.0
                        hl7.fhir.uv.extensions.{{{targetSupport.Package.ShortName.ToLowerInvariant()}}} : 5.2.0
                        hl7.fhir.uv.tools.{{{targetSupport.Package.ShortName.ToLowerInvariant()}}} : current

                    #   hl7.fhir.us.core: 3.1.0
                    #   hl7.fhir.us.mcode:
                    #     id: mcode
                    #     uri: http://hl7.org/fhir/us/mcode/ImplementationGuide/hl7.fhir.us.mcode
                    #     version: 1.0.0
                    #
                    #
                    # The pages property corresponds to IG.definition.page. SUSHI can
                    # auto-generate the page list, but if the author includes pages in
                    # this file, it is assumed that the author will fully manage the
                    # pages section and SUSHI will not generate any page entries.
                    # The page file name is used as the key. If title is not provided,
                    # then the title will be generated from the file name.  If a
                    # generation value is not provided, it will be inferred from the
                    # file name extension.  Any subproperties that are valid filenames
                    # with supported extensions (e.g., .md/.xml) will be treated as
                    # sub-pages.
                    #
                    # pages:
                    #   index.md:
                    #     title: Example Home
                    #   implementation.xml:
                    #   examples.xml:
                    #     title: Examples Overview
                    #     simpleExamples.xml:
                    #     complexExamples.xml:
                    #
                    #
                    # The parameters property represents IG.definition.parameter. Rather
                    # than a list of code/value pairs (as in the ImplementationGuide
                    # resource), the code is the YAML key. If a parameter allows repeating
                    # values, the value in the YAML should be a sequence/array.
                    # For parameters defined by core FHIR see:
                    # http://build.fhir.org/codesystem-guide-parameter-code.html
                    # For parameters defined by the FHIR Tools IG see:
                    # http://build.fhir.org/ig/FHIR/fhir-tools-ig/branches/master/CodeSystem-ig-parameters.html
                    #
                    # parameters:
                    #   excludettl: true
                    #   validation: [allow-any-extensions, no-broken-links]
                    parameters:
                        apply-wg: true
                        default-wg: true
                        show-inherited-invariants: false
                        usage-stats-opt-out: true
                        shownav: 'true'

                    extension:
                        - url: http://hl7.org/fhir/StructureDefinition/structuredefinition-standards-status
                            valueCode: trial-use
                        - url: http://hl7.org/fhir/StructureDefinition/structuredefinition-wg
                            valueCode: fhir
                    #
                    # ╭────────────────────────────────────────────menu.xml────────────────────────────────────────────╮
                    # │ The menu property will be used to generate the input/menu.xml file. The menu is represented    │
                    # │ as a simple structure where the YAML key is the menu item name and the value is the URL.       │
                    # │ The IG publisher currently only supports one level deep on sub-menus. To provide a             │
                    # │ custom menu.xml file, do not include this property and include a `menu.xml` file in            │
                    # │ input/includes. To use a provided input/includes/menu.xml file, delete the "menu"              │
                    # │ property below.                                                                                │
                    # ╰────────────────────────────────────────────────────────────────────────────────────────────────╯
                    menu:
                      Home: index.html
                      Lookups: lookups.html
                      Artifacts: artifacts.html

                    # ╭───────────────────────────Less Common Implementation Guide Properties──────────────────────────╮
                    # │  Uncomment the properties below to configure additional properties on the ImplementationGuide  │
                    # │  resource. These properties are less commonly needed than those above.                         │
                    # ╰────────────────────────────────────────────────────────────────────────────────────────────────╯
                    #
                    # Those who need more control or want to add additional details to the contact values can use
                    # contact directly and follow the format outlined in the ImplementationGuide resource and
                    # ContactDetail.
                    #
                    # contact:
                    #   - name: Bob Smith
                    #     telecom:
                    #       - system: email # phone | fax | email | pager | url | sms | other
                    #         value: bobsmith@example.org
                    #         use: work
                    #
                    #
                    # The global property corresponds to the IG.global property, but it
                    # uses the type as the YAML key and the profile as its value. Since
                    # FHIR does not explicitly disallow more than one profile per type,
                    # neither do we; the value can be a single profile URL or an array
                    # of profile URLs. If a value is an id or name, SUSHI will replace
                    # it with the correct canonical when generating the IG JSON.
                    #
                    # global:
                    #   Patient: http://example.org/fhir/StructureDefinition/my-patient-profile
                    #   Encounter: http://example.org/fhir/StructureDefinition/my-encounter-profile
                    #
                    #
                    # The resources property corresponds to IG.definition.resource.
                    # SUSHI can auto-generate all of the resource entries based on
                    # the FSH definitions and/or information in any user-provided
                    # JSON or XML resource files. If the generated entries are not
                    # sufficient or complete, however, the author can add entries
                    # here. If the reference matches a generated entry, it will
                    # replace the generated entry. If it doesn't match any generated
                    # entries, it will be added to the generated entries. The format
                    # follows IG.definition.resource with the following differences:
                    #   * use IG.definition.resource.reference.reference as the YAML key.
                    #   * if the key is an id or name, SUSHI will replace it with the
                    #     correct URL when generating the IG JSON.
                    #   * specify "omit" to omit a FSH-generated resource from the
                    #     resource list.
                    #   * if the exampleCanonical is an id or name, SUSHI will replace
                    #     it with the correct canonical when generating the IG JSON.
                    #   * groupingId can be used, but top-level groups syntax may be a
                    #     better option (see below).
                    # The following are simple examples to demonstrate what this might
                    # look like:
                    #
                    # resources:
                    #   Patient/my-example-patient:
                    #     name: My Example Patient
                    #     description: An example Patient
                    #     exampleBoolean: true
                    #   Patient/bad-example: omit
                    #
                    #
                    # Groups can control certain aspects of the IG generation.  The IG
                    # documentation recommends that authors use the default groups that
                    # are provided by the templating framework, but if authors want to
                    # use their own instead, they can use the mechanism below.  This will
                    # create IG.definition.grouping entries and associate the individual
                    # resource entries with the corresponding groupIds. If a resource
                    # is specified by id or name, SUSHI will replace it with the correct
                    # URL when generating the IG JSON.
                    #
                    # groups:
                    #   GroupA:
                    #     name: Group A
                    #     description: The Alpha Group
                    #     resources:
                    #     - StructureDefinition/animal-patient
                    #     - StructureDefinition/arm-procedure
                    #   GroupB:
                    #     name: Group B
                    #     description: The Beta Group
                    #     resources:
                    #     - StructureDefinition/bark-control
                    #     - StructureDefinition/bee-sting
                    #
                    #
                    # The ImplementationGuide resource defines several other properties
                    # not represented above. These properties can be used as-is and
                    # should follow the format defined in ImplementationGuide:
                    # * date
                    # * meta
                    # * implicitRules
                    # * language
                    # * text
                    # * contained
                    # * extension
                    # * modifierExtension
                    # * experimental
                    # * useContext
                    # * copyright
                    # * packageId
                    #
                    #
                    # ╭──────────────────────────────────────────SUSHI flags───────────────────────────────────────────╮
                    # │  The flags below configure aspects of how SUSHI processes FSH.                                 │
                    # ╰────────────────────────────────────────────────────────────────────────────────────────────────╯
                    # The FSHOnly flag indicates if only FSH resources should be exported.
                    # If set to true, no IG related content will be generated.
                    # The default value for this property is false.
                    #
                    # FSHOnly: false
                    #
                    #
                    # When set to true, the "short" and "definition" field on the root element of an Extension will
                    # be set to the "Title" and "Description" of that Extension. Default is true.
                    #
                    # applyExtensionMetadataToRoot: true
                    #
                    #
                    # The instanceOptions property is used to configure certain aspects of how SUSHI processes instances.
                    # See the individual option definitions below for more detail.
                    #
                    instanceOptions:
                        # When set to true, slices must be referred to by name and not only by a numeric index in order to be used
                        # in an Instance's assignment rule. All slices appear in the order in which they are specified in FSH rules.
                        # While SUSHI defaults to false for legacy reasons, manualSliceOrding is recommended for new projects.
                        manualSliceOrdering: true # true | false

                        # Determines for which types of Instances SUSHI will automatically set meta.profile
                        # if InstanceOf references a profile:
                        #
                        # setMetaProfile: always # always | never | inline-only | standalone-only
                        #
                        #
                        # Determines for which types of Instances SUSHI will automatically set id
                        # if InstanceOf references a profile:
                        #
                        # setId: always # always | standalone-only
                    """;
                File.WriteAllText(filename, contents);
            }
        }
    }

    private void writeXverValueSets(
        List<DbFhirPackage> packages,
        int focusPackageIndex,
        Dictionary<(int sourceVsKey, int targetPackageId), ValueSet> xverValueSets,
        string fhirDir)
    {
        Dictionary<int, DbFhirPackage> packageDict = packages.ToDictionary(p => p.Key);
        DbFhirPackage focusPackage = packages[focusPackageIndex];

        // iterate over the value sets
        foreach (((int sourceVsKey, int targetPackageId), ValueSet vs) in xverValueSets)
        {
            DbFhirPackage targetPackage = packageDict[targetPackageId];

            string dir = createExportPackageDir(fhirDir, focusPackage, targetPackage);

            dir = _config.XverExportForPublisher
                ? Path.Combine(dir, "input", "vocabulary")
                : Path.Combine(dir, "package");
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            // write the value set to a file
            string filename = $"ValueSet-{vs.Id}.json";
            string path = Path.Combine(dir, filename);
            File.WriteAllText(path, vs.ToJson(new FhirJsonSerializationSettings() { Pretty = true }));
        }
    }


    private void writeXverStructures(
        List<PackageXverSupport> packageSupports,
        int focusPackageIndex,
        Dictionary<(int sourceElementKey, int targetPackageId), (StructureDefinition, DbExtensionSubstitution?)> xverExtensions,
        string fhirDir)
    {
        Dictionary<int, DbFhirPackage> packageDict = packageSupports.Select(ps => ps.Package).ToDictionary(p => p.Key);
        DbFhirPackage focusPackage = packageSupports[focusPackageIndex].Package;

        ILookup<int, SnapshotGenerator?> generatorsById = packageSupports.ToLookup(ps => ps.Package.Key, ps => ps.SnapshotGenerator);

        // iterate over the structures
        foreach (((int sourceKey, int targetPackageId), (StructureDefinition sd, DbExtensionSubstitution? extensionSubstitution)) in xverExtensions)
        {
            // if this extension is substituted out, we do not need to write it
            if (extensionSubstitution != null)
            {
                continue;
            }

            if (_config.XverGenerateSnapshots)
            {
                try
                {
                    if (sd.Snapshot == null)
                    {
                        // create a new snapshot
                        sd.Snapshot = new StructureDefinition.SnapshotComponent();
                    }

                    // a valid snapshot will always have at least the root element
                    if (sd.Snapshot.Element.Count == 0)
                    {
                        //sd.Snapshot.Element = packageSupports[targetPackageId].SnapshotGenerator?.GenerateAsync(sd).Result ?? [];
                        sd.Snapshot.Element = generatorsById[targetPackageId]?.FirstOrDefault()?.GenerateAsync(sd).Result ?? [];
                    }
                }
                catch (Exception) { }
            }

            DbFhirPackage targetPackage = packageDict[targetPackageId];

            string packageId = getPackageId(focusPackage, targetPackage);

            // build a path for this direction
            string dir = createExportPackageDir(fhirDir, focusPackage, targetPackage);

            dir = _config.XverExportForPublisher
                ? Path.Combine(dir, "input", "extensions")
                : Path.Combine(dir, "package");
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            // write the structure to a file
            string filename = $"StructureDefinition-{sd.Id}.json";

            string path = Path.Combine(dir, filename);
            File.WriteAllText(path, sd.ToJson(new FhirJsonSerializationSettings() { Pretty = true }));
        }
    }


    private void buildXverStructures(
        List<PackageXverSupport> packageSupports,
        int sourcePackageIndex,
        Dictionary<(int sourceVsKey, int targetPackageId), ValueSet> xverValueSets,
        Dictionary<(int sourceElementKey, int targetPackageId), (StructureDefinition, DbExtensionSubstitution?)> xverExtensions,
        Dictionary<(int, string), List<List<XverOutcome>>> xverOutcomes,
        FhirArtifactClassEnum artifactClass)
    {
        DbFhirPackage sourcePackage = packageSupports[sourcePackageIndex].Package;

        // resolve the extension types for this version of FHIR
        HashSet<string> allowedExtensionTypes = getAllowedExtensionTypes(sourcePackage.Key);

        // get the list of structures in this version
        List<DbStructureDefinition> structures = DbStructureDefinition.SelectList(
                _db!.DbConnection,
                FhirPackageKey: sourcePackage.Key,
                ArtifactClass: artifactClass);

        // iterate over the structures
        foreach (DbStructureDefinition sd in structures)
        {
            if (!xverOutcomes.ContainsKey((sourcePackageIndex, sd.Name)))
            {
                xverOutcomes[(sourcePackageIndex, sd.Name)] = [];
                for (int i = 0; i < packageSupports.Count; i++)
                {
                    xverOutcomes[(sourcePackageIndex, sd.Name)].Add([]);
                }
            }

            // build a graph for this structure
            DbGraphSd sdGraph = new()
            {
                DB = _db!.DbConnection,
                Packages = packageSupports.Select(ps => ps.Package).ToList(),
                KeySd = sd,
            };

            // build a dictionary of all element projections, by element key
            Dictionary<int, List<DbGraphSd.DbElementRow>> elementProjectionDict = [];
            foreach (DbGraphSd.DbSdRow sdRow in sdGraph.Projection)
            {
                foreach (DbGraphSd.DbElementRow sdElementRow in sdRow.Projection)
                {
                    if (sdElementRow.KeyCell == null)
                    {
                        continue;
                    }

                    if (!elementProjectionDict.TryGetValue(sdElementRow.KeyCell.Element.Key, out List<DbGraphSd.DbElementRow>? elementList))
                    {
                        elementList = [];
                        elementProjectionDict.Add(sdElementRow.KeyCell.Element.Key, elementList);
                    }

                    elementList.Add(sdElementRow);
                }
            }

            List<HashSet<int>> generatedElementKeys = [];
            for (int i = 0; i < packageSupports.Count; i++)
            {
                generatedElementKeys.Add([]);
            }

            List<bool> structureMapsToBasic = [];
            for (int i = 0; i < packageSupports.Count; i++)
            {
                if (sdGraph.Projection.Any(sdRow => sdRow[i] != null))
                {
                    structureMapsToBasic.Add(false);
                    continue;
                }

                structureMapsToBasic.Add(true);
            }

            // iterate over the elements of our structure
            foreach (DbElement element in DbElement.SelectList(_db!.DbConnection, StructureKey: sd.Key, orderByProperties: [nameof(DbElement.ResourceFieldOrder)]))
            {
                // resolve the projection rows for this element
                List<DbGraphSd.DbElementRow> elementProjection = elementProjectionDict[element.Key];

                bool extensionNeeded = false;

                // work upwards first
                for (int currentIndex = sourcePackageIndex; currentIndex < (packageSupports.Count - 1); currentIndex++)
                {
                    int targetPackageIndex = currentIndex + 1;
                    DbFhirPackage targetPackage = packageSupports[targetPackageIndex].Package;

                    // if we already generated the parent of this element, flag it as added and move on
                    if ((element.ParentElementKey != null) &&
                        generatedElementKeys[targetPackageIndex].Contains(element.ParentElementKey.Value))
                    {
                        // add this element to the generated list
                        generatedElementKeys[targetPackageIndex].Add(element.Key);
                        xverOutcomes[(sourcePackageIndex, sd.Name)][targetPackageIndex].Add(new()
                        {
                            SourcePackageKey = sourcePackage.Key,
                            SourceStructureName = sd.Name,
                            SourceElementId = element.Id,
                            SourceElementFieldOrder = element.ResourceFieldOrder,
                            TargetPackageKey = targetPackage.Key,
                            OutcomeCode = XverOutcomeCodes.UseExtensionFromAncestor,
                            TargetElementId = null,
                            TargetExtensionUrl = null,
                            ReplacementExtensionUrl = null,
                        });
                        continue;
                    }

                    // do not generate if this element is part of a mapped structure and has an equivalent in the target's basic resource definition
                    if (structureMapsToBasic[targetPackageIndex] &&
                        (element.ParentElementKey != null) &&
                        packageSupports[targetPackageIndex].BasicElements.Contains(element.Path.Substring(sd.Name.Length)))
                    {
                        xverOutcomes[(sourcePackageIndex, sd.Name)][targetPackageIndex].Add(new()
                        {
                            SourcePackageKey = sourcePackage.Key,
                            SourceStructureName = sd.Name,
                            SourceElementId = element.Id,
                            SourceElementFieldOrder = element.ResourceFieldOrder,
                            TargetPackageKey = targetPackage.Key,
                            OutcomeCode = XverOutcomeCodes.UseBasicElement,
                            TargetElementId = null,
                            TargetExtensionUrl = null,
                            ReplacementExtensionUrl = null,
                        });
                        continue;
                    }

                    List<DbElementComparison> comparisons = [];

                    // resolve the current column
                    List<DbGraphSd.DbElementCell?> sourceCells = elementProjection
                        .Select(row => row[currentIndex])
                        .ToList();

                    // if we have not hit a need for the extension yet in this direction, test the curent pair
                    if (!extensionNeeded)
                    {
                        // check to see if this element has already been mapped in the previous version
                        if ((currentIndex > sourcePackageIndex) &&
                            generatedElementKeys[currentIndex - 1].Contains(element.Key))
                        {
                            extensionNeeded = true;
                        }
                        // only generate entire structures if there is no mappable structure in the target
                        else if (element.ResourceFieldOrder == 0)
                        {
                            extensionNeeded = structureMapsToBasic[targetPackageIndex];
                        }
                        // if we have no mappings, we need a new extension
                        else if (sourceCells.Count == 0)
                        {
                            extensionNeeded = true;
                        }
                        // if all cells or right projections are null, we need an extension
                        else if (sourceCells.All(cell => cell?.RightCell == null))
                        {
                            extensionNeeded = true;
                        }
                        // need to check aggregate relationship
                        else
                        {
                            // easier to check inverse here
                            extensionNeeded = true;

                            List<DbGraphSd.DbElementCell> matchedCells = sourceCells
                                .Where(c => (c?.RightComparison?.Relationship == CMR.Equivalent) || (c?.RightComparison?.Relationship == CMR.SourceIsNarrowerThanTarget))
                                .Select(c => c!)
                                .ToList();

                            if (matchedCells.Count != 0)
                            {
                                extensionNeeded = false;
                                XverOutcomeCodes oc = matchedCells.Count > 1
                                    ? XverOutcomeCodes.UseOneOfElements
                                    : matchedCells[0].RightCell?.Element.Id == element.Id
                                        ? XverOutcomeCodes.UseElementSameName
                                        : XverOutcomeCodes.UseElementRenamed;

                                xverOutcomes[(sourcePackageIndex, sd.Name)][targetPackageIndex].Add(new()
                                {
                                    SourcePackageKey = sourcePackage.Key,
                                    SourceStructureName = sd.Name,
                                    SourceElementId = element.Id,
                                    SourceElementFieldOrder = element.ResourceFieldOrder,
                                    TargetPackageKey = targetPackage.Key,
                                    OutcomeCode = oc,
                                    TargetElementId = string.Join(',', matchedCells.Select(c => c.RightCell?.Element.Id)),
                                    TargetExtensionUrl = null,
                                    ReplacementExtensionUrl = null,
                                });
                            }

                            //foreach (DbGraphSd.DbElementCell? cell in sourceCells)
                            //{
                            //    // do not need to generate if equivalent or source is NARROWER
                            //    if ((cell?.RightComparison?.Relationship == CMR.Equivalent) ||
                            //        (cell?.RightComparison?.Relationship == CMR.SourceIsNarrowerThanTarget))
                            //    {
                            //        extensionNeeded = false;

                            //        if (cell!.RightCell?.Element.Id == element.Id)
                            //        {
                            //            xverOutcomes[(sourcePackageIndex, sd.Name)][targetPackageIndex].Add(new()
                            //            {
                            //                SourcePackageKey = sourcePackage.Key,
                            //                SourceStructureName = sd.Name,
                            //                SourceElementId = element.Id,
                            //                SourceElementFieldOrder = element.ResourceFieldOrder,
                            //                TargetPackageKey = targetPackage.Key,
                            //                OutcomeCode = XverOutcomeCodes.UseElementSameName,
                            //                TargetElementId = cell!.RightCell!.Element.Id,
                            //                TargetExtensionUrl = null,
                            //                ReplacementExtensionUrl = null,
                            //            });
                            //        }
                            //        else
                            //        {
                            //            xverOutcomes[(sourcePackageIndex, sd.Name)][targetPackageIndex].Add(new()
                            //            {
                            //                SourcePackageKey = sourcePackage.Key,
                            //                SourceStructureName = sd.Name,
                            //                SourceElementId = element.Id,
                            //                SourceElementFieldOrder = element.ResourceFieldOrder,
                            //                TargetPackageKey = targetPackage.Key,
                            //                OutcomeCode = XverOutcomeCodes.UseElementRenamed,
                            //                TargetElementId = cell!.RightCell!.Element.Id,
                            //                TargetExtensionUrl = null,
                            //                ReplacementExtensionUrl = null,
                            //            });
                            //        }

                            //        break;
                            //    }
                            //}
                        }
                    }

                    // if we still do not need an extension, go to next package
                    if (!extensionNeeded)
                    {
                        continue;
                    }

                    // check to see if we have already generated this extension
                    if (xverExtensions.ContainsKey((element.Key, targetPackage.Key)))
                    {
                        continue;
                    }

                    foreach (DbGraphSd.DbElementCell? cell in sourceCells)
                    {
                        if (cell?.RightComparison == null)
                        {
                            continue;
                        }

                        comparisons.Add(cell.RightComparison);
                    }

                    // build an extension for the original source element to target the current target version
                    StructureDefinition? extSd = createExtensionSd(
                        sourcePackageIndex,
                        packageSupports[sourcePackageIndex],
                        targetPackageIndex,
                        packageSupports[targetPackageIndex],
                        sd,
                        element,
                        comparisons,
                        elementProjectionDict,
                        xverValueSets);

                    if (extSd != null)
                    {
                        DbExtensionSubstitution? extSub = DbExtensionSubstitution.SelectSingle(_db!.DbConnection, SourceElementId: element.Id);

                        xverExtensions.Add((element.Key, packageSupports[targetPackageIndex].Package.Key), (extSd, extSub));
                        generatedElementKeys[targetPackageIndex].Add(element.Key);

                        xverOutcomes[(sourcePackageIndex, sd.Name)][targetPackageIndex].Add(new()
                        {
                            SourcePackageKey = sourcePackage.Key,
                            SourceStructureName = sd.Name,
                            SourceElementId = element.Id,
                            SourceElementFieldOrder = element.ResourceFieldOrder,
                            TargetPackageKey = targetPackage.Key,
                            OutcomeCode = XverOutcomeCodes.UseExtension,
                            TargetElementId = null,
                            TargetExtensionUrl = extSd.Url,
                            ReplacementExtensionUrl = extSub?.ReplacementUrl
                        });
                    }
                }

                extensionNeeded = false;

                // then work downwards
                for (int currentIndex = sourcePackageIndex; currentIndex > 0; currentIndex--)
                {
                    int targetPackageIndex = currentIndex - 1;
                    DbFhirPackage targetPackage = packageSupports[targetPackageIndex].Package;

                    // if we already generated the parent of this element, flag it as added and move on
                    if ((element.ParentElementKey != null) &&
                        generatedElementKeys[targetPackageIndex].Contains(element.ParentElementKey.Value))
                    {
                        // add this element to the generated list
                        generatedElementKeys[targetPackageIndex].Add(element.Key);
                        xverOutcomes[(sourcePackageIndex, sd.Name)][targetPackageIndex].Add(new()
                        {
                            SourcePackageKey = sourcePackage.Key,
                            SourceStructureName = sd.Name,
                            SourceElementId = element.Id,
                            SourceElementFieldOrder = element.ResourceFieldOrder,
                            TargetPackageKey = targetPackage.Key,
                            OutcomeCode = XverOutcomeCodes.UseExtensionFromAncestor,
                            TargetElementId = null,
                            TargetExtensionUrl = null,
                            ReplacementExtensionUrl = null,
                        });
                        continue;
                    }

                    // do not generate if this element is equivalent in the target basic resource
                    if (structureMapsToBasic[targetPackageIndex] &&
                        (element.ParentElementKey != null) &&
                        packageSupports[targetPackageIndex].BasicElements.Contains(element.Path.Substring(sd.Name.Length)))
                    {
                        xverOutcomes[(sourcePackageIndex, sd.Name)][targetPackageIndex].Add(new()
                        {
                            SourcePackageKey = sourcePackage.Key,
                            SourceStructureName = sd.Name,
                            SourceElementId = element.Id,
                            SourceElementFieldOrder = element.ResourceFieldOrder,
                            TargetPackageKey = targetPackage.Key,
                            OutcomeCode = XverOutcomeCodes.UseBasicElement,
                            TargetElementId = null,
                            TargetExtensionUrl = null,
                            ReplacementExtensionUrl = null,
                        });
                        continue;
                    }

                    List<DbElementComparison> comparisons = [];

                    // resolve the current column
                    List<DbGraphSd.DbElementCell?> sourceCells = elementProjection
                        .Select(row => row[currentIndex])
                        .ToList();

                    // if we have not hit a need for the extension yet in this direction, test the curent pair
                    if (!extensionNeeded)
                    {
                        // check to see if this element has been mapped in the previous version
                        if ((currentIndex < sourcePackageIndex) &&
                            generatedElementKeys[currentIndex + 1].Contains(element.Key))
                        {
                            extensionNeeded = true;
                        }
                        // only generate entire structures if there is no mappable structure in the target
                        else if (element.ResourceFieldOrder == 0)
                        {
                            extensionNeeded = structureMapsToBasic[targetPackageIndex];
                        }
                        // if we have no mappings, we need a new extension
                        else if (sourceCells.Count == 0)
                        {
                            extensionNeeded = true;
                        }
                        // if all cells or left projections are null, we need an extension
                        else if (sourceCells.All(cell => cell?.LeftComparison == null))
                        {
                            extensionNeeded = true;
                        }
                        // need to check aggregate relationship
                        else
                        {
                            // easier to check inverse here
                            extensionNeeded = true;

                            List<DbGraphSd.DbElementCell> matchedCells = sourceCells
                                .Where(c => (c?.LeftComparison?.Relationship == CMR.Equivalent) || (c?.LeftComparison?.Relationship == CMR.SourceIsNarrowerThanTarget))
                                .Select(c => c!)
                                .ToList();

                            if (matchedCells.Count != 0)
                            {
                                extensionNeeded = false;
                                XverOutcomeCodes oc = matchedCells.Count > 1
                                    ? XverOutcomeCodes.UseOneOfElements
                                    : matchedCells[0].LeftCell?.Element.Id == element.Id
                                        ? XverOutcomeCodes.UseElementSameName
                                        : XverOutcomeCodes.UseElementRenamed;

                                xverOutcomes[(sourcePackageIndex, sd.Name)][targetPackageIndex].Add(new()
                                {
                                    SourcePackageKey = sourcePackage.Key,
                                    SourceStructureName = sd.Name,
                                    SourceElementId = element.Id,
                                    SourceElementFieldOrder = element.ResourceFieldOrder,
                                    TargetPackageKey = targetPackage.Key,
                                    OutcomeCode = oc,
                                    TargetElementId = string.Join(',', matchedCells.Select(c => c.LeftCell?.Element.Id)),
                                    TargetExtensionUrl = null,
                                    ReplacementExtensionUrl = null,
                                });
                            }

                            //foreach (DbGraphSd.DbElementCell? cell in sourceCells)
                            //{
                            //    // do not need to generate if equivalent or source is NARROWER
                            //    if ((cell?.LeftComparison?.Relationship == CMR.Equivalent) ||
                            //        (cell?.LeftComparison?.Relationship == CMR.SourceIsNarrowerThanTarget))
                            //    {
                            //        extensionNeeded = false;

                            //        if (cell!.LeftCell?.Element.Id == element.Id)
                            //        {
                            //            xverOutcomes[(sourcePackageIndex, sd.Name)][targetPackageIndex].Add(new()
                            //            {
                            //                SourcePackageKey = sourcePackage.Key,
                            //                SourceStructureName = sd.Name,
                            //                SourceElementId = element.Id,
                            //                SourceElementFieldOrder = element.ResourceFieldOrder,
                            //                TargetPackageKey = targetPackage.Key,
                            //                OutcomeCode = XverOutcomeCodes.UseElementSameName,
                            //                TargetElementId = cell!.LeftCell!.Element.Id,
                            //                TargetExtensionUrl = null,
                            //                ReplacementExtensionUrl = null,
                            //            });
                            //        }
                            //        else
                            //        {
                            //            xverOutcomes[(sourcePackageIndex, sd.Name)][targetPackageIndex].Add(new()
                            //            {
                            //                SourcePackageKey = sourcePackage.Key,
                            //                SourceStructureName = sd.Name,
                            //                SourceElementId = element.Id,
                            //                SourceElementFieldOrder = element.ResourceFieldOrder,
                            //                TargetPackageKey = targetPackage.Key,
                            //                OutcomeCode = XverOutcomeCodes.UseElementRenamed,
                            //                TargetElementId = cell!.LeftCell!.Element.Id,
                            //                TargetExtensionUrl = null,
                            //                ReplacementExtensionUrl = null,
                            //            });
                            //        }
                            //        break;
                            //    }
                            //}
                        }
                    }

                    // if we still do not need an extension, go to next package
                    if (!extensionNeeded)
                    {
                        continue;
                    }

                    // check to see if we have already generated this extension
                    if (xverExtensions.ContainsKey((element.Key, targetPackage.Key)))
                    {
                        continue;
                    }

                    foreach (DbGraphSd.DbElementCell? cell in sourceCells)
                    {
                        if (cell?.LeftComparison == null)
                        {
                            continue;
                        }

                        comparisons.Add(cell.LeftComparison);
                    }

                    // build an extension for the original source element to target the current target version
                    StructureDefinition? extSd = createExtensionSd(
                        sourcePackageIndex,
                        packageSupports[sourcePackageIndex],
                        targetPackageIndex,
                        packageSupports[targetPackageIndex],
                        sd,
                        element,
                        comparisons,
                        elementProjectionDict,
                        xverValueSets);

                    if (extSd != null)
                    {
                        DbExtensionSubstitution? extSub = DbExtensionSubstitution.SelectSingle(_db!.DbConnection, SourceElementId: element.Id);

                        xverExtensions.Add((element.Key, targetPackage.Key), (extSd, extSub));
                        generatedElementKeys[targetPackageIndex].Add(element.Key);

                        xverOutcomes[(sourcePackageIndex, sd.Name)][targetPackageIndex].Add(new()
                        {
                            SourcePackageKey = sourcePackage.Key,
                            SourceStructureName = sd.Name,
                            SourceElementId = element.Id,
                            SourceElementFieldOrder = element.ResourceFieldOrder,
                            TargetPackageKey = targetPackage.Key,
                            OutcomeCode = XverOutcomeCodes.UseExtension,
                            TargetElementId = null,
                            TargetExtensionUrl = extSd.Url,
                            ReplacementExtensionUrl = extSub?.ReplacementUrl,
                        });
                    }
                }
            }
        }

        return;

        HashSet<string> getAllowedExtensionTypes(int packageKey)
        {
            // resolve the 'extension' structure definition
            DbStructureDefinition? extSd = DbStructureDefinition.SelectSingle(
                _db!.DbConnection,
                FhirPackageKey: packageKey,
                Name: "Extension");

            if (extSd == null)
            {
                return [];
            }

            // get the 'value[x]' element
            DbElement? extValueElement = DbElement.SelectSingle(
                _db!.DbConnection,
                StructureKey: extSd.Key,
                Id: "Extension.value[x]");

            if (extValueElement == null)
            {
                return [];
            }

            // get the types allowed in the Extension.value element
            List<DbElementType> extValueTypes = DbElementType.SelectList(
                _db!.DbConnection,
                ElementKey: extValueElement.Key);

            return new HashSet<string>(extValueTypes.Select(et => et.TypeName!));
        }
    }

    private StructureDefinition? createExtensionSd(
        int sourceIndex,
        PackageXverSupport sourcePackageSupport,
        int targetIndex,
        PackageXverSupport targetPackageSupport,
        DbStructureDefinition sd,
        DbElement element,
        List<DbElementComparison> relevantComparisons,
        Dictionary<int, List<DbGraphSd.DbElementRow>> elementProjectionDict,
        Dictionary<(int sourceVsKey, int targetPackageId), ValueSet> xverValueSets)
    {
        DbFhirPackage sourcePackage = sourcePackageSupport.Package;
        DbFhirPackage targetPackage = targetPackageSupport.Package;

        HashSet<string> basicElementPaths = targetPackageSupport.BasicElements;

        //string sdId = $"{sourcePackage.ShortName}-{element.Path}-for-{targetPackage.ShortName}";
        string sdId = $"ext-{sourcePackage.ShortName}-{collapsePathForId(element.Path)}";

        bool isRootElement = element.ResourceFieldOrder == 0;
        int elementPathLen = element.Path.Length;

        List<StructureDefinition.ContextComponent> contexts = [];

        // if our source element is a resource or datatype, we can only apply it to the basic resource
        if (isRootElement)
        {
            contexts.Add(new()
            {
                Type = StructureDefinition.ExtensionContextType.Element,
                Expression = "Basic",
            });
        }
        else
        {
            HashSet<string> contextElementPaths = [];

            discoverContexts(contextElementPaths, sourceIndex, targetIndex, targetPackageSupport, element, elementProjectionDict);

            if (contextElementPaths.Count != 0)
            {
                foreach (string path in contextElementPaths.Distinct().Order())
                {
                    contexts.Add(new()
                    {
                        Type = StructureDefinition.ExtensionContextType.Element,
                        Expression = path,
                    });
                }
            }
        }

        // fallback to element if we have no contexts
        if (contexts.Count == 0)
        {
            contexts.Add(new()
            {
                Type = StructureDefinition.ExtensionContextType.Element,
                Expression = "Element",
            });
        }

        StructureDefinition extSd = new()
        {
            Id = sdId,
            //Url = $"http://hl7.org/fhir/uv/xver/{sourcePackage.FhirVersionShort}/StructureDefinition/extension-{element.Path.Replace("[x]", string.Empty)}",
            Url = $"http://hl7.org/fhir/{sourcePackage.FhirVersionShort}/StructureDefinition/extension-{element.Path.Replace("[x]", string.Empty)}",
            Name = sdId.Replace('-', '_').Replace('.', '_'),
            Version = _crossDefinitionVersion,
            FhirVersion = EnumUtility.ParseLiteral<FHIRVersion>(targetPackageSupport!.Package.PackageVersion) ?? FHIRVersion.N5_0_0,
            DateElement = new FhirDateTime(DateTimeOffset.Now),
            Title = $"Cross-version Extension for {sourcePackage.ShortName}.{element.Path} for use in FHIR {targetPackage.ShortName}",
            Description = $"This cross-version extension represents {element.Path} from {sd.VersionedUrl} for use in FHIR {targetPackage.ShortName}.",
            Status = PublicationStatus.Draft,
            Experimental = false,
            Kind = StructureDefinition.StructureDefinitionKind.ComplexType,
            Abstract = false,
            Context = contexts,
            Type = "Extension",
            BaseDefinition = "http://hl7.org/fhir/StructureDefinition/Extension",
            Derivation = StructureDefinition.TypeDerivationRule.Constraint,
            Differential = new()
            {
                Element = [],
            },
        };

        Dictionary<int, string> extPathByElementKey = [];

        // add this element to the structure, including the child elements
        addElementToExtension(
            extSd,
            "Extension",
            "Extension",
            null,
            sourcePackageSupport,
            targetPackageSupport,
            sd,
            element,
            relevantComparisons,
            xverValueSets);

        // fix the URL in the definition (needs to be last element)
        extSd.Differential.Element.Add(new()
        {
            ElementId = "Extension.url",
            Path = "Extension.url",
            Min = 1,
            Max = "1",
            Fixed = new FhirUri(extSd.Url)
        });

        return extSd;
    }


    private void addElementToExtension(
        StructureDefinition extSd,
        string extElementId,
        string extElementPath,
        string? sliceName,
        PackageXverSupport sourcePackageSupport,
        PackageXverSupport targetPackageSupport,
        DbStructureDefinition sd,
        DbElement element,
        List<DbElementComparison> relevantComparisons,
        Dictionary<(int sourceVsKey, int targetPackageId), ValueSet> xverValueSets)
    {
        // do not build extensions for extension or base elements
        switch (element.FullCollatedTypeLiteral)
        {
            case "Extension":
            case "Base":
                return;
        }

        // skip id elements, they are part of every element and do not need to be written
        if (extElementId.EndsWith(".extension:id", StringComparison.Ordinal))
        {
            return;
        }

        HashSet<string> basicElementPaths = targetPackageSupport.BasicElements;

        // check to see if this element is in the 'basic' resource of this version (do not add)
        if ((element.Path.Length > sd.Name.Length) &&
            basicElementPaths.Contains(element.Path.Substring(sd.Name.Length)))
        {
            return;
        }

        int sourceCol = sourcePackageSupport.PackageIndex;
        int targetCol = targetPackageSupport.PackageIndex;

        string? reason = relevantComparisons.Count == 0
            ? null
            : string.Join(' ', relevantComparisons.Select(c => c.UserMessage ?? string.Empty));

        (string? edShortText, string? edDefinition, string? edComment) = getTextForExtensionElement(element, reason);

        ElementDefinition extEd = new()
        {
            ElementId = extElementId,
            Path = extElementPath,
            SliceName = sliceName,
            Short = edShortText,
            Definition = edDefinition,
            Comment = edComment,
            Min = element.MinCardinality,
            Max = element.MaxCardinalityString,
            IsModifier = element.IsModifier,
            IsModifierReason = element.IsModifierReason
                ?? (element.IsModifier == true ? $"This extension is a modifier because the target element {element.Id} is flagged IsModifier" : null),
        };

        extSd.Differential.Element.Add(extEd);

        // if there are no child elements, we are done
        if (element.ChildElementCount != 0)
        {
            ElementDefinition edForChildren = new()
            {
                ElementId = extElementId + ".extension",
                Path = extElementPath + ".extension",
                Slicing = new()
                {
                    Discriminator = [
                        new() {
                            Type = ElementDefinition.DiscriminatorType.Value,
                            Path = "url",
                        }
                    ],
                    Ordered = false,
                    Rules = ElementDefinition.SlicingRules.Closed,
                },
                Min = 0,
                Max = "*",
            };

            // if we have child extensions, we cannot have a value
            extSd.Differential.Element.Add(edForChildren);

            int minRequired = 0;

            // iterate over our child elements and add them
            foreach (DbElement childElement in DbElement.SelectList(
                _db!.DbConnection,
                ParentElementKey: element.Key,
                orderByProperties: [nameof(DbElement.ResourceFieldOrder)]))
            {
                minRequired += childElement.MinCardinality;

                // add this child element to the extension
                addElementToExtension(
                    extSd,
                    $"{extElementId}.extension:{childElement.Name}",
                    $"{extElementPath}.extension",
                    childElement.Name,
                    sourcePackageSupport,
                    targetPackageSupport,
                    sd,
                    childElement,
                    relevantComparisons,
                    xverValueSets);
            }

            //extSd.Differential.Element.Add(new()
            //{
            //    ElementId = extElementId + ".value[x]",
            //    Path = extElementPath + ".value[x]",
            //    Max = "0",
            //});

            edForChildren.Min = minRequired;

            return;
        }

        //bool addedEdExt = false;

        bool addedEdValue = false;
        //bool addedEdValueExtension = false;
        //ElementDefinition? extEdValueExtension = null;
        ElementDefinition extensionEdValue = new()
        {
            ElementId = extElementId + ".value[x]",
            Path = extElementPath + ".value[x]",
            Short = edShortText,
            Definition = edDefinition,
            Comment = edComment,
            Base = new()
            {
                Path = "Extension.value[x]",
                Min = 0,
                Max = "1",
            },
            Type = [],
        };

        // check to see if we need to add a binding
        if ((element.ValueSetBindingStrength != null) &&
            (element.BindingValueSet != null))
        {
            string vsUrl;

            if ((element.BindingValueSetKey != null) &&
                xverValueSets.TryGetValue((element.BindingValueSetKey.Value, targetPackageSupport.Package.Key), out ValueSet? vs))
            {
                vsUrl = vs.Url;
            }
            else
            {
                vsUrl = element.BindingValueSet;
            }

            extensionEdValue.Binding = new()
            {
                Strength = element.ValueSetBindingStrength,
                Description = element.BindingDescription,
                ValueSet = vsUrl,
            };
        }

        // build the value types
        List<DbElementType> elementValueTypes = DbElementType.SelectList(
            _db!.DbConnection,
            ElementKey: element.Key);

        // setup a lookup by type name
        ILookup<string, DbElementType> collectedValueTypes = elementValueTypes.ToLookup(t => t.TypeName ?? string.Empty);
        List<string> extAllowedTypes = [];
        Dictionary<string, List<string>> extReplaceableTypes = [];
        List<string> extMappedTypes = [];

        HashSet<string> quantityProfilesMovedToTypes = [];

        // categorize the types based on how we process them
        foreach (string valueTypeName in elementValueTypes.Select(t => t.TypeName ?? string.Empty).Distinct())
        {
            if (targetPackageSupport.AllowedExtensionTypes.Contains(valueTypeName))
            {
                // check for this being the "Quantity" type to do special type profile handling
                if (valueTypeName == "Quantity")
                {
                    List<string> typeProfiles = collectedValueTypes[valueTypeName].Select(t => t.TypeProfile).Where(t => t != null)!.ToList<string>();

                    if (typeProfiles.Count > 0)
                    {
                        // check the profiled types
                        foreach (string typeProfile in typeProfiles)
                        {
                            string tpShort = typeProfile.Split('/')[^1];
                            if (targetPackageSupport.AllowedExtensionTypes.Contains(tpShort))
                            {
                                quantityProfilesMovedToTypes.Add(tpShort);
                                extAllowedTypes.Add(tpShort);
                            }
                        }

                        // skip this quantity if it was only this type
                        if (typeProfiles.Count == quantityProfilesMovedToTypes.Count)
                        {
                            continue;
                        }
                    }
                }

                extAllowedTypes.Add(valueTypeName);
                continue;
            }

            if (FhirTypeMappings.PrimitiveTypeFallbacks.TryGetValue(valueTypeName, out string? replacementType))
            {
                if (!extReplaceableTypes.TryGetValue(replacementType, out List<string>? replaceableTypes))
                {
                    replaceableTypes = [];
                    extReplaceableTypes.Add(replacementType, replaceableTypes);
                }
                extReplaceableTypes[replacementType].Add(valueTypeName);
                continue;
            }

            extMappedTypes.Add(valueTypeName);
        }

        HashSet<string> usedTypes = [];
        ElementDefinition? extensionDatatypeValueElement = null;

        // process mapped types (extension before value)
        foreach (string typeName in extMappedTypes)
        {
            addDatatypeExtension(
                extSd,
                element,
                sourcePackageSupport,
                ref extensionDatatypeValueElement,
                extElementId,           //extElementId + ".value[x].extension",
                extElementPath,         //extElementPath + ".value[x].extension",
                typeName);

            // resolve this structure
            DbStructureDefinition? etSd = DbStructureDefinition.SelectSingle(
                _db!.DbConnection,
                FhirPackageKey: sourcePackageSupport.Package.Key,
                Name: typeName);

            if (etSd == null)
            {
                continue;
            }

            // get the root element of the structure
            DbElement etRootElement = DbElement.SelectSingle(_db!.DbConnection, StructureKey: etSd.Key, ResourceFieldOrder: 0)
                ?? throw new Exception($"Failed to resolve the root element of {etSd.Name} ({etSd.Key})");

            // get the elements for this structure
            List<DbElement> etElements = DbElement.SelectList(
                _db!.DbConnection,
                StructureKey: etSd.Key,
                ParentElementKey: etRootElement.Key,
                orderByProperties: [nameof(DbElement.ResourceFieldOrder)]);

            // iterate over the elements to add them to the extension
            foreach (DbElement etElement in etElements)
            {
                addElementToExtension(
                    extSd,
                    $"{extElementId}.extension:{etElement.Name}",
                    $"{extElementPath}.extension",
                    etElement.Name,
                    sourcePackageSupport,
                    targetPackageSupport,
                    sd,
                    etElement,
                    relevantComparisons,
                    xverValueSets);
            }
        }

        // process replaced quantity types
        foreach (string typeName in quantityProfilesMovedToTypes)
        {
            if (usedTypes.Contains(typeName))
            {
                continue;
            }

            // add the value element if we are supposed to
            if (!addedEdValue)
            {
                extSd.Differential.Element.Add(extensionEdValue);
                addedEdValue = true;
            }

            // consolidate profiles
            List<string> typeProfiles = collectedValueTypes.Contains(typeName) ? collectedValueTypes[typeName].Select(t => t.TypeProfile).Where(t => t != null)!.ToList<string>() : [];
            List<string> targetProfiles = collectedValueTypes.Contains(typeName) ? collectedValueTypes[typeName].Select(t => t.TargetProfile).Where(t => t != null)!.ToList<string>() : [];

            // create a new type reference
            ElementDefinition.TypeRefComponent? edValueType = new()
            {
                Code = typeName,
                ProfileElement = typeProfiles.Select(v => new Canonical(v)).ToList(),
                TargetProfileElement = targetProfiles.Select(v => new Canonical(v)).ToList(),
            };

            extensionEdValue.Type.Add(edValueType);

            // check to see if we use the type to contain data from another type too (need extensions)
            if (extReplaceableTypes.TryGetValue(typeName, out List<string>? replaceableTypes))
            {
                // add each of the replaceable types
                foreach (string rt in replaceableTypes)
                {
                    addDatatypeExtension(
                        extSd,
                        element,
                        sourcePackageSupport,
                        ref extensionDatatypeValueElement,
                        extElementId,           //extElementId + ".value[x].extension",
                        extElementPath,         //extElementPath + ".value[x].extension",
                        rt);
                }
            }

            usedTypes.Add(typeName);
        }

        // process allowed and replaceable types
        foreach (string typeName in extAllowedTypes)
        {
            if (usedTypes.Contains(typeName))
            {
                continue;
            }

            // add the value element if we are supposed to
            if (!addedEdValue)
            {
                extSd.Differential.Element.Add(extensionEdValue);
                addedEdValue = true;
            }

            // consolidate profiles
            List<string> typeProfiles = collectedValueTypes[typeName].Select(t => t.TypeProfile).Where(t => t != null)!.ToList<string>();
            List<string> targetProfiles = collectedValueTypes[typeName].Select(t => t.TargetProfile).Where(t => t != null)!.ToList<string>();

            // remove any quantity type profiles that got promoted
            if ((typeName == "Quantity") &&
                (typeProfiles.Count > 0) &&
                (quantityProfilesMovedToTypes.Count > 0))
            {
                List<string> toRemove = typeProfiles.Where(tp => quantityProfilesMovedToTypes.Contains(tp.Split('/')[^1])).ToList();
                foreach (string tr in toRemove)
                {
                    typeProfiles.Remove(tr);
                }
            }

            // create a new type reference
            ElementDefinition.TypeRefComponent? edValueType = new()
            {
                Code = typeName,
                ProfileElement = typeProfiles.Select(v => new Canonical(v)).ToList(),
                TargetProfileElement = targetProfiles.Select(v => new Canonical(v)).ToList(),
            };

            extensionEdValue.Type.Add(edValueType);

            // check to see if we use the type to contain data from another type too (need extensions)
            if (extReplaceableTypes.TryGetValue(typeName, out List<string>? replaceableTypes))
            {
                // add each of the replaceable types
                foreach (string rt in replaceableTypes)
                {
                    addDatatypeExtension(
                        extSd,
                        element,
                        sourcePackageSupport,
                        ref extensionDatatypeValueElement,
                        extElementId,           //extElementId + ".value[x].extension",
                        extElementPath,         //extElementPath + ".value[x].extension",
                        rt);
                }
            }

            usedTypes.Add(typeName);
        }

        // check for any missed replaceable types
        foreach ((string typeName, List<string> replaceableTypes) in extReplaceableTypes)
        {
            if (usedTypes.Contains(typeName))
            {
                continue;
            }

            // add the value element if we are supposed to
            if (!addedEdValue)
            {
                extSd.Differential.Element.Add(extensionEdValue);
                addedEdValue = true;
            }

            // create a new type reference
            ElementDefinition.TypeRefComponent? edValueType = new()
            {
                Code = typeName,
            };

            extensionEdValue.Type.Add(edValueType);

            // add each of the replaceable types
            foreach (string rt in replaceableTypes)
            {
                addDatatypeExtension(
                    extSd,
                    element,
                    sourcePackageSupport,
                    ref extensionDatatypeValueElement,
                    extElementId,           //extElementId + ".value[x].extension",
                    extElementPath,         //extElementPath + ".value[x].extension",
                    rt);
            }

            usedTypes.Add(typeName);
        }

        return;
    }


    private void addDatatypeExtension(
        StructureDefinition extSd,
        DbElement sourceDbElement,
        PackageXverSupport sourcePackageSupport,
        ref ElementDefinition? extensionDatatypeValueElement,
        string parentId,
        string parentPath,
        string typeName)
    {
        // if we don't have the element already, we need to create the whole set
        if (extensionDatatypeValueElement == null)
        {
            extSd.Differential.Element.Add(new()
            {
                ElementId = parentId + ".extension:_datatype",
                Path = parentPath + ".extension",
                SliceName = "_datatype",
                Short = $"Data type name for {sourceDbElement.Id} from FHIR {sourcePackageSupport.Package.ShortName}",
                Definition = $"Data type name for {sourceDbElement.Id} from FHIR {sourcePackageSupport.Package.ShortName}",
                Min = 0,
                Max = "1",
                Type = [
                        new()
                        {
                            Code = "Extension",
                            Profile = ["http://hl7.org/fhir/StructureDefinition/_datatype"],
                        }
                    ],
            });

            extensionDatatypeValueElement = new()
            {
                ElementId = parentId + ".extension:_datatype.value[x]",
                Path = parentPath + ".extension.value[x]",
                Comment = $"Must be: {typeName}",
                Min = 1,
                Max = "1",
                Base = new()
                {
                    Path = "Extension.value[x]",
                    Min = 0,
                    Max = "1",
                },
                Type = [
                        new()
                            {
                                Code = "string",
                            }
                    ],
                Fixed = new FhirString(typeName),
            };

            extSd.Differential.Element.Add(extensionDatatypeValueElement);

            // done
            return;
        }

        // need to add this type
        extensionDatatypeValueElement.Fixed = null;
        extensionDatatypeValueElement.Comment += "|" + typeName;
    }


    private void discoverContexts(
        HashSet<string> contextElementPaths,
        int sourceIndex,
        int targetIndex,
        PackageXverSupport targetPackageSupport,
        DbElement element,
        Dictionary<int, List<DbGraphSd.DbElementRow>> elementProjectionDict)

    {
        // iterate over the element projection rows
        foreach ((DbGraphSd.DbElementRow elementRow, int elementRowNumber) in elementProjectionDict[element.Key].Select((r, i) => (r, i)))
        {
            // extract the element cell for this target
            DbGraphSd.DbElementCell? eCell = elementRow[targetIndex];

            // if the cell is not null, use the target path from the cell
            if (eCell != null)
            {
                contextElementPaths.Add(eCell.Element.Path);
                continue;
            }

            // need to try and find a parent for a path
            bool addedSomething = false;
            int? parentKey = element.ParentElementKey;
            while (parentKey != null)
            {
                int key = parentKey.Value;
                parentKey = null;

                if (elementProjectionDict.TryGetValue(key, out List<DbGraphSd.DbElementRow>? parentRows))
                {
                    foreach ((DbGraphSd.DbElementRow parentRow, int parentRowNumber) in parentRows.Select((r, i) => (r, i)))
                    {
                        // only match the equivalent row number
                        if (parentRowNumber != elementRowNumber)
                        {
                            continue;
                        }

                        // extract the element cell for this target
                        DbGraphSd.DbElementCell? parentCell = parentRow[targetIndex];
                        if (parentCell != null)
                        {
                            contextElementPaths.Add(parentCell.Element.Path);
                            addedSomething = true;
                            break;
                        }

                        DbGraphSd.DbElementCell? contextCell = parentRow[sourceIndex];
                        if (contextCell != null)
                        {
                            if (contextCell.Element.ResourceFieldOrder == 0)
                            {
                                // add this as the context
                                contextElementPaths.Add(contextCell.Element.Path);
                                addedSomething = true;
                                break;
                            }

                            parentKey = contextCell.Element.ParentElementKey;
                            break;
                        }
                    }
                }
            }

            if (!addedSomething)
            {
                // if we can't find anything that matches, see if this structure exists in the target
                string name = element.Path.Split('.')[0];

                if ((DbStructureDefinition.SelectCount(_db!.DbConnection, FhirPackageKey: targetPackageSupport.Package.Key, Id: name) != 0) ||
                    (targetPackageSupport.CoreDC?.ComplexTypesByName.ContainsKey(name) == true) ||
                    (targetPackageSupport.CoreDC?.ResourcesByName.ContainsKey(name) == true))
                {
                    contextElementPaths.Add(name);
                }

                // if we do not find *anything* that matches, the caller will default by adding Element
            }
        }
    }


    private (string? shortText, string? definition, string? comment) getTextForExtensionElement(DbElement ed, string? reason)
    {
        List<string> strings = [];

        if (!string.IsNullOrEmpty(ed.Short))
        {
            strings.Add(ed.Short);
        }

        if (!string.IsNullOrEmpty(ed.Definition) &&
            !ed.Definition.Equals(ed.Short, StringComparison.Ordinal) &&
            !ed.Definition.Equals(ed.Short + ".", StringComparison.Ordinal))
        {
            strings.Add(ed.Definition);
        }

        if (!string.IsNullOrEmpty(reason))
        {
            strings.Add(reason!);
        }

        switch (strings.Count)
        {
            case 0:
                return (null, null, null);

            case 1:
                return (strings[0], null, null);

            case 2:
                return (strings[0], strings[1], null);

            default:
                return (strings[0], strings[1], string.Join("\n", strings.Skip(2)));
        }
    }


    private string collapsePathForId(string path)
    {
        string pathClean = path.Replace("[x]", string.Empty);
        string[] components = pathClean.Replace("[x]", string.Empty).Split('.');
        switch (components.Length)
        {
            case 0:
                return pathClean;

            case 1:
                return pathClean;

            case 2:
                {
                    if (pathClean.Length > 45)
                    {
                        string rName = (components[0].Length > 20)
                            ? new string(components[0].Where(char.IsUpper).ToArray())
                            : components[0];

                        string eName = (components[1].Length > 20)
                            ? $"{components[1][0]}" + new string(components[1].Where(char.IsUpper).ToArray())
                            : components[1];

                        return rName + "." + eName;
                    }

                    return pathClean;
                }

            default:
                {
                    // use the full first and last, and one character from each in-between
                    if (components[0].Length > 20)
                    {
                        components[0] = new string(components[0].Where(char.IsUpper).ToArray());
                    }

                    for (int i = 1; i < components.Length - 1; i++)
                    {
                        if (components[i].Length > 3)
                        {
                            components[i] = $"{components[i][0]}{components[i][1]}";
                        }
                    }

                    if (components.Last().Length > 20)
                    {
                        components[components.Length - 1] = $"{components[components.Length - 1][0]}" + new string(components[0].Where(char.IsUpper).ToArray());
                    }

                    return string.Join('.', components);
                }

        }
    }


    private Dictionary<(int sourceVsKey, int targetPackageId), ValueSet> buildXverValueSets(
        List<DbFhirPackage> packages,
        int sourcePackageIndex)
    {
        DbFhirPackage sourcePackage = packages[sourcePackageIndex];

        Dictionary<(int sourceVsKey, int targetPackageId), ValueSet> xverValueSets = [];

        // get the list of value sets in this version that have a required binding
        List<DbValueSet> valueSets = DbValueSet.SelectList(
            _db!.DbConnection,
            FhirPackageKey: sourcePackage.Key,
            StrongestBindingCore: BindingStrength.Required);

        // iterate over the value sets
        foreach (DbValueSet vs in valueSets)
        {
            // build a graph for this value set
            DbGraphVs vsGraph = new()
            {
                DB = _db!.DbConnection,
                Packages = packages,
                KeyVs = vs,
            };

            // build a dictionary of all concept projections, by concept key
            Dictionary<int, List<DbGraphVs.DbVsConceptRow>> conceptProjectionDict = [];
            foreach (DbGraphVs.DbVsRow vsRow in vsGraph.Projection)
            {
                List<DbGraphVs.DbVsConceptRow> conceptProjections = vsRow.Projection;
                foreach (DbGraphVs.DbVsConceptRow vsConceptRow in conceptProjections)
                {
                    if (vsConceptRow.KeyCell == null)
                    {
                        continue;
                    }

                    if (!conceptProjectionDict.TryGetValue(vsConceptRow.KeyCell.Concept.Key, out List<DbGraphVs.DbVsConceptRow>? conceptList))
                    {
                        conceptList = [];
                        conceptProjectionDict.Add(vsConceptRow.KeyCell.Concept.Key, conceptList);
                    }

                    conceptList.Add(vsConceptRow);
                }
            }

            // build the value sets for this package
            buildXverValueSets(
                packages,
                sourcePackageIndex,
                vs,
                conceptProjectionDict,
                xverValueSets);
        }

        return xverValueSets;
    }

    private void buildXverValueSets(
        List<DbFhirPackage> packages,
        int sourcePackageIndex,
        DbValueSet sourceVs,
        Dictionary<int, List<DbGraphVs.DbVsConceptRow>> conceptProjectionDict,
        Dictionary<(int sourceVsKey, int targetPackageId), ValueSet> xverValueSets,
        HashSet<int>? conceptsWithoutEquivalent = null,
        ValueSet? xverVs = null,
        int currentPackageIndex = -1,
        int targetPackageIndex = -1)
    {
        // check for starting conditions
        if ((currentPackageIndex == -1) ||
            (targetPackageIndex == -1))
        {
            // if we are not the last package, build upwards
            if (sourcePackageIndex < (packages.Count - 1))
            {
                buildXverValueSets(
                    packages,
                    sourcePackageIndex,
                    sourceVs,
                    conceptProjectionDict,
                    xverValueSets,
                    conceptsWithoutEquivalent,
                    xverVs,
                    currentPackageIndex: sourcePackageIndex,
                    targetPackageIndex: sourcePackageIndex + 1);
            }

            // if we are not the first package, build downwards
            if (sourcePackageIndex > 0)
            {
                buildXverValueSets(
                    packages,
                    sourcePackageIndex,
                    sourceVs,
                    conceptProjectionDict,
                    xverValueSets,
                    conceptsWithoutEquivalent,
                    xverVs,
                    currentPackageIndex: sourcePackageIndex,
                    targetPackageIndex: sourcePackageIndex - 1);
            }

            // done
            return;
        }

        bool testingRight = currentPackageIndex < targetPackageIndex;
        bool testingLeft = !testingRight;
        conceptsWithoutEquivalent ??= [];

        DbFhirPackage sourcePackage = packages[sourcePackageIndex];
        DbFhirPackage targetPackage = packages[targetPackageIndex];

        //string sourceDashTarget = $"{sourcePackage.ShortName}-{targetPackage.ShortName}";
        string vsId = $"{sourcePackage.ShortName}-{sourceVs.Id}-for-{targetPackage.ShortName}";
        //string vsId = $"{sourceDashTarget}-{sourceVs.Id}";

        ValueSet vs = new()
        {
            Url = $"http://hl7.org/fhir/uv/xver/{sourcePackage.FhirVersionShort}/ValueSet/{vsId}",
            Id = vsId,
            Version = _crossDefinitionVersion,
            Name = vsId,
            Title = $"Cross-version VS for {sourcePackage.ShortName}.{sourceVs.Name} for use in FHIR {targetPackage.ShortName}",
            Status = PublicationStatus.Draft,
            Experimental = false,
            DateElement = new FhirDateTime(DateTimeOffset.Now),
            Description = $"This cross-version ValueSet represents concepts from {sourceVs.VersionedUrl} for use in FHIR {targetPackage.ShortName}." +
                    $" Concepts not present here have direct `equivalent` mappings crossing all versions from {sourcePackage.ShortName} to {targetPackage.ShortName}.",
            Compose = new()
            {
                Include = [],
            },
            Expansion = new()
            {
                Contains = [],
            },
        };

        Dictionary<string, ValueSet.ConceptSetComponent> composeIncludes = [];

        // if we have an existing VS, start with the compose and expansion from that one (note that nonEquivalentConceptKeys will already be populated)
        if (xverVs != null)
        {
            vs.Compose = (ValueSet.ComposeComponent)xverVs.Compose.DeepCopy();
            foreach (ValueSet.ConceptSetComponent composeInclude in vs.Compose.Include)
            {
                composeIncludes.Add(composeInclude.System + "|" + composeInclude.Version, composeInclude);
            }

            vs.Expansion = (ValueSet.ExpansionComponent)xverVs.Expansion.DeepCopy();
        }

        // iterate over the projections
        foreach ((int sourceConceptKey, List<DbGraphVs.DbVsConceptRow> conceptProjections) in conceptProjectionDict)
        {
            // skip if we know this concept has already mapped out
            if (conceptsWithoutEquivalent.Contains(sourceConceptKey))
            {
                continue;
            }

            // check to see if we have any equivalent mappings
            if (testingRight &&
                conceptProjections.Any((DbGraphVs.DbVsConceptRow vsConceptRow) => vsConceptRow[currentPackageIndex]?.RightComparison?.Relationship == CMR.Equivalent))
            {
                continue;
            }

            if (testingLeft &&
                conceptProjections.Any((DbGraphVs.DbVsConceptRow vsConceptRow) => vsConceptRow[currentPackageIndex]?.LeftComparison?.Relationship == CMR.Equivalent))
            {
                continue;
            }

            // add this concept as not directly equivalent
            conceptsWithoutEquivalent.Add(sourceConceptKey);

            // check to see if we have this concept
            DbValueSetConcept concept = conceptProjections[0].KeyCell?.Concept ?? throw new Exception($"Failed to resolve concept for {sourceConceptKey} in {sourceVs.Name}!");

            string composeKey = concept.System + "|" + concept.SystemVersion;

            if (!composeIncludes.TryGetValue(composeKey, out ValueSet.ConceptSetComponent? composeInclude))
            {
                // create a new include for this concept
                composeInclude = new()
                {
                    System = concept.System,
                    Version = concept.SystemVersion,
                    Concept = [],
                };
                composeIncludes.Add(composeKey, composeInclude);
                vs.Compose.Include.Add(composeInclude);
            }

            composeInclude.Concept.Add(new()
            {
                Code = concept.Code,
                Display = concept.Display,
            });

            // add this concept to the expansion
            vs.Expansion.Contains.Add(new()
            {
                System = concept.System,
                Version = concept.SystemVersion,
                Code = concept.Code,
                Display = concept.Display,
            });
        }

        // add this value set to the dictionary if it has any concepts
        if (vs.Expansion.Contains.Count > 0)
        {
            xverValueSets.Add((sourceVs.Key, targetPackage.Key), vs);
        }

        // check for continuing to the next package to the right
        if (testingRight &&
            (targetPackageIndex < packages.Count - 1))
        {
            // build the value set for this package
            buildXverValueSets(
                packages,
                sourcePackageIndex,
                sourceVs,
                conceptProjectionDict,
                xverValueSets,
                conceptsWithoutEquivalent,
                vs,
                currentPackageIndex: targetPackageIndex,
                targetPackageIndex: targetPackageIndex + 1);
        }

        // check for continuing to the next package to the left
        if (testingLeft &&
            (targetPackageIndex > 0))
        {
            // build the value set for this package
            buildXverValueSets(
                packages,
                sourcePackageIndex,
                sourceVs,
                conceptProjectionDict,
                xverValueSets,
                conceptsWithoutEquivalent,
                vs,
                currentPackageIndex: targetPackageIndex,
                targetPackageIndex: targetPackageIndex - 1);
        }

        return;
    }



}
