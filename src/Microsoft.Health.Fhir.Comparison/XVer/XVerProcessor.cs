// <copyright file="XVerProcessor.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Hl7.Fhir.Model;
using Hl7.Fhir.Utility;
using Microsoft.Extensions.Logging;
using Microsoft.Health.Fhir.Comparison.CompareTool;
using Microsoft.Health.Fhir.CodeGen.Configuration;
using Microsoft.Health.Fhir.CodeGen.FhirExtensions;
using Microsoft.Health.Fhir.CodeGen.Language;
using Microsoft.Health.Fhir.CodeGen.Models;
using Microsoft.Health.Fhir.CodeGenCommon.Extensions;
using Microsoft.Health.Fhir.CodeGenCommon.Packaging;
using Microsoft.Health.Fhir.CodeGenCommon.Utils;
using System.CommandLine;
using System.Linq;
using System.Data.Common;
using System.Collections.Concurrent;
using Microsoft.Health.Fhir.CodeGenCommon.Models;
using Microsoft.Health.Fhir.Comparison.Models;
using System.Xml.Linq;
using System.Data;
using CMR = Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship;
using Hl7.Fhir.Serialization;
using Hl7.Fhir.Specification.Snapshot;
using static System.Net.Mime.MediaTypeNames;
using Hl7.FhirPath.Sprache;
using Tasks = System.Threading.Tasks;
using System.IO;
using System.Reflection.Metadata;


namespace Microsoft.Health.Fhir.Comparison.XVer;

internal static partial class XVerProcessorLogMessages
{
    [LoggerMessage(Level = LogLevel.Warning, Message = "Failed to load requested cross-version maps for {cvMapKey}! Processing will be only algorithmic!")]
    internal static partial void LogMapsNotFound(this ILogger logger, string cvMapKey);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Failed to expand ValueSet {url} for comparison: {details}")]
    internal static partial void LogValueSetNotExpanded(this ILogger logger, string url, string? details);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Failed to retrieve ValueSet {url} from {dcKey}")]
    internal static partial void LogValueSetNotFound(this ILogger logger, string url, string dcKey);
}

internal static class XVerExtensions
{
    internal static string ForMdTable(this string? value) => string.IsNullOrEmpty(value) ? string.Empty : value.Replace("|", "\\|").Replace("\n", "<br/>").Replace("\r", "<br/>");

    internal static string ComparisonKey(this ValueSet vs, string graphId) => graphId + "_" + vs.Name.ToPascalCase();
}

public class XVerProcessor
{
    private string _crossDefinitionVersion = "0.7.0";

    internal static readonly ComparisonDirection[] _directions = [ComparisonDirection.Up, ComparisonDirection.Down];

    internal static readonly HashSet<string> _exclusionSet =
    [
        "http://hl7.org/fhir/ValueSet/ucum-units",
        "http://hl7.org/fhir/ValueSet/all-languages",
        "http://tools.ietf.org/html/bcp47",             // DSTU2 version of all-languages
        "http://hl7.org/fhir/ValueSet/mimetypes",
        //"http://hl7.org/fhir/ValueSet/use-context",
        //"http://hl7.org/fhir/ValueSet/jurisdiction",
    ];

    internal static readonly HashSet<string> _escapeValveCodes = [
        "OTHER",
        "Other",
        "other",
        "OTH",      // v3 Null Flavor of other
        "UNKNOWN",
        "Unknown",
        "unknown",
        "UNK",      // v3 Null Flavor of Unknown
        //"NI",       // v3 Null Flavor of No Information
        ];

    private ConfigXVer _config;
    private ILogger _logger;
    private DefinitionCollection[] _definitions = [];
    private Dictionary<string, int> _definitionIndexes = [];
    private Dictionary<(string left, string right), FhirCoreComparer> _comparisonCache;
    private ComparisonDatabase? _db = null;
    private Dictionary<string, HashSet<string>> _vsUrlsToInclude = [];

    private string _dbPath;
    private string? _dbName;

    public XVerProcessor(ConfigXVer config)
    {
        _config = config;
        _logger = config.LogFactory.CreateLogger<XVerProcessor>();

        string path = string.IsNullOrEmpty(_config.CrossVersionDbPath)
            ? Path.Combine(_config.CrossVersionMapSourcePath, "db")
            : _config.CrossVersionDbPath;

        if (path.EndsWith(".db"))
        {
            _dbPath = Path.GetDirectoryName(path) ?? path;
            _dbName = Path.GetFileName(path) ?? path;
        }
        else
        {
            _dbPath = path;
            _dbName = null;
        }

        _comparisonCache = [];
    }

    public XVerProcessor(ComparisonDatabase db, string outputDirectory, ILoggerFactory loggerFactory)
    {
        _config = new()
        {
            CrossVersionDbPath = Path.Combine(db.DbFilePath, db.DbFileName),
            OutputDirectory = outputDirectory,
            LogFactory = loggerFactory,
        };

        _logger = loggerFactory.CreateLogger<XVerProcessor>();

        _dbPath = db.DbFilePath;
        _dbName = db.DbFileName;

        _comparisonCache = [];
        _db = db;
    }

    private void loadDefinitionCollections()
    {
        List<DefinitionCollection> definitions = [];

        foreach (string directive in _config.ComparePackages)
        {
            if (FhirPackageUtils.PackageIsFhirCore(directive))
            {
                throw new Exception($"Package {directive} is not a FHIR Core package!");
            }

            // create a loader because these are all different FHIR core versions
            using CodeGen.Loader.PackageLoader loader = new(_config, new()
            {
                JsonModel = CodeGen.Loader.LoaderOptions.JsonDeserializationModel.SystemTextJson,
            });

            DefinitionCollection loaded = loader.LoadPackages([directive]).Result
                ?? throw new Exception($"Could not load package: {directive}");

            definitions.Add(loaded);
        }

        _definitions = definitions.ToArray();
        _definitions.ForEach((DefinitionCollection dc, int i) =>
        {
            _definitionIndexes.Add(dc.Key, i);
            return true;
        });
    }

    public void ProcessCommand(string? command)
    {
        switch (command)
        {
            case "create-db":
                LoadDatabase(true, true);
                LoadFhirCrossVersionMaps(preferV1Maps: true);
                break;

            case "compare":
                LoadDatabase(_config.ReloadDatabase, true);
                CompareInDatabase();
                break;

            case "compare-vs":
                LoadDatabase(_config.ReloadDatabase, true, FhirArtifactClassEnum.ValueSet);
                CompareInDatabase(FhirArtifactClassEnum.ValueSet);
                break;

            case "compare-sd":
                LoadDatabase(_config.ReloadDatabase, false, FhirArtifactClassEnum.Resource);
                CompareInDatabase(FhirArtifactClassEnum.Resource);
                break;

            case "docs":
                LoadDatabase(false, false);
                WriteDocsFromDatabase();
                break;

            case "docs-vs":
                LoadDatabase(false, false);
                WriteDocsFromDatabase(FhirArtifactClassEnum.ValueSet);
                break;

            case "docs-sd":
                LoadDatabase(false, false);
                WriteDocsFromDatabase(FhirArtifactClassEnum.Resource);
                break;

            case "fhir":
                LoadDatabase(false, false);
                WriteFhirFromDatabase();
                break;

            default:
                LoadDatabase(true, true);
                LoadFhirCrossVersionMaps(preferV1Maps: true);
                CompareInDatabase();
                WriteDocsFromDatabase();
                WriteFhirFromDatabase();
                break;
        }
    }

    public void LoadDatabase(
        bool forceCreate,
        bool allowSourceCopy,
        FhirArtifactClassEnum? artifactFilter = null)
    {
        // check if we have a database filename
        if (!forceCreate &&
            !string.IsNullOrEmpty(_dbName))
        {
            // try loading the database
            _db = new(_dbPath, _dbName);
            if (_db != null)
            {
                // check for copying from source
                if (!allowSourceCopy ||
                    string.IsNullOrEmpty(_config.CrossVersionSourceDb))
                {
                    return;
                }

                _db.LoadFromSourceDb(_config.CrossVersionSourceDb, artifactFilter);

                return;
            }
        }

        // load definitions if we have not done so
        if (_definitions.Length == 0)
        {
            loadDefinitionCollections();
        }

        // creating the database with defintions loads all the content
        _db = new(_definitions, _dbPath, _dbName, _config.LogFactory);
        _dbName = _db.DbFileName;

        // save the definition content in the database
        if (!_db.TryLoadFromDefinitionCollections(_exclusionSet, _escapeValveCodes))
        {
            throw new Exception($"Failed to load FHIR-based definitions into the database: {string.Join(", ", _definitions.Select(d => d.Key))}");
        }

        return;
    }

    public void CompareInDatabase(FhirArtifactClassEnum? artifactFilter = null)
    {
        if (_db == null)
        {
            throw new Exception("Comparison cannot run without a loaded database!");
        }

        FhirDbComparer dbComparer = new(_db, _config.LogFactory);
        dbComparer.Compare(artifactFilter, _config.ComparisonPairFilterKeys);
    }

    private record class PackageXverSupport
    {
        public required int PackageIndex { get; init; }
        public required DbFhirPackage Package { get; init; }
        public HashSet<string> BasicElements { get; init; } = [];
        public HashSet<string> AllowedExtensionTypes { get; init; } = [];
        public DefinitionCollection? CoreDC { get; set; } = null;
        public Hl7.Fhir.Specification.Snapshot.SnapshotGenerator? SnapshotGenerator { get; set; } = null;

    }

    /// <summary>
    /// Loads the definitions and initializes the comparison cache.
    /// </summary>
    /// <remarks>
    /// TODO(ginoc): this is only used to convert origin maps into the database.
    /// </remarks>
    /// <param name="preferV1Maps">Indicates whether to prefer version 1 maps.</param>
    /// <exception cref="InvalidOperationException">Thrown when there are less than two definitions available for comparison.</exception>
    public void LoadFhirCrossVersionMaps(bool preferV1Maps)
    {
        // need definitions loaded in order for existing cross-version maps to be usable
        if (_definitions.Length == 0)
        {
            loadDefinitionCollections();
        }

        if (_definitions.Length < 2)
        {
            throw new InvalidOperationException("At least two definitions are required to compare.");
        }

        if (_db == null)
        {
            LoadDatabase(_config.ReloadDatabase, true);
            if (_db == null)
            {
                throw new Exception($"Failed to create or load a comparison database!");
            }
        }

        // if this is a core comparison and we have a location, try to load existing cross-version maps
        if (_db.IsCoreComparison &&
            !string.IsNullOrEmpty(_config.CrossVersionMapSourcePath))
        {
            _ = _db.TryLoadFhirCrossVersionMaps(_config.CrossVersionMapSourcePath);
        }
    }


    private enum XverOutcomeCodes
    {
        UseElementSameName,
        UseElementRenamed,
        UseExtension,
        UseExtensionFromAncestor,
        UseBasicElement,
    }

    private record class XverOutcome
    {
        public required int SourcePackageKey { get; init; }
        public required string SourceStructureName { get; init; }
        public required string SourceElementId { get; init; }
        public required int SourceElementFieldOrder { get; init; }
        public required int TargetPackageKey { get; init; }
        public required XverOutcomeCodes OutcomeCode { get; init; }
        public required string? TargetElementId { get; init; }
        public required string? TargetExtensionUrl { get; init; }
    }

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

        // iterate over the packages to build the Basic resource element paths
        foreach ((DbFhirPackage package, int index) in packages.Select((p, i) => (p, i)))
        {
            // need to create a definition collection with the matching core package so that we can build everything
            string packageDirective = $"{package.PackageId}#{package.PackageVersion}";

            // create a loader because these are all different FHIR core versions
            using CodeGen.Loader.PackageLoader loader = new(_config, new()
            {
                JsonModel = CodeGen.Loader.LoaderOptions.JsonDeserializationModel.SystemTextJson,
            });

            DefinitionCollection coreDc = loader.LoadPackages([packageDirective]).Result
                ?? throw new Exception($"Could not load package: {packageDirective}");

            PackageXverSupport packageSupport = new()
            {
                PackageIndex = index,
                Package = package,
                BasicElements = [],
                CoreDC = coreDc,
                SnapshotGenerator = new(coreDc),
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

            Dictionary<(int sourceVsKey, int targetPackageId), ValueSet>  xverValueSets = buildXverValueSets(packages, focusPackageIndex);

            writeXverValueSets(packages, focusPackageIndex, xverValueSets, fhirDir);
            //writeFhirValueSets(packages, i, packageComparisonPairs, fhirDir, differentialVsBySourceKey);

            Dictionary<(int sourceElementKey, int targetPackageId), StructureDefinition> xverExtensions = [];
            buildXverStructures(packageSupports, focusPackageIndex, xverValueSets, xverExtensions, xverOutcomes, FhirArtifactClassEnum.ComplexType);
            buildXverStructures(packageSupports, focusPackageIndex, xverValueSets, xverExtensions, xverOutcomes, FhirArtifactClassEnum.Resource);

            writeXverStructures(packageSupports, focusPackageIndex, xverExtensions, fhirDir);

            writeXverSupportFiles(packageSupports, focusPackageIndex, xverValueSets, xverExtensions, fhirDir);
        }

        // write all of our outcome lists
        writeXverOutcomes(packageSupports, xverOutcomes, fhirDir);

        // //writeFhirStructures(packageComparisonPairs, fhirDir, differentialVsBySourceKey, FhirArtifactClassEnum.PrimitiveType);
        //writeFhirStructures(packageComparisonPairs, fhirDir, differentialVsBySourceKey, FhirArtifactClassEnum.ComplexType); 
        //writeFhirStructures(packageComparisonPairs, fhirDir, differentialVsBySourceKey, FhirArtifactClassEnum.Resource);
    }

    private void writeXverSupportFiles(
        List<PackageXverSupport> packageSupports,
        int focusPackageIndex,
        Dictionary<(int sourceVsKey, int targetPackageId), ValueSet> xverValueSets,
        Dictionary<(int sourceElementKey, int targetPackageId), StructureDefinition> xverExtensions,
        string fhirDir)
    {
        DbFhirPackage sourcePackage = packageSupports[focusPackageIndex].Package;

        foreach (PackageXverSupport targetSupport in packageSupports)
        {
            if (targetSupport.Package.Key == sourcePackage.Key)
            {
                continue;
            }

            string id = $"hl7.fhir.uv.xver.{sourcePackage.ShortName.ToLowerInvariant()}-{targetSupport.Package.ShortName.ToLowerInvariant()}";

            // build and write the ImplementationGuide resource
            {
                string igJson;

                if (targetSupport.Package.FhirVersionShort.StartsWith('4'))
                {
                    igJson = getIgJsonR4(sourcePackage, targetSupport.Package, xverValueSets, xverExtensions, id);
                }
                else if (targetSupport.Package.FhirVersionShort.StartsWith('5'))
                {
                    igJson = getIgJsonR5(sourcePackage, targetSupport.Package, xverValueSets, xverExtensions, id);
                }
                else
                {
                    // TODO: Implment DSTU2 and STU3
                    continue;
                }

                string filename = $"ImplementationGuide-{id}.json";
                File.WriteAllText(Path.Combine(fhirDir, $"{sourcePackage.ShortName}-for-{targetSupport.Package.ShortName}", "package", filename), igJson);
            }

            // build and write the package.manifest.json file
            {
                string pmJson = $$$"""
                    {
                      "version" : "{{{_crossDefinitionVersion}}}",
                      "fhirVersion" : ["{{{targetSupport.Package.PackageVersion}}}"],
                      "date" : "{{{DateTime.Now.ToString("yyyyMMddHHmmss")}}}",
                      "name" : "{{{id}}}",
                      "jurisdiction" : "http://unstats.un.org/unsd/methods/m49/m49.htm#001"
                    }
                    """;

                string filename = "package.manifest.json";
                File.WriteAllText(Path.Combine(fhirDir, $"{sourcePackage.ShortName}-for-{targetSupport.Package.ShortName}", "package", filename), pmJson);
            }

            // build and write the .index.json file
            {
                string indexJson = getIndexJson(sourcePackage, targetSupport.Package, xverValueSets, xverExtensions, id);
                string filename = ".index.json";
                File.WriteAllText(Path.Combine(fhirDir, $"{sourcePackage.ShortName}-for-{targetSupport.Package.ShortName}", "package", filename), indexJson);
            }

            // build and write the package.json file
            {
                string packageJson = $$$"""
                    {
                        "name" : "{{{id}}}",
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
                            "hl7.terminology.{{{targetSupport.Package.ShortName.ToLowerInvariant()}}}" : "6.3.0",
                            "hl7.fhir.uv.extensions.{{{targetSupport.Package.ShortName.ToLowerInvariant()}}}" : "5.2.0",
                            "hl7.fhir.uv.tools.{{{targetSupport.Package.ShortName.ToLowerInvariant()}}}" : "current"
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
                            "example" : "example"
                        },
                        "jurisdiction" : "http://unstats.un.org/unsd/methods/m49/m49.htm#001"
                    }
                    """;

                string filename = "package.json";
                File.WriteAllText(Path.Combine(fhirDir, $"{sourcePackage.ShortName}-for-{targetSupport.Package.ShortName}", "package", filename), packageJson);
            }
        }
    }

    private string getIndexJson(
        DbFhirPackage sourcePackage,
        DbFhirPackage targetPackage,
        Dictionary<(int sourceVsKey, int targetPackageId), ValueSet> xverValueSets,
        Dictionary<(int sourceElementKey, int targetPackageId), StructureDefinition> xverExtensions,
        string id)
    {
        // build the list of structures we are defining
        List<string> structureJsons = [];
        foreach (((int sourceElementKey, int targetPackageId), StructureDefinition sd) in xverExtensions)
        {
            if (targetPackageId != targetPackage.Key)
            {
                continue;
            }

            structureJsons.Add($$$"""
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
        List<string> valueSetJsons = [];
        foreach (((int sourceElementKey, int targetPackageId), ValueSet vs) in xverValueSets)
        {
            if (targetPackageId != targetPackage.Key)
            {
                continue;
            }

            valueSetJsons.Add($$$"""
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
                        "filename" : "ImplementationGuide-{{{id}}}.json",
                        "resourceType" : "ImplementationGuide",
                        "id" : "{{{id}}}",
                        "url" : "http://hl7.org/fhir/uv/xver/ImplementationGuide/{{{id}}}",
                        "version" : "{{{_crossDefinitionVersion}}}"
                    },
                    {{{string.Join(", ", structureJsons)}}},
                    {{{string.Join(", ", valueSetJsons)}}}
                ]
            }
            """;

        return indexJson;
    }


    private string getIgJsonR5(
        DbFhirPackage sourcePackage,
        DbFhirPackage targetPackage,
        Dictionary<(int sourceVsKey, int targetPackageId), ValueSet> xverValueSets,
        Dictionary<(int sourceElementKey, int targetPackageId), StructureDefinition> xverExtensions,
        string id)
    {
        ImplementationGuide ig = new()
        {
            Id = id,
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
            Url = $"http://hl7.org/fhir/uv/xver/ImplementationGuide/{id}",
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
            PackageId = $"hl7.fhir.uv.xver.{sourcePackage.ShortName.ToLowerInvariant()}-{targetPackage.ShortName.ToLowerInvariant()}",
            License = ImplementationGuide.SPDXLicense.CC01_0,
            FhirVersion = [FHIRVersion.N5_0_0],
            DependsOn = [
                new()
                {
                    ElementId = "hl7tx",
                    Uri = "http://terminology.hl7.org/ImplementationGuide/hl7.terminology",
                    PackageId = "hl7.terminology.r5",
                    Version = "6.3.0",
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
                    Version = "5.2.0",
                },
                new()
                {
                    ElementId = "hl7_fhir_uv_tools",
                    Uri = "http://hl7.org/fhir/tools/ImplementationGuide/hl7.fhir.uv.tools",
                    PackageId = "hl7.fhir.uv.tools.r5",
                    Version = "current",
                },
            ],
            Definition = new()
            {
                Resource = [],
            }
        };

        // add our structures
        foreach (((int sourceElementKey, int targetPackageId), StructureDefinition sd) in xverExtensions)
        {
            if (targetPackageId != targetPackage.Key)
            {
                continue;
            }

            ig.Definition.Resource.Add(new()
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

        // add our value sets
        foreach (((int sourceElementKey, int targetPackageId), ValueSet vs) in xverValueSets)
        {
            if (targetPackageId != targetPackage.Key)
            {
                continue;
            }

            ig.Definition.Resource.Add(new()
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

        return ig.ToJson(new FhirJsonSerializationSettings() { Pretty = true });
    }

    private string getIgJsonR4(
        DbFhirPackage sourcePackage,
        DbFhirPackage targetPackage,
        Dictionary<(int sourceVsKey, int targetPackageId), ValueSet> xverValueSets,
        Dictionary<(int sourceElementKey, int targetPackageId), StructureDefinition> xverExtensions,
        string id)
    {
        // build the list of structures we are defining
        List<string> structureJsons = [];
        foreach (((int sourceElementKey, int targetPackageId), StructureDefinition sd) in xverExtensions)
        {
            if (targetPackageId != targetPackage.Key)
            {
                continue;
            }

            structureJsons.Add($$$"""
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
        List<string> valueSetJsons = [];
        foreach (((int sourceElementKey, int targetPackageId), ValueSet vs) in xverValueSets)
        {
            if (targetPackageId != targetPackage.Key)
            {
                continue;
            }

            valueSetJsons.Add($$$"""
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

        string igJson = $$$"""
            {
              "resourceType" : "ImplementationGuide",
              "id" : "hl7.fhir.uv.xver.{{{sourcePackage.ShortName.ToLowerInvariant()}}}-{{{targetPackage.ShortName.ToLowerInvariant()}}}",
              "extension" : [{
                "url" : "http://hl7.org/fhir/StructureDefinition/structuredefinition-standards-status",
                "valueCode" : "trial-use"
              },
              {
                "url" : "http://hl7.org/fhir/StructureDefinition/structuredefinition-wg",
                "valueCode" : "fhir"
              }],
              "url" : "http://hl7.org/fhir/uv/xver/ImplementationGuide/hl7.fhir.uv.xver.{{{sourcePackage.ShortName.ToLowerInvariant()}}}-{{{targetPackage.ShortName.ToLowerInvariant()}}}",
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
              "packageId" : "hl7.fhir.uv.xver.{{{sourcePackage.ShortName.ToLowerInvariant()}}}-{{{targetPackage.ShortName.ToLowerInvariant()}}}",
              "license" : "CC0-1.0",
              "fhirVersion" : ["{{{targetPackage.PackageVersion}}}"],
              "dependsOn" : [{
                "id" : "hl7tx",
                "extension" : [{
                  "url" : "http://hl7.org/fhir/tools/StructureDefinition/implementationguide-dependency-comment",
                  "valueMarkdown" : "Automatically added as a dependency - all IGs depend on HL7 Terminology"
                }],
                "uri" : "http://terminology.hl7.org/ImplementationGuide/hl7.terminology",
                "packageId" : "hl7.terminology.{{{targetPackage.ShortName.ToLowerInvariant()}}}",
                "version" : "6.3.0"
              },
              {
                "id" : "hl7_fhir_uv_extensions",
                "uri" : "http://hl7.org/fhir/extensions/ImplementationGuide/hl7.fhir.uv.extensions",
                "packageId" : "hl7.fhir.uv.extensions.{{{targetPackage.ShortName.ToLowerInvariant()}}}",
                "version" : "5.2.0"
              },
              {
                "id" : "hl7_fhir_uv_tools",
                "uri" : "http://hl7.org/fhir/tools/ImplementationGuide/hl7.fhir.uv.tools",
                "packageId" : "hl7.fhir.uv.tools.{{{targetPackage.ShortName.ToLowerInvariant()}}}",
                "version" : "current"
              }],
              "definition" : {
                "resource" : [
                {{{string.Join(", ", structureJsons)}}},
                {{{string.Join(", ", valueSetJsons)}}}]
              }
            }
            """;

        return igJson;
    }

    private void writeXverOutcomes(
        List<PackageXverSupport> packageSupports,
        Dictionary<(int, string), List<List<XverOutcome>>> xverOutcomes,
        string fhirDir)
    {
        HashSet<string> createdDirs = [];

        // iterate over each structure in each source package
        foreach (((int sourcePackageIndex, string sourceStructureName), List<List<XverOutcome>> structureOutcomesByTarget) in xverOutcomes)
        {
            // iterate across each of our targets
            foreach ((List<XverOutcome> outcomes, int targetPackageIndex) in structureOutcomesByTarget.Select((ol, i) => (ol, i)))
            {
                if (sourcePackageIndex == targetPackageIndex)
                {
                    continue;
                }

                DbFhirPackage sourcePackage = packageSupports[sourcePackageIndex].Package;
                DbFhirPackage targetPackage = packageSupports[targetPackageIndex].Package;

                string packageFor = $"{sourcePackage.ShortName}-for-{targetPackage.ShortName}";
                string dir;
                if (createdDirs.Contains(packageFor))
                {
                    dir = Path.Combine(fhirDir, packageFor, "package", "doc");
                }
                else
                {
                    dir = Path.Combine(fhirDir, $"{sourcePackage.ShortName}-for-{targetPackage.ShortName}");
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }

                    dir = Path.Combine(dir, "package");
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }

                    dir = Path.Combine(dir, "doc");
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }

                    createdDirs.Add(packageFor);
                }

                // create a filename for this structure's md file
                string mdFilename = $"Lookup-{sourcePackage.ShortName}-{sourceStructureName}-{targetPackage.ShortName}.md";
                string htmlFilename = $"Lookup-{sourcePackage.ShortName}-{sourceStructureName}-{targetPackage.ShortName}.html";

                // open our files
                using ExportStreamWriter mdWriter = createMarkdownWriter(Path.Combine(dir, mdFilename), false, false);
                using ExportStreamWriter htmlWriter = createHtmlWriter(Path.Combine(dir, htmlFilename), false, false);

                // write a header
                mdWriter.WriteLine($"### Lookup for FHIR {sourcePackage.ShortName} {sourceStructureName} for use in FHIR {targetPackage.ShortName}");
                htmlWriter.WriteLine($"<h2>Lookup for FHIR {sourcePackage.ShortName} {sourceStructureName} for use in FHIR {targetPackage.ShortName}</h2>");

                mdWriter.WriteLine();
                mdWriter.WriteLine("| Source Element | Usage | Target |");
                mdWriter.WriteLine("| -------------- | ----- | ------ |");

                htmlWriter.WriteLine();
                htmlWriter.WriteLine("<table border=\"1\">");
                htmlWriter.WriteLine("<tr><th>Source Element</th><th>Usage</th><th>Target</th></tr>");

                // iterate over the elements of this structure in element order
                foreach (XverOutcome outcome in outcomes.OrderBy(xo => xo.SourceElementFieldOrder))
                {
                    mdWriter.WriteLine($"| {outcome.SourceElementId} | {outcome.OutcomeCode} | {outcome.TargetElementId ?? outcome.TargetExtensionUrl ?? "-"} |");
                    htmlWriter.WriteLine($"<tr><td>{outcome.SourceElementId}</td><td>{outcome.OutcomeCode}</td><td>{outcome.TargetElementId ?? outcome.TargetExtensionUrl ?? "-"}</td></tr>");
                }

                htmlWriter.WriteLine("</table>");

                mdWriter.Close();
                htmlWriter.Close();
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

            // build a path for this direction
            string dir = Path.Combine(fhirDir, focusPackage.ShortName + "-for-" + targetPackage.ShortName);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            dir = Path.Combine(dir, "package");
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

    private void writeXverStructures(
        List<PackageXverSupport> packageSupports,
        int focusPackageIndex,
        Dictionary<(int sourceElementKey, int targetPackageId), StructureDefinition> xverExtensions,
        string fhirDir)
    {
        Dictionary<int, DbFhirPackage> packageDict = packageSupports.Select(ps => ps.Package).ToDictionary(p => p.Key);
        DbFhirPackage focusPackage = packageSupports[focusPackageIndex].Package;

        ILookup<int, SnapshotGenerator?> generatorsById = packageSupports.ToLookup(ps => ps.Package.Key, ps => ps.SnapshotGenerator);

        int fileIndex = 0;

        // iterate over the value sets
        foreach (((int sourceKey, int targetPackageId), StructureDefinition sd) in xverExtensions)
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


            DbFhirPackage targetPackage = packageDict[targetPackageId];

            // build a path for this direction
            string dir = Path.Combine(fhirDir, focusPackage.ShortName + "-for-" + targetPackage.ShortName);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            dir = Path.Combine(dir, "package");
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            // write the value set to a file
            string filename = $"StructureDefinition-{sd.Id}.json";

            string path = Path.Combine(dir, filename);
            File.WriteAllText(path, sd.ToJson(new FhirJsonSerializationSettings() { Pretty = true }));
            fileIndex++;
        }
    }


    private void buildXverStructures(
        List<PackageXverSupport> packageSupports,
        int sourcePackageIndex,
        Dictionary<(int sourceVsKey, int targetPackageId), ValueSet> xverValueSets,
        Dictionary<(int sourceElementKey, int targetPackageId), StructureDefinition> xverExtensions,
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

            // iterate over the elements of our structure
            foreach (DbElement element in DbElement.SelectList(_db!.DbConnection, StructureKey: sd.Key, orderByProperties: [nameof(DbElement.ResourceFieldOrder)]))
            {
                // resolve the projection rows for this element
                List<DbGraphSd.DbElementRow> elementProjection = elementProjectionDict[element.Key];

                // check to see if this is the root element
                if (element.ResourceFieldOrder == 0)
                {
                    // iterate across each target version to see if this resource maps at all
                    for (int i = 0; i < packageSupports.Count; i++)
                    {
                        if (i == sourcePackageIndex)
                        {
                            structureMapsToBasic.Add(false);
                            continue;
                        }

                        // resolve the current column
                        List<DbGraphSd.DbElementCell?> sourceCells = elementProjection
                            .Select(row => row[i])
                            .ToList();

                        if (i > sourcePackageIndex)
                        {
                            structureMapsToBasic.Add((sourceCells.Count == 0) || (sourceCells.All(c => (c?.Element == null) || (c?.RightComparison == null))));
                            continue;
                        }

                        structureMapsToBasic.Add((sourceCells.Count == 0) || (sourceCells.All(c => (c?.Element == null) || (c?.LeftComparison == null))));
                    }
                }

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
                        if ((currentIndex > sourcePackageIndex) &&
                            generatedElementKeys[currentIndex-1].Contains(element.Key))
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

                            foreach (DbGraphSd.DbElementCell? cell in sourceCells)
                            {
                                // do not need to generate if equivalent or source is broader
                                if ((cell?.RightComparison?.Relationship == CMR.Equivalent) ||
                                    (cell?.RightComparison?.Relationship == CMR.SourceIsBroaderThanTarget))
                                {
                                    extensionNeeded = false;

                                    if (cell!.RightCell?.Element.Id == element.Id)
                                    {
                                        xverOutcomes[(sourcePackageIndex, sd.Name)][targetPackageIndex].Add(new()
                                        {
                                            SourcePackageKey = sourcePackage.Key,
                                            SourceStructureName = sd.Name,
                                            SourceElementId = element.Id,
                                            SourceElementFieldOrder = element.ResourceFieldOrder,
                                            TargetPackageKey = targetPackage.Key,
                                            OutcomeCode = XverOutcomeCodes.UseElementSameName,
                                            TargetElementId = cell!.RightCell!.Element.Id,
                                            TargetExtensionUrl = null,
                                        });
                                    }
                                    else
                                    {
                                        xverOutcomes[(sourcePackageIndex, sd.Name)][targetPackageIndex].Add(new()
                                        {
                                            SourcePackageKey = sourcePackage.Key,
                                            SourceStructureName = sd.Name,
                                            SourceElementId = element.Id,
                                            SourceElementFieldOrder = element.ResourceFieldOrder,
                                            TargetPackageKey = targetPackage.Key,
                                            OutcomeCode = XverOutcomeCodes.UseElementRenamed,
                                            TargetElementId = cell!.RightCell!.Element.Id,
                                            TargetExtensionUrl = null,
                                        });
                                    }

                                    break;
                                }
                            }
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
                        packageSupports[sourcePackageIndex],
                        packageSupports[targetPackageIndex],
                        sd,
                        element,
                        comparisons,
                        xverValueSets);

                    if (extSd != null)
                    {
                        xverExtensions.Add((element.Key, packageSupports[targetPackageIndex].Package.Key), extSd);
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

                            foreach (DbGraphSd.DbElementCell? cell in sourceCells)
                            {
                                // do not need to generate if equivalent or source is broader
                                if ((cell?.LeftComparison?.Relationship == CMR.Equivalent) ||
                                    (cell?.LeftComparison?.Relationship == CMR.SourceIsBroaderThanTarget))
                                {
                                    extensionNeeded = false;

                                    if (cell!.RightCell?.Element.Id == element.Id)
                                    {
                                        xverOutcomes[(sourcePackageIndex, sd.Name)][targetPackageIndex].Add(new()
                                        {
                                            SourcePackageKey = sourcePackage.Key,
                                            SourceStructureName = sd.Name,
                                            SourceElementId = element.Id,
                                            SourceElementFieldOrder = element.ResourceFieldOrder,
                                            TargetPackageKey = targetPackage.Key,
                                            OutcomeCode = XverOutcomeCodes.UseElementSameName,
                                            TargetElementId = cell!.LeftCell!.Element.Id,
                                            TargetExtensionUrl = null,
                                        });
                                    }
                                    else
                                    {
                                        xverOutcomes[(sourcePackageIndex, sd.Name)][targetPackageIndex].Add(new()
                                        {
                                            SourcePackageKey = sourcePackage.Key,
                                            SourceStructureName = sd.Name,
                                            SourceElementId = element.Id,
                                            SourceElementFieldOrder = element.ResourceFieldOrder,
                                            TargetPackageKey = targetPackage.Key,
                                            OutcomeCode = XverOutcomeCodes.UseElementRenamed,
                                            TargetElementId = cell!.LeftCell!.Element.Id,
                                            TargetExtensionUrl = null,
                                        });
                                    }
                                    break;
                                }
                            }
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
                        packageSupports[sourcePackageIndex],
                        packageSupports[targetPackageIndex],
                        sd,
                        element,
                        comparisons,
                        xverValueSets);

                    if (extSd != null)
                    {
                        xverExtensions.Add((element.Key, targetPackage.Key), extSd);
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

    //private void buildXverStructures(
    //    Dictionary<(int sourceElementKey, int targetPackageId), StructureDefinition> xverExtensions,
    //    List<PackageXverSupport> packageSupports,
    //    int sourcePackageIndex,
    //    HashSet<string> allowedExtensionTypes,
    //    Dictionary<(int sourceVsKey, int targetPackageId), ValueSet> xverValueSets,
    //    DbStructureDefinition sourceSd,
    //    Dictionary<int, List<DbGraphSd.DbElementRow>>? elementProjectionDict = null,
    //    HashSet<int>? elementsWithoutEquivalent = null,
    //    int currentPackageIndex = -1,
    //    int targetPackageIndex = -1)
    //{
    //    if (elementProjectionDict == null)
    //    {
    //        // build a graph for this structure
    //        DbGraphSd sdGraph = new()
    //        {
    //            DB = _db!.DbConnection,
    //            Packages = packageSupports.Select(ps => ps.Package).ToList(),
    //            KeySd = sourceSd,
    //        };

    //        // build a dictionary of all element projections, by element key
    //        elementProjectionDict = [];
    //        foreach (DbGraphSd.DbSdRow sdRow in sdGraph.Projection)
    //        {
    //            foreach (DbGraphSd.DbElementRow sdElementRow in sdRow.Projection)
    //            {
    //                if (sdElementRow.KeyCell == null)
    //                {
    //                    continue;
    //                }

    //                if (!elementProjectionDict.TryGetValue(sdElementRow.KeyCell.Element.Key, out List<DbGraphSd.DbElementRow>? elementList))
    //                {
    //                    elementList = [];
    //                    elementProjectionDict.Add(sdElementRow.KeyCell.Element.Key, elementList);
    //                }

    //                elementList.Add(sdElementRow);
    //            }
    //        }
    //    }

    //    // check for starting conditions
    //    if ((currentPackageIndex == -1) ||
    //        (targetPackageIndex == -1))
    //    {
    //        // if we are not the last package, build upwards
    //        if (sourcePackageIndex < (packageSupports.Count - 1))
    //        {
    //            buildXverStructures(
    //                xverExtensions,
    //                packageSupports,
    //                sourcePackageIndex,
    //                allowedExtensionTypes,
    //                xverValueSets,
    //                sourceSd,
    //                elementProjectionDict,
    //                elementsWithoutEquivalent,
    //                currentPackageIndex: sourcePackageIndex,
    //                targetPackageIndex: sourcePackageIndex + 1);
    //        }

    //        // if we are not the first package, build downwards
    //        if (sourcePackageIndex > 0)
    //        {
    //            buildXverStructures(
    //                xverExtensions,
    //                packageSupports,
    //                sourcePackageIndex,
    //                allowedExtensionTypes,
    //                xverValueSets,
    //                sourceSd,
    //                elementProjectionDict,
    //                elementsWithoutEquivalent,
    //                currentPackageIndex: sourcePackageIndex,
    //                targetPackageIndex: sourcePackageIndex - 1);
    //        }

    //        // done
    //        return;
    //    }

    //    // resolve the basic element hash for testing
    //    bool testingRight = currentPackageIndex < targetPackageIndex;
    //    bool testingLeft = !testingRight;
    //    elementsWithoutEquivalent ??= [];

    //    DbFhirPackage sourcePackage = packageSupports[sourcePackageIndex].Package;
    //    DbFhirPackage targetPackage = packageSupports[targetPackageIndex].Package;

    //    // check for the entire structure being unmapped (e.g., new resource)
    //    bool isUnmappedStructure = !elementProjectionDict.Values.Any(rows => rows.Any(row => row[targetPackage] != null));

    //    // if this is an unmapped structure, the entire structure is unmapped
    //    if (isUnmappedStructure)
    //    {
    //        // resolve the root row
    //        DbGraphSd.DbElementRow rootRow = elementProjectionDict
    //            .Values
    //            .First(rows => rows.Any(row => row.KeyCell?.Element.ResourceFieldOrder == 0))
    //            .First();


    //        StructureDefinition sd = createExtensionSd(
    //            packageSupports[sourcePackageIndex],
    //            packageSupports[targetPackageIndex],
    //            sourceSd,
    //            rootRow.KeyCell!.Element,
    //            elementProjectionDict);


    //        /*
    //            * TODO:
    //            * - filter target types based on allowed types (HashSet)
    //            * - map types that do not exist based on their mappings (e.g., R2:Quantity <-> R3:SimpleQuantity)
    //            * - make complex extensions based on elements in the tree
    //            * - filter top-level structures/elements for new types (only allow entire structures)
    //            * - determine correct targets
    //            */

    //        //// create a new snapshot
    //        //sd.Snapshot = new StructureDefinition.SnapshotComponent();


    //        // add this element extension to the dictionary
    //        xverExtensions.Add((rootRow.KeyCell!.Element.Key, targetPackage.Key), sd);


    //    }
    //    else
    //    {
    //        Dictionary<int, int> includedChildCounts = [];
    //        List<DbGraphSd.DbElementCell> cellsNeedingExtensions = [];

    //        // iterate over the projections, sorted by the key cell resource field order
    //        foreach ((int sourceElementKey, List<DbGraphSd.DbElementRow> elementProjections) in elementProjectionDict.OrderBy(kvp => kvp.Value.First().KeyCell!.Element.ResourceFieldOrder))
    //        {
    //            // skip testing if we know this element has already mapped out - always must generate
    //            if (elementsWithoutEquivalent.Contains(sourceElementKey))
    //            {
    //                continue;
    //            }

    //            // check to see if we have any equivalent mappings
    //            if (testingRight &&
    //                elementProjections.Any((DbGraphSd.DbElementRow elementRow) => elementRow[currentPackageIndex]?.RightComparison?.Relationship == CMR.Equivalent))
    //            {
    //                continue;
    //            }

    //            if (testingLeft &&
    //                elementProjections.Any((DbGraphSd.DbElementRow elementRow) => elementRow[currentPackageIndex]?.LeftComparison?.Relationship == CMR.Equivalent))
    //            {
    //                continue;
    //            }

    //            // add this element as not directly equivalent
    //            elementsWithoutEquivalent.Add(sourceElementKey);

    //            cellsNeedingExtensions.Add(elementProjections[0].KeyCell!);

    //            // resolve the element to build the parent id
    //            DbElement element = elementProjections[0].KeyCell?.Element ?? throw new Exception($"Failed to resolve element for {sourceElementKey} in {sourceSd.Name}!");

    //            // if this is not the root element, track the unmapped child count for later comparison
    //            if (element.ParentElementKey != null)
    //            {
    //                if (!includedChildCounts.TryGetValue((int)element.ParentElementKey, out int includedChildCount))
    //                {
    //                    includedChildCount = 0;
    //                }

    //                includedChildCount++;
    //                includedChildCounts[(int)element.ParentElementKey] = includedChildCount;
    //            }

    //            string sdId = $"{sourcePackage.ShortName}-{element.Path}-for-{targetPackage.ShortName}";

    //            StructureDefinition sd = new()
    //            {
    //                Url = $"http://hl7.org/fhir/uv/xver/{sourcePackage.FhirVersionShort}/StructureDefinition/{sdId}",
    //                Id = sdId,
    //                Version = _crossDefinitionVersion,
    //                Name = sdId,
    //                Title = $"Cross-version Extension for {sourcePackage.ShortName}.{element.Path} for use in FHIR {targetPackage.ShortName}",
    //                Status = PublicationStatus.Draft,
    //                Experimental = false,
    //                DateElement = new FhirDateTime(DateTimeOffset.Now),
    //                Description = $"This cross-version extension represents the {element.Path} element from {sourceSd.VersionedUrl} for use in FHIR {targetPackage.ShortName}.",
    //            };

    //            /*
    //             * TODO:
    //             * - filter target types based on allowed types (HashSet)
    //             * - map types that do not exist based on their mappings (e.g., R2:Quantity <-> R3:SimpleQuantity)
    //             * - make complex extensions based on elements in the tree
    //             * - filter top-level structures/elements for new types (only allow entire structures)
    //             * - determine correct targets
    //             */

    //            // add this element extension to the dictionary
    //            xverExtensions.Add((element.Key, targetPackage.Key), sd);
    //        }

    //        Dictionary<int, (DbGraphSd.DbElementCell elementCell, StructureDefinition extensionSd)> proposedExtensionsByKey = [];

    //        // iterate over our list of elements we are including, sorted by resource field order
    //        foreach (DbGraphSd.DbElementCell elementCell in cellsNeedingExtensions.OrderBy(ec => ec.Element.ResourceFieldOrder))
    //        {
    //            // TODO(ginoc)
    //        }
    //    }



    //    // check for continuing to the next package to the right
    //    if (testingRight &&
    //        (targetPackageIndex < packageSupports.Count - 1))
    //    {
    //        // build the value set for this package
    //        buildXverStructures(
    //            xverExtensions,
    //            packageSupports,
    //            sourcePackageIndex,
    //            allowedExtensionTypes,
    //            xverValueSets,
    //            sourceSd,
    //            elementProjectionDict,
    //            elementsWithoutEquivalent,
    //            currentPackageIndex: targetPackageIndex,
    //            targetPackageIndex: targetPackageIndex + 1);
    //    }

    //    // check for continuing to the next package to the left
    //    if (testingLeft &&
    //        (targetPackageIndex > 0))
    //    {
    //        // build the value set for this package
    //        buildXverStructures(
    //            xverExtensions,
    //            packageSupports,
    //            sourcePackageIndex,
    //            allowedExtensionTypes,
    //            xverValueSets,
    //            sourceSd,
    //            elementProjectionDict,
    //            elementsWithoutEquivalent,
    //            currentPackageIndex: targetPackageIndex,
    //            targetPackageIndex: targetPackageIndex - 1);
    //    }

    //    return;
    //}

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

    private StructureDefinition? createExtensionSd(
        PackageXverSupport sourcePackageSupport,
        PackageXverSupport targetPackageSupport,
        DbStructureDefinition sd,
        DbElement element,
        List<DbElementComparison> relevantComparisons,
        Dictionary<(int sourceVsKey, int targetPackageId), ValueSet> xverValueSets)
    {
        DbFhirPackage sourcePackage = sourcePackageSupport.Package;
        DbFhirPackage targetPackage = targetPackageSupport.Package;

        HashSet<string> basicElementPaths = targetPackageSupport.BasicElements;

        //string sdId = $"{sourcePackage.ShortName}-{element.Path}-for-{targetPackage.ShortName}";
        string sdId = $"extension-{collapsePathForId(element.Path)}";

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
        // if there is exactly one comparison relevant here, use it as the source
        else if (relevantComparisons.Count == 0)
        {
            // check to see if this structure type exists
            string sdName = element.Path.Split('.')[0];
            if (targetPackageSupport.CoreDC!.ComplexTypesByName.ContainsKey(sdName) ||
                targetPackageSupport.CoreDC!.ResourcesByName.ContainsKey(sdName))
            {
                contexts.Add(new()
                {
                    Type = StructureDefinition.ExtensionContextType.Element,
                    Expression = sdName,
                });
            }
            else
            {
                contexts.Add(new()
                {
                    Type = StructureDefinition.ExtensionContextType.Element,
                    Expression = "Element",
                });
            }
        }
        else
        {
            // build the list of possible contexts
            List<string> possible = relevantComparisons.Select(comp => comp.TargetElementToken ?? string.Empty).Distinct().ToList();

            // make sure the element exists in the target version
            possible = possible.Where(pe => targetPackageSupport.CoreDC!.TryFindElementByPath(pe, out _, out _)).ToList();

            if (possible.Count == 0)
            {
                possible.Add("Element");
            }

            contexts = possible.Select(pe => new StructureDefinition.ContextComponent()
            {
                Type = StructureDefinition.ExtensionContextType.Element,
                Expression = pe,
            }).ToList();
        }


        StructureDefinition extSd = new()
        {
            Id = sdId,
            //Url = $"http://hl7.org/fhir/uv/xver/{sourcePackage.FhirVersionShort}/StructureDefinition/extension-{element.Path.Replace("[x]", string.Empty)}",
            Url = $"http://hl7.org/fhir/{sourcePackage.FhirVersionShort}/StructureDefinition/extension-{element.Path.Replace("[x]", string.Empty)}",
            Name = sdId.Replace('-', '_').Replace('.', '_'),
            Version = _crossDefinitionVersion,
            FhirVersion = EnumUtility.ParseLiteral<FHIRVersion>(targetPackageSupport!.CoreDC!.FhirVersionLiteral) ?? FHIRVersion.N5_0_0,
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
        switch (element.CollatedTypeLiteral)
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



    public void WriteDocsFromDatabase(FhirArtifactClassEnum? artifactFilter = null, string? outputDir = null)
    {
        // check for no database
        if (_db == null)
        {
            throw new Exception("Cannot generate docs without a loaded database!");
        }

        //string outputDir = !string.IsNullOrEmpty(_config.OutputDirectory)
        //    ? _config.OutputDirectory
        //    : _config.CrossVersionMapSourcePath;

        outputDir ??= _config.CrossVersionMapSourcePath;

        // check for no output location
        if (string.IsNullOrEmpty(outputDir))
        {
            throw new Exception("Cannot write markdown docs without output or map source folder!");
        }

        string docDir = Path.Combine(outputDir, "docs");
        if (!Directory.Exists(docDir))
        {
            Directory.CreateDirectory(docDir);
        }

        _logger.LogInformation($"Writing markdown documentation to {docDir}");

        // if we are writing primitives, put the overall mapping doc in the root
        if ((artifactFilter == null) ||
            (artifactFilter == FhirArtifactClassEnum.PrimitiveType) ||
            (artifactFilter == FhirArtifactClassEnum.ComplexType) ||
            (artifactFilter == FhirArtifactClassEnum.Resource))
        {
            // write the generic primitive mappings (across all versions)
            writeMarkdownRootPrimitiveMaps(docDir);
        }

        // grab the FHIR Packages we are processing
        List<DbFhirPackage> packages = DbFhirPackage.SelectList(_db.DbConnection, orderByProperties: [nameof(DbFhirPackage.ShortName)]);
        List<DbFhirPackageComparisonPair> packageComparisonPairs = DbFhirPackageComparisonPair.SelectList(
            _db.DbConnection,
            orderByProperties: [nameof(DbFhirPackageComparisonPair.SourcePackageKey), nameof(DbFhirPackageComparisonPair.TargetPackageKey)]);

        // iterate over the list of packages
        foreach (DbFhirPackage package in packages)
        {
            // create the export directory for this package
            string packageDir = Path.Combine(docDir, FhirSanitizationUtils.SanitizeForProperty(package.ShortName));

            // check for the directory already existing
            if (Directory.Exists(packageDir))
            {
                // remove the directory and contents (start clean)
                if (artifactFilter == null)
                {
                    Directory.Delete(packageDir, true);
                    Directory.CreateDirectory(packageDir);
                }
                else if (artifactFilter == FhirArtifactClassEnum.ValueSet)
                {
                    Directory.Delete(Path.Combine(packageDir, "ValueSets"), true);
                }
                else if ((artifactFilter == FhirArtifactClassEnum.PrimitiveType) ||
                    (artifactFilter == FhirArtifactClassEnum.ComplexType) ||
                    (artifactFilter == FhirArtifactClassEnum.Resource))
                {
                    if (Directory.Exists(Path.Combine(packageDir, "PrimitiveTypes")))
                    {
                        Directory.Delete(Path.Combine(packageDir, "PrimitiveTypes"), true);
                    }

                    if (Directory.Exists(Path.Combine(packageDir, "ComplexTypes")))
                    {
                        Directory.Delete(Path.Combine(packageDir, "ComplexTypes"), true);
                    }

                    if (Directory.Exists(Path.Combine(packageDir, "Resources")))
                    {
                        Directory.Delete(Path.Combine(packageDir, "Resources"), true);
                    }
                }
            }
            else
            {
                Directory.CreateDirectory(packageDir);
            }

            // write the contents of our value sets if requested
            if ((artifactFilter == null) ||
                (artifactFilter == FhirArtifactClassEnum.ValueSet))
            {
                writeMarkdownValueSets(packages, packageComparisonPairs, package, packageDir);
            }

            if ((artifactFilter == null) ||
                (artifactFilter == FhirArtifactClassEnum.PrimitiveType) ||
                (artifactFilter == FhirArtifactClassEnum.ComplexType) ||
                (artifactFilter == FhirArtifactClassEnum.Resource))
            {
                //writeMarkdownStructures(packages, packageComparisonPairs, package, packageDir, FhirArtifactClassEnum.PrimitiveType);
                writeMarkdownStructures(packages, packageComparisonPairs, package, packageDir, FhirArtifactClassEnum.ComplexType);
                writeMarkdownStructures(packages, packageComparisonPairs, package, packageDir, FhirArtifactClassEnum.Resource);
            }
        }
    }

    private void writeMarkdownStructures(
        List<DbFhirPackage> packages,
        List<DbFhirPackageComparisonPair> packageComparisonPairs,
        DbFhirPackage package,
        string dir,
        FhirArtifactClassEnum artifactClass)
    {
        int keyPackageColIndex = packages.FindIndex(fp => fp.Key == package.Key);

        string artifactPascal = artifactClass switch
        {
            FhirArtifactClassEnum.PrimitiveType => "PrimitiveTypes",
            FhirArtifactClassEnum.ComplexType => "ComplexTypes",
            FhirArtifactClassEnum.Resource => "Resources",
            _ => throw new InvalidOperationException("Invalid artifact class."),
        };

        string artifactLower = artifactPascal.ToLowerInvariant();

        string sdDir = Path.Combine(dir, artifactPascal);
        if (!Directory.Exists(sdDir))
        {
            Directory.CreateDirectory(sdDir);
        }

        string overviewFilename = Path.Combine(dir, artifactPascal + ".md");

        using ExportStreamWriter overviewWriter = createMarkdownWriter(overviewFilename, true, true);

        ConcurrentBag<string> overviewEntries = [];

        // get the list of all Value Sets in this version
        List<DbStructureDefinition> structures = DbStructureDefinition.SelectList(
            _db!.DbConnection,
            FhirPackageKey: package.Key,
            ArtifactClass: artifactClass);

        // iterate over our value sets and generate documents
        Parallel.ForEach(structures, (sd, cancellationToken) =>
        //foreach (DbStructureDefinition sd in structures)
        {
            DbGraphSd sdGraph = new()
            {
                DB = _db!.DbConnection,
                Packages = packages,
                KeySd = sd,
            };

            // get the overview entry for this value set
            string content = getMdOverviewEntry(packages, package, sd, sdGraph.Projection);

            // get the overview entry for this value set
            overviewEntries.Add(content);

            string filename = Path.Combine(sdDir, getSdFilename(sd.Name, artifactClass, includeRelativeDir: false));
            using (ExportStreamWriter vsWriter = createMarkdownWriter(filename, true, true))
            {
                writeMdDetailedSd(_db!.DbConnection, vsWriter, packages, package, keyPackageColIndex, sd, sdGraph);
            }
        });
        //}

        writeMdOverviewSd(overviewWriter, packages, package, artifactClass);
        foreach (string line in overviewEntries.Order())
        {
            overviewWriter.WriteLineIndented(line);
        }

        return;
    }

    private void writeMdDetailedSd(
        IDbConnection db,
        ExportStreamWriter writer,
        List<DbFhirPackage> packages,
        DbFhirPackage package,
        int keyPackageColIndex,
        DbStructureDefinition keySd,
        DbGraphSd sdGraph)
    {
        writer.WriteLine($"""
            ### {keySd.Name}

            |      |     |
            | ---: | --- |
            | Package | {package.PackageId.ForMdTable()}@{package.PackageVersion.ForMdTable()} |
            | Stucture Name | {keySd.Name.ForMdTable()} |
            | Canonical URL | `{keySd.UnversionedUrl.ForMdTable()}` |
            | Version | {keySd.Version.ForMdTable()} |
            | Description | {keySd.Description.ForMdTable()} |
            | Status | `{keySd.Status}` |
            | Artifact Class | `{keySd.ArtifactClass}` |
            | Database Key | `{keySd.Key}` |
            | Database Snapshot Count | `{keySd.SnapshotCount}` |
            | Database Differential Count | `{keySd.DifferentialCount}` |

            ### Elements

            | Id | Path | Name | Base Path | Short | Cardinality | Collated Type | Binding Strength | Binding Value Set |
            | -- | ---- | ---- | --------- | ----- | ----------- | ------------- | ---------------- | ----------------- |
            """);

        foreach (DbElement e in DbElement.SelectList(db, StructureKey: keySd.Key, orderByProperties: [nameof(DbElement.Id)]))
        {
            writer.WriteLine(
                $"| `{e.Id.ForMdTable()}`" +
                $" | `{e.Path.ForMdTable()}`" +
                $" | `{e.Name.ForMdTable()}`" +
                $" | {e.BasePath?.ForMdTable()}" +
                $" | {e.Short.ForMdTable()}" +
                $" | {e.MinCardinality}..{e.MaxCardinalityString}" +
                $" | {e.CollatedTypeLiteral.ForMdTable()}" +
                $" | {(e.ValueSetBindingStrength == null ? string.Empty : "`" + e.ValueSetBindingStrength + "`")}" +
                $" | {(e.BindingValueSet == null ? string.Empty : "`" + e.BindingValueSet + "`")}" +
                $" |");
        }

        // if there are no mappings, we are done writing this file
        if ((sdGraph.Projection.Count == 0) ||
            (
                (sdGraph.Projection.Count == 1) &&
                (sdGraph.Projection[0][keyPackageColIndex]?.LeftComparison == null) &&
                (sdGraph.Projection[0][keyPackageColIndex]?.RightComparison == null)
            ))
        {
            writer.WriteLine($"""
                ### Empty Projection

                This Structure ({keySd.ArtifactClass}) resulted in no projection (no mappings to other packages).

                """);

            return;
        }

        int byTwoColumnCount = (packages.Count * 2) - 1;

        string sdNamePascal = keySd.Name.ToPascalCase();
        string sdNameClean = FhirSanitizationUtils.SanitizeForProperty(keySd.Name, convertToConvention: FhirNameConventionExtensions.NamingConvention.PascalCase);

        string artifactPascal = keySd.ArtifactClass switch
        {
            FhirArtifactClassEnum.PrimitiveType => "PrimitiveTypes",
            FhirArtifactClassEnum.ComplexType => "ComplexTypes",
            FhirArtifactClassEnum.Resource => "Resources",
            _ => throw new InvalidOperationException("Invalid artifact class."),
        };

        string[] sdRootUrlsByVersion = packages.Select(fp => $"/docs/{FhirSanitizationUtils.SanitizeForProperty(fp.ShortName)}/{artifactPascal}").ToArray();

        (string key, bool hasMapping)[] allKeys = packages.Select((fp, i) => (fp.ShortName, sdGraph.Projection.Any(r => r[i] != null))).ToArray();

        // generate table showing the mappings
        writer.WriteLine("### Mapping Table");
        writer.WriteLine();
        writer.WriteLine("| " + string.Join(" | Comparison | ", allKeys.Select(v => v.key)));
        writeTableColumns(writer, "---", byTwoColumnCount, appendNewline: true);

        foreach (DbGraphSd.DbSdRow row in sdGraph.Projection)
        {
            int column = -1;
            // traverse columns
            foreach (DbGraphSd.DbSdCell? cell in row)
            {
                column++;

                if (cell == null)
                {
                    writer.Write("| | ");
                    continue;
                }

                writer.Write(
                    $"| [{cell.Sd.Name.ForMdTable()}]({sdRootUrlsByVersion[column]}/{getSdFilename(cell.Sd.Name, cell.Sd.ArtifactClass, includeRelativeDir: false)})" +
                    $"<br/>" +
                    $" `{cell.Sd.VersionedUrl.ForMdTable()}` ");

                if (column == (row.Length - 1))
                {
                    writer.WriteLine();
                    continue;
                }

                (string toRight, string fromRight) = getMappingMdTableCell(cell, true);

                // write mapping notes
                writer.Write(
                    $"| {toRight}" +
                    $"<hr/>" +
                    $"{fromRight}");
            }
        }
        writer.WriteLine();

        // write a section for the element table
        writer.WriteLine("### Element Mappings");
        writer.WriteLine();

        int mapGroupIndex = 0;

        foreach (DbGraphSd.DbSdRow structureRow in sdGraph.Projection)
        {
            if (structureRow[keyPackageColIndex] == null)
            {
                continue;
            }

            writer.WriteLine();
            writer.WriteLine("#### Map Group " + mapGroupIndex++);
            writer.WriteLine();
            writer.WriteLine($"This group is centered on the Structure Definition {structureRow[keyPackageColIndex]!.Sd.Name} from {package.PackageId}@{package.PackageVersion} ({package.ShortName}, key {package.Key}).");
            writer.WriteLine("All elements from this structure are listed while other structures only show contents that have relationships with those elements.");
            writer.WriteLine();

            // write the table header
            for (int col = 0; col < packages.Count; col++)
            {
                if (col > 0)
                {
                    writer.Write("| Relationship ");
                }

                DbGraphSd.DbSdCell? cell = structureRow[col];

                if (cell == null)
                {
                    writer.Write("| *No Map* ");
                    continue;
                }

                if (col == keyPackageColIndex)
                {
                    writer.Write($"| {packages[col].ShortName} {cell.Sd.Name.ForMdTable()}");
                }
                else
                {
                    writer.Write($"| [{packages[col].ShortName} {cell.Sd.Name.ForMdTable()}]({sdRootUrlsByVersion[col]}/{getSdFilename(cell.Sd.Name, cell.Sd.ArtifactClass, includeRelativeDir: false)})");
                }
            }
            writer.WriteLine();
            writeTableColumns(writer, "---", byTwoColumnCount, appendNewline: true);

            HashSet<string>[] elementsPerSd = packages.Select(_ => new HashSet<string>()).ToArray();

            // iterate over the components in the concept projection
            foreach (DbGraphSd.DbElementRow elementRow in structureRow.Projection)
            {
                int column = -1;

                // traverse columns
                foreach (DbGraphSd.DbElementCell? cell in elementRow)
                {
                    column++;

                    if (cell == null)
                    {
                        writer.Write("| | ");
                        continue;
                    }

                    elementsPerSd[column].Add(cell.Element.Id);

                    if (column == keyPackageColIndex)
                    {
                        writer.Write($"| **`{cell.Element.Id.ForMdTable()}`**");
                    }
                    else
                    {
                        writer.Write($"| `{cell.Element.Id.ForMdTable()}`");
                    }

                    if (column == (elementRow.Length - 1))
                    {
                        continue;
                    }

                    if ((cell.RightCell == null) ||
                        (cell.RightComparison == null) ||
                        (cell.RightElement == null))
                    {
                        writer.Write("| ");
                    }
                    else
                    {
                        if ((cell.RightComparison.Relationship == ConceptMap.ConceptMapRelationship.Equivalent) &&
                            (cell.RightCell.LeftComparison?.Relationship == ConceptMap.ConceptMapRelationship.Equivalent))
                        {
                            writer.Write($"| _{cell.RightComparison.Relationship}_<br/>({cell.RightComparison.Key}/{cell.RightCell.LeftComparison?.Key})");
                        }
                        //else if ((cell.RightComparison.Relationship == ConceptMap.ConceptMapRelationship.SourceIsNarrowerThanTarget) &&
                        //    (cell.RightCell.LeftComparison?.Relationship == ConceptMap.ConceptMapRelationship.SourceIsBroaderThanTarget))
                        //{
                        //    writer.Write($"| ↢↢↢ ({cell.RightComparison.Key}/{cell.RightCell.LeftComparison?.Key})");
                        //}
                        //else if ((cell.RightComparison.Relationship == ConceptMap.ConceptMapRelationship.SourceIsBroaderThanTarget) &&
                        //    (cell.RightCell.LeftComparison?.Relationship == ConceptMap.ConceptMapRelationship.SourceIsNarrowerThanTarget))
                        //{
                        //    writer.Write($"| ↣↣↣ ({cell.RightComparison.Key}/{cell.RightCell.LeftComparison?.Key})");
                        //}
                        //if (cell.RightComparison.Relationship != cell.RightCell.LeftComparison?.Relationship)
                        //{
                        //    // write mapping notes
                        //    writer.Write(
                        //        $"| → {cell.RightComparison.Relationship} → " +
                        //        $"<hr/>" +
                        //        $"← {cell.RightCell.LeftComparison?.Relationship} ← ");
                        //}
                        //else if (cell.RightComparison.Relationship == ConceptMap.ConceptMapRelationship.Equivalent)
                        //{
                        //    writer.Write("| == ");
                        //}
                        //else if (cell.RightComparison.Relationship == ConceptMap.ConceptMapRelationship.SourceIsNarrowerThanTarget)
                        //{
                        //    writer.Write("| > ");
                        //}
                        //else if (cell.RightComparison.Relationship == ConceptMap.ConceptMapRelationship.SourceIsBroaderThanTarget)
                        //{
                        //    writer.Write("| < ");
                        //}
                        else
                        {
                            // write mapping notes
                            writer.Write(
                                $"| →→→→ _{cell.RightComparison.Relationship}_ →→→→ <br/>({cell.RightComparison.Key})" +
                                $"<hr/>" +
                                $"←←←← _{cell.RightCell.LeftComparison?.Relationship}_ ←←←← <br/>({cell.RightCell.LeftComparison?.Key})");
                        }
                    }
                }

                writer.WriteLine();
            }

            // check for unused elements in structures
            for (int i = 0; i < structureRow.Length; i++)
            {
                if (i != 0)
                {
                    writer.Write("| ");
                }

                if (structureRow[i] == null)
                {
                    writer.Write("| ");
                }
                else
                {
                    writer.Write($"| *{elementsPerSd[i].Count} of {structureRow[i]!.Sd.SnapshotCount} elements used* ");
                    if (elementsPerSd[i].Count < structureRow[i]!.Sd.SnapshotCount)
                    {
                        HashSet<string> allElements = structureRow[i]!.Elements.Select(c => c.Id).ToHashSet();
                        IEnumerable<string> unusedElements = allElements.Except(elementsPerSd[i]);
                        writer.Write($"<br/>remaining elements:<br/>{string.Join(", ", unusedElements.Select(v => "`" + v + "`"))}");
                    }

                }
            }

            writer.WriteLine();
            writer.WriteLine();
        }

        return;
    }

    private (string to, string from) getMappingMdTableCell(DbGraphSd.DbSdCell cell, bool movingRight)
    {
        DbGraphSd.DbSdCell? targetCell = movingRight ? cell.RightCell : cell.LeftCell;

        if (targetCell == null)
        {
            return ("<br/>*no map*<br/>", "<br/>*no map*<br/>");
        }

        DbStructureComparison? toComparison = movingRight ? cell.RightComparison : cell.LeftComparison;
        DbStructureComparison? fromComparison = movingRight ? targetCell.LeftComparison : cell.RightComparison;

        return (getLink(toComparison, targetCell, "→→→→→→→"), getLink(fromComparison, targetCell, "←←←←←←←"));

        string getLink(DbStructureComparison? comparison, DbGraphSd.DbSdCell? target, string arrows)
        {
            if ((comparison == null) || (target == null))
            {
                return arrows + "<br/>*no map*<br/>" + arrows;
            }

            return
                $"{arrows}" +
                $"<br/>`{comparison.Relationship}`" +
                $"<br/>- DBKey: `{comparison.Key}`" +
                $"<br/>- Reviewed: `{comparison.LastReviewedOn?.ToString("o") ?? "n/a"}`" +
                $"<br/>- By: `{comparison.LastReviewedBy ?? "n/a"}`" +
                $"<br/>- Identical: `{comparison.IsIdentical ?? false}`" +
                $"<br/>{arrows}";
        }
    }

    private void writeMdOverviewSd(
        ExportStreamWriter writer,
        List<DbFhirPackage> packages,
        DbFhirPackage package,
        FhirArtifactClassEnum artifactClass)
    {
        string artifactDisplay = artifactClass switch
        {
            FhirArtifactClassEnum.PrimitiveType => "Primitive Type",
            FhirArtifactClassEnum.ComplexType => "Complex Type",
            FhirArtifactClassEnum.Resource => "Resource",
            _ => throw new InvalidOperationException("Invalid artifact class."),
        };

        /*
            # Contents

            * [Required-Binding Value Sets](#required-binding-value-sets)
            * [Excluded Value Sets](#excluded-value-sets)
            * [Other Value Sets](#other-value-sets)
         */

        writer.Write($"""
            Keyed off: {package.PackageId}@{package.PackageVersion} - {package.ShortName}
            Canonical: {package.CanonicalUrl}
            
            ## {artifactDisplay} Overview
            
            """);


        List<string> headers = ["Name", "Canonical", "Description", ];
        foreach (DbFhirPackage targetPackage in packages.OrderBy(fp => fp.ShortName))
        {
            if (targetPackage.Key == package.Key)
            {
                continue;
            }

            headers.Add($"Path to {targetPackage.ShortName.ForMdTable()}");
        }

        writer.WriteLineIndented($"| {string.Join(" | ", headers)} |");
        writer.WriteLineIndented($"| {string.Join(" | ", Enumerable.Repeat("---", headers.Count))} |");
    }

    private string getMdOverviewEntry(
        List<DbFhirPackage> packages,
        DbFhirPackage package,
        DbStructureDefinition sd,
        List<DbGraphSd.DbSdRow> projection)
    {
        List<string> mapsTo = [];
        for (int i = 0; i < packages.Count; i++)
        {
            if (packages[i].Key == sd.FhirPackageKey)
            {
                continue;
            }
            mapsTo.Add(projection.Any(r => r[i] != null) ? "✔" : "");
        }

        return
            $"| [{sd.Name.ForMdTable()}]({getSdFilename(sd.Name, sd.ArtifactClass)})" +
            $" | `{sd.VersionedUrl.ForMdTable()}`" +
            $" | {sd.Description.ForMdTable()}" +
            $" | {string.Join(" | ", mapsTo)} |";
    }

    private void writeMarkdownValueSets(
        List<DbFhirPackage> packages,
        List<DbFhirPackageComparisonPair> packageComparisonPairs,
        DbFhirPackage package,
        string dir)
    {
        int keyPackageColIndex = packages.FindIndex(fp => fp.Key == package.Key);

        string vsDir = Path.Combine(dir, "ValueSets");
        if (!Directory.Exists(vsDir))
        {
            Directory.CreateDirectory(vsDir);
        }

        string overviewFilename = Path.Combine(dir, "ValueSets.md");

        using ExportStreamWriter overviewWriter = createMarkdownWriter(overviewFilename, true, true);

        ConcurrentBag<string> requiredOverviewEntries = [];
        ConcurrentBag<string> excludedOverviewEntries = [];
        ConcurrentBag<string> otherOverviewEntries = [];

        // get the list of all Value Sets in this version
        List<DbValueSet> valueSets = DbValueSet.SelectList(_db!.DbConnection, FhirPackageKey: package.Key);

        // iterate over our value sets and generate documents
        Parallel.ForEach(valueSets, (vs, cancellationToken) =>
        {
            DbGraphVs vsGraph = new()
            {
                DB = _db!.DbConnection,
                Packages = packages,
                KeyVs = vs,
            };

            // get the overview entry for this value set
            string content = getMdOverviewEntry(packages, package, vs, vsGraph.Projection);

            if (vs.IsExcluded == true)
            {
                excludedOverviewEntries.Add(content);
            }
            else if (vs.StrongestBindingCore == BindingStrength.Required)
            {
                // get the overview entry for this value set
                requiredOverviewEntries.Add(content);
            }
            else
            {
                // get the overview entry for this value set
                otherOverviewEntries.Add(content);
            }

            string filename = Path.Combine(vsDir, getVsFilename(vs.Name, includeRelativeDir: false));
            using (ExportStreamWriter vsWriter = createMarkdownWriter(filename, true, true))
            {
                writeMdDetailedVs(_db!.DbConnection, vsWriter, packages, package, keyPackageColIndex, vs, vsGraph);

                //// check for failures - write a stub file with information about the value set
                //if (ca.FailureCode != null)
                //{
                //    writeMdComparisonFailed(vsWriter, vs);
                //    continue;
                //}
            }

        });

        ConcurrentBag<string>[] sectionEntries = [requiredOverviewEntries, excludedOverviewEntries, otherOverviewEntries];

        // write sections
        for (int i = 0; i < sectionEntries.Length; i++)
        {
            writeMdOverviewSectionVs(overviewWriter, packages, package, i);
            foreach (string line in sectionEntries[i].Order())
            {
                overviewWriter.WriteLineIndented(line);
            }
        }

        return;
    }

    private string getMdOverviewEntry(
        List<DbFhirPackage> packages,
        DbFhirPackage package,
        DbValueSet vs,
        List<DbGraphVs.DbVsRow> projection)
    {
        List<string> mapsTo = [];
        for (int i = 0; i < packages.Count; i++)
        {
            if (packages[i].Key == vs.FhirPackageKey)
            {
                continue;
            }

            mapsTo.Add(projection.Any(r => r[i] != null) ? "✔" : string.Empty);
        }

        string expandCell = vs.CanExpand ? "✔" : $"✘ {vs.Message.ForMdTable()}";
        //string excludedCell = vs.IsExcluded ? "⚠" : string.Empty;

        return
            $"| [{vs.Name.ForMdTable()}]({getVsFilename(vs.Name)})" +
            $" | `{vs.VersionedUrl.ForMdTable()}`" +
            $" | {vs.Description?.ForMdTable()}" +
            $" | {expandCell}" +
            $" | {string.Join(" | ", mapsTo)} |";
    }

    private void writeMdOverviewSectionVs(
        ExportStreamWriter writer,
        List<DbFhirPackage> packages,
        DbFhirPackage package,
        int section)
    {
        switch (section)
        {
            case 0:
                writer.Write($"""
                    Keyed off: {package.PackageId}@{package.PackageVersion} - {package.ShortName}
                    Canonical: {package.CanonicalUrl}

                    # Contents

                    * [Required-Binding Value Sets](#required-binding-value-sets)
                    * [Excluded Value Sets](#excluded-value-sets)
                    * [Other Value Sets](#other-value-sets)
            
                    ## Required-Binding Value Sets

                    """);
                break;

            case 1:
                writer.Write($"""

                    ## Excluded Value Sets

                    """);
                break;

            case 2:
                writer.Write($"""

                    ## Other Value Sets

                    """);
                break;
        }

        List<string> headers = ["Name", "Canonical", "Description", "Expands"];
        foreach (DbFhirPackage targetPackage in packages.OrderBy(fp => fp.ShortName))
        {
            if (targetPackage.Key == package.Key)
            {
                continue;
            }

            headers.Add($"Path to {targetPackage.ShortName.ForMdTable()}");
        }

        writer.WriteLineIndented($"| {string.Join(" | ", headers)} |");
        writer.WriteLineIndented($"| {string.Join(" | ", Enumerable.Repeat("---", headers.Count))} |");
    }


    /// <summary>
    /// Writes a detailed markdown with information about this value set, keyed from this version.
    /// </summary>
    /// <remarks>
    /// Note this function is currently too long and very inefficient - will fix once output is
    /// finalized.
    /// </remarks>
    private void writeMdDetailedVs(
        IDbConnection db,
        ExportStreamWriter writer,
        List<DbFhirPackage> packages,
        DbFhirPackage package,
        int keyPackageColIndex,
        DbValueSet keyVs,
        DbGraphVs vsGraph)
    {
        writer.WriteLine($"""
            ### {keyVs.Name}

            |      |     |
            | ---: | --- |
            | Package | {package.PackageId.ForMdTable()}@{package.PackageVersion.ForMdTable()} |
            | VS Name | {keyVs.Name.ForMdTable()} |
            | Canonical URL | `{keyVs.UnversionedUrl.ForMdTable()}` |
            | Version | {keyVs.Version.ForMdTable()} |
            | Description | {keyVs.Description.ForMdTable()} |
            | Status | `{keyVs.Status}` |
            | Has Escape Valve Code | `{keyVs.HasEscapeValveCode}` |
            | Database Key | `{keyVs.Key}` |
            | Database Concept Count | `{keyVs.ConceptCount}` |
            | Database Active Concept Count | `{keyVs.ActiveConcreteConceptCount}` |
            """);

        writer.WriteLine("### Bindings");
        writer.WriteLine();
        writer.WriteLine("| Source | Element | Binding | Strength | Element Short |");
        writer.WriteLine("| ------ | ------- | ------- | -------- | ------------- |");

        // get the elements with bindings
        {
            List<DbElement> boundElements = DbElement.SelectList(db, BindingValueSetKey: keyVs.Key, orderByProperties: [nameof(DbElement.Key)]);

            foreach (DbElement ed in boundElements)
            {
                DbStructureDefinition? sd = DbStructureDefinition.SelectSingle(db, Key: ed.StructureKey);

                if (sd == null)
                {
                    writer.WriteLine(
                        $"| Unresolved Key: `{ed.StructureKey}`" +
                        $" | `{ed.Path.ForMdTable()}`" +
                        $" | `{ed.BindingValueSet.ForMdTable()}`" +
                        $" | `{ed.ValueSetBindingStrength}`" +
                        $" | {ed.Short.ForMdTable()}" +
                        $" |");
                }
                else
                {
                    writer.WriteLine(
                        $"| `{sd.UnversionedUrl.ForMdTable()}`" +
                        $" | `{ed.Path.ForMdTable()}`" +
                        $" | `{ed.BindingValueSet.ForMdTable()}`" +
                        $" | `{ed.ValueSetBindingStrength}`" +
                        $" | {ed.Short.ForMdTable()}" +
                        $" |");
                }
            }
        }

        writer.WriteLine();

        if (keyVs.CanExpand == false)
        {
            writer.WriteLine($"""
                ### Expansion Failure

                Failed to expand this value set: {keyVs.Message}
                """);
            return;
        }

        writer.WriteLine("### Referenced Systems");
        writer.WriteLine();

        if (string.IsNullOrEmpty(keyVs.ReferencedSystems))
        {
            writer.WriteLine("No referenced systems.");
        }
        else
        {
            string[] systems = keyVs.ReferencedSystems.Split(", ");
            foreach (string system in systems)
            {
                writer.WriteLine($"* `{system}`");
            }
        }

        // if there are no mappings, we are done writing this file
        if ((vsGraph.Projection.Count == 0) ||
            (
                (vsGraph.Projection.Count == 1) &&
                (vsGraph.Projection[0][keyPackageColIndex]?.LeftComparison == null) &&
                (vsGraph.Projection[0][keyPackageColIndex]?.RightComparison == null)
            ))
        {
            writer.WriteLine($"""
                ### Empty Projection

                This Value Set resulted in no projection (no mappings to other packages).

                ### Codes

                | System | Code | Display |
                | ------ | ---- | ------- |
                """);

            foreach (DbValueSetConcept c in DbValueSetConcept.SelectList(
                db,
                ValueSetKey: keyVs.Key,
                Inactive: false,
                Abstract: false,
                orderByProperties: [nameof(DbValueSetConcept.System), nameof(DbValueSetConcept.Code)]))
            {
                writer.WriteLine(
                    $"| `{c.System.ForMdTable()}`" +
                    $" | `{c.Code.ForMdTable()}`" +
                    $" | {c.Display?.ForMdTable()}" +
                    $" |");
            }

            return;
        }

        int byTwoColumnCount = (packages.Count * 2) - 1;

        string vsNamePascal = keyVs.Name.ToPascalCase();
        string vsNameClean = FhirSanitizationUtils.SanitizeForProperty(keyVs.Name, convertToConvention: FhirNameConventionExtensions.NamingConvention.PascalCase);

        string[] vsRootUrlsByVersion = packages.Select(fp => $"/docs/{FhirSanitizationUtils.SanitizeForProperty(fp.ShortName)}/ValueSets").ToArray();

        (string key, bool hasMapping)[] allKeys = packages.Select((fp, i) => (fp.ShortName, vsGraph.Projection.Any(r => r[i] != null))).ToArray();

        // generate table showing the mappings
        writer.WriteLine("### Mapping Table");
        writer.WriteLine();
        writer.WriteLine("| " + string.Join(" | Comparison | ", allKeys.Select(v => v.key)));
        writeTableColumns(writer, "---", byTwoColumnCount, appendNewline: true);

        foreach (DbGraphVs.DbVsRow row in vsGraph.Projection)
        {
            int column = -1;
            // traverse columns
            foreach (DbGraphVs.DbVsCell? cell in row)
            {
                column++;

                if (cell == null)
                {
                    writer.Write("| | ");
                    continue;
                }

                writer.Write(
                    $"| [{cell.Vs.Name.ForMdTable()}]({vsRootUrlsByVersion[column]}/{getVsFilename(cell.Vs.Name, includeRelativeDir: false)})" +
                    $"<br/>" +
                    $" `{cell.Vs.VersionedUrl.ForMdTable()}` ");

                if (column == (row.Length - 1))
                {
                    writer.WriteLine();
                    continue;
                }

                (string toRight, string fromRight) = getMappingMdTableCell(cell, true);

                // write mapping notes
                writer.Write(
                    $"| {toRight}" +
                    $"<hr/>" +
                    $"{fromRight}");
            }
        }
        writer.WriteLine();

        // write a section for the code table
        writer.WriteLine("### Code Mappings");
        writer.WriteLine();

        int mapGroupIndex = 0;
        //List<DbValueSetConcept> keyConcepts = DbValueSetConcept.SelectList(
        //    db,
        //    ValueSetKey: keyVs.Key,
        //    orderByProperties: [nameof(DbValueSetConcept.System), nameof(DbValueSetConcept.Code)]);

        foreach (DbGraphVs.DbVsRow valueSetRow in vsGraph.Projection)
        {
            if (valueSetRow[keyPackageColIndex] == null)
            {
                continue;
            }

            writer.WriteLine();
            writer.WriteLine("#### Map Group " + mapGroupIndex++);
            writer.WriteLine();
            writer.WriteLine($"This group is centered on the Value Set {valueSetRow[keyPackageColIndex]!.Vs.Name} from {package.PackageId}@{package.PackageVersion} ({package.ShortName}, key {package.Key}).");
            writer.WriteLine("All codes from this value set are listed while other value sets only show contents that have relationships with those codes.");
            writer.WriteLine();

            // write the table header
            for (int col = 0; col < packages.Count; col++)
            {
                if (col > 0)
                {
                    writer.Write("| Relationship ");
                }

                DbGraphVs.DbVsCell? cell = valueSetRow[col];

                if (cell == null)
                {
                    writer.Write("| *No Map* ");
                    continue;
                }

                if (col == keyPackageColIndex)
                {
                    writer.Write($"| {packages[keyPackageColIndex].ShortName} {cell.Vs.Name.ForMdTable()}");
                }
                else
                {
                    writer.Write($"| [{packages[keyPackageColIndex].ShortName} {cell.Vs.Name.ForMdTable()}]({vsRootUrlsByVersion[col]}/{getVsFilename(cell.Vs.Name, includeRelativeDir: false)})");
                }
            }
            writer.WriteLine();
            writeTableColumns(writer, "---", byTwoColumnCount, appendNewline: true);

            HashSet<string>[] codesPerVs = packages.Select(_ => new HashSet<string>()).ToArray();
            string? lastSystem = null;

            // iterate over the components in the concept projection
            foreach (DbGraphVs.DbVsConceptRow conceptRow in valueSetRow.Projection)
            {
                if (conceptRow[keyPackageColIndex]?.Concept.System != lastSystem)
                {
                    lastSystem = conceptRow[keyPackageColIndex]?.Concept.System;
                    writer.WriteLine($"""| <td colspan="{byTwoColumnCount - 1}">**{package.ShortName.ForMdTable()}** System: `{lastSystem.ForMdTable()}`""");

                    //writeTableColumns(
                    //    writer,
                    //    $"""{package.ShortName.ForMdTable()} System:<br/>`{lastSystem.ForMdTable()}`""",
                    //    byTwoColumnCount,
                    //    appendNewline: true,
                    //    valueOnlyInColumn: keyPackageColIndex * 2);
                }

                int column = -1;

                // traverse columns
                foreach (DbGraphVs.DbVsConceptCell? cell in conceptRow)
                {
                    column++;

                    if (cell == null)
                    {
                        writer.Write("| | ");
                        continue;
                    }

                    codesPerVs[column].Add(cell.Concept.FhirKey);

                    if (column == keyPackageColIndex)
                    {
                        writer.Write($"| **`{cell.Concept.Code.ForMdTable()}`**");
                    }
                    else
                    {
                        writer.Write($"| `{cell.Concept.Code.ForMdTable()}`");
                    }

                    if (column == (conceptRow.Length - 1))
                    {
                        continue;
                    }

                    if ((cell.RightCell == null) ||
                        (cell.RightComparison == null) ||
                        (cell.RightConcept == null))
                    {
                        writer.Write("| ");
                    }
                    else
                    {
                        if ((cell.RightComparison.Relationship == ConceptMap.ConceptMapRelationship.Equivalent) &&
                            (cell.RightCell.LeftComparison?.Relationship == ConceptMap.ConceptMapRelationship.Equivalent))
                        {
                            writer.Write($"| _{cell.RightComparison.Relationship}_ <br/>({cell.RightComparison.Key}/{cell.RightCell.LeftComparison?.Key})");
                        }
                        //else if ((cell.RightComparison.Relationship == ConceptMap.ConceptMapRelationship.SourceIsNarrowerThanTarget) &&
                        //    (cell.RightCell.LeftComparison?.Relationship == ConceptMap.ConceptMapRelationship.SourceIsBroaderThanTarget))
                        //{
                        //    writer.Write($"| ↢↢↢ ({cell.RightComparison.Key}/{cell.RightCell.LeftComparison?.Key})");
                        //}
                        //else if ((cell.RightComparison.Relationship == ConceptMap.ConceptMapRelationship.SourceIsBroaderThanTarget) &&
                        //    (cell.RightCell.LeftComparison?.Relationship == ConceptMap.ConceptMapRelationship.SourceIsNarrowerThanTarget))
                        //{
                        //    writer.Write($"| ↣↣↣ ({cell.RightComparison.Key}/{cell.RightCell.LeftComparison?.Key})");
                        //}
                        //else if (cell.RightComparison.Relationship != cell.RightCell.LeftComparison?.Relationship)
                        //{
                        //    // write mapping notes
                        //    writer.Write(
                        //        $"| → {cell.RightComparison.Relationship} → " +
                        //        $"<hr/>" +
                        //        $"← {cell.RightCell.LeftComparison?.Relationship} ← ");
                        //}
                        else
                        {
                            // write mapping notes
                            writer.Write(
                                $"| →→→→ _{cell.RightComparison.Relationship}_ →→→→ <br/>({cell.RightComparison.Key})" +
                                $"<hr/>" +
                                $"←←←← _{cell.RightCell.LeftComparison?.Relationship}_ ←←←← <br/>({cell.RightCell.LeftComparison?.Key}) ");
                        }
                    }
                }

                writer.WriteLine();
            }

            // check for unused codes in value sets
            for (int i = 0; i < valueSetRow.Length; i++)
            {
                if (i != 0)
                {
                    writer.Write("| ");
                }

                if (valueSetRow[i] == null)
                {
                    writer.Write("| ");
                }
                else
                {
                    writer.Write($"| *{codesPerVs[i].Count} of {valueSetRow[i]!.Vs.ConceptCount} codes used* ");
                    if (codesPerVs[i].Count < valueSetRow[i]!.Vs.ConceptCount)
                    {
                        HashSet<string> allCodes = valueSetRow[i]!.Concepts.Select(c => c.FhirKey).ToHashSet();
                        IEnumerable<string> unusedCodes = allCodes.Except(codesPerVs[i]);
                        writer.Write($"<br/>remaining codes:<br/>{string.Join(", ", unusedCodes.Select(v => "`" + v.Split('#')[^1] + "`"))}");
                    }
                }
            }

            writer.WriteLine();
            writer.WriteLine();
        }

        return;
    }

    private (string to, string from) getMappingMdTableCell(DbGraphVs.DbVsCell cell, bool movingRight)
    {
        DbGraphVs.DbVsCell? targetCell = movingRight ? cell.RightCell : cell.LeftCell;

        if (targetCell == null)
        {
            return ("<br/>*no map*<br/>", "<br/>*no map*<br/>");
        }

        DbValueSetComparison? toComparison = movingRight ? cell.RightComparison : cell.LeftComparison;
        DbValueSetComparison? fromComparison = movingRight ? targetCell.LeftComparison : cell.RightComparison;

        return (getLink(toComparison, targetCell, "→→→→→→→"), getLink(fromComparison, targetCell, "←←←←←←←"));

        string getLink(DbValueSetComparison? comparison, DbGraphVs.DbVsCell? target, string arrows)
        {
            if ((comparison == null) || (target == null))
            {
                return arrows + "<br/>*no map*<br/>" + arrows;
            }

            return
                $"{arrows}" +
                $"<br/>`{comparison.Relationship}`" +
                $"<br/>- DBKey: `{comparison.Key}`" +
                $"<br/>- Reviewed: `{comparison.LastReviewedOn?.ToString("o") ?? "n/a"}`" +
                $"<br/>- By: `{comparison.LastReviewedBy ?? "n/a"}`" +
                $"<br/>- Identical: `{comparison.IsIdentical ?? false}`" +
                $"<br/>{arrows}";

            //return $"[{comparison.CompositeName.ForMdTable()} ({comparison.Key})]" +
            //    $"(/input/codes_v2/{cell.DC.FhirSequence.ToRLiteral()}to{target.DC.FhirSequence.ToRLiteral()}/ConceptMap-{comparison.Name}.json)";
        }
    }


    //public void Compare(bool? saveUpdates = null, FhirArtifactClassEnum? artifactFilter = null)
    //{
    //    if (_definitions.Length < 2)
    //    {
    //        throw new InvalidOperationException("At least two definitions are required to compare.");
    //    }

    //    // load the current cross version maps if necessary
    //    if (_comparisonCache.Count == 0)
    //    {
    //        LoadFhirCrossVersionMaps(preferV1Maps: false);
    //    }

    //    if ((artifactFilter == null) ||
    //        (artifactFilter == FhirArtifactClassEnum.ValueSet) ||
    //        (artifactFilter == FhirArtifactClassEnum.Resource))
    //    {
    //        // discover the set of value sets that we want to compare across all selected versions
    //        _vsUrlsToInclude = getValueSetsToCompare();
    //    }

    //    // walk the definitions to run the comparisons between each version pair
    //    for (int definitionIndex = 1; definitionIndex < _definitions.Length; definitionIndex++)
    //    {
    //        DefinitionCollection left = _definitions[definitionIndex - 1];
    //        DefinitionCollection right = _definitions[definitionIndex];

    //        // grab the comparer for this pair (the same comparer will exist for either direction of the pair)
    //        if (!_comparisonCache.TryGetValue((left.Key, right.Key), out FhirCoreComparer? comparer))
    //        {
    //            _logger.LogMapsNotFound($"{left.Key} -> {right.Key}");
    //            continue;
    //        }

    //        if ((artifactFilter == null) ||
    //            (artifactFilter == FhirArtifactClassEnum.ValueSet) ||
    //            (artifactFilter == FhirArtifactClassEnum.Resource))
    //        {
    //            // register our filtered sets of value sets
    //            comparer.RegisterValueSetFilters(_vsUrlsToInclude[left.Key], _vsUrlsToInclude[right.Key]);
    //        }

    //        // run the comparison (bi-directional)
    //        comparer.Compare(artifactFilter);

    //        // save our results if necessary
    //        if (saveUpdates ?? _config.SaveComparisonResult)
    //        {
    //            comparer.Save(artifactFilter);
    //        }
    //    }
    //}

    /// <summary>
    /// Retrieves a dictionary of value sets to compare, based on required bindings and mappings between definition collections.
    /// </summary>
    /// <returns>A dictionary where the key is the definition collection key and the value is a set of unversioned value set URLs to include in the comparison.</returns>
    private Dictionary<string, HashSet<string>> getValueSetsToCompare()
    {
        Dictionary<string, HashSet<string>> vsUrlsToInclude = [];

        // first pass - find value sets that have required bindings
        foreach (DefinitionCollection dc in _definitions)
        {
            // iterate over the value sets in the first definition collection
            foreach ((string unversionedUrl, string[] versions) in dc.ValueSetVersions.OrderBy(kvp => kvp.Key))
            {
                // skip value sets we know we will not process
                if (_exclusionSet.Contains(unversionedUrl))
                {
                    continue;
                }

                // only compare on the highest version in this package
                string vsVersion = versions.OrderDescending().First();
                string versionedUrl = unversionedUrl + "|" + vsVersion;

                // we only need to process value sets that have a required binding
                if (dc.cgHasRequiredBinding(versionedUrl, unversionedUrl))
                {
                    vsUrlsToInclude.AddToValue(dc.Key, unversionedUrl);
                }
            }
        }

        // second pass - find value sets that have a map from a neighbor and were not already included, iterate until we do not find anything new
        bool addedVs = false;
        do
        {
            addedVs = false;

            for (int definitionIndex = 1; definitionIndex < _definitions.Length; definitionIndex++)
            {
                DefinitionCollection left = _definitions[definitionIndex - 1];
                DefinitionCollection right = _definitions[definitionIndex];

                // grab the comparer for this pair (always left to right)
                if (!_comparisonCache.TryGetValue((left.Key, right.Key), out FhirCoreComparer? comparer))
                {
                    _logger.LogMapsNotFound($"{left.Key} -> {right.Key}");
                    continue;
                }

                HashSet<string> leftValueSets = vsUrlsToInclude[left.Key];
                HashSet<string> rightValueSets = vsUrlsToInclude[right.Key];

                // iterate over all the currently-selected value sets in the left collection
                foreach (string leftVsUrl in leftValueSets)
                {
                    // get all the map targets for this value set (left to right)
                    List<string> targets = comparer.LeftToRight?.GetMapTargetsForVs(leftVsUrl) ?? [];

                    // make sure all these targets exist in the right set
                    foreach (string target in targets)
                    {
                        if (!rightValueSets.Contains(target))
                        {
                            rightValueSets.Add(target);
                            addedVs = true;
                        }
                    }
                }

                // check value set targets from right to left
                foreach (string rightVsUrl in rightValueSets)
                {
                    // get all the map targets for this value set (left to right)
                    List<string> targets = comparer.RightToLeft?.GetMapTargetsForVs(rightVsUrl) ?? [];

                    // make sure all these targets exist in the right set
                    foreach (string target in targets)
                    {
                        if (!leftValueSets.Contains(target))
                        {
                            leftValueSets.Add(target);
                            addedVs = true;
                        }
                    }
                }
            }
        } while (addedVs);

        return vsUrlsToInclude;
    }

    public void WriteComparisonDocs(FhirArtifactClassEnum? artifactFilter = null)
    {
        // check for no output location
        if (string.IsNullOrEmpty(_config.CrossVersionMapSourcePath))
        {
            return;
        }

        string docDir = Path.Combine(_config.CrossVersionMapSourcePath, "docs");
        if (!Directory.Exists(docDir))
        {
            Directory.CreateDirectory(docDir);
        }

        ValueSetGraph? vsGraph = null;

        if ((artifactFilter == null) ||
            (artifactFilter == FhirArtifactClassEnum.ValueSet) ||
            (artifactFilter == FhirArtifactClassEnum.Resource))
        {
            vsGraph = new()
            {
                Definitions = _definitions,
            };

            vsGraph.Build(_comparisonCache.Values);
        }

        StructureDefinitionGraph? primitiveGraph = null;
        StructureDefinitionGraph? complexGraph = null;

        if ((artifactFilter == null) ||
            (artifactFilter == FhirArtifactClassEnum.PrimitiveType) ||
            (artifactFilter == FhirArtifactClassEnum.ComplexType) ||
            (artifactFilter == FhirArtifactClassEnum.Resource))
        {
            primitiveGraph = new()
            {
                Definitions = _definitions,
                ArtifactType = FhirArtifactClassEnum.PrimitiveType,
            };

            primitiveGraph.Build(_comparisonCache.Values);

            complexGraph = new()
            {
                Definitions = _definitions,
                ArtifactType = FhirArtifactClassEnum.ComplexType,
            };

            complexGraph.Build(_comparisonCache.Values);
        }

        // if we are writing primitives, put the overall mapping doc in the root
        if ((artifactFilter == null) ||
            (artifactFilter == FhirArtifactClassEnum.PrimitiveType) ||
            (artifactFilter == FhirArtifactClassEnum.ComplexType) ||
            (artifactFilter == FhirArtifactClassEnum.Resource))
        {
            writeMarkdownRootPrimitiveMaps(docDir);
        }

        // walk the definitions to write comparisons
        foreach (DefinitionCollection dc in _definitions)
        {
            string versionDir = Path.Combine(docDir, dc.FhirSequence.ToRLiteral());

            // check for the directory already existing
            if (Directory.Exists(versionDir))
            {
                // remove the directory and contents (start clean)
                Directory.Delete(versionDir, true);
            }

            Directory.CreateDirectory(versionDir);

            // write the contents of our value sets
            if ((artifactFilter == null) ||
                (artifactFilter == FhirArtifactClassEnum.ValueSet))
            {
                writeMarkdownValueSets(versionDir, dc, vsGraph!);
            }

            // write the contents of our types
            if ((artifactFilter == null) ||
                (artifactFilter == FhirArtifactClassEnum.PrimitiveType) ||
                (artifactFilter == FhirArtifactClassEnum.ComplexType) ||
                (artifactFilter == FhirArtifactClassEnum.Resource))
            {
                writeMarkdownStructureDefinitions(versionDir, dc, primitiveGraph, FhirArtifactClassEnum.PrimitiveType);
                writeMarkdownStructureDefinitions(versionDir, dc, complexGraph, FhirArtifactClassEnum.ComplexType);
            }
        }
    }


    private void writeMarkdownRootPrimitiveMaps(string dir)
    {
        string overviewFilename = Path.Combine(dir, "PrimitiveTypes.md");

        using ExportStreamWriter writer = createMarkdownWriter(overviewFilename, true, true);

        writer.Write($"""
            ## Primitive Type Mappings

            Primitive types are mapped across all versions using the following table.

            Note that in this table, "concept" refers to the FHIR concept domain and "value" refers to the FHIR value domain.

            The statement: "`typeA` and `typeB` are conceptually interchangeable where appropriate" means that when an *element* provides
            the appropriate context, the concepts are not so disparate that they cannot be used.  For example, an element that
            was defined as a `id` in one version could be defined as a `code` in another version. While the types do not
            *inherently* have a conceptual overlap, the context of the element allows the substitution.  This is different than if
            an element was defined as an `boolean` and changed to a `dateTime`, which do not have the ability to be conceptually mapped.

            | Source Type | Target Type | Concept Relationship | Concept Comment | Value Relationship | Value Comment |
            | --- | --- | --- | --- | --- | --- |

            """);

        foreach (FhirTypeMappings.CodeGenTypeMapping mapping in FhirTypeMappings.PrimitiveMappings)
        {
            writer.WriteLine(
                $"| `{mapping.SourceType}` " +
                $"| `{mapping.TargetType}` " +
                $"| `{mapping.ConceptDomainRelationship}` " +
                $"| {mapping.ConceptDomainComment} " +
                $"| `{mapping.ValueDomainRelationship}` " +
                $"| {mapping.ValueDomainComment} " +
                $"|");
        }
    }

    private void writeMarkdownStructureDefinitions(string dir, DefinitionCollection dc, StructureDefinitionGraph? graph, FhirArtifactClassEnum artifactClass)
    {
        if (graph == null)
        {
            return;
        }

        string artifactPascal = artifactClass switch
        {
            FhirArtifactClassEnum.PrimitiveType => "PrimitiveTypes",
            FhirArtifactClassEnum.ComplexType => "ComplexTypes",
            FhirArtifactClassEnum.Resource => "Resources",
            _ => throw new InvalidOperationException("Invalid artifact class."),
        };

        string artifactLower = artifactPascal.ToLowerInvariant();

        string artifactDir = Path.Combine(dir, artifactPascal);
        if (!Directory.Exists(artifactDir))
        {
            Directory.CreateDirectory(artifactDir);
        }

        string overviewFilename = Path.Combine(dir, $"{artifactPascal}.md");

        using ExportStreamWriter overviewWriter = createMarkdownWriter(overviewFilename, true, true);

        writeMdOverviewIntroStructureDefinitions(overviewWriter, dc, artifactClass);

        ConcurrentBag<string> overviewEntries = [];

        IReadOnlyDictionary<string, StructureDefinition> structureDict = dc.GetStructureIndexDict(artifactClass);

        // iterate over our value sets from this version
        Parallel.ForEach(structureDict, (kvp, cancellationToken) =>
        {
            // build the projection for this value set
            List<StructureDefinitionGraphCell?[]> projection = graph.Project(dc, kvp.Value);

            // add our overview entry
            overviewEntries.Add(getMdOverviewEntry(kvp.Value, artifactClass, dc, projection));

            string filename = Path.Combine(artifactDir, getSdFilename(kvp.Value.Name.ToPascalCase(), artifactClass, includeRelativeDir: false));
            using (ExportStreamWriter artifactWriter = createMarkdownWriter(filename, true, true))
            {
                writeMdDetailed(artifactWriter, kvp.Value, artifactClass, dc, projection);

                //// check for failures - write a stub file with information about the structure
                //if (ca.FailureCode != null)
                //{
                //    writeMdComparisonFailed(vsWriter, vs);
                //    continue;
                //}
            }
        });

        // write our overview file
        foreach (string line in overviewEntries.Order())
        {
            overviewWriter.WriteLineIndented(line);
        }
    }

    private void writeMarkdownValueSets(string dir, DefinitionCollection dc, ValueSetGraph? graph)
    {
        if (graph == null)
        {
            return;
        }

        string vsDir = Path.Combine(dir, "ValueSets");
        if (!Directory.Exists(vsDir))
        {
            Directory.CreateDirectory(vsDir);
        }

        string overviewFilename = Path.Combine(dir, "ValueSets.md");

        using ExportStreamWriter overviewWriter = createMarkdownWriter(overviewFilename, true, true);

        writeMdOverviewIntroValueSets(overviewWriter, dc);

        // build our set of value sets if necessary
        if (_vsUrlsToInclude.Count == 0)
        {
            _vsUrlsToInclude = getValueSetsToCompare();
        }

        ConcurrentBag<string> overviewEntries = [];

        // iterate over our value sets from this version
        Parallel.ForEach(_vsUrlsToInclude[dc.Key], (vsUrl, cancellationToken) =>
        {
            bool expanded = true;

            // resolve this value set
            if (!dc.TryExpandVs(vsUrl, out ValueSet? vs, out string? expandMessage))
            {
                _logger.LogValueSetNotFound(vsUrl, expandMessage ?? "failed to expand");
                expanded = false;

                // check to see if we can get an unexpanded one for the overview
                if (!dc.TryGetValueSet(vsUrl, out vs))
                {
                    _logger.LogValueSetNotFound(vsUrl, dc.Key);
                    return;
                }
            }

            // build the projection for this value set
            List<ValueSetGraphCell?[]> projection = expanded ? graph.Project(dc, vs) : [];

            // add our overview entry
            overviewEntries.Add(getMdOverviewEntry(vs, dc, projection, expanded, expandMessage));

            string filename = Path.Combine(vsDir, getVsFilename(vs.Name.ToPascalCase(), includeRelativeDir: false));
            using (ExportStreamWriter vsWriter = createMarkdownWriter(filename, true, true))
            {
                writeMdDetailed(vsWriter, vs, dc, projection, expanded, expandMessage);

                //// check for failures - write a stub file with information about the value set
                //if (ca.FailureCode != null)
                //{
                //    writeMdComparisonFailed(vsWriter, vs);
                //    continue;
                //}
            }
        });

        // write our overview file
        foreach (string line in overviewEntries.Order())
        {
            overviewWriter.WriteLineIndented(line);
        }
    }


    private string getMdOverviewEntry(
        StructureDefinition sd,
        FhirArtifactClassEnum artifactClass,
        DefinitionCollection dc,
        List<StructureDefinitionGraphCell?[]> projection)
    {
        string name = sd.Name.ToPascalCase();

        List<string> mapsTo = [];
        for (int i = 0; i < _definitions.Length; i++)
        {
            if (i == _definitionIndexes[dc.Key])
            {
                continue;
            }
            mapsTo.Add(projection.Any(r => r[i] != null) ? "✔" : "");
        }

        return
            $"| [{sd.Name.ForMdTable()}]({getSdFilename(name, artifactClass)})" +
            $" | `{sd.Url.ForMdTable()}`" +
            $" | {sd.Description.ForMdTable()}" +
            $" | {string.Join(" | ", mapsTo)} |";
    }

    private void writeMdOverviewIntroStructureDefinitions(ExportStreamWriter writer, DefinitionCollection dc, FhirArtifactClassEnum artifactClass)
    {
        string artifactDisplay = artifactClass switch
        {
            FhirArtifactClassEnum.PrimitiveType => "Primitive Type",
            FhirArtifactClassEnum.ComplexType => "Complex Type",
            FhirArtifactClassEnum.Resource => "Resource",
            _ => throw new InvalidOperationException("Invalid artifact class."),
        };

        writer.Write($"""
            Keyed off: {dc.Key}
            Canonical: {dc.MainPackageCanonical}
            
            ## {artifactDisplay} Overview

            """);

        List<string> headers = ["Canonical", "Name", "Description", ];
        foreach (DefinitionCollection targetDc in _definitions)
        {
            if (targetDc.Key == dc.Key)
            {
                continue;
            }

            headers.Add($"Path to {targetDc.Key}");
        }

        writer.WriteLineIndented($"| {string.Join(" | ", headers)} |");
        writer.WriteLineIndented($"| {string.Join(" | ", Enumerable.Repeat("---", headers.Count))} |");
    }

    private void writeMdDetailed(
        ExportStreamWriter writer,
        StructureDefinition keySd,
        FhirArtifactClassEnum artifactClass,
        DefinitionCollection keyDc,
        List<StructureDefinitionGraphCell?[]> projection)
    {
        string artifactDisplay = artifactClass switch
        {
            FhirArtifactClassEnum.PrimitiveType => "Primitive Type",
            FhirArtifactClassEnum.ComplexType => "Complex Type",
            FhirArtifactClassEnum.Resource => "Resource",
            _ => throw new InvalidOperationException("Invalid artifact class."),
        };

        string artifactPascal = artifactClass switch
        {
            FhirArtifactClassEnum.PrimitiveType => "PrimitiveTypes",
            FhirArtifactClassEnum.ComplexType => "ComplexTypes",
            FhirArtifactClassEnum.Resource => "Resources",
            _ => throw new InvalidOperationException("Invalid artifact class."),
        };

        int keyColumn = Array.IndexOf(_definitions, keyDc);

        writer.WriteLine($"""
            ### {keySd.Name}

            |      |     |
            | ---: | --- |
            | Package | {keyDc.Key} |
            | Name | {keySd.Name.ForMdTable()} |
            | URL | `{keySd.Url.ForMdTable()}` |
            | Version | {keySd.Version.ForMdTable()} |
            | Description | {keySd.Description.ForMdTable()} |
            """);

        // if there are no mappings, we are done writing this file
        if (projection.Count == 0)
        {
            writer.WriteLine($"""
                ### Empty Projection

                This {artifactDisplay} resulted in no projection.
                """);
            return;
        }

        string sdName = keySd.Name.ToPascalCase();

        string[] sdRootUrlsByVersion = _definitions.Select(dc => $"/docs/{dc.FhirSequence.ToRLiteral()}/{artifactPascal}").ToArray();

        (string key, bool hasMapping)[] allKeys = _definitions.Select((dc, i) => (dc.Key, projection.Any(r => r[i] != null))).ToArray();

        // generate table showing the mappings
        writer.WriteLine("### Mapping Table");
        writer.WriteLine();
        writer.WriteLine("| " + string.Join(" | Maps | ", allKeys.Select(v => v.key)));
        writeTableColumns(writer, "---", (_definitions.Length * 2) - 1, appendNewline: true);

        foreach (StructureDefinitionGraphCell?[] row in projection)
        {
            int column = -1;
            // traverse columns
            foreach (StructureDefinitionGraphCell? cell in row)
            {
                column++;

                if (cell == null)
                {
                    writer.Write("| | ");
                    continue;
                }

                writer.Write(
                    $"| [{cell.Resource.Name.ForMdTable()}]({sdRootUrlsByVersion[column]}/{getSdFilename(cell.Resource.Name.ToPascalCase(), artifactClass, includeRelativeDir: false)})" +
                    $"<br/>" +
                    $" `{cell.Resource.Url}` ");

                if (column == (row.Length - 1))
                {
                    writer.WriteLine();
                    continue;
                }

                (string overviewToRight, string toRight, string overviewFromRight, string fromRight) = getConceptMapMdLinks(cell, ComparisonDirection.Up, artifactClass);

                // primitives do not have artifact level maps
                if (artifactClass == FhirArtifactClassEnum.PrimitiveType)
                {
                    // write mapping notes
                    writer.Write(
                        $"| →→→→→→→ <br/>Overview: {overviewToRight}<br/> →→→→→→→ " +
                        $"<hr/>" +
                        $"←←←←←←← <br/>Overview: {overviewFromRight}<br/> ←←←←←←← ");
                }
                else
                {
                    // write mapping notes
                    writer.Write(
                        $"| →→→→→→→ <br/>Overview: {overviewToRight}<br/>Artifact: {toRight}<br/> →→→→→→→ " +
                        $"<hr/>" +
                        $"←←←←←←← <br/>Overview: {overviewFromRight}<br/>Artifact: {fromRight}<br/> ←←←←←←← ");
                }
            }
        }
        writer.WriteLine();


        //// write a section for the code table
        //writer.WriteLine("### Code Mappings");
        //writer.WriteLine();

        //int mapGroupIndex = 0;

        //foreach (ValueSetGraphCell?[] valueSetRow in projection)
        //{
        //    if (valueSetRow[keyColumn] == null)
        //    {
        //        continue;
        //    }

        //    writer.WriteLine();
        //    writer.WriteLine("#### Map Group " + mapGroupIndex++);
        //    writer.WriteLine();
        //    writer.WriteLine($"This group is centered on the Value Set {valueSetRow[keyColumn]!.Resource.Name} from {valueSetRow[keyColumn]!.DC.Key} (column {keyColumn}).");
        //    writer.WriteLine("All codes from this value set are listed while other value sets only show contents that have relationships with those codes.");
        //    writer.WriteLine();

        //    // write the table header
        //    for (int col = 0; col < _definitions.Length; col++)
        //    {
        //        if (col > 0)
        //        {
        //            writer.Write("| Relationship ");
        //        }

        //        ValueSetGraphCell? cell = valueSetRow[col];

        //        if (cell == null)
        //        {
        //            writer.Write("| *No Map* ");
        //            continue;
        //        }

        //        if (col == keyColumn)
        //        {
        //            writer.Write($"| {cell.DC.Key} {cell.Resource.Name.ForMdTable()}");
        //        }
        //        else
        //        {
        //            writer.Write($"| [{cell.DC.Key} {cell.Resource.Name.ForMdTable()}]({sdRootUrlsByVersion[col]}/{getVsFilename(cell.Resource.Name.ToPascalCase(), includeRelativeDir: false)})");
        //        }
        //    }
        //    writer.WriteLine();
        //    writeTableColumns(writer, "---", (_definitions.Length * 2) - 1, appendNewline: true);

        //    // build a code map graph
        //    ValueSetComponentGraph codeMapGraph = new()
        //    {
        //        SourceRow = valueSetRow,
        //    };

        //    HashSet<string>[] codesPerVs = _definitions.Select(_ => new HashSet<string>()).ToArray();

        //    // iterate over the components in the key value set
        //    foreach (ValueSet.ContainsComponent component in valueSetRow[keyColumn]!.Resource.cgGetFlatContains())
        //    {
        //        bool hasMap = false;

        //        // project this component
        //        foreach (ValueSetComponentGraphCell?[] componentRow in codeMapGraph.Project(valueSetRow[keyColumn]!, component))
        //        {
        //            hasMap = true;
        //            int column = -1;

        //            // traverse columns
        //            foreach (ValueSetComponentGraphCell? cell in componentRow)
        //            {
        //                column++;

        //                if (cell == null)
        //                {
        //                    writer.Write("| | ");
        //                    continue;
        //                }

        //                codesPerVs[column].Add(cell.Component.cgKey());

        //                if (column == keyColumn)
        //                {
        //                    writer.Write($"| **`{cell.Component.Code.ForMdTable()}`**");
        //                }
        //                else
        //                {
        //                    writer.Write($"| `{cell.Component.Code.ForMdTable()}`");
        //                }

        //                if (column == (componentRow.Length - 1))
        //                {
        //                    continue;
        //                }

        //                if (cell.RightEdge == null)
        //                {
        //                    writer.Write("| ");
        //                }
        //                else
        //                {
        //                    if ((cell.RightEdge?.UpTarget?.Relationship == ConceptMap.ConceptMapRelationship.Equivalent) &&
        //                        (cell.RightEdge?.DownTarget?.Relationship == ConceptMap.ConceptMapRelationship.Equivalent))
        //                    {
        //                        writer.Write("| == ");
        //                    }
        //                    else if ((cell.RightEdge?.UpTarget?.Relationship == ConceptMap.ConceptMapRelationship.SourceIsNarrowerThanTarget) &&
        //                             (cell.RightEdge?.DownTarget?.Relationship == ConceptMap.ConceptMapRelationship.SourceIsBroaderThanTarget))
        //                    {
        //                        writer.Write("| > ");
        //                    }
        //                    else if ((cell.RightEdge?.UpTarget?.Relationship == ConceptMap.ConceptMapRelationship.SourceIsBroaderThanTarget) &&
        //                             (cell.RightEdge?.DownTarget?.Relationship == ConceptMap.ConceptMapRelationship.SourceIsNarrowerThanTarget))
        //                    {
        //                        writer.Write("| < ");
        //                    }
        //                    else
        //                    {
        //                        // write mapping notes
        //                        writer.Write(
        //                            $"| → {cell.RightEdge?.UpTarget?.Relationship} → " +
        //                            $"<hr/>" +
        //                            $"← {cell.RightEdge?.DownTarget?.Relationship} ← ");
        //                    }
        //                }
        //            }

        //            writer.WriteLine();
        //        }

        //        // check for unmapped concepts
        //        if (!hasMap)
        //        {
        //            for (int i = 0; i < valueSetRow.Length; i++)
        //            {
        //                if (i == keyColumn)
        //                {
        //                    writer.Write($"| **`{component.Code.ForMdTable()}`**");
        //                }
        //                else
        //                {
        //                    writer.Write("| ");
        //                }
        //            }
        //            writer.WriteLine();
        //        }
        //    }

        //    // check for unused codes in value sets
        //    for (int i = 0; i < valueSetRow.Length; i++)
        //    {
        //        if (i != 0)
        //        {
        //            writer.Write("| ");
        //        }

        //        if (valueSetRow[i] == null)
        //        {
        //            writer.Write("| ");
        //        }
        //        else
        //        {
        //            writer.Write($"| *{codesPerVs[i].Count} of {valueSetRow[i]!.UniqueCodeCount} codes used* ");
        //        }
        //    }
        //    writer.WriteLine();

        //    writer.WriteLine();
        //}

        return;

        //bool isRelated(ConceptDomainRelationshipCodes? relationship) =>
        //    (relationship == ConceptDomainRelationshipCodes.Equivalent) ||
        //    (relationship == ConceptDomainRelationshipCodes.SourceIsNarrowerThanTarget) ||
        //    (relationship == ConceptDomainRelationshipCodes.SourceIsBroaderThanTarget) ||
        //    (relationship == ConceptDomainRelationshipCodes.Related);
    }

    private void writeMdOverviewIntroValueSets(ExportStreamWriter writer, DefinitionCollection dc)
    {
        writer.Write($"""
            Keyed off: {dc.Key}
            Canonical: {dc.MainPackageCanonical}
            
            ## Value Set Overview

            """);

        List<string> headers = [ "Canonical", "Name", "Description", "Expands" ];
        foreach (DefinitionCollection targetDc in _definitions)
        {
            if (targetDc.Key == dc.Key)
            {
                continue;
            }

            headers.Add($"Path to {targetDc.Key}");
        }

        writer.WriteLineIndented($"| {string.Join(" | ", headers)} |");
        writer.WriteLineIndented($"| {string.Join(" | ", Enumerable.Repeat("---", headers.Count))} |");
    }

    private string getMdOverviewEntry(
        ValueSet vs,
        DefinitionCollection dc,
        List<ValueSetGraphCell?[]> projection,
        bool expanded,
        string? expandFailureMessage)
    {
        string vsName = vs.Name.ToPascalCase();

        List<string> mapsTo = [];
        for (int i = 0; i < _definitions.Length; i++)
        {
            if (i == _definitionIndexes[dc.Key])
            {
                continue;
            }
            mapsTo.Add(projection.Any(r => r[i] != null) ? "✔" : "");
        }

        string expandCell = expanded ? "✔" : $"✘ {expandFailureMessage}";

        return
            $"| [{vs.Name.ForMdTable()}]({getVsFilename(vsName)})" +
            $" | `{vs.Url.ForMdTable()}`" +
            $" | {vs.Description.ForMdTable()}" +
            $" | {expandCell}" +
            $" | {string.Join(" | ", mapsTo)} |";
    }

    /// <summary>
    /// Writes a detailed markdown with information about this value set, keyed from this version.
    /// </summary>
    /// <remarks>
    /// Note this function is currently too long and very inefficient - will fix once output is
    /// finalized.
    /// </remarks>
    /// <param name="writer">       The writer.</param>
    /// <param name="keyVs">        The key vs.</param>
    /// <param name="keyDc">        The key device-context.</param>
    /// <param name="projection">   The projection.</param>
    /// <param name="expanded">     True if expanded.</param>
    /// <param name="expandMessage">Message describing the expand.</param>
    private void writeMdDetailed(
        ExportStreamWriter writer,
        ValueSet keyVs,
        DefinitionCollection keyDc,
        List<ValueSetGraphCell?[]> projection,
        bool expanded,
        string? expandMessage)
    {
        int keyColumn = Array.IndexOf(_definitions, keyDc);

        writer.WriteLine($"""
            ### {keyVs.Name}

            |      |     |
            | ---: | --- |
            | Package | {keyDc.Key} |
            | Name | {keyVs.Name.ForMdTable()} |
            | URL | `{keyVs.Url.ForMdTable()}` |
            | Version | {keyVs.Version.ForMdTable()} |
            | Description | {keyVs.Description.ForMdTable()} |

            ### Bindings

            | Source | Element | Binding | Strength |
            | ------ | ------- | ------- | -------- |
            """);

        // get the elements with bindings
        {
            IEnumerable<StructureElementCollection> bindings = keyDc.AllBindingsForVs(keyVs.Url);
            foreach (StructureElementCollection binding in bindings)
            {
                foreach (ElementDefinition ed in binding.Elements)
                {
                    writer.WriteLine($"| `{binding.Structure.Url}` | {ed.Path} | `{ed.Binding.ValueSet}` | {ed.Binding.Strength} |");
                }
            }
        }

        writer.WriteLine();

        if (!expanded)
        {
            writer.WriteLine($"""
                ### Expansion Failure

                Failed to expand this value set: {expandMessage}
                """);
            return;
        }

        // if there are no mappings, we are done writing this file
        if (projection.Count == 0)
        {
            writer.WriteLine($"""
                ### Empty Projection

                This Value Set resulted in no projection.
                """);
            return;
        }

        string vsName = keyVs.Name.ToPascalCase();

        string[] vsRootUrlsByVersion = _definitions.Select(dc => $"/docs/{dc.FhirSequence.ToRLiteral()}/ValueSets").ToArray();

        (string key, bool hasMapping)[] allKeys = _definitions.Select((dc, i) => (dc.Key, projection.Any(r => r[i] != null))).ToArray();

        // generate table showing the mappings
        writer.WriteLine("### Mapping Table");
        writer.WriteLine();
        writer.WriteLine("| " + string.Join(" | Maps | ", allKeys.Select(v => v.key)));
        writeTableColumns(writer, "---", (_definitions.Length * 2) - 1, appendNewline: true);

        foreach (ValueSetGraphCell?[] row in projection)
        {
            int column = -1;
            // traverse columns
            foreach (ValueSetGraphCell? cell in row)
            {
                column++;

                if (cell == null)
                {
                    writer.Write("| | ");
                    continue;
                }

                writer.Write(
                    $"| [{cell.Resource.Name.ForMdTable()}]({vsRootUrlsByVersion[column]}/{getVsFilename(cell.Resource.Name.ToPascalCase(), includeRelativeDir: false)})" +
                    $"<br/>" +
                    $" `{cell.Resource.Url}` ");

                if (column == (row.Length - 1))
                {
                    writer.WriteLine();
                    continue;
                }

                (string toRight, string fromRight) = getConceptMapMdLinks(cell, ComparisonDirection.Up);

                // write mapping notes
                writer.Write(
                    $"| →→→→→→→ <br/> {toRight} <br/> →→→→→→→ " +
                    $"<hr/>" +
                    $"←←←←←←← <br/> {fromRight} <br/> ←←←←←←← ");
            }
        }
        writer.WriteLine();


        // write a section for the code table
        writer.WriteLine("### Code Mappings");
        writer.WriteLine();

        int mapGroupIndex = 0;

        foreach (ValueSetGraphCell?[] valueSetRow in projection)
        {
            if (valueSetRow[keyColumn] == null)
            {
                continue;
            }

            writer.WriteLine();
            writer.WriteLine("#### Map Group " + mapGroupIndex++);
            writer.WriteLine();
            writer.WriteLine($"This group is centered on the Value Set {valueSetRow[keyColumn]!.Resource.Name} from {valueSetRow[keyColumn]!.DC.Key} (column {keyColumn}).");
            writer.WriteLine("All codes from this value set are listed while other value sets only show contents that have relationships with those codes.");
            writer.WriteLine();

            // write the table header
            for (int col = 0; col < _definitions.Length; col++)
            {
                if (col > 0)
                {
                    writer.Write("| Relationship ");
                }

                ValueSetGraphCell? cell = valueSetRow[col];

                if (cell == null)
                {
                    writer.Write("| *No Map* ");
                    continue;
                }

                if (col == keyColumn)
                {
                    writer.Write($"| {cell.DC.Key} {cell.Resource.Name.ForMdTable()}");
                }
                else
                {
                    writer.Write($"| [{cell.DC.Key} {cell.Resource.Name.ForMdTable()}]({vsRootUrlsByVersion[col]}/{getVsFilename(cell.Resource.Name.ToPascalCase(), includeRelativeDir: false)})");
                }
            }
            writer.WriteLine();
            writeTableColumns(writer, "---", (_definitions.Length * 2) - 1, appendNewline: true);

            // build a code map graph
            ValueSetComponentGraph codeMapGraph = new()
            {
                SourceRow = valueSetRow,
            };

            HashSet<string>[] codesPerVs = _definitions.Select(_ => new HashSet<string>()).ToArray();

            // iterate over the components in the key value set
            foreach (ValueSet.ContainsComponent component in valueSetRow[keyColumn]!.Resource.cgGetFlatContains())
            {
                bool hasMap = false;

                // project this component
                foreach (ValueSetComponentGraphCell?[] componentRow in codeMapGraph.Project(valueSetRow[keyColumn]!, component))
                {
                    hasMap = true;
                    int column = -1;

                    // traverse columns
                    foreach (ValueSetComponentGraphCell? cell in componentRow)
                    {
                        column++;

                        if (cell == null)
                        {
                            writer.Write("| | ");
                            continue;
                        }

                        codesPerVs[column].Add(cell.Component.cgKey());

                        if (column == keyColumn)
                        {
                            writer.Write($"| **`{cell.Component.Code.ForMdTable()}`**");
                        }
                        else
                        {
                            writer.Write($"| `{cell.Component.Code.ForMdTable()}`");
                        }

                        if (column == (componentRow.Length - 1))
                        {
                            continue;
                        }

                        if (cell.RightEdge == null)
                        {
                            writer.Write("| ");
                        }
                        else
                        {
                            if ((cell.RightEdge?.UpTarget?.Relationship == ConceptMap.ConceptMapRelationship.Equivalent) &&
                                (cell.RightEdge?.DownTarget?.Relationship == ConceptMap.ConceptMapRelationship.Equivalent))
                            {
                                writer.Write("| == ");
                            }
                            else if ((cell.RightEdge?.UpTarget?.Relationship == ConceptMap.ConceptMapRelationship.SourceIsNarrowerThanTarget) &&
                                     (cell.RightEdge?.DownTarget?.Relationship == ConceptMap.ConceptMapRelationship.SourceIsBroaderThanTarget))
                            {
                                writer.Write("| > ");
                            }
                            else if ((cell.RightEdge?.UpTarget?.Relationship == ConceptMap.ConceptMapRelationship.SourceIsBroaderThanTarget) &&
                                     (cell.RightEdge?.DownTarget?.Relationship == ConceptMap.ConceptMapRelationship.SourceIsNarrowerThanTarget))
                            {
                                writer.Write("| < ");
                            }
                            else
                            {
                                // write mapping notes
                                writer.Write(
                                    $"| → {cell.RightEdge?.UpTarget?.Relationship} → " +
                                    $"<hr/>" +
                                    $"← {cell.RightEdge?.DownTarget?.Relationship} ← ");
                            }
                        }
                    }

                    writer.WriteLine();
                }

                // check for unmapped concepts
                if (!hasMap)
                {
                    for (int i = 0; i < valueSetRow.Length; i++)
                    {
                        if (i == keyColumn)
                        {
                            writer.Write($"| **`{component.Code.ForMdTable()}`**");
                        }
                        else
                        {
                            writer.Write("| ");
                        }
                    }
                    writer.WriteLine();
                }
            }

            // check for unused codes in value sets
            for (int i = 0; i < valueSetRow.Length; i++)
            {
                if (i != 0)
                {
                    writer.Write("| ");
                }

                if (valueSetRow[i] == null)
                {
                    writer.Write("| ");
                }
                else
                {
                    writer.Write($"| *{codesPerVs[i].Count} of {valueSetRow[i]!.UniqueCodeCount} codes used* ");
                }
            }
            writer.WriteLine();

            writer.WriteLine();
        }

        return;
            
        //bool isRelated(ConceptDomainRelationshipCodes? relationship) =>
        //    (relationship == ConceptDomainRelationshipCodes.Equivalent) ||
        //    (relationship == ConceptDomainRelationshipCodes.SourceIsNarrowerThanTarget) ||
        //    (relationship == ConceptDomainRelationshipCodes.SourceIsBroaderThanTarget) ||
        //    (relationship == ConceptDomainRelationshipCodes.Related);
    }

    private void writeTableColumns(
        ExportStreamWriter writer,
        string value,
        int count,
        bool appendNewline = true,
        int? valueOnlyInColumn = null)
    {
        if (valueOnlyInColumn == null)
        {
            for (int i = 0; i < count; i++)
            {
                if (appendNewline && (i == count - 1))
                {
                    writer.WriteLine(" | " + value);
                }
                else if (i == 0)
                {
                    writer.Write("| " + value);
                }
                else
                {
                    writer.Write(" | " + value);
                }
            }

            return;
        }


        for (int i = 0; i < count; i++)
        {
            if (appendNewline && (i == count - 1))
            {
                if (valueOnlyInColumn == i)
                {
                    writer.WriteLine(" | " + value);
                }
                else
                {
                    writer.WriteLine(" | ");
                }
            }
            else if (i == 0)
            {
                if (valueOnlyInColumn == i)
                {
                    writer.Write("| " + value);
                }
                else
                {
                    writer.Write("| ");
                }
            }
            else
            {
                if (valueOnlyInColumn == i)
                {
                    writer.Write(" | " + value);
                }
                else
                {
                    writer.Write(" | ");
                }
            }
        }
    }

    private string getSdFilename(string sourceName, FhirArtifactClassEnum artifactClass, bool includeRelativeDir = true)
    {
        string artifactPascal = artifactClass switch
        {
            FhirArtifactClassEnum.PrimitiveType => "PrimitiveTypes",
            FhirArtifactClassEnum.ComplexType => "ComplexTypes",
            FhirArtifactClassEnum.Resource => "Resources",
            _ => throw new InvalidOperationException("Invalid artifact class."),
        };

        return includeRelativeDir
            ? $"{artifactPascal}/{FhirSanitizationUtils.SanitizeForProperty(sourceName, convertToConvention: FhirNameConventionExtensions.NamingConvention.PascalCase)}.md"
            : FhirSanitizationUtils.SanitizeForProperty(sourceName, convertToConvention: FhirNameConventionExtensions.NamingConvention.PascalCase) + ".md";
    }

    private (string overviewTo, string artifactTo, string overviewFrom, string artifactFrom) getConceptMapMdLinks(
        StructureDefinitionGraphCell cell,
        ComparisonDirection direction,
        FhirArtifactClassEnum artifactClass)
    {
        StructureDefinitionGraphCell? targetCell = direction == ComparisonDirection.Up ? cell.RightCell : cell.LeftCell;

        if (targetCell == null)
        {
            return ("*no map*", "*no map*", "*no map*", "*no map*");
        }

        string overviewRoot = artifactClass == FhirArtifactClassEnum.Resource ? "resources" : "types";

        StructureDefinitionGraphEdge? edge = direction == ComparisonDirection.Up ? cell.RightEdge : cell.LeftEdge;
        ConceptMap? overviewMapTo = direction == ComparisonDirection.Up ? edge?.OverviewUp : edge?.OverviewDown;
        ConceptMap? mapTo = direction == ComparisonDirection.Up ? edge?.Up : edge?.Down;
        ConceptMap? overviewMapFrom = direction == ComparisonDirection.Up ? edge?.OverviewDown : edge?.OverviewUp;
        ConceptMap? mapFrom = direction == ComparisonDirection.Up ? edge?.Down : edge?.Up;

        return (
            getOverviewLink(overviewMapTo, targetCell),
            getArtifactLink(mapTo, targetCell),
            getOverviewLink(overviewMapFrom, targetCell),
            getArtifactLink(mapFrom, targetCell));

        string getOverviewLink(ConceptMap? map, StructureDefinitionGraphCell? target)
        {
            if ((map == null) || (target == null))
            {
                return "*no map*";
            }

            return $"[{map.Name.ForMdTable()}]" +
                $"(/input/{overviewRoot}_v2/ConceptMap-{map.Name}.json)";
        }

        string getArtifactLink(ConceptMap? map, StructureDefinitionGraphCell? target)
        {
            if ((map == null) || (target == null))
            {
                return "*no map*";
            }

            return $"[{map.Name.ForMdTable()}]" +
                $"(/input/{overviewRoot}_v2/{cell.DC.FhirSequence.ToRLiteral()}to{target.DC.FhirSequence.ToRLiteral()}/ConceptMap-{map.Name}.json)";
        }
    }

    private string getVsFilename(string sourceVsName, bool includeRelativeDir = true)
    {
        return includeRelativeDir
            ? $"ValueSets/{FhirSanitizationUtils.SanitizeForProperty(sourceVsName, convertToConvention: FhirNameConventionExtensions.NamingConvention.PascalCase)}.md"
            : FhirSanitizationUtils.SanitizeForProperty(sourceVsName, convertToConvention: FhirNameConventionExtensions.NamingConvention.PascalCase) + ".md";

        //return includeRelativeDir
        //    ? $"ValueSets/{sourceVsName}_{cd.TargetDefinition.FhirSequence.ToRLiteral()}_{cd.Target?.Name.ToPascalCase()}"
        //    : $"{sourceVsName}_{cd.TargetDefinition.FhirSequence.ToRLiteral()}_{cd.Target?.Name.ToPascalCase()}";
    }

    private (string to, string from) getConceptMapMdLinks(ValueSetGraphCell cell, ComparisonDirection direction)
    {
        ValueSetGraphCell? targetCell = direction == ComparisonDirection.Up ? cell.RightCell : cell.LeftCell;

        if (targetCell == null)
        {
            return ("*no map*", "*no map*");
        }

        ValueSetGraphEdge? edge = direction == ComparisonDirection.Up ? cell.RightEdge : cell.LeftEdge;
        ConceptMap? mapTo = direction == ComparisonDirection.Up ? edge?.Up : edge?.Down;
        ConceptMap? mapFrom = direction == ComparisonDirection.Up ? edge?.Down : edge?.Up;

        return (getLink(mapTo, targetCell), getLink(mapFrom, targetCell));

        //if (direction == ComparisonDirection.Up)
        //{
        //    if ((cell.RightCell == null) ||
        //        (cell.RightEdge?.Up == null) ||
        //        (cell.RightEdge?.Down == null))
        //    {
        //        return ("*no map*", "*no map*");
        //    }

        //    return (
        //        $"[{cell.RightEdge.Up.Name.ForMdTable()}]" +
        //        $"(/input/codes_v2/{cell.DC.FhirSequence.ToRLiteral()}to{cell.RightCell.DC.FhirSequence.ToRLiteral()}/ConceptMap-{cell.RightEdge.Up.Name}.json)",
        //        $"[{cell.RightEdge.Down.Name.ForMdTable()}]" +
        //        $"(/input/codes_v2/{cell.RightCell.DC.FhirSequence.ToRLiteral()}to{cell.DC.FhirSequence.ToRLiteral()}/ConceptMap-{cell.RightEdge.Down.Name}.json)");
        //}

        //if ((cell.LeftCell == null) ||
        //    (cell.LeftEdge?.Up == null) ||
        //    (cell.LeftEdge?.Down == null))
        //{
        //    return ("*no map*", "*no map*");
        //}

        //return (
        //    $"[{cell.LeftEdge.Down.Name.ForMdTable()}]" +
        //    $"(/input/codes_v2/{cell.DC.FhirSequence.ToRLiteral()}to{cell.LeftCell.DC.FhirSequence.ToRLiteral()}/ConceptMap-{cell.LeftEdge.Down.Name}.json)",
        //    $"[{cell.LeftEdge.Up.Name.ForMdTable()}]" +
        //    $"(/input/codes_v2/{cell.LeftCell.DC.FhirSequence.ToRLiteral()}to{cell.DC.FhirSequence.ToRLiteral()}/ConceptMap-{cell.LeftEdge.Up.Name}.json)");

        string getLink(ConceptMap? map, ValueSetGraphCell? target)
        {
            if ((map == null) || (target == null))
            {
                return "*no map*";
            }

            return $"[{map.Name.ForMdTable()}]" +
                $"(/input/codes_v2/{cell.DC.FhirSequence.ToRLiteral()}to{target.DC.FhirSequence.ToRLiteral()}/ConceptMap-{map.Name}.json)";
        }
    }


    private ExportStreamWriter createMarkdownWriter(string filename, bool writeGenerationHeader = true, bool includeGenerationTime = false)
    {
        for (int i = 0; i < 3; i++)
        {
            try
            {
                ExportStreamWriter writer = new(filename);

                if (writeGenerationHeader)
                {
                    writer.WriteLine($"Comparison of {string.Join(", ", _definitions.Select(dc => dc.Key))}");

                    if (includeGenerationTime)
                    {
                        writer.WriteLine($"Generated at {DateTime.Now.ToString("F")}");
                    }

                    writer.WriteLine();
                }

                return writer;
            }
            catch (IOException)
            {
                // wait a bit and try again
                Thread.Sleep(1000);
            }
        }

        throw new IOException("Failed to create file after 3 attempts.");
    }

    private ExportStreamWriter createHtmlWriter(string filename, bool writeGenerationHeader = true, bool includeGenerationTime = false)
    {
        for (int i = 0; i < 3; i++)
        {
            try
            {
                ExportStreamWriter writer = new(filename);

                if (writeGenerationHeader)
                {
                    writer.WriteLine($"<h2>Comparison of {string.Join(", ", _definitions.Select(dc => dc.Key))}</h2>");

                    if (includeGenerationTime)
                    {
                        writer.WriteLine($"Generated at {DateTime.Now.ToString("F")}");
                    }

                    writer.WriteLine();
                }

                return writer;
            }
            catch (IOException)
            {
                // wait a bit and try again
                Thread.Sleep(1000);
            }
        }

        throw new IOException("Failed to create file after 3 attempts.");
    }

    /// <summary>
    /// Applies the relationship between existing and change concept domain relationship codes.
    /// </summary>
    /// <param name="existing">The existing concept domain relationship code.</param>
    /// <param name="change">The change concept domain relationship code.</param>
    /// <returns>The resulting concept domain relationship code.</returns>
    private ConceptDomainRelationshipCodes applyRelationship(ConceptDomainRelationshipCodes? existing, ConceptDomainRelationshipCodes? change) => existing switch
    {
        ConceptDomainRelationshipCodes.Unknown => change ?? ConceptDomainRelationshipCodes.Unknown,
        ConceptDomainRelationshipCodes.Equivalent => cdrCodeIsBroader(change)
            ? ConceptDomainRelationshipCodes.SourceIsBroaderThanTarget
            : cdrCodeIsNarrower(change)
            ? ConceptDomainRelationshipCodes.SourceIsNarrowerThanTarget
            : change ?? ConceptDomainRelationshipCodes.Equivalent,
        ConceptDomainRelationshipCodes.SourceIsNew => cdrCodeIsBroader(change)
            ? ConceptDomainRelationshipCodes.SourceIsBroaderThanTarget
            : ConceptDomainRelationshipCodes.Related,
        ConceptDomainRelationshipCodes.SourceIsDeprecated => cdrCodeIsNarrower(change)
            ? ConceptDomainRelationshipCodes.SourceIsNarrowerThanTarget
            : ConceptDomainRelationshipCodes.Related,
        ConceptDomainRelationshipCodes.NotMapped => cdrCodeIsBroader(change)
            ? ConceptDomainRelationshipCodes.SourceIsBroaderThanTarget
            : ConceptDomainRelationshipCodes.Related,
        ConceptDomainRelationshipCodes.SourceIsNarrowerThanTarget => cdrCodeIsNarrower(change)
            ? ConceptDomainRelationshipCodes.SourceIsNarrowerThanTarget
            : ConceptDomainRelationshipCodes.Related,
        ConceptDomainRelationshipCodes.SourceIsBroaderThanTarget => cdrCodeIsBroader(change)
            ? ConceptDomainRelationshipCodes.SourceIsBroaderThanTarget
            : ConceptDomainRelationshipCodes.Related,
        ConceptDomainRelationshipCodes.Related => (change == ConceptDomainRelationshipCodes.NotRelated)
            ? ConceptDomainRelationshipCodes.NotRelated
            : ConceptDomainRelationshipCodes.Related,
        ConceptDomainRelationshipCodes.NotRelated => change ?? ConceptDomainRelationshipCodes.NotRelated,
        _ => change ?? existing ?? ConceptDomainRelationshipCodes.Unknown,
    };

    /// <summary>
    /// Determines if the given ConceptDomainRelationshipCodes is narrower.
    /// </summary>
    /// <param name="cdr">The ConceptDomainRelationshipCodes to check.</param>
    /// <returns>True if the ConceptDomainRelationshipCodes is narrower; otherwise, false.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool cdrCodeIsNarrower(ConceptDomainRelationshipCodes? cdr) =>
        cdr == ConceptDomainRelationshipCodes.SourceIsNarrowerThanTarget ||
        cdr == ConceptDomainRelationshipCodes.SourceIsDeprecated;

    /// <summary>
    /// Determines if the given ConceptDomainRelationshipCodes is broader.
    /// </summary>
    /// <param name="cdr">The ConceptDomainRelationshipCodes to check.</param>
    /// <returns>True if the ConceptDomainRelationshipCodes is broader; otherwise, false.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool cdrCodeIsBroader(ConceptDomainRelationshipCodes? cdr) =>
        cdr == ConceptDomainRelationshipCodes.SourceIsBroaderThanTarget ||
        cdr == ConceptDomainRelationshipCodes.SourceIsNew ||
        cdr == ConceptDomainRelationshipCodes.NotMapped;

}
