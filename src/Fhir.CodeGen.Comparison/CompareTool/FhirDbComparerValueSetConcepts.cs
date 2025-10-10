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

    private List<DbValueSetConceptComparison> doValueSetConceptComparisons(
        DbComparisonCache<DbValueSetConceptComparison> conceptComparisonCache,
        DbFhirPackage sourcePackage,
        DbValueSet sourceVs,
        DbFhirPackage targetPackage,
        DbValueSet targetVs,
        DbValueSetComparison forwardComparison,
        DbValueSetComparison reverseComparison,
        DbFhirPackageComparisonPair forwardPair,
        DbFhirPackageComparisonPair reversePair)
    {
        List<DbValueSetConceptComparison> vsConceptComparisons = [];

        // select only active and concrete concepts
        List<DbValueSetConcept> sourceConcepts = DbValueSetConcept.SelectList(
            _db,
            ValueSetKey: sourceVs.Key,
            Inactive: false,
            Abstract: false);

        foreach (DbValueSetConcept sourceConcept in sourceConcepts)
        {
            // look in our cache for existing comparisons
            List<DbValueSetConceptComparison> comparisons = conceptComparisonCache.ForSource(sourceConcept.Key)
                .Where(c => c.TargetFhirPackageKey == targetPackage.Key)
                .ToList();

            List<DbValueSetConceptComparison> existingDbComparisons = DbValueSetConceptComparison.SelectList(
                _db,
                ValueSetComparisonKey: forwardComparison.Key,
                SourceValueSetKey: sourceVs.Key,
                SourceConceptKey: sourceConcept.Key,
                TargetFhirPackageKey: targetPackage.Key);

            // look at the database for exising comparisons
            foreach (DbValueSetConceptComparison existingDbComparison in existingDbComparisons)
            {
                if (comparisons.Contains(existingDbComparison))
                {
                    continue;
                }
                comparisons.Add(existingDbComparison);
            }

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

                bool isIdentical;
                bool codeIsIdentical;
                CMR relationship;

                if (targetConcepts.Count == 1)
                {
                    relationship = CMR.Equivalent;
                    isIdentical = sourceConcept.FhirKey == targetConcepts[0].FhirKey;
                    codeIsIdentical = sourceConcept.Code == targetConcepts[0].Code;
                }
                else
                {
                    relationship = CMR.SourceIsBroaderThanTarget;
                    isIdentical = false;
                    codeIsIdentical = false;
                }

                // iterate over the possible targets
                foreach (DbValueSetConcept targetConcept in targetConcepts)
                {
                    DbValueSetConceptComparison comp = new()
                    {
                        Key = DbValueSetConceptComparison.GetIndex(),
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
                        TechnicalMessage = $"Created mapping based on literal match of code `{sourceConcept.Code}`",
                        UserMessage = $"`{sourceConcept.System}`#`{sourceConcept.Code}`" +
                            $" maps to {targetPackage.ShortName} `{targetConcept.System}`#`{targetConcept.Code}`",
                        IsGenerated = true,
                        LastReviewedBy = null,
                        LastReviewedOn = null,
                        IsIdentical = isIdentical,
                        CodesAreIdentical = codeIsIdentical,
                    };

                    conceptComparisonCache.CacheAdd(comp);
                    comparisons.Add(comp);
                }
            }

            // if there are still no comparisons, add a no-map
            if (comparisons.Count == 0)
            {
                DbValueSetConceptComparison noMap = new()
                {
                    Key = DbValueSetConceptComparison.GetIndex(),
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
                    TechnicalMessage = $"No mapping exists and no literal match found - created no-map entry for `{sourceVs.VersionedUrl}`#`{sourceConcept.Code}`",
                    UserMessage = $"There is no mapping for FHIR {sourcePackage.ShortName}:{sourceVs.Name} (`{sourceVs.VersionedUrl}`) into FHIR {targetPackage.ShortName}",
                    IsGenerated = true,
                    LastReviewedBy = null,
                    LastReviewedOn = null,
                    IsIdentical = null,
                    CodesAreIdentical = null,
                };

                // insert into the database
                conceptComparisonCache.CacheAdd(noMap);

                // nothing else to do on this pass
                continue;
            }

            // iterate over the comparisons to check relationships
            foreach (DbValueSetConceptComparison conceptComparison in comparisons)
            {
                vsConceptComparisons.Add(conceptComparison);

                DbValueSetConcept? targetConcept = (conceptComparison.TargetConceptKey == null)
                    ? null
                    : DbValueSetConcept.SelectSingle(_db, Key: conceptComparison.TargetConceptKey);

                // if there is no target, there is nothing to check
                if (targetConcept == null)
                {
                    continue;
                }

                // get the list of all comparisons that source this ValueSet and target the same concept
                List<DbValueSetConceptComparison> targetComparisons = conceptComparisonCache.ForSource(targetConcept.Key)
                    .Where(c => c.TargetFhirPackageKey == sourcePackage.Key)
                    .ToList();

                List<DbValueSetConceptComparison> existingTargetComparisons = DbValueSetConceptComparison.SelectList(
                    _db,
                    PackageComparisonKey: forwardPair.Key,
                    SourceValueSetKey: sourceVs.Key,
                    TargetConceptKey: targetConcept.Key);

                targetComparisons.AddRange(existingTargetComparisons.Where(etc => !targetComparisons.Contains(etc)));

                // look for an inverse comparison
                DbValueSetConceptComparison? inverseComparison = null;

                if (conceptComparison.InverseComparisonKey != null)
                {
                    inverseComparison = conceptComparisonCache.Get((int)conceptComparison.InverseComparisonKey) ??
                         DbValueSetConceptComparison.SelectSingle(_db, Key: conceptComparison.InverseComparisonKey);
                }

                if (inverseComparison == null)
                {
                    inverseComparison = conceptComparisonCache.Get(targetConcept.Key, conceptComparison.SourceContentKey)
                        ?? DbValueSetConceptComparison.SelectSingle(
                            _db,
                            PackageComparisonKey: reversePair.Key,
                            SourceFhirPackageKey: targetPackage.Key,
                            SourceValueSetKey: conceptComparison.TargetValueSetKey,
                            SourceConceptKey: conceptComparison.TargetConceptKey,
                            TargetFhirPackageKey: sourcePackage.Key,
                            TargetValueSetKey: conceptComparison.SourceValueSetKey);
                }

                // if there is no inverse and there should be, create one
                if (inverseComparison == null)
                {
                    inverseComparison = invert(conceptComparison, sourceConcept, targetConcept!, reverseComparison, reversePair);
                    conceptComparisonCache.CacheAdd(inverseComparison);

                    conceptComparison.InverseComparisonKey = inverseComparison.Key;
                    conceptComparisonCache.Changed(conceptComparison);
                }

                // do basic checks if this has not been reviewed
                if (conceptComparison.LastReviewedOn == null)
                {
                    bool userMessageIsComplete = false;
                    string userMessage = $"`{sourceConcept.System}`#`{sourceConcept.Code}`" +
                        $" maps to {targetPackage.ShortName} `{targetConcept.System}`#`{targetConcept.Code}`";

                    // check for missing no-map value
                    if ((conceptComparison.TargetConceptKey == null) &&
                        (conceptComparison.NoMap != true))
                    {
                        conceptComparisonCache.Changed(conceptComparison);
                        conceptComparison.NoMap = true;
                    }

                    // check for incorrectly-flagged-as-equivalent escape-value code mappings
                    if ((targetConcept != null) &&
                        (conceptComparison.Relationship == CMR.Equivalent) &&
                        XVerProcessor._escapeValveCodes.Contains(sourceConcept.Code) &&
                        (sourceVs.ActiveConcreteConceptCount != targetVs.ActiveConcreteConceptCount))
                    {
                        conceptComparisonCache.Changed(conceptComparison);
                        conceptComparison.Relationship = RelationshipForCounts(sourceVs.ActiveConcreteConceptCount, targetVs.ActiveConcreteConceptCount);
                        conceptComparison.IsGenerated = true;
                        conceptComparison.TechnicalMessage = conceptComparison.TechnicalMessage +
                            $" Escape-valve code `{sourceConcept.System}`#`{sourceConcept.Code}` maps to `{targetConcept.System}`#`{targetConcept.Code}`, but represent different concept domains (different number of codes).";
                        userMessage = $"Escape-valve code `{sourceConcept.System}`#`{sourceConcept.Code}`" +
                            $" maps to `{targetConcept.System}`#`{targetConcept.Code}`," +
                            $" but represent different concept domains (different number of codes).";
                        userMessageIsComplete = true;
                    }

                    // check for a single source with multiple targets and any that map as equivalent
                    if ((conceptComparison.Relationship == CMR.Equivalent) &&
                        (targetComparisons.Count > 1))
                    {
                        conceptComparisonCache.Changed(conceptComparison);

                        // build a target concept list
                        List<DbValueSetConcept?> targetConceptCodes = targetComparisons
                            .Where(tc => tc.TargetConceptKey != null)
                            .Select(tc => DbValueSetConcept.SelectSingle(_db, Key: tc.TargetConceptKey))
                            .Where(c => c != null)
                            .ToList();

                        // mark as not equivalent
                        conceptComparison.Relationship = CMR.SourceIsBroaderThanTarget;
                        conceptComparison.IsGenerated = true;
                        conceptComparison.TechnicalMessage = conceptComparison.TechnicalMessage +
                            $" `{sourceConcept.Code}` maps to multiple codes in `{targetVs.VersionedUrl}` and cannot be equivalent.";

                        if (!userMessageIsComplete)
                        {
                            userMessage += $", but maps to multiple codes in {targetVs.VersionedUrl}:" +
                                $" ({string.Join(", ", targetConceptCodes.Select(c => $"`{c?.System}`#`{c?.Code}`"))}), so maps as";
                        }
                    }

                    if (!userMessageIsComplete)
                    {
                        userMessage += $" as {conceptComparison.Relationship}.";
                    }

                    conceptComparison.UserMessage = userMessage;
                    conceptComparisonCache.Changed(conceptComparison);
                }

                if (inverseComparison != null)
                {
                    // check to see if the inverted relationship makes sense
                    CMR? expected = invert(conceptComparison.Relationship);
                    if ((inverseComparison.LastReviewedOn == null) &&
                        (inverseComparison.Relationship != expected))
                    {
                        // check for both being reviewed
                        if ((conceptComparison.LastReviewedOn != null) &&
                            (inverseComparison.LastReviewedOn != null))
                        {
                            // cannot update either
                        }
                        // if the inverse has been reviewed, update the current
                        else if (inverseComparison.LastReviewedOn != null)
                        {
                            CMR? updatedCurrentRelationship = invert(inverseComparison.Relationship);

                            conceptComparison.TechnicalMessage = conceptComparison.TechnicalMessage +
                                $" Updated relationship from `{conceptComparison.Relationship}` to `{updatedCurrentRelationship}`" +
                                $" based on the inverse comparsion which has a relationship" +
                                $" of `{inverseComparison.Relationship}`.";
                            conceptComparison.Relationship = updatedCurrentRelationship;
                            conceptComparisonCache.Changed(conceptComparison);
                        }
                        else
                        {
                            // if one is equivalent and the other is not, we want the not-equivalent by default
                            if ((conceptComparison.Relationship == CMR.Equivalent) &&
                                (inverseComparison.Relationship != CMR.Equivalent))
                            {
                                // update the current record
                                CMR? updatedCurrentRelationship = invert(inverseComparison.Relationship);

                                conceptComparison.TechnicalMessage = conceptComparison.TechnicalMessage +
                                    $" Updated relationship from `{conceptComparison.Relationship}` to `{updatedCurrentRelationship}`" +
                                    $" based on the inverse comparsion which has a relationship" +
                                    $" of `{inverseComparison.Relationship}`.";
                                conceptComparison.Relationship = updatedCurrentRelationship;
                                conceptComparisonCache.Changed(conceptComparison);
                            }
                            else if ((conceptComparison.Relationship != CMR.Equivalent) &&
                                     (inverseComparison.Relationship == CMR.Equivalent))
                            {
                                // update the inverse record
                                inverseComparison.TechnicalMessage = inverseComparison.TechnicalMessage +
                                    $" Updated relationship from `{inverseComparison.Relationship}` to `{expected}`" +
                                    $" based on the inverse comparsion which has a relationship" +
                                    $" of `{conceptComparison.Relationship}`.";
                                inverseComparison.Relationship = expected;
                                conceptComparisonCache.Changed(inverseComparison);
                            }

                            // odd relationship - leave as mismatched so that a user will review
                        }

                        //inverseComparison.Message = inverseComparison.Message +
                        //    $" Updated relationship from `{inverseComparison.Relationship}` to `{expected}`" +
                        //    $" based on the inverse comparsion {conceptComparison.Key}, which has a relationship" +
                        //    $" of `{conceptComparison.Relationship}`.";
                        //inverseComparison.Relationship = expected;
                        //conceptComparisons.CacheUpdate(inverseComparison);
                    }
                }
            }
        }

        return vsConceptComparisons;
    }
}
