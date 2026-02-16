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
using Hl7.Fhir.Model;
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

        public DbElement? SourceRootElement { get; set; } = null;
        public DbElement? TargetRootElement { get; set; } = null;

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
    private DbRecordCache<DbElementOutcomeTarget> _edOutcomeTargetCache;

    public StructureOutcomeGenerator(
        IDbConnection db,
        ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
        _logger = loggerFactory.CreateLogger<StructureOutcomeGenerator>();

        _db = db;

        _sdOutcomeCache = new();
        _edOutcomeCache = new();
        _edOutcomeTargetCache = new();
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
            updateOutcomeContextsForContentReferences(packagePair);
        }
    }

    private void updateOutcomeContextsForContentReferences(FhirPackageComparisonPair packagePair)
    {
        DbFhirPackage sourcePackage = packagePair.SourcePackage;
        DbFhirPackage targetPackage = packagePair.TargetPackage;

        // get all the elements that are content references and export extensions
        List<DbElementOutcome> edOutcomesWithCRs = DbElementOutcome.SelectList(
            _db,
            SourceFhirPackageKey: sourcePackage.Key,
            TargetFhirPackageKey: targetPackage.Key,
            SourceUsedAsContentReference: true,
            SourceAncestorUsedAsContentReferenceIdIsNull: true,
            RequiresXVerDefinition: true,
            ParentRequiresXverDefinition: false);

        // iterate over these elements to add contexts of elements that use them as content references
        foreach (DbElementOutcome edOutcomeWithCR in edOutcomesWithCRs)
        {
            // find the elements that use this element as a content reference
            List<DbElement> referencingEds = DbElement.SelectList(
                _db,
                FhirPackageKey: sourcePackage.Key,
                ContentReferenceSourceKey: edOutcomeWithCR.SourceElementKey);

            int sourceIdLen = edOutcomeWithCR.SourceId.Length;

            List<string> ctxToAdd = [];
            // iterate over the contexts and create replacements as necessary
            foreach (string ctx in edOutcomeWithCR.ExtensionContexts)
            {
                if (!ctx.StartsWith(edOutcomeWithCR.SourceId, StringComparison.Ordinal))
                {
                    continue;
                }

                foreach (DbElement referencingEd in referencingEds)
                {
                    string newCtx = referencingEd.Id + ctx[sourceIdLen..];
                    ctxToAdd.Add(newCtx);
                }
            }

            if (ctxToAdd.Count == 0)
            {
                continue;
            }

            List<string> allCtx = [];
            allCtx.AddRange(edOutcomeWithCR.ExtensionContexts);
            allCtx.AddRange(ctxToAdd);

            edOutcomeWithCR.ExtensionContexts = allCtx.Order().Distinct().ToList();
            _edOutcomeCache.CacheUpdate(edOutcomeWithCR);
        }

        // do the same for every extension that has an ancestor that is a content reference
        edOutcomesWithCRs = DbElementOutcome.SelectList(
            _db,
            SourceFhirPackageKey: sourcePackage.Key,
            TargetFhirPackageKey: targetPackage.Key,
            SourceUsedAsContentReference: false,
            SourceAncestorUsedAsContentReferenceIdIsNull: false,
            RequiresXVerDefinition: true,
            ParentRequiresXverDefinition: false);

        // iterate over these elements to add contexts of elements that use them as content references
        foreach (DbElementOutcome edOutcomeWithCR in edOutcomesWithCRs)
        {
            string sourceId = edOutcomeWithCR.SourceAncestorUsedAsContentReferenceId!;

            // resolve the element
            DbElement? crEd = DbElement.SelectSingle(
                _db,
                FhirPackageKey: sourcePackage.Key,
                Id: sourceId);

            if (crEd is null)
            {
                throw new Exception($"Could not find content reference ancestor element with id {sourceId} for ElementOutcome with key {edOutcomeWithCR.Key}");
            }

            // find the elements that use this element as a content reference
            List<DbElement> referencingEds = DbElement.SelectList(
                _db,
                FhirPackageKey: sourcePackage.Key,
                ContentReferenceSourceKey: crEd.Key);

            int sourceIdLen = sourceId.Length;

            List<string> ctxToAdd = [];
            // iterate over the contexts and create replacements as necessary
            foreach (string ctx in edOutcomeWithCR.ExtensionContexts)
            {
                if (!ctx.StartsWith(sourceId, StringComparison.Ordinal))
                {
                    continue;
                }

                foreach (DbElement referencingEd in referencingEds)
                {
                    string newCtx = referencingEd.Id + ctx[sourceIdLen..];
                    ctxToAdd.Add(newCtx);
                }
            }

            if (ctxToAdd.Count == 0)
            {
                continue;
            }

            List<string> allCtx = [];
            allCtx.AddRange(edOutcomeWithCR.ExtensionContexts);
            allCtx.AddRange(ctxToAdd);

            edOutcomeWithCR.ExtensionContexts = allCtx.Order().Distinct().ToList();
            _edOutcomeCache.CacheUpdate(edOutcomeWithCR);
        }
#if false
        // get the outcomes that have content reference extension URLs
        List<DbElementOutcome> relatedEdOutcomes = DbElementOutcome.SelectList(
            _db,
            SourceFhirPackageKey: sourcePackage.Key,
            TargetFhirPackageKey: targetPackage.Key,
            ContentReferenceExtensionUrlIsNull: false);

        // iterate over the outcomes and update the contents of the primary definition
        foreach (DbElementOutcome relatedEdOutcome in relatedEdOutcomes)
        {
            if (relatedEdOutcome.ContentReferenceOutcomeKey is null)
            {
                throw new Exception($"ElementOutcome with key {relatedEdOutcome.Key} has a content reference extension URL but no ContentReferenceOutcomeKey");
            }

            // resolve the primary content reference outcome
            DbElementOutcome? crEdOutcome = DbElementOutcome.SelectSingle(
                _db,
                Key: relatedEdOutcome.ContentReferenceOutcomeKey);
            if (crEdOutcome is null)
            {
                throw new Exception($"Could not find primary ElementOutcome with key {relatedEdOutcome.ContentReferenceOutcomeKey} for ElementOutcome with key {relatedEdOutcome.Key}");
            }

            // merge the related outcome's extension contexts into the primary outcome's contexts
            HashSet<string> mergedContexts = new(crEdOutcome.ExtensionContexts);
            if (relatedEdOutcome.ExtensionContexts is not null)
            {
                foreach (string context in relatedEdOutcome.ExtensionContexts)
                {
                    mergedContexts.Add(context);
                }
            }

            // flag the primary outcome for update if necessary
            if (mergedContexts.Count != crEdOutcome.ExtensionContexts.Count)
            {
                crEdOutcome.ExtensionContexts = mergedContexts.Order().ToList();

                _edOutcomeCache.CacheUpdate(crEdOutcome);
            }
        }
#endif
        // apply our changes
        applyCachedChanges(packagePair);
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

        if (_edOutcomeTargetCache.ToAddCount > 0)
        {
            _logger.LogInformation($"Adding {_edOutcomeTargetCache.ToAddCount} element outcome targets from {sourcePackage.ShortName} to {targetPackage.ShortName}");
            _edOutcomeTargetCache.ToAdd.Insert(_db, ignoreDuplicates: true, insertPrimaryKey: true);
        }

        if (_edOutcomeTargetCache.ToUpdateCount > 0)
        {
            _logger.LogInformation($"Updating {_edOutcomeTargetCache.ToUpdateCount} element outcome targets from {sourcePackage.ShortName} to {targetPackage.ShortName}");
            _edOutcomeTargetCache.ToUpdate.Update(_db);
        }

        _sdOutcomeCache.Clear();
        _edOutcomeCache.Clear();
        _edOutcomeTargetCache.Clear();
    }

    private void buildOutcomes(FhirPackageComparisonPair packagePair)
    {
        _logger.LogInformation($"Generating Structure Outcomes for {packagePair.SourcePackageShortName}->{packagePair.TargetPackageShortName}...");

        // just get all the source and target structures and elements now to avoid hitting the db so much
        Dictionary<int, DbStructureDefinition> allSourceStructures = DbStructureDefinition.SelectDict(_db, FhirPackageKey: packagePair.SourcePackageKey);
        Dictionary<int, DbStructureDefinition> allTargetStructures = DbStructureDefinition.SelectDict(_db, FhirPackageKey: packagePair.TargetPackageKey);

        Dictionary<string, DbElement> sourceStructureRootElements = DbElement.SelectList(
            _db,
            FhirPackageKey: packagePair.SourcePackageKey,
            ResourceFieldOrder: 0)
            .ToDictionary(ed => ed.Id);
        Dictionary<string, DbElement> targetStructureRootElements = DbElement.SelectList(
            _db,
            FhirPackageKey: packagePair.TargetPackageKey,
            ResourceFieldOrder: 0)
            .ToDictionary(ed => ed.Id);

        List <DbStructureComparison> sdComparisons = DbStructureComparison.SelectList(
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
            _edOutcomeCache,
            _edOutcomeTargetCache);

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
                if (structureComparison.NotMapped ||
                    (structureComparison.TargetContentKey is null) ||
                    !allTargetStructures.TryGetValue(structureComparison.TargetContentKey.Value, out DbStructureDefinition? targetSd))
                {
                    trackingRecords.Add(
                        0,
                        new()
                        {
                            SourceStructure = sourceSd,
                            SourceRootElement = sourceStructureRootElements.GetValueOrDefault(sourceSd.Id),
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
                        SourceRootElement = sourceStructureRootElements.GetValueOrDefault(sourceSd.Id),
                        StructureComparison = structureComparison,
                        TargetStructure = targetSd,
                        TargetRootElement = targetStructureRootElements.GetValueOrDefault(targetSd.Id),
                        StructureOutcomeKey = DbStructureOutcome.GetIndex(),
                        IsFullyMappedAcrossAllTargets = true,
                        IsFullyMappedToThisTarget = true,
                    });
            }

            // process elements to determine how elements map across our target structures, if neither side is a primitive type
            elementOutcomeGenerator.ProcessSourceStructure(sourceSd, trackingRecords);

            int discreteTargetCount = trackingRecords.Values
                .Where(tr => tr.TargetStructure is not null)
                .Select(tr => tr.TargetStructure!.Key)
                .Distinct()
                .Count();

            // iterate over our tracking records to build structure outcomes
            foreach (StructureOutcomeTrackingRecord sdTr in trackingRecords.Values)
            {
                DbStructureDefinition? targetSd = sdTr.TargetStructure;

                (string idLong, string idShort, string name) = XVerProcessor.GenerateProfileId(
                    packagePair.SourcePackageShortName,
                    sourceSd.Id,
                    packagePair.TargetPackageShortName,
                    targetSd?.Id);

                string url = $"http://hl7.org/fhir/{packagePair.SourceFhirVersionShort}/StructureDefinition/{idLong}";

                (string emIdLong, string emIdShort, string emName) = XVerProcessor.GenerateSdElementMapId(
                    packagePair.SourcePackageShortName,
                    sourceSd.Id,
                    packagePair.TargetPackageShortName,
                    targetSd?.Id);

                string emUrl = $"http://hl7.org/fhir/{packagePair.SourceFhirVersionShort}/ConceptMap/{emIdLong}";

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
                        structureOutcomeKey: sdTr.StructureOutcomeKey,
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
                    GenName = name,
                    GenFileName = $"StructureDefinition-{idShort}.json",

                    ElementConceptMapLongId = emIdLong,
                    ElementConceptMapShortId = emIdShort,
                    ElementConceptMapUrl = emUrl,
                    ElementConceptMapName = emName,
                    ElementConceptMapFileName = $"{emIdLong}.json",
                };

                _sdOutcomeCache.CacheAdd(sdOutcome);
            }
        }
    }

    private DbStructureOutcome createNoMapStructureOutcome(
        FhirPackageComparisonPair packagePair,
        DbStructureDefinition sourceSd,
        int? structureOutcomeKey,
        int? structureComparisonKey,
        string? comments)
    {
        (string idLong, string idShort, string name) = XVerProcessor.GenerateProfileId(
            packagePair.SourcePackageShortName,
            sourceSd.Id,
            packagePair.TargetPackageShortName);

        string url = $"http://hl7.org/fhir/{packagePair.SourceFhirVersionShort}/StructureDefinition/{idLong}";

        (string emIdLong, string emIdShort, string emName) = XVerProcessor.GenerateSdElementMapId(
            packagePair.SourcePackageShortName,
            sourceSd.Id,
            packagePair.TargetPackageShortName);

        string emUrl = $"http://hl7.org/fhir/{packagePair.SourceFhirVersionShort}/ConceptMap/{emIdLong}";

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
            GenName = name,
            GenFileName = $"StructureDefinition-{idShort}.json",

            ElementConceptMapLongId = emIdLong,
            ElementConceptMapShortId = emIdShort,
            ElementConceptMapUrl = emUrl,
            ElementConceptMapName = emName,
            ElementConceptMapFileName = $"{emIdLong}.json",

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
