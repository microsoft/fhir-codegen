using Fhir.CodeGen.Comparison.Models;
using Fhir.CodeGen.Common.Packaging;
using Microsoft.Extensions.Logging;
using System.Data;
using CMR = Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship;
using Fhir.CodeGen.Comparison.XVer;
using Fhir.CodeGen.Common.Utils;
using Fhir.CodeGen.Common.Models;

namespace Fhir.CodeGen.Comparison.CompareTool;

public class TransitiveMappingBuilder
{
    private readonly IDbConnection _db;
    private ILoggerFactory? _loggerFactory;
    private readonly ILogger _logger;
    private readonly List<DbFhirPackage> _packages;

    public TransitiveMappingBuilder(
        IDbConnection dbConnection,
        ILoggerFactory? loggerFactory,
        List<DbFhirPackage> packages)
    {
        _db = dbConnection;
        _loggerFactory = loggerFactory;
        _logger = loggerFactory?.CreateLogger<TransitiveMappingBuilder>()
            ?? LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<TransitiveMappingBuilder>();
        _packages = packages;
    }

    public void BuildTransitiveValueSetMappings()
    {
        _logger.LogInformation("Building transitive value set mappings...");

        // iterate over all the packages to use as source
        for (int sourceIndex = 0; sourceIndex < _packages.Count; sourceIndex++)
        {
            DbFhirPackage sourcePackage = _packages[sourceIndex];

            // iterate upwards first
            for (int targetIndex = sourceIndex + 1; targetIndex < _packages.Count; targetIndex++)
            {
                DbFhirPackage targetPackage = _packages[targetIndex];
                int hopCount = Math.Abs(targetIndex - sourceIndex);
                _logger.LogInformation(
                    $"Building transitive value set mappings: {sourcePackage.ShortName} -> {targetPackage.ShortName} ({hopCount} hops)");
                buildTransitiveValueSetMappings(sourcePackage, targetPackage, sourceIndex, targetIndex);
            }

            // iterate downwards
            for (int targetIndex = sourceIndex - 1; targetIndex >= 0; targetIndex--)
            {
                DbFhirPackage targetPackage = _packages[targetIndex];
                int hopCount = Math.Abs(targetIndex - sourceIndex);
                _logger.LogInformation(
                    $"Building transitive value set mappings: {sourcePackage.ShortName} -> {targetPackage.ShortName} ({hopCount} hops)");
                buildTransitiveValueSetMappings(sourcePackage, targetPackage, sourceIndex, targetIndex);
            }
        }
    }

    private void buildTransitiveValueSetMappings(
        DbFhirPackage sourcePackage,
        DbFhirPackage targetPackage,
        int sourceIndex,
        int targetIndex)
    {
        DbArtifactMapCache<DbValueSetMappingRecord> mappingCache = new();

        // get all direct mappings in the chain
        List<List<DbValueSetMappingRecord>> chainMappings = [];

        int increment = sourceIndex < targetIndex ? 1 : -1;
        int hops = Math.Abs(targetIndex - sourceIndex);

        int currentSourceIndex = sourceIndex;
        int currentTargetIndex = sourceIndex + increment;

        for (int step = 0; step < hops; step++)
        {
            DbFhirPackage fromPkg = _packages[currentSourceIndex];
            DbFhirPackage toPkg = _packages[currentTargetIndex];

            List<DbValueSetMappingRecord> directMaps = DbValueSetMappingRecord.SelectList(
                _db,
                SourceFhirPackageKey: fromPkg.Key,
                TargetFhirPackageKey: toPkg.Key);

            chainMappings.Add(directMaps);

            // increment our indices
            currentSourceIndex += increment;
            currentTargetIndex += increment;
        }

        // Get all source value sets
        List<DbValueSet> sourceValueSets = DbValueSet.SelectList(
            _db,
            FhirPackageKey: sourcePackage.Key);

        foreach (DbValueSet sourceVs in sourceValueSets)
        {
            // trace this value set through the chain (may produce multiple paths)
            traceValueSetThroughChain(
                sourceVs,
                chainMappings,
                sourceIndex,
                targetIndex,
                mappingCache);
        }

        if (mappingCache.ToAddCount > 0)
        {
            mappingCache.ToAdd.Insert(_db, ignoreDuplicates: true, insertPrimaryKey: true);
            _logger.LogInformation($"  Created {mappingCache.ToAddCount} transitive value set mappings");
        }

        if (mappingCache.ToUpdateCount > 0)
        {
            mappingCache.ToUpdate.Update(_db);
            _logger.LogInformation($"  Updated {mappingCache.ToUpdateCount} transitive value set mappings");
        }
    }

    private void traceValueSetThroughChain(
        DbValueSet sourceVs,
        List<List<DbValueSetMappingRecord>> chainMappings,
        int sourceIndex,
        int targetIndex,
        DbArtifactMapCache<DbValueSetMappingRecord> mappingCache)
    {
        // Each path tracks: currentKey, currentId, relationships, structureIds, versionKeys
        List<MappingPath> activePaths =
        [
            new MappingPath
            {
                CurrentKey = sourceVs.Key,
                CurrentId = sourceVs.Id,
                Relationships = [],
                Ids = [sourceVs.Id],
                VersionKeys = new Dictionary<FhirReleases.FhirSequenceCodes, int?>
                {
                    [_packages[sourceIndex].DefinitionFhirSequence] = sourceVs.Key
                }
            }
        ];

        int increment = sourceIndex < targetIndex ? 1 : -1;
        int hopIndex = sourceIndex + increment;

        // Walk through each step, expanding paths when multiple mappings exist
        for (int step = 0; step < chainMappings.Count; step++)
        {
            DbFhirPackage hopPackage = _packages[hopIndex];
            List<MappingPath> nextPaths = [];

            foreach (MappingPath path in activePaths)
            {
                if (path.CurrentKey == null)
                {
                    // value set doesn't exist from this point forward - continue path as-is
                    MappingPath continued = path.Clone();
                    continued.PreviousKey = path.CurrentKey;
                    continued.Ids.Add(null);
                    continued.VersionKeys[hopPackage.DefinitionFhirSequence] = null;
                    continued.Relationships.Add(null);
                    nextPaths.Add(continued);
                    continue;
                }

                // Find ALL mappings for this value set in this step
                List<DbValueSetMappingRecord> mappings = chainMappings[step]
                    .Where(m => m.SourceValueSetKey == path.CurrentKey)
                    .ToList();

                if (mappings.Count == 0)
                {
                    // check to see if there is a value set with the same id in the target package
                    DbValueSet? possibleVs = DbValueSet.SelectSingle(
                        _db,
                        FhirPackageKey: hopPackage.Key,
                        Id: path.CurrentId);

                    if (possibleVs is null)
                    {
                        // No mapping found - value set is unmapped at this step
                        MappingPath continued = path.Clone();
                        continued.CurrentKey = null;
                        continued.PreviousKey = path.CurrentKey;
                        continued.CurrentId = null;
                        continued.Ids.Add(null);
                        continued.VersionKeys[hopPackage.DefinitionFhirSequence] = null;
                        continued.Relationships.Add(null);
                        nextPaths.Add(continued);
                    }
                    else
                    {
                        // Create a new path for this possible match
                        MappingPath newPath = path.Clone();
                        newPath.Relationships.Add(CMR.Equivalent);
                        newPath.CurrentKey = possibleVs.Key;
                        newPath.PreviousKey = path.CurrentKey;
                        newPath.CurrentId = possibleVs.Id;
                        newPath.Ids.Add(possibleVs.Id);
                        newPath.VersionKeys[hopPackage.DefinitionFhirSequence] = possibleVs.Key;
                        newPath.ImplicitBasedOnIds = true;
                        nextPaths.Add(newPath);
                    }
                }
                else
                {
                    // Create a new path for each mapping (handles 1:N splits)
                    foreach (DbValueSetMappingRecord mapping in mappings)
                    {
                        MappingPath newPath = path.Clone();
                        newPath.Relationships.Add(mapping.Relationship);
                        newPath.CurrentKey = mapping.TargetValueSetKey;
                        newPath.PreviousKey = path.CurrentKey;
                        newPath.CurrentId = mapping.TargetValueSetId;
                        newPath.Ids.Add(mapping.TargetValueSetId);
                        newPath.VersionKeys[hopPackage.DefinitionFhirSequence] = mapping.TargetValueSetKey;
                        nextPaths.Add(newPath);
                    }
                }
            }

            activePaths = nextPaths;
            hopIndex += increment;
        }

        // Convert all completed paths to records
        foreach (MappingPath path in activePaths)
        {
            createValueSetMappingRecord(
                sourceVs,
                path,
                sourceIndex,
                targetIndex,
                mappingCache);
        }
    }

    private void createValueSetMappingRecord(
        DbValueSet sourceVs,
        MappingPath path,
        int sourceIndex,
        int targetIndex,
        DbArtifactMapCache<DbValueSetMappingRecord> mappingCache)
    {
        CMR? computedRelationship = RelationshipComposition.ComposeChain(path.Relationships);

        // get a previous key if one exists
        //

        (string idLong, string idShort) = XVerProcessor.GenerateArtifactId(
            _packages[sourceIndex].ShortName,
            sourceVs.Id,
            _packages[targetIndex].ShortName,
            path.CurrentId);

        // check to see if there is an existing record
        DbValueSetMappingRecord? rec = DbValueSetMappingRecord.SelectSingle(
            _db,
            IdLong: idLong);

        if (rec is null)
        {
            // check for cached record
            mappingCache.TryGet(idLong, out rec);
        }


        if (rec is not null)
        {
            // update with current data
            rec.ContentKeys = path.GetVersionKeyArray();

            rec.TechnicalNotes = (rec.TechnicalNotes ?? string.Empty) +
                $" Computed transitively through {string.Join("->", path.Ids.Where(s => s != null))}";

            mappingCache.CacheUpdate(rec);
        }
        else
        {
            // create a new record
            rec = new()
            {
                Key = DbValueSetMappingRecord.GetIndex(),
                PreviousStepMapRecordKey = null,
                Steps = Math.Abs(sourceIndex - targetIndex),

                SourceFhirPackageKey = _packages[sourceIndex].Key,
                SourceFhirSequence = _packages[sourceIndex].DefinitionFhirSequence,
                SourceValueSetKey = sourceVs.Key,
                SourceValueSetId = sourceVs.Id,

                TargetFhirPackageKey = _packages[targetIndex].Key,
                TargetFhirSequence = _packages[targetIndex].DefinitionFhirSequence,
                TargetValueSetKey = path.CurrentKey,
                TargetValueSetId = path.CurrentId,

                ContentKeys = path.GetVersionKeyArray(),

                ExplicitNoMap = false,
                Relationship = computedRelationship,

                IdLong = idLong,
                IdShort = idShort,
                Url = $"http://hl7.org/fhir/{_packages[sourceIndex].FhirVersionShort}/ConceptMap/{idLong}",
                Name = FhirSanitizationUtils.ReformatIdForName(idLong),
                Title = path.ImplicitBasedOnIds
                    ? $"Assumed mapping of {sourceVs.UnversionedUrl} from {_packages[sourceIndex].ShortName} to {_packages[targetIndex].ShortName} based on ID"
                    : $"Transitive mapping of {sourceVs.UnversionedUrl} from {_packages[sourceIndex].ShortName} to {_packages[targetIndex].ShortName}",

                TechnicalNotes = $"Computed transitively through {string.Join("->", path.Ids.Where(s => s != null))}",
            };

            mappingCache.CacheAdd(rec);
        }
    }

    /// <summary>
    /// Builds transitive mappings for all non-adjacent version pairs of structures
    /// </summary>
    public void BuildTransitiveStructureMappings(FhirArtifactClassEnum? artifactClass)
    {
        if (artifactClass is null)
        {
            _logger.LogInformation("Building transitive structure mappings...");
        }
        else
        {
            _logger.LogInformation($"Building transitive structure mappings for {artifactClass}...");
        }

        // iterate over all the packages to use as source
        for (int sourceIndex = 0; sourceIndex < _packages.Count; sourceIndex++)
        {
            DbFhirPackage sourcePackage = _packages[sourceIndex];

            // iterate upwards first (skip immediate neigbor)
            for (int targetIndex = sourceIndex + 2; targetIndex < _packages.Count; targetIndex++)
            {
                DbFhirPackage targetPackage = _packages[targetIndex];
                int hopCount = Math.Abs(targetIndex - sourceIndex);
                _logger.LogInformation(
                    $"Building transitive structure mappings: {sourcePackage.ShortName} -> {targetPackage.ShortName} ({hopCount} hops)");
                buildTransitiveStructureMappings(sourcePackage, targetPackage, sourceIndex, targetIndex, artifactClass);
            }

            // iterate downwards (skip immediate neigbor)
            for (int targetIndex = sourceIndex - 2; targetIndex >= 0; targetIndex--)
            {
                DbFhirPackage targetPackage = _packages[targetIndex];
                int hopCount = Math.Abs(targetIndex - sourceIndex);
                _logger.LogInformation(
                    $"Building transitive structure mappings: {sourcePackage.ShortName} -> {targetPackage.ShortName} ({hopCount} hops)");
                buildTransitiveStructureMappings(sourcePackage, targetPackage, sourceIndex, targetIndex, artifactClass);
            }
        }
    }

    private void buildTransitiveStructureMappings(
        DbFhirPackage sourcePackage,
        DbFhirPackage targetPackage,
        int sourceIndex,
        int targetIndex,
        FhirArtifactClassEnum? artifactClass)
    {
        DbArtifactMapCache<DbStructureMappingRecord> mappingCache = new();

        // get all direct mappings in the chain
        List<List<DbStructureMappingRecord>> chainMappings = [];

        int increment = sourceIndex < targetIndex ? 1 : -1;
        int hops = Math.Abs(targetIndex - sourceIndex);

        int currentSourceIndex = sourceIndex;
        int currentTargetIndex = sourceIndex + increment;

        for (int step = 0; step < hops; step++)
        {
            DbFhirPackage fromPkg = _packages[currentSourceIndex];
            DbFhirPackage toPkg = _packages[currentTargetIndex];

            List<DbStructureMappingRecord> directMaps = DbStructureMappingRecord.SelectList(
                _db,
                SourceFhirPackageKey: fromPkg.Key,
                TargetFhirPackageKey: toPkg.Key);

            chainMappings.Add(directMaps);

            // increment our indices
            currentSourceIndex += increment;
            currentTargetIndex += increment;
        }

        // Get all source structures
        List<DbStructureDefinition> sourceStructures = DbStructureDefinition.SelectList(
            _db,
            FhirPackageKey: sourcePackage.Key,
            ArtifactClass: artifactClass);

        foreach (DbStructureDefinition sourceSd in sourceStructures)
        {
            // trace this structure through the chain (may produce multiple paths)
            traceStructureThroughChain(
                sourceSd,
                chainMappings,
                sourceIndex,
                targetIndex,
                mappingCache);
        }

        if (mappingCache.ToAddCount > 0)
        {
            mappingCache.ToAdd.Insert(_db, ignoreDuplicates: true, insertPrimaryKey: true);
            _logger.LogInformation($"  Created {mappingCache.ToAddCount} transitive structure mappings");
        }

        if (mappingCache.ToUpdateCount > 0)
        {
            mappingCache.ToUpdate.Update(_db);
            _logger.LogInformation($"  Updated {mappingCache.ToUpdateCount} transitive structure mappings");
        }
    }

    private void traceStructureThroughChain(
        DbStructureDefinition sourceStructure,
        List<List<DbStructureMappingRecord>> chainMappings,
        int sourceIndex,
        int targetIndex,
        DbArtifactMapCache<DbStructureMappingRecord> mappingCache)
    {
        // Each path tracks: currentKey, currentId, relationships, structureIds, versionKeys
        List<MappingPath> activePaths =
        [
            new MappingPath
            {
                CurrentKey = sourceStructure.Key,
                PreviousKey = null,
                CurrentId = sourceStructure.Id,
                Relationships = [],
                Ids = [sourceStructure.Id],
                VersionKeys = new Dictionary<FhirReleases.FhirSequenceCodes, int?>
                {
                    [_packages[sourceIndex].DefinitionFhirSequence] = sourceStructure.Key
                }
            }
        ];

        int increment = sourceIndex < targetIndex ? 1 : -1;
        int hopIndex = sourceIndex + increment;

        // Walk through each step, expanding paths when multiple mappings exist
        for (int step = 0; step < chainMappings.Count; step++)
        {
            DbFhirPackage hopPackage = _packages[hopIndex];
            List<MappingPath> nextPaths = [];

            foreach (MappingPath path in activePaths)
            {
                if (path.CurrentKey == null)
                {
                    // Structure doesn't exist from this point forward - continue path as-is
                    MappingPath continued = path.Clone();
                    continued.PreviousKey = path.CurrentKey;
                    continued.Ids.Add(null);
                    continued.VersionKeys[hopPackage.DefinitionFhirSequence] = null;
                    continued.Relationships.Add(null);
                    nextPaths.Add(continued);
                    continue;
                }

                // Find ALL mappings for this structure in this step
                List<DbStructureMappingRecord> mappings = chainMappings[step]
                    .Where(m => m.SourceStructureKey == path.CurrentKey)
                    .ToList();

                if (mappings.Count == 0)
                {
                    // check to see if there is a structure with the same id in the target package
                    DbStructureDefinition? possibleSd = DbStructureDefinition.SelectSingle(
                        _db,
                        FhirPackageKey: hopPackage.Key,
                        Id: path.CurrentId);

                    if (possibleSd is null)
                    {
                        // No mapping found - structure is unmapped at this step
                        MappingPath continued = path.Clone();
                        continued.CurrentKey = null;
                        continued.PreviousKey = path.CurrentKey;
                        continued.CurrentId = null;
                        continued.Ids.Add(null);
                        continued.VersionKeys[hopPackage.DefinitionFhirSequence] = null;
                        continued.Relationships.Add(null);
                        nextPaths.Add(continued);
                    }
                    else
                    {
                        // Create a new path for this possible match
                        MappingPath newPath = path.Clone();
                        newPath.Relationships.Add(CMR.Equivalent);
                        newPath.CurrentKey = possibleSd.Key;
                        newPath.PreviousKey = path.CurrentKey;
                        newPath.CurrentId = possibleSd.Id;
                        newPath.Ids.Add(possibleSd.Id);
                        newPath.VersionKeys[hopPackage.DefinitionFhirSequence] = possibleSd.Key;
                        newPath.ImplicitBasedOnIds = true;
                        nextPaths.Add(newPath);
                    }
                }
                else
                {
                    // Create a new path for each mapping (handles 1:N splits)
                    foreach (DbStructureMappingRecord mapping in mappings)
                    {
                        MappingPath newPath = path.Clone();
                        newPath.Relationships.Add(mapping.Relationship);
                        newPath.CurrentKey = mapping.TargetStructureKey;
                        newPath.PreviousKey = path.CurrentKey;
                        newPath.CurrentId = mapping.TargetStructureId;
                        newPath.Ids.Add(mapping.TargetStructureId);
                        newPath.VersionKeys[hopPackage.DefinitionFhirSequence] = mapping.TargetStructureKey;
                        nextPaths.Add(newPath);
                    }
                }
            }

            activePaths = nextPaths;
            hopIndex += increment;
        }

        // Convert all completed paths to records
        foreach (MappingPath path in activePaths)
        {
            createStructureMappingRecord(
                sourceStructure,
                path,
                sourceIndex,
                targetIndex,
                mappingCache);
        }
    }

    private void createStructureMappingRecord(
        DbStructureDefinition sourceStructure,
        MappingPath path,
        int sourceIndex,
        int targetIndex,
        DbArtifactMapCache<DbStructureMappingRecord> mappingCache)
    {
        CMR? computedRelationship = RelationshipComposition.ComposeChain(path.Relationships);

        (string idLong, string idShort) = XVerProcessor.GenerateArtifactId(
            _packages[sourceIndex].ShortName,
            sourceStructure.Id,
            _packages[targetIndex].ShortName,
            path.CurrentId);

        // check to see if there is an existing record
        DbStructureMappingRecord? rec = DbStructureMappingRecord.SelectSingle(
            _db,
            IdLong: idLong);

        if (rec is null)
        {
            // check for cached record
            mappingCache.TryGet(idLong, out rec);
        }

        if (rec is not null)
        {
            // update with current data
            rec.ContentKeys = path.GetVersionKeyArray();

            rec.TechnicalNotes = (rec.TechnicalNotes ?? string.Empty) + 
                $" Computed transitively through {string.Join("->", path.Ids.Where(s => s != null))}";

            mappingCache.CacheUpdate(rec);
        }
        else
        {
            // create a new record
            rec = new()
            {
                Key = DbStructureMappingRecord.GetIndex(),
                PreviousStepMapRecordKey = path.PreviousKey,
                Steps = Math.Abs(sourceIndex - targetIndex),

                SourceFhirPackageKey = _packages[sourceIndex].Key,
                SourceFhirSequence = _packages[sourceIndex].DefinitionFhirSequence,
                SourceStructureKey = sourceStructure.Key,
                SourceStructureId = sourceStructure.Id,

                TargetFhirPackageKey = _packages[targetIndex].Key,
                TargetFhirSequence = _packages[targetIndex].DefinitionFhirSequence,
                TargetStructureKey = path.CurrentKey,
                TargetStructureId = path.CurrentId,

                ContentKeys = path.GetVersionKeyArray(),

                ExplicitNoMap = false,
                Relationship = computedRelationship,

                FmlExists = false,
                FmlUrl = null,
                FmlFilename = null,

                IdLong = idLong,
                IdShort = idShort,
                Url = $"http://hl7.org/fhir/{_packages[sourceIndex].FhirVersionShort}/ConceptMap/{idLong}",
                Name = FhirSanitizationUtils.ReformatIdForName(idLong),
                Title = path.ImplicitBasedOnIds
                    ? $"Assumed mapping of {sourceStructure.Name} from {_packages[sourceIndex].ShortName} to {_packages[targetIndex].ShortName} based on ID"
                    : $"Transitive mapping of {sourceStructure.Name} from {_packages[sourceIndex].ShortName} to {_packages[targetIndex].ShortName}",

                TechnicalNotes = $"Computed transitively through {string.Join("->", path.Ids.Where(s => s != null))}",
            };

            mappingCache.CacheAdd(rec);
        }
    }

    private class MappingPath
    {
        public int? CurrentKey { get; set; }
        public int? PreviousKey { get; set; }
        public string? CurrentId { get; set; }
        public List<CMR?> Relationships { get; set; } = [];
        public List<string?> Ids { get; set; } = [];
        public Dictionary<FhirReleases.FhirSequenceCodes, int?> VersionKeys { get; set; } = [];
        public bool ImplicitBasedOnIds { get; set; } = false;

        public int?[] GetVersionKeyArray()
        {
            return new int?[]
            {
                VersionKeys.GetValueOrDefault(FhirReleases.FhirSequenceCodes.DSTU2),
                VersionKeys.GetValueOrDefault(FhirReleases.FhirSequenceCodes.STU3),
                VersionKeys.GetValueOrDefault(FhirReleases.FhirSequenceCodes.R4),
                VersionKeys.GetValueOrDefault(FhirReleases.FhirSequenceCodes.R4B),
                VersionKeys.GetValueOrDefault(FhirReleases.FhirSequenceCodes.R5),
                VersionKeys.GetValueOrDefault(FhirReleases.FhirSequenceCodes.R6),
            };
        }

        public MappingPath Clone()
        {
            return new MappingPath
            {
                CurrentKey = CurrentKey,
                CurrentId = CurrentId,
                Relationships = [.. Relationships],
                Ids = [.. Ids],
                VersionKeys = new Dictionary<FhirReleases.FhirSequenceCodes, int?>(VersionKeys)
            };
        }
    }
}
