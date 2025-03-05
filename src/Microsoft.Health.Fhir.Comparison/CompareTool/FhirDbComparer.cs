using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Health.Fhir.Comparison.Models;
using Microsoft.Health.Fhir.Comparison.XVer;
using CMR = Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship;

namespace Microsoft.Health.Fhir.Comparison.CompareTool;

public class FhirDbComparer
{
    private readonly ComparisonDatabase _comparisonDb;
    private readonly IDbConnection _db;

    private ILoggerFactory _loggerFactory;
    private ILogger _logger;


    public FhirDbComparer(
        ComparisonDatabase db,
        ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
        _logger = loggerFactory.CreateLogger<FhirDbComparer>();

        _comparisonDb = db;
        _db = db.DbConnection;
    }

    public void RunAllComparisons()
    {
        Dictionary<int, DbFhirPackage> packages = DbFhirPackage.SelectList(_db).ToDictionary(p => p.Key);

        // iterate over each FHIR Package we have
        foreach (DbFhirPackage sourcePackage in packages.Values)
        {
            _logger.LogInformation($"Processing source package {sourcePackage.Key}: {sourcePackage.PackageId}@{sourcePackage.PackageVersion}");
            
            List<(DbFhirPackageComparisonPair forward, DbFhirPackageComparisonPair reverse)> bidirectionalPairs = [];

            foreach (DbFhirPackageComparisonPair pf in DbFhirPackageComparisonPair.SelectList(_db, SourcePackageKey: sourcePackage.Key))
            {
                DbFhirPackageComparisonPair? reverse = DbFhirPackageComparisonPair.SelectSingle(
                    _db,
                    SourcePackageKey: pf.TargetPackageKey,
                    TargetPackageKey: pf.SourcePackageKey);

                if (reverse != null)
                {
                    bidirectionalPairs.Add((pf, reverse));
                    continue;
                }

                reverse = invert(pf);
                reverse.Insert(_db);
                bidirectionalPairs.Add((pf, reverse));
            }

            // consistency check
            if (bidirectionalPairs.Any(biPair => !packages.ContainsKey(biPair.forward.TargetPackageKey)))
            {
                throw new Exception("Failed to resolve packages in all pairwise comparisons!");
            }

            List<DbValueSet> valueSets = DbValueSet.SelectList(_db, FhirPackageKey: sourcePackage.Key, StrongestBindingCore: Hl7.Fhir.Model.BindingStrength.Required);
            _logger.LogInformation($" <<< processing ValueSets with required bindings, count: {valueSets.Count}");

            // iterate over value sets in the package
            foreach (DbValueSet sourceVs in valueSets)
            {
                _logger.LogInformation($" <<< processing ValueSet {sourceVs.VersionedUrl}");

                // iterate over the comparison pairs
                foreach ((DbFhirPackageComparisonPair forward, DbFhirPackageComparisonPair reverse) in bidirectionalPairs)
                {
                    // grab our target package
                    DbFhirPackage targetPackage = packages[forward.TargetPackageKey];

                    doValueSetComparisons(sourcePackage, sourceVs, targetPackage, forward, reverse);
                }
            }
        }

        return;
    }


    private void doValueSetComparisons(
        DbFhirPackage sourcePackage,
        DbValueSet sourceVs,
        DbFhirPackage targetPackage,
        DbFhirPackageComparisonPair forwardPair,
        DbFhirPackageComparisonPair reversePair)
    {
        Dictionary<int, DbValueSetComparison> comparisonsToAdd = [];
        Dictionary<int, DbValueSetComparison> comparisonsToUpdate = [];

        // check for a existing comparisons
        List<DbValueSetComparison> forwardComparisons = DbValueSetComparison.SelectList(
            _db,
            PackageComparisonKey: forwardPair.Key,
            SourceFhirPackageKey: sourcePackage.Key,
            TargetFhirPackageKey: targetPackage.Key,
            SourceValueSetKey: sourceVs.Key);

        // if there are none, see if we can find an equivalent value set to compare with in the target
        if (forwardComparisons.Count == 0)
        {
            string message = "Inferred comparison based on ";

            List<DbValueSet> potentialTargets = DbValueSet.SelectList(_db, FhirPackageKey: targetPackage.Key, UnversionedUrl: sourceVs.UnversionedUrl);
            if (potentialTargets.Count != 0)
            {
                message += $" unversioned URL match from source: `{sourceVs.UnversionedUrl}`";
            }
            else
            {
                potentialTargets = DbValueSet.SelectList(_db, FhirPackageKey: targetPackage.Key, Name: sourceVs.Name);

                if (potentialTargets.Count != 0)
                {
                    message += $" Name match from source: `{sourceVs.Name}`";
                }
                else
                {
                    potentialTargets = DbValueSet.SelectList(_db, FhirPackageKey: targetPackage.Key, Id: sourceVs.Id);

                    if (potentialTargets.Count != 0)
                    {
                        message += $" Id match from source: {sourceVs.Id}";
                    }
                }
            }

            foreach (DbValueSet targetVs in potentialTargets)
            {
                // create this comparison
                DbValueSetComparison vsc = new()
                {
                    Key = _comparisonDb.GetValueSetComparisonKey(),
                    PackageComparisonKey = forwardPair.Key,
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
                    Message = message,
                };

                comparisonsToAdd[vsc.Key] = vsc;
                forwardComparisons.Add(vsc);
            }
        }

        // iterate across the forward comparisons
        foreach (DbValueSetComparison forwardComparison in forwardComparisons)
        {
            // get the target value set for this comparison
            DbValueSet targetVs = DbValueSet.SelectSingle(
                _db,
                Key: forwardComparison.TargetValueSetKey)
                ?? throw new Exception($"Could not resolve target ValueSet with Key: {forwardComparison.TargetValueSetKey} (`{forwardComparison.TargetCanonicalVersioned}`)");

            // look for an inverse comparison
            DbValueSetComparison? reverseComparison = DbValueSetComparison.SelectSingle(
                _db,
                PackageComparisonKey: reversePair.Key,
                SourceValueSetKey: forwardComparison.TargetValueSetKey,
                TargetValueSetKey: forwardComparison.SourceValueSetKey);

            if (reverseComparison == null)
            {
                reverseComparison = invert(forwardComparison, reversePair);
                comparisonsToAdd[reverseComparison.Key] = reverseComparison;
            }

            // process this comparison
            doValueSetConceptComparisons(
                sourcePackage,
                sourceVs,
                targetPackage,
                targetVs,
                forwardComparison,
                reverseComparison,
                forwardPair,
                reversePair);

            if (aggregateValueSetRelationships(forwardComparison) &&
                !comparisonsToAdd.ContainsKey(forwardComparison.Key))
            {
                comparisonsToUpdate[forwardComparison.Key] = forwardComparison;
            }

            if (aggregateValueSetRelationships(reverseComparison) &&
                !comparisonsToAdd.ContainsKey(reverseComparison.Key))
            {
                comparisonsToAdd[reverseComparison.Key] = reverseComparison;
            }
        }

        // update the database
        comparisonsToAdd.Values.ToList().Insert(_db);
        comparisonsToUpdate.Values.ToList().Update(_db);

        return;
    }

    /// <summary>
    /// Aggregates the relationships of value sets within a FHIR package comparison.
    /// </summary>
    /// <param name="vsComparsion">The comparison object for the value set.</param>
    /// <returns>True if the relationship was updated, otherwise false.</returns>
    private bool aggregateValueSetRelationships(DbValueSetComparison vsComparsion)
    {
        List<DbValueSetConceptComparison> conceptComparisons = DbValueSetConceptComparison.SelectList(_db, ValueSetComparisonKey: vsComparsion.Key);
        List<CMR?> relationships = conceptComparisons.Select(c => c.Relationship).Distinct().ToList();

        // check for no relationships
        if (relationships.Count == 0)
        {
            // don't change anything
            return false;
        }

        // check for all the same relationship
        if (relationships.Count == 1)
        {
            if (vsComparsion.Relationship == relationships[0])
            {
                return false;
            }

            vsComparsion.Relationship = relationships[0];
            return true;
        }

        bool hasNoMaps = relationships.Any(r => r == null);

        // use an existing relationship if we have one, otherwise assume broader if there are non-mapping relationships or equivalent if not
        CMR? r = vsComparsion.Relationship ?? (hasNoMaps ? CMR.SourceIsBroaderThanTarget : CMR.Equivalent);

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

        if (vsComparsion.Relationship == r)
        {
            return false;
        }

        vsComparsion.Relationship = r;
        return true;
    }

    private CMR applyRelationship(CMR? existing, CMR? change) => existing switch
    {
        CMR.Equivalent => change ?? CMR.Equivalent,
        CMR.RelatedTo => (change == CMR.NotRelatedTo) ? CMR.NotRelatedTo : CMR.RelatedTo,
        CMR.SourceIsNarrowerThanTarget => (change == CMR.SourceIsNarrowerThanTarget || change == CMR.Equivalent)
            ? CMR.SourceIsNarrowerThanTarget : CMR.RelatedTo,
        CMR.SourceIsBroaderThanTarget => (change == CMR.SourceIsBroaderThanTarget || change == CMR.Equivalent)
            ? CMR.SourceIsBroaderThanTarget : CMR.RelatedTo,
        CMR.NotRelatedTo => change ?? CMR.NotRelatedTo,
        _ => change ?? existing ?? CMR.NotRelatedTo,
    };

    private void doValueSetConceptComparisons(
        DbFhirPackage sourcePackage,
        DbValueSet sourceVs,
        DbFhirPackage targetPackage,
        DbValueSet targetVs,
        DbValueSetComparison forwardComparison,
        DbValueSetComparison reverseComparison,
        DbFhirPackageComparisonPair forwardPair,
        DbFhirPackageComparisonPair reversePair)
    {
        Dictionary<int, DbValueSetConceptComparison> comparisonsToAdd = [];
        Dictionary<int, DbValueSetConceptComparison> comparisonsToUpdate = [];

        List<DbValueSetConcept> sourceConcepts = DbValueSetConcept.SelectList(_db, ValueSetKey: sourceVs.Key);

        // iterate over the source concepts to ensure every concept has a 
        foreach (DbValueSetConcept sourceConcept in sourceConcepts)
        {
            // check for existing comparisons for this concept
            List<DbValueSetConceptComparison> comparisons = DbValueSetConceptComparison.SelectList(
                _db,
                ValueSetComparisonKey: forwardComparison.Key,
                SourceValueSetKey: sourceVs.Key,
                SourceConceptKey: sourceConcept.Key,
                TargetFhirPackageKey: targetPackage.Key);

            // if there are no existing comparisons, see if we can find a matching concept
            if ((comparisons.Count == 0) &&
                (DbValueSetConcept.SelectList(_db, ValueSetKey: targetVs.Key, Code: sourceConcept.Code) is List<DbValueSetConcept> targetConcepts) &&
                (targetConcepts.Count != 0))
            {

                // if there is more than one, see if there is an exact match on systems
                if ((targetConcepts.Count > 1) &&
                    (targetConcepts.FirstOrDefault(tc => tc.System == sourceConcept.System) is DbValueSetConcept exact))
                {
                    // only use the exact match
                    targetConcepts = [exact];
                }

                CMR relationship = (targetConcepts.Count == 1)
                    ? CMR.Equivalent
                    : CMR.SourceIsBroaderThanTarget;

                // iterate over the possible targets
                foreach (DbValueSetConcept targetConcept in targetConcepts)
                {
                    DbValueSetConceptComparison comp = new()
                    {
                        Key = _comparisonDb.GetConceptComparisonKey(),
                        PackageComparisonKey = forwardPair.Key,
                        ValueSetComparisonKey = forwardComparison.Key,
                        SourceFhirPackageKey = forwardPair.SourcePackageKey,
                        SourceValueSetKey = forwardComparison.SourceValueSetKey,
                        SourceConceptKey = sourceConcept.Key,
                        TargetFhirPackageKey = forwardPair.TargetPackageKey,
                        TargetValueSetKey = forwardComparison.TargetValueSetKey,
                        TargetConceptKey = targetConcept.Key,
                        Relationship = relationship,
                        NoMap = false,
                        Message = $"Created mapping based on literal match of code `{sourceConcept.Code}`",
                        IsGenerated = true,
                        LastReviewedBy = null,
                        LastReviewedOn = null,
                    };

                    comparisonsToAdd.Add(comp.Key, comp);
                    comparisons.Add(comp);
                }
            }

            // if there are still no comparisons, add a no-map
            if (comparisons.Count == 0)
            {
                DbValueSetConceptComparison noMap = new()
                {
                    Key = _comparisonDb.GetConceptComparisonKey(),
                    PackageComparisonKey = forwardPair.Key,
                    ValueSetComparisonKey = forwardComparison.Key,
                    SourceFhirPackageKey = forwardPair.SourcePackageKey,
                    SourceValueSetKey = forwardComparison.SourceValueSetKey,
                    SourceConceptKey = sourceConcept.Key,
                    TargetFhirPackageKey = forwardPair.TargetPackageKey,
                    TargetValueSetKey = forwardComparison.TargetValueSetKey,
                    TargetConceptKey = null,
                    Relationship = null,
                    NoMap = true,
                    Message = $"No mapping exists and no literal match found - created no-map entry for `{sourceVs.VersionedUrl}`#`{sourceConcept.Code}`",
                    IsGenerated = true,
                    LastReviewedBy = null,
                    LastReviewedOn = null,
                };

                // insert into the database
                comparisonsToAdd.Add(noMap.Key, noMap);

                // nothing else to do on this pass
                continue;
            }

            // iterate over the comparisons to check relationships
            foreach (DbValueSetConceptComparison conceptComparison in comparisons)
            {
                DbValueSetConcept? targetConcept = (conceptComparison.TargetConceptKey == null)
                    ? null
                    : DbValueSetConcept.SelectSingle(_db, Key: conceptComparison.TargetConceptKey);

                DbValueSetConceptComparison? inverseComparison = (conceptComparison.TargetConceptKey == null)
                    ? null
                    : DbValueSetConceptComparison.SelectSingle(
                        _db,
                        ValueSetComparisonKey: reverseComparison.Key,
                        SourceConceptKey: conceptComparison.TargetConceptKey,
                        TargetConceptKey: conceptComparison.SourceConceptKey);

                bool needsUpdate = false;

                // do basic checks if this has not been reviewed
                if (conceptComparison.LastReviewedOn == null)
                {
                    // check for missing no-map value
                    if ((conceptComparison.TargetConceptKey == null) &&
                        (conceptComparison.NoMap != true))
                    {
                        needsUpdate = true;
                        conceptComparison.NoMap = true;
                    }

                    // check for incorrectly-flagged-as-equivalent escape-value code mappings
                    if ((targetConcept != null) &&
                        (conceptComparison.Relationship == CMR.Equivalent) &&
                        XVerProcessor._escapeValveCodes.Contains(sourceConcept.Code) &&
                        (sourceVs.ConceptCount != targetVs.ConceptCount))
                    {
                        needsUpdate = true;
                        conceptComparison.Relationship = (sourceVs.ConceptCount > targetVs.ConceptCount)
                            ? CMR.SourceIsNarrowerThanTarget
                            : CMR.SourceIsBroaderThanTarget;
                        conceptComparison.IsGenerated = true;
                        conceptComparison.Message = conceptComparison.Message +
                            $" Escape-valve code `{sourceConcept.Code}` maps to `{targetConcept.Code}`, but represent different concept domains (different number of codes).";
                    }

                    // check for a single source with multiple targets and any that map as equivalent
                    if ((conceptComparison.Relationship == CMR.Equivalent) &&
                        (comparisons.Count > 1))
                    {
                        needsUpdate = true;

                        // mark as not equivalent
                        conceptComparison.Relationship = CMR.SourceIsBroaderThanTarget;
                        conceptComparison.IsGenerated = true;
                        conceptComparison.Message = conceptComparison.Message +
                            $" `{sourceConcept.Code}` maps to multiple codes in {targetVs.VersionedUrl} and cannot be equivalent.";
                    }
                }

                // if there is no inverse and there should be, create one
                if ((inverseComparison == null) &&
                    (conceptComparison.TargetConceptKey != null))
                {
                    inverseComparison = invert(conceptComparison, sourceConcept, targetConcept!, reverseComparison, reversePair);
                    comparisonsToAdd.Add(inverseComparison.Key, inverseComparison);
                }
                else if (inverseComparison != null)
                {
                    // check to see if the relationship makes sense as an inverse
                    CMR? expected = invert(conceptComparison.Relationship);
                    if ((inverseComparison.LastReviewedOn != null) &&
                        (inverseComparison.Relationship != expected))
                    {
                        inverseComparison.Message = inverseComparison.Message +
                            $" Updated relationship from `{inverseComparison.Relationship}` to `{expected}`" +
                            $" based on the inverse comparsion {conceptComparison.Key}, which has a relationship" +
                            $" of `{conceptComparison.Relationship}`.";
                        inverseComparison.Relationship = expected;

                        if (!comparisonsToAdd.ContainsKey(inverseComparison.Key))
                        {
                            comparisonsToUpdate[inverseComparison.Key] = inverseComparison;
                        }
                    }
                }

                // if any changes have been made and this is not a new record, it needs to be updated
                if (needsUpdate && !comparisonsToAdd.ContainsKey(conceptComparison.Key))
                {
                    comparisonsToUpdate[conceptComparison.Key] = conceptComparison;
                }
            }
        }

        // update the database
        if (comparisonsToAdd.Count != 0)
        {
            comparisonsToAdd.Values.ToList().Insert(_db);
        }

        if (comparisonsToUpdate.Count != 0)
        {
            comparisonsToUpdate.Values.ToList().Update(_db);
        }

        return;

    }

    private DbFhirPackageComparisonPair invert(DbFhirPackageComparisonPair other)
    {
        return new()
        {
            SourcePackageKey = other.TargetPackageKey,
            SourcePackageShortName = other.TargetPackageShortName,
            TargetPackageKey = other.SourcePackageKey,
            TargetPackageShortName = other.SourcePackageShortName,
        };
    }

    private DbValueSetComparison invert(
        DbValueSetComparison other,
        DbFhirPackageComparisonPair reversePair)
    {
        return new()
        {
            Key = _comparisonDb.GetValueSetComparisonKey(),
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
            Message = $"Mapping was inverted from ValueSet comparison {other.Key} of {other.SourceCanonicalVersioned} -> {other.TargetCanonicalVersioned}",
        };
    }

    private DbValueSetConceptComparison invert(
        DbValueSetConceptComparison other,
        DbValueSetConcept otherSourceConcept,
        DbValueSetConcept otherTargetConcept,
        DbValueSetComparison reverseCanonicalComparison,
        DbFhirPackageComparisonPair reversePair)
    {
        return new()
        {
            Key = _comparisonDb.GetConceptComparisonKey(),
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
            Message = $"Mapping was inverted from ValueSet Concept comparison {other.Key}" +
                $" of `{otherSourceConcept.System}#{otherSourceConcept.Code}` ->" +
                $" `{otherTargetConcept.System}#{otherTargetConcept.Code}`",
        };
    }

    private CMR? invert(CMR? existing) => existing switch
    {
        CMR.RelatedTo => CMR.RelatedTo,
        CMR.Equivalent => CMR.Equivalent,
        CMR.SourceIsNarrowerThanTarget => CMR.SourceIsBroaderThanTarget,
        CMR.SourceIsBroaderThanTarget => CMR.SourceIsNarrowerThanTarget,
        CMR.NotRelatedTo => CMR.NotRelatedTo,
        _ => null,
    };

}
