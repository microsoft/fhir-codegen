using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Health.Fhir.CodeGenCommon.Models;
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

    public void Compare(
        FhirArtifactClassEnum? artifactFilter = null,
        HashSet<int>? comparisonPairFilterSet = null)
    {
        Dictionary<int, DbFhirPackage> packages = DbFhirPackage.SelectList(_db).ToDictionary(p => p.Key);

        // iterate over each FHIR Package we have
        foreach (DbFhirPackage sourcePackage in packages.Values)
        {
            _logger.LogInformation($"Processing source package {sourcePackage.Key}: {sourcePackage.PackageId}@{sourcePackage.PackageVersion}");
            
            List<(DbFhirPackageComparisonPair forward, DbFhirPackageComparisonPair reverse)> bidirectionalPairs = [];

            foreach (DbFhirPackageComparisonPair pf in DbFhirPackageComparisonPair.SelectList(_db, SourcePackageKey: sourcePackage.Key))
            {
                // skip pairs that are not in the filter
                if ((comparisonPairFilterSet != null) &&
                    (comparisonPairFilterSet.Count != 0) &&
                    !comparisonPairFilterSet.Contains(pf.Key))
                {
                    continue;
                }

                DbFhirPackageComparisonPair? reverse = DbFhirPackageComparisonPair.SelectSingle(
                    _db,
                    SourcePackageKey: pf.TargetPackageKey,
                    TargetPackageKey: pf.SourcePackageKey);

                if (reverse != null)
                {
                    if (pf.InverseComparisonKey != reverse.Key)
                    {
                        pf.InverseComparisonKey = reverse.Key;
                        pf.Update(_db);
                    }

                    if (reverse.InverseComparisonKey != pf.Key)
                    {
                        reverse.InverseComparisonKey = pf.Key;
                        reverse.Update(_db);
                    }

                    bidirectionalPairs.Add((pf, reverse));
                    continue;
                }

                reverse = invert(pf);
                reverse.Insert(_db);
                pf.InverseComparisonKey = reverse.Key;
                pf.Update(_db);

                bidirectionalPairs.Add((pf, reverse));
            }

            // consistency check
            if (bidirectionalPairs.Any(biPair => !packages.ContainsKey(biPair.forward.TargetPackageKey)))
            {
                throw new Exception("Failed to resolve packages in all pairwise comparisons!");
            }

            if ((artifactFilter == null) ||
                (artifactFilter == FhirArtifactClassEnum.ValueSet))
            {
                Dictionary<int, DbValueSetComparison> vsComparisonsToAdd = [];
                Dictionary<int, DbValueSetComparison> vsComparisonsToUpdate = [];
                Dictionary<int, DbValueSetConceptComparison> conceptComparisonsToAdd = [];
                Dictionary<int, DbValueSetConceptComparison> conceptComparisonsToUpdate = [];

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

                        doValueSetComparisons(
                            sourcePackage,
                            sourceVs,
                            targetPackage,
                            forward,
                            reverse,
                            vsComparisonsToAdd,
                            vsComparisonsToUpdate,
                            conceptComparisonsToAdd,
                            conceptComparisonsToUpdate);
                    }
                }

                // update the database
                vsComparisonsToAdd.Values.ToList().Insert(_db);
                vsComparisonsToUpdate.Values.ToList().Update(_db);
                conceptComparisonsToAdd.Values.ToList().Insert(_db);
                conceptComparisonsToUpdate.Values.ToList().Update(_db);
            }

            // any structure triggers all of them
            if ((artifactFilter == null) ||
                (artifactFilter == FhirArtifactClassEnum.PrimitiveType) ||
                (artifactFilter == FhirArtifactClassEnum.ComplexType) ||
                (artifactFilter == FhirArtifactClassEnum.Resource) ||
                (artifactFilter == FhirArtifactClassEnum.Profile) ||
                (artifactFilter == FhirArtifactClassEnum.LogicalModel))
            {
                Dictionary<int, DbStructureComparison> sdComparisonsToAdd = [];
                Dictionary<int, DbStructureComparison> sdComparisonsToUpdate = [];
                Dictionary<int, DbElementComparison> elementComparisonsToAdd = [];
                Dictionary<int, DbElementComparison> elementComparisonsToUpdate = [];

                Dictionary<(int sourceTypeGroup, int targetTypeGroup), DbElementTypeGroupComparison> typeGroupComparisonsToAdd = [];
                Dictionary<(int sourceTypeGroup, int targetTypeGroup), DbElementTypeGroupComparison> typeGroupComparisonsToUpdate = [];


                // iterate over our artifact types
                foreach (FhirArtifactClassEnum artifactClass in getArtifactClassSequence())
                {
                    List<DbStructureDefinition> structures = DbStructureDefinition.SelectList(_db, FhirPackageKey: sourcePackage.Key, ArtifactClass: artifactClass);
                    _logger.LogInformation($" <<< processing Structures:{artifactClass}, count: {structures.Count}");

                    // iterate over the structures in the package
                    foreach (DbStructureDefinition sourceSd in structures)
                    {
                        _logger.LogInformation($" <<< processing Structure:{artifactClass} {sourceSd.VersionedUrl}");

                        // iterate over the comparison pairs
                        foreach ((DbFhirPackageComparisonPair forward, DbFhirPackageComparisonPair reverse) in bidirectionalPairs)
                        {
                            // grab our target package
                            DbFhirPackage targetPackage = packages[forward.TargetPackageKey];
                            doStructureComparisons(
                                sourcePackage,
                                sourceSd,
                                targetPackage,
                                forward,
                                reverse,
                                sdComparisonsToAdd,
                                sdComparisonsToUpdate,
                                elementComparisonsToAdd,
                                elementComparisonsToUpdate,
                                typeGroupComparisonsToAdd,
                                typeGroupComparisonsToUpdate);
                        }
                    }
                }

                // update the database
                sdComparisonsToAdd.Values.Insert(_db);
                sdComparisonsToUpdate.Values.Update(_db);
                elementComparisonsToAdd.Values.Insert(_db);
                elementComparisonsToUpdate.Values.Update(_db);
                typeGroupComparisonsToAdd.Values.Insert(_db);
                typeGroupComparisonsToUpdate.Values.Update(_db);
            }
        }

        return;

        FhirArtifactClassEnum[] getArtifactClassSequence() => [
            FhirArtifactClassEnum.PrimitiveType,
            FhirArtifactClassEnum.ComplexType,
            FhirArtifactClassEnum.Resource,
            FhirArtifactClassEnum.Profile,
            FhirArtifactClassEnum.LogicalModel,
            ];
    }

    private void doStructureComparisons(
        DbFhirPackage sourcePackage,
        DbStructureDefinition sourceSd,
        DbFhirPackage targetPackage,
        DbFhirPackageComparisonPair forwardPair,
        DbFhirPackageComparisonPair reversePair,
        Dictionary<int, DbStructureComparison> sdComparisonsToAdd,
        Dictionary<int, DbStructureComparison> sdComparisonsToUpdate,
        Dictionary<int, DbElementComparison> elementComparisonsToAdd,
        Dictionary<int, DbElementComparison> elementComparisonsToUpdate,
        Dictionary<(int sourceTypeGroup, int targetTypeGroup), DbElementTypeGroupComparison> typeGroupComparisonsToAdd,
        Dictionary<(int sourceTypeGroup, int targetTypeGroup), DbElementTypeGroupComparison> typeGroupComparisonsToUpdate)
    {
        // check for a existing comparisons
        List<DbStructureComparison> forwardComparisons = DbStructureComparison.SelectList(
            _db,
            PackageComparisonKey: forwardPair.Key,
            SourceFhirPackageKey: sourcePackage.Key,
            TargetFhirPackageKey: targetPackage.Key,
            SourceStructureKey: sourceSd.Key);

        // if there are none, see if we can find an equivalent value set to compare with in the target
        if (forwardComparisons.Count == 0)
        {
            switch (sourceSd.ArtifactClass)
            {
                // if this is a primitive type, manually create the known relationships from FhirTypeMappings
                case FhirArtifactClassEnum.PrimitiveType:
                    {
                        foreach (FhirTypeMappings.CodeGenTypeMapping tm in FhirTypeMappings.PrimitiveMappings)
                        {
                            if (tm.SourceType != sourceSd.Name)
                            {
                                continue;
                            }

                            DbStructureDefinition? primitiveSource = DbStructureDefinition.SelectSingle(_db, FhirPackageKey: sourcePackage.Key, Name: tm.SourceType);
                            if (primitiveSource == null)
                            {
                                continue;
                            }

                            DbStructureDefinition? primitiveTarget = DbStructureDefinition.SelectSingle(_db, FhirPackageKey: targetPackage.Key, Name: tm.TargetType);
                            if (primitiveTarget == null)
                            {
                                continue;
                            }

                            // add this forward mapping
                            DbStructureComparison pc = new()
                            {
                                Key = _comparisonDb.GetStructureComparisonKey(),
                                PackageComparisonKey = forwardPair.Key,
                                SourceFhirPackageKey = sourcePackage.Key,
                                TargetFhirPackageKey = targetPackage.Key,
                                SourceStructureKey = primitiveSource.Key,
                                SourceCanonicalVersioned = primitiveSource.VersionedUrl,
                                SourceCanonicalUnversioned = primitiveSource.UnversionedUrl,
                                SourceVersion = primitiveSource.Version,
                                SourceName = primitiveSource.Name,
                                TargetStructureKey = primitiveTarget.Key,
                                TargetCanonicalVersioned = primitiveTarget.VersionedUrl,
                                TargetCanonicalUnversioned = primitiveTarget.UnversionedUrl,
                                TargetVersion = primitiveTarget.Version,
                                TargetName = primitiveTarget.Name,
                                CompositeName = ComparisonDatabase.GetCompositeName(sourcePackage, primitiveSource, targetPackage, primitiveTarget),
                                SourceOverviewConceptMapUrl = null,
                                SourceStructureFmlUrl = null,
                                Relationship = tm.Relationship,
                                ConceptDomainRelationship = tm.ConceptDomainRelationship,
                                ValueDomainRelationship = tm.ValueDomainRelationship,
                                IsGenerated = true,
                                LastReviewedBy = null,
                                LastReviewedOn = null,
                                Message = tm.Comment,
                            };

                            sdComparisonsToAdd[pc.Key] = pc;
                            forwardComparisons.Add(pc);
                        }
                    }
                    break;

                // check to see if we can find a match to this in the target package
                default:
                    {
                        string message = "Inferred comparison based on ";

                        List<DbStructureDefinition> potentialTargets = DbStructureDefinition.SelectList(_db, FhirPackageKey: targetPackage.Key, UnversionedUrl: sourceSd.UnversionedUrl);
                        if (potentialTargets.Count != 0)
                        {
                            message += $" unversioned URL match from source: `{sourceSd.UnversionedUrl}`";
                        }
                        else
                        {
                            potentialTargets = DbStructureDefinition.SelectList(_db, FhirPackageKey: targetPackage.Key, Name: sourceSd.Name);

                            if (potentialTargets.Count != 0)
                            {
                                message += $" Name match from source: `{sourceSd.Name}`";
                            }
                            else
                            {
                                potentialTargets = DbStructureDefinition.SelectList(_db, FhirPackageKey: targetPackage.Key, Id: sourceSd.Id);

                                if (potentialTargets.Count != 0)
                                {
                                    message += $" Id match from source: {sourceSd.Id}";
                                }
                            }
                        }

                        foreach (DbStructureDefinition targetSd in potentialTargets)
                        {
                            // create this comparison
                            DbStructureComparison sdc = new()
                            {
                                Key = _comparisonDb.GetStructureComparisonKey(),
                                PackageComparisonKey = forwardPair.Key,
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
                                Message = message,
                            };

                            sdComparisonsToAdd[sdc.Key] = sdc;
                            forwardComparisons.Add(sdc);
                        }
                    }
                    break;
            }
        }

        // iterate across the forward comparisons
        foreach (DbStructureComparison forwardComparison in forwardComparisons)
        {
            bool forwardModified = false;
            bool inverseModified = false;

            // get the target value set for this comparison
            DbStructureDefinition targetSd = DbStructureDefinition.SelectSingle(
                _db,
                Key: forwardComparison.TargetStructureKey)
                ?? throw new Exception($"Could not resolve target Structure with Key: {forwardComparison.TargetStructureKey} (`{forwardComparison.TargetCanonicalVersioned}`)");

            // look for an inverse comparison
            DbStructureComparison? inverseComparison = null;
            if (forwardComparison.InverseComparisonKey == null)
            {
                inverseComparison = DbStructureComparison.SelectSingle(
                    _db,
                    PackageComparisonKey: reversePair.Key,
                    SourceFhirPackageKey: targetPackage.Key,
                    SourceStructureKey: forwardComparison.TargetStructureKey,
                    TargetFhirPackageKey: sourcePackage.Key,
                    TargetStructureKey: forwardComparison.SourceStructureKey);
            }
            else
            {
                inverseComparison = DbStructureComparison.SelectSingle(_db, Key: forwardComparison.InverseComparisonKey);
            }

            if (inverseComparison == null)
            {
                inverseComparison = invert(forwardComparison, reversePair);
                sdComparisonsToAdd[inverseComparison.Key] = inverseComparison;
            }

            if (forwardComparison.InverseComparisonKey != inverseComparison.Key)
            {
                forwardComparison.InverseComparisonKey = inverseComparison.Key;
                forwardModified = true;
            }

            // process this comparison
            doElementComparisons(
                sourcePackage,
                sourceSd,
                targetPackage,
                targetSd,
                forwardComparison,
                inverseComparison,
                forwardPair,
                reversePair,
                elementComparisonsToAdd,
                elementComparisonsToUpdate);

            if (aggregateStructureRelationships(forwardComparison, sourceSd, targetSd))
            {
                forwardModified = true;
            }

            if (aggregateStructureRelationships(inverseComparison, targetSd, sourceSd))
            {
                inverseModified = true;
            }

            // update primitives manually according to known relationships
            if (FhirTypeMappings.TryGetMapping(sourceSd.Name, targetSd.Name, out FhirTypeMappings.CodeGenTypeMapping? sourcePM) &&
                (forwardComparison.Relationship != sourcePM.Value.Relationship))
            {
                forwardComparison.Relationship = sourcePM.Value.Relationship;
                forwardComparison.ConceptDomainRelationship = sourcePM.Value.ConceptDomainRelationship;
                forwardComparison.ValueDomainRelationship = sourcePM.Value.ValueDomainRelationship;
                forwardModified = true;
            }

            if (FhirTypeMappings.TryGetMapping(targetSd.Name, sourceSd.Name, out FhirTypeMappings.CodeGenTypeMapping? targetPM) &&
                (inverseComparison.Relationship != targetPM.Value.Relationship))
            {
                inverseComparison.Relationship = targetPM.Value.Relationship;
                inverseComparison.ConceptDomainRelationship = targetPM.Value.ConceptDomainRelationship;
                inverseComparison.ValueDomainRelationship = targetPM.Value.ValueDomainRelationship;
                inverseModified = true;
            }

            if (forwardModified &&
                !sdComparisonsToAdd.ContainsKey(forwardComparison.Key))
            {
                sdComparisonsToUpdate[forwardComparison.Key] = forwardComparison;
            }

            if (inverseModified &&
                !sdComparisonsToAdd.ContainsKey(inverseComparison.Key))
            {
                sdComparisonsToUpdate[inverseComparison.Key] = inverseComparison;
            }
        }

        return;
    }

    private void doElementTypeGroupComparison(
        DbFhirPackage sourcePackage,
        DbElementTypeGroup sourceTypeGroup,
        DbFhirPackage targetPackage,
        DbElementTypeGroup targetTypeGroup,
        Dictionary<(int sourceTypeGroup, int targetTypeGroup), DbElementTypeGroupComparison> typeGroupComparisonsToAdd,
        Dictionary<(int sourceTypeGroup, int targetTypeGroup), DbElementTypeGroupComparison> typeGroupComparisonsToUpdate)
    {

    }

    private void doElementComparisons(
        DbFhirPackage sourcePackage,
        DbStructureDefinition sourceSd,
        DbFhirPackage targetPackage,
        DbStructureDefinition targetSd,
        DbStructureComparison forwardComparison,
        DbStructureComparison reverseComparison,
        DbFhirPackageComparisonPair forwardPair,
        DbFhirPackageComparisonPair reversePair,
        Dictionary<int, DbElementComparison> elementComparisonsToAdd,
        Dictionary<int, DbElementComparison> elementComparisonsToUpdate)
    {
        // select only active and concrete concepts
        List<DbElement> sourceElements = DbElement.SelectList(_db, StructureKey: sourceSd.Key);

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

            // if there are no existing comparisons, see if we can find a matching element
            if ((comparisons.Count == 0) &&
                (DbElement.SelectList(_db, StructureKey: targetSd.Key, Id: sourceElement.Id) is List<DbElement> targetElements) &&
                (targetElements.Count != 0))
            {
                CMR relationship = (targetElements.Count == 1)
                    ? CMR.Equivalent
                    : CMR.SourceIsBroaderThanTarget;

                // TODO: check for a compatible type

                // iterate over the possible targets
                foreach (DbElement targetElement in targetElements)
                {
                    DbElementComparison comp = new()
                    {
                        Key = _comparisonDb.GetElementComparisonKey(),
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
                        NoMap = false,
                        Message = $"Created mapping based on literal match of id `{sourceElement.Id}`",
                        IsGenerated = true,
                        LastReviewedBy = null,
                        LastReviewedOn = null,
                    };

                    elementComparisonsToAdd.Add(comp.Key, comp);
                    comparisons.Add(comp);
                }
            }

            // if there are still no comparisons, add a no-map
            if (comparisons.Count == 0)
            {
                DbElementComparison noMap = new()
                {
                    Key = _comparisonDb.GetElementComparisonKey(),
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
                    NoMap = true,
                    Message = $"No mapping exists and no literal match found - created no-map entry for `{sourceSd.Name}`: `{sourceElement.Id}`",
                    IsGenerated = true,
                    LastReviewedBy = null,
                    LastReviewedOn = null,
                };

                // insert into the database
                elementComparisonsToAdd.Add(noMap.Key, noMap);

                // nothing else to do on this pass
                continue;
            }

            // iterate over the comparisons to check relationships
            foreach (DbElementComparison elementComparison in comparisons)
            {
                bool forwardModified = false;
                bool inverseModified = false;

                DbElement? targetElement = (elementComparison.TargetElementKey == null)
                    ? null
                    : DbElement.SelectSingle(_db, Key: elementComparison.TargetElementKey);

                DbElementComparison? inverseComparison = null;
                if (elementComparison.TargetElementKey != null)
                {
                    if (forwardComparison.InverseComparisonKey == null)
                    {
                        inverseComparison = DbElementComparison.SelectSingle(
                                _db,
                                StructureComparisonKey: reverseComparison.Key,
                                SourceElementKey: elementComparison.TargetElementKey,
                                TargetElementKey: elementComparison.SourceElementKey);
                    }
                    else
                    {
                        inverseComparison = DbElementComparison.SelectSingle(_db, Key: elementComparison.InverseComparisonKey);
                    }
                }

                // if there is no inverse and there should be, create one
                if ((inverseComparison == null) &&
                    (elementComparison.TargetElementKey != null))
                {
                    inverseComparison = invert(elementComparison, sourceElement, targetElement!, reverseComparison, reversePair);
                    elementComparisonsToAdd.Add(inverseComparison.Key, inverseComparison);

                    elementComparison.InverseComparisonKey = inverseComparison.Key;
                    forwardModified = true;
                }

                // do basic checks if this has not been reviewed
                if (elementComparison.LastReviewedOn == null)
                {
                    // check for missing no-map value
                    if ((elementComparison.TargetElementKey == null) &&
                        (elementComparison.NoMap != true))
                    {
                        forwardModified = true;
                        elementComparison.NoMap = true;
                    }

                    // check for a single source with multiple targets and any that map as equivalent
                    if ((elementComparison.Relationship == CMR.Equivalent) &&
                        (comparisons.Count > 1))
                    {
                        forwardModified = true;

                        // mark as not equivalent
                        elementComparison.Relationship = CMR.SourceIsBroaderThanTarget;
                        elementComparison.IsGenerated = true;
                        elementComparison.Message = elementComparison.Message +
                            $" `{sourceElement.Id}` maps to multiple elements in {targetSd.Name} and cannot be equivalent.";
                    }

                    // TODO: add actual checks
                }

                if (inverseComparison != null)
                {
                    // check to see if the relationship makes sense as an inverse
                    CMR? expected = invert(elementComparison.Relationship);
                    if ((inverseComparison.LastReviewedOn != null) &&
                        (inverseComparison.Relationship != expected))
                    {
                        inverseComparison.Message = inverseComparison.Message +
                            $" Updated relationship from `{inverseComparison.Relationship}` to `{expected}`" +
                            $" based on the inverse comparsion {elementComparison.Key}, which has a relationship" +
                            $" of `{elementComparison.Relationship}`.";
                        inverseComparison.Relationship = expected;

                        inverseModified = true;
                    }
                }

                // if any changes have been made and this is not a new record, it needs to be updated
                if (forwardModified && !elementComparisonsToAdd.ContainsKey(elementComparison.Key))
                {
                    elementComparisonsToUpdate[elementComparison.Key] = elementComparison;
                }

                if (inverseModified && !elementComparisonsToAdd.ContainsKey(inverseComparison!.Key))
                {
                    elementComparisonsToUpdate[inverseComparison.Key] = inverseComparison;
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
        DbFhirPackageComparisonPair reversePair,
        Dictionary<int, DbValueSetComparison> vsComparisonsToAdd,
        Dictionary<int, DbValueSetComparison> vsComparisonsToUpdate,
        Dictionary<int, DbValueSetConceptComparison> conceptComparisonsToAdd,
        Dictionary<int, DbValueSetConceptComparison> conceptComparisonsToUpdate)
    {
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

                vsComparisonsToAdd[vsc.Key] = vsc;
                forwardComparisons.Add(vsc);
            }
        }

        // iterate across the forward comparisons
        foreach (DbValueSetComparison forwardComparison in forwardComparisons)
        {
            bool forwardModified = false;
            bool inverseModified = false;

            // get the target value set for this comparison
            DbValueSet targetVs = DbValueSet.SelectSingle(
                _db,
                Key: forwardComparison.TargetValueSetKey)
                ?? throw new Exception($"Could not resolve target ValueSet with Key: {forwardComparison.TargetValueSetKey} (`{forwardComparison.TargetCanonicalVersioned}`)");

            // look for an inverse comparison
            DbValueSetComparison? inverseComparison = null;
            if (forwardComparison.InverseComparisonKey == null)
            {
                inverseComparison = DbValueSetComparison.SelectSingle(
                    _db,
                    PackageComparisonKey: reversePair.Key,
                    SourceFhirPackageKey: targetPackage.Key,
                    SourceValueSetKey: forwardComparison.TargetValueSetKey,
                    TargetFhirPackageKey: sourcePackage.Key,
                    TargetValueSetKey: forwardComparison.SourceValueSetKey);
            }
            else
            {
                inverseComparison = DbValueSetComparison.SelectSingle(_db, Key: forwardComparison.InverseComparisonKey);
            }

            if (inverseComparison == null)
            {
                inverseComparison = invert(forwardComparison, reversePair);
                vsComparisonsToAdd[inverseComparison.Key] = inverseComparison;
            }

            if (forwardComparison.InverseComparisonKey != inverseComparison.Key)
            {
                forwardComparison.InverseComparisonKey = inverseComparison.Key;
                forwardModified = true;
            }

            // process this comparison
            doValueSetConceptComparisons(
                sourcePackage,
                sourceVs,
                targetPackage,
                targetVs,
                forwardComparison,
                inverseComparison,
                forwardPair,
                reversePair,
                conceptComparisonsToAdd,
                conceptComparisonsToUpdate);

            if (aggregateValueSetRelationships(forwardComparison, sourceVs, targetVs))
            {
                forwardModified = true;
            }

            if (aggregateValueSetRelationships(inverseComparison, targetVs, sourceVs))
            {
                inverseModified = true;
            }

            if (forwardModified &&
                !vsComparisonsToAdd.ContainsKey(forwardComparison.Key))
            {
                vsComparisonsToUpdate[forwardComparison.Key] = forwardComparison;
            }

            if (inverseModified &&
                !vsComparisonsToAdd.ContainsKey(inverseComparison.Key))
            {
                vsComparisonsToUpdate[inverseComparison.Key] = inverseComparison;
            }
        }

        return;
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

    internal static CMR? RelationshipForCounts(int? sourceCount, int? targetCount)
    {
        // zero counts mean the target cannot be evaluated
        if ((sourceCount == null) || (targetCount == null) ||
            (sourceCount == 0) || (targetCount == 0))
        {
            return null;
        }

        return ((int)sourceCount).CompareTo((int)targetCount) switch
        {
            < 0 => CMR.SourceIsNarrowerThanTarget,
            > 0 => CMR.SourceIsBroaderThanTarget,
            _ => CMR.Equivalent,
        };
    }

    /// <summary>
    /// Aggregates the relationships of value sets within a FHIR package comparison.
    /// </summary>
    /// <param name="vsComparison">The comparison object for the value set.</param>
    /// <returns>True if the relationship was updated, otherwise false.</returns>
    private bool aggregateValueSetRelationships(DbValueSetComparison vsComparison, DbValueSet sourceVs, DbValueSet targetVs)
    {
        List<DbValueSetConceptComparison> conceptComparisons = DbValueSetConceptComparison.SelectList(_db, ValueSetComparisonKey: vsComparison.Key);
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
        DbFhirPackageComparisonPair reversePair,
        Dictionary<int, DbValueSetConceptComparison> conceptComparisonsToAdd,
        Dictionary<int, DbValueSetConceptComparison> conceptComparisonsToUpdate)
    {
        // select only active and concrete concepts
        List<DbValueSetConcept> sourceConcepts = DbValueSetConcept.SelectList(_db, ValueSetKey: sourceVs.Key, Inactive: false, Abstract: false);

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

                    conceptComparisonsToAdd.Add(comp.Key, comp);
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
                conceptComparisonsToAdd.Add(noMap.Key, noMap);

                // nothing else to do on this pass
                continue;
            }

            // iterate over the comparisons to check relationships
            foreach (DbValueSetConceptComparison conceptComparison in comparisons)
            {
                bool forwardModified = false;
                bool inverseModified = false;

                DbValueSetConcept? targetConcept = (conceptComparison.TargetConceptKey == null)
                    ? null
                    : DbValueSetConcept.SelectSingle(_db, Key: conceptComparison.TargetConceptKey);

                // look for an inverse comparison
                DbValueSetConceptComparison? inverseComparison = null;
                if (conceptComparison.TargetConceptKey != null)
                {
                    if (forwardComparison.InverseComparisonKey == null)
                    {
                        inverseComparison = DbValueSetConceptComparison.SelectSingle(
                            _db,
                            PackageComparisonKey: reversePair.Key,
                            SourceFhirPackageKey: targetPackage.Key,
                            SourceValueSetKey: conceptComparison.TargetValueSetKey,
                            SourceConceptKey: conceptComparison.TargetConceptKey,
                            TargetFhirPackageKey: sourcePackage.Key,
                            TargetValueSetKey: conceptComparison.SourceValueSetKey,
                            TargetConceptKey: conceptComparison.SourceConceptKey);
                    }
                    else
                    {
                        inverseComparison = DbValueSetConceptComparison.SelectSingle(_db, Key: forwardComparison.InverseComparisonKey);
                    }
                }

                // if there is no inverse and there should be, create one
                if ((inverseComparison == null) &&
                    (conceptComparison.TargetConceptKey != null))
                {
                    inverseComparison = invert(conceptComparison, sourceConcept, targetConcept!, reverseComparison, reversePair);
                    conceptComparisonsToAdd.Add(inverseComparison.Key, inverseComparison);

                    conceptComparison.InverseComparisonKey = inverseComparison.Key;
                    forwardModified = true;
                }

                // do basic checks if this has not been reviewed
                if (conceptComparison.LastReviewedOn == null)
                {
                    // check for missing no-map value
                    if ((conceptComparison.TargetConceptKey == null) &&
                        (conceptComparison.NoMap != true))
                    {
                        forwardModified = true;
                        conceptComparison.NoMap = true;
                    }

                    // check for incorrectly-flagged-as-equivalent escape-value code mappings
                    if ((targetConcept != null) &&
                        (conceptComparison.Relationship == CMR.Equivalent) &&
                        XVerProcessor._escapeValveCodes.Contains(sourceConcept.Code) &&
                        (sourceVs.ActiveConcreteConceptCount != targetVs.ActiveConcreteConceptCount))
                    {
                        forwardModified = true;
                        conceptComparison.Relationship = RelationshipForCounts(sourceVs.ActiveConcreteConceptCount, targetVs.ActiveConcreteConceptCount);
                        conceptComparison.IsGenerated = true;
                        conceptComparison.Message = conceptComparison.Message +
                            $" Escape-valve code `{sourceConcept.Code}` maps to `{targetConcept.Code}`, but represent different concept domains (different number of codes).";
                    }

                    // check for a single source with multiple targets and any that map as equivalent
                    if ((conceptComparison.Relationship == CMR.Equivalent) &&
                        (comparisons.Count > 1))
                    {
                        forwardModified = true;

                        // mark as not equivalent
                        conceptComparison.Relationship = CMR.SourceIsBroaderThanTarget;
                        conceptComparison.IsGenerated = true;
                        conceptComparison.Message = conceptComparison.Message +
                            $" `{sourceConcept.Code}` maps to multiple codes in {targetVs.VersionedUrl} and cannot be equivalent.";
                    }
                }

                if (inverseComparison != null)
                {
                    // check to see if the inverted relationship makes sense
                    CMR? expected = invert(conceptComparison.Relationship);
                    if ((inverseComparison.LastReviewedOn != null) &&
                        (inverseComparison.Relationship != expected))
                    {
                        inverseComparison.Message = inverseComparison.Message +
                            $" Updated relationship from `{inverseComparison.Relationship}` to `{expected}`" +
                            $" based on the inverse comparsion {conceptComparison.Key}, which has a relationship" +
                            $" of `{conceptComparison.Relationship}`.";
                        inverseComparison.Relationship = expected;
                        inverseModified = true;
                    }
                }

                // if any changes have been made and this is not a new record, it needs to be updated
                if (forwardModified &&
                    !conceptComparisonsToAdd.ContainsKey(conceptComparison.Key))
                {
                    conceptComparisonsToUpdate[conceptComparison.Key] = conceptComparison;
                }

                if (inverseModified &&
                    !conceptComparisonsToAdd.ContainsKey(inverseComparison!.Key))
                {
                    conceptComparisonsToUpdate[inverseComparison.Key] = inverseComparison;
                }
            }
        }

        return;
    }

    private DbFhirPackageComparisonPair invert(DbFhirPackageComparisonPair other)
    {
        return new()
        {
            InverseComparisonKey = other.Key,
            SourcePackageKey = other.TargetPackageKey,
            SourcePackageShortName = other.TargetPackageShortName,
            TargetPackageKey = other.SourcePackageKey,
            TargetPackageShortName = other.SourcePackageShortName,
        };
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
                Key = _comparisonDb.GetStructureComparisonKey(),
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
                Message = tm.Value.Comment,
            };
        }

        return new()
        {
            Key = _comparisonDb.GetStructureComparisonKey(),
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
            Message = $"Mapping was inverted from Structure comparison {other.Key} of {other.SourceCanonicalVersioned} -> {other.TargetCanonicalVersioned}",
        };
    }

    private DbElementComparison invert(
        DbElementComparison other,
        DbElement otherSourceElement,
        DbElement otherTargetElement,
        DbStructureComparison reverseCanonicalComparison,
        DbFhirPackageComparisonPair reversePair)
    {
        return new()
        {
            Key = _comparisonDb.GetElementComparisonKey(),
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
            Relationship = invert(other.Relationship),
            ConceptDomainRelationship = invert(other.ConceptDomainRelationship),
            ValueDomainRelationship = invert(other.ValueDomainRelationship),
            NoMap = false,
            IsGenerated = true,
            LastReviewedBy = null,
            LastReviewedOn = null,
            Message = $"Mapping was inverted from Element comparison {other.Key}" +
                $" of `{otherSourceElement.Id}` -> `{otherTargetElement.Id}`",
        };
    }

    private DbValueSetComparison invert(
        DbValueSetComparison other,
        DbFhirPackageComparisonPair reversePair)
    {
        return new()
        {
            Key = _comparisonDb.GetValueSetComparisonKey(),
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
