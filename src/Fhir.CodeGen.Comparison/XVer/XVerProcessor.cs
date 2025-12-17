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
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Fhir.CodeGen.Common.Extensions;
using Fhir.CodeGen.Common.Models;
using Fhir.CodeGen.Common.Packaging;
using Fhir.CodeGen.Common.Utils;
using Fhir.CodeGen.Comparison.CompareTool;
using Fhir.CodeGen.Comparison.Models;
using Fhir.CodeGen.Lib.Configuration;
using Fhir.CodeGen.Lib.FhirExtensions;
using Fhir.CodeGen.Lib.Language;
using Fhir.CodeGen.Lib.Models;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Hl7.Fhir.Specification.Snapshot;
using Hl7.Fhir.Utility;
using Hl7.FhirPath.Sprache;
using Microsoft.Extensions.Logging;
using static System.Net.Mime.MediaTypeNames;
using CMR = Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship;
using Tasks = System.Threading.Tasks;

namespace Fhir.CodeGen.Comparison.XVer;

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
public partial class XVerProcessor
{
    private string _crossDefinitionVersion = "0.0.1-snapshot-2";
    private static readonly DateTimeOffset _runTime = DateTimeOffset.UtcNow;

    /// <summary>
    /// The directions for comparison (Up and Down).
    /// </summary>
    internal static readonly ComparisonDirection[] _directions = [ComparisonDirection.Up, ComparisonDirection.Down];

    /// <summary>
    /// Set of ValueSet and CodeSystem URLs to exclude from processing.
    /// </summary>
    internal static readonly HashSet<string> _exclusionSet =
    [
        "http://hl7.org/fhir/ValueSet/ucum-units",
        "http://hl7.org/fhir/ValueSet/all-languages",
        "http://tools.ietf.org/html/bcp47",             // DSTU2 version of all-languages
        "http://hl7.org/fhir/ValueSet/mimetypes",
        "http://hl7.org/fhir/ValueSet/timezones",
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
    private ILoggerFactory _loggerFactory;
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
        _loggerFactory = config.LogFactory;
        _logger = config.LogFactory.CreateLogger<XVerProcessor>();

        string path = string.IsNullOrEmpty(_config.CrossVersionDbPath)
            ? Path.Combine(_config.CrossVersionMapSourcePath, "db")
            : _config.CrossVersionDbPath;

        if (path.EndsWith(".sqlite") || path.EndsWith(".db"))
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

        _loggerFactory = loggerFactory;
        _logger = loggerFactory.CreateLogger<XVerProcessor>();

        _dbPath = db.DbFilePath;
        _dbName = db.DbFileName;

        _comparisonCache = [];
        _db = db;
    }

    /// <summary>
    /// Loads FHIR package definition collections based on configuration.
    /// </summary>
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
            using Lib.Loader.PackageLoader loader = new(_config, new()
            {
                JsonModel = Lib.Loader.LoaderOptions.JsonDeserializationModel.SystemTextJson,
            });

            List<string> loadDirectives = FhirReleases.TryGetSequence(directive, out FhirReleases.FhirSequenceCodes packageSequence)
                ? packageSequence switch
                {
                    FhirReleases.FhirSequenceCodes.R5 => [ directive, "hl7.terminology@5.0.1"],
                    _ => [ directive ]
                }
                : [directive];

            DefinitionCollection loaded = loader.LoadPackages(loadDirectives).Result
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
            case "wip":
                //LoadDatabase(true, true);
                LoadFhirCrossVersionMaps(preferV1Maps: true);
                //BuildComparisonPairs();
                //CompareInDatabase();
                //GenerateOutcomesFromComparisons();
                //WriteFhirFromDbOutcomes();
                break;

            case "load":
                LoadDatabase(true, true);
                LoadFhirCrossVersionMaps(preferV1Maps: true);
                break;

            case "load-base":
                LoadDatabase(true, true);
                break;

            case "load-maps":
                LoadDatabase(_config.ReloadDatabase, true);
                LoadFhirCrossVersionMaps(preferV1Maps: true);
                break;

            //case "create-db":
            //    LoadDatabase(true, true);
            //    LoadFhirCrossVersionMaps(preferV1Maps: true);
            //    break;

            case "discover":
                LoadDatabase(_config.ReloadDatabase, true);
                BuildComparisonPairs();
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

            case "outcomes":
                LoadDatabase(false, false);
                GenerateOutcomesFromComparisons();
                break;

            //case "docs":
            //    LoadDatabase(false, false);
            //    WriteDocsFromDatabase();
            //    break;

            //case "docs-vs":
            //    LoadDatabase(false, false);
            //    WriteDocsFromDatabase(FhirArtifactClassEnum.ValueSet);
            //    break;

            //case "docs-sd":
            //    LoadDatabase(false, false);
            //    WriteDocsFromDatabase(FhirArtifactClassEnum.Resource);
            //    break;

            case "fhir":
                LoadDatabase(false, false);
                WriteFhirFromDbOutcomes();
                //WriteFhirFromDatabase();
                break;

            default:
                LoadDatabase(true, true);
                LoadFhirCrossVersionMaps(preferV1Maps: true);
                BuildComparisonPairs();
                CompareInDatabase();
                GenerateOutcomesFromComparisons();
                //WriteDocsFromDatabase();
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
                    string.IsNullOrEmpty(_config.CrossVersionSourceDb) &&
                    (_config.CrossVersionSourceDb.ToLowerInvariant() != _config.CrossVersionDbPath.ToLowerInvariant()))
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
    /// Builds the comparison pairs in the loaded database, can filter by artifact type.
    /// </summary>
    /// <param name="artifactFilter"></param>
    /// <exception cref="Exception"></exception>
    public void BuildComparisonPairs(FhirArtifactClassEnum? artifactFilter = null)
    {
        if (_db == null)
        {
            throw new Exception("Cannot build comparison pairs without a loaded database!");
        }

        FhirDbComparer dbComparer = new(_db, _config.LogFactory);
        dbComparer.BuildComparisonPairs(artifactFilter, _config.ComparisonPairFilterKeys);
    }

    /// <summary>
    /// Runs the comparison process in the loaded database, can filter by artifact type.
    /// </summary>
    /// <param name="artifactFilter">Optional artifact type filter.</param>
    public void CompareInDatabase(FhirArtifactClassEnum? artifactFilter = null)
    {
        if (_db == null)
        {
            throw new Exception("Comparison cannot run without a loaded database!");
        }

        FhirDbComparer dbComparer = new(_db, _config.LogFactory);
        dbComparer.Compare(artifactFilter, _config.ComparisonPairFilterKeys);
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
        public Dictionary<string, string?> BasicElements { get; init; } = [];
        /// <summary>
        /// Gets the set of allowed extension types.
        /// </summary>
        public HashSet<string> AllowedExtensionTypes { get; init; } = [];
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
        //// need definitions loaded in order for existing cross-version maps to be usable
        //if (_definitions.Length == 0)
        //{
        //    loadDefinitionCollections();
        //}

        //if (_definitions.Length < 2)
        //{
        //    throw new InvalidOperationException("At least two definitions are required to compare.");
        //}

        if (_db == null)
        {
            LoadDatabase(false, true);
            if (_db == null)
            {
                throw new Exception($"Failed to create or load a comparison database!");
            }
        }

        // if this is a core comparison and we have a location, try to load existing cross-version maps
        if (!string.IsNullOrEmpty(_config.CrossVersionMapSourcePath))
        {
            _ = _db.TryLoadCrossVersionSourceMaps(
                _config.CrossVersionMapSourcePath,
                _config.UseInternalTypeMaps);
            //_ = _db.TryLoadFhirCrossVersionMaps(_config.CrossVersionMapSourcePath);
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



    internal static (string idLong, string idShort) GenerateArtifactId(
        string sourcePackageShortName,
        string sourceArtifactId,
        string targetPackageShortName)
    {
        string idLong = $"{sourcePackageShortName}-{sourceArtifactId}-for-{targetPackageShortName}";

        if (idLong.Length <= 64)
        {
            return (idLong, idLong);
        }

        string idShort;

        string[] sourceIdComponents = sourceArtifactId.Split('-');
        if (sourceArtifactId.StartsWith("v3-", StringComparison.Ordinal) ||
            sourceArtifactId.StartsWith("v2-", StringComparison.Ordinal))
        {
            // the second component is a PascalCase name, extract it into components - e.g. ActInvoiceElementModifier -> [Act, Invoice, Element, Modifier]
            string[] pascalComponents = Regex.Matches(sourceIdComponents[1], @"([A-Z][a-z0-9]+)")
                .Select(m => m.Value)
                .ToArray();

            // use the prefix (v2 or v3) plus the first word, capitals in the middle, and the last word
            // e.g. v3-ActInvoiceElementModifier -> v3ActIEModifier
            idShort = $"{sourcePackageShortName}" +
                $"-{sourceIdComponents[0]}" +
                $"{pascalComponents[0]}" +
                $"{string.Join(string.Empty, pascalComponents[1..^1].Select(c => c[0]))}" +
                $"{pascalComponents[^1]}" +
                $"-for-{targetPackageShortName}";

        }
        else if (sourceIdComponents.Length > 2)
        {
            // use the first and last components completely, but abbreviate the middle components
            idShort = $"{sourcePackageShortName}" +
                $"-{sourceIdComponents[0]}" +
                $"-{string.Join('-', sourceIdComponents.Skip(1).Take(sourceIdComponents.Length - 2).Select(c => c.Substring(0, int.Min(c.Length, 3))))}" +
                $"-{sourceIdComponents[^1]}" +
                $"-for-{targetPackageShortName}";
        }
        else
        {
            // truncate the source ID so it all fits
            idShort = $"{sourcePackageShortName}-{sourceArtifactId.Substring(0, 50)}-for-{targetPackageShortName}";
        }

        return (idLong, idShort);
    }

    internal static (string idLong, string idShort) GenerateArtifactId(
        string sourcePackageShortName,
        string sourceArtifactId,
        string targetPackageShortName,
        string? targetArtifactId)
    {
        if ((targetArtifactId is null) ||
            (sourceArtifactId.Equals(targetArtifactId, StringComparison.OrdinalIgnoreCase)))
        {
            return GenerateArtifactId(sourcePackageShortName, sourceArtifactId, targetPackageShortName);
        }

        string idLong = $"{sourcePackageShortName}-{sourceArtifactId}-for-{targetPackageShortName}-{targetArtifactId}";

        if (idLong.Length <= 64)
        {
            return (idLong, idLong);
        }

        string shortSource;

        string[] sourceIdComponents = sourceArtifactId.Split('-');
        if (sourceArtifactId.StartsWith("v3-", StringComparison.Ordinal) ||
            sourceArtifactId.StartsWith("v2-", StringComparison.Ordinal))
        {
            // the second component is a PascalCase name, extract it into components - e.g. ActInvoiceElementModifier -> [Act, Invoice, Element, Modifier]
            string[] pascalComponents = Regex.Matches(sourceIdComponents[1], @"([A-Z][a-z0-9]+)")
                .Select(m => m.Value)
                .ToArray();

            // use the prefix (v2 or v3) plus the first word, capitals in the middle, and the last word
            // e.g. v3-ActInvoiceElementModifier -> v3ActIEModifier
            shortSource = $"{sourceIdComponents[0]}" +
                $"{pascalComponents[0]}" +
                $"{string.Join(string.Empty, pascalComponents[1..^1].Select(c => c[0]))}" +
                $"{pascalComponents[^1]}";
        }
        else if (sourceIdComponents.Length > 2)
        {
            // use the first and last components completely, but abbreviate the middle components
            shortSource = $"{sourceIdComponents[0]}" +
                $"-{string.Join('-', sourceIdComponents.Skip(1).Take(sourceIdComponents.Length - 2).Select(c => c.Substring(0, int.Min(c.Length, 3))))}" +
                $"-{sourceIdComponents[^1]}";
        }
        else
        {
            // truncate the source ID so it all fits
            shortSource = sourceArtifactId.Substring(0, 25);
        }

        string shortTarget;

        string[] targetIdComponents = targetArtifactId.Split('-');
        if (targetArtifactId.StartsWith("v3-", StringComparison.Ordinal) ||
            targetArtifactId.StartsWith("v2-", StringComparison.Ordinal))
        {
            // the second component is a PascalCase name, extract it into components - e.g. ActInvoiceElementModifier -> [Act, Invoice, Element, Modifier]
            string[] pascalComponents = Regex.Matches(targetIdComponents[1], @"([A-Z][a-z0-9]+)")
                .Select(m => m.Value)
                .ToArray();

            // use the prefix (v2 or v3) plus the first word, capitals in the middle, and the last word
            // e.g. v3-ActInvoiceElementModifier -> v3ActIEModifier
            shortTarget = $"{targetIdComponents[0]}" +
                $"{pascalComponents[0]}" +
                $"{string.Join(string.Empty, pascalComponents[1..^1].Select(c => c[0]))}" +
                $"{pascalComponents[^1]}";
        }
        else if (targetIdComponents.Length > 2)
        {
            // use the first and last components completely, but abbreviate the middle components
            shortTarget = $"{targetIdComponents[0]}" +
                $"-{string.Join('-', targetIdComponents.Skip(1).Take(targetIdComponents.Length - 2).Select(c => c.Substring(0, int.Min(c.Length, 3))))}" +
                $"-{targetIdComponents[^1]}";
        }
        else
        {
            // truncate the target ID so it all fits
            shortTarget = targetArtifactId.Substring(0, 25);
        }

        string idShort = $"{sourcePackageShortName}-{shortSource}-for-{targetPackageShortName}-{shortTarget}";

        return (idLong, idShort);
    }

}
