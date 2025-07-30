using System.Data;
using System.Diagnostics.CodeAnalysis;
using Hl7.Fhir.Utility;
using Microsoft.Extensions.Logging;
using Microsoft.Health.Fhir.CodeGenCommon.Models;
using Microsoft.Health.Fhir.Comparison.Extensions;
using Microsoft.Health.Fhir.Comparison.Models;
using Microsoft.Health.Fhir.Comparison.XVer;
using static Microsoft.Health.Fhir.Comparison.CompareTool.FhirTypeMappings;
using CMR = Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship;

namespace Microsoft.Health.Fhir.Comparison.CompareTool;

public partial class FhirDbComparer
{
    private record class TypeComparisonTrackingRecord
    {
        public required CollatedType? TargetCollated { get; set; }
        public required DbStructureComparison? SdComparison { get; set; }
        public required CMR? TypeProfileRelationship { get; set; }
        public required string? TypeProfileMessage { get; set; }
        public required CMR? TargetProfileRelationship { get; set; }
        public required string? TargetProfileMessage { get; set; }
    }

    private (DbCollatedTypeComparison etComparison, List<DbElementTypeComparison> typeComparisons) doElementTypeComparison(
        DbComparisonCache<DbStructureComparison> sdComparisonCache,
        DbComparisonCache<DbCollatedTypeComparison> collatedTypeComparisonCache,
        DbComparisonCache<DbElementTypeComparison> typeComparisonCache,
        DbFhirPackageComparisonPair packageForwardPair,
        DbFhirPackageComparisonPair packageReversePair,
        DbElementComparison elementComparison,
        DbFhirPackage sourcePackage,
        DbElement sourceElement,
        DbFhirPackage targetPackage,
        DbElement targetElement)
    {
        // get the source types
        List<DbElementType> sourceTypeList = DbElementType.SelectList(_db, ElementKey: sourceElement.Key);
        ILookup<string?, DbElementType> sourceTypesByName = sourceTypeList.ToLookup(et => et.TypeName);

        // get the source collated types
        List<DbCollatedType> sourceCollatedTypeList = DbCollatedType.SelectList(_db, ElementKey: sourceElement.Key);

        // get the target types
        List<DbElementType> targetTypeList = DbElementType.SelectList(_db, ElementKey: targetElement.Key);
        ILookup<string?, DbElementType> targetTypesByName = targetTypeList.ToLookup(et => et.TypeName);

        // get the target collated types
        List<DbCollatedType> targetCollatedTypeList = DbCollatedType.SelectList(_db, ElementKey: targetElement.Key);

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

                DbStructureComparison? potential = sdComparisonCache.Get(sourceType.TypeStructureKey!.Value, targetType.TypeStructureKey) ??
                    DbStructureComparison.SelectSingle(
                        _db,
                        PackageComparisonKey: elementComparison.PackageComparisonKey,
                        SourceFhirPackageKey: sourcePackage.Key,
                        TargetFhirPackageKey: targetPackage.Key,
                        SourceStructureKey: sourceType.TypeStructureKey,
                        TargetStructureKey: targetType.TypeStructureKey);

                // if we did not find a target, keep looking
                if (potential == null)
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
        List<DbElementTypeComparison> individualTypeComparisons = [];

        foreach ((DbElementType sourceType, DbElementType? targetType) in sourceTargetTypeMappings)
        {
            // calculate relationships for this type pair
            (CMR? conceptDomainRelationship, CMR? valueDomainRelationship, string userMessage, string technicalMessage) =
                calculateTypeRelationships(sourceType, targetType, elementComparison, sourcePackage, targetPackage, sdComparisonCache);

            // create individual type comparison
            DbElementTypeComparison typeComparison = new()
            {
                Key = DbElementTypeComparison.GetIndex(),
                ElementComparisonKey = elementComparison.Key,
                CollatedTypeComparisonKey = -1, // Will be set later
                SourceElementKey = sourceElement.Key,
                SourceTypeKey = sourceType.Key,
                SourceTypeLiteral = sourceType.Literal,
                TargetElementKey = targetElement.Key,
                TargetTypeKey = targetType?.Key,
                TargetTypeLiteral = targetType?.Literal,
                NoMap = targetType == null,
                ConceptDomainRelationship = conceptDomainRelationship,
                ValueDomainRelationship = valueDomainRelationship,
                SourceFhirPackageKey = sourcePackage.Key,
                TargetFhirPackageKey = targetPackage.Key,
                PackageComparisonKey = elementComparison.PackageComparisonKey,
                UserMessage = userMessage,
                TechnicalMessage = technicalMessage,
                Relationship = calculateOverallRelationship(conceptDomainRelationship, valueDomainRelationship),
                IsGenerated = true,
                LastReviewedBy = null,
                LastReviewedOn = null,
            };

            individualTypeComparisons.Add(typeComparison);
        }

        // create collated type comparison from individual comparisons
        DbCollatedTypeComparison collatedComparison = createCollatedTypeComparison(
            elementComparison,
            sourcePackage,
            sourceElement,
            targetPackage,
            targetElement,
            individualTypeComparisons,
            sourceCollatedTypeList,
            targetCollatedTypeList);

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

        return (collatedComparison, individualTypeComparisons);
    }

    private (CMR? conceptDomainRelationship, CMR? valueDomainRelationship, string userMessage, string technicalMessage)
        calculateTypeRelationships(
            DbElementType sourceType,
            DbElementType? targetType,
            DbElementComparison elementComparison,
            DbFhirPackage sourcePackage,
            DbFhirPackage targetPackage,
            DbComparisonCache<DbStructureComparison> sdComparisonCache)
    {
        if (targetType == null)
        {
            return (CMR.SourceIsBroaderThanTarget, CMR.SourceIsBroaderThanTarget,
                    $"Type `{sourceType.Literal}` has no mapping", "No target type mapping");
        }

        CMR? conceptDomainRelationship = null;
        CMR? valueDomainRelationship = null;
        List<string> userMessages = [];
        List<string> technicalMessages = [];

        // check for structure comparison if both types have resolved structures
        if ((sourceType.TypeStructureKey != null) && (targetType.TypeStructureKey != null))
        {
            DbStructureComparison? sdComparison = sdComparisonCache.Get(sourceType.TypeStructureKey.Value, targetType.TypeStructureKey) ??
                DbStructureComparison.SelectSingle(
                    _db,
                    PackageComparisonKey: elementComparison.PackageComparisonKey,
                    SourceFhirPackageKey: sourcePackage.Key,
                    TargetFhirPackageKey: targetPackage.Key,
                    SourceStructureKey: sourceType.TypeStructureKey,
                    TargetStructureKey: targetType.TypeStructureKey);

            if (sdComparison != null)
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
        valueDomainRelationship = applyRelationship(valueDomainRelationship, typeProfileRelationship);
        if (!string.IsNullOrEmpty(typeProfileMessage))
        {
            userMessages.Add(typeProfileMessage);
        }

        // compare target profiles
        (CMR targetProfileRelationship, string targetProfileMessage) = compareTargetProfiles(sourceType, targetType);
        valueDomainRelationship = applyRelationship(valueDomainRelationship, targetProfileRelationship);
        if (!string.IsNullOrEmpty(targetProfileMessage))
        {
            userMessages.Add(targetProfileMessage);
        }

        // default relationships if not set
        conceptDomainRelationship ??= CMR.Equivalent;
        valueDomainRelationship ??= CMR.Equivalent;

        return (conceptDomainRelationship, valueDomainRelationship,
                string.Join(" ", userMessages), string.Join(" ", technicalMessages));
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

        return applyRelationship(conceptDomainRelationship, valueDomainRelationship);
    }

    private DbCollatedTypeComparison createCollatedTypeComparison(
        DbElementComparison elementComparison,
        DbFhirPackage sourcePackage,
        DbElement sourceElement,
        DbFhirPackage targetPackage,
        DbElement targetElement,
        List<DbElementTypeComparison> individualTypeComparisons,
        List<DbCollatedType> sourceCollatedTypeList,
        List<DbCollatedType> targetCollatedTypeList)
    {
        // aggregate relationships from individual comparisons
        CMR conceptDomainRelationship = CMR.Equivalent;
        CMR valueDomainRelationship = CMR.Equivalent;
        bool noMap = true;
        List<string> userMessages = [];

        foreach (DbElementTypeComparison typeComparison in individualTypeComparisons)
        {
            if (typeComparison.NoMap != true)
            {
                noMap = false;
            }

            conceptDomainRelationship = applyRelationship(conceptDomainRelationship, typeComparison.ConceptDomainRelationship);
            valueDomainRelationship = applyRelationship(valueDomainRelationship, typeComparison.ValueDomainRelationship);

            if (!string.IsNullOrEmpty(typeComparison.UserMessage))
            {
                userMessages.Add(typeComparison.UserMessage);
            }
        }

        if (noMap)
        {
            conceptDomainRelationship = applyRelationship(conceptDomainRelationship, CMR.SourceIsBroaderThanTarget);
            valueDomainRelationship = applyRelationship(valueDomainRelationship, CMR.SourceIsBroaderThanTarget);
            userMessages = ["Has no type mapping"];
        }

        // create collated type comparison
        DbCollatedTypeComparison collatedComparison = new()
        {
            Key = DbCollatedTypeComparison.GetIndex(),
            ElementComparisonKey = elementComparison.Key,
            SourceElementKey = sourceElement.Key,
            SourceCollatedTypeKey = sourceCollatedTypeList.FirstOrDefault()?.Key ?? -1,
            TargetElementKey = targetElement?.Key,
            TargetCollatedTypeKey = targetCollatedTypeList.FirstOrDefault()?.Key,
            NoMap = noMap,
            ConceptDomainRelationship = conceptDomainRelationship,
            ValueDomainRelationship = valueDomainRelationship,
            TargetProfileRelationship = CMR.Equivalent, // Simplified for now
            TargetProfileMessage = string.Empty,
            TypeProfileRelationship = CMR.Equivalent, // Simplified for now
            TypeProfileMessage = string.Empty,
            SourceFhirPackageKey = sourcePackage.Key,
            TargetFhirPackageKey = targetPackage.Key,
            PackageComparisonKey = elementComparison.PackageComparisonKey,
            UserMessage = string.Join(" ", userMessages),
            TechnicalMessage = $"Collated comparison of {individualTypeComparisons.Count} type pairs: {string.Join(", ", individualTypeComparisons.Select(tc => "`" + tc.SourceTypeLiteral + ":" + tc.TargetTypeLiteral + "`"))}",
            Relationship = calculateOverallRelationship(conceptDomainRelationship, valueDomainRelationship),
            IsGenerated = true,
            LastReviewedBy = null,
            LastReviewedOn = null,
        };

        return collatedComparison;
    }


    private (DbCollatedTypeComparison etComparison, List<DbElementTypeComparison> typeComparisons) doElementTypeComparisonV0(
        DbComparisonCache<DbStructureComparison> sdComparisonCache,
        DbComparisonCache<DbCollatedTypeComparison> collatedTypeComparisonCache,
        DbComparisonCache<DbElementTypeComparison> typeComparisonCache,
        DbElementComparison elementComparison,
        DbFhirPackage sourcePackage,
        DbElement sourceElement,
        DbFhirPackage targetPackage,
        DbElement targetElement)
    {
        // check for an existing and reviewed collated comparison
        DbCollatedTypeComparison? existingCollated = collatedTypeComparisonCache.Get(elementComparison.Key) ??
            DbCollatedTypeComparison.SelectSingle(_db, ElementComparisonKey: elementComparison.Key);

        if (existingCollated?.LastReviewedOn != null)
        {
            return (existingCollated, DbElementTypeComparison.SelectList(_db, ElementComparisonKey: elementComparison.Key));
        }

        List<DbElementType> sourceTypeList = DbElementType.SelectList(_db, ElementKey: sourceElement.Key);
        ILookup<string?, DbElementType> sourceTypesByName = sourceTypeList.ToLookup(et => et.TypeName);

        List<DbElementType> targetTypeList = DbElementType.SelectList(_db, ElementKey: targetElement.Key);
        ILookup<string?, DbElementType> targetTypesByName = targetTypeList.ToLookup(et => et.TypeName);

        Dictionary<string, CollatedType> collatedTargetTypes = [];
        foreach (IGrouping<string?, DbElementType> expandedType in targetTypesByName)
        {
            CollatedType collated = new(expandedType.AsEnumerable());
            collatedTargetTypes[collated.NormalizedName] = collated;
        }

        HashSet<string> usedTargetTypes = [];
        Dictionary<CollatedType, List<TypeComparisonTrackingRecord>> typeComparisons = [];

        // iterate over each of the types in the source element to determine possible mappings
        foreach (IGrouping<string?, DbElementType> groupedSourceType in sourceTypesByName)
        {
            CollatedType sourceCollated = new(groupedSourceType);
            List<TypeComparisonTrackingRecord> comparisonList = [];
            typeComparisons.Add(sourceCollated, comparisonList);

            CollatedType? targetCollated = null;
            DbStructureComparison? sdComparison = null;

            string sourceNormalizedTypeName = sourceCollated.NormalizedName;

            // first, check for a literal match, since it is simplest
            if (collatedTargetTypes.TryGetValue(sourceNormalizedTypeName, out targetCollated))
            {
                string targetNormalizedTypeName = targetCollated.NormalizedName;

                if ((sourceCollated.TypeStructureKey != null) &&
                    (targetCollated.TypeStructureKey != null))
                {
                    // resolve the comparison for later use
                    sdComparison = sdComparisonCache.Get(sourceCollated.TypeStructureKey!.Value, targetCollated.TypeStructureKey) ??
                        DbStructureComparison.SelectSingle(
                            _db,
                            PackageComparisonKey: elementComparison.PackageComparisonKey,
                            SourceFhirPackageKey: sourcePackage.Key,
                            TargetFhirPackageKey: targetPackage.Key,
                            SourceStructureKey: sourceCollated.TypeStructureKey,
                            TargetStructureKey: targetCollated.TypeStructureKey);
                }

                // if these are both representing the *same* quantity type, we need to handle them specially
                if (sourceCollated.IsQuantityType &&
                    targetCollated.IsQuantityType &&
                    (sourceNormalizedTypeName == targetNormalizedTypeName) &&
                    (((sourceCollated.TypeProfiles.Length == 0) && targetCollated.HasSingleMatchingTypeProfile(sourceNormalizedTypeName)) ||
                     ((targetCollated.TypeProfiles.Length == 0) && sourceCollated.HasSingleMatchingTypeProfile(targetNormalizedTypeName))))
                {
                    comparisonList.Add(new()
                    {
                        TargetCollated = targetCollated,
                        SdComparison = sdComparison,
                        TypeProfileRelationship = sdComparison?.Relationship ?? CMR.Equivalent,
                        TypeProfileMessage = null,
                        TargetProfileRelationship = CMR.Equivalent,
                        TargetProfileMessage = null,
                    });
                }
                else
                {
                    (CMR relationship, string message) typeProfileComparison = compareCollatedProfiles(
                        sourceCollated.TypeProfiles,
                        targetCollated.TypeProfiles,
                        sourceCollated.TypeName ?? string.Empty,
                        "type");
                    (CMR relationship, string message) targetProfileComparison = compareCollatedProfiles(
                        sourceCollated.TargetProfiles,
                        targetCollated.TargetProfiles,
                        sourceCollated.TypeName ?? string.Empty,
                        "target");

                    comparisonList.Add(new()
                    {
                        TargetCollated = targetCollated,
                        SdComparison = sdComparison,
                        TypeProfileRelationship = typeProfileComparison.relationship,
                        TypeProfileMessage = typeProfileComparison.message,
                        TargetProfileRelationship = targetProfileComparison.relationship,
                        TargetProfileMessage = targetProfileComparison.message,
                    });
                }

                usedTargetTypes.Add(targetCollated.NormalizedName);
            }
            else
            {
                // if we don't have a literal match for the type, iterate over our potential targets
                foreach (CollatedType collated in collatedTargetTypes.Values)
                {
                    // we can only test types that have a resolved structure in the database
                    if ((sourceCollated.TypeStructureKey == null) ||
                        (collated.TypeStructureKey == null))
                    {
                        continue;
                    }

                    DbStructureComparison? potential = sdComparisonCache.Get(sourceCollated.TypeStructureKey!.Value, collated.TypeStructureKey) ??
                        DbStructureComparison.SelectSingle(
                            _db,
                            PackageComparisonKey: elementComparison.PackageComparisonKey,
                            SourceFhirPackageKey: sourcePackage.Key,
                            TargetFhirPackageKey: targetPackage.Key,
                            SourceStructureKey: sourceCollated.TypeStructureKey,
                            TargetStructureKey: collated.TypeStructureKey);

                    // if we did not find a target, keep looking
                    if (potential == null)
                    {
                        continue;
                    }

                    (CMR relationship, string message) typeProfileComparison = compareCollatedProfiles(
                        sourceCollated.TypeProfiles,
                        collated.TypeProfiles,
                        (sourceCollated.TypeName ?? string.Empty) + ":" + (collated.TypeName ?? string.Empty),
                        "type");
                    (CMR relationship, string message) targetProfileComparison = compareCollatedProfiles(
                        sourceCollated.TargetProfiles,
                        collated.TargetProfiles,
                        (sourceCollated.TypeName ?? string.Empty) + ":" + (collated.TypeName ?? string.Empty),
                        "type");

                    comparisonList.Add(new()
                    {
                        TargetCollated = collated,
                        SdComparison = potential,
                        TypeProfileRelationship = typeProfileComparison.relationship,
                        TypeProfileMessage = typeProfileComparison.message,
                        TargetProfileRelationship = targetProfileComparison.relationship,
                        TargetProfileMessage = targetProfileComparison.message,
                    });

                    usedTargetTypes.Add(collated.TypeName ?? string.Empty);
                }
            }

            // if we have no matches, there is nothing else to do with this source type
            if (comparisonList.Count == 0)
            {
                continue;
            }
        }

        // be optimitistic
        CMR conceptRelationship = CMR.Equivalent;
        CMR valueRelationship = CMR.Equivalent;
        bool noMap = true;

        List<string> userMessages = [];

        foreach ((CollatedType collated, List<TypeComparisonTrackingRecord> comparisonList) in typeComparisons)
        {
            // if we don't have any matches, this type is a no-map
            if (comparisonList.Count == 0)
            {
                valueRelationship = applyRelationship(valueRelationship, CMR.SourceIsBroaderThanTarget);
                continue;
            }

            noMap = false;
            foreach (TypeComparisonTrackingRecord tcRec in comparisonList)
            {
                conceptRelationship = applyRelationship(conceptRelationship, tcRec.SdComparison?.ConceptDomainRelationship);
                valueRelationship = applyRelationship(valueRelationship, tcRec.SdComparison?.ValueDomainRelationship);
                valueRelationship = applyRelationship(valueRelationship, tcRec.TypeProfileRelationship);
                valueRelationship = applyRelationship(valueRelationship, tcRec.TargetProfileRelationship);

                if ((tcRec.TypeProfileRelationship != CMR.Equivalent) &&
                    !string.IsNullOrEmpty(tcRec.TypeProfileMessage))
                {
                    userMessages.Add(tcRec.TypeProfileMessage);
                }

                if ((tcRec.TargetProfileRelationship != CMR.Equivalent) &&
                    !string.IsNullOrEmpty(tcRec.TargetProfileMessage))
                {
                    userMessages.Add(tcRec.TargetProfileMessage);
                }
            }
        }

        // check for unused target types
        if (usedTargetTypes.Count < collatedTargetTypes.Count)
        {
            valueRelationship = applyRelationship(valueRelationship, CMR.SourceIsNarrowerThanTarget);

            List<string> unusedTargetTypes = collatedTargetTypes.Keys.Except(usedTargetTypes).ToList();

            userMessages.Add($"Added the types: {string.Join(", ", unusedTargetTypes.Select(t => $"`{t}`"))}");
        }

        if (noMap)
        {
            conceptRelationship = applyRelationship(conceptRelationship, CMR.SourceIsBroaderThanTarget);
            valueRelationship = applyRelationship(valueRelationship, CMR.SourceIsBroaderThanTarget);
            userMessages = [$"Has no type mapping"];
        }

        DbCollatedTypeComparison? inverse = null;

        // check for an inverse mapping
        if (!collatedTypeComparisonCache.TryGet((targetElement.Key, sourceElement.Key), out inverse))
        {
            inverse = collatedTypeComparisonCache.Get(targetElement.Key, sourceElement.Key) ??
                DbCollatedTypeComparison.SelectSingle(
                    _db,
                    SourceElementKey: targetElement.Key,
                    TargetElementKey: sourceElement.Key);
        }

        // for simplicity, create a code-gen type mapping that applies relationship rules for us
        CodeGenTypeMapping cgTypeMapping = new(
            sourceElement.FullCollatedTypeLiteral,
            targetElement.FullCollatedTypeLiteral,
            conceptRelationship,
            valueRelationship);

        DbCollatedTypeComparison? typeComparison = existingCollated;

        // compare against our existing comparison if we have one
        if ((existingCollated != null) &&
            (existingCollated.Relationship != cgTypeMapping.Relationship))
        {
            existingCollated.NoMap = noMap;
            existingCollated.Relationship = cgTypeMapping.Relationship;
            existingCollated.ConceptDomainRelationship = cgTypeMapping.ConceptDomainRelationship;
            existingCollated.ValueDomainRelationship = cgTypeMapping.ValueDomainRelationship;
            existingCollated.TechnicalMessage = cgTypeMapping.Comment;
            existingCollated.UserMessage = string.Join(" ", userMessages);
            existingCollated.IsGenerated = true;
            existingCollated.LastReviewedBy = null;
            existingCollated.LastReviewedOn = null;
        }

        throw new NotImplementedException();

        //// build an overal type comparison for this element if we do not have one
        //typeComparison ??= new()
        //{
        //    Key = _comparisonDb.GetElementTypeComparisonKey(),
        //    InverseComparisonKey = inverse?.Key,
        //    SourceFhirPackageKey = sourcePackage.Key,
        //    SourceCollatedTypeKey = ,
        //    SourceElementKey = sourceElement.Key,
        //    SourceCollatedTypeLiteral = sourceElement.FullCollatedTypeLiteral,
        //    TargetFhirPackageKey = targetPackage.Key,
        //    TargetStructureKey = elementComparison.TargetStructureKey,
        //    TargetElementKey = targetElement.Key,
        //    TargetCollatedTypeLiteral = targetElement.FullCollatedTypeLiteral,
        //    PackageComparisonKey = elementComparison.PackageComparisonKey,
        //    StructureComparisonKey = elementComparison.StructureComparisonKey,
        //    ElementComparisonKey = elementComparison.Key,

        //    Relationship = cgTypeMapping.Relationship,
        //    ConceptDomainRelationship = conceptRelationship,
        //    ValueDomainRelationship = valueRelationship,
        //    TechnicalMessage = cgTypeMapping.Comment,
        //    UserMessage = string.Join(" ", userMessages),

        //    NoMap = noMap,

        //    IsGenerated = true,
        //    LastReviewedBy = null,
        //    LastReviewedOn = null,
        //};

        //if ((inverse != null) &&
        //    (inverse.InverseComparisonKey != typeComparison.Key))
        //{
        //    inverse.InverseComparisonKey = typeComparison.Key;
        //    collatedTypeComparisonCache.Changed(inverse);
        //}

        //if (existingCollated == null)
        //{
        //    collatedTypeComparisonCache.CacheAdd(typeComparison);
        //}
        //else
        //{
        //    collatedTypeComparisonCache.Changed(typeComparison);
        //}

        //return typeComparison;

        (CMR relationship, string message) compareCollatedProfiles(string[] sourceProfileList, string[] targetProfileList, string typeName, string profileType)
        {
            if ((sourceProfileList.Length == 0) &&
                (targetProfileList.Length == 0))
            {
                return (CMR.Equivalent, $"Neither element type {typeName} includes {profileType} profiles");
            }

            if (sourceProfileList.Length == 0)
            {
                return (CMR.SourceIsBroaderThanTarget, $"Target added {typeName} {profileType} profiles: {(string.Join(", ", targetProfileList))}");
            }

            if (targetProfileList.Length == 0)
            {
                return (CMR.SourceIsNarrowerThanTarget, $"Target removed {typeName} {profileType} profiles: {(string.Join(", ", sourceProfileList))}");
            }

            HashSet<string> sourceTypeProfiles = new(sourceProfileList);
            HashSet<string> targetTypeProfiles = new(targetProfileList);

            List<string> missingProfiles = sourceTypeProfiles.Except(targetTypeProfiles).ToList();
            List<string> addedProfiles = targetTypeProfiles.Except(sourceTypeProfiles).ToList();

            if ((missingProfiles.Count == 0) &&
                (addedProfiles.Count == 0))
            {
                return (CMR.Equivalent, $"Element type {typeName} {profileType} Profiles are equivalent");
            }

            if (missingProfiles.Count == 0)
            {
                return (CMR.SourceIsBroaderThanTarget, $"Target added {typeName} {profileType} profiles: {(string.Join(", ", addedProfiles))}");
            }

            if (addedProfiles.Count == 0)
            {
                return (CMR.SourceIsNarrowerThanTarget, $"Target removed {typeName} {profileType} profiles: {(string.Join(", ", missingProfiles))}");
            }

            return (
                CMR.RelatedTo,
                $"Target added {typeName} {profileType} profiles: {(string.Join(", ", addedProfiles))}, removed {profileType} profiles: {(string.Join(", ", missingProfiles))}");
        }
    }

}
