// <copyright file="XVerProcessor.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

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
using Hl7.Fhir.Model.CdsHooks;
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
using static System.Net.Mime.MediaTypeNames;
using CMR = Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship;
using Tasks = System.Threading.Tasks;

namespace Microsoft.Health.Fhir.Comparison.XVer;

/// <summary>
/// Provides logging message templates for cross-version processing events.
/// </summary>
internal static partial class XVerProcessorLogMessages
{
    /// <summary>
    /// Logs a warning when cross-version maps could not be loaded for a given key.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="cvMapKey">The cross-version map key.</param>
    [LoggerMessage(Level = LogLevel.Warning, Message = "Failed to load requested cross-version maps for {cvMapKey}! Processing will be only algorithmic!")]
    internal static partial void LogMapsNotFound(this ILogger logger, string cvMapKey);

    /// <summary>
    /// Logs a warning when a ValueSet could not be expanded for comparison.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="url">The ValueSet URL.</param>
    /// <param name="details">Additional details about the failure.</param>
    [LoggerMessage(Level = LogLevel.Warning, Message = "Failed to expand ValueSet {url} for comparison: {details}")]
    internal static partial void LogValueSetNotExpanded(this ILogger logger, string url, string? details);

    /// <summary>
    /// Logs a warning when a ValueSet could not be retrieved from a data collection.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="url">The ValueSet URL.</param>
    /// <param name="dcKey">The data collection key.</param>
    [LoggerMessage(Level = LogLevel.Warning, Message = "Failed to retrieve ValueSet {url} from {dcKey}")]
    internal static partial void LogValueSetNotFound(this ILogger logger, string url, string dcKey);
}

/// <summary>
/// Extension methods for cross-version processing.
/// </summary>
internal static class XVerExtensions
{
    /// <summary>
    /// Escapes special characters for Markdown table output.
    /// </summary>
    /// <param name="value">The string value to escape.</param>
    /// <returns>The escaped string for Markdown tables.</returns>

    internal static string ForMdTable(this string? value) => string.IsNullOrEmpty(value) ? string.Empty : value.Replace("|", "\\|").Replace("\n", "<br/>").Replace("\r", "<br/>");

    /// <summary>
    /// Generates a comparison key for a ValueSet and graph identifier.
    /// </summary>
    /// <param name="vs">The ValueSet.</param>
    /// <param name="graphId">The graph identifier.</param>
    /// <returns>The comparison key string.</returns>
    internal static string ComparisonKey(this ValueSet vs, string graphId) => graphId + "_" + vs.Name.ToPascalCase();
}

/// <summary>
/// Processes cross-version FHIR package comparisons, generates artifacts, and manages database operations.
/// </summary>
public class XVerProcessor
{
    private string _crossDefinitionVersion = "0.7.0";

    /// <summary>
    /// The directions for comparison (Up and Down).
    /// </summary>
    internal static readonly ComparisonDirection[] _directions = [ComparisonDirection.Up, ComparisonDirection.Down];

    /// <summary>
    /// Set of ValueSet URLs to exclude from processing.
    /// </summary>
    internal static readonly HashSet<string> _exclusionSet =
    [
        "http://hl7.org/fhir/ValueSet/ucum-units",
        "http://hl7.org/fhir/ValueSet/all-languages",
        "http://tools.ietf.org/html/bcp47",             // DSTU2 version of all-languages
        "http://hl7.org/fhir/ValueSet/mimetypes",
        //"http://hl7.org/fhir/ValueSet/use-context",
        //"http://hl7.org/fhir/ValueSet/jurisdiction",
    ];

    /// <summary>
    /// Set of codes considered as "escape valve" codes (e.g., OTHER, UNKNOWN).
    /// </summary>
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

    /// <summary>
    /// Initializes a new instance of the <see cref="XVerProcessor"/> class using configuration.
    /// </summary>
    /// <param name="config">The cross-version configuration.</param>
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

    /// <summary>
    /// Initializes a new instance of the <see cref="XVerProcessor"/> class using an existing database.
    /// </summary>
    /// <param name="db">The comparison database.</param>
    /// <param name="outputDirectory">The output directory for artifacts.</param>
    /// <param name="loggerFactory">The logger factory.</param>
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
    }    /// <summary>
    /// Loads FHIR package definition collections based on configuration settings.
    /// </summary>
    /// <remarks>
    /// This method iterates through the configured comparison packages, validates they are FHIR Core packages,
    /// creates package loaders, loads the packages, and populates the internal definitions array and indexes.
    /// </remarks>
    /// <exception cref="Exception">Thrown when a package is not a FHIR Core package or when package loading fails.</exception>
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

    /// <summary>
    /// Processes a command to perform cross-version operations such as database creation, comparison, or documentation generation.
    /// </summary>
    /// <param name="command">The command to process.</param>
    public void ProcessCommand(string? command)
    {
        switch (command)
        {
            case "create-db":
                LoadDatabase(true, true);
                LoadFhirCrossVersionMaps(preferV1Maps: true);
                break;

            case "compare":
                LoadDatabase(_config.ReloadDatabase, _config.XverAllowComparisonUpdates);
                CompareInDatabase(allowUpdates: _config.XverAllowComparisonUpdates);
                break;

            case "compare-vs":
                LoadDatabase(_config.ReloadDatabase, _config.XverAllowComparisonUpdates, FhirArtifactClassEnum.ValueSet);
                CompareInDatabase(FhirArtifactClassEnum.ValueSet, allowUpdates: _config.XverAllowComparisonUpdates);
                break;

            case "compare-sd":
                LoadDatabase(_config.ReloadDatabase, false, FhirArtifactClassEnum.Resource);
                CompareInDatabase(FhirArtifactClassEnum.Resource, allowUpdates: _config.XverAllowComparisonUpdates);
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

    /// <summary>
    /// Loads or creates the comparison database, optionally filtering by artifact type.
    /// </summary>
    /// <param name="forceCreate">Whether to force creation of the database.</param>
    /// <param name="allowSourceCopy">Whether to allow copying from a source database.</param>
    /// <param name="artifactFilter">Optional artifact type filter.</param>
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

    /// <summary>
    /// Runs the comparison process in the loaded database for the specified artifact type.
    /// </summary>
    /// <param name="artifactFilter">Optional artifact type filter.</param>
    public void CompareInDatabase(FhirArtifactClassEnum? artifactFilter = null, bool allowUpdates = true)
    {
        if (_db == null)
        {
            throw new Exception("Comparison cannot run without a loaded database!");
        }

        FhirDbComparer dbComparer = new(_db, _config.LogFactory);
        dbComparer.Compare(artifactFilter, _config.ComparisonPairFilterKeys, allowUpdates);
    }

    /// <summary>
    /// Represents support information for a FHIR package during cross-version processing.
    /// </summary>
    private record class PackageXverSupport
    {
        /// <summary>
        /// Gets the package index.
        /// </summary>
        public required int PackageIndex { get; init; }

        /// <summary>
        /// Gets the FHIR package.
        /// </summary>
        public required DbFhirPackage Package { get; init; }

        /// <summary>
        /// Gets the set of basic element paths.
        /// </summary>
        public HashSet<string> BasicElements { get; init; } = [];

        /// <summary>
        /// Gets the set of allowed extension types.
        /// </summary>
        public HashSet<string> AllowedExtensionTypes { get; init; } = [];

        /// <summary>
        /// Gets the set of allowed canonical target types.
        /// </summary>
        public HashSet<string> AllowedCanonicalTypes { get; init; } = [];

        /// <summary>
        /// Gets or sets the core definition collection.
        /// </summary>
        public DefinitionCollection? CoreDC { get; set; } = null;

        /// <summary>
        /// Gets or sets the snapshot generator.
        /// </summary>
        public Hl7.Fhir.Specification.Snapshot.SnapshotGenerator? SnapshotGenerator { get; set; } = null;
    }

    /// <summary>
    /// Loads the definitions and initializes the comparison cache.
    /// </summary>
    /// <remarks>
    /// This is only used to convert origin maps into the database.
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


    /// <summary>
    /// Enumerates the possible outcomes for cross-version element mapping in FHIR processing.
    /// </summary>
    private enum XverOutcomeCodes
    {
        /// <summary>
        /// The element is used with the same name in the target version.
        /// </summary>
        UseElementSameName,
        /// <summary>
        /// The element is used but has been renamed in the target version.
        /// </summary>
        UseElementRenamed,
        /// <summary>
        /// The element is represented as an extension in the target version.
        /// </summary>
        UseExtension,
        /// <summary>
        /// The element is represented as an extension inherited from an ancestor.
        /// </summary>
        UseExtensionFromAncestor,
        /// <summary>
        /// The element is mapped to a basic element in the target version.
        /// </summary>
        UseBasicElement,
        /// <summary>
        /// The element is mapped to one of several possible elements in the target version.
        /// </summary>
        UseOneOfElements,
    }

    /// <summary>
    /// Represents the outcome of a cross-version element mapping operation.
    /// </summary>
    private record class XverOutcome
    {
        /// <summary>
        /// Gets the key of the source FHIR package.
        /// </summary>
        public required int SourcePackageKey { get; init; }

        /// <summary>
        /// Gets the key of the source structure
        /// </summary>
        public required int SourceStructureKey { get; init; }

        /// <summary>
        /// Gets the name of the source structure.
        /// </summary>
        public required string SourceStructureName { get; init; }

        /// <summary>
        /// Gets the identifier of the source element.
        /// </summary>
        public required string SourceElementId { get; init; }

        /// <summary>
        /// Gets the field order of the source element within the structure.
        /// </summary>
        public required int SourceElementFieldOrder { get; init; }

        /// <summary>
        /// Gets the key of the target FHIR package.
        /// </summary>
        public required int TargetPackageKey { get; init; }

        /// <summary>
        /// Gets the outcome code describing the mapping result.
        /// </summary>
        public required XverOutcomeCodes OutcomeCode { get; init; }

        /// <summary>
        /// Gets the identifier of the target element, if applicable.
        /// </summary>
        public required string? TargetElementId { get; init; }

        /// <summary>
        /// Gets the URL of the target extension, if the mapping resulted in an extension.
        /// </summary>
        public required string? TargetExtensionUrl { get; init; }

        /// <summary>
        /// Gets the URL of a replacement extension, if the mapping resulted in a substitution.
        /// </summary>
        public required string? ReplacementExtensionUrl { get; init; }
    }

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

            // create a loader because these are all different FHIR core versions
            using CodeGen.Loader.PackageLoader loader = new(_config, new()
            {
                JsonModel = CodeGen.Loader.LoaderOptions.JsonDeserializationModel.SystemTextJson,
            });

            DefinitionCollection coreDc = loader.LoadPackages([packageDirective]).Result
                ?? throw new Exception($"Could not load package: {packageDirective}");

            // add required extra extensions to our defintions for use later in snapshot generation
            addRequiredExtensionDefinitions(coreDc);

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

            // create the intersection of possible canonical types and resources in this package
            foreach (string canonicalType in FhirTypeMappings.CanonicalTargets)
            {
                if (coreDc.ResourcesByName.ContainsKey(canonicalType))
                {
                    packageSupport.AllowedCanonicalTypes.Add(canonicalType);
                    packageSupport.AllowedCanonicalTypes.Add("http://hl7.org/fhir/StructureDefinition/" + canonicalType);
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

            Dictionary<string, DbGraphSd> graphsForStructures = [];

            // iterate over the structures to setup baseline necessary mappings
            foreach (DbStructureDefinition sd in DbStructureDefinition.SelectList(_db!.DbConnection, FhirPackageKey: packages[focusPackageIndex].Key))
            {
                if (!xverOutcomes.ContainsKey((focusPackageIndex, sd.Name)))
                {
                    xverOutcomes[(focusPackageIndex, sd.Name)] = [];
                    for (int i = 0; i < packageSupports.Count; i++)
                    {
                        xverOutcomes[(focusPackageIndex, sd.Name)].Add([]);
                    }
                }

                // build a graph for this structure
                DbGraphSd sdGraph = new()
                {
                    DB = _db!.DbConnection,
                    Packages = packageSupports.Select(ps => ps.Package).ToList(),
                    KeySd = sd,
                };

                // ensure there is an initial structure projection
                sdGraph.BuildProjection();

                // add to our database
                graphsForStructures.Add(sd.Id, sdGraph);
            }

            Dictionary<(int sourceSdKey, int sourceElementKey, int targetPackageId), (StructureDefinition, DbExtensionSubstitution?)> xverExtensions = [];
            buildXverExtensions(packageSupports, focusPackageIndex, xverValueSets, xverExtensions, xverOutcomes, graphsForStructures, FhirArtifactClassEnum.ComplexType);
            buildXverExtensions(packageSupports, focusPackageIndex, xverValueSets, xverExtensions, xverOutcomes, graphsForStructures, FhirArtifactClassEnum.Resource);

            writeXverExtensions(packageSupports, focusPackageIndex, xverExtensions, fhirDir);

            Dictionary<(int sourceSdKey, int targetSdKey, int targetPackageId), StructureDefinition> xverProfiles = [];
            buildXverProfiles(packageSupports, focusPackageIndex, xverValueSets, xverExtensions, xverOutcomes, xverProfiles, graphsForStructures);
            writeXverProfiles(packageSupports, focusPackageIndex, xverProfiles, fhirDir);

            List<XverPackageIndexInfo> focusedIndexInfos = writeXverSinglePackageSupportFiles(packageSupports, focusPackageIndex, xverValueSets, xverExtensions, fhirDir);
            allIndexInfos.AddRange(focusedIndexInfos);
        }

        // write our combined package support files
        writeXverValidationPackageSupportFiles(packageSupports, allIndexInfos, fhirDir);

        // write all of our outcome lists
        writeXverOutcomes(packageSupports, xverOutcomes, outputDir);

        // make the make package tgz files
        if (_config.XverGenerateNpms)
        {
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
            Console.WriteLine($"Failed to create tgz {outputTgzFile} from source {sourceDirectory} ({ex.Message})");
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
            string dir = Path.Combine(fhirDir, packageSupport.Package.ShortName);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

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
                    $"hl7.fhir.uv.xver-{sourcePackage.Package.ShortName.ToLowerInvariant()}.{packageSupport.Package.ShortName.ToLowerInvariant()}",
                    _crossDefinitionVersion));
            }

            // get the list of index informations that *target* this version
            List<XverPackageIndexInfo> packageIndexInfos = allPackageIndexInfos.Where(ii => ii.TargetPackageSupport.Package.Key == packageSupport.Package.Key).ToList();

            string packageId = $"hl7.fhir.uv.xver.{packageSupport.Package.ShortName.ToLowerInvariant()}";

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
                            "hl7.terminology.{{{packageSupport.Package.ShortName.ToLowerInvariant()}}}" : "6.3.0",
                            "hl7.fhir.uv.extensions.{{{packageSupport.Package.ShortName.ToLowerInvariant()}}}" : "5.2.0",
                            "hl7.fhir.uv.tools.{{{packageSupport.Package.ShortName.ToLowerInvariant()}}}" : "current"
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
        Dictionary<(int sourceSdKey, int sourceElementKey, int targetPackageId), (StructureDefinition, DbExtensionSubstitution?)> xverExtensions,
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

            string packageId = $"hl7.fhir.uv.xver-{sourcePackage.ShortName.ToLowerInvariant()}.{targetSupport.Package.ShortName.ToLowerInvariant()}";

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
                File.WriteAllText(Path.Combine(fhirDir, $"{sourcePackage.ShortName}-for-{targetSupport.Package.ShortName}", "package", filename), igJson);
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
                File.WriteAllText(Path.Combine(fhirDir, $"{sourcePackage.ShortName}-for-{targetSupport.Package.ShortName}", "package", filename), pmJson);
            }

            // build and write the .index.json file
            {
                string indexJson = getIndexJson(sourcePackage, targetSupport.Package, xverValueSets, xverExtensions, indexInfo);
                string filename = ".index.json";
                File.WriteAllText(Path.Combine(fhirDir, $"{sourcePackage.ShortName}-for-{targetSupport.Package.ShortName}", "package", filename), indexJson);
            }

            // build and write the package.json file
            {
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
                            "doc" : "doc"
                        },
                        "jurisdiction" : "http://unstats.un.org/unsd/methods/m49/m49.htm#001"
                    }
                    """;

                string filename = "package.json";
                File.WriteAllText(Path.Combine(fhirDir, $"{sourcePackage.ShortName}-for-{targetSupport.Package.ShortName}", "package", filename), packageJson);
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
        Dictionary<(int sourceSdKey, int sourceElementKey, int targetPackageId), (StructureDefinition, DbExtensionSubstitution?)> xverExtensions,
        XverPackageIndexInfo indexInfo)
    {
        // build the list of structures we are defining
        foreach (((int _, int sourceElementKey, int targetPackageId), (StructureDefinition sd, DbExtensionSubstitution? extensionSubstitution)) in xverExtensions)
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
        Dictionary<(int sourceSdKey, int sourceElementKey, int targetPackageId), (StructureDefinition, DbExtensionSubstitution?)> xverExtensions,
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
        foreach (((int _, int _, int targetPackageId), (StructureDefinition sd, DbExtensionSubstitution? extensionSubstitution)) in xverExtensions)
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
        Dictionary<(int sourceSdKey, int sourceElementKey, int targetPackageId), (StructureDefinition, DbExtensionSubstitution?)> xverExtensions,
        XverPackageIndexInfo indexInfo)
    {
        // build the list of structures we are defining
        foreach (((int _, int _, int targetPackageId), (StructureDefinition sd, DbExtensionSubstitution? extensionSubstitution)) in xverExtensions)
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
                "packageId" : "hl7.terminology.{{{package.ShortName.ToLowerInvariant()}}}",
                "version" : "6.3.0"
              },
              {
                "id" : "hl7_fhir_uv_extensions",
                "uri" : "http://hl7.org/fhir/extensions/ImplementationGuide/hl7.fhir.uv.extensions",
                "packageId" : "hl7.fhir.uv.extensions.{{{package.ShortName.ToLowerInvariant()}}}",
                "version" : "5.2.0"
              },
              {
                "id" : "hl7_fhir_uv_tools",
                "uri" : "http://hl7.org/fhir/tools/ImplementationGuide/hl7.fhir.uv.tools",
                "packageId" : "hl7.fhir.uv.tools.{{{package.ShortName.ToLowerInvariant()}}}",
                "version" : "current"
              }{{{additionalDependencies}}}
              ]{{{resources}}}
            }
            """;

        return igJson;
    }


    private void writeXverOutcomes(
        List<PackageXverSupport> packageSupports,
        Dictionary<(int, string), List<List<XverOutcome>>> xverOutcomes,
        string outputDir)
    {
        HashSet<string> createdDirs = [];

        if (!Directory.Exists(outputDir))
        {
            Directory.CreateDirectory(outputDir);
        }

        string fhirDir = Path.Combine(outputDir, "fhir");
        if (!Directory.Exists (fhirDir))
        {
            Directory.CreateDirectory (fhirDir);
        }

        string docsDir = Path.Combine(outputDir, "docs");
        if (!Directory.Exists (docsDir))
        {
            Directory.CreateDirectory (docsDir);
        }

        // iterate over each structure in each source package
        foreach (((int sourcePackageIndex, string sourceStructureName), List<List<XverOutcome>> structureOutcomesByTarget) in xverOutcomes)
        {
            string sourceStructureNameForFile = FhirSanitizationUtils.SanitizeForProperty(sourceStructureName, replacements: []);

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
                string htmlDir;
                string mdDir;
                if (createdDirs.Contains(packageFor))
                {
                    htmlDir = Path.Combine(fhirDir, packageFor, "package", "doc");
                    mdDir = Path.Combine(docsDir, packageFor);
                }
                else
                {
                    htmlDir = Path.Combine(fhirDir, $"{sourcePackage.ShortName}-for-{targetPackage.ShortName}");
                    if (!Directory.Exists(htmlDir))
                    {
                        Directory.CreateDirectory(htmlDir);
                    }

                    htmlDir = Path.Combine(htmlDir, "package");
                    if (!Directory.Exists(htmlDir))
                    {
                        Directory.CreateDirectory(htmlDir);
                    }

                    htmlDir = Path.Combine(htmlDir, "doc");
                    if (!Directory.Exists(htmlDir))
                    {
                        Directory.CreateDirectory(htmlDir);
                    }

                    mdDir = Path.Combine(docsDir, packageFor);
                    if (!Directory.Exists (mdDir))
                    {
                        Directory.CreateDirectory(mdDir);
                    }

                    createdDirs.Add(packageFor);
                }

                // create a filename for this structure's md file
                string mdFilename = $"Lookup-{sourcePackage.ShortName}-{sourceStructureNameForFile}-{targetPackage.ShortName}.md";
                string htmlFilename = $"Lookup-{sourcePackage.ShortName}-{sourceStructureNameForFile}-{targetPackage.ShortName}.html";

                // open our files
                using ExportStreamWriter mdWriter = createMarkdownWriter(Path.Combine(mdDir, mdFilename), false, false);
                using ExportStreamWriter htmlWriter = createHtmlWriter(Path.Combine(htmlDir, htmlFilename), false, false);

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
                    string target = outcome.ReplacementExtensionUrl ?? outcome.TargetElementId ?? outcome.TargetExtensionUrl ?? "-";
                    mdWriter.WriteLine($"| {outcome.SourceElementId} | {outcome.OutcomeCode} | {target} |");
                    htmlWriter.WriteLine($"<tr><td>{outcome.SourceElementId}</td><td>{outcome.OutcomeCode}</td><td>{target}</td></tr>");
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
            FhirPackageKey: sourcePackage.Key);

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

    private void writeXverExtensions(
        List<PackageXverSupport> packageSupports,
        int focusPackageIndex,
        Dictionary<(int sourceSdKey, int sourceElementKey, int targetPackageId), (StructureDefinition, DbExtensionSubstitution?)> xverExtensions,
        string fhirDir)
    {
        Dictionary<int, DbFhirPackage> packageDict = packageSupports.Select(ps => ps.Package).ToDictionary(p => p.Key);
        DbFhirPackage focusPackage = packageSupports[focusPackageIndex].Package;

        ILookup<int, SnapshotGenerator?> generatorsById = packageSupports.ToLookup(ps => ps.Package.Key, ps => ps.SnapshotGenerator);

        FhirJsonSerializer jsonSerializer = new FhirJsonSerializer(new SerializerSettings() { Pretty = true });

        // iterate over the structures
        foreach (((int _, int _, int targetPackageId), (StructureDefinition sd, DbExtensionSubstitution? extensionSubstitution)) in xverExtensions)
        {
            // if this extension is substituted out, we do not need to write it
            if (extensionSubstitution != null)
            {
                continue;
            }

            // there are unfixable errors in the snapshot generator right now, so we are adding a step to generate snapshots via IG publisher by default
            if (_config.XverGenerateSnapshots == true)
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
            File.WriteAllText(path, jsonSerializer.SerializeToString(sd));
        }
    }

    private void buildXverProfiles(
        List<PackageXverSupport> packageSupports,
        int sourcePackageIndex,
        Dictionary<(int sourceVsKey, int targetPackageId), ValueSet> xverValueSets,
        Dictionary<(int sourceSdKey, int sourceElementKey, int targetPackageId), (StructureDefinition extSd, DbExtensionSubstitution? extSub)> xverExtensions,
        Dictionary<(int, string), List<List<XverOutcome>>> xverOutcomes,
        Dictionary<(int sourceSdKey, int targetSdKey, int targetPackageId), StructureDefinition> xverProfiles,
        Dictionary<string, DbGraphSd> graphsForStructures)
    {
        DbFhirPackage sourcePackage = packageSupports[sourcePackageIndex].Package;

        // iterate across the packages
        foreach (PackageXverSupport targetPackageSupport in packageSupports)
        {
            DbFhirPackage targetPackage = targetPackageSupport.Package;

            if (targetPackage.Key == sourcePackage.Key)
            {
                continue; // skip the source package
            }

            ILookup<int, (StructureDefinition extSd, DbExtensionSubstitution? extSub)> extSdLookup = xverExtensions
                .Where(kvp => kvp.Key.targetPackageId == targetPackage.Key)
                .ToLookup(kvp => kvp.Key.sourceSdKey, kvp => kvp.Value);

            // iterate over the structure groups
            foreach (IGrouping<int, (StructureDefinition extSd, DbExtensionSubstitution? extSub)> sourceSdGroup in extSdLookup)
            {
                // resolve the source structure
                DbStructureDefinition? sourceSd = DbStructureDefinition.SelectSingle(
                    _db!.DbConnection,
                    FhirPackageKey: sourcePackage.Key,
                    Key: sourceSdGroup.Key);

                if (sourceSd == null)
                {
                    continue; // skip if the source structure is not found
                }

                HashSet<string> targetStructures = [];

                // create a dictionary that has the contexts for extensions based on this source structure
                Dictionary<string, List<(StructureDefinition extSd, DbExtensionSubstitution? extSub)>> contextDict = [];
                foreach ((StructureDefinition extSd, DbExtensionSubstitution? extSub) in sourceSdGroup)
                {
                    // if we have a substitution, use that
                    if (extSub != null)
                    {
                        if (!contextDict.TryGetValue(extSub.Context, out List<(StructureDefinition, DbExtensionSubstitution?)>? extList))
                        {
                            extList = [];
                            contextDict.Add(extSub.Context, extList);
                        }
                        extList.Add((extSd, extSub));

                        int dotIndex = extSub.Context.IndexOf('.');
                        string targetStructureName = dotIndex == -1
                            ? extSub.Context
                            : extSub.Context.Substring(0, dotIndex);
                        _ = targetStructures.Add(targetStructureName);

                        continue;
                    }

                    foreach (string context in extSd.Context.Select(c => c.Expression))
                    {
                        if (!contextDict.TryGetValue(context, out List<(StructureDefinition, DbExtensionSubstitution?)>? extList))
                        {
                            extList = [];
                            contextDict.Add(context, extList);
                        }
                        extList.Add((extSd, extSub));

                        int dotIndex = context.IndexOf('.');
                        string targetStructureName = dotIndex == -1
                            ? context
                            : context.Substring(0, dotIndex);
                        _ = targetStructures.Add(targetStructureName);
                    }
                }

                // iterate across each of the target structures
                foreach (string tsName in targetStructures)
                {
                    // try to resolve the target structure
                    DbStructureDefinition? targetSd = DbStructureDefinition.SelectSingle(
                        _db!.DbConnection,
                        FhirPackageKey: targetPackage.Key,
                        Name: tsName);

                    if (targetSd == null)
                    {
                        continue;
                    }

                    string profileId = $"pro-{targetPackage.ShortName}-{targetSd.Name}-for-{sourcePackage.ShortName}-{sourceSd.Name}";

                    // build our profile structure definition
                    StructureDefinition profileSd = new()
                    {
                        Id = profileId,
                        //Url = $"http://hl7.org/fhir/uv/xver/{sourcePackage.FhirVersionShort}/StructureDefinition/extension-{element.Path.Replace("[x]", string.Empty)}",
                        Url = $"http://hl7.org/fhir/{sourcePackage.FhirVersionShort}/StructureDefinition/profile-{profileId}",
                        Name = char.ToUpperInvariant(profileId[0]) + profileId[1..].Replace('-', '_').Replace('.', '_'),
                        Version = _crossDefinitionVersion,
                        FhirVersion = EnumUtility.ParseLiteral<FHIRVersion>(targetPackageSupport!.CoreDC!.FhirVersionLiteral) ?? FHIRVersion.N5_0_0,
                        DateElement = new FhirDateTime(DateTimeOffset.Now),
                        Title = $"Cross-version Profile to represent an {sourcePackage.ShortName} {sourceSd.Name} via an {targetPackage.ShortName} {targetSd.Name}",
                        Description = $"This cross-version profile describes how to represent the FHIR {sourcePackage.ShortName} {sourceSd.Name} ({sourceSd.VersionedUrl}) via a FHIR {targetPackage.ShortName} {targetSd.Name}.",
                        Status = PublicationStatus.Draft,
                        Experimental = false,
                        Kind = targetSd.ArtifactClass == FhirArtifactClassEnum.Resource
                            ? StructureDefinition.StructureDefinitionKind.Resource
                            : StructureDefinition.StructureDefinitionKind.ComplexType,
                        Abstract = false,
                        Type = tsName,
                        BaseDefinition = "http://hl7.org/fhir/StructureDefinition/" + tsName,
                        Derivation = StructureDefinition.TypeDerivationRule.Constraint,
                        Differential = new()
                        {
                            Element = [],
                        },
                    };

                    profileSd.Differential.Element.Add(new()
                    {
                        ElementId = tsName,
                        Path = tsName,
                        Short = profileSd.Title,
                        Min = 0,
                        Max = "*",
                        Base = new()
                        {
                            Path = tsName,
                            Min = 0,
                            Max = "*",
                        },
                    });

                    // get the elements in this structure, so we can build the profile elements
                    List<DbElement> tsElements = DbElement.SelectList(
                        _db!.DbConnection,
                        StructureKey: targetSd.Key,
                        orderByProperties: [nameof(DbElement.ResourceFieldOrder)]);

                    // iterate over the elements
                    foreach (DbElement element in tsElements)
                    {
                        // check if we have extensions that target this element
                        if (!contextDict.TryGetValue(element.Id, out List<(StructureDefinition, DbExtensionSubstitution?)>? extensions))
                        {
                            continue;
                        }

                        // create the extension element with slicing
                        profileSd.Differential.Element.Add(new()
                        {
                            ElementId = element.Id + ".extension",
                            Path = element.Path + ".extension",
                            Slicing = new()
                            {
                                Discriminator = [
                                    new() {
                                            Type = ElementDefinition.DiscriminatorType.Value,
                                            Path = "url",
                                        }
                                ],
                                Ordered = false,
                                Rules = ElementDefinition.SlicingRules.Open,
                            },
                            Min = 0,
                            Max = "*",
                            Base = new()
                            {
                                Path = "Element.extension",
                                Min = 0,
                                Max = "*",
                            },
                        });

                        // iterate over the extensions for this element
                        foreach ((StructureDefinition extSd, DbExtensionSubstitution? extSub) in extensions)
                        {
                            // add the extension slice
                            profileSd.Differential.Element.Add(new()
                            {
                                ElementId = element.Id + ".extension:" + extSd.Name,
                                Path = element.Path + ".extension",
                                SliceName = extSd.Name,
                                Short = extSd.Title,
                                Definition = extSd.Description,
                                Base = new()
                                {
                                    Path = "Element.extension",
                                    Min = 0,
                                    Max = "*",
                                },
                            });

                            // add the url slice
                            profileSd.Differential.Element.Add(new()
                            {
                                ElementId = element.Id + ".extension:" + extSd.Name + ".url",
                                Path = element.Path + ".extension.url",
                                Short = extSd.Title,
                                Definition = extSd.Description,
                                Base = new()
                                {
                                    Path = "Extension.url",
                                    Min = 0,
                                    Max = "*",
                                },
                                Fixed = extSub != null
                                    ? new FhirUri(extSub.ReplacementUrl)
                                    : new FhirUri(extSd.Url),
                            });
                        }
                    }

                    // add to our dictionary
                    xverProfiles.Add((sourceSd.Key, targetSd.Key, targetPackage.Key), profileSd);
                }
            }
        }
    }

    private void writeXverProfiles(
        List<PackageXverSupport> packageSupports,
        int focusPackageIndex,
        Dictionary<(int sourceSdKey, int targetSdKey, int targetPackageId), StructureDefinition> xverProfiles,
        string fhirDir)
    {
        Dictionary<int, DbFhirPackage> packageDict = packageSupports.Select(ps => ps.Package).ToDictionary(p => p.Key);
        DbFhirPackage focusPackage = packageSupports[focusPackageIndex].Package;

        ILookup<int, SnapshotGenerator?> generatorsById = packageSupports.ToLookup(ps => ps.Package.Key, ps => ps.SnapshotGenerator);

        FhirJsonSerializer jsonSerializer = new FhirJsonSerializer(new SerializerSettings() { Pretty = true });

        // iterate over the profiles
        foreach (((int _, int _, int targetPackageId), StructureDefinition sd) in xverProfiles)
        {
            // there are unfixable errors in the snapshot generator right now, so we are adding a step to generate snapshots via IG publisher by default
            if (_config.XverGenerateSnapshots == true)
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
            File.WriteAllText(path, jsonSerializer.SerializeToString(sd));
        }
    }


    /// <summary>
    /// Builds cross-version FHIR StructureDefinitions (extensions) and tracks mapping outcomes between source and target packages.
    /// </summary>
    /// <param name="packageSupports">List of package support objects representing each FHIR package.</param>
    /// <param name="sourcePackageIndex">Index of the source package in the packageSupports list.</param>
    /// <param name="xverValueSets">Dictionary of cross-version ValueSets, keyed by (source ValueSet key, target package id).</param>
    /// <param name="xverExtensions">Dictionary of cross-version StructureDefinitions (extensions), keyed by (source element key, target package id).</param>
    /// <param name="xverOutcomes">Dictionary tracking mapping outcomes for each structure and element, keyed by (source package index, structure name).</param>
    /// <param name="artifactClass">The artifact class (e.g., Resource, ComplexType) to process.</param>
    private void buildXverExtensions(
        List<PackageXverSupport> packageSupports,
        int sourcePackageIndex,
        Dictionary<(int sourceVsKey, int targetPackageId), ValueSet> xverValueSets,
        Dictionary<(int sourceSdKey, int sourceElementKey, int targetPackageId), (StructureDefinition, DbExtensionSubstitution?)> xverExtensions,
        Dictionary<(int, string), List<List<XverOutcome>>> xverOutcomes,
        Dictionary<string, DbGraphSd> graphsForStructures,
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
            DbGraphSd sdGraph = graphsForStructures[sd.Id];

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
                            SourceStructureKey = sd.Key,
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
                            SourceStructureKey = sd.Key,
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
                                    SourceStructureKey = sd.Key,
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
                        }
                    }

                    // if we still do not need an extension, go to next package
                    if (!extensionNeeded)
                    {
                        continue;
                    }

                    // check to see if we have already generated this extension
                    if (xverExtensions.ContainsKey((element.StructureKey, element.Key, targetPackage.Key)))
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
                        graphsForStructures,
                        element,
                        comparisons,
                        elementProjectionDict,
                        xverValueSets);

                    if (extSd != null)
                    {
                        DbExtensionSubstitution? extSub = DbExtensionSubstitution.SelectSingle(_db!.DbConnection, SourceElementId: element.Id);

                        xverExtensions.Add((element.StructureKey, element.Key, packageSupports[targetPackageIndex].Package.Key), (extSd, extSub));
                        generatedElementKeys[targetPackageIndex].Add(element.Key);

                        xverOutcomes[(sourcePackageIndex, sd.Name)][targetPackageIndex].Add(new()
                        {
                            SourcePackageKey = sourcePackage.Key,
                            SourceStructureKey = sd.Key,
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
                            SourceStructureKey = sd.Key,
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
                            SourceStructureKey = sd.Key,
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
                                    SourceStructureKey = sd.Key,
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
                        }
                    }

                    // if we still do not need an extension, go to next package
                    if (!extensionNeeded)
                    {
                        continue;
                    }

                    // check to see if we have already generated this extension
                    if (xverExtensions.ContainsKey((element.StructureKey, element.Key, targetPackage.Key)))
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
                        graphsForStructures,
                        element,
                        comparisons,
                        elementProjectionDict,
                        xverValueSets);

                    if (extSd != null)
                    {
                        DbExtensionSubstitution? extSub = DbExtensionSubstitution.SelectSingle(_db!.DbConnection, SourceElementId: element.Id);

                        xverExtensions.Add((element.StructureKey, element.Key, targetPackage.Key), (extSd, extSub));
                        generatedElementKeys[targetPackageIndex].Add(element.Key);

                        xverOutcomes[(sourcePackageIndex, sd.Name)][targetPackageIndex].Add(new()
                        {
                            SourcePackageKey = sourcePackage.Key,
                            SourceStructureKey = sd.Key,
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

                if ((targetPackageSupport.CoreDC?.ComplexTypesByName.ContainsKey(name) == true) ||
                    (targetPackageSupport.CoreDC?.ResourcesByName.ContainsKey(name) == true))
                {
                    contextElementPaths.Add(name);
                }

                // if we do not find *anything* that matches, the caller will default by adding Element
            }
        }
    }

    private record TargetContextModifierRec
    {
        public required StructureDefinition.ContextComponent Context { get; init; }
        public required bool IsModifier { get; init; }
        public required bool HasChildElements { get; init; }
        public required bool HasPrimitiveType { get; init; }
        public required bool IsArray { get; init; }
        public required bool IsRootElement { get; init; }
    }

    /// <summary>
    /// Creates a StructureDefinition for a cross-version extension based on the provided source and target package supports,
    /// structure definition, and element. This method determines the appropriate extension context, builds the extension StructureDefinition,
    /// and adds the relevant elements and child elements to the differential.
    /// </summary>
    /// <param name="sourceIndex">The index of the source package in the package supports list.</param>
    /// <param name="sourcePackageSupport">The source package support information.</param>
    /// <param name="targetIndex">The index of the target package in the package supports list.</param>
    /// <param name="targetPackageSupport">The target package support information.</param>
    /// <param name="sd">The source StructureDefinition from which the element originates.</param>
    /// <param name="element">The source DbElement to add to the extension.</param>
    /// <param name="relevantComparisons">A list of relevant element comparisons for this element.</param>
    /// <param name="elementProjectionDict">A dictionary mapping element keys to their projection rows for context discovery.</param>
    /// <param name="xverValueSets">A dictionary of cross-version ValueSets, keyed by (source ValueSet key, target package ID).</param>
    /// <returns>
    /// A new <see cref="StructureDefinition"/> representing the cross-version extension, or <c>null</c> if the extension cannot be created.
    /// </returns>
    private StructureDefinition? createExtensionSd(
        int sourceIndex,
        PackageXverSupport sourcePackageSupport,
        int targetIndex,
        PackageXverSupport targetPackageSupport,
        DbStructureDefinition sd,
        Dictionary<string, DbGraphSd> graphsForStructures,
        DbElement element,
        List<DbElementComparison> relevantComparisons,
        Dictionary<int, List<DbGraphSd.DbElementRow>> elementProjectionDict,
        Dictionary<(int sourceVsKey, int targetPackageId), ValueSet> xverValueSets)
    {
        // never create extensions for '.extension', '.modifierExtension', or '.id' elements
        switch (element.Name)
        {
            case "extension":
            case "modifierExtension":
            case "id":
                return null;
        }

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
            Name = char.ToUpperInvariant(sdId[0]) + sdId[1..].Replace('-', '_').Replace('.', '_'),
            Version = _crossDefinitionVersion,
            FhirVersion = EnumUtility.ParseLiteral<FHIRVersion>(targetPackageSupport!.CoreDC!.FhirVersionLiteral) ?? FHIRVersion.N5_0_0,
            DateElement = new FhirDateTime(DateTimeOffset.Now),
            Title = $"Cross-version Extension for {sourcePackage.ShortName}.{element.Path} for use in FHIR {targetPackage.ShortName}",
            Description = $"This cross-version extension represents {element.Path} from {sd.VersionedUrl} for use in FHIR {targetPackage.ShortName}.",
            Status = PublicationStatus.Draft,
            Experimental = false,
            Kind = StructureDefinition.StructureDefinitionKind.ComplexType,
            Abstract = false,
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
            graphsForStructures,
            element,
            relevantComparisons,
            xverValueSets);

        // check if the element was a modifier so we can determine if the extension is a modifier
        if (element.IsModifier)
        {
            /*
             * Rules for determining if an extension is a modifier:
             * source is modifier element -> target is modifier element: extension is not modifier
             * source is modifier element -> target is backbone that is not modifier: extension is modifier
             * source is modifier element -> target is primitive scalar that is not modifier: need to move the context up one level and extension is modifier
             * source is modifier element -> target is primitive array that is modifier: throw for now and resolve if we hit any
             */

            List<StructureDefinition.ContextComponent> contextsToShorten = [];
            List<TargetContextModifierRec> tcInfoRecs = [];

            // iterate across the contexts
            foreach (StructureDefinition.ContextComponent context in contexts)
            {
                // resolve the context element from the target package
                if (!targetPackageSupport.CoreDC!.TryResolveElementTree(context.Expression, out StructureDefinition? targetSd, out ElementDefinition[] targetElements))
                {
                    // if we cannot resolve the context element, we have a serious problem
                    throw new Exception($"Failed to resolve context element {context.Expression} in target package {targetPackage.ShortName} for extension {extSd.Url}!");
                }

                ElementDefinition targetContextElement = targetElements[^1];

                // fill out our target context modifier info
                tcInfoRecs.Add(new()
                {
                    Context = context,
                    IsModifier = targetContextElement.IsModifier == true,
                    HasChildElements = targetPackageSupport.CoreDC!.HasChildElements(targetContextElement.Path),
                    HasPrimitiveType = targetContextElement.Type.Any(t => targetPackageSupport.CoreDC!.PrimitiveTypesByName.ContainsKey(t.Code)),
                    IsArray = targetContextElement.cgCardinalityMax() != 1,
                    IsRootElement = targetContextElement.IsRootElement(),
                });
            }

            // check if all contexts are modifier elements
            if (tcInfoRecs.All(r => r.IsModifier == true))
            {
                // do not need to make this a modifier extension
            }
            // check if all contexts have child elements
            else if (tcInfoRecs.All(r => r.HasChildElements))
            {
                // this needs to be a modifier extension
                extSd.Differential.Element[0].IsModifier = true;
                extSd.Differential.Element[0].IsModifierReason = element.IsModifierReason
                        ?? (element.IsModifier == true ? $"This extension is a modifier because the source element {element.Id} is a modifier" : null);
            }
            // check if any context is an array with primitive types
            else if (tcInfoRecs.Any(r => r.IsArray && r.HasPrimitiveType))
            {
                // throw an exception - we do not know how to handle this yet (note this has never thrown - precautionary in case something in R6+ causes in the future)
                throw new Exception($"Extension {extSd.Url} has a context element that is an array of primitive types, which is not supported for modifier extensions in cross-version processing.");
            }
            // falling through rules, this needs to be a modifier and any primitive type context should be moved up a level
            else
            {
                // this needs to be a modifier extension
                extSd.Differential.Element[0].IsModifier = true;
                extSd.Differential.Element[0].IsModifierReason = element.IsModifierReason
                        ?? (element.IsModifier == true ? $"This extension is a modifier because the source element {element.Id} is a modifier" : null);

                // modify any contexts we need to shorten
                foreach (TargetContextModifierRec tcInfoRec in tcInfoRecs)
                {
                    if (tcInfoRec.HasPrimitiveType)
                    {
                        tcInfoRec.Context.Expression = tcInfoRec.Context.Expression.Substring(0, tcInfoRec.Context.Expression.LastIndexOf('.'));
                    }
                }
            }
        }

        // set the contexts for our extension
        extSd.Context = contexts;

        return extSd;
    }

    /// <summary>
    /// Extracts and combines the short, definition, and comment text for an extension element.
    /// </summary>
    /// <param name="ed">The <see cref="DbElement"/> representing the FHIR element.</param>
    /// <param name="reason">An optional reason or additional context to include in the comment.</param>
    /// <returns>
    /// A tuple containing the short text, definition, and comment for the extension element.
    /// The tuple values are:
    /// <list type="bullet">
    /// <item><description><c>shortText</c>: The short description of the element, or <c>null</c> if not available.</description></item>
    /// <item><description><c>definition</c>: The detailed definition of the element, or <c>null</c> if not available or redundant with <c>shortText</c>.</description></item>
    /// <item><description><c>comment</c>: Additional comments, including the <paramref name="reason"/>, or <c>null</c> if not available.</description></item>
    /// </list>
    /// </returns>
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

    /// <summary>
    /// Adds an element and its children to the extension StructureDefinition.
    /// </summary>
    /// <param name="extSd">The extension StructureDefinition to which the element will be added.</param>
    /// <param name="extElementId">The element ID for the extension element.</param>
    /// <param name="extElementPath">The FHIR path for the extension element.</param>
    /// <param name="sliceName">The slice name, if applicable, for the extension element.</param>
    /// <param name="sourcePackageSupport">The source package support information.</param>
    /// <param name="targetPackageSupport">The target package support information.</param>
    /// <param name="sd">The source StructureDefinition from which the element originates.</param>
    /// <param name="element">The source DbElement to add to the extension.</param>
    /// <param name="relevantComparisons">A list of relevant element comparisons for this element.</param>
    /// <param name="xverValueSets">A dictionary of cross-version ValueSets, keyed by (source ValueSet key, target package ID).</param>
    private void addElementToExtension(
        StructureDefinition extSd,
        string extElementId,
        string extElementPath,
        string? sliceName,
        PackageXverSupport sourcePackageSupport,
        PackageXverSupport targetPackageSupport,
        DbStructureDefinition sd,
        Dictionary<string, DbGraphSd> graphsForStructures,
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

        //// check for R5:SubscriptionStatus.eventsSinceSubscriptionStart
        //if (element.Key == 42051)
        //{
        //    Console.Write("");
        //}

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
        };

        if (string.IsNullOrEmpty(sliceName))
        {
            extEd.Base = new()
            {
                Path = "Extension",
                Min = 0,
                Max = "*",
            };
        }
        else
        {
            extEd.Base = new()
            {
                Path = "Extension.extension",
                Min = 0,
                Max = "*",
            };

            //extEd.Slicing = new()
            //{
            //    Discriminator = [
            //        new() {
            //            Type = ElementDefinition.DiscriminatorType.Value,
            //            Path = "url",
            //        }
            //    ],
            //    Ordered = false,
            //    Rules = ElementDefinition.SlicingRules.Open,
            //};
        }

        extSd.Differential.Element.Add(extEd);

        bool addedEdForChildren = false;
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
                Rules = ElementDefinition.SlicingRules.Open,
            },
            Min = 0,
            Max = "*",
            Base = new()
            {
                Path = "Element.extension",
                Min = 0,
                Max = "*",
            },
        };

        // if there are child elements, we are done
        if (element.ChildElementCount != 0)
        {
            if (!addedEdForChildren)
            {
                // add the extension element for children
                extSd.Differential.Element.Add(edForChildren);
                addedEdForChildren = true;
            }

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
                    graphsForStructures,
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

            // before we leave, we need to add the URL
            if (string.IsNullOrEmpty(sliceName))
            {
                extSd.Differential.Element.Add(new()
                {
                    ElementId = extElementId + ".url",
                    Path = extElementPath + ".url",
                    Min = 1,
                    Max = "1",
                    Base = new()
                    {
                        Path = "Extension.url",
                        Min = 1,
                        Max = "1",
                    },
                    Fixed = new FhirUri(extSd.Url)
                });
            }
            else
            {
                extSd.Differential.Element.Add(new()
                {
                    ElementId = extElementId + ".url",
                    Path = extElementPath + ".url",
                    Min = 1,
                    Max = "1",
                    Base = new()
                    {
                        Path = "Extension.url",
                        Min = 1,
                        Max = "1",
                    },
                    Fixed = new FhirUri(sliceName)
                });
            }

            // done processing
            return;
        }

        //bool addedEdExt = false;

        bool shouldAddEdValue = false;
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
        Dictionary<string, List<DbElementType>> collectedValueTypes = [];
        Dictionary<string, List<string>> collectedTypeProfiles = [];
        Dictionary<string, List<string>> collectedTargetProfiles = [];
        List<string> alternateCanonicalTargets = [];

        foreach (DbElementType valueType in elementValueTypes)
        {
            string typeName = valueType.TypeName ?? string.Empty;
            if (!collectedValueTypes.TryGetValue(typeName, out List<DbElementType>? typeList))
            {
                typeList = [];
                collectedValueTypes.Add(typeName, typeList);
            }
            typeList.Add(valueType);

            if (!collectedTypeProfiles.TryGetValue(typeName, out List<string>? typeProfiles))
            {
                typeProfiles = [];
                collectedTypeProfiles.Add(typeName, typeProfiles);
            }
            if (!string.IsNullOrEmpty(valueType.TypeProfile))
            {
                typeProfiles.Add(valueType.TypeProfile);
            }

            if (!collectedTargetProfiles.TryGetValue(typeName, out List<string>? targetProfiles))
            {
                targetProfiles = [];
                collectedTargetProfiles.Add(typeName, targetProfiles);
            }
            if (!string.IsNullOrEmpty(valueType.TargetProfile))
            {
                int lastSlashIndex = valueType.TargetProfile.LastIndexOf('/');
                string targetProfileTypeName = lastSlashIndex == -1
                    ? valueType.TargetProfile
                    : valueType.TargetProfile.Substring(lastSlashIndex + 1);

                // try to lookup the type in the structure graph
                if (!graphsForStructures.TryGetValue(targetProfileTypeName, out DbGraphSd? sdGraph))
                {
                    // this does not occur - just a safety check
                    Console.WriteLine($"addElementToExtension <<< failed to resolve target profile: {targetProfileTypeName} in graphsForStructures, skipping!");
                    continue;
                }

                // pull the type names from the graph for the target version
                List<string> resolvedTargetTypes = sdGraph.Projection
                    .Select(sdRow => sdRow.CellAt(targetPackageSupport.PackageIndex)?.Sd?.UnversionedUrl ?? string.Empty)
                    .ToList();

                // if we are in a canonical reference, check the allowed types for the target package
                switch (typeName)
                {
                    case "canonical":
                        {
                            List<string> valid = [];
                            List<string> invalid = [];

                            foreach (string value in resolvedTargetTypes)
                            {
                                if (string.IsNullOrEmpty(value))
                                {
                                    continue;
                                }

                                if (targetPackageSupport.AllowedCanonicalTypes.Contains(value))
                                {
                                    valid.Add(value);
                                }
                                else
                                {
                                    invalid.Add(value);
                                }
                            }

                            if ((valid.Count == 0) && (invalid.Count == 0))
                            {
                                // add the original type as an alternate target
                                alternateCanonicalTargets.Add(valueType.TargetProfile);
                                continue;
                            }

                            if (invalid.Count > 0)
                            {
                                alternateCanonicalTargets = invalid.Distinct().ToList();
                            }

                            if (valid.Count > 0)
                            {
                                targetProfiles.AddRange(valid.Distinct());
                            }
                        }
                        break;

                    case "Reference":
                        {
                            List<string> valid = [];
                            List<string> invalid = [];
                            bool needsBasic = false;

                            foreach (string value in resolvedTargetTypes)
                            {
                                if (string.IsNullOrEmpty(value))
                                {
                                    needsBasic = true;
                                    continue;
                                }

                                if (targetPackageSupport.CoreDC!.CanResolveCanonicalUri(value))
                                {
                                    valid.Add(value);
                                }
                                else
                                {
                                    invalid.Add(value);
                                }
                            }

                            if (valid.Count > 0)
                            {
                                targetProfiles.AddRange(valid.Distinct());
                            }

                            // check for needing basic support
                            if (needsBasic || (invalid.Count > 0))
                            {
                                typeProfiles.Add("http://hl7.org/fhir/StructureDefinition/Basic");
                                //resolvedTargetTypes.Add("http://hl7.org/fhir/StructureDefinition/Basic");
                                //resolvedTargetTypes.RemoveAll(string.IsNullOrEmpty);
                            }
                        }
                        break;
                }

            }
        }

        //// if we have moved target profiles from canonical to uri, make sure that we have a URI type
        //if (collectedTargetProfiles.ContainsKey("uri") && !collectedValueTypes.ContainsKey("uri"))
        //{
        //    DbElementType? uriElementType = DbElementType.SelectSingle(_db!.DbConnection,
        //        ElementKey: element.Key,
        //        TypeName: "uri",
        //        FhirPackageKey: targetPackageSupport.Package.Key);

        //    uriElementType ??= new DbElementType()
        //    {
        //        FhirPackageKey = targetPackageSupport.Package.Key,
        //        StructureKey = element.StructureKey,
        //        ElementKey = element.Key,
        //        TypeName = "uri",
        //        TypeProfile = null,
        //        TargetProfile = null,
        //        TypeStructureKey = null,
        //    };

        //    collectedValueTypes.Add("uri", [uriElementType]);
        //    elementValueTypes.Add(uriElementType);
        //}

        //// similarly, if we removed *all* the types from canonical, it should no longer appear
        //if (collectedValueTypes.ContainsKey("canonical") &&
        //    (collectedTargetProfiles["canonical"].Count == 0))
        //{
        //    collectedValueTypes.Remove("canonical");
        //    collectedTypeProfiles.Remove("canonical");
        //    collectedTargetProfiles.Remove("canonical");
        //    elementValueTypes.RemoveAll(et => et.TypeName == "canonical");
        //}

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
                    collectedTypeProfiles.TryGetValue(valueTypeName, out List<string>? typeProfiles);
                    typeProfiles ??= [];

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

        HashSet<string> addedTypes = [];
        ElementDefinition? extensionDatatypeValueElement = null;

        // process mapped types (extension before value)
        foreach (string typeName in extMappedTypes)
        {
            if (!addedEdForChildren)
            {
                // add the extension element for children
                extSd.Differential.Element.Add(edForChildren);
                addedEdForChildren = true;
            }

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
                    graphsForStructures,
                    etElement,
                    relevantComparisons,
                    xverValueSets);
            }
        }

        // if we are listing the alternate-canonical extension, we need to add the extension before adding our slice URL
        if (alternateCanonicalTargets.Count > 0)
        {
            addAlternateCanonicalExtension(
                extSd,
                element,
                sourcePackageSupport,
                ref extensionDatatypeValueElement,
                extElementId,
                extElementPath,
                alternateCanonicalTargets);
        }

        // process replaced quantity types - sort types for readability
        foreach (string typeName in quantityProfilesMovedToTypes.Order())
        {
            if (addedTypes.Contains(typeName))
            {
                continue;
            }

            // add the value element if we are supposed to
            if (!shouldAddEdValue)
            {
                shouldAddEdValue = true;
            }

            collectedTypeProfiles.TryGetValue(typeName, out List<string>? typeProfiles);
            typeProfiles ??= [];

            collectedTargetProfiles.TryGetValue(typeName, out List<string>? targetProfiles);
            targetProfiles ??= [];

            // create a new type reference
            ElementDefinition.TypeRefComponent edValueType = new()
            {
                Code = typeName,
                ProfileElement = typeProfiles.Select(v => new Canonical(v)).ToList(),
            };

            if ((typeName == "Reference") || (typeName == "canonical"))
            {
                edValueType.TargetProfileElement = targetProfiles.Select(v => new Canonical(v)).ToList();
            }

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

            addedTypes.Add(typeName);
        }

        // process allowed and replaceable types - sort types for readability
        foreach (string typeName in extAllowedTypes.Order())
        {
            if (addedTypes.Contains(typeName))
            {
                continue;
            }

            // add the value element if we are supposed to
            if (!shouldAddEdValue)
            {
                shouldAddEdValue = true;
            }
            collectedTypeProfiles.TryGetValue(typeName, out List<string>? typeProfiles);
            typeProfiles ??= [];

            collectedTargetProfiles.TryGetValue(typeName, out List<string>? targetProfiles);
            targetProfiles ??= [];

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
            ElementDefinition.TypeRefComponent edValueType = new()
            {
                Code = typeName,
                ProfileElement = typeProfiles.Select(v => new Canonical(v)).ToList(),
            };

            if ((typeName == "Reference") || (typeName == "canonical"))
            {
                edValueType.TargetProfileElement = targetProfiles.Select(v => new Canonical(v)).ToList();
            }

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

            addedTypes.Add(typeName);
        }

        // check for any missed replaceable types - sort types for readability
        foreach ((string typeName, List<string> replaceableTypes) in extReplaceableTypes.OrderBy(kvp => kvp.Key))
        {
            foreach (string sourceTypeName in replaceableTypes)
            {
                if (addedTypes.Contains(sourceTypeName))
                {
                    continue;
                }

                if (!addedEdForChildren)
                {
                    // add the extension element for children
                    extSd.Differential.Element.Add(edForChildren);
                    addedEdForChildren = true;
                }

                // add the _datatype extension for this type (or add to the existing one)
                addDatatypeExtension(
                    extSd,
                    element,
                    sourcePackageSupport,
                    ref extensionDatatypeValueElement,
                    extElementId,
                    extElementPath,
                    sourceTypeName);

                string typeSliceName = "value" + sourceTypeName.ToPascalCase();

                // add this type as an extension slice
                extSd.Differential.Element.Add(new()
                {
                    ElementId = $"{extElementId}.extension:{typeSliceName}",
                    Path = extElementPath + ".extension",
                    SliceName = typeSliceName,
                    Min = 0,
                    Max = element.MaxCardinalityString,
                    Base = new()
                    {
                        Path = "Element.extension",
                        Min = 0,
                        Max = "*",
                    },
                });

                // add the url element for this extension slice
                extSd.Differential.Element.Add(new()
                {
                    ElementId = $"{extElementId}.extension:{typeSliceName}.url",
                    Path = extElementPath + ".extension.url",
                    Min = 1,
                    Max = "1",
                    Base = new()
                    {
                        Path = "Extension.url",
                        Min = 1,
                        Max = "1",
                    },
                    Fixed = new FhirUri(typeSliceName),
                });

                // add the value[x] element for this extension slice, using the replacement type
                extSd.Differential.Element.Add(new()
                {
                    ElementId = $"{extElementId}.extension:{typeSliceName}.value[x]",
                    Path = extElementPath + ".extension.value[x]",
                    Min = 0,
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
                            Code = typeName,
                        }
                    ],
                });

                addedTypes.Add(sourceTypeName);
            }
        }

        // we are switching from .extension to .value[x] here, so we need to add the url element
        if (string.IsNullOrEmpty(sliceName))
        {
            extSd.Differential.Element.Add(new()
            {
                ElementId = extElementId + ".url",
                Path = extElementPath + ".url",
                Min = 1,
                Max = "1",
                Base = new()
                {
                    Path = "Extension.url",
                    Min = 1,
                    Max = "1",
                },
                Fixed = new FhirUri(extSd.Url)
            });
        }
        else
        {
            extSd.Differential.Element.Add(new()
            {
                ElementId = extElementId + ".url",
                Path = extElementPath + ".url",
                Min = 1,
                Max = "1",
                Base = new()
                {
                    Path = "Extension.url",
                    Min = 1,
                    Max = "1",
                },
                Fixed = new FhirUri(sliceName)
            });
        }

        // check if we need to add the value element
        if (shouldAddEdValue)
        {
            extSd.Differential.Element.Add(extensionEdValue);
        }

        return;
    }

    private void addAlternateCanonicalExtension(
        StructureDefinition extSd,
        DbElement sourceDbElement,
        PackageXverSupport sourcePackageSupport,
        ref ElementDefinition? extensionDatatypeValueElement,
        string parentId,
        string parentPath,
        List<string> canonicalTargets)
    {
        extSd.Differential.Element.Add(new()
        {
            ElementId = parentId + ".extension:alternate-canonical",
            Path = parentPath + ".extension",
            SliceName = "alternate-canonical",
            Short = "Alternative reference (target type is wrong)",
            Definition = "Used when the target of the reference has a type that is not allowed by the definition of the element. In general, this should only arise when wrangling between versions using cross-version extensions.",
            Comment = $"Allowed for resources representing FHIR {sourcePackageSupport.Package.FhirVersionShort}: {string.Join(", ", canonicalTargets.Select(ct => ct.Split('/')[^1]))}",
            Min = 0,
            Max = "1",
            Base = new()
            {
                Path = "Extension.extension",
                Min = 0,
                Max = "*",
            },
            Type = [
                new()
                {
                    Code = "Extension",
                    Profile = ["http://hl7.org/fhir/StructureDefinition/alternate-canonical"],
                }
            ],
        });

        extSd.Differential.Element.Add(new()
        {
            ElementId = parentId + ".extension:alternate-canonical.url",
            Path = parentPath + ".extension.url",
            Min = 1,
            Max = "1",
            Base = new()
            {
                Path = "Extension.url",
                Min = 1,
                Max = "1",
            },
            Fixed = new FhirUri("http://hl7.org/fhir/StructureDefinition/alternate-canonical")
        });

        extSd.Differential.Element.Add(new()
        {
            ElementId = parentId + ".extension:alternate-canonical.value[x]",
            Path = parentPath + ".extension.value[x]",
            Comment = $"Allowed for resources representing FHIR {sourcePackageSupport.Package.FhirVersionShort}: {string.Join(", ", canonicalTargets.Select(ct => ct.Split('/')[^1]))}",
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
                    Code = "url",
                }
            ]
        });
    }

    /// <summary>
    /// Adds a datatype extension element to the provided <see cref="StructureDefinition"/> for a given type name.
    /// This method ensures that the extension for the datatype is created only once, and if already present,
    /// updates the fixed value and comment to include the new type.
    /// </summary>
    /// <param name="extSd">The <see cref="StructureDefinition"/> to which the datatype extension will be added.</param>
    /// <param name="sourceDbElement">The source <see cref="DbElement"/> representing the FHIR element being extended.</param>
    /// <param name="sourcePackageSupport">The <see cref="PackageXverSupport"/> for the source FHIR package.</param>
    /// <param name="extensionDatatypeValueElement">
    /// A reference to the <see cref="ElementDefinition"/> representing the extension's value element.
    /// This will be initialized if not already present.
    /// </param>
    /// <param name="parentId">The parent element ID for the extension element.</param>
    /// <param name="parentPath">The parent FHIR path for the extension element.</param>
    /// <param name="typeName">The name of the datatype to add as an extension.</param>
    private void addDatatypeExtension(
        StructureDefinition extSd,
        DbElement sourceDbElement,
        PackageXverSupport sourcePackageSupport,
        ref ElementDefinition? extensionDatatypeValueElement,
        string parentId,
        string parentPath,
        string typeName,
        bool isRequired = false)
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
                Min = isRequired ? 1 : 0,
                Max = "1",
                Base = new()
                {
                    Path = "Extension.extension",
                    Min = 0,
                    Max = "*",
                },
                Type = [
                        new()
                        {
                            Code = "Extension",
                            Profile = ["http://hl7.org/fhir/StructureDefinition/_datatype"],
                        }
                    ],
            });

            extSd.Differential.Element.Add(new()
            {
                ElementId = parentId + ".extension:_datatype.url",
                Path = parentPath + ".extension.url",
                Min = 1,
                Max = "1",
                Base = new()
                {
                    Path = "Extension.url",
                    Min = 1,
                    Max = "1",
                },
                Fixed = new FhirUri("_datatype")
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

    private void addRequiredExtensionDefinitions(DefinitionCollection dc)
    {
        string alternateCanonicalJson = """
            {
              "resourceType" : "StructureDefinition",
              "id" : "alternate-canonical",
              "text" : {
                "status" : "extensions",
                "div" : "<div xmlns=\"http://www.w3.org/1999/xhtml\"><p class=\"res-header-id\"><b>Generated Narrative: StructureDefinition alternate-canonical</b></p><a name=\"alternate-canonical\"> </a><a name=\"hcalternate-canonical\"> </a><a name=\"alternate-canonical-en-US\"> </a><table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" style=\"border: 0px #F0F0F0 solid; font-size: 11px; font-family: verdana; vertical-align: top;\"><tr style=\"border: 1px #F0F0F0 solid; font-size: 11px; font-family: verdana; vertical-align: top\"><th style=\"vertical-align: top; text-align : left; background-color: white; border: 0px #F0F0F0 solid; padding:0px 4px 0px 4px; padding-top: 3px; padding-bottom: 3px\" class=\"hierarchy\"><a href=\"https://build.fhir.org/ig/FHIR/ig-guidance/readingIgs.html#table-views\" title=\"The logical name of the element\">Name</a></th><th style=\"vertical-align: top; text-align : left; background-color: white; border: 0px #F0F0F0 solid; padding:0px 4px 0px 4px; padding-top: 3px; padding-bottom: 3px\" class=\"hierarchy\"><a href=\"https://build.fhir.org/ig/FHIR/ig-guidance/readingIgs.html#table-views\" title=\"Information about the use of the element\">Flags</a></th><th style=\"vertical-align: top; text-align : left; background-color: white; border: 0px #F0F0F0 solid; padding:0px 4px 0px 4px; padding-top: 3px; padding-bottom: 3px\" class=\"hierarchy\"><a href=\"https://build.fhir.org/ig/FHIR/ig-guidance/readingIgs.html#table-views\" title=\"Minimum and Maximum # of times the element can appear in the instance\">Card.</a></th><th style=\"width: 100px\" class=\"hierarchy\"><a href=\"https://build.fhir.org/ig/FHIR/ig-guidance/readingIgs.html#table-views\" title=\"Reference to the type of the element\">Type</a></th><th style=\"vertical-align: top; text-align : left; background-color: white; border: 0px #F0F0F0 solid; padding:0px 4px 0px 4px; padding-top: 3px; padding-bottom: 3px\" class=\"hierarchy\"><a href=\"https://build.fhir.org/ig/FHIR/ig-guidance/readingIgs.html#table-views\" title=\"Additional information about the element\">Description &amp; Constraints</a><span style=\"float: right\"><a href=\"https://build.fhir.org/ig/FHIR/ig-guidance/readingIgs.html#table-views\" title=\"Legend for this format\"><img src=\"data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAABmJLR0QA/wD/AP+gvaeTAAAACXBIWXMAAAsTAAALEwEAmpwYAAAAB3RJTUUH3goXBCwdPqAP0wAAAldJREFUOMuNk0tIlFEYhp9z/vE2jHkhxXA0zJCMitrUQlq4lnSltEqCFhFG2MJFhIvIFpkEWaTQqjaWZRkp0g26URZkTpbaaOJkDqk10szoODP//7XIMUe0elcfnPd9zsfLOYplGrpRwZaqTtw3K7PtGem7Q6FoidbGgqHVy/HRb669R+56zx7eRV1L31JGxYbBtjKK93cxeqfyQHbehkZbUkK20goELEuIzEd+dHS+qz/Y8PTSif0FnGkbiwcAjHaU1+QWOptFiyCLp/LnKptpqIuXHx6rbR26kJcBX3yLgBfnd7CxwJmflpP2wUg0HIAoUUpZBmKzELGWcN8nAr6Gpu7tLU/CkwAaoKTWRSQyt89Q8w6J+oVQkKnBoblH7V0PPvUOvDYXfopE/SJmALsxnVm6LbkotrUtNowMeIrVrBcBpaMmdS0j9df7abpSuy7HWehwJdt1lhVwi/J58U5beXGAF6c3UXLycw1wdFklArBn87xdh0ZsZtArghBdAA3+OEDVubG4UEzP6x1FOWneHh2VDAHBAt80IbdXDcesNoCvs3E5AFyNSU5nbrDPZpcUEQQTFZiEVx+51fxMhhyJEAgvlriadIJZZksRuwBYMOPBbO3hePVVqgEJhFeUuFLhIPkRP6BQLIBrmMenujm/3g4zc398awIe90Zb5A1vREALqneMcYgP/xVQWlG+Ncu5vgwwlaUNx+3799rfe96u9K0JSDXcOzOTJg4B6IgmXfsygc7/Bvg9g9E58/cDVmGIBOP/zT8Bz1zqWqpbXIsd0O9hajXfL6u4BaOS6SeWAAAAAElFTkSuQmCC\" alt=\"doco\" style=\"background-color: inherit\"/></a></span></th></tr><tr style=\"border: 0px #F0F0F0 solid; padding:0px; vertical-align: top; background-color: white\"><td style=\"vertical-align: top; text-align : left; background-color: white; border: 0px #F0F0F0 solid; padding:0px 4px 0px 4px; white-space: nowrap; background-image: url(tbl_bck1.png)\" class=\"hierarchy\"><img src=\"tbl_spacer.png\" alt=\".\" style=\"background-color: inherit\" class=\"hierarchy\"/><img src=\"icon_element.gif\" alt=\".\" style=\"background-color: white; background-color: inherit\" title=\"Element\" class=\"hierarchy\"/> <a href=\"StructureDefinition-alternate-canonical-definitions.html#Extension\" title=\"Used with inter-version extensions where the element being referenced by inter-version extension is of type 'canonical' and includes a reference to a resource whose canonical URL is different in the referencing version than it is in the FHIR version where the element was defined.  E.g. if an R5 implementation were using inter-version extensions to support an element that referenced Bar, when in R7, the url would have been .../Foo.  In this situation, the canonical element would have no value and would instead have an extension that referred to the canonical URL of the '../Bar' resource (which would technically not be supported in R7, but is appropriate in R5).\">Extension</a><a name=\"Extension\"> </a></td><td style=\"vertical-align: top; text-align : left; background-color: white; border: 0px #F0F0F0 solid; padding:0px 4px 0px 4px\" class=\"hierarchy\"/><td style=\"vertical-align: top; text-align : left; background-color: white; border: 0px #F0F0F0 solid; padding:0px 4px 0px 4px\" class=\"hierarchy\">0..1</td><td style=\"vertical-align: top; text-align : left; background-color: white; border: 0px #F0F0F0 solid; padding:0px 4px 0px 4px\" class=\"hierarchy\"><a href=\"http://hl7.org/fhir/R5/extensibility.html#Extension\">Extension</a></td><td style=\"vertical-align: top; text-align : left; background-color: white; border: 0px #F0F0F0 solid; padding:0px 4px 0px 4px\" class=\"hierarchy\">Alternative reference (target type is wrong)</td></tr>\r\n<tr style=\"border: 0px #F0F0F0 solid; padding:0px; vertical-align: top; background-color: #F7F7F7\"><td style=\"vertical-align: top; text-align : left; background-color: #F7F7F7; border: 0px #F0F0F0 solid; padding:0px 4px 0px 4px; white-space: nowrap; background-image: url(tbl_bck10.png)\" class=\"hierarchy\"><img src=\"tbl_spacer.png\" alt=\".\" style=\"background-color: inherit\" class=\"hierarchy\"/><img src=\"tbl_vjoin.png\" alt=\".\" style=\"background-color: inherit\" class=\"hierarchy\"/><img src=\"icon_extension_simple.png\" alt=\".\" style=\"background-color: #F7F7F7; background-color: inherit\" title=\"Simple Extension\" class=\"hierarchy\"/> <span style=\"text-decoration:line-through\">extension</span><a name=\"Extension.extension\"> </a></td><td style=\"vertical-align: top; text-align : left; background-color: #F7F7F7; border: 0px #F0F0F0 solid; padding:0px 4px 0px 4px\" class=\"hierarchy\"/><td style=\"vertical-align: top; text-align : left; background-color: #F7F7F7; border: 0px #F0F0F0 solid; padding:0px 4px 0px 4px\" class=\"hierarchy\"><span style=\"text-decoration:line-through\"/><span style=\"text-decoration:line-through\">0</span><span style=\"text-decoration:line-through\">..</span><span style=\"text-decoration:line-through\">0</span></td><td style=\"vertical-align: top; text-align : left; background-color: #F7F7F7; border: 0px #F0F0F0 solid; padding:0px 4px 0px 4px\" class=\"hierarchy\"/><td style=\"vertical-align: top; text-align : left; background-color: #F7F7F7; border: 0px #F0F0F0 solid; padding:0px 4px 0px 4px\" class=\"hierarchy\"/></tr>\r\n<tr style=\"border: 0px #F0F0F0 solid; padding:0px; vertical-align: top; background-color: white\"><td style=\"vertical-align: top; text-align : left; background-color: white; border: 0px #F0F0F0 solid; padding:0px 4px 0px 4px; white-space: nowrap; background-image: url(tbl_bck10.png)\" class=\"hierarchy\"><img src=\"tbl_spacer.png\" alt=\".\" style=\"background-color: inherit\" class=\"hierarchy\"/><img src=\"tbl_vjoin.png\" alt=\".\" style=\"background-color: inherit\" class=\"hierarchy\"/><img src=\"icon_element.gif\" alt=\".\" style=\"background-color: white; background-color: inherit\" title=\"Element\" class=\"hierarchy\"/> <a href=\"StructureDefinition-alternate-canonical-definitions.html#Extension.url\">url</a><a name=\"Extension.url\"> </a></td><td style=\"vertical-align: top; text-align : left; background-color: white; border: 0px #F0F0F0 solid; padding:0px 4px 0px 4px\" class=\"hierarchy\"/><td style=\"vertical-align: top; text-align : left; background-color: white; border: 0px #F0F0F0 solid; padding:0px 4px 0px 4px\" class=\"hierarchy\"><span style=\"opacity: 0.5\">1</span><span style=\"opacity: 0.5\">..</span><span style=\"opacity: 0.5\">1</span></td><td style=\"vertical-align: top; text-align : left; background-color: white; border: 0px #F0F0F0 solid; padding:0px 4px 0px 4px\" class=\"hierarchy\"><a style=\"opacity: 0.5\" href=\"http://hl7.org/fhir/R5/datatypes.html#uri\">uri</a></td><td style=\"vertical-align: top; text-align : left; background-color: white; border: 0px #F0F0F0 solid; padding:0px 4px 0px 4px\" class=\"hierarchy\"><span style=\"color: darkgreen\">&quot;http://hl7.org/fhir/StructureDefinition/alternate-canonical&quot;</span></td></tr>\r\n<tr style=\"border: 0px #F0F0F0 solid; padding:0px; vertical-align: top; background-color: #F7F7F7\"><td style=\"vertical-align: top; text-align : left; background-color: #F7F7F7; border: 0px #F0F0F0 solid; padding:0px 4px 0px 4px; white-space: nowrap; background-image: url(tbl_bck00.png)\" class=\"hierarchy\"><img src=\"tbl_spacer.png\" alt=\".\" style=\"background-color: inherit\" class=\"hierarchy\"/><img src=\"tbl_vjoin_end.png\" alt=\".\" style=\"background-color: inherit\" class=\"hierarchy\"/><img src=\"icon_primitive.png\" alt=\".\" style=\"background-color: #F7F7F7; background-color: inherit\" title=\"Primitive Data Type\" class=\"hierarchy\"/> <a href=\"StructureDefinition-alternate-canonical-definitions.html#Extension.value[x]\">value[x]</a><a name=\"Extension.value_x_\"> </a></td><td style=\"vertical-align: top; text-align : left; background-color: #F7F7F7; border: 0px #F0F0F0 solid; padding:0px 4px 0px 4px\" class=\"hierarchy\"/><td style=\"vertical-align: top; text-align : left; background-color: #F7F7F7; border: 0px #F0F0F0 solid; padding:0px 4px 0px 4px\" class=\"hierarchy\">1..<span style=\"opacity: 0.5\">1</span></td><td style=\"vertical-align: top; text-align : left; background-color: #F7F7F7; border: 0px #F0F0F0 solid; padding:0px 4px 0px 4px\" class=\"hierarchy\"><a href=\"http://hl7.org/fhir/R5/datatypes.html#url\">url</a></td><td style=\"vertical-align: top; text-align : left; background-color: #F7F7F7; border: 0px #F0F0F0 solid; padding:0px 4px 0px 4px\" class=\"hierarchy\"><span style=\"opacity: 0.5\">Value of extension</span></td></tr>\r\n<tr><td colspan=\"5\" class=\"hierarchy\"><br/><a href=\"https://build.fhir.org/ig/FHIR/ig-guidance/readingIgs.html#table-views\" title=\"Legend for this format\"><img src=\"data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAABmJLR0QA/wD/AP+gvaeTAAAACXBIWXMAAAsTAAALEwEAmpwYAAAAB3RJTUUH3goXBCwdPqAP0wAAAldJREFUOMuNk0tIlFEYhp9z/vE2jHkhxXA0zJCMitrUQlq4lnSltEqCFhFG2MJFhIvIFpkEWaTQqjaWZRkp0g26URZkTpbaaOJkDqk10szoODP//7XIMUe0elcfnPd9zsfLOYplGrpRwZaqTtw3K7PtGem7Q6FoidbGgqHVy/HRb669R+56zx7eRV1L31JGxYbBtjKK93cxeqfyQHbehkZbUkK20goELEuIzEd+dHS+qz/Y8PTSif0FnGkbiwcAjHaU1+QWOptFiyCLp/LnKptpqIuXHx6rbR26kJcBX3yLgBfnd7CxwJmflpP2wUg0HIAoUUpZBmKzELGWcN8nAr6Gpu7tLU/CkwAaoKTWRSQyt89Q8w6J+oVQkKnBoblH7V0PPvUOvDYXfopE/SJmALsxnVm6LbkotrUtNowMeIrVrBcBpaMmdS0j9df7abpSuy7HWehwJdt1lhVwi/J58U5beXGAF6c3UXLycw1wdFklArBn87xdh0ZsZtArghBdAA3+OEDVubG4UEzP6x1FOWneHh2VDAHBAt80IbdXDcesNoCvs3E5AFyNSU5nbrDPZpcUEQQTFZiEVx+51fxMhhyJEAgvlriadIJZZksRuwBYMOPBbO3hePVVqgEJhFeUuFLhIPkRP6BQLIBrmMenujm/3g4zc398awIe90Zb5A1vREALqneMcYgP/xVQWlG+Ncu5vgwwlaUNx+3799rfe96u9K0JSDXcOzOTJg4B6IgmXfsygc7/Bvg9g9E58/cDVmGIBOP/zT8Bz1zqWqpbXIsd0O9hajXfL6u4BaOS6SeWAAAAAElFTkSuQmCC\" alt=\"doco\" style=\"background-color: inherit\"/> Documentation for this format</a></td></tr></table></div>"
              },
              "extension" : [{
                "url" : "http://hl7.org/fhir/StructureDefinition/structuredefinition-wg",
                "valueCode" : "fhir"
              },
              {
                "url" : "http://hl7.org/fhir/StructureDefinition/structuredefinition-fmm",
                "valueInteger" : 2
              },
              {
                "url" : "http://hl7.org/fhir/StructureDefinition/structuredefinition-standards-status",
                "valueCode" : "trial-use"
              },
              {
                "url" : "http://hl7.org/fhir/StructureDefinition/structuredefinition-type-characteristics",
                "valueCode" : "can-bind"
              }],
              "url" : "http://hl7.org/fhir/StructureDefinition/alternate-canonical",
              "identifier" : [{
                "system" : "urn:ietf:rfc:3986",
                "value" : "urn:oid:2.16.840.1.113883.4.642.5.1674"
              }],
              "version" : "5.2.0",
              "name" : "AlternateCanonical",
              "title" : "Alternate Canonical",
              "status" : "draft",
              "experimental" : false,
              "date" : "2014-04-27",
              "publisher" : "HL7 International / FHIR Infrastructure",
              "contact" : [{
                "telecom" : [{
                  "system" : "url",
                  "value" : "http://www.hl7.org/Special/committees/fiwg"
                }]
              }],
              "description" : "Used with inter-version extensions where the element being referenced by inter-version extension is of type 'canonical' and includes a reference to a resource whose canonical URL is different in the referencing version than it is in the FHIR version where the element was defined.  E.g. if an R5 implementation were using inter-version extensions to support an element that referenced Bar, when in R7, the url would have been .../Foo.  In this situation, the canonical element would have no value and would instead have an extension that referred to the canonical URL of the '../Bar' resource (which would technically not be supported in R7, but is appropriate in R5).",
              "jurisdiction" : [{
                "coding" : [{
                  "system" : "http://unstats.un.org/unsd/methods/m49/m49.htm",
                  "code" : "001"
                }]
              }],
              "fhirVersion" : "5.0.0",
              "mapping" : [{
                "identity" : "rim",
                "uri" : "http://hl7.org/v3",
                "name" : "RIM Mapping"
              }],
              "kind" : "complex-type",
              "abstract" : false,
              "context" : [{
                "type" : "element",
                "expression" : "canonical"
              }],
              "type" : "Extension",
              "baseDefinition" : "http://hl7.org/fhir/StructureDefinition/Extension",
              "derivation" : "constraint",
              "snapshot" : {
                "extension" : [{
                  "url" : "http://hl7.org/fhir/tools/StructureDefinition/snapshot-base-version",
                  "valueString" : "5.0.0"
                }],
                "element" : [{
                  "id" : "Extension",
                  "path" : "Extension",
                  "short" : "Alternative reference (target type is wrong)",
                  "definition" : "Used with inter-version extensions where the element being referenced by inter-version extension is of type 'canonical' and includes a reference to a resource whose canonical URL is different in the referencing version than it is in the FHIR version where the element was defined.  E.g. if an R5 implementation were using inter-version extensions to support an element that referenced Bar, when in R7, the url would have been .../Foo.  In this situation, the canonical element would have no value and would instead have an extension that referred to the canonical URL of the '../Bar' resource (which would technically not be supported in R7, but is appropriate in R5).",
                  "min" : 0,
                  "max" : "1",
                  "base" : {
                    "path" : "Extension",
                    "min" : 0,
                    "max" : "*"
                  },
                  "constraint" : [{
                    "key" : "ele-1",
                    "severity" : "error",
                    "human" : "All FHIR elements must have a @value or children",
                    "expression" : "hasValue() or (children().count() > id.count())",
                    "source" : "http://hl7.org/fhir/StructureDefinition/Element"
                  },
                  {
                    "key" : "ext-1",
                    "severity" : "error",
                    "human" : "Must have either extensions or value[x], not both",
                    "expression" : "extension.exists() != value.exists()",
                    "source" : "http://hl7.org/fhir/StructureDefinition/Extension"
                  }],
                  "isModifier" : false
                },
                {
                  "id" : "Extension.id",
                  "path" : "Extension.id",
                  "representation" : ["xmlAttr"],
                  "short" : "Unique id for inter-element referencing",
                  "definition" : "Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.",
                  "min" : 0,
                  "max" : "1",
                  "base" : {
                    "path" : "Element.id",
                    "min" : 0,
                    "max" : "1"
                  },
                  "type" : [{
                    "extension" : [{
                      "url" : "http://hl7.org/fhir/StructureDefinition/structuredefinition-fhir-type",
                      "valueUrl" : "id"
                    }],
                    "code" : "http://hl7.org/fhirpath/System.String"
                  }],
                  "condition" : ["ele-1"],
                  "isModifier" : false,
                  "isSummary" : false,
                  "mapping" : [{
                    "identity" : "rim",
                    "map" : "n/a"
                  }]
                },
                {
                  "id" : "Extension.extension",
                  "path" : "Extension.extension",
                  "slicing" : {
                    "discriminator" : [{
                      "type" : "value",
                      "path" : "url"
                    }],
                    "description" : "Extensions are always sliced by (at least) url",
                    "rules" : "open"
                  },
                  "short" : "Extension",
                  "definition" : "An Extension",
                  "min" : 0,
                  "max" : "0",
                  "base" : {
                    "path" : "Element.extension",
                    "min" : 0,
                    "max" : "*"
                  },
                  "type" : [{
                    "code" : "Extension"
                  }],
                  "constraint" : [{
                    "key" : "ele-1",
                    "severity" : "error",
                    "human" : "All FHIR elements must have a @value or children",
                    "expression" : "hasValue() or (children().count() > id.count())",
                    "source" : "http://hl7.org/fhir/StructureDefinition/Element"
                  },
                  {
                    "key" : "ext-1",
                    "severity" : "error",
                    "human" : "Must have either extensions or value[x], not both",
                    "expression" : "extension.exists() != value.exists()",
                    "source" : "http://hl7.org/fhir/StructureDefinition/Extension"
                  }],
                  "isModifier" : false,
                  "isSummary" : false
                },
                {
                  "id" : "Extension.url",
                  "path" : "Extension.url",
                  "representation" : ["xmlAttr"],
                  "short" : "identifies the meaning of the extension",
                  "definition" : "Source of the definition for the extension code - a logical name or a URL.",
                  "comment" : "The definition may point directly to a computable or human-readable definition of the extensibility codes, or it may be a logical URI as declared in some other specification. The definition SHALL be a URI for the Structure Definition defining the extension.",
                  "min" : 1,
                  "max" : "1",
                  "base" : {
                    "path" : "Extension.url",
                    "min" : 1,
                    "max" : "1"
                  },
                  "type" : [{
                    "extension" : [{
                      "url" : "http://hl7.org/fhir/StructureDefinition/structuredefinition-fhir-type",
                      "valueUrl" : "uri"
                    }],
                    "code" : "http://hl7.org/fhirpath/System.String"
                  }],
                  "fixedUri" : "http://hl7.org/fhir/StructureDefinition/alternate-canonical",
                  "isModifier" : false,
                  "isSummary" : false,
                  "mapping" : [{
                    "identity" : "rim",
                    "map" : "N/A"
                  }]
                },
                {
                  "id" : "Extension.value[x]",
                  "path" : "Extension.value[x]",
                  "short" : "Value of extension",
                  "definition" : "Value of extension - must be one of a constrained set of the data types (see [Extensibility](http://hl7.org/fhir/R5/extensibility.html) for a list).",
                  "min" : 1,
                  "max" : "1",
                  "base" : {
                    "path" : "Extension.value[x]",
                    "min" : 0,
                    "max" : "1"
                  },
                  "type" : [{
                    "code" : "url"
                  }],
                  "condition" : ["ext-1"],
                  "constraint" : [{
                    "key" : "ele-1",
                    "severity" : "error",
                    "human" : "All FHIR elements must have a @value or children",
                    "expression" : "hasValue() or (children().count() > id.count())",
                    "source" : "http://hl7.org/fhir/StructureDefinition/Element"
                  }],
                  "isModifier" : false,
                  "isSummary" : false,
                  "mapping" : [{
                    "identity" : "rim",
                    "map" : "N/A"
                  }]
                }]
              },
              "differential" : {
                "element" : [{
                  "id" : "Extension",
                  "path" : "Extension",
                  "short" : "Alternative reference (target type is wrong)",
                  "definition" : "Used with inter-version extensions where the element being referenced by inter-version extension is of type 'canonical' and includes a reference to a resource whose canonical URL is different in the referencing version than it is in the FHIR version where the element was defined.  E.g. if an R5 implementation were using inter-version extensions to support an element that referenced Bar, when in R7, the url would have been .../Foo.  In this situation, the canonical element would have no value and would instead have an extension that referred to the canonical URL of the '../Bar' resource (which would technically not be supported in R7, but is appropriate in R5).",
                  "min" : 0,
                  "max" : "1"
                },
                {
                  "id" : "Extension.extension",
                  "path" : "Extension.extension",
                  "max" : "0"
                },
                {
                  "id" : "Extension.url",
                  "path" : "Extension.url",
                  "fixedUri" : "http://hl7.org/fhir/StructureDefinition/alternate-canonical"
                },
                {
                  "id" : "Extension.value[x]",
                  "path" : "Extension.value[x]",
                  "min" : 1,
                  "type" : [{
                    "code" : "url"
                  }]
                }]
              }
            }
            """;

        string datatypeJson = """
            {
              "resourceType" : "StructureDefinition",
              "id" : "datatype",
              "text" : {
                "status" : "extensions",
                "div" : "<div xmlns=\"http://www.w3.org/1999/xhtml\"><p class=\"res-header-id\"><b>Generated Narrative: StructureDefinition datatype</b></p><a name=\"datatype\"> </a><a name=\"hcdatatype\"> </a><a name=\"datatype-en-US\"> </a><table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" style=\"border: 0px #F0F0F0 solid; font-size: 11px; font-family: verdana; vertical-align: top;\"><tr style=\"border: 1px #F0F0F0 solid; font-size: 11px; font-family: verdana; vertical-align: top\"><th style=\"vertical-align: top; text-align : left; background-color: white; border: 0px #F0F0F0 solid; padding:0px 4px 0px 4px; padding-top: 3px; padding-bottom: 3px\" class=\"hierarchy\"><a href=\"https://build.fhir.org/ig/FHIR/ig-guidance/readingIgs.html#table-views\" title=\"The logical name of the element\">Name</a></th><th style=\"vertical-align: top; text-align : left; background-color: white; border: 0px #F0F0F0 solid; padding:0px 4px 0px 4px; padding-top: 3px; padding-bottom: 3px\" class=\"hierarchy\"><a href=\"https://build.fhir.org/ig/FHIR/ig-guidance/readingIgs.html#table-views\" title=\"Information about the use of the element\">Flags</a></th><th style=\"vertical-align: top; text-align : left; background-color: white; border: 0px #F0F0F0 solid; padding:0px 4px 0px 4px; padding-top: 3px; padding-bottom: 3px\" class=\"hierarchy\"><a href=\"https://build.fhir.org/ig/FHIR/ig-guidance/readingIgs.html#table-views\" title=\"Minimum and Maximum # of times the element can appear in the instance\">Card.</a></th><th style=\"width: 100px\" class=\"hierarchy\"><a href=\"https://build.fhir.org/ig/FHIR/ig-guidance/readingIgs.html#table-views\" title=\"Reference to the type of the element\">Type</a></th><th style=\"vertical-align: top; text-align : left; background-color: white; border: 0px #F0F0F0 solid; padding:0px 4px 0px 4px; padding-top: 3px; padding-bottom: 3px\" class=\"hierarchy\"><a href=\"https://build.fhir.org/ig/FHIR/ig-guidance/readingIgs.html#table-views\" title=\"Additional information about the element\">Description &amp; Constraints</a><span style=\"float: right\"><a href=\"https://build.fhir.org/ig/FHIR/ig-guidance/readingIgs.html#table-views\" title=\"Legend for this format\"><img src=\"data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAABmJLR0QA/wD/AP+gvaeTAAAACXBIWXMAAAsTAAALEwEAmpwYAAAAB3RJTUUH3goXBCwdPqAP0wAAAldJREFUOMuNk0tIlFEYhp9z/vE2jHkhxXA0zJCMitrUQlq4lnSltEqCFhFG2MJFhIvIFpkEWaTQqjaWZRkp0g26URZkTpbaaOJkDqk10szoODP//7XIMUe0elcfnPd9zsfLOYplGrpRwZaqTtw3K7PtGem7Q6FoidbGgqHVy/HRb669R+56zx7eRV1L31JGxYbBtjKK93cxeqfyQHbehkZbUkK20goELEuIzEd+dHS+qz/Y8PTSif0FnGkbiwcAjHaU1+QWOptFiyCLp/LnKptpqIuXHx6rbR26kJcBX3yLgBfnd7CxwJmflpP2wUg0HIAoUUpZBmKzELGWcN8nAr6Gpu7tLU/CkwAaoKTWRSQyt89Q8w6J+oVQkKnBoblH7V0PPvUOvDYXfopE/SJmALsxnVm6LbkotrUtNowMeIrVrBcBpaMmdS0j9df7abpSuy7HWehwJdt1lhVwi/J58U5beXGAF6c3UXLycw1wdFklArBn87xdh0ZsZtArghBdAA3+OEDVubG4UEzP6x1FOWneHh2VDAHBAt80IbdXDcesNoCvs3E5AFyNSU5nbrDPZpcUEQQTFZiEVx+51fxMhhyJEAgvlriadIJZZksRuwBYMOPBbO3hePVVqgEJhFeUuFLhIPkRP6BQLIBrmMenujm/3g4zc398awIe90Zb5A1vREALqneMcYgP/xVQWlG+Ncu5vgwwlaUNx+3799rfe96u9K0JSDXcOzOTJg4B6IgmXfsygc7/Bvg9g9E58/cDVmGIBOP/zT8Bz1zqWqpbXIsd0O9hajXfL6u4BaOS6SeWAAAAAElFTkSuQmCC\" alt=\"doco\" style=\"background-color: inherit\"/></a></span></th></tr><tr style=\"border: 0px #F0F0F0 solid; padding:0px; vertical-align: top; background-color: white\"><td style=\"vertical-align: top; text-align : left; background-color: white; border: 0px #F0F0F0 solid; padding:0px 4px 0px 4px; white-space: nowrap; background-image: url(tbl_bck1.png)\" class=\"hierarchy\"><img src=\"tbl_spacer.png\" alt=\".\" style=\"background-color: inherit\" class=\"hierarchy\"/><img src=\"icon_element.gif\" alt=\".\" style=\"background-color: white; background-color: inherit\" title=\"Element\" class=\"hierarchy\"/> <a href=\"StructureDefinition-datatype-definitions.html#Extension\" title=\"Used when the actual type is not allowed by the definition of the element. In general, this should only arise when wrangling between versions using cross-version extensions - see [Cross Version Extensions](versions.html#extensions).\">Extension</a><a name=\"Extension\"> </a></td><td style=\"vertical-align: top; text-align : left; background-color: white; border: 0px #F0F0F0 solid; padding:0px 4px 0px 4px\" class=\"hierarchy\"/><td style=\"vertical-align: top; text-align : left; background-color: white; border: 0px #F0F0F0 solid; padding:0px 4px 0px 4px\" class=\"hierarchy\">0..1</td><td style=\"vertical-align: top; text-align : left; background-color: white; border: 0px #F0F0F0 solid; padding:0px 4px 0px 4px\" class=\"hierarchy\"><a href=\"http://hl7.org/fhir/R5/extensibility.html#Extension\">Extension</a></td><td style=\"vertical-align: top; text-align : left; background-color: white; border: 0px #F0F0F0 solid; padding:0px 4px 0px 4px\" class=\"hierarchy\">Alternative datatype name (for cross version extensions)</td></tr>\r\n<tr style=\"border: 0px #F0F0F0 solid; padding:0px; vertical-align: top; background-color: #F7F7F7\"><td style=\"vertical-align: top; text-align : left; background-color: #F7F7F7; border: 0px #F0F0F0 solid; padding:0px 4px 0px 4px; white-space: nowrap; background-image: url(tbl_bck10.png)\" class=\"hierarchy\"><img src=\"tbl_spacer.png\" alt=\".\" style=\"background-color: inherit\" class=\"hierarchy\"/><img src=\"tbl_vjoin.png\" alt=\".\" style=\"background-color: inherit\" class=\"hierarchy\"/><img src=\"icon_extension_simple.png\" alt=\".\" style=\"background-color: #F7F7F7; background-color: inherit\" title=\"Simple Extension\" class=\"hierarchy\"/> <span style=\"text-decoration:line-through\">extension</span><a name=\"Extension.extension\"> </a></td><td style=\"vertical-align: top; text-align : left; background-color: #F7F7F7; border: 0px #F0F0F0 solid; padding:0px 4px 0px 4px\" class=\"hierarchy\"/><td style=\"vertical-align: top; text-align : left; background-color: #F7F7F7; border: 0px #F0F0F0 solid; padding:0px 4px 0px 4px\" class=\"hierarchy\"><span style=\"text-decoration:line-through\"/><span style=\"text-decoration:line-through\">0</span><span style=\"text-decoration:line-through\">..</span><span style=\"text-decoration:line-through\">0</span></td><td style=\"vertical-align: top; text-align : left; background-color: #F7F7F7; border: 0px #F0F0F0 solid; padding:0px 4px 0px 4px\" class=\"hierarchy\"/><td style=\"vertical-align: top; text-align : left; background-color: #F7F7F7; border: 0px #F0F0F0 solid; padding:0px 4px 0px 4px\" class=\"hierarchy\"/></tr>\r\n<tr style=\"border: 0px #F0F0F0 solid; padding:0px; vertical-align: top; background-color: white\"><td style=\"vertical-align: top; text-align : left; background-color: white; border: 0px #F0F0F0 solid; padding:0px 4px 0px 4px; white-space: nowrap; background-image: url(tbl_bck10.png)\" class=\"hierarchy\"><img src=\"tbl_spacer.png\" alt=\".\" style=\"background-color: inherit\" class=\"hierarchy\"/><img src=\"tbl_vjoin.png\" alt=\".\" style=\"background-color: inherit\" class=\"hierarchy\"/><img src=\"icon_element.gif\" alt=\".\" style=\"background-color: white; background-color: inherit\" title=\"Element\" class=\"hierarchy\"/> <a href=\"StructureDefinition-datatype-definitions.html#Extension.url\">url</a><a name=\"Extension.url\"> </a></td><td style=\"vertical-align: top; text-align : left; background-color: white; border: 0px #F0F0F0 solid; padding:0px 4px 0px 4px\" class=\"hierarchy\"/><td style=\"vertical-align: top; text-align : left; background-color: white; border: 0px #F0F0F0 solid; padding:0px 4px 0px 4px\" class=\"hierarchy\"><span style=\"opacity: 0.5\">1</span><span style=\"opacity: 0.5\">..</span><span style=\"opacity: 0.5\">1</span></td><td style=\"vertical-align: top; text-align : left; background-color: white; border: 0px #F0F0F0 solid; padding:0px 4px 0px 4px\" class=\"hierarchy\"><a style=\"opacity: 0.5\" href=\"http://hl7.org/fhir/R5/datatypes.html#uri\">uri</a></td><td style=\"vertical-align: top; text-align : left; background-color: white; border: 0px #F0F0F0 solid; padding:0px 4px 0px 4px\" class=\"hierarchy\"><span style=\"color: darkgreen\">&quot;http://hl7.org/fhir/StructureDefinition/_datatype&quot;</span></td></tr>\r\n<tr style=\"border: 0px #F0F0F0 solid; padding:0px; vertical-align: top; background-color: #F7F7F7\"><td style=\"vertical-align: top; text-align : left; background-color: #F7F7F7; border: 0px #F0F0F0 solid; padding:0px 4px 0px 4px; white-space: nowrap; background-image: url(tbl_bck00.png)\" class=\"hierarchy\"><img src=\"tbl_spacer.png\" alt=\".\" style=\"background-color: inherit\" class=\"hierarchy\"/><img src=\"tbl_vjoin_end.png\" alt=\".\" style=\"background-color: inherit\" class=\"hierarchy\"/><img src=\"icon_primitive.png\" alt=\".\" style=\"background-color: #F7F7F7; background-color: inherit\" title=\"Primitive Data Type\" class=\"hierarchy\"/> <a href=\"StructureDefinition-datatype-definitions.html#Extension.value[x]\">value[x]</a><a name=\"Extension.value_x_\"> </a></td><td style=\"vertical-align: top; text-align : left; background-color: #F7F7F7; border: 0px #F0F0F0 solid; padding:0px 4px 0px 4px\" class=\"hierarchy\"/><td style=\"vertical-align: top; text-align : left; background-color: #F7F7F7; border: 0px #F0F0F0 solid; padding:0px 4px 0px 4px\" class=\"hierarchy\">1..<span style=\"opacity: 0.5\">1</span></td><td style=\"vertical-align: top; text-align : left; background-color: #F7F7F7; border: 0px #F0F0F0 solid; padding:0px 4px 0px 4px\" class=\"hierarchy\"><a href=\"http://hl7.org/fhir/R5/datatypes.html#string\">string</a></td><td style=\"vertical-align: top; text-align : left; background-color: #F7F7F7; border: 0px #F0F0F0 solid; padding:0px 4px 0px 4px\" class=\"hierarchy\"><span style=\"opacity: 0.5\">Value of extension</span></td></tr>\r\n<tr><td colspan=\"5\" class=\"hierarchy\"><br/><a href=\"https://build.fhir.org/ig/FHIR/ig-guidance/readingIgs.html#table-views\" title=\"Legend for this format\"><img src=\"data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAABmJLR0QA/wD/AP+gvaeTAAAACXBIWXMAAAsTAAALEwEAmpwYAAAAB3RJTUUH3goXBCwdPqAP0wAAAldJREFUOMuNk0tIlFEYhp9z/vE2jHkhxXA0zJCMitrUQlq4lnSltEqCFhFG2MJFhIvIFpkEWaTQqjaWZRkp0g26URZkTpbaaOJkDqk10szoODP//7XIMUe0elcfnPd9zsfLOYplGrpRwZaqTtw3K7PtGem7Q6FoidbGgqHVy/HRb669R+56zx7eRV1L31JGxYbBtjKK93cxeqfyQHbehkZbUkK20goELEuIzEd+dHS+qz/Y8PTSif0FnGkbiwcAjHaU1+QWOptFiyCLp/LnKptpqIuXHx6rbR26kJcBX3yLgBfnd7CxwJmflpP2wUg0HIAoUUpZBmKzELGWcN8nAr6Gpu7tLU/CkwAaoKTWRSQyt89Q8w6J+oVQkKnBoblH7V0PPvUOvDYXfopE/SJmALsxnVm6LbkotrUtNowMeIrVrBcBpaMmdS0j9df7abpSuy7HWehwJdt1lhVwi/J58U5beXGAF6c3UXLycw1wdFklArBn87xdh0ZsZtArghBdAA3+OEDVubG4UEzP6x1FOWneHh2VDAHBAt80IbdXDcesNoCvs3E5AFyNSU5nbrDPZpcUEQQTFZiEVx+51fxMhhyJEAgvlriadIJZZksRuwBYMOPBbO3hePVVqgEJhFeUuFLhIPkRP6BQLIBrmMenujm/3g4zc398awIe90Zb5A1vREALqneMcYgP/xVQWlG+Ncu5vgwwlaUNx+3799rfe96u9K0JSDXcOzOTJg4B6IgmXfsygc7/Bvg9g9E58/cDVmGIBOP/zT8Bz1zqWqpbXIsd0O9hajXfL6u4BaOS6SeWAAAAAElFTkSuQmCC\" alt=\"doco\" style=\"background-color: inherit\"/> Documentation for this format</a></td></tr></table></div>"
              },
              "extension" : [{
                "url" : "http://hl7.org/fhir/StructureDefinition/structuredefinition-wg",
                "valueCode" : "fhir"
              },
              {
                "url" : "http://hl7.org/fhir/StructureDefinition/structuredefinition-fmm",
                "valueInteger" : 2
              },
              {
                "url" : "http://hl7.org/fhir/StructureDefinition/structuredefinition-standards-status",
                "valueCode" : "trial-use"
              },
              {
                "url" : "http://hl7.org/fhir/StructureDefinition/structuredefinition-type-characteristics",
                "valueCode" : "can-bind"
              }],
              "url" : "http://hl7.org/fhir/StructureDefinition/_datatype",
              "identifier" : [{
                "system" : "urn:ietf:rfc:3986",
                "value" : "urn:oid:2.16.840.1.113883.4.642.5.1774"
              }],
              "version" : "5.2.0",
              "name" : "Datatype",
              "title" : "Datatype",
              "status" : "draft",
              "experimental" : false,
              "date" : "2014-04-27",
              "publisher" : "HL7 International / FHIR Infrastructure",
              "contact" : [{
                "telecom" : [{
                  "system" : "url",
                  "value" : "http://www.hl7.org/Special/committees/fiwg"
                }]
              }],
              "description" : "Used when the actual type is not allowed by the definition of the element. In general, this should only arise when wrangling between versions using cross-version extensions - see [Cross Version Extensions](versions.html#extensions).",
              "jurisdiction" : [{
                "coding" : [{
                  "system" : "http://unstats.un.org/unsd/methods/m49/m49.htm",
                  "code" : "001"
                }]
              }],
              "fhirVersion" : "5.0.0",
              "mapping" : [{
                "identity" : "rim",
                "uri" : "http://hl7.org/v3",
                "name" : "RIM Mapping"
              }],
              "kind" : "complex-type",
              "abstract" : false,
              "context" : [{
                "type" : "element",
                "expression" : "Base"
              }],
              "type" : "Extension",
              "baseDefinition" : "http://hl7.org/fhir/StructureDefinition/Extension",
              "derivation" : "constraint",
              "snapshot" : {
                "extension" : [{
                  "url" : "http://hl7.org/fhir/tools/StructureDefinition/snapshot-base-version",
                  "valueString" : "5.0.0"
                }],
                "element" : [{
                  "id" : "Extension",
                  "path" : "Extension",
                  "short" : "Alternative datatype name (for cross version extensions)",
                  "definition" : "Used when the actual type is not allowed by the definition of the element. In general, this should only arise when wrangling between versions using cross-version extensions - see [Cross Version Extensions](versions.html#extensions).",
                  "min" : 0,
                  "max" : "1",
                  "base" : {
                    "path" : "Extension",
                    "min" : 0,
                    "max" : "*"
                  },
                  "constraint" : [{
                    "key" : "ele-1",
                    "severity" : "error",
                    "human" : "All FHIR elements must have a @value or children",
                    "expression" : "hasValue() or (children().count() > id.count())",
                    "source" : "http://hl7.org/fhir/StructureDefinition/Element"
                  },
                  {
                    "key" : "ext-1",
                    "severity" : "error",
                    "human" : "Must have either extensions or value[x], not both",
                    "expression" : "extension.exists() != value.exists()",
                    "source" : "http://hl7.org/fhir/StructureDefinition/Extension"
                  }],
                  "isModifier" : false
                },
                {
                  "id" : "Extension.id",
                  "path" : "Extension.id",
                  "representation" : ["xmlAttr"],
                  "short" : "Unique id for inter-element referencing",
                  "definition" : "Unique id for the element within a resource (for internal references). This may be any string value that does not contain spaces.",
                  "min" : 0,
                  "max" : "1",
                  "base" : {
                    "path" : "Element.id",
                    "min" : 0,
                    "max" : "1"
                  },
                  "type" : [{
                    "extension" : [{
                      "url" : "http://hl7.org/fhir/StructureDefinition/structuredefinition-fhir-type",
                      "valueUrl" : "id"
                    }],
                    "code" : "http://hl7.org/fhirpath/System.String"
                  }],
                  "condition" : ["ele-1"],
                  "isModifier" : false,
                  "isSummary" : false,
                  "mapping" : [{
                    "identity" : "rim",
                    "map" : "n/a"
                  }]
                },
                {
                  "id" : "Extension.extension",
                  "path" : "Extension.extension",
                  "slicing" : {
                    "discriminator" : [{
                      "type" : "value",
                      "path" : "url"
                    }],
                    "description" : "Extensions are always sliced by (at least) url",
                    "rules" : "open"
                  },
                  "short" : "Extension",
                  "definition" : "An Extension",
                  "min" : 0,
                  "max" : "0",
                  "base" : {
                    "path" : "Element.extension",
                    "min" : 0,
                    "max" : "*"
                  },
                  "type" : [{
                    "code" : "Extension"
                  }],
                  "constraint" : [{
                    "key" : "ele-1",
                    "severity" : "error",
                    "human" : "All FHIR elements must have a @value or children",
                    "expression" : "hasValue() or (children().count() > id.count())",
                    "source" : "http://hl7.org/fhir/StructureDefinition/Element"
                  },
                  {
                    "key" : "ext-1",
                    "severity" : "error",
                    "human" : "Must have either extensions or value[x], not both",
                    "expression" : "extension.exists() != value.exists()",
                    "source" : "http://hl7.org/fhir/StructureDefinition/Extension"
                  }],
                  "isModifier" : false,
                  "isSummary" : false
                },
                {
                  "id" : "Extension.url",
                  "path" : "Extension.url",
                  "representation" : ["xmlAttr"],
                  "short" : "identifies the meaning of the extension",
                  "definition" : "Source of the definition for the extension code - a logical name or a URL.",
                  "comment" : "The definition may point directly to a computable or human-readable definition of the extensibility codes, or it may be a logical URI as declared in some other specification. The definition SHALL be a URI for the Structure Definition defining the extension.",
                  "min" : 1,
                  "max" : "1",
                  "base" : {
                    "path" : "Extension.url",
                    "min" : 1,
                    "max" : "1"
                  },
                  "type" : [{
                    "extension" : [{
                      "url" : "http://hl7.org/fhir/StructureDefinition/structuredefinition-fhir-type",
                      "valueUrl" : "uri"
                    }],
                    "code" : "http://hl7.org/fhirpath/System.String"
                  }],
                  "fixedUri" : "http://hl7.org/fhir/StructureDefinition/_datatype",
                  "isModifier" : false,
                  "isSummary" : false,
                  "mapping" : [{
                    "identity" : "rim",
                    "map" : "N/A"
                  }]
                },
                {
                  "id" : "Extension.value[x]",
                  "path" : "Extension.value[x]",
                  "short" : "Value of extension",
                  "definition" : "Value of extension - must be one of a constrained set of the data types (see [Extensibility](http://hl7.org/fhir/R5/extensibility.html) for a list).",
                  "min" : 1,
                  "max" : "1",
                  "base" : {
                    "path" : "Extension.value[x]",
                    "min" : 0,
                    "max" : "1"
                  },
                  "type" : [{
                    "code" : "string"
                  }],
                  "condition" : ["ext-1"],
                  "constraint" : [{
                    "key" : "ele-1",
                    "severity" : "error",
                    "human" : "All FHIR elements must have a @value or children",
                    "expression" : "hasValue() or (children().count() > id.count())",
                    "source" : "http://hl7.org/fhir/StructureDefinition/Element"
                  }],
                  "isModifier" : false,
                  "isSummary" : false,
                  "mapping" : [{
                    "identity" : "rim",
                    "map" : "N/A"
                  }]
                }]
              },
              "differential" : {
                "element" : [{
                  "id" : "Extension",
                  "path" : "Extension",
                  "short" : "Alternative datatype name (for cross version extensions)",
                  "definition" : "Used when the actual type is not allowed by the definition of the element. In general, this should only arise when wrangling between versions using cross-version extensions - see [Cross Version Extensions](versions.html#extensions).",
                  "min" : 0,
                  "max" : "1"
                },
                {
                  "id" : "Extension.extension",
                  "path" : "Extension.extension",
                  "max" : "0"
                },
                {
                  "id" : "Extension.url",
                  "path" : "Extension.url",
                  "fixedUri" : "http://hl7.org/fhir/StructureDefinition/_datatype"
                },
                {
                  "id" : "Extension.value[x]",
                  "path" : "Extension.value[x]",
                  "min" : 1,
                  "type" : [{
                    "code" : "string"
                  }]
                }]
              }
            }
            """;

        FhirJsonParser parser = new FhirJsonParser(new ParserSettings()
        {
            AcceptUnknownMembers = true,
            AllowUnrecognizedEnums = true,
            PermissiveParsing = true,
        });

        StructureDefinition alternateCanonical = parser.Parse<StructureDefinition>(alternateCanonicalJson);
        dc.AddStructureDefinition(alternateCanonical, dc.FhirSequence, "hl7.fhir.uv.extensions", "5.2.0");

        StructureDefinition datatype = parser.Parse<StructureDefinition>(datatypeJson);
        dc.AddStructureDefinition(datatype, dc.FhirSequence, "hl7.fhir.uv.extensions", "5.2.0");
    }
}
