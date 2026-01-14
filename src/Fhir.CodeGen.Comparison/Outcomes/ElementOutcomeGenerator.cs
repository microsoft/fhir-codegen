using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;
using Fhir.CodeGen.Comparison.CompareTool;
using Fhir.CodeGen.Comparison.Models;
using Fhir.CodeGen.Comparison.XVer;
using Microsoft.Extensions.Logging;
using CMR = Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship;

namespace Fhir.CodeGen.Comparison.Outcomes;

public class ElementOutcomeGenerator
{
    private class ElementOutcomeTrackingRecord
    {
        public required DbElement SourceElement { get; set; }
        public List<DbElementOutcome> ElementOutcomes { get; set; } = [];
        public List<DbElementComparison> ElementComparisons { get; set; } = [];
        public List<string> Messages { get; set; } = [];
        public bool IsFullyMappedAcrossAllTargets { get; set; } = false;
        public List<DbElementComparison> MapsToIndividualTargets { get; set; } = [];
        public List<List<DbElementComparison>> MapsToCombinationOfTargets { get; set; } = [];
        public int DiscreteTargetCount { get; set; } = 0;
    }

    private readonly IDbConnection _db;

    private ILoggerFactory _loggerFactory;
    private ILogger _logger;

    private readonly FhirPackageComparisonPair _packagePair;

    private readonly Dictionary<int, DbElement> _allSourceElements;
    private readonly ILookup<int, DbElement> _allSourceElementsBySdKey;

    private readonly Dictionary<int, DbElement> _allTargetElements;
    private readonly ILookup<int, DbElement> _allTargetElementsBySdKey;

    private readonly List<DbElementComparison> _edComparisons;
    private readonly ILookup<int, DbElementComparison> _edComparsionsBySourceElementKey;
    private readonly ILookup<int, DbElementComparison> _edComparsionsBySourceStructureKey;

    private readonly Dictionary<int, DbElementType> _allSourceElementTypes;
    private readonly ILookup<int, DbElementType> _sourceElementTypesByElementKey;

    private readonly Dictionary<int, DbElementType> _allTargetElementTypes;
    private readonly ILookup<int, DbElementType> _targetElementTypesByElementKey;

    private readonly List<DbElementTypeComparison> _etcComparisons;
    private readonly ILookup<int, DbElementTypeComparison> _etcComparisonsBySourceElementKey;

    private DbRecordCache<DbElementOutcome> _edOutcomeCache;

    public ElementOutcomeGenerator(
        IDbConnection db,
        ILoggerFactory loggerFactory,
        FhirPackageComparisonPair packagePair,
        DbRecordCache<DbElementOutcome> edOutcomeCache)
    {
        _db = db;
        _loggerFactory = loggerFactory;
        _logger = loggerFactory.CreateLogger<ElementOutcomeGenerator>();
        _packagePair = packagePair;
        _edOutcomeCache = edOutcomeCache;

        _allSourceElements = DbElement.SelectDict(_db, FhirPackageKey: _packagePair.SourcePackageKey);
        _allTargetElements = DbElement.SelectDict(_db, FhirPackageKey: _packagePair.TargetPackageKey);

        _edComparisons = DbElementComparison.SelectList(
            _db,
            SourceFhirPackageKey: packagePair.SourcePackageKey,
            TargetFhirPackageKey: packagePair.TargetPackageKey);

        _logger.LogInformation(
            $"Element comparisons: {_edComparisons.Count}, from" +
            $" {_allSourceElements.Count} Elements" +
            $" to {_allTargetElements.Count} Elements");

        _allSourceElementTypes = DbElementType.SelectDict(_db, FhirPackageKey: packagePair.SourcePackageKey);
        _allTargetElementTypes = DbElementType.SelectDict(_db, FhirPackageKey: packagePair.TargetPackageKey);

        _etcComparisons = DbElementTypeComparison.SelectList(
            _db,
            SourceFhirPackageKey: packagePair.SourcePackageKey,
            TargetFhirPackageKey: packagePair.TargetPackageKey);

        _logger.LogInformation(
            $"Element Type comparisons: {_etcComparisons.Count}, from" +
            $" {_allSourceElementTypes.Count} Element Types" +
            $" to {_allTargetElementTypes.Count} Element Types");


        _allSourceElementsBySdKey = _allSourceElements.Values.ToLookup(c => c.StructureKey);
        _allTargetElementsBySdKey = _allTargetElements.Values.ToLookup(c => c.StructureKey);

        _sourceElementTypesByElementKey = _allSourceElementTypes.Values.ToLookup(c => c.ElementKey);
        _targetElementTypesByElementKey = _allTargetElementTypes.Values.ToLookup(c => c.ElementKey);

        //ILookup<int, DbElementComparison> edComparsionsBySdComparisonKey = edComparisons.ToLookup(c => c.StructureComparisonKey);
        _edComparsionsBySourceElementKey = _edComparisons.ToLookup(c => c.SourceContentKey);
        _edComparsionsBySourceStructureKey = _edComparisons.ToLookup(c => c.SourceStructureKey);

        _etcComparisonsBySourceElementKey = _etcComparisons.ToLookup(c => c.SourceElementKey);
    }

    public void ProcessStructure(
        DbStructureDefinition sourceSd,
        Dictionary<int, StructureOutcomeGenerator.StructureOutcomeTrackingRecord> structureTrackingRecords)
    {
        if (sourceSd.ArtifactClass == Common.Models.FhirArtifactClassEnum.PrimitiveType)
        {
            return;
        }

        Dictionary<int, DbElement> sourceElements = _allSourceElementsBySdKey[sourceSd.Key]
            .ToDictionary(c => c.Key);

        //Dictionary<int, Dictionary<int, DbElement>> targetElementsBySdKey = [];

        HashSet<int> fullyMappedElementsAllTargets = [];

        //Dictionary<int, HashSet<int>> fullyMappedElementKeysByComparisonKey = [];
        //HashSet<int> currentlyMappedComparisonKeys = [];
        //fullyMappedElementKeysByComparisonKey[sourceStructureComparison.Key] = currentlyMappedComparisonKeys;

        Dictionary<int, ElementOutcomeTrackingRecord> elementTrackingRecords = [];

        // iterate over the source elements for this structure to determine mapping completeness
        foreach (DbElement sourceEd in sourceElements.Values)
        {
            // get the all comparisons for this element to the target FHIR package
            List<DbElementComparison> elementComparisons = _edComparsionsBySourceElementKey[sourceEd.Key]
                .ToList();

            if (!elementTrackingRecords.TryGetValue(sourceEd.Key, out ElementOutcomeTrackingRecord? elementTrackingRec))
            {
                elementTrackingRec = new()
                {
                    SourceElement = sourceEd,
                    ElementComparisons = elementComparisons,
                    DiscreteTargetCount = elementComparisons
                        .Select(ec => ec.TargetElementKey)
                        .Where(tk => tk is not null)
                        .Distinct()
                        .Count(),
                };
                elementTrackingRecords[sourceEd.Key] = elementTrackingRec;
            }

            // easy check for any single comparison that fully maps
            List<DbElementComparison> fullyMappedComparisons = elementComparisons.Where(ec =>
                (ec.IsIdentical == true) ||
                (ec.Relationship == CMR.Equivalent) ||
                (ec.Relationship == CMR.SourceIsNarrowerThanTarget))
                .ToList();

            if (fullyMappedComparisons.Count != 0)
            {
                fullyMappedElementsAllTargets.Add(sourceEd.Key);

                elementTrackingRec.IsFullyMappedAcrossAllTargets = true;
                elementTrackingRec.MapsToIndividualTargets = fullyMappedComparisons;

                elementTrackingRec.Messages.Add(
                    $"Element `{sourceEd.Id}` is fully mapped to individual targets:" +
                    $" {string.Join(", ", fullyMappedComparisons.Select(fmc => $"`{fmc.TargetElementId}`"))}");

                continue;
            }

            Dictionary<int, DbElement> currentTargetElements = [];
            foreach (DbElementComparison ec in elementComparisons)
            {
                if ((ec.TargetStructureKey is null) ||
                    (ec.TargetContentKey is null))
                {
                    continue;
                }

                DbElement targetEd = _allTargetElements[ec.TargetContentKey!.Value];
                currentTargetElements[targetEd.Key] = targetEd;
            }

            // check the types to see if they map across all targets
            Dictionary<int, DbElementType> sourceEts = _sourceElementTypesByElementKey[sourceEd.Key]
                .ToDictionary(et => et.Key);

            Dictionary<int, DbElementType> unmappedTypes = sourceEts
                .Select(kvp => kvp)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            List<DbElementTypeComparison> etComparisons = _etcComparisonsBySourceElementKey[sourceEd.Key]
                .ToList();

            foreach (DbElementTypeComparison etc in etComparisons)
            {
                if ((etc.IsIdentical == true) ||
                    (etc.Relationship == CMR.Equivalent) ||
                    (etc.Relationship == CMR.SourceIsNarrowerThanTarget))
                {
                    unmappedTypes.Remove(etc.SourceElementTypeKey);
                    if (unmappedTypes.Count == 0)
                    {
                        break;
                    }
                }
            }

            // if we have any unmapped types, then this element is NOT mapped across all targets
            if (unmappedTypes.Count != 0)
            {
                elementTrackingRec.Messages.Add(
                    $"Element `{sourceEd.Id}` does not fully map to targets:" +
                    $" {string.Join(", ", fullyMappedComparisons.Select(fmc => $"`{fmc.TargetElementId}`"))}," +
                    $" because it does not account for source types:" +
                    $" {string.Join(", ", unmappedTypes.Values.Select(et => $"`{et.Literal}`"))}");

                // with unmapped types, there is no full mapping
                continue;
            }

            bool vsMappingOk = true;

            // we need to check bound value set relationships for fully-mapping, but only if the source is required
            if (sourceEd.ValueSetBindingStrength == Hl7.Fhir.Model.BindingStrength.Required)
            {
                // need to check each target's bound value set relationship
                vsMappingOk = false;
 
                foreach (DbElementComparison ec in elementComparisons)
                {
                    if ((ec.TargetStructureKey is null) ||
                        (ec.TargetContentKey is null))
                    {
                        continue;
                    }

                    DbElement targetEd = currentTargetElements[ec.TargetContentKey!.Value];

                    // if the source and target are required, we need to consider the vs relationship
                    if (targetEd.ValueSetBindingStrength == Hl7.Fhir.Model.BindingStrength.Required)
                    {
                        // if the vs relationship is equivalent or source narrower, then we are good
                        if ((ec.BoundValueSetRelationship == CMR.Equivalent) ||
                            (ec.BoundValueSetRelationship == CMR.SourceIsNarrowerThanTarget))
                        {
                            vsMappingOk = true;

                            elementTrackingRec.Messages.Add(
                                $"Element `{sourceEd.Id}` has a required binding to `{sourceEd.BindingValueSet}`" +
                                $" and maps to `{targetEd.Id}` that has a required binding to `{targetEd.BindingValueSet}`" +
                                $" - the value set relationship is {ec.BoundValueSetRelationship}, which is considered fully-mapped.");

                            elementTrackingRec.IsFullyMappedAcrossAllTargets = true;
                            if (elementComparisons.Count == 1)
                            {
                                elementTrackingRec.MapsToIndividualTargets = elementComparisons;
                            }
                            else
                            {
                                elementTrackingRec.MapsToCombinationOfTargets.Add(elementComparisons);
                            }

                            continue;
                        }

                        elementTrackingRec.Messages.Add(
                            $"Element `{sourceEd.Id}` has a required binding to `{sourceEd.BindingValueSet}`" +
                            $" and maps to `{targetEd.Id}` that has a required binding to `{targetEd.BindingValueSet}`" +
                            $" - the value set relationship is {ec.BoundValueSetRelationship}, which is NOT considered fully-mapped.");

                        continue;
                    }
                }
            }
            else if (sourceEd.BindingValueSetKey is not null)
            {
                elementTrackingRec.Messages.Add(
                    $"Element `{sourceEd.Id}` has a {sourceEd.ValueSetBindingStrength} binding to `{sourceEd.BindingValueSet}`," +
                    $" which allows for any value and is not considered for full-mapping.");
            }

            if (!vsMappingOk)
            {
                continue;
            }

            elementTrackingRec.Messages.Add(
                $"Element `{sourceEd.Id}` is considered fully-mapped across: " +
                $" {string.Join(", ", currentTargetElements.Values.Select(te => $"`{te.Id}`"))}");

            elementTrackingRec.IsFullyMappedAcrossAllTargets = true;
            if (elementComparisons.Count == 1)
            {
                elementTrackingRec.MapsToIndividualTargets = elementComparisons;
            }
            else
            {
                elementTrackingRec.MapsToCombinationOfTargets.Add(elementComparisons);
            }
        }

        //// iterate over the comparisons for this source structure
        //foreach ((int targetSdKey, StructureOutcomeGenerator.StructureOutcomeTrackingRecord structureTrackingRecord) in structureTrackingRecords)
        //{
        //    // ignore no-maps for this pass
        //    if (structureTrackingRecord.TargetStructure is null)
        //    {
        //        continue;
        //    }

        //    DbStructureComparison structureComparison = structureTrackingRecord.StructureComparison;
        //    DbStructureDefinition targetSd = structureTrackingRecord.TargetStructure;

        //    Dictionary<int, DbElement> structureTargetElements = _allTargetElementsBySdKey[targetSd.Key]
        //        .ToDictionary(c => c.Key);

        //    // iterate over our source elements for this structure
        //    foreach (ElementOutcomeTrackingRecord edTr in elementTrackingRecords.Values)
        //    {
        //        DbElement sourceEd = edTr.SourceElement;
        //        List<DbElementComparison> elementComparisons = edTr.ElementComparisons;

        //        // easy check for any single comparison that fully maps
        //        List<DbElementComparison> fullyMappedComparisons = elementComparisons.Where(ec =>
        //            (ec.IsIdentical == true) ||
        //            (ec.Relationship == CMR.Equivalent) ||
        //            (ec.Relationship == CMR.SourceIsNarrowerThanTarget))
        //            .ToList();

        //        if (fullyMappedComparisons.Count != 0)
        //        {
        //            fullyMappedElementsAllTargets.Add(sourceEd.Key);
        //            foreach (DbElementComparison fmc in fullyMappedComparisons)
        //            {
        //                if (fmc.StructureComparisonKey != structureComparison.Key)
        //                {
        //                    continue;
        //                }

        //                structureTrackingRecord.Messages.Add(
        //                    $"Element `{sourceEd.Id}` is fully mapped to target element `{structureTargetElements[fmc.TargetContentKey!.Value].Id}`");

        //                break;
        //            }

        //            continue;
        //        }

        //        Dictionary<int, DbElement> currentTargetElements = [];
        //        foreach (DbElementComparison ec in elementComparisons)
        //        {
        //            if (ec.TargetStructureKey is null)
        //            {
        //                continue;
        //            }

        //            DbElement targetEd = structureTargetElements[ec.TargetContentKey!.Value];
        //            currentTargetElements[targetEd.Key] = targetEd;
        //        }

        //        // check the types to see if they map across all targets
        //        Dictionary<int, DbElementType> sourceEts = _sourceElementTypesByElementKey[sourceEd.Key]
        //            .ToDictionary(et => et.Key);

        //        Dictionary<int, DbElementType> unmappedTypes = sourceEts
        //            .Select(kvp => kvp)
        //            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        //        List<DbElementTypeComparison> etComparisons = _etcComparisonsBySourceElementKey[sourceEd.Key]
        //            .ToList();

        //        foreach (DbElementTypeComparison etc in etComparisons)
        //        {
        //            if ((etc.IsIdentical == true) ||
        //                (etc.Relationship == CMR.Equivalent) ||
        //                (etc.Relationship == CMR.SourceIsNarrowerThanTarget))
        //            {
        //                unmappedTypes.Remove(etc.SourceElementTypeKey);
        //                if (unmappedTypes.Count == 0)
        //                {
        //                    break;
        //                }
        //            }
        //        }

        //        // if we have no unmapped types, then this element is fully mapped across all targets
        //        if (unmappedTypes.Count != 0)
        //        {
        //            string expandedMissingTypes = string.Join(
        //                ", ",
        //                unmappedTypes.Values.Select(et => $"`{et.Literal}`"));
        //            structureTrackingRecord.Messages.Add(
        //                $"Element `{sourceEd.Id}` mappings do not account for the following types: {expandedMissingTypes}");

        //            // with unmapped types, there is no full mapping
        //            continue;
        //        }

        //        // we only care about the bound value set if the strength is required
        //        if (sourceEd.BindingValueSetKey is null)
        //        {
        //            bool isRequired = sourceEd.ValueSetBindingStrength == Hl7.Fhir.Model.BindingStrength.Required;

        //            if (!isRequired)
        //            {
        //                isRequired = currentTargetElements.Values.Any(ted => ted.ValueSetBindingStrength == Hl7.Fhir.Model.BindingStrength.Required);
        //            }

        //            if (isRequired)
        //            {
        //                List<DbElementComparison> completeVsComparisons = elementComparisons.Where(ec =>
        //                    (ec.BoundValueSetRelationship == CMR.Equivalent) ||
        //                    (ec.BoundValueSetRelationship == CMR.SourceIsNarrowerThanTarget))
        //                    .ToList();

        //                if (completeVsComparisons.Count == 0)
        //                {
        //                    structureTrackingRecord.Messages.Add(
        //                        $"Element `{sourceEd.Id}` mappings do not fully map the bound value set: `{sourceEd.BindingValueSet}`");

        //                    // with unmapped types, there is no full mapping
        //                    continue;
        //                }
        //            }
        //        }

        //        fullyMappedElementsAllTargets.Add(sourceEd.Key);
        //        structureTrackingRecord.Messages.Add(
        //            $"Element `{sourceEd.Id}` is fully mapped to across all target elements");
        //    }

        //    structureTrackingRecord.NumberFullyMappedToAllTargets = fullyMappedElementsAllTargets.Count;
        //}

        Dictionary<(int sourceElementKey, int? targetStructureKey), (int outcomeKey, string id)> edKeyOutcomeLookup = [];

        // iterate over our element tracking records to create outcomes
        foreach (ElementOutcomeTrackingRecord edTr in elementTrackingRecords.Values.OrderBy(etr => etr.SourceElement.ResourceFieldOrder))
        {
            DbElement sourceEd = edTr.SourceElement;
            List<DbElementComparison> elementComparisons = edTr.ElementComparisons;

            (string idLong, string idShort) = XVerProcessor.GenerateExtensionId(
                _packagePair.SourcePackageShortName,
                sourceEd.Id);

            // iterate over the structure tracking records to create outcomes
            foreach (StructureOutcomeGenerator.StructureOutcomeTrackingRecord sdTr in structureTrackingRecords.Values)
            {
                sdTr.IsFullyMappedAcrossAllTargets = sdTr.IsFullyMappedAcrossAllTargets &&
                    edTr.IsFullyMappedAcrossAllTargets;

                if (sdTr.TargetStructure is null)
                {
                    DbElementComparison? noMapElementComparison = elementComparisons
                        .FirstOrDefault(ec => ec.TargetStructureKey is null);
                        //?? throw new Exception($"Non-mapped {sdTr.SourceStructure.Name} element {sourceEd.Id} has no non-mapped comparison!");

                    // create the no-map element outcome
                    DbElementOutcome noMapEdOutcome = new()
                    {
                        Key = DbElementOutcome.GetIndex(),
                        StructureOutcomeKey = sdTr.StructureOutcomeKey,

                        SourceFhirPackageKey = _packagePair.SourcePackageKey,
                        SourceFhirSequence = _packagePair.SourceFhirSequence,
                        SourceStructureKey = sourceSd.Key,
                        SourceElementKey = sourceEd.Key,
                        TotalSourceCount = 1,

                        TargetFhirPackageKey = _packagePair.TargetPackageKey,
                        TargetFhirSequence = _packagePair.TargetFhirSequence,
                        TargetStructureKey = null,
                        TargetElementKey = null,
                        TotalTargetCount = edTr.DiscreteTargetCount,

                        ElementComparisonKey = noMapElementComparison?.Key ?? -1,

                        RequiresXVerDefinition = !edTr.IsFullyMappedAcrossAllTargets,
                        PartOfElementOutcomeKey = null,

                        IsRenamed = false,
                        IsUnmapped = false,
                        IsIdentical = false,
                        IsEquivalent = false,
                        IsBroaderThanTarget = false,
                        IsNarrowerThanTarget = false,

                        FullyMapsToThisTarget = false,
                        FullyMapsAcrossAllTargets = edTr.IsFullyMappedAcrossAllTargets,

                        Comments = edTr.Messages.Count > 0 ? string.Join('\n', edTr.Messages) : "TODO",

                        SourceCanonicalUnversioned = sourceSd.UnversionedUrl,
                        SourceCanonicalVersioned = sourceSd.VersionedUrl,
                        SourceVersion = sourceSd.Version,
                        SourceId = sourceEd.Id,
                        SourceName = sourceEd.Name,
                        TargetCanonicalUnversioned = null,
                        TargetCanonicalVersioned = null,
                        TargetVersion = null,
                        TargetId = null,
                        TargetName = null,
                        PotentialGenLongId = idLong,
                        //PotentialGenShortId = idShort,
                        //PotentialGenUrl = extUrl,
                    };

                    _edOutcomeCache.CacheAdd(noMapEdOutcome);
                    edTr.ElementOutcomes.Add(noMapEdOutcome);
                    sdTr.ElementOutcomes.Add(noMapEdOutcome);
                    edKeyOutcomeLookup.Add((sourceEd.Key, null), (noMapEdOutcome.Key, idLong));
                    continue;
                }

                // skip primitive targets
                if (sdTr.TargetStructure.ArtifactClass == Common.Models.FhirArtifactClassEnum.PrimitiveType)
                {
                    continue;
                }

                // traverse the comparisons for this element (can map to multiple elments in target structure)
                foreach (DbElementComparison elementComparison in elementComparisons.Where(ec => ec.TargetStructureKey == sdTr.TargetStructure.Key))
                {
                    DbElement? targetEd = elementComparison.TargetElementKey is null
                        ? null
                        : _allTargetElements[elementComparison.TargetElementKey.Value];

                    int? partOfXVerOutcomeKey = null;
                    string? partOfXVerOutcomeId = null;

                    if (!edTr.IsFullyMappedAcrossAllTargets)
                    {
                        sdTr.IsFullyMappedAcrossAllTargets = false;
                        if (!edTr.MapsToIndividualTargets.Any(ec => ec.TargetElementKey == elementComparison.TargetElementKey))
                        {
                            sdTr.IsFullyMappedToThisTarget = false;
                        }
                    }

                    bool elementRequiresXVer = !edTr.IsFullyMappedAcrossAllTargets;
                    if (elementRequiresXVer &&
                        (sourceEd.ParentElementKey is not null) &&
                        edKeyOutcomeLookup.TryGetValue((sourceEd.ParentElementKey!.Value, sdTr.TargetStructure.Key), out (int outcomeKey, string outcomeGenId) po))
                    {
                        partOfXVerOutcomeKey = po.outcomeKey;
                        partOfXVerOutcomeId = po.outcomeGenId;
                    }

                    // create the mapped element outcome
                    DbElementOutcome elementOutcome = new()
                    {
                        Key = DbElementOutcome.GetIndex(),
                        StructureOutcomeKey = sdTr.StructureOutcomeKey,
                        ElementComparisonKey = elementComparison.Key,

                        SourceFhirPackageKey = _packagePair.SourcePackageKey,
                        SourceFhirSequence = _packagePair.SourceFhirSequence,
                        SourceStructureKey = sourceSd.Key,
                        SourceElementKey = sourceEd.Key,
                        TotalSourceCount = -1,

                        TargetFhirPackageKey = _packagePair.TargetPackageKey,
                        TargetFhirSequence = _packagePair.TargetFhirSequence,
                        TargetStructureKey = sdTr.TargetStructure.Key,
                        TargetElementKey = elementComparison.TargetElementKey,
                        TotalTargetCount = edTr.DiscreteTargetCount,

                        RequiresXVerDefinition = elementRequiresXVer,
                        PartOfElementOutcomeKey = partOfXVerOutcomeKey,

                        IsRenamed = targetEd is null ? false : (sourceEd.Name != targetEd.Name),
                        IsUnmapped = targetEd is null || elementComparison.NotMapped,
                        IsIdentical = elementComparison.IsIdentical == true,
                        IsEquivalent = elementComparison.Relationship == CMR.Equivalent,
                        IsBroaderThanTarget = elementComparison.Relationship == CMR.SourceIsBroaderThanTarget,
                        IsNarrowerThanTarget = elementComparison.Relationship == CMR.SourceIsNarrowerThanTarget,

                        FullyMapsToThisTarget = edTr.IsFullyMappedAcrossAllTargets &&
                            (targetEd is not null) &&
                            edTr.MapsToIndividualTargets.Any(ec => ec.TargetElementKey == targetEd?.Key),
                        FullyMapsAcrossAllTargets = edTr.IsFullyMappedAcrossAllTargets,

                        Comments = edTr.Messages.Count > 0
                            ? string.Join('\n', edTr.Messages)
                            : elementComparison.UserMessage ?? elementComparison.TechnicalMessage ?? "TODO",

                        SourceCanonicalUnversioned = sourceSd.UnversionedUrl,
                        SourceCanonicalVersioned = sourceSd.VersionedUrl,
                        SourceVersion = sourceSd.Version,
                        SourceId = sourceEd.Id,
                        SourceName = sourceEd.Name,
                        TargetCanonicalUnversioned = null,
                        TargetCanonicalVersioned = null,
                        TargetVersion = null,
                        TargetId = null,
                        TargetName = null,
                        PotentialGenLongId = partOfXVerOutcomeId ?? idLong,
                        //PotentialGenShortId = extIdShort,
                        //PotentialGenUrl = extUrl,
                    };

                    _edOutcomeCache.CacheAdd(elementOutcome);
                    edTr.ElementOutcomes.Add(elementOutcome);
                    sdTr.ElementOutcomes.Add(elementOutcome);
                    if (elementRequiresXVer && !edKeyOutcomeLookup.ContainsKey((sourceEd.Key, sdTr.TargetStructure.Key)))
                    {
                        edKeyOutcomeLookup.Add(
                            (sourceEd.Key, sdTr.TargetStructure.Key),
                            (partOfXVerOutcomeKey ?? elementOutcome.Key, elementOutcome.PotentialGenLongId));
                    }
                }
            }
        }
    }
}
