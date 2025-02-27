using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;
using Hl7.Fhir.Model;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Health.Fhir.CodeGen.FhirExtensions;
using Microsoft.Health.Fhir.CodeGen.Models;
using Microsoft.Health.Fhir.CodeGen.Utils;
using Microsoft.Health.Fhir.CodeGenCommon.Extensions;
using Microsoft.Health.Fhir.CodeGenCommon.FhirExtensions;
using Microsoft.Health.Fhir.CodeGenCommon.Models;
using Microsoft.Health.Fhir.CodeGenCommon.Packaging;
using Microsoft.Health.Fhir.CodeGenCommon.Utils;
using Microsoft.Health.Fhir.Comparison.CompareTool;
using Microsoft.Health.Fhir.Comparison.Extensions;
using Microsoft.Health.Fhir.Comparison.Models;
using Newtonsoft.Json.Linq;
using CMR = Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship;

namespace Microsoft.Health.Fhir.Comparison.Models;


internal static partial class ContentDatabaseLogMessages
{
    [LoggerMessage(Level = LogLevel.Warning, Message = "Failed to resolve ValueSet {vsUrl} in {sourcePackage}:{sourceVersion}.")]
    internal static partial void LogFailedToResolveVs(this ILogger logger, string vsUrl, string sourcePackage, string sourceVersion);
}


public class ComparisonDatabase : IDisposable
{
    private const string _canonicalRootCrossVersion = "http://hl7.org/fhir/uv/xver/";
    private bool _disposedValue;

    private ILoggerFactory? _loggerFactory;
    private ILogger _logger;

    private class DcInfoRec
    {
        public required FhirReleases.FhirSequenceCodes FhirSequence;
        public required string PackageCanonical;
        public required string RLiteral;
        public required string ShortVersion;
        public required string ShortVersionUrlSegment;
        public required string PackageVersion;
    }

    private (DefinitionCollection dc, DcInfoRec info)[]? _definitions = null;

    private bool _isCoreComparison;
    private bool _isVersionComparison;

    private string _dbPath;
    private string _dbName;

    private IDbConnection _dbConnection;

    private int _dbValueSetIndex = 0;
    private int _dbConceptIndex = 0;
    private int _dbStructureIndex = 0;
    private int _dbElementIndex = 0;
    private int _dbValueSetComparisonIndex = 0;
    private int _dbConceptComparisonIndex = 0;
    private int _dbUnresolvedConceptComparisonIndex = 0;

    public ComparisonDatabase(
        DefinitionCollection[] definitions,
        string dbPath,
        ILoggerFactory? loggerFactory = null)
    {
        if (definitions.Length == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(definitions));
        }

        _loggerFactory = loggerFactory;
        _logger = loggerFactory?.CreateLogger<ComparisonDatabase>() ?? definitions[0].Logger;
        _definitions = definitions.Select(dc => (dc, new DcInfoRec()
        {
            FhirSequence = dc.FhirSequence,
            PackageCanonical = dc.MainPackageCanonical,
            RLiteral = dc.FhirSequence.ToRLiteral(),
            ShortVersion = dc.FhirSequence.ToShortVersion(),
            ShortVersionUrlSegment = "/" + dc.FhirSequence.ToShortVersion() + "/",
            PackageVersion = dc.FhirVersionLiteral,
        })).ToArray();

        _dbPath = dbPath;
        if (!Directory.Exists(_dbPath))
        {
            Directory.CreateDirectory(_dbPath);
        }

        _isCoreComparison = definitions.All(dc => FhirPackageUtils.PackageIsFhirCore(dc.MainPackageId));
        _isVersionComparison = definitions.Select(dc => dc.MainPackageId).Distinct().Count() == 1;

        switch (definitions.Length)
        {
            case 1:
                _dbName = $"{definitions[0].MainPackageId.ToPascalCase()}-V{definitions[0].MainPackageVersion.Replace('.', '_')}.db";
                break;
            case 2:
                {
                    if (_isVersionComparison)
                    {
                        _dbName = string.Join('_', definitions.Select(dc => $"V{dc.MainPackageVersion.Replace('.', '_')}")) + ".db";
                    }
                    else if (_isCoreComparison)
                    {
                        _dbName = string.Join('_', definitions.Select(dc => $"{dc.FhirSequence.ToRLiteral()}")) + ".db";
                    }
                    else
                    {
                        _dbName = string.Join('_', definitions.Select(dc => $"{dc.MainPackageId.ToPascalCase()}-V{dc.MainPackageVersion.Replace('.', '_')}")) + ".db";
                    }
                }
                break;
            default:
                _dbName = "fhir-comparison.db";
                break;
        }

        string connectionString = new SqliteConnectionStringBuilder()
        {
            DataSource = Path.Combine(_dbPath, _dbName),
            Mode = SqliteOpenMode.ReadWriteCreate,
        }.ToString();

        _dbConnection = new SqliteConnection(connectionString);
        _dbConnection.Open();

        initNewDb(true);
    }

    public ComparisonDatabase(
        string dbPath,
        string dbName,
        ILoggerFactory? loggerFactory = null)
    {
        _loggerFactory = loggerFactory;
        _logger = loggerFactory?.CreateLogger<ComparisonDatabase>() ?? NullLoggerFactory.Instance.CreateLogger(nameof(ComparisonDatabase));

        _dbPath = dbPath;
        if (!Directory.Exists(_dbPath))
        {
            Directory.CreateDirectory(_dbPath);
        }

        _dbName = dbName;
        string connectionString = new SqliteConnectionStringBuilder()
        {
            DataSource = Path.Combine(_dbPath, _dbName),
            Mode = SqliteOpenMode.ReadWriteCreate,
        }.ToString();

        _dbConnection = new SqliteConnection(connectionString);
        _dbConnection.Open();
    }

    public bool IsCoreComparison => _isCoreComparison;
    public bool IsVersionComparison => _isVersionComparison;

    /// <summary>
    /// Initializes the database connection and sets up the necessary tables and metadata.
    /// </summary>
    /// <param name="createdNew">Outputs a boolean indicating whether a new database was created.</param>
    /// <exception cref="Exception">Thrown when the package information does not match or the database connection is not initialized.</exception>
    private void initNewDb(
        bool ensureDeleted)
    {
        if (_definitions == null)
        {
            throw new Exception("Cannot initialize clean database without packages!");
        }

        if (ensureDeleted)
        {
            DbFhirPackage.DropTable(_dbConnection);

            DbValueSet.DropTable(_dbConnection);
            DbValueSetConcept.DropTable(_dbConnection);
            DbStructureDefinition.DropTable(_dbConnection);
            DbElement.DropTable(_dbConnection);

            DbFhirPackageComparisonPair.DropTable(_dbConnection);
            DbValueSetComparison.DropTable(_dbConnection);
            DbValueSetConceptComparison.DropTable(_dbConnection);
            DbUnresolvedConceptComparisons.DropTable(_dbConnection);
        }

        // create all our tables
        DbFhirPackage.CreateTable(_dbConnection);

        DbValueSet.CreateTable(_dbConnection);
        DbValueSetConcept.CreateTable(_dbConnection);
        DbStructureDefinition.CreateTable(_dbConnection);
        DbElement.CreateTable(_dbConnection);

        DbFhirPackageComparisonPair.CreateTable(_dbConnection);
        DbValueSetComparison.CreateTable(_dbConnection);
        DbValueSetConceptComparison.CreateTable(_dbConnection);
        DbUnresolvedConceptComparisons.CreateTable(_dbConnection);

        foreach ((DefinitionCollection dc, DcInfoRec _) in _definitions)
        {
            string shortName;
            if (_isCoreComparison)
            {
                shortName = dc.FhirSequence.ToRLiteral();
            }
            else if (_isVersionComparison)
            {
                shortName = $"V{dc.MainPackageVersion.Replace('.', '_')}";
            }
            else
            {
                shortName = $"{dc.MainPackageId.ToPascalCase()}-V{dc.MainPackageVersion.Replace('.', '_')}";
            }

            // add data about our packages
            if (DbFhirPackage.SelectSingle(_dbConnection, PackageId: dc.MainPackageId, PackageVersion: dc.MainPackageVersion) is not DbFhirPackage pm)
            {
                pm = new()
                {
                    Name = dc.Name,
                    ShortName = shortName,
                    PackageId = dc.MainPackageId,
                    PackageVersion = dc.MainPackageVersion,
                    CanonicalUrl = dc.MainPackageCanonical,
                };

                pm = DbFhirPackage.Insert(_dbConnection, pm);
            }
        }

        // look for cross-version collections
        for (int definitionIndex = 1; definitionIndex < _definitions.Length; definitionIndex++)
        {
            DefinitionCollection left = _definitions[definitionIndex - 1].dc;
            DefinitionCollection right = _definitions[definitionIndex].dc;

            // get the db package definitions
            DbFhirPackage leftDbPackage = DbFhirPackage.SelectSingle(_dbConnection, PackageId: left.MainPackageId, PackageVersion: left.MainPackageVersion)
                ?? throw new Exception($"Package {left.MainPackageId}@{left.MainPackageVersion} was not found in the database!");
            DbFhirPackage rightDbPackage = DbFhirPackage.SelectSingle(_dbConnection, PackageId: right.MainPackageId, PackageVersion: right.MainPackageVersion)
                ?? throw new Exception($"Package {right.MainPackageId}@{right.MainPackageVersion} was not found in the database!");

            // check for a package pair for left-to-right comparison
            DbFhirPackageComparisonPair? dbPairLtoR = DbFhirPackageComparisonPair.SelectSingle(
                _dbConnection,
                SourcePackageKey: leftDbPackage.Key,
                TargetPackageKey: rightDbPackage.Key);

            if (dbPairLtoR == null)
            {
                dbPairLtoR = new()
                {
                    SourcePackageKey = leftDbPackage.Key,
                    TargetPackageKey = rightDbPackage.Key,
                    ProccessedAt = DateTime.UtcNow,
                };

                dbPairLtoR = DbFhirPackageComparisonPair.Insert(_dbConnection, dbPairLtoR);
            }

            // check for a package pair for right-to-left comparison
            DbFhirPackageComparisonPair? dbPairRtoL = DbFhirPackageComparisonPair.SelectSingle(
                _dbConnection,
                SourcePackageKey: rightDbPackage.Key,
                TargetPackageKey: leftDbPackage.Key);

            if (dbPairRtoL == null)
            {
                dbPairRtoL = new()
                {
                    SourcePackageKey = rightDbPackage.Key,
                    TargetPackageKey = leftDbPackage.Key,
                    ProccessedAt = DateTime.UtcNow,
                };

                dbPairRtoL = DbFhirPackageComparisonPair.Insert(_dbConnection, dbPairRtoL);
            }
        }
    }

    public bool TryLoadFhirCrossVersionMaps(string crossVersionMapSourcePath)
    {
        if ((_definitions == null) || (_definitions.Length == 0))
        {
            _logger.LogError("Cannot initialize clean database without packages!");
            return false;
        }

        if (_definitions.Length < 2)
        {
            _logger.LogError($"Cannot perform comparisons in a single package: {string.Join(", ", _definitions.Select(d => d.dc.Key))}");
            return false;
        }

        if (!_isCoreComparison)
        {
            _logger.LogError($"Cannot load FHIR Cross Version maps for non-core comparison: {string.Join(", ", _definitions.Select(d => d.dc.Key))}");
            return false;
        }

        if (string.IsNullOrEmpty(crossVersionMapSourcePath) ||
            (!Directory.Exists(crossVersionMapSourcePath)))
        {
            _logger.LogError($"Invalid map source path: {crossVersionMapSourcePath}");
            return false;
        }

        List<DbValueSetComparison> dbValueSetComparisons = [];
        List<DbValueSetConceptComparison> dbValueSetConceptComparions = [];
        List<DbUnresolvedConceptComparisons> dbUnresolvedConceptComparisons = [];

        // look for cross-version collections
        for (int definitionIndex = 1; definitionIndex < _definitions.Length; definitionIndex++)
        {
            DefinitionCollection left = _definitions[definitionIndex - 1].dc;
            DefinitionCollection right = _definitions[definitionIndex].dc;

            DbFhirPackage leftDbPackage = DbFhirPackage.SelectSingle(_dbConnection, PackageId: left.MainPackageId, PackageVersion: left.MainPackageVersion)
                ?? throw new Exception($"Package {left.MainPackageId}@{left.MainPackageVersion} was not found in the database!");
            DbFhirPackage rightDbPackage = DbFhirPackage.SelectSingle(_dbConnection, PackageId: right.MainPackageId, PackageVersion: right.MainPackageVersion)
                ?? throw new Exception($"Package {right.MainPackageId}@{right.MainPackageVersion} was not found in the database!");

            DbFhirPackageComparisonPair dbPairLtoR = DbFhirPackageComparisonPair.SelectSingle(
                _dbConnection,
                SourcePackageKey: leftDbPackage.Key,
                TargetPackageKey: rightDbPackage.Key)
                ?? throw new Exception($"Comparison {left.MainPackageId}@{left.MainPackageVersion} to {right.MainPackageId}@{right.MainPackageVersion} was not found in the database!");

            DbFhirPackageComparisonPair dbPairRtoL = DbFhirPackageComparisonPair.SelectSingle(
                _dbConnection,
                SourcePackageKey: rightDbPackage.Key,
                TargetPackageKey: leftDbPackage.Key)
                ?? throw new Exception($"Comparison {right.MainPackageId}@{right.MainPackageVersion} to {left.MainPackageId}@{left.MainPackageVersion} was not found in the database!");

            CrossVersionMapCollection cvLtoR = new(left, right, _dbPath, _loggerFactory);
            CrossVersionMapCollection cvRtoL = new(right, left, _dbPath, _loggerFactory);

            if (cvLtoR.TryLoadCrossVersionMaps(crossVersionMapSourcePath, true))
            {
                processCrossVersionMaps(
                    left,
                    leftDbPackage,
                    right,
                    rightDbPackage,
                    cvLtoR,
                    dbPairLtoR);
            }
            else
            {
                _logger.LogMapsNotLoaded(left.Key, right.Key);
            }

            if (cvRtoL.TryLoadCrossVersionMaps(crossVersionMapSourcePath, true))
            {
                processCrossVersionMaps(
                    right,
                    rightDbPackage,
                    left,
                    leftDbPackage,
                    cvRtoL,
                    dbPairRtoL);
            }
            else
            {
                _logger.LogMapsNotLoaded(right.Key, left.Key);
            }
        }

        DbValueSetComparison.Insert(_dbConnection, dbValueSetComparisons);
        DbValueSetConceptComparison.Insert(_dbConnection, dbValueSetConceptComparions);
        DbUnresolvedConceptComparisons.Insert(_dbConnection, dbUnresolvedConceptComparisons);

        return true;

        void processCrossVersionMaps(
            DefinitionCollection source,
            DbFhirPackage sourceDbPackage,
            DefinitionCollection target,
            DbFhirPackage targetDbPackage,
            CrossVersionMapCollection cv,
            DbFhirPackageComparisonPair dbPackagePair)
        {
            // iterate over the concept maps in the cross version map collection
            foreach (ConceptMap cm in cv.CrossVersionConceptMaps)
            {
                // only process maps we have categorized

                UsageContext? cmContext = cm.UseContext.FirstOrDefault(uc => uc.Code.System == CommonDefinitions.ConceptMapUsageContextSystem);
                if ((cmContext == null) ||
                    (cmContext.Value is not CodeableConcept uc))
                {
                    continue;
                }

                string? cmCategory = uc.Coding.FirstOrDefault(c => c.System == CommonDefinitions.ConceptMapUsageContextSystem)?.Code;
                switch (cmCategory)
                {
                    case CommonDefinitions.ConceptMapUsageContextValueSet:
                        processValueSetMap(dbPackagePair, sourceDbPackage, targetDbPackage, cm);
                        break;

                    case CommonDefinitions.ConceptMapUsageContextTypeOverview:
                        break;

                    case CommonDefinitions.ConceptMapUsageContextDataType:
                        break;

                    case CommonDefinitions.ConceptMapUsageContextResourceOverview:
                        break;

                    case CommonDefinitions.ConceptMapUsageContextResource:
                        break;

                    default:
                        continue;
                }
            }
        }

        void processValueSetMap(
            DbFhirPackageComparisonPair dbPackagePair,
            DbFhirPackage sourceDbPackage,
            DbFhirPackage targetDbPackage,
            ConceptMap cm)
        {
            if (cm.SourceScope is not Canonical sourceScopeCanonical)
            {
                throw new Exception($"Cannot process existing Cross-Version ValueSet Map: {cm.Id} - invalid source scope: {cm.SourceScope}");
            }

            if (cm.TargetScope is not Canonical targetScopeCanonical)
            {
                throw new Exception($"Cannot process existing Cross-Version ValueSet Map: {cm.Id} - invalid target scope: {cm.TargetScope}");
            }

            DbValueSet sourceDbVs = DbValueSet.SelectSingle(
                _dbConnection,
                FhirPackageKey: sourceDbPackage.Key,
                UnversionedUrl: sourceScopeCanonical.Uri)
                ?? throw new Exception($"Could not find {sourceScopeCanonical.Uri} in {sourceDbPackage.PackageId}@{sourceDbPackage.PackageVersion}");

            DbValueSet targetDbVs = DbValueSet.SelectSingle(
                _dbConnection,
                FhirPackageKey: targetDbPackage.Key,
                UnversionedUrl: targetScopeCanonical.Uri)
                ?? throw new Exception($"Could not find {targetScopeCanonical.Uri} in {targetDbPackage.PackageId}@{targetDbPackage.PackageVersion}");

            List<string> additionalUrls = [];
            foreach (Extension ext in cm.GetExtensions(CommonDefinitions.ExtUrlConceptMapAdditionalUrls))
            {
                if (ext.Value is FhirUrl fhirUrl)
                {
                    // use the official URL as the key
                    additionalUrls.Add(fhirUrl.Value);
                }
            }

            // create our canonical comparison record
            DbValueSetComparison dbVsComparison = new()
            {
                Key = Interlocked.Increment(ref _dbValueSetComparisonIndex),
                PackageComparisonKey = dbPackagePair.Key,
                SourceFhirPackageKey = sourceDbPackage.Key,
                TargetFhirPackageKey = targetDbPackage.Key,
                SourceKey = sourceDbVs.Key,
                SourceCanonicalVersioned = sourceDbVs.VersionedUrl,
                SourceCanonicalUnversioned = sourceDbVs.UnversionedUrl,
                SourceVersion = sourceDbVs.Version,
                SourceName = sourceDbVs.Name,
                TargetKey = targetDbVs.Key,
                TargetCanonicalVersioned = targetDbVs.VersionedUrl,
                TargetCanonicalUnversioned = targetDbVs.UnversionedUrl,
                TargetVersion = targetDbVs.Version,
                TargetName = targetDbVs.Name,
                CompositeName = getCompositeName(sourceDbPackage, sourceDbVs, targetDbPackage, targetDbVs),
                SourceConceptMapUrl = cm.Url,
                SourceConceptMapAdditionalUrls = additionalUrls.Count == 0 ? null : string.Join(", ", additionalUrls),
                Relationship = null,
                IsGenerated = false,
                LastReviewedBy = null,
                LastReviewedOn = null,
                Message = $"Imported from existing ConceptMap {cm.Id} ({cm.Url}).",
            };

            dbValueSetComparisons.Add(dbVsComparison);
            //dbVsComparison = DbValueSetComparison.Insert(_dbConnection, dbVsComparison);

            // check for manual exclusion
            if (sourceDbVs.IsExcluded)
            {
                dbVsComparison.Message += $" Note: comparison source: {sourceDbVs.Id} ({sourceDbVs.VersionedUrl}) has been manually excluded.";
                //dbVsComparison.Message += $" Skipping due to manual exclusion of source content.";
                //return;
            }

            if (targetDbVs.IsExcluded)
            {
                dbVsComparison.Message += $" Note: comparison target: {targetDbVs.Id} ({targetDbVs.VersionedUrl}) has been manually excluded.";
                //dbVsComparison.Message += $" Skipping due to manual exclusion of target content.";
                //return;
            }

            // check for failure to expand
            if (sourceDbVs.CanExpand == false)
            {
                dbVsComparison.Message += $" Note: source value set {sourceDbVs.Id} ({sourceDbVs.VersionedUrl}) cannot be expanded.";
                //dbVsComparison.Message += $" Skipping because the source value set {sourceDbVs.Id} ({sourceDbVs.VersionedUrl}) cannot be expanded.";
                //return;
            }

            if (targetDbVs.CanExpand == false)
            {
                dbVsComparison.Message += $" Note: target value set {targetDbVs.Id} ({targetDbVs.VersionedUrl}) cannot be expanded.";
                //dbVsComparison.Message += $" Skipping because the target value set {targetDbVs.Id} ({targetDbVs.VersionedUrl}) cannot be expanded.";
                //return;
            }

            // process each group
            foreach (ConceptMap.GroupComponent cmGroup in cm.Group)
            {
                string sourceSystem = cmGroup.Source;
                string targetSystem = cmGroup.Target;

                // iterate over the elements in the group
                foreach (ConceptMap.SourceElementComponent sourceElement in cmGroup.Element)
                {
                    // get the concept matching this source element
                    DbValueSetConcept? sourceDbConcept = DbValueSetConcept.SelectSingle(
                        _dbConnection,
                        ValueSetKey: sourceDbVs.Key,
                        System: sourceSystem,
                        Code: sourceElement.Code);

                    // handle non-mapping codes
                    if (sourceElement.NoMap == true)
                    {
                        addComparison(
                            dbPackagePair,
                            sourceDbVs,
                            targetDbVs,
                            dbVsComparison,
                            cm,
                            sourceDbConcept,
                            sourceSystem,
                            sourceElement.Code,
                            sourceElement.Display,
                            noMap: true,
                            relationship: null);

                        continue;
                    }

                    // iterate across the targets
                    foreach (ConceptMap.TargetElementComponent targetElement in sourceElement.Target)
                    {
                        // get the concept matching this target element
                        DbValueSetConcept? targetDbConcept = DbValueSetConcept.SelectSingle(
                            _dbConnection,
                            ValueSetKey: targetDbVs.Key,
                            System: targetSystem,
                            Code: targetElement.Code);

                        addComparison(
                            dbPackagePair,
                            sourceDbVs,
                            targetDbVs,
                            dbVsComparison,
                            cm,
                            sourceDbConcept,
                            sourceSystem,
                            sourceElement.Code,
                            sourceElement.Display,
                            noMap: false,
                            relationship: targetElement.Relationship,
                            targetDbConcept,
                            targetSystem,
                            targetElement.Code,
                            targetElement.Display);

                        continue;
                    }
                }
            }
        }

        void addComparison(
            DbFhirPackageComparisonPair dbPackagePair,
            DbValueSet sourceDbVs,
            DbValueSet targetDbVs,
            DbValueSetComparison dbVsComparison,
            ConceptMap cm,
            DbValueSetConcept? sourceDbConcept,
            string sourceSystem,
            string sourceCode,
            string? sourceDisplay,
            bool? noMap,
            CMR? relationship,
            DbValueSetConcept? targetDbConcept = null,
            string? targetSystem = null,
            string? targetCode = null,
            string? targetDisplay = null)
        {
            if (noMap == true)
            {
                if (sourceDbConcept == null)
                {
                    DbUnresolvedConceptComparisons unresolvedNoMap = new()
                    {
                        Key = Interlocked.Increment(ref _dbUnresolvedConceptComparisonIndex),
                        PackageComparisonKey = dbPackagePair.Key,
                        SourceFhirPackageKey = dbPackagePair.SourcePackageKey,
                        SourceCanonicalKey = sourceDbVs.Key,
                        TargetFhirPackageKey = dbPackagePair.TargetPackageKey,
                        TargetCanonicalKey = targetDbVs.Key,
                        CanonicalComparisonKey = dbVsComparison.Key,
                        ConceptMapId = cm.Id,
                        ConceptMapUrl = cm.Url,
                        Relationship = null,
                        NoMap = true,
                        Message = $"Code flagged as noMap in {cm.Id} ({cm.Url}), but does not exist in source {sourceDbVs.Id} ({sourceDbVs.VersionedUrl}).",
                        SourceConceptExists = false,
                        SourceConceptKey = null,
                        SourceSystem = sourceSystem,
                        SourceCode = sourceCode,
                        SourceDisplay = sourceDisplay,
                        TargetConceptExists = null,
                        TargetConceptKey = null,
                        TargetSystem = null,
                        TargetCode = null,
                        TargetDisplay = null,
                        IsGenerated = false,
                        LastReviewedBy = null,
                        LastReviewedOn = null,
                    };

                    dbUnresolvedConceptComparisons.Add(unresolvedNoMap);

                    ////dbPackagePair.InvalidImportedConceptComparisons.Add(invalidComparison);
                    //dbVsComparison.InvalidImportedComparisons.Add(invalidComparison);

                    return;
                }

                DbValueSetConceptComparison nonMappedComparison = new()
                {
                    PackageComparisonKey = dbPackagePair.Key,
                    SourceFhirPackageKey = dbPackagePair.SourcePackageKey,
                    SourceCanonicalKey = sourceDbVs.Key,
                    TargetFhirPackageKey = dbPackagePair.TargetPackageKey,
                    TargetCanonicalKey = targetDbVs.Key,
                    CanonicalComparisonKey = dbVsComparison.Key,
                    SourceKey = sourceDbConcept.Key,
                    TargetKey = null,
                    Relationship = null,
                    NoMap = true,
                    Message = $"Code flagged as noMap in {cm.Id} ({cm.Url})",
                    IsGenerated = false,
                    LastReviewedBy = null,
                    LastReviewedOn = null,
                };

                dbValueSetConceptComparions.Add(nonMappedComparison);
                //DbValueSetConceptComparison.Insert(_dbConnection, nonMappedComparison);

                return;
            }

            bool sourceExists = sourceDbConcept == null;
            bool targetExists = targetDbConcept == null;

            string message = $"Mapping exists in {cm.Id} ({cm.Url}), but:";

            if (!sourceExists)
            {
                message += $" {sourceSystem}|{sourceCode} does not exist in source {sourceDbVs.Id} ({sourceDbVs.VersionedUrl}).";
            }

            if (!targetExists)
            {
                message += $" {targetSystem}|{targetCode} does not exist in target {targetDbVs.Id} ({targetDbVs.VersionedUrl}).";
            }

            if ((sourceDbConcept == null) || (targetDbConcept == null))
            {
                DbUnresolvedConceptComparisons unresolvedComparison = new()
                {
                    Key = Interlocked.Increment(ref _dbUnresolvedConceptComparisonIndex),
                    PackageComparisonKey = dbPackagePair.Key,
                    SourceFhirPackageKey = dbPackagePair.SourcePackageKey,
                    SourceCanonicalKey = sourceDbVs.Key,
                    TargetFhirPackageKey = dbPackagePair.TargetPackageKey,
                    TargetCanonicalKey = targetDbVs.Key,
                    CanonicalComparisonKey = dbVsComparison.Key,
                    ConceptMapId = cm.Id,
                    ConceptMapUrl = cm.Url,
                    Relationship = null,
                    NoMap = true,
                    Message = message,
                    SourceConceptExists = sourceExists,
                    SourceConceptKey = sourceDbConcept?.Key,
                    SourceSystem = sourceSystem,
                    SourceCode = sourceCode,
                    SourceDisplay = sourceDisplay,
                    TargetConceptExists = targetExists,
                    TargetConceptKey = targetDbConcept?.Key,
                    TargetSystem = targetSystem,
                    TargetCode = targetCode,
                    TargetDisplay = targetDisplay,
                    IsGenerated = false,
                    LastReviewedBy = null,
                    LastReviewedOn = null,
                };

                dbUnresolvedConceptComparisons.Add(unresolvedComparison);

                ////dbPackagePair.InvalidImportedConceptComparisons.Add(invalidComparison);
                //dbVsComparison.InvalidImportedComparisons.Add(invalidComparison);

                return;
            }

            DbValueSetConceptComparison mappedComparison = new()
            {
                PackageComparisonKey = dbPackagePair.Key,
                SourceFhirPackageKey = dbPackagePair.SourcePackageKey,
                SourceCanonicalKey = sourceDbVs.Key,
                TargetFhirPackageKey = dbPackagePair.TargetPackageKey,
                TargetCanonicalKey = targetDbVs.Key,
                CanonicalComparisonKey = dbVsComparison.Key,
                SourceKey = sourceDbConcept.Key,
                TargetKey = targetDbConcept.Key,
                Relationship = relationship,
                NoMap = false,
                Message = message,
                IsGenerated = false,
                LastReviewedBy = null,
                LastReviewedOn = null,
            };

            dbValueSetConceptComparions.Add(mappedComparison);
            //DbValueSetConceptComparison.Insert(_dbConnection, mappedComparison);
        }
    }

    private static string getCompositeName(
        DbFhirPackage sourceDbPackage,
        DbCanonicalResource sourceDbCanonical,
        DbFhirPackage targetDbPackage,
        DbCanonicalResource? targetDbCanonical)
    {
        if (targetDbCanonical == null)
        {
            return
                $"{sourceDbPackage.ShortName}-" +
                $"{FhirSanitizationUtils.SanitizeForProperty(sourceDbCanonical.Name).ToPascalCase()}-" +
                $"{targetDbPackage.ShortName}";
        }

        return
            $"{sourceDbPackage.ShortName}-" +
            $"{FhirSanitizationUtils.SanitizeForProperty(sourceDbCanonical.Name).ToPascalCase()}-" +
            $"{targetDbPackage.ShortName}-" +
            $"{FhirSanitizationUtils.SanitizeForProperty(targetDbCanonical.Name).ToPascalCase()}";
    }


    public bool TryLoadFromDefinitionCollections(
        HashSet<string> _exclusionSet,
        HashSet<string> _escapeValveCodes)
    {
        if (_definitions == null)
        {
            _logger.LogError("Cannot export contents without a DefinitionCollection!");
            return false;
        }

        foreach ((DefinitionCollection dc, DcInfoRec _) in _definitions)
        {
            // get the package metadata for this definition collection
            DbFhirPackage pm = DbFhirPackage.SelectSingle(_dbConnection, PackageId: dc.MainPackageId, PackageVersion: dc.MainPackageVersion)
                    ?? throw new Exception($"Package {dc.MainPackageId}@{dc.MainPackageVersion} was not found in the database!");

            // load our value sets
            addValueSetsToDb(pm, dc, _exclusionSet, _escapeValveCodes);

            // load our structures
            addStructuresToDb(pm, dc, _exclusionSet);
        }

        return true;
    }

    private void addValueSetsToDb(
        DbFhirPackage pm,
        DefinitionCollection dc,
        HashSet<string> _exclusionSet,
        HashSet<string> _escapeValveCodes)
    {
        List<DbValueSet> dbValueSets = [];
        List<DbValueSetConcept> allDbConcepts = [];

        // iterate over the value sets in the definition collection
        foreach ((string unversionedUrl, string[] versions) in dc.ValueSetVersions.OrderBy(kvp => kvp.Key))
        {
            // only use the highest version in the package
            string vsVersion = versions.OrderDescending().First();
            string versionedUrl = unversionedUrl + "|" + vsVersion;

            DbValueSet? existingDbVs = DbValueSet.SelectSingle(
                _dbConnection,
                FhirPackageKey: pm.Key,
                UnversionedUrl: unversionedUrl,
                Version: vsVersion);

            // check to see if this value set already exists
            if (existingDbVs != null)
            {
                continue;
            }

            // try to expand this value set
            if (!dc.ValueSetsByVersionedUrl.TryGetValue(versionedUrl, out ValueSet? uvs))
            {
                throw new Exception($"Failed to resolve ValueSet {versionedUrl} in {dc.MainPackageId}:{dc.MainPackageVersion}.");
            }

            bool canExpand = dc.TryExpandVs(versionedUrl, out ValueSet? vs, out string? expandMessage);

            bool? hasEscapeCode = !canExpand
                ? null
                : vs?.cgHasCode(_escapeValveCodes);

            IEnumerable<StructureElementCollection> coreBindings = dc.CoreBindingsForVs(versionedUrl);
            BindingStrength? strongestBindingCore = dc.StrongestBinding(coreBindings);
            IReadOnlyDictionary<string, BindingStrength> coreBindingStrengthByType = dc.BindingStrengthByType(coreBindings);

            IEnumerable<StructureElementCollection> extendedBindings = dc.ExtendedBindingsForVs(versionedUrl);
            BindingStrength? strongestBindingExtended = dc.StrongestBinding(extendedBindings);
            IReadOnlyDictionary<string, BindingStrength> extendedBindingStrengthByType = dc.BindingStrengthByType(extendedBindings);

            bool isExcluded = _exclusionSet.Contains(unversionedUrl);
            
            // will not further process value sets we know we will not process
            if (isExcluded ||
                !canExpand ||
                (vs == null))
            {
                // still add a metadata record
                DbValueSet vsmExcluded = new()
                {
                    Key = Interlocked.Increment(ref _dbValueSetIndex),
                    FhirPackageKey = pm.Key,
                    Id = uvs.Id,
                    VersionedUrl = versionedUrl,
                    UnversionedUrl = unversionedUrl,
                    Name = uvs.Name,
                    Version = vsVersion,
                    Status = uvs.Status,
                    Title = uvs.Title,
                    Description = uvs.Description,
                    Purpose = uvs.Purpose,
                    IsExcluded = isExcluded,
                    CanExpand = canExpand,
                    ConceptCount = 0,
                    HasEscapeValveCode = hasEscapeCode,
                    Message = expandMessage,
                    ReferencedSystems = string.Join(", ", uvs.cgReferencedCodeSystems()),
                    BindingCountCore = coreBindings.Count(),
                    StrongestBindingCore = strongestBindingCore,
                    StrongestBindingCoreCode = coreBindingStrengthByType.TryGetValue("code", out BindingStrength ebscCode) ? ebscCode : null,
                    StrongestBindingCoreCoding = coreBindingStrengthByType.TryGetValue("Coding", out BindingStrength ebscCoding) ? ebscCoding : null,
                    BindingCountExtended = extendedBindings.Count(),
                    StrongestBindingExtended = strongestBindingExtended,
                    StrongestBindingExtendedCode = extendedBindingStrengthByType.TryGetValue("code", out BindingStrength ebseCode) ? ebseCode : null,
                    StrongestBindingExtendedCoding = extendedBindingStrengthByType.TryGetValue("Coding", out BindingStrength ebseCoding) ? ebseCoding : null,
                };

                dbValueSets.Add(vsmExcluded);
                //DbValueSet.Insert(_dbConnection, vsmExcluded);

                continue;
            }

            // create a new metadata record
            DbValueSet dbVs = new()
            {
                Key = Interlocked.Increment(ref _dbValueSetIndex),
                FhirPackageKey = pm.Key,
                Id = vs.Id,
                VersionedUrl = versionedUrl,
                UnversionedUrl = unversionedUrl,
                Name = vs.Name,
                Version = vsVersion,
                Status = vs.Status,
                Title = vs.Title,
                Description = vs.Description,
                Purpose = vs.Purpose,
                IsExcluded = isExcluded,
                CanExpand = canExpand,
                ConceptCount = 0,
                HasEscapeValveCode = hasEscapeCode,
                Message = expandMessage,
                ReferencedSystems = string.Join(", ", vs.cgReferencedCodeSystems()),
                BindingCountCore = coreBindings.Count(),
                StrongestBindingCore = strongestBindingCore,
                StrongestBindingCoreCode = coreBindingStrengthByType.TryGetValue("code", out BindingStrength bscCode) ? bscCode : null,
                StrongestBindingCoreCoding = coreBindingStrengthByType.TryGetValue("Coding", out BindingStrength bscCoding) ? bscCoding : null,
                BindingCountExtended = extendedBindings.Count(),
                StrongestBindingExtended = strongestBindingExtended,
                StrongestBindingExtendedCode = extendedBindingStrengthByType.TryGetValue("code", out BindingStrength bseCode) ? bseCode : null,
                StrongestBindingExtendedCoding = extendedBindingStrengthByType.TryGetValue("Coding", out BindingStrength bseCoding) ? bseCoding : null,
            };

            dbValueSets.Add(dbVs);
            //dbVs = DbValueSet.Insert(_dbConnection, dbVs);

            List<DbValueSetConcept> dbConcepts = [];
            int conceptCount = 0;

            // iterate over all the contents of the value set
            foreach (FhirConcept fc in vs.cgGetFlatConcepts(dc))
            {
                conceptCount++;

                // check for this record already existing
                if (DbValueSetConcept.SelectSingle(_dbConnection, FhirPackageKey: pm.Key, ValueSetKey: dbVs.Key, System: fc.System, Code: fc.Code) != null)
                {
                    continue;
                }

                // create a new content record
                DbValueSetConcept dbConcept = new()
                {
                    Key = Interlocked.Increment(ref _dbConceptIndex),
                    FhirPackageKey = pm.Key,
                    ValueSetKey = dbVs.Key,
                    System = fc.System,
                    Code = fc.Code,
                    Display = fc.Display,
                };

                dbConcepts.Add(dbConcept);
                //DbValueSetConcept.Insert(_dbConnection, dbConcept);
            }

            dbVs.ConceptCount = conceptCount;

            allDbConcepts.AddRange(dbConcepts);
            //DbValueSetConcept.Insert(_dbConnection, dbConcepts);
        }

        DbValueSet.Insert(_dbConnection, dbValueSets);
        DbValueSetConcept.Insert(_dbConnection, allDbConcepts);

        return;
    }

    private void addStructuresToDb(
        DbFhirPackage pm,
        DefinitionCollection dc,
        HashSet<string> _exclusionSet)
    {
        List<DbStructureDefinition> dbStructures = [];
        List<DbElement> dbElements = [];

        // iterate over the types of structures
        foreach ((IEnumerable<StructureDefinition> structures, FhirArtifactClassEnum cgClass) in getStructures(dc))
        {
            foreach (StructureDefinition sd in structures)
            {
                // will not further process value sets we know we will not process
                if (_exclusionSet.Contains(sd.Url))
                {
                    // still add a metadata record
                    DbStructureDefinition sdmExcluded = new()
                    {
                        Key = Interlocked.Increment(ref _dbStructureIndex),
                        FhirPackageKey = pm.Key,
                        Id = sd.Id,
                        VersionedUrl = sd.Url + "|" + sd.Version,
                        UnversionedUrl = sd.Url,
                        Name = sd.Name,
                        Version = sd.Version,
                        Status = sd.Status,
                        Title = sd.Title ?? sd.Snapshot?.Element.FirstOrDefault()?.Short,
                        Description = sd.Description ?? sd.Snapshot?.Element.FirstOrDefault()?.Definition,
                        Purpose = sd.Purpose,
                        Comment = sd.Snapshot?.Element.FirstOrDefault()?.Comment,
                        ArtifactClass = cgClass,
                        Message = "Manually excluded",
                    };

                    dbStructures.Add(sdmExcluded);
                    //DbStructureDefinition.Insert(_dbConnection, sdmExcluded);

                    continue;
                }

                // create a new metadata record
                DbStructureDefinition dbStructure = new()
                {
                    Key = Interlocked.Increment(ref _dbStructureIndex),
                    FhirPackageKey = pm.Key,
                    Id = sd.Id,
                    VersionedUrl = sd.Url + "|" + sd.Version,
                    UnversionedUrl = sd.Url,
                    Name = sd.Name,
                    Version = sd.Version,
                    Status = sd.Status,
                    Title = sd.Title ?? sd.Snapshot?.Element.FirstOrDefault()?.Short,
                    Description = sd.Description ?? sd.Snapshot?.Element.FirstOrDefault()?.Definition,
                    Purpose = sd.Purpose,
                    Comment = sd.Snapshot?.Element.FirstOrDefault()?.Comment,
                    ArtifactClass = cgClass,
                    Message = string.Empty,
                };

                dbStructures.Add(dbStructure);
                //DbStructureDefinition.Insert(_dbConnection, dbStructure);

                // iterate over all the elements of the structure
                foreach (ElementDefinition ed in sd.cgElements(skipSlices: false))
                {
                    addElement(dbStructure, sd, ed);
                }
            }
        }

        // save changes
        DbStructureDefinition.Insert(_dbConnection, dbStructures);
        DbElement.Insert(_dbConnection, dbElements);

        return;

        (IEnumerable<StructureDefinition> structures, FhirArtifactClassEnum cgClass)[] getStructures(DefinitionCollection dc) => [
            (dc.PrimitiveTypesByName.Values, FhirArtifactClassEnum.PrimitiveType),
            (dc.ComplexTypesByName.Values, FhirArtifactClassEnum.ComplexType),
            (dc.ResourcesByName.Values, FhirArtifactClassEnum.Resource),
            (dc.ExtensionsByUrl.Values, FhirArtifactClassEnum.Extension),
            (dc.ProfilesByUrl.Values, FhirArtifactClassEnum.Profile),
            (dc.LogicalModelsByUrl.Values, FhirArtifactClassEnum.LogicalModel),
            ];

        void addElement(DbStructureDefinition dbStructure, StructureDefinition sd, ElementDefinition ed)
        {
            // check for children
            int childCount = sd.cgElements(
                ed.Path,
                topLevelOnly: true,
                includeRoot: false,
                skipSlices: true).Count();

            List<(string? typeName, string? typeProfile, string? profile)> elementTypes = [];

            IEnumerable<ElementDefinition.TypeRefComponent> definedTypes = ed.Type.Select(tr => tr.cgAsR5());
            foreach (ElementDefinition.TypeRefComponent tr in definedTypes)
            {
                if ((tr.ProfileElement.Count == 0) &&
                    (tr.TargetProfileElement.Count == 0))
                {
                    elementTypes.Add((tr.cgName(), null, null));
                    continue;
                }

                if (tr.ProfileElement.Count == 0)
                {
                    foreach (Canonical tp in tr.TargetProfile)
                    {
                        elementTypes.Add((tr.cgName(), null, tp.Value));
                    }

                    continue;
                }

                if (tr.TargetProfileElement.Count == 0)
                {
                    foreach (Canonical p in tr.Profile)
                    {
                        elementTypes.Add((tr.cgName(), p.Value, null));
                    }

                    continue;
                }

                foreach (Canonical p in tr.Profile)
                {
                    foreach (Canonical tp in tr.TargetProfile)
                    {
                        elementTypes.Add((tr.cgName(), p.Value, tp.Value));
                    }
                }
            }

            if (elementTypes.Count == 0)
            {
                elementTypes.Add((null, null, null));
            }

            foreach ((string? typeName, string? typeProfile, string? targetProfile) in elementTypes)
            {
                DbElement dbElement = new()
                {
                    Key = Interlocked.Increment(ref _dbElementIndex),
                    FhirPackageKey = pm.Key,
                    StructureKey = dbStructure.Key,
                    ResourceFieldOrder = ed.cgFieldOrder(),
                    ComponentFieldOrder = ed.cgComponentFieldOrder(),
                    Id = ed.ElementId,
                    Path = ed.Path,
                    ChildElementCount = childCount,
                    Name = ed.cgName(),
                    Short = ed.Short,
                    Definition = ed.Definition,
                    MinCardinality = ed.cgCardinalityMin(),
                    MaxCardinality = ed.cgCardinalityMax(),
                    MaxCardinalityString = ed.Max ?? "*",
                    SliceName = ed.SliceName,
                    ValueSetBindingStrength = ed.Binding?.Strength,
                    BindingValueSet = ed.Binding?.ValueSet,
                    TypeName = typeName,
                    TypeProfile = typeProfile,
                    TargetProfile = typeProfile,
                };

                dbElements.Add(dbElement);
            }
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                if (_dbConnection != null)
                {
                    _dbConnection.Close();
                    _dbConnection.Dispose();
                }
            }

            _disposedValue = true;
        }
    }

    void IDisposable.Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
