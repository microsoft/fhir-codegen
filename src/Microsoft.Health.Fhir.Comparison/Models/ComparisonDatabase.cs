using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;
using Hl7.Fhir.Model;
using Hl7.Fhir.Utility;
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
    private int _dbElementTypeIndex = 0;
    private int _dbElementTypeGroupIndex = 0;
    private int _dbElementTypeGroupMapIndex = 0;
    private int _dbValueSetComparisonIndex = 0;
    private int _dbConceptComparisonIndex = 0;
    private int _dbUnresolvedConceptComparisonIndex = 0;
    private int _dbStructureComparisonIndex = 0;
    private int _dbUnresolvedStructureComparisonIndex = 0;
    private int _dbElementTypeComparisonIndex = 0;
    private int _dbElementTypeGroupComparisonIndex = 0;
    private int _dbElementComparisonIndex = 0;
    private int _dbUnresolvedElementComparisonIndex = 0;

    public ComparisonDatabase(
        DefinitionCollection[] definitions,
        string dbPath,
        string? dbName,
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

        if (!string.IsNullOrEmpty(dbName))
        {
            _dbName = dbName;
        }
        else
        {
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

        getCurrentIndexValues();
    }

    public bool IsCoreComparison => _isCoreComparison;
    public bool IsVersionComparison => _isVersionComparison;
    public string DbFileName => _dbName;
    public string DbFilePath => _dbPath;

    public IDbConnection DbConnection => _dbConnection;

    public int GetValueSetKey() => Interlocked.Increment(ref _dbValueSetIndex);
    public int GetConceptKey() => Interlocked.Increment(ref _dbConceptIndex);
    public int GetStructureKey() => Interlocked.Increment(ref _dbStructureIndex);
    public int GetElementKey() => Interlocked.Increment(ref _dbElementIndex);
    public int GetElementTypeKey() => Interlocked.Increment(ref _dbElementTypeIndex);
    public int GetElementTypeGroupKey() => Interlocked.Increment(ref _dbElementTypeGroupIndex);
    public int GetElementTypeMapKey() => Interlocked.Increment(ref _dbElementTypeGroupMapIndex);
    public int GetValueSetComparisonKey() => Interlocked.Increment(ref _dbValueSetComparisonIndex);
    public int GetConceptComparisonKey() => Interlocked.Increment(ref _dbConceptComparisonIndex);
    public int GetUnresolvedConceptComparisonKey() => Interlocked.Increment(ref _dbUnresolvedConceptComparisonIndex);
    public int GetStructureComparisonKey() => Interlocked.Increment(ref _dbStructureComparisonIndex);
    public int GetUnresolvedStructureComparisonKey() => Interlocked.Increment(ref _dbUnresolvedStructureComparisonIndex);
    public int GetElementTypeComparisonKey() => Interlocked.Increment(ref _dbElementTypeComparisonIndex);
    public int GetElementTypeGroupComparisonKey() => Interlocked.Increment(ref _dbElementTypeGroupComparisonIndex);
    public int GetElementComparisonKey() => Interlocked.Increment(ref _dbElementComparisonIndex);
    public int GetUnresolvedElementComparisonKey() => Interlocked.Increment(ref _dbUnresolvedElementComparisonIndex);

    private void getCurrentIndexValues()
    {
        try
        {
            _dbValueSetIndex = DbValueSet.SelectMaxKey(_dbConnection) ?? 0;
            _dbConceptIndex = DbValueSetConcept.SelectMaxKey(_dbConnection) ?? 0;
            _dbStructureIndex = DbStructureDefinition.SelectMaxKey(_dbConnection) ?? 0;
            _dbElementIndex = DbElement.SelectMaxKey(_dbConnection) ?? 0;
            _dbElementTypeIndex = DbElementType.SelectMaxKey(_dbConnection) ?? 0;

            _dbValueSetComparisonIndex = DbValueSetComparison.SelectMaxKey(_dbConnection) ?? 0;
            _dbConceptComparisonIndex = DbValueSetConceptComparison.SelectMaxKey(_dbConnection) ?? 0;
            _dbUnresolvedConceptComparisonIndex = DbUnresolvedConceptComparison.SelectMaxKey(_dbConnection) ?? 0;

            _dbStructureComparisonIndex = DbStructureComparison.SelectMaxKey(_dbConnection) ?? 0;
            _dbUnresolvedStructureComparisonIndex = DbUnresolvedStructureComparison.SelectMaxKey(_dbConnection) ?? 0;

            _dbElementTypeComparisonIndex = DbElementTypeComparison.SelectMaxKey(_dbConnection) ?? 0;
            _dbElementComparisonIndex = DbElementComparison.SelectMaxKey(_dbConnection) ?? 0;
            _dbUnresolvedElementComparisonIndex = DbUnresolvedElementComparison.SelectMaxKey(_dbConnection) ?? 0;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to get current index values from the database, assuming tables are all new.");
        }
    }

    public bool LoadFromSourceDb(
        string sourceDbPath,
        FhirArtifactClassEnum? artifactFilter = null)
    {
        if (string.IsNullOrEmpty(sourceDbPath))
        {
            return false;
        }

        if (!File.Exists(sourceDbPath))
        {
            return false;
        }

        string sourceConnectionString = new SqliteConnectionStringBuilder()
        {
            DataSource = sourceDbPath,
            Mode = SqliteOpenMode.ReadOnly,
        }.ToString();

        using IDbConnection sourceConnection = new SqliteConnection(sourceConnectionString);
        sourceConnection.Open();

        // recreate all local tables
        dropTables(_dbConnection, artifactFilter);
        createTables(_dbConnection, artifactFilter);

        // copy contents of each type
        copyContents(sourceConnection, _dbConnection, artifactFilter);

        // update our current index values
        getCurrentIndexValues();

        return true;
    }

    private void copyContents(
        IDbConnection sourceDb,
        IDbConnection targetDb,
        FhirArtifactClassEnum? processFilter = null)
    {
        switch (processFilter)
        {
            case FhirArtifactClassEnum.ValueSet:
                {
                    DbValueSet.Insert(targetDb, DbValueSet.SelectList(sourceDb));
                    DbValueSetConcept.Insert(targetDb, DbValueSetConcept.SelectList(sourceDb));
                    DbValueSetComparison.Insert(targetDb, DbValueSetComparison.SelectList(sourceDb));
                    DbValueSetConceptComparison.Insert(targetDb, DbValueSetConceptComparison.SelectList(sourceDb));
                    DbUnresolvedConceptComparison.Insert(targetDb, DbUnresolvedConceptComparison.SelectList(sourceDb));
                }
                break;

            case FhirArtifactClassEnum.PrimitiveType:
            case FhirArtifactClassEnum.ComplexType:
            case FhirArtifactClassEnum.Resource:
            case FhirArtifactClassEnum.Profile:
            case FhirArtifactClassEnum.LogicalModel:
                {
                    DbStructureDefinition.Insert(targetDb, DbStructureDefinition.SelectList(sourceDb));
                    DbElement.Insert(targetDb, DbElement.SelectList(sourceDb));
                    DbElementType.Insert(targetDb, DbElementType.SelectList(sourceDb));
                    DbStructureComparison.Insert(targetDb, DbStructureComparison.SelectList(sourceDb));
                    DbUnresolvedStructureComparison.Insert(targetDb, DbUnresolvedStructureComparison.SelectList(sourceDb));
                    DbElementTypeComparison.Insert(targetDb, DbElementTypeComparison.SelectList(sourceDb));
                    DbElementComparison.Insert(targetDb, DbElementComparison.SelectList(sourceDb));
                    DbUnresolvedElementComparison.Insert(targetDb, DbUnresolvedElementComparison.SelectList(sourceDb));
                }
                break;

            default:
                {
                    DbFhirPackage.Insert(targetDb, DbFhirPackage.SelectList(sourceDb));
                    DbFhirPackageComparisonPair.Insert(targetDb, DbFhirPackageComparisonPair.SelectList(sourceDb));

                    DbValueSet.Insert(targetDb, DbValueSet.SelectList(sourceDb));
                    DbValueSetConcept.Insert(targetDb, DbValueSetConcept.SelectList(sourceDb));
                    DbValueSetComparison.Insert(targetDb, DbValueSetComparison.SelectList(sourceDb));
                    DbValueSetConceptComparison.Insert(targetDb, DbValueSetConceptComparison.SelectList(sourceDb));
                    DbUnresolvedConceptComparison.Insert(targetDb, DbUnresolvedConceptComparison.SelectList(sourceDb));

                    DbStructureDefinition.Insert(targetDb, DbStructureDefinition.SelectList(sourceDb));
                    DbElement.Insert(targetDb, DbElement.SelectList(sourceDb));
                    DbElementType.Insert(targetDb, DbElementType.SelectList(sourceDb));
                    DbStructureComparison.Insert(targetDb, DbStructureComparison.SelectList(sourceDb));
                    DbUnresolvedStructureComparison.Insert(targetDb, DbUnresolvedStructureComparison.SelectList(sourceDb));
                    DbElementTypeComparison.Insert(targetDb, DbElementTypeComparison.SelectList(sourceDb));
                    DbElementComparison.Insert(targetDb, DbElementComparison.SelectList(sourceDb));
                    DbUnresolvedElementComparison.Insert(targetDb, DbUnresolvedElementComparison.SelectList(sourceDb));
                }
                break;
        }
    }

    private void dropTables(
        IDbConnection db,
        FhirArtifactClassEnum? processFilter = null)
    {
        switch (processFilter)
        {
            case FhirArtifactClassEnum.ValueSet:
                {
                    DbValueSet.DropTable(db);
                    DbValueSetConcept.DropTable(db);
                    DbValueSetComparison.DropTable(db);
                    DbValueSetConceptComparison.DropTable(db);
                }
                break;

            case FhirArtifactClassEnum.PrimitiveType:
            case FhirArtifactClassEnum.ComplexType:
            case FhirArtifactClassEnum.Resource:
            case FhirArtifactClassEnum.Profile:
            case FhirArtifactClassEnum.LogicalModel:
                {
                    DbStructureDefinition.DropTable(db);
                    DbElement.DropTable(db);
                    DbElementType.DropTable(db);
                    DbStructureComparison.DropTable(db);
                    DbUnresolvedStructureComparison.DropTable(db);
                    DbElementTypeComparison.DropTable(db);
                    DbElementComparison.DropTable(db);
                    DbUnresolvedElementComparison.DropTable(db);
                }
                break;

            default:
                {
                    DbFhirPackage.DropTable(db);
                    DbFhirPackageComparisonPair.DropTable(db);

                    DbValueSet.DropTable(db);
                    DbValueSetConcept.DropTable(db);
                    DbValueSetComparison.DropTable(db);
                    DbValueSetConceptComparison.DropTable(db);
                    DbUnresolvedConceptComparison.DropTable(db);

                    DbStructureDefinition.DropTable(db);
                    DbElement.DropTable(db);
                    DbElementType.DropTable(db);
                    DbStructureComparison.DropTable(db);
                    DbUnresolvedStructureComparison.DropTable(db);
                    DbElementTypeComparison.DropTable(db);
                    DbElementComparison.DropTable(db);
                    DbUnresolvedElementComparison.DropTable(db);
                }
                break;
        }
    }

    private void createTables(
        IDbConnection db,
        FhirArtifactClassEnum? processFilter = null)
    {
        switch (processFilter)
        {
            case FhirArtifactClassEnum.ValueSet:
                {
                    DbValueSet.CreateTable(db);
                    DbValueSetConcept.CreateTable(db);
                    DbValueSetComparison.CreateTable(db);
                    DbValueSetConceptComparison.CreateTable(db);
                }
                break;

            case FhirArtifactClassEnum.PrimitiveType:
            case FhirArtifactClassEnum.ComplexType:
            case FhirArtifactClassEnum.Resource:
            case FhirArtifactClassEnum.Profile:
            case FhirArtifactClassEnum.LogicalModel:
                {
                    DbStructureDefinition.CreateTable(db);
                    DbElement.CreateTable(db);
                    DbElementType.CreateTable(db);
                    DbStructureComparison.CreateTable(db);
                    DbUnresolvedStructureComparison.CreateTable(db);
                    DbElementTypeComparison.CreateTable(db);
                    DbElementComparison.CreateTable(db);
                    DbUnresolvedElementComparison.CreateTable(db);
                }
                break;

            default:
                {
                    DbFhirPackage.CreateTable(db);
                    DbFhirPackageComparisonPair.CreateTable(db);

                    DbValueSet.CreateTable(db);
                    DbValueSetConcept.CreateTable(db);
                    DbValueSetComparison.CreateTable(db);
                    DbValueSetConceptComparison.CreateTable(db);
                    DbUnresolvedConceptComparison.CreateTable(db);

                    DbStructureDefinition.CreateTable(db);
                    DbElement.CreateTable(db);
                    DbElementType.CreateTable(db);
                    DbStructureComparison.CreateTable(db);
                    DbUnresolvedStructureComparison.CreateTable(db);
                    DbElementTypeComparison.CreateTable(db);
                    DbElementComparison.CreateTable(db);
                    DbUnresolvedElementComparison.CreateTable(db);
                }
                break;
        }
    }

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
            dropTables(_dbConnection);
        }

        // create all our tables
        createTables(_dbConnection);

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

                _dbConnection.Insert(pm);
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
                    SourcePackageShortName = leftDbPackage.ShortName,
                    TargetPackageKey = rightDbPackage.Key,
                    TargetPackageShortName = rightDbPackage.ShortName,
                    ProccessedAt = DateTime.UtcNow,
                };

                _dbConnection.Insert(dbPairLtoR);
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
                    SourcePackageShortName = rightDbPackage.ShortName,
                    TargetPackageKey = leftDbPackage.Key,
                    TargetPackageShortName = leftDbPackage.ShortName,
                    ProccessedAt = DateTime.UtcNow,
                };

                _dbConnection.Insert(dbPairRtoL);
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

        DbComparisonCache<DbValueSetComparison> vsComparisons = new();
        DbComparisonCache<DbValueSetConceptComparison> conceptComparisons = new();

        DbComparisonCache<DbStructureComparison> sdComparisons = new();
        DbComparisonCache<DbElementComparison> elementComparisons = new();
        DbComparisonCache<DbElementTypeComparison> typeComparisons = new();

        List<DbUnresolvedConceptComparison> uresolvedConceptComparisons = [];
        List<DbUnresolvedStructureComparison> unresolvedSdComparisons = [];
        List<DbUnresolvedElementComparison> unresolvedElementComparisons = [];

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

            // load our primitive maps first so they have lower keys (nice when browsing data)
            loadDefaultPrimitiveMaps(left, leftDbPackage, right, rightDbPackage, dbPairLtoR);

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

            // load our primitive maps first so they have lower keys (nice when browsing data)
            loadDefaultPrimitiveMaps(right, rightDbPackage, left, leftDbPackage, dbPairRtoL);

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

        _logger.LogInformation("Inserting existing cross version maps into the database...");

        _dbConnection.Insert(vsComparisons.ComparisonsToAdd);
        _logger.LogInformation($" <<< added {vsComparisons.Count} ValueSet Comparisons");

        _dbConnection.Insert(conceptComparisons.ComparisonsToAdd);
        _logger.LogInformation($" <<< added {conceptComparisons.Count} ValueSet Concept Comparisons");

        _dbConnection.Insert(uresolvedConceptComparisons);
        _logger.LogInformation($" <<< added {uresolvedConceptComparisons.Count} Unresolved ValueSet Concept Comparisons");

        _dbConnection.Insert(sdComparisons.ComparisonsToAdd);
        _logger.LogInformation($" <<< added {sdComparisons.Count} Structure Comparisons");

        _dbConnection.Insert(unresolvedSdComparisons);
        _logger.LogInformation($" <<< added {unresolvedSdComparisons.Count} Unresolved Structure Comparisons");

        _dbConnection.Insert(elementComparisons.ComparisonsToAdd);
        _logger.LogInformation($" <<< added {elementComparisons.Count} Element Comparisons");

        _dbConnection.Insert(typeComparisons.ComparisonsToAdd);
        _logger.LogInformation($" <<< added {typeComparisons.Count} Element Type Comparisons");

        _dbConnection.Insert(unresolvedElementComparisons);
        _logger.LogInformation($" <<< added {unresolvedElementComparisons.Count} Unresolved Element Comparisons");

        return true;

        void loadDefaultPrimitiveMaps(
            DefinitionCollection source,
            DbFhirPackage sourceDbPackage,
            DefinitionCollection target,
            DbFhirPackage targetDbPackage,
            DbFhirPackageComparisonPair dbPackagePair)
        {
            // iterate over our known the primitives
            foreach (FhirTypeMappings.CodeGenTypeMapping tm in FhirTypeMappings.PrimitiveMappings)
            {
                // make sure each of the types exists
                if (!source.PrimitiveTypesByName.TryGetValue(tm.SourceType, out StructureDefinition? sourceSd) ||
                    !target.PrimitiveTypesByName.TryGetValue(tm.TargetType, out StructureDefinition? targetSd))
                {
                    continue;
                }

                // resolve the database records
                DbStructureDefinition sourceDbSd = DbStructureDefinition.SelectSingle(
                    _dbConnection,
                    FhirPackageKey: sourceDbPackage.Key,
                    Id: sourceSd.Id)
                    ?? throw new Exception($"Source structure {sourceSd.Id} not found in package: {sourceDbPackage.Key} ({sourceDbPackage.Name})!");

                DbStructureDefinition targetDbSd = DbStructureDefinition.SelectSingle(
                    _dbConnection,
                    FhirPackageKey: targetDbPackage.Key,
                    Id: targetSd.Id)
                    ?? throw new Exception($"Target structure {targetSd.Id} not found in package: {targetDbPackage.Key} ({targetDbPackage.Name})!");

                _ = sdComparisons.TryGet((targetDbSd.Key, sourceDbSd.Key), out DbStructureComparison? inverseSdComparison);

                // look for an existing comparison record
                if (!sdComparisons.TryGet((sourceDbSd.Key, targetDbSd.Key), out DbStructureComparison? sdComparison))
                {
                    // create our comparison record
                    sdComparison = new()
                    {
                        Key = Interlocked.Increment(ref _dbStructureComparisonIndex),
                        InverseComparisonKey = inverseSdComparison?.Key,
                        PackageComparisonKey = dbPackagePair.Key,
                        SourceFhirPackageKey = sourceDbPackage.Key,
                        TargetFhirPackageKey = targetDbPackage.Key,
                        SourceStructureKey = sourceDbSd.Key,
                        SourceCanonicalVersioned = sourceDbSd.VersionedUrl,
                        SourceCanonicalUnversioned = sourceDbSd.UnversionedUrl,
                        SourceVersion = sourceDbSd.Version,
                        SourceName = sourceDbSd.Name,
                        TargetStructureKey = targetDbSd.Key,
                        TargetCanonicalVersioned = targetDbSd.VersionedUrl,
                        TargetCanonicalUnversioned = targetDbSd.UnversionedUrl,
                        TargetVersion = targetDbSd.Version,
                        TargetName = targetDbSd.Name,
                        CompositeName = GetCompositeName(sourceDbPackage, targetDbSd, targetDbPackage, targetDbSd),
                        SourceOverviewConceptMapUrl = null,
                        SourceStructureFmlUrl = null,
                        Relationship = tm.Relationship,
                        ConceptDomainRelationship = tm.ConceptDomainRelationship,
                        ValueDomainRelationship = tm.ValueDomainRelationship,
                        IsGenerated = true,
                        LastReviewedBy = null,
                        LastReviewedOn = null,
                        Message = tm.Comment,
                    };

                    sdComparisons.CacheAdd(sdComparison);
                }

                if (inverseSdComparison != null)
                {
                    if (sdComparison.InverseComparisonKey != inverseSdComparison.Key)
                    {
                        // update the inverse comparison key
                        sdComparison.InverseComparisonKey = inverseSdComparison.Key;
                        sdComparisons.Changed(sdComparison);
                    }

                    if (inverseSdComparison.InverseComparisonKey != sdComparison.Key)
                    {
                        // update the inverse comparison key
                        inverseSdComparison.InverseComparisonKey = sdComparison.Key;
                        sdComparisons.Changed(inverseSdComparison);
                    }
                }
            }
        }


        void processCrossVersionMaps(
            DefinitionCollection source,
            DbFhirPackage sourceDbPackage,
            DefinitionCollection target,
            DbFhirPackage targetDbPackage,
            CrossVersionMapCollection cv,
            DbFhirPackageComparisonPair dbPackagePair)
        {
            HashSet<string> processedOverviewMaps = [];

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
                    case CommonDefinitions.ConceptMapUsageContextResourceOverview:
                        {
                            if (processedOverviewMaps.Contains(cm.Url))
                            {
                                continue;
                            }

                            processOverviewMap(dbPackagePair, sourceDbPackage, targetDbPackage, cm);
                            processedOverviewMaps.Add(cm.Url);
                        }
                        break;

                    case CommonDefinitions.ConceptMapUsageContextDataType:
                    case CommonDefinitions.ConceptMapUsageContextResource:
                        {
                            processStructureConceptMap(dbPackagePair, sourceDbPackage, targetDbPackage, cm);
                        }
                        break;

                    default:
                        continue;
                }
            }
        }

        void processStructureConceptMap(
            DbFhirPackageComparisonPair dbPackagePair,
            DbFhirPackage sourceDbPackage,
            DbFhirPackage targetDbPackage,
            ConceptMap cm)
        {
            string sourceVersioned = cm.SourceScope.ToString()!;
            string? targetVersioned = cm.TargetScope?.ToString();

            DbStructureDefinition? sourceDbSd = DbStructureDefinition.SelectSingle(
                _dbConnection,
                FhirPackageKey: sourceDbPackage.Key,
                VersionedUrl: sourceVersioned);

            DbStructureDefinition? targetDbSd = (targetVersioned == null)
                ? null
                : DbStructureDefinition.SelectSingle(
                    _dbConnection,
                    FhirPackageKey: targetDbPackage.Key,
                    VersionedUrl: targetVersioned);

            DbStructureComparison? sdComparison = null;
            DbStructureComparison? inverseSdComparison = null;

            // if we have a source and a target, get or make a comparison record
            if ((sourceDbSd != null) &&
                (targetDbSd != null))
            {
                inverseSdComparison = DbStructureComparison.SelectSingle(
                    _dbConnection,
                    PackageComparisonKey: dbPackagePair.Key,
                    SourceStructureKey: targetDbSd.Key,
                    TargetStructureKey: sourceDbSd.Key);

                if (inverseSdComparison == null)
                {
                    // check the cache
                    _ = sdComparisons.TryGet((targetDbSd.Key, sourceDbSd.Key), out inverseSdComparison);
                }

                sdComparison = DbStructureComparison.SelectSingle(
                    _dbConnection,
                    PackageComparisonKey: dbPackagePair.Key,
                    SourceStructureKey: sourceDbSd.Key,
                    TargetStructureKey: targetDbSd.Key);

                if (sdComparison == null)
                {
                    // check the cache
                    _ = sdComparisons.TryGet((sourceDbSd.Key, targetDbSd.Key), out sdComparison);
                }

                // if we still don't have a record, create one
                if (sdComparison == null)
                {
                    sdComparison = new()
                    {
                        Key = Interlocked.Increment(ref _dbStructureComparisonIndex),
                        PackageComparisonKey = dbPackagePair.Key,
                        SourceFhirPackageKey = sourceDbPackage.Key,
                        TargetFhirPackageKey = targetDbPackage.Key,
                        SourceStructureKey = sourceDbSd.Key,
                        SourceCanonicalVersioned = sourceDbSd.VersionedUrl,
                        SourceCanonicalUnversioned = sourceDbSd.UnversionedUrl,
                        SourceVersion = sourceDbSd.Version,
                        SourceName = sourceDbSd.Name,
                        TargetStructureKey = targetDbSd.Key,
                        TargetCanonicalVersioned = targetDbSd.VersionedUrl,
                        TargetCanonicalUnversioned = targetDbSd.UnversionedUrl,
                        TargetVersion = targetDbSd.Version,
                        TargetName = targetDbSd.Name,
                        CompositeName = GetCompositeName(sourceDbPackage, targetDbSd, targetDbPackage, targetDbSd),
                        SourceOverviewConceptMapUrl = cm.Url,
                        SourceStructureFmlUrl = null,
                        Relationship = null,
                        ConceptDomainRelationship = (sourceDbSd.Name == targetDbSd.Name) ? CMR.Equivalent : null,
                        ValueDomainRelationship = FhirDbComparer.RelationshipForCounts(sourceDbSd.SnapshotCount, targetDbSd.SnapshotCount),
                        IsGenerated = false,
                        LastReviewedBy = null,
                        LastReviewedOn = null,
                        Message = $"Imported from existing ConceptMap {cm.Id} ({cm.Url}).",
                    };

                    sdComparisons.CacheAdd(sdComparison);
                }

                if (inverseSdComparison != null)
                {
                    if (sdComparison.InverseComparisonKey != inverseSdComparison.Key)
                    {
                        // update the inverse comparison key
                        sdComparison.InverseComparisonKey = inverseSdComparison.Key;
                        sdComparisons.Changed(sdComparison);
                    }
                    if (inverseSdComparison.InverseComparisonKey != sdComparison.Key)
                    {
                        // update the inverse comparison key
                        inverseSdComparison.InverseComparisonKey = sdComparison.Key;
                        sdComparisons.Changed(inverseSdComparison);
                    }
                }
            }

            DbUnresolvedStructureComparison? unresolvedSdComparison = DbUnresolvedStructureComparison.SelectSingle(
                _dbConnection,
                PackageComparisonKey: dbPackagePair.Key,
                SourceCanonicalVersioned: sourceVersioned,
                TargetCanonicalVersioned: targetVersioned);

            if ((sdComparison == null) &&
                (unresolvedSdComparison == null))
            {
                unresolvedSdComparison = new()
                {
                    Key = Interlocked.Increment(ref _dbUnresolvedStructureComparisonIndex),
                    PackageComparisonKey = dbPackagePair.Key,
                    SourceFhirPackageKey = sourceDbPackage.Key,
                    TargetFhirPackageKey = targetDbPackage.Key,
                    SourceStructureKey = sourceDbSd?.Key,
                    SourceCanonicalVersioned = sourceDbSd?.VersionedUrl,
                    SourceCanonicalUnversioned = sourceDbSd?.UnversionedUrl,
                    SourceVersion = sourceDbSd?.Version,
                    SourceName = sourceDbSd?.Name,
                    TargetStructureKey = targetDbSd?.Key,
                    TargetCanonicalVersioned = targetDbSd?.VersionedUrl,
                    TargetCanonicalUnversioned = targetDbSd?.UnversionedUrl,
                    TargetVersion = targetDbSd?.Version,
                    TargetName = targetDbSd?.Name,
                    ConceptMapId = cm.Id,
                    ConceptMapUrl = cm.Url,
                    Relationship = null,
                    IsGenerated = false,
                    LastReviewedBy = null,
                    LastReviewedOn = null,
                    Message = $"Imported from existing ConceptMap {cm.Id} ({cm.Url}).",
                };

                unresolvedSdComparisons.Add(unresolvedSdComparison);
            }

            // iterate over the groups
            foreach (ConceptMap.GroupComponent mapGroup in cm.Group)
            {
                // iterate over the source elements
                foreach (ConceptMap.SourceElementComponent mapSourceElement in mapGroup.Element)
                {
                    DbElement? sourceDbElement = (sourceDbSd == null)
                        ? null
                        : DbElement.SelectSingle(
                            _dbConnection,
                            FhirPackageKey: sourceDbPackage.Key,
                            StructureKey: sourceDbSd.Key,
                            Path: mapSourceElement.Code);

                    // check for no-map
                    if (mapSourceElement.NoMap == true)
                    {
                        addElementComparison(
                            dbPackagePair,
                            sourceDbSd,
                            targetDbSd,
                            sdComparison,
                            unresolvedSdComparison,
                            cm,
                            sourceDbElement,
                            sourceStructureUrl: sourceVersioned,
                            sourceToken: mapSourceElement.Code,
                            mapSourceElement.NoMap,
                            relationship: null,
                            comment: $"Loaded from existing Concept Map {cm.Id} ({cm.Url})",
                            targetStructureUrl: targetVersioned);

                        continue;
                    }

                    // iterate over the targets
                    foreach (ConceptMap.TargetElementComponent mapTargetElement in mapSourceElement.Target)
                    {
                        string comment = string.IsNullOrEmpty(mapTargetElement.Comment)
                            ? $"Loaded from existing Concept Map {cm.Id} ({cm.Url})"
                            : mapTargetElement.Comment;

                        DbElement? targetDbElement = (targetDbSd == null)
                            ? null
                            : DbElement.SelectSingle(
                                _dbConnection,
                                FhirPackageKey: targetDbPackage.Key,
                                StructureKey: targetDbSd.Key,
                                Path: mapTargetElement.Code);

                        addElementComparison(
                            dbPackagePair,
                            sourceDbSd,
                            targetDbSd,
                            sdComparison,
                            unresolvedSdComparison,
                            cm,
                            sourceDbElement,
                            sourceStructureUrl: sourceVersioned,
                            sourceToken: mapSourceElement.Code,
                            mapSourceElement.NoMap,
                            mapTargetElement.Relationship,
                            comment,
                            targetDbElement,
                            targetStructureUrl: targetVersioned,
                            targetToken: mapTargetElement.Code);

                        continue;
                    }
                }
            }

        }

        void addElementComparison(
            DbFhirPackageComparisonPair dbPackagePair,
            DbStructureDefinition? sourceDbSd,
            DbStructureDefinition? targetDbSd,
            DbStructureComparison? dbSdComparison,
            DbUnresolvedStructureComparison? dbUnresolvedSdComparison,
            ConceptMap cm,
            DbElement? sourceDbElement,
            string sourceStructureUrl,
            string sourceToken,
            bool? noMap,
            CMR? relationship,
            string? comment,
            DbElement? targetDbElement = null,
            string? targetStructureUrl = null,
            string? targetToken = null)
        {
            if ((dbUnresolvedSdComparison != null) ||
                (dbSdComparison == null) ||
                (sourceDbSd == null))
            {
                comment ??= string.Empty;

                DbUnresolvedElementComparison unresolved = new()
                {
                    Key = Interlocked.Increment(ref _dbUnresolvedElementComparisonIndex),
                    PackageComparisonKey = dbPackagePair.Key,
                    SourceFhirPackageKey = dbPackagePair.SourcePackageKey,
                    SourceStructureKey = sourceDbSd?.Key,
                    TargetFhirPackageKey = dbPackagePair.TargetPackageKey,
                    TargetStructureKey = targetDbSd?.Key,
                    StructureComparisonKey = dbSdComparison?.Key,
                    UnresolvedStructureComparisonKey = dbUnresolvedSdComparison?.Key,
                    Relationship = relationship,
                    NoMap = noMap,
                    Message = comment + $" Record found in {cm.Id} ({cm.Url}) with unresolved structure mapping {sourceStructureUrl} -> {targetStructureUrl}.",
                    SourceElementExists = sourceDbElement != null,
                    SourceStructureUrl = sourceStructureUrl,
                    SourceElementToken = sourceToken,
                    TargetStructureUrl = targetStructureUrl,
                    TargetElementToken = targetToken,
                    TargetElementExists = targetDbElement != null,
                    IsGenerated = false,
                    LastReviewedBy = null,
                    LastReviewedOn = null,
                };

                unresolvedElementComparisons.Add(unresolved);

                return;
            }

            if ((sourceDbElement == null) ||
                ((targetDbElement == null) && (targetToken != null)))
            {
                comment ??= string.Empty;

                DbUnresolvedElementComparison unresolved = new()
                {
                    Key = Interlocked.Increment(ref _dbUnresolvedElementComparisonIndex),
                    PackageComparisonKey = dbPackagePair.Key,
                    SourceFhirPackageKey = dbPackagePair.SourcePackageKey,
                    SourceStructureKey = sourceDbSd.Key,
                    TargetFhirPackageKey = dbPackagePair.TargetPackageKey,
                    TargetStructureKey = targetDbSd?.Key,
                    StructureComparisonKey = dbSdComparison?.Key,
                    UnresolvedStructureComparisonKey = dbUnresolvedSdComparison?.Key,
                    Relationship = relationship,
                    NoMap = noMap,
                    Message = comment + $" Record found in {cm.Id} ({cm.Url}) with unresolved element mapping {sourceStructureUrl}:{sourceToken} -> {targetStructureUrl}:{targetToken}.",
                    SourceElementExists = sourceDbElement != null,
                    SourceStructureUrl = sourceStructureUrl,
                    SourceElementToken = sourceToken,
                    TargetStructureUrl = targetStructureUrl,
                    TargetElementToken = targetToken,
                    TargetElementExists = targetDbElement != null,
                    IsGenerated = false,
                    LastReviewedBy = null,
                    LastReviewedOn = null,
                };

                unresolvedElementComparisons.Add(unresolved);

                return;
            }

            // check the cache for an inverse record
            DbElementComparison? inverseComparison = null;

            if (targetDbElement != null)
            {
                _ = elementComparisons.TryGet((targetDbElement.Key, sourceDbElement.Key), out inverseComparison);
            }

            // create our record
            DbElementComparison resolved = new()
            {
                Key = Interlocked.Increment(ref _dbElementComparisonIndex),
                InverseComparisonKey = inverseComparison?.Key,
                PackageComparisonKey = dbPackagePair.Key,
                SourceFhirPackageKey = dbPackagePair.SourcePackageKey,
                SourceStructureKey = sourceDbSd.Key,
                TargetFhirPackageKey = dbPackagePair.TargetPackageKey,
                TargetStructureKey = targetDbSd?.Key,
                StructureComparisonKey = dbSdComparison.Key,
                Relationship = relationship,
                NoMap = noMap,
                ConceptDomainRelationship = null,
                ValueDomainRelationship = null,
                Message = comment,
                SourceElementKey = sourceDbElement.Key,
                SourceStructureUrl = sourceStructureUrl,
                SourceElementToken = sourceToken,
                TargetElementKey = targetDbElement?.Key,
                TargetStructureUrl = targetStructureUrl,
                TargetElementToken = targetToken,
                ElementTypeComparisonKey = null,
                BoundValueSetComparisonKey = null,
                IsGenerated = false,
                LastReviewedBy = null,
                LastReviewedOn = null,
            };

            elementComparisons.CacheAdd(resolved);

            if (inverseComparison != null)
            {
                if (resolved.InverseComparisonKey != inverseComparison.Key)
                {
                    // update the inverse comparison key
                    resolved.InverseComparisonKey = inverseComparison.Key;
                    elementComparisons.Changed(resolved);
                }

                if (inverseComparison.InverseComparisonKey != resolved.Key)
                {
                    // update the inverse comparison key
                    inverseComparison.InverseComparisonKey = resolved.Key;
                    elementComparisons.Changed(inverseComparison);
                }
            }

            return;
        }

        void processOverviewMap(
            DbFhirPackageComparisonPair dbPackagePair,
            DbFhirPackage sourceDbPackage,
            DbFhirPackage targetDbPackage,
            ConceptMap cm)
        {
            // process each group
            foreach (ConceptMap.GroupComponent cmGroup in cm.Group)
            {
                foreach (ConceptMap.SourceElementComponent groupSource in cmGroup.Element)
                {
                    DbStructureDefinition? sourceDbSd = DbStructureDefinition.SelectSingle(
                        _dbConnection,
                        FhirPackageKey: sourceDbPackage.Key,
                        Id: groupSource.Code);

                    // iterate over the target types
                    foreach (ConceptMap.TargetElementComponent groupTarget in groupSource.Target)
                    {
                        DbStructureDefinition? targetDbSd = DbStructureDefinition.SelectSingle(
                            _dbConnection,
                            FhirPackageKey: targetDbPackage.Key,
                            Id: groupTarget.Code);

                        if ((sourceDbSd == null) || (targetDbSd == null))
                        {
                            string message = $"Mapping from {groupSource.Code} to {groupTarget.Code} exists in {cm.Id} ({cm.Url}) but";
                            
                            if (sourceDbSd == null)
                            {
                                message += $" {groupSource.Code} does not exist in source package {sourceDbPackage.PackageId}@{sourceDbPackage.PackageVersion}";
                            }

                            if (targetDbSd == null)
                            {
                                message += $" {groupTarget.Code} does not exist in target package {targetDbPackage.PackageId}@{targetDbPackage.PackageVersion}";
                            }

                            DbUnresolvedStructureComparison dbUnresolvedSdComparison = new()
                            {
                                Key = Interlocked.Increment(ref _dbUnresolvedStructureComparisonIndex),
                                PackageComparisonKey = dbPackagePair.Key,
                                SourceFhirPackageKey = sourceDbPackage.Key,
                                TargetFhirPackageKey = targetDbPackage.Key,
                                SourceStructureKey = sourceDbSd?.Key,
                                SourceCanonicalVersioned = sourceDbSd?.VersionedUrl,
                                SourceCanonicalUnversioned = sourceDbSd?.UnversionedUrl,
                                SourceVersion = sourceDbSd?.Version,
                                SourceName = sourceDbSd?.Name,
                                TargetStructureKey = targetDbSd?.Key,
                                TargetCanonicalVersioned = targetDbSd?.VersionedUrl,
                                TargetCanonicalUnversioned = targetDbSd?.UnversionedUrl,
                                TargetVersion = targetDbSd?.Version,
                                TargetName = targetDbSd?.Name,
                                ConceptMapId = cm.Id,
                                ConceptMapUrl = cm.Url,
                                Relationship = groupTarget.Relationship,
                                IsGenerated = false,
                                LastReviewedBy = null,
                                LastReviewedOn = null,
                                Message = message,
                            };

                            unresolvedSdComparisons.Add(dbUnresolvedSdComparison);

                            continue;
                        }

                        // skip if they are both primitives - that mapping is handled by FhirTypeMappings.CodeGenTypeMapping for consistency
                        if ((sourceDbSd.ArtifactClass == FhirArtifactClassEnum.PrimitiveType) &&
                            (targetDbSd.ArtifactClass == FhirArtifactClassEnum.PrimitiveType))
                        {
                            continue;
                        }

                        // create our record
                        DbStructureComparison dbSdComparison = new()
                        {
                            Key = Interlocked.Increment(ref _dbStructureComparisonIndex),
                            PackageComparisonKey = dbPackagePair.Key,
                            SourceFhirPackageKey = sourceDbPackage.Key,
                            TargetFhirPackageKey = targetDbPackage.Key,
                            SourceStructureKey = sourceDbSd.Key,
                            SourceCanonicalVersioned = sourceDbSd.VersionedUrl,
                            SourceCanonicalUnversioned = sourceDbSd.UnversionedUrl,
                            SourceVersion = sourceDbSd.Version,
                            SourceName = sourceDbSd.Name,
                            TargetStructureKey = targetDbSd.Key,
                            TargetCanonicalVersioned = targetDbSd.VersionedUrl,
                            TargetCanonicalUnversioned = targetDbSd.UnversionedUrl,
                            TargetVersion = targetDbSd.Version,
                            TargetName = targetDbSd.Name,
                            CompositeName = GetCompositeName(sourceDbPackage, targetDbSd, targetDbPackage, targetDbSd),
                            SourceOverviewConceptMapUrl = cm.Url,
                            SourceStructureFmlUrl = null,
                            Relationship = groupTarget.Relationship,
                            ConceptDomainRelationship = (sourceDbSd.Name == targetDbSd.Name) ? CMR.Equivalent : CMR.RelatedTo,
                            ValueDomainRelationship = FhirDbComparer.RelationshipForCounts(sourceDbSd.SnapshotCount, targetDbSd.SnapshotCount),
                            IsGenerated = false,
                            LastReviewedBy = null,
                            LastReviewedOn = null,
                            Message = $"Imported from existing ConceptMap {cm.Id} ({cm.Url}).",
                        };

                        sdComparisons.CacheAdd(dbSdComparison);
                    }
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

            _ = vsComparisons.TryGet((targetDbVs.Key, sourceDbVs.Key), out DbValueSetComparison? inverseComparison);

            // create our canonical comparison record
            DbValueSetComparison dbVsComparison = new()
            {
                Key = Interlocked.Increment(ref _dbValueSetComparisonIndex),
                InverseComparisonKey = inverseComparison?.Key,
                PackageComparisonKey = dbPackagePair.Key,
                SourceFhirPackageKey = sourceDbPackage.Key,
                TargetFhirPackageKey = targetDbPackage.Key,
                SourceValueSetKey = sourceDbVs.Key,
                SourceCanonicalVersioned = sourceDbVs.VersionedUrl,
                SourceCanonicalUnversioned = sourceDbVs.UnversionedUrl,
                SourceVersion = sourceDbVs.Version,
                SourceName = sourceDbVs.Name,
                TargetValueSetKey = targetDbVs.Key,
                TargetCanonicalVersioned = targetDbVs.VersionedUrl,
                TargetCanonicalUnversioned = targetDbVs.UnversionedUrl,
                TargetVersion = targetDbVs.Version,
                TargetName = targetDbVs.Name,
                CompositeName = GetCompositeName(sourceDbPackage, sourceDbVs, targetDbPackage, targetDbVs),
                SourceConceptMapUrl = cm.Url,
                SourceConceptMapAdditionalUrls = additionalUrls.Count == 0 ? null : string.Join(", ", additionalUrls),
                Relationship = null,
                IsGenerated = false,
                LastReviewedBy = null,
                LastReviewedOn = null,
                Message = $"Imported from existing ConceptMap {cm.Id} ({cm.Url}).",
            };

            vsComparisons.CacheAdd(dbVsComparison);

            if (inverseComparison != null)
            {
                if (dbVsComparison.InverseComparisonKey != inverseComparison.Key)
                {
                    // update the inverse comparison key
                    dbVsComparison.InverseComparisonKey = inverseComparison.Key;
                    vsComparisons.Changed(dbVsComparison);
                }

                if (inverseComparison.InverseComparisonKey != dbVsComparison.Key)
                {
                    // update the inverse comparison key
                    inverseComparison.InverseComparisonKey = dbVsComparison.Key;
                    vsComparisons.Changed(inverseComparison);
                }
            }

            // check for manual exclusion
            if (sourceDbVs.IsExcluded)
            {
                dbVsComparison.Message += $" Note: comparison source: {sourceDbVs.Id} ({sourceDbVs.VersionedUrl}) has been manually excluded.";
            }

            if (targetDbVs.IsExcluded)
            {
                dbVsComparison.Message += $" Note: comparison target: {targetDbVs.Id} ({targetDbVs.VersionedUrl}) has been manually excluded.";
            }

            // check for failure to expand
            if (sourceDbVs.CanExpand == false)
            {
                dbVsComparison.Message += $" Note: source value set {sourceDbVs.Id} ({sourceDbVs.VersionedUrl}) cannot be expanded.";
            }

            if (targetDbVs.CanExpand == false)
            {
                dbVsComparison.Message += $" Note: target value set {targetDbVs.Id} ({targetDbVs.VersionedUrl}) cannot be expanded.";
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
                        addVsConceptComparison(
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

                        addVsConceptComparison(
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

        void addVsConceptComparison(
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
                    DbUnresolvedConceptComparison unresolvedNoMap = new()
                    {
                        Key = Interlocked.Increment(ref _dbUnresolvedConceptComparisonIndex),
                        PackageComparisonKey = dbPackagePair.Key,
                        SourceFhirPackageKey = dbPackagePair.SourcePackageKey,
                        SourceValueSetKey = sourceDbVs.Key,
                        TargetFhirPackageKey = dbPackagePair.TargetPackageKey,
                        TargetValueSetKey = targetDbVs.Key,
                        ValueSetComparisonKey = dbVsComparison.Key,
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

                    uresolvedConceptComparisons.Add(unresolvedNoMap);

                    ////dbPackagePair.InvalidImportedConceptComparisons.Add(invalidComparison);
                    //dbVsComparison.InvalidImportedComparisons.Add(invalidComparison);

                    return;
                }

                DbValueSetConceptComparison nonMappedComparison = new()
                {
                    Key = Interlocked.Increment(ref _dbConceptComparisonIndex),
                    PackageComparisonKey = dbPackagePair.Key,
                    SourceFhirPackageKey = dbPackagePair.SourcePackageKey,
                    SourceValueSetKey = sourceDbVs.Key,
                    TargetFhirPackageKey = dbPackagePair.TargetPackageKey,
                    TargetValueSetKey = targetDbVs.Key,
                    ValueSetComparisonKey = dbVsComparison.Key,
                    SourceConceptKey = sourceDbConcept.Key,
                    TargetConceptKey = null,
                    Relationship = null,
                    NoMap = true,
                    Message = $"Code flagged as noMap in {cm.Id} ({cm.Url})",
                    IsGenerated = false,
                    LastReviewedBy = null,
                    LastReviewedOn = null,
                };

                conceptComparisons.CacheAdd(nonMappedComparison);

                return;
            }

            if ((sourceDbConcept == null) || (targetDbConcept == null))
            {
                bool sourceExists = sourceDbConcept != null;
                bool targetExists = targetDbConcept != null;

                string message = $"Mapping exists in {cm.Id} ({cm.Url}), but:";

                if (!sourceExists)
                {
                    message += $" {sourceSystem}|{sourceCode} does not exist in source {sourceDbVs.Id} ({sourceDbVs.VersionedUrl}).";
                }

                if (!targetExists)
                {
                    message += $" {targetSystem}|{targetCode} does not exist in target {targetDbVs.Id} ({targetDbVs.VersionedUrl}).";
                }

                DbUnresolvedConceptComparison unresolvedComparison = new()
                {
                    Key = Interlocked.Increment(ref _dbUnresolvedConceptComparisonIndex),
                    PackageComparisonKey = dbPackagePair.Key,
                    SourceFhirPackageKey = dbPackagePair.SourcePackageKey,
                    SourceValueSetKey = sourceDbVs.Key,
                    TargetFhirPackageKey = dbPackagePair.TargetPackageKey,
                    TargetValueSetKey = targetDbVs.Key,
                    ValueSetComparisonKey = dbVsComparison.Key,
                    ConceptMapId = cm.Id,
                    ConceptMapUrl = cm.Url,
                    Relationship = relationship,
                    NoMap = targetCode == null,
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

                uresolvedConceptComparisons.Add(unresolvedComparison);

                return;
            }

            // check the cache for an inverse record
            _ = conceptComparisons.TryGet((targetDbConcept.Key, sourceDbConcept.Key), out DbValueSetConceptComparison? inverseComparison);

            DbValueSetConceptComparison mappedComparison = new()
            {
                Key = Interlocked.Increment(ref _dbConceptComparisonIndex),
                InverseComparisonKey = inverseComparison?.Key,
                PackageComparisonKey = dbPackagePair.Key,
                SourceFhirPackageKey = dbPackagePair.SourcePackageKey,
                SourceValueSetKey = sourceDbVs.Key,
                TargetFhirPackageKey = dbPackagePair.TargetPackageKey,
                TargetValueSetKey = targetDbVs.Key,
                ValueSetComparisonKey = dbVsComparison.Key,
                SourceConceptKey = sourceDbConcept.Key,
                TargetConceptKey = targetDbConcept.Key,
                Relationship = relationship,
                NoMap = false,
                Message = $"Loaded existing mapping of `{sourceDbVs.VersionedUrl}`#`{sourceDbConcept.Code}` to `{targetDbVs.VersionedUrl}`#`{targetDbConcept.Code}` by {cm.Id} ({cm.Url})",
                IsGenerated = false,
                LastReviewedBy = null,
                LastReviewedOn = null,
            };

            conceptComparisons.CacheAdd(mappedComparison);

            if (inverseComparison != null)
            {
                if (mappedComparison.InverseComparisonKey != inverseComparison.Key)
                {
                    // update the inverse comparison key
                    mappedComparison.InverseComparisonKey = inverseComparison.Key;
                    conceptComparisons.Changed(mappedComparison);
                }

                if (inverseComparison.InverseComparisonKey != mappedComparison.Key)
                {
                    // update the inverse comparison key
                    inverseComparison.InverseComparisonKey = mappedComparison.Key;
                    conceptComparisons.Changed(inverseComparison);
                }
            }
        }
    }

    internal static string GetCompositeName(
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

    internal static string GetCompositeName(
        string sourcePackageShortName,
        string sourceCanonicalName,
        string targetPacakgeShortName,
        string? targetCanonicalName)
    {
        if (targetCanonicalName == null)
        {
            return
                $"{sourcePackageShortName}-" +
                $"{FhirSanitizationUtils.SanitizeForProperty(sourceCanonicalName).ToPascalCase()}-" +
                $"{targetPacakgeShortName}";
        }

        return
            $"{sourcePackageShortName}-" +
            $"{FhirSanitizationUtils.SanitizeForProperty(sourceCanonicalName).ToPascalCase()}-" +
            $"{targetPacakgeShortName}-" +
            $"{FhirSanitizationUtils.SanitizeForProperty(targetCanonicalName).ToPascalCase()}";
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
                    ActiveConcreteConceptCount = 0,
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

                continue;
            }

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
                ActiveConcreteConceptCount = 0,
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

            List<DbValueSetConcept> dbConcepts = [];
            int conceptCount = 0;
            int activeConcreteConceptCount = 0;

            // iterate over all the contents of the value set
            foreach (FhirConcept fc in vs.cgGetFlatConcepts(dc))
            {
                conceptCount++;

                // check for inactive or abstract
                if ((fc.IsInactive != true) &&
                    (fc.IsAbstract != true))
                {
                    activeConcreteConceptCount++;
                }

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
                    Inactive = (fc.IsInactive == true),
                    Abstract = (fc.IsAbstract == true),
                    Properties = fc.Properties.Length == 0 ? null : string.Join(", ", fc.Properties.Select(p => $"{p.Code}={p.Value}")),
                };

                dbConcepts.Add(dbConcept);
            }

            dbVs.ConceptCount = conceptCount;
            dbVs.ActiveConcreteConceptCount = activeConcreteConceptCount;

            allDbConcepts.AddRange(dbConcepts);
        }

        _logger.LogInformation($"Inserting ValueSets for {pm.PackageId}@{pm.PackageVersion} into database...");

        _dbConnection.Insert(dbValueSets);
        _logger.LogInformation($" <<< added {dbValueSets.Count} ValueSets");

        _dbConnection.Insert(allDbConcepts);
        _logger.LogInformation($" <<< added {allDbConcepts.Count} ValueSet Concepts");

        return;
    }

    private void addStructuresToDb(
        DbFhirPackage pm,
        DefinitionCollection dc,
        HashSet<string> _exclusionSet)
    {
        Dictionary<string, DbStructureDefinition> dbStructures = [];
        List<DbElement> dbElements = [];
        List<DbElementType> dbElementTypes = [];

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
                        SnapshotCount = sd.Snapshot?.Element.Count ?? 0,
                        DifferentialCount = sd.Differential?.Element.Count ?? 0,
                    };

                    dbStructures.Add(sd.Id, sdmExcluded);

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
                    SnapshotCount = sd.Snapshot?.Element.Count ?? 0,
                    DifferentialCount = sd.Differential?.Element.Count ?? 0,
                };

                dbStructures.Add(sd.Id, dbStructure);

                // iterate over all the elements of the structure
                foreach (ElementDefinition ed in sd.cgElements(skipSlices: false))
                {
                    addElement(dbStructure, sd, ed);
                }
            }
        }

        // resolve the type structure keys (need to to after all structures have been added)
        foreach (DbElementType dbElementType in dbElementTypes)
        {
            if (string.IsNullOrEmpty(dbElementType.TypeName))
            {
                continue;
            }

            if (dbStructures.TryGetValue(dbElementType.TypeName, out DbStructureDefinition? dbStructure))
            {
                dbElementType.TypeStructureKey = dbStructure.Key;
            }
        }

        // save changes

        _logger.LogInformation($"Inserting Structures for {pm.PackageId}@{pm.PackageVersion} into database...");

        _dbConnection.Insert(dbStructures.Values);
        _logger.LogInformation($" <<< added {dbStructures.Count} Structures");

        _dbConnection.Insert(dbElements);
        _logger.LogInformation($" <<< added {dbElements.Count} Elements");

        _dbConnection.Insert(dbElementTypes);
        _logger.LogInformation($" <<< added {dbElementTypes.Count} Element Types");

        return;

        // TODO(ginoc): For now, exclude profiles and logical models - we will want them for generic packages, but do not care for core
        (IEnumerable<StructureDefinition> structures, FhirArtifactClassEnum cgClass)[] getStructures(DefinitionCollection dc) => [
            (dc.PrimitiveTypesByName.Values, FhirArtifactClassEnum.PrimitiveType),
            (dc.ComplexTypesByName.Values, FhirArtifactClassEnum.ComplexType),
            (dc.ResourcesByName.Values, FhirArtifactClassEnum.Resource),
            (dc.ExtensionsByUrl.Values, FhirArtifactClassEnum.Extension),
            //(dc.ProfilesByUrl.Values, FhirArtifactClassEnum.Profile),
            //(dc.LogicalModelsByUrl.Values, FhirArtifactClassEnum.LogicalModel),
            ];

        string literalForType(string? typeName, string? typeProfile, string? targetProfile) =>
            (string.IsNullOrEmpty(typeName) ? string.Empty : typeName) +
            (string.IsNullOrEmpty(typeProfile) ? string.Empty : $"[{typeProfile}]") +
            (string.IsNullOrEmpty(targetProfile) ? string.Empty : $"({targetProfile})");
        
        void addElement(
            DbStructureDefinition dbStructure,
            StructureDefinition sd,
            ElementDefinition ed)
        {
            int elementKey = Interlocked.Increment(ref _dbElementIndex);

            // check for children
            int childCount = sd.cgElements(
                ed.Path,
                topLevelOnly: true,
                includeRoot: false,
                skipSlices: true).Count();

            List<DbElementType> currentElementTypes = [];

            int? bindingVsKey = ed.Binding?.ValueSet == null
                ? null
                : (DbValueSet.SelectSingle(_dbConnection, FhirPackageKey: pm.Key, UnversionedUrl: ed.Binding?.ValueSet)?.Key
                  ?? DbValueSet.SelectSingle(_dbConnection, FhirPackageKey: pm.Key, VersionedUrl: ed.Binding?.ValueSet)?.Key);

            IEnumerable<ElementDefinition.TypeRefComponent> definedTypes = ed.Type.Select(tr => tr.cgAsR5());
            foreach (ElementDefinition.TypeRefComponent tr in definedTypes)
            {
                if ((tr.ProfileElement.Count == 0) &&
                    (tr.TargetProfileElement.Count == 0))
                {
                    string tl = literalForType(tr.cgName(), null, null);

                    DbElementType et = new()
                    {
                        Key = Interlocked.Increment(ref _dbElementTypeIndex),
                        FhirPackageKey = dbStructure.FhirPackageKey,
                        StructureKey = dbStructure.Key,
                        ElementKey = elementKey,
                        TypeName = tr.cgName(),
                        TypeProfile = null,
                        TargetProfile = null,
                        TypeStructureKey = null,
                    };
                    currentElementTypes.Add(et);
                    dbElementTypes.Add( et);

                    continue;
                }

                if (tr.ProfileElement.Count == 0)
                {
                    foreach (Canonical tp in tr.TargetProfile)
                    {
                        string tl = literalForType(tr.cgName(), null, tp.Value);
                        DbElementType et = new()
                        {
                            Key = Interlocked.Increment(ref _dbElementTypeIndex),
                            FhirPackageKey = dbStructure.FhirPackageKey,
                            StructureKey = dbStructure.Key,
                            ElementKey = elementKey,
                            TypeName = tr.cgName(),
                            TypeProfile = null,
                            TargetProfile = tp.Value,
                            TypeStructureKey = null,
                        };
                        currentElementTypes.Add(et);
                        dbElementTypes.Add(et);
                    }

                    continue;
                }

                if (tr.TargetProfileElement.Count == 0)
                {
                    foreach (Canonical p in tr.Profile)
                    {
                        string tl = literalForType(tr.cgName(), p.Value, null);
                        DbElementType et = new()
                        {
                            Key = Interlocked.Increment(ref _dbElementTypeIndex),
                            FhirPackageKey = dbStructure.FhirPackageKey,
                            StructureKey = dbStructure.Key,
                            ElementKey = elementKey,
                            TypeName = tr.cgName(),
                            TypeProfile = p.Value,
                            TargetProfile = null,
                            TypeStructureKey = null,
                        };
                        currentElementTypes.Add(et);
                        dbElementTypes.Add(et);
                    }

                    continue;
                }

                foreach (Canonical p in tr.Profile)
                {
                    foreach (Canonical tp in tr.TargetProfile)
                    {
                        string tl = literalForType(tr.cgName(), p.Value, tp.Value);
                        DbElementType et = new()
                        {
                            Key = Interlocked.Increment(ref _dbElementTypeIndex),
                            FhirPackageKey = dbStructure.FhirPackageKey,
                            StructureKey = dbStructure.Key,
                            ElementKey = elementKey,
                            TypeName = tr.cgName(),
                            TypeProfile = p.Value,
                            TargetProfile = tp.Value,
                            TypeStructureKey = null,
                        };
                        currentElementTypes.Add(et);
                        dbElementTypes.Add(et);
                    }
                }
            }

            if (currentElementTypes.Count == 0)
            {
                if (ed.ElementId == sd.Id)
                {
                    string tl = literalForType(sd.Id, null, null);

                    DbElementType et = new()
                    {
                        Key = Interlocked.Increment(ref _dbElementTypeIndex),
                        FhirPackageKey = dbStructure.FhirPackageKey,
                        StructureKey = dbStructure.Key,
                        ElementKey = elementKey,
                        TypeName = sd.Id,
                        TypeProfile = null,
                        TargetProfile = null,
                        TypeStructureKey = null,
                    };
                    currentElementTypes.Add(et);
                    dbElementTypes.Add(et);
                }
                else
                {
                    string btn = ed.cgBaseTypeName(dc, true);

                    string tl = literalForType(btn, null, null);

                    DbElementType et = new()
                    {
                        Key = Interlocked.Increment(ref _dbElementTypeIndex),
                        FhirPackageKey = dbStructure.FhirPackageKey,
                        StructureKey = dbStructure.Key,
                        ElementKey = elementKey,
                        TypeName = btn,
                        TypeProfile = null,
                        TargetProfile = null,
                        TypeStructureKey = null,
                    };
                    currentElementTypes.Add(et);
                    dbElementTypes.Add(et);
                }
            }

            bool isInherited = ed.cgIsInherited(sd);
            string? basePath = ed.Base?.Path;

            // check for a type group that has all the types we need
            string typeGroupLiteral = string.Join(", ", currentElementTypes.OrderBy(et => et.Literal).Select(et => et.Literal));

            DbElement dbElement = new()
            {
                Key = elementKey,
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
                BindingValueSetKey = bindingVsKey,
                CollatedTypeLiteral = typeGroupLiteral,
                IsInherited = isInherited,
                BasePath = basePath,
            };

            dbElements.Add(dbElement);
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
