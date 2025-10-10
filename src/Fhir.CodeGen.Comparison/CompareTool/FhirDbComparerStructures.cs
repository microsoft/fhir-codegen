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
    private void buildStructureComparisonPairsForSource(
        DbFhirPackage sourcePackage,
        DbFhirPackage targetPackage,
        DbFhirPackageComparisonPair packageForwardPair,
        DbFhirPackageComparisonPair packageReversePair,
        bool allowUpdates = true)
    {
        _sdComparisonCache.Clear();

        List<DbStructureDefinition> sourceStructures = DbStructureDefinition.SelectList(_db, FhirPackageKey: sourcePackage.Key);
        _logger.LogInformation($" <<< processing {sourcePackage.ShortName} Structures, count: {sourceStructures.Count}");

        // iterate over each structure in the source package
        foreach (DbStructureDefinition sourceSd in sourceStructures)
        {
            // skip structures we know we will not process
            if (XVerProcessor._exclusionSet.Contains(sourceSd.UnversionedUrl))
            {
                continue;
            }

            _logger.LogInformation($" <<< processing Structure {sourceSd.VersionedUrl}");

            // check for existing comparisons
            List<DbStructureComparison> cachedForwardComparisons = _sdComparisonCache.ForSource(sourceSd.Key)
                .Where(c => c.TargetFhirPackageKey == targetPackage.Key)
                .ToList();

            List<DbStructureComparison> cachedReverseComparisons = _sdComparisonCache.ForTarget(sourceSd.Key)
                .Where(c => c.SourceFhirPackageKey == targetPackage.Key)
                .ToList();

            List<DbStructureComparison> dbForwardComparisons = DbStructureComparison.SelectList(
                _db,
                PackageComparisonKey: packageForwardPair.Key,
                SourceStructureKey: sourceSd.Key);

            List<DbStructureComparison> dbReverseComparisons = DbStructureComparison.SelectList(
                _db,
                PackageComparisonKey: packageReversePair.Key,
                TargetStructureKey: sourceSd.Key);

            Dictionary<int, DbStructureComparison> forwardComparisons = cachedForwardComparisons.ToDictionary(c => c.TargetStructureKey ?? 0);
            foreach (DbStructureComparison vsc in dbForwardComparisons)
            {
                if (!forwardComparisons.ContainsKey(vsc.TargetStructureKey ?? 0))
                {
                    forwardComparisons[vsc.TargetStructureKey ?? 0] = vsc;
                }
            }

            Dictionary<int, DbStructureComparison> reverseComparisons = cachedReverseComparisons.ToDictionary(c => c.SourceStructureKey);
            foreach (DbStructureComparison vsc in dbReverseComparisons)
            {
                if (!reverseComparisons.ContainsKey(vsc.SourceStructureKey))
                {
                    reverseComparisons[vsc.SourceStructureKey] = vsc;
                }
            }

            // if we found zero comparisons, try to infer some
            if ((forwardComparisons.Count == 0) &&
                (reverseComparisons.Count == 0))
            {
                string techMessage = "Inferred comparison based on ";

                List<DbStructureDefinition> potentialTargets = DbStructureDefinition.SelectList(_db, FhirPackageKey: targetPackage.Key, UnversionedUrl: sourceSd.UnversionedUrl);
                if (potentialTargets.Count != 0)
                {
                    techMessage += $" unversioned URL match from source: `{sourceSd.UnversionedUrl}`";
                }
                else
                {
                    potentialTargets = DbStructureDefinition.SelectList(_db, FhirPackageKey: targetPackage.Key, Name: sourceSd.Name);

                    if (potentialTargets.Count != 0)
                    {
                        techMessage += $" Name match from source: `{sourceSd.Name}`";
                    }
                    else
                    {
                        potentialTargets = DbStructureDefinition.SelectList(_db, FhirPackageKey: targetPackage.Key, Id: sourceSd.Id);

                        if (potentialTargets.Count != 0)
                        {
                            techMessage += $" Id match from source: {sourceSd.Id}";
                        }
                    }
                }

                foreach (DbStructureDefinition targetSd in potentialTargets)
                {
                    // create this comparison
                    DbStructureComparison sdc = new()
                    {
                        Key = DbStructureComparison.GetIndex(),
                        PackageComparisonKey = packageForwardPair.Key,
                        SourceFhirPackageKey = sourcePackage.Key,
                        TargetFhirPackageKey = targetPackage.Key,
                        SourceStructureKey = sourceSd.Key,
                        SourceCanonicalVersioned = sourceSd.VersionedUrl,
                        SourceCanonicalUnversioned = sourceSd.UnversionedUrl,
                        SourceVersion = sourceSd.Version,
                        SourceName = sourceSd.Name,
                        TargetStructureKey = targetSd.Key,
                        TargetCanonicalVersioned = targetSd.VersionedUrl,
                        TargetCanonicalUnversioned = targetSd.UnversionedUrl,
                        TargetVersion = targetSd.Version,
                        TargetName = targetSd.Name,
                        CompositeName = ComparisonDatabase.GetCompositeName(sourcePackage, sourceSd, targetPackage, targetSd),
                        SourceOverviewConceptMapUrl = null,
                        SourceStructureFmlUrl = null,
                        Relationship = null,
                        ConceptDomainRelationship = null,
                        ValueDomainRelationship = null,
                        IsGenerated = true,
                        LastReviewedBy = null,
                        LastReviewedOn = null,
                        ReviewType = null,
                        TechnicalMessage = techMessage,
                        UserMessage = null,
                        IsIdentical = null,
                    };

                    _sdComparisonCache.CacheAdd(sdc);
                    cachedForwardComparisons.Add(sdc);
                }
            }

            // ensure that all forward comparisons have a reverse comparison
            foreach ((int targetKey, DbStructureComparison forward) in forwardComparisons)
            {
                // if our comparison target key is zero, it is an unmapped comparison
                if (targetKey == 0)
                {
                    continue;
                }

                // check to see if we have a known reverse comparison
                if (reverseComparisons.ContainsKey(targetKey))
                {
                    continue;
                }

                // create an inverse comparison
                DbStructureComparison reverse = invert(forward, packageReversePair);
                reverseComparisons.Add(targetKey, reverse);
                _sdComparisonCache.CacheAdd(reverse);

                // update the forward comparison to point to the reverse
                forward.InverseComparisonKey = reverse.Key;
                _sdComparisonCache.CacheUpdate(forward);

                // resolve the target structure definition
                DbStructureDefinition? targetSd = DbStructureDefinition.SelectSingle(
                    _db,
                    Key: targetKey,
                    FhirPackageKey: targetPackage.Key);

                if (targetSd == null)
                {
                    continue;
                }

                // can check for identical Structure Definitions here
                if ((sourceSd.SnapshotCount == targetSd.SnapshotCount) &&
                    (sourceSd.DifferentialCount == targetSd.DifferentialCount) &&
                    (_db.ElementCountThatLookIdentical(sourceSd.Key, targetKey) == int.Max(sourceSd.SnapshotCount, sourceSd.DifferentialCount)))
                {
                    forward.IsIdentical = true;
                    forward.Relationship = CMR.Equivalent;
                    _sdComparisonCache.CacheUpdate(forward);

                    reverse.IsIdentical = true;
                    reverse.Relationship = CMR.Equivalent;
                    _sdComparisonCache.CacheUpdate(reverse);
                }
            }

            // ensure that all reverse comparisons have a forward comparison
            foreach ((int reverseSourceKey, DbStructureComparison reverse) in reverseComparisons)
            {
                // if our comparison source key is zero, it is an unmapped comparison
                if (reverseSourceKey == 0)
                {
                    continue;
                }

                // check to see if we have a known forward comparison
                if (forwardComparisons.ContainsKey(reverseSourceKey))
                {
                    continue;
                }

                // create an inverse comparison
                DbStructureComparison forward = invert(reverse, packageForwardPair);
                forwardComparisons.Add(reverseSourceKey, forward);
                _sdComparisonCache.CacheAdd(forward);

                // update the reverse comparison to point to the forward
                reverse.InverseComparisonKey = forward.Key;
                _sdComparisonCache.CacheUpdate(reverse);

                // resolve the target structure definition
                DbStructureDefinition? targetSd = DbStructureDefinition.SelectSingle(
                    _db,
                    Key: reverseSourceKey,
                    FhirPackageKey: targetPackage.Key);

                if (targetSd == null)
                {
                    continue;
                }

                // can check for identical Structure Definitions here
                if ((sourceSd.SnapshotCount == targetSd.SnapshotCount) &&
                    (sourceSd.DifferentialCount == targetSd.DifferentialCount) &&
                    (_db.ElementCountThatLookIdentical(sourceSd.Key, reverseSourceKey) == int.Max(sourceSd.SnapshotCount, sourceSd.DifferentialCount)))
                {
                    forward.IsIdentical = true;
                    forward.Relationship = CMR.Equivalent;
                    forward.TechnicalMessage += " All elements have 'identical' matches in source and target";
                    forward.UserMessage = "The structures appear identical - they have the same number of" +
                        " elements, with matching Ids, Cardinalities, Types, and primary bindings. Note that" +
                        " it is still possible that changes in meanings of elements could occur.";
                    _sdComparisonCache.CacheUpdate(forward);

                    reverse.IsIdentical = true;
                    reverse.Relationship = CMR.Equivalent;
                    reverse.TechnicalMessage += " All elements have 'identical' matches in source and target";
                    reverse.UserMessage = "The structures appear identical - they have the same number of" +
                        " elements, with matching Ids, Cardinalities, Types, and primary bindings. Note that" +
                        " it is still possible that changes in meanings of elements could occur.";
                    _sdComparisonCache.CacheUpdate(reverse);
                }
            }
        }

        // apply database changes
        _sdComparisonCache.ComparisonsToAdd.Insert(_db);
        _sdComparisonCache.ComparisonsToUpdate.Update(_db);
        _sdComparisonCache.ComparisonsToDelete.Delete(_db);
    }

    public (DbStructureComparison? inverted, bool? changed) UpdateInversion(
        DbStructureComparison forwardComparison,
        DbStructureComparison? inverseComparsion = null)
    {
        bool addedInverse = false;

        DbFhirPackageComparisonPair? inversePackagePair = DbFhirPackageComparisonPair.SelectSingle(
            _db,
            SourcePackageKey: forwardComparison.TargetFhirPackageKey,
            TargetPackageKey: forwardComparison.SourceFhirPackageKey);

        if (inversePackagePair == null)
        {
            return (null, null);
        }

        if ((inverseComparsion == null) &&
            (forwardComparison.InverseComparisonKey != null))
        {
            // look for an existing inverse comparison
            inverseComparsion = DbStructureComparison.SelectSingle(
                _db,
                Key: forwardComparison.InverseComparisonKey);
        }

        if (inverseComparsion == null)
        {
            // create an inverse comparison
            inverseComparsion = invert(forwardComparison, inversePackagePair);
            addedInverse = true;
        }

        DbComparisonCache<DbElementComparison> edComparisonCache = new();

        // get the source elements
        Dictionary<int, DbElement> sourceElements = DbElement.SelectDict(
            _db,
            StructureKey: forwardComparison.SourceStructureKey);

        // get the target elements
        Dictionary<int, DbElement> targetElements = DbElement.SelectDict(
            _db,
            StructureKey: forwardComparison.TargetStructureKey);

        // get the list of forward element comparisons
        List<DbElementComparison> forwardElementComparisons = DbElementComparison.SelectList(
            _db,
            StructureComparisonKey: forwardComparison.Key,
            SourceStructureKey: forwardComparison.SourceStructureKey,
            TargetFhirPackageKey: forwardComparison.TargetFhirPackageKey);

        // get the list of existing inverse element comparisons
        Dictionary<int, DbElementComparison> existingInverseConceptComparisons = DbElementComparison.SelectDict(
            _db,
            StructureComparisonKey: inverseComparsion.Key,
            SourceStructureKey: inverseComparsion.SourceStructureKey,
            TargetFhirPackageKey: inverseComparsion.TargetFhirPackageKey);

        HashSet<int> usedInverseKeys = [];

        List<DbElementComparison> invertedComparisons = [];
        foreach (DbElementComparison forwardConceptComparison in forwardElementComparisons)
        {
            if ((forwardConceptComparison.NoMap == true) ||
                (forwardConceptComparison.TargetElementKey == null))
            {
                continue;
            }

            // create a new inverse comparison
            DbElementComparison computedInverse = invert(
                forwardConceptComparison,
                sourceElements[forwardConceptComparison.SourceElementKey],
                targetElements[(int)forwardConceptComparison.TargetElementKey],
                inverseComparsion,
                inversePackagePair);

            if ((forwardConceptComparison.InverseComparisonKey == null) ||
                (!existingInverseConceptComparisons.TryGetValue((int)forwardConceptComparison.InverseComparisonKey, out DbElementComparison? existingInverse)))
            {
                usedInverseKeys.Add(computedInverse.Key);
                edComparisonCache.CacheAdd(computedInverse);

                if (forwardConceptComparison.InverseComparisonKey != computedInverse.Key)
                {
                    forwardConceptComparison.InverseComparisonKey = computedInverse.Key;
                    edComparisonCache.CacheUpdate(forwardConceptComparison);
                }

                continue;
            }

            usedInverseKeys.Add(existingInverse.Key);

            // check to see if the inverse comparison has the same relationship
            if (existingInverse.Relationship != computedInverse.Relationship)
            {
                existingInverse.Relationship = computedInverse.Relationship;
                edComparisonCache.CacheUpdate(existingInverse);
            }
        }

        // flag we are deleting any inverse comparisons that are not used
        foreach ((int key, DbElementComparison existing) in existingInverseConceptComparisons)
        {
            if (usedInverseKeys.Contains(key))
            {
                continue;
            }
            edComparisonCache.CacheDelete(existing);
        }

        // apply inverse updates to the VS
        if (addedInverse)
        {
            inverseComparsion.Insert(_db);
        }
        else
        {
            inverseComparsion.Update(_db);
        }

        // apply our concept changes
        edComparisonCache.ComparisonsToAdd.Insert(_db);
        edComparisonCache.ComparisonsToUpdate.Update(_db);
        edComparisonCache.ComparisonsToDelete.Delete(_db);

        // return the comparison in case the caller needs it
        return (inverseComparsion, edComparisonCache.Count != 0);
    }

    public void MarkSdMappingsReviewed(
        DbGraphSd.DbSdRow sdRow,
        List<DbGraphSd.DbElementRow> elementProjection,
        string? reviewer)
    {
        DbComparisonCache<DbStructureComparison> sdComparisonCache = new();
        DbComparisonCache<DbElementComparison> edComparisonCache = new();

        // iterate over each of the structure cells
        foreach (DbGraphSd.DbSdCell? sdCell in sdRow.Cells)
        {
            if (sdCell == null)
            {
                continue;
            }

            // check for a left comparison
            if (sdCell.LeftComparison != null)
            {
                sdCell.LeftComparison.LastReviewedOn = DateTime.UtcNow;
                sdCell.LeftComparison.LastReviewedBy = reviewer;
                sdComparisonCache.CacheUpdate(sdCell.LeftComparison);
            }

            // check for a right comparison
            if (sdCell.RightComparison != null)
            {
                sdCell.RightComparison.LastReviewedOn = DateTime.UtcNow;
                sdCell.RightComparison.LastReviewedBy = reviewer;
                sdComparisonCache.CacheUpdate(sdCell.RightComparison);
            }
        }

        // traverse the element projection
        foreach (DbGraphSd.DbElementRow edRow in elementProjection)
        {
            // iterate over the cells in the row
            foreach (DbGraphSd.DbElementCell? edCell in edRow.Cells)
            {
                if (edCell == null)
                {
                    continue;
                }

                // check for a left comparison
                if (edCell.LeftComparison != null)
                {
                    edCell.LeftComparison.LastReviewedOn = DateTime.UtcNow;
                    edCell.LeftComparison.LastReviewedBy = reviewer;
                    edComparisonCache.CacheUpdate(edCell.LeftComparison);
                }

                // check for a right comparison
                if (edCell.RightComparison != null)
                {
                    edCell.RightComparison.LastReviewedOn = DateTime.UtcNow;
                    edCell.RightComparison.LastReviewedBy = reviewer;
                    edComparisonCache.CacheUpdate(edCell.RightComparison);
                }
            }
        }

        // apply the changes to the structure comparisons
        if (sdComparisonCache.ComparisonsToAdd.Any())
        {
            sdComparisonCache.ComparisonsToAdd.Insert(_db);
        }

        if (sdComparisonCache.ComparisonsToUpdate.Any())
        {
            sdComparisonCache.ComparisonsToUpdate.Update(_db);
        }

        if (sdComparisonCache.ComparisonsToDelete.Any())
        {
            sdComparisonCache.ComparisonsToDelete.Delete(_db);
        }

        // apply the changes to the element comparisons
        if (edComparisonCache.ComparisonsToAdd.Any())
        {
            edComparisonCache.ComparisonsToAdd.Insert(_db);
        }

        if (edComparisonCache.ComparisonsToUpdate.Any())
        {
            edComparisonCache.ComparisonsToUpdate.Update(_db);
        }

        if (edComparisonCache.ComparisonsToDelete.Any())
        {
            edComparisonCache.ComparisonsToDelete.Delete(_db);
        }
    }

    public void ApplySdElementChanges(
        List<DbGraphSd.DbElementRow> originalProjection,
        IEnumerable<DbGraphSd.DbElementRow> updatedProjection,
        int sourceColumnIndex,
        bool isComparingRight,
        string? reviewer)
    {
        DbComparisonCache<DbElementComparison> edComparisonCache = new();

        // traverse the locally-modified concept projection and determine changes (add/remove/update)
        foreach (DbGraphSd.DbElementRow row in updatedProjection)
        {
            if (row[sourceColumnIndex] == null)
            {
                continue;
            }

            DbGraphSd.DbElementCell sourceConceptCell = row[sourceColumnIndex]!;
            DbElementComparison sourceToTargetComparison = isComparingRight
                ? sourceConceptCell.RightComparison!
                : sourceConceptCell.LeftComparison!;

            // flag this has been compared if desired
            if (reviewer != null)
            {
                sourceToTargetComparison.LastReviewedOn = DateTime.UtcNow;
                sourceToTargetComparison.LastReviewedBy = reviewer;
            }

            // check for added rows
            if (sourceToTargetComparison.Key == -1)
            {
                // get a good key value
                sourceToTargetComparison.Key = DbElementComparison.GetIndex();

                // cache the addition
                edComparisonCache.CacheAdd(sourceToTargetComparison);
            }
            else
            {
                // cache as an update
                edComparisonCache.CacheUpdate(sourceToTargetComparison);
            }
        }

        // check for deleted rows
        ILookup<Guid, DbGraphSd.DbElementRow> currentConceptRows = updatedProjection.ToLookup(c => c.RowId);
        foreach (DbGraphSd.DbElementRow row in originalProjection)
        {
            if (!currentConceptRows.Contains(row.RowId))
            {
                DbGraphSd.DbElementCell sourceConceptCell = row[sourceColumnIndex]!;
                DbElementComparison sourceToTargetComparison = isComparingRight
                    ? sourceConceptCell.RightComparison!
                    : sourceConceptCell.LeftComparison!;
                // cache the deletion
                edComparisonCache.CacheDelete(sourceToTargetComparison);
            }
        }

        // apply the changes to the concept comparisons
        edComparisonCache.ComparisonsToAdd.Insert(_db);
        edComparisonCache.ComparisonsToUpdate.Update(_db);
        edComparisonCache.ComparisonsToDelete.Delete(_db);
    }

    private void doStructureComparisons(
        DbFhirPackage sourcePackage,
        DbStructureDefinition sourceSd,
        DbFhirPackage targetPackage,
        DbFhirPackageComparisonPair forwardPair,
        DbFhirPackageComparisonPair reversePair)
    {
        // check for a existing comparisons
        List<DbStructureComparison> forwardComparisons = _sdComparisonCache.ForSource(sourceSd.Key)
            .Where(c => c.TargetFhirPackageKey == targetPackage.Key)
            .ToList();

        List<DbStructureComparison> existingDbComparisons = DbStructureComparison.SelectList(
            _db,
            PackageComparisonKey: forwardPair.Key,
            SourceFhirPackageKey: sourcePackage.Key,
            TargetFhirPackageKey: targetPackage.Key,
            SourceStructureKey: sourceSd.Key);

        foreach (DbStructureComparison c in existingDbComparisons)
        {
            if (forwardComparisons.Contains(c))
            {
                continue;
            }
            forwardComparisons.Add(c);
        }

        // if we have no comparisons, there is nothing to do (matches found in discovery phase)
        if (forwardComparisons.Count == 0)
        {
            _logger.LogInformation($"No forward comparisons found for {sourcePackage.ShortName}:{sourceSd.Name} to {targetPackage.ShortName}");
            return;
        }

        // iterate across the forward comparisons
        foreach (DbStructureComparison forwardComparison in forwardComparisons)
        {
            if ((forwardComparison.LastReviewedOn != null) &&
                (forwardComparison.ReviewType == StructureReviewTypeCodes.Complete))
            {
                continue;
            }

            // resolve the target for this comparison
            DbStructureDefinition targetSd = DbStructureDefinition.SelectSingle(
                _db,
                Key: forwardComparison.TargetStructureKey)
                ?? throw new Exception($"Could not resolve target Structure with Key: {forwardComparison.TargetStructureKey} (`{forwardComparison.TargetCanonicalVersioned}`)");

            // run the comparison
            DoStructureComparison(
                _sdComparisonCache,
                _edComparisonCache,
                _collatedTypeComparisonCache,
                _typeComparisonCache,
                sourcePackage,
                sourceSd,
                targetPackage,
                targetSd,
                forwardComparison,
                forwardPair,
                reversePair);
        }

        return;
    }


    public void DoStructureComparison(
        DbComparisonCache<DbStructureComparison> sdComparisonCache,
        DbComparisonCache<DbElementComparison> edComparisonCache,
        DbComparisonCache<DbCollatedTypeComparison> collatedTypeComparisonCache,
        DbComparisonCache<DbElementTypeComparison> typeComparisonCache,
        DbFhirPackage sourcePackage,
        DbStructureDefinition sourceSd,
        DbFhirPackage targetPackage,
        DbStructureDefinition targetSd,
        DbStructureComparison forwardComparison,
        DbFhirPackageComparisonPair forwardPair,
        DbFhirPackageComparisonPair reversePair)
    {
        DbStructureComparison inverseComparison = findOrCreateInverse(
            forwardComparison,
            sdComparisonCache,
            forwardPair,
            reversePair);

        // process this comparison
        List<DbElementComparison> sdElementComparisons = doElementComparisons(
            sdComparisonCache,
            edComparisonCache,
            collatedTypeComparisonCache,
            typeComparisonCache,
            sourcePackage,
            sourceSd,
            targetPackage,
            targetSd,
            forwardComparison,
            inverseComparison,
            forwardPair,
            reversePair,
            out bool identical);

        // check for identical structures
        if (forwardComparison.IsIdentical != identical)
        {
            forwardComparison.IsIdentical = identical;
            sdComparisonCache.Changed(forwardComparison);
        }

        if (inverseComparison.IsIdentical != forwardComparison.IsIdentical)
        {
            inverseComparison.IsIdentical = forwardComparison.IsIdentical;
            sdComparisonCache.Changed(inverseComparison);
        }

        // if we are flagged as reviewed, we do not want to update the structure comparison
        if ((forwardComparison.LastReviewedOn != null) &&
            (forwardComparison.ReviewType > StructureReviewTypeCodes.None))
        {
            return;
        }

        if (aggregateStructureRelationships(forwardComparison, sourceSd, targetSd))
        {
            sdComparisonCache.Changed(forwardComparison);
        }

        if (aggregateStructureRelationships(inverseComparison, targetSd, sourceSd))
        {
            sdComparisonCache.Changed(inverseComparison);
        }

        // update manual relationships we have pre-specified
        if (FhirTypeMappings.CompositeMappingOverrides.TryGetValue(forwardComparison.CompositeName, out CodeGenTypeMapping forwardOverride))
        {
            forwardComparison.Relationship = forwardOverride.Relationship;
            forwardComparison.ConceptDomainRelationship = forwardOverride.ConceptDomainRelationship;
            forwardComparison.ValueDomainRelationship = forwardOverride.ValueDomainRelationship;
            forwardComparison.IsGenerated = true;
            forwardComparison.TechnicalMessage = forwardOverride.Comment;
            sdComparisonCache.Changed(forwardComparison);
        }
        else if (FhirTypeMappings.TryGetMapping(sourceSd.Name, targetSd.Name, out FhirTypeMappings.CodeGenTypeMapping? sourcePM))
        {
            forwardComparison.Relationship = sourcePM.Value.Relationship;
            forwardComparison.ConceptDomainRelationship = sourcePM.Value.ConceptDomainRelationship;
            forwardComparison.ValueDomainRelationship = sourcePM.Value.ValueDomainRelationship;
            forwardComparison.IsGenerated = true;
            forwardComparison.TechnicalMessage = sourcePM.Value.Comment;
            sdComparisonCache.Changed(forwardComparison);
        }

        if (FhirTypeMappings.CompositeMappingOverrides.TryGetValue(inverseComparison.CompositeName, out CodeGenTypeMapping inverseOverride))
        {
            inverseComparison.Relationship = inverseOverride.Relationship;
            inverseComparison.ConceptDomainRelationship = inverseOverride.ConceptDomainRelationship;
            inverseComparison.ValueDomainRelationship = inverseOverride.ValueDomainRelationship;
            inverseComparison.IsGenerated = true;
            inverseComparison.TechnicalMessage = inverseOverride.Comment;
            sdComparisonCache.Changed(inverseComparison);
        }
        else if (FhirTypeMappings.TryGetMapping(targetSd.Name, sourceSd.Name, out FhirTypeMappings.CodeGenTypeMapping? targetPM))
        {
            inverseComparison.Relationship = targetPM.Value.Relationship;
            inverseComparison.ConceptDomainRelationship = targetPM.Value.ConceptDomainRelationship;
            inverseComparison.ValueDomainRelationship = targetPM.Value.ValueDomainRelationship;
            inverseComparison.IsGenerated = true;
            inverseComparison.TechnicalMessage = targetPM.Value.Comment;
            sdComparisonCache.Changed(inverseComparison);
        }

        // build a user message
        List<string> messages = [];

        if (forwardComparison.Relationship == CMR.Equivalent)
        {
            messages.Add($"FHIR {sourcePackage.ShortName}:{sourceSd.Name} " +
                $" maps to {targetPackage.ShortName}:{targetSd.Name} as {forwardComparison.Relationship}.");
        }
        else
        {
            messages.Add($"FHIR {sourcePackage.ShortName}:{sourceSd.Name} " +
                $" maps to {targetPackage.ShortName}:{targetSd.Name} as {forwardComparison.Relationship} based on the element comparisons:");

            ILookup<int, DbElementComparison> elementComparisonsBySourceKey = sdElementComparisons.ToLookup(c => c.SourceElementKey);

            foreach (DbElement element in DbElement.SelectList(_db, StructureKey: sourceSd.Key, orderByProperties: [nameof(DbElement.ResourceFieldOrder)]))
            {
                if (!elementComparisonsBySourceKey.Contains(element.Key))
                {
                    messages.Add($"* `{element.Id}` has no mapping to {targetPackage.ShortName}:{targetSd.Name}");
                    continue;
                }

                foreach (DbElementComparison elementComparison in elementComparisonsBySourceKey[element.Key])
                {
                    if (elementComparison.Relationship == CMR.Equivalent)
                    {
                        continue;
                    }

                    if (elementComparison.UserMessage != null)
                    {
                        messages.Add("* " + elementComparison.UserMessage);
                    }
                    else if (DbElement.SelectSingle(_db, Key: elementComparison.TargetElementKey) is DbElement targetElement)
                    {
                        messages.Add($"* `{element.Id}` maps to `{targetElement.Id}` as {elementComparison.Relationship}");
                    }
                    else
                    {
                        messages.Add($"* `{element.Id}` does not map to {targetPackage.ShortName}:{targetSd.Name}");
                    }
                }
            }
        }

        forwardComparison.UserMessage = string.Join("\n", messages);
        sdComparisonCache.Changed(forwardComparison);
    }


    private DbStructureComparison findOrCreateInverse(
        DbStructureComparison forwardComparison,
        DbComparisonCache<DbStructureComparison> sdComparisonCache,
        DbFhirPackageComparisonPair forwardPair,
        DbFhirPackageComparisonPair reversePair)
    {
        // look for an inverse comparison
        DbStructureComparison? inverseComparison = null;

        if ((forwardComparison.InverseComparisonKey != null) &&
            (forwardComparison.InverseComparisonKey != -1))
        {
            inverseComparison = sdComparisonCache.Get((int)forwardComparison.InverseComparisonKey) ??
                DbStructureComparison.SelectSingle(
                _db,
                Key: forwardComparison.InverseComparisonKey);
        }

        if ((inverseComparison == null) &&
            (forwardComparison.TargetStructureKey != null))
        {
            inverseComparison = sdComparisonCache.Get(forwardComparison.TargetStructureKey!.Value, forwardComparison.SourceStructureKey) ??
                DbStructureComparison.SelectSingle(
                    _db,
                    PackageComparisonKey: reversePair.Key,
                    SourceFhirPackageKey: reversePair.SourcePackageKey,
                    SourceStructureKey: forwardComparison.TargetStructureKey,
                    TargetFhirPackageKey: reversePair.TargetPackageKey,
                    TargetStructureKey: forwardComparison.SourceStructureKey);
        }

        if (inverseComparison == null)
        {
            inverseComparison = invert(forwardComparison, reversePair);
            sdComparisonCache.CacheAdd(inverseComparison);
        }

        if (forwardComparison.InverseComparisonKey != inverseComparison.Key)
        {
            forwardComparison.InverseComparisonKey = inverseComparison.Key;
            sdComparisonCache.Changed(forwardComparison);
        }

        if (inverseComparison.InverseComparisonKey != forwardComparison.Key)
        {
            inverseComparison.InverseComparisonKey = forwardComparison.Key;
            sdComparisonCache.Changed(inverseComparison);
        }

        return inverseComparison;
    }

    public bool AggregateStructureRelationships(DbStructureComparison sdComparison)
    {
        DbStructureDefinition? sourceSd = DbStructureDefinition.SelectSingle(_db, Key: sdComparison.SourceStructureKey);
        DbStructureDefinition? targetSd = DbStructureDefinition.SelectSingle(_db, Key: sdComparison.TargetStructureKey);

        if ((sourceSd == null) || (targetSd == null))
        {
            throw new Exception($"Failed to resolve source or target structure for comparison {sdComparison.Key}");
        }

        return aggregateStructureRelationships(sdComparison, sourceSd!, targetSd!);
    }

    /// <summary>
    /// Aggregates the relationships of value sets within a FHIR package comparison.
    /// </summary>
    /// <param name="sdComparison">The comparison object for the value set.</param>
    /// <returns>True if the relationship was updated, otherwise false.</returns>
    private bool aggregateStructureRelationships(DbStructureComparison sdComparison, DbStructureDefinition sourceSd, DbStructureDefinition targetSd)
    {
        List<DbElementComparison> elementComparisons = DbElementComparison.SelectList(_db, StructureComparisonKey: sdComparison.Key);
        List<CMR?> relationships = elementComparisons.Select(c => c.Relationship).Distinct().ToList();

        // check for no relationships
        if (relationships.Count == 0)
        {
            // don't change anything
            return false;
        }

        // get an initial guess based on the number of elements on each side
        CMR? elementCountRelationship = RelationshipForCounts(sourceSd.SnapshotCount, targetSd.SnapshotCount);
        CMR? domainRelationship = sdComparison.ConceptDomainRelationship switch
        {
            CMR.Equivalent => sdComparison.ValueDomainRelationship ?? CMR.Equivalent,
            _ => (sdComparison.ValueDomainRelationship == null)
                ? sdComparison.ConceptDomainRelationship
                : applyRelationship(sdComparison.ValueDomainRelationship, sdComparison.ValueDomainRelationship)
        };

        CMR? r;

        // check for all the same relationship
        if (relationships.Count == 1)
        {
            r = applyRelationship(relationships[0], elementCountRelationship);
            if (domainRelationship != null)
            {
                r = applyRelationship(r, domainRelationship);
            }

            if (sdComparison.Relationship == r)
            {
                return false;
            }

            sdComparison.Relationship = r;
            return true;
        }

        bool hasNoMaps = relationships.Any(r => r == null);

        // use an existing relationship if we have one, otherwise assume broader if there are non-mapping relationships or equivalent if not
        r = sdComparison.Relationship ?? (hasNoMaps ? CMR.SourceIsBroaderThanTarget : CMR.Equivalent);

        foreach (CMR? relationship in relationships)
        {
            // since we are aggregating a null (no-map) means the higher-level content is broader
            if (relationship == null)
            {
                r = applyRelationship(r, CMR.SourceIsBroaderThanTarget);
                continue;
            }

            r = applyRelationship(r, relationship);
        }

        applyRelationship(r, elementCountRelationship);

        if (domainRelationship != null)
        {
            r = applyRelationship(r, domainRelationship);
        }

        if (sdComparison.Relationship == r)
        {
            return false;
        }

        sdComparison.Relationship = r;
        return true;
    }


    private DbStructureComparison invert(
        DbStructureComparison other,
        DbFhirPackageComparisonPair reversePair)
    {
        // if this is a primitive mapping, override some properties
        if (FhirTypeMappings.TryGetMapping(other.TargetName!, other.SourceName, out FhirTypeMappings.CodeGenTypeMapping? tm))
        {
            return new()
            {
                Key = DbStructureComparison.GetIndex(),
                InverseComparisonKey = other.Key,
                PackageComparisonKey = reversePair.Key,
                SourceFhirPackageKey = other.TargetFhirPackageKey,
                TargetFhirPackageKey = other.SourceFhirPackageKey,
                SourceStructureKey = other.TargetStructureKey!.Value,
                SourceCanonicalVersioned = other.TargetCanonicalVersioned!,
                SourceCanonicalUnversioned = other.TargetCanonicalUnversioned!,
                SourceVersion = other.TargetVersion!,
                SourceName = other.TargetName!,
                TargetStructureKey = other.SourceStructureKey,
                TargetCanonicalVersioned = other.SourceCanonicalVersioned,
                TargetCanonicalUnversioned = other.SourceCanonicalUnversioned,
                TargetVersion = other.SourceVersion,
                TargetName = other.SourceName,
                CompositeName = ComparisonDatabase.GetCompositeName(reversePair.SourcePackageShortName, other.TargetName!, reversePair.TargetPackageShortName, other.SourceName),
                SourceOverviewConceptMapUrl = null,
                SourceStructureFmlUrl = null,
                Relationship = tm.Value.Relationship,
                ConceptDomainRelationship = tm.Value.ConceptDomainRelationship,
                ValueDomainRelationship = tm.Value.ValueDomainRelationship,
                IsGenerated = true,
                LastReviewedBy = null,
                LastReviewedOn = null,
                ReviewType = null,
                TechnicalMessage = tm.Value.Comment,
                UserMessage = null,
                IsIdentical = other.IsIdentical,
            };
        }

        DbFhirPackage? iSourcePackage = DbFhirPackage.SelectSingle(_db, Key: other.TargetFhirPackageKey);
        DbFhirPackage? iTargetPackage = DbFhirPackage.SelectSingle(_db, Key: other.SourceFhirPackageKey);
        DbStructureDefinition? iSourceSd = DbStructureDefinition.SelectSingle(_db, Key: other.TargetStructureKey);
        DbStructureDefinition? iTargetSd = DbStructureDefinition.SelectSingle(_db, Key: other.SourceStructureKey);

        return new()
        {
            Key = DbStructureComparison.GetIndex(),
            InverseComparisonKey = other.Key,
            PackageComparisonKey = reversePair.Key,
            SourceFhirPackageKey = other.TargetFhirPackageKey,
            TargetFhirPackageKey = other.SourceFhirPackageKey,
            SourceStructureKey = other.TargetStructureKey!.Value,
            SourceCanonicalVersioned = other.TargetCanonicalVersioned!,
            SourceCanonicalUnversioned = other.TargetCanonicalUnversioned!,
            SourceVersion = other.TargetVersion!,
            SourceName = other.TargetName!,
            TargetStructureKey = other.SourceStructureKey,
            TargetCanonicalVersioned = other.SourceCanonicalVersioned,
            TargetCanonicalUnversioned = other.SourceCanonicalUnversioned,
            TargetVersion = other.SourceVersion,
            TargetName = other.SourceName,
            CompositeName = ComparisonDatabase.GetCompositeName(reversePair.SourcePackageShortName, other.TargetName!, reversePair.TargetPackageShortName, other.SourceName),
            SourceOverviewConceptMapUrl = null,
            SourceStructureFmlUrl = null,
            Relationship = invert(other.Relationship),
            ConceptDomainRelationship = invert(other.ConceptDomainRelationship),
            ValueDomainRelationship = invert(other.ValueDomainRelationship),
            IsGenerated = true,
            LastReviewedBy = null,
            LastReviewedOn = null,
            ReviewType = null,
            TechnicalMessage = $"Mapping was inverted from Structure comparison {other.Key} of {other.SourceCanonicalVersioned} -> {other.TargetCanonicalVersioned}",
            UserMessage = $"Mapping from FHIR {iSourcePackage?.ShortName}:{iSourceSd?.Name} to FHIR {iTargetPackage?.ShortName}:{iTargetSd?.Name}",
            IsIdentical = other.IsIdentical,
        };
    }
}
