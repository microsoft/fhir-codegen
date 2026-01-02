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
            // look in our cache for dbRec comparisons
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

            // if there are no dbRec comparisons, see if we can find a matching concept
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
                        CodeLiteralsAreIdentical = codeIsIdentical,
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
                    CodeLiteralsAreIdentical = null,
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

                if ((conceptComparison.TargetValueSetKey is null) ||
                    (conceptComparison.TargetConceptKey is null))
                {
                    // nothing to check
                    continue;
                }

                DbValueSetConcept? targetConcept = DbValueSetConcept.SelectSingle(_db, Key: conceptComparison.TargetConceptKey);
                if (targetConcept is null)
                {
                    throw new Exception($"Unable to locate target concept with Key={conceptComparison.TargetConceptKey}");
                }

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
                    // all unreviewed records are updated in some way (even if just the message)
                    conceptComparisonCache.Changed(conceptComparison);

                    // check for no-map value
                    if (!checkNoMap(sourceConcept, conceptComparison))
                    {
                        string userMessage = $"`{sourceConcept.System}`#`{sourceConcept.Code}` has a mapping" +
                            $" to the ValueSet `{targetVs.VersionedUrl}` (defined by FHIR {targetPackage.ShortName})" +
                            $" for the code `{targetConcept.System}`#`{targetConcept.Code}`";

                        // check escape-valve codes for proper relationships
                        checkEscapeValve(sourceConcept, targetConcept, conceptComparison, ref userMessage);

                        // check to see if there are multiple source codes mapping to the same target code as Equivalent
                        checkMultipleSourcesToTarget(sourceConcept, targetConcept, conceptComparison, ref userMessage);

                        // check to see if there are multiple target codes for the same source code
                        checkMultipleTargets(sourceConcept, targetConcept, conceptComparison, ref userMessage);

                        //if (!userMessageIsComplete)
                        //{
                        //    userMessage += $" as {conceptComparison.Relationship}.";
                        //}

                        conceptComparison.UserMessage = userMessage;
                    }
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

        /// <returns>true if the comparison is a no-map</returns>
        bool checkNoMap(DbValueSetConcept sourceConcept, DbValueSetConceptComparison conceptComparison)
        {
            if (conceptComparison.NoMap == true)
            {
                conceptComparison.Relationship = null;
                conceptComparison.TargetConceptKey = null;

                conceptComparison.UserMessage =
                    $"`{sourceConcept.System}`#`{sourceConcept.Code}` is not mapped to any codes in" +
                    $" the ValueSet `{targetVs.VersionedUrl}` defined in FHIR {targetPackage.ShortName}.";

                return true;
            }

            if (conceptComparison.TargetConceptKey is null)
            {
                conceptComparison.NoMap = true;
                conceptComparison.Relationship = null;

                conceptComparison.UserMessage =
                    $"`{sourceConcept.System}`#`{sourceConcept.Code}` is not mapped to any codes in" +
                    $" the ValueSet `{targetVs.VersionedUrl}` defined in FHIR {targetPackage.ShortName}.";

                return true;
            }

            return false;
        }

        /// <returns>true if the source concept is an escape-valve code</returns>
        bool checkEscapeValve(
            DbValueSetConcept sourceConcept,
            DbValueSetConcept targetConcept,
            DbValueSetConceptComparison conceptComparison,
            ref string userMessage)
        {
            if (!XVerProcessor._escapeValveCodes.Contains(sourceConcept.Code))
            {
                return false;
            }

            bool countsDiffer = sourceVs.ActiveConcreteConceptCount != targetVs.ActiveConcreteConceptCount;

            if (countsDiffer)
            {
                conceptComparison.Relationship = RelationshipForCounts(sourceVs.ActiveConcreteConceptCount, targetVs.ActiveConcreteConceptCount);
                conceptComparison.IsGenerated = true;
                conceptComparison.TechnicalMessage = conceptComparison.TechnicalMessage +
                    $" Escape-valve code `{sourceConcept.System}`#`{sourceConcept.Code}` is mapped to" +
                    $" `{targetConcept.System}`#`{targetConcept.Code}`," +
                    $" but represent different concept domains (different number of codes).";
                userMessage += $" but is an 'escape-valve code'" +
                    $" and the ValueSets have different numbers of codes (represent different concept domains).";
            }
            else
            {
                conceptComparison.Relationship = CMR.Equivalent;
                conceptComparison.IsGenerated = true;
                conceptComparison.TechnicalMessage = conceptComparison.TechnicalMessage +
                    $" Escape-valve code `{sourceConcept.System}`#`{sourceConcept.Code}` is mapped to" +
                    $" `{targetConcept.System}`#`{targetConcept.Code}`" +
                    $" and both ValueSets have the same number of active codes.";
                userMessage += $" and is a possible 'escape-valve code'.";
            }

            return true;
        }

        void checkMultipleSourcesToTarget(
            DbValueSetConcept sourceConcept,
            DbValueSetConcept targetConcept,
            DbValueSetConceptComparison conceptComparison,
            ref string userMessage)
        {
            // start with local cache
            List<DbValueSetConceptComparison> comparisonsTargeting = conceptComparisonCache.ForTarget(targetConcept.Key)
                //.Where(c => c.SourceFhirPackageKey == sourcePackage.Key)
                .Where(c => c.SourceValueSetKey == sourceVs.Key)
                .ToList();

            // grab any existing database records
            List<DbValueSetConceptComparison> comparisonsTargetingFromDb = DbValueSetConceptComparison.SelectList(
                _db,
                PackageComparisonKey: forwardPair.Key,
                TargetConceptKey: targetConcept.Key,
                SourceValueSetKey: sourceVs.Key);

            // add items from the db that are not already in the list
            comparisonsTargeting.AddRange(comparisonsTargetingFromDb.Where(dbRec => !comparisonsTargeting.Any(tracked => tracked.Key == dbRec.Key)));

            // if there are zero or one comparisons, nothing to do here
            if (comparisonsTargeting.Count < 2)
            {
                return;
            }

            // get the source concept list
            List<DbValueSetConcept?> sourceConceptCodes = comparisonsTargeting
                .Select(tc => DbValueSetConcept.SelectSingle(_db, Key: tc.SourceConceptKey))
                .Where(c => c != null)
                .ToList();

            userMessage += $". Note that multiple codes are mapped to the concept `{targetConcept.System}#{targetConcept.Code}`" +
                $" ({string.Join(", ", sourceConceptCodes.Select(c => $"`{c!.System}`#`{c!.Code}`"))}).";

            switch (conceptComparison.Relationship)
            {
                // if this is marked equivalent and there are multiple source codes mapping to the same target code, this source is actually narrower
                case CMR.Equivalent:
                    {
                        conceptComparison.Relationship = CMR.SourceIsNarrowerThanTarget;
                        conceptComparison.IsGenerated = true;
                        conceptComparison.TechnicalMessage = conceptComparison.TechnicalMessage +
                            $" Found multiple codes with mappings to `{targetConcept.System}#{targetConcept.Code}`" +
                            $" ({string.Join(", ", sourceConceptCodes.Select(c => $"`{c!.System}`#`{c!.Code}`"))})." +
                            $" The relationship was updated from `Equivalent` to `SourceIsNarrowerThanTarget`.";
                    }
                    break;

                // if this is marked narrower or related to, just update our messages
                case CMR.RelatedTo:
                case CMR.SourceIsNarrowerThanTarget:
                    {
                        conceptComparison.TechnicalMessage = conceptComparison.TechnicalMessage +
                            $" Found multiple codes with mappings to `{targetConcept.System}#{targetConcept.Code}`" +
                            $" ({string.Join(", ", sourceConceptCodes.Select(c => $"`{c!.System}`#`{c!.Code}`"))})." +
                            $" The relationship of `{conceptComparison.Relationship}` is still appropriate.";
                    }
                    break;

                // if this is marked broader and there are multiple source codes mapping to the same target code, this is a complicated mapping
                case CMR.SourceIsBroaderThanTarget:
                    {
                        conceptComparison.Relationship = CMR.RelatedTo;
                        conceptComparison.IsGenerated = true;
                        conceptComparison.TechnicalMessage = conceptComparison.TechnicalMessage +
                            $" Found multiple codes with mappings to `{targetConcept.System}#{targetConcept.Code}`" +
                            $" ({string.Join(", ", sourceConceptCodes.Select(c => $"`{c!.System}`#`{c!.Code}`"))})." +
                            $" The relationship of `SourceIsBroaderThanTarget` is inappropriate - this is a complex relationship described as `RelatedTo`.";
                    }
                    break;
            }
        }

        void checkMultipleTargets(
            DbValueSetConcept sourceConcept,
            DbValueSetConcept targetConcept,
            DbValueSetConceptComparison conceptComparison,
            ref string userMessage)
        {
            // start with local cache
            List<DbValueSetConceptComparison> comparisonsForSource = conceptComparisonCache.ForSource(sourceConcept.Key)
                //.Where(c => c.TargetFhirPackageKey == targetPackage.Key)
                .Where(c => c.TargetValueSetKey == targetVs.Key)
                .ToList();

            // grab any existing database records
            List<DbValueSetConceptComparison> comparisonsForSourceFromDb = DbValueSetConceptComparison.SelectList(
                _db,
                PackageComparisonKey: forwardPair.Key,
                SourceConceptKey: sourceConcept.Key,
                TargetValueSetKey: targetVs.Key);

            // add items from the db that are not already in the list
            comparisonsForSource.AddRange(comparisonsForSourceFromDb.Where(dbRec => !comparisonsForSource.Any(tracked => tracked.Key == dbRec.Key)));

            // if there are zero or one comparisons, nothing to do here
            if (comparisonsForSource.Count < 2)
            {
                return;
            }

            // get the target concept list
            List<DbValueSetConcept?> targetConceptCodes = comparisonsForSource
                .Select(tc => DbValueSetConcept.SelectSingle(_db, Key: tc.TargetConceptKey))
                .Where(c => c != null)
                .ToList();

            userMessage += $". Note that this concept is mapped to the multiple concepts: " +
                $" {string.Join(", ", targetConceptCodes.Select(c => $"`{c!.System}`#`{c!.Code}`"))}.";

            switch (conceptComparison.Relationship)
            {
                // if this is marked equivalent and there are multiple target codes, this source is actually broader than each individual target
                case CMR.Equivalent:
                    {
                        conceptComparison.Relationship = CMR.SourceIsBroaderThanTarget;
                        conceptComparison.IsGenerated = true;
                        conceptComparison.TechnicalMessage = conceptComparison.TechnicalMessage +
                            $" Found multiple mapping targets for `{sourceConcept.System}#{sourceConcept.Code}`:" +
                            $" {string.Join(", ", targetConceptCodes.Select(c => $"`{c!.System}`#`{c!.Code}`"))}." +
                            $" The relationship was updated from `Equivalent` to `SourceIsBroaderThanTarget`.";
                    }
                    break;

                // if this is marked broader or related to, just update our messages
                case CMR.RelatedTo:
                case CMR.SourceIsBroaderThanTarget:
                    {
                        conceptComparison.TechnicalMessage = conceptComparison.TechnicalMessage +
                            $" Found multiple mapping targets for `{sourceConcept.System}#{sourceConcept.Code}`:" +
                            $" {string.Join(", ", targetConceptCodes.Select(c => $"`{c!.System}`#`{c!.Code}`"))}." +
                            $" The relationship of `{conceptComparison.Relationship}` is still appropriate.";
                    }
                    break;

                // if this is marked narrower and there are multiple targets, this is a complicated mapping
                case CMR.SourceIsNarrowerThanTarget:
                    {
                        conceptComparison.Relationship = CMR.RelatedTo;
                        conceptComparison.IsGenerated = true;
                        conceptComparison.TechnicalMessage = conceptComparison.TechnicalMessage +
                            $" Found multiple mapping targets for `{sourceConcept.System}#{sourceConcept.Code}`:" +
                            $" {string.Join(", ", targetConceptCodes.Select(c => $"`{c!.System}`#`{c!.Code}`"))}." +
                            $" The relationship of `SourceIsNarrowerThanTarget` is inappropriate - this is a complex relationship described as `RelatedTo`.";
                    }
                    break;
            }
        }
    }
#endif
}
