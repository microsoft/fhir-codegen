using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using Fhir.CodeGen.Common.FhirExtensions;
using Fhir.CodeGen.Common.Models;
using Fhir.CodeGen.Common.Packaging;
using Fhir.CodeGen.Comparison.CompareTool;
using Fhir.CodeGen.Comparison.Models;
using Fhir.CodeGen.Comparison.XVer;
using Fhir.CodeGen.Lib.Loader;
using Hl7.Fhir.Model;
using Hl7.Fhir.Utility;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using static Fhir.CodeGen.Comparison.CompareTool.FhirTypeMappings;
using CMR = Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship;

namespace Fhir.CodeGen.Comparison.CrossVersionSource;

public class ConceptMapLoader
{
    private ILoggerFactory? _loggerFactory;
    private ILogger _logger;

    private MappingLoader _mappingLoader;
    private IDbConnection _db;
    private PackageLoader _loader;

    private List<DbFhirPackage> _packages = [];

    public ConceptMapLoader(
        MappingLoader mappingLoader,
        IDbConnection db,
        PackageLoader loader,
        List<DbFhirPackage> packages,
        ILoggerFactory? loggerFactory = null)
    {
        _loggerFactory = loggerFactory;
        _logger = loggerFactory?.CreateLogger<ConceptMapLoader>()
            ?? LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<ConceptMapLoader>()
            ?? NullLoggerFactory.Instance.CreateLogger(nameof(ConceptMapLoader));

        _mappingLoader = mappingLoader;
        _db = db;
        _loader = loader;
        _packages = packages;
    }

    public void LoadInternalTypeMaps(bool loadPrimitives = true, bool loadComplex = true)
    {
        HashSet<(FhirReleases.FhirSequenceCodes, FhirReleases.FhirSequenceCodes)> processedPairs = [];

        // iterate across package pairs
        for (int sourceIndex = 0; sourceIndex < _packages.Count; sourceIndex++)
        {
            DbFhirPackage sourcePackage = _packages[sourceIndex];

            for (int targetIndex = 0; targetIndex < sourceIndex; targetIndex++)
            {
                if (targetIndex == sourceIndex)
                {
                    continue;
                }

                DbFhirPackage targetPackage = _packages[targetIndex];

                if (processedPairs.Add((sourcePackage.DefinitionFhirSequence, targetPackage.DefinitionFhirSequence)))
                {
                    loadInternalTypeMap(
                        sourcePackage,
                        targetPackage,
                        loadPrimitives,
                        loadComplex);
                }

                if (processedPairs.Add((targetPackage.DefinitionFhirSequence, sourcePackage.DefinitionFhirSequence)))
                {
                    loadInternalTypeMap(
                        targetPackage,
                        sourcePackage,
                        loadPrimitives,
                        loadComplex);
    }
}
        }

    }

    private void loadInternalTypeMap(
        DbFhirPackage sourcePackage,
        DbFhirPackage targetPackage,
        bool loadPrimitives = true,
        bool loadComplex = true)
    {
        List<DbStructureMappingRecord> toAdd = [];

        // add the primitive type mappings first
        if (loadPrimitives)
        {
            toAdd.AddRange(
                buildInternalPrimitiveTypeMapRecs(
                    sourcePackage,
                    targetPackage));
        }

        // add the complex type mappings second
        if (loadComplex)
        {
            toAdd.AddRange(
                buildInternalComplexTypeMapRecs(
                    sourcePackage,
                    targetPackage));
        }

        // insert our mapping records
        if (toAdd.Count > 0)
        {
            toAdd.Insert(_db, ignoreDuplicates: true, insertPrimaryKey: true);
        }
    }

    private List<DbStructureMappingRecord> buildInternalPrimitiveTypeMapRecs(
        DbFhirPackage sourcePackage,
        DbFhirPackage targetPackage)
    {
        List<DbStructureMappingRecord> toAdd = [];

        // get the source and target primitive types
        List<DbStructureDefinition> sourceTypes = DbStructureDefinition.SelectList(
            _db,
            FhirPackageKey: sourcePackage.Key,
            ArtifactClass: FhirArtifactClassEnum.PrimitiveType);

        Dictionary<string, DbStructureDefinition> targetTypes = DbStructureDefinition.SelectList(
            _db,
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

                // create the structure mapping record
                DbStructureMappingRecord mappingRec = new()
                {
                    Key = DbStructureMappingRecord.GetIndex(),

                    SourceFhirPackageKey = sourcePackage.Key,
                    SourceFhirSequence = sourcePackage.DefinitionFhirSequence,
                    SourceStructureKey = sourceSd.Key,
                    SourceStructureId = sourceSd.Id,

                    TargetFhirPackageKey = targetPackage.Key,
                    TargetFhirSequence = targetPackage.DefinitionFhirSequence,
                    TargetStructureKey = targetSd.Key,
                    TargetStructureId = tm.TargetType,

                    ConceptMapSourceKey = null,
                    FmlSourceKey = null,

                    ExplicitNoMap = false,
                    Relationship = tm.Relationship,
                    ConceptDomainRelationship = null,
                    ValueDomainRelationship = null,
                };

                toAdd.Add(mappingRec);
            }

            // if there are no maps for this source type, add a no-map entry
            if (!mapsAdded)
            {
                // create the structure mapping record
                DbStructureMappingRecord mappingRec = new()
                {
                    Key = DbStructureMappingRecord.GetIndex(),

                    SourceFhirPackageKey = sourcePackage.Key,
                    SourceFhirSequence = sourcePackage.DefinitionFhirSequence,
                    SourceStructureKey = sourceSd.Key,
                    SourceStructureId = sourceSd.Id,

                    TargetFhirPackageKey = targetPackage.Key,
                    TargetFhirSequence = targetPackage.DefinitionFhirSequence,
                    TargetStructureKey = null,
                    TargetStructureId = null,

                    ConceptMapSourceKey = null,
                    FmlSourceKey = null,

                    ExplicitNoMap = false,
                    Relationship = null,
                    ConceptDomainRelationship = null,
                    ValueDomainRelationship = null,
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
            _db,
            FhirPackageKey: sourcePackage.Key,
            ArtifactClass: FhirArtifactClassEnum.ComplexType);

        Dictionary<string, DbStructureDefinition> targetTypes = DbStructureDefinition.SelectList(
            _db,
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

                // create the structure mapping record
                DbStructureMappingRecord mappingRec = new()
                {
                    Key = DbStructureMappingRecord.GetIndex(),

                    SourceFhirPackageKey = sourcePackage.Key,
                    SourceFhirSequence = sourcePackage.DefinitionFhirSequence,
                    SourceStructureKey = sourceSd.Key,
                    SourceStructureId = sourceSd.Id,

                    TargetFhirPackageKey = targetPackage.Key,
                    TargetFhirSequence = targetPackage.DefinitionFhirSequence,
                    TargetStructureKey = targetSd.Key,
                    TargetStructureId = tm.TargetType,

                    ConceptMapSourceKey = null,
                    FmlSourceKey = null,

                    ExplicitNoMap = false,
                    Relationship = tm.Relationship,
                    ConceptDomainRelationship = tm.ConceptDomainRelationship,
                    ValueDomainRelationship = tm.ValueDomainRelationship,

                    Comments = tm.Comment,
                };
                toAdd.Add(mappingRec);
            }

            // if there are no maps for this source type, add a no-map entry
            if (!mapsAdded)
            {
                // create the structure mapping record
                DbStructureMappingRecord mappingRec = new()
                {
                    Key = DbStructureMappingRecord.GetIndex(),

                    SourceFhirPackageKey = sourcePackage.Key,
                    SourceFhirSequence = sourcePackage.DefinitionFhirSequence,
                    SourceStructureKey = sourceSd.Key,
                    SourceStructureId = sourceSd.Id,

                    TargetFhirPackageKey = targetPackage.Key,
                    TargetFhirSequence = targetPackage.DefinitionFhirSequence,
                    TargetStructureKey = null,
                    TargetStructureId = null,

                    ConceptMapSourceKey = null,
                    FmlSourceKey = null,

                    ExplicitNoMap = false,
                    Relationship = null,
                    ConceptDomainRelationship = null,
                    ValueDomainRelationship = null,
                };
                toAdd.Add(mappingRec);
            }
        }

        return toAdd;
    }


    private enum SourceMapTypeCodes : int
    {
        Types = 0,
        Codes,
        Resources,
        Elements,
        SearchParams,
    }

    public void LoadSourceMaps(string basePath, string relativePath, string searchPattern)
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
            FhirReleases.FhirSequenceCodes sourceVersion;
            FhirReleases.FhirSequenceCodes targetVersion;

            if (sourceMapType == SourceMapTypeCodes.Codes)
            {
                (sourceVersion, targetVersion) = _mappingLoader.getSequenceFromFilename(filename);
            }
            else
            {
                (string sv, string tv) = _mappingLoader.getVersionsFromFilename(filename);

                if (!FhirReleases.TryGetSequence(sv, out sourceVersion))
                {
                    throw new Exception($"Invalid source version: {sv} from file: {filename}!");
                }

                if (!FhirReleases.TryGetSequence(tv, out targetVersion))
                {
                    throw new Exception($"Invalid target version: {tv} from file: {filename}!");
                }
            }

            // ensure we have these versions in the database
            DbFhirPackage? sourcePackage = DbFhirPackage.SelectSingle(
                _db,
                PackageId: sourceVersion.ToCorePackageId(),
                PackageVersion: sourceVersion.ToLongVersion());
            if (sourcePackage is null)
            {
                _logger.LogWarning($"Skipping map with source version: {sourceVersion.ToRLiteral()} since it is not in the database!");
                continue;
            }

            DbFhirPackage? targetPackage = DbFhirPackage.SelectSingle(
                _db,
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
                        int addedMapCount = loadSourceTypeMap(
                            sourcePackage,
                            targetPackage,
                            cm,
                            Path.GetFileName(filename),
                            inputPath);
                        typeMapCount += addedMapCount;
                    }
                    break;

                case SourceMapTypeCodes.Codes:
                    {
                        (int addedVsMapCount, int addedConceptMapCount) = loadSourceCodeMap(
                            sourcePackage,
                            targetPackage,
                            cm,
                            Path.GetFileName(filename));
                        valueSetMapCount += addedVsMapCount;
                        valueSetConceptMapCount += addedConceptMapCount;
                    }
                    break;

                case SourceMapTypeCodes.Resources:
                    {
                        int addedMapCount = loadSourceResourceMap(
                            sourcePackage,
                            targetPackage,
                            cm,
                            Path.GetFileName(filename),
                            inputPath);
                        resourceMapCount += addedMapCount;
                    }
                    break;

                case SourceMapTypeCodes.Elements:
                    {
                        int addedMapCount = loadSourceElementMap(
                            sourcePackage,
                            targetPackage,
                            cm,
                            Path.GetFileName(filename));
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
        ConceptMap cm,
        string conceptMapFilename)
    {
        List<DbElementMappingRecord> elementMapsToAdd = [];

        int sourceFileKey = _mappingLoader.getOrCreateMappingSourceFileKey(
            conceptMapFilename,
            MappingLoader.SourceFileTypeCodes.ConceptMap,
            cm.Url);

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
                    _db,
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
                            _db,
                            SourceFhirPackageKey: sourcePackage.Key,
                            SourceStructureId: groupSourceElement.Code.Split('.').First(),
                            TargetFhirPackageKey: targetPackage.Key);
                    }
                    else
                    {
                        relevantMaps = DbStructureMappingRecord.SelectList(
                            _db,
                            SourceFhirPackageKey: sourcePackage.Key,
                            SourceStructureKey: sourceElement.StructureKey,
                            TargetFhirPackageKey: targetPackage.Key);
                    }

                    if (relevantMaps.Count == 0)
                    {
                        // TODO: This is an error in the map, it needs to be fixed but just log for now
                        _logger.LogWarning($"No relevant structure maps found for source element: {groupSourceElement.Code} in map: {cm.Url} ({cm.Id})!");
                        continue;
                        //throw new Exception($"No relevant structure maps found for source element: {groupSourceElement.Code} in map: {cm.Url} ({cm.Id})!");
                    }

                    foreach (DbStructureMappingRecord relevantMap in relevantMaps)
                    {
                        DbElementMappingRecord mapRec = new()
                        {
                            Key = DbElementMappingRecord.GetIndex(),
                            StructureMappingKey = relevantMap.Key,

                            SourceFhirPackageKey = sourcePackage.Key,
                            SourceFhirSequence = sourcePackage.DefinitionFhirSequence,
                            SourceElementKey = sourceElement?.Key,
                            SourceElementId = groupSourceElement.Code,

                            TargetFhirPackageKey = targetPackage.Key,
                            TargetFhirSequence = targetPackage.DefinitionFhirSequence,
                            TargetElementKey = null,
                            TargetElementId = null,

                            ConceptMapSourceKey = sourceFileKey,
                            FmlSourceKey = null,
                            FmlIsSimpleCopy = null,

                            ExplicitNoMap = true,
                            Relationship = null,
                            ConceptDomainRelationship = null,
                            ValueDomainRelationship = null,
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
                        _db,
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
                                _db,
                                SourceFhirPackageKey: sourcePackage.Key,
                                SourceStructureId: groupSourceElement.Code.Split('.').First(),
                                TargetFhirPackageKey: targetPackage.Key,
                                TargetStructureId: elementTarget.Code.Split('.').First());
                        }
                        else
                        {
                            relevantMap = DbStructureMappingRecord.SelectSingle(
                                _db,
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
                                _db,
                                SourceFhirPackageKey: sourcePackage.Key,
                                SourceStructureId: groupSourceElement.Code.Split('.').First(),
                                TargetFhirPackageKey: targetPackage.Key,
                                TargetStructureKey: targetElement.StructureKey);
                        }
                        else
                        {
                            relevantMap = DbStructureMappingRecord.SelectSingle(
                                _db,
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

                    // get extended relationship properties if they exist
                    CMR? cdRelationship = elementTarget.Property
                        .FirstOrDefault(p => p.Code == ConceptMapProperties.PropertyCodeConceptDomainRelationship)?.Value is Code cdCodeValue
                        ? EnumUtility.ParseLiteral<CMR>(cdCodeValue.Value)
                        : null;

                    CMR? vdRelationship = elementTarget.Property
                        .FirstOrDefault(p => p.Code == ConceptMapProperties.PropertyCodeValueDomainRelationship)?.Value is Code vdCodeValue
                        ? EnumUtility.ParseLiteral<CMR>(vdCodeValue.Value)
                        : null;

                    // create a record for the database
                    DbElementMappingRecord mapRec = new()
                    {
                        Key = DbElementMappingRecord.GetIndex(),
                        StructureMappingKey = relevantMap.Key,

                        SourceFhirPackageKey = sourcePackage.Key,
                        SourceFhirSequence = sourcePackage.DefinitionFhirSequence,
                        SourceElementKey = sourceElement?.Key,
                        SourceElementId = groupSourceElement.Code,

                        TargetFhirPackageKey = targetPackage.Key,
                        TargetFhirSequence = targetPackage.DefinitionFhirSequence,
                        TargetElementKey = targetElement?.Key,
                        TargetElementId = elementTarget.Code,

                        ConceptMapSourceKey = sourceFileKey,
                        FmlSourceKey = null,
                        FmlIsSimpleCopy = null,

                        ExplicitNoMap = false,
                        Relationship = elementTarget.Relationship,
                        ConceptDomainRelationship = cdRelationship,
                        ValueDomainRelationship = vdRelationship,
                    };

                    elementMapsToAdd.Add(mapRec);
                }
            }
        }

        // insert into the database
        elementMapsToAdd.Insert(_db, ignoreDuplicates: true, insertPrimaryKey: true);
        //_logger.LogInformation($"Inserted {elementMapsToAdd.Count} Type Definition Map records");

        return elementMapsToAdd.Count;
    }


    private int loadSourceResourceMap(
        DbFhirPackage sourcePackage,
        DbFhirPackage targetPackage,
        ConceptMap cm,
        string conceptMapFilename,
        string sourceInputPath)
    {
        List<DbStructureMappingRecord> resourceMapsToAdd = [];

        int sourceFileKey = _mappingLoader.getOrCreateMappingSourceFileKey(
            conceptMapFilename,
            MappingLoader.SourceFileTypeCodes.ConceptMap,
            cm.Url);

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
                    _db,
                    FhirPackageKey: sourcePackage.Key,
                    Id: groupSourceElement.Code);
                if (sourceSd is null)
                {
                    throw new Exception($"Invalid source resource: `{groupSourceElement.Code}` in map: {cm.Url} ({cm.Id})!");
                }

                // check for no map
                if (groupSourceElement.NoMap == true)
                {
                    DbStructureMappingRecord mapRec = new()
                    {
                        Key = DbStructureMappingRecord.GetIndex(),

                        SourceFhirPackageKey = sourcePackage.Key,
                        SourceFhirSequence = sourcePackage.DefinitionFhirSequence,
                        SourceStructureKey = sourceSd.Key,
                        SourceStructureId = sourceSd.Id,

                        TargetFhirPackageKey = targetPackage.Key,
                        TargetFhirSequence = targetPackage.DefinitionFhirSequence,
                        TargetStructureKey = null,
                        TargetStructureId = null,

                        ConceptMapSourceKey = sourceFileKey,
                        FmlSourceKey = null,

                        ExplicitNoMap = true,
                        Relationship = null,
                        ConceptDomainRelationship = null,
                        ValueDomainRelationship = null,
                    };

                    resourceMapsToAdd.Add(mapRec);
                    continue;
                }

                // iterate over the map targets
                foreach (ConceptMap.TargetElementComponent elementTarget in groupSourceElement.Target)
                {
                    // resolve the target type
                    DbStructureDefinition? targetSd = DbStructureDefinition.SelectSingle(
                        _db,
                        FhirPackageKey: targetPackage.Key,
                        Id: elementTarget.Code);

                    if (targetSd is null)
                    {
                        throw new Exception($"Invalid target resource: `{elementTarget.Code}` for source: {groupSourceElement.Code} in map: {cm.Url} ({cm.Id})!");
                    }

                    // get extended relationship properties if they exist
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
                        SourceFhirSequence = sourcePackage.DefinitionFhirSequence,
                        SourceStructureKey = sourceSd.Key,
                        SourceStructureId = sourceSd.Id,

                        TargetFhirPackageKey = targetPackage.Key,
                        TargetFhirSequence = targetPackage.DefinitionFhirSequence,
                        TargetStructureKey = targetSd.Key,
                        TargetStructureId = targetSd.Id,

                        ConceptMapSourceKey = sourceFileKey,
                        FmlSourceKey = null,

                        ExplicitNoMap = false,
                        Relationship = elementTarget.Relationship,
                        Comments = elementTarget.Comment,
                        ConceptDomainRelationship = cdRelationship,
                        ValueDomainRelationship = vdRelationship,
                    };

                    resourceMapsToAdd.Add(mapRec);
                }
            }
        }

        // insert into the database
        resourceMapsToAdd.Insert(_db, ignoreDuplicates: true, insertPrimaryKey: true);
        //_logger.LogInformation($"Inserted {elementMapsToAdd.Count} Type Definition Map records");

        return resourceMapsToAdd.Count;
    }



    private (int insertedVsMapCount, int insertedConceptMapCount) loadSourceCodeMap(
        DbFhirPackage sourcePackage,
        DbFhirPackage targetPackage,
        ConceptMap cm,
        string conceptMapFilename)
    {
        int sourceFileKey = _mappingLoader.getOrCreateMappingSourceFileKey(
            conceptMapFilename,
            MappingLoader.SourceFileTypeCodes.ConceptMap,
            cm.Url);

        List<DbValueSetMappingRecord> valueSetMapsToAdd = [];
        List<DbValueSetConceptMappingRecord> conceptMapsToAdd = [];

        //List<string> originalUrls = [];
        //foreach (Extension ext in cm.GetExtensions(CommonDefinitions.ExtUrlConceptMapAdditionalUrls))
        //{
        //    if (ext.Value is FhirUrl url)
        //    {
        //        originalUrls.Add(url.Value);
        //    }
        //}

        if (cm.SourceScope is not Canonical sourceScopeCanonical)
        {
            throw new Exception($"Invalid source scope for codes: {cm.SourceScope}");
        }

        string? sourceVersioned;
        string sourceUnversioned;

        int lastPipeIndex = sourceScopeCanonical.Value.LastIndexOf('|');
        if (lastPipeIndex != -1)
        {
            sourceVersioned = sourceScopeCanonical.Value;
            sourceUnversioned = sourceScopeCanonical.Value[0..lastPipeIndex];
        }
        else
        {
            sourceVersioned = null;
            sourceUnversioned = sourceScopeCanonical.Value;
        }

        DbValueSet? sourceVs = DbValueSet.SelectSingle(
            _db,
            FhirPackageKey: sourcePackage.Key,
            UnversionedUrl: sourceUnversioned);

        if (sourceVs is null)
        {
            if (XVerProcessor._exclusionSet.Contains(sourceUnversioned))
            {
                // skip this one
                return (0, 0);
            }

            throw new Exception($"Failed to resolve source value set: {sourceUnversioned} in FHIR {sourcePackage.ShortName}");
        }

        if (sourceVs.CanExpand != true)
        {
            // skip this
            return (0, 0);
        }

        if (cm.TargetScope is not Canonical targetScopeCanonical)
        {
            throw new Exception($"Invalid target scope for codes: {cm.TargetScope}");
        }

        string? targetVersioned;
        string targetUnversioned;

        lastPipeIndex = targetScopeCanonical.Value.LastIndexOf('|');
        if (lastPipeIndex != -1)
        {
            targetVersioned = targetScopeCanonical.Value;
            targetUnversioned = targetScopeCanonical.Value[0..lastPipeIndex];
        }
        else
        {
            targetVersioned = null;
            targetUnversioned = targetScopeCanonical.Value;
        }

        DbValueSet? targetVs = DbValueSet.SelectSingle(
            _db,
            FhirPackageKey: targetPackage.Key,
            UnversionedUrl: targetUnversioned);

        if (targetVs is null)
        {
            if (XVerProcessor._exclusionSet.Contains(targetUnversioned))
            {
                // skip this one
                return (0, 0);
            }

            throw new Exception($"Failed to resolve target value set: {targetUnversioned} in FHIR {targetPackage.ShortName}");
        }

        if (targetVs.CanExpand != true)
        {
            // skip this
            return (0, 0);
        }

        // groups are systems within the value sets
        foreach (ConceptMap.GroupComponent group in cm.Group)
        {
            // get from the db or create a new map
            DbValueSetMappingRecord? vsMap = DbValueSetMappingRecord.SelectSingle(
                _db,
                SourceValueSetKey: sourceVs.Key,
                TargetValueSetKey: targetVs.Key);
            if (vsMap is null)
            {
                vsMap = new()
                {
                    Key = DbValueSetMappingRecord.GetIndex(),

                    SourceFhirPackageKey = sourcePackage.Key,
                    SourceFhirSequence = sourcePackage.DefinitionFhirSequence,
                    SourceValueSetKey = sourceVs.Key,
                    SourceValueSetId = sourceVs.Id,
                    SourceValueSetUrl = sourceVs.UnversionedUrl,
                    SourceValueSetVersion = sourceVs.Version,

                    TargetFhirPackageKey = targetPackage.Key,
                    TargetFhirSequence = targetPackage.DefinitionFhirSequence,
                    TargetValueSetKey = targetVs.Key,
                    TargetValueSetId = targetVs.Id,
                    TargetValueSetUrl = targetVs.UnversionedUrl,
                    TargetValueSetVersion = targetVs.Version,

                    ConceptMapSourceKey = sourceFileKey,

                    ExplicitNoMap = false,
                    Relationship = null,
                };

                valueSetMapsToAdd.Add(vsMap);
            }

            // iterate over the source elements of the map
            foreach (ConceptMap.SourceElementComponent groupSourceElement in group.Element)
            {
                // resolve the source concept
                DbValueSetConcept? sourceConcept = DbValueSetConcept.SelectSingle(
                    _db,
                    ValueSetKey: sourceVs.Key,
                    Code: groupSourceElement.Code);
                if (sourceConcept is null)
                {
                    // TODO: These are incorrect source mappings and need to be fixed, just not right now
                    _logger.LogWarning($"Invalid source concept literal `{groupSourceElement.Code}`" +
                        $" for Value Set: `{sourceVs.VersionedUrl}` from map: {cm.Url} ({cm.Id})" +
                        $" - fix the map!");
                    continue;

                    //throw new Exception($"Invalid source concept literal `{groupSourceElement.Code}` for Value Set: `{sourceVs.VersionedUrl}` from map: {cm.Url} ({cm.Id})");
                }

                // check for no map
                if (groupSourceElement.NoMap == true)
                {
                    // check to see if we already have this in the database
                    DbValueSetConceptMappingRecord? conceptMapRec = DbValueSetConceptMappingRecord.SelectSingle(
                        _db,
                        ValueSetMappingKey: vsMap.Key,
                        SourceValueSetConceptKey: sourceConcept.Key,
                        TargetValueSetConceptKeyIsNull: true);
                    if (conceptMapRec is not null)
                    {
                        continue;
                    }

                    // create a record for the database
                    conceptMapRec = new()
                    {
                        Key = DbValueSetConceptMappingRecord.GetIndex(),
                        ValueSetMappingKey = vsMap.Key,

                        SourceFhirPackageKey = sourcePackage.Key,
                        SourceFhirSequence = sourcePackage.DefinitionFhirSequence,
                        SourceValueSetConceptKey = sourceConcept.Key,

                        TargetFhirPackageKey = targetPackage.Key,
                        TargetFhirSequence = targetPackage.DefinitionFhirSequence,
                        TargetValueSetConceptKey = null,

                        ExplicitNoMap = true,
                        Relationship = null,

                        Comments = null,
                        TechnicalNotes = null,
                    };

                    conceptMapsToAdd.Add(conceptMapRec);
                    continue;
                }

                // iterate over the map targets
                foreach (ConceptMap.TargetElementComponent elementTarget in groupSourceElement.Target)
                {
                    // resolve the target concept
                    DbValueSetConcept? targetConcept = DbValueSetConcept.SelectSingle(
                        _db,
                        ValueSetKey: targetVs.Key,
                        Code: elementTarget.Code);

                    if (targetConcept is null)
                    {
                        // TODO: These are incorrect source mappings and need to be fixed, just not right now
                        _logger.LogWarning($"Invalid target concept literal `{elementTarget.Code}`" +
                            $" for Value Set: `{sourceVs.VersionedUrl}` source: `{groupSourceElement.Code}`" +
                            $" from map: {cm.Url} ({cm.Id}) - fix the map!");
                        continue;

                        //throw new Exception($"Invalid target concept literal `{elementTarget.Code}`" +
                        //    $" for Value Set: `{sourceVs.VersionedUrl}` source: `{groupSourceElement.Code}`" +
                        //    $" from map: {cm.Url} ({cm.Id})");
                    }

                    // check to see if we already have this in the database
                    DbValueSetConceptMappingRecord? conceptMapRec = DbValueSetConceptMappingRecord.SelectSingle(
                        _db,
                        ValueSetMappingKey: vsMap.Key,
                        SourceValueSetConceptKey: sourceConcept.Key,
                        TargetValueSetConceptKey: targetConcept.Key);

                    if (conceptMapRec is not null)
                    {
                        continue;
                    }

                    // create a record for the database
                    conceptMapRec = new()
                    {
                        Key = DbValueSetConceptMappingRecord.GetIndex(),
                        ValueSetMappingKey = vsMap.Key,

                        SourceFhirPackageKey = sourcePackage.Key,
                        SourceFhirSequence = sourcePackage.DefinitionFhirSequence,
                        SourceValueSetConceptKey = sourceConcept.Key,

                        TargetFhirPackageKey = targetPackage.Key,
                        TargetFhirSequence = targetPackage.DefinitionFhirSequence,
                        TargetValueSetConceptKey = targetConcept.Key,

                        ExplicitNoMap = false,
                        Relationship = elementTarget.Relationship,
                        Comments = elementTarget.Comment,
                        TechnicalNotes = null,
                    };

                    conceptMapsToAdd.Add(conceptMapRec);
                }
            }
        }

        // insert into the database
        valueSetMapsToAdd.Insert(_db, ignoreDuplicates: true, insertPrimaryKey: true);
        //_logger.LogInformation($"Inserted {valueSetMapsToAdd.Count} Value Set Map records");

        conceptMapsToAdd.Insert(_db, ignoreDuplicates: true, insertPrimaryKey: true);
        //_logger.LogInformation($"Inserted {conceptMapsToAdd.Count} Value Set Concept Map records");

        return (valueSetMapsToAdd.Count, conceptMapsToAdd.Count);
    }


    private int loadSourceTypeMap(
        DbFhirPackage sourcePackage,
        DbFhirPackage targetPackage,
        ConceptMap cm,
        string conceptMapFilename,
        string sourceInputPath)
    {
        List<DbStructureMappingRecord> typeDefinitionMapsToAdd = [];

        int sourceFileKey = _mappingLoader.getOrCreateMappingSourceFileKey(
            conceptMapFilename,
            MappingLoader.SourceFileTypeCodes.ConceptMap,
            cm.Url);

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
                    _db,
                    FhirPackageKey: sourcePackage.Key,
                    Id: groupSourceElement.Code);
                if (sourceSd is null)
                {
                    throw new Exception($"Invalid source type: `{groupSourceElement.Code}` in map: {cm.Url} ({cm.Id})!");
                }

                // TODO: for now, just ignore primitives - need to fix the source maps but am just using internal maps for them for now
                if (sourceSd.ArtifactClass == FhirArtifactClassEnum.PrimitiveType)
                {
                    continue;
                }

                // check for no map
                if (groupSourceElement.NoMap == true)
                {
                    DbStructureMappingRecord mapRec = new()
                    {
                        Key = DbStructureMappingRecord.GetIndex(),

                        SourceFhirPackageKey = sourcePackage.Key,
                        SourceFhirSequence = sourcePackage.DefinitionFhirSequence,
                        SourceStructureKey = sourceSd.Key,
                        SourceStructureId = sourceSd.Id,

                        TargetFhirPackageKey = targetPackage.Key,
                        TargetFhirSequence = targetPackage.DefinitionFhirSequence,
                        TargetStructureKey = null,
                        TargetStructureId = null,

                        ConceptMapSourceKey = sourceFileKey,
                        FmlSourceKey = null,

                        ExplicitNoMap = true,
                        Relationship = null,
                        ConceptDomainRelationship = null,
                        ValueDomainRelationship = null,
                    };

                    typeDefinitionMapsToAdd.Add(mapRec);
                    continue;
                }

                // iterate over the map targets
                foreach (ConceptMap.TargetElementComponent elementTarget in groupSourceElement.Target)
                {
                    // resolve the target type
                    DbStructureDefinition? targetSd = DbStructureDefinition.SelectSingle(
                        _db,
                        FhirPackageKey: targetPackage.Key,
                        Id: elementTarget.Code);

                    if (targetSd is null)
                    {
                        throw new Exception($"Invalid target type: `{elementTarget.Code}` for source: {groupSourceElement.Code} in map: {cm.Url} ({cm.Id})!");
                    }

                    // get extended relationship properties if they exist
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
                        SourceFhirSequence = sourcePackage.DefinitionFhirSequence,
                        SourceStructureKey = sourceSd.Key,
                        SourceStructureId = sourceSd.Id,

                        TargetFhirPackageKey = targetPackage.Key,
                        TargetFhirSequence = targetPackage.DefinitionFhirSequence,
                        TargetStructureKey = targetSd.Key,
                        TargetStructureId = targetSd.Id,

                        ConceptMapSourceKey = sourceFileKey,
                        FmlSourceKey = null,

                        ExplicitNoMap = false,
                        Relationship = elementTarget.Relationship,
                        Comments = elementTarget.Comment,

                        ConceptDomainRelationship = cdRelationship,
                        ValueDomainRelationship = vdRelationship,
                    };

                    typeDefinitionMapsToAdd.Add(mapRec);
                }
            }
        }

        // insert into the database
        typeDefinitionMapsToAdd.Insert(_db, ignoreDuplicates: true, insertPrimaryKey: true);
        //_logger.LogInformation($"Inserted {elementMapsToAdd.Count} Type Definition Map records");

        return typeDefinitionMapsToAdd.Count;
    }

}
