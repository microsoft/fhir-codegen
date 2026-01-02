using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using Fhir.CodeGen.Comparison.Models;
using Fhir.CodeGen.Comparison.XVer;
using Microsoft.Extensions.Logging;
using CMR = Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship;

namespace Fhir.CodeGen.Comparison.CompareTool;

public class FhirMappingComparerVs
{
    private readonly IDbConnection _db;

    private ILoggerFactory? _loggerFactory;
    private ILogger _logger;

    //DbRecordCache<DbValueSetMapping> _vsMappingCache;
    //DbRecordCache<DbValueSetConceptMapping> _conceptMappingCache;

    private DbRecordCache<DbValueSetOutcome> _vsOutcomeCache;
    private DbRecordCache<DbValueSetConceptOutcome> _conceptOutcomeCache;

    public FhirMappingComparerVs(
        IDbConnection db,
        ILoggerFactory? loggerFactory)
    {
        _db = db;
        _loggerFactory = loggerFactory;
        _logger = _loggerFactory?.CreateLogger<FhirMappingComparerVs>()
            ?? LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<FhirMappingComparerVs>();

        //_vsMappingCache = new();
        //_conceptMappingCache = new();
        _vsOutcomeCache = new();
        _conceptOutcomeCache = new();
    }

    public void CompareValueSets()
    {
        // reset outcome tables
        dropVsOutcomeTables(_db);
        createVsOutcomeTables(_db);

        List<DbFhirPackage> packages = DbFhirPackage.SelectList(
            _db,
            orderByProperties: [nameof(DbFhirPackage.PackageVersion)]);

        // we want to process closer versions first, so we do a stepped approach
        for (int stepSize = 1; stepSize < packages.Count; stepSize++)
        {
            for (int i = 0; i < packages.Count - stepSize; i++)
            {
                DbFhirPackage sourcePackage = packages[i];
                DbFhirPackage targetPackage = packages[i + stepSize];

                // ascending
                _logger.LogInformation($"Processing {sourcePackage.ShortName} -> {targetPackage.ShortName}");
                doComparisonsVs(sourcePackage, targetPackage);
                applyCachedChanges(sourcePackage, targetPackage);

                // descending
                _logger.LogInformation($"Processing {targetPackage.ShortName} -> {sourcePackage.ShortName}");
                doComparisonsVs(targetPackage, sourcePackage);
                applyCachedChanges(targetPackage, sourcePackage);
            }
        }
    }

    private void applyCachedChanges(DbFhirPackage sourcePackage, DbFhirPackage targetPackage)
    {
        if (_vsOutcomeCache.ToAddCount > 0)
        {
            _logger.LogInformation($"Adding {_vsOutcomeCache.ToAddCount} value set outcomes from {sourcePackage.ShortName} to {targetPackage.ShortName}");
            _vsOutcomeCache.ToAdd.Insert(_db, ignoreDuplicates: true, insertPrimaryKey: true);
        }

        if (_vsOutcomeCache.ToUpdateCount > 0)
        {
            _logger.LogInformation($"Updating {_vsOutcomeCache.ToUpdateCount} value set outcomes from {sourcePackage.ShortName} to {targetPackage.ShortName}");
            _vsOutcomeCache.ToUpdate.Update(_db);
        }

        if (_conceptOutcomeCache.ToAddCount > 0)
        {
            _logger.LogInformation($"Adding {_conceptOutcomeCache.ToAddCount} value set concept outcomes from {sourcePackage.ShortName} to {targetPackage.ShortName}");
            _conceptOutcomeCache.ToAdd.Insert(_db, ignoreDuplicates: true, insertPrimaryKey: true);
        }

        if (_conceptOutcomeCache.ToUpdateCount > 0)
        {
            _logger.LogInformation($"Updating {_conceptOutcomeCache.ToUpdateCount} value set concept outcomes from {sourcePackage.ShortName} to {targetPackage.ShortName}");
            _conceptOutcomeCache.ToUpdate.Update(_db);
        }

        _vsOutcomeCache.Clear();
        _conceptOutcomeCache.Clear();
    }

    private void dropVsOutcomeTables(IDbConnection db)
    {
        DbValueSetOutcome.DropTable(db);
        DbValueSetConceptOutcome.DropTable(db);
    }

    private void createVsOutcomeTables(IDbConnection db)
    {
        DbValueSetOutcome.CreateTable(db);
        DbValueSetConceptOutcome.CreateTable(db);
    }


    private void doComparisonsVs(
        DbFhirPackage sourcePackage,
        DbFhirPackage targetPackage)
    {
        List<DbValueSet> sourceValueSets = DbValueSet.SelectList(
            _db,
            FhirPackageKey: sourcePackage.Key,
            IsExcluded: false);

        _logger.LogInformation($"Performing Value Set comparisons for {sourceValueSets.Count} value sets from FHIR {sourcePackage.ShortName} to {targetPackage.ShortName}...");

        int i = 0;

        // iterate over the source value sets
        foreach (DbValueSet sourceVs in sourceValueSets)
        {
            if ((++i % 100) == 0)
            {
                _logger.LogInformation($"Processing Value Set #: {i}...");
            }

            // process this value set
            processSourceValueSet(
                sourcePackage,
                sourceVs,
                targetPackage);
        }
    }

    private void processSourceValueSet(
        DbFhirPackage sourcePackage,
        DbValueSet sourceVs,
        DbFhirPackage targetPackage)
    {
        //_logger.LogInformation($"processSourceValueSet <<< {sourceVs.VersionedUrl} from {sourcePackage.ShortName} to {targetPackage.ShortName}");

        // get all mappings for this value set from the source package to the target package
        Dictionary<int, DbValueSetMapping> vsMappings = DbValueSetMapping.SelectDict(
            _db,
            SourceFhirPackageKey: sourcePackage.Key,
            SourceValueSetKey: sourceVs.Key,
            TargetFhirPackageKey: targetPackage.Key);

        if (vsMappings.Count == 0)
        {
            throw new Exception(
                $"No value set mappings found" +
                $" from {sourcePackage.ShortName} to {targetPackage.ShortName}" +
                $" for source value set: {sourceVs.VersionedUrl}");
        }

        List<int> vsMappingKeyValues = vsMappings.Keys.ToList();

        // get all the source concepts for this value set
        List<DbValueSetConcept> sourceConcepts = DbValueSetConcept.SelectList(
            _db,
            ValueSetKey: sourceVs.Key);

        // get all the concept mappings for all the value set mappings
        List<DbValueSetConceptMapping> conceptMappings = DbValueSetConceptMapping.SelectList(
            _db,
            ValueSetMappingKeyValues: vsMappingKeyValues);

        // create a lookup of concept mappings by source concept key
        ILookup<int, DbValueSetConceptMapping> conceptMappingsBySourceKey = conceptMappings
            .ToLookup(cm => cm.SourceValueSetConceptKey);

        ILookup<(int, int?), DbValueSetConceptMapping> conceptMappingsBySourceAndTargetVs = conceptMappings
            .ToLookup(cm => (cm.SourceValueSetConceptKey, vsMappings.GetValueOrDefault(cm.ValueSetMappingKey)?.TargetValueSetKey));

        List<int> targetValueSetKeys = vsMappings.Values
            .Where(vsMap => vsMap.TargetValueSetKey is not null)
            .Select(vsMap => vsMap.TargetValueSetKey!.Value)
            .Distinct()
            .ToList();

        // get all the target value sets for all mappings (if they exist)
        Dictionary<int, DbValueSet> targetValueSets = DbValueSet.SelectDict(
            _db,
            FhirPackageKey: targetPackage.Key,
            KeyValues: targetValueSetKeys);

        // get all the target concepts for all target value sets
        Dictionary<int, DbValueSetConcept> targetConcepts = DbValueSetConcept.SelectDict(
            _db,
            ValueSetKeyValues: targetValueSetKeys);

        // batch load inverse target counts for value sets
        Dictionary<int, int> inverseVsTargetCounts = [];
        foreach (int targetVsKey in targetValueSetKeys)
        {
            inverseVsTargetCounts[targetVsKey] = DbValueSetMapping.SelectCount(
                _db,
                SourceFhirPackageKey: sourcePackage.Key,
                TargetValueSetKey: targetVsKey);
        }

        // batch load inverse target counts for concepts
        List<int> targetConceptKeys = conceptMappings
            .Where(cm => cm.TargetValueSetConceptKey is not null)
            .Select(cm => cm.TargetValueSetConceptKey!.Value)
            .Distinct()
            .ToList();

        Dictionary<int, int> inverseConceptTargetCounts = [];
        foreach (int targetConceptKey in targetConceptKeys)
        {
            inverseConceptTargetCounts[targetConceptKey] = DbValueSetConceptMapping.SelectCount(
                _db,
                SourceFhirPackageKey: sourcePackage.Key,
                TargetValueSetConceptKey: targetConceptKey);
        }

        Dictionary<int, DbValueSetOutcome> vsOutcomesByMappingKey = [];
        Dictionary<int, bool> allConceptsFullyMapByMappingKey = [];

        // iterate over the mappings to create shell outcomes
        foreach (DbValueSetMapping vsMapping in vsMappings.Values)
        {
            DbValueSet? targetVs = vsMapping.TargetValueSetKey is null
                ? null
                : targetValueSets.GetValueOrDefault(vsMapping.TargetValueSetKey.Value);

            int inverseTargetCount = targetVs is null
                ? 0
                : inverseVsTargetCounts.GetValueOrDefault(targetVs.Key);

            DbValueSetOutcome vsOutcome = new()
            {
                Key = DbValueSetOutcome.GetIndex(),
                SourceFhirPackageKey = sourcePackage.Key,
                SourceValueSetKey = sourceVs.Key,
                SourceValueSetUnversionedUrl = sourceVs.UnversionedUrl,
                SourceValueSetVersion = sourceVs.Version,

                TargetFhirPackageKey = targetPackage.Key,
                TargetValueSetKey = targetVs?.Key,
                TargetValueSetUnversionedUrl = targetVs?.UnversionedUrl,
                TargetValueSetVersion = targetVs?.Version,

                TotalTargetCount = targetValueSets.Count,
                TotalSourceCount = inverseTargetCount,

                IsRenamed = (targetValueSets.Count == 1) && (targetVs is not null) && (targetVs.Id != sourceVs.Id),
                IsUnmapped = targetVs is null,
                IsIdentical = true,
                IsEquivalent = true,
                IsBroaderThanTarget = false,
                IsNarrowerThanTarget = false,

                FullyMapsToThisTarget = true,
                FullyMapsAcrossAllTargets = false,

                ConceptDomainRelationship = null,
                ValueDomainRelationship = null,

                Comments = vsMapping.Comments ?? string.Empty,
                OutcomeAction = null,

                PotentialGenResourceType = "ValueSet",
                PotentialGenLongId = null,
                PotentialGenShortId = null,
                PotentialGenUrl = null,

                //PotentialGenResourceType = "ValueSet",
                //PotentialGenLongId = vsMapping.IdLong,
                //PotentialGenShortId = vsMapping.IdShort,
                //PotentialGenUrl = $"http://hl7.org/fhir/{sourcePackage.ShortName}/ValueSet/{vsMapping.IdLong}",
            };

            vsOutcomesByMappingKey[vsMapping.Key] = vsOutcome;
            _vsOutcomeCache.CacheAdd(vsOutcome);

            allConceptsFullyMapByMappingKey[vsMapping.Key] = true;
        }

        bool allConceptsFullyMap = true;

        // use lookup by ValueSetOutcomeKey for finalization
        Dictionary<int, List<DbValueSetConceptOutcome>> conceptOutcomesByVsOutcomeKey = [];
        foreach (DbValueSetOutcome vsOutcome in vsOutcomesByMappingKey.Values)
        {
            conceptOutcomesByVsOutcomeKey[vsOutcome.Key] = [];
        }

        // iterate over the source concepts
        foreach (DbValueSetConcept sourceConcept in sourceConcepts)
        {
            List<DbValueSetConceptOutcome> currentConceptOutcomes = [];

            List<DbValueSetConceptMapping> conceptMappingsForSource = conceptMappingsBySourceKey[sourceConcept.Key]
                .ToList();

            // determine if this concept is an 'escape valve' code
            bool isEscapeValve = XVerProcessor._escapeValveCodes.Contains(sourceConcept.Code);

            // iterate over each of the mappings for THIS SOURCE CONCEPT
            foreach (DbValueSetConceptMapping conceptMapping in conceptMappingsForSource)
            {
                // resolve the vs mapping we are using
                DbValueSetMapping vsMapping = vsMappings[conceptMapping.ValueSetMappingKey];

                // resolve the target value set (if any)
                DbValueSet? targetVs = vsMapping.TargetValueSetKey is null
                    ? null
                    : targetValueSets.GetValueOrDefault(vsMapping.TargetValueSetKey.Value);

                // resolve the target concept (if any)
                DbValueSetConcept? targetConcept = conceptMapping.TargetValueSetConceptKey is null
                    ? null
                    : targetConcepts.GetValueOrDefault(conceptMapping.TargetValueSetConceptKey.Value);

                // use pre-loaded counts
                int inverseTargetCount = targetConcept is null
                    ? 0
                    : inverseConceptTargetCounts.GetValueOrDefault(targetConcept.Key);

                // get the number of mappings from this source concept to any target concepts in the same target value set
                //int mappingCount = conceptMappingsForSource.Count;
                int mappingCount = targetVs is null
                    ? conceptMappingsForSource.Count
                    : conceptMappingsBySourceAndTargetVs[(sourceConcept.Key, targetVs.Key)].Count();

                bool codeLiteralsMatch = (targetConcept?.Code == sourceConcept.Code);
                bool conceptIsRenamed = (mappingCount == 1) && (targetConcept is not null) && (!codeLiteralsMatch);
                bool conceptIsIdentical = (mappingCount == 1) && (targetConcept is not null) && (targetConcept.FhirKey == sourceConcept.FhirKey);
                bool conceptIsEquivalent = conceptIsIdentical ||
                    ((mappingCount == 1) && (targetConcept is not null) && (conceptMapping.Relationship == CMR.Equivalent));
                bool conceptIsBroaderThanTarget = (conceptMapping.Relationship == CMR.SourceIsBroaderThanTarget) ||
                    ((mappingCount > 1) && (targetConcept is not null));
                bool conceptIsNarrowerThanTarget = (conceptMapping.Relationship == CMR.SourceIsNarrowerThanTarget) ||
                    ((inverseTargetCount > 1) && (targetConcept is not null));

                // if this is an escape-valve code, we need to compare the counts of concepts too
                if (isEscapeValve)
                {
                    conceptIsIdentical = conceptIsIdentical && (sourceVs.ActiveConcreteConceptCount == targetVs?.ActiveConcreteConceptCount);
                    conceptIsEquivalent = conceptIsEquivalent && (sourceVs.ActiveConcreteConceptCount == targetVs?.ActiveConcreteConceptCount);
                    conceptIsBroaderThanTarget = conceptIsBroaderThanTarget || (sourceVs.ActiveConcreteConceptCount < (targetVs?.ActiveConcreteConceptCount ?? 0));
                    conceptIsNarrowerThanTarget = conceptIsNarrowerThanTarget || (sourceVs.ActiveConcreteConceptCount > (targetVs?.ActiveConcreteConceptCount ?? 0));
                }

                OutcomeValueSetConceptActionCodes? conceptOutcomeAction = null;
                if (conceptIsIdentical)
                {
                    conceptOutcomeAction = OutcomeValueSetConceptActionCodes.UseConceptSameCode;
                }
                else if (conceptIsEquivalent || conceptIsNarrowerThanTarget)
                {
                    if (codeLiteralsMatch)
                    {
                        conceptOutcomeAction = OutcomeValueSetConceptActionCodes.UseConceptSameCode;
                    }
                    else
                    {
                        conceptOutcomeAction = OutcomeValueSetConceptActionCodes.UseConceptChangedCode;
                    }
                }
                else if (conceptIsBroaderThanTarget)
                {
                    if (conceptMappingsForSource.Count(cm => cm.TargetValueSetConceptKey is not null) > 1)
                    {
                        conceptOutcomeAction = OutcomeValueSetConceptActionCodes.UseOneOfMultipleCodes;
                    }
                    else
                    {
                        conceptOutcomeAction = OutcomeValueSetConceptActionCodes.UseCodeAndCrossVersion;
                    }
                }
                else if (targetConcept is null)
                {
                    if (conceptMappingsForSource.Any(cm => cm.TargetValueSetConceptKey is not null))
                    {
                        conceptOutcomeAction = OutcomeValueSetConceptActionCodes.MappedElsewhere;
                    }
                    else
                    {
                        conceptOutcomeAction = OutcomeValueSetConceptActionCodes.UseCrossVersionDefinition;
                    }
                }
                else
                {
                    Console.Write("");
                }

                int vsOutcomeKey = vsOutcomesByMappingKey[conceptMapping.ValueSetMappingKey].Key;

                // create the initial outcome record
                DbValueSetConceptOutcome conceptOutcome = new()
                {
                    Key = DbValueSetConceptOutcome.GetIndex(),
                    ValueSetOutcomeKey = vsOutcomeKey,

                    SourceFhirPackageKey = sourcePackage.Key,
                    SourceValueSetKey = sourceConcept.ValueSetKey,
                    SourceValueSetConceptKey = sourceConcept.Key,
                    SourceValueSetConceptSystem = sourceConcept.System,
                    SourceValueSetConceptCode = sourceConcept.Code,

                    TargetFhirPackageKey = targetPackage.Key,
                    TargetValueSetKey = targetVs?.Key,
                    TargetValueSetConceptKey = targetConcept?.Key,
                    TargetValueSetConceptSystem = targetConcept?.System,
                    TargetValueSetConceptCode = targetConcept?.Code,

                    TotalTargetCount = mappingCount,
                    TotalSourceCount = inverseTargetCount,

                    IsRenamed = conceptIsRenamed,
                    IsUnmapped = targetConcept is null,
                    IsIdentical = conceptIsIdentical,
                    IsEquivalent = conceptIsEquivalent,
                    IsBroaderThanTarget = conceptIsBroaderThanTarget,
                    IsNarrowerThanTarget = conceptIsNarrowerThanTarget,
                    CodeLiteralsMatch = codeLiteralsMatch,
                    CodeTreatedAsEscapeValve = isEscapeValve,

                    FullyMapsToThisTarget = conceptIsEquivalent,
                    FullyMapsAcrossAllTargets = false,

                    ConceptDomainRelationship = null,
                    ValueDomainRelationship = null,

                    Comments = vsMapping.Comments ?? string.Empty,
                    OutcomeAction = conceptOutcomeAction,
                };

                currentConceptOutcomes.Add(conceptOutcome);
                conceptOutcomesByVsOutcomeKey[vsOutcomeKey].Add(conceptOutcome);
                _conceptOutcomeCache.CacheAdd(conceptOutcome);

                // if any concept is not identical, the value set cannot be identical
                if (!conceptOutcome.IsIdentical)
                {
                    vsOutcomesByMappingKey[vsMapping.Key].IsIdentical = false;
                }
            }

            // evaluate fully-maps-across-all-targets for the concept outcomes
            bool fullyMapped = currentConceptOutcomes.Any(o => o.FullyMapsToThisTarget);

            if ((!fullyMapped) && (currentConceptOutcomes.Count(o => o.TargetValueSetConceptKey is not null) > 1))
            {
                fullyMapped = true;
            }

            if (fullyMapped)
            {
                foreach (DbValueSetConceptOutcome conceptOutcome in currentConceptOutcomes)
                {
                    conceptOutcome.FullyMapsAcrossAllTargets = true;
                }
            }

            if (!fullyMapped)
            {
                if (allConceptsFullyMap)
                {
                    allConceptsFullyMap = false;
                    foreach (DbValueSetOutcome vsOutcome in vsOutcomesByMappingKey.Values)
                    {
                        vsOutcome.FullyMapsAcrossAllTargets = false;
                    }
                }

                foreach (int key in vsMappingKeyValues)
                {
                    allConceptsFullyMapByMappingKey[key] = false;
                }
            }
        }

        // finalize outcome records using pre-built lookup
        foreach ((int vsMappingKey, DbValueSetOutcome vsOutcome) in vsOutcomesByMappingKey)
        {
            DbValueSetMapping vsMapping = vsMappings[vsMappingKey];

            DbValueSet? targetVs = vsMapping.TargetValueSetKey is null
                ? null
                : targetValueSets.GetValueOrDefault(vsMapping.TargetValueSetKey.Value);

            vsOutcome.FullyMapsAcrossAllTargets = allConceptsFullyMap;
            vsOutcome.FullyMapsToThisTarget = allConceptsFullyMapByMappingKey[vsMappingKey];

            // use pre-built lookup instead of filtering allConceptOutcomes
            List<DbValueSetConceptOutcome> outcomesList = conceptOutcomesByVsOutcomeKey[vsOutcome.Key];

            vsOutcome.IsBroaderThanTarget = ((targetVs is not null) && (sourceVs.ActiveConcreteConceptCount > targetVs.ActiveConcreteConceptCount)) ||
                outcomesList.Any(co => co.IsBroaderThanTarget || co.IsUnmapped);

            vsOutcome.IsNarrowerThanTarget = ((targetVs is not null) && (sourceVs.ActiveConcreteConceptCount < targetVs.ActiveConcreteConceptCount)) ||
                outcomesList.Any(co => co.IsNarrowerThanTarget);

            if (vsOutcome.IsIdentical)
            {
                vsOutcome.OutcomeAction = OutcomeValueSetActionCodes.UseValueSetSameName;
            }
            else if (vsOutcome.IsEquivalent)
            {
                vsOutcome.OutcomeAction = OutcomeValueSetActionCodes.UseValueSetRenamed;
            }
            else if (vsOutcome.FullyMapsAcrossAllTargets)
            {
                vsOutcome.OutcomeAction = sourceVs.Id == targetVs?.Id
                    ? OutcomeValueSetActionCodes.UseValueSetSameName
                    : OutcomeValueSetActionCodes.UseValueSetRenamed;
            }
            else if (targetVs is null)
            {
                vsOutcome.OutcomeAction = OutcomeValueSetActionCodes.UseCrossVersionDefinition;
            }
            else
            {
                vsOutcome.OutcomeAction = sourceVs.Id == targetVs?.Id
                    ? OutcomeValueSetActionCodes.UseSameNameAndCrossVersion
                    : OutcomeValueSetActionCodes.UseRenamedAndCrossVersion;
            }
        }
    }

    private DbValueSetConceptOutcome createOutcome(
        DbFhirPackage sourcePackage,
        DbFhirPackage targetPackage,
        DbValueSet sourceVs,
        DbValueSet? targetVs,
        DbValueSetConcept sourceConcept,
        DbValueSetConcept? targetConcept,
        DbValueSetMapping valueSetMappingRecord,
        DbValueSetConceptMapping conceptMapping)
    {
        string? userMessage = conceptMapping.Comments;

        if (userMessage is null)
        {
            if (targetVs is null)
            {
                userMessage = $"The concept `{sourceConcept.FhirKey}` ({sourceConcept.Display}) from" +
                    $" Value Set `{sourceVs.VersionedUrl}` in" +
                    $" FHIR {sourcePackage.ShortName} has no representation in" +
                    $" FHIR {targetPackage.ShortName}";
            }
            else if (targetConcept is null)
            {
                userMessage = $"The concept `{sourceConcept.FhirKey}` ({sourceConcept.Display}) from" +
                    $" Value Set `{sourceVs.VersionedUrl}` in" +
                    $" FHIR {sourcePackage.ShortName} has no representation in" +
                    $" FHIR {targetPackage.ShortName}" +
                    $" Value Set `{targetVs.VersionedUrl}`.";
            }
            else
            {
                userMessage = $"The concept `{sourceConcept.FhirKey}` ({sourceConcept.Display}) from" +
                        $" Value Set `{sourceVs.VersionedUrl}` from" +
                        $" FHIR {sourcePackage.ShortName} maps to" +
                        $" concept `{targetConcept.FhirKey}` in" +
                        $" Value Set `{targetVs.VersionedUrl}` from" +
                        $" FHIR {targetPackage.ShortName}";
            }
        }

        if (targetConcept is null)
        {
        }

        bool isEscapeValve = XVerProcessor._escapeValveCodes.Contains(sourceConcept.Code);

        throw new NotImplementedException();
    }


}
