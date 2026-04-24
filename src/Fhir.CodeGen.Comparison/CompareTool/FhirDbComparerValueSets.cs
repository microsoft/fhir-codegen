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
#if false
    private void buildValueSetComparisonPairsForSource(
        DbFhirPackage sourcePackage,
        DbFhirPackage targetPackage,
        DbFhirPackageComparisonPair packageForwardPair,
        DbFhirPackageComparisonPair packageReversePair,
        bool allowUpdates = true)
    {
        _vsComparisonCache.Clear();

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

            _logger.LogInformation($" <<< processing ValueSet {sourceVs.VersionedUrl}");

            // check for existing comparisons
            List<DbValueSetComparison> cachedForwardComparisons = _vsComparisonCache.ForSource(sourceVs.Key)
                .Where(c => c.TargetFhirPackageKey == targetPackage.Key)
                .ToList();

            List<DbValueSetComparison> cachedReverseComparisons = _vsComparisonCache.ForTarget(sourceVs.Key)
                .Where(c => c.SourceFhirPackageKey == targetPackage.Key)
                .ToList();

            List<DbValueSetComparison> dbForwardComparisons = DbValueSetComparison.SelectList(
                _db,
                PackageComparisonKey: packageForwardPair.Key,
                SourceValueSetKey: sourceVs.Key);

            List<DbValueSetComparison> dbReverseComparisons = DbValueSetComparison.SelectList(
                _db,
                PackageComparisonKey: packageReversePair.Key,
                TargetValueSetKey: sourceVs.Key);

            Dictionary<int, DbValueSetComparison> forwardComparisons = cachedForwardComparisons.ToDictionary(c => c.TargetValueSetKey ?? 0);
            foreach (DbValueSetComparison vsc in dbForwardComparisons)
            {
                if (!forwardComparisons.ContainsKey(vsc.TargetValueSetKey ?? 0))
                {
                    forwardComparisons[vsc.TargetValueSetKey ?? 0] = vsc;
                }
            }

            Dictionary<int, DbValueSetComparison> reverseComparisons = cachedReverseComparisons.ToDictionary(c => c.SourceValueSetKey);
            foreach (DbValueSetComparison vsc in dbReverseComparisons)
            {
                if (!reverseComparisons.ContainsKey(vsc.SourceValueSetKey))
                {
                    reverseComparisons[vsc.SourceValueSetKey] = vsc;
                }
            }

            // if we found zero comparisons, try to infer some
            if ((forwardComparisons.Count == 0) &&
                (reverseComparisons.Count == 0))
            {
                string techMessage = "Inferred comparison based on ";

                List<DbValueSet> potentialTargets = DbValueSet.SelectList(_db, FhirPackageKey: targetPackage.Key, UnversionedUrl: sourceVs.UnversionedUrl);
                if (potentialTargets.Count != 0)
                {
                    techMessage += $" unversioned URL match from source: `{sourceVs.UnversionedUrl}`";
                }
                else
                {
                    potentialTargets = DbValueSet.SelectList(_db, FhirPackageKey: targetPackage.Key, Name: sourceVs.Name);

                    if (potentialTargets.Count != 0)
                    {
                        techMessage += $" Name match from source: `{sourceVs.Name}`";
                    }
                    else
                    {
                        potentialTargets = DbValueSet.SelectList(_db, FhirPackageKey: targetPackage.Key, Id: sourceVs.Id);

                        if (potentialTargets.Count != 0)
                        {
                            techMessage += $" Id match from source: {sourceVs.Id}";
                        }
                    }
                }

                foreach (DbValueSet targetVs in potentialTargets)
                {
                    // create this comparison
                    DbValueSetComparison vsc = new()
                    {
                        Key = DbValueSetComparison.GetIndex(),
                        PackageComparisonKey = packageForwardPair.Key,
                        SourceFhirPackageKey = sourcePackage.Key,
                        TargetFhirPackageKey = targetPackage.Key,
                        SourceValueSetKey = sourceVs.Key,
                        SourceCanonicalVersioned = sourceVs.VersionedUrl,
                        SourceCanonicalUnversioned = sourceVs.UnversionedUrl,
                        SourceVersion = sourceVs.Version,
                        SourceName = sourceVs.Name,
                        TargetValueSetKey = targetVs.Key,
                        TargetCanonicalVersioned = targetVs.VersionedUrl,
                        TargetCanonicalUnversioned = targetVs.UnversionedUrl,
                        TargetVersion = targetVs.Version,
                        TargetName = targetVs.Name,
                        CompositeName = ComparisonDatabase.GetCompositeName(sourcePackage, sourceVs, targetPackage, targetVs),
                        SourceConceptMapUrl = null,
                        SourceConceptMapAdditionalUrls = null,
                        Relationship = null,
                        IsGenerated = true,
                        LastReviewedBy = null,
                        LastReviewedOn = null,
                        TechnicalMessage = techMessage,
                        UserMessage = $"Value Set {sourcePackage.ShortName}:{sourceVs.Name} (`{sourceVs.VersionedUrl}`)" +
                            $" maps to {targetPackage.ShortName}:{targetVs.Name} (`{targetVs.VersionedUrl}`)",
                        IsIdentical = false,
                        CodeLiteralsAreIdentical = false,
                    };

                    _vsComparisonCache.CacheAdd(vsc);
                    cachedForwardComparisons.Add(vsc);
                }
            }

            // ensure that all forward comparisons have a reverse comparison
            foreach ((int targetKey, DbValueSetComparison forward) in forwardComparisons)
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
                DbValueSetComparison reverse = invert(forward, packageReversePair);
                reverseComparisons.Add(targetKey, reverse);
                _vsComparisonCache.CacheAdd(reverse);

                // update the forward comparison to point to the reverse
                forward.InverseComparisonKey = reverse.Key;
                _vsComparisonCache.CacheUpdate(forward);

                // resolve the target value set
                DbValueSet? targetVs = DbValueSet.SelectSingle(
                    _db,
                    Key: targetKey,
                    FhirPackageKey: targetPackage.Key);

                if (targetVs == null)
                {
                    continue;
                }

                // can check for identical Value Sets here
                if ((sourceVs.ConceptCount == targetVs.ConceptCount) &&
                    (_db.ConceptCountWithLiteralMatches(sourceVs.Key, targetKey) == sourceVs.ConceptCount))
                {
                    forward.IsIdentical = true;
                    forward.Relationship = CMR.Equivalent;
                    _vsComparisonCache.CacheUpdate(forward);

                    reverse.IsIdentical = true;
                    reverse.Relationship = CMR.Equivalent;
                    _vsComparisonCache.CacheUpdate(reverse);
                }
            }

            // ensure that all reverse comparisons have a forward comparison
            foreach ((int reverseSourceKey, DbValueSetComparison reverse) in reverseComparisons)
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
                DbValueSetComparison forward = invert(reverse, packageForwardPair);
                forwardComparisons.Add(reverseSourceKey, forward);
                _vsComparisonCache.CacheAdd(forward);

                // update the reverse comparison to point to the forward
                reverse.InverseComparisonKey = forward.Key;
                _vsComparisonCache.CacheUpdate(reverse);

                // resolve the target value set
                DbValueSet? targetVs = DbValueSet.SelectSingle(
                    _db,
                    Key: reverseSourceKey,
                    FhirPackageKey: targetPackage.Key);

                if (targetVs == null)
                {
                    continue;
                }

                // can check for identical Value Sets here
                if ((sourceVs.ConceptCount == targetVs.ConceptCount) &&
                    (_db.ConceptCountWithLiteralMatches(sourceVs.Key, reverseSourceKey) == sourceVs.ConceptCount))
                {
                    forward.IsIdentical = true;
                    forward.Relationship = CMR.Equivalent;
                    forward.TechnicalMessage += " All concepts have literal matches in source and target";
                    forward.UserMessage = "The value sets appear identical - they have the same number of" +
                        " concepts and all concepts are literal matches between versions. Note that the" +
                        " changes in meanings of concepts could still occur.";
                    _vsComparisonCache.CacheUpdate(forward);

                    reverse.IsIdentical = true;
                    reverse.Relationship = CMR.Equivalent;
                    reverse.TechnicalMessage += " All concepts have literal matches in source and target";
                    reverse.UserMessage = "The value sets appear identical - they have the same number of" +
                        " concepts and all concepts are literal matches between versions. Note that the" +
                        " changes in meanings of concepts could still occur.";
                    _vsComparisonCache.CacheUpdate(reverse);
                }
            }
        }

        // apply database changes
        _vsComparisonCache.ToAdd.Insert(_db, insertPrimaryKey: true);
        _vsComparisonCache.ToUpdate.Update(_db);
        _vsComparisonCache.ToDelete.Delete(_db);
    }


    public void DoValueSetComparison(
        DbComparisonCache<DbValueSetComparison> vsComparisonCache,
        DbComparisonCache<DbValueSetConceptComparison> conceptComparisonCache,
        DbFhirPackage sourcePackage,
        DbValueSet sourceVs,
        DbFhirPackage targetPackage,
        DbValueSet targetVs,
        DbValueSetComparison forwardComparison,
        DbFhirPackageComparisonPair forwardPair,
        DbFhirPackageComparisonPair reversePair)
    {
        // look for a known inverse comparison
        DbValueSetComparison? inverseComparison = null;
        if (forwardComparison.InverseComparisonKey != null)
        {
            inverseComparison = vsComparisonCache.Get((int)forwardComparison.InverseComparisonKey) ??
                DbValueSetComparison.SelectSingle(_db, Key: forwardComparison.InverseComparisonKey);
        }

        // look for an inverse comparison based on the source and target
        if ((inverseComparison == null) &&
            (forwardComparison.InverseComparisonKey == null))
        {
            inverseComparison = vsComparisonCache.Get(targetVs.Key, forwardComparison.SourceContentKey)
                ?? DbValueSetComparison.SelectSingle(
                    _db,
                    PackageComparisonKey: reversePair.Key,
                    SourceFhirPackageKey: targetPackage.Key,
                    SourceValueSetKey: forwardComparison.TargetValueSetKey,
                    TargetFhirPackageKey: sourcePackage.Key,
                    TargetValueSetKey: forwardComparison.SourceValueSetKey);
        }

        // create a new inverse comparison if we don't have one
        if (inverseComparison == null)
        {
            inverseComparison = invert(forwardComparison, reversePair);
            vsComparisonCache.CacheAdd(inverseComparison);
        }

        // check to see if the inverse comparison key is set
        if (forwardComparison.InverseComparisonKey != inverseComparison.Key)
        {
            forwardComparison.InverseComparisonKey = inverseComparison.Key;
            vsComparisonCache.Changed(inverseComparison);
        }

        // process this comparison
        List<DbValueSetConceptComparison> vsConceptComparisons = doValueSetConceptComparisons(
            conceptComparisonCache,
            sourcePackage,
            sourceVs,
            targetPackage,
            targetVs,
            forwardComparison,
            inverseComparison,
            forwardPair,
            reversePair);

        // check for identical system and code flags
        if ((forwardComparison.IsIdentical == null) || (forwardComparison.CodeLiteralsAreIdentical == null))
        {
            // select only active and concrete concepts
            List<DbValueSetConcept> sourceConcepts = DbValueSetConcept.SelectList(_db, ValueSetKey: sourceVs.Key, Inactive: false, Abstract: false);
            List<DbValueSetConcept> targetConcepts = DbValueSetConcept.SelectList(_db, ValueSetKey: targetVs.Key, Inactive: false, Abstract: false);

            if (sourceConcepts.Count != targetConcepts.Count)
            {
                forwardComparison.IsIdentical = false;
                forwardComparison.CodeLiteralsAreIdentical = false;
            }
            else
            {
                HashSet<string> targetCombined = targetConcepts.Select(c => c.FhirKey).ToHashSet();

                if (sourceConcepts.All(c => targetCombined.Contains(c.FhirKey)))
                {
                    forwardComparison.IsIdentical = true;
                    forwardComparison.CodeLiteralsAreIdentical = true;
                }
                else
                {
                    HashSet<string> targetCodes = targetConcepts.Select(c => c.Code).ToHashSet();

                    if (sourceConcepts.All(c => targetCodes.Contains(c.Code)))
                    {
                        forwardComparison.IsIdentical = false;
                        forwardComparison.CodeLiteralsAreIdentical = true;
                    }
                    else
                    {
                        forwardComparison.IsIdentical = false;
                        forwardComparison.CodeLiteralsAreIdentical = false;
                    }
                }
            }

            vsComparisonCache.Changed(forwardComparison);
        }

        if ((inverseComparison.IsIdentical == null) || (inverseComparison.CodeLiteralsAreIdentical == null))
        {
            inverseComparison.IsIdentical = forwardComparison.IsIdentical;
            inverseComparison.CodeLiteralsAreIdentical = forwardComparison.CodeLiteralsAreIdentical;

            vsComparisonCache.Changed(inverseComparison);
        }

        if (aggregateValueSetRelationships(forwardComparison, sourceVs, targetVs, conceptComparisonCache))
        {
            vsComparisonCache.Changed(forwardComparison);
        }

        if (aggregateValueSetRelationships(inverseComparison, targetVs, sourceVs, conceptComparisonCache))
        {
            vsComparisonCache.Changed(inverseComparison);
        }

        // build a user message
        List<string> messages = [];

        if (forwardComparison.Relationship == CMR.Equivalent)
        {
            messages.Add($"FHIR {sourcePackage.ShortName}:{sourceVs.Name} (`{sourceVs.VersionedUrl}`)" +
                $" maps to {targetPackage.ShortName}:{targetVs.Name} (`{targetVs.VersionedUrl}`) as {forwardComparison.Relationship}.");
        }
        else
        {
            messages.Add($"FHIR {sourcePackage.ShortName}:{sourceVs.Name} (`{sourceVs.VersionedUrl}`)" +
                $" maps to {targetPackage.ShortName}:{targetVs.Name} (`{targetVs.VersionedUrl}`) as {forwardComparison.Relationship} based on the concept comparisons:");

            ILookup<int, DbValueSetConceptComparison> conceptComparisonsBySourceKey = vsConceptComparisons.ToLookup(c => c.SourceConceptKey);

            foreach (DbValueSetConcept concept in DbValueSetConcept.SelectList(_db, ValueSetKey: sourceVs.Key, Inactive: false, Abstract: false))
            {
                if (!conceptComparisonsBySourceKey.Contains(concept.Key))
                {
                    messages.Add($"* `{concept.System}`#`{concept.Code}` has no comparison into `{targetVs.VersionedUrl}`");
                    continue;
                }

                foreach (DbValueSetConceptComparison conceptComparison in conceptComparisonsBySourceKey[concept.Key])
                {
                    if (conceptComparison.Relationship == CMR.Equivalent)
                    {
                        continue;
                    }

                    if (conceptComparison.UserMessage != null)
                    {
                        messages.Add("* " + conceptComparison.UserMessage);
                    }
                    else if (DbValueSetConcept.SelectSingle(_db, Key: conceptComparison.TargetConceptKey) is DbValueSetConcept targetConcept)
                    {
                        messages.Add($"* `{concept.System}`#`{concept.Code}` maps to `{targetConcept.System}`#`{targetConcept.Code}` as {conceptComparison.Relationship}.");
                    }
                    else
                    {
                        messages.Add($"* `{concept.System}`#`{concept.Code}` has no target concept.");
                    }
                }
            }
        }

        forwardComparison.UserMessage = string.Join("\n", messages);
        vsComparisonCache.Changed(forwardComparison);
    }


    private void doValueSetComparisons(
        DbFhirPackage sourcePackage,
        DbValueSet sourceVs,
        DbFhirPackage targetPackage,
        DbFhirPackageComparisonPair forwardPair,
        DbFhirPackageComparisonPair reversePair)
    {
        // check for existing comparisons
        List<DbValueSetComparison> forwardComparisons = _vsComparisonCache.ForSource(sourceVs.Key)
            .Where(c => c.TargetFhirPackageKey == targetPackage.Key)
            .ToList();

        List<DbValueSetComparison> existingDbComparisons = DbValueSetComparison.SelectList(
            _db,
            PackageComparisonKey: forwardPair.Key,
            SourceFhirPackageKey: sourcePackage.Key,
            TargetFhirPackageKey: targetPackage.Key,
            SourceValueSetKey: sourceVs.Key);

        foreach (DbValueSetComparison c in existingDbComparisons)
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
            _logger.LogInformation($"No forward comparisons found for {sourcePackage.ShortName}:{sourceVs.Name} (`{sourceVs.VersionedUrl}`) to {targetPackage.ShortName}");
            return;
        }

        // iterate across the forward comparisons
        foreach (DbValueSetComparison forwardComparison in forwardComparisons)
        {
            if (forwardComparison.LastReviewedOn != null)
            {
                continue;
            }

            // get the target value set for this comparison
            DbValueSet targetVs = DbValueSet.SelectSingle(_db, Key: forwardComparison.TargetValueSetKey)
                ?? throw new Exception($"Could not resolve target ValueSet with Key: {forwardComparison.TargetValueSetKey} (`{forwardComparison.TargetCanonicalVersioned}`)");

            DoValueSetComparison(
                _vsComparisonCache,
                _conceptComparisonCache,
                sourcePackage,
                sourceVs,
                targetPackage,
                targetVs,
                forwardComparison,
                forwardPair,
                reversePair);
        }

        return;
    }

    public bool AggregateValueSetRelationships(DbValueSetComparison vsComparison)
    {
        DbValueSet? sourceVs = DbValueSet.SelectSingle(_db, Key: vsComparison.SourceValueSetKey);
        DbValueSet? targetVs = DbValueSet.SelectSingle(_db, Key: vsComparison.TargetValueSetKey);

        if ((sourceVs == null) || (targetVs == null))
        {
            throw new Exception($"Failed to resolve source or target value set for comparison {vsComparison.Key}");
        }

        return aggregateValueSetRelationships(vsComparison, sourceVs!, targetVs!);
    }


    /// <summary>
    /// Aggregates the relationships of value sets within a FHIR package comparison.
    /// </summary>
    /// <param name="vsComparison">The comparison object for the value set.</param>
    /// <returns>True if the relationship was updated, otherwise false.</returns>
    private bool aggregateValueSetRelationships(
        DbValueSetComparison vsComparison,
        DbValueSet sourceVs,
        DbValueSet targetVs,
        DbComparisonCache<DbValueSetConceptComparison>? conceptComparisonCache = null)
    {
        List<DbValueSetConceptComparison> conceptComparisons = conceptComparisonCache?.Values
            .Where(c => (c.SourceValueSetKey == sourceVs.Key) && (c.TargetValueSetKey == targetVs.Key))
            .ToList()
            ?? [];

        List<DbValueSetConceptComparison> existingDbComparisons = DbValueSetConceptComparison.SelectList(
            _db,
            ValueSetComparisonKey: vsComparison.Key);

        // look at the database for exising comparisons
        foreach (DbValueSetConceptComparison existingDbComparison in existingDbComparisons)
        {
            if (conceptComparisons.Contains(existingDbComparison))
            {
                continue;
            }
            conceptComparisons.Add(existingDbComparison);
        }

        List<CMR?> relationships = conceptComparisons.Select(c => c.Relationship).Distinct().ToList();

        // check for no relationships
        if (relationships.Count == 0)
        {
            // don't change anything
            return false;
        }

        // get an initial guess based on the number of concepts on each side
        CMR? conceptCountRelationship = RelationshipForCounts(sourceVs.ActiveConcreteConceptCount, targetVs.ActiveConcreteConceptCount);

        CMR? r;

        // check for all the same relationship
        if (relationships.Count == 1)
        {
            r = applyRelationship(relationships[0], conceptCountRelationship);

            if (vsComparison.Relationship == r)
            {
                return false;
            }

            vsComparison.Relationship = r;
            return true;
        }

        bool hasNoMaps = relationships.Any(r => r == null);

        // use an existing relationship if we have one, otherwise assume broader if there are non-mapping relationships or equivalent if not
        r = vsComparison.Relationship ?? (hasNoMaps ? CMR.SourceIsBroaderThanTarget : CMR.Equivalent);

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

        applyRelationship(r, conceptCountRelationship);

        if (vsComparison.Relationship == r)
        {
            return false;
        }

        vsComparison.Relationship = r;
        return true;
    }


    public void ApplyVsConceptChanges(
        List<DbGraphVs.DbVsConceptRow> originalProjection,
        IEnumerable<DbGraphVs.DbVsConceptRow> updatedProjection,
        int sourceColumnIndex,
        bool isComparingRight,
        string? reviewer)
    {
        DbComparisonCache<DbValueSetConceptComparison> conceptComparisonCache = new();

        // traverse the locally-modified concept projection and determine changes (add/remove/update)
        foreach (DbGraphVs.DbVsConceptRow row in updatedProjection)
        {
            if (row[sourceColumnIndex] == null)
            {
                continue;
            }

            DbGraphVs.DbVsConceptCell sourceConceptCell = row[sourceColumnIndex]!;
            DbValueSetConceptComparison sourceToTargetComparison = isComparingRight
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
                sourceToTargetComparison.Key = DbValueSetConceptComparison.GetIndex();

                // cache the addition
                conceptComparisonCache.CacheAdd(sourceToTargetComparison);
            }
            else
            {
                // cache as an update
                conceptComparisonCache.CacheUpdate(sourceToTargetComparison);
            }
        }

        // check for deleted rows
        ILookup<Guid, DbGraphVs.DbVsConceptRow> currentConceptRows = updatedProjection.ToLookup(c => c.RowId);
        foreach (DbGraphVs.DbVsConceptRow row in originalProjection)
        {
            if (!currentConceptRows.Contains(row.RowId))
            {
                DbGraphVs.DbVsConceptCell sourceConceptCell = row[sourceColumnIndex]!;
                DbValueSetConceptComparison sourceToTargetComparison = isComparingRight
                    ? sourceConceptCell.RightComparison!
                    : sourceConceptCell.LeftComparison!;
                // cache the deletion
                conceptComparisonCache.CacheDelete(sourceToTargetComparison);
            }
        }

        // apply the changes to the concept comparisons
        conceptComparisonCache.ToAdd.Insert(_db, insertPrimaryKey: true);
        conceptComparisonCache.ToUpdate.Update(_db);
        conceptComparisonCache.ToDelete.Delete(_db);
    }

    public void MarkVsMappingsReviewed(
        DbGraphVs.DbVsRow vsRow,
        List<DbGraphVs.DbVsConceptRow> conceptProjection,
        string? reviewer)
    {
        DbComparisonCache<DbValueSetComparison> vsComparisonCache = new();
        DbComparisonCache<DbValueSetConceptComparison> conceptComparisonCache = new();

        // iterate over each of the structure cells
        foreach (DbGraphVs.DbVsCell? vsCell in vsRow.Cells)
        {
            if (vsCell == null)
            {
                continue;
            }

            // check for a left comparison
            if (vsCell.LeftComparison != null)
            {
                vsCell.LeftComparison.LastReviewedOn = DateTime.UtcNow;
                vsCell.LeftComparison.LastReviewedBy = reviewer;
                vsComparisonCache.CacheUpdate(vsCell.LeftComparison);
            }

            // check for a right comparison
            if (vsCell.RightComparison != null)
            {
                vsCell.RightComparison.LastReviewedOn = DateTime.UtcNow;
                vsCell.RightComparison.LastReviewedBy = reviewer;
                vsComparisonCache.CacheUpdate(vsCell.RightComparison);
            }
        }

        // traverse the concept projection
        foreach (DbGraphVs.DbVsConceptRow conceptRow in conceptProjection)
        {
            // iterate over the cells in the row
            foreach (DbGraphVs.DbVsConceptCell? conceptCell in conceptRow.Cells)
            {
                if (conceptCell == null)
                {
                    continue;
                }

                // check for a left comparison
                if (conceptCell.LeftComparison != null)
                {
                    conceptCell.LeftComparison.LastReviewedOn = DateTime.UtcNow;
                    conceptCell.LeftComparison.LastReviewedBy = reviewer;
                    conceptComparisonCache.CacheUpdate(conceptCell.LeftComparison);
                }

                // check for a right comparison
                if (conceptCell.RightComparison != null)
                {
                    conceptCell.RightComparison.LastReviewedOn = DateTime.UtcNow;
                    conceptCell.RightComparison.LastReviewedBy = reviewer;
                    conceptComparisonCache.CacheUpdate(conceptCell.RightComparison);
                }
            }
        }

        // apply the changes to the structure comparisons
        if (vsComparisonCache.ToAdd.Any())
        {
            vsComparisonCache.ToAdd.Insert(_db, insertPrimaryKey: true);
        }

        if (vsComparisonCache.ToUpdate.Any())
        {
            vsComparisonCache.ToUpdate.Update(_db);
        }

        if (vsComparisonCache.ToDelete.Any())
        {
            vsComparisonCache.ToDelete.Delete(_db);
        }

        // apply the changes to the element comparisons
        if (conceptComparisonCache.ToAdd.Any())
        {
            conceptComparisonCache.ToAdd.Insert(_db, insertPrimaryKey: true);
        }

        if (conceptComparisonCache.ToUpdate.Any())
        {
            conceptComparisonCache.ToUpdate.Update(_db);
        }

        if (conceptComparisonCache.ToDelete.Any())
        {
            conceptComparisonCache.ToDelete.Delete(_db);
        }
    }

    public (DbValueSetComparison? inverted, bool? changed) UpdateInversion(
        DbValueSetComparison forwardComparison,
        DbValueSetComparison? inverseComparsion = null)
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
            inverseComparsion = DbValueSetComparison.SelectSingle(
                _db,
                Key: forwardComparison.InverseComparisonKey);
        }

        if (inverseComparsion == null)
        {
            // create an inverse comparison
            inverseComparsion = invert(forwardComparison, inversePackagePair);
            addedInverse = true;
        }

        DbComparisonCache<DbValueSetConceptComparison> conceptComparisonCache = new();

        // get the source concepts
        Dictionary<int, DbValueSetConcept> sourceConcepts = DbValueSetConcept.SelectDict(
            _db,
            ValueSetKey: forwardComparison.SourceValueSetKey,
            Inactive: false,
            Abstract: false);

        // get the target concepts
        Dictionary<int, DbValueSetConcept> targetConcepts = DbValueSetConcept.SelectDict(
            _db,
            ValueSetKey: forwardComparison.TargetValueSetKey,
            Inactive: false,
            Abstract: false);

        // get the list of forward concept comparisons
        List<DbValueSetConceptComparison> forwardConceptComparisons = DbValueSetConceptComparison.SelectList(
            _db,
            ValueSetComparisonKey: forwardComparison.Key,
            SourceValueSetKey: forwardComparison.SourceValueSetKey,
            TargetFhirPackageKey: forwardComparison.TargetFhirPackageKey);

        // get the list of existing inverse concept comparisons
        Dictionary<int, DbValueSetConceptComparison> existingInverseConceptComparisons = DbValueSetConceptComparison.SelectDict(
            _db,
            ValueSetComparisonKey: inverseComparsion.Key,
            SourceValueSetKey: inverseComparsion.SourceValueSetKey,
            TargetFhirPackageKey: inverseComparsion.TargetFhirPackageKey);

        HashSet<int> usedInverseKeys = [];

        List<DbValueSetConceptComparison> invertedComparisons = [];
        foreach (DbValueSetConceptComparison forwardConceptComparison in forwardConceptComparisons)
        {
            if ((forwardConceptComparison.NoMap == true) ||
                (forwardConceptComparison.TargetConceptKey == null))
            {
                continue;
            }

            // create a new inverse comparison
            DbValueSetConceptComparison computedInverse = invert(
                forwardConceptComparison,
                sourceConcepts[forwardConceptComparison.SourceConceptKey],
                targetConcepts[(int)forwardConceptComparison.TargetConceptKey],
                inverseComparsion,
                inversePackagePair);

            if ((forwardConceptComparison.InverseComparisonKey == null) ||
                (!existingInverseConceptComparisons.TryGetValue((int)forwardConceptComparison.InverseComparisonKey, out DbValueSetConceptComparison? existingInverse)))
            {
                usedInverseKeys.Add(computedInverse.Key);
                conceptComparisonCache.CacheAdd(computedInverse);

                if (forwardConceptComparison.InverseComparisonKey != computedInverse.Key)
                {
                    forwardConceptComparison.InverseComparisonKey = computedInverse.Key;
                    conceptComparisonCache.CacheUpdate(forwardConceptComparison);
                }

                continue;
            }

            usedInverseKeys.Add(existingInverse.Key);

            // check to see if the inverse comparison has the same relationship
            if (existingInverse.Relationship != computedInverse.Relationship)
            {
                existingInverse.Relationship = computedInverse.Relationship;
                conceptComparisonCache.CacheUpdate(existingInverse);
            }
        }

        // flag we are deleting any inverse comparisons that are not used
        foreach ((int key, DbValueSetConceptComparison existing) in existingInverseConceptComparisons)
        {
            if (usedInverseKeys.Contains(key))
            {
                continue;
            }
            conceptComparisonCache.CacheDelete(existing);
        }

        // apply inverse updates to the VS
        if (addedInverse)
        {
            inverseComparsion.Insert(_db, insertPrimaryKey: true);
        }
        else
        {
            inverseComparsion.Update(_db);
        }

        // apply our concept changes
        conceptComparisonCache.ToAdd.Insert(_db, insertPrimaryKey: true);
        conceptComparisonCache.ToUpdate.Update(_db);
        conceptComparisonCache.ToDelete.Delete(_db);

        // return the comparison in case the caller needs it
        return (inverseComparsion, conceptComparisonCache.Count != 0);
    }


    private DbValueSetComparison invert(
        DbValueSetComparison other,
        DbFhirPackageComparisonPair reversePair)
    {
        DbFhirPackage? iSourcePackage = DbFhirPackage.SelectSingle(_db, Key: other.TargetFhirPackageKey);
        DbFhirPackage? iTargetPackage = DbFhirPackage.SelectSingle(_db, Key: other.SourceFhirPackageKey);
        DbValueSet? iSourceVs = DbValueSet.SelectSingle(_db, Key: other.TargetValueSetKey);
        DbValueSet? iTargetVs = DbValueSet.SelectSingle(_db, Key: other.SourceValueSetKey);

        return new()
        {
            Key = DbValueSetComparison.GetIndex(),
            InverseComparisonKey = other.Key,
            PackageComparisonKey = reversePair.Key,
            SourceFhirPackageKey = other.TargetFhirPackageKey,
            TargetFhirPackageKey = other.SourceFhirPackageKey,
            SourceValueSetKey = other.TargetValueSetKey!.Value,
            SourceCanonicalVersioned = other.TargetCanonicalVersioned!,
            SourceCanonicalUnversioned = other.TargetCanonicalUnversioned!,
            SourceVersion = other.TargetVersion!,
            SourceName = other.TargetName!,
            TargetValueSetKey = other.SourceValueSetKey,
            TargetCanonicalVersioned = other.SourceCanonicalVersioned,
            TargetCanonicalUnversioned = other.SourceCanonicalUnversioned,
            TargetVersion = other.SourceVersion,
            TargetName = other.SourceName,
            CompositeName = ComparisonDatabase.GetCompositeName(reversePair.SourcePackageShortName, other.TargetName!, reversePair.TargetPackageShortName, other.SourceName),
            SourceConceptMapUrl = null,
            SourceConceptMapAdditionalUrls = null,
            Relationship = invert(other.Relationship),
            IsGenerated = true,
            LastReviewedBy = null,
            LastReviewedOn = null,
            TechnicalMessage = $"Mapping was inverted from ValueSet comparison {other.Key} of {other.SourceCanonicalVersioned} -> {other.TargetCanonicalVersioned}",
            UserMessage = $"Mapping from FHIR {iSourcePackage?.ShortName}:{iSourceVs?.Name} (`{iSourceVs?.VersionedUrl}`) " +
                $"to FHIR {iTargetPackage?.ShortName}:{iTargetVs?.Name} (`{iTargetVs?.VersionedUrl}`)",
            IsIdentical = other.IsIdentical,
            CodeLiteralsAreIdentical = other.CodeLiteralsAreIdentical,
        };
    }

    private DbValueSetConceptComparison invert(
        DbValueSetConceptComparison other,
        DbValueSetConcept otherSourceConcept,
        DbValueSetConcept otherTargetConcept,
        DbValueSetComparison reverseCanonicalComparison,
        DbFhirPackageComparisonPair reversePair)
    {
        DbFhirPackage? iSourcePackage = DbFhirPackage.SelectSingle(_db, Key: other.TargetFhirPackageKey);
        DbFhirPackage? iTargetPackage = DbFhirPackage.SelectSingle(_db, Key: other.SourceFhirPackageKey);
        DbValueSet? iSourceVs = DbValueSet.SelectSingle(_db, Key: other.TargetValueSetKey);
        DbValueSet? iTargetVs = DbValueSet.SelectSingle(_db, Key: other.SourceValueSetKey);
        DbValueSetConcept? iSourceConcept = DbValueSetConcept.SelectSingle(_db, Key: other.TargetConceptKey);
        DbValueSetConcept? iTargetConcept = DbValueSetConcept.SelectSingle(_db, Key: other.SourceConceptKey);

        return new()
        {
            Key = DbValueSetComparison.GetIndex(),
            InverseComparisonKey = other.Key,
            PackageComparisonKey = reversePair.Key,
            SourceFhirPackageKey = other.TargetFhirPackageKey,
            TargetFhirPackageKey = other.SourceFhirPackageKey,
            SourceValueSetKey = other.TargetValueSetKey!.Value,
            TargetValueSetKey = other.SourceValueSetKey,
            ValueSetComparisonKey = reverseCanonicalComparison.Key,
            SourceConceptKey = other.TargetConceptKey!.Value,
            TargetConceptKey = other.SourceConceptKey,
            Relationship = invert(other.Relationship),
            NoMap = false,
            IsGenerated = true,
            LastReviewedBy = null,
            LastReviewedOn = null,
            TechnicalMessage = $"Mapping was inverted from ValueSet Concept comparison {other.Key}" +
                $" of `{otherSourceConcept.System}#{otherSourceConcept.Code}` ->" +
                $" `{otherTargetConcept.System}#{otherTargetConcept.Code}`",
            UserMessage = $"`{iSourceConcept?.System}`#`{iSourceConcept?.Code}`" +
                $" maps to FHIR `{iTargetConcept?.System}`#`{iTargetConcept?.Code}`",
            IsIdentical = other.IsIdentical,
            CodeLiteralsAreIdentical = other.CodeLiteralsAreIdentical,
        };
    }
#endif
}
