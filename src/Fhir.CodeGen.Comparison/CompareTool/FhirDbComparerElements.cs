using System.Data;
using System.Diagnostics.CodeAnalysis;
using Hl7.Fhir.Utility;
using Microsoft.Extensions.Logging;
using Fhir.CodeGen.Common.Models;
using Fhir.CodeGen.Comparison.Extensions;
using Fhir.CodeGen.Comparison.Models;
using Fhir.CodeGen.Comparison.XVer;
using static Fhir.CodeGen.Comparison.CompareTool.FhirTypeMappings;
using CMR = Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship;

namespace Fhir.CodeGen.Comparison.CompareTool;

public partial class FhirDbComparer
{
    private List<DbElementComparison> doElementComparisons(
        DbComparisonCache<DbStructureComparison> sdComparisonCache,
        DbComparisonCache<DbElementComparison> edComparisonCache,
        DbComparisonCache<DbCollatedTypeComparison> collatedTypeComparisonCache,
        DbComparisonCache<DbElementTypeComparison> typeComparisonCache,
        DbFhirPackage sourcePackage,
        DbStructureDefinition sourceSd,
        DbFhirPackage targetPackage,
        DbStructureDefinition targetSd,
        DbStructureComparison forwardComparison,
        DbStructureComparison reverseComparison,
        DbFhirPackageComparisonPair forwardPair,
        DbFhirPackageComparisonPair reversePair,
        out bool identical)
    {
        List<DbElementComparison> sdElementComparisons = [];

        // select only active and concrete concepts
        List<DbElement> sourceElements = DbElement.SelectList(_db, StructureKey: sourceSd.Key);
        HashSet<string> usedTargetElements = [];

        // be optimistic
        CMR aggregateStructureRelationship = CMR.Equivalent;
        identical = true;

        // iterate over the source elements - note that each type for a choice tyes gets its own record
        foreach (DbElement sourceElement in sourceElements)
        {
            // check for existing comparisons for this element
            List<DbElementComparison> comparisons = DbElementComparison.SelectList(
                _db,
                StructureComparisonKey: forwardComparison.Key,
                SourceStructureKey: sourceSd.Key,
                SourceElementKey: sourceElement.Key,
                TargetFhirPackageKey: targetPackage.Key);

            // build an estimated target id
            string possibleTargetId = sourceElement.Id.Replace(sourceSd.Name!, targetSd.Name, StringComparison.Ordinal);

            // if there are no existing comparisons, see if we can find a matching element
            if ((comparisons.Count == 0) &&
                (DbElement.SelectList(_db, StructureKey: targetSd.Key, Id: possibleTargetId) is List<DbElement> targetElements) &&
                (targetElements.Count != 0))
            {
                CMR relationship = (targetElements.Count == 1)
                    ? CMR.Equivalent
                    : CMR.SourceIsBroaderThanTarget;

                // iterate over the possible targets
                foreach (DbElement targetElement in targetElements)
                {
                    DbElementComparison comp = new()
                    {
                        Key = DbElementComparison.GetIndex(),
                        PackageComparisonKey = forwardPair.Key,
                        StructureComparisonKey = forwardComparison.Key,
                        SourceFhirPackageKey = forwardPair.SourcePackageKey,
                        SourceStructureKey = forwardComparison.SourceStructureKey,
                        SourceStructureUrl = sourceSd.UnversionedUrl,
                        SourceElementToken = sourceElement.Id,
                        SourceElementKey = sourceElement.Key,
                        TargetFhirPackageKey = forwardPair.TargetPackageKey,
                        TargetStructureKey = forwardComparison.TargetStructureKey,
                        TargetStructureUrl = targetSd.UnversionedUrl,
                        TargetElementToken = targetElement.Id,
                        TargetElementKey = targetElement.Key,
                        Relationship = relationship,
                        ConceptDomainRelationship = null,
                        ValueDomainRelationship = null,
                        ElementTypeComparisonKey = null,
                        BoundValueSetComparisonKey = ((sourceElement.BindingValueSetKey != null) && (targetElement.BindingValueSetKey != null))
                            ? DbValueSetComparison.SelectSingle(
                                _db,
                                PackageComparisonKey: forwardPair.Key,
                                SourceFhirPackageKey: forwardPair.SourcePackageKey,
                                TargetFhirPackageKey: forwardPair.TargetPackageKey,
                                SourceValueSetKey: (int)sourceElement.BindingValueSetKey,
                                TargetValueSetKey: (int)targetElement.BindingValueSetKey)?.Key
                            : null,
                        NoMap = false,
                        TechnicalMessage = $"Created mapping based on literal match of id `{sourceElement.Id}`",
                        UserMessage = $"Mapping for FHIR {sourcePackage.ShortName}:{sourceSd.Name} element `{sourceElement.Id}`" +
                            $" to FHIR {targetPackage.ShortName}:{targetSd.Name} element `{targetElement.Id}`",
                        IsGenerated = true,
                        LastReviewedBy = null,
                        LastReviewedOn = null,
                        IsIdentical = null,
                    };

                    edComparisonCache.CacheAdd(comp);
                    comparisons.Add(comp);
                }
            }

            // if there are still no comparisons, add a no-map
            if (comparisons.Count == 0)
            {
                DbElementComparison noMap = new()
                {
                    Key = DbElementComparison.GetIndex(),
                    PackageComparisonKey = forwardPair.Key,
                    StructureComparisonKey = forwardComparison.Key,
                    SourceFhirPackageKey = forwardPair.SourcePackageKey,
                    SourceStructureKey = forwardComparison.SourceStructureKey,
                    SourceStructureUrl = sourceSd.UnversionedUrl,
                    SourceElementToken = sourceElement.Id,
                    SourceElementKey = sourceElement.Key,
                    TargetFhirPackageKey = forwardPair.TargetPackageKey,
                    TargetStructureKey = forwardComparison.TargetStructureKey,
                    TargetStructureUrl = forwardComparison.TargetCanonicalUnversioned,
                    TargetElementToken = null,
                    TargetElementKey = null,
                    Relationship = null,
                    ConceptDomainRelationship = null,
                    ValueDomainRelationship = null,
                    ElementTypeComparisonKey = null,
                    BoundValueSetComparisonKey = null,
                    NoMap = true,
                    TechnicalMessage = $"No mapping exists and no literal match found - created no-map entry for `{sourceSd.Name}`: `{sourceElement.Id}`",
                    UserMessage = $"`{sourceElement.Id}` has no related element in {targetPackage.ShortName}:{targetSd.Name}",
                    IsGenerated = true,
                    LastReviewedBy = null,
                    LastReviewedOn = null,
                    IsIdentical = null,
                };

                // insert into the database
                edComparisonCache.CacheAdd(noMap);

                identical = false;

                // nothing else to do on this pass
                continue;
            }

            // iterate over the comparisons to check relationships
            foreach (DbElementComparison elementComparison in comparisons)
            {
                sdElementComparisons.Add(elementComparison);

                DbElement? targetElement = (elementComparison.TargetElementKey == null)
                    ? null
                    : DbElement.SelectSingle(_db, Key: elementComparison.TargetElementKey);

                if (targetElement == null)
                {
                    if (elementComparison.TargetElementKey != null)
                    {
                        throw new Exception($"Failed to resolve {elementComparison.TargetElementToken} ({elementComparison.TargetElementKey})");
                    }

                    // if there is no target (non-mapping element), there is nothing else to check
                    aggregateStructureRelationship = applyRelationship(aggregateStructureRelationship, CMR.SourceIsBroaderThanTarget);
                    continue;
                }

                usedTargetElements.Add(targetElement.Id);

                DbElementComparison? inverseComparison = null;

                if (elementComparison.InverseComparisonKey != null)
                {
                    inverseComparison = edComparisonCache.Get((int)elementComparison.InverseComparisonKey) ??
                        DbElementComparison.SelectSingle(_db, Key: elementComparison.InverseComparisonKey);
                }

                if (inverseComparison == null)
                {
                    inverseComparison = edComparisonCache.Get(targetElement.Key, elementComparison.TargetElementKey) ??
                        DbElementComparison.SelectSingle(
                            _db,
                            StructureComparisonKey: reverseComparison.Key,
                            SourceElementKey: elementComparison.TargetElementKey,
                            TargetElementKey: elementComparison.SourceElementKey);
                }

                // if there is no inverse we need to create it
                if (inverseComparison == null)
                {
                    inverseComparison = invert(elementComparison, sourceElement, targetElement!, reverseComparison, reversePair, collatedTypeComparisonCache);
                    edComparisonCache.CacheAdd(inverseComparison);
                }

                if (elementComparison.InverseComparisonKey != inverseComparison.Key)
                {
                    elementComparison.InverseComparisonKey = inverseComparison.Key;
                    edComparisonCache.Changed(elementComparison);
                }

                // do basic checks if this has not been reviewed
                if (elementComparison.LastReviewedOn == null)
                {
                    // be optimitistic
                    CMR conceptRelationship = elementComparison.ConceptDomainRelationship ?? CMR.Equivalent;
                    CMR valueRelationship = elementComparison.ValueDomainRelationship ?? CMR.Equivalent;
                    bool noMap = elementComparison.NoMap ?? false;
                    bool isGenerated = elementComparison.IsGenerated ?? false;
                    DbValueSetComparison? boundValueSetComparison = null;

                    bool changed = false;
                    List<string> messages = [];

                    bool firstMessageOnly = false;
                    List<string> userMessages = [];

                    // check for missing no-map value
                    if ((elementComparison.TargetElementKey == null) &&
                        (noMap != true))
                    {
                        noMap = true;
                        messages.Add("No mapping exists and no literal match found - created no-map entry.");
                        changed = true;

                        userMessages.Add($"`{sourceElement.Id}` does not map to {targetPackage.ShortName} `{targetSd.Name}`.");
                        firstMessageOnly = true;
                    }
                    else
                    {
                        userMessages.Add($"`{sourceElement.Id}` maps to {targetPackage.ShortName} `{targetElement.Id}`.");
                    }

                    // check for a single source with multiple targets and any that map as equivalent
                    if ((elementComparison.Relationship == CMR.Equivalent) &&
                        (comparisons.Count > 1))
                    {
                        // mark as not equivalent
                        conceptRelationship = applyRelationship(conceptRelationship, CMR.SourceIsBroaderThanTarget);
                        isGenerated = true;
                        messages.Add($"`{sourceElement.Id}` maps to multiple elements in {targetSd.Name} and cannot be equivalent.");
                        changed = true;

                        if (!firstMessageOnly)
                        {
                            List<DbElement> tes = comparisons
                                .Select(c => c.TargetElementKey)
                                .Where(tk => tk != null)
                                .Distinct()
                                .Select(tk => DbElement.SelectSingle(_db, Key: tk)!)
                                .ToList();

                            userMessages.Add($"To multiple elements in {targetSd.Name}: ({string.Join(", ", tes.Select(e => $"`{e.Id}`"))}).");
                        }
                    }

                    // do type check comparison
                    (DbCollatedTypeComparison collatedTypeComparison, List<DbElementTypeComparison> typeComparisons) = doElementTypeComparison(
                        sdComparisonCache,
                        collatedTypeComparisonCache,
                        typeComparisonCache,
                        forwardPair,
                        reversePair,
                        elementComparison,
                        sourcePackage,
                        sourceElement,
                        targetPackage,
                        targetElement);

                    if (elementComparison.ElementTypeComparisonKey != collatedTypeComparison.Key)
                    {
                        changed = true;
                    }

                    if (valueRelationship != collatedTypeComparison.Relationship)
                    {
                        valueRelationship = applyRelationship(valueRelationship, collatedTypeComparison.Relationship);
                        messages.Add(
                            $"Applied type comparison relationship of: `{collatedTypeComparison.Relationship}`" +
                            $" to existing value relationship: `{elementComparison.Relationship}`.");

                        if (collatedTypeComparison.UserMessage != null)
                        {
                            userMessages.Add(collatedTypeComparison.UserMessage);
                        }
                        changed = true;
                    }

                    // check for optional becoming mandatory: target has a broader concept than source since it requires content
                    // note: 2025.05.10 - Grahame and I agree that there is nothing we need an extension for here - you cannot say anything differently
                    //if ((sourceElement.MinCardinality == 0) &&
                    //    (targetElement.MinCardinality != 0))
                    //{
                    //    conceptRelationship = applyRelationship(conceptRelationship, CMR.SourceIsNarrowerThanTarget);
                    //    messages.Add($"`{targetElement.Name}` made the element mandatory (min cardinality from 0 to {targetElement.MinCardinality}).");
                    //    changed = true;
                    //}

                    // check for source allowing fewer than target requires: target has a broader concept and value than the source
                    // note: 2025.05.10 - Grahame and I agree that there is nothing we need an extension for here - you cannot say anything differently
                    //if (sourceElement.MinCardinality < targetElement.MinCardinality)
                    //{
                    //    conceptRelationship = applyRelationship(conceptRelationship, CMR.SourceIsNarrowerThanTarget);
                    //    valueRelationship = applyRelationship(valueRelationship, CMR.SourceIsNarrowerThanTarget);
                    //    messages.Add($"`{targetElement.Name}` increased the minimum cardinality from {sourceElement.MinCardinality} to {targetElement.MinCardinality}.");
                    //    changed = true;
                    //}

                    // check for element being constrained out: source is broader than target in concept and value
                    if ((sourceElement.MaxCardinality != 0) &&
                        (targetElement.MaxCardinality == 0))
                    {
                        conceptRelationship = applyRelationship(conceptRelationship, CMR.SourceIsBroaderThanTarget);
                        valueRelationship = applyRelationship(valueRelationship, CMR.SourceIsBroaderThanTarget);
                        messages.Add(
                            $"`{targetElement.Name}` constrained the element out" +
                            $" (max cardinality from {sourceElement.MaxCardinalityString} to {targetElement.MaxCardinalityString}).");
                        changed = true;

                        userMessages.Add($"{targetSd.Name} constrained `{targetElement.Id}` out" +
                            $" (max cardinality from {sourceElement.MaxCardinalityString} to {targetElement.MaxCardinalityString}).");
                    }

                    // check for changing from scalar to array: source is narrower than target in value
                    if ((sourceElement.MaxCardinality == 1) &&
                        (targetElement.MaxCardinality != 1))
                    {
                        valueRelationship = applyRelationship(valueRelationship, CMR.SourceIsNarrowerThanTarget);
                        messages.Add($"`{targetElement.Name}` changed from scalar to array (max cardinality from 1 to {targetElement.MaxCardinality}).");
                        changed = true;

                        userMessages.Add($"Changed from a single value to an array" +
                            $" (max cardinality from {sourceElement.MaxCardinalityString} to {targetElement.MaxCardinalityString}).");
                    }

                    // check for changing from array to scalar: source is broader than target in value
                    if ((sourceElement.MaxCardinality != 1) &&
                        (targetElement.MaxCardinality == 1))
                    {
                        valueRelationship = applyRelationship(valueRelationship, CMR.SourceIsBroaderThanTarget);
                        messages.Add($"`{targetElement.Name}` changed from array to scalar (max cardinality from {sourceElement.MaxCardinalityString} to 1).");
                        changed = true;

                        userMessages.Add($"Changed from an array to a single value" +
                            $" (max cardinality from {sourceElement.MaxCardinalityString} to {targetElement.MaxCardinalityString}).");
                    }

                    // check for source allowing more than target allows: target has a broader concept and value than the source
                    if ((targetElement.MaxCardinality != -1) &&
                        (sourceElement.MaxCardinality > targetElement.MaxCardinality))
                    {
                        conceptRelationship = applyRelationship(conceptRelationship, CMR.SourceIsBroaderThanTarget);
                        valueRelationship = applyRelationship(valueRelationship, CMR.SourceIsBroaderThanTarget);
                        messages.Add($"`{targetElement.Name}` decreased the maximum cardinality from {sourceElement.MaxCardinalityString} to {targetElement.MaxCardinalityString}.");
                        changed = true;

                        userMessages.Add($"Decreased the maximum cardinality from {sourceElement.MaxCardinalityString} to {targetElement.MaxCardinalityString}.");
                    }

                    doElementBindingComparison(
                        forwardPair,
                        elementComparison,
                        sourceElement,
                        targetElement,
                        ref boundValueSetComparison,
                        ref conceptRelationship,
                        ref valueRelationship,
                        messages,
                        userMessages,
                        ref changed);

                    // combine the concept and value domain relationships
                    CMR relationship = conceptRelationship switch
                    {
                        CMR.Equivalent => valueRelationship,
                        _ => conceptRelationship,
                    };

                    if (elementComparison.Relationship != relationship)
                    {
                        messages.Add(
                            $"Concept relationship `{conceptRelationship}` and value relationship `{valueRelationship}` combined for relationship: `{relationship}`");
                        changed = true;
                    }

                    // allow identical elements that have different base names
                    if ((elementComparison.ConceptDomainRelationship == CMR.Equivalent) &&
                        (elementComparison.ValueDomainRelationship == CMR.Equivalent) &&
                        (elementComparison.Relationship == CMR.Equivalent) &&
                        (sourceElement.Name == targetElement.Name) &&
                        (sourceElement.FullCollatedTypeLiteral == targetElement.FullCollatedTypeLiteral) &&
                        (sourceElement.BindingValueSet == targetElement.BindingValueSet))
                    {
                        elementComparison.IsIdentical = true;
                        changed = true;
                    }
                    else
                    {
                        identical = false;
                        elementComparison.IsIdentical = false;
                        changed = true;
                    }

                    if (inverseComparison.IsIdentical != elementComparison.IsIdentical)
                    {
                        inverseComparison.IsIdentical = elementComparison.IsIdentical;
                        edComparisonCache.Changed(inverseComparison);
                    }

                    if (firstMessageOnly)
                    {
                        elementComparison.UserMessage = userMessages.First();
                    }
                    else
                    {
                        userMessages.Add($"So is mapped as {relationship}.");
                        elementComparison.UserMessage = string.Join(' ', userMessages);
                    }

                    changed = true;

                    // if we changed something, apply all changes and update the record
                    if (changed)
                    {
                        elementComparison.Relationship = relationship;
                        elementComparison.ConceptDomainRelationship = conceptRelationship;
                        elementComparison.ValueDomainRelationship = valueRelationship;
                        elementComparison.NoMap = noMap;
                        elementComparison.IsGenerated = isGenerated;
                        elementComparison.TechnicalMessage = string.Join(" ", messages);
                        elementComparison.ElementTypeComparisonKey = collatedTypeComparison.Key;
                        elementComparison.BoundValueSetComparisonKey = boundValueSetComparison?.Key;
                        edComparisonCache.Changed(elementComparison);
                    }
                }

                // check to see if the relationship makes sense as an inverse
                CMR? expectedInverseRelationship = invert(elementComparison.Relationship);
                if (inverseComparison.Relationship != expectedInverseRelationship)
                {
                    // check for both being reviewed
                    if ((elementComparison.LastReviewedOn != null) &&
                        (inverseComparison.LastReviewedOn != null))
                    {
                        // cannot update either
                    }
                    //// if the current has been reviewed, update the inverse
                    //else if (elementComparison.LastReviewedOn != null)
                    //{
                    //    inverseComparison.Message = inverseComparison.Message +
                    //        $" Updated relationship from `{inverseComparison.Relationship}` to `{expectedInverseRelationship}`" +
                    //        $" based on the inverse comparsion {elementComparison.Key}, which has a relationship" +
                    //        $" of `{elementComparison.Relationship}`.";
                    //    inverseComparison.Relationship = expectedInverseRelationship;
                    //    elementComparisons.Changed(inverseComparison);
                    //}
                    // if the inverse has been reviewed, update the current
                    else if (inverseComparison.LastReviewedOn != null)
                    {
                        CMR? updatedCurrentRelationship = invert(inverseComparison.Relationship);

                        elementComparison.TechnicalMessage = elementComparison.TechnicalMessage +
                            $" Updated relationship from `{elementComparison.Relationship}` to `{updatedCurrentRelationship}`" +
                            $" based on the inverse comparsion which has a relationship" +
                            $" of `{inverseComparison.Relationship}`.";
                        elementComparison.Relationship = updatedCurrentRelationship;
                        edComparisonCache.Changed(elementComparison);
                    }
                    else
                    {
                        // if one is equivalent and the other is not, we want the not-equivalent by default
                        if ((elementComparison.Relationship == CMR.Equivalent) &&
                            (inverseComparison.Relationship != CMR.Equivalent))
                        {
                            // update the current record
                            CMR? updatedCurrentRelationship = invert(inverseComparison.Relationship);

                            elementComparison.TechnicalMessage = elementComparison.TechnicalMessage +
                                $" Updated relationship from `{elementComparison.Relationship}` to `{updatedCurrentRelationship}`" +
                                $" based on the inverse comparsion which has a relationship" +
                                $" of `{inverseComparison.Relationship}`.";
                            elementComparison.Relationship = updatedCurrentRelationship;
                            edComparisonCache.Changed(elementComparison);
                        }
                        else if ((elementComparison.Relationship != CMR.Equivalent) &&
                                 (inverseComparison.Relationship == CMR.Equivalent))
                        {
                            // update the inverse record
                            inverseComparison.TechnicalMessage = inverseComparison.TechnicalMessage +
                                $" Updated relationship from `{inverseComparison.Relationship}` to `{expectedInverseRelationship}`" +
                                $" based on the inverse comparsion which has a relationship" +
                                $" of `{elementComparison.Relationship}`.";
                            inverseComparison.Relationship = expectedInverseRelationship;
                            edComparisonCache.Changed(inverseComparison);
                        }

                        // odd relationship - leave as mismatched so that a user will review
                    }

                }

                // process the current element's relationship
                aggregateStructureRelationship = applyRelationship(aggregateStructureRelationship, elementComparison.Relationship);
            }
        }

        return sdElementComparisons;
    }

    private void doElementBindingComparison(
        DbFhirPackageComparisonPair forwardPair,
        DbElementComparison elementComparison,
        DbElement sourceElement,
        DbElement targetElement,
        ref DbValueSetComparison? boundValueSetComparison,
        ref CMR conceptRelationship,
        ref CMR valueRelationship,
        List<string> messages,
        List<string> userMessages,
        ref bool changed)
    {
        if ((sourceElement.ValueSetBindingStrength == null) &&
            (targetElement.ValueSetBindingStrength == null))
        {
            return;
        }

        // if neither is a required binding, we do not need to check anything else
        if ((sourceElement.ValueSetBindingStrength != Hl7.Fhir.Model.BindingStrength.Required) &&
            (targetElement.ValueSetBindingStrength != Hl7.Fhir.Model.BindingStrength.Required))
        {
            return;
        }

        // check for increasing binding strength to required - can no longer express anything outside the bound VS
        if ((sourceElement.ValueSetBindingStrength != Hl7.Fhir.Model.BindingStrength.Required) &&
            (targetElement.ValueSetBindingStrength == Hl7.Fhir.Model.BindingStrength.Required))
        {
            conceptRelationship = applyRelationship(conceptRelationship, CMR.SourceIsNarrowerThanTarget);
            valueRelationship = applyRelationship(valueRelationship, CMR.SourceIsNarrowerThanTarget);
            messages.Add($"`{targetElement.Name}` added a required value set binding to `{targetElement.BindingValueSet}`.");
            userMessages.Add($"`{targetElement.Name}` added a required value set binding to `{targetElement.BindingValueSet}`.");
            changed = true;

            // regardless of any other changes, this is a narrowing of content - we don't need to check anything else
            return;
        }

        // check for loosening binding strength from required - can now express *anything*
        if ((sourceElement.ValueSetBindingStrength == Hl7.Fhir.Model.BindingStrength.Required) &&
            (targetElement.ValueSetBindingStrength != Hl7.Fhir.Model.BindingStrength.Required))
        {
            conceptRelationship = applyRelationship(conceptRelationship, CMR.SourceIsBroaderThanTarget);
            valueRelationship = applyRelationship(valueRelationship, CMR.SourceIsBroaderThanTarget);
            messages.Add($"`{targetElement.Name}` removed the required value set binding to `{sourceElement.BindingValueSet}`.");
            userMessages.Add($"`{targetElement.Name}` removed the required value set binding to `{sourceElement.BindingValueSet}`.");
            changed = true;

            // regardless of any other changes, this is a broadening of content - we don't need to check anything else
            return;
        }

        DbValueSet? sourceValueSet = (sourceElement.BindingValueSetKey != null)
            ? DbValueSet.SelectSingle(_db, Key: sourceElement.BindingValueSetKey)
            : null;

        DbValueSet? targetValueSet = (targetElement.BindingValueSetKey != null)
            ? DbValueSet.SelectSingle(_db, Key: targetElement.BindingValueSetKey)
            : null;

        bool excludeSource = (sourceValueSet == null) ||
            (sourceValueSet.IsExcluded == true) ||
            (sourceValueSet.CanExpand == false);

        bool excludeTarget = (targetValueSet == null) ||
            (targetValueSet.IsExcluded == true) ||
            (targetValueSet.CanExpand == false);

        // look for the the source and target value sets not existing, being excluded, or not being expandable
        if (excludeSource && excludeTarget)
        {
            messages.Add($"Failed to resolve or expand both `{sourceElement.BindingValueSet}` and `{targetElement.BindingValueSet}`.");
            userMessages.Add($"The Value Set bindings of `{sourceElement.BindingValueSet}` and `{targetElement.BindingValueSet}` could not be expanded, so the binding was ignored.");
            changed = true;
            return;
        }

        // excluding the source and not the target means the source is broader than the target
        if (excludeSource)
        {
            conceptRelationship = applyRelationship(conceptRelationship, CMR.SourceIsBroaderThanTarget);
            valueRelationship = applyRelationship(valueRelationship, CMR.SourceIsBroaderThanTarget);
            messages.Add($"Failed to resolve or expand the source value set `{sourceElement.BindingValueSet}`, assuming the target is narrower.");
            userMessages.Add($"The source Value Set bindings of `{sourceElement.BindingValueSet}` could not be expanded, so it is assumed to contain more contents.");
            changed = true;
            return;
        }

        // excluding the target and not the source means the source is narrower than the target
        if (excludeTarget)
        {
            conceptRelationship = applyRelationship(conceptRelationship, CMR.SourceIsNarrowerThanTarget);
            valueRelationship = applyRelationship(valueRelationship, CMR.SourceIsNarrowerThanTarget);
            messages.Add($"Failed to resolve or expand the target value set `{targetElement.BindingValueSet}`, assuming the target is broader.");
            userMessages.Add($"The target Value Set bindings of `{targetElement.BindingValueSet}` could not be expanded, so it is assumed to contain more contents.");
            changed = true;
            return;
        }

        // TODO: add handling for additional bindings


        // resolve the value set comparison between these element bindings
        boundValueSetComparison = ((sourceElement.BindingValueSetKey != null) && (targetElement.BindingValueSetKey != null))
            ? DbValueSetComparison.SelectSingle(
                _db,
                PackageComparisonKey: forwardPair.Key,
                SourceFhirPackageKey: forwardPair.SourcePackageKey,
                TargetFhirPackageKey: forwardPair.TargetPackageKey,
                SourceValueSetKey: (int)sourceElement.BindingValueSetKey,
                TargetValueSetKey: (int)targetElement.BindingValueSetKey)
            : null;

        // if we do not have a resolved VS comparison, there is nothing else we want to check here
        if (boundValueSetComparison == null)
        {
            return;
        }

        if (elementComparison.BoundValueSetComparisonKey != boundValueSetComparison.Key)
        {
            elementComparison.BoundValueSetComparisonKey = boundValueSetComparison.Key;
            changed = true;
        }

        // check to see if we need to process the value set comparison due to binding constraints
        List<DbElementType> sourceElementTypes = DbElementType.SelectList(_db, ElementKey: sourceElement.Key);
        List<DbElementType> targetElementTypes = DbElementType.SelectList(_db, ElementKey: targetElement.Key);

        // check for a code type in the source or target
        if (sourceElementTypes.Any(et => et.TypeName == "code") ||
            targetElementTypes.Any(et => et.TypeName == "code"))
        {
            // if the value set is equivalent or broadens, just apply the comparison
            if ((boundValueSetComparison.Relationship == CMR.Equivalent) ||
                (boundValueSetComparison.Relationship == CMR.SourceIsNarrowerThanTarget))
            {
                conceptRelationship = applyRelationship(conceptRelationship, boundValueSetComparison.Relationship);
                valueRelationship = applyRelationship(valueRelationship, boundValueSetComparison.Relationship);
                messages.Add($"Applied bound value set relationship of `{boundValueSetComparison.Relationship}`.");
            }
            else if ((sourceValueSet != null) && (targetValueSet != null))
            {
                // need to resolve the value set contents to check codes
                List<DbValueSetConcept> sourceVsConcepts = DbValueSetConcept.SelectList(
                    _db,
                    ValueSetKey: sourceElement.BindingValueSetKey,
                    Inactive: false,
                    Abstract: false);
                HashSet<DbValueSetConcept> targetVsConcepts = new(DbValueSetConcept.SelectList(
                    _db,
                    ValueSetKey: targetElement.BindingValueSetKey,
                    Inactive: false,
                    Abstract: false));

                // check for all codes having a match in the target
                if (sourceVsConcepts.All(c => targetVsConcepts.Contains(c)))
                {
                    // check for same number of concepts
                    if (sourceVsConcepts.Count == targetVsConcepts.Count)
                    {
                        conceptRelationship = applyRelationship(conceptRelationship, CMR.Equivalent);
                        valueRelationship = applyRelationship(valueRelationship, CMR.Equivalent);
                        messages.Add($"Source and target bound value sets have same number of codes and all codes match - required binding is COMPATIBLE for `code` type.");
                        userMessages.Add($"The source and target bound value sets have same number of codes and all codes match, so the required binding is COMPATIBLE for a required binding on a `code` type.");
                    }
                    else
                    {
                        conceptRelationship = applyRelationship(conceptRelationship, CMR.SourceIsNarrowerThanTarget);
                        valueRelationship = applyRelationship(valueRelationship, CMR.SourceIsNarrowerThanTarget);
                        messages.Add($"Target bound value set has more codes than source - required binding is COMPATIBLE for `code` type.");
                        userMessages.Add($"The target bound value set has more codes than source, so the required binding is COMPATIBLE for a required binding on a `code` type in this direction.");
                    }
                }
                else
                {
                    conceptRelationship = applyRelationship(conceptRelationship, boundValueSetComparison.Relationship);
                    valueRelationship = applyRelationship(valueRelationship, boundValueSetComparison.Relationship);
                    messages.Add(
                        $"Target value set is INCOMPATIBLE for a required binding on a `code` type," +
                        $" VS Relationship: {boundValueSetComparison.Relationship}.");
                    userMessages.Add($"The value set relationship of {boundValueSetComparison.Relationship} is INCOMPATIBLE for a required binding on a `code` type in this direction.");
                }
            }
            // excluded value sets are assumed compatible
            else if ((sourceValueSet?.IsExcluded == true) ||
                (targetValueSet?.IsExcluded == true))
            {
                conceptRelationship = applyRelationship(conceptRelationship, CMR.Equivalent);
                valueRelationship = applyRelationship(valueRelationship, CMR.Equivalent);
                messages.Add($"Source or target value set is excluded - assuming required binding is COMPATIBLE for `code` type.");
                userMessages.Add($"The source or target value set has been manually flagged as COMPATIBLE for a required binding on a `code` type in this direction.");
            }
            else
            {
                conceptRelationship = applyRelationship(conceptRelationship, boundValueSetComparison.Relationship);
                valueRelationship = applyRelationship(valueRelationship, boundValueSetComparison.Relationship);
                messages.Add(
                    $"Source and target bound value sets are not compatible ({boundValueSetComparison.Relationship})" +
                    $" - required binding is INCOMPATIBLE for `code` type binding.");
                userMessages.Add($"The value set relationship of {boundValueSetComparison.Relationship} is INCOMPATIBLE for a required binding on a `code` type in this direction.");
            }

            changed = true;
        }

        // check if there are any non-code types in the source or target
        if (sourceElementTypes.Any(et => et.TypeName != "code") ||
            targetElementTypes.Any(et => et.TypeName != "code"))
        {
            // if the value set is equivalent or broadens, just apply the comparison
            if ((boundValueSetComparison.Relationship == CMR.Equivalent) ||
                (boundValueSetComparison.Relationship == CMR.SourceIsNarrowerThanTarget))
            {
                conceptRelationship = applyRelationship(conceptRelationship, boundValueSetComparison.Relationship);
                valueRelationship = applyRelationship(valueRelationship, boundValueSetComparison.Relationship);
                messages.Add($"Applied bound value set relationship of `{boundValueSetComparison.Relationship}`.");
            }
            // excluded value sets are assumed compatible
            else if ((sourceValueSet?.IsExcluded == true) ||
                (targetValueSet?.IsExcluded == true))
            {
                conceptRelationship = applyRelationship(conceptRelationship, CMR.Equivalent);
                valueRelationship = applyRelationship(valueRelationship, CMR.Equivalent);
                messages.Add($"Source or target value set is excluded - assuming required binding is COMPATIBLE for non-code type.");
                userMessages.Add($"The source or target value set has been manually flagged as COMPATIBLE for the `code` type in this direction.");
            }
            else
            {
                conceptRelationship = applyRelationship(conceptRelationship, boundValueSetComparison.Relationship);
                valueRelationship = applyRelationship(valueRelationship, boundValueSetComparison.Relationship);
                messages.Add(
                    $"Source and target bound value sets are not compatible ({boundValueSetComparison.Relationship})" +
                    $" - required binding is INCOMPATIBLE for non-code type binding.");
                userMessages.Add($"The value set relationship of {boundValueSetComparison.Relationship} is INCOMPATIBLE for a required binding on a non-`code` type in this direction.");
            }
            changed = true;
        }
    }

    private DbElementComparison invert(
        DbElementComparison other,
        DbElement otherSourceElement,
        DbElement otherTargetElement,
        DbStructureComparison reverseCanonicalComparison,
        DbFhirPackageComparisonPair reversePair,
        DbComparisonCache<DbCollatedTypeComparison>? collatedTypeComparisonCache = null)
    {
        int? inverseTypeComparisonKey = collatedTypeComparisonCache?.Get(otherTargetElement.Key, otherSourceElement.Key)?.Key
            ?? DbCollatedTypeComparison.SelectSingle(_db, SourceElementKey: otherTargetElement.Key, TargetElementKey: otherSourceElement.Key)?.Key;
        int? boundValueSetComparisonKey = (otherSourceElement.BindingValueSetKey == null) || (otherTargetElement.BindingValueSetKey == null)
            ? null
            : _vsComparisonCache.Get((int)otherTargetElement.BindingValueSetKey, otherSourceElement.BindingValueSetKey)?.Key ??
                DbValueSetComparison.SelectSingle(
                    _db,
                    PackageComparisonKey: reversePair.Key,
                    SourceFhirPackageKey: other.TargetFhirPackageKey,
                    SourceValueSetKey: otherTargetElement.BindingValueSetKey,
                    TargetFhirPackageKey: other.SourceFhirPackageKey,
                    TargetValueSetKey: otherSourceElement.BindingValueSetKey)?.Key;

        DbFhirPackage? iSourcePackage = DbFhirPackage.SelectSingle(_db, Key: other.TargetFhirPackageKey);
        DbFhirPackage? iTargetPackage = DbFhirPackage.SelectSingle(_db, Key: other.SourceFhirPackageKey);
        DbStructureDefinition? iSourceSd = DbStructureDefinition.SelectSingle(_db, Key: other.TargetStructureKey);
        DbStructureDefinition? iTargetSd = DbStructureDefinition.SelectSingle(_db, Key: other.SourceStructureKey);

        return new()
        {
            Key = DbElementComparison.GetIndex(),
            InverseComparisonKey = other.Key,
            PackageComparisonKey = reversePair.Key,
            SourceFhirPackageKey = other.TargetFhirPackageKey,
            TargetFhirPackageKey = other.SourceFhirPackageKey,
            SourceStructureKey = other.TargetStructureKey!.Value,
            SourceStructureUrl = other.TargetStructureUrl!,
            SourceElementKey = other.TargetElementKey!.Value,
            SourceElementToken = other.TargetElementToken!,
            TargetStructureKey = other.SourceStructureKey,
            TargetStructureUrl = other.SourceStructureUrl,
            TargetElementKey = other.SourceElementKey,
            TargetElementToken = other.SourceElementToken,
            StructureComparisonKey = reverseCanonicalComparison.Key,
            ElementTypeComparisonKey = inverseTypeComparisonKey,
            BoundValueSetComparisonKey = boundValueSetComparisonKey,
            Relationship = invert(other.Relationship),
            ConceptDomainRelationship = invert(other.ConceptDomainRelationship),
            ValueDomainRelationship = invert(other.ValueDomainRelationship),
            NoMap = false,
            IsGenerated = true,
            LastReviewedBy = null,
            LastReviewedOn = null,
            TechnicalMessage = $"Mapping was inverted from Element comparison {other.Key}" +
                $" of `{otherSourceElement.Id}` -> `{otherTargetElement.Id}`",
            UserMessage = $"Element `{other.TargetElementKey}`" +
                $" maps to {iTargetPackage?.ShortName} `{other.SourceElementKey}`",
            IsIdentical = other.IsIdentical,
        };
    }

}
