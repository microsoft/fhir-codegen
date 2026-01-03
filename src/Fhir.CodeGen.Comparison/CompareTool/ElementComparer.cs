using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fhir.CodeGen.Common.Models;
using Fhir.CodeGen.Comparison.Extensions;
using Fhir.CodeGen.Comparison.Models;
using Fhir.CodeGen.Comparison.XVer;
using Hl7.Fhir.Model;
using Microsoft.Extensions.Logging;
using static Fhir.CodeGen.Comparison.CompareTool.StructureComparer;
using CMR = Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship;

namespace Fhir.CodeGen.Comparison.CompareTool;

public class ElementComparer
{
    private class ElementTrackingRecord
    {
        public required DbElement SourceElement { get; init; }
        public int? CurrentElementKey { get; set; }
        public required int?[] ContentKeys { get; set; }
        public CMR? Relationship { get; set; }
        public CMR? ConceptDomainRelationship { get; set; }
        public CMR? ValueDomainRelationship { get; set; }
        public bool IsIdentical { get; set; }
        public bool RelativePathsAreIdentical { get; set; }
    }

    private class ElementTypeTrackingRecord
    {
        public required DbElementType SourceType { get; init; }
        public required DbElementType? TargetType { get; set; }

        public required CMR? Relationship { get; set; }
        public required CMR? ConceptDomainRelationship { get; set; }
        public required CMR? ValueDomainRelationship { get; set; }

        public required CMR? TargetProfileRelationship { get; set; }
        public required string? TargetProfileMessage { get; set; }

        public required CMR? TypeProfileRelationship { get; set; }
        public required string? TypeProfileMessage { get; set; }

        public required string? UserMessage { get; set; }
        public required string? TechnicalMessage { get; set; }
    }

    private readonly IDbConnection _db;

    private ILoggerFactory _loggerFactory;
    private ILogger _logger;

    private DbComparisonCache<DbElementComparison> _elementComparisonCache;

    private List<DbFhirPackage> _packages = [];

    public ElementComparer(
        IDbConnection db,
        ILoggerFactory loggerFactory,
        DbComparisonCache<DbElementComparison> elementComparisonCache,
        List<DbFhirPackage> packages)
    {
        _loggerFactory = loggerFactory;
        _logger = loggerFactory.CreateLogger<FhirDbComparer>();
        _db = db;
        _elementComparisonCache = elementComparisonCache;
        _packages = packages;
    }

    internal List<DbElementComparison> DoTransitiveElementComparisons(StructureComparisonTrackingRecord trackingRecord)
    {
        if (trackingRecord.ComparisonSteps.Count == 0)
        {
            throw new Exception("Cannot build transitive comparisons without comparison steps!");
        }

        List<DbElementComparison> elementComparisons = [];

        int sourceIndex = trackingRecord.SourcePackage.PackageArrayIndex;
        int targetIndex = trackingRecord.TargetPackage.PackageArrayIndex;
        int increment = sourceIndex < targetIndex ? 1 : -1;

        // get element comparisons for each step, keyed by StructureComparisonKey
        Dictionary<int, List<DbElementComparison>> stepElementComparisons = [];
        foreach (DbStructureComparison sdCompStep in trackingRecord.ComparisonSteps)
        {
            List<DbElementComparison> currentStepComparisons = DbElementComparison.SelectList(
                _db,
                StructureComparisonKey: sdCompStep.Key);
            stepElementComparisons[sdCompStep.Key] = currentStepComparisons;
        }

        // get the initial source elements
        List<DbElement> sourceElements = DbElement.SelectList(
            _db,
            StructureKey: trackingRecord.SourceStructure.Key);

        // get the target elements (if we have a target)
        Dictionary<int, DbElement> targetElements = trackingRecord.TargetStructure is null
            ? []
            : DbElement.SelectDict(_db, StructureKey: trackingRecord.TargetStructure.Key);

        // build a lookup from source element key to the first step's element comparisons
        ILookup<int, DbElementComparison> firstStepBySourceElement = stepElementComparisons[trackingRecord.ComparisonSteps[0].Key]
            .ToLookup(ec => ec.SourceElementKey);

        // iterate over each source element and follow it through the transitive steps
        foreach (DbElement sourceElement in sourceElements)
        {
            // get the initial element comparisons for this source element
            List<DbElementComparison> initialComparisons = firstStepBySourceElement[sourceElement.Key].ToList();

            // if there are no initial comparisons, this is a no-map
            if (initialComparisons.Count == 0)
            {
                int?[] contentKeys = new int?[6];
                contentKeys[sourceIndex] = sourceElement.Key;

                DbElementComparison noMapComparison = createElementComparison(
                    trackingRecord,
                    sourceElement,
                    targetElement: null,
                    elementComparisonKey: DbElementComparison.GetIndex(),
                    relationship: null,
                    cdRelationship: null,
                    vdRelationship: null,
                    technicalMessage: null,
                    userMessage: null,
                    contentStepKeys: contentKeys,
                    boundVsComparison: null,
                    collatedTypeComparison: null);

                _elementComparisonCache.CacheAdd(noMapComparison);
                elementComparisons.Add(noMapComparison);
                continue;
            }

            // follow the element through each step
            List<ElementTrackingRecord> currentPaths = initialComparisons
                .Select(ec => new ElementTrackingRecord
                {
                    SourceElement = sourceElement,
                    CurrentElementKey = ec.TargetElementKey,
                    ContentKeys = getKeyArray(sourceIndex, sourceElement.Key, sourceIndex + increment, ec.TargetElementKey),
                    Relationship = ec.Relationship,
                    IsIdentical = ec.IsIdentical == true,
                    RelativePathsAreIdentical = ec.RelativePathsAreIdentical == true,
                })
                .ToList();

            // process subsequent steps
            for (int step = 1; step < trackingRecord.ComparisonSteps.Count; step++)
            {
                DbStructureComparison stepComparison = trackingRecord.ComparisonSteps[step];
                List<DbElementComparison> stepElements = stepElementComparisons[stepComparison.Key];
                ILookup<int, DbElementComparison> stepBySourceElement = stepElements.ToLookup(ec => ec.SourceElementKey);

                int stepTargetIndex = sourceIndex + (increment * (step + 1));

                List<ElementTrackingRecord> nextPaths = [];

                foreach (ElementTrackingRecord path in currentPaths)
                {
                    // if current element is null, continue with null
                    if (path.CurrentElementKey is null)
                    {
                        path.ContentKeys[stepTargetIndex] = null;
                        nextPaths.Add(path);
                        continue;
                    }

                    // get the next comparisons for this element
                    List<DbElementComparison> nextComparisons = stepBySourceElement[path.CurrentElementKey.Value].ToList();

                    if (nextComparisons.Count == 0)
                    {
                        // no mapping found - treat as no-map from this point
                        path.CurrentElementKey = null;
                        path.Relationship = FhirDbComparer.ApplyRelationship(path.Relationship, null);
                        path.IsIdentical = false;
                        path.RelativePathsAreIdentical = false;
                        nextPaths.Add(path);
                        continue;
                    }

                    // expand paths for each next comparison
                    foreach (DbElementComparison nextComp in nextComparisons)
                    {
                        int?[] newContentKeys = path.ContentKeys.ToArray();
                        newContentKeys[stepTargetIndex] = nextComp.TargetElementKey;

                        nextPaths.Add(new ElementTrackingRecord
                        {
                            SourceElement = sourceElement,
                            CurrentElementKey = nextComp.TargetElementKey,
                            ContentKeys = newContentKeys,
                            Relationship = FhirDbComparer.ApplyRelationship(path.Relationship, nextComp.Relationship),
                            ConceptDomainRelationship = FhirDbComparer.ApplyRelationship(path.ConceptDomainRelationship, nextComp.ConceptDomainRelationship),
                            ValueDomainRelationship = FhirDbComparer.ApplyRelationship(path.ValueDomainRelationship, nextComp.ValueDomainRelationship),
                            IsIdentical = path.IsIdentical && (nextComp.IsIdentical == true),
                            RelativePathsAreIdentical = path.RelativePathsAreIdentical && (nextComp.RelativePathsAreIdentical == true),
                        });
                    }
                }

                currentPaths = nextPaths;
            }

            // create the final element comparison records for each completed path
            foreach (ElementTrackingRecord path in currentPaths)
            {
                DbElement? targetElement = null;

                if (path.CurrentElementKey is not null)
                {
                    targetElements.TryGetValue(path.CurrentElementKey.Value, out targetElement);
                }

                DbValueSetComparison? boundVsComparison = null;
                if ((sourceElement.BindingValueSetKey is not null) &&
                    (targetElement?.BindingValueSetKey is not null))
                {
                    boundVsComparison = DbValueSetComparison.SelectSingle(
                           _db,
                           SourceValueSetKey: sourceElement.BindingValueSetKey,
                           TargetValueSetKey: targetElement.BindingValueSetKey);
                }

                int elementComparisonKey = DbElementComparison.GetIndex();

                DbCollatedTypeComparison? etComparison = null;
                if (targetElement is not null)
                {
                    etComparison = doCollatedTypeComparison(
                        trackingRecord.SourcePackage,
                        trackingRecord.TargetPackage,
                        sourceElement,
                        targetElement,
                        elementComparisonKey);
                }

                // build the element comparison db record
                DbElementComparison elementComparison = createElementComparison(
                    trackingRecord,
                    sourceElement,
                    targetElement,
                    elementComparisonKey,
                    path.Relationship,
                    path.ConceptDomainRelationship,
                    path.ValueDomainRelationship,
                    boundVsComparison: boundVsComparison,
                    collatedTypeComparison: etComparison,
                    technicalMessage: null,
                    userMessage: null,
                    contentStepKeys: path.ContentKeys);

                _elementComparisonCache.CacheAdd(elementComparison);
                elementComparisons.Add(elementComparison);
            }
        }

        return elementComparisons;
    }

    internal List<DbElementComparison> DoElementComparisons(
        StructureComparisonTrackingRecord trackingRecord)
    {
        List<DbElementComparison> elementComparisons = [];

        DbStructureDefinition sourceSd = trackingRecord.SourceStructure;
        DbStructureDefinition? targetSd = trackingRecord.TargetStructure;

        // get the source elements
        List<DbElement> sourceElements = DbElement.SelectList(
            _db,
            StructureKey: trackingRecord.SourceStructure.Key);

        // if there is no target structure, every element is a no map
        if (targetSd is null)
        {
            // create our element comparisons
            foreach (DbElement sourceElement in sourceElements)
            {
                // build the element comparison db record
                DbElementComparison elementComparison = createElementComparison(
                    trackingRecord,
                    sourceElement,
                    targetElement: null,
                    elementComparisonKey: DbElementComparison.GetIndex(),
                    relationship: null,
                    cdRelationship: null,
                    vdRelationship: null,
                    technicalMessage: null,
                    userMessage: null,
                    contentStepKeys: getKeyArray(trackingRecord.SourcePackage, sourceElement.Key),
                    boundVsComparison: null,
                    collatedTypeComparison: null);

                _elementComparisonCache.CacheAdd(elementComparison);
                elementComparisons.Add(elementComparison);
            }

            return elementComparisons;
        }


        // get the target elements
        Dictionary<int, DbElement> targetElements = DbElement.SelectDict(
            _db,
            StructureKey: targetSd.Key);

        // get any explicit element mappings
        List<DbElementMapping> elementMappings = trackingRecord.ExplicitMapping is null
            ? []
            : DbElementMapping.SelectList(_db, StructureMappingKey: trackingRecord.ExplicitMapping.Key);

        // create lookups
        ILookup<string, DbElement> targetElementsByPath = targetElements.Values.ToLookup(e => e.Path);
        ILookup<int, DbElementMapping> elementMappingsBySourceKey = elementMappings
            .Where(m => m.SourceElementKey is not null)
            .ToLookup(m => m.SourceElementKey!.Value);
        ILookup<string, DbElementMapping> elementMappingsBySourceId = elementMappings.ToLookup(m => m.SourceElementId);

        // iterate over each source element
        foreach (DbElement sourceElement in sourceElements)
        {
            CMR? elementRelationship = null;
            CMR? elementConceptRelationship = null;
            CMR? elementValueRelationship = null;
            string? technicalMessage = null;
            string? userMessage = null;

            // check for an explicit mapping first
            DbElementMapping? mapping = elementMappingsBySourceKey[sourceElement.Key].FirstOrDefault();
            if (mapping is not null)
            {
                int mappedElementComparisonKey = DbElementComparison.GetIndex();

                DbElement? targetElement = null;

                if (mapping.TargetElementKey is not null)
                {
                    targetElements.TryGetValue(mapping.TargetElementKey.Value, out targetElement);
                }

                // use the explicit relationship
                elementRelationship = mapping.Relationship;
                elementConceptRelationship = mapping.ConceptDomainRelationship;
                elementValueRelationship = mapping.ValueDomainRelationship;

                technicalMessage = $"Using explicit mapping" +
                    $" from `{sourceSd.VersionedUrl}`" +
                    $" to `{targetSd.VersionedUrl}`" +
                    $" in `{trackingRecord.ExplicitMappingSource?.Url}` (`{trackingRecord.ExplicitMappingSource?.Filename}`)";

                DbValueSetComparison? boundValueSetComparsion = null;
                if ((sourceElement.BindingValueSetKey is not null) &&
                    (targetElement?.BindingValueSetKey is not null))
                {
                    boundValueSetComparsion = DbValueSetComparison.SelectSingle(
                           _db,
                           SourceValueSetKey: sourceElement.BindingValueSetKey,
                           TargetValueSetKey: targetElement.BindingValueSetKey);
                }

                // still need to build the type comparisons, even if we are mapped
                DbCollatedTypeComparison? mappedTypeComparison = null;
                if (targetElement is not null)
                {
                    mappedTypeComparison = doCollatedTypeComparison(
                        trackingRecord.SourcePackage,
                        trackingRecord.TargetPackage,
                        sourceElement,
                        targetElement,
                        mappedElementComparisonKey);
                }

                // build the element comparison db record
                DbElementComparison elementComparison = createElementComparison(
                    trackingRecord,
                    sourceElement,
                    targetElement,
                    mappedElementComparisonKey,
                    elementRelationship,
                    elementConceptRelationship,
                    elementValueRelationship,
                    boundVsComparison: boundValueSetComparsion,
                    collatedTypeComparison: mappedTypeComparison,
                    technicalMessage,
                    mapping.Comments,
                    contentStepKeys: getKeyArray(
                        trackingRecord.SourcePackage,
                        sourceElement.Key,
                        trackingRecord.TargetPackage,
                        targetElement?.Key));

                _elementComparisonCache.CacheAdd(elementComparison);
                elementComparisons.Add(elementComparison);

                continue;
            }

            // no explicit mapping, try to find by path
            List<DbElement> possibleTargets = targetElementsByPath[sourceElement.Path].ToList();

            if (possibleTargets.Count > 1)
            {
                technicalMessage = $"Multiple target elements with path {sourceElement.Path} found in target structure {trackingRecord.TargetStructure!.Name}";
                userMessage = $"Multiple elements with path {sourceElement.Path} found in target structure.";
            }

            // if no targets found by path, this is a no-map
            if (possibleTargets.Count == 0)
            {
                DbElementComparison noMapComparison = createElementComparison(
                    trackingRecord,
                    sourceElement,
                    targetElement: null,
                    elementComparisonKey: DbElementComparison.GetIndex(),
                    relationship: null,
                    cdRelationship: null,
                    vdRelationship: null,
                    technicalMessage: null,
                    userMessage: null,
                    contentStepKeys: getKeyArray(
                        trackingRecord.SourcePackage,
                        sourceElement.Key,
                        trackingRecord.TargetPackage,
                        null),
                    boundVsComparison: null,
                    collatedTypeComparison: null);
                _elementComparisonCache.CacheAdd(noMapComparison);
                elementComparisons.Add(noMapComparison);
                continue;
            }

            // iterate over the possible targets
            foreach (DbElement targetElement in possibleTargets)
            {
                int elementComparisonKey = DbElementComparison.GetIndex();

                DbValueSetComparison? boundVsComparison = null;
                if ((sourceElement.BindingValueSetKey is not null) &&
                    (targetElement.BindingValueSetKey is not null))
                {
                    boundVsComparison = DbValueSetComparison.SelectSingle(
                           _db,
                           SourceValueSetKey: sourceElement.BindingValueSetKey,
                           TargetValueSetKey: targetElement.BindingValueSetKey);
                }

                bool vsIsEquivalent = (boundVsComparison is null) || (boundVsComparison.Relationship == CMR.Equivalent);

                DbCollatedTypeComparison etComparison = doCollatedTypeComparison(
                        trackingRecord.SourcePackage,
                        trackingRecord.TargetPackage,
                        sourceElement,
                        targetElement,
                        elementComparisonKey);

                bool elementIsIdentical = (possibleTargets.Count == 1) &&
                    (sourceElement.Id == targetElement.Id) &&
                    (etComparison.Relationship == CMR.Equivalent) &&
                    vsIsEquivalent;
                bool elementIsEquivalent = elementIsIdentical ||
                    ((possibleTargets.Count == 1) && (etComparison.Relationship == CMR.Equivalent) && vsIsEquivalent);
                bool elementIsBroaderThanTarget = (possibleTargets.Count > 1) || (etComparison.Relationship == CMR.SourceIsBroaderThanTarget);

                if (elementIsIdentical)
                {
                    elementRelationship = CMR.Equivalent;
                }
                else if (elementIsEquivalent)
                {
                    elementRelationship = CMR.Equivalent;
                }
                else if (elementIsBroaderThanTarget)
                {
                    elementRelationship = CMR.SourceIsBroaderThanTarget;
                }

                // build the element comparison db record
                DbElementComparison elementComparison = createElementComparison(
                    trackingRecord,
                    sourceElement,
                    targetElement,
                    elementComparisonKey,
                    elementRelationship,
                    elementConceptRelationship,
                    elementValueRelationship,
                    boundVsComparison: boundVsComparison,
                    collatedTypeComparison: etComparison,
                    technicalMessage,
                    userMessage,
                    contentStepKeys: getKeyArray(
                        trackingRecord.SourcePackage,
                        sourceElement.Key,
                        trackingRecord.TargetPackage,
                        targetElement.Key));
                _elementComparisonCache.CacheAdd(elementComparison);
                elementComparisons.Add(elementComparison);
            }
        }

        return elementComparisons;
    }

    private DbCollatedTypeComparison doCollatedTypeComparison(
        DbFhirPackage sourcePackage,
        DbFhirPackage targetPackage,
        DbElement sourceElement,
        DbElement targetElement,
        int elementComparisonKey)
    {
        DbCollatedTypeComparison? existing = DbCollatedTypeComparison.SelectSingle(
                        _db,
                        SourceElementKey: sourceElement.Key,
                        TargetElementKey: targetElement.Key);
        if (existing is not null)
        {
            return existing;
        }

        DbCollatedType? sourceCollated = DbCollatedType.SelectSingle(
            _db,
            ElementKey: sourceElement.Key);
        if (sourceCollated is null)
        {
            throw new Exception($"Source element {sourceElement.Id} ({sourceElement.Key}) has no collated type!");
        }

        DbCollatedType? targetCollated = DbCollatedType.SelectSingle(
            _db,
            ElementKey: targetElement.Key);
        if (targetCollated is null)
        {
            throw new Exception($"Target element {targetElement.Id} ({targetElement.Key}) has no collated type!");
        }

        // get the source types
        List<DbElementType> sourceTypeList = DbElementType.SelectList(_db, ElementKey: sourceElement.Key);
        ILookup<string?, DbElementType> sourceTypesByName = sourceTypeList.ToLookup(et => et.TypeName);

        // get the target types
        List<DbElementType> targetTypeList = DbElementType.SelectList(_db, ElementKey: targetElement.Key);
        ILookup<string?, DbElementType> targetTypesByName = targetTypeList.ToLookup(et => et.TypeName);



        // start by assuming everything is a no-map
        Dictionary<DbElementType, DbElementType?> sourceTargetTypeMappings = sourceTypeList.ToDictionary(et => et, et => (DbElementType?)null);

        // iterate over the source types, looking for matching target types
        foreach (DbElementType sourceType in sourceTypeList)
        {
            // check to see if there is a literal match for this type
            if (targetTypesByName.Contains(sourceType.TypeName))
            {
                // check to see if any of our matched types have the same type and target profiles
                DbElementType? literalMatchType = targetTypesByName[sourceType.TypeName].FirstOrDefault(sourceType.IsEquivalent);
                if (literalMatchType != null)
                {
                    // override any existing mappings, since this is an exact match
                    sourceTargetTypeMappings[sourceType] = literalMatchType;
                }
                continue;
            }

            // check to see if this is a quantity type and look for quantity matches
            if (sourceType.IsQuantityType())
            {
                bool foundQuantityMatch = false;
                string sourceNormalizedTypeName = sourceType.GetNormalizedName();
                List<DbElementType> targetQuantityTypes = targetTypeList.Where(et => et.IsQuantityType()).ToList();

                // iterate over any matches
                foreach (DbElementType targetType in targetQuantityTypes)
                {
                    string targetNormalizedTypeName = targetType.GetNormalizedName();

                    // if these are the same type, it is the best match
                    if (sourceNormalizedTypeName == targetNormalizedTypeName)
                    {
                        sourceTargetTypeMappings[sourceType] = targetType;
                        foundQuantityMatch = true;
                        break;
                    }

                    // if either side is "Quantity", we can use it, but do not override an exact match and keep looking
                    if ((sourceTargetTypeMappings[sourceType] == null) &&
                        ((sourceNormalizedTypeName == "Quantity") || (targetNormalizedTypeName == "Quantity")))
                    {
                        sourceTargetTypeMappings[sourceType] = targetType;
                        foundQuantityMatch = true;
                        continue;
                    }
                }

                // if we found a quantity match, stop looking for type matches
                if (foundQuantityMatch)
                {
                    continue;
                }
            }

            // look for a compatible mapping among types, based on structure comparisons in the database
            foreach (DbElementType targetType in targetTypeList)
            {
                // we can only test types when both sides have resolved structures
                if ((sourceType.TypeStructureKey == null) ||
                    (targetType.TypeStructureKey == null))
                {
                    continue;
                }

                int matchCount = DbStructureComparison.SelectCount(
                    _db,
                    SourceFhirPackageKey: sourceElement.FhirPackageKey,
                    SourceStructureKey: sourceType.TypeStructureKey,
                    TargetFhirPackageKey: targetElement.FhirPackageKey,
                    TargetStructureKey: targetType.TypeStructureKey);

                if (matchCount == 0)
                {
                    matchCount = DbStructureMapping.SelectCount(
                        _db,
                        SourceFhirPackageKey: sourceElement.FhirPackageKey,
                        SourceStructureKey: sourceType.TypeStructureKey,
                        TargetFhirPackageKey: targetElement.FhirPackageKey,
                        TargetStructureKey: targetType.TypeStructureKey);
                }

                // if we did not find a target, keep looking
                if (matchCount == 0)
                {
                    continue;
                }

                // if we have matching profiles (e.g., moving from reference to canonical of same type), always use it
                if (sourceType.HaveEquivalentProfiles(targetType))
                {
                    sourceTargetTypeMappings[sourceType] = targetType;
                    break;
                }
                else if (sourceTargetTypeMappings[sourceType] == null)
                {
                    sourceTargetTypeMappings[sourceType] = targetType;
                    continue;
                }
            }
        }

        // process our granular type pairs - compare or add as no-map
        List<ElementTypeTrackingRecord> individualTypeComparisons = [];

        foreach ((DbElementType sourceType, DbElementType? targetType) in sourceTargetTypeMappings)
        {
            // calculate relationships for this type pair
            ElementTypeTrackingRecord etr = calculateTypeRelationships(
                sourcePackage,
                targetPackage,
                sourceType,
                targetType);

            individualTypeComparisons.Add(etr);
        }



        // create collated type comparison from individual comparisons
        DbCollatedTypeComparison collatedComparison = createCollatedTypeComparison(
            sourcePackage,
            sourceElement,
            sourceCollated,
            targetPackage,
            targetElement,
            targetCollated,
            individualTypeComparisons);

        // update individual comparisons with collated comparison key
        foreach (DbElementTypeComparison typeComparison in individualTypeComparisons)
        {
            typeComparison.CollatedTypeComparisonKey = collatedComparison.Key;
        }

        // check for an inverse collated comparison
        if ((elementComparison.InverseComparisonKey != null) &&
            (elementComparison.InverseComparisonKey != -1))
        {
            DbCollatedTypeComparison? collatedInverse = DbCollatedTypeComparison.SelectSingle(
                _db,
                PackageComparisonKey: packageReversePair.Key,
                ElementComparisonKey: elementComparison.InverseComparisonKey);

            if (collatedInverse != null)
            {
                collatedComparison.InverseComparisonKey = collatedInverse.Key;

                collatedInverse.InverseComparisonKey = collatedComparison.Key;
                collatedTypeComparisonCache.Changed(collatedInverse);
            }
        }

        // add to caches
        collatedTypeComparisonCache.CacheAdd(collatedComparison);
        foreach (DbElementTypeComparison typeComparison in individualTypeComparisons)
        {
            typeComparisonCache.CacheAdd(typeComparison);
        }

        //return (collatedComparison, individualTypeComparisons);



        // create new comparison
        DbCollatedTypeComparison newComparison = new()
        {
            SourceFhirPackageKey = sourceElement.FhirPackageKey,
            SourceElementKey = sourceElement.Key,
            SourceCollatedTypeKey = sourceCollated.Key,

            TargetFhirPackageKey = targetElement.FhirPackageKey,
            TargetElementKey = targetElement.Key,
            TargetCollatedTypeKey = targetCollated.Key,

            ElementComparisonKey = elementComparisonKey,
        };
        newComparison.Insert(_db, insertPrimaryKey: true);
        return newComparison;
    }


    private ElementTypeTrackingRecord calculateTypeRelationships(
        DbFhirPackage sourcePackage,
        DbFhirPackage targetPackage,
        DbElementType sourceType,
        DbElementType? targetType)
    {
        if (targetType == null)
        {
            return new()
            {
                SourceType = sourceType,
                TargetType = null,
                Relationship = CMR.SourceIsBroaderThanTarget,
                ConceptDomainRelationship = CMR.SourceIsBroaderThanTarget,
                ValueDomainRelationship = CMR.SourceIsBroaderThanTarget,
                UserMessage = $"FHIR {sourcePackage.ShortName} Type `{sourceType.Literal}` has no mapping in FHIR {targetPackage.ShortName}",
                TechnicalMessage = "No target type mapping",
            };
        }

        CMR? conceptDomainRelationship = null;
        CMR? valueDomainRelationship = null;
        List<string> userMessages = [];
        List<string> technicalMessages = [];

        // check for structure comparison if both types have resolved structures
        if ((sourceType.TypeStructureKey != null) && (targetType.TypeStructureKey != null))
        {
            DbStructureComparison? sdComparison = DbStructureComparison.SelectSingle(
                    _db,
                    SourceFhirPackageKey: sourcePackage.Key,
                    SourceStructureKey: sourceType.TypeStructureKey,
                    TargetFhirPackageKey: targetPackage.Key,
                    TargetStructureKey: targetType.TypeStructureKey);
            if (sdComparison is not null)
            {
                conceptDomainRelationship = sdComparison.ConceptDomainRelationship;
                valueDomainRelationship = sdComparison.ValueDomainRelationship;

                if (!string.IsNullOrEmpty(sdComparison.TechnicalMessage))
                {
                    technicalMessages.Add(sdComparison.TechnicalMessage);
                }
            }
        }

        // compare type profiles
        (CMR typeProfileRelationship, string typeProfileMessage) = compareTypeProfiles(sourceType, targetType);
        valueDomainRelationship = FhirDbComparer.ApplyRelationship(valueDomainRelationship, typeProfileRelationship);
        if (!string.IsNullOrEmpty(typeProfileMessage))
        {
            userMessages.Add(typeProfileMessage);
        }

        // compare target profiles
        (CMR targetProfileRelationship, string targetProfileMessage) = compareTargetProfiles(sourceType, targetType);
        valueDomainRelationship = FhirDbComparer.ApplyRelationship(valueDomainRelationship, targetProfileRelationship);
        if (!string.IsNullOrEmpty(targetProfileMessage))
        {
            userMessages.Add(targetProfileMessage);
        }

        // default relationships if not set
        conceptDomainRelationship ??= CMR.Equivalent;
        valueDomainRelationship ??= CMR.Equivalent;

        return new()
        {
            SourceType = sourceType,
            TargetType = targetType,
            Relationship = calculateOverallRelationship(conceptDomainRelationship, valueDomainRelationship),
            ConceptDomainRelationship = conceptDomainRelationship,
            ValueDomainRelationship = valueDomainRelationship,
            UserMessage = string.Join(" ", userMessages),
            TechnicalMessage = string.Join(" ", technicalMessages),
        };
    }


    private (CMR relationship, string message) compareTypeProfiles(DbElementType sourceType, DbElementType targetType)
    {
        string[] sourceProfiles = string.IsNullOrEmpty(sourceType.TypeProfile) ? [] : [sourceType.TypeProfile];
        string[] targetProfiles = string.IsNullOrEmpty(targetType.TypeProfile) ? [] : [targetType.TypeProfile];

        return compareProfiles(sourceProfiles, targetProfiles, $"{sourceType.Literal}:{targetType.Literal}", "type");
    }

    private (CMR relationship, string message) compareTargetProfiles(DbElementType sourceType, DbElementType targetType)
    {
        string[] sourceProfiles = string.IsNullOrEmpty(sourceType.TargetProfile) ? [] : [sourceType.TargetProfile];
        string[] targetProfiles = string.IsNullOrEmpty(targetType.TargetProfile) ? [] : [targetType.TargetProfile];

        return compareProfiles(sourceProfiles, targetProfiles, $"{sourceType.Literal}:{targetType.Literal}", "target");
    }


    private (CMR relationship, string message) compareProfiles(string[] sourceProfileList, string[] targetProfileList, string typeName, string profileType)
    {
        if ((sourceProfileList.Length == 0) && (targetProfileList.Length == 0))
        {
            return (CMR.Equivalent, string.Empty);
        }

        if (sourceProfileList.Length == 0)
        {
            return (CMR.SourceIsBroaderThanTarget, $"Target added {typeName} {profileType} profiles: {string.Join(", ", targetProfileList)}");
        }

        if (targetProfileList.Length == 0)
        {
            return (CMR.SourceIsNarrowerThanTarget, $"Target removed {typeName} {profileType} profiles: {string.Join(", ", sourceProfileList)}");
        }

        HashSet<string> sourceTypeProfiles = new(sourceProfileList);
        HashSet<string> targetTypeProfiles = new(targetProfileList);

        List<string> missingProfiles = sourceTypeProfiles.Except(targetTypeProfiles).ToList();
        List<string> addedProfiles = targetTypeProfiles.Except(sourceTypeProfiles).ToList();

        if ((missingProfiles.Count == 0) && (addedProfiles.Count == 0))
        {
            return (CMR.Equivalent, string.Empty);
        }

        if (missingProfiles.Count == 0)
        {
            return (CMR.SourceIsBroaderThanTarget, $"Target added {typeName} {profileType} profiles: {string.Join(", ", addedProfiles)}");
        }

        if (addedProfiles.Count == 0)
        {
            return (CMR.SourceIsNarrowerThanTarget, $"Target removed {typeName} {profileType} profiles: {string.Join(", ", missingProfiles)}");
        }

        return (CMR.RelatedTo, $"Target added {typeName} {profileType} profiles: {string.Join(", ", addedProfiles)}, removed {profileType} profiles: {string.Join(", ", missingProfiles)}");
    }


    private CMR? calculateOverallRelationship(CMR? conceptDomainRelationship, CMR? valueDomainRelationship)
    {
        if (conceptDomainRelationship == null && valueDomainRelationship == null)
        {
            return CMR.Equivalent;
        }

        if (conceptDomainRelationship == null)
        {
            return valueDomainRelationship;
        }

        if (valueDomainRelationship == null)
        {
            return conceptDomainRelationship;
        }

        return FhirDbComparer.ApplyRelationship(conceptDomainRelationship, valueDomainRelationship);
    }

    private int?[] getKeyArray(
        int sourceIndex,
        int sourceKey,
        int targetIndex,
        int? targetKey)
    {
        int?[] result = [null, null, null, null, null, null];
        result[sourceIndex] = sourceKey;
        result[targetIndex] = targetKey;
        return result;
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

    private DbElementComparison createElementComparison(
        StructureComparisonTrackingRecord sdTrackingRecord,
        DbElement sourceElement,
        DbElement? targetElement,
        int elementComparisonKey,
        CMR? relationship,
        CMR? cdRelationship,
        CMR? vdRelationship,
        DbValueSetComparison? boundVsComparison,
        DbCollatedTypeComparison? collatedTypeComparison,
        string? technicalMessage,
        string? userMessage,
        int?[] contentStepKeys)
    {
        sdTrackingRecord.ComparisonRecordKey ??= DbStructureComparison.GetIndex();

        bool? isIdentical = targetElement is null
            ? null
            : (sourceElement.Id == targetElement.Id);

        bool? relativePathsAreIdentical = targetElement is null
            ? null
            : (sourceElement.Id[sourceElement.StructureName.Length..] == targetElement.Id[targetElement.StructureName.Length..]);

        return new()
        {
            Key = elementComparisonKey,
            StructureComparisonKey = sdTrackingRecord.ComparisonRecordKey.Value,
            ElementMappingKey = sdTrackingRecord.ExplicitMapping?.Key,

            Steps = Math.Abs(sdTrackingRecord.SourcePackage.DefinitionFhirSequence - sdTrackingRecord.TargetPackage.DefinitionFhirSequence),

            SourceFhirPackageKey = sdTrackingRecord.SourcePackage.Key,
            SourceFhirSequence = sdTrackingRecord.SourcePackage.DefinitionFhirSequence,
            SourceStructureKey = sdTrackingRecord.SourceStructure.Key,
            SourceElementKey = sourceElement.Key,
            SourceElementToken = sourceElement.Path,

            TargetFhirPackageKey = sdTrackingRecord.TargetPackage.Key,
            TargetFhirSequence = sdTrackingRecord.TargetPackage.DefinitionFhirSequence,
            TargetStructureKey = sdTrackingRecord.TargetStructure?.Key,
            TargetElementKey = targetElement?.Key,
            TargetElementToken = targetElement?.Path,

            ContentKeys = contentStepKeys,

            Relationship = relationship,
            ConceptDomainRelationship = cdRelationship,
            ValueDomainRelationship = vdRelationship,

            NotMapped = targetElement is null,

            IsIdentical = isIdentical,
            RelativePathsAreIdentical = relativePathsAreIdentical,

            TechnicalMessage = technicalMessage,
            UserMessage = userMessage,

            BoundValueSetComparisonKey = boundVsComparison?.Key,
            CollatedTypeComparisonKey = collatedTypeComparison?.Key,
        };
    }

    private DbCollatedTypeComparison createCollatedTypeComparison(
        DbFhirPackage sourcePackage,
        DbElement sourceElement,
        DbCollatedType sourceCollatedType,
        DbFhirPackage targetPackage,
        DbElement? targetElement,
        DbCollatedType? targetCollatedType,
        int elementComparisonKey,
        List<ElementTypeTrackingRecord> typeComparisons,
        int?[] contentStepKeys)
    {
        bool? isIdentical = targetElement is null
            ? null
            : (sourceElement.Id == targetElement.Id);

        bool? relativePathsAreIdentical = targetElement is null
            ? null
            : (sourceElement.Id[sourceElement.StructureName.Length..] == targetElement.Id[targetElement.StructureName.Length..]);

        List<string> targetProfileMessages = typeComparisons
            .Select(etr => etr.TargetProfileMessage)
            .Where(v => v is not null)!
            .ToList<string>();

        string? targetProfileMessage = targetProfileMessages.Count == 0
            ? null
            : string.Join(' ', targetProfileMessages);

        List<string> typeProfileMessages = typeComparisons
            .Select(etr => etr.TypeProfileMessage)
            .Where(v => v is not null)!
            .ToList<string>();

        string? typeProfileMessage = typeProfileMessages.Count == 0
            ? null
            : string.Join(' ', typeProfileMessages);

        List<string> technicalMessages = typeComparisons
            .Select(etr => etr.TechnicalMessage)
            .Where(v => v is not null)!
            .ToList<string>();

        string? technicalMessage = technicalMessages.Count == 0
            ? null
            : string.Join(' ', technicalMessages);

        List<string> userMessages = typeComparisons
            .Select(etr => etr.UserMessage)
            .Where(v => v is not null)!
            .ToList<string>();

        string? userMessage = userMessages.Count == 0
            ? null
            : string.Join(' ', userMessages);

        return new()
        {
            Key = DbCollatedTypeComparison.GetIndex(),
            ElementComparisonKey = elementComparisonKey,

            Steps = Math.Abs(sourcePackage.DefinitionFhirSequence - targetPackage.DefinitionFhirSequence),

            SourceFhirPackageKey = sourcePackage.Key,
            SourceFhirSequence = sourcePackage.DefinitionFhirSequence,
            SourceElementKey = sourceElement.Key,
            SourceCollatedTypeKey = sourceCollatedType.Key,


            TargetFhirPackageKey = targetPackage.Key,
            TargetFhirSequence = targetPackage.DefinitionFhirSequence,
            TargetElementKey = targetElement?.Key,
            TargetCollatedTypeKey = targetCollatedType?.Key,

            ContentKeys = contentStepKeys,

            Relationship = relationship,
            ConceptDomainRelationship = cdRelationship,
            ValueDomainRelationship = vdRelationship,

            TargetProfileRelationship = ?,
            TargetProfileMessage = targetProfileMessage,
            TypeProfileRelationship = ?,
            TypeProfileMessage = typeProfileMessage,

            NotMapped = targetElement is null,

            IsIdentical = isIdentical,

            TechnicalMessage = technicalMessage,
            UserMessage = userMessage,
        };

    }
}
