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
    private readonly IDbConnection _dbConnection;
    private ILoggerFactory? _loggerFactory;
    private readonly ILogger _logger;
    private readonly List<DbFhirPackage> _packages;

    public TransitiveMappingBuilder(
        IDbConnection dbConnection,
        ILoggerFactory? loggerFactory,
        List<DbFhirPackage> packages)
    {
        _dbConnection = dbConnection;
        _loggerFactory = loggerFactory;
        _logger = loggerFactory?.CreateLogger<TransitiveMappingBuilder>()
            ?? LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<TransitiveMappingBuilder>();
        _packages = packages;
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
                _dbConnection,
                SourceFhirPackageKey: fromPkg.Key,
                TargetFhirPackageKey: toPkg.Key);

            chainMappings.Add(directMaps);

            // increment our indices
            currentSourceIndex += increment;
            currentTargetIndex += increment;
        }

        // Get all source structures
        List<DbStructureDefinition> sourceStructures = DbStructureDefinition.SelectList(
            _dbConnection,
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
            mappingCache.ToAdd.Insert(_dbConnection, ignoreDuplicates: true, insertPrimaryKey: true);
            _logger.LogInformation($"  Created {mappingCache.ToAddCount} transitive structure mappings");
        }

        if (mappingCache.ToUpdateCount > 0)
        {
            mappingCache.ToUpdate.Update(_dbConnection);
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
                CurrentId = sourceStructure.Id,
                Relationships = [],
                CdRelationships = [],
                VdRelationships = [],
                StructureIds = [sourceStructure.Id],
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
                    continued.StructureIds.Add(null);
                    continued.VersionKeys[hopPackage.DefinitionFhirSequence] = null;
                    continued.Relationships.Add(null);
                    continued.CdRelationships.Add(null);
                    continued.VdRelationships.Add(null);
                    nextPaths.Add(continued);
                    continue;
                }

                // Find ALL mappings for this structure in this step
                List<DbStructureMappingRecord> mappings = chainMappings[step]
                    .Where(m => m.SourceStructureKey == path.CurrentKey)
                    .ToList();

                if (mappings.Count == 0)
                {
                    // No mapping found - structure is unmapped at this step
                    MappingPath continued = path.Clone();
                    continued.CurrentKey = null;
                    continued.CurrentId = null;
                    continued.StructureIds.Add(null);
                    continued.VersionKeys[hopPackage.DefinitionFhirSequence] = null;
                    continued.Relationships.Add(null);
                    continued.CdRelationships.Add(null);
                    continued.VdRelationships.Add(null);
                    nextPaths.Add(continued);
                }
                else
                {
                    // Create a new path for each mapping (handles 1:N splits)
                    foreach (DbStructureMappingRecord mapping in mappings)
                    {
                        MappingPath newPath = path.Clone();
                        newPath.Relationships.Add(mapping.Relationship);
                        newPath.CdRelationships.Add(mapping.ConceptDomainRelationship);
                        newPath.VdRelationships.Add(mapping.ValueDomainRelationship);
                        newPath.CurrentKey = mapping.TargetStructureKey;
                        newPath.CurrentId = mapping.TargetStructureId;
                        newPath.StructureIds.Add(mapping.TargetStructureId);
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
        CMR? cdRelationship = RelationshipComposition.ComposeChain(path.CdRelationships);
        CMR? vdRelationship = RelationshipComposition.ComposeChain(path.VdRelationships);

        (string idLong, string idShort) = XVerProcessor.GenerateArtifactId(
            _packages[sourceIndex].ShortName,
            sourceStructure.Id,
            _packages[targetIndex].ShortName,
            path.CurrentId);

        // check to see if there is an existing record
        DbStructureMappingRecord? rec = DbStructureMappingRecord.SelectSingle(
            _dbConnection,
            IdLong: idLong);

        if (rec is null)
        {
            // check for cached record
            mappingCache.TryGet(idLong, out rec);
        }

        if (rec is not null)
        {
            // update with current data
            rec.StructureKeyR2 = path.VersionKeys.GetValueOrDefault(FhirReleases.FhirSequenceCodes.DSTU2);
            rec.StructureKeyR3 = path.VersionKeys.GetValueOrDefault(FhirReleases.FhirSequenceCodes.STU3);
            rec.StructureKeyR4 = path.VersionKeys.GetValueOrDefault(FhirReleases.FhirSequenceCodes.R4);
            rec.StructureKeyR4B = path.VersionKeys.GetValueOrDefault(FhirReleases.FhirSequenceCodes.R4B);
            rec.StructureKeyR5 = path.VersionKeys.GetValueOrDefault(FhirReleases.FhirSequenceCodes.R5);
            rec.StructureKeyR6 = path.VersionKeys.GetValueOrDefault(FhirReleases.FhirSequenceCodes.R6);

            rec.ComputedRelationship ??= computedRelationship;
            rec.ConceptDomainRelationship ??= cdRelationship;
            rec.ValueDomainRelationship ??= vdRelationship;

            rec.TechnicalNotes = (rec.TechnicalNotes ?? string.Empty) + 
                $" Computed transitively through {string.Join("->", path.StructureIds.Where(s => s != null))}";

            mappingCache.CacheUpdate(rec);
        }
        else
        {
            // create a new record
            rec = new()
            {
                Key = DbStructureMappingRecord.GetIndex(),
                SourceFhirPackageKey = _packages[sourceIndex].Key,
                TargetFhirPackageKey = _packages[targetIndex].Key,
                SourceStructureKey = sourceStructure.Key,
                SourceStructureId = sourceStructure.Id,
                TargetStructureKey = path.CurrentKey,
                TargetStructureId = path.CurrentId,

                StructureKeyR2 = path.VersionKeys.GetValueOrDefault(FhirReleases.FhirSequenceCodes.DSTU2),
                StructureKeyR3 = path.VersionKeys.GetValueOrDefault(FhirReleases.FhirSequenceCodes.STU3),
                StructureKeyR4 = path.VersionKeys.GetValueOrDefault(FhirReleases.FhirSequenceCodes.R4),
                StructureKeyR4B = path.VersionKeys.GetValueOrDefault(FhirReleases.FhirSequenceCodes.R4B),
                StructureKeyR5 = path.VersionKeys.GetValueOrDefault(FhirReleases.FhirSequenceCodes.R5),
                StructureKeyR6 = path.VersionKeys.GetValueOrDefault(FhirReleases.FhirSequenceCodes.R6),

                Relationship = computedRelationship,
                ComputedRelationship = computedRelationship,
                ConceptDomainRelationship = cdRelationship,
                ValueDomainRelationship = vdRelationship,

                FmlExists = false,
                FmlUrl = null,
                FmlFilename = null,

                IdLong = idLong,
                IdShort = idShort,
                Url = $"http://hl7.org/fhir/{_packages[sourceIndex].FhirVersionShort}/ConceptMap/{idLong}",
                Name = FhirSanitizationUtils.ReformatIdForName(idLong),
                Title = $"Transitive mapping of {sourceStructure.Name} from {_packages[sourceIndex].ShortName} to {_packages[targetIndex].ShortName}",

                TechnicalNotes = $"Computed transitively through {string.Join("->", path.StructureIds.Where(s => s != null))}",
            };

            mappingCache.CacheAdd(rec);
        }
    }

    private class MappingPath
    {
        public int? CurrentKey { get; set; }
        public string? CurrentId { get; set; }
        public List<CMR?> Relationships { get; set; } = [];
        public List<CMR?> CdRelationships { get; set; } = [];
        public List<CMR?> VdRelationships { get; set; } = [];
        public List<string?> StructureIds { get; set; } = [];
        public Dictionary<FhirReleases.FhirSequenceCodes, int?> VersionKeys { get; set; } = [];

        public MappingPath Clone()
        {
            return new MappingPath
            {
                CurrentKey = CurrentKey,
                CurrentId = CurrentId,
                Relationships = [.. Relationships],
                CdRelationships = [.. CdRelationships],
                VdRelationships = [.. VdRelationships],
                StructureIds = [.. StructureIds],
                VersionKeys = new Dictionary<FhirReleases.FhirSequenceCodes, int?>(VersionKeys)
            };
        }
    }
}
