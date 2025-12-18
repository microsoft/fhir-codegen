using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using Antlr4.Runtime.Tree.Xpath;
using Fhir.CodeGen.Common.Extensions;
using Fhir.CodeGen.Common.FhirExtensions;
using Fhir.CodeGen.Common.Models;
using Fhir.CodeGen.Common.Packaging;
using Fhir.CodeGen.Common.Utils;
using Fhir.CodeGen.Comparison.CompareTool;
using Fhir.CodeGen.Comparison.Extensions;
using Fhir.CodeGen.Comparison.Models;
using Fhir.CodeGen.Comparison.XVer;
using Fhir.CodeGen.Lib.FhirExtensions;
using Fhir.CodeGen.Lib.Loader;
using Fhir.CodeGen.Lib.Models;
using Fhir.CodeGen.MappingLanguage;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Hl7.Fhir.Specification;
using Hl7.Fhir.Utility;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Octokit;
using static Fhir.CodeGen.Comparison.CompareTool.FhirTypeMappings;
using CMR = Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship;

namespace Fhir.CodeGen.Comparison.Models;


internal static partial class ContentDatabaseLogMessages
{
    [LoggerMessage(Level = LogLevel.Warning, Message = "Failed to resolve ValueSet {vsUrl} in {sourcePackage}:{sourceVersion}.")]
    internal static partial void LogFailedToResolveVs(this ILogger logger, string vsUrl, string sourcePackage, string sourceVersion);
}


public class ComparisonDatabase : IDisposable
{
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
    private PackageLoader? _loader = null;


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
        _logger = loggerFactory?.CreateLogger<ComparisonDatabase>()
            ?? definitions[0].Logger
            ?? LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<ComparisonDatabase>();
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
                    _dbName = $"{definitions[0].MainPackageId.ToPascalCase()}-V{definitions[0].MainPackageVersion.Replace('.', '_')}.sqlite";
                    break;
                case 2:
                    {
                        if (_isVersionComparison)
                        {
                            _dbName = string.Join('_', definitions.Select(dc => $"V{dc.MainPackageVersion.Replace('.', '_')}")) + ".sqlite";
                        }
                        else if (_isCoreComparison)
                        {
                            _dbName = string.Join('_', definitions.Select(dc => $"{dc.FhirSequence.ToRLiteral()}")) + ".sqlite";
                        }
                        else
                        {
                            _dbName = string.Join('_', definitions.Select(dc => $"{dc.MainPackageId.ToPascalCase()}-V{dc.MainPackageVersion.Replace('.', '_')}")) + ".sqlite";
                        }
                    }
                    break;
                default:
                    _dbName = "fhir-comparison.sqlite";
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
        _logger = loggerFactory?.CreateLogger<ComparisonDatabase>()
            ?? LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<ComparisonDatabase>()
            ?? NullLoggerFactory.Instance.CreateLogger(nameof(ComparisonDatabase));

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

    private void getCurrentIndexValues()
    {
        try
        {
            DbFhirPackage.LoadMaxKey(_dbConnection);
            //DbFhirPackageComparisonPair.LoadMaxKey(_dbConnection);

            DbCodeSystem.LoadMaxKey(_dbConnection);
            DbCodeSystemPropertyDefinition.LoadMaxKey(_dbConnection);
            DbCodeSystemFilter.LoadMaxKey(_dbConnection);
            DbCodeSystemConcept.LoadMaxKey(_dbConnection);
            DbCodeSystemConceptProperty.LoadMaxKey(_dbConnection);

            DbValueSet.LoadMaxKey(_dbConnection);
            DbValueSetConcept.LoadMaxKey(_dbConnection);

            DbStructureDefinition.LoadMaxKey(_dbConnection);
            DbElement.LoadMaxKey(_dbConnection);
            DbElementType.LoadMaxKey(_dbConnection);
            DbCollatedType.LoadMaxKey(_dbConnection);
            DbElementAdditionalBinding.LoadMaxKey(_dbConnection);

            //DbValueSetComparison.LoadMaxKey(_dbConnection);
            //DbValueSetConceptComparison.LoadMaxKey(_dbConnection);
            //DbUnresolvedConceptComparison.LoadMaxKey(_dbConnection);

            //DbStructureComparison.LoadMaxKey(_dbConnection);
            //DbUnresolvedStructureComparison.LoadMaxKey(_dbConnection);

            //DbElementComparison.LoadMaxKey(_dbConnection);
            //DbElementTypeComparison.LoadMaxKey(_dbConnection);
            //DbCollatedTypeComparison.LoadMaxKey(_dbConnection);
            //DbUnresolvedElementComparison.LoadMaxKey(_dbConnection);

            DbExtensionSubstitution.LoadMaxKey(_dbConnection);
            DbExternalInclusion.LoadMaxKey(_dbConnection);

            //DbValueSetOutcome.LoadMaxKey(_dbConnection);
            //DbValueSetConceptOutcome.LoadMaxKey(_dbConnection);

            //DbStructureOutcome.LoadMaxKey(_dbConnection);
            //DbElementOutcome.LoadMaxKey(_dbConnection);
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
            case FhirArtifactClassEnum.CodeSystem:
                {
                    DbCodeSystem.Insert(targetDb, DbCodeSystem.SelectList(sourceDb), ignoreDuplicates: true, insertPrimaryKey: true);
                    DbCodeSystemPropertyDefinition.Insert(targetDb, DbCodeSystemPropertyDefinition.SelectList(sourceDb), ignoreDuplicates: true, insertPrimaryKey: true);
                    DbCodeSystemFilter.Insert(targetDb, DbCodeSystemFilter.SelectList(sourceDb), ignoreDuplicates: true, insertPrimaryKey: true);
                    DbCodeSystemConcept.Insert(targetDb, DbCodeSystemConcept.SelectList(sourceDb), ignoreDuplicates: true, insertPrimaryKey: true);
                    DbCodeSystemConceptProperty.Insert(targetDb, DbCodeSystemConceptProperty.SelectList(sourceDb), ignoreDuplicates: true, insertPrimaryKey: true);
                    DbExternalInclusion.Insert(targetDb, DbExternalInclusion.SelectList(sourceDb), ignoreDuplicates: true, insertPrimaryKey: true);
                }
                break;

            case FhirArtifactClassEnum.ValueSet:
                {
                    DbValueSet.Insert(targetDb, DbValueSet.SelectList(sourceDb), ignoreDuplicates: true, insertPrimaryKey: true);
                    DbValueSetConcept.Insert(targetDb, DbValueSetConcept.SelectList(sourceDb), ignoreDuplicates: true, insertPrimaryKey: true);
                    //DbValueSetComparison.Insert(targetDb, DbValueSetComparison.SelectList(sourceDb), ignoreDuplicates: true, insertPrimaryKey: true);
                    //DbValueSetConceptComparison.Insert(targetDb, DbValueSetConceptComparison.SelectList(sourceDb), ignoreDuplicates: true, insertPrimaryKey: true);
                    //DbUnresolvedConceptComparison.Insert(targetDb, DbUnresolvedConceptComparison.SelectList(sourceDb), ignoreDuplicates: true, insertPrimaryKey: true);
                    DbExternalInclusion.Insert(targetDb, DbExternalInclusion.SelectList(sourceDb), ignoreDuplicates: true, insertPrimaryKey: true);
                }
                break;

            case FhirArtifactClassEnum.PrimitiveType:
            case FhirArtifactClassEnum.ComplexType:
            case FhirArtifactClassEnum.Resource:
            case FhirArtifactClassEnum.Profile:
            case FhirArtifactClassEnum.LogicalModel:
                {
                    DbStructureDefinition.Insert(targetDb, DbStructureDefinition.SelectList(sourceDb), ignoreDuplicates: true, insertPrimaryKey: true);
                    DbElement.Insert(targetDb, DbElement.SelectList(sourceDb), ignoreDuplicates: true, insertPrimaryKey: true);
                    DbElementType.Insert(targetDb, DbElementType.SelectList(sourceDb), ignoreDuplicates: true, insertPrimaryKey: true);
                    DbCollatedType.Insert(targetDb, DbCollatedType.SelectList(sourceDb), ignoreDuplicates: true, insertPrimaryKey: true);
                    DbElementAdditionalBinding.Insert(targetDb, DbElementAdditionalBinding.SelectList(sourceDb), ignoreDuplicates: true, insertPrimaryKey: true);
                    //DbStructureComparison.Insert(targetDb, DbStructureComparison.SelectList(sourceDb), ignoreDuplicates: true, insertPrimaryKey: true);
                    //DbUnresolvedStructureComparison.Insert(targetDb, DbUnresolvedStructureComparison.SelectList(sourceDb), ignoreDuplicates: true, insertPrimaryKey: true);
                    //DbElementComparison.Insert(targetDb, DbElementComparison.SelectList(sourceDb), ignoreDuplicates: true, insertPrimaryKey: true);
                    //DbElementTypeComparison.Insert(targetDb, DbElementTypeComparison.SelectList(sourceDb), ignoreDuplicates: true, insertPrimaryKey: true);
                    //DbCollatedTypeComparison.Insert(targetDb, DbCollatedTypeComparison.SelectList(sourceDb), ignoreDuplicates: true, insertPrimaryKey: true);
                    //DbUnresolvedElementComparison.Insert(targetDb, DbUnresolvedElementComparison.SelectList(sourceDb), ignoreDuplicates: true, insertPrimaryKey: true);

                    DbExtensionSubstitution.Insert(targetDb, DbExtensionSubstitution.SelectList(sourceDb), ignoreDuplicates: true, insertPrimaryKey: true);
                    DbExternalInclusion.Insert(targetDb, DbExternalInclusion.SelectList(sourceDb), ignoreDuplicates: true, insertPrimaryKey: true);
                }
                break;

            default:
                {
                    DbFhirPackage.Insert(targetDb, DbFhirPackage.SelectList(sourceDb), ignoreDuplicates: true, insertPrimaryKey: true);
                    //DbFhirPackageComparisonPair.Insert(targetDb, DbFhirPackageComparisonPair.SelectList(sourceDb), ignoreDuplicates: true, insertPrimaryKey: true);

                    DbCodeSystem.Insert(targetDb, DbCodeSystem.SelectList(sourceDb), ignoreDuplicates: true, insertPrimaryKey: true);
                    DbCodeSystemPropertyDefinition.Insert(targetDb, DbCodeSystemPropertyDefinition.SelectList(sourceDb), ignoreDuplicates: true, insertPrimaryKey: true);
                    DbCodeSystemFilter.Insert(targetDb, DbCodeSystemFilter.SelectList(sourceDb), ignoreDuplicates: true, insertPrimaryKey: true);
                    DbCodeSystemConcept.Insert(targetDb, DbCodeSystemConcept.SelectList(sourceDb), ignoreDuplicates: true, insertPrimaryKey: true);
                    DbCodeSystemConceptProperty.Insert(targetDb, DbCodeSystemConceptProperty.SelectList(sourceDb), ignoreDuplicates: true, insertPrimaryKey: true);

                    DbValueSet.Insert(targetDb, DbValueSet.SelectList(sourceDb), ignoreDuplicates: true, insertPrimaryKey: true);
                    DbValueSetConcept.Insert(targetDb, DbValueSetConcept.SelectList(sourceDb), ignoreDuplicates: true, insertPrimaryKey: true);
                    //DbValueSetComparison.Insert(targetDb, DbValueSetComparison.SelectList(sourceDb), ignoreDuplicates: true, insertPrimaryKey: true);
                    //DbValueSetConceptComparison.Insert(targetDb, DbValueSetConceptComparison.SelectList(sourceDb), ignoreDuplicates: true, insertPrimaryKey: true);
                    //DbUnresolvedConceptComparison.Insert(targetDb, DbUnresolvedConceptComparison.SelectList(sourceDb), ignoreDuplicates: true, insertPrimaryKey: true);

                    DbStructureDefinition.Insert(targetDb, DbStructureDefinition.SelectList(sourceDb), ignoreDuplicates: true, insertPrimaryKey: true);
                    DbElement.Insert(targetDb, DbElement.SelectList(sourceDb), ignoreDuplicates: true, insertPrimaryKey: true);
                    DbElementType.Insert(targetDb, DbElementType.SelectList(sourceDb), ignoreDuplicates: true, insertPrimaryKey: true);
                    DbCollatedType.Insert(targetDb, DbCollatedType.SelectList(sourceDb), ignoreDuplicates: true, insertPrimaryKey: true);
                    DbElementAdditionalBinding.Insert(targetDb, DbElementAdditionalBinding.SelectList(sourceDb), ignoreDuplicates: true, insertPrimaryKey: true);
                    //DbStructureComparison.Insert(targetDb, DbStructureComparison.SelectList(sourceDb), ignoreDuplicates: true, insertPrimaryKey: true);
                    //DbUnresolvedStructureComparison.Insert(targetDb, DbUnresolvedStructureComparison.SelectList(sourceDb), ignoreDuplicates: true, insertPrimaryKey: true);
                    //DbElementComparison.Insert(targetDb, DbElementComparison.SelectList(sourceDb), ignoreDuplicates: true, insertPrimaryKey: true);
                    //DbElementTypeComparison.Insert(targetDb, DbElementTypeComparison.SelectList(sourceDb), ignoreDuplicates: true, insertPrimaryKey: true);
                    //DbCollatedTypeComparison.Insert(targetDb, DbCollatedTypeComparison.SelectList(sourceDb), ignoreDuplicates: true, insertPrimaryKey: true);
                    //DbUnresolvedElementComparison.Insert(targetDb, DbUnresolvedElementComparison.SelectList(sourceDb), ignoreDuplicates: true, insertPrimaryKey: true);

                    DbExtensionSubstitution.Insert(targetDb, DbExtensionSubstitution.SelectList(sourceDb), ignoreDuplicates: true, insertPrimaryKey: true);
                    DbExternalInclusion.Insert(targetDb, DbExternalInclusion.SelectList(sourceDb), ignoreDuplicates: true, insertPrimaryKey: true);
                }
                break;
        }
    }

    private void dropMapTables(IDbConnection db)
    {
        DbValueSetMapRecord.DropTable(db);
        DbValueSetConceptMapRecord.DropTable(db);
        DbStructureMappingRecord.DropTable(db);
        DbElementMappingRecord.DropTable(db);
    }

    private void createMapTables(IDbConnection db)
    {
        DbValueSetMapRecord.CreateTable(db);
        DbValueSetConceptMapRecord.CreateTable(db);
        DbStructureMappingRecord.CreateTable(db);
        DbElementMappingRecord.CreateTable(db);
    }

    private void dropTables(
        IDbConnection db,
        FhirArtifactClassEnum? processFilter = null)
    {
        switch (processFilter)
        {
            case FhirArtifactClassEnum.CodeSystem:
                {
                    DbCodeSystem.DropTable(db);
                    DbCodeSystemPropertyDefinition.DropTable(db);
                    DbCodeSystemFilter.DropTable(db);
                    DbCodeSystemConcept.DropTable(db);
                    DbCodeSystemConceptProperty.DropTable(db);
                    DbExternalInclusion.DropTable(db);
                }
                break;

            case FhirArtifactClassEnum.ValueSet:
                {
                    DbValueSet.DropTable(db);
                    DbValueSetConcept.DropTable(db);
                    //DbValueSetComparison.DropTable(db);
                    //DbValueSetConceptComparison.DropTable(db);
                    DbExternalInclusion.DropTable(db);

                    //DbValueSetOutcome.DropTable(db);
                    //DbValueSetConceptOutcome.DropTable(db);
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
                    DbCollatedType.DropTable(db);
                    DbElementAdditionalBinding.DropTable(db);
                    //DbStructureComparison.DropTable(db);
                    //DbUnresolvedStructureComparison.DropTable(db);
                    //DbElementComparison.DropTable(db);
                    //DbElementTypeComparison.DropTable(db);
                    //DbCollatedTypeComparison.DropTable(db);
                    //DbUnresolvedElementComparison.DropTable(db);

                    DbExtensionSubstitution.DropTable(db);
                    DbExternalInclusion.DropTable(db);

                    //DbStructureOutcome.DropTable(db);
                    //DbElementOutcome.DropTable(db);
                }
                break;

            default:
                {
                    DbFhirPackage.DropTable(db);
                    //DbFhirPackageComparisonPair.DropTable(db);

                    DbCodeSystem.DropTable(db);
                    DbCodeSystemPropertyDefinition.DropTable(db);
                    DbCodeSystemFilter.DropTable(db);
                    DbCodeSystemConcept.DropTable(db);
                    DbCodeSystemConceptProperty.DropTable(db);

                    DbValueSet.DropTable(db);
                    DbValueSetConcept.DropTable(db);
                    //DbValueSetComparison.DropTable(db);
                    //DbValueSetConceptComparison.DropTable(db);
                    //DbUnresolvedConceptComparison.DropTable(db);

                    //DbValueSetOutcome.DropTable(db);
                    //DbValueSetConceptOutcome.DropTable(db);

                    DbStructureDefinition.DropTable(db);
                    DbElement.DropTable(db);
                    DbElementType.DropTable(db);
                    DbCollatedType.DropTable(db);
                    DbElementAdditionalBinding.DropTable(db);
                    //DbStructureComparison.DropTable(db);
                    //DbUnresolvedStructureComparison.DropTable(db);
                    //DbElementComparison.DropTable(db);
                    //DbElementTypeComparison.DropTable(db);
                    //DbCollatedTypeComparison.DropTable(db);
                    //DbUnresolvedElementComparison.DropTable(db);

                    DbExtensionSubstitution.DropTable(db);
                    DbExternalInclusion.DropTable(db);

                    //DbStructureOutcome.DropTable(db);
                    //DbElementOutcome.DropTable(db);
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
            case FhirArtifactClassEnum.CodeSystem:
                {
                    DbCodeSystem.CreateTable(db);
                    DbCodeSystemPropertyDefinition.CreateTable(db);
                    DbCodeSystemFilter.CreateTable(db);
                    DbCodeSystemConcept.CreateTable(db);
                    DbCodeSystemConceptProperty.CreateTable(db);
                    DbExternalInclusion.CreateTable(db);
                }
                break;

            case FhirArtifactClassEnum.ValueSet:
                {
                    DbValueSet.CreateTable(db);
                    DbValueSetConcept.CreateTable(db);
                    //DbValueSetComparison.CreateTable(db);
                    //DbValueSetConceptComparison.CreateTable(db);
                    DbExternalInclusion.CreateTable(db);

                    //DbValueSetOutcome.CreateTable(db);
                    //DbValueSetConceptOutcome.CreateTable(db);
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
                    DbCollatedType.CreateTable(db);
                    DbElementAdditionalBinding.CreateTable(db);
                    //DbStructureComparison.CreateTable(db);
                    //DbUnresolvedStructureComparison.CreateTable(db);
                    //DbElementComparison.CreateTable(db);
                    //DbElementTypeComparison.CreateTable(db);
                    //DbCollatedTypeComparison.CreateTable(db);
                    //DbUnresolvedElementComparison.CreateTable(db);
                    DbExternalInclusion.CreateTable(db);

                    //DbStructureOutcome.CreateTable(db);
                    //DbElementOutcome.CreateTable(db);
                }
                break;

            default:
                {
                    DbFhirPackage.CreateTable(db);
                    //DbFhirPackageComparisonPair.CreateTable(db);

                    DbCodeSystem.CreateTable(db);
                    DbCodeSystemPropertyDefinition.CreateTable(db);
                    DbCodeSystemFilter.CreateTable(db);
                    DbCodeSystemConcept.CreateTable(db);
                    DbCodeSystemConceptProperty.CreateTable(db);

                    DbValueSet.CreateTable(db);
                    DbValueSetConcept.CreateTable(db);
                    //DbValueSetComparison.CreateTable(db);
                    //DbValueSetConceptComparison.CreateTable(db);
                    //DbUnresolvedConceptComparison.CreateTable(db);
                    //DbValueSetOutcome.CreateTable(db);
                    //DbValueSetConceptOutcome.CreateTable(db);

                    DbStructureDefinition.CreateTable(db);
                    DbElement.CreateTable(db);
                    DbCollatedType.CreateTable(db);
                    DbElementType.CreateTable(db);
                    DbElementAdditionalBinding.CreateTable(db);
                    //DbStructureComparison.CreateTable(db);
                    //DbUnresolvedStructureComparison.CreateTable(db);
                    //DbElementComparison.CreateTable(db);
                    //DbElementTypeComparison.CreateTable(db);
                    //DbCollatedTypeComparison.CreateTable(db);
                    //DbUnresolvedElementComparison.CreateTable(db);
                    //DbStructureOutcome.CreateTable(db);
                    //DbElementOutcome.CreateTable(db);

                    DbExtensionSubstitution.CreateTable(db);
                    DbExternalInclusion.CreateTable(db);
                }
                break;
        }
    }

    private void loadKnownSubstitutions()
    {
        List<DbExtensionSubstitution> substitutions = [
            new()
            {
                Key = DbExtensionSubstitution.GetIndex(),
                ReplacementUrl = "http://hl7.org/fhir/StructureDefinition/patient-animal",
                SourceElementId = "Patient.animal",
                SourceVersion = FhirReleases.FhirSequenceCodes.DSTU2,
                Context = "Patient",
            },
            new()
            {
                Key = DbExtensionSubstitution.GetIndex(),
                ReplacementUrl = "http://hl7.org/fhir/StructureDefinition/conceptmap-notarget-comment",
                SourceElementId = "ConceptMap.group.element.target.comment",
                SourceVersion = FhirReleases.FhirSequenceCodes.R5,
                Context = "ConceptMap.group.element",
            },
            new()
            {
                Key = DbExtensionSubstitution.GetIndex(),
                ReplacementUrl = "http://hl7.org/fhir/tools/StructureDefinition/additional-binding",
                SourceElementId = "ElementDefinition.binding.additional",
                SourceVersion = FhirReleases.FhirSequenceCodes.R5,
                Context = "ElementDefinition.binding",
            }
            //new()
            //{
            //    Key = GetExtensionSubstitutionKey(),
            //    ReplacementUrl = "http://hl7.org/fhir/guide-parameter-code",
            //    SourceElementId = "ImplementationGuide.definition.parameter",
            //    Context = "ImplementationGuide.definition.parameter",
            //},
        ];

        substitutions.Insert(_dbConnection, insertPrimaryKey: true);
    }

    private void loadKnownExternalInclusions()
    {
        List<DbExternalInclusion> inclusions = [
            new()
            {
                Key = DbExternalInclusion.GetIndex(),
                ResourceType = FHIRAllTypes.CodeSystem,
                Id = "designation-usage",
                Name = "Designation Usage",
                Version = "4.2.0",
                OverrideVersion = false,
                UnversionedUrl = "http://terminology.hl7.org/CodeSystem/designation-usage",
                VersionedUrl = "http://terminology.hl7.org/CodeSystem/designation-usage|4.2.0",
                Reason = "This CodeSystem was only published in hl7.terminology@1.0.0. Currently working through resolution.",
                IncludeInPackages = null,
                Json = """
                {
                  "resourceType": "CodeSystem",
                  "id": "designation-usage",
                  "meta": {
                    "profile": [
                      "http://hl7.org/fhir/StructureDefinition/shareablecodesystem"
                    ]
                  },
                  "text": {
                    "status": "generated",
                    "div": "<div xmlns=\"http://www.w3.org/1999/xhtml\"><h2>Designation Usage</h2><div><p>Preferred value set for Condition Categories.</p>\n</div><p>This code system http://terminology.hl7.org/CodeSystem/designation-usage defines the following codes:</p><table class=\"codes\"><tr><td style=\"white-space:nowrap\"><b>Code</b></td><td><b>Display</b></td><td><b>Definition</b></td></tr><tr><td style=\"white-space:nowrap\">display<a name=\"designation-usage-display\"> </a></td><td>Display</td><td>A deisgnation suitable for display to an end-user</td></tr></table></div>"
                  },
                  "extension": [
                    {
                      "url": "http://hl7.org/fhir/StructureDefinition/structuredefinition-wg",
                      "valueCode": "fhir"
                    }
                  ],
                  "url": "http://terminology.hl7.org/CodeSystem/designation-usage",
                  "version": "4.2.0",
                  "name": "DesignationUsage",
                  "title": "Designation Usage",
                  "status": "draft",
                  "experimental": false,
                  "date": "2020-05-09T12:49:00-04:00",
                  "publisher": "FHIR Project team",
                  "contact": [
                    {
                      "telecom": [
                        {
                          "system": "url",
                          "value": "http://hl7.org/fhir"
                        }
                      ]
                    }
                  ],
                  "description": "Preferred value set for Condition Categories.",
                  "caseSensitive": true,
                  "valueSet": "http://terminology.hl7.org/ValueSet/designation-usage",
                  "content": "complete",
                  "concept": [
                    {
                      "code": "display",
                      "display": "Display",
                      "definition": "A deisgnation suitable for display to an end-user"
                    }
                  ]
                }
                """,
            },
            new()
            {
                Key = DbExternalInclusion.GetIndex(),
                ResourceType = FHIRAllTypes.ValueSet,
                Id = "designation-usage",
                Name = "Designation Usage",
                Version = "4.1.0",
                OverrideVersion = false,
                UnversionedUrl = "http://terminology.hl7.org/ValueSet/designation-usage",
                VersionedUrl = "http://terminology.hl7.org/ValueSet/designation-usage|4.1.0",
                Reason = "This ValueSet was only published in hl7.terminology@1.0.0. Currently working through resolution.",
                IncludeInPackages = null,
                Json = """
                {
                  "resourceType": "ValueSet",
                  "id": "designation-usage",
                  "meta": {
                    "profile": [
                      "http://hl7.org/fhir/StructureDefinition/shareablevalueset"
                    ]
                  },
                  "text": {
                    "status": "generated",
                    "div": "<div xmlns=\"http://www.w3.org/1999/xhtml\"><h2>Designation Usage</h2><div><p>Preferred value set for Condition Categories.</p>\n</div><ul><li>Include all codes defined in <a href=\"CodeSystem-designation-usage.html\"><code>http://terminology.hl7.org/CodeSystem/designation-usage</code></a></li></ul></div>"
                  },
                  "extension": [
                    {
                      "url": "http://hl7.org/fhir/StructureDefinition/structuredefinition-wg",
                      "valueCode": "fhir"
                    }
                  ],
                  "url": "http://terminology.hl7.org/ValueSet/designation-usage",
                  "version": "4.1.0",
                  "name": "DesignationUsage",
                  "title": "Designation Usage",
                  "status": "draft",
                  "experimental": false,
                  "date": "2020-02-23T20:41:39-05:00",
                  "publisher": "HL7 (FHIR Project)",
                  "contact": [
                    {
                      "telecom": [
                        {
                          "system": "url",
                          "value": "http://hl7.org/fhir"
                        },
                        {
                          "system": "email",
                          "value": "fhir@lists.hl7.org"
                        }
                      ]
                    }
                  ],
                  "description": "Preferred value set for Condition Categories.",
                  "immutable": true,
                  "compose": {
                    "include": [
                      {
                        "system": "http://terminology.hl7.org/CodeSystem/designation-usage"
                      }
                    ]
                  }
                }
                """,
            }
        ];

        inclusions.Insert(_dbConnection, insertPrimaryKey: true);
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

            List<string> deps = dc.Manifests.Keys
                .Where(key => !key.id.StartsWith(dc.MainPackageId, StringComparison.OrdinalIgnoreCase))
                .Select(key => key.id + "@" + key.version)
                .ToList();

            // add data about our packages
            if (DbFhirPackage.SelectSingle(_dbConnection, PackageId: dc.MainPackageId, PackageVersion: dc.MainPackageVersion) is not DbFhirPackage pm)
            {
                pm = new()
                {
                    Name = dc.Name,
                    ShortName = shortName,
                    PackageId = dc.MainPackageId,
                    PackageVersion = dc.MainPackageVersion,
                    FhirVersionShort = dc.FhirSequence.ToShortVersion(),
                    CanonicalUrl = dc.MainPackageCanonical,
                    DefinitionFhirSequence = dc.FhirSequence,
                    Dependencies = deps.Count > 0 ? string.Join(",", deps) : null,
                };

                _dbConnection.Insert(pm);
            }
        }

        //// look for cross-version collections
        //for (int definitionIndex = 1; definitionIndex < _definitions.Length; definitionIndex++)
        //{
        //    DefinitionCollection left = _definitions[definitionIndex - 1].dc;
        //    DefinitionCollection right = _definitions[definitionIndex].dc;

        //    // get the db package definitions
        //    DbFhirPackage leftDbPackage = DbFhirPackage.SelectSingle(_dbConnection, PackageId: left.MainPackageId, PackageVersion: left.MainPackageVersion)
        //        ?? throw new Exception($"Package {left.MainPackageId}@{left.MainPackageVersion} was not found in the database!");
        //    DbFhirPackage rightDbPackage = DbFhirPackage.SelectSingle(_dbConnection, PackageId: right.MainPackageId, PackageVersion: right.MainPackageVersion)
        //        ?? throw new Exception($"Package {right.MainPackageId}@{right.MainPackageVersion} was not found in the database!");

        //    // check for a package pair for left-to-right comparison
        //    DbFhirPackageComparisonPair? dbPairLtoR = DbFhirPackageComparisonPair.SelectSingle(
        //        _dbConnection,
        //        SourcePackageKey: leftDbPackage.Key,
        //        TargetPackageKey: rightDbPackage.Key);

        //    bool insertLtoR = false;
        //    bool insertRtoL = false;

        //    if (dbPairLtoR == null)
        //    {
        //        insertLtoR = true;
        //        dbPairLtoR = new()
        //        {
        //            Key = DbFhirPackageComparisonPair.GetIndex(),
        //            SourcePackageKey = leftDbPackage.Key,
        //            SourcePackageShortName = leftDbPackage.ShortName,
        //            TargetPackageKey = rightDbPackage.Key,
        //            TargetPackageShortName = rightDbPackage.ShortName,
        //            ProcessedAt = DateTime.UtcNow,
        //        };
        //    }

        //    // check for a package pair for right-to-left comparison
        //    DbFhirPackageComparisonPair? dbPairRtoL = DbFhirPackageComparisonPair.SelectSingle(
        //        _dbConnection,
        //        SourcePackageKey: rightDbPackage.Key,
        //        TargetPackageKey: leftDbPackage.Key);

        //    if (dbPairRtoL == null)
        //    {
        //        insertRtoL = true;
        //        dbPairRtoL = new()
        //        {
        //            Key = DbFhirPackageComparisonPair.GetIndex(),
        //            InverseComparisonKey = dbPairLtoR.Key,
        //            SourcePackageKey = rightDbPackage.Key,
        //            SourcePackageShortName = rightDbPackage.ShortName,
        //            TargetPackageKey = leftDbPackage.Key,
        //            TargetPackageShortName = leftDbPackage.ShortName,
        //            ProcessedAt = DateTime.UtcNow,
        //        };

        //        dbPairLtoR.InverseComparisonKey = dbPairRtoL.Key;
        //    }

        //    dbPairLtoR.InverseComparisonKey = dbPairRtoL.Key;
        //    dbPairRtoL.InverseComparisonKey = dbPairLtoR.Key;

        //    if (insertLtoR)
        //    {
        //        _dbConnection.Insert(dbPairLtoR, insertPrimaryKey: true);
        //    }
        //    else
        //    {
        //        _dbConnection.Update(dbPairLtoR);
        //    }

        //    if (insertRtoL)
        //    {
        //        _dbConnection.Insert(dbPairRtoL, insertPrimaryKey: true);
        //    }
        //    else
        //    {
        //        _dbConnection.Update(dbPairRtoL);
        //    }

        //}

        loadKnownSubstitutions();
        loadKnownExternalInclusions();
    }

    public bool TryLoadCrossVersionSourceMaps(
        string sourcePath,
        bool useInternalTypeMaps)
    {
        if (string.IsNullOrEmpty(sourcePath) ||
            (!Directory.Exists(sourcePath)))
        {
            _logger.LogError($"Invalid map source path: {sourcePath}");
            return false;
        }

        // sanity check for db access
        if (_dbConnection is null)
        {
            _logger.LogError("Database connection is not initialized!");
            return false;
        }

        _loader ??= new(new()
        {
            AutoLoadExpansions = false,
            ResolvePackageDependencies = false,
        }, new()
        {
            JsonModel = LoaderOptions.JsonDeserializationModel.SystemTextJson,
        });

        // ensure our tables exist and are empty
        dropMapTables(_dbConnection);
        createMapTables(_dbConnection);

        // ensure we have these versions in the database
        List<DbFhirPackage> packages = DbFhirPackage.SelectList(
            _dbConnection,
            orderByProperties: [nameof(DbFhirPackage.PackageVersion)]);

        // complex types need transitive mappings built, primitives are direct only
        TransitiveMappingBuilder transitiveBuilder = new(_dbConnection, _loggerFactory, packages);

        loadSourceMaps(sourcePath, "codes", "ConceptMap-*-*.json");

        // TODO: determine when we should call this
        if (useInternalTypeMaps)
        {
            loadInternalTypeMaps(packages);

            // complex types need transitive mappings built, primitives are direct only when using internal
            transitiveBuilder.BuildTransitiveStructureMappings(FhirArtifactClassEnum.ComplexType);
        }
        else
        {
            loadSourceMaps(sourcePath, "types", "ConceptMap-types-*.json");

            transitiveBuilder.BuildTransitiveStructureMappings(FhirArtifactClassEnum.PrimitiveType);
            transitiveBuilder.BuildTransitiveStructureMappings(FhirArtifactClassEnum.ComplexType);
        }

        loadSourceMaps(sourcePath, "resources", "ConceptMap-resources-*.json");
        transitiveBuilder.BuildTransitiveStructureMappings(FhirArtifactClassEnum.Resource);

        // reconcile records that have mappings and explicit no-map records
        reconcileStructureNoMaps();

        loadSourceMaps(sourcePath, "elements", "ConceptMap-elements-*.json");

        loadSourceFml(sourcePath, packages);

        loadSourceMaps(sourcePath, "search-params", "ConceptMap-search-params-*.json");


        return true;
    }

    private void reconcileStructureNoMaps()
    {
        // get the list of all the structure mappings that have no target
        List<DbStructureMappingRecord> noMapRecords = DbStructureMappingRecord.SelectList(
            _dbConnection,
            TargetStructureKeyIsNull: true);

        // iterate over the no-maps to see if there is a derived mapping it needs to override
        foreach (DbStructureMappingRecord noMapRecord in noMapRecords)
        {
            List<DbStructureMappingRecord> otherMaps = DbStructureMappingRecord.SelectList(
                _dbConnection,
                SourceFhirPackageKey: noMapRecord.SourceFhirPackageKey,
                SourceStructureKey: noMapRecord.SourceStructureKey,
                TargetFhirPackageKey: noMapRecord.TargetFhirPackageKey,
                TargetStructureKeyIsNull: false);

            foreach (DbStructureMappingRecord otherMap in otherMaps)
            {
                // we are using the concept maps as the source of truth
                if ((noMapRecord.OriginatingConceptMapUrlsLiteral is null) &&
                    (otherMap.OriginatingConceptMapUrlsLiteral is not null))
                {
                    try
                    {
                        noMapRecord.Delete(_dbConnection);
                        DbElementMappingRecord.Delete(_dbConnection, StructureMappingKey: noMapRecord.Key);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Error deleting no-map structure mapping record Key={noMapRecord.Key}: {ex.Message}");
                    }

                    // once we delete a no-map, we can stop checking other maps for this record
                    break;
                }

                try
                {
                    // all other scenarios delete the other map
                    otherMap.Delete(_dbConnection);
                    DbElementMappingRecord.Delete(_dbConnection, StructureMappingKey: otherMap.Key);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error deleting structure mapping record Key={otherMap.Key}: {ex.Message}");
                }
            }
        }
    }

    private void loadSourceFml(
        string basePath,
        List<DbFhirPackage> packages)
    {
        string inputPath = Path.Combine(basePath, "input");
        if (!Directory.Exists(inputPath))
        {
            _logger.LogWarning($"Path not found: {inputPath}");
            return;
        }

        // iterate over the source packages
        for (int sourceIndex = 0; sourceIndex < packages.Count - 1; sourceIndex++)
        {
            DbFhirPackage sourcePackage = packages[sourceIndex];

            // iterate over the target packages
            for (int targetIndex = sourceIndex + 1; targetIndex < packages.Count; targetIndex++)
            {
                // skip same package
                if (sourceIndex == targetIndex)
                {
                    continue;
                }

                DbFhirPackage targetPackage = packages[targetIndex];

                string relativePath = $"{sourcePackage.ShortName}to{targetPackage.ShortName}";

                string path = Path.Combine(inputPath, relativePath);
                if (!Directory.Exists(path))
                {
                    continue;
                }

                loadSourceFml(sourcePackage, targetPackage, path);
            }
        }

    }

    private void loadSourceFml(
        DbFhirPackage sourcePackage,
        DbFhirPackage targetPackage,
        string path)
    {
        _logger.LogInformation($"Loading FML maps from: {path}");

        string sourceToTargetNoR = $"{sourcePackage.ShortName[1..]}to{targetPackage.ShortName[1..]}";
        int sourceToTargetNoRLen = sourceToTargetNoR.Length;

        FhirMappingLanguage fmlParser = new();

        // files have different naming conventions in each directory, so just process anything FML
        string[] files = Directory.GetFiles(path, $"*.fml", SearchOption.TopDirectoryOnly);
        foreach (string filename in files)
        {
            try
            {
                string fmlContent = File.ReadAllText(filename);

                if (!fmlParser.TryParse(fmlContent, out FhirStructureMap? fml))
                {
                    _logger.LogError($"Error loading {filename}: could not parse");
                    continue;
                }

                // extract the name root
                string name;

                if (fml.MetadataByPath.TryGetValue("name", out MetadataDeclaration? nameMeta))
                {
                    name = nameMeta.Literal?.ValueAsString ?? throw new Exception($"Cross-version structure maps require a metadata name property: {filename}");
                }
                else
                {
                    name = Path.GetFileNameWithoutExtension(filename);
                }

                if (name.EndsWith(sourceToTargetNoR, StringComparison.OrdinalIgnoreCase))
                {
                    name = name[..^sourceToTargetNoRLen];
                }

                if (name.Equals("primitives", StringComparison.OrdinalIgnoreCase))
                {
                    // skip primitive type map - we have that information internally already, see FhirTypeMappings.cs
                    continue;
                }

                // process this file
                FmlDbProcessor fmlDbProcessor = new(
                    _dbConnection,
                    sourcePackage,
                    targetPackage,
                    filename,
                    name,
                    fml);

                fmlDbProcessor.ProcessElementRelationships();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading {filename}: {ex.Message}");
            }
        }
    }


    private enum SourceMapTypeCodes : int
    {
        Types = 0,
        Codes,
        Resources,
        Elements,
        SearchParams,
    }
    
    private void loadSourceMaps(string basePath, string relativePath, string searchPattern)
    {
        string inputPath = Path.Combine(basePath, "input");
        if (!Directory.Exists(inputPath))
        {
            _logger.LogWarning($"Path not found: {inputPath}");
            return;
        }

        string path = Path.Combine(inputPath, relativePath);
        if (!Directory.Exists(path))
        {
            _logger.LogWarning($"Path not found: {path}");
            return;
        }

        SourceMapTypeCodes sourceMapType = relativePath switch
        {
            "types" => SourceMapTypeCodes.Types,
            "codes" => SourceMapTypeCodes.Codes,
            "resources" => SourceMapTypeCodes.Resources,
            "elements" => SourceMapTypeCodes.Elements,
            "search-params" => SourceMapTypeCodes.SearchParams,
            _ => throw new Exception($"Invalid source map relative path: {relativePath}")
        };

        int typeMapCount = 0;
        int valueSetMapCount = 0;
        int valueSetConceptMapCount = 0;
        int resourceMapCount = 0;
        int elementMapCount = 0;

        // get the specified map files
        string[] files = Directory.GetFiles(path, searchPattern, SearchOption.TopDirectoryOnly);
        foreach (string filename in files)
        {
            // extract the source and target version information
            (string sv, string tv) = getVersionsFromFilename(filename);

            if (!FhirReleases.TryGetSequence(sv, out FhirReleases.FhirSequenceCodes sourceVersion))
            {
                throw new Exception($"Invalid source version: {sv} from file: {filename}!");
            }

            if (!FhirReleases.TryGetSequence(tv, out FhirReleases.FhirSequenceCodes targetVersion))
            {
                throw new Exception($"Invalid target version: {tv} from file: {filename}!");
            }

            // ensure we have these versions in the database
            DbFhirPackage? sourcePackage = DbFhirPackage.SelectSingle(
                _dbConnection,
                PackageId: sourceVersion.ToCorePackageId(),
                PackageVersion: sourceVersion.ToLongVersion());
            if (sourcePackage is null)
            {
                _logger.LogWarning($"Skipping map with source version: {sourceVersion.ToRLiteral()} since it is not in the database!");
                continue;
            }

            DbFhirPackage? targetPackage = DbFhirPackage.SelectSingle(
                _dbConnection,
                PackageId: targetVersion.ToCorePackageId(),
                PackageVersion: targetVersion.ToLongVersion());
            if (targetPackage is null)
            {
                _logger.LogWarning($"Skipping map with target version: {targetVersion.ToRLiteral()} since it is not in the database!");
                continue;
            }

            object? loaded = _loader!.ParseContentsSystemTextStream("fhir+json", typeof(ConceptMap), path: filename);
            if (loaded is not ConceptMap cm)
            {
                _logger.LogError($"Error loading {filename}: could not parse as ConceptMap");
                continue;
            }

            // process the map
            switch (sourceMapType)
            {
                case SourceMapTypeCodes.Types:
                    {
                        int addedMapCount = loadSourceTypeMap(sourcePackage, targetPackage, cm, inputPath);
                        typeMapCount += addedMapCount;
                    }
                    break;

                case SourceMapTypeCodes.Codes:
                    {
                        (int addedVsMapCount, int addedConceptMapCount) = loadSourceCodeMap(sourcePackage, targetPackage, cm);
                        valueSetMapCount += addedVsMapCount;
                        valueSetConceptMapCount += addedConceptMapCount;
                    }
                    break;

                case SourceMapTypeCodes.Resources:
                    {
                        int addedMapCount = loadSourceResourceMap(sourcePackage, targetPackage, cm, inputPath);
                        resourceMapCount += addedMapCount;
                    }
                    break;

                case SourceMapTypeCodes.Elements:
                    {
                        int addedMapCount = loadSourceElementMap(sourcePackage, targetPackage, cm);
                        elementMapCount += addedMapCount;
                    }
                    break;

                case SourceMapTypeCodes.SearchParams:
                    //loadSourceTypeMap(sourcePackage, targetPackage, cm);
                    break;

                default:
                    throw new Exception($"Unhandled relative path resolution: {sourceMapType} ({relativePath})!");
            }
        }

        // log what we loaded
        switch (sourceMapType)
        {
            case SourceMapTypeCodes.Types:
                _logger.LogInformation($"Loaded {typeMapCount} Type Definition map records from: {path}");
                break;
            case SourceMapTypeCodes.Codes:
                _logger.LogInformation($"Loaded {valueSetMapCount} ValueSet and {valueSetConceptMapCount} ValueSet.Concept map records from: {path}");
                break;
            case SourceMapTypeCodes.Resources:
                _logger.LogInformation($"Loaded {resourceMapCount} Resource map records from: {path}");
                break;
            case SourceMapTypeCodes.Elements:
                _logger.LogInformation($"Loaded {elementMapCount} Element map records from: {path}");
                break;
            case SourceMapTypeCodes.SearchParams:
                break;
            default:
                break;
        }
    }

    private int loadSourceElementMap(
        DbFhirPackage sourcePackage,
        DbFhirPackage targetPackage,
        ConceptMap cm)
    {
        List<DbElementMappingRecord> elementMapsToAdd = [];

        // there *should* only be one group, but iterate just in case
        foreach (ConceptMap.GroupComponent group in cm.Group)
        {
            // ensure we are in a valid element grouping
            if (!group.Source.EndsWith("element-names", StringComparison.Ordinal))
            {
                throw new Exception($"Invalid source group: {group.Source} in map: {cm.Url} ({cm.Id})!");
            }

            if (!group.Target.EndsWith("element-names", StringComparison.Ordinal))
            {
                throw new Exception($"Invalid target group: {group.Target} in map: {cm.Url} ({cm.Id})!");
            }

            // iterate over the source elements of the map
            foreach (ConceptMap.SourceElementComponent groupSourceElement in group.Element)
            {
                // resolve the source element
                DbElement? sourceElement = DbElement.SelectSingle(
                    _dbConnection,
                    FhirPackageKey: sourcePackage.Key,
                    Id: groupSourceElement.Code);
                if (sourceElement is null)
                {
                    _logger.LogWarning($"Invalid source element: `{groupSourceElement.Code}` in map: {cm.Url} ({cm.Id})!");
                }

                // check for no map
                if (groupSourceElement.NoMap == true)
                {
                    // see if there are maps we can apply this to
                    List<DbStructureMappingRecord> relevantMaps;

                    if (sourceElement is null)
                    {
                        relevantMaps = DbStructureMappingRecord.SelectList(
                            _dbConnection,
                            SourceFhirPackageKey: sourcePackage.Key,
                            SourceStructureId: groupSourceElement.Code.Split('.').First(),
                            TargetFhirPackageKey: targetPackage.Key);
                    }
                    else
                    {
                        relevantMaps = DbStructureMappingRecord.SelectList(
                            _dbConnection,
                            SourceFhirPackageKey: sourcePackage.Key,
                            SourceStructureKey: sourceElement.StructureKey,
                            TargetFhirPackageKey: targetPackage.Key);
                    }

                    if (relevantMaps.Count == 0)
                    {
                        throw new Exception($"No relevant structure maps found for source element: {groupSourceElement.Code} in map: {cm.Url} ({cm.Id})!");
                    }

                    foreach (DbStructureMappingRecord relevantMap in relevantMaps)
                    {
                        DbElementMappingRecord mapRec = new()
                        {
                            Key = DbElementMappingRecord.GetIndex(),
                            StructureMappingKey = relevantMap.Key,
                            SourceFhirPackageKey = sourcePackage.Key,
                            SourceElementKey = sourceElement?.Key,
                            SourceElementId = groupSourceElement.Code,
                            TargetFhirPackageKey = targetPackage.Key,
                            TargetElementKey = null,
                            TargetElementId = null,
                            OriginatingConceptMapUrlsLiteral = cm.Url,

                            ElementKeys = getKeyArray(sourcePackage, targetPackage, sourceElement?.Key, null),

                            Relationship = null,
                            ConceptDomainRelationship = null,
                            ValueDomainRelationship = null,
                            ComputedRelationship = null,

                            ElementTypeChange = null,
                            TypesAddedLiteral = null,
                            TypesRemovedLiteral = sourceElement?.FullCollatedTypeLiteral,
                            TypesIdenticalLiteral = null,
                            TypesMappedLiteral = null,

                            ReferenceTargetChange = null,
                            ReferenceTargetsAddedLiteral = null,
                            ReferenceTargetsRemovedLiteral = sourceElement?.FullCollatedReferenceTypesLiteral,
                            ReferenceTargetsIdenticalLiteral = null,
                            ReferenceTargetsMappedLiteral = null,

                            BindingStrengthChange = null,
                            BindingBecameRequired = null,
                            BindingNoLongerRequired = null,
                            BindingTargetChange = null,
                            BoundValueSetMapKey = null,

                            MaxCardinalityChange = null,
                            BecameProhibited = true,
                            BecameMandatory = false,
                            BecameOptional = false,
                            BecameArray = false,
                            BecameScalar = false,
                        };

                        elementMapsToAdd.Add(mapRec);
                    }

                    continue;
                }

                // iterate over the map targets
                foreach (ConceptMap.TargetElementComponent elementTarget in groupSourceElement.Target)
                {
                    // resolve the target type
                    DbElement? targetElement = DbElement.SelectSingle(
                        _dbConnection,
                        FhirPackageKey: targetPackage.Key,
                        Id: elementTarget.Code);

                    // see if there are maps we can apply this to
                    DbStructureMappingRecord? relevantMap;

                    if (targetElement is null)
                    {
                        _logger.LogWarning($"Invalid target element: `{elementTarget.Code}` for source: {groupSourceElement.Code} in map: {cm.Url} ({cm.Id})!");

                        if (sourceElement is null)
                        {
                            relevantMap = DbStructureMappingRecord.SelectSingle(
                                _dbConnection,
                                SourceFhirPackageKey: sourcePackage.Key,
                                SourceStructureId: groupSourceElement.Code.Split('.').First(),
                                TargetFhirPackageKey: targetPackage.Key,
                                TargetStructureId: elementTarget.Code.Split('.').First());
                        }
                        else
                        {
                            relevantMap = DbStructureMappingRecord.SelectSingle(
                                _dbConnection,
                                SourceFhirPackageKey: sourcePackage.Key,
                                SourceStructureKey: sourceElement.StructureKey,
                                TargetFhirPackageKey: targetPackage.Key,
                                TargetStructureId: elementTarget.Code.Split('.').First());
                        }
                    }
                    else
                    {
                        if (sourceElement is null)
                        {
                            relevantMap = DbStructureMappingRecord.SelectSingle(
                                _dbConnection,
                                SourceFhirPackageKey: sourcePackage.Key,
                                SourceStructureId: groupSourceElement.Code.Split('.').First(),
                                TargetFhirPackageKey: targetPackage.Key,
                                TargetStructureKey: targetElement.StructureKey);
                        }
                        else
                        {
                            relevantMap = DbStructureMappingRecord.SelectSingle(
                                _dbConnection,
                                SourceFhirPackageKey: sourcePackage.Key,
                                SourceStructureKey: sourceElement.StructureKey,
                                TargetFhirPackageKey: targetPackage.Key,
                                TargetStructureKey: targetElement.StructureKey);
                        }
                    }

                    if (relevantMap is null)
                    {
                        throw new Exception($"No relevant structure map found for source element: {groupSourceElement.Code} to target: {elementTarget.Code} in map: {cm.Url} ({cm.Id})!");
                    }

                    // create a record for the database
                    DbElementMappingRecord mapRec = new()
                    {
                        Key = DbElementMappingRecord.GetIndex(),
                        StructureMappingKey = relevantMap.Key,
                        SourceFhirPackageKey = sourcePackage.Key,
                        SourceElementKey = sourceElement?.Key,
                        SourceElementId = groupSourceElement.Code,
                        TargetFhirPackageKey = targetPackage.Key,
                        TargetElementKey = targetElement?.Key,
                        TargetElementId = elementTarget.Code,
                        OriginatingConceptMapUrlsLiteral = cm.Url,

                        ElementKeys = getKeyArray(sourcePackage, targetPackage, sourceElement?.Key, targetElement?.Key),

                        Relationship = elementTarget.Relationship,
                        ConceptDomainRelationship = null,
                        ValueDomainRelationship = null,
                        ComputedRelationship = null,

                        ElementTypeChange = null,
                        TypesAddedLiteral = null,
                        TypesRemovedLiteral = null,
                        TypesIdenticalLiteral = null,
                        TypesMappedLiteral = null,

                        ReferenceTargetChange = null,
                        ReferenceTargetsAddedLiteral = null,
                        ReferenceTargetsRemovedLiteral = null,
                        ReferenceTargetsIdenticalLiteral = null,
                        ReferenceTargetsMappedLiteral = null,

                        BindingStrengthChange = null,
                        BindingBecameRequired = null,
                        BindingNoLongerRequired = null,
                        BindingTargetChange = null,
                        BoundValueSetMapKey = null,

                        MaxCardinalityChange = null,
                        BecameProhibited = null,
                        BecameMandatory = null,
                        BecameOptional = null,
                        BecameArray = null,
                        BecameScalar = null,
                    };

                    elementMapsToAdd.Add(mapRec);
                }
            }
        }

        // insert into the database
        elementMapsToAdd.Insert(_dbConnection, ignoreDuplicates: true, insertPrimaryKey: true);
        //_logger.LogInformation($"Inserted {elementMapsToAdd.Count} Type Definition Map records");

        return elementMapsToAdd.Count;
    }

    private int loadSourceResourceMap(
        DbFhirPackage sourcePackage,
        DbFhirPackage targetPackage,
        ConceptMap cm,
        string sourceInputPath)
    {
        List<DbStructureMappingRecord> resourceMapsToAdd = [];

        // there *should* only be one group, but iterate just in case
        foreach (ConceptMap.GroupComponent group in cm.Group)
        {
            // ensure we are in a valid resource type grouping
            if (!group.Source.EndsWith("resource-types", StringComparison.Ordinal))
            {
                throw new Exception($"Invalid source group: {group.Source} in map: {cm.Url} ({cm.Id})!");
            }

            if (!group.Target.EndsWith("resource-types", StringComparison.Ordinal))
            {
                throw new Exception($"Invalid target group: {group.Target} in map: {cm.Url} ({cm.Id})!");
            }

            // iterate over the source elements of the map
            foreach (ConceptMap.SourceElementComponent groupSourceElement in group.Element)
            {
                // resolve the source type
                DbStructureDefinition? sourceSd = DbStructureDefinition.SelectSingle(
                    _dbConnection,
                    FhirPackageKey: sourcePackage.Key,
                    Id: groupSourceElement.Code);
                if (sourceSd is null)
                {
                    throw new Exception($"Invalid source resource: `{groupSourceElement.Code}` in map: {cm.Url} ({cm.Id})!");
                }

                // check for no map
                if (groupSourceElement.NoMap == true)
                {
                    (string idLong, string idShort) = XVerProcessor.GenerateArtifactId(sourcePackage.ShortName, sourceSd.Id, targetPackage.ShortName);

                    DbStructureMappingRecord mapRec = new()
                    {
                        Key = DbStructureMappingRecord.GetIndex(),
                        SourceFhirPackageKey = sourcePackage.Key,
                        SourceStructureKey = sourceSd.Key,
                        SourceStructureId = sourceSd.Id,

                        TargetFhirPackageKey = targetPackage.Key,
                        TargetStructureKey = null,
                        TargetStructureId = null,

                        StructureKeys = getKeyArray(sourcePackage, targetPackage, sourceSd.Key, null),

                        FmlExists = null,
                        FmlUrl = null,
                        FmlFilename = null,

                        Relationship = null,

                        ConceptDomainRelationship = null,
                        ValueDomainRelationship = null,
                        ComputedRelationship = null,

                        OriginatingConceptMapUrlsLiteral = cm.Url,
                        IdLong = idLong,
                        IdShort = idShort,
                        Url = $"http://hl7.org/fhir/{sourcePackage.FhirVersionShort}/ConceptMap/{idLong}",
                        Name = FhirSanitizationUtils.ReformatIdForName(idLong),
                        Title = $"Concept Map of FHIR {sourcePackage.ShortName} resource {sourceSd.Name} to FHIR {targetPackage.ShortName}"
                    };

                    resourceMapsToAdd.Add(mapRec);
                    continue;
                }

                // iterate over the map targets
                foreach (ConceptMap.TargetElementComponent elementTarget in groupSourceElement.Target)
                {
                    // resolve the target type
                    DbStructureDefinition? targetSd = DbStructureDefinition.SelectSingle(
                        _dbConnection,
                        FhirPackageKey: targetPackage.Key,
                        Id: elementTarget.Code);

                    if (targetSd is null)
                    {
                        throw new Exception($"Invalid target resource: `{elementTarget.Code}` for source: {groupSourceElement.Code} in map: {cm.Url} ({cm.Id})!");
                    }

                    (string idLong, string idShort) = XVerProcessor.GenerateArtifactId(sourcePackage.ShortName, sourceSd.Id, targetPackage.ShortName, targetSd.Id);

                    // create a record for the database
                    DbStructureMappingRecord mapRec = new()
                    {
                        Key = DbStructureMappingRecord.GetIndex(),
                        SourceFhirPackageKey = sourcePackage.Key,
                        SourceStructureKey = sourceSd.Key,
                        SourceStructureId = sourceSd.Id,

                        TargetFhirPackageKey = targetPackage.Key,
                        TargetStructureKey = targetSd.Key,
                        TargetStructureId = targetSd.Id,

                        StructureKeys = getKeyArray(sourcePackage, targetPackage, sourceSd.Key, targetSd.Key),

                        FmlExists = null,
                        FmlFilename = null,
                        FmlUrl = null,

                        Relationship = elementTarget.Relationship,
                        Comments = elementTarget.Comment,

                        ConceptDomainRelationship = null,
                        ValueDomainRelationship = null,
                        ComputedRelationship = null,

                        OriginatingConceptMapUrlsLiteral = cm.Url,

                        IdLong = idLong,
                        IdShort = idShort,
                        Url = $"http://hl7.org/fhir/{sourcePackage.FhirVersionShort}/ConceptMap/{idLong}",
                        Name = FhirSanitizationUtils.ReformatIdForName(idLong),
                        Title = $"Concept Map of FHIR {sourcePackage.ShortName} type {sourceSd.Name} to FHIR {targetPackage.ShortName} type {targetSd.Name}"
                    };

                    resourceMapsToAdd.Add(mapRec);
                }
            }
        }

        // insert into the database
        resourceMapsToAdd.Insert(_dbConnection, ignoreDuplicates: true, insertPrimaryKey: true);
        //_logger.LogInformation($"Inserted {elementMapsToAdd.Count} Type Definition Map records");

        return resourceMapsToAdd.Count;
    }

    private (int insertedVsMapCount, int insertedConceptMapCount) loadSourceCodeMap(
        DbFhirPackage sourcePackage,
        DbFhirPackage targetPackage,
        ConceptMap cm)
    {
        List<DbValueSetMapRecord> valueSetMapsToAdd = [];
        List<DbValueSetConceptMapRecord> conceptMapsToAdd = [];

        // there *should* only be one group, but iterate just in case
        foreach (ConceptMap.GroupComponent group in cm.Group)
        {
            // try to resolve the source value set
            string sourceVsId = group.Source.Substring(group.Source.LastIndexOf('/') + 1);
            DbValueSet? sourceVs = DbValueSet.SelectSingle(
                _dbConnection,
                FhirPackageKey: sourcePackage.Key,
                Id: sourceVsId);

            if (sourceVs is null)
            {
                // try to resolve the element instead of the value set id
                string elementId = cm.SourceScope is FhirUri sourceUri
                    ? sourceUri.Value.Substring(sourceUri.Value.LastIndexOf('#') + 1)
                    : cm.SourceScope is Canonical sourceCanonical
                    ? sourceCanonical.Value.Substring(sourceCanonical.Value.LastIndexOf('#') + 1)
                    : throw new Exception($"Failed to resolve Element ID for source `{cm.SourceScope}` in map: {cm.Url} ({cm.Id})");

                DbElement? sourceScopeElement = DbElement.SelectSingle(
                    _dbConnection,
                    FhirPackageKey: sourcePackage.Key,
                    Id: elementId); 
                if ((sourceScopeElement is not null) &&
                    (sourceScopeElement.BindingValueSetKey is not null))
                {
                    sourceVs = DbValueSet.SelectSingle(
                        _dbConnection,
                        Key: sourceScopeElement.BindingValueSetKey);

                    if (sourceVs is null)
                    {
                        throw new Exception($"Failed to resolve bound value set key: {sourceScopeElement.Key}" +
                            $" for element {sourceScopeElement.Id} ({sourceScopeElement.Key}) while resolving" +
                            $" source scope: `{cm.SourceScope.ToString()}` in map: {cm.Url} ({cm.Id})");
                    }
                }

                if (sourceVs is null)
                {
                    throw new Exception($"Invalid source Value Set: {sourceVsId} ({group.Source}) from map: {cm.Url} ({cm.Id})");
                }
            }

            if (sourceVs.CanExpand != true)
            {
                // skip this
                continue;
            }

            string targetVsId = group.Target.Substring(group.Target.LastIndexOf('/') + 1);
            DbValueSet? targetVs = DbValueSet.SelectSingle(
                _dbConnection,
                FhirPackageKey: targetPackage.Key,
                Id: targetVsId);
            if (targetVs is null)
            {
                // try to resolve the element instead of the value set id
                string elementId = cm.TargetScope is FhirUri targetUri
                    ? targetUri.Value.Substring(targetUri.Value.LastIndexOf('#') + 1)
                    : cm.TargetScope is Canonical targetCanonical
                    ? targetCanonical.Value.Substring(targetCanonical.Value.LastIndexOf('#') + 1)
                    : throw new Exception($"Failed to resolve Element ID for target `{cm.TargetScope}` in map: {cm.Url} ({cm.Id})");

                DbElement? targetScopeElement = DbElement.SelectSingle(
                    _dbConnection,
                    FhirPackageKey: targetPackage.Key,
                    Id: elementId);
                if ((targetScopeElement is not null) &&
                    (targetScopeElement.BindingValueSetKey is not null))
                {
                    targetVs = DbValueSet.SelectSingle(
                        _dbConnection,
                        Key: targetScopeElement.BindingValueSetKey);

                    if (targetVs is null)
                    {
                        throw new Exception($"Failed to resolve bound value set key: {targetScopeElement.Key}" +
                            $" for element {targetScopeElement.Id} ({targetScopeElement.Key}) while resolving" +
                            $" target scope: `{cm.TargetScope.ToString()}` in map: {cm.Url} ({cm.Id})");
                    }
                }

                if (targetVs is null)
                {
                    throw new Exception($"Invalid target Value Set: {targetVsId} ({group.Target}) from map: {cm.Url} ({cm.Id})");
                }
            }

            if (targetVs.CanExpand != true)
            {
                // skip this
                continue;
            }

            // build the ID for the value set map
            (string vsIdLong, string vsIdShort) = XVerProcessor.GenerateArtifactId(sourcePackage.ShortName, sourceVs.Id, targetPackage.ShortName, targetVs.Id);

            // get from the db or create a new map
            DbValueSetMapRecord? vsMap = DbValueSetMapRecord.SelectSingle(
                _dbConnection,
                IdLong: vsIdLong);
            if (vsMap is null)
            {
                vsMap = new()
                {
                    Key = DbValueSetMapRecord.GetIndex(),
                    SourceFhirPackageKey = sourcePackage.Key,
                    SourceValueSetKey = sourceVs.Key,

                    TargetFhirPackageKey = targetPackage.Key,
                    TargetValueSetKey = targetVs.Key,

                    Relationship = null,

                    OriginatingConceptMapUrlsLiteral = cm.Url,

                    IdLong = vsIdLong,
                    IdShort = vsIdShort,
                    Url = $"http://hl7.org/fhir/{sourcePackage.FhirVersionShort}/ConceptMap/{vsIdLong}",
                    Name = FhirSanitizationUtils.ReformatIdForName(vsIdLong),
                    Title = $"Concept Map of FHIR {sourcePackage.ShortName} Value Set `{sourceVs.VersionedUrl}` to FHIR {targetPackage.ShortName} Value Set `{targetVs.VersionedUrl}`"
                };

                valueSetMapsToAdd.Add(vsMap);
            }
            else
            {
                // add this source map as a source
                vsMap.OriginatingConceptMapUrls = [..vsMap.OriginatingConceptMapUrls!, cm.Url];

                // just update now
                vsMap.Update(_dbConnection);
            }

            // iterate over the source elements of the map
            foreach (ConceptMap.SourceElementComponent groupSourceElement in group.Element)
            {
                // resolve the source concept
                DbValueSetConcept? sourceConcept = DbValueSetConcept.SelectSingle(
                    _dbConnection,
                    ValueSetKey: sourceVs.Key,
                    Code: groupSourceElement.Code);
                if (sourceConcept is null)
                {
                    throw new Exception($"Invalid source concept literal `{groupSourceElement.Code}` for Value Set: `{sourceVs.VersionedUrl}` from map: {cm.Url} ({cm.Id})");
                }

                // check for no map
                if (groupSourceElement.NoMap == true)
                {
                    // check to see if we already have this in the database
                    DbValueSetConceptMapRecord? conceptMapRec = DbValueSetConceptMapRecord.SelectSingle(
                        _dbConnection,
                        ValueSetMapKey: vsMap.Key,
                        SourceValueSetConceptKey: sourceConcept.Key,
                        TargetValueSetConceptKeyIsNull: true);
                    if (conceptMapRec is not null)
                    {
                        continue;
                    }

                    conceptMapRec = new()
                    {
                        Key = DbValueSetConceptMapRecord.GetIndex(),
                        ValueSetMapKey = vsMap.Key,

                        SourceFhirPackageKey = sourcePackage.Key,
                        SourceValueSetConceptKey = sourceConcept.Key,
                        TargetFhirPackageKey = targetPackage.Key,
                        TargetValueSetConceptKey = null,

                        Relationship = null,
                        CodesAreIdentical = false,
                    };

                    conceptMapsToAdd.Add(conceptMapRec);
                    continue;
                }

                // iterate over the map targets
                foreach (ConceptMap.TargetElementComponent elementTarget in groupSourceElement.Target)
                {
                    // resolve the target concept
                    DbValueSetConcept? targetConcept = DbValueSetConcept.SelectSingle(
                        _dbConnection,
                        ValueSetKey: targetVs.Key,
                        Code: elementTarget.Code);

                    if (targetConcept is null)
                    {
                        throw new Exception($"Invalid target concept literal `{elementTarget.Code}`" +
                            $" for Value Set: `{sourceVs.VersionedUrl}` source: `{groupSourceElement.Code}`" +
                            $" from map: {cm.Url} ({cm.Id})");
                    }

                    // check to see if we already have this in the database
                    DbValueSetConceptMapRecord? conceptMapRec = DbValueSetConceptMapRecord.SelectSingle(
                        _dbConnection,
                        ValueSetMapKey: vsMap.Key,
                        SourceValueSetConceptKey: sourceConcept.Key,
                        TargetValueSetConceptKey: targetConcept.Key);

                    if (conceptMapRec is not null)
                    {
                        continue;
                    }

                    // create a record for the database
                    conceptMapRec = new()
                    {
                        Key = DbValueSetConceptMapRecord.GetIndex(),
                        ValueSetMapKey = vsMap.Key,

                        SourceFhirPackageKey = sourcePackage.Key,
                        SourceValueSetConceptKey = sourceConcept.Key,
                        TargetFhirPackageKey = targetPackage.Key,
                        TargetValueSetConceptKey = targetConcept.Key,

                        Relationship = elementTarget.Relationship,
                        Comments = elementTarget.Comment,
                        CodesAreIdentical = groupSourceElement.Code == elementTarget.Code,
                    };

                    conceptMapsToAdd.Add(conceptMapRec);
                }
            }
        }

        // insert into the database
        valueSetMapsToAdd.Insert(_dbConnection, ignoreDuplicates: true, insertPrimaryKey: true);
        //_logger.LogInformation($"Inserted {valueSetMapsToAdd.Count} Value Set Map records");

        conceptMapsToAdd.Insert(_dbConnection, ignoreDuplicates: true, insertPrimaryKey: true);
        //_logger.LogInformation($"Inserted {conceptMapsToAdd.Count} Value Set Concept Map records");

        return (valueSetMapsToAdd.Count, conceptMapsToAdd.Count);
    }

    private void loadInternalTypeMaps(List<DbFhirPackage> packages)
    {
        HashSet<(FhirReleases.FhirSequenceCodes, FhirReleases.FhirSequenceCodes)> processedPairs = [];

        // iterate across package pairs
        for (int sourceIndex = 0; sourceIndex < packages.Count; sourceIndex++)
        {
            DbFhirPackage sourcePackage = packages[sourceIndex];

            for (int targetIndex = 0; targetIndex < sourceIndex; targetIndex++)
            {
                if (targetIndex == sourceIndex)
                {
                    continue;
                }

                DbFhirPackage targetPackage = packages[targetIndex];

                if (processedPairs.Add((sourcePackage.DefinitionFhirSequence, targetPackage.DefinitionFhirSequence)))
                {
                    loadInternalTypeMap(
                        packages,
                        sourcePackage,
                        targetPackage);
                }

                if (processedPairs.Add((targetPackage.DefinitionFhirSequence, sourcePackage.DefinitionFhirSequence)))
                {
                    loadInternalTypeMap(
                        packages,
                        targetPackage,
                        sourcePackage);
                }
            }
        }

    }

    private int?[] getKeyArray(
        DbFhirPackage sourcePackage,
        DbFhirPackage targetPackage,
        int? sourceKey,
        int? targetKey)
    {
        int?[] keyArray = [ null, null, null, null, null, null ];
        keyArray[sourcePackage.PackageArrayIndex] = sourceKey;
        keyArray[targetPackage.PackageArrayIndex] = targetKey;
        return keyArray;
    }

    private List<DbStructureMappingRecord> buildInternalPrimitiveTypeMapRecs(
        DbFhirPackage sourcePackage,
        DbFhirPackage targetPackage)
    {
        List<DbStructureMappingRecord> toAdd = [];

        // get the source and target primitive types
        List<DbStructureDefinition> sourceTypes = DbStructureDefinition.SelectList(
            _dbConnection,
            FhirPackageKey: sourcePackage.Key,
            ArtifactClass: FhirArtifactClassEnum.PrimitiveType);

        Dictionary<string, DbStructureDefinition> targetTypes = DbStructureDefinition.SelectList(
            _dbConnection,
            FhirPackageKey: targetPackage.Key,
            ArtifactClass: FhirArtifactClassEnum.PrimitiveType)
            .ToDictionary(sd => sd.Id, sd => sd);

        ILookup<string, CodeGenTypeMapping> typeMappingLookup = FhirTypeMappings.PrimitiveMappings.ToLookup(
            tm => tm.SourceType,
            tm => tm);

        // iterate over the source types
        foreach (DbStructureDefinition sourceSd in sourceTypes)
        {
            bool mapsAdded = false;

            // get the mappings for this type
            IEnumerable<FhirTypeMappings.CodeGenTypeMapping> mappingsForSource = typeMappingLookup[sourceSd.Id];
            foreach (FhirTypeMappings.CodeGenTypeMapping tm in mappingsForSource)
            {
                // ensure the target type exists
                if (!targetTypes.TryGetValue(tm.TargetType, out DbStructureDefinition? targetSd))
                {
                    continue;
                }

                mapsAdded = true;

                (string idLong, string idShort) = XVerProcessor.GenerateArtifactId(sourcePackage.ShortName, sourceSd.Id, targetPackage.ShortName, tm.TargetType);

                // create the structure mapping record
                DbStructureMappingRecord mappingRec = new()
                {
                    Key = DbStructureMappingRecord.GetIndex(),
                    SourceFhirPackageKey = sourcePackage.Key,
                    SourceStructureKey = sourceSd.Key,
                    SourceStructureId = sourceSd.Id,
                    TargetFhirPackageKey = targetPackage.Key,
                    TargetStructureKey = targetSd.Key,
                    TargetStructureId = tm.TargetType,

                    StructureKeys = getKeyArray(sourcePackage, targetPackage, sourceSd.Key, targetSd.Key),

                    FmlExists = null,
                    FmlUrl = null,
                    FmlFilename = null,

                    Relationship = tm.Relationship,
                    ConceptDomainRelationship = tm.ConceptDomainRelationship,
                    ValueDomainRelationship = tm.ValueDomainRelationship,
                    ComputedRelationship = RelationshipComposition.ComputeForDomains(tm.ConceptDomainRelationship, tm.ValueDomainRelationship),

                    OriginatingConceptMapUrlsLiteral = null,
                    IdLong = idLong,
                    IdShort = idShort,
                    Url = $"http://hl7.org/fhir/{sourcePackage.FhirVersionShort}/ConceptMap/{idLong}",
                    Name = FhirSanitizationUtils.ReformatIdForName(idLong),
                    Title = $"Cross Version Concept Map for FHIR {sourcePackage.ShortName} {sourceSd.Id} to FHIR {targetPackage.ShortName} {targetSd.Id}",
                };

                toAdd.Add(mappingRec);
            }

            // if there are no maps for this source type, add a no-map entry
            if (!mapsAdded)
            {
                (string idLong, string idShort) = XVerProcessor.GenerateArtifactId(sourcePackage.ShortName, sourceSd.Id, targetPackage.ShortName);
                // create the structure mapping record
                DbStructureMappingRecord mappingRec = new()
                {
                    Key = DbStructureMappingRecord.GetIndex(),
                    SourceFhirPackageKey = sourcePackage.Key,
                    SourceStructureKey = sourceSd.Key,
                    SourceStructureId = sourceSd.Id,
                    TargetFhirPackageKey = targetPackage.Key,
                    TargetStructureKey = null,
                    TargetStructureId = null,
                    StructureKeys = getKeyArray(sourcePackage, targetPackage, sourceSd.Key, null),
                    FmlExists = null,
                    FmlUrl = null,
                    FmlFilename = null,
                    Relationship = null,
                    ConceptDomainRelationship = null,
                    ValueDomainRelationship = null,
                    ComputedRelationship = null,
                    OriginatingConceptMapUrlsLiteral = null,
                    IdLong = idLong,
                    IdShort = idShort,
                    Url = $"http://hl7.org/fhir/{sourcePackage.FhirVersionShort}/ConceptMap/{idLong}",
                    Name = FhirSanitizationUtils.ReformatIdForName(idLong),
                    Title = $"Cross Version Concept Map for FHIR {sourcePackage.ShortName} {sourceSd.Id} to FHIR {targetPackage.ShortName} - No Map",
                };
                toAdd.Add(mappingRec);
            }
        }

        return toAdd;
    }

    private List<DbStructureMappingRecord> buildInternalComplexTypeMapRecs(
        DbFhirPackage sourcePackage,
        DbFhirPackage targetPackage)
    {
        // get the complex type mappings for this pair
        ILookup<string, CodeGenTypeMapping> typeMappingLookup = FhirTypeMappings.GetComplexTypeMaps(
            sourcePackage.DefinitionFhirSequence,
            targetPackage.DefinitionFhirSequence)
            .ToLookup(tm => tm.SourceType, tm => tm);

        if (typeMappingLookup.Count == 0)
        {
            // nothing to do here
            return [];
        }


        List<DbStructureDefinition> sourceTypes = DbStructureDefinition.SelectList(
            _dbConnection,
            FhirPackageKey: sourcePackage.Key,
            ArtifactClass: FhirArtifactClassEnum.ComplexType);

        Dictionary<string, DbStructureDefinition> targetTypes = DbStructureDefinition.SelectList(
            _dbConnection,
            FhirPackageKey: targetPackage.Key,
            ArtifactClass: FhirArtifactClassEnum.ComplexType)
            .ToDictionary(sd => sd.Id, sd => sd);

        List<DbStructureMappingRecord> toAdd = [];

        // iterate over the source types
        foreach (DbStructureDefinition sourceSd in sourceTypes)
        {
            bool mapsAdded = false;

            // get the mappings for this type
            IEnumerable<FhirTypeMappings.CodeGenTypeMapping> mappingsForSource = typeMappingLookup[sourceSd.Id];
            foreach (FhirTypeMappings.CodeGenTypeMapping tm in mappingsForSource)
            {
                // ensure the target type exists
                if (!targetTypes.TryGetValue(tm.TargetType, out DbStructureDefinition? targetSd))
                {
                    continue;
                }

                mapsAdded = true;

                (string idLong, string idShort) = XVerProcessor.GenerateArtifactId(sourcePackage.ShortName, sourceSd.Id, targetPackage.ShortName, tm.TargetType);

                // create the structure mapping record
                DbStructureMappingRecord mappingRec = new()
                {
                    Key = DbStructureMappingRecord.GetIndex(),
                    SourceFhirPackageKey = sourcePackage.Key,
                    SourceStructureKey = sourceSd.Key,
                    SourceStructureId = sourceSd.Id,
                    TargetFhirPackageKey = targetPackage.Key,
                    TargetStructureKey = targetSd.Key,
                    TargetStructureId = tm.TargetType,

                    StructureKeys = getKeyArray(sourcePackage, targetPackage, sourceSd.Key, targetSd.Key),

                    FmlExists = null,
                    FmlUrl = null,
                    FmlFilename = null,

                    Relationship = tm.Relationship,
                    ConceptDomainRelationship = tm.ConceptDomainRelationship,
                    ValueDomainRelationship = tm.ValueDomainRelationship,
                    ComputedRelationship = RelationshipComposition.ComputeForDomains(tm.ConceptDomainRelationship, tm.ValueDomainRelationship),

                    OriginatingConceptMapUrlsLiteral = null,
                    IdLong = idLong,
                    IdShort = idShort,
                    Url = $"http://hl7.org/fhir/{sourcePackage.FhirVersionShort}/ConceptMap/{idLong}",
                    Name = FhirSanitizationUtils.ReformatIdForName(idLong),
                    Title = $"Cross Version Concept Map for FHIR {sourcePackage.ShortName} {sourceSd.Id} to FHIR {targetPackage.ShortName} {targetSd.Id}",
                };
                toAdd.Add(mappingRec);
            }

            // if there are no maps for this source type, add a no-map entry
            if (!mapsAdded)
            {
                (string idLong, string idShort) = XVerProcessor.GenerateArtifactId(sourcePackage.ShortName, sourceSd.Id, targetPackage.ShortName);

                // create the structure mapping record
                DbStructureMappingRecord mappingRec = new()
                {
                    Key = DbStructureMappingRecord.GetIndex(),
                    SourceFhirPackageKey = sourcePackage.Key,
                    SourceStructureKey = sourceSd.Key,
                    SourceStructureId = sourceSd.Id,
                    TargetFhirPackageKey = targetPackage.Key,
                    TargetStructureKey = null,
                    TargetStructureId = null,

                    StructureKeys = getKeyArray(sourcePackage, targetPackage, sourceSd.Key, null),

                    FmlExists = null,
                    FmlUrl = null,
                    FmlFilename = null,
                    Relationship = null,

                    ConceptDomainRelationship = null,
                    ValueDomainRelationship = null,
                    ComputedRelationship = null,
                    OriginatingConceptMapUrlsLiteral = null,

                    IdLong = idLong,
                    IdShort = idShort,
                    Url = $"http://hl7.org/fhir/{sourcePackage.FhirVersionShort}/ConceptMap/{idLong}",
                    Name = FhirSanitizationUtils.ReformatIdForName(idLong),
                    Title = $"Cross Version Concept Map for FHIR {sourcePackage.ShortName} {sourceSd.Id} to FHIR {targetPackage.ShortName} - No Map",
                };
                toAdd.Add(mappingRec);
            }
        }

        return toAdd;
    }

    private void loadInternalTypeMap(
        List<DbFhirPackage> packages,
        DbFhirPackage sourcePackage,
        DbFhirPackage targetPackage)
    {
        List<DbStructureMappingRecord> toAdd = [];

        // add the primitive type mappings first
        toAdd.AddRange(
            buildInternalPrimitiveTypeMapRecs(
                sourcePackage,
                targetPackage));

        // add the complex type mappings second
        toAdd.AddRange(
            buildInternalComplexTypeMapRecs(
                sourcePackage,
                targetPackage));

        // insert our mapping records
        if (toAdd.Count > 0)
        {
            toAdd.Insert(_dbConnection, ignoreDuplicates: true, insertPrimaryKey: true);
        }
    }

    private int loadSourceTypeMap(
        DbFhirPackage sourcePackage,
        DbFhirPackage targetPackage,
        ConceptMap cm,
        string sourceInputPath)
    {
        List<DbStructureMappingRecord> typeDefinitionMapsToAdd = [];

        // there *should* only be one group, but iterate just in case
        foreach (ConceptMap.GroupComponent group in cm.Group)
        {
            // ensure we are in a valid data type grouping
            if (!group.Source.EndsWith("data-types", StringComparison.Ordinal))
            {
                throw new Exception($"Invalid source group: {group.Source} in map: {cm.Url} ({cm.Id})!");
            }

            if (!group.Target.EndsWith("data-types", StringComparison.Ordinal))
            {
                throw new Exception($"Invalid target group: {group.Target} in map: {cm.Url} ({cm.Id})!");
            }

            // iterate over the source elements of the map
            foreach (ConceptMap.SourceElementComponent groupSourceElement in group.Element)
            {
                // resolve the source type
                DbStructureDefinition? sourceSd = DbStructureDefinition.SelectSingle(
                    _dbConnection,
                    FhirPackageKey: sourcePackage.Key,
                    Id: groupSourceElement.Code);
                if (sourceSd is null)
                {
                    throw new Exception($"Invalid source type: `{groupSourceElement.Code}` in map: {cm.Url} ({cm.Id})!");
                }

                // check for no map
                if (groupSourceElement.NoMap == true)
                {
                    (string idLong, string idShort) = XVerProcessor.GenerateArtifactId(sourcePackage.ShortName, sourceSd.Id, targetPackage.ShortName);

                    DbStructureMappingRecord mapRec = new()
                    {
                        Key = DbStructureMappingRecord.GetIndex(),
                        SourceFhirPackageKey = sourcePackage.Key,
                        SourceStructureKey = sourceSd.Key,
                        SourceStructureId = sourceSd.Id,

                        TargetFhirPackageKey = targetPackage.Key,
                        TargetStructureKey = null,
                        TargetStructureId = null,

                        StructureKeys = getKeyArray(sourcePackage, targetPackage, sourceSd.Key, null),

                        FmlExists = null,
                        FmlFilename = null,
                        FmlUrl = null,

                        Relationship = null,

                        ConceptDomainRelationship = null,
                        ValueDomainRelationship = null,
                        ComputedRelationship = null,

                        OriginatingConceptMapUrlsLiteral = cm.Url,
                        IdLong = idLong,
                        IdShort = idShort,
                        Url = $"http://hl7.org/fhir/{sourcePackage.FhirVersionShort}/ConceptMap/{idLong}",
                        Name = FhirSanitizationUtils.ReformatIdForName(idLong),
                        Title = $"Concept Map of FHIR {sourcePackage.ShortName} type {sourceSd.Name} to FHIR {targetPackage.ShortName}"
                    };

                    typeDefinitionMapsToAdd.Add(mapRec);
                    continue;
                }

                // iterate over the map targets
                foreach (ConceptMap.TargetElementComponent elementTarget in groupSourceElement.Target)
                {
                    // resolve the target type
                    DbStructureDefinition? targetSd = DbStructureDefinition.SelectSingle(
                        _dbConnection,
                        FhirPackageKey: targetPackage.Key,
                        Id: elementTarget.Code);

                    if (targetSd is null)
                    {
                        throw new Exception($"Invalid target type: `{elementTarget.Code}` for source: {groupSourceElement.Code} in map: {cm.Url} ({cm.Id})!");
                    }

                    (string idLong, string idShort) = XVerProcessor.GenerateArtifactId(sourcePackage.ShortName, sourceSd.Id, targetPackage.ShortName, targetSd.Id);

                    string fmlFolder = sourcePackage.ShortName + "to" + targetPackage.ShortName;
                    string fmlFile = sourceSd.Name + ".fml";
                    bool fmlExists = File.Exists(Path.Combine(sourceInputPath, fmlFolder, fmlFile));

                    // get cdRelationship properties if they exist
                    CMR? cdRelationship = elementTarget.Property
                        .FirstOrDefault(p => p.Code == ConceptMapProperties.PropertyCodeConceptDomainRelationship)?.Value is Code cdCodeValue
                        ? EnumUtility.ParseLiteral<CMR>(cdCodeValue.Value)
                        : null;

                    CMR? vdRelationship = elementTarget.Property
                        .FirstOrDefault(p => p.Code == ConceptMapProperties.PropertyCodeValueDomainRelationship)?.Value is Code vdCodeValue
                        ? EnumUtility.ParseLiteral<CMR>(vdCodeValue.Value)
                        : null;

                    // create a record for the database
                    DbStructureMappingRecord mapRec = new()
                    {
                        Key = DbStructureMappingRecord.GetIndex(),
                        SourceFhirPackageKey = sourcePackage.Key,
                        SourceStructureKey = sourceSd.Key,
                        SourceStructureId = sourceSd.Id,

                        TargetFhirPackageKey = targetPackage.Key,
                        TargetStructureKey = targetSd.Key,
                        TargetStructureId = targetSd.Id,

                        StructureKeys = getKeyArray(sourcePackage, targetPackage, sourceSd.Key, targetSd.Key),

                        FmlExists = fmlExists,
                        FmlFilename = fmlFile,
                        FmlUrl = $"http://hl7.org/fhir/uv/xver/StructureMap/{sourceSd.Name}{sourcePackage.ShortName[1..]}to{targetPackage.ShortName[1..]}",

                        Relationship = elementTarget.Relationship,
                        Comments = elementTarget.Comment,

                        ConceptDomainRelationship = cdRelationship,
                        ValueDomainRelationship = vdRelationship,
                        ComputedRelationship = RelationshipComposition.ComputeForDomains(cdRelationship, vdRelationship),

                        OriginatingConceptMapUrlsLiteral = cm.Url,

                        IdLong = idLong,
                        IdShort = idShort,
                        Url = $"http://hl7.org/fhir/{sourcePackage.FhirVersionShort}/ConceptMap/{idLong}",
                        Name = FhirSanitizationUtils.ReformatIdForName(idLong),
                        Title = $"Concept Map of FHIR {sourcePackage.ShortName} type {sourceSd.Name} to FHIR {targetPackage.ShortName} type {targetSd.Name}"
                    };

                    typeDefinitionMapsToAdd.Add(mapRec);
                }
            }
        }

        // insert into the database
        typeDefinitionMapsToAdd.Insert(_dbConnection, ignoreDuplicates: true, insertPrimaryKey: true);
        //_logger.LogInformation($"Inserted {elementMapsToAdd.Count} Type Definition Map records");

        return typeDefinitionMapsToAdd.Count;
    }


    private (string sourceVersion, string targetVersion) getVersionsFromFilename(string filename)
    {
        string fileOnly = Path.GetFileNameWithoutExtension(filename);
        string versionPart = fileOnly.Substring(fileOnly.LastIndexOf('-') + 1);

        int toIndex = versionPart.IndexOf("to", StringComparison.OrdinalIgnoreCase);
        if (toIndex < 1)
        {
            throw new Exception($"Invalid source map filename: {filename}!");
        }

        string sourceVersion = versionPart.Substring(0, toIndex);
        string targetVersion = versionPart.Substring(toIndex + 2);

        return (sourceVersion, targetVersion);
    }


    [Obsolete]
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
        DbComparisonCache<DbCollatedTypeComparison> collatedTypeComparisons = new();
        DbComparisonCache<DbElementTypeComparison> typeComparisons = new();

        List<DbUnresolvedConceptComparison> unresolvedConceptComparisons = [];
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

        _dbConnection.Insert(vsComparisons.ComparisonsToAdd, insertPrimaryKey: true);
        _logger.LogInformation($" <<< added {vsComparisons.Count} ValueSet Comparisons");

        _dbConnection.Insert(conceptComparisons.ComparisonsToAdd, insertPrimaryKey: true);
        _logger.LogInformation($" <<< added {conceptComparisons.Count} ValueSet Concept Comparisons");

        _dbConnection.Insert(unresolvedConceptComparisons);
        _logger.LogInformation($" <<< added {unresolvedConceptComparisons.Count} Unresolved ValueSet Concept Comparisons");

        _dbConnection.Insert(sdComparisons.ComparisonsToAdd, insertPrimaryKey: true);
        _logger.LogInformation($" <<< added {sdComparisons.Count} Structure Comparisons");

        _dbConnection.Insert(unresolvedSdComparisons, insertPrimaryKey: true);
        _logger.LogInformation($" <<< added {unresolvedSdComparisons.Count} Unresolved Structure Comparisons");

        _dbConnection.Insert(elementComparisons.ComparisonsToAdd, insertPrimaryKey: true);
        _logger.LogInformation($" <<< added {elementComparisons.Count} Element Comparisons");

        _dbConnection.Insert(collatedTypeComparisons.ComparisonsToAdd, insertPrimaryKey: true);
        _logger.LogInformation($" <<< added {collatedTypeComparisons.Count} Collated Type Comparisons");

        _dbConnection.Insert(typeComparisons.ComparisonsToAdd, insertPrimaryKey: true);
        _logger.LogInformation($" <<< added {typeComparisons.Count} Type Comparisons");

        _dbConnection.Insert(unresolvedElementComparisons, insertPrimaryKey: true);
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
                // make sure each of the types fmlExists
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

                // look for an cdRelationship comparison record
                if (!sdComparisons.TryGet((sourceDbSd.Key, targetDbSd.Key), out DbStructureComparison? sdComparison))
                {
                    // create our comparison record
                    sdComparison = new()
                    {
                        Key = DbStructureComparison.GetIndex(),
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
                        CompositeName = GetCompositeName(sourceDbPackage, sourceDbSd, targetDbPackage, targetDbSd),
                        SourceOverviewConceptMapUrl = null,
                        SourceStructureFmlUrl = null,
                        Relationship = tm.Relationship,
                        ConceptDomainRelationship = tm.ConceptDomainRelationship,
                        ValueDomainRelationship = tm.ValueDomainRelationship,
                        IsGenerated = true,
                        LastReviewedBy = null,
                        LastReviewedOn = null,
                        ReviewType = null,
                        TechnicalMessage = tm.Comment,
                        UserMessage = $"Mapping of FHIR {sourceDbPackage.ShortName}:{sourceDbSd.Name} to {targetDbPackage.ShortName}:{targetDbSd.Name}",
                        IsIdentical = null,
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
            if ((sourceDbSd is not null) &&
                (targetDbSd is not null))
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
                        Key = DbStructureComparison.GetIndex(),
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
                        CompositeName = GetCompositeName(sourceDbPackage, sourceDbSd, targetDbPackage, targetDbSd),
                        SourceOverviewConceptMapUrl = cm.Url,
                        SourceStructureFmlUrl = null,
                        Relationship = null,
                        ConceptDomainRelationship = (sourceDbSd.Name == targetDbSd.Name) ? CMR.Equivalent : null,
                        ValueDomainRelationship = FhirDbComparer.RelationshipForCounts(sourceDbSd.SnapshotCount, targetDbSd.SnapshotCount),
                        IsGenerated = false,
                        LastReviewedBy = null,
                        LastReviewedOn = null,
                        ReviewType = null,
                        TechnicalMessage = $"Imported from existing ConceptMap {cm.Id} ({cm.Url}).",
                        UserMessage = $"Mapping of FHIR {sourceDbPackage.ShortName}:{sourceDbSd.Name} to {targetDbPackage.ShortName}:{targetDbSd.Name}",
                        IsIdentical = null,
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
                    Key = DbUnresolvedStructureComparison.GetIndex(),
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
                    TechnicalMessage = $"Imported from existing ConceptMap {cm.Id} ({cm.Url}).",
                    UserMessage = null,
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
            DbFhirPackage sourceDbPackage = DbFhirPackage.SelectSingle(
                _dbConnection,
                Key: dbPackagePair.SourcePackageKey)
                ?? throw new Exception($"Source package {dbPackagePair.SourcePackageKey} not found in the database!");

            DbFhirPackage targetDbPackage = DbFhirPackage.SelectSingle(
                _dbConnection,
                Key: dbPackagePair.TargetPackageKey)
                ?? throw new Exception($"Target package {dbPackagePair.TargetPackageKey} not found in the database!");

            if ((dbUnresolvedSdComparison != null) ||
                (dbSdComparison == null) ||
                (sourceDbSd == null))
            {
                comment ??= string.Empty;

                DbUnresolvedElementComparison unresolved = new()
                {
                    Key = DbUnresolvedElementComparison.GetIndex(),
                    PackageComparisonKey = dbPackagePair.Key,
                    SourceFhirPackageKey = dbPackagePair.SourcePackageKey,
                    SourceStructureKey = sourceDbSd?.Key,
                    TargetFhirPackageKey = dbPackagePair.TargetPackageKey,
                    TargetStructureKey = targetDbSd?.Key,
                    StructureComparisonKey = dbSdComparison?.Key,
                    UnresolvedStructureComparisonKey = dbUnresolvedSdComparison?.Key,
                    Relationship = relationship,
                    NoMap = noMap,
                    TechnicalMessage = comment + $" Record found in {cm.Id} ({cm.Url}) with unresolved structure mapping {sourceStructureUrl} -> {targetStructureUrl}.",
                    UserMessage = null,
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
                    Key = DbUnresolvedElementComparison.GetIndex(),
                    PackageComparisonKey = dbPackagePair.Key,
                    SourceFhirPackageKey = dbPackagePair.SourcePackageKey,
                    SourceStructureKey = sourceDbSd.Key,
                    TargetFhirPackageKey = dbPackagePair.TargetPackageKey,
                    TargetStructureKey = targetDbSd?.Key,
                    StructureComparisonKey = dbSdComparison?.Key,
                    UnresolvedStructureComparisonKey = dbUnresolvedSdComparison?.Key,
                    Relationship = relationship,
                    NoMap = noMap,
                    TechnicalMessage = comment + $" Record found in {cm.Id} ({cm.Url}) with unresolved element mapping {sourceStructureUrl}:{sourceToken} -> {targetStructureUrl}:{targetToken}.",
                    UserMessage = null,
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
                Key = DbElementComparison.GetIndex(),
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
                TechnicalMessage = comment,
                UserMessage = $"Mapping of FHIR {sourceDbPackage.ShortName}:{sourceDbSd.Name} element `{sourceDbElement.Id}`" +
                    $" to {targetDbPackage.ShortName}:{targetDbSd?.Name} element `{targetDbElement?.Path}`",
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
                IsIdentical = null,
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
                                Key = DbUnresolvedStructureComparison.GetIndex(),
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
                                TechnicalMessage = message,
                                UserMessage = null,
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
                            Key = DbStructureComparison.GetIndex(),
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
                            CompositeName = GetCompositeName(sourceDbPackage, sourceDbSd, targetDbPackage, targetDbSd),
                            SourceOverviewConceptMapUrl = cm.Url,
                            SourceStructureFmlUrl = null,
                            Relationship = groupTarget.Relationship,
                            ConceptDomainRelationship = (sourceDbSd.Name == targetDbSd.Name) ? CMR.Equivalent : CMR.RelatedTo,
                            ValueDomainRelationship = FhirDbComparer.RelationshipForCounts(sourceDbSd.SnapshotCount, targetDbSd.SnapshotCount),
                            IsGenerated = false,
                            LastReviewedBy = null,
                            LastReviewedOn = null,
                            ReviewType = null,
                            TechnicalMessage = $"Imported from existing ConceptMap {cm.Id} ({cm.Url}).",
                            UserMessage = $"Mapping of FHIR {sourceDbPackage.ShortName}:{sourceDbSd.Name} to {targetDbPackage.ShortName}:{targetDbSd.Name}",
                            IsIdentical = null,
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
                Key = DbValueSetComparison.GetIndex(),
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
                IsIdentical = null,
                CodesAreIdentical = null,
                TechnicalMessage = $"Imported from existing ConceptMap {cm.Id} ({cm.Url}).",
                UserMessage = $"Mapping of FHIR {sourceDbPackage.ShortName}:{sourceDbVs.Name} to {targetDbPackage.ShortName}:{targetDbVs.Name}",
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
                dbVsComparison.TechnicalMessage += $" Note: comparison source: {sourceDbVs.Id} ({sourceDbVs.VersionedUrl}) has been manually excluded.";
            }

            if (targetDbVs.IsExcluded)
            {
                dbVsComparison.TechnicalMessage += $" Note: comparison target: {targetDbVs.Id} ({targetDbVs.VersionedUrl}) has been manually excluded.";
            }

            // check for failure to expand
            if (sourceDbVs.CanExpand == false)
            {
                dbVsComparison.TechnicalMessage += $" Note: source value set {sourceDbVs.Id} ({sourceDbVs.VersionedUrl}) cannot be expanded.";
            }

            if (targetDbVs.CanExpand == false)
            {
                dbVsComparison.TechnicalMessage += $" Note: target value set {targetDbVs.Id} ({targetDbVs.VersionedUrl}) cannot be expanded.";
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
                            sourceDbPackage,
                            sourceDbVs,
                            targetDbPackage,
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
                            sourceDbPackage,
                            sourceDbVs,
                            targetDbPackage,
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
            DbFhirPackage sourceDbPackage,
            DbValueSet sourceDbVs,
            DbFhirPackage targetDbPackage,
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
                        Key = DbUnresolvedConceptComparison.GetIndex(),
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
                        TechnicalMessage = $"Code flagged as noMap in {cm.Id} ({cm.Url}), but does not exist in source {sourceDbVs.Id} ({sourceDbVs.VersionedUrl}).",
                        UserMessage = null,
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

                    unresolvedConceptComparisons.Add(unresolvedNoMap);

                    ////dbPackagePair.InvalidImportedConceptComparisons.Add(invalidComparison);
                    //dbVsComparison.InvalidImportedComparisons.Add(invalidComparison);

                    return;
                }

                DbValueSetConceptComparison nonMappedComparison = new()
                {
                    Key = DbValueSetConceptComparison.GetIndex(),
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
                    TechnicalMessage = $"Code flagged as noMap in {cm.Id} ({cm.Url})",
                    UserMessage = $"FHIR {sourceDbPackage.ShortName}:{sourceDbVs.Name} (`{sourceDbVs.VersionedUrl}`) concept `{sourceDbConcept.System}`#`{sourceDbConcept.Code}`" +
                        $" does not map to FHIR {targetDbPackage.ShortName}:{targetDbVs.Name}.",
                    IsGenerated = false,
                    LastReviewedBy = null,
                    LastReviewedOn = null,
                    IsIdentical = null,
                    CodesAreIdentical = null,
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
                    Key = DbUnresolvedConceptComparison.GetIndex(),
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
                    TechnicalMessage = message,
                    UserMessage = null,
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

                unresolvedConceptComparisons.Add(unresolvedComparison);

                return;
            }

            // check the cache for an inverse record
            _ = conceptComparisons.TryGet((targetDbConcept.Key, sourceDbConcept.Key), out DbValueSetConceptComparison? inverseComparison);

            DbValueSetConceptComparison mappedComparison = new()
            {
                Key = DbValueSetConceptComparison.GetIndex(),
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
                TechnicalMessage = $"Loaded existing mapping of `{sourceDbVs.VersionedUrl}`#`{sourceDbConcept.Code}` to `{targetDbVs.VersionedUrl}`#`{targetDbConcept.Code}` by {cm.Id} ({cm.Url})",
                UserMessage = $"FHIR {sourceDbPackage.ShortName}:{sourceDbVs.Name} (`{sourceDbVs.VersionedUrl}`) code `{sourceDbVs.VersionedUrl}`#`{sourceDbConcept.Code}`" +
                    $" maps to FHIR {targetDbPackage.ShortName}:{targetDbVs.Name} (`{targetDbVs.VersionedUrl}`) code `{targetDbVs.VersionedUrl}`#`{targetDbConcept.Code}`",
                IsGenerated = false,
                LastReviewedBy = null,
                LastReviewedOn = null,
                IsIdentical = null,
                CodesAreIdentical = null,
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

    public static string GetCompositeName(
        DbFhirPackage sourceDbPackage,
        DbMetadataResource sourceDbCanonical,
        DbFhirPackage targetDbPackage,
        DbMetadataResource? targetDbCanonical)
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

    public static string GetCompositeName(
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

            // load our code systems
            addCodeSystemsToDb(pm, dc, _exclusionSet);

            // load our value sets
            addValueSetsToDb(pm, dc, _exclusionSet, _escapeValveCodes);

            // load our structures
            addStructuresToDb(pm, dc, _exclusionSet);
        }

        // do value set post-processing
        doValueSetPostProcessing(_escapeValveCodes);

        // do element post-processing
        doElementPostProcessing();

        // do code-system post-processing
        doCodeSystemPostProcessing();

        return true;
    }


    private void doCodeSystemPostProcessing()
    {
        DbFhirPackage? r5 = DbFhirPackage.SelectSingle(
            _dbConnection,
            PackageId: "hl7.fhir.r5.core");
        
        if (r5 != null)
        {
            IDbCommand command = _dbConnection.CreateCommand();
            command.CommandText = $"""
                delete from {DbCodeSystem.DefaultTableName}
                where {nameof(DbCodeSystem.FhirPackageKey)} = {r5.Key}
                and {nameof(DbCodeSystem.SourcePackageMoniker)} = 'hl7.terminology@5.1.0'
                and {nameof(DbCodeSystem.UnversionedUrl)} not in (
                    select distinct {nameof(DbValueSetConcept.System)}
                    from {DbValueSetConcept.DefaultTableName}
                    where {nameof(DbValueSetConcept.FhirPackageKey)} = {r5.Key}
                    and {nameof(DbValueSetConcept.ValueSetKey)} in (
                        select distinct {nameof(DbElement.BindingValueSetKey)}
                        from {DbElement.DefaultTableName}
                        where {nameof(DbElement.FhirPackageKey)} = {r5.Key}
                        and {nameof(DbElement.BindingValueSetKey)} is not null
                    )
                    and {nameof(DbValueSetConcept.System)} is not null
                )
                """;
            command.ExecuteNonQuery();

            command = _dbConnection.CreateCommand();
            command.CommandText =
                $"delete from {DbCodeSystemConcept.DefaultTableName}" +
                $" where {nameof(DbCodeSystemConcept.CodeSystemKey)} not in (select {nameof(DbCodeSystem.Key)} from {DbCodeSystem.DefaultTableName})";
            command.ExecuteNonQuery();

            command = _dbConnection.CreateCommand();
            command.CommandText =
                $"delete from {DbCodeSystemFilter.DefaultTableName}" +
                $" where {nameof(DbCodeSystemFilter.CodeSystemKey)} not in (select {nameof(DbCodeSystem.Key)} from {DbCodeSystem.DefaultTableName})";
            command.ExecuteNonQuery();

            command = _dbConnection.CreateCommand();
            command.CommandText =
                $"delete from {DbCodeSystemPropertyDefinition.DefaultTableName}" +
                $" where {nameof(DbCodeSystemPropertyDefinition.CodeSystemKey)} not in (select {nameof(DbCodeSystem.Key)} from {DbCodeSystem.DefaultTableName})";
            command.ExecuteNonQuery();

            command = _dbConnection.CreateCommand();
            command.CommandText =
                $"delete from {DbCodeSystemConceptProperty.DefaultTableName}" +
                $" where {nameof(DbCodeSystemConceptProperty.CodeSystemPropertyDefinitionKey)}" +
                $" not in (select {nameof(DbCodeSystemPropertyDefinition.Key)} from {DbCodeSystemPropertyDefinition.DefaultTableName})";
            command.ExecuteNonQuery();
        }

        return;
    }

    private void doElementPostProcessing()
    {
        throw new Exception("Fix this");

        // TODO: update elements that use content references to expand out their child elements

        // get the list of elements that do not have BaseElementKey and BaseStructureKey set, but do have types that are other elements

        // update the elements to have the correct keys

        // get the list of elements that are content references (basePath includes a period?)

        // traverse the list of elements and expand (fixing the path) of the child elements

        return;
    }

    private void addCodeSystemsToDb(
        DbFhirPackage pm,
        DefinitionCollection dc,
        HashSet<string> _exclusionSet)
    {
        List<DbCodeSystem> dbCodeSystems = [];
        List<DbCodeSystemFilter> dbCodeSystemFilters = [];
        List<DbCodeSystemPropertyDefinition> dbCodeSystemPropertyDefinitions = [];
        List<DbCodeSystemConcept> allDbConcepts = [];
        List<DbCodeSystemConceptProperty> allDbConceptProperties = [];

        string fhirVersionLiteral = pm.DefinitionFhirSequence.ToString();

        // iterate over the code systems in the definition collection
        foreach ((string codeSystemUrl, CodeSystem cs) in dc.CodeSystemsByUrl.OrderBy(kvp => kvp.Key))
        {
            DbCodeSystem? existingDbCs = DbCodeSystem.SelectSingle(
                _dbConnection,
                FhirPackageKey: pm.Key,
                UnversionedUrl: codeSystemUrl);

            // check to see if this code system already fmlExists
            if (existingDbCs != null)
            {
                continue;
            }

            bool isExcluded = _exclusionSet.Contains(codeSystemUrl);

            // will not further process code systems we know we will not process
            if (isExcluded || (cs == null))
            {
                if (cs == null)
                {
                    continue;
                }

                int cseVsPipeIndex = string.IsNullOrEmpty(cs.ValueSet) ? -1 : cs.ValueSet.LastIndexOf('|');
                int cseSuppPipeIndex = string.IsNullOrEmpty(cs.Supplements) ? -1 : cs.Supplements.LastIndexOf('|');

                // still add a metadata record for excluded or null code systems
                DbCodeSystem excludedDbCodeSystem = new()
                {
                    Key = DbCodeSystem.GetIndex(),
                    FhirPackageKey = pm.Key,
                    Id = cs.Id,
                    VersionedUrl = cs.Url + (string.IsNullOrEmpty(cs.Version) ? "" : "|" + cs.Version),
                    UnversionedUrl = cs.Url ?? codeSystemUrl,
                    SourcePackageMoniker = cs.cgPackageSourceAsMoniker(),
                    Name = cs.Name ?? cs.Id,
                    Version = cs.Version ?? pm.PackageVersion,
                    VersionAlgorithmString = (cs.VersionAlgorithm != null) && (cs.VersionAlgorithm is FhirString cseVaFs) ? cseVaFs.Value : null,
                    VersionAlgorithmCoding = (cs.VersionAlgorithm != null) && (cs.VersionAlgorithm is Coding cseVaC) ? cseVaC : null,
                    Status = cs.Status,
                    Title = cs.Title.ProcessCoreTextForLinks(fhirVersionLiteral),
                    Description = cs.Description.ProcessCoreTextForLinks(fhirVersionLiteral),
                    Purpose = cs.Purpose.ProcessCoreTextForLinks(fhirVersionLiteral),
                    Narrative = cs.Text.ProcessCoreTextForLinks(fhirVersionLiteral),
                    StandardStatus = cs.cgStandardStatus(),
                    WorkGroup = cs.cgWorkGroup(),
                    FhirMaturity = cs.cgMaturityLevel(),
                    IsExperimental = cs.Experimental,
                    LastChangedDate = cs.DateElement?.ToDateTimeOffset(TimeSpan.Zero),
                    Publisher = cs.Publisher,
                    Copyright = cs.Copyright,
                    CopyrightLabel = cs.CopyrightLabel,
                    ApprovalDate = cs.ApprovalDate,
                    LastReviewDate = cs.LastReviewDate,
                    EffectivePeriodStart = cs.EffectivePeriod?.StartElement?.ToDateTimeOffset(TimeSpan.Zero),
                    EffectivePeriodEnd = cs.EffectivePeriod?.EndElement?.ToDateTimeOffset(TimeSpan.Zero),
                    Topic = cs.Topic,
                    RelatedArtifacts = cs.RelatedArtifact,
                    Jurisdictions = cs.Jurisdiction,
                    UseContexts = cs.UseContext,
                    Contacts = cs.Contact,
                    Authors = cs.Author,
                    Editors = cs.Editor,
                    Reviewers = cs.Reviewer,
                    Endorsers = cs.Endorser,
                    RootExtensions = cs.Extension,
                    IsCaseSensitive = cs.CaseSensitive,
                    ValueSetVersioned = cs.ValueSet,
                    ValueSetUnversioned = string.IsNullOrEmpty(cs.ValueSet) ? null : (cseVsPipeIndex == -1 ? cs.ValueSet : cs.ValueSet[0..cseVsPipeIndex]),
                    HierarchyMeaning = cs.HierarchyMeaning,
                    IsCompositional = cs.Compositional,
                    VersionNeeded = cs.VersionNeeded,
                    Content = cs.Content,
                    SupplementsVersioned = cs.Supplements,
                    SupplementsUnversioned = string.IsNullOrEmpty(cs.Supplements) ? null : (cseSuppPipeIndex == -1 ? cs.Supplements : cs.Supplements[0..cseSuppPipeIndex]),
                    Count = 0, // no concepts processed for excluded items
                };

                dbCodeSystems.Add(excludedDbCodeSystem);

                continue;
            }

            int csVsPipeIndex = string.IsNullOrEmpty(cs.ValueSet) ? -1 : cs.ValueSet.LastIndexOf('|');
            int csSuppPipeIndex = string.IsNullOrEmpty(cs.Supplements) ? -1 : cs.Supplements.LastIndexOf('|');

            // create the DbCodeSystem record
            DbCodeSystem dbCodeSystem = new()
            {
                Key = DbCodeSystem.GetIndex(),
                FhirPackageKey = pm.Key,
                Id = cs.Id,
                VersionedUrl = cs.Url + (string.IsNullOrEmpty(cs.Version) ? "" : "|" + cs.Version),
                UnversionedUrl = cs.Url ?? codeSystemUrl,
                SourcePackageMoniker = cs.cgPackageSourceAsMoniker(),
                Name = cs.Name ?? cs.Id,
                Version = cs.Version ?? pm.PackageVersion,
                VersionAlgorithmString = (cs.VersionAlgorithm != null) && (cs.VersionAlgorithm is FhirString csVaFs) ? csVaFs.Value : null,
                VersionAlgorithmCoding = (cs.VersionAlgorithm != null) && (cs.VersionAlgorithm is Coding csVaC) ? csVaC : null,
                Status = cs.Status,
                Title = cs.Title.ProcessCoreTextForLinks(fhirVersionLiteral),
                Description = cs.Description.ProcessCoreTextForLinks(fhirVersionLiteral),
                Purpose = cs.Purpose.ProcessCoreTextForLinks(fhirVersionLiteral),
                Narrative = cs.Text.ProcessCoreTextForLinks(fhirVersionLiteral),
                StandardStatus = cs.cgStandardStatus(),
                WorkGroup = cs.cgWorkGroup(),
                FhirMaturity = cs.cgMaturityLevel(),
                IsExperimental = cs.Experimental,
                LastChangedDate = cs.DateElement?.ToDateTimeOffset(TimeSpan.Zero),
                Publisher = cs.Publisher,
                Copyright = cs.Copyright,
                CopyrightLabel = cs.CopyrightLabel,
                ApprovalDate = cs.ApprovalDate,
                LastReviewDate = cs.LastReviewDate,
                EffectivePeriodStart = cs.EffectivePeriod?.StartElement?.ToDateTimeOffset(TimeSpan.Zero),
                EffectivePeriodEnd = cs.EffectivePeriod?.EndElement?.ToDateTimeOffset(TimeSpan.Zero),
                Topic = cs.Topic,
                RelatedArtifacts = cs.RelatedArtifact,
                Jurisdictions = cs.Jurisdiction,
                UseContexts = cs.UseContext,
                Contacts = cs.Contact,
                Authors = cs.Author,
                Editors = cs.Editor,
                Reviewers = cs.Reviewer,
                Endorsers = cs.Endorser,
                RootExtensions = cs.Extension,
                IsCaseSensitive = cs.CaseSensitive,
                ValueSetVersioned = cs.ValueSet,
                ValueSetUnversioned = string.IsNullOrEmpty(cs.ValueSet) ? null : (csVsPipeIndex == -1 ? cs.ValueSet : cs.ValueSet[0..csVsPipeIndex]),
                HierarchyMeaning = cs.HierarchyMeaning,
                IsCompositional = cs.Compositional,
                VersionNeeded = cs.VersionNeeded,
                Content = cs.Content,
                SupplementsVersioned = cs.Supplements,
                SupplementsUnversioned = string.IsNullOrEmpty(cs.Supplements) ? null : (csSuppPipeIndex == -1 ? cs.Supplements : cs.Supplements[0..csSuppPipeIndex]),
                Count = cs.Count,
            };

            dbCodeSystems.Add(dbCodeSystem);

            // add defined filters to the list
            foreach (CodeSystem.FilterComponent filter in cs.Filter)
            {
                DbCodeSystemFilter dbFilter = new()
                {
                    Key = DbCodeSystemFilter.GetIndex(),
                    FhirPackageKey = pm.Key,
                    CodeSystemKey = dbCodeSystem.Key,
                    Code = filter.Code,
                    Description = filter.Description,
                    Operators = string.Join("|", filter.Operator?.Select(op => op.GetLiteral()) ?? []),
                    Value = filter.Value,
                };

                dbCodeSystemFilters.Add(dbFilter);
            }

            // add property definitions to the list
            foreach (CodeSystem.PropertyComponent? property in cs.Property)
            {
                DbCodeSystemPropertyDefinition dbPropertyDefinition = new()
                {
                    Key = DbCodeSystemPropertyDefinition.GetIndex(),
                    FhirPackageKey = pm.Key,
                    CodeSystemKey = dbCodeSystem.Key,
                    Code = property.Code,
                    Uri = property.Uri,
                    Description = property.Description,
                    Type = property.Type ?? Hl7.Fhir.Model.CodeSystem.PropertyType.Code,
                };

                dbCodeSystemPropertyDefinitions.Add(dbPropertyDefinition);
            }
            // add concepts to the list (handling hierarchy)
            List<DbCodeSystemConcept> conceptsForThisCodeSystem = [];
            List<DbCodeSystemConceptProperty> conceptPropertiesForThisCodeSystem = [];
            
            // create a lookup for property definitions by code for this code system
            ILookup<string, DbCodeSystemPropertyDefinition> propertyDefsByCode = dbCodeSystemPropertyDefinitions
                .Where(pd => pd.CodeSystemKey == dbCodeSystem.Key)
                .ToLookup(pd => pd.Code);

            int globalOrder = allDbConcepts.Count;
            processConceptHierarchy(
                cs.Concept,
                dbCodeSystem.Key,
                pm.Key,
                null,
                0,
                ref globalOrder, 
                conceptsForThisCodeSystem,
                conceptPropertiesForThisCodeSystem,
                propertyDefsByCode,
                fhirVersionLiteral);

            allDbConcepts.AddRange(conceptsForThisCodeSystem);
            allDbConceptProperties.AddRange(conceptPropertiesForThisCodeSystem);

        }

        _logger.LogInformation($"Inserting CodeSystems for {pm.PackageId}@{pm.PackageVersion} into database...");

        _dbConnection.Insert(dbCodeSystems, insertPrimaryKey: true);
        _logger.LogInformation($" <<< added {dbCodeSystems.Count} CodeSystems");

        _dbConnection.Insert(dbCodeSystemFilters, insertPrimaryKey: true);
        _logger.LogInformation($" <<< added {dbCodeSystemFilters.Count} CodeSystem Filters");

        _dbConnection.Insert(dbCodeSystemPropertyDefinitions, insertPrimaryKey: true);
        _logger.LogInformation($" <<< added {dbCodeSystemPropertyDefinitions.Count} CodeSystem Property Definitions");

        _dbConnection.Insert(allDbConcepts, insertPrimaryKey: true);
        _logger.LogInformation($" <<< added {allDbConcepts.Count} CodeSystem Concepts");

        _dbConnection.Insert(allDbConceptProperties, insertPrimaryKey: true);
        _logger.LogInformation($" <<< added {allDbConceptProperties.Count} CodeSystem Concept Properties");

        return;
    }

    private void processConceptHierarchy(
        IList<CodeSystem.ConceptDefinitionComponent> concepts,
        int codeSystemKey,
        int fhirPackageKey,
        int? parentConceptKey,
        int relativeOrder,
        ref int globalOrder,
        List<DbCodeSystemConcept> allConcepts,
        List<DbCodeSystemConceptProperty> allConceptProperties,
        ILookup<string, DbCodeSystemPropertyDefinition> propertyDefsByCode,
        string fhirVersionLiteral)
    {
        foreach (CodeSystem.ConceptDefinitionComponent concept in concepts)
        {
            // skip concepts without valid codes - I have no way of correctly passing them through
            if (string.IsNullOrEmpty(concept.Code))
            {
                continue;
            }

            // create the DbCodeSystemConcept record
            DbCodeSystemConcept dbConcept = new()
            {
                Key = DbCodeSystemConcept.GetIndex(),
                FhirPackageKey = fhirPackageKey,
                CodeSystemKey = codeSystemKey,
                FlatOrder = globalOrder++,
                RelativeOrder = relativeOrder,
                Code = concept.Code,
                Display = concept.Display,
                Definition = concept.Definition.ProcessCoreTextForLinks(fhirVersionLiteral),
                Designations = concept.Designation,
                Properties = concept.Property,
                ParentConceptKey = parentConceptKey,
                ChildConceptCount = concept.Concept?.Count ?? 0,
            };

            allConcepts.Add(dbConcept);

            // process concept properties
            foreach (CodeSystem.ConceptPropertyComponent conceptProperty in concept.Property)
            {
                // find the corresponding property definition
                DbCodeSystemPropertyDefinition? propertyDef = propertyDefsByCode[conceptProperty.Code].FirstOrDefault();
                if (propertyDef != null)
                {
                    DbCodeSystemConceptProperty dbConceptProperty = new()
                    {
                        Key = DbCodeSystemConceptProperty.GetIndex(),
                        FhirPackageKey = fhirPackageKey,
                        CodeSystemConceptKey = dbConcept.Key,
                        CodeSystemPropertyDefinitionKey = propertyDef.Key,
                        Code = conceptProperty.Code,
                        Type = getPropertyTypeFromValue(conceptProperty.Value),
                        Value = getPropertyValueString(conceptProperty.Value),
                    };

                    allConceptProperties.Add(dbConceptProperty);
                }
            }

            // recursively process child concepts
            if (concept.Concept?.Count > 0)
            {
                processConceptHierarchy(
                    concept.Concept,
                    codeSystemKey,
                    fhirPackageKey,
                    dbConcept.Key,
                    0, // reset relative order for children
                    ref globalOrder,
                    allConcepts,
                    allConceptProperties,
                    propertyDefsByCode,
                    fhirVersionLiteral);
            }

            relativeOrder++;
        }
    }

    private static Hl7.Fhir.Model.CodeSystem.PropertyType getPropertyTypeFromValue(DataType? value)
    {
        return value switch
        {
            Code => Hl7.Fhir.Model.CodeSystem.PropertyType.Code,
            Coding => Hl7.Fhir.Model.CodeSystem.PropertyType.Coding,
            FhirString => Hl7.Fhir.Model.CodeSystem.PropertyType.String,
            Integer => Hl7.Fhir.Model.CodeSystem.PropertyType.Integer,
            FhirBoolean => Hl7.Fhir.Model.CodeSystem.PropertyType.Boolean,
            FhirDateTime => Hl7.Fhir.Model.CodeSystem.PropertyType.DateTime,
            FhirDecimal => Hl7.Fhir.Model.CodeSystem.PropertyType.Decimal,
            _ => Hl7.Fhir.Model.CodeSystem.PropertyType.Code
        };
    }

    private static string getPropertyValueString(DataType? value)
    {
        return value switch
        {
            Code c => c.Value ?? "",
            FhirString s => s.Value ?? "",
            Integer i => i.Value?.ToString() ?? "",
            FhirBoolean b => b.Value?.ToString() ?? "",
            FhirDateTime dt => dt.Value ?? "",
            FhirDecimal d => d.Value?.ToString() ?? "",
            Coding coding => $"{coding.System}|{coding.Code}|{coding.Display}",
            _ => value?.ToString() ?? ""
        };
    }

    private void doValueSetPostProcessing(HashSet<string> _escapeValveCodes)
    {
        {
            IDbCommand command = _dbConnection.CreateCommand();
            command.CommandText = $"""
                update {DbValueSet.DefaultTableName}
                set {nameof(DbValueSet.HasEscapeValveCode)} = 1
                where {nameof(DbValueSet.Key)} in
                (
                  select distinct {nameof(DbValueSetConcept.ValueSetKey)}
                  from {DbValueSetConcept.DefaultTableName}
                  where {nameof(DbValueSetConcept.Code)} in ({string.Join(", ", _escapeValveCodes.Select(v => "'" + v + "'"))})
                )
                """;

            command.ExecuteNonQuery();
        }

        {
            IDbCommand command = _dbConnection.CreateCommand();
            command.CommandText = $"""
                update {DbValueSetConcept.DefaultTableName}
                set {nameof(DbValueSetConcept.System)} = 'http://hl7.org/fhir/sample-security-structural-roles'
                where {nameof(DbValueSetConcept.System)} = 'sample-security-structural-roles'
                and {nameof(DbValueSetConcept.SystemVersion)} = '5.0.0'
                """;

            command.ExecuteNonQuery();
        }

        {
            // update display values from code systems where possible
            IDbCommand command = _dbConnection.CreateCommand();
            command.CommandText = $"""
                UPDATE {DbValueSetConcept.DefaultTableName} 
                SET {nameof(DbValueSetConcept.Display)} = (
                    SELECT csc.{nameof(DbCodeSystemConcept.Display)}
                    FROM {DbCodeSystem.DefaultTableName} cs
                    JOIN {DbCodeSystemConcept.DefaultTableName} csc ON cs.{nameof(DbCodeSystem.Key)} = csc.{nameof(DbCodeSystemConcept.CodeSystemKey)}
                    WHERE {DbValueSetConcept.DefaultTableName}.{nameof(DbValueSetConcept.FhirPackageKey)} = cs.{nameof(DbCodeSystem.FhirPackageKey)}
                      AND {DbValueSetConcept.DefaultTableName}.{nameof(DbValueSetConcept.System)} = cs.{nameof(DbCodeSystem.UnversionedUrl)}
                      AND {DbValueSetConcept.DefaultTableName}.{nameof(DbValueSetConcept.SystemVersion)} = cs.{nameof(DbCodeSystem.Version)}
                      AND {DbValueSetConcept.DefaultTableName}.{nameof(DbValueSetConcept.Code)} = csc.{nameof(DbCodeSystemConcept.Code)}
                )
                WHERE {DbValueSetConcept.DefaultTableName}.{nameof(DbValueSetConcept.Display)} IS NULL
                  AND EXISTS (
                    SELECT 1 
                    FROM {DbCodeSystem.DefaultTableName} cs
                    JOIN {DbCodeSystemConcept.DefaultTableName} csc ON cs.{nameof(DbCodeSystem.Key)} = csc.{nameof(DbCodeSystemConcept.CodeSystemKey)}
                    WHERE {DbValueSetConcept.DefaultTableName}.{nameof(DbValueSetConcept.FhirPackageKey)} = cs.{nameof(DbCodeSystem.FhirPackageKey)}
                      AND {DbValueSetConcept.DefaultTableName}.{nameof(DbValueSetConcept.System)} = cs.{nameof(DbCodeSystem.UnversionedUrl)}
                      AND {DbValueSetConcept.DefaultTableName}.{nameof(DbValueSetConcept.SystemVersion)} = cs.{nameof(DbCodeSystem.Version)}
                      AND {DbValueSetConcept.DefaultTableName}.{nameof(DbValueSetConcept.Code)} = csc.{nameof(DbCodeSystemConcept.Code)}
                  )
                """;

            command.ExecuteNonQuery();
        }

        return;
    }

    private void addValueSetsToDb(
        DbFhirPackage pm,
        DefinitionCollection dc,
        HashSet<string> _exclusionSet,
        HashSet<string> _escapeValveCodes)
    {
        List<DbValueSet> dbValueSets = [];
        List<DbValueSetConcept> allDbConcepts = [];

        string fhirVersionLiteral = pm.DefinitionFhirSequence.ToString();

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

            // check to see if this value set already fmlExists
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
                    Key = DbValueSet.GetIndex(),
                    FhirPackageKey = pm.Key,
                    Id = uvs.Id,
                    VersionedUrl = versionedUrl,
                    UnversionedUrl = unversionedUrl,
                    SourcePackageMoniker = uvs.cgPackageSourceAsMoniker(),
                    Name = uvs.Name,
                    Version = vsVersion,
                    VersionAlgorithmString = (uvs.VersionAlgorithm != null) && (uvs.VersionAlgorithm is FhirString vsmVaFs) ? vsmVaFs.Value : null,
                    VersionAlgorithmCoding = (uvs.VersionAlgorithm != null) && (uvs.VersionAlgorithm is Coding vsmVaC) ? vsmVaC : null,
                    Status = uvs.Status,
                    Title = uvs.Title.ProcessCoreTextForLinks(fhirVersionLiteral),
                    Description = uvs.Description.ProcessCoreTextForLinks(fhirVersionLiteral),
                    Purpose = uvs.Purpose.ProcessCoreTextForLinks(fhirVersionLiteral),
                    Narrative = uvs.Text.ProcessCoreTextForLinks(fhirVersionLiteral),
                    StandardStatus = uvs.cgStandardStatus(),
                    WorkGroup = uvs.cgWorkGroup(),
                    FhirMaturity = uvs.cgMaturityLevel(),
                    IsExperimental = uvs.Experimental,
                    LastChangedDate = uvs.DateElement?.ToDateTimeOffset(TimeSpan.Zero),
                    Publisher = uvs.Publisher,
                    Copyright = uvs.Copyright,
                    CopyrightLabel = uvs.CopyrightLabel,
                    ApprovalDate = uvs.ApprovalDate,
                    LastReviewDate = uvs.LastReviewDate,
                    EffectivePeriodStart = uvs.EffectivePeriod?.StartElement?.ToDateTimeOffset(TimeSpan.Zero),
                    EffectivePeriodEnd = uvs.EffectivePeriod?.EndElement?.ToDateTimeOffset(TimeSpan.Zero),
                    Topic = uvs.Topic,
                    RelatedArtifacts = uvs.RelatedArtifact,
                    Jurisdictions = uvs.Jurisdiction,
                    UseContexts = uvs.UseContext,
                    Contacts = uvs.Contact,
                    Authors = uvs.Author,
                    Editors = uvs.Editor,
                    Reviewers = uvs.Reviewer,
                    Endorsers = uvs.Endorser,
                    RootExtensions = uvs.Extension,
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
                    Compose = uvs.Compose,
                };

                dbValueSets.Add(vsmExcluded);

                continue;
            }

            DbValueSet dbVs = new()
            {
                Key = DbValueSet.GetIndex(),
                FhirPackageKey = pm.Key,
                Id = vs.Id,
                VersionedUrl = versionedUrl,
                UnversionedUrl = unversionedUrl,
                SourcePackageMoniker = vs.cgPackageSourceAsMoniker(),
                Name = vs.Name,
                Version = vsVersion,
                VersionAlgorithmString = (vs.VersionAlgorithm != null) && (vs.VersionAlgorithm is FhirString vsVaFs) ? vsVaFs.Value : null,
                VersionAlgorithmCoding = (vs.VersionAlgorithm != null) && (vs.VersionAlgorithm is Coding vsVaC) ? vsVaC : null,
                Status = vs.Status,
                Title = vs.Title.ProcessCoreTextForLinks(fhirVersionLiteral),
                Description = vs.Description.ProcessCoreTextForLinks(fhirVersionLiteral),
                Purpose = vs.Purpose.ProcessCoreTextForLinks(fhirVersionLiteral),
                Narrative = vs.Text.ProcessCoreTextForLinks(fhirVersionLiteral),
                StandardStatus = vs.cgStandardStatus(),
                WorkGroup = vs.cgWorkGroup(),
                FhirMaturity = vs.cgMaturityLevel(),
                IsExperimental = vs.Experimental,
                LastChangedDate = vs.DateElement?.ToDateTimeOffset(TimeSpan.Zero),
                Publisher = vs.Publisher,
                Copyright = vs.Copyright,
                CopyrightLabel = vs.CopyrightLabel,
                ApprovalDate = vs.ApprovalDate,
                LastReviewDate = vs.LastReviewDate,
                EffectivePeriodStart = vs.EffectivePeriod?.StartElement?.ToDateTimeOffset(TimeSpan.Zero),
                EffectivePeriodEnd = vs.EffectivePeriod?.EndElement?.ToDateTimeOffset(TimeSpan.Zero),
                Topic = vs.Topic,
                RelatedArtifacts = vs.RelatedArtifact,
                Jurisdictions = vs.Jurisdiction,
                UseContexts = vs.UseContext,
                Contacts = vs.Contact,
                Authors = vs.Author,
                Editors = vs.Editor,
                Reviewers = vs.Reviewer,
                Endorsers = vs.Endorser,
                RootExtensions = vs.Extension,
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
                Compose = vs.Compose,
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

                // check for this record already cdRelationship
                if (DbValueSetConcept.SelectSingle(_dbConnection, FhirPackageKey: pm.Key, ValueSetKey: dbVs.Key, System: fc.System, Code: fc.Code) != null)
                {
                    continue;
                }

                // create a new content record
                DbValueSetConcept dbConcept = new()
                {
                    Key = DbValueSetConcept.GetIndex(),
                    FhirPackageKey = pm.Key,
                    ValueSetKey = dbVs.Key,
                    System = fc.System,
                    SystemVersion = fc.Version,
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

        _dbConnection.Insert(dbValueSets, insertPrimaryKey: true);
        _logger.LogInformation($" <<< added {dbValueSets.Count} ValueSets");

        _dbConnection.Insert(allDbConcepts, insertPrimaryKey: true);
        _logger.LogInformation($" <<< added {allDbConcepts.Count} ValueSet Concepts");

        return;
    }

    private void addStructuresToDb(
        DbFhirPackage pm,
        DefinitionCollection dc,
        HashSet<string> _exclusionSet)
    {
        Dictionary<string, DbStructureDefinition> dbStructures = [];
        Dictionary<string, DbElement> dbElements = [];
        List<DbCollatedType> dbCollatedTypes = [];
        List<DbElementType> dbElementTypes = [];
        List<DbElementAdditionalBinding> dbAdditionalBindings = [];

        string fhirVersionLiteral = pm.DefinitionFhirSequence.ToString();

        // iterate over the types of structures
        foreach ((IEnumerable<StructureDefinition> structures, FhirArtifactClassEnum cgClass) in getStructures(dc))
        {
            foreach (StructureDefinition sd in structures)
            {
                string? sdImplements = sd.cgImplementsJoined();

                // will not further process value sets we know we will not process
                if (_exclusionSet.Contains(sd.Url))
                {
                    // still add a metadata record
                    DbStructureDefinition sdmExcluded = new()
                    {
                        Key = DbStructureDefinition.GetIndex(),
                        FhirPackageKey = pm.Key,
                        Id = sd.Id,
                        VersionedUrl = sd.Url + "|" + sd.Version,
                        UnversionedUrl = sd.Url,
                        SourcePackageMoniker = sd.cgPackageSourceAsMoniker(),
                        Name = FhirSanitizationUtils.SanitizeForProperty(sd.Name, replacements: []),
                        Version = sd.Version,
                        VersionAlgorithmString = (sd.VersionAlgorithm != null) && (sd.VersionAlgorithm is FhirString sdeVaFs) ? sdeVaFs.Value : null,
                        VersionAlgorithmCoding = (sd.VersionAlgorithm != null) && (sd.VersionAlgorithm is Coding sdeVaC) ? sdeVaC : null,
                        Status = sd.Status,
                        Title = (sd.Title ?? sd.Snapshot?.Element.FirstOrDefault()?.Short).ProcessCoreTextForLinks(fhirVersionLiteral),
                        Description = (sd.Description ?? sd.Snapshot?.Element.FirstOrDefault()?.Definition).ProcessCoreTextForLinks(fhirVersionLiteral),
                        Purpose = sd.Purpose.ProcessCoreTextForLinks(fhirVersionLiteral),
                        Narrative = sd.Text.ProcessCoreTextForLinks(fhirVersionLiteral),
                        StandardStatus = sd.cgStandardStatus(),
                        WorkGroup = sd.cgWorkGroup(),
                        FhirMaturity = sd.cgMaturityLevel(),
                        IsExperimental = sd.Experimental,
                        LastChangedDate = sd.DateElement?.ToDateTimeOffset(TimeSpan.Zero),
                        Publisher = sd.Publisher,
                        Copyright = sd.Copyright,
                        CopyrightLabel = sd.CopyrightLabel,
                        ApprovalDate = null,
                        LastReviewDate = null,
                        EffectivePeriodStart = null,
                        EffectivePeriodEnd = null,
                        Topic = null,
                        RelatedArtifacts = null,
                        Jurisdictions = sd.Jurisdiction,
                        UseContexts = sd.UseContext,
                        Contacts = sd.Contact,
                        Authors = null,
                        Editors = null,
                        Reviewers = null,
                        Endorsers = null,
                        RootExtensions = sd.Extension,
                        Comment = sd.Snapshot?.Element.FirstOrDefault()?.Comment.ProcessCoreTextForLinks(fhirVersionLiteral),
                        ArtifactClass = cgClass,
                        Message = "Manually excluded",
                        SnapshotCount = sd.Snapshot?.Element.Count ?? 0,
                        DifferentialCount = sd.Differential?.Element.Count ?? 0,
                        Implements = sdImplements,
                    };

                    dbStructures.Add(sd.Id, sdmExcluded);

                    continue;
                }

                // create a new metadata record
                DbStructureDefinition dbStructure = new()
                {
                    Key = DbStructureDefinition.GetIndex(),
                    FhirPackageKey = pm.Key,
                    Id = sd.Id,
                    VersionedUrl = sd.Url + "|" + sd.Version,
                    UnversionedUrl = sd.Url,
                    SourcePackageMoniker = sd.cgPackageSourceAsMoniker(),
                    Name = sd.Name,
                    Version = sd.Version,
                    VersionAlgorithmString = (sd.VersionAlgorithm != null) && (sd.VersionAlgorithm is FhirString sdVaFs) ? sdVaFs.Value : null,
                    VersionAlgorithmCoding = (sd.VersionAlgorithm != null) && (sd.VersionAlgorithm is Coding sdVaC) ? sdVaC : null,
                    Status = sd.Status,
                    Title = (sd.Title ?? sd.Snapshot?.Element.FirstOrDefault()?.Short).ProcessCoreTextForLinks(fhirVersionLiteral),
                    Description = (sd.Description ?? sd.Snapshot?.Element.FirstOrDefault()?.Definition).ProcessCoreTextForLinks(fhirVersionLiteral),
                    Purpose = sd.Purpose.ProcessCoreTextForLinks(fhirVersionLiteral),
                    Narrative = sd.Text.ProcessCoreTextForLinks(fhirVersionLiteral),
                    StandardStatus = sd.cgStandardStatus(),
                    WorkGroup = sd.cgWorkGroup(),
                    FhirMaturity = sd.cgMaturityLevel(),
                    IsExperimental = sd.Experimental,
                    LastChangedDate = sd.DateElement?.ToDateTimeOffset(TimeSpan.Zero),
                    Publisher = sd.Publisher,
                    Copyright = sd.Copyright,
                    CopyrightLabel = sd.CopyrightLabel,
                    ApprovalDate = null,
                    LastReviewDate = null,
                    EffectivePeriodStart = null,
                    EffectivePeriodEnd = null,
                    Topic = null,
                    RelatedArtifacts = null,
                    Jurisdictions = sd.Jurisdiction,
                    UseContexts = sd.UseContext,
                    Contacts = sd.Contact,
                    Authors = null,
                    Editors = null,
                    Reviewers = null,
                    Endorsers = null,
                    RootExtensions = sd.Extension,
                    Comment = (sd.Snapshot?.Element.FirstOrDefault()?.Comment).ProcessCoreTextForLinks(fhirVersionLiteral),
                    ArtifactClass = cgClass,
                    Message = string.Empty,
                    SnapshotCount = sd.Snapshot?.Element.Count ?? 0,
                    DifferentialCount = sd.Differential?.Element.Count ?? 0,
                    Implements = sdImplements,
                };

                dbStructures.Add(sd.Id, dbStructure);

                // iterate over all the elements of the structure
                foreach (ElementDefinition ed in sd.cgElements(skipSlices: false))
                {
                    addElementAndTypes(dbStructure, sd, ed);
                }
            }
        }

        // save changes
        _logger.LogInformation($"Inserting Structures for {pm.PackageId}@{pm.PackageVersion} into database...");

        _dbConnection.Insert(dbStructures.Values, insertPrimaryKey: true);
        _logger.LogInformation($" <<< added {dbStructures.Count} Structures");

        _dbConnection.Insert(dbElements.Values, insertPrimaryKey: true);
        _logger.LogInformation($" <<< added {dbElements.Count} Elements");

        _dbConnection.Insert(dbCollatedTypes, insertPrimaryKey: true);
        _logger.LogInformation($" <<< added {dbCollatedTypes.Count} Collated Element Types");

        _dbConnection.Insert(dbElementTypes, insertPrimaryKey: true);
        _logger.LogInformation($" <<< added {dbElementTypes.Count} Discrete Element Types");

        _dbConnection.Insert(dbAdditionalBindings, insertPrimaryKey: true);
        _logger.LogInformation($" <<< added {dbAdditionalBindings.Count} Additional Bindings");

        int affectedRows;

        // after all the records are inserted, execute any remaining key-resolution queries
        affectedRows = _dbConnection.UpdateCollatedTypeStructureKeys(pm.Key);
        _logger.LogInformation($" <<< updated {affectedRows} Collated Type Structure Keys");

        affectedRows = _dbConnection.UpdateElementTypeStructureKeys(pm.Key);
        _logger.LogInformation($" <<< updated {affectedRows} Element Type Structure Keys");

        affectedRows = _dbConnection.UpdateElementBaseKeys(pm.Key);
        _logger.LogInformation($" <<< updated {affectedRows} Element Base Keys");

        return;

        // TODO(ginoc): For now, exclude extensions, profiles, and logical models - we will want them for generic packages, but do not care for core
        (IEnumerable<StructureDefinition> structures, FhirArtifactClassEnum cgClass)[] getStructures(DefinitionCollection dc) => [
            (dc.PrimitiveTypesByName.Values, FhirArtifactClassEnum.PrimitiveType),
            (dc.ComplexTypesByName.Values, FhirArtifactClassEnum.ComplexType),
            (dc.ResourcesByName.Values, FhirArtifactClassEnum.Resource),
            //(dc.ExtensionsByUrl.Values, FhirArtifactClassEnum.Extension),
            //(dc.ProfilesByUrl.Values, FhirArtifactClassEnum.Profile),
            //(dc.LogicalModelsByUrl.Values, FhirArtifactClassEnum.LogicalModel),
            ];

        string literalForType(string? typeName, string? typeProfile, string? targetProfile) =>
            (string.IsNullOrEmpty(typeName) ? string.Empty : typeName) +
            (string.IsNullOrEmpty(typeProfile) ? string.Empty : $"[{typeProfile}]") +
            (string.IsNullOrEmpty(targetProfile) ? string.Empty : $"({targetProfile})");
        
        void addElementAndTypes(
            DbStructureDefinition dbStructure,
            StructureDefinition sd,
            ElementDefinition ed)
        {
            int elementKey = DbElement.GetIndex();

            // check for children
            int childCount = sd.cgElements(
                ed.Path,
                topLevelOnly: true,
                includeRoot: false,
                skipSlices: true).Count();

            Dictionary<string, DbCollatedType> currentCollatedTypes = [];
            List<DbElementType> currentElementTypes = [];
            Dictionary<string, List<string>> literalAccumulator = [];

            int? bindingVsKey = ed.Binding?.ValueSet == null
                ? null
                : (DbValueSet.SelectSingle(_dbConnection, FhirPackageKey: pm.Key, UnversionedUrl: ed.Binding?.ValueSet)?.Key
                  ?? DbValueSet.SelectSingle(_dbConnection, FhirPackageKey: pm.Key, VersionedUrl: ed.Binding?.ValueSet)?.Key);

            IEnumerable<ElementDefinition.TypeRefComponent> definedTypes = ed.Type.Select(tr => tr.cgAsR5());
            foreach (ElementDefinition.TypeRefComponent tr in definedTypes)
            {
                string typeName = tr.cgName();

                if (!currentCollatedTypes.TryGetValue(typeName, out DbCollatedType? collatedType))
                {
                    collatedType = new()
                    {
                        Key = DbCollatedType.GetIndex(),
                        FhirPackageKey = dbStructure.FhirPackageKey,
                        StructureKey = dbStructure.Key,
                        ElementKey = elementKey,
                        TypeName = typeName,
                        CollatedLiteral = string.Empty,
                        TypeStructureKey = null,
                    };
                    currentCollatedTypes.Add(typeName, collatedType);
                    dbCollatedTypes.Add(collatedType);
                }
                if (!literalAccumulator.TryGetValue(collatedType.TypeName, out List<string>? literalComponents))
                {
                    literalComponents = [];
                    literalAccumulator.Add(collatedType.TypeName, literalComponents);
                }

                if ((tr.ProfileElement.Count == 0) &&
                    (tr.TargetProfileElement.Count == 0))
                {
                    string tl = literalForType(tr.cgName(), null, null);

                    DbElementType et = new()
                    {
                        Key = DbElementType.GetIndex(),
                        FhirPackageKey = dbStructure.FhirPackageKey,
                        StructureKey = dbStructure.Key,
                        ElementKey = elementKey,
                        CollatedTypeKey = collatedType.Key,
                        TypeName = typeName,
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
                            Key = DbElementType.GetIndex(),
                            FhirPackageKey = dbStructure.FhirPackageKey,
                            StructureKey = dbStructure.Key,
                            ElementKey = elementKey,
                            CollatedTypeKey = collatedType.Key,
                            TypeName = typeName,
                            TypeProfile = null,
                            TargetProfile = tp.Value,
                            TypeStructureKey = null,
                        };
                        currentElementTypes.Add(et);
                        dbElementTypes.Add(et);
                        literalComponents.Add(tp.Value);
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
                            Key = DbElementType.GetIndex(),
                            FhirPackageKey = dbStructure.FhirPackageKey,
                            StructureKey = dbStructure.Key,
                            ElementKey = elementKey,
                            CollatedTypeKey = collatedType.Key,
                            TypeName = typeName,
                            TypeProfile = p.Value,
                            TargetProfile = null,
                            TypeStructureKey = null,
                        };
                        currentElementTypes.Add(et);
                        dbElementTypes.Add(et);
                        literalComponents.Add(p.Value);
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
                            Key = DbElementType.GetIndex(),
                            FhirPackageKey = dbStructure.FhirPackageKey,
                            StructureKey = dbStructure.Key,
                            ElementKey = elementKey,
                            CollatedTypeKey = collatedType.Key,
                            TypeName = typeName,
                            TypeProfile = p.Value,
                            TargetProfile = tp.Value,
                            TypeStructureKey = null,
                        };
                        currentElementTypes.Add(et);
                        dbElementTypes.Add(et);
                        literalComponents.Add($"{p.Value}[{tp.Value}]");
                    }
                }
            }

            if (currentElementTypes.Count == 0)
            {
                if (ed.ElementId == sd.Id)
                {
                    string tl = literalForType(sd.Id, null, null);
                    if (!currentCollatedTypes.TryGetValue(tl, out DbCollatedType? collatedType))
                    {
                        collatedType = new()
                        {
                            Key = DbCollatedType.GetIndex(),
                            FhirPackageKey = dbStructure.FhirPackageKey,
                            StructureKey = dbStructure.Key,
                            ElementKey = elementKey,
                            TypeName = tl,
                            CollatedLiteral = string.Empty,
                            TypeStructureKey = null,
                        };
                        currentCollatedTypes.Add(tl, collatedType);
                        dbCollatedTypes.Add(collatedType);
                    }

                    DbElementType et = new()
                    {
                        Key = DbElementType.GetIndex(),
                        FhirPackageKey = dbStructure.FhirPackageKey,
                        StructureKey = dbStructure.Key,
                        ElementKey = elementKey,
                        CollatedTypeKey = collatedType.Key,
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
                    if (!currentCollatedTypes.TryGetValue(tl, out DbCollatedType? collatedType))
                    {
                        collatedType = new()
                        {
                            Key = DbCollatedType.GetIndex(),
                            FhirPackageKey = dbStructure.FhirPackageKey,
                            StructureKey = dbStructure.Key,
                            ElementKey = elementKey,
                            TypeName = tl,
                            CollatedLiteral = string.Empty,
                            TypeStructureKey = null,
                        };
                        currentCollatedTypes.Add(tl, collatedType);
                        dbCollatedTypes.Add(collatedType);
                    }

                    DbElementType et = new()
                    {
                        Key = DbElementType.GetIndex(),
                        FhirPackageKey = dbStructure.FhirPackageKey,
                        StructureKey = dbStructure.Key,
                        ElementKey = elementKey,
                        CollatedTypeKey = collatedType.Key,
                        TypeName = btn,
                        TypeProfile = null,
                        TargetProfile = null,
                        TypeStructureKey = null,
                    };
                    currentElementTypes.Add(et);
                    dbElementTypes.Add(et);
                }
            }

            int additionalBindingCount = 0;

            // check for additional bindings
            if (ed.Binding?.Additional.Count > 0)
            {
                foreach (ElementDefinition.AdditionalComponent additional in ed.Binding.Additional)
                {
                    DbElementAdditionalBinding dbAdditionalBinding = new()
                    {
                        Key = DbElementAdditionalBinding.GetIndex(),
                        FhirPackageKey = pm.Key,
                        StructureKey = dbStructure.Key,
                        ElementKey = elementKey,
                        FhirKey = null,     // TODO: R6 added additional.Key
                        Purpose = additional.Purpose,
                        BindingValueSet = additional.ValueSet,
                        BindingValueSetKey = string.IsNullOrEmpty(additional.ValueSet) ? null : DbValueSet.SelectSingle(_dbConnection, FhirPackageKey: pm.Key, UnversionedUrl: additional.ValueSet)?.Key,
                        Documentation = additional.Documentation.ProcessCoreTextForLinks(fhirVersionLiteral),
                        ShortDocumentation = additional.ShortDoco.ProcessCoreTextForLinks(fhirVersionLiteral),
                        CollatedUsageContexts = additional.Usage.Count == 0
                            ? null
                            : string.Join(", ", additional.Usage.Select(uc => uc.Code.System + "#" + uc.Code.Code + ": `" + uc.Value.ToString() + "`")),
                        SatisfiedBySingleRepetition = additional.Any,
                    };
                    dbAdditionalBindings.Add(dbAdditionalBinding);
                    additionalBindingCount++;
                }
            }

            bool isInherited = ed.cgIsInherited(sd);
            string? basePath = ed.Base?.Path;

            List<string> completeLiteralComponents = [];
            // build our collated type literals
            foreach ((string typeName, DbCollatedType collatedType) in currentCollatedTypes)
            {
                if (!literalAccumulator.TryGetValue(typeName, out List<string>? literalComponents))
                {
                    literalComponents = [];
                    literalAccumulator.Add(typeName, literalComponents);
                }

                if (literalComponents.Count == 0)
                {
                    collatedType.CollatedLiteral = typeName; // no components, just the type name
                }
                else
                {
                    // multiple components, sort and join them
                    collatedType.CollatedLiteral = typeName + "(" + string.Join(", ", literalComponents.OrderBy(lc => lc)) + ")";
                }

                completeLiteralComponents.Add(collatedType.CollatedLiteral);
            }

            string typeGroupLiteral = string.Join(", ", completeLiteralComponents.Order());

            List<string> completeTargetLiterals = [];
            foreach (DbElementType currentElementType in currentElementTypes)
            {
                if (currentElementType.TargetProfile is null)
                {
                    continue;
                }

                completeLiteralComponents.Add(currentElementType.TargetProfile);
            }

            int resourceFieldOrder = ed.cgFieldOrder();
            int? parentElementDbKey = null;

            if (resourceFieldOrder != 0)
            {
                string parentKey = dbStructure.Key.ToString() + ":" + ed.ElementId.Substring(0, ed.ElementId.LastIndexOf('.'));
                if (dbElements.TryGetValue(parentKey, out DbElement? parentElement))
                {
                    parentElementDbKey = parentElement.Key;
                }
            }

            DbElement dbElement = new()
            {
                Key = elementKey,
                FhirPackageKey = pm.Key,
                StructureKey = dbStructure.Key,
                ParentElementKey = parentElementDbKey,
                ResourceFieldOrder = resourceFieldOrder,
                ComponentFieldOrder = ed.cgComponentFieldOrder(),
                Id = ed.ElementId,
                Path = ed.Path,
                ChildElementCount = childCount,
                Name = ed.cgName(),
                Short = ed.Short.ProcessCoreTextForLinks(fhirVersionLiteral),
                Definition = ed.Definition.ProcessCoreTextForLinks(fhirVersionLiteral),
                MinCardinality = ed.cgCardinalityMin(),
                MaxCardinality = ed.cgCardinalityMax(),
                MaxCardinalityString = ed.Max ?? "*",
                SliceName = ed.SliceName,
                ValueSetBindingStrength = ed.Binding?.Strength,
                BindingValueSet = ed.Binding?.ValueSet,
                BindingValueSetKey = bindingVsKey,
                BindingDescription = ed.Binding?.Description,
                AdditionalBindingCount = additionalBindingCount,
                FullCollatedTypeLiteral = typeGroupLiteral,
                FullCollatedReferenceTypesLiteral = completeTargetLiterals.Count == 0
                    ? null
                    : string.Join(", ", completeTargetLiterals.Order()),
                IsInherited = isInherited,
                BasePath = basePath,
                BaseElementKey = null,
                BaseStructureKey = null,
                IsSimpleType = ed.cgIsSimple(),
                IsModifier = ed.IsModifier == true,
                IsModifierReason = ed.IsModifierReason,
            };

            dbElements.Add(dbStructure.Key.ToString() + ":" + ed.ElementId, dbElement);
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
        // Do not vdRelationship this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
