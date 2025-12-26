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


    public FhirMappingComparerVs(
        IDbConnection db,
        ILoggerFactory? loggerFactory)
    {
        _db = db;
        _loggerFactory = loggerFactory;
        _logger = _loggerFactory?.CreateLogger<FhirMappingComparerVs>()
            ?? LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<FhirMappingComparerVs>(); ;
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
            }

            // iterate downward over packages to use as target
            for (int j = i - 1; j >= 0; j--)
            {
                DbFhirPackage targetPackage = packages[j];
                doComparisonsVs(sourcePackage, targetPackage);
            }
        }
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
        DbRecordCache<DbValueSetOutcome> vsOutcomeCache = new();
        DbRecordCache<DbValueSetConceptOutcome> conceptOutcomeCache = new();

        List<DbValueSet> sourceValueSets = DbValueSet.SelectList(
            _db,
            FhirPackageKey: sourcePackage.Key,
            IsExcluded: false);

        // iterate over the source value sets
        foreach (DbValueSet sourceVs in sourceValueSets)
        {
            processValueSet(
                sourcePackage,
                targetPackage,
                sourceVs,
                vsOutcomeCache,
                conceptOutcomeCache);
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
        if (vsOutcomeCache.ToAddCount > 0)
        {
            _logger.LogInformation($"Adding {vsOutcomeCache.ToAddCount} value set outcomes from {sourcePackage.ShortName} to {targetPackage.ShortName}");
            vsOutcomeCache.ToAdd.Insert(_db, ignoreDuplicates: true, insertPrimaryKey: true);
        }

        if (vsOutcomeCache.ToUpdateCount > 0)
        {
            _logger.LogInformation($"Updating {vsOutcomeCache.ToUpdateCount} value set outcomes from {sourcePackage.ShortName} to {targetPackage.ShortName}");
            vsOutcomeCache.ToUpdate.Update(_db);
        }

        if (conceptOutcomeCache.ToAddCount > 0)
        {
            _logger.LogInformation($"Adding {conceptOutcomeCache.ToAddCount} value set concept outcomes from {sourcePackage.ShortName} to {targetPackage.ShortName}");
            conceptOutcomeCache.ToAdd.Insert(_db, ignoreDuplicates: true, insertPrimaryKey: true);
        }

        if (conceptOutcomeCache.ToUpdateCount > 0)
        {
            _logger.LogInformation($"Updating {conceptOutcomeCache.ToUpdateCount} value set concept outcomes from {sourcePackage.ShortName} to {targetPackage.ShortName}");
            conceptOutcomeCache.ToUpdate.Update(_db);
        }
    }

    private void processValueSet(
        DbFhirPackage sourcePackage,
        DbFhirPackage targetPackage,
        DbValueSet sourceVs,
        DbRecordCache<DbValueSetOutcome> vsOutcomeCache,
        DbRecordCache<DbValueSetConceptOutcome> conceptOutcomeCache)
    {
        // get the value set mappings for this source value set to the target package
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

        // resolve the list of target value sets
        Dictionary<int, (DbValueSet vs, List<DbValueSetConcept> concepts)> targetValueSets = [];
        foreach (DbValueSetMappingRecord vsMapping in vsMappings.Values)
        {
            if (vsMapping.TargetValueSetKey is null)
            {
                continue;
            }

            DbValueSet? targetVs = DbValueSet.SelectSingle(
                _db,
                Key: vsMapping.TargetValueSetKey.Value);
            if (targetVs is not null)
            {
                targetValueSets[targetVs.Key] = (targetVs, DbValueSetConcept.SelectList(_db, ValueSetKey: targetVs.Key));
            }
        }

        // if there are no targets, all outcomes are no-maps

        ILookup<string, DbValueSetConcept> targetConceptsByFhirKey = targetValueSets
            .Values
            .SelectMany(tv => tv.concepts)
            .ToLookup(c => c.FhirKey);
        ILookup<string, DbValueSetConcept> targetConceptsByCode = targetValueSets
            .Values
            .SelectMany(tv => tv.concepts)
            .ToLookup(c => c.Code);

        // resolve the source concepts
        List<DbValueSetConcept> sourceConcepts = DbValueSetConcept.SelectList(
            _db,
            ValueSetKey: sourceVs.Key);

        // iterate over the source concepts for this value set
        foreach (DbValueSetConcept sourceConcept in sourceConcepts)
        {
            // check for existing records
            List<DbValueSetConceptMappingRecord> conceptMappings = DbValueSetConceptMappingRecord.SelectList(
                _db,
                SourceValueSetConceptKey: sourceConcept.Key,
                ValueSetMappingKeyValues: vsMappingKeyValues);

            // if there are no mappings, check for possible target concepts
            if (conceptMappings.Count == 0)
            {
                List<DbValueSetConcept> possibleTargetConcepts = [];

                // first try to match on FHIR key
                List<DbValueSetConcept> matchesByFhirKey = targetConceptsByFhirKey[sourceConcept.FhirKey].ToList();
                possibleTargetConcepts.AddRange(matchesByFhirKey);

                // if no matches, try to match on just code
                if (possibleTargetConcepts.Count == 0)
                {
                    List<DbValueSetConcept> matchesByCode = targetConceptsByCode[sourceConcept.Code].ToList();
                    possibleTargetConcepts.AddRange(matchesByCode);
                }

                // if still no matches, create no-maps for each mapping
                if (possibleTargetConcepts.Count == 0)
                {
                    foreach (DbValueSetMappingRecord vsMapRec in vsMappings.Values)
                    {
                        //DbValueSetConceptMappingRecord conceptMapRec = createMappingRecord(
                        //        sourcePackage,
                        //        targetPackage,
                        //        vsMapRec,
                        //        sourceVs,
                        //        targetVs: null,
                        //        sourceConcept,
                        //        targetConcept: null,
                        //        relationship: null);

                        //conceptMappingCache.CacheAdd(conceptMapRec);
                        //conceptMappings.Add(conceptMapRec);
                    }
                    continue;
                }

                // if one match, create equivalent mapping
                if (possibleTargetConcepts.Count == 1)
                {
                    foreach (DbValueSetMappingRecord vsMapRec in vsMappings.Values)
                    {
                        //DbValueSetConceptMappingRecord conceptMapRec = createMappingRecord(
                        //        sourcePackage,
                        //        targetPackage,
                        //        vsMapRec,
                        //        sourceVs,
                        //        targetVs: vsMapRec.TargetValueSetKey is null
                        //            ? null
                        //            : targetValueSets[vsMapRec.TargetValueSetKey.Value].vs,
                        //        sourceConcept,
                        //        targetConcept: possibleTargetConcepts[0],
                        //        relationship: CMR.Equivalent);

                        //conceptMappingCache.CacheAdd(conceptMapRec);
                        //conceptMappings.Add(conceptMapRec);
                    }
                    continue;
                }

                // multiple possibleTargetConcepts found - create broader-than mappings
                foreach (DbValueSetMappingRecord vsMapRec in vsMappings.Values)
                {
                    foreach (DbValueSetConcept match in possibleTargetConcepts)
                    {
                        //DbValueSetConceptMappingRecord conceptMapRec = createMappingRecord(
                        //        sourcePackage,
                        //        targetPackage,
                        //        vsMapRec,
                        //        sourceVs,
                        //        targetVs: vsMapRec.TargetValueSetKey is null
                        //            ? null
                        //            : targetValueSets[vsMapRec.TargetValueSetKey.Value].vs,
                        //        sourceConcept,
                        //        targetConcept: match,
                        //        relationship: CMR.SourceIsBroaderThanTarget);

                        //conceptMappingCache.CacheAdd(conceptMapRec);
                        //conceptMappings.Add(conceptMapRec);
                    }
                }
                continue;
            }


            // iterate over concept mappings to create outcomes
            foreach (DbValueSetConceptMappingRecord conceptMapping in conceptMappings)
            {
                // resolve the vs mapping we are using
                DbValueSetMappingRecord vsMapping = vsMappings[conceptMapping.ValueSetMappingKey];

                // resolve the target info
                DbValueSet? targetVs = vsMapping.TargetValueSetKey is null
                    ? null
                    : targetValueSets[vsMapping.TargetValueSetKey.Value].vs;

                // create the no-map outcome record
                DbValueSetConceptOutcome outcomeRec = createOutcome(
                    sourcePackage,
                    targetPackage,
                    sourceVs,
                    targetVs,
                    sourceConcept,
                    targetConcept: conceptMapping.TargetValueSetConceptKey is null
                        ? null
                        : targetValueSets[vsMapping.TargetValueSetKey!.Value].concepts
                            .First(c => c.Key == conceptMapping.TargetValueSetConceptKey.Value),
                    vsMapping,
                    conceptMapping);


                string? userMessage = conceptMapping.Comments;

                if (userMessage is null)
                {
                    if (targetVs is null)
                    {
                        userMessage = $"The concept `{sourceConcept.System}#{sourceConcept.Code}` ({sourceConcept.Display}) from" +
                            $" Value Set `{sourceVs.VersionedUrl}` in" +
                            $" FHIR {sourcePackage.ShortName} has no representation in" +
                            $" FHIR {targetPackage.ShortName}";


                        continue;
                    }
                    userMessage = $"The concept `{sourceConcept.System}#{sourceConcept.Code}` ({sourceConcept.Display}) from" +
                        $" Value Set `{sourceVs.VersionedUrl}` from" +
                        $" FHIR {sourcePackage.ShortName} has no representation in" +
                        $" Value Set `{targetVs.VersionedUrl}` from" +
                        $" FHIR {targetPackage.ShortName}";
                }

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
