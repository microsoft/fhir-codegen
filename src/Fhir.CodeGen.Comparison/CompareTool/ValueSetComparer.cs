using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fhir.CodeGen.Comparison.Models;
using Fhir.CodeGen.Comparison.XVer;
using Microsoft.Extensions.Logging;
using CMR = Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship;

namespace Fhir.CodeGen.Comparison.CompareTool;

public class ValueSetComparer
{
    private class ValueSetComparisonTrackingRecord
    {
        public required DbFhirPackage SourcePackage { get; init; }
        public required DbValueSet SourceValueSet { get; init; }
        public required DbFhirPackage TargetPackage { get; set; }
        public required DbValueSet? TargetValueSet { get; set; }
        public int? TargetValueSetKey { get; set; } = null;

        public DbValueSetMapping? ExplicitMapping { get; set; } = null;
        public DbMappingSourceFile? ExplicitMappingSource { get; set; } = null;

        public int? ComparisonRecordKey { get; set; } = null;
        public DbValueSetComparison? ComparisonRecord { get; set; } = null;

        public DbValueSet?[] Contents { get; set; } = new DbValueSet?[6];
        public int?[] ContentKeys { get; set; } = new int?[6];
        public List<DbValueSetComparison> ComparisonSteps { get; set; } = [];

        public ValueSetComparisonTrackingRecord Clone() => new()
        {
            SourcePackage = this.SourcePackage,
            SourceValueSet = this.SourceValueSet,
            TargetPackage = this.TargetPackage,
            TargetValueSet = this.TargetValueSet,
            TargetValueSetKey = this.TargetValueSetKey,
            ExplicitMapping = this.ExplicitMapping,
            ExplicitMappingSource = this.ExplicitMappingSource,
            ComparisonRecordKey = this.ComparisonRecordKey,
            ComparisonRecord = this.ComparisonRecord,
            Contents = this.Contents.Select(v => v).ToArray(),
            ContentKeys = this.ContentKeys.Select(v => v).ToArray(),
            ComparisonSteps = this.ComparisonSteps.Select(v => v).ToList(),
        };
    }

    private class ConceptTrackingRecord
    {
        public required DbValueSetConcept SourceConcept { get; init; }
        public int? CurrentConceptKey { get; set; }
        public required int?[] ContentKeys { get; set; }
        public CMR? Relationship { get; set; }
        public bool IsIdentical { get; set; }
        public bool CodeLiteralsAreIdentical { get; set; }
        public List<string> TechnicalMessages { get; set; } = [];
    }


    private readonly IDbConnection _db;

    private ILoggerFactory _loggerFactory;
    private ILogger _logger;

    private DbComparisonCache<DbValueSetComparison> _vsComparisonCache;
    private DbComparisonCache<DbValueSetConceptComparison> _conceptComparisonCache;

    private List<DbFhirPackage> _packages = [];

    public ValueSetComparer(
        IDbConnection db,
        ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
        _logger = loggerFactory.CreateLogger<FhirDbComparer>();

        _db = db;

        _vsComparisonCache = new();
        _conceptComparisonCache = new();
    }

    public void CompareValueSets(int? maxStepSize = null)
    {
        // get the list of packages
        _packages = DbFhirPackage.SelectList(_db, orderByProperties: [nameof(DbFhirPackage.PackageVersion)]);

        maxStepSize ??= _packages.Count - 1;

        // we want to process closer versions first, so we do a stepped approach
        for (int stepSize = 1; stepSize <= maxStepSize; stepSize++)
        {
            for (int i = 0; i < _packages.Count - stepSize; i++)
            {
                DbFhirPackage sourcePackage = _packages[i];
                DbFhirPackage targetPackage = _packages[i + stepSize];

                // ascending
                _logger.LogInformation($"Processing {sourcePackage.ShortName} -> {targetPackage.ShortName}");
                doComparison(sourcePackage, targetPackage);
                applyCachedChanges(sourcePackage, targetPackage);

                // descending
                _logger.LogInformation($"Processing {targetPackage.ShortName} -> {sourcePackage.ShortName}");
                doComparison(targetPackage, sourcePackage);
                applyCachedChanges(targetPackage, sourcePackage);
            }
        }
    }

    private void applyCachedChanges(DbFhirPackage sourcePackage, DbFhirPackage targetPackage)
    {
        if (_vsComparisonCache.ToAddCount > 0)
        {
            _logger.LogInformation($"Adding {_vsComparisonCache.ToAddCount} value set comparisons from {sourcePackage.ShortName} to {targetPackage.ShortName}");
            _vsComparisonCache.ToAdd.Insert(_db, ignoreDuplicates: true, insertPrimaryKey: true);
        }

        if (_vsComparisonCache.ToUpdateCount > 0)
        {
            _logger.LogInformation($"Updating {_vsComparisonCache.ToUpdateCount} value set comparisons from {sourcePackage.ShortName} to {targetPackage.ShortName}");
            _vsComparisonCache.ToUpdate.Update(_db);
        }

        if (_conceptComparisonCache.ToAddCount > 0)
        {
            _logger.LogInformation($"Adding {_conceptComparisonCache.ToAddCount} value set concept comparisons from {sourcePackage.ShortName} to {targetPackage.ShortName}");
            _conceptComparisonCache.ToAdd.Insert(_db, ignoreDuplicates: true, insertPrimaryKey: true);
        }

        if (_conceptComparisonCache.ToUpdateCount > 0)
        {
            _logger.LogInformation($"Updating {_conceptComparisonCache.ToUpdateCount} value set concept comparisons from {sourcePackage.ShortName} to {targetPackage.ShortName}");
            _conceptComparisonCache.ToUpdate.Update(_db);
        }

        _vsComparisonCache.Clear();
        _conceptComparisonCache.Clear();
    }

    private void doComparison(DbFhirPackage sourcePackage, DbFhirPackage targetPackage)
    {
        int sourceIndex = sourcePackage.PackageArrayIndex;
        int targetIndex = targetPackage.PackageArrayIndex;

        int steps = Math.Abs(targetIndex - sourceIndex);

        List<DbValueSet> sourceValueSets = DbValueSet.SelectList(_db, FhirPackageKey: sourcePackage.Key);
        _logger.LogInformation($" <<< processing {sourcePackage.ShortName} ValueSets, count: {sourceValueSets.Count}");

        // iterate over each value set in the source package
        foreach (DbValueSet sourceVs in sourceValueSets)
        {
            // skip value sets we know we will not process
            if (XVerProcessor._exclusionSet.Contains(sourceVs.UnversionedUrl))
            {
                continue;
            }

            // when we have a single step, do the comparisons directly
            if (steps == 1)
            {
                // discover targets
                List<ValueSetComparisonTrackingRecord> trackingRecords = buildNeighborComparisonPaths(
                    sourcePackage,
                    sourceVs,
                    targetPackage);

                // do the comparisons
                foreach (ValueSetComparisonTrackingRecord trackingRecord in trackingRecords)
                {
                    doComparison(trackingRecord);
                }
            }
            else
            {
                // discover targets
                List<ValueSetComparisonTrackingRecord> trackingRecords = discoverTransitivePaths(
                    sourcePackage,
                    sourceVs,
                    targetPackage);

                // do the comparisons transitively
                foreach (ValueSetComparisonTrackingRecord trackingRecord in trackingRecords)
                {
                    doTransitiveComparison(trackingRecord);
                }
            }
        }
    }

    private void doTransitiveComparison(ValueSetComparisonTrackingRecord trackingRecord)
    {
        // get our key (if necessary)
        trackingRecord.ComparisonRecordKey ??= DbValueSetComparison.GetIndex();

        // build the relevant concept comparison records
        doTransitiveConceptComparisons(trackingRecord);

        // determine the conceptRelationship based on the comparison steps
        CMR? vsRelationship = CMR.Equivalent;
        bool isIdentical = true;
        bool codeLiteralsAreIdentical = true;
        List<string> technicalMessages = [];

        foreach (DbValueSetComparison comparisonStep in trackingRecord.ComparisonSteps)
        {
            vsRelationship = FhirDbComparer.ApplyRelationship(vsRelationship, comparisonStep.Relationship);
            isIdentical = isIdentical && (comparisonStep.IsIdentical == true);
            codeLiteralsAreIdentical = codeLiteralsAreIdentical && (comparisonStep.CodeLiteralsAreIdentical == true);
            if (comparisonStep.TechnicalMessage is not null)
            {
                technicalMessages.Add(comparisonStep.TechnicalMessage);
            }
        }

        // check for an explicit mapping of this pair
        DbValueSetMapping? vsMapping = DbValueSetMapping.SelectSingle(
            _db,
            SourceFhirPackageKey: trackingRecord.SourcePackage.Key,
            SourceValueSetKey: trackingRecord.SourceValueSet.Key,
            TargetFhirPackageKey: trackingRecord.TargetPackage.Key,
            TargetValueSetKey: trackingRecord.TargetValueSetKey);

        if (vsMapping is not null)
        {
            trackingRecord.ExplicitMapping = vsMapping;
            trackingRecord.ExplicitMappingSource = vsMapping.ConceptMapSourceKey is null
                ? null
                : DbMappingSourceFile.SelectSingle(_db, Key: vsMapping.ConceptMapSourceKey);
            vsRelationship = vsMapping.Relationship ?? vsRelationship;

            technicalMessages.Add($"Using explicit mapping" +
                $" from `{trackingRecord.SourceValueSet.VersionedUrl}`" +
                $" to `{trackingRecord.TargetValueSet?.VersionedUrl}`" +
                $" in `{trackingRecord.ExplicitMappingSource?.Url}` (`{trackingRecord.ExplicitMappingSource?.Filename}`)");
        }

        // create our value set comparison
        DbValueSetComparison vsComparison = createValueSetComparison(
            trackingRecord,
            isIdentical,
            codeLiteralsAreIdentical,
            relationship: vsRelationship,
            technicalMessage: technicalMessages.Count == 0 ? null : string.Join('\n', technicalMessages),
            userMessage: null,
            contentStepKeys: trackingRecord.ContentKeys);

        _vsComparisonCache.CacheAdd(vsComparison);
    }

    private void doTransitiveConceptComparisons(ValueSetComparisonTrackingRecord trackingRecord)
    {
        if (trackingRecord.ComparisonSteps.Count == 0)
        {
            throw new Exception("Cannot build transitive comparisons without comparison steps!");
        }

        int sourceIndex = trackingRecord.SourcePackage.PackageArrayIndex;
        int targetIndex = trackingRecord.TargetPackage.PackageArrayIndex;
        int increment = sourceIndex < targetIndex ? 1 : -1;

        // get concept comparisons for each step, keyed by ValueSetComparisonKey
        Dictionary<int, List<DbValueSetConceptComparison>> stepConceptComparisons = [];
        foreach (DbValueSetComparison vsCompStep in trackingRecord.ComparisonSteps)
        {
            List<DbValueSetConceptComparison> conceptComparisons = DbValueSetConceptComparison.SelectList(
                _db,
                ValueSetComparisonKey: vsCompStep.Key);
            stepConceptComparisons[vsCompStep.Key] = conceptComparisons;
        }

        // get the initial source concepts
        List<DbValueSetConcept> sourceConcepts = DbValueSetConcept.SelectList(
            _db,
            ValueSetKey: trackingRecord.SourceValueSet.Key);

        // get the target concepts (if we have a target)
        Dictionary<int, DbValueSetConcept> targetConcepts = trackingRecord.TargetValueSet is null
            ? []
            : DbValueSetConcept.SelectDict(_db, ValueSetKey: trackingRecord.TargetValueSet.Key);

        // build a lookup from source concept key to the first step's concept comparisons
        ILookup<int, DbValueSetConceptComparison> firstStepBySourceConcept = stepConceptComparisons[trackingRecord.ComparisonSteps[0].Key]
            .ToLookup(cc => cc.SourceConceptKey);

        // iterate over each source concept and follow it through the transitive steps
        foreach (DbValueSetConcept sourceConcept in sourceConcepts)
        {
            bool sourceConceptIsEscape = XVerProcessor._escapeValveCodes.Contains(sourceConcept.Code);

            // get the initial concept comparisons for this source concept
            List<DbValueSetConceptComparison> initialComparisons = firstStepBySourceConcept[sourceConcept.Key].ToList();

            // if there are no initial comparisons, this is a no-map
            if (initialComparisons.Count == 0)
            {
                int?[] contentKeys = new int?[6];
                contentKeys[sourceIndex] = sourceConcept.Key;

                DbValueSetConceptComparison noMapComparison = createConceptComparison(
                    trackingRecord,
                    sourceConcept,
                    targetConcept: null,
                    relationship: null,
                    sourceConceptIsEscape: sourceConceptIsEscape,
                    targetConceptIsEscape: null,
                    technicalMessage: null,
                    userMessage: null,
                    contentStepKeys: contentKeys);

                _conceptComparisonCache.CacheAdd(noMapComparison);
                continue;
            }

            // follow the concept through each step
            List<ConceptTrackingRecord> currentPaths = initialComparisons
                .Select(cc => new ConceptTrackingRecord
                {
                    SourceConcept = sourceConcept,
                    CurrentConceptKey = cc.TargetConceptKey,
                    ContentKeys = getKeyArray(sourceIndex, sourceConcept.Key, sourceIndex + increment, cc.TargetConceptKey),
                    Relationship = cc.Relationship,
                    IsIdentical = cc.IsIdentical == true,
                    CodeLiteralsAreIdentical = cc.CodeLiteralsAreIdentical == true,
                    TechnicalMessages = cc.TechnicalMessage is null ? [] : [cc.TechnicalMessage],
                })
                .ToList();

            // process subsequent steps
            for (int step = 1; step < trackingRecord.ComparisonSteps.Count; step++)
            {
                DbValueSetComparison stepComparison = trackingRecord.ComparisonSteps[step];
                List<DbValueSetConceptComparison> stepConcepts = stepConceptComparisons[stepComparison.Key];
                ILookup<int, DbValueSetConceptComparison> stepBySourceConcept = stepConcepts.ToLookup(cc => cc.SourceConceptKey);

                int stepTargetIndex = sourceIndex + (increment * (step + 1));

                List<ConceptTrackingRecord> nextPaths = [];

                foreach (ConceptTrackingRecord path in currentPaths)
                {
                    // if current concept is null, continue with null
                    if (path.CurrentConceptKey is null)
                    {
                        path.ContentKeys[stepTargetIndex] = null;
                        nextPaths.Add(path);
                        continue;
                    }

                    // get the next comparisons for this concept
                    List<DbValueSetConceptComparison> nextComparisons = stepBySourceConcept[path.CurrentConceptKey.Value].ToList();

                    if (nextComparisons.Count == 0)
                    {
                        // no mapping found - treat as no-map from this point
                        path.CurrentConceptKey = null;
                        path.Relationship = FhirDbComparer.ApplyRelationship(path.Relationship, null);
                        path.IsIdentical = false;
                        path.CodeLiteralsAreIdentical = false;
                        nextPaths.Add(path);
                        continue;
                    }

                    // expand paths for each next comparison
                    foreach (DbValueSetConceptComparison nextComp in nextComparisons)
                    {
                        int?[] newContentKeys = path.ContentKeys.ToArray();
                        newContentKeys[stepTargetIndex] = nextComp.TargetConceptKey;

                        nextPaths.Add(new ConceptTrackingRecord
                        {
                            SourceConcept = sourceConcept,
                            CurrentConceptKey = nextComp.TargetConceptKey,
                            ContentKeys = newContentKeys,
                            Relationship = FhirDbComparer.ApplyRelationship(path.Relationship, nextComp.Relationship),
                            IsIdentical = path.IsIdentical && (nextComp.IsIdentical == true),
                            CodeLiteralsAreIdentical = path.CodeLiteralsAreIdentical && (nextComp.CodeLiteralsAreIdentical == true),
                            TechnicalMessages = nextComp.TechnicalMessage is null
                                ? [..path.TechnicalMessages]
                                : [..path.TechnicalMessages, nextComp.TechnicalMessage],
                        });
                    }
                }

                currentPaths = nextPaths;
            }

            // create the final concept comparison records for each completed path
            foreach (ConceptTrackingRecord path in currentPaths)
            {
                DbValueSetConcept? targetConcept = null;
                bool? targetConceptIsEscape = null;

                if (path.CurrentConceptKey is not null)
                {
                    targetConcepts.TryGetValue(path.CurrentConceptKey.Value, out targetConcept);
                    targetConceptIsEscape = targetConcept is null
                        ? null
                        : XVerProcessor._escapeValveCodes.Contains(targetConcept.Code);
                }

                DbValueSetConceptComparison conceptComparison = createConceptComparison(
                    trackingRecord,
                    sourceConcept,
                    targetConcept,
                    path.Relationship,
                    sourceConceptIsEscape: sourceConceptIsEscape,
                    targetConceptIsEscape: targetConceptIsEscape,
                    technicalMessage: path.TechnicalMessages.Count == 0 ? null : string.Join('\n', path.TechnicalMessages),
                    userMessage: null,
                    contentStepKeys: path.ContentKeys);

                _conceptComparisonCache.CacheAdd(conceptComparison);
            }
        }
    }

    private int?[] getKeyArray(int sourceIndex, int sourceKey, int targetIndex, int? targetKey)
    {
        int?[] result = [null, null, null, null, null, null];
        result[sourceIndex] = sourceKey;
        result[targetIndex] = targetKey;
        return result;
    }

    private void doComparison(ValueSetComparisonTrackingRecord trackingRecord)
    {
        DbValueSet sourceVs = trackingRecord.SourceValueSet;
        DbValueSet? targetVs = trackingRecord.TargetValueSet;

        // get the source concepts
        List<DbValueSetConcept> sourceConcepts = DbValueSetConcept.SelectList(
            _db,
            ValueSetKey: trackingRecord.SourceValueSet.Key);

        // track local comparisons so we can build the value set comparison
        List<DbValueSetConceptComparison> conceptComparisons = [];

        // if there is no target value set, every concept is a no map
        if (targetVs is null)
        {
            // create our concept comparisons
            foreach (DbValueSetConcept sourceConcept in sourceConcepts)
            {
                // build the concept comparison db record
                DbValueSetConceptComparison conceptComparison = createConceptComparison(
                    trackingRecord,
                    sourceConcept,
                    targetConcept: null,
                    relationship: null,
                    sourceConceptIsEscape: XVerProcessor._escapeValveCodes.Contains(sourceConcept.Code),
                    targetConceptIsEscape: null,
                    technicalMessage: null,
                    userMessage: null,
                    contentStepKeys: getKeyArray(trackingRecord.SourcePackage, sourceConcept.Key));

                _conceptComparisonCache.CacheAdd(conceptComparison);
                conceptComparisons.Add(conceptComparison);
            }

            bool vsAreIdentical = conceptComparisons.All(cc => cc.IsIdentical == true);
            bool vsCodeLiteralsAreIdentical = conceptComparisons.All(cc => cc.CodeLiteralsAreIdentical == true);

            // create our value set comparison
            DbValueSetComparison noMapComparison = createValueSetComparison(
                trackingRecord,
                vsAreIdentical,
                vsCodeLiteralsAreIdentical,
                relationship: null,
                technicalMessage: null,
                userMessage: null,
                contentStepKeys: getKeyArray(
                    trackingRecord.SourcePackage,
                    sourceVs.Key,
                    trackingRecord.TargetPackage,
                    null));

            _vsComparisonCache.CacheAdd(noMapComparison);

            return;
        }

        // get the target concepts
        Dictionary<int, DbValueSetConcept> targetConcepts = DbValueSetConcept.SelectDict(
            _db,
            ValueSetKey: targetVs.Key);

        // get any explicit concept mappings
        List<DbValueSetConceptMapping> conceptMappings = trackingRecord.ExplicitMapping is null
            ? []
            : DbValueSetConceptMapping.SelectList(_db, ValueSetMappingKey: trackingRecord.ExplicitMapping.Key);

        // create lookups
        ILookup<string, DbValueSetConcept> targetConceptsByCode = targetConcepts.Values.ToLookup(c => c.Code);
        ILookup<int, DbValueSetConceptMapping> conceptMappingsBySourceKey = conceptMappings.ToLookup(m => m.SourceValueSetConceptKey);

        // iterate over each source concept
        foreach (DbValueSetConcept sourceConcept in sourceConcepts)
        {
            CMR? conceptRelationship = null;
            string? technicalMessage = null;
            string? userMessage = null;

            bool sourceConceptIsEscape = XVerProcessor._escapeValveCodes.Contains(sourceConcept.Code);

            // check for an explicit mapping first
            DbValueSetConceptMapping? mapping = conceptMappingsBySourceKey[sourceConcept.Key].FirstOrDefault();
            if (mapping is not null)
            {
                DbValueSetConcept? targetConcept = null;

                if (mapping.TargetValueSetConceptKey is not null)
                {
                    targetConcepts.TryGetValue(mapping.TargetValueSetConceptKey.Value, out targetConcept);
                }

                // use the explicit conceptRelationship
                conceptRelationship = mapping.Relationship;

                technicalMessage = $"Using explicit mapping" +
                    $" from `{sourceVs.VersionedUrl}`" +
                    $" to `{targetVs.VersionedUrl}`" +
                    $" in `{trackingRecord.ExplicitMappingSource?.Url}` (`{trackingRecord.ExplicitMappingSource?.Filename}`)";

                bool? targetConceptIsEscape = targetConcept is null
                    ? null
                    : XVerProcessor._escapeValveCodes.Contains(targetConcept.Code);

                // check for escape-valve tracking
                if (sourceConceptIsEscape)
                {
                    if (targetConcept is null)
                    {
                        // this is a no-map, just add a message
                        technicalMessage += $"\nNote that the source concept `{sourceConcept.Code}` is flagged as an 'escape-valve' code and has no mapping.";
                    }
                    else if (targetConceptIsEscape != true)
                    {
                        // the source concept is broader than the target
                        conceptRelationship = FhirDbComparer.ApplyRelationship(conceptRelationship, CMR.SourceIsBroaderThanTarget);
                        technicalMessage += $"\nSince source concept `{sourceConcept.Code}` is flagged as an 'escape-valve' code" +
                            $" and mapped to a non-escape-valve code (`{targetConcept.Code}`)," +
                            $" the source is broader than the target.";
                    }
                    else
                    {
                        // check the active counts
                        if (sourceVs.ActiveConcreteConceptCount == targetVs.ActiveConcreteConceptCount)
                        {
                            conceptRelationship = FhirDbComparer.ApplyRelationship(conceptRelationship, CMR.Equivalent);
                            technicalMessage += $"\nSince source concept `{sourceConcept.Code}` is flagged as an 'escape-valve' code" +
                                $" and the source value set contains the same number of concepts as the target" +
                                $" ({sourceVs.ActiveConcreteConceptCount} vs. {targetVs.ActiveConcreteConceptCount})," +
                                $" we can assume equivalence.";
                        }
                        else if (sourceVs.ActiveConcreteConceptCount > targetVs.ActiveConcreteConceptCount)
                        {
                            conceptRelationship = FhirDbComparer.ApplyRelationship(conceptRelationship, CMR.SourceIsNarrowerThanTarget);
                            technicalMessage += $"\nSince source concept `{sourceConcept.Code}` is flagged as an 'escape-valve' code" +
                                $" and the source value set contains more concepts than the target" +
                                $" ({sourceVs.ActiveConcreteConceptCount} vs. {targetVs.ActiveConcreteConceptCount})," +
                                $" the source is narrower than the target.";
                        }
                        else
                        {
                            conceptRelationship = FhirDbComparer.ApplyRelationship(conceptRelationship, CMR.SourceIsBroaderThanTarget);
                            technicalMessage += $"\nSince source concept `{sourceConcept.Code}` is flagged as an 'escape-valve' code" +
                                $" and the source value set contains fewer concepts than the target" +
                                $" ({sourceVs.ActiveConcreteConceptCount} vs. {targetVs.ActiveConcreteConceptCount})," +
                                $" the source is broader than the target.";
                        }
                    }
                }

                // build the concept comparison db record
                DbValueSetConceptComparison conceptComparison = createConceptComparison(
                    trackingRecord,
                    sourceConcept,
                    targetConcept,
                    conceptRelationship,
                    sourceConceptIsEscape: sourceConceptIsEscape,
                    targetConceptIsEscape: targetConceptIsEscape,
                    technicalMessage,
                    mapping.Comments,
                    contentStepKeys: getKeyArray(
                        trackingRecord.SourcePackage,
                        sourceConcept.Key,
                        trackingRecord.TargetPackage,
                        targetConcept?.Key));
                _conceptComparisonCache.CacheAdd(conceptComparison);
                conceptComparisons.Add(conceptComparison);

                continue;
            }

            // no explicit mapping, try to find by code
            List<DbValueSetConcept> possibleTargets = targetConceptsByCode[sourceConcept.Code].ToList();

            // if there are multiple possible targets, we want to filter by system
            if (possibleTargets.Count > 1)
            {
                List<DbValueSetConcept> matches = possibleTargets.Where(c => c.System == sourceConcept.System).ToList();

                if (matches.Count != 0)
                {
                    possibleTargets = matches;
                }
            }

            if (possibleTargets.Count > 1)
            {
                technicalMessage = $"Multiple target concepts with code {sourceConcept.Code} found in target value set {trackingRecord.TargetValueSet!.Name}";
                userMessage = $"Multiple concepts with code {sourceConcept.Code} found in target value set.";
            }

            // iterate over the possible targets
            foreach (DbValueSetConcept targetConcept in possibleTargets)
            {
                // be optimistic
                conceptRelationship = CMR.Equivalent;

                int possibleTargetsInValueSetCount = possibleTargets.Where(tc => tc.ValueSetKey == targetConcept.ValueSetKey).Count();

                // check for multiple possible targets in this target value set
                if (possibleTargetsInValueSetCount > 1)
                {
                    technicalMessage += "\nMultiple mapping targets exist in the same value set, so the source is broader than any individual target.";
                    conceptRelationship = CMR.SourceIsBroaderThanTarget;
                }

                bool codeLiteralsMatch = targetConcept.Code == sourceConcept.Code;
                bool conceptIsRenamed = (conceptRelationship == CMR.Equivalent) && (!codeLiteralsMatch);
                bool conceptIsIdentical = (conceptRelationship == CMR.Equivalent) && (targetConcept.FhirKey == sourceConcept.FhirKey);
                bool conceptIsEquivalent = conceptRelationship == CMR.Equivalent;
                //bool conceptIsBroaderThanTarget = possibleTargetsInValueSetCount > 1;
                //bool? conceptIsNarrowerThanTarget = null;

                bool targetConceptIsEscape = XVerProcessor._escapeValveCodes.Contains(targetConcept.Code);

                // if this is an escape-valve code, we need to compare the counts of concepts too
                if (sourceConceptIsEscape)
                {
                    conceptIsIdentical = conceptIsIdentical && (sourceVs.ActiveConcreteConceptCount == targetVs.ActiveConcreteConceptCount);
                    conceptIsEquivalent = conceptIsEquivalent && (sourceVs.ActiveConcreteConceptCount == targetVs.ActiveConcreteConceptCount);

                    // check for escape-valve tracking
                    if (targetConceptIsEscape != true)
                    {
                        // the source concept is broader than the target
                        conceptRelationship = FhirDbComparer.ApplyRelationship(conceptRelationship, CMR.SourceIsBroaderThanTarget);
                        technicalMessage += $"\nSince source concept `{sourceConcept.Code}` is flagged as an 'escape-valve' code" +
                            $" and mapped to a non-escape-valve code (`{targetConcept.Code}`)," +
                            $" the source is broader than the target.";
                    }
                    else
                    {
                        // check the active counts
                        if (sourceVs.ActiveConcreteConceptCount == targetVs.ActiveConcreteConceptCount)
                        {
                            conceptRelationship = FhirDbComparer.ApplyRelationship(conceptRelationship, CMR.Equivalent);
                            technicalMessage += $"\nSince source concept `{sourceConcept.Code}` is flagged as an 'escape-valve' code" +
                                $" and the source value set contains the same number of concepts as the target" +
                                $" ({sourceVs.ActiveConcreteConceptCount} vs. {targetVs.ActiveConcreteConceptCount})," +
                                $" we can assume equivalence.";
                        }
                        else if (sourceVs.ActiveConcreteConceptCount > targetVs.ActiveConcreteConceptCount)
                        {
                            //conceptIsNarrowerThanTarget = true;
                            conceptRelationship = FhirDbComparer.ApplyRelationship(conceptRelationship, CMR.SourceIsNarrowerThanTarget);
                            technicalMessage += $"\nSince source concept `{sourceConcept.Code}` is flagged as an 'escape-valve' code" +
                                $" and the source value set contains more concepts than the target" +
                                $" ({sourceVs.ActiveConcreteConceptCount} vs. {targetVs.ActiveConcreteConceptCount})," +
                                $" the source is narrower than the target.";
                        }
                        else
                        {
                            //conceptIsBroaderThanTarget = true;
                            conceptRelationship = FhirDbComparer.ApplyRelationship(conceptRelationship, CMR.SourceIsBroaderThanTarget);
                            technicalMessage += $"\nSince source concept `{sourceConcept.Code}` is flagged as an 'escape-valve' code" +
                                $" and the source value set contains fewer concepts than the target" +
                                $" ({sourceVs.ActiveConcreteConceptCount} vs. {targetVs.ActiveConcreteConceptCount})," +
                                $" the source is broader than the target.";
                        }
                    }
                }

                // build the concept comparison db record
                DbValueSetConceptComparison conceptComparison = createConceptComparison(
                    trackingRecord,
                    sourceConcept,
                    targetConcept,
                    conceptRelationship,
                    sourceConceptIsEscape: sourceConceptIsEscape,
                    targetConceptIsEscape: targetConceptIsEscape,
                    technicalMessage,
                    userMessage,
                    contentStepKeys: getKeyArray(
                        trackingRecord.SourcePackage,
                        sourceConcept.Key,
                        trackingRecord.TargetPackage,
                        targetConcept.Key));
                _conceptComparisonCache.CacheAdd(conceptComparison);
                conceptComparisons.Add(conceptComparison);
            }
        }

        List<string> technicalMessages = [];

        // determine the conceptRelationship based on the concept comparisons
        CMR? vsRelationship = CMR.Equivalent;
        if (conceptComparisons.Any(cc => cc.NotMapped) ||
            conceptComparisons.Any(cc => cc.Relationship == CMR.SourceIsBroaderThanTarget))
        {
            vsRelationship = FhirDbComparer.ApplyRelationship(vsRelationship, CMR.SourceIsBroaderThanTarget);
            technicalMessages.Add("One or more source concepts are either not mapped or broader than their targets, so the value set relationship is broadened.");
        }

        if (conceptComparisons.Any(cc => cc.Relationship == CMR.SourceIsNarrowerThanTarget))
        {
            vsRelationship = FhirDbComparer.ApplyRelationship(vsRelationship, CMR.SourceIsNarrowerThanTarget);
            technicalMessages.Add("One or more source concepts are narrower than their targets, so the value set relationship is narrowed.");
        }

        bool isIdentical = conceptComparisons.All(cc => cc.IsIdentical == true);
        bool codeLiteralsAreIdentical = conceptComparisons.All(cc => cc.CodeLiteralsAreIdentical == true);

        if (isIdentical)
        {
            technicalMessages.Add("All concepts in the comparison are listed as identical.");
        }
        else if (codeLiteralsAreIdentical)
        {
            technicalMessages.Add("All concepts in the comparison are have code literals listed as identical.");
        }

        // include the relative concept counts in the relationship (value sets with more concepts are broader)
        if (sourceVs.ActiveConcreteConceptCount > targetVs.ActiveConcreteConceptCount)
        {
            vsRelationship = FhirDbComparer.ApplyRelationship(vsRelationship, CMR.SourceIsBroaderThanTarget);
            technicalMessages.Add($"The source value set has more active concepts ({sourceVs.ActiveConcreteConceptCount}) than the target ({targetVs.ActiveConcreteConceptCount}), so the source is broader than the target.");
        }
        else if (sourceVs.ActiveConcreteConceptCount < targetVs.ActiveConcreteConceptCount)
        {
            vsRelationship = FhirDbComparer.ApplyRelationship(vsRelationship, CMR.SourceIsNarrowerThanTarget);
            technicalMessages.Add($"The source value set has fewer active concepts ({sourceVs.ActiveConcreteConceptCount}) than the target ({targetVs.ActiveConcreteConceptCount}), so the source is narrower than the target.");
        }
        else
        {
            technicalMessages.Add($"The source and target value sets have the same number of active concepts ({sourceVs.ActiveConcreteConceptCount}).");
        }

        // create our value set comparison
        DbValueSetComparison vsComparison = createValueSetComparison(
            trackingRecord,
            isIdentical,
            codeLiteralsAreIdentical,
            relationship: vsRelationship,
            technicalMessage: technicalMessages.Count == 0 ? null : string.Join('\n', technicalMessages),
            userMessage: null,
            contentStepKeys: getKeyArray(
                trackingRecord.SourcePackage,
                sourceVs.Key,
                trackingRecord.TargetPackage,
                targetVs.Key));

        _vsComparisonCache.CacheAdd(vsComparison);
    }


    private int?[] getKeyArray(
        DbFhirPackage sourcePackage,
        int sourceKey)
    {
        int?[] result = [null, null, null, null, null, null];
        result[sourcePackage.PackageArrayIndex] = sourceKey;
        return result;
    }

    private int?[] getKeyArray(
        DbFhirPackage sourcePackage,
        int sourceKey,
        DbFhirPackage targetPackage,
        int? targetKey)
    {
        int?[] result = [null, null, null, null, null, null];
        result[sourcePackage.PackageArrayIndex] = sourceKey;
        result[targetPackage.PackageArrayIndex] = targetKey;
        return result;
    }


    //private List<ValueSetComparisonTrackingRecord> discoverComparisonPaths(
    //    DbFhirPackage sourcePackage,
    //    DbValueSet sourceVs,
    //    DbFhirPackage targetPackage)
    //{
    //    int sourceIndex = sourcePackage.PackageArrayIndex;
    //    int targetIndex = targetPackage.PackageArrayIndex;

    //    int steps = Math.Abs(targetIndex - sourceIndex);

    //    if (steps == 1)
    //    {
    //        return buildNeighborComparisonPaths(
    //            sourcePackage,
    //            sourceVs,
    //            targetPackage);
    //    }

    //    return discoverTransitivePaths(
    //        sourcePackage,
    //        sourceVs,
    //        targetPackage);
    //}

    private List<ValueSetComparisonTrackingRecord> discoverTransitivePaths(
        DbFhirPackage sourcePackage,
        DbValueSet sourceVs,
        DbFhirPackage targetPackage)
    {
        int sourceIndex = sourcePackage.PackageArrayIndex;
        int targetIndex = targetPackage.PackageArrayIndex;

        int steps = Math.Abs(targetIndex - sourceIndex);

        int increment = sourceIndex < targetIndex ? 1 : -1;

        int currentSourceIndex = sourceIndex;
        int currentTargetIndex = sourceIndex + increment;

        List<int> currentValueSetKeys = [sourceVs.Key];

        Dictionary<int, List<DbValueSetComparison>> stepComparisons = [];

        // build the comparisons possible at each step
        for (int step = 0; step < steps; step++)
        {
            DbFhirPackage fromPackage = _packages[currentSourceIndex];
            DbFhirPackage toPackage = _packages[currentTargetIndex];

            // get the existing comparisons for this step
            List<DbValueSetComparison> currentComparisons = DbValueSetComparison.SelectList(
                _db,
                SourceFhirPackageKey: fromPackage.Key,
                TargetFhirPackageKey: toPackage.Key,
                SourceValueSetKeyValues: currentValueSetKeys);

            // add our current records to our dictionary to reconcile later
            stepComparisons[step] = currentComparisons;

            // update our source value sets for the next iteration
            currentValueSetKeys = currentComparisons
                .Where(r => r.TargetValueSetKey is not null)
                .Select(r => r.TargetValueSetKey!.Value)
                .Distinct()
                .ToList();

            // increment our indices
            currentSourceIndex += increment;
            currentTargetIndex += increment;
        }

        ILookup<int, DbValueSetComparison> comparisonsBySourceKey = stepComparisons.Values
            .SelectMany(l => l)
            .ToLookup(c => c.SourceContentKey);

        Dictionary<int, DbFhirPackage> packages = _packages.ToDictionary(p => p.Key);

        List<ValueSetComparisonTrackingRecord> trackingRecords = [];

        // iterate over the initial comparisons and build the path chains transitively
        foreach (DbValueSetComparison initialComparison in stepComparisons[0])
        {
            currentSourceIndex = sourceIndex;
            currentTargetIndex = sourceIndex + increment;

            ValueSetComparisonTrackingRecord tr = new()
            {
                SourcePackage = sourcePackage,
                SourceValueSet = sourceVs,
                TargetPackage = targetPackage,
                TargetValueSet = null,
                TargetValueSetKey = initialComparison.TargetValueSetKey,
                ComparisonRecordKey = initialComparison.Key,
            };

            tr.Contents[sourceIndex] = sourceVs;
            tr.ContentKeys[sourceIndex] = sourceVs.Key;

            tr.ContentKeys[currentTargetIndex] = initialComparison.TargetValueSetKey;

            tr.ComparisonSteps.Add(initialComparison);

            List<ValueSetComparisonTrackingRecord> currentTrackingRecords = [tr];

            // follow the steps and continue building our tracking records
            for (int step = 1; step < steps; step++)
            {
                if (!stepComparisons.ContainsKey(step))
                {
                    break;
                }

                currentSourceIndex += increment;
                currentTargetIndex += increment;

                List<ValueSetComparisonTrackingRecord> nextTrackingRecords = [];

                // iterate over the current tracking records to expand them
                foreach (ValueSetComparisonTrackingRecord currentTr in currentTrackingRecords)
                {
                    int? currentTargetVsKey = currentTr.ContentKeys[currentSourceIndex];
                    if (currentTargetVsKey is null)
                    {
                        continue;
                    }

                    // get the possible next comparisons
                    IEnumerable<DbValueSetComparison> possibleNextComparisons = comparisonsBySourceKey[currentTargetVsKey.Value];

                    // iterate over the possible next comparisons to build new tracking records
                    foreach (DbValueSetComparison nextComparison in possibleNextComparisons)
                    {
                        ValueSetComparisonTrackingRecord nextTr = currentTr.Clone();

                        nextTr.TargetValueSetKey = nextComparison.TargetValueSetKey;
                        nextTr.ComparisonRecordKey = nextComparison.Key;
                        nextTr.ContentKeys[currentTargetIndex] = nextComparison.TargetValueSetKey;
                        nextTr.ComparisonSteps.Add(nextComparison);
                        nextTrackingRecords.Add(nextTr);
                    }
                }

                currentTrackingRecords = nextTrackingRecords;
            }

            // process the completed tracking record
            foreach (ValueSetComparisonTrackingRecord currentTr in currentTrackingRecords)
            {
                if ((currentTr.TargetValueSetKey is not null) &&
                    (currentTr.TargetValueSet is null))
                {
                    currentTr.TargetValueSet = DbValueSet.SelectSingle(
                        _db,
                        Key: currentTr.TargetValueSetKey.Value);
                }

                if ((currentTr.TargetValueSetKey is not null) &&
                    (currentTr.TargetValueSet is null))
                {
                    currentTr.TargetValueSet = DbValueSet.SelectSingle(
                        _db,
                        Key: currentTr.TargetValueSetKey.Value);
                }

                // need to create a new comparison record for this full path
                currentTr.ComparisonRecordKey = null;
                currentTr.ComparisonRecord = null;
            }

            // add the completed tracking records
            trackingRecords.AddRange(currentTrackingRecords);
        }

        return trackingRecords;
    }

    private List<ValueSetComparisonTrackingRecord> buildNeighborComparisonPaths(
        DbFhirPackage sourcePackage,
        DbValueSet sourceVs,
        DbFhirPackage targetPackage)
    {
        List<ValueSetComparisonTrackingRecord> trackingRecords = [];
        int sourceIndex = sourcePackage.PackageArrayIndex;
        int targetIndex = targetPackage.PackageArrayIndex;

        // check for explicit mappings for this value set to the target package LAST
        List<DbValueSetMapping> mappings = DbValueSetMapping.SelectList(
            _db,
            SourceFhirPackageKey: sourcePackage.Key,
            SourceValueSetKey: sourceVs.Key,
            TargetFhirPackageKey: targetPackage.Key);

        HashSet<int> targetKeys = [];
        HashSet<string> targetIds = [];
        HashSet<string> targetUrls = [];

        // iterate over the mappings to add to our tracking records
        foreach (DbValueSetMapping mapping in mappings)
        {
            // resolve the target value set (if necessary)
            DbValueSet? targetVs = mapping.TargetValueSetKey is null
                ? null
                : DbValueSet.SelectSingle(_db, Key: mapping.TargetValueSetKey);

            if ((targetVs is null) &&
                (mapping.TargetValueSetKey is not null))
            {
                throw new Exception($"Unable to resolve target value set with key {mapping.TargetValueSetKey} for mapping {mapping.Key}");
            }

            if (targetVs is not null)
            {
                targetIds.Add(targetVs.Id);
                targetUrls.Add(targetVs.UnversionedUrl);
                if (!targetKeys.Add(targetVs.Key))
                {
                    throw new Exception($"Duplicate target value set key {targetVs.Key} for mapping {mapping.Key}");
                }
            }

            ValueSetComparisonTrackingRecord tr = new()
            {
                SourcePackage = sourcePackage,
                SourceValueSet = sourceVs,
                TargetPackage = targetPackage,
                TargetValueSet = targetVs,
                ExplicitMapping = mapping,
                ExplicitMappingSource = mapping.ConceptMapSourceKey is null ? null : DbMappingSourceFile.SelectSingle(_db, Key: mapping.ConceptMapSourceKey),
            };

            tr.Contents[sourceIndex] = sourceVs;
            tr.Contents[targetIndex] = targetVs;

            trackingRecords.Add(tr);
        }

        // check if there is a value set in the target package with the same ID
        DbValueSet? possibleTargetVs = DbValueSet.SelectSingle(
            _db,
            FhirPackageKey: targetPackage.Key,
            Id: sourceVs.Id);
        if ((possibleTargetVs is not null) &&
            targetKeys.Add(possibleTargetVs.Key))
        {
            targetIds.Add(possibleTargetVs.Id);
            targetUrls.Add(possibleTargetVs.UnversionedUrl);

            ValueSetComparisonTrackingRecord tr = new()
            {
                SourcePackage = sourcePackage,
                SourceValueSet = sourceVs,
                TargetPackage = targetPackage,
                TargetValueSet = possibleTargetVs,
                ExplicitMapping = null,
                ExplicitMappingSource = null,
            };

            tr.Contents[sourceIndex] = sourceVs;
            tr.Contents[targetIndex] = possibleTargetVs;

            trackingRecords.Add(tr);
        }

        // check if there is a value set in the target package with the same URL
        possibleTargetVs = DbValueSet.SelectSingle(
            _db,
            FhirPackageKey: targetPackage.Key,
            UnversionedUrl: sourceVs.UnversionedUrl);
        if ((possibleTargetVs is not null) &&
            targetKeys.Add(possibleTargetVs.Key))
        {
            targetIds.Add(possibleTargetVs.Id);
            targetUrls.Add(possibleTargetVs.UnversionedUrl);

            ValueSetComparisonTrackingRecord tr = new()
            {
                SourcePackage = sourcePackage,
                SourceValueSet = sourceVs,
                TargetPackage = targetPackage,
                TargetValueSet = possibleTargetVs,
                ExplicitMapping = null,
                ExplicitMappingSource = null,
            };

            tr.Contents[sourceIndex] = sourceVs;
            tr.Contents[targetIndex] = possibleTargetVs;

            trackingRecords.Add(tr);
        }

        // if we have no targets, check for a matching name too
        if (trackingRecords.Count == 0)
        {
            possibleTargetVs = DbValueSet.SelectSingle(
                _db,
                FhirPackageKey: targetPackage.Key,
                Name: sourceVs.Name);
            if ((possibleTargetVs is not null) &&
                targetKeys.Add(possibleTargetVs.Key))
            {
                targetIds.Add(possibleTargetVs.Id);
                targetUrls.Add(possibleTargetVs.UnversionedUrl);

                ValueSetComparisonTrackingRecord tr = new()
                {
                    SourcePackage = sourcePackage,
                    SourceValueSet = sourceVs,
                    TargetPackage = targetPackage,
                    TargetValueSet = possibleTargetVs,
                    ExplicitMapping = null,
                    ExplicitMappingSource = null,
                };

                tr.Contents[sourceIndex] = sourceVs;
                tr.Contents[targetIndex] = possibleTargetVs;

                trackingRecords.Add(tr);
            }
        }

        // check to see if we have any inverse mappings from the target to the source
        if (trackingRecords.Count == 0)
        {
            List<DbValueSetMapping> inverseMappings = DbValueSetMapping.SelectList(
                _db,
                SourceFhirPackageKey: targetPackage.Key,
                TargetFhirPackageKey: sourcePackage.Key,
                TargetValueSetKey: sourceVs.Key);

            // iterate over the mappings to add to our tracking records
            foreach (DbValueSetMapping mapping in inverseMappings)
            {
                // resolve the source value set 
                DbValueSet? inverseSourceVs = DbValueSet.SelectSingle(_db, Key: mapping.SourceValueSetKey);

                if (inverseSourceVs is null)
                {
                    throw new Exception($"Unable to resolve  value set with key {mapping.SourceValueSetKey} for mapping {mapping.Key}");
                }

                targetIds.Add(inverseSourceVs.Id);
                targetUrls.Add(inverseSourceVs.UnversionedUrl);
                if (!targetKeys.Add(inverseSourceVs.Key))
                {
                    throw new Exception($"Duplicate target value set key {inverseSourceVs.Key} for mapping {mapping.Key}");
                }

                ValueSetComparisonTrackingRecord tr = new()
                {
                    SourcePackage = sourcePackage,
                    SourceValueSet = sourceVs,
                    TargetPackage = targetPackage,
                    TargetValueSet = inverseSourceVs,
                    ExplicitMapping = mapping,
                    ExplicitMappingSource = null,
                };

                tr.Contents[sourceIndex] = sourceVs;
                tr.Contents[targetIndex] = inverseSourceVs;

                trackingRecords.Add(tr);
            }
        }

        // if we still have no targets, treat as a no map
        if (trackingRecords.Count == 0)
        {
            ValueSetComparisonTrackingRecord tr = new()
            {
                SourcePackage = sourcePackage,
                SourceValueSet = sourceVs,
                TargetPackage = targetPackage,
                TargetValueSet = null,
                ExplicitMapping = null,
                ExplicitMappingSource = null,
            };

            tr.Contents[sourceIndex] = sourceVs;

            trackingRecords.Add(tr);
        }

        return trackingRecords;
    }

    private DbValueSetComparison createValueSetComparison(
        ValueSetComparisonTrackingRecord vsTrackingRecord,
        bool isIdentical,
        bool codeLiteralsAreIdentical,
        CMR? relationship,
        string? technicalMessage,
        string? userMessage,
        int?[] contentStepKeys)
    {
        vsTrackingRecord.ComparisonRecordKey ??= DbValueSetComparison.GetIndex();

        return new()
        {
            Key = vsTrackingRecord.ComparisonRecordKey.Value,
            ValueSetMappingKey = vsTrackingRecord.ExplicitMapping?.Key,

            Steps = Math.Abs(vsTrackingRecord.SourcePackage.DefinitionFhirSequence - vsTrackingRecord.TargetPackage.DefinitionFhirSequence),

            SourceFhirPackageKey = vsTrackingRecord.SourcePackage.Key,
            SourceFhirSequence = vsTrackingRecord.SourcePackage.DefinitionFhirSequence,
            SourceValueSetKey = vsTrackingRecord.SourceValueSet.Key,
            SourceCanonicalUnversioned = vsTrackingRecord.SourceValueSet.UnversionedUrl,
            SourceCanonicalVersioned = vsTrackingRecord.SourceValueSet.VersionedUrl,
            SourceId = vsTrackingRecord.SourceValueSet.Id,
            SourceName = vsTrackingRecord.SourceValueSet.Name,
            SourceVersion = vsTrackingRecord.SourceValueSet.Version,

            TargetFhirPackageKey = vsTrackingRecord.TargetPackage.Key,
            TargetFhirSequence = vsTrackingRecord.TargetPackage.DefinitionFhirSequence,
            TargetValueSetKey = vsTrackingRecord.TargetValueSet?.Key,
            TargetCanonicalUnversioned = vsTrackingRecord.TargetValueSet?.UnversionedUrl,
            TargetCanonicalVersioned = vsTrackingRecord.TargetValueSet?.VersionedUrl,
            TargetId = vsTrackingRecord.TargetValueSet?.Id,
            TargetName = vsTrackingRecord.TargetValueSet?.Name,
            TargetVersion = vsTrackingRecord.TargetValueSet?.Version,

            ContentKeys = contentStepKeys,

            Relationship = relationship,
            NotMapped = vsTrackingRecord.TargetValueSet is null,

            IsIdentical = isIdentical,
            CodeLiteralsAreIdentical = codeLiteralsAreIdentical,

            TechnicalMessage = technicalMessage,
            UserMessage = userMessage,
        };
    }

    private DbValueSetConceptComparison createConceptComparison(
        ValueSetComparisonTrackingRecord vsTrackingRecord,
        DbValueSetConcept sourceConcept,
        DbValueSetConcept? targetConcept,
        CMR? relationship,
        bool sourceConceptIsEscape,
        bool? targetConceptIsEscape,
        string? technicalMessage,
        string? userMessage,
        int?[] contentStepKeys)
    {
        vsTrackingRecord.ComparisonRecordKey ??= DbValueSetComparison.GetIndex();

        return new()
        {
            Key = DbValueSetConceptComparison.GetIndex(),
            ValueSetComparisonKey = vsTrackingRecord.ComparisonRecordKey.Value,
            ValueSetConceptMappingKey = vsTrackingRecord.ExplicitMapping?.Key,

            Steps = Math.Abs(vsTrackingRecord.SourcePackage.DefinitionFhirSequence - vsTrackingRecord.TargetPackage.DefinitionFhirSequence),

            SourceFhirPackageKey = vsTrackingRecord.SourcePackage.Key,
            SourceFhirSequence = vsTrackingRecord.SourcePackage.DefinitionFhirSequence,
            SourceValueSetKey = vsTrackingRecord.SourceValueSet.Key,
            SourceConceptKey = sourceConcept.Key,
            SourceSystem = sourceConcept.System,
            SourceCode = sourceConcept.Code,

            TargetFhirPackageKey = vsTrackingRecord.TargetPackage.Key,
            TargetFhirSequence = vsTrackingRecord.TargetPackage.DefinitionFhirSequence,
            TargetValueSetKey = vsTrackingRecord.TargetValueSet?.Key,
            TargetConceptKey = targetConcept?.Key,
            TargetSystem = targetConcept?.System,
            TargetCode = targetConcept?.Code,

            ContentKeys = contentStepKeys,

            Relationship = relationship,
            NotMapped = targetConcept is null,

            IsIdentical = (targetConcept is not null) &&
                          (sourceConcept.Code == targetConcept.Code) &&
                          (sourceConcept.Display == targetConcept.Display),
            CodeLiteralsAreIdentical = (targetConcept is not null) &&
                                      (sourceConcept.Code == targetConcept.Code),
            SourceCodeTreatedAsEscapeValve = sourceConceptIsEscape,
            TargetCodeTreatedAsEscapeValve = targetConceptIsEscape,

            TechnicalMessage = technicalMessage,
            UserMessage = userMessage,
        };
    }
}
