using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.CommandLine;
using System.Data;
using System.Data.Common;
using System.Formats.Tar;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.CompilerServices;
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
using CMR = Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship;
using Tasks = System.Threading.Tasks;

namespace Microsoft.Health.Fhir.Comparison.XVer;

public partial class XVerProcessor
{
    private const string _thoPackageVersion = "6.5.0";
    private const string _extensionsPackVersion = "5.3.0-ballot-tc1";
    private const string _toolsPackageVersion = "0.8.0";

    private const string _xverChangelogMd = $$$"""

        ### 0.0.1-snapshot-2

        * Fix issue with modifier extensions:
            * Modifier element -> Modifier element : extension
            * Modifier element -> Backbone element (not modifier) : modifier extension
            * Modifier element -> Primitive-type element (not modifier) : modifier extension, context moves up a level
            * Modifier element -> Primitive-type element (array, not modifier) : currently unresolvable, but also has not happened yet
        * Fix: canonical targets that do not exist (use `alternate-canonical` extension)
        * Fix: reference targets that do not exist (use `Basic` as target)
        * Define profiles for `Basic` resources to represent undefined resource types
        * Define profiles for existing resources to represent other versions of the same resource type
        * Define cross-version value sets for all expandable and not-excluded value sets instead of only ones with `required` bindings
        * Fix: `.url` appearing twice in snapshots
        * Fix: some elements being out-of-order in snapshots
        * Fix: `_datatype` being out-of-order in some extensions (see R5:`SubscriptionStatus.eventsSinceSubscriptionStart`)
        * Fix: slicing discriminator being `closed` instead of `open` in some cases
        * Fix: `StructureDefinition.name` must begin with a capital letter (`sdf-0` / `cnl-0`)
        * Fix: `ValueSet.name` must begin with a capital letter (`sdf-0` / `cnl-0`)
        * Fix: cross-version profiles for `Basic` resources had incorrect root extension slice names
        * Fix: cross-version profiles for `Basic` resource were missing `sliceName` on the `extension` element
        * Fix: cross-version profiles for `Basic` should use *pattern* instead of *fixed* for the `code` element (avoid over-constraining the value)
        * Added `changelog.md` file to generated packages (see XVerProcessorDbPackage.cs)
        * Added `lookup.md` file to generated single-version packages as index file for all resource lookups
        * Fix: sushi-config.yaml `name` property was not properly formatted (`ig-0`)
        * Added `ignoreWarnings.txt` file to generated packages to suppress known non-critical warnings during IG publishing
        * Fix: ValueSets that have `equivalent` definitions in target versions were referencing the source version number
        * Added porting of source `CodeSystem` resources from core packages
        * Fix: replacing of 'special' FHIR links formatted as `[[[{structure[#fragment]}]]]` by converting to version-specific markdown links
        * Added generation of unexpandable value sets by copying the source `ValueSet.compose` element to an XVer ValueSet
        * Added `useContext` from source ValueSets to equivalent XVer ValueSets
        * Added `jurisdiction` from source ValueSets to equivalent XVer ValueSets
        * Fix: DSTU2 `usageContext` elements were not loading correctly
        * Fix: DSTU2 `usageContext` element processing was not categorizing jurisdiction values correctly (now moving those values to `jurisdiction` elements)
        * Fix: XVer ValueSets and CodeSystems were being exported with an empty `structuredefinition-wg` extension
        * Fix: Checking `definition` and `comment` elements for HTML Anchor links and converting them to markdown links
        * Added External Inclusions to support `http://terminology.hl7.org/CodeSystem/designation-usage`
        * Added `ValueSet.compose` to generated XVer ValueSets.
        * Fix: Generated value sets will no longer allow for `id` values longer than 64 characters.
        * Added `publisher` and `contact` elements for definitional resources if none are present on canonical resources
        * Fix: hl7.terminology.r4b does not appear to be published on the same regularity as other versions - changed to the r4 package.

        ### 0.0.1-snapshot-1

        * Initial published version.

        """;

    private const string _xverIndexMd = $$$"""

        The Cross-Version Extensions for FHIR (XVer) project provides a set of packages that define extensions, profiles, and value sets for cross-version compatibility in FHIR. These packages are designed to support the validation and interoperability of FHIR resources across different versions.

        ### About This Guide

        Cross-version extension content is generated via a combination of automatic differential detection and manual editing.

        For details or questions, please reach out on [Zulip](https://chat.fhir.org/#narrow/channel/426854-FHIR-cross-version-issues) or
        to the FHIR Infrastructure WorkGroup.
        
        ### Goals

        * Ability to use elements from another version of FHIR
        * Encode information **added** in a *newer* version of FHIR
          * Community identified additional data that is needed
          * Need to provide a consistent way of representing it in an earlier version of FHIR
        * Encode information **removed** in a *newer* version of FHIR
          * Community decided data was no longer necessary
          * Need to provide a consistent way of representing it in a later version of FHIR
            * Version migration requires interoperability between versions and cannot break old expectations
            * Scenarios where data needs to round-trip conversion between different versions

        ### Known Limitations

        * Not 100% coverage of all elements in all versions
            * `Resource` type elements are excluded (e.g., R5:`Bundle.issues`)
        * Mappings are generally between adjacent versions, so "back-and-forth" conversions may be needlessly verbose
        * Large / infinite value sets (e.g., UCUM, LOINC, MIME) are excluded and assumed 'equivalent' across versions

        ### Intellectual Property Statements

        {% include ip-statements.xhtml %}
        """;

    private const string _xverDownloadsMd = $$$"""
        ### Package File

        The following package file includes an NPM package file used by many of the FHIR tools. It contains all the value sets,
        profiles, extensions, list of pages and urls in the IG, etc defined as part of this version of the Implementation Guides.
        This file should be the first choice whenever generating any implementation artifacts since it contains all of the rules
        about what makes the profiles valid. Implementers will still need to be familiar with the content of the specification
        and profiles that apply in order to make a conformant implementation. See the overview on
        [validating FHIR profiles and resources](http://hl7.org/fhir/validation.html):

        * [Package](package.tgz)

        ### Examples

        * [JSON](examples.json.zip)
        * [XML](examples.xml.zip)

        ### Downloadable Copy of this Specification

        A downloadable version of this IG is available so it can be hosted locally:

        * [Downloadable Copy](full-ig.zip)

        ### Package Dependencies

        {% include dependency-table.xhtml %}

        ### Global Profile Definitions

        {% include globals-table.xhtml %}

        ### Cross-Version Analysis

        {% include cross-version-analysis.xhtml %}
        """;

    private const string _xverIgnoreWarningsTxt = $$$"""
        == Suppressed Messages ==

        # ==== 01. Extension names and ids are shortened to avoid exceeding 64 character limit ====
        RESOURCE_ID_MISMATCH
        ERROR: StructureDefinition.url: Resource id/url mismatch: %

        RESOURCE_CANONICAL_MISMATCH
        ERROR: StructureDefinition.where(url = '%'): Conformance resource % - the canonical URL (%) does not match the URL (%)
        ERROR: %: URL Mismatch % vs $
        
        ## TODO: I think this was caused by not setting `ValueSet.compose` in the value sets
        # VALUESET_INCLUDE_INVALID_CONCEPT_CODE_VER

        # ==== 03. These are the 'default' ValueSets from ported CodeSystem resources. We do not want to define them ====
        TYPE_SPECIFIC_CHECKS_DT_CANONICAL_RESOLVE
        A definition could not be found for Canonical URL %
        ERROR: CodeSystem/%: CodeSystem.valueSet: A definition could not be found for Canonical URL %

        # ==== 04. We are not building examples for everything ====
        The Implementation Guide contains no examples for this extension
        WARNING: StructureDefinition.where(url = %): The Implementation Guide contains no examples for this extension
        WARNING: StructureDefinition.where(url = %): The Implementation Guide contains no examples for this profile
 
        # ==== 05. We are faithfully reproducing existing Code Systems and cannot address these ====
        CODESYSTEM_CONCEPT_NO_DEFINITION
        HL7 Defined CodeSystems should ensure that every concept has a definition

        CODESYSTEM_CONCEPT_NO_DISPLAY
        HL7 Defined CodeSystems should ensure that every concept has a display

        CODESYSTEM_CS_COMPLETE_AND_EMPTY
        When a CodeSystem has content = 'complete', it doesnt make sense for there to be no concepts defined

        CODESYSTEM_CS_HL7_MISSING_ELEMENT_SHOULD
        HL7 Defined CodeSystems SHOULD have a stated value for the hierarchyMeaning element so that users know the status and meaning of the code system clearly

        ## TODO: I think this was caused by not setting `ValueSet.compose` in the value sets
        # CODESYSTEM_PROPERTY_BAD_INTERNAL_REFERENCE
        # The code '%' is not a valid code in this code system

        CODESYSTEM_PROPERTY_CODE_DEFAULT_WARNING
        The type of property 'code' is 'code', but no ValueSet information was found, so the codes will be validated as internal codes

        CODESYSTEM_PROPERTY_UNKNOWN_CODE
        This property has only a code ('%') and not a URI, so it has no clearly defined meaning in the terminology ecosystem

        CODESYSTEM_PROPERTY_URI_INVALID
        The uri '%' for the property '%' implies a property with that URI exists in the CodeSystem FHIR Defined Concept Properties for http://hl7.org/fhir/concept-properties, or the code '%' does, but neither were found

        CODESYSTEM_THO_CHECK
        Most code systems defined in HL7 IGs will need to move to THO later during the process. Consider giving this code system a THO URL now (See https://confluence.hl7.org/display/TSMG/Terminology+Play+Book, and/or talk to TSMG)

        ## TODO: I think this is due to not creating the 'all system' ValueSets for source-ported CodeSystems, and I do not want to create them
        CodeSystem_CS_VS_WrongSystem

        MSG_DRAFT
        Reference to draft CodeSystem http://hl7.org/fhir/CodeSystem/knowledge-representation-level|5.0.0

        VALIDATION_VAL_STATUS_INCONSISTENT
        The resource status 'active' and the standards status 'draft' are not consistent
        The resource status 'draft' and the standards status 'normative' are not consistent
        ERROR: % If a resource is not implementable, is marked as experimental or example, the standards status can only be 'informative', 'draft' or 'deprecated', not 'trial-use'.

        VALIDATION_VAL_STATUS_INCONSISTENT_HINT
        The resource status 'draft' and the standards status 'trial-use' may not be consistent and should be reviewed

        # ==== 06. We cannot change bindings from the core specification ====
        The extension http://hl7.org/fhir/StructureDefinition/elementdefinition-maxValueSet|% is deprecated with the note: 'Use additionalBinding extension or element instead'
        
        # ==== 07. We cannot honor inactive flags since we are porting existing values ====
        VALUESET_BAD_FILTER_VALUE_VALID_CODE_INACTIVE
        The code for the filter 'concept' is inactive %

        # ==== 08. We cannot change filters since we are porting existing values ====
        VALUESET_BAD_FILTER_VALUE_VALID_CODE_CHANGE
        The value for a filter based on property 'SCALE_TYP' must be a valid code from the system 'http://loinc.org', and 'Doc' is not (Unknown code 'Doc' in the CodeSystem 'http://loinc.org' version '2.80'). Note that this is change from the past; terminology servers are expected to still continue to support this filter
        The value for a filter based on property 'parent' must be a valid code from the system 'http://loinc.org', and 'LP43571-6' is not (Unknown code 'LP43571-6' in the CodeSystem 'http://loinc.org' version '2.80'). Note that this is change from the past; terminology servers are expected to still continue to support this filter

        # ==== 09. These are bindings inherited from core and cannot be overridden here ====
        MSG_DEPENDS_ON_DEPRECATED_NOTE
        The extension http://hl7.org/fhir/StructureDefinition/elementdefinition-maxValueSet|% is deprecated with the note: 'Use additionalBinding extension or element instead'
        The extension http://hl7.org/fhir/StructureDefinition/codesystem-use-markdown|% is deprecated with the note: 'This extension is deprecated as the Terminology Infrastructure work group felt there wasn't a use case for the extension'
        The extension http://hl7.org/fhir/StructureDefinition/valueset-special-status|% is deprecated with the note: 'This extension is deprecated as Terminology Infrastructure was unable to determine a use for it'

        # ==== 10. We cannot change the experimental flag on any existing content ====
        SD_ED_EXPERIMENTAL_BINDING
        The definition for the element 'Extension.extension.extension.value[x]' binds to the value set '%' which is experimental, but this structure is not labeled as experimental
        INFORMATION: I%: ImplementationGuide.definition.parameter[0].code: Reference to experimental CodeSystem %

        # ==== 11. This URL should not have been used for an example code system, but it was and we cannot change it ====
        A definition for CodeSystem 'http://acme.com/config/fhir/codesystems/internal' could not be found, so the code cannot be validated

        # ==== 12. FHIR-I is publishing this package, but we preserve the WG responsible for content where possible ====
        VALIDATION_HL7_PUBLISHER_MISMATCH
        The nominated WG '%' means that the publisher should be '%' but 'HL7 International / FHIR Infrastructure' was found

        # ==== 13. We cannot add display values to existing code systems that lack them ====
        VALUESET_CONCEPT_DISPLAY_PRESENCE_MIXED
        This include has some concepts with displays and some without - check that this is what is intended

        # ==== 14. We cannot change the semantic structure of any existing content ====
        VALUESET_CONCEPT_DISPLAY_SCT_TAG_MIXED
        This SNOMED-CT based include has some concepts with semantic tags (FSN terms) and some without (preferred terms) - check that this is what is intended %

        """;

    /// <summary>
    /// Creates a compressed .tgz (tar.gz) archive from the specified source directory.
    /// </summary>
    /// <param name="sourceDirectory">The directory to archive and compress.</param>
    /// <param name="outputTgzFile">The path to the output .tgz file.</param>
    /// <remarks>
    /// This method creates a tar archive of the specified directory and compresses it using GZip.
    /// If an error occurs during the process, a message is written to the console.
    /// </remarks>
    private static void createTgzFromDirectory(string sourceDirectory, string outputTgzFile)
    {
        try
        {
            // Compress the tar file into a .tgz file
            using (FileStream tgzFileStream = File.Create(outputTgzFile))
            using (GZipStream gzipStream = new GZipStream(tgzFileStream, CompressionLevel.Optimal))
            {
                TarFile.CreateFromDirectory(sourceDirectory, gzipStream, false);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to create tgz {outputTgzFile} from source {sourceDirectory}: {ex.Message}");
        }
    }

    /// <summary>
    /// Writes the ImplementationGuide, manifest, index, and package.json files for each validation package.
    /// </summary>
    /// <param name="packageSupports">The list of package support objects representing each FHIR package.</param>
    /// <param name="allPackageIndexInfos">The list of all cross-version package index information objects.</param>
    /// <param name="fhirDir">The root directory where FHIR artifacts are written.</param>
    private void writeXverValidationPackageSupportFiles(
        List<PackageXverSupport> packageSupports,
        List<XverPackageIndexInfo> allPackageIndexInfos,
        string fhirDir)
    {
        // iterate over the support packages
        foreach (PackageXverSupport packageSupport in packageSupports)
        {
            string packageId = getPackageId(null, packageSupport.Package);
            string dir = createExportPackageDir(fhirDir, null, packageSupport.Package);

            dir = Path.Combine(dir, "package");
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            List<(string packageId, string packageVersion)> internalDependencies = [];
            foreach (PackageXverSupport sourcePackage in packageSupports)
            {
                if (sourcePackage.Package.Key == packageSupport.Package.Key)
                {
                    continue;
                }

                internalDependencies.Add((
                    getPackageId(sourcePackage.Package, packageSupport.Package),
                    _crossDefinitionVersion));
            }

            // get the list of index informations that *target* this version
            List<XverPackageIndexInfo> packageIndexInfos = allPackageIndexInfos.Where(ii => ii.TargetPackageSupport.Package.Key == packageSupport.Package.Key).ToList();

            // build and write the ImplementationGuide resource for the combination package (single source and target)
            {
                string igJson;

                if (packageSupport.Package.FhirVersionShort.StartsWith('4'))
                {
                    igJson = getIgJsonR4(packageSupport.Package, packageId, internalDependencies, packageIndexInfos);
                }
                else if (packageSupport.Package.FhirVersionShort.StartsWith('5'))
                {
                    igJson = getIgJsonR5(packageSupport.Package, packageId, internalDependencies, packageIndexInfos);
                }
                else
                {
                    // TODO: Implment DSTU2 and STU3
                    continue;
                }

                string filename = $"ImplementationGuide-{packageId}.json";
                File.WriteAllText(Path.Combine(dir, filename), igJson);
            }

            // build and write the package.manifest.json file
            {
                string pmJson = $$$"""
                    {
                      "version" : "{{{_crossDefinitionVersion}}}",
                      "fhirVersion" : ["{{{packageSupport.Package.PackageVersion}}}"],
                      "date" : "{{{DateTime.Now.ToString("yyyyMMddHHmmss")}}}",
                      "name" : "{{{packageId}}}",
                      "jurisdiction" : "http://unstats.un.org/unsd/methods/m49/m49.htm#001"
                    }
                    """;

                string filename = "package.manifest.json";
                File.WriteAllText(Path.Combine(dir, filename), pmJson);
            }

            // build and write the .index.json file
            {
                string indexJson = getIndexJson(packageSupport.Package, packageId, internalDependencies, packageIndexInfos);
                string filename = ".index.json";
                File.WriteAllText(Path.Combine(dir, filename), indexJson);
            }

            // build and write the package.json file
            {
                string additionalDependencies = internalDependencies.Count == 0
                    ? string.Empty
                    : (", " + string.Join(", ", internalDependencies.Select(pi => $"\"{pi.packageId}\" : \"{pi.packageVersion}\"")));

                string packageSuffix = packageSupport.Package.ShortName.ToLowerInvariant();

                // TODO: hl7.fhir.uv.tools does not output an R4B package as of 0.8.0, remove this once it does
                string toolsPackageSuffix = packageSupport.Package.DefinitionFhirSequence == FhirReleases.FhirSequenceCodes.R4B
                    ? "r4"
                    : packageSupport.Package.ShortName.ToLowerInvariant();

                string packageJson = $$$"""
                    {
                        "name" : "{{{packageId}}}",
                        "version" : "{{{_crossDefinitionVersion}}}",
                        "tools-version" : 3,
                        "type" : "IG",
                        "date" : "{{{DateTime.Now.ToString("yyyyMMddHHmmss")}}}",
                        "license" : "CC0-1.0",
                        "canonical" : "http://hl7.org/fhir/uv/xver",
                        "notForPublication" : true,
                        "url" : "http://hl7.org/fhir/uv/xver",
                        "title" : "XVer-{{{packageSupport.Package.ShortName}}}",
                        "description" : "All Cross Version Extensions for FHIR {{{packageSupport.Package.ShortName}}}",
                        "fhirVersions" : ["{{{packageSupport.Package.PackageVersion}}}"],
                        "dependencies" : {
                            "{{{packageSupport.Package.PackageId}}}" : "{{{packageSupport.Package.PackageVersion}}}",
                            "hl7.terminology.{{{packageSuffix}}}" : "{{{_thoPackageVersion}}}",
                            "hl7.fhir.uv.extensions.{{{packageSuffix}}}" : "{{{_extensionsPackVersion}}}",
                            "hl7.fhir.uv.tools.{{{toolsPackageSuffix}}}" : "{{{_toolsPackageVersion}}}"
                            {{{additionalDependencies}}}
                        },
                        "author" : "HL7 International / FHIR Infrastructure",
                        "maintainers" : [
                            {
                                "name" : "HL7 International / FHIR Infrastructure",
                                "url" : "http://www.hl7.org/Special/committees/fiwg"
                            }
                        ],
                        "directories" : {
                            "lib" : "package",
                            "doc" : "doc"
                        },
                        "jurisdiction" : "http://unstats.un.org/unsd/methods/m49/m49.htm#001"
                    }
                    """;

                string filename = "package.json";
                File.WriteAllText(Path.Combine(dir, filename), packageJson);
            }
        }
    }



    /// <summary>
    /// Determines whether a file should be skipped when copying from the GitHub repository.
    /// </summary>
    /// <param name="filePath">The relative file path from the repository root.</param>
    /// <returns>True if the file should be skipped, false otherwise.</returns>
    private static bool shouldSkipScriptRepoFile(string filePath)
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
        if (_config.XverIncludeScripts == false)
        {
            return [];
        }

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
                    if (content.Type == ContentType.File && !shouldSkipScriptRepoFile(content.Path))
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
                else if (content.Type == ContentType.Dir && !shouldSkipScriptRepoFile(content.Path))
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

            string fshDir = Path.Combine(inputDir, "fsh");
            if (!Directory.Exists(fshDir))
            {
                Directory.CreateDirectory(fshDir);
            }

            string pagesDir = Path.Combine(inputDir, "pagecontent");
            if (!Directory.Exists(pagesDir))
            {
                Directory.CreateDirectory(pagesDir);
            }

            string extensionsDir = Path.Combine(inputDir, "extensions");
            if (!Directory.Exists(extensionsDir))
            {
                Directory.CreateDirectory(extensionsDir);
            }

            string profilesDir = Path.Combine(inputDir, "profiles");
            if (!Directory.Exists(profilesDir))
            {
                Directory.CreateDirectory(profilesDir);
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
        string fhirDir,
        Dictionary<string, List<(string structureName, string filename)>> packageMdList)
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
                    : string.Join("\n    ", internalDependencies.Select(pi => $"{pi.packageId} : {pi.packageVersion}"));

                string lookupPages = string.Empty;
                if (packageMdList.TryGetValue(packageId, out List<(string structureName, string lookupFilename)>? packageMdFiles))
                {
                    lookupPages = packageMdFiles.Count == 0
                        ? string.Empty
                        : string.Join("\n", packageMdFiles.Select(p => $"    {p.lookupFilename}:\n        title: Lookup for {p.structureName}"));
                }

                string packageSuffix = targetPackage.ShortName.ToLowerInvariant();

                // TODO: hl7.fhir.uv.tools does not output an R4B package as of 0.8.0, remove this once it does
                string toolsPackageSuffix = targetPackage.DefinitionFhirSequence == FhirReleases.FhirSequenceCodes.R4B
                    ? "r4"
                    : targetPackage.ShortName.ToLowerInvariant();

                string filename = Path.Combine(dir, "sushi-config.yaml");
                string contents = $$$"""
                    # ╭─────────────────────────Commonly Used ImplementationGuide Properties───────────────────────────╮
                    # │  The properties below are used to create the ImplementationGuide resource. The most commonly   │
                    # │  used properties are included. For a list of all supported properties and their functions,     │
                    # │  see: https://fshschool.org/docs/sushi/configuration/.                                         │
                    # ╰────────────────────────────────────────────────────────────────────────────────────────────────╯
                    id: {{{packageId}}}
                    canonical: http://hl7.org/fhir/uv/xver
                    name: {{{FhirSanitizationUtils.ReformatIdForName(packageId)}}}
                    title: Cross-Version Extensions validation package for FHIR {{{targetPackage.ShortName}}}
                    description: All cross-version extensions available in FHIR {{{targetPackage.ShortName}}}
                    status: active # draft | active | retired | unknown
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
                        # {{{targetPackage.PackageId}}} : {{{targetPackage.PackageVersion}}}
                        hl7.terminology.{{{packageSuffix}}} : {{{_thoPackageVersion}}}
                        hl7.fhir.uv.extensions.{{{packageSuffix}}} : {{{_extensionsPackVersion}}}
                        hl7.fhir.uv.tools.{{{toolsPackageSuffix}}} : {{{_toolsPackageVersion}}}
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
                    pages:
                        index.md:
                            title: Home
                        downloads.md:
                            title: Downloads
                        changelog.md:
                            title: Change Log
                    {{{lookupPages}}}
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
                        apply-wg: false
                        default-wg: true
                        apply-version: false
                        default-version: true
                        show-inherited-invariants: false
                        usage-stats-opt-out: true
                        shownav: 'true'
                        pin-canonicals: pin-all
                        # These are standard directories, they do not need to be specified
                        # path-resource:
                        #     - input/extensions/*
                        #     - input/profiles/*
                        #     - input/resources/*
                        #     - input/vocabulary/*

                    extension:
                        - url: http://hl7.org/fhir/StructureDefinition/structuredefinition-standards-status
                          valueCode: trial-use
                        - url: http://hl7.org/fhir/StructureDefinition/structuredefinition-wg
                          valueCode: fhir
                        - url: http://hl7.org/fhir/StructureDefinition/structuredefinition-fmm
                          valueInteger: 0
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
                        Contents: toc.html
                        Home: index.html
                        Support:
                            Downloads: downloads.html
                            Change Log: changelog.html
                    
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

            {
                string filename = Path.Combine(dir, "input", "pagecontent", "index.md");
                File.WriteAllText(filename, _xverIndexMd);
            }

            {
                string filename = Path.Combine(dir, "input", "pagecontent", "changelog.md");
                File.WriteAllText(filename, _xverChangelogMd);
            }

            {
                string filename = Path.Combine(dir, "input", "pagecontent", "downloads.md");
                File.WriteAllText(filename, _xverDownloadsMd);
            }

            {
                string filename = Path.Combine(dir, "input", "ignoreWarnings.txt");
                File.WriteAllText(filename, _xverIgnoreWarningsTxt);
            }
        }
    }

    private void writePublisherSinglePackageConfig(
        List<PackageXverSupport> packageSupports,
        int focusPackageIndex,
        string fhirDir,
        Dictionary<string, List<(string structureName, string filename)>> packageMdList)
    {
        DbFhirPackage sourcePackage = packageSupports[focusPackageIndex].Package;

        // Fetch files from GitHub repository
        Dictionary<string, string> githubFiles = getCurrentPublisherScripts();

        foreach (PackageXverSupport targetSupport in packageSupports)
        {
            // skip self - no comparison
            if (targetSupport.Package.Key == sourcePackage.Key)
            {
                continue;
            }

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
                List<(string filename, string title)> pages = [
                    ("index.md", "Home"),
                    ("lookup.md", "Artifact Lookup"),
                    ("downloads.md", "Downloads"),
                    ("changelog.md", "Change Log"),
                    ];

                if (packageMdList.TryGetValue(packageId, out List<(string structureName, string lookupFilename)>? packageMdFiles))
                {
                    pages.AddRange(packageMdFiles.Select(p => ($"{p.lookupFilename}.md", $"Lookup for {p.structureName}")));
                }

                string pagesYaml = string.Join("\n", pages.Select(p => $"    {p.filename}:\n        title: {p.title}"));

                string packageSuffix = targetPackage.ShortName.ToLowerInvariant();

                // TODO: hl7.fhir.uv.tools does not output an R4B package as of 0.8.0, remove this once it does
                string toolsPackageSuffix = targetPackage.DefinitionFhirSequence == FhirReleases.FhirSequenceCodes.R4B
                    ? "r4"
                    : targetPackage.ShortName.ToLowerInvariant();

                string filename = Path.Combine(dir, "sushi-config.yaml");
                string contents = $$$"""
                    # ╭─────────────────────────Commonly Used ImplementationGuide Properties───────────────────────────╮
                    # │  The properties below are used to create the ImplementationGuide resource. The most commonly   │
                    # │  used properties are included. For a list of all supported properties and their functions,     │
                    # │  see: https://fshschool.org/docs/sushi/configuration/.                                         │
                    # ╰────────────────────────────────────────────────────────────────────────────────────────────────╯
                    id: {{{packageId}}}
                    canonical: http://hl7.org/fhir/{{{sourcePackage.FhirVersionShort}}}
                    name: {{{FhirSanitizationUtils.ReformatIdForName(packageId)}}}
                    title: FHIR Cross-Version Extensions package for FHIR {{{targetPackage.ShortName}}} from FHIR {{{sourcePackage.ShortName}}}
                    description: The cross-version extensions available in FHIR {{{targetPackage.ShortName}}} from FHIR {{{sourcePackage.ShortName}}}
                    status: active # draft | active | retired | unknown
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
                        # {{{targetPackage.PackageId}}} : {{{targetPackage.PackageVersion}}}
                        hl7.terminology.{{{packageSuffix}}} : {{{_thoPackageVersion}}}
                        hl7.fhir.uv.extensions.{{{packageSuffix}}} : {{{_extensionsPackVersion}}}
                        hl7.fhir.uv.tools.{{{toolsPackageSuffix}}} : {{{_toolsPackageVersion}}}

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
                    pages:
                    {{{pagesYaml}}}
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
                        apply-wg: false
                        default-wg: true
                        apply-version: false
                        default-version: true
                        show-inherited-invariants: false
                        usage-stats-opt-out: true
                        shownav: 'true'
                        # These are standard directories, they do not need to be specified
                        # path-resource:
                        #     - input/extensions/*
                        #     - input/profiles/*
                        #     - input/resources/*
                        #     - input/vocabulary/*
                                        
                    extension:
                        - url: http://hl7.org/fhir/StructureDefinition/structuredefinition-standards-status
                          valueCode: trial-use
                        - url: http://hl7.org/fhir/StructureDefinition/structuredefinition-wg
                          valueCode: fhir
                        - url: http://hl7.org/fhir/StructureDefinition/structuredefinition-fmm
                          valueInteger: 0
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
                        Contents: toc.html
                        Home: index.html
                        Lookup: lookup.html
                        Artifacts: artifacts.html
                        Support:
                            Downloads: downloads.html
                            Change Log: changelog.html
                    
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

            {
                string filename = Path.Combine(dir, "input", "pagecontent", "index.md");
                File.WriteAllText(filename, _xverIndexMd);
            }

            {
                string filename = Path.Combine(dir, "input", "pagecontent", "changelog.md");
                File.WriteAllText(filename, _xverChangelogMd);
            }

            {
                string filename = Path.Combine(dir, "input", "pagecontent", "downloads.md");
                File.WriteAllText(filename, _xverDownloadsMd);
            }

            {
                string filename = Path.Combine(dir, "input", "ignoreWarnings.txt");
                File.WriteAllText(filename, _xverIgnoreWarningsTxt);
            }
        }
    }

    /// <summary>
    /// Writes ImplementationGuide, manifest, index, and package.json files for each single source-target package combination.
    /// </summary>
    /// <param name="packageSupports">The list of package support objects representing each FHIR package.</param>
    /// <param name="focusPackageIndex">The index of the source package in the packageSupports list.</param>
    /// <param name="xverValueSets">The dictionary of cross-version ValueSets, keyed by (source ValueSet key, target package id).</param>
    /// <param name="xverExtensions">The dictionary of cross-version StructureDefinitions (extensions), keyed by (source element key, target package id).</param>
    /// <param name="fhirDir">The root directory where FHIR artifacts are written.</param>
    /// <returns>A list of <see cref="XverPackageIndexInfo"/> objects containing index information for each source-target package combination.</returns>
    private List<XverPackageIndexInfo> writeXverSinglePackageSupportFiles(
        List<PackageXverSupport> packageSupports,
        int focusPackageIndex,
        Dictionary<(int sourceVsKey, int targetPackageId), ValueSet> xverValueSets,
        Dictionary<(int sourceElementKey, int targetPackageId), (StructureDefinition, DbExtensionSubstitution?)> xverExtensions,
        string fhirDir)
    {
        List<XverPackageIndexInfo> infos = [];

        DbFhirPackage sourcePackage = packageSupports[focusPackageIndex].Package;

        foreach (PackageXverSupport targetSupport in packageSupports)
        {
            if (targetSupport.Package.Key == sourcePackage.Key)
            {
                continue;
            }

            string packageId = getPackageId(sourcePackage, targetSupport.Package);

            XverPackageIndexInfo indexInfo = new()
            {
                SourcePackageSupport = packageSupports[focusPackageIndex],
                TargetPackageSupport = targetSupport,
                PackageId = packageId,
            };

            infos.Add(indexInfo);

            // build and write the ImplementationGuide resource for the combination package (single source and target)
            {
                string igJson;

                if (targetSupport.Package.FhirVersionShort.StartsWith('4'))
                {
                    igJson = getIgJsonR4(sourcePackage, targetSupport.Package, xverValueSets, xverExtensions, indexInfo);
                }
                else if (targetSupport.Package.FhirVersionShort.StartsWith('5'))
                {
                    igJson = getIgJsonR5(sourcePackage, targetSupport.Package, xverValueSets, xverExtensions, indexInfo);
                }
                else
                {
                    // TODO: Implment DSTU2 and STU3
                    continue;
                }

                string filename = $"ImplementationGuide-{packageId}.json";
                File.WriteAllText(Path.Combine(fhirDir, packageId, "package", filename), igJson);
            }

            // build and write the package.manifest.json file
            {
                string pmJson = $$$"""
                    {
                      "version" : "{{{_crossDefinitionVersion}}}",
                      "fhirVersion" : ["{{{targetSupport.Package.PackageVersion}}}"],
                      "date" : "{{{DateTime.Now.ToString("yyyyMMddHHmmss")}}}",
                      "name" : "{{{packageId}}}",
                      "jurisdiction" : "http://unstats.un.org/unsd/methods/m49/m49.htm#001"
                    }
                    """;

                string filename = "package.manifest.json";
                File.WriteAllText(Path.Combine(fhirDir, packageId, "package", filename), pmJson);
            }

            // build and write the .index.json file
            {
                string indexJson = getIndexJson(sourcePackage, targetSupport.Package, xverValueSets, xverExtensions, indexInfo);
                string filename = ".index.json";
                File.WriteAllText(Path.Combine(fhirDir, packageId, "package", filename), indexJson);
            }

            // build and write the package.json file
            {
                string packageSuffix = targetSupport.Package.ShortName.ToLowerInvariant();

                // TODO: hl7.fhir.uv.tools does not output an R4B package as of 0.8.0, remove this once it does
                string toolsPackageSuffix = targetSupport.Package.DefinitionFhirSequence == FhirReleases.FhirSequenceCodes.R4B
                    ? "r4"
                    : targetSupport.Package.ShortName.ToLowerInvariant();

                string packageJson = $$$"""
                    {
                        "name" : "{{{packageId}}}",
                        "version" : "{{{_crossDefinitionVersion}}}",
                        "tools-version" : 3,
                        "type" : "IG",
                        "date" : "{{{DateTime.Now.ToString("yyyyMMddHHmmss")}}}",
                        "license" : "CC0-1.0",
                        "canonical" : "http://hl7.org/fhir/uv/xver",
                        "notForPublication" : true,
                        "url" : "http://hl7.org/fhir/uv/xver",
                        "title" : "XVer-{{{sourcePackage.ShortName}}}-{{{targetSupport.Package.ShortName}}}",
                        "description" : "Cross Version Extensions for using FHIR {{{sourcePackage.ShortName}}} in FHIR {{{targetSupport.Package.ShortName}}}",
                        "fhirVersions" : ["{{{targetSupport.Package.PackageVersion}}}"],
                        "dependencies" : {
                            "{{{targetSupport.Package.PackageId}}}" : "{{{targetSupport.Package.PackageVersion}}}",
                            "hl7.terminology.{{{packageSuffix}}}" : "{{{_thoPackageVersion}}}",
                            "hl7.fhir.uv.extensions.{{{packageSuffix}}}" : "{{{_extensionsPackVersion}}}",
                            "hl7.fhir.uv.tools.{{{toolsPackageSuffix}}}" : "{{{_toolsPackageVersion}}}"
                        },
                        "author" : "HL7 International / FHIR Infrastructure",
                        "maintainers" : [
                            {
                                "name" : "HL7 International / FHIR Infrastructure",
                                "url" : "http://www.hl7.org/Special/committees/fiwg"
                            }
                        ],
                        "directories" : {
                            "lib" : "package",
                            "doc" : "doc"
                        },
                        "jurisdiction" : "http://unstats.un.org/unsd/methods/m49/m49.htm#001"
                    }
                    """;

                string filename = "package.json";
                File.WriteAllText(Path.Combine(fhirDir, packageId, "package", filename), packageJson);
            }
        }

        return infos;
    }


    /// <summary>
    /// Generates the .index.json content for a cross-version package, listing all FHIR package contents
    /// defined for a specific source-target package combination.
    /// </summary>
    /// <param name="sourcePackage">The source <see cref="DbFhirPackage"/> for the cross-version package.</param>
    /// <param name="targetPackage">The target <see cref="DbFhirPackage"/> for the cross-version package.</param>
    /// <param name="xverValueSets">A dictionary of cross-version <see cref="ValueSet"/>s, keyed by (source ValueSet key, target package id).</param>
    /// <param name="xverExtensions">A dictionary of cross-version <see cref="StructureDefinition"/>s (extensions), keyed by (source element key, target package id).</param>
    /// <param name="indexInfo">The <see cref="XverPackageIndexInfo"/> object to populate with index entries.</param>
    /// <returns>A JSON string representing the .index.json file for the cross-version package.</returns>
    private string getIndexJson(
        DbFhirPackage sourcePackage,
        DbFhirPackage targetPackage,
        Dictionary<(int sourceVsKey, int targetPackageId), ValueSet> xverValueSets,
        Dictionary<(int sourceElementKey, int targetPackageId), (StructureDefinition, DbExtensionSubstitution?)> xverExtensions,
        XverPackageIndexInfo indexInfo)
    {
        // build the list of structures we are defining
        foreach (((int sourceElementKey, int targetPackageId), (StructureDefinition sd, DbExtensionSubstitution? extensionSubstitution)) in xverExtensions)
        {
            if (targetPackageId != targetPackage.Key)
            {
                continue;
            }

            if (extensionSubstitution != null)
            {
                continue;
            }

            indexInfo.IndexStructureJsons.Add($$$"""
                {
                    "filename" : "StructureDefinition-{{{sd.Id}}}.json",
                    "resourceType" : "StructureDefinition",
                    "id" : "{{{sd.Id}}}",
                    "url" : "{{{sd.Url}}}",
                    "version" : "{{{_crossDefinitionVersion}}}",
                    "kind" : "complex-type",
                    "type" : "Extension",
                    "derivation" : "constraint"
                }
                """);
        }

        // build the list of value sets we are defining
        foreach (((int sourceElementKey, int targetPackageId), ValueSet vs) in xverValueSets)
        {
            if (targetPackageId != targetPackage.Key)
            {
                continue;
            }

            indexInfo.IndexValueSetJsons.Add($$$"""
                {
                    "filename" : "ValueSet-{{{vs.Id}}}.json",
                    "resourceType" : "ValueSet",
                    "id" : "{{{vs.Id}}}",
                    "url" : "{{{vs.Url}}}",
                    "version" : "{{{_crossDefinitionVersion}}}"
                }
                """);
        }

        string indexJson = $$$"""
            {
                "index-version" : 2,
                "files" : [
                    {
                        "filename" : "ImplementationGuide-{{{indexInfo.PackageId}}}.json",
                        "resourceType" : "ImplementationGuide",
                        "id" : "{{{indexInfo.PackageId}}}",
                        "url" : "http://hl7.org/fhir/uv/xver/ImplementationGuide/{{{indexInfo.PackageId}}}",
                        "version" : "{{{_crossDefinitionVersion}}}"
                    },
                    {{{string.Join(", ", indexInfo.IndexStructureJsons)}}},
                    {{{string.Join(", ", indexInfo.IndexValueSetJsons)}}}
                ]
            }
            """;

        return indexJson;
    }



    /// <summary>
    /// Generates the .index.json content for a cross-version package, listing all FHIR package contents
    /// defined for a specific source-target package combination.
    /// </summary>
    /// <param name="package">The <see cref="DbFhirPackage"/> representing the package for which the index is generated.</param>
    /// <param name="packageId">The unique package identifier for this cross-version package.</param>
    /// <param name="internalDependencies">A list of internal package dependencies, each as a tuple of package ID and version.</param>
    /// <param name="targetInfos">A list of <see cref="XverPackageIndexInfo"/> objects containing index information for each target package.</param>
    /// <returns>A JSON string representing the .index.json file for the cross-version package.</returns>

    private string getIndexJson(
        DbFhirPackage package,
        string packageId,
        List<(string packageId, string packageVersion)> internalDependencies,
        List<XverPackageIndexInfo> targetInfos)
    {
        if (internalDependencies.Count == 0)
        {
            return $$$"""
            {
                "index-version" : 2,
                "files" : [
                    {
                        "filename" : "ImplementationGuide-{{{packageId}}}.json",
                        "resourceType" : "ImplementationGuide",
                        "id" : "{{{packageId}}}",
                        "url" : "http://hl7.org/fhir/uv/xver/ImplementationGuide/{{{packageId}}}",
                        "version" : "{{{_crossDefinitionVersion}}}"
                    }
                ]
            }
            """;
        }

        return $$$"""
            {
                "index-version" : 2,
                "files" : [
                    {
                        "filename" : "ImplementationGuide-{{{packageId}}}.json",
                        "resourceType" : "ImplementationGuide",
                        "id" : "{{{packageId}}}",
                        "url" : "http://hl7.org/fhir/uv/xver/ImplementationGuide/{{{packageId}}}",
                        "version" : "{{{_crossDefinitionVersion}}}"
                    },
                    {{{string.Join(", ", targetInfos.SelectMany(ii => ii.IndexStructureJsons))}}},
                    {{{string.Join(", ", targetInfos.SelectMany(ii => ii.IndexValueSetJsons))}}}
                ]
            }
            """;
    }

    private string getIgJsonR5(
        DbFhirPackage sourcePackage,
        DbFhirPackage targetPackage,
        Dictionary<(int sourceVsKey, int targetPackageId), ValueSet> xverValueSets,
        Dictionary<(int sourceElementKey, int targetPackageId), (StructureDefinition, DbExtensionSubstitution?)> xverExtensions,
        XverPackageIndexInfo indexInfo)
    {
        ImplementationGuide ig = new()
        {
            Id = "ImplementationGuide-" + indexInfo.PackageId,
            Extension = [
                new()
                {
                    Url = "http://hl7.org/fhir/StructureDefinition/structuredefinition-standards-status",
                    Value = new Code("trial-use"),
                },
                new()
                {
                    Url = "http://hl7.org/fhir/StructureDefinition/structuredefinition-wg",
                    Value = new Code("fhir"),
                }
            ],
            Url = $"http://hl7.org/fhir/uv/xver/ImplementationGuide/{indexInfo.PackageId}",
            Version = _crossDefinitionVersion,
            Name = $"XVer_{sourcePackage.ShortName.ToLowerInvariant()}_{targetPackage.ShortName.ToLowerInvariant()}",
            Title = $"XVer-{sourcePackage.ShortName}-{targetPackage.ShortName}",
            Status = PublicationStatus.Active,
            Date = "2025-05-19T00:00:00+00:00",
            Publisher = "HL7 International / FHIR Infrastructure",
            Contact = [
                new()
                {
                    Name = "HL7 International / FHIR Infrastructure",
                    Telecom = [
                        new()
                        {
                            System = ContactPoint.ContactPointSystem.Url,
                            Value = "http://www.hl7.org/Special/committees/fiwg",
                        },
                    ],
                }
            ],
            Description = $"Cross Version Extensions for using FHIR {sourcePackage.ShortName} in FHIR {targetPackage.ShortName}",
            Jurisdiction = [
                new()
                {
                    Coding = [
                        new()
                        {
                            System = "http://unstats.un.org/unsd/methods/m49/m49.htm",
                            Code = "001",
                            Display = "World",
                        }
                    ],
                }
            ],
            PackageId = indexInfo.PackageId,
            License = ImplementationGuide.SPDXLicense.CC01_0,
            FhirVersion = [FHIRVersion.N5_0_0],
            DependsOn = [
                new()
                {
                    ElementId = "hl7tx",
                    Uri = "http://terminology.hl7.org/ImplementationGuide/hl7.terminology",
                    PackageId = "hl7.terminology.r5",
                    Version = _thoPackageVersion,
                    Extension = [
                        new()
                        {
                            Url = "http://hl7.org/fhir/tools/StructureDefinition/implementationguide-dependency-comment",
                            Value = new Markdown("Automatically added as a dependency - all IGs depend on HL7 Terminology"),
                        },
                    ],
                },
                new()
                {
                    ElementId = "hl7_fhir_uv_extensions",
                    Uri = "http://hl7.org/fhir/extensions/ImplementationGuide/hl7.fhir.uv.extensions",
                    PackageId = "hl7.fhir.uv.extensions.r5",
                    Version = _extensionsPackVersion,
                },
                new()
                {
                    ElementId = "hl7_fhir_uv_tools",
                    Uri = "http://hl7.org/fhir/tools/ImplementationGuide/hl7.fhir.uv.tools",
                    PackageId = "hl7.fhir.uv.tools.r5",
                    Version = _toolsPackageVersion,
                },
            ],
            Definition = new()
            {
                Resource = [],
            }
        };

        // add our structures
        foreach (((int sourceElementKey, int targetPackageId), (StructureDefinition sd, DbExtensionSubstitution? extensionSubstitution)) in xverExtensions)
        {
            if (targetPackageId != targetPackage.Key)
            {
                continue;
            }

            if (extensionSubstitution != null)
            {
                continue;
            }

            indexInfo.IgStructures.Add(new()
            {
                Reference = new ResourceReference($"StructureDefinition/{sd.Id}"),
                Name = sd.Name,
                Description = sd.Description,
                Extension = [
                    new()
                    {
                        Url = "http://hl7.org/fhir/tools/StructureDefinition/resource-information",
                        Value = new FhirString("StructureDefinition:extension"),
                    },
                ],
            });
        }

        ig.Definition.Resource.AddRange(indexInfo.IgStructures);

        // add our value sets
        foreach (((int sourceElementKey, int targetPackageId), ValueSet vs) in xverValueSets)
        {
            if (targetPackageId != targetPackage.Key)
            {
                continue;
            }

            indexInfo.IgValueSets.Add(new()
            {
                Reference = new ResourceReference($"StructureDefinition/{vs.Id}"),
                Name = vs.Name,
                Description = vs.Description,
                Extension = [
                    new()
                    {
                        Url = "http://hl7.org/fhir/tools/StructureDefinition/resource-information",
                        Value = new FhirString("ValueSet"),
                    },
                ],
            });
        }

        ig.Definition.Resource.AddRange(indexInfo.IgValueSets);

        return ig.ToJson(new FhirJsonSerializationSettings() { Pretty = true });
    }


    private string getIgJsonR5(
        DbFhirPackage package,
        string packageId,
        List<(string packageId, string packageVersion)> internalDependencies,
        List<XverPackageIndexInfo> targetInfos)
    {
        ImplementationGuide ig = new()
        {
            Id = "ImplementationGuide-" + packageId,
            Extension = [
                new()
                {
                    Url = "http://hl7.org/fhir/StructureDefinition/structuredefinition-standards-status",
                    Value = new Code("trial-use"),
                },
                new()
                {
                    Url = "http://hl7.org/fhir/StructureDefinition/structuredefinition-wg",
                    Value = new Code("fhir"),
                }
            ],
            Url = $"http://hl7.org/fhir/uv/xver/ImplementationGuide/{packageId}",
            Version = _crossDefinitionVersion,
            Name = $"XVer_{package.ShortName.ToLowerInvariant()}",
            Title = $"XVer-{package.ShortName}",
            Status = PublicationStatus.Active,
            Date = "2025-05-19T00:00:00+00:00",
            Publisher = "HL7 International / FHIR Infrastructure",
            Contact = [
                new()
                {
                    Name = "HL7 International / FHIR Infrastructure",
                    Telecom = [
                        new()
                        {
                            System = ContactPoint.ContactPointSystem.Url,
                            Value = "http://www.hl7.org/Special/committees/fiwg",
                        },
                    ],
                }
            ],
            Description = $"All Cross Version Extensions for FHIR {package.ShortName}",
            Jurisdiction = [
                new()
                {
                    Coding = [
                        new()
                        {
                            System = "http://unstats.un.org/unsd/methods/m49/m49.htm",
                            Code = "001",
                            Display = "World",
                        }
                    ],
                }
            ],
            PackageId = packageId,
            License = ImplementationGuide.SPDXLicense.CC01_0,
            FhirVersion = [FHIRVersion.N5_0_0],
            DependsOn = [
                new()
                {
                    ElementId = "hl7tx",
                    Uri = "http://terminology.hl7.org/ImplementationGuide/hl7.terminology",
                    PackageId = "hl7.terminology.r5",
                    Version = _thoPackageVersion,
                    Extension = [
                        new()
                        {
                            Url = "http://hl7.org/fhir/tools/StructureDefinition/implementationguide-dependency-comment",
                            Value = new Markdown("Automatically added as a dependency - all IGs depend on HL7 Terminology"),
                        },
                    ],
                },
                new()
                {
                    ElementId = "hl7_fhir_uv_extensions",
                    Uri = "http://hl7.org/fhir/extensions/ImplementationGuide/hl7.fhir.uv.extensions",
                    PackageId = "hl7.fhir.uv.extensions.r5",
                    Version = _extensionsPackVersion,
                },
                new()
                {
                    ElementId = "hl7_fhir_uv_tools",
                    Uri = "http://hl7.org/fhir/tools/ImplementationGuide/hl7.fhir.uv.tools",
                    PackageId = "hl7.fhir.uv.tools.r5",
                    Version = _toolsPackageVersion,
                },
            ],
            Definition = new()
            {
                Resource = [],
            }
        };

        if (internalDependencies.Count == 0)
        {
            ig.Definition = new() { Resource = [] };
            ig.Definition.Resource.AddRange(targetInfos.SelectMany(ii => ii.IgStructures));
            ig.Definition.Resource.AddRange(targetInfos.SelectMany(ii => ii.IgValueSets));
        }
        else
        {
            foreach ((string depPackageId, string depPackageVersion) in internalDependencies)
            {
                ig.DependsOn.Add(new()
                {
                    ElementId = depPackageId.Replace('.', '_'),
                    Uri = $"http://hl7.org/fhir/uv/xver/ImplementationGuide/{depPackageId}",
                    PackageId = depPackageId,
                    Version = depPackageVersion
                });
            }
        }

        return ig.ToJson(new FhirJsonSerializationSettings() { Pretty = true });
    }



    private string getIgJsonR4(
        DbFhirPackage sourcePackage,
        DbFhirPackage targetPackage,
        Dictionary<(int sourceVsKey, int targetPackageId), ValueSet> xverValueSets,
        Dictionary<(int sourceElementKey, int targetPackageId), (StructureDefinition, DbExtensionSubstitution?)> xverExtensions,
        XverPackageIndexInfo indexInfo)
    {
        // build the list of structures we are defining
        foreach (((int sourceElementKey, int targetPackageId), (StructureDefinition sd, DbExtensionSubstitution? extensionSubstitution)) in xverExtensions)
        {
            if (targetPackageId != targetPackage.Key)
            {
                continue;
            }

            if (extensionSubstitution != null)
            {
                continue;
            }

            indexInfo.IgStructureJsons.Add($$$"""
                {
                    "extension" : [{
                        "url" : "http://hl7.org/fhir/tools/StructureDefinition/resource-information",
                        "valueString" : "StructureDefinition:extension"
                    }],
                    "reference" : {
                        "reference" : "StructureDefinition/{{{sd.Id}}}"
                    },
                    "name" : "{{{sd.Name}}}",
                    "description" : "{{{sd.Description}}}"
                }
                """);
        }

        // build the list of value sets we are defining
        foreach (((int sourceElementKey, int targetPackageId), ValueSet vs) in xverValueSets)
        {
            if (targetPackageId != targetPackage.Key)
            {
                continue;
            }

            indexInfo.IgValueSetJsons.Add($$$"""
                {
                    "extension" : [{
                        "url" : "http://hl7.org/fhir/tools/StructureDefinition/resource-information",
                        "valueString" : "ValueSet"
                    }],
                    "reference" : {
                        "reference" : "ValueSet/{{{vs.Id}}}"
                    },
                    "name" : "{{{vs.Name}}}",
                    "description" : "{{{vs.Description}}}"
                }
                """);
        }

        string packageSuffix = targetPackage.ShortName.ToLowerInvariant();

        // hl7.fhir.uv.tools and hl7.terminology do not publish R4B specific packages since they are definitionally R4 packages
        string hl7PackageSuffix = targetPackage.DefinitionFhirSequence == FhirReleases.FhirSequenceCodes.R4B
            ? "r4"
            : targetPackage.ShortName.ToLowerInvariant();

        string igJson = $$$"""
            {
              "resourceType" : "ImplementationGuide",
              "id" : "ImplementationGiude-{{{indexInfo.PackageId}}}",
              "extension" : [{
                "url" : "http://hl7.org/fhir/StructureDefinition/structuredefinition-standards-status",
                "valueCode" : "trial-use"
              },
              {
                "url" : "http://hl7.org/fhir/StructureDefinition/structuredefinition-wg",
                "valueCode" : "fhir"
              }],
              "url" : "http://hl7.org/fhir/uv/xver/ImplementationGuide/{{{indexInfo.PackageId}}}",
              "version" : "{{{_crossDefinitionVersion}}}",
              "name" : "XVer_{{{sourcePackage.ShortName.ToLowerInvariant()}}}_{{{targetPackage.ShortName.ToLowerInvariant()}}}",
              "title" : "XVer-{{{sourcePackage.ShortName}}}-{{{targetPackage.ShortName}}}",
              "status" : "active",
              "date" : "2025-05-19T00:00:00+00:00",
              "publisher" : "HL7 International / FHIR Infrastructure",
              "contact" : [{
                "name" : "HL7 International / FHIR Infrastructure",
                "telecom" : [{
                  "system" : "url",
                  "value" : "http://www.hl7.org/Special/committees/fiwg"
                }]
              }],
              "description" : "Cross Version Extensions for using FHIR {{{sourcePackage.ShortName}}} in FHIR {{{targetPackage.ShortName}}}",
              "jurisdiction" : [{
                "coding" : [{
                  "system" : "http://unstats.un.org/unsd/methods/m49/m49.htm",
                  "code" : "001",
                  "display" : "World"
                }]
              }],
              "packageId" : "{{{indexInfo.PackageId}}}",
              "license" : "CC0-1.0",
              "fhirVersion" : ["{{{targetPackage.PackageVersion}}}"],
              "dependsOn" : [{
                "id" : "hl7tx",
                "extension" : [{
                  "url" : "http://hl7.org/fhir/tools/StructureDefinition/implementationguide-dependency-comment",
                  "valueMarkdown" : "Automatically added as a dependency - all IGs depend on HL7 Terminology"
                }],
                "uri" : "http://terminology.hl7.org/ImplementationGuide/hl7.terminology",
                "packageId" : "hl7.terminology.{{{hl7PackageSuffix}}}",
                "version" : "{{{_thoPackageVersion}}}"
              },
              {
                "id" : "hl7_fhir_uv_extensions",
                "uri" : "http://hl7.org/fhir/extensions/ImplementationGuide/hl7.fhir.uv.extensions",
                "packageId" : "hl7.fhir.uv.extensions.{{{packageSuffix}}}",
                "version" : "{{{_extensionsPackVersion}}}"
              },
              {
                "id" : "hl7_fhir_uv_tools",
                "uri" : "http://hl7.org/fhir/tools/ImplementationGuide/hl7.fhir.uv.tools",
                "packageId" : "hl7.fhir.uv.tools.{{{hl7PackageSuffix}}}",
                "version" : "{{{_toolsPackageVersion}}}"
              }],
              "definition" : {
                "resource" : [
                {{{string.Join(", ", indexInfo.IgStructureJsons)}}},
                {{{string.Join(", ", indexInfo.IgValueSetJsons)}}}]
              }
            }
            """;

        return igJson;
    }

    private string getIgJsonR4(
        DbFhirPackage package,
        string packageId,
        List<(string packageId, string packageVersion)> internalDependencies,
        List<XverPackageIndexInfo> targetInfos)
    {
        string additionalDependencies;
        string resources;

        if (internalDependencies.Count == 0)
        {
            additionalDependencies = string.Empty;
            resources = $$$"""
                ,
                  "definition" : {
                    "resource" : [
                    {{{string.Join(", ", targetInfos.SelectMany(ii => ii.IgStructureJsons))}}},
                    {{{string.Join(", ", targetInfos.SelectMany(ii => ii.IgValueSetJsons))}}}]
                  }
                """;
        }
        else
        {
            additionalDependencies = "," + string.Join(
                ",",
                internalDependencies.Select(pi => $$$"""
                    {
                        "id" : "{{{pi.packageId.Replace('.', '_')}}}",
                        "uri" : "http://hl7.org/fhir/uv/xver/ImplementationGuide/{{{packageId}}}",
                        "packageId" : "{{{pi.packageId}}}",
                        "version" : "{{{pi.packageVersion}}}"
                    }
                """)
                );
            resources = string.Empty;
        }

        string packageSuffix = package.ShortName.ToLowerInvariant();

        // TODO: hl7.fhir.uv.tools does not output an R4B package as of 0.8.0, remove this once it does
        string toolsPackageSuffix = package.DefinitionFhirSequence == FhirReleases.FhirSequenceCodes.R4B
            ? "r4"
            : package.ShortName.ToLowerInvariant();

        string igJson = $$$"""
            {
              "resourceType" : "ImplementationGuide",
              "id" : "{{{packageId}}}",
              "extension" : [{
                "url" : "http://hl7.org/fhir/StructureDefinition/structuredefinition-standards-status",
                "valueCode" : "trial-use"
              },
              {
                "url" : "http://hl7.org/fhir/StructureDefinition/structuredefinition-wg",
                "valueCode" : "fhir"
              }],
              "url" : "http://hl7.org/fhir/uv/xver/ImplementationGuide/{{{packageId}}}",
              "version" : "{{{_crossDefinitionVersion}}}",
              "name" : "XVer_{{{package.ShortName.ToLowerInvariant()}}}",
              "title" : "XVer-{{{package.ShortName}}}",
              "status" : "active",
              "date" : "2025-05-19T00:00:00+00:00",
              "publisher" : "HL7 International / FHIR Infrastructure",
              "contact" : [{
                "name" : "HL7 International / FHIR Infrastructure",
                "telecom" : [{
                  "system" : "url",
                  "value" : "http://www.hl7.org/Special/committees/fiwg"
                }]
              }],
              "description" : "All Cross Version Extensions for for FHIR {{{package.ShortName}}}",
              "jurisdiction" : [{
                "coding" : [{
                  "system" : "http://unstats.un.org/unsd/methods/m49/m49.htm",
                  "code" : "001",
                  "display" : "World"
                }]
              }],
              "packageId" : "{{{packageId}}}",
              "license" : "CC0-1.0",
              "fhirVersion" : ["{{{package.PackageVersion}}}"],
              "dependsOn" : [{
                "id" : "hl7tx",
                "extension" : [{
                  "url" : "http://hl7.org/fhir/tools/StructureDefinition/implementationguide-dependency-comment",
                  "valueMarkdown" : "Automatically added as a dependency - all IGs depend on HL7 Terminology"
                }],
                "uri" : "http://terminology.hl7.org/ImplementationGuide/hl7.terminology",
                "packageId" : "hl7.terminology.{{{packageSuffix}}}",
                "version" : "{{{_thoPackageVersion}}}"
              },
              {
                "id" : "hl7_fhir_uv_extensions",
                "uri" : "http://hl7.org/fhir/extensions/ImplementationGuide/hl7.fhir.uv.extensions",
                "packageId" : "hl7.fhir.uv.extensions.{{{packageSuffix}}}",
                "version" : "{{{_extensionsPackVersion}}}"
              },
              {
                "id" : "hl7_fhir_uv_tools",
                "uri" : "http://hl7.org/fhir/tools/ImplementationGuide/hl7.fhir.uv.tools",
                "packageId" : "hl7.fhir.uv.tools.{{{toolsPackageSuffix}}}",
                "version" : "{{{_toolsPackageVersion}}}"
              }{{{additionalDependencies}}}
              ]{{{resources}}}
            }
            """;

        return igJson;
    }

}
