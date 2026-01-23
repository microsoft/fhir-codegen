using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fhir.CodeGen.Common.Packaging;
using Fhir.CodeGen.Comparison.CompareTool;
using Fhir.CodeGen.Comparison.Models;
using Fhir.CodeGen.Comparison.XVer;
using Microsoft.Extensions.Logging;
using CMR = Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship;


namespace Fhir.CodeGen.Comparison.Outcomes;

public class StructureOutcomeGenerator
{
    public class StructureOutcomeTrackingRecord
    {
        public required DbStructureDefinition SourceStructure { get; set; }
        public required DbStructureComparison StructureComparison { get; set; }
        public required DbStructureDefinition? TargetStructure { get; set; }

        public DbElement? RootElement { get; set; } = null;

        public required int StructureOutcomeKey { get; set; }

        public List<DbElementOutcome> ElementOutcomes { get; set; } = [];

        public bool IsFullyMappedAcrossAllTargets { get; set; } = false;
        public bool IsFullyMappedToThisTarget { get; set; } = false;

        public List<string> Messages { get; } = [];
    }

    private readonly IDbConnection _db;

    private ILoggerFactory _loggerFactory;
    private ILogger _logger;

    private List<DbFhirPackage> _packages = [];

    private DbRecordCache<DbStructureOutcome> _sdOutcomeCache;
    private DbRecordCache<DbElementOutcome> _edOutcomeCache;

    public StructureOutcomeGenerator(
        IDbConnection db,
        ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
        _logger = loggerFactory.CreateLogger<StructureOutcomeGenerator>();

        _db = db;

        _sdOutcomeCache = new();
        _edOutcomeCache = new();
    }


    public void CreateOutcomesForStructures(
        int? maxStepSize = null,
        HashSet<(FhirReleases.FhirSequenceCodes s, FhirReleases.FhirSequenceCodes t)>? specificPairs = null)
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
            if ((specificPairs is not null) &&
                !specificPairs.Contains((packagePair.SourceFhirSequence, packagePair.TargetFhirSequence)))
            {
                continue;
            }

            buildOutcomes(packagePair);
            applyCachedChanges(packagePair);
        }
    }


    private void applyCachedChanges(FhirPackageComparisonPair packagePair)
    {
        DbFhirPackage sourcePackage = packagePair.SourcePackage;
        DbFhirPackage targetPackage = packagePair.TargetPackage;

        if (_sdOutcomeCache.ToAddCount > 0)
        {
            _logger.LogInformation($"Adding {_sdOutcomeCache.ToAddCount} structure outcomes from {sourcePackage.ShortName} to {targetPackage.ShortName}");
            _sdOutcomeCache.ToAdd.Insert(_db, ignoreDuplicates: true, insertPrimaryKey: true);
        }

        if (_sdOutcomeCache.ToUpdateCount > 0)
        {
            _logger.LogInformation($"Updating {_sdOutcomeCache.ToUpdateCount} structure outcomes from {sourcePackage.ShortName} to {targetPackage.ShortName}");
            _sdOutcomeCache.ToUpdate.Update(_db);
        }

        if (_edOutcomeCache.ToAddCount > 0)
        {
            _logger.LogInformation($"Adding {_edOutcomeCache.ToAddCount} element outcomes from {sourcePackage.ShortName} to {targetPackage.ShortName}");
            _edOutcomeCache.ToAdd.Insert(_db, ignoreDuplicates: true, insertPrimaryKey: true);
        }

        if (_edOutcomeCache.ToUpdateCount > 0)
        {
            _logger.LogInformation($"Updating {_edOutcomeCache.ToUpdateCount} element outcomes from {sourcePackage.ShortName} to {targetPackage.ShortName}");
            _edOutcomeCache.ToUpdate.Update(_db);
        }

        _sdOutcomeCache.Clear();
        _edOutcomeCache.Clear();
    }

    private void buildOutcomes(FhirPackageComparisonPair packagePair)
    {
        _logger.LogInformation($"Generating Structure Outcomes for {packagePair.SourcePackageShortName}->{packagePair.TargetPackageShortName}...");

        // just get all the source and target structures and elements now to avoid hitting the db so much
        Dictionary<int, DbStructureDefinition> allSourceStructures = DbStructureDefinition.SelectDict(_db, FhirPackageKey: packagePair.SourcePackageKey);
        Dictionary<int, DbStructureDefinition> allTargetStructures = DbStructureDefinition.SelectDict(_db, FhirPackageKey: packagePair.TargetPackageKey);

        List<DbStructureComparison> sdComparisons = DbStructureComparison.SelectList(
            _db,
            SourceFhirPackageKey: packagePair.SourcePackageKey,
            TargetFhirPackageKey: packagePair.TargetPackageKey);

        _logger.LogInformation(
            $"Structure comparisons: {sdComparisons.Count}, from" +
            $" {allSourceStructures.Count} Structures" +
            $" to {allTargetStructures.Count} Structures");

        // create lookups of our comparisons that we need to generate outcomes
        ILookup<int, DbStructureComparison> sdComparsionsBySourceKey = sdComparisons.ToLookup(c => c.SourceContentKey);
        ILookup<int, DbStructureComparison> sdComparsionsByTargetKey = sdComparisons
            .Where(c => c.TargetContentKey is not null)
            .ToLookup(c => c.TargetContentKey!.Value);

        // create our element outcome generator for this package pair
        ElementOutcomeGenerator elementOutcomeGenerator = new(
            _db,
            _loggerFactory,
            packagePair,
            _edOutcomeCache);

        // iterate over our source structures
        foreach (DbStructureDefinition sourceSd in allSourceStructures.Values)
        {
            // skip primitive structures
            if (sourceSd.ArtifactClass == Common.Models.FhirArtifactClassEnum.PrimitiveType)
            {
                continue;
            }

            List<DbStructureComparison> structureComparisons = sdComparsionsBySourceKey[sourceSd.Key]
                .Where(sc => sc.TargetFhirPackageKey == packagePair.TargetPackageKey)
                .ToList();
            if (structureComparisons.Count == 0)
            {
                // create our structure no-map outcome
                DbStructureOutcome noMapSdOutcome = createNoMapStructureOutcome(
                    packagePair,
                    sourceSd,
                    structureOutcomeKey: null,
                    structureComparisonKey: null,
                    comments: null);

                // create our element no-map outcomes
                elementOutcomeGenerator.ProcessNoMapStructure(sourceSd, noMapSdOutcome);

                continue;
            }

            Dictionary<int, StructureOutcomeTrackingRecord> trackingRecords = [];

            // build our tracking records, be optimistic that all elements will be fully mapped
            foreach (DbStructureComparison structureComparison in structureComparisons)
            {
                if (structureComparison.NotMapped || (structureComparison.TargetContentKey is null))
                {
                    trackingRecords.Add(
                        0,
                        new()
                        {
                            SourceStructure = sourceSd,
                            StructureComparison = structureComparison,
                            TargetStructure = null,
                            StructureOutcomeKey = DbStructureOutcome.GetIndex(),
                            IsFullyMappedAcrossAllTargets = true,
                            IsFullyMappedToThisTarget = false,
                        });
                    continue;
                }

                trackingRecords.Add(
                    structureComparison.TargetContentKey!.Value,
                    new()
                    {
                        SourceStructure = sourceSd,
                        StructureComparison = structureComparison,
                        TargetStructure = allTargetStructures[structureComparison.TargetContentKey.Value],
                        StructureOutcomeKey = DbStructureOutcome.GetIndex(),
                        IsFullyMappedAcrossAllTargets = true,
                        IsFullyMappedToThisTarget = true,
                    });
            }

            // process elements to determine how elements map across our target structures, if neither side is a primitive type
            elementOutcomeGenerator.ProcessStructure(sourceSd, trackingRecords);

            int discreteTargetCount = trackingRecords.Values
                .Where(tr => tr.TargetStructure is not null)
                .Select(tr => tr.TargetStructure!.Key)
                .Distinct()
                .Count();

            // iterate over our tracking records to build structure outcomes
            foreach (StructureOutcomeTrackingRecord sdTr in trackingRecords.Values)
            {
                DbStructureDefinition? targetSd = sdTr.TargetStructure;

                (string idLong, string idShort) = XVerProcessor.GenerateArtifactId(
                    packagePair.SourcePackageShortName,
                    sourceSd.Id,
                    packagePair.TargetPackageShortName,
                    targetSd?.Id);

                string url = $"http://hl7.org/fhir/{packagePair.SourcePackageShortName}/StructureDefinition/{idLong}";

                if (sdTr.StructureComparison.NotMapped || (targetSd is null))
                {
                    bool noMapSdRequiresXVer = !sdTr.IsFullyMappedAcrossAllTargets;

                    string noMapComments;
                    if (sdTr.Messages.Count > 0)
                    {
                        noMapComments =
                            $"FHIR {packagePair.SourceFhirSequence} `{sourceSd.Name}` does not map to" +
                            $" FHIR {packagePair.TargetFhirSequence}." +
                            $"\n{string.Join('\n', sdTr.Messages)}";
                    }
                    else
                    {
                        noMapComments =
                            $"FHIR {packagePair.SourceFhirSequence} `{sourceSd.Name}` does not map to" +
                            $" FHIR {packagePair.TargetFhirSequence}.";
                        if (sdTr.StructureComparison.TechnicalMessage is not null)
                        {
                            noMapComments += "\n" + sdTr.StructureComparison.TechnicalMessage;
                        }
                        if (sdTr.StructureComparison.UserMessage is not null)
                        {
                            noMapComments += "\n" + sdTr.StructureComparison.UserMessage;
                        }
                    }

                    // create our no-map structure outcome
                    DbStructureOutcome noMapSdOutcome = createNoMapStructureOutcome(
                        packagePair,
                        sourceSd,
                        structureOutcomeKey: null,
                        structureComparisonKey: null,
                        comments: noMapComments);

                    // element outcomes have been created in the element outcome generator
                    continue;
                }

                bool isRenamed = (discreteTargetCount == 1) && (sourceSd.Id != targetSd.Id);
                bool isUnmapped = false;
                bool isIdentical = sdTr.StructureComparison.IsIdentical == true;
                bool isEquivalent = sdTr.StructureComparison.Relationship == CMR.Equivalent;
                bool isBroaderThanTarget = sdTr.StructureComparison.Relationship == CMR.SourceIsBroaderThanTarget;
                bool isNarrowerThanTarget = sdTr.StructureComparison.Relationship == CMR.SourceIsNarrowerThanTarget;

                bool fullyMapsToThisTarget = sdTr.IsFullyMappedToThisTarget;

                bool sdRequiresXVer;

                if (isIdentical)
                {
                    sdRequiresXVer = false;
                }
                else if (isEquivalent)
                {
                    sdRequiresXVer = false;
                }
                else if (sdTr.IsFullyMappedAcrossAllTargets)
                {
                    sdRequiresXVer = false;
                }
                else
                {
                    sdRequiresXVer = true;
                }

                string comments;
                if (sdTr.Messages.Count > 0)
                {
                    comments =
                        $"FHIR {packagePair.SourceFhirSequence} `{sourceSd.Name}` is mapped to " +
                        $" FHIR {packagePair.TargetFhirSequence} `{targetSd.Name}`." +
                        $"\n{string.Join('\n', sdTr.Messages)}";
                }
                else
                {
                    comments = 
                        $"FHIR {packagePair.SourceFhirSequence} `{sourceSd.Name}` is mapped to " +
                        $" FHIR {packagePair.TargetFhirSequence} `{targetSd.Name}`.";
                    if (sdTr.StructureComparison.TechnicalMessage is not null)
                    {
                        comments += "\n" + sdTr.StructureComparison.TechnicalMessage;
                    }
                    if (sdTr.StructureComparison.UserMessage is not null)
                    {
                        comments += "\n" + sdTr.StructureComparison.UserMessage;
                    }
                }

                // create our structure outcome
                DbStructureOutcome sdOutcome = new()
                {
                    Key = sdTr.StructureOutcomeKey,
                    StructureComparisonKey = sdTr.StructureComparison.Key,

                    SourceFhirPackageKey = packagePair.SourcePackageKey,
                    SourceFhirSequence = packagePair.SourceFhirSequence,
                    SourceStructureKey = sourceSd.Key,
                    SourceArtifactClass = sourceSd.ArtifactClass,
                    TotalSourceCount = -1,

                    TargetFhirPackageKey = packagePair.TargetPackageKey,
                    TargetFhirSequence = packagePair.TargetFhirSequence,
                    TargetStructureKey = targetSd.Key,
                    TargetArtifactClass = targetSd.ArtifactClass,
                    TotalTargetCount = discreteTargetCount,

                    RequiresXVerDefinition = sdRequiresXVer,

                    IsRenamed = isRenamed,
                    IsUnmapped = isUnmapped,
                    IsIdentical = isIdentical,
                    IsEquivalent = isEquivalent,
                    IsBroaderThanTarget = isBroaderThanTarget,
                    IsNarrowerThanTarget = isNarrowerThanTarget,

                    FullyMapsToThisTarget = fullyMapsToThisTarget,
                    FullyMapsAcrossAllTargets = sdTr.IsFullyMappedAcrossAllTargets,

                    Comments = comments,

                    SourceCanonicalUnversioned = sourceSd.UnversionedUrl,
                    SourceCanonicalVersioned = sourceSd.VersionedUrl,
                    SourceVersion = sourceSd.Version,
                    SourceId = sourceSd.Id,
                    SourceName = sourceSd.Name,
                    TargetCanonicalUnversioned = targetSd.UnversionedUrl,
                    TargetCanonicalVersioned = targetSd.VersionedUrl,
                    TargetVersion = targetSd.Version,
                    TargetId = targetSd.Id,
                    TargetName = targetSd.Name,

                    GenLongId = idLong,
                    GenShortId = idShort,
                    GenUrl = url,
                };

                _sdOutcomeCache.CacheAdd(sdOutcome);
            }



            //bool fullyMapsAcrossAllTargets = fullyMappedElementKeys.Count == sourceElements.Count;
            //int totalTargetCount = targetStructures.Count;

            //// traverse our comparisons to build matching outcomes
            //foreach (DbStructureComparison sourceComparison in structureComparisons)
            //{
            //    DbStructureDefinition? targetSd = sourceComparison.TargetContentKey is null
            //        ? null
            //        : targetStructures[sourceComparison.TargetContentKey.Value];

            //    (string idLong, string idShort) = XVerProcessor.GenerateArtifactId(
            //        packagePair.SourcePackageShortName,
            //        sourceSd.Id,
            //        packagePair.TargetPackageShortName,
            //        targetSd?.Id);

            //    string url = $"http://hl7.org/fhir/{packagePair.SourcePackageShortName}/StructureDefinition/{idLong}";

            //    if ((targetSd is null) ||
            //        sourceComparison.NotMapped)
            //    {
            //        bool noMapSdRequiresXVer = fullyMapsAcrossAllTargets != true;

            //        // build our no-map outcome
            //        DbStructureOutcome noMapOutcome = new()
            //        {
            //            Key = DbStructureOutcome.GetIndex(),
            //            StructureComparisonKey = sourceComparison.Key,

            //            SourceFhirPackageKey = packagePair.SourcePackageKey,
            //            SourceFhirSequence = packagePair.SourceFhirSequence,
            //            SourceStructureKey = sourceSd.Key,
            //            SourceArtifactClass = sourceSd.ArtifactClass,
            //            TotalSourceCount = -1,

            //            TargetFhirPackageKey = packagePair.TargetPackageKey,
            //            TargetFhirSequence = packagePair.TargetFhirSequence,
            //            TargetStructureKey = null,
            //            TargetArtifactClass = null,

            //            TotalTargetCount = totalTargetCount,

            //            RequiresXVerDefinition = noMapSdRequiresXVer,

            //            IsRenamed = false,
            //            IsUnmapped = false,
            //            IsIdentical = false,
            //            IsEquivalent = false,
            //            IsBroaderThanTarget = false,
            //            IsNarrowerThanTarget = false,

            //            FullyMapsToThisTarget = false,
            //            FullyMapsAcrossAllTargets = fullyMapsAcrossAllTargets,

            //            Comments = sourceComparison.UserMessage ?? sourceComparison.TechnicalMessage ?? "TODO",

            //            SourceCanonicalUnversioned = sourceSd.UnversionedUrl,
            //            SourceCanonicalVersioned = sourceSd.VersionedUrl,
            //            SourceVersion = sourceSd.Version,
            //            SourceId = sourceSd.Id,
            //            SourceName = sourceSd.Name,
            //            TargetCanonicalUnversioned = null,
            //            TargetCanonicalVersioned = null,
            //            TargetVersion = null,
            //            TargetId = null,
            //            TargetName = null,
            //            GenLongId = idLong,
            //            //GenShortId = idShort,
            //            //GenUrl = url,
            //        };

            //        _sdOutcomeCache.CacheAdd(noMapOutcome);

            //        // build our no-map element outcomes
            //        foreach (DbElementComparison elementComparison in edComparsionsBySdComparisonKey[sourceComparison.Key])
            //        {
            //            DbElement sourceElement = allSourceElements[elementComparison.SourceContentKey];

            //            (string extIdLong, string extIdShort) = XVerProcessor.GenerateExtensionId(
            //                packagePair.SourcePackageShortName,
            //                sourceElement.Id);

            //            DbElementOutcome noMapElementOutcome = new()
            //            {
            //                Key = DbElementOutcome.GetIndex(),
            //                StructureOutcomeKey = noMapOutcome.Key,
            //                ElementComparisonKey = elementComparison.Key,

            //                SourceFhirPackageKey = packagePair.SourcePackageKey,
            //                SourceFhirSequence = packagePair.SourceFhirSequence,
            //                SourceStructureKey = sourceSd.Key,
            //                SourceElementKey = sourceElement.Key,
            //                TotalSourceCount = -1,

            //                TargetFhirPackageKey = packagePair.TargetPackageKey,
            //                TargetFhirSequence = packagePair.TargetFhirSequence,
            //                TargetStructureKey = null,
            //                TargetElementKey = null,
            //                TotalTargetCount = -1,

            //                RequiresXVerDefinition = noMapSdRequiresXVer,
            //                AncestorElementOutcomeKey = null,

            //                IsRenamed = false,
            //                IsUnmapped = false,
            //                IsIdentical = false,
            //                IsEquivalent = false,
            //                IsBroaderThanTarget = false,
            //                IsNarrowerThanTarget = false,

            //                FullyMapsToThisTarget = false,
            //                FullyMapsAcrossAllTargets = fullyMappedElementKeys.Contains(sourceElement.Key),

            //                Comments = sourceComparison.UserMessage ?? sourceComparison.TechnicalMessage ?? "TODO",

            //                SourceCanonicalUnversioned = sourceSd.UnversionedUrl,
            //                SourceCanonicalVersioned = sourceSd.VersionedUrl,
            //                SourceVersion = sourceSd.Version,
            //                SourceId = sourceElement.Id,
            //                SourceName = sourceElement.Name,
            //                TargetCanonicalUnversioned = null,
            //                TargetCanonicalVersioned = null,
            //                TargetVersion = null,
            //                TargetId = null,
            //                TargetName = null,
            //                GenLongId = extIdLong,
            //                //GenShortId = extIdShort,
            //                //GenUrl = extUrl,
            //            };

            //            _edOutcomeCache.CacheAdd(noMapElementOutcome);
            //        }

            //        // move to next comparison
            //        continue;
            //    }

            //    int targetSdKey = targetSd.Key;

            //    Dictionary<int, DbElement> targetElements = targetElementsBySdKey[targetSdKey];

            //    bool isRenamed = (totalTargetCount == 1) && (sourceSd.Id != targetSd.Id);
            //    bool isUnmapped = false;
            //    bool isIdentical = sourceComparison.IsIdentical == true;
            //    bool isEquivalent = sourceComparison.Relationship == CMR.Equivalent;
            //    bool isBroaderThanTarget = sourceComparison.Relationship == CMR.SourceIsBroaderThanTarget;
            //    bool isNarrowerThanTarget = sourceComparison.Relationship == CMR.SourceIsNarrowerThanTarget;

            //    bool fullyMapsToThisTarget = fullyMappedElementKeysByComparisonKey.TryGetValue(sourceComparison.Key, out HashSet<int>? fmKeys) &&
            //        (fmKeys.Count == sourceElements.Count);

            //    bool sdRequiresXVer;

            //    if (isIdentical)
            //    {
            //        sdRequiresXVer = false;
            //    }
            //    else if (isEquivalent)
            //    {
            //        sdRequiresXVer = false;
            //    }
            //    else if (fullyMapsAcrossAllTargets)
            //    {
            //        sdRequiresXVer = false;
            //    }
            //    else
            //    {
            //        sdRequiresXVer = true;
            //    }

            //    // create our structure outcome
            //    DbStructureOutcome sdOutcome = new()
            //    {
            //        Key = DbStructureOutcome.GetIndex(),
            //        StructureComparisonKey = sourceComparison.Key,

            //        SourceFhirPackageKey = packagePair.SourcePackageKey,
            //        SourceFhirSequence = packagePair.SourceFhirSequence,
            //        SourceStructureKey = sourceSd.Key,
            //        SourceArtifactClass = sourceSd.ArtifactClass,
            //        TotalSourceCount = -1,

            //        TargetFhirPackageKey = packagePair.TargetPackageKey,
            //        TargetFhirSequence = packagePair.TargetFhirSequence,
            //        TargetStructureKey = targetSd.Key,
            //        TargetArtifactClass = targetSd.ArtifactClass,
            //        TotalTargetCount = totalTargetCount,

            //        RequiresXVerDefinition = sdRequiresXVer,

            //        IsRenamed = isRenamed,
            //        IsUnmapped = isUnmapped,
            //        IsIdentical = isIdentical,
            //        IsEquivalent = isEquivalent,
            //        IsBroaderThanTarget = isBroaderThanTarget,
            //        IsNarrowerThanTarget = isNarrowerThanTarget,

            //        FullyMapsToThisTarget = fullyMapsToThisTarget,
            //        FullyMapsAcrossAllTargets = fullyMapsAcrossAllTargets,

            //        Comments = sourceComparison.UserMessage ?? sourceComparison.TechnicalMessage ?? "TODO",

            //        SourceCanonicalUnversioned = sourceSd.UnversionedUrl,
            //        SourceCanonicalVersioned = sourceSd.VersionedUrl,
            //        SourceVersion = sourceSd.Version,
            //        SourceId = sourceSd.Id,
            //        SourceName = sourceSd.Name,
            //        TargetCanonicalUnversioned = targetSd.UnversionedUrl,
            //        TargetCanonicalVersioned = targetSd.VersionedUrl,
            //        TargetVersion = targetSd.Version,
            //        TargetId = targetSd.Id,
            //        TargetName = targetSd.Name,
            //        GenLongId = idLong,
            //        //GenShortId = idShort,
            //        //GenUrl = url,
            //    };

            //    _sdOutcomeCache.CacheAdd(sdOutcome);

            //    //HashSet<int> currentlyMappedComparisonKeys = fullyMappedElementKeysByComparisonKey.TryGetValue(sourceComparison.Key, out HashSet<int>? fmcKeys)
            //    //    ? fmcKeys
            //    //    : [];
            //    //Dictionary<int, (int outcomeKey, string id)> edKeyToDefinitionOutcomeKeyMap = [];

            //    //// build our element outcomes
            //    //foreach (DbElementComparison elementComparison in edComparsionsBySdComparisonKey[sourceComparison.Key])
            //    //{
            //    //    DbElement sourceElement = allSourceElements[elementComparison.SourceContentKey];
            //    //    DbElement? targetElement = elementComparison.TargetContentKey is null
            //    //        ? null
            //    //        : allTargetElements[elementComparison.TargetContentKey.Value];

            //    //    bool elementFullyMapsToThisTarget = currentlyMappedComparisonKeys.Contains(sourceElement.Key);
            //    //    bool elementFullyMapsAcrossAllTargets = fullyMappedElementKeys.Contains(sourceElement.Key);

            //    //    int? partOfXVerOutcomeKey = null;
            //    //    string? partOfXVerOutcomeId = null;

            //    //    bool elementRequiresXVer = !elementFullyMapsToThisTarget && !elementFullyMapsAcrossAllTargets;
            //    //    if (elementRequiresXVer &&
            //    //        (sourceElement.ParentElementKey is not null) &&
            //    //        edKeyToDefinitionOutcomeKeyMap.TryGetValue(sourceElement.ParentElementKey!.Value, out (int outcomeKey, string id) po))
            //    //    {
            //    //        partOfXVerOutcomeKey = po.outcomeKey;
            //    //        partOfXVerOutcomeId = po.id;
            //    //    }

            //    //    (string extIdLong, string extIdShort) = XVerProcessor.GenerateExtensionId(
            //    //        packagePair.SourcePackageShortName,
            //    //        sourceElement.Id);

            //    //    DbElementOutcome elementOutcome = new()
            //    //    {
            //    //        Key = DbElementOutcome.GetIndex(),
            //    //        StructureOutcomeKey = sdOutcome.Key,
            //    //        ElementComparisonKey = elementComparison.Key,

            //    //        SourceFhirPackageKey = packagePair.SourcePackageKey,
            //    //        SourceFhirSequence = packagePair.SourceFhirSequence,
            //    //        SourceStructureKey = sourceSd.Key,
            //    //        SourceElementKey = sourceElement.Key,
            //    //        TotalSourceCount = -1,

            //    //        TargetFhirPackageKey = packagePair.TargetPackageKey,
            //    //        TargetFhirSequence = packagePair.TargetFhirSequence,
            //    //        TargetStructureKey = targetSd.Key,
            //    //        TargetElementKey = targetElement?.Key,
            //    //        TotalTargetCount = -1,

            //    //        RequiresXVerDefinition = elementRequiresXVer,
            //    //        AncestorElementOutcomeKey = partOfXVerOutcomeKey,

            //    //        IsRenamed = targetElement is null ? false : (sourceElement.Name != targetElement.Name),
            //    //        IsUnmapped = targetElement is null || elementComparison.NotMapped,
            //    //        IsIdentical = elementComparison.IsIdentical == true,
            //    //        IsEquivalent = elementComparison.Relationship == CMR.Equivalent,
            //    //        IsBroaderThanTarget = elementComparison.Relationship == CMR.SourceIsBroaderThanTarget,
            //    //        IsNarrowerThanTarget = elementComparison.Relationship == CMR.SourceIsNarrowerThanTarget,

            //    //        FullyMapsToThisTarget = elementFullyMapsToThisTarget,
            //    //        FullyMapsAcrossAllTargets = elementFullyMapsAcrossAllTargets,

            //    //        Comments = sourceComparison.UserMessage ?? sourceComparison.TechnicalMessage ?? "TODO",

            //    //        SourceCanonicalUnversioned = sourceSd.UnversionedUrl,
            //    //        SourceCanonicalVersioned = sourceSd.VersionedUrl,
            //    //        SourceVersion = sourceSd.Version,
            //    //        SourceId = sourceElement.Id,
            //    //        SourceName = sourceElement.Name,
            //    //        TargetCanonicalUnversioned = null,
            //    //        TargetCanonicalVersioned = null,
            //    //        TargetVersion = null,
            //    //        TargetId = null,
            //    //        TargetName = null,
            //    //        GenLongId = partOfXVerOutcomeId ?? extIdLong,
            //    //        //GenShortId = extIdShort,
            //    //        //GenUrl = extUrl,
            //    //    };

            //    //    _edOutcomeCache.CacheAdd(elementOutcome);

            //    //    if (elementRequiresXVer)
            //    //    {
            //    //        edKeyToDefinitionOutcomeKeyMap.Add(
            //    //            elementOutcome.Key,
            //    //            (partOfXVerOutcomeKey ?? elementOutcome.Key, elementOutcome.GenLongId));
            //    //    }
            //    //}

            //}
        }
    }

    private DbStructureOutcome createNoMapStructureOutcome(
        FhirPackageComparisonPair packagePair,
        DbStructureDefinition sourceSd,
        int? structureOutcomeKey,
        int? structureComparisonKey,
        string? comments)
    {
        (string idLong, string idShort) = XVerProcessor.GenerateArtifactId(
            packagePair.SourcePackageShortName,
            sourceSd.Id,
            packagePair.TargetPackageShortName);

        string url = $"http://hl7.org/fhir/{packagePair.SourcePackageShortName}/StructureDefinition/{idLong}";

        structureOutcomeKey ??= DbStructureOutcome.GetIndex();
        comments ??=
            $"FHIR {packagePair.SourceFhirSequence} `{sourceSd.Name}` does not map to" +
            $" FHIR {packagePair.TargetFhirSequence}.";

        // build our no-map outcome
        DbStructureOutcome noMapOutcome = new()
        {
            Key = structureOutcomeKey!.Value,
            StructureComparisonKey = structureComparisonKey,

            SourceFhirPackageKey = packagePair.SourcePackageKey,
            SourceFhirSequence = packagePair.SourceFhirSequence,
            SourceStructureKey = sourceSd.Key,
            SourceArtifactClass = sourceSd.ArtifactClass,
            TotalSourceCount = -1,

            TargetFhirPackageKey = packagePair.TargetPackageKey,
            TargetFhirSequence = packagePair.TargetFhirSequence,
            TargetStructureKey = null,
            TargetArtifactClass = null,

            TotalTargetCount = 0,

            RequiresXVerDefinition = true,
            GenLongId = idLong,
            GenShortId = idShort,
            GenUrl = url,

            IsRenamed = false,
            IsUnmapped = false,
            IsIdentical = false,
            IsEquivalent = false,
            IsBroaderThanTarget = false,
            IsNarrowerThanTarget = false,

            FullyMapsToThisTarget = false,
            FullyMapsAcrossAllTargets = false,

            Comments = comments,

            SourceCanonicalUnversioned = sourceSd.UnversionedUrl,
            SourceCanonicalVersioned = sourceSd.VersionedUrl,
            SourceVersion = sourceSd.Version,
            SourceId = sourceSd.Id,
            SourceName = sourceSd.Name,
            TargetCanonicalUnversioned = null,
            TargetCanonicalVersioned = null,
            TargetVersion = null,
            TargetId = null,
            TargetName = null,
        };

        _sdOutcomeCache.CacheAdd(noMapOutcome);
        return noMapOutcome;
    }
}
