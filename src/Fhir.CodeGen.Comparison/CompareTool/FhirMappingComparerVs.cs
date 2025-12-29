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

    //DbRecordCache<DbValueSetMappingRecord> _vsMappingCache;
    //DbRecordCache<DbValueSetConceptMappingRecord> _conceptMappingCache;

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

        // iterate over packages to use as source
        for (int i = 0; i < packages.Count; i++)
        {
            DbFhirPackage sourcePackage = packages[i];

            // iterate upward over packages to use as target
            for (int j = i + 1; j < packages.Count; j++)
            {
                DbFhirPackage targetPackage = packages[j];
                doComparisonsVs(sourcePackage, targetPackage);
                applyCachedChanges(sourcePackage, targetPackage);
            }

            // iterate downward over packages to use as target
            for (int j = i - 1; j >= 0; j--)
            {
                DbFhirPackage targetPackage = packages[j];
                doComparisonsVs(sourcePackage, targetPackage);
                applyCachedChanges(sourcePackage, targetPackage);
            }
        }
    }

    private void applyCachedChanges(DbFhirPackage sourcePackage, DbFhirPackage targetPackage)
    {
        //if (_vsMappingCache.ToAddCount > 0)
        //{
        //    _logger.LogInformation($"Adding {_vsMappingCache.ToAddCount} value set mappings from {sourcePackage.ShortName} to {targetPackage.ShortName}");
        //    _vsMappingCache.ToAdd.Insert(_db, ignoreDuplicates: true, insertPrimaryKey: true);
        //}

        //if (_vsMappingCache.ToUpdateCount > 0)
        //{
        //    _logger.LogInformation($"Updating {_vsMappingCache.ToUpdateCount} value set mappings from {sourcePackage.ShortName} to {targetPackage.ShortName}");
        //    _vsMappingCache.ToUpdate.Update(_db);
        //}

        //if (_conceptMappingCache.ToAddCount > 0)
        //{
        //    _logger.LogInformation($"Adding {_conceptMappingCache.ToAddCount} value set concept mappings from {sourcePackage.ShortName} to {targetPackage.ShortName}");
        //    _conceptMappingCache.ToAdd.Insert(_db, ignoreDuplicates: true, insertPrimaryKey: true);
        //}

        //if (_conceptMappingCache.ToUpdateCount > 0)
        //{
        //    _logger.LogInformation($"Updating {_conceptMappingCache.ToUpdateCount} value set concept mappings from {sourcePackage.ShortName} to {targetPackage.ShortName}");
        //    _conceptMappingCache.ToUpdate.Update(_db);
        //}

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

        //_vsMappingCache.Clear();
        //_conceptMappingCache.Clear();
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

        _logger.LogInformation($"Performing Value Set comparisons for {sourceValueSets.Count} value sets from FHIR {sourcePackage.ShortName} to {targetPackage.ShortName}");

        // iterate over the source value sets
        foreach (DbValueSet sourceVs in sourceValueSets)
        {
            processSourceValueSet(
                sourcePackage,
                sourceVs,
                targetPackage);
        }

#if false
        // iterate across the list and perform the comparisons
        foreach (DbValueSetMappingRecord vsMapRec in vsMappings)
        {
            // resolve the source (required) and target (optional) value sets
            DbValueSet sourceVs = DbValueSet.SelectSingle(
                _db,
                Key: vsMapRec.SourceValueSetKey)
                ?? throw new Exception($"Failed to resolve source value set: {vsMapRec.SourceFhirPackageKey} for mapping: {vsMapRec.Key} ({vsMapRec.IdLong})");

            DbValueSet? targetVs = DbValueSet.SelectSingle(
                _db,
                Key: vsMapRec.TargetValueSetKey);

            // update based on no-map data
            if (targetVs is null)
            {
                vsMapRec.Comments ??= $"The {sourcePackage.ShortName} Value Set" +
                    $" {sourceVs.Id} ({sourceVs.UnversionedUrl}) has no representation in" +
                    $" FHIR {targetPackage.ShortName}";

                vsMapCache.CacheUpdate(vsMapRec);
                continue;
            }


            // iterate across source concepts to compare to target
            foreach (DbValueSetConcept sourceConcept in sourceConcepts)
            {
                DbValueSetConceptMappingRecord? conceptMapRec = null;

                // check for any existing vsMappings (0 or more)
                List<DbValueSetConceptMappingRecord> currentMappings = conceptMappingsBySourceKey[sourceConcept.Key].ToList();

                if (currentMappings.Count == 0)
                {
                    // no existing mapping - see if we can find a match in the target
                    List<DbValueSetConcept> matches = targetConceptsByFhirKey[sourceConcept.FhirKey].ToList();
                    if (matches.Count == 0)
                    {
                        matches = targetConceptsByCode[sourceConcept.Code].ToList();
                    }

                    doConceptComparison(
                        sourcePackage,
                        targetPackage,
                        vsMapRec,
                        ref conceptMapRec,
                        sourceVs,
                        targetVs,
                        sourceConcept,
                        matches);


                }

                // we have vsMappings, update them with current info
                foreach (DbValueSetConceptMappingRecord currentMapping in currentMappings)
                {
                    // if there is a target concept, resolve it
                    DbValueSetConcept? targetConcept = currentMapping.TargetValueSetConceptKey is null
                        ? null
                        : targetConceptsByKey[currentMapping.TargetValueSetConceptKey.Value].FirstOrDefault();

                    string comments;

                    if (targetConcept is null)
                    {
                        if (targetVs is null)
                        {
                            comments = $"The concept `{sourceConcept.System}#{sourceConcept.Code}` ({sourceConcept.Display}) from" +
                                $" Value Set `{sourceVs.UnversionedUrl}` in" +
                                $" FHIR {sourcePackage.ShortName} has no representation in" +
                                $" FHIR {targetPackage.ShortName}";
                        }
                        else
                        {
                            comments = $"The concept `{sourceConcept.System}#{sourceConcept.Code}` ({sourceConcept.Display}) from" +
                                $" Value Set `{sourceVs.UnversionedUrl}` from" +
                                $" FHIR {sourcePackage.ShortName} has no representation in" +
                                $" Value Set `{targetVs.UnversionedUrl}` from" +
                                $" FHIR {targetPackage.ShortName}";
                        }

                        currentMapping.ExplicitNoMap = false;
                        currentMapping.Comments = comments;
                        currentMapping.TechnicalNotes = "Auto-generated";
                        currentMapping.Relationship = null;

                        conceptMapCache.CacheUpdate(currentMapping);
                        continue;
                    }

                    comments = $"The concept `{sourceConcept.System}#{sourceConcept.Code}` ({sourceConcept.Display}) from" +
                        $" Value Set `{sourceVs.UnversionedUrl}` from" +
                        $" FHIR {sourcePackage.ShortName} maps to" +
                        $" `{targetConcept.System}#{targetConcept.Code}`" +
                        $" Value Set `{targetVs!.UnversionedUrl}` from" +
                        $" FHIR {targetPackage.ShortName}";

                    currentMapping.ExplicitNoMap = false;
                    currentMapping.Comments = comments;
                    currentMapping.TechnicalNotes = "Auto-generated";
                    currentMapping.Relationship ??= CMR.Equivalent;

                    conceptMapCache.CacheUpdate(currentMapping);
                }
            }
        }
#endif

        // apply changes

    }

    private void processSourceValueSet(
        DbFhirPackage sourcePackage,
        DbValueSet sourceVs,
        DbFhirPackage targetPackage)
    {
        // get all mappings for this value set from the source package to the target package
        Dictionary<int, DbValueSetMappingRecord> vsMappings = DbValueSetMappingRecord.SelectDict(
            _db,
            SourceFhirPackageKey: sourcePackage.Key,
            SourceValueSetKey: sourceVs.Key,
            TargetFhirPackageKey: targetPackage.Key);

        // there should always be at least one mapping (a no-map, if nothing else)
        if (vsMappings.Count == 0)
        {
            throw new Exception($"No value set mappings found from {sourcePackage.ShortName} to {targetPackage.ShortName} for source value set: {sourceVs.VersionedUrl}");
        }

        List<int> vsMappingKeyValues = vsMappings.Select(vsMap => vsMap.Key).ToList();

        // get all the source concepts for this value set
        List<DbValueSetConcept> sourceConcepts = DbValueSetConcept.SelectList(
            _db,
            ValueSetKey: sourceVs.Key);

        // get all the concept mappings for all the value set mappings
        List<DbValueSetConceptMappingRecord> conceptMappings = DbValueSetConceptMappingRecord.SelectList(
            _db,
            ValueSetMappingKeyValues: vsMappingKeyValues);

        // create a lookup of concept mappings by source concept key
        ILookup<int, DbValueSetConceptMappingRecord> conceptMappingsBySourceKey = conceptMappings
            .ToLookup(cm => cm.SourceValueSetConceptKey);

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

        Dictionary<int, DbValueSetOutcome> vsOutcomesByMappingKey = [];
        Dictionary<int, bool> allConceptsFullyMapByMappingKey = [];

        // iterate over the mappings to create shell outcomes
        foreach (DbValueSetMappingRecord vsMapping in vsMappings.Values)
        {
            DbValueSet? targetVs = vsMapping.TargetValueSetKey is null
                ? null
                : targetValueSets[vsMapping.TargetValueSetKey.Value];

            int inverseTargetCount = targetVs is null
                ? 0
                : DbValueSetMappingRecord.SelectCount(
                    _db,
                    SourceFhirPackageKey: sourcePackage.Key,
                    TargetValueSetKey: targetVs.Key);

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
                PotentialGenLongId = vsMapping.IdLong,
                PotentialGenShortId = vsMapping.IdShort,
                PotentialGenUrl = $"http://hl7.org/fhir/{sourcePackage.ShortName}/ValueSet/{vsMapping.IdLong}",
            };

            vsOutcomesByMappingKey[vsMapping.Key] = vsOutcome;
            _vsOutcomeCache.CacheAdd(vsOutcome);

            allConceptsFullyMapByMappingKey[vsMapping.Key] = true;
        }

        bool allConceptsFullyMap = true;
        List<DbValueSetConceptOutcome> allConceptOutcomes = [];

        // iterate over the source concepts
        foreach (DbValueSetConcept sourceConcept in sourceConcepts)
        {
            List<DbValueSetConceptOutcome> currentConceptOutcomes = [];

            // get the concept mappings for this source concept
            List<DbValueSetConceptMappingRecord> conceptMappingsForSource = conceptMappingsBySourceKey[sourceConcept.Key]
                .ToList();

            // determine if this concept is an 'escape valve' code
            bool isEscapeValve = XVerProcessor._escapeValveCodes.Contains(sourceConcept.Code);

            // iterate over each of the mappings for this concept to create the initial outcome records
            foreach (DbValueSetConceptMappingRecord conceptMapping in conceptMappings)
            {
                // resolve the vs mapping we are using
                DbValueSetMappingRecord vsMapping = vsMappings[conceptMapping.ValueSetMappingKey];

                // resolve the target value set (if any)
                DbValueSet? targetVs = vsMapping.TargetValueSetKey is null
                    ? null
                    : targetValueSets[vsMapping.TargetValueSetKey.Value];

                // resolve the target concept (if any)
                DbValueSetConcept? targetConcept = conceptMapping.TargetValueSetConceptKey is null
                    ? null
                    : targetConcepts[conceptMapping.TargetValueSetConceptKey.Value];

                int inverseTargetCount = targetConcept is null
                    ? 0
                    : DbValueSetConceptMappingRecord.SelectCount(
                        _db,
                        SourceFhirPackageKey: sourcePackage.Key,
                        TargetValueSetConceptKey: targetConcept.Key);

                bool conceptIsRenamed = (conceptMappings.Count == 1) && (targetConcept is not null) && (targetConcept.Code != sourceConcept.Code);
                bool conceptIsIdentical = (conceptMappings.Count == 1) && (targetConcept is not null) && (targetConcept.FhirKey == sourceConcept.FhirKey);
                bool conceptIsEquivalent = conceptIsIdentical ||
                    ((conceptMappings.Count == 1) && (targetConcept is not null) && (conceptMapping.Relationship == CMR.Equivalent));
                bool conceptIsBroaderThanTarget = (conceptMapping.Relationship == CMR.SourceIsBroaderThanTarget) ||
                    ((conceptMappings.Count > 1) && (targetConcept is not null));
                bool conceptIsNarrowerThanTarget = (conceptMapping.Relationship == CMR.SourceIsNarrowerThanTarget) ||
                    ((inverseTargetCount > 1) && (targetConcept is not null));

                // if this is an escape-valve code, we need to compare the counts of concepts too
                if (isEscapeValve)
                {
                    conceptIsIdentical = conceptIsIdentical && (sourceVs.ActiveConcreteConceptCount == targetVs?.ActiveConcreteConceptCount);
                    conceptIsEquivalent = conceptIsEquivalent && (sourceVs.ActiveConcreteConceptCount == targetVs?.ActiveConcreteConceptCount);
                    conceptIsBroaderThanTarget = conceptIsBroaderThanTarget && (sourceVs.ActiveConcreteConceptCount < (targetVs?.ActiveConcreteConceptCount ?? 0));
                    conceptIsNarrowerThanTarget = conceptIsNarrowerThanTarget && (sourceVs.ActiveConcreteConceptCount > (targetVs?.ActiveConcreteConceptCount ?? 0));
                }

                OutcomeValueSetConceptActionCodes? conceptOutcomeAction = null;
                if (conceptIsIdentical)
                {
                    conceptOutcomeAction = OutcomeValueSetConceptActionCodes.UseConceptSameCode;
                }
                else if (conceptIsEquivalent)
                {
                    conceptOutcomeAction = OutcomeValueSetConceptActionCodes.UseConceptChangedCode;
                }
                else if (targetConcept is null)
                {
                    if (conceptMappings.Any(cm => cm.TargetValueSetConceptKey is not null))
                    {
                        conceptOutcomeAction = OutcomeValueSetConceptActionCodes.MappedElsewhere;
                    }
                    else
                    {
                        conceptOutcomeAction = OutcomeValueSetConceptActionCodes.UseCrossVersionDefinition;
                    }
                }

                // create the initial outcome record
                DbValueSetConceptOutcome conceptOutcome = new()
                {
                    Key = DbValueSetConceptOutcome.GetIndex(),
                    ValueSetOutcomeKey = vsOutcomesByMappingKey[conceptMapping.ValueSetMappingKey].Key,

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

                    TotalTargetCount = conceptMappings.Count,
                    TotalSourceCount = inverseTargetCount,

                    IsRenamed = conceptIsRenamed,
                    IsUnmapped = targetConcept is null,
                    IsIdentical = conceptIsIdentical,
                    IsEquivalent = conceptIsEquivalent,
                    IsBroaderThanTarget = conceptIsBroaderThanTarget,
                    IsNarrowerThanTarget = conceptIsNarrowerThanTarget,

                    FullyMapsToThisTarget = conceptIsEquivalent,
                    FullyMapsAcrossAllTargets = false,

                    ConceptDomainRelationship = null,
                    ValueDomainRelationship = null,

                    Comments = vsMapping.Comments ?? string.Empty,
                    OutcomeAction = conceptOutcomeAction,
                };

                currentConceptOutcomes.Add(conceptOutcome);
                allConceptOutcomes.Add(conceptOutcome);
                _conceptOutcomeCache.CacheAdd(conceptOutcome);

                // if any concept is not identical, the value set cannot be identical
                if (!conceptOutcome.IsIdentical)
                {
                    vsOutcomesByMappingKey[vsMapping.Key].IsIdentical = false;
                }
            }

            // evaluate fully-maps-across-all-targets for the concept outcomes (considers all target value sets)
            bool fullyMapped = currentConceptOutcomes.Any(o => o.FullyMapsToThisTarget);

            // we also have the case where a mapping is complete because there are multiple mappings
            if ((!fullyMapped) && (currentConceptOutcomes.Count(o => o.TargetValueSetConceptKey is not null) > 1))
            {
                // TODO: verify that the multiple mappings cover all target value sets
                fullyMapped = true;
            }

            // set fully-maps-across-all-targets for the concept if the outcome for this concept is mapped to fully mapped to anything
            if (fullyMapped)
            {
                foreach (DbValueSetConceptOutcome conceptOutcome in currentConceptOutcomes)
                {
                    conceptOutcome.FullyMapsAcrossAllTargets = true;
                }
            }

            // if any concept is not fully mapped, the value set cannot be fully mapped and cannot be equivalent
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

                // this concept does not map across all target value sets
                foreach (int key in vsMappingKeyValues)
                {
                    allConceptsFullyMapByMappingKey[key] = false;
                }
            }
        }

        // iterate over each of the mappings for this value set to finalize the outcome records
        foreach ((int vsMappingKey, DbValueSetOutcome vsOutcome) in vsOutcomesByMappingKey)
        {
            DbValueSetMappingRecord vsMapping = vsMappings[vsMappingKey];

            DbValueSet? targetVs = vsMapping.TargetValueSetKey is null
                ? null
                : targetValueSets[vsMapping.TargetValueSetKey.Value];

            vsOutcome.FullyMapsAcrossAllTargets = allConceptsFullyMap;
            vsOutcome.FullyMapsToThisTarget = allConceptsFullyMapByMappingKey[vsMappingKey];

            vsOutcome.IsBroaderThanTarget = ((targetVs is not null) && (sourceVs.ActiveConcreteConceptCount > targetVs.ActiveConcreteConceptCount)) ||
                allConceptOutcomes
                    .Where(co => co.ValueSetOutcomeKey == vsOutcome.Key)
                    .Any(co => co.IsBroaderThanTarget || co.IsUnmapped);

            vsOutcome.IsNarrowerThanTarget = ((targetVs is not null) && (sourceVs.ActiveConcreteConceptCount < targetVs.ActiveConcreteConceptCount)) ||
                allConceptOutcomes
                    .Where(co => co.ValueSetOutcomeKey == vsOutcome.Key)
                    .Any(co => co.IsNarrowerThanTarget);

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
                if (sourceVs.Id == targetVs?.Id)
                {
                    vsOutcome.OutcomeAction = OutcomeValueSetActionCodes.UseValueSetSameName;
                }
                else
                {
                    vsOutcome.OutcomeAction = OutcomeValueSetActionCodes.UseValueSetRenamed;
                }
            }
            else if (targetVs is null)
            {
                vsOutcome.OutcomeAction = OutcomeValueSetActionCodes.UseCrossVersionDefinition;
            }
            else
            {
                if (sourceVs.Id == targetVs?.Id)
                {
                    vsOutcome.OutcomeAction = OutcomeValueSetActionCodes.UseSameNameAndCrossVersion;
                }
                else
                {
                    vsOutcome.OutcomeAction = OutcomeValueSetActionCodes.UseRenamedAndCrossVersion;
                }
            }
        }



        //// resolve all known target value sets, their concepts and existing mappings
        //Dictionary<int, (DbValueSet vs, List<DbValueSetConcept> concepts, DbValueSetMappingRecord? vsMapping)> targetValueSets = [];
        //foreach (DbValueSetMappingRecord vsMapping in vsMappings.Values)
        //{
        //    if (vsMapping.TargetValueSetKey is null)
        //    {
        //        continue;
        //    }

        //    DbValueSet? targetVs = DbValueSet.SelectSingle(
        //        _db,
        //        Key: vsMapping.TargetValueSetKey.Value);
        //    if (targetVs is not null)
        //    {
        //        targetValueSets[targetVs.Key] = (
        //            targetVs,
        //            DbValueSetConcept.SelectList(_db, ValueSetKey: targetVs.Key),
        //            vsMapping);
        //    }
        //}

        //// if there are no targets, all outcomes are no-maps

        //ILookup<string, DbValueSetConcept> targetConceptsByFhirKey = targetValueSets
        //    .Values
        //    .SelectMany(tv => tv.concepts)
        //    .ToLookup(c => c.FhirKey);
        //ILookup<string, DbValueSetConcept> targetConceptsByCode = targetValueSets
        //    .Values
        //    .SelectMany(tv => tv.concepts)
        //    .ToLookup(c => c.Code);

        //// resolve the source concepts
        //List<DbValueSetConcept> sourceConcepts = DbValueSetConcept.SelectList(
        //    _db,
        //    ValueSetKey: sourceVs.Key);

        //// iterate over the source concepts for this value set
        //foreach (DbValueSetConcept sourceConcept in sourceConcepts)
        //{
        //    // check for existing records
        //    List<DbValueSetConceptMappingRecord> conceptMappings = DbValueSetConceptMappingRecord.SelectList(
        //        _db,
        //        SourceValueSetConceptKey: sourceConcept.Key,
        //        ValueSetMappingKeyValues: vsMappingKeyValues);

        //    // if there are no mappings, check for possible target concepts
        //    if (conceptMappings.Count == 0)
        //    {
        //        List<DbValueSetConcept> possibleTargetConcepts = [];

        //        // first try to match on FHIR key
        //        List<DbValueSetConcept> matchesByFhirKey = targetConceptsByFhirKey[sourceConcept.FhirKey].ToList();
        //        possibleTargetConcepts.AddRange(matchesByFhirKey);

        //        // if no matches, try to match on just code
        //        if (possibleTargetConcepts.Count == 0)
        //        {
        //            List<DbValueSetConcept> matchesByCode = targetConceptsByCode[sourceConcept.Code].ToList();
        //            possibleTargetConcepts.AddRange(matchesByCode);
        //        }

        //        // if still no matches, create no-maps for each mapping
        //        if (possibleTargetConcepts.Count == 0)
        //        {
        //            foreach (DbValueSetMappingRecord vsMapRec in vsMappings.Values)
        //            {
        //                //DbValueSetConceptMappingRecord conceptMapRec = createMappingRecord(
        //                //        sourcePackage,
        //                //        targetPackage,
        //                //        vsMapRec,
        //                //        sourceVs,
        //                //        targetVs: null,
        //                //        sourceConcept,
        //                //        targetConcept: null,
        //                //        relationship: null);

        //                //conceptMappingCache.CacheAdd(conceptMapRec);
        //                //conceptMappings.Add(conceptMapRec);
        //            }
        //            continue;
        //        }

        //        // if one match, create equivalent mapping
        //        if (possibleTargetConcepts.Count == 1)
        //        {
        //            foreach (DbValueSetMappingRecord vsMapRec in vsMappings.Values)
        //            {
        //                //DbValueSetConceptMappingRecord conceptMapRec = createMappingRecord(
        //                //        sourcePackage,
        //                //        targetPackage,
        //                //        vsMapRec,
        //                //        sourceVs,
        //                //        targetVs: vsMapRec.TargetValueSetKey is null
        //                //            ? null
        //                //            : targetValueSets[vsMapRec.TargetValueSetKey.Value].vs,
        //                //        sourceConcept,
        //                //        targetConcept: possibleTargetConcepts[0],
        //                //        relationship: CMR.Equivalent);

        //                //conceptMappingCache.CacheAdd(conceptMapRec);
        //                //conceptMappings.Add(conceptMapRec);
        //            }
        //            continue;
        //        }

        //        // multiple possibleTargetConcepts found - create broader-than mappings
        //        foreach (DbValueSetMappingRecord vsMapRec in vsMappings.Values)
        //        {
        //            foreach (DbValueSetConcept match in possibleTargetConcepts)
        //            {
        //                //DbValueSetConceptMappingRecord conceptMapRec = createMappingRecord(
        //                //        sourcePackage,
        //                //        targetPackage,
        //                //        vsMapRec,
        //                //        sourceVs,
        //                //        targetVs: vsMapRec.TargetValueSetKey is null
        //                //            ? null
        //                //            : targetValueSets[vsMapRec.TargetValueSetKey.Value].vs,
        //                //        sourceConcept,
        //                //        targetConcept: match,
        //                //        relationship: CMR.SourceIsBroaderThanTarget);

        //                //conceptMappingCache.CacheAdd(conceptMapRec);
        //                //conceptMappings.Add(conceptMapRec);
        //            }
        //        }
        //        continue;
        //    }


        //    // iterate over concept mappings to create outcomes
        //    foreach (DbValueSetConceptMappingRecord conceptMapping in conceptMappings)
        //    {
        //        // resolve the vs mapping we are using
        //        DbValueSetMappingRecord vsMapping = vsMappings[conceptMapping.ValueSetMappingKey];

        //        // resolve the target info
        //        DbValueSet? targetVs = vsMapping.TargetValueSetKey is null
        //            ? null
        //            : targetValueSets[vsMapping.TargetValueSetKey.Value].vs;

        //        // create the no-map outcome record
        //        DbValueSetConceptOutcome outcomeRec = createOutcome(
        //            sourcePackage,
        //            targetPackage,
        //            sourceVs,
        //            targetVs,
        //            sourceConcept,
        //            targetConcept: conceptMapping.TargetValueSetConceptKey is null
        //                ? null
        //                : targetValueSets[vsMapping.TargetValueSetKey!.Value].concepts
        //                    .First(c => c.Key == conceptMapping.TargetValueSetConceptKey.Value),
        //            vsMapping,
        //            conceptMapping);


        //        string? userMessage = conceptMapping.Comments;

        //        if (userMessage is null)
        //        {
        //            if (targetVs is null)
        //            {
        //                userMessage = $"The concept `{sourceConcept.System}#{sourceConcept.Code}` ({sourceConcept.Display}) from" +
        //                    $" Value Set `{sourceVs.VersionedUrl}` in" +
        //                    $" FHIR {sourcePackage.ShortName} has no representation in" +
        //                    $" FHIR {targetPackage.ShortName}";


        //                continue;
        //            }
        //            userMessage = $"The concept `{sourceConcept.System}#{sourceConcept.Code}` ({sourceConcept.Display}) from" +
        //                $" Value Set `{sourceVs.VersionedUrl}` from" +
        //                $" FHIR {sourcePackage.ShortName} has no representation in" +
        //                $" Value Set `{targetVs.VersionedUrl}` from" +
        //                $" FHIR {targetPackage.ShortName}";
        //        }

        //    }
        //}

    }

    private DbValueSetConceptOutcome createOutcome(
        DbFhirPackage sourcePackage,
        DbFhirPackage targetPackage,
        DbValueSet sourceVs,
        DbValueSet? targetVs,
        DbValueSetConcept sourceConcept,
        DbValueSetConcept? targetConcept,
        DbValueSetMappingRecord valueSetMappingRecord,
        DbValueSetConceptMappingRecord conceptMapping)
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
