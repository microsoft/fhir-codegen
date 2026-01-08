using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fhir.CodeGen.Comparison.CompareTool;
using Fhir.CodeGen.Comparison.Models;
using Fhir.CodeGen.Comparison.XVer;
using Microsoft.Extensions.Logging;
using CMR = Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship;

namespace Fhir.CodeGen.Comparison.Outcomes;

public class ValueSetOutcomeGenerator
{
    private readonly IDbConnection _db;

    private ILoggerFactory _loggerFactory;
    private ILogger _logger;

    private List<DbFhirPackage> _packages = [];

    private DbRecordCache<DbValueSetOutcome> _vsOutcomeCache;
    private DbRecordCache<DbValueSetConceptOutcome> _conceptOutcomeCache;


    public ValueSetOutcomeGenerator(
        IDbConnection db,
        ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
        _logger = loggerFactory.CreateLogger<FhirDbComparer>();

        _db = db;

        _vsOutcomeCache = new();
        _conceptOutcomeCache = new();
    }

    public void CreateOutcomesForValueSets(int? maxStepSize = null)
    {
        // get the list of packages
        _packages = DbFhirPackage.SelectList(_db, orderByProperties: [nameof(DbFhirPackage.PackageVersion)]);

        List<FhirPackageComparisonPair> packagePairs = [];
        maxStepSize ??= _packages.Count - 1;

        // we want to process closer versions first, so we do a stepped approach
        for (int stepSize = 1; stepSize <= maxStepSize; stepSize++)
        {
            for (int i = 0; i < _packages.Count - stepSize; i++)
            {
                DbFhirPackage sourcePackage = _packages[i];
                DbFhirPackage targetPackage = _packages[i + stepSize];

                packagePairs.Add(new(sourcePackage, targetPackage));
                packagePairs.Add(new(targetPackage, sourcePackage));
            }
        }

        // iterate over our pairs in the order we built them
        foreach (FhirPackageComparisonPair packagePair in packagePairs)
        {
            buildOutcomes(packagePair);
            applyCachedChanges(packagePair);
        }
    }

    private void applyCachedChanges(FhirPackageComparisonPair packagePair)
    {
        DbFhirPackage sourcePackage = packagePair.SourcePackage;
        DbFhirPackage targetPackage = packagePair.TargetPackage;

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

    private void buildOutcomes(FhirPackageComparisonPair packagePair)
    {
        _logger.LogInformation($" Generating ValueSet Outcomes for {packagePair.SourcePackageShortName}->{packagePair.TargetPackageShortName}...");

        // just get all the source and target value sets now to avoid hitting the db so much
        Dictionary<int, DbValueSet> allSourceValueSets = DbValueSet.SelectDict(_db, FhirPackageKey: packagePair.SourcePackageKey);
        Dictionary<int, DbValueSetConcept> allSourceConcepts = DbValueSetConcept.SelectDict(_db, FhirPackageKey: packagePair.SourcePackageKey);
        Dictionary<int, DbValueSet> allTargetValueSets = DbValueSet.SelectDict(_db, FhirPackageKey: packagePair.TargetPackageKey);
        Dictionary<int, DbValueSetConcept> allTargetConcepts = DbValueSetConcept.SelectDict(_db, FhirPackageKey: packagePair.TargetPackageKey);

        List<DbValueSetComparison> vsComparisons = DbValueSetComparison.SelectList(
            _db,
            SourceFhirPackageKey: packagePair.SourcePackageKey,
            TargetFhirPackageKey: packagePair.TargetPackageKey);

        _logger.LogInformation(
            $" ValueSet comparisons: {vsComparisons.Count}, from" +
            $" {allSourceValueSets.Count} Value Sets" +
            $" to {allTargetValueSets.Count} Value Sets");

        List<DbValueSetConceptComparison> conceptComparisons = DbValueSetConceptComparison.SelectList(
            _db,
            SourceFhirPackageKey: packagePair.SourcePackageKey,
            TargetFhirPackageKey: packagePair.TargetPackageKey);

        _logger.LogInformation(
            $" ValueSet Concept comparisons: {conceptComparisons.Count}, from" +
            $" {allSourceConcepts.Count} Value Set Concepts" +
            $" to {allTargetConcepts.Count} Value Set Concepts");

        // create lookups of our comparisons that we need to generate outcomes
        ILookup<int, DbValueSetComparison> vsComparsionsBySourceKey = vsComparisons.ToLookup(c => c.SourceContentKey);
        ILookup<int, DbValueSetComparison> vsComparsionsByTargetKey = vsComparisons
            .Where(c => c.TargetContentKey is not null)
            .ToLookup(c => c.TargetContentKey!.Value);

        ILookup<int, DbValueSetConcept> allSourceConceptsByVsKey = allSourceConcepts.Values.ToLookup(c => c.ValueSetKey);
        ILookup<int, DbValueSetConcept> allTargetConceptsByVsKey = allTargetConcepts.Values.ToLookup(c => c.ValueSetKey);

        ILookup<int, DbValueSetConceptComparison> conceptComparsionsByVsComparisonKey = conceptComparisons.ToLookup(c => c.ValueSetComparisonKey);

        // iterate over our source value sets
        foreach (DbValueSet sourceVs in allSourceValueSets.Values)
        {
            List<DbValueSetComparison> sourceComparisons = vsComparsionsBySourceKey[sourceVs.Key].ToList();
            if (sourceComparisons.Count == 0)
            {
                continue;
            }

            Dictionary<int, DbValueSetConcept> sourceConcepts = allSourceConceptsByVsKey[sourceVs.Key]
                .ToDictionary(c => c.Key);

            Dictionary<int, Dictionary<int, DbValueSetConcept>> targetConceptsByVsKey = [];

            HashSet<int> fullyMappedConceptKeys = [];
            Dictionary<int, HashSet<int>> fullyMappedConceptKeysByComparisonKey = [];

            //CMR? relationship = CMR.Equivalent;
            //bool hasNoMap = false;

            Dictionary<int, DbValueSet> targetValueSets = [];
            // build objects we need for processing
            foreach (DbValueSetComparison sourceComparison in sourceComparisons)
            {
                //relationship = FhirDbComparer.ApplyRelationship(relationship, sourceComparison.Relationship);

                if ((sourceComparison.TargetContentKey is null) ||
                    sourceComparison.NotMapped)
                {
                    //hasNoMap = true;
                    continue;
                }

                int targetVsKey = sourceComparison.TargetContentKey.Value;

                if (targetValueSets.ContainsKey(targetVsKey))
                {
                    continue;
                }

                targetValueSets[targetVsKey] = allTargetValueSets[targetVsKey];
                targetConceptsByVsKey[targetVsKey] = allTargetConceptsByVsKey[targetVsKey].ToDictionary(c => c.Key);

                HashSet<int> currentlyMappedComparisonKeys = [];
                fullyMappedConceptKeysByComparisonKey[sourceComparison.Key] = currentlyMappedComparisonKeys;

                // iterate over the concepts and track ones that are fully mapped
                foreach (DbValueSetConceptComparison conceptComparison in conceptComparsionsByVsComparisonKey[sourceComparison.Key])
                {
                    if ((conceptComparison.IsIdentical == true) ||
                        (conceptComparison.Relationship == CMR.Equivalent) ||
                        (conceptComparison.Relationship == CMR.SourceIsNarrowerThanTarget))
                    {
                        fullyMappedConceptKeys.Add(conceptComparison.SourceConceptKey);
                        currentlyMappedComparisonKeys.Add(conceptComparison.SourceConceptKey);
                    }
                }
            }

            bool fullyMapsAcrossAllTargets = fullyMappedConceptKeys.Count == sourceConcepts.Count;
            int totalTargetCount = targetValueSets.Count;

            // traverse our comparisons to build matching outcomes
            foreach (DbValueSetComparison sourceComparison in sourceComparisons)
            {
                DbValueSet? targetVs = sourceComparison.TargetContentKey is null
                    ? null
                    : targetValueSets[sourceComparison.TargetContentKey.Value];

                (string idLong, string idShort) = XVerProcessor.GenerateArtifactId(
                    packagePair.SourcePackageShortName,
                    sourceVs.Id,
                    packagePair.TargetPackageShortName,
                    targetVs?.Id);

                string url = $"http://hl7.org/fhir/{packagePair.SourcePackageShortName}/ValueSet/{idLong}";

                if ((targetVs is null) ||
                    sourceComparison.NotMapped)
                {
                    OutcomeValueSetActionCodes noMapAction;

                    if (fullyMapsAcrossAllTargets)
                    {
                        noMapAction = OutcomeValueSetActionCodes.UseOtherValueSets;
                    }
                    else if (sourceComparisons.Count > 1)
                    {
                        noMapAction = OutcomeValueSetActionCodes.UseOtherAndCrossVersion;
                    }
                    else
                    {
                        noMapAction = OutcomeValueSetActionCodes.UseCrossVersionDefinition;
                    }

                    // build our no-map outcome
                    DbValueSetOutcome noMapOutcome = new()
                    {
                        Key = DbValueSetOutcome.GetIndex(),
                        ValueSetComparisonKey = sourceComparison.Key,

                        SourceFhirPackageKey = packagePair.SourcePackageKey,
                        SourceFhirSequence = packagePair.SourceFhirSequence,
                        SourceValueSetKey = sourceVs.Key,
                        SourceCanonicalUnversioned = sourceVs.UnversionedUrl,
                        SourceCanonicalVersioned = sourceVs.VersionedUrl,
                        SourceVersion = sourceVs.Version,
                        SourceId = sourceVs.Id,
                        SourceName = sourceVs.Name,
                        TotalSourceCount = -1,

                        TargetFhirPackageKey = packagePair.TargetPackageKey,
                        TargetFhirSequence = packagePair.TargetFhirSequence,
                        TargetValueSetKey = null,
                        TargetCanonicalUnversioned = null,
                        TargetCanonicalVersioned = null,
                        TargetVersion = null,
                        TargetId = null,
                        TargetName = null,
                        TotalTargetCount = totalTargetCount,

                        PotentialGenResourceType = "ValueSet",
                        PotentialGenLongId = idLong,
                        PotentialGenShortId = idShort,
                        PotentialGenUrl = url,

                        IsRenamed = false,
                        IsUnmapped = false,
                        IsIdentical = false,
                        IsEquivalent = false,
                        IsBroaderThanTarget = false,
                        IsNarrowerThanTarget = false,

                        FullyMapsToThisTarget = false,
                        FullyMapsAcrossAllTargets = fullyMapsAcrossAllTargets,

                        Comments = sourceComparison.UserMessage ?? sourceComparison.TechnicalMessage ?? "TODO",
                        OutcomeAction = noMapAction,

                        ContentKeys = sourceComparison.ContentKeys,
                    };

                    _vsOutcomeCache.CacheAdd(noMapOutcome);

                    // build our no-map concept outcomes
                    foreach (DbValueSetConceptComparison conceptComparison in conceptComparsionsByVsComparisonKey[sourceComparison.Key])
                    {
                        OutcomeValueSetConceptActionCodes noMapConceptAction = noMapAction switch
                        {
                            OutcomeValueSetActionCodes.UseOtherValueSets => OutcomeValueSetConceptActionCodes.MappedElsewhere,
                            OutcomeValueSetActionCodes.UseOtherAndCrossVersion => fullyMappedConceptKeys.Contains(conceptComparison.SourceConceptKey)
                                ? OutcomeValueSetConceptActionCodes.MappedElsewhere
                                : OutcomeValueSetConceptActionCodes.UseCrossVersionDefinition,
                            _ => OutcomeValueSetConceptActionCodes.UseCrossVersionDefinition,
                        };

                        DbValueSetConcept sourceConcept = allSourceConcepts[conceptComparison.SourceConceptKey];

                        DbValueSetConceptOutcome noMapConceptOutcome = new()
                        {
                            Key = DbValueSetConceptOutcome.GetIndex(),
                            ValueSetOutcomeKey = noMapOutcome.Key,
                            ValueSetConceptComparisonKey = conceptComparison.Key,

                            SourceFhirPackageKey = packagePair.SourcePackageKey,
                            SourceFhirSequence = packagePair.SourceFhirSequence,
                            SourceValueSetKey = sourceVs.Key,
                            SourceValueSetConceptKey = sourceConcept.Key,
                            SourceSystem = sourceConcept.System,
                            SourceCode = sourceConcept.Code,
                            TotalSourceCount = -1,

                            TargetFhirPackageKey = packagePair.TargetPackageKey,
                            TargetFhirSequence = packagePair.TargetFhirSequence,
                            TargetValueSetKey = null,
                            TargetValueSetConceptKey = null,
                            TargetSystem = null,
                            TargetCode = null,
                            TotalTargetCount = -1,

                            IsRenamed = false,
                            IsUnmapped = false,
                            IsIdentical = false,
                            IsEquivalent = false,
                            IsBroaderThanTarget = false,
                            IsNarrowerThanTarget = false,

                            FullyMapsToThisTarget = false,
                            FullyMapsAcrossAllTargets = fullyMappedConceptKeys.Contains(sourceConcept.Key),

                            CodeLiteralsMatch = false,
                            SourceCodeTreatedAsEscapeValve = conceptComparison.SourceCodeTreatedAsEscapeValve,
                            TargetCodeTreatedAsEscapeValve = false,

                            Comments = sourceComparison.UserMessage ?? sourceComparison.TechnicalMessage ?? "TODO",
                            OutcomeAction = noMapConceptAction,

                            ContentKeys = conceptComparison.ContentKeys,
                        };

                        _conceptOutcomeCache.CacheAdd(noMapConceptOutcome);
                    }

                    // move to next comparison
                    continue;
                }

                int targetVsKey = targetVs.Key;

                Dictionary<int, DbValueSetConcept> targetConcepts = targetConceptsByVsKey[targetVsKey];

                bool isRenamed = (totalTargetCount == 1) && (sourceVs.Id != targetVs.Id);
                bool isUnmapped = false;
                bool isIdentical = sourceComparison.IsIdentical == true;
                bool isEquivalent = sourceComparison.Relationship == CMR.Equivalent;
                bool isBroaderThanTarget = sourceComparison.Relationship == CMR.SourceIsBroaderThanTarget;
                bool isNarrowerThanTarget = sourceComparison.Relationship == CMR.SourceIsNarrowerThanTarget;

                bool fullyMapsToThisTarget = fullyMappedConceptKeysByComparisonKey[sourceComparison.Key].Count == sourceConcepts.Count;

                OutcomeValueSetActionCodes vsAction;

                if (isIdentical)
                {
                    vsAction = OutcomeValueSetActionCodes.UseValueSetSameName;
                }
                else if (isEquivalent)
                {
                    vsAction = OutcomeValueSetActionCodes.UseValueSetRenamed;
                }
                else if (fullyMapsAcrossAllTargets)
                {
                    vsAction = sourceVs.Id == targetVs.Id
                        ? OutcomeValueSetActionCodes.UseValueSetSameName
                        : OutcomeValueSetActionCodes.UseValueSetRenamed;
                }
                else
                {
                    vsAction = sourceVs.Id == targetVs.Id
                        ? OutcomeValueSetActionCodes.UseSameNameAndCrossVersion
                        : OutcomeValueSetActionCodes.UseRenamedAndCrossVersion;
                }

                // create our value set outcome
                DbValueSetOutcome vsOutcome = new()
                {
                    Key = DbValueSetOutcome.GetIndex(),
                    ValueSetComparisonKey = sourceComparison.Key,

                    SourceFhirPackageKey = packagePair.SourcePackageKey,
                    SourceFhirSequence = packagePair.SourceFhirSequence,
                    SourceValueSetKey = sourceVs.Key,
                    SourceCanonicalUnversioned = sourceVs.UnversionedUrl,
                    SourceCanonicalVersioned = sourceVs.VersionedUrl,
                    SourceVersion = sourceVs.Version,
                    SourceId = sourceVs.Id,
                    SourceName = sourceVs.Name,
                    TotalSourceCount = -1,

                    TargetFhirPackageKey = packagePair.TargetPackageKey,
                    TargetFhirSequence = packagePair.TargetFhirSequence,
                    TargetValueSetKey = targetVs.Key,
                    TargetCanonicalUnversioned = targetVs.UnversionedUrl,
                    TargetCanonicalVersioned = targetVs.VersionedUrl,
                    TargetVersion = targetVs.Version,
                    TargetId = targetVs.Id,
                    TargetName = targetVs.Name,
                    TotalTargetCount = totalTargetCount,

                    PotentialGenResourceType = "ValueSet",
                    PotentialGenLongId = idLong,
                    PotentialGenShortId = idShort,
                    PotentialGenUrl = url,

                    IsRenamed = isRenamed,
                    IsUnmapped = isUnmapped,
                    IsIdentical = isIdentical,
                    IsEquivalent = isEquivalent,
                    IsBroaderThanTarget = isBroaderThanTarget,
                    IsNarrowerThanTarget = isNarrowerThanTarget,

                    FullyMapsToThisTarget = fullyMapsToThisTarget,
                    FullyMapsAcrossAllTargets = fullyMapsAcrossAllTargets,

                    Comments = sourceComparison.UserMessage ?? sourceComparison.TechnicalMessage ?? "TODO",
                    OutcomeAction = vsAction,

                    ContentKeys = sourceComparison.ContentKeys,
                };

                _vsOutcomeCache.CacheAdd(vsOutcome);

                HashSet<int> currentlyMappedComparisonKeys = fullyMappedConceptKeysByComparisonKey[sourceComparison.Key];

                // build our concept outcomes
                foreach (DbValueSetConceptComparison conceptComparison in conceptComparsionsByVsComparisonKey[sourceComparison.Key])
                {
                    DbValueSetConcept sourceConcept = allSourceConcepts[conceptComparison.SourceConceptKey];
                    DbValueSetConcept? targetConcept = conceptComparison.TargetConceptKey is null
                        ? null
                        : allTargetConcepts[conceptComparison.TargetConceptKey.Value];

                    OutcomeValueSetConceptActionCodes conceptAction = vsAction switch
                    {
                        OutcomeValueSetActionCodes.UseValueSetSameName => (sourceConcept.Code == targetConcept?.Code)
                            ? OutcomeValueSetConceptActionCodes.UseConceptSameCode
                            : OutcomeValueSetConceptActionCodes.UseConceptChangedCode,
                        OutcomeValueSetActionCodes.UseValueSetRenamed => (sourceConcept.Code == targetConcept?.Code)
                            ? OutcomeValueSetConceptActionCodes.UseConceptSameCode
                            : OutcomeValueSetConceptActionCodes.UseConceptChangedCode,

                        OutcomeValueSetActionCodes.UseSameNameAndCrossVersion => (targetConcept is null)
                            ? OutcomeValueSetConceptActionCodes.UseCrossVersionDefinition
                            : (sourceConcept.Code == targetConcept?.Code)
                            ? OutcomeValueSetConceptActionCodes.UseConceptSameCode
                            : OutcomeValueSetConceptActionCodes.UseConceptChangedCode,

                        OutcomeValueSetActionCodes.UseRenamedAndCrossVersion => (targetConcept is null)
                            ? OutcomeValueSetConceptActionCodes.UseCrossVersionDefinition
                            : (sourceConcept.Code == targetConcept?.Code)
                            ? OutcomeValueSetConceptActionCodes.UseConceptSameCode
                            : OutcomeValueSetConceptActionCodes.UseConceptChangedCode,

                        OutcomeValueSetActionCodes.UseOtherValueSets => OutcomeValueSetConceptActionCodes.MappedElsewhere,
                        OutcomeValueSetActionCodes.UseOtherAndCrossVersion => fullyMappedConceptKeys.Contains(conceptComparison.SourceConceptKey)
                            ? OutcomeValueSetConceptActionCodes.MappedElsewhere
                            : OutcomeValueSetConceptActionCodes.UseCrossVersionDefinition,

                        _ => OutcomeValueSetConceptActionCodes.UseCrossVersionDefinition,
                    };

                    DbValueSetConceptOutcome conceptOutcome = new()
                    {
                        Key = DbValueSetConceptOutcome.GetIndex(),
                        ValueSetOutcomeKey = vsOutcome.Key,
                        ValueSetConceptComparisonKey = conceptComparison.Key,

                        SourceFhirPackageKey = packagePair.SourcePackageKey,
                        SourceFhirSequence = packagePair.SourceFhirSequence,
                        SourceValueSetKey = sourceVs.Key,
                        SourceValueSetConceptKey = sourceConcept.Key,
                        SourceSystem = sourceConcept.System,
                        SourceCode = sourceConcept.Code,
                        TotalSourceCount = -1,

                        TargetFhirPackageKey = packagePair.TargetPackageKey,
                        TargetFhirSequence = packagePair.TargetFhirSequence,
                        TargetValueSetKey = targetVs.Key,
                        TargetValueSetConceptKey = targetConcept?.Key,
                        TargetSystem = targetConcept?.System,
                        TargetCode = targetConcept?.Code,
                        TotalTargetCount = -1,

                        IsRenamed = targetConcept is null ? false : (sourceConcept.Code != targetConcept.Code),
                        IsUnmapped = targetConcept is null || conceptComparison.NotMapped,
                        IsIdentical = conceptComparison.IsIdentical == true,
                        IsEquivalent = conceptComparison.Relationship == CMR.Equivalent,
                        IsBroaderThanTarget = conceptComparison.Relationship == CMR.SourceIsBroaderThanTarget,
                        IsNarrowerThanTarget = conceptComparison.Relationship == CMR.SourceIsNarrowerThanTarget,

                        FullyMapsToThisTarget = currentlyMappedComparisonKeys.Contains(sourceConcept.Key),
                        FullyMapsAcrossAllTargets = fullyMappedConceptKeys.Contains(sourceConcept.Key),

                        CodeLiteralsMatch = sourceConcept.Code == targetConcept?.Code,
                        SourceCodeTreatedAsEscapeValve = conceptComparison.SourceCodeTreatedAsEscapeValve,
                        TargetCodeTreatedAsEscapeValve = conceptComparison.TargetCodeTreatedAsEscapeValve,

                        Comments = sourceComparison.UserMessage ?? sourceComparison.TechnicalMessage ?? "TODO",
                        OutcomeAction = conceptAction,

                        ContentKeys = conceptComparison.ContentKeys,
                    };

                    _conceptOutcomeCache.CacheAdd(conceptOutcome);
                }

            }
        }
    }
}
