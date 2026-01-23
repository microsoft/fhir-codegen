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

    private IDbConnection _db;
    private PackageLoader? _loader = null;
    private List<DbFhirPackage> _packages = [];

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

        _db = new SqliteConnection(connectionString);
        _db.Open();

        initNewDb(true);

        _packages = DbFhirPackage.SelectList(_db, orderByProperties: [nameof(DbFhirPackage.PackageVersion)]);
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

        _db = new SqliteConnection(connectionString);
        _db.Open();

        DbContentClasses.LoadIndices(_db);

        _packages = DbFhirPackage.SelectList(_db, orderByProperties: [nameof(DbFhirPackage.PackageVersion)]);
    }

    public bool IsCoreComparison => _isCoreComparison;
    public bool IsVersionComparison => _isVersionComparison;
    public string DbFileName => _dbName;
    public string DbFilePath => _dbPath;

    public IDbConnection DbConnection => _db;

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
        DbContentClasses.DropTables(
            _db,
            forTerminologies: artifactFilter is null || artifactFilter == FhirArtifactClassEnum.ValueSet,
            forStructures: artifactFilter is null | artifactFilter == FhirArtifactClassEnum.Resource);

        DbContentClasses.CreateTables(
            _db,
            forTerminologies: artifactFilter is null || artifactFilter == FhirArtifactClassEnum.ValueSet,
            forStructures: artifactFilter is null | artifactFilter == FhirArtifactClassEnum.Resource);

        // copy contents of each type
        copyContents(sourceConnection, _db, artifactFilter);

        // update our current index values
        DbContentClasses.LoadIndices(_db);

        return true;
    }

    [Obsolete("There is no reason to have source databases any more, will remove once confirmed.")]
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
                    //DbCollatedType.Insert(targetDb, DbCollatedType.SelectList(sourceDb), ignoreDuplicates: true, insertPrimaryKey: true);
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
                    //FhirPackageComparisonPair.Insert(targetDb, FhirPackageComparisonPair.SelectList(sourceDb), ignoreDuplicates: true, insertPrimaryKey: true);

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
                    //DbCollatedType.Insert(targetDb, DbCollatedType.SelectList(sourceDb), ignoreDuplicates: true, insertPrimaryKey: true);
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

        substitutions.Insert(_db, insertPrimaryKey: true);
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

        inclusions.Insert(_db, insertPrimaryKey: true);
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
            DbContentClasses.DropTables(_db);
        }

        // create all our tables (non-destructive)
        DbContentClasses.CreateTables(_db);

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
            if (DbFhirPackage.SelectSingle(_db, PackageId: dc.MainPackageId, PackageVersion: dc.MainPackageVersion) is not DbFhirPackage pm)
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

                _db.Insert(pm);
            }
        }

        loadKnownSubstitutions();
        loadKnownExternalInclusions();
    }


#if false
    private void checkInversions()
    {
        // iterate over packages to use as source
        for (int i = 0; i < _packages.Count; i++)
        {
            DbFhirPackage sourcePackage = _packages[i];

            // iterate upward over packages to use as target
            for (int j = i + 1; j < _packages.Count; j++)
            {
                DbFhirPackage targetPackage = _packages[j];
                checkInversionsVs(sourcePackage, targetPackage);
                checkInversionsSd(sourcePackage, targetPackage);
            }

            // iterate downward over packages to use as target
            for (int j = i - 1; j >= 0; j--)
            {
                DbFhirPackage targetPackage = _packages[j];
                checkInversionsVs(sourcePackage, targetPackage);
                checkInversionsSd(sourcePackage, targetPackage);
            }
        }
    }


    private void checkInversionsSd(
        DbFhirPackage sourcePackage,
        DbFhirPackage targetPackage)
    {
        DbRecordCache<DbStructureMappingRecord> sdMappingCache = new();
        DbRecordCache<DbElementMappingRecord> elementMappingCache = new();

        // get the invertable structure mappings for this package pair
        List<DbStructureMappingRecord> sdMappings = DbStructureMappingRecord.SelectList(
            _db,
            SourceFhirPackageKey: sourcePackage.Key,
            TargetFhirPackageKey: targetPackage.Key,
            TargetStructureKeyIsNull: false);

        // iterate over the structure mappings and check for an inverse mapping
        foreach (DbStructureMappingRecord sdMapping in sdMappings)
        {
            // check for an inverse mapping
            DbStructureMappingRecord? inverseMapping = DbStructureMappingRecord.SelectSingle(
                _db,
                SourceFhirPackageKey: targetPackage.Key,
                TargetFhirPackageKey: sourcePackage.Key,
                SourceStructureKey: sdMapping.TargetStructureKey,
                TargetStructureKey: sdMapping.SourceStructureKey);

            if (inverseMapping is null)
            {
                createInverseMappings(
                    sourcePackage,
                    targetPackage,
                    sdMapping,
                    sdMappingCache,
                    elementMappingCache);
            }
        }

        // apply changes
        if (sdMappingCache.ToAddCount > 0)
        {
            _logger.LogInformation($"Adding {sdMappingCache.ToAddCount} inverse structure mappings from {targetPackage.ShortName} to {sourcePackage.ShortName}");
            sdMappingCache.ToAdd.Insert(_db, ignoreDuplicates: true, insertPrimaryKey: true);
        }

        if (sdMappingCache.ToUpdateCount > 0)
        {
            _logger.LogInformation($"Updating {sdMappingCache.ToUpdateCount} inverse structure mappings from {targetPackage.ShortName} to {sourcePackage.ShortName}");
            sdMappingCache.ToUpdate.Update(_db);
        }

        if (elementMappingCache.ToAddCount > 0)
        {
            _logger.LogInformation($"Adding {elementMappingCache.ToAddCount} inverse element mappings from {sourcePackage.ShortName} to {targetPackage.ShortName}");
            elementMappingCache.ToAdd.Insert(_db, ignoreDuplicates: true, insertPrimaryKey: true);
        }

        if (elementMappingCache.ToUpdateCount > 0)
        {
            _logger.LogInformation($"Updating {elementMappingCache.ToUpdateCount} inverse element mappings from {sourcePackage.ShortName} to {targetPackage.ShortName}");
            elementMappingCache.ToUpdate.Update(_db);
        }
    }

    private void createInverseMappings(
        DbFhirPackage sourcePackage,
        DbFhirPackage targetPackage,
        DbStructureMappingRecord sdMapping,
        DbRecordCache<DbStructureMappingRecord> sdMappingCache,
        DbRecordCache<DbElementMappingRecord> elementMappingCache)
    {
        DbStructureDefinition sourceSd = DbStructureDefinition.SelectSingle(
            _db,
            Key: sdMapping.SourceStructureKey)
            ?? throw new Exception($"Source Structure with key {sdMapping.SourceStructureKey} not found!");

        DbStructureDefinition targetSd = DbStructureDefinition.SelectSingle(
            _db,
            Key: sdMapping.TargetStructureKey!.Value)
            ?? throw new Exception($"Target Structure with key {sdMapping.TargetStructureKey} not found!");

        // build the ID for the structure map
        (string vsIdLong, string vsIdShort) = XVerProcessor.GenerateArtifactId(
            targetPackage.ShortName,
            targetSd.Id,
            sourcePackage.ShortName,
            sourceSd.Id);

        // create the inverse structure mapping record
        DbStructureMappingRecord inverseSdMapping = new()
        {
            Key = DbStructureMappingRecord.GetIndex(),
            PreviousStepMapRecordKey = sdMapping.PriorContentKeyFromArrayInverted,
            Steps = sdMapping.Steps,

            SourceFhirPackageKey = targetPackage.Key,
            SourceFhirSequence = targetPackage.DefinitionFhirSequence,
            SourceStructureKey = targetSd.Key,
            SourceStructureId = targetSd.Id,

            TargetFhirPackageKey = sourcePackage.Key,
            TargetFhirSequence = sourcePackage.DefinitionFhirSequence,
            TargetStructureKey = sourceSd.Key,
            TargetStructureId = sourceSd.Id,

            ContentKeys = sdMapping.ContentKeysInverted,

            ExplicitNoMap = false,
            Relationship = invertRelationship(sdMapping.Relationship),

            ConceptMapUrl = null,
            FmlExists = false,
            FmlUrl = null,
            FmlFilename = null,

            IdLong = vsIdLong,
            IdShort = vsIdShort,
            Url = $"http://hl7.org/fhir/{sourcePackage.FhirVersionShort}/ConceptMap/{vsIdLong}",
            Name = FhirSanitizationUtils.ReformatIdForName(vsIdLong),
            Title = $"Concept Map of FHIR {sourcePackage.ShortName} Structure `{sourceSd.VersionedUrl}`" +
                $" to FHIR {targetPackage.ShortName} Structure `{targetSd.VersionedUrl}`," +
                $" Created as an inverse of mapping of `{sdMapping.IdLong}` (`{sdMapping.Url}`)",

            TechnicalNotes = $"This structure mapping was auto-generated as the inverse of the mapping `{sdMapping.IdLong}`" +
                $" (`{sdMapping.Url}`) from FHIR {sourcePackage.ShortName} to FHIR {targetPackage.ShortName}.",
        };

        sdMappingCache.CacheAdd(inverseSdMapping);

        // invert the element mappings
        List<DbElementMappingRecord> elementMappings = DbElementMappingRecord.SelectList(
            _db,
            StructureMappingKey: sdMapping.Key);
        foreach (DbElementMappingRecord elementMapping in elementMappings)
        {
            if (elementMapping.TargetElementKey is null)
            {
                // no-map element, skip
                continue;
            }

            DbElementMappingRecord inverseElementMapping = new()
            {
                Key = DbElementMappingRecord.GetIndex(),
                PreviousStepMapRecordKey = elementMapping.PriorContentKeyFromArrayInverted,
                Steps = elementMapping.Steps,
                StructureMappingKey = inverseSdMapping.Key,

                SourceFhirPackageKey = inverseSdMapping.SourceFhirPackageKey,
                SourceFhirSequence = inverseSdMapping.SourceFhirSequence,
                SourceElementKey = elementMapping.TargetElementKey.Value,
                SourceElementId = elementMapping.TargetElementId!,

                TargetFhirPackageKey = inverseSdMapping.TargetFhirPackageKey,
                TargetFhirSequence = inverseSdMapping.TargetFhirSequence,
                TargetElementKey = elementMapping.SourceElementKey,
                TargetElementId = elementMapping.SourceElementId,

                Relationship = invertRelationship(elementMapping.Relationship),

                ExplicitNoMap = false,

                TechnicalNotes = $"This element mapping was auto-generated as the inverse of the mapping of element" +
                    $" `{elementMapping.SourceElementId}` to `{elementMapping.TargetElementId}`" +
                    $" from FHIR {sourcePackage.ShortName} to FHIR {targetPackage.ShortName}.",
            };
            elementMappingCache.CacheAdd(inverseElementMapping);
        }
    }

    private void checkInversionsVs(
        DbFhirPackage sourcePackage,
        DbFhirPackage targetPackage)
    {
        DbRecordCache<DbValueSetMappingRecord> vsMappingCache = new();
        DbRecordCache<DbValueSetConceptMappingRecord> conceptMappingCache = new();

        // get the invertable value set mappings for this package pair
        List<DbValueSetMappingRecord> vsMappings = DbValueSetMappingRecord.SelectList(
            _db,
            SourceFhirPackageKey: sourcePackage.Key,
            TargetFhirPackageKey: targetPackage.Key,
            TargetValueSetKeyIsNull: false);

        // iterate over the value set mappings and check for an inverse mapping
        foreach (DbValueSetMappingRecord vsMapping in vsMappings)
        {
            // check for an inverse mapping
            DbValueSetMappingRecord? inverseMapping = DbValueSetMappingRecord.SelectSingle(
                _db,
                SourceFhirPackageKey: targetPackage.Key,
                TargetFhirPackageKey: sourcePackage.Key,
                SourceValueSetKey: vsMapping.TargetValueSetKey,
                TargetValueSetKey: vsMapping.SourceValueSetKey);

            if (inverseMapping is null)
            {
                createInverseMappings(
                    sourcePackage,
                    targetPackage,
                    vsMapping,
                    vsMappingCache,
                    conceptMappingCache);
            }
        }

        // apply changes
        if (vsMappingCache.ToAddCount > 0)
        {
            _logger.LogInformation($"Adding {vsMappingCache.ToAddCount} inverse value set mappings from {targetPackage.ShortName} to {sourcePackage.ShortName}");
            vsMappingCache.ToAdd.Insert(_db, ignoreDuplicates: true, insertPrimaryKey: true);
        }

        if (vsMappingCache.ToUpdateCount > 0)
        {
            _logger.LogInformation($"Updating {vsMappingCache.ToUpdateCount} inverse value set mappings from {targetPackage.ShortName} to {sourcePackage.ShortName}");
            vsMappingCache.ToUpdate.Update(_db);
        }

        if (conceptMappingCache.ToAddCount > 0)
        {
            _logger.LogInformation($"Adding {conceptMappingCache.ToAddCount} inverse value set concept mappings from {sourcePackage.ShortName} to {targetPackage.ShortName}");
            conceptMappingCache.ToAdd.Insert(_db, ignoreDuplicates: true, insertPrimaryKey: true);
        }

        if (conceptMappingCache.ToUpdateCount > 0)
        {
            _logger.LogInformation($"Updating {conceptMappingCache.ToUpdateCount} inverse value set concept mappings from {sourcePackage.ShortName} to {targetPackage.ShortName}");
            conceptMappingCache.ToUpdate.Update(_db);
        }
    }

    private void createInverseMappings(
        DbFhirPackage sourcePackage,
        DbFhirPackage targetPackage,
        DbValueSetMappingRecord vsMapping,
        DbRecordCache<DbValueSetMappingRecord> vsMappingCache,
        DbRecordCache<DbValueSetConceptMappingRecord> conceptMappingCache)
    {
        DbValueSet sourceVs = DbValueSet.SelectSingle(
            _db,
            Key: vsMapping.SourceValueSetKey)
            ?? throw new Exception($"Source ValueSet with key {vsMapping.SourceValueSetKey} not found!");

        DbValueSet targetVs = DbValueSet.SelectSingle(
            _db,
            Key: vsMapping.TargetValueSetKey!.Value)
            ?? throw new Exception($"Target ValueSet with key {vsMapping.TargetValueSetKey} not found!");

        // build the ID for the value set map
        (string vsIdLong, string vsIdShort) = XVerProcessor.GenerateArtifactId(
            targetPackage.ShortName,
            targetVs.Id,
            sourcePackage.ShortName,
            sourceVs.Id);


        // create the inverse value set mapping record
        DbValueSetMappingRecord inverseVsMapping = new()
        {
            Key = DbValueSetMappingRecord.GetIndex(),
            PreviousStepMapRecordKey = vsMapping.PriorContentKeyFromArrayInverted,
            Steps = vsMapping.Steps,

            SourceFhirPackageKey = targetPackage.Key,
            SourceFhirSequence = targetPackage.DefinitionFhirSequence,
            SourceValueSetKey = targetVs.Key,
            SourceValueSetId = targetVs.Id,

            TargetFhirPackageKey = sourcePackage.Key,
            TargetFhirSequence = sourcePackage.DefinitionFhirSequence,
            TargetValueSetKey = sourceVs.Key,
            TargetValueSetId = sourceVs.Id,

            ContentKeys = vsMapping.ContentKeysInverted,

            ExplicitNoMap = false,
            Relationship = invertRelationship(vsMapping.Relationship),

            OriginatingConceptMapUrlsLiteral = null,

            IdLong = vsIdLong,
            IdShort = vsIdShort,
            Url = $"http://hl7.org/fhir/{sourcePackage.FhirVersionShort}/ConceptMap/{vsIdLong}",
            Name = FhirSanitizationUtils.ReformatIdForName(vsIdLong),
            Title = $"Concept Map of FHIR {sourcePackage.ShortName} Value Set `{sourceVs.VersionedUrl}`" +
                $" to FHIR {targetPackage.ShortName} Value Set `{targetVs.VersionedUrl}`," +
                $" Created as an inverse of mapping of `{vsMapping.IdLong}` (`{vsMapping.Url}`)",

            TechnicalNotes = $"This value set mapping was auto-generated as the inverse of the mapping `{vsMapping.IdLong}`" +
                $" (`{vsMapping.Url}`) from FHIR {sourcePackage.ShortName} to FHIR {targetPackage.ShortName}.",
        };

        vsMappingCache.CacheAdd(inverseVsMapping);

        // invert the concept mappings
        List<DbValueSetConceptMappingRecord> conceptMappings = DbValueSetConceptMappingRecord.SelectList(
            _db,
            ValueSetMappingKey: vsMapping.Key);
        foreach (DbValueSetConceptMappingRecord conceptMapping in conceptMappings)
        {
            if (conceptMapping.TargetValueSetConceptKey is null)
            {
                // no-map concept, skip
                continue;
            }

            DbValueSetConceptMappingRecord inverseConceptMapping = new()
            {
                Key = DbValueSetConceptMappingRecord.GetIndex(),
                PreviousStepMapRecordKey = conceptMapping.PriorContentKeyFromArrayInverted,
                Steps = conceptMapping.Steps,
                ValueSetMappingKey = inverseVsMapping.Key,

                SourceFhirPackageKey = inverseVsMapping.SourceFhirPackageKey,
                SourceFhirSequence = inverseVsMapping.SourceFhirSequence,
                SourceValueSetConceptKey = conceptMapping.TargetValueSetConceptKey.Value,

                TargetFhirPackageKey = inverseVsMapping.TargetFhirPackageKey,
                TargetFhirSequence = inverseVsMapping.TargetFhirSequence,
                TargetValueSetConceptKey = conceptMapping.SourceValueSetConceptKey,

                Relationship = invertRelationship(conceptMapping.Relationship),

                ExplicitNoMap = false,

                TechnicalNotes = $"This concept mapping was auto-generated as the inverse of the mapping of concept" +
                    $" `{conceptMapping.SourceValueSetConceptKey}` to `{conceptMapping.TargetValueSetConceptKey}`" +
                    $" from FHIR {sourcePackage.ShortName} to FHIR {targetPackage.ShortName}.",
            };
            conceptMappingCache.CacheAdd(inverseConceptMapping);
        }
    }

    private CMR? invertRelationship(CMR? relationship) => relationship switch
    {
        CMR.Equivalent => CMR.Equivalent,
        CMR.SourceIsBroaderThanTarget => CMR.SourceIsNarrowerThanTarget,
        CMR.SourceIsNarrowerThanTarget => CMR.SourceIsBroaderThanTarget,
        _ => relationship,
    };
#endif

#if false
    private void addMissingElementMappings()
    {
        // iterate over packages to use as source
        for (int i = 0; i < _packages.Count; i++)
        {
            DbFhirPackage sourcePackage = _packages[i];

            // iterate upward over packages to use as target
            for (int j = i + 1; j < _packages.Count; j++)
            {
                DbFhirPackage targetPackage = _packages[j];
                addMissingElementMappings(sourcePackage, targetPackage);
            }

            // iterate downward over packages to use as target
            for (int j = i - 1; j >= 0; j--)
            {
                DbFhirPackage targetPackage = _packages[j];
                addMissingElementMappings(sourcePackage, targetPackage);
            }
        }
    }

    private void addMissingElementMappings(
        DbFhirPackage sourcePackage,
        DbFhirPackage targetPackage)
    {
        DbRecordCache<DbElementMappingRecord> elementMappingCache = new();

        // get the structure mappings for this structure set to the target package
        List<DbStructureMappingRecord> sdMappings = DbStructureMappingRecord.SelectList(
            _db,
            SourceFhirPackageKey: sourcePackage.Key,
            TargetFhirPackageKey: targetPackage.Key);

        // iterate over the structure mappings
        foreach (DbStructureMappingRecord sdMapping in sdMappings)
        {
            // resolve the source structure
            DbStructureDefinition sourceSd = DbStructureDefinition.SelectSingle(
                _db,
                Key: sdMapping.SourceStructureKey)
                ?? throw new Exception($"Source Structure with key {sdMapping.SourceStructureKey} not found!");

            // get the source elements for this structure mapping
            List<DbElement> sourceElements = DbElement.SelectList(
                _db,
                StructureKey: sdMapping.SourceStructureKey);

            // get the element mappings for this structure mapping
            List<DbElementMappingRecord> elementMappings = DbElementMappingRecord.SelectList(
                _db,
                StructureMappingKey: sdMapping.Key);

            ILookup<int?, DbElementMappingRecord> elementMappingsBySourceKey = elementMappings
                .ToLookup(c => c.SourceElementKey);

            // if there is no target, these are all no-maps
            if (sdMapping.TargetStructureKey is null)
            {
                // iterate over the source concepts
                foreach (DbElement sourceElement in sourceElements)
                {
                    // check to see if there is at least one mapping record
                    if (elementMappingsBySourceKey.Contains(sourceElement.Key))
                    {
                        // at least one mapping exists, nothing to do
                        continue;
                    }

                    // create the no-map record
                    elementMappingCache.CacheAdd(createMappingRecord(
                        sourcePackage,
                        targetPackage,
                        sdMapping,
                        sourceSd,
                        null,
                        sourceElement,
                        null,
                        null));
                }

                continue;
            }

            // resolve the target structure
            DbStructureDefinition targetSd = DbStructureDefinition.SelectSingle(
                _db,
                Key: sdMapping.TargetStructureKey)
                ?? throw new Exception($"Target Structure with key {sdMapping.TargetStructureKey} not found!");

            // resolve the target elements
            Dictionary<int, DbElement> targetElementsByKey = DbElement.SelectDict(
                _db,
                StructureKey: sdMapping.TargetStructureKey);

            ILookup<string, DbElement> targetElementsById = targetElementsByKey
                .Values
                .ToLookup(c => c.Id);

            int sourceIdPrefixLen = sourceSd.Name.Length;
            int targetIdPrefixLen = targetSd.Name.Length;

            // iterate over the source elements
            foreach (DbElement sourceElement in sourceElements)
            {
                // check to see if there is at least one mapping record
                if (elementMappingsBySourceKey.Contains(sourceElement.Key))
                {
                    // at least one mapping exists, nothing to do
                    continue;
                }

                // check for a full match
                if (targetElementsById.Contains(sourceElement.Id))
                {
                    foreach (DbElement targetElement in targetElementsById[sourceElement.Id])
                    {
                        elementMappingCache.CacheAdd(createMappingRecord(
                            sourcePackage,
                            targetPackage,
                            sdMapping,
                            sourceSd,
                            targetSd,
                            sourceElement,
                            targetElement,
                            CMR.Equivalent));
                    }
                    continue;
                }

                string updatedName = targetSd.Name + sourceElement.Id[sourceIdPrefixLen..];

                // check for a name-replaced id match
                if (targetElementsById.Contains(updatedName))
                {
                    List<DbElement> matches = targetElementsById[updatedName].ToList();

                    foreach (DbElement targetElement in matches)
                    {
                        elementMappingCache.CacheAdd(createMappingRecord(
                            sourcePackage,
                            targetPackage,
                            sdMapping,
                            sourceSd,
                            targetSd,
                            sourceElement,
                            targetElement,
                            matches.Count == 1 ? CMR.Equivalent : CMR.SourceIsBroaderThanTarget));
                    }
                    continue;
                }

                // check to see if there is a parent we can work from
                if (sdMapping.PreviousStepMapRecordKey is not null)
                {
                    // get the prior step mapping for this source element
                    List<DbElementMappingRecord> priorMappings = DbElementMappingRecord.SelectList(
                        _db,
                        StructureMappingKey: sdMapping.PreviousStepMapRecordKey.Value,
                        SourceElementKey: sourceElement.Key);

                    bool processed = false;

                    if (priorMappings.Count > 0)
                    {
                        foreach (DbElementMappingRecord priorMapping in priorMappings)
                        {
                            if (priorMapping.TargetElementKey is null)
                            {
                                // no target in prior mapping, so no-map here as well
                                elementMappingCache.CacheAdd(createMappingRecord(
                                    sourcePackage,
                                    targetPackage,
                                    sdMapping,
                                    sourceSd,
                                    targetSd,
                                    sourceElement,
                                    null,
                                    relationship: null,
                                    priorStepKey: priorMapping.Key));

                                processed = true;
                                continue;
                            }

                            // resolve the prior element
                            DbElement priorTargetElement = DbElement.SelectSingle(
                                _db,
                                Key: priorMapping.TargetElementKey.Value)
                                ?? throw new Exception($"Target Element with key {priorMapping.TargetElementKey.Value} not found!");

                            // check for a full match
                            if (targetElementsById.Contains(priorTargetElement.Id))
                            {
                                foreach (DbElement targetElement in targetElementsById[priorTargetElement.Id])
                                {
                                    elementMappingCache.CacheAdd(createMappingRecord(
                                        sourcePackage,
                                        targetPackage,
                                        sdMapping,
                                        sourceSd,
                                        targetSd,
                                        sourceElement,
                                        targetElement,
                                        priorMapping.Relationship ?? CMR.Equivalent,
                                        priorMapping.Key));
                                }

                                processed = true;
                                continue;
                            }

                            updatedName = targetSd.Name + priorTargetElement.Id[priorTargetElement.Id.IndexOf('.')..];

                            // check for a code match
                            if (targetElementsById.Contains(updatedName))
                            {
                                List<DbElement> matches = targetElementsById[updatedName].ToList();

                                foreach (DbElement targetElement in matches)
                                {
                                    elementMappingCache.CacheAdd(createMappingRecord(
                                        sourcePackage,
                                        targetPackage,
                                        sdMapping,
                                        sourceSd,
                                        targetSd,
                                        sourceElement,
                                        targetElement,
                                        priorMapping.Relationship ??
                                            (matches.Count == 1 ? CMR.Equivalent : CMR.SourceIsBroaderThanTarget),
                                        priorMapping.Key));
                                }

                                processed = true;
                                continue;
                            }
                        }
                    }

                    // if we did something, do not fall-through
                    if (processed)
                    {
                        continue;
                    }
                }

                // no match found, create no-map record
                elementMappingCache.CacheAdd(createMappingRecord(
                    sourcePackage,
                    targetPackage,
                    sdMapping,
                    sourceSd,
                    targetSd,
                    sourceElement,
                    null,
                    relationship: null,
                    priorStepKey: null));
            }
        }

        // apply changes
        if (elementMappingCache.ToAddCount > 0)
        {
            _logger.LogInformation($"Adding {elementMappingCache.ToAddCount} element mappings from {sourcePackage.ShortName} to {targetPackage.ShortName}");
            elementMappingCache.ToAdd.Insert(_db, ignoreDuplicates: true, insertPrimaryKey: true);
        }

        if (elementMappingCache.ToUpdateCount > 0)
        {
            _logger.LogInformation($"Updating {elementMappingCache.ToUpdateCount} element mappings from {sourcePackage.ShortName} to {targetPackage.ShortName}");
            elementMappingCache.ToUpdate.Update(_db);
        }
    }

    private DbElementMappingRecord createMappingRecord(
        DbFhirPackage sourcePackage,
        DbFhirPackage targetPackage,
        DbStructureMappingRecord sdMappingRecord,
        DbStructureDefinition sourceSd,
        DbStructureDefinition? targetSd,
        DbElement sourceElement,
        DbElement? targetElement,
        CMR? relationship = null,
        int? priorStepKey = null)
    {
        string comments;

        if (targetElement is null)
        {
            if (targetSd is null)
            {
                comments = $"The element `{sourceElement.Id}` ({sourceElement.Short}) from" +
                    $" Structure `{sourceSd.VersionedUrl}` in" +
                    $" FHIR {sourcePackage.ShortName} has no representation in" +
                    $" FHIR {targetPackage.ShortName}";
            }
            else
            {
                comments = $"The element `{sourceElement.Id}` ({sourceElement.Short}) from" +
                    $" Structure `{sourceSd.VersionedUrl}` in" +
                    $" FHIR {sourcePackage.ShortName} has no representation in" +
                    $" Value Set `{targetSd.VersionedUrl}` from" +
                    $" FHIR {targetPackage.ShortName}";
            }
        }
        else
        {
            comments = $"The element `{sourceElement.Id}` ({sourceElement.Short}) from" +
                $" Structure `{sourceSd.VersionedUrl}` in" +
                $" FHIR {sourcePackage.ShortName} maps to" +
                $" `{targetElement.Id}`" +
                $" Structure `{targetSd!.VersionedUrl}` from" +
                $" FHIR {targetPackage.ShortName}";
        }

        return new()
        {
            Key = DbElementMappingRecord.GetIndex(),
            PreviousStepMapRecordKey = priorStepKey,
            Steps = Math.Abs(sourcePackage.DefinitionFhirSequence - targetPackage.DefinitionFhirSequence),

            SourceFhirPackageKey = sourcePackage.Key,
            SourceFhirSequence = sourcePackage.DefinitionFhirSequence,
            SourceElementKey = sourceElement.Key,
            SourceElementId = sourceElement.Id,

            TargetFhirPackageKey = targetPackage.Key,
            TargetFhirSequence = targetPackage.DefinitionFhirSequence,
            TargetElementKey = targetElement?.Key,
            TargetElementId = targetElement?.Id,

            StructureMappingKey = sdMappingRecord.Key,
            ContentKeys = getKeyArray(sourcePackage, targetPackage, sourceElement.Key, targetElement?.Key),

            ExplicitNoMap = false,
            Comments = comments,
            TechnicalNotes = "Auto-generated",

            Relationship = relationship,
        };
    }
#endif

#if false
    private void addMissingConceptMappings()
    {
        // we want to process closer versions first, so we do a stepped approach
        for (int stepSize = 1; stepSize < _packages.Count; stepSize++)
        {
            for (int i = 0; i < _packages.Count - stepSize; i++)
            {
                DbFhirPackage sourcePackage = _packages[i];
                DbFhirPackage targetPackage = _packages[i + stepSize];

                // ascending
                _logger.LogInformation($"Adding missing concepts for {sourcePackage.ShortName} -> {targetPackage.ShortName}");
                addMissingConceptMappings(sourcePackage, targetPackage);

                // descending
                _logger.LogInformation($"Adding missing concepts for {targetPackage.ShortName} -> {sourcePackage.ShortName}");
                addMissingConceptMappings(sourcePackage, targetPackage);
            }
        }

        //// iterate over packages to use as source
        //for (int i = 0; i < _packages.Count; i++)
        //{
        //    DbFhirPackage sourcePackage = _packages[i];

        //    // iterate upward over packages to use as target
        //    for (int j = i + 1; j < _packages.Count; j++)
        //    {
        //        DbFhirPackage targetPackage = _packages[j];
        //        addMissingConceptMappings(sourcePackage, targetPackage);
        //    }

        //    // iterate downward over packages to use as target
        //    for (int j = i - 1; j >= 0; j--)
        //    {
        //        DbFhirPackage targetPackage = _packages[j];
        //        addMissingConceptMappings(sourcePackage, targetPackage);
        //    }
        //}
    }

    private void addMissingConceptMappings(
        DbFhirPackage sourcePackage,
        DbFhirPackage targetPackage)
    {
        DbRecordCache<DbValueSetConceptMappingRecord> conceptMappingCache = new();

        // get the value set mappings for this source value set to the target package
        List<DbValueSetMappingRecord> vsMappings = DbValueSetMappingRecord.SelectList(
            _db,
            SourceFhirPackageKey: sourcePackage.Key,
            TargetFhirPackageKey: targetPackage.Key);

        // iterate over the value set mappings
        foreach (DbValueSetMappingRecord vsMapping in vsMappings)
        {
            // resolve the source vs
            DbValueSet sourceVs = DbValueSet.SelectSingle(
                _db,
                Key: vsMapping.SourceValueSetKey)
                ?? throw new Exception($"Source ValueSet with key {vsMapping.SourceValueSetKey} not found!");

            // get the source concepts for this value set mapping
            List<DbValueSetConcept> sourceConcepts = DbValueSetConcept.SelectList(
                _db,
                ValueSetKey: vsMapping.SourceValueSetKey);

            // get the concept mappings for this value set mapping
            List<DbValueSetConceptMappingRecord> conceptMappings = DbValueSetConceptMappingRecord.SelectList(
                _db,
                ValueSetMappingKey: vsMapping.Key);

            ILookup<int, DbValueSetConceptMappingRecord> conceptMappingsBySourceKey = conceptMappings
                .ToLookup(c => c.SourceValueSetConceptKey);

            // if there is no target, these are all no-maps
            if (vsMapping.TargetValueSetKey is null)
            {
                // iterate over the source concepts
                foreach (DbValueSetConcept sourceConcept in sourceConcepts)
                {
                    // check to see if there is at least one mapping record
                    if (conceptMappingsBySourceKey.Contains(sourceConcept.Key))
                    {
                        // at least one mapping exists, nothing to do
                        continue;
                    }

                    // create the no-map record
                    conceptMappingCache.CacheAdd(createMappingRecord(
                        sourcePackage,
                        targetPackage,
                        vsMapping,
                        sourceVs,
                        null,
                        sourceConcept,
                        null,
                        null));
                }

                continue;
            }

            // resolve the target value set
            DbValueSet targetVs = DbValueSet.SelectSingle(
                _db,
                Key: vsMapping.TargetValueSetKey)
                ?? throw new Exception($"Target ValueSet with key {vsMapping.TargetValueSetKey} not found!");

            // resolve the target value set concepts
            Dictionary<int, DbValueSetConcept> targetConceptsByKey = DbValueSetConcept.SelectDict(
                _db,
                ValueSetKey: vsMapping.TargetValueSetKey);

            ILookup<string, DbValueSetConcept> targetConceptsByFhirKey = targetConceptsByKey
                .Values
                .ToLookup(c => c.FhirKey);

            ILookup<string, DbValueSetConcept> targetConceptsByCode = targetConceptsByKey
                .Values
                .ToLookup(c => c.Code);

            // iterate over the source concepts
            foreach (DbValueSetConcept sourceConcept in sourceConcepts)
            {
                // check to see if there is at least one mapping record
                if (conceptMappingsBySourceKey.Contains(sourceConcept.Key))
                {
                    // at least one mapping exists, nothing to do
                    continue;
                }

                // check for a full match
                if (targetConceptsByFhirKey.Contains(sourceConcept.FhirKey))
                {
                    foreach (DbValueSetConcept targetConcept in targetConceptsByFhirKey[sourceConcept.FhirKey])
                    {
                        conceptMappingCache.CacheAdd(createMappingRecord(
                            sourcePackage,
                            targetPackage,
                            vsMapping,
                            sourceVs,
                            targetVs,
                            sourceConcept,
                            targetConcept,
                            CMR.Equivalent));
                    }
                    continue;
                }

                // check for a code match
                if (targetConceptsByCode.Contains(sourceConcept.Code))
                {
                    List<DbValueSetConcept> matches = targetConceptsByCode[sourceConcept.Code].ToList();

                    foreach (DbValueSetConcept targetConcept in matches)
                    {
                        conceptMappingCache.CacheAdd(createMappingRecord(
                            sourcePackage,
                            targetPackage,
                            vsMapping,
                            sourceVs,
                            targetVs,
                            sourceConcept,
                            targetConcept,
                            matches.Count == 1 ? CMR.Equivalent : CMR.SourceIsBroaderThanTarget));
                    }
                    continue;
                }

                // check to see if there is a parent we can work from
                if (vsMapping.PreviousStepMapRecordKey is not null)
                {
                    // get the prior step mapping for this source concept
                    List<DbValueSetConceptMappingRecord> priorMappings = DbValueSetConceptMappingRecord.SelectList(
                        _db,
                        ValueSetMappingKey: vsMapping.PreviousStepMapRecordKey.Value,
                        SourceValueSetConceptKey: sourceConcept.Key);

                    bool processed = false;

                    if (priorMappings.Count > 0)
                    {
                        foreach (DbValueSetConceptMappingRecord priorMapping in priorMappings)
                        {
                            if (priorMapping.TargetValueSetConceptKey is null)
                            {
                                // no target in prior mapping, so no-map here as well
                                conceptMappingCache.CacheAdd(createMappingRecord(
                                    sourcePackage,
                                    targetPackage,
                                    vsMapping,
                                    sourceVs,
                                    targetVs,
                                    sourceConcept,
                                    null,
                                    relationship: null,
                                    priorStepKey: priorMapping.Key));

                                processed = true;
                                continue;
                            }

                            // resolve the prior concept
                            DbValueSetConcept priorTargetConcept = DbValueSetConcept.SelectSingle(
                                _db,
                                Key: priorMapping.TargetValueSetConceptKey.Value)
                                ?? throw new Exception($"Target ValueSetConcept with key {priorMapping.TargetValueSetConceptKey.Value} not found!");

                            // check for a full match
                            if (targetConceptsByFhirKey.Contains(priorTargetConcept.FhirKey))
                            {
                                foreach (DbValueSetConcept targetConcept in targetConceptsByFhirKey[priorTargetConcept.FhirKey])
                                {
                                    conceptMappingCache.CacheAdd(createMappingRecord(
                                        sourcePackage,
                                        targetPackage,
                                        vsMapping,
                                        sourceVs,
                                        targetVs,
                                        sourceConcept,
                                        targetConcept,
                                        priorMapping.Relationship ?? CMR.Equivalent,
                                        priorMapping.Key));
                                }

                                processed = true;
                                continue;
                            }

                            // check for a code match
                            if (targetConceptsByCode.Contains(priorTargetConcept.Code))
                            {
                                List<DbValueSetConcept> matches = targetConceptsByCode[priorTargetConcept.Code].ToList();

                                foreach (DbValueSetConcept targetConcept in matches)
                                {
                                    conceptMappingCache.CacheAdd(createMappingRecord(
                                        sourcePackage,
                                        targetPackage,
                                        vsMapping,
                                        sourceVs,
                                        targetVs,
                                        sourceConcept,
                                        targetConcept,
                                        priorMapping.Relationship ??
                                            (matches.Count == 1 ? CMR.Equivalent : CMR.SourceIsBroaderThanTarget),
                                        priorMapping.Key));
                                }

                                processed = true;
                                continue;
                            }
                        }
                    }

                    // if we did something, do not fall-through
                    if (processed)
                    {
                        continue;
                    }
                }

                // no match found, create no-map record
                conceptMappingCache.CacheAdd(createMappingRecord(
                    sourcePackage,
                    targetPackage,
                    vsMapping,
                    sourceVs,
                    targetVs,
                    sourceConcept,
                    null,
                    relationship: null,
                    priorStepKey: null));
            }
        }

        // apply changes
        if (conceptMappingCache.ToAddCount > 0)
        {
            _logger.LogInformation($"Adding {conceptMappingCache.ToAddCount} value set concept mappings from {sourcePackage.ShortName} to {targetPackage.ShortName}");
            conceptMappingCache.ToAdd.Insert(_db, ignoreDuplicates: true, insertPrimaryKey: true);
        }

        if (conceptMappingCache.ToUpdateCount > 0)
        {
            _logger.LogInformation($"Updating {conceptMappingCache.ToUpdateCount} value set concept mappings from {sourcePackage.ShortName} to {targetPackage.ShortName}");
            conceptMappingCache.ToUpdate.Update(_db);
        }
    }

    private DbValueSetConceptMappingRecord createMappingRecord(
        DbFhirPackage sourcePackage,
        DbFhirPackage targetPackage,
        DbValueSetMappingRecord valueSetMappingRecord,
        DbValueSet sourceVs,
        DbValueSet? targetVs,
        DbValueSetConcept sourceConcept,
        DbValueSetConcept? targetConcept,
        CMR? relationship = null,
        int? priorStepKey = null)
    {
        string comments;

        if (targetConcept is null)
        {
            if (targetVs is null)
            {
                comments = $"The concept `{sourceConcept.FhirKey}` ({sourceConcept.Display}) from" +
                    $" Value Set `{sourceVs.VersionedUrl}` in" +
                    $" FHIR {sourcePackage.ShortName} has no representation in" +
                    $" FHIR {targetPackage.ShortName}";
            }
            else
            {
                comments = $"The concept `{sourceConcept.FhirKey}` ({sourceConcept.Display}) from" +
                    $" Value Set `{sourceVs.VersionedUrl}` from" +
                    $" FHIR {sourcePackage.ShortName} has no representation in" +
                    $" Value Set `{targetVs.VersionedUrl}` from" +
                    $" FHIR {targetPackage.ShortName}";
            }
        }
        else
        {
            comments = $"The concept `{sourceConcept.FhirKey}` ({sourceConcept.Display}) from" +
                $" Value Set `{sourceVs.VersionedUrl}` from" +
                $" FHIR {sourcePackage.ShortName} maps to" +
                $" `{targetConcept.FhirKey}`" +
                $" Value Set `{targetVs!.VersionedUrl}` from" +
                $" FHIR {targetPackage.ShortName}";
        }

        return new()
        {
            Key = DbValueSetConceptMappingRecord.GetIndex(),
            PreviousStepMapRecordKey = priorStepKey,
            Steps = Math.Abs(sourcePackage.DefinitionFhirSequence - targetPackage.DefinitionFhirSequence),

            SourceFhirPackageKey = sourcePackage.Key,
            SourceFhirSequence = sourcePackage.DefinitionFhirSequence,
            SourceValueSetConceptKey = sourceConcept.Key,

            TargetFhirPackageKey = targetPackage.Key,
            TargetFhirSequence = targetPackage.DefinitionFhirSequence,
            TargetValueSetConceptKey = targetConcept?.Key,
            ValueSetMappingKey = valueSetMappingRecord.Key,

            ContentKeys = getKeyArray(sourcePackage, targetPackage, sourceConcept.Key, targetConcept?.Key),

            ExplicitNoMap = false,
            Comments = comments,
            TechnicalNotes = "Auto-generated",

            Relationship = relationship,
        };
    }
#endif

#if false
    private void fillPriorStepsVs()
    {
        // iterate over packages to use as source
        for (int i = 0; i < _packages.Count; i++)
        {
            DbFhirPackage sourcePackage = _packages[i];

            // iterate upward over packages to use as target
            for (int j = i + 2; j < _packages.Count; j++)
            {
                DbFhirPackage targetPackage = _packages[j];
                DbFhirPackage priorTargetPackage = _packages[j - 1];

                fillPriorStepsVs(sourcePackage, targetPackage, priorTargetPackage);
            }

            // iterate downward over packages to use as target
            for (int j = i - 2; j >= 0; j--)
            {
                DbFhirPackage targetPackage = _packages[j];
                DbFhirPackage priorTargetPackage = _packages[j + 1];

                fillPriorStepsVs(sourcePackage, targetPackage, priorTargetPackage);
            }
        }
    }

    private void fillPriorStepsVs(
        DbFhirPackage sourcePackage,
        DbFhirPackage targetPackage,
        DbFhirPackage priorTargetPackage)
    {
        int steps = Math.Abs(targetPackage.DefinitionFhirSequence - sourcePackage.DefinitionFhirSequence);

        if (steps < 2)
        {
            // nothing to do
            return;
        }

        {
            string sourceColName = priorTargetPackage.DefinitionFhirSequence switch
            {
                FhirReleases.FhirSequenceCodes.DSTU2 => nameof(DbValueSetMappingRecord.ContentKeyR2),
                FhirReleases.FhirSequenceCodes.STU3 => nameof(DbValueSetMappingRecord.ContentKeyR3),
                FhirReleases.FhirSequenceCodes.R4 => nameof(DbValueSetMappingRecord.ContentKeyR4),
                FhirReleases.FhirSequenceCodes.R4B => nameof(DbValueSetMappingRecord.ContentKeyR4B),
                FhirReleases.FhirSequenceCodes.R5 => nameof(DbValueSetMappingRecord.ContentKeyR5),
                FhirReleases.FhirSequenceCodes.R6 => nameof(DbValueSetMappingRecord.ContentKeyR6),
                _ => throw new Exception($"Unsupported FHIR version code: {priorTargetPackage.DefinitionFhirSequence}"),
            };

            IDbCommand command = _db.CreateCommand();
            command.CommandText = $"""
            UPDATE {DbValueSetMappingRecord.DefaultTableName} 
            SET {nameof(DbValueSetMappingRecord.PreviousStepMapRecordKey)} = {sourceColName}
            WHERE {nameof(DbValueSetMappingRecord.TargetFhirPackageKey)} = {targetPackage.Key}
                AND {nameof(DbValueSetMappingRecord.PreviousStepMapRecordKey)} IS NULL
                AND Steps = {steps}
            """;

            command.ExecuteNonQuery();
        }

        {
            string sourceColName = priorTargetPackage.DefinitionFhirSequence switch
            {
                FhirReleases.FhirSequenceCodes.DSTU2 => nameof(DbValueSetConceptMappingRecord.ContentKeyR2),
                FhirReleases.FhirSequenceCodes.STU3 => nameof(DbValueSetConceptMappingRecord.ContentKeyR3),
                FhirReleases.FhirSequenceCodes.R4 => nameof(DbValueSetConceptMappingRecord.ContentKeyR4),
                FhirReleases.FhirSequenceCodes.R4B => nameof(DbValueSetConceptMappingRecord.ContentKeyR4B),
                FhirReleases.FhirSequenceCodes.R5 => nameof(DbValueSetConceptMappingRecord.ContentKeyR5),
                FhirReleases.FhirSequenceCodes.R6 => nameof(DbValueSetConceptMappingRecord.ContentKeyR6),
                _ => throw new Exception($"Unsupported FHIR version code: {priorTargetPackage.DefinitionFhirSequence}"),
            };

            IDbCommand command = _db.CreateCommand();
            command.CommandText = $"""
            UPDATE {DbValueSetConceptMappingRecord.DefaultTableName} 
            SET {nameof(DbValueSetConceptMappingRecord.PreviousStepMapRecordKey)} = {sourceColName}
            WHERE {nameof(DbValueSetConceptMappingRecord.TargetFhirPackageKey)} = {targetPackage.Key}
                AND {nameof(DbValueSetConceptMappingRecord.PreviousStepMapRecordKey)} IS NULL
                AND Steps = {steps}
            """;

            command.ExecuteNonQuery();
        }
    }
#endif

#if false
    private void fillPriorStepsSd()
    {
        // iterate over packages to use as source
        for (int i = 0; i < _packages.Count; i++)
        {
            DbFhirPackage sourcePackage = _packages[i];

            // iterate upward over packages to use as target
            for (int j = i + 2; j < _packages.Count; j++)
            {
                DbFhirPackage targetPackage = _packages[j];
                DbFhirPackage priorTargetPackage = _packages[j - 1];

                fillPriorStepsSd(sourcePackage, targetPackage, priorTargetPackage);
            }

            // iterate downward over packages to use as target
            for (int j = i - 2; j >= 0; j--)
            {
                DbFhirPackage targetPackage = _packages[j];
                DbFhirPackage priorTargetPackage = _packages[j + 1];

                fillPriorStepsSd(sourcePackage, targetPackage, priorTargetPackage);
            }
        }
    }

    private void fillPriorStepsSd(
        DbFhirPackage sourcePackage,
        DbFhirPackage targetPackage,
        DbFhirPackage priorTargetPackage)
    {
        int steps = Math.Abs(targetPackage.DefinitionFhirSequence - sourcePackage.DefinitionFhirSequence);

        if (steps < 2)
        {
            // nothing to do
            return;
        }

        {
            string sourceColName = priorTargetPackage.DefinitionFhirSequence switch
            {
                FhirReleases.FhirSequenceCodes.DSTU2 => nameof(DbStructureMappingRecord.ContentKeyR2),
                FhirReleases.FhirSequenceCodes.STU3 => nameof(DbStructureMappingRecord.ContentKeyR3),
                FhirReleases.FhirSequenceCodes.R4 => nameof(DbStructureMappingRecord.ContentKeyR4),
                FhirReleases.FhirSequenceCodes.R4B => nameof(DbStructureMappingRecord.ContentKeyR4B),
                FhirReleases.FhirSequenceCodes.R5 => nameof(DbStructureMappingRecord.ContentKeyR5),
                FhirReleases.FhirSequenceCodes.R6 => nameof(DbStructureMappingRecord.ContentKeyR6),
                _ => throw new Exception($"Unsupported FHIR version code: {priorTargetPackage.DefinitionFhirSequence}"),
            };

            IDbCommand command = _db.CreateCommand();
            command.CommandText = $"""
            UPDATE {DbStructureMappingRecord.DefaultTableName} 
            SET {nameof(DbStructureMappingRecord.PreviousStepMapRecordKey)} = {sourceColName}
            WHERE {nameof(DbStructureMappingRecord.TargetFhirPackageKey)} = {targetPackage.Key}
                AND {nameof(DbStructureMappingRecord.PreviousStepMapRecordKey)} IS NULL
                AND Steps = {steps}
            """;

            command.ExecuteNonQuery();
        }

        {
            string sourceColName = priorTargetPackage.DefinitionFhirSequence switch
            {
                FhirReleases.FhirSequenceCodes.DSTU2 => nameof(DbElementMappingRecord.ContentKeyR2),
                FhirReleases.FhirSequenceCodes.STU3 => nameof(DbElementMappingRecord.ContentKeyR3),
                FhirReleases.FhirSequenceCodes.R4 => nameof(DbElementMappingRecord.ContentKeyR4),
                FhirReleases.FhirSequenceCodes.R4B => nameof(DbElementMappingRecord.ContentKeyR4B),
                FhirReleases.FhirSequenceCodes.R5 => nameof(DbElementMappingRecord.ContentKeyR5),
                FhirReleases.FhirSequenceCodes.R6 => nameof(DbElementMappingRecord.ContentKeyR6),
                _ => throw new Exception($"Unsupported FHIR version code: {priorTargetPackage.DefinitionFhirSequence}"),
            };

            IDbCommand command = _db.CreateCommand();
            command.CommandText = $"""
            UPDATE {DbElementMappingRecord.DefaultTableName} 
            SET {nameof(DbElementMappingRecord.PreviousStepMapRecordKey)} = {sourceColName}
            WHERE {nameof(DbElementMappingRecord.TargetFhirPackageKey)} = {targetPackage.Key}
                AND {nameof(DbElementMappingRecord.PreviousStepMapRecordKey)} IS NULL
                AND Steps = {steps}
            """;

            command.ExecuteNonQuery();
        }
    }
#endif

#if false
    private void reconcileStructureNoMaps()
    {
        // get the list of all the structure mappings that have no target
        List<DbStructureMappingRecord> noMapRecords = DbStructureMappingRecord.SelectList(
            _db,
            TargetStructureKeyIsNull: true);

        // iterate over the no-maps to see if there is a derived mapping it needs to override
        foreach (DbStructureMappingRecord noMapRecord in noMapRecords)
        {
            List<DbStructureMappingRecord> otherMaps = DbStructureMappingRecord.SelectList(
                _db,
                SourceFhirPackageKey: noMapRecord.SourceFhirPackageKey,
                SourceStructureKey: noMapRecord.SourceStructureKey,
                TargetFhirPackageKey: noMapRecord.TargetFhirPackageKey,
                TargetStructureKeyIsNull: false);

            foreach (DbStructureMappingRecord otherMap in otherMaps)
            {
                // we are using the concept maps as the source of truth
                if ((noMapRecord.ConceptMapUrl is null) &&
                    (otherMap.ConceptMapUrl is not null))
                {
                    try
                    {
                        noMapRecord.Delete(_db);
                        DbElementMappingRecord.Delete(_db, StructureMappingKey: noMapRecord.Key);
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
                    otherMap.Delete(_db);
                    DbElementMappingRecord.Delete(_db, StructureMappingKey: otherMap.Key);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error deleting structure mapping record Key={otherMap.Key}: {ex.Message}");
                }
            }
        }
    }
#endif




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

        throw new NotImplementedException("This call is deprecated.");

#if false
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

            DbFhirPackage leftDbPackage = DbFhirPackage.SelectSingle(_db, PackageId: left.MainPackageId, PackageVersion: left.MainPackageVersion)
                ?? throw new Exception($"Package {left.MainPackageId}@{left.MainPackageVersion} was not found in the database!");
            DbFhirPackage rightDbPackage = DbFhirPackage.SelectSingle(_db, PackageId: right.MainPackageId, PackageVersion: right.MainPackageVersion)
                ?? throw new Exception($"Package {right.MainPackageId}@{right.MainPackageVersion} was not found in the database!");

            DbFhirPackageComparisonPair dbPairLtoR = DbFhirPackageComparisonPair.SelectSingle(
                _db,
                SourcePackageKey: leftDbPackage.Key,
                TargetPackageKey: rightDbPackage.Key)
                ?? throw new Exception($"Comparison {left.MainPackageId}@{left.MainPackageVersion} to {right.MainPackageId}@{right.MainPackageVersion} was not found in the database!");

            DbFhirPackageComparisonPair dbPairRtoL = DbFhirPackageComparisonPair.SelectSingle(
                _db,
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

        _db.Insert(vsComparisons.ToAdd, insertPrimaryKey: true);
        _logger.LogInformation($" <<< added {vsComparisons.Count} ValueSet Comparisons");

        _db.Insert(conceptComparisons.ToAdd, insertPrimaryKey: true);
        _logger.LogInformation($" <<< added {conceptComparisons.Count} ValueSet Concept Comparisons");

        _db.Insert(unresolvedConceptComparisons);
        _logger.LogInformation($" <<< added {unresolvedConceptComparisons.Count} Unresolved ValueSet Concept Comparisons");

        _db.Insert(sdComparisons.ToAdd, insertPrimaryKey: true);
        _logger.LogInformation($" <<< added {sdComparisons.Count} Structure Comparisons");

        _db.Insert(unresolvedSdComparisons, insertPrimaryKey: true);
        _logger.LogInformation($" <<< added {unresolvedSdComparisons.Count} Unresolved Structure Comparisons");

        _db.Insert(elementComparisons.ToAdd, insertPrimaryKey: true);
        _logger.LogInformation($" <<< added {elementComparisons.Count} Element Comparisons");

        _db.Insert(collatedTypeComparisons.ToAdd, insertPrimaryKey: true);
        _logger.LogInformation($" <<< added {collatedTypeComparisons.Count} Collated Type Comparisons");

        _db.Insert(typeComparisons.ToAdd, insertPrimaryKey: true);
        _logger.LogInformation($" <<< added {typeComparisons.Count} Type Comparisons");

        _db.Insert(unresolvedElementComparisons, insertPrimaryKey: true);
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
                    _db,
                    FhirPackageKey: sourceDbPackage.Key,
                    Id: sourceSd.Id)
                    ?? throw new Exception($"Source structure {sourceSd.Id} not found in package: {sourceDbPackage.Key} ({sourceDbPackage.Name})!");

                DbStructureDefinition targetDbSd = DbStructureDefinition.SelectSingle(
                    _db,
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
                        SourceStructureKey = sourceDbSd.Key,
                        SourceCanonicalVersioned = sourceDbSd.VersionedUrl,
                        SourceCanonicalUnversioned = sourceDbSd.UnversionedUrl,
                        SourceVersion = sourceDbSd.Version,
                        SourceName = sourceDbSd.Name,
                        TargetFhirPackageKey = targetDbPackage.Key,
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
                _db,
                FhirPackageKey: sourceDbPackage.Key,
                VersionedUrl: sourceVersioned);

            DbStructureDefinition? targetDbSd = (targetVersioned == null)
                ? null
                : DbStructureDefinition.SelectSingle(
                    _db,
                    FhirPackageKey: targetDbPackage.Key,
                    VersionedUrl: targetVersioned);

            DbStructureComparison? sdComparison = null;
            DbStructureComparison? inverseSdComparison = null;

            // if we have a source and a target, get or make a comparison record
            if ((sourceDbSd is not null) &&
                (targetDbSd is not null))
            {
                inverseSdComparison = DbStructureComparison.SelectSingle(
                    _db,
                    PackageComparisonKey: dbPackagePair.Key,
                    SourceStructureKey: targetDbSd.Key,
                    TargetStructureKey: sourceDbSd.Key);

                if (inverseSdComparison == null)
                {
                    // check the cache
                    _ = sdComparisons.TryGet((targetDbSd.Key, sourceDbSd.Key), out inverseSdComparison);
                }

                sdComparison = DbStructureComparison.SelectSingle(
                    _db,
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
                _db,
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
                            _db,
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
                                _db,
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
                _db,
                Key: dbPackagePair.SourcePackageKey)
                ?? throw new Exception($"Source package {dbPackagePair.SourcePackageKey} not found in the database!");

            DbFhirPackage targetDbPackage = DbFhirPackage.SelectSingle(
                _db,
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
                CollatedTypeComparisonKey = null,
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
                        _db,
                        FhirPackageKey: sourceDbPackage.Key,
                        Id: groupSource.Code);

                    // iterate over the target types
                    foreach (ConceptMap.TargetElementComponent groupTarget in groupSource.Target)
                    {
                        DbStructureDefinition? targetDbSd = DbStructureDefinition.SelectSingle(
                            _db,
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
                _db,
                FhirPackageKey: sourceDbPackage.Key,
                UnversionedUrl: sourceScopeCanonical.Uri)
                ?? throw new Exception($"Could not find {sourceScopeCanonical.Uri} in {sourceDbPackage.PackageId}@{sourceDbPackage.PackageVersion}");

            DbValueSet targetDbVs = DbValueSet.SelectSingle(
                _db,
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
                CodeLiteralsAreIdentical = null,
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
                        _db,
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
                            _db,
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
                    CodeLiteralsAreIdentical = null,
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
                CodeLiteralsAreIdentical = null,
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
#endif
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
            DbFhirPackage pm = DbFhirPackage.SelectSingle(_db, PackageId: dc.MainPackageId, PackageVersion: dc.MainPackageVersion)
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
        expandContentReferenceElements();

        // do code-system post-processing
        doCodeSystemPostProcessing();

        return true;
    }

    private void doCodeSystemPostProcessing()
    {
        DbFhirPackage? r5 = DbFhirPackage.SelectSingle(
            _db,
            PackageId: "hl7.fhir.r5.core");
        
        if (r5 != null)
        {
            IDbCommand command = _db.CreateCommand();
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

            command = _db.CreateCommand();
            command.CommandText =
                $"delete from {DbCodeSystemConcept.DefaultTableName}" +
                $" where {nameof(DbCodeSystemConcept.CodeSystemKey)} not in (select {nameof(DbCodeSystem.Key)} from {DbCodeSystem.DefaultTableName})";
            command.ExecuteNonQuery();

            command = _db.CreateCommand();
            command.CommandText =
                $"delete from {DbCodeSystemFilter.DefaultTableName}" +
                $" where {nameof(DbCodeSystemFilter.CodeSystemKey)} not in (select {nameof(DbCodeSystem.Key)} from {DbCodeSystem.DefaultTableName})";
            command.ExecuteNonQuery();

            command = _db.CreateCommand();
            command.CommandText =
                $"delete from {DbCodeSystemPropertyDefinition.DefaultTableName}" +
                $" where {nameof(DbCodeSystemPropertyDefinition.CodeSystemKey)} not in (select {nameof(DbCodeSystem.Key)} from {DbCodeSystem.DefaultTableName})";
            command.ExecuteNonQuery();

            command = _db.CreateCommand();
            command.CommandText =
                $"delete from {DbCodeSystemConceptProperty.DefaultTableName}" +
                $" where {nameof(DbCodeSystemConceptProperty.CodeSystemPropertyDefinitionKey)}" +
                $" not in (select {nameof(DbCodeSystemPropertyDefinition.Key)} from {DbCodeSystemPropertyDefinition.DefaultTableName})";
            command.ExecuteNonQuery();
        }

        return;
    }

    private void expandContentReferenceElements()
    {
        // update elements that use content references to expand out their child elements
        DbContentWithIdCache<DbStructureDefinition> sdCache = new();
        DbContentWithIdCache<DbElement> edCache = new();
        
        // iterate over resources (need to handle individually since we are inserting elements and need to update the resource field order)
        List<DbStructureDefinition> structures = DbStructureDefinition.SelectList(
            _db,
            ArtifactClass: FhirArtifactClassEnum.Resource);
        foreach (DbStructureDefinition dbSd in structures)
        {
            // get the elements for this structure
            List<DbElement> elements = DbElement.SelectList(
                _db,
                StructureKey: dbSd.Key,
                orderByProperties: [nameof(DbElement.ResourceFieldOrder)]);

            int resourceFieldOrderAdjustment = 0;

            foreach (DbElement dbEd in elements)
            {
                // if we have an adjustment, need to apply it
                if (resourceFieldOrderAdjustment != 0)
                {
                    dbEd.ResourceFieldOrder += resourceFieldOrderAdjustment;
                    edCache.CacheUpdate(dbEd);
                }

                // if this is not a content reference, we are done
                if (!dbEd.DefinedAsContentReference)
                {
                    continue;
                }

                // do NOT nest into recursive content references
                if (dbEd.Id.StartsWith(dbEd.FullCollatedTypeLiteral))
                {
                    continue;
                }

                // get all the elements that start at the content reference location
                List<DbElement> crElements = DbElement.SelectList(
                    _db,
                    FhirPackageKey: dbEd.FhirPackageKey,
                    StructureKey: dbEd.StructureKey,
                    Id: dbEd.FullCollatedTypeLiteral + "%",
                    compareStringsWithLike: true,
                    orderByProperties: [nameof(DbElement.ResourceFieldOrder)]);

                if (crElements.Count == 0)
                {
                    throw new Exception($"Could not find content reference elements for {dbEd.Id} in structure {dbSd.Name} ({dbSd.VersionedUrl})!");
                }

                DbElement crElement = crElements[0];

                // update properties on the original element to match the content reference base element
                dbEd.ChildElementCount = crElement.ChildElementCount;
                dbEd.FullCollatedTypeLiteral = crElement.FullCollatedTypeLiteral;

                edCache.CacheUpdate(dbEd);

                Dictionary<int, int> parentKeyMappingLookup = [];
                parentKeyMappingLookup[crElement.Key] = dbEd.Key;

                int prefixLen = crElement.Id.Length;

                // iterate over the elements to create copies with the correct paths (skip the actual content reference element)
                foreach (DbElement crEd in crElements.Skip(1))
                {
                    resourceFieldOrderAdjustment++;
                    dbSd.SnapshotCount++;
                    dbSd.DifferentialCount++;

                    int key = DbElement.GetIndex();
                    parentKeyMappingLookup[crEd.Key] = key;

                    int? mappedParentKey = (crEd.ParentElementKey is not null) && parentKeyMappingLookup.TryGetValue(crEd.ParentElementKey.Value, out int value)
                        ? value
                        : null;

                    // can update this object and just insert since we are not changing the original
                    crEd.Key = key;
                    crEd.Id = dbEd.Id + crEd.Id[prefixLen..];
                    crEd.Path = dbEd.Path + crEd.Path[prefixLen..];
                    crEd.ResourceFieldOrder = dbEd.ResourceFieldOrder + resourceFieldOrderAdjustment;
                    crEd.ParentElementKey = mappedParentKey;

                    edCache.CacheAdd(crEd);
                }
            }

            if (resourceFieldOrderAdjustment != 0)
            {
                sdCache.CacheUpdate(dbSd);
            }
        }

        // commit our changes
        if (sdCache.ToUpdateCount > 0)
        {
            DbStructureDefinition.Update(_db, sdCache.ToUpdate);
            _logger.LogInformation($"Expanded content reference elements in {sdCache.ToUpdateCount} structure definitions.");
        }

        if (edCache.ToUpdateCount > 0)
        {
            DbElement.Update(_db, edCache.ToUpdate);
            _logger.LogInformation($"Expanded content reference elements updated {edCache.ToUpdateCount} existing elements.");
        }

        if (edCache.ToAddCount > 0)
        {
            DbElement.Insert(_db, edCache.ToAdd, ignoreDuplicates: true, insertPrimaryKey: true);
            _logger.LogInformation($"Expanded content reference elements with {edCache.ToAddCount} new elements.");
        }
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
                _db,
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

        _db.Insert(dbCodeSystems, insertPrimaryKey: true);
        _logger.LogInformation($" <<< added {dbCodeSystems.Count} CodeSystems");

        _db.Insert(dbCodeSystemFilters, insertPrimaryKey: true);
        _logger.LogInformation($" <<< added {dbCodeSystemFilters.Count} CodeSystem Filters");

        _db.Insert(dbCodeSystemPropertyDefinitions, insertPrimaryKey: true);
        _logger.LogInformation($" <<< added {dbCodeSystemPropertyDefinitions.Count} CodeSystem Property Definitions");

        _db.Insert(allDbConcepts, insertPrimaryKey: true);
        _logger.LogInformation($" <<< added {allDbConcepts.Count} CodeSystem Concepts");

        _db.Insert(allDbConceptProperties, insertPrimaryKey: true);
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
        // link code systems to value sets where possible
        {
            IDbCommand command = _db.CreateCommand();
            command.CommandText = $"""
                UPDATE {DbValueSetConcept.DefaultTableName} 
                SET {nameof(DbValueSetConcept.CodeSystemConceptKey)} = (
                    SELECT csc.{nameof(DbCodeSystemConcept.Key)}
                    FROM {DbCodeSystem.DefaultTableName} cs
                    JOIN {DbCodeSystemConcept.DefaultTableName} csc ON cs.{nameof(DbCodeSystem.Key)} = csc.{nameof(DbCodeSystemConcept.CodeSystemKey)}
                    WHERE {DbValueSetConcept.DefaultTableName}.{nameof(DbValueSetConcept.FhirPackageKey)} = cs.{nameof(DbCodeSystem.FhirPackageKey)}
                      AND {DbValueSetConcept.DefaultTableName}.{nameof(DbValueSetConcept.System)} = cs.{nameof(DbCodeSystem.UnversionedUrl)}
                      AND {DbValueSetConcept.DefaultTableName}.{nameof(DbValueSetConcept.SystemVersion)} = cs.{nameof(DbCodeSystem.Version)}
                      AND {DbValueSetConcept.DefaultTableName}.{nameof(DbValueSetConcept.Code)} = csc.{nameof(DbCodeSystemConcept.Code)}
                )
                WHERE {DbValueSetConcept.DefaultTableName}.{nameof(DbValueSetConcept.CodeSystemConceptKey)} IS NULL
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

        // because we have the wrong version of THO (correct version no longer exists), we need to remove some duplicate definitions
        {
            // get the list of value sets rooted in THO that are not bound to structures
            List<DbValueSet> thoVsList = DbValueSet.SelectList(
                _db,
                UnversionedUrl: "http://terminology.hl7.org/ValueSet/%",
                BindingCountExtended: 0,
                compareStringsWithLike: true);

            List<DbValueSet> toDelete = [];

            // traverse the list of THO value sets and see if there is a matching HL7.org value set
            foreach (DbValueSet thoVs in thoVsList)
            {
                DbValueSet? hl7Vs = DbValueSet.SelectSingle(
                    _db,
                    FhirPackageKey: thoVs.FhirPackageKey,
                    Id: thoVs.Id,
                    UnversionedUrl: "http://hl7.org/fhir/ValueSet/%",
                    compareStringsWithLike: true);

                if (hl7Vs is not null)
                {
                    toDelete.Add(thoVs);
                }
            }

            // delete the offending duplicate value sets
            if (toDelete.Count > 0)
            {
                _logger.LogInformation($"Removing {toDelete.Count} duplicate THO-rooted value sets that have HL7.org counterparts...");
                DbValueSet.Delete(_db, toDelete);

                // also need to delete associated concepts
                IDbCommand command = _db.CreateCommand();
                command.CommandText = $"""
                delete from {DbValueSetConcept.DefaultTableName}
                where {nameof(DbValueSetConcept.ValueSetKey)} not in
                (
                  select distinct {nameof(DbValueSet.Key)}
                  from {DbValueSet.DefaultTableName}
                )
                """;

                int count = command.ExecuteNonQuery();
                _logger.LogInformation($" Removed {count} associated ValueSetConcept records.");
            }
        }

        // mark value sets that contain escape valve codes
        {
            IDbCommand command = _db.CreateCommand();
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

        // fix known system URL issues
        {
            IDbCommand command = _db.CreateCommand();
            command.CommandText = $"""
                update {DbValueSetConcept.DefaultTableName}
                set {nameof(DbValueSetConcept.System)} = 'http://hl7.org/fhir/sample-security-structural-roles'
                where {nameof(DbValueSetConcept.System)} = 'sample-security-structural-roles'
                and {nameof(DbValueSetConcept.SystemVersion)} = '5.0.0'
                """;

            command.ExecuteNonQuery();
        }

        // fill in missing display values from code systems
        {
            // update display values from code systems where possible
            IDbCommand command = _db.CreateCommand();
            command.CommandText = $"""
                UPDATE {DbValueSetConcept.DefaultTableName} 
                SET {nameof(DbValueSetConcept.Display)} = (
                    SELECT csc.{nameof(DbCodeSystemConcept.Display)}
                    FROM {DbCodeSystemConcept.DefaultTableName} csc
                    WHERE {DbValueSetConcept.DefaultTableName}.{nameof(DbValueSetConcept.CodeSystemConceptKey)} = csc.{nameof(DbCodeSystemConcept.Key)}
                )
                WHERE {DbValueSetConcept.DefaultTableName}.{nameof(DbValueSetConcept.Display)} IS NULL
                  AND {DbValueSetConcept.DefaultTableName}.{nameof(DbValueSetConcept.CodeSystemConceptKey)} IS NOT NULL
                """;

            command.ExecuteNonQuery();

            //IDbCommand command = _db.CreateCommand();
            //command.CommandText = $"""
            //    UPDATE {DbValueSetConcept.DefaultTableName} 
            //    SET {nameof(DbValueSetConcept.Display)} = (
            //        SELECT csc.{nameof(DbCodeSystemConcept.Display)}
            //        FROM {DbCodeSystem.DefaultTableName} cs
            //        JOIN {DbCodeSystemConcept.DefaultTableName} csc ON cs.{nameof(DbCodeSystem.Key)} = csc.{nameof(DbCodeSystemConcept.CodeSystemKey)}
            //        WHERE {DbValueSetConcept.DefaultTableName}.{nameof(DbValueSetConcept.FhirPackageKey)} = cs.{nameof(DbCodeSystem.FhirPackageKey)}
            //          AND {DbValueSetConcept.DefaultTableName}.{nameof(DbValueSetConcept.System)} = cs.{nameof(DbCodeSystem.UnversionedUrl)}
            //          AND {DbValueSetConcept.DefaultTableName}.{nameof(DbValueSetConcept.SystemVersion)} = cs.{nameof(DbCodeSystem.Version)}
            //          AND {DbValueSetConcept.DefaultTableName}.{nameof(DbValueSetConcept.Code)} = csc.{nameof(DbCodeSystemConcept.Code)}
            //    )
            //    WHERE {DbValueSetConcept.DefaultTableName}.{nameof(DbValueSetConcept.Display)} IS NULL
            //      AND EXISTS (
            //        SELECT 1 
            //        FROM {DbCodeSystem.DefaultTableName} cs
            //        JOIN {DbCodeSystemConcept.DefaultTableName} csc ON cs.{nameof(DbCodeSystem.Key)} = csc.{nameof(DbCodeSystemConcept.CodeSystemKey)}
            //        WHERE {DbValueSetConcept.DefaultTableName}.{nameof(DbValueSetConcept.FhirPackageKey)} = cs.{nameof(DbCodeSystem.FhirPackageKey)}
            //          AND {DbValueSetConcept.DefaultTableName}.{nameof(DbValueSetConcept.System)} = cs.{nameof(DbCodeSystem.UnversionedUrl)}
            //          AND {DbValueSetConcept.DefaultTableName}.{nameof(DbValueSetConcept.SystemVersion)} = cs.{nameof(DbCodeSystem.Version)}
            //          AND {DbValueSetConcept.DefaultTableName}.{nameof(DbValueSetConcept.Code)} = csc.{nameof(DbCodeSystemConcept.Code)}
            //      )
            //    """;

            //command.ExecuteNonQuery();
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
                _db,
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
                if (DbValueSetConcept.SelectSingle(_db, FhirPackageKey: pm.Key, ValueSetKey: dbVs.Key, System: fc.System, Code: fc.Code) != null)
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

        _db.Insert(dbValueSets, insertPrimaryKey: true);
        _logger.LogInformation($" <<< added {dbValueSets.Count} ValueSets");

        _db.Insert(allDbConcepts, insertPrimaryKey: true);
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

        _db.Insert(dbStructures.Values, insertPrimaryKey: true);
        _logger.LogInformation($" <<< added {dbStructures.Count} Structures");

        _db.Insert(dbElements.Values, insertPrimaryKey: true);
        _logger.LogInformation($" <<< added {dbElements.Count} Elements");

        _db.Insert(dbElementTypes, insertPrimaryKey: true);
        _logger.LogInformation($" <<< added {dbElementTypes.Count} Discrete Element Types");

        _db.Insert(dbAdditionalBindings, insertPrimaryKey: true);
        _logger.LogInformation($" <<< added {dbAdditionalBindings.Count} Additional Bindings");

        int affectedRows;

        // after all the records are inserted, execute any remaining key-resolution queries
        //affectedRows = _db.UpdateCollatedTypeStructureKeys(pm.Key);
        //_logger.LogInformation($" <<< updated {affectedRows} Collated Type Structure Keys");

        affectedRows = _db.UpdateElementTypeStructureKeys(pm.Key);
        _logger.LogInformation($" <<< updated {affectedRows} Element Type Structure Keys");

        affectedRows = _db.UpdateElementTypeProfileStructureKeys(pm.Key);
        _logger.LogInformation($" <<< updated {affectedRows} Element Type Profile Structure Keys");

        affectedRows = _db.UpdateElementTargetProfileStructureKeys(pm.Key);
        _logger.LogInformation($" <<< updated {affectedRows} Element Target Profile Structure Keys");

        affectedRows = _db.UpdateElementBaseKeys(pm.Key);
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

            List<DbElementType> currentElementTypes = [];
            Dictionary<string, (List<string> targetProfiles, List<string> typeProfiles)> literalAccumulator = [];

            int? bindingVsKey = ed.Binding?.ValueSet == null
                ? null
                : (DbValueSet.SelectSingle(_db, FhirPackageKey: pm.Key, UnversionedUrl: ed.Binding?.ValueSet)?.Key
                  ?? DbValueSet.SelectSingle(_db, FhirPackageKey: pm.Key, VersionedUrl: ed.Binding?.ValueSet)?.Key);

            IEnumerable<ElementDefinition.TypeRefComponent> elementTypes = ed.Type.Select(tr => tr.cgAsR5());
            foreach (ElementDefinition.TypeRefComponent tr in elementTypes)
            {
                string typeName = tr.cgName();

                if (!literalAccumulator.TryGetValue(typeName, out (List<string> targetProfiles, List<string> typeProfiles) accumulator))
                {
                    accumulator = ([], []);
                    literalAccumulator.Add(typeName, accumulator);
                }

                if ((tr.ProfileElement.Count == 0) &&
                    (tr.TargetProfileElement.Count == 0))
                {
                    //string tl = literalForType(tr.cgName(), null, null);

                    DbElementType et = new()
                    {
                        Key = DbElementType.GetIndex(),
                        FhirPackageKey = dbStructure.FhirPackageKey,
                        StructureKey = dbStructure.Key,
                        ElementKey = elementKey,
                        TypeName = typeName,
                        TypeStructureKey = null,
                        TypeProfile = null,
                        TypeProfileStructureKey = null,
                        TargetProfile = null,
                        TargetProfileStructureKey = null,
                    };
                    currentElementTypes.Add(et);
                    dbElementTypes.Add( et);

                    continue;
                }

                if (tr.ProfileElement.Count == 0)
                {
                    foreach (Canonical tp in tr.TargetProfile)
                    {
                        //string tl = literalForType(tr.cgName(), null, tp.Value);
                        DbElementType et = new()
                        {
                            Key = DbElementType.GetIndex(),
                            FhirPackageKey = dbStructure.FhirPackageKey,
                            StructureKey = dbStructure.Key,
                            ElementKey = elementKey,
                            TypeName = typeName,
                            TypeStructureKey = null,
                            TypeProfile = null,
                            TypeProfileStructureKey = null,
                            TargetProfile = tp.Value,
                            TargetProfileStructureKey = null,
                        };
                        currentElementTypes.Add(et);
                        dbElementTypes.Add(et);
                        accumulator.targetProfiles.Add(tp.Value);
                    }

                    continue;
                }

                if (tr.TargetProfileElement.Count == 0)
                {
                    foreach (Canonical p in tr.Profile)
                    {
                        //string tl = literalForType(tr.cgName(), p.Value, null);
                        DbElementType et = new()
                        {
                            Key = DbElementType.GetIndex(),
                            FhirPackageKey = dbStructure.FhirPackageKey,
                            StructureKey = dbStructure.Key,
                            ElementKey = elementKey,
                            TypeName = typeName,
                            TypeStructureKey = null,
                            TypeProfile = p.Value,
                            TypeProfileStructureKey = null,
                            TargetProfile = null,
                            TargetProfileStructureKey = null,
                        };
                        currentElementTypes.Add(et);
                        dbElementTypes.Add(et);
                        accumulator.typeProfiles.Add(p.Value);
                    }

                    continue;
                }

                foreach (Canonical p in tr.Profile)
                {
                    foreach (Canonical tp in tr.TargetProfile)
                    {
                        //string tl = literalForType(tr.cgName(), p.Value, tp.Value);
                        DbElementType et = new()
                        {
                            Key = DbElementType.GetIndex(),
                            FhirPackageKey = dbStructure.FhirPackageKey,
                            StructureKey = dbStructure.Key,
                            ElementKey = elementKey,
                            TypeName = typeName,
                            TypeStructureKey = null,
                            TypeProfile = p.Value,
                            TypeProfileStructureKey = null,
                            TargetProfile = tp.Value,
                            TargetProfileStructureKey = null,
                        };
                        currentElementTypes.Add(et);
                        dbElementTypes.Add(et);
                        accumulator.targetProfiles.Add(tp.Value);
                        accumulator.typeProfiles.Add(p.Value);
                        //literalComponents.Add($"{p.Value}[{tp.Value}]");
                    }
                }
            }

            if (currentElementTypes.Count == 0)
            {
                string resolvedTypeName;

                if (ed.ElementId == sd.Id)
                {
                    resolvedTypeName = sd.Id;
                    //string tl = literalForType(sd.Id, null, null);
                    DbElementType et = new()
                    {
                        Key = DbElementType.GetIndex(),
                        FhirPackageKey = dbStructure.FhirPackageKey,
                        StructureKey = dbStructure.Key,
                        ElementKey = elementKey,
                        TypeName = sd.Id,
                        TypeStructureKey = null,
                        TypeProfile = null,
                        TypeProfileStructureKey = null,
                        TargetProfile = null,
                        TargetProfileStructureKey = null,
                    };
                    currentElementTypes.Add(et);
                    dbElementTypes.Add(et);
                }
                else
                {
                    string btn = ed.cgBaseTypeName(dc, true);
                    resolvedTypeName = btn;
                    //string tl = literalForType(btn, null, null);

                    DbElementType et = new()
                    {
                        Key = DbElementType.GetIndex(),
                        FhirPackageKey = dbStructure.FhirPackageKey,
                        StructureKey = dbStructure.Key,
                        ElementKey = elementKey,
                        TypeName = btn,
                        TypeStructureKey = null,
                        TypeProfile = null,
                        TypeProfileStructureKey = null,
                        TargetProfile = null,
                        TargetProfileStructureKey = null,
                    };
                    currentElementTypes.Add(et);
                    dbElementTypes.Add(et);
                }

                if (!literalAccumulator.TryGetValue(resolvedTypeName, out (List<string> targetProfiles, List<string> typeProfiles) accumulator))
                {
                    accumulator = ([], []);
                    literalAccumulator.Add(resolvedTypeName, accumulator);
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
                        BindingValueSetKey = string.IsNullOrEmpty(additional.ValueSet) ? null : DbValueSet.SelectSingle(_db, FhirPackageKey: pm.Key, UnversionedUrl: additional.ValueSet)?.Key,
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
            string basePath = ed.Base?.Path ?? ed.Path;

            List<string> completeLiteralComponents = [];
            foreach ((string typeName, (List<string> targetProfiles, List<string> typeProfiles)) in literalAccumulator)
            {
                string current = typeName;

                if (typeProfiles.Count != 0)
                {
                    current += "[" + string.Join(',', typeProfiles) + "]";
                }

                if (targetProfiles.Count != 0)
                {
                    current += "(" + string.Join(',', targetProfiles) + ")";
                }

                completeLiteralComponents.Add(current);
            }

            string fullTypeLiteral = string.Join(", ", completeLiteralComponents.Order());

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
                Comments = ed.Comment.ProcessCoreTextForLinks(fhirVersionLiteral),
                Requirements = ed.Requirements.ProcessCoreTextForLinks(fhirVersionLiteral),
                MinCardinality = ed.cgCardinalityMin(),
                MaxCardinality = ed.cgCardinalityMax(),
                MaxCardinalityString = ed.Max ?? "*",
                SliceName = ed.SliceName,
                ValueSetBindingStrength = ed.Binding?.Strength,
                BindingValueSet = ed.Binding?.ValueSet,
                BindingValueSetKey = bindingVsKey,
                BindingDescription = ed.Binding?.Description,
                AdditionalBindingCount = additionalBindingCount,
                FullCollatedTypeLiteral = fullTypeLiteral,
                IsInherited = isInherited,
                BasePath = basePath,
                BaseElementKey = null,
                BaseStructureKey = null,
                DefinedAsContentReference = !string.IsNullOrEmpty(ed.ContentReference),
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
                if (_db != null)
                {
                    _db.Close();
                    _db.Dispose();
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
