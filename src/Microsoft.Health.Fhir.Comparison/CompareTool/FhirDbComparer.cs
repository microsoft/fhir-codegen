using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using Hl7.Fhir.Utility;
using Microsoft.Extensions.Logging;
using Microsoft.Health.Fhir.CodeGen.FhirExtensions;
using Microsoft.Health.Fhir.CodeGenCommon.Models;
using Microsoft.Health.Fhir.Comparison.Models;
using Microsoft.Health.Fhir.Comparison.XVer;
using static Microsoft.Health.Fhir.Comparison.CompareTool.FhirTypeMappings;
using CMR = Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship;

namespace Microsoft.Health.Fhir.Comparison.CompareTool;

public class FhirDbComparer
{
    private record class CollatedType
    {
        private static readonly string[] _quantityTypes = [
            "Quantity",
            "Age",
            "Count",
            "Distance",
            "Duration",
            "Money",
            "MoneyQuantity",
            "SimpleQuantity",
            ];

        public required string? TypeName { get; init; }
        public required int? TypeStructureKey { get; init; }
        public required string[] TypeProfiles { get; init; }
        public required string[] TargetProfiles { get; init; }
        public required List<DbElementType> UnderlyingElements { get; init; }

        public CollatedType() { }

        [SetsRequiredMembers]
        public CollatedType(IEnumerable<DbElementType> types)
        {
            UnderlyingElements = types is List<DbElementType> tl ? tl : types.ToList();
            TypeName = types.First().TypeName;
            TypeStructureKey = types.First().TypeStructureKey;
            TypeProfiles = types
                .Where(st => (st.TypeName == TypeName) && (!string.IsNullOrEmpty(st.TypeProfile)))
                .Select(st => st.TypeProfile!)
                .ToArray();
            TargetProfiles = types
                .Where(st => (st.TypeName == TypeName) && (!string.IsNullOrEmpty(st.TargetProfile)))
                .Select(st => st.TargetProfile!)
                .ToArray();
        }

        public string NormalizedName
        {
            get
            {
                if (string.IsNullOrEmpty(TypeName))
                {
                    return string.Empty;
                }

                if ((TypeName == "Quantity") &&
                    (TypeProfiles.Length == 1) &&
                    TypeProfiles[0].StartsWith("http://hl7.org/", StringComparison.Ordinal))
                {
                    return TypeProfiles[0].Split('/')[^1];
                }

                return TypeName;
            }
        }

        public bool HasSingleMatchingTypeProfile(string value)
        {
            if (TypeProfiles.Length != 1)
            {
                return false;
            }

            return TypeProfiles[0].EndsWith(value, StringComparison.Ordinal);
        }

        public bool IsQuantityType => _quantityTypes.Contains(NormalizedName);
    }

    private readonly ComparisonDatabase _comparisonDb;
    private readonly IDbConnection _db;

    private ILoggerFactory _loggerFactory;
    private ILogger _logger;

    private DbComparisonCache<DbValueSetComparison> _vsComparisons = new();
    private DbComparisonCache<DbValueSetConceptComparison> _conceptComparisons = new();

    private DbComparisonCache<DbStructureComparison> _sdComparisons = new();
    private DbComparisonCache<DbElementComparison> _elementComparisons = new();
    private DbComparisonCache<DbElementTypeComparison> _elementTypeComparisons = new();

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
        Dictionary<int, DbFhirPackage> packages = DbFhirPackage.SelectDict(_db);

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
                _vsComparisons.Clear();
                _conceptComparisons.Clear();

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
                            reverse);
                    }
                }

                // update the database
                _vsComparisons.ComparisonsToAdd.Insert(_db);
                _vsComparisons.ComparisonsToUpdate.Update(_db);
                _conceptComparisons.ComparisonsToAdd.Insert(_db);
                _conceptComparisons.ComparisonsToUpdate.Update(_db);
            }

            // any structure triggers all of them
            if ((artifactFilter == null) ||
                (artifactFilter == FhirArtifactClassEnum.PrimitiveType) ||
                (artifactFilter == FhirArtifactClassEnum.ComplexType) ||
                (artifactFilter == FhirArtifactClassEnum.Resource) ||
                (artifactFilter == FhirArtifactClassEnum.Profile) ||
                (artifactFilter == FhirArtifactClassEnum.LogicalModel))
            {
                _sdComparisons.Clear();
                _elementComparisons.Clear();
                _elementTypeComparisons.Clear();

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
                                reverse);
                        }
                    }
                }

                // update the database
                _sdComparisons.ComparisonsToAdd.Insert(_db);
                _sdComparisons.ComparisonsToUpdate.Update(_db);
                _elementComparisons.ComparisonsToAdd.Insert(_db);
                _elementComparisons.ComparisonsToUpdate.Update(_db);
                _elementTypeComparisons.ComparisonsToAdd.Insert(_db);
                _elementTypeComparisons.ComparisonsToUpdate.Update(_db);
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

        DbComparisonCache<DbElementComparison> elementChanges = new();

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
                elementChanges.CacheAdd(computedInverse);

                if (forwardConceptComparison.InverseComparisonKey != computedInverse.Key)
                {
                    forwardConceptComparison.InverseComparisonKey = computedInverse.Key;
                    elementChanges.CacheUpdate(forwardConceptComparison);
                }

                continue;
            }

            usedInverseKeys.Add(existingInverse.Key);

            // check to see if the inverse comparison has the same relationship
            if (existingInverse.Relationship != computedInverse.Relationship)
            {
                existingInverse.Relationship = computedInverse.Relationship;
                elementChanges.CacheUpdate(existingInverse);
            }
        }

        // flag we are deleting any inverse comparisons that are not used
        foreach ((int key, DbElementComparison existing) in existingInverseConceptComparisons)
        {
            if (usedInverseKeys.Contains(key))
            {
                continue;
            }
            elementChanges.CacheDelete(existing);
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
        elementChanges.ComparisonsToAdd.Insert(_db);
        elementChanges.ComparisonsToUpdate.Update(_db);
        elementChanges.ComparisonsToDelete.Delete(_db);

        // return the comparison in case the caller needs it
        return (inverseComparsion, elementChanges.Count != 0);
    }

    public void ApplySdConceptChanges(
        List<DbGraphSd.DbElementRow> originalProjection,
        IEnumerable<DbGraphSd.DbElementRow> updatedProjection,
        int sourceColumnIndex,
        bool isComparingRight,
        string? reviewer)
    {
        DbComparisonCache<DbElementComparison> changes = new();

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
                sourceToTargetComparison.Key = _comparisonDb.GetConceptComparisonKey();

                // cache the addition
                changes.CacheAdd(sourceToTargetComparison);
            }
            else
            {
                // cache as an update
                changes.CacheUpdate(sourceToTargetComparison);
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
                changes.CacheDelete(sourceToTargetComparison);
            }
        }

        // apply the changes to the concept comparisons
        changes.ComparisonsToAdd.Insert(_db);
        changes.ComparisonsToUpdate.Update(_db);
        changes.ComparisonsToDelete.Delete(_db);
    }

    private void doStructureComparisons(
        DbFhirPackage sourcePackage,
        DbStructureDefinition sourceSd,
        DbFhirPackage targetPackage,
        DbFhirPackageComparisonPair forwardPair,
        DbFhirPackageComparisonPair reversePair)
    {
        // check for a existing comparisons
        List<DbStructureComparison> forwardComparisons = _sdComparisons.ForSource(sourceSd.Key)
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
                                IsIdentical = null,
                            };

                            _sdComparisons.CacheAdd(pc);
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
                                IsIdentical = null,
                            };

                            _sdComparisons.CacheAdd(sdc);
                            forwardComparisons.Add(sdc);
                        }
                    }
                    break;
            }
        }

        // iterate across the forward comparisons
        foreach (DbStructureComparison forwardComparison in forwardComparisons)
        {
            if (forwardComparison.LastReviewedOn != null)
            {
                continue;
            }

            // get the target value set for this comparison
            DbStructureDefinition targetSd = DbStructureDefinition.SelectSingle(
                _db,
                Key: forwardComparison.TargetStructureKey)
                ?? throw new Exception($"Could not resolve target Structure with Key: {forwardComparison.TargetStructureKey} (`{forwardComparison.TargetCanonicalVersioned}`)");

            // run the comparison
            DoStructureComparison(
                _sdComparisons,
                _elementComparisons,
                _elementTypeComparisons,
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
        DbComparisonCache<DbStructureComparison> sdComparisons,
        DbComparisonCache<DbElementComparison> elementComparisons,
        DbComparisonCache<DbElementTypeComparison> elementTypeComparisons,
        DbFhirPackage sourcePackage,
        DbStructureDefinition sourceSd,
        DbFhirPackage targetPackage,
        DbStructureDefinition targetSd,
        DbStructureComparison forwardComparison,
        DbFhirPackageComparisonPair forwardPair,
        DbFhirPackageComparisonPair reversePair)
    {
        // look for an inverse comparison
        DbStructureComparison? inverseComparison = null;

        if ((forwardComparison.InverseComparisonKey != null) &&
            (forwardComparison.InverseComparisonKey != -1))
        {
            inverseComparison = sdComparisons.Get((int)forwardComparison.InverseComparisonKey) ??
                DbStructureComparison.SelectSingle(
                _db,
                Key: forwardComparison.InverseComparisonKey);
        }

        if (inverseComparison == null)
        {
            inverseComparison = sdComparisons.Get(targetSd.Key, forwardComparison.SourceStructureKey) ??
                DbStructureComparison.SelectSingle(
                    _db,
                    PackageComparisonKey: reversePair.Key,
                    SourceFhirPackageKey: targetPackage.Key,
                    SourceStructureKey: forwardComparison.TargetStructureKey,
                    TargetFhirPackageKey: sourcePackage.Key,
                    TargetStructureKey: forwardComparison.SourceStructureKey);
        }

        if (inverseComparison == null)
        {
            inverseComparison = invert(forwardComparison, reversePair);
            sdComparisons.CacheAdd(inverseComparison);
        }

        if (forwardComparison.InverseComparisonKey != inverseComparison.Key)
        {
            forwardComparison.InverseComparisonKey = inverseComparison.Key;
            sdComparisons.Changed(forwardComparison);
        }

        if (inverseComparison.InverseComparisonKey != forwardComparison.Key)
        {
            inverseComparison.InverseComparisonKey = forwardComparison.Key;
            sdComparisons.Changed(inverseComparison);
        }

        // process this comparison
        doElementComparisons(
            sdComparisons,
            elementComparisons,
            elementTypeComparisons,
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
            sdComparisons.Changed(forwardComparison);
        }

        if (inverseComparison.IsIdentical != forwardComparison.IsIdentical)
        {
            inverseComparison.IsIdentical = forwardComparison.IsIdentical;
            sdComparisons.Changed(inverseComparison);
        }

        if (aggregateStructureRelationships(forwardComparison, sourceSd, targetSd))
        {
            sdComparisons.Changed(forwardComparison);
        }

        if (aggregateStructureRelationships(inverseComparison, targetSd, sourceSd))
        {
            sdComparisons.Changed(inverseComparison);
        }

        // update manual relationships we have pre-specified
        if (FhirTypeMappings.CompositeMappingOverrides.TryGetValue(forwardComparison.CompositeName, out CodeGenTypeMapping forwardOverride))
        {
            forwardComparison.Relationship = forwardOverride.Relationship;
            forwardComparison.ConceptDomainRelationship = forwardOverride.ConceptDomainRelationship;
            forwardComparison.ValueDomainRelationship = forwardOverride.ValueDomainRelationship;
            forwardComparison.IsGenerated = true;
            forwardComparison.Message = forwardOverride.Comment;
            sdComparisons.Changed(forwardComparison);
        }
        else if (FhirTypeMappings.TryGetMapping(sourceSd.Name, targetSd.Name, out FhirTypeMappings.CodeGenTypeMapping? sourcePM))
        {
            forwardComparison.Relationship = sourcePM.Value.Relationship;
            forwardComparison.ConceptDomainRelationship = sourcePM.Value.ConceptDomainRelationship;
            forwardComparison.ValueDomainRelationship = sourcePM.Value.ValueDomainRelationship;
            forwardComparison.IsGenerated = true;
            forwardComparison.Message = sourcePM.Value.Comment;
            sdComparisons.Changed(forwardComparison);
        }

        if (FhirTypeMappings.CompositeMappingOverrides.TryGetValue(inverseComparison.CompositeName, out CodeGenTypeMapping inverseOverride))
        {
            inverseComparison.Relationship = inverseOverride.Relationship;
            inverseComparison.ConceptDomainRelationship = inverseOverride.ConceptDomainRelationship;
            inverseComparison.ValueDomainRelationship = inverseOverride.ValueDomainRelationship;
            inverseComparison.IsGenerated = true;
            inverseComparison.Message = inverseOverride.Comment;
            sdComparisons.Changed(inverseComparison);
        }
        else if (FhirTypeMappings.TryGetMapping(targetSd.Name, sourceSd.Name, out FhirTypeMappings.CodeGenTypeMapping? targetPM))
        {
            inverseComparison.Relationship = targetPM.Value.Relationship;
            inverseComparison.ConceptDomainRelationship = targetPM.Value.ConceptDomainRelationship;
            inverseComparison.ValueDomainRelationship = targetPM.Value.ValueDomainRelationship;
            inverseComparison.IsGenerated = true;
            inverseComparison.Message = targetPM.Value.Comment;
            sdComparisons.Changed(inverseComparison);
        }
    }

    private record class TypeComparisonTrackingRecord
    {
        public required CollatedType? TargetCollated { get; set; }
        public required DbStructureComparison? SdComparison { get; set; }
        public required CMR? TypeProfileRelationship { get; set; }
        public required string? TypeProfileMessage { get; set; }
        public required CMR? TargetProfileRelationship { get; set; }
        public required string? TargetProfileMessage { get; set; }
    }

    private DbElementTypeComparison doElementTypeComparison(
        DbComparisonCache<DbStructureComparison> sdComparisons,
        DbComparisonCache<DbElementTypeComparison> elementTypeComparisons,
        DbElementComparison elementComparison,
        DbFhirPackage sourcePackage,
        DbElement sourceElement,
        DbFhirPackage targetPackage,
        DbElement targetElement)
    {
        // check for existing comparisons
        DbElementTypeComparison? existing = elementTypeComparisons.Get(elementComparison.Key) ??
            DbElementTypeComparison.SelectSingle(_db, ElementComparisonKey: elementComparison.Key);

        if ((existing != null) &&
            (existing.LastReviewedOn != null))
        {
            return existing;
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
                    sdComparison = sdComparisons.Get(sourceCollated.TypeStructureKey!.Value, targetCollated.TypeStructureKey) ??
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

                    DbStructureComparison? potential = sdComparisons.Get(sourceCollated.TypeStructureKey!.Value, collated.TypeStructureKey) ??
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
            }
        }

        // check for unused target types
        if (usedTargetTypes.Count < collatedTargetTypes.Count)
        {
            valueRelationship = applyRelationship(valueRelationship, CMR.SourceIsNarrowerThanTarget);
        }

        if (noMap)
        {
            conceptRelationship =  applyRelationship(conceptRelationship, CMR.SourceIsBroaderThanTarget);
            valueRelationship = applyRelationship(valueRelationship, CMR.SourceIsBroaderThanTarget);
        }

        DbElementTypeComparison? inverse = null;

        // check for an inverse mapping
        if (!elementTypeComparisons.TryGet((targetElement.Key, sourceElement.Key), out inverse))
        {
            inverse = elementTypeComparisons.Get(targetElement.Key, sourceElement.Key) ??
                DbElementTypeComparison.SelectSingle(
                    _db,
                    SourceElementKey: targetElement.Key,
                    TargetElementKey: sourceElement.Key);
        }

        // for simplicity, create a code-gen type mapping that applies relationship rules for us
        CodeGenTypeMapping cgTypeMapping = new(
            sourceElement.CollatedTypeLiteral,
            targetElement.CollatedTypeLiteral,
            conceptRelationship,
            valueRelationship);

        DbElementTypeComparison? typeComparison = existing;

        // compare against our existing comparison if we have one
        if ((existing != null) &&
            (existing.Relationship != cgTypeMapping.Relationship))
        {
            existing.NoMap = noMap;
            existing.Relationship = cgTypeMapping.Relationship;
            existing.ConceptDomainRelationship = cgTypeMapping.ConceptDomainRelationship;
            existing.ValueDomainRelationship = cgTypeMapping.ValueDomainRelationship;
            existing.Message = cgTypeMapping.Comment;
            existing.IsGenerated = true;
            existing.LastReviewedBy = null;
            existing.LastReviewedOn = null;
        }

        // build an overal type comparison for this element if we do not have one
        typeComparison ??= new()
        {
            Key = _comparisonDb.GetElementTypeComparisonKey(),
            InverseComparisonKey = inverse?.Key,
            SourceFhirPackageKey = sourcePackage.Key,
            SourceStructureKey = elementComparison.SourceStructureKey,
            SourceElementKey = sourceElement.Key,
            SourceElementTypeLiteral = sourceElement.CollatedTypeLiteral,
            TargetFhirPackageKey = targetPackage.Key,
            TargetStructureKey = elementComparison.TargetStructureKey,
            TargetElementKey = targetElement.Key,
            TargetElementTypeLiteral = targetElement.CollatedTypeLiteral,
            PackageComparisonKey = elementComparison.PackageComparisonKey,
            StructureComparisonKey = elementComparison.StructureComparisonKey,
            ElementComparisonKey = elementComparison.Key,

            Relationship = cgTypeMapping.Relationship,
            ConceptDomainRelationship = conceptRelationship,
            ValueDomainRelationship = valueRelationship,
            Message = cgTypeMapping.Comment,
            NoMap = noMap,

            IsGenerated = true,
            LastReviewedBy = null,
            LastReviewedOn = null,
        };

        if ((inverse != null) &&
            (inverse.InverseComparisonKey != typeComparison.Key))
        {
            inverse.InverseComparisonKey = typeComparison.Key;
            elementTypeComparisons.Changed(inverse);
        }

        if (existing == null)
        {
            elementTypeComparisons.CacheAdd(typeComparison);
        }
        else
        {
            elementTypeComparisons.Changed(typeComparison);
        }

        return typeComparison;

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

    private void doElementComparisons(
        DbComparisonCache<DbStructureComparison> sdComparisons,
        DbComparisonCache<DbElementComparison> elementComparisons,
        DbComparisonCache<DbElementTypeComparison> elementTypeComparisons,
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
            string possibleTargetId = sourceElement.Id.Replace(sourceSd.Name, targetSd.Name, StringComparison.Ordinal);

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
                        Message = $"Created mapping based on literal match of id `{sourceElement.Id}`",
                        IsGenerated = true,
                        LastReviewedBy = null,
                        LastReviewedOn = null,
                        IsIdentical = null,
                    };

                    elementComparisons.CacheAdd(comp);
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
                    ElementTypeComparisonKey = null,
                    BoundValueSetComparisonKey = null,
                    NoMap = true,
                    Message = $"No mapping exists and no literal match found - created no-map entry for `{sourceSd.Name}`: `{sourceElement.Id}`",
                    IsGenerated = true,
                    LastReviewedBy = null,
                    LastReviewedOn = null,
                    IsIdentical = null,
                };

                // insert into the database
                elementComparisons.CacheAdd(noMap);

                identical = false;

                // nothing else to do on this pass
                continue;
            }

            // iterate over the comparisons to check relationships
            foreach (DbElementComparison elementComparison in comparisons)
            {
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
                    inverseComparison = elementComparisons.Get((int)elementComparison.InverseComparisonKey) ??
                        DbElementComparison.SelectSingle(_db, Key: elementComparison.InverseComparisonKey);
                }

                if (inverseComparison == null)
                {
                    inverseComparison = elementComparisons.Get(targetElement.Key, elementComparison.TargetElementKey) ??
                        DbElementComparison.SelectSingle(
                            _db,
                            StructureComparisonKey: reverseComparison.Key,
                            SourceElementKey: elementComparison.TargetElementKey,
                            TargetElementKey: elementComparison.SourceElementKey);
                }

                // if there is no inverse we need to create it
                if (inverseComparison == null)
                {
                    inverseComparison = invert(elementComparison, sourceElement, targetElement!, reverseComparison, reversePair, elementTypeComparisons);
                    elementComparisons.CacheAdd(inverseComparison);
                }

                if (elementComparison.InverseComparisonKey != inverseComparison.Key)
                {
                    elementComparison.InverseComparisonKey = inverseComparison.Key;
                    elementComparisons.Changed(elementComparison);
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

                    // check for missing no-map value
                    if ((elementComparison.TargetElementKey == null) &&
                        (noMap != true))
                    {
                        noMap = true;
                        messages.Add("No mapping exists and no literal match found - created no-map entry.");
                        changed = true;
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
                    }

                    // do type check comparison
                    DbElementTypeComparison elementTypeComparison = doElementTypeComparison(
                        sdComparisons,
                        elementTypeComparisons,
                        elementComparison,
                        sourcePackage,
                        sourceElement,
                        targetPackage,
                        targetElement);

                    if (elementComparison.ElementTypeComparisonKey != elementTypeComparison.Key)
                    {
                        changed = true;
                    }

                    if (valueRelationship != elementTypeComparison.Relationship)
                    {
                        valueRelationship = applyRelationship(valueRelationship, elementTypeComparison.Relationship);
                        messages.Add(
                            $"Applied type comparison relationship of: `{elementTypeComparison.Relationship}`" +
                            $" to existing value relationship: `{elementComparison.Relationship}`.");
                        changed = true;
                    }

                    // check for optional becoming mandatory: target has a broader concept than source since it requires content
                    if ((sourceElement.MinCardinality == 0) &&
                        (targetElement.MinCardinality != 0))
                    {
                        conceptRelationship = applyRelationship(conceptRelationship, CMR.SourceIsNarrowerThanTarget);
                        messages.Add($"`{targetElement.Name}` made the element mandatory (min cardinality from 0 to {targetElement.MinCardinality}).");
                        changed = true;
                    }

                    // check for source allowing fewer than target requires: target has a broader concept and value than the source
                    if (sourceElement.MinCardinality < targetElement.MinCardinality)
                    {
                        conceptRelationship = applyRelationship(conceptRelationship, CMR.SourceIsNarrowerThanTarget);
                        valueRelationship = applyRelationship(valueRelationship, CMR.SourceIsNarrowerThanTarget);
                        messages.Add($"`{targetElement.Name}` increased the minimum cardinality from {sourceElement.MinCardinality} to {targetElement.MinCardinality}.");
                        changed = true;
                    }

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
                    }

                    // check for changing from scalar to array: source is narrower than target in value
                    if ((sourceElement.MaxCardinality == 1) &&
                        (targetElement.MaxCardinality != 1))
                    {
                        valueRelationship = applyRelationship(valueRelationship, CMR.SourceIsNarrowerThanTarget);
                        messages.Add($"`{targetElement.Name}` changed from scalar to array (max cardinality from 1 to {targetElement.MaxCardinality}).");
                        changed = true;
                    }

                    // check for changing from array to scalar: source is broader than target in value
                    if ((sourceElement.MaxCardinality != 1) &&
                        (targetElement.MaxCardinality == 1))
                    {
                        valueRelationship = applyRelationship(valueRelationship, CMR.SourceIsBroaderThanTarget);
                        messages.Add($"`{targetElement.Name}` changed from array to scalar (max cardinality from {sourceElement.MaxCardinalityString} to 1).");
                        changed = true;
                    }

                    // check for source allowing more than target allows: target has a broader concept and value than the source
                    if ((targetElement.MaxCardinality != -1) &&
                        (sourceElement.MaxCardinality > targetElement.MaxCardinality))
                    {
                        conceptRelationship = applyRelationship(conceptRelationship, CMR.SourceIsBroaderThanTarget);
                        valueRelationship = applyRelationship(valueRelationship, CMR.SourceIsBroaderThanTarget);
                        messages.Add($"`{targetElement.Name}` decreased the maximum cardinality from {sourceElement.MaxCardinalityString} to {targetElement.MaxCardinalityString}.");
                        changed = true;
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
                        (sourceElement.CollatedTypeLiteral == targetElement.CollatedTypeLiteral) &&
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
                        elementComparisons.Changed(inverseComparison);
                    }

                    // if we changed something, apply all changes and update the record
                    if (changed)
                    {
                        elementComparison.Relationship = relationship;
                        elementComparison.ConceptDomainRelationship = conceptRelationship;
                        elementComparison.ValueDomainRelationship = valueRelationship;
                        elementComparison.NoMap = noMap;
                        elementComparison.IsGenerated = isGenerated;
                        elementComparison.Message = string.Join(" ", messages);
                        elementComparison.ElementTypeComparisonKey = elementTypeComparison.Key;
                        elementComparison.BoundValueSetComparisonKey = boundValueSetComparison?.Key;
                        elementComparisons.Changed(elementComparison);
                    }
                }

                // check to see if the relationship makes sense as an inverse
                CMR? expectedInverseRelationship = invert(elementComparison.Relationship);
                if ((inverseComparison.LastReviewedOn == null) &&
                    (inverseComparison.Relationship != expectedInverseRelationship))
                {
                    inverseComparison.Message = inverseComparison.Message +
                        $" Updated relationship from `{inverseComparison.Relationship}` to `{expectedInverseRelationship}`" +
                        $" based on the inverse comparsion {elementComparison.Key}, which has a relationship" +
                        $" of `{elementComparison.Relationship}`.";
                    inverseComparison.Relationship = expectedInverseRelationship;

                    elementComparisons.Changed(inverseComparison);
                }

                // process the current element's relationship
                aggregateStructureRelationship = applyRelationship(aggregateStructureRelationship, elementComparison.Relationship);
            }
        }

        return;
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
            changed = true;
            return;
        }

        // excluding the source and not the target means the source is broader than the target
        if (excludeSource)
        {
            conceptRelationship = applyRelationship(conceptRelationship, CMR.SourceIsBroaderThanTarget);
            valueRelationship = applyRelationship(valueRelationship, CMR.SourceIsBroaderThanTarget);
            messages.Add($"Failed to resolve or expand the source value set `{sourceElement.BindingValueSet}`, assuming the target is narrower.");
            changed = true;
            return;
        }

        // excluding the target and not the source means the source is narrower than the target
        if (excludeTarget)
        {
            conceptRelationship = applyRelationship(conceptRelationship, CMR.SourceIsNarrowerThanTarget);
            valueRelationship = applyRelationship(valueRelationship, CMR.SourceIsNarrowerThanTarget);
            messages.Add($"Failed to resolve or expand the target value set `{sourceElement.BindingValueSet}`, assuming the target is broader.");
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
                    }
                    else
                    {
                        conceptRelationship = applyRelationship(conceptRelationship, CMR.SourceIsNarrowerThanTarget);
                        valueRelationship = applyRelationship(valueRelationship, CMR.SourceIsNarrowerThanTarget);
                        messages.Add($"Target bound value set has more codes than source - required binding is COMPATIBLE for `code` type.");
                    }
                }
                else
                {
                    conceptRelationship = applyRelationship(conceptRelationship, boundValueSetComparison.Relationship);
                    valueRelationship = applyRelationship(valueRelationship, boundValueSetComparison.Relationship);
                    messages.Add(
                        $"Target value set is INCOMPATIBLE for a required binding on a `code` type," +
                        $" VS Relationship: {boundValueSetComparison.Relationship}.");
                }
            }
            // excluded value sets are assumed compatible
            else if ((sourceValueSet?.IsExcluded == true) ||
                (targetValueSet?.IsExcluded == true))
            {
                conceptRelationship = applyRelationship(conceptRelationship, CMR.Equivalent);
                valueRelationship = applyRelationship(valueRelationship, CMR.Equivalent);
                messages.Add($"Source or target value set is excluded - assuming required binding is COMPATIBLE for `code` type.");
            }
            else
            {
                conceptRelationship = applyRelationship(conceptRelationship, boundValueSetComparison.Relationship);
                valueRelationship = applyRelationship(valueRelationship, boundValueSetComparison.Relationship);
                messages.Add(
                    $"Source and target bound value sets are not compatible ({boundValueSetComparison.Relationship})" +
                    $" - required binding is INCOMPATIBLE for `code` type binding.");
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
            }
            else
            {
                conceptRelationship = applyRelationship(conceptRelationship, boundValueSetComparison.Relationship);
                valueRelationship = applyRelationship(valueRelationship, boundValueSetComparison.Relationship);
                messages.Add(
                    $"Source and target bound value sets are not compatible ({boundValueSetComparison.Relationship})" +
                    $" - required binding is INCOMPATIBLE for non-code type binding.");
            }
            changed = true;
        }
    }

    private void doValueSetComparisons(
        DbFhirPackage sourcePackage,
        DbValueSet sourceVs,
        DbFhirPackage targetPackage,
        DbFhirPackageComparisonPair forwardPair,
        DbFhirPackageComparisonPair reversePair)
    {
        // check for existing comparisons
        List<DbValueSetComparison> forwardComparisons = _vsComparisons.ForSource(sourceVs.Key)
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
                    IsIdentical = false,
                    CodesAreIdentical = false,
                };

                _vsComparisons.CacheAdd(vsc);
                forwardComparisons.Add(vsc);
            }
        }

        // iterate across the forward comparisons
        foreach (DbValueSetComparison forwardComparison in forwardComparisons)
        {
            if (forwardComparison.LastReviewedOn != null)
            {
                continue;
            }

            // get the target value set for this comparison
            DbValueSet targetVs = DbValueSet.SelectSingle(
                _db,
                Key: forwardComparison.TargetValueSetKey)
                ?? throw new Exception($"Could not resolve target ValueSet with Key: {forwardComparison.TargetValueSetKey} (`{forwardComparison.TargetCanonicalVersioned}`)");
            DoValueSetComparison(
                _vsComparisons,
                _conceptComparisons,
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

    public void DoValueSetComparison(
        DbComparisonCache<DbValueSetComparison> vsComparisons,
        DbComparisonCache<DbValueSetConceptComparison> conceptComparisons,
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
            inverseComparison = vsComparisons.Get((int)forwardComparison.InverseComparisonKey) ??
                DbValueSetComparison.SelectSingle(_db, Key: forwardComparison.InverseComparisonKey);
        }

        // look for an inverse comparison based on the source and target
        if ((inverseComparison == null) &&
            (forwardComparison.InverseComparisonKey == null))
        {
            inverseComparison = vsComparisons.Get(targetVs.Key, forwardComparison.SourceContentKey)
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
            vsComparisons.CacheAdd(inverseComparison);
        }

        // check to see if the inverse comparison key is set
        if (forwardComparison.InverseComparisonKey != inverseComparison.Key)
        {
            forwardComparison.InverseComparisonKey = inverseComparison.Key;
            vsComparisons.Changed(inverseComparison);
        }

        // process this comparison
        doValueSetConceptComparisons(
            conceptComparisons,
            sourcePackage,
            sourceVs,
            targetPackage,
            targetVs,
            forwardComparison,
            inverseComparison,
            forwardPair,
            reversePair);

        // check for identical system and code flags
        if ((forwardComparison.IsIdentical == null) || (forwardComparison.CodesAreIdentical == null))
        {
            // select only active and concrete concepts
            List<DbValueSetConcept> sourceConcepts = DbValueSetConcept.SelectList(_db, ValueSetKey: sourceVs.Key, Inactive: false, Abstract: false);
            List<DbValueSetConcept> targetConcepts = DbValueSetConcept.SelectList(_db, ValueSetKey: targetVs.Key, Inactive: false, Abstract: false);

            if (sourceConcepts.Count != targetConcepts.Count)
            {
                forwardComparison.IsIdentical = false;
                forwardComparison.CodesAreIdentical = false;
            }
            else
            {
                HashSet<string> targetCombined = targetConcepts.Select(c => c.FhirKey).ToHashSet();

                if (sourceConcepts.All(c => targetCombined.Contains(c.FhirKey)))
                {
                    forwardComparison.IsIdentical = true;
                    forwardComparison.CodesAreIdentical = true;
                }
                else
                {
                    HashSet<string> targetCodes = targetConcepts.Select(c => c.Code).ToHashSet();

                    if (sourceConcepts.All(c => targetCodes.Contains(c.Code)))
                    {
                        forwardComparison.IsIdentical = false;
                        forwardComparison.CodesAreIdentical = true;
                    }
                    else
                    {
                        forwardComparison.IsIdentical = false;
                        forwardComparison.CodesAreIdentical = false;
                    }
                }
            }

            vsComparisons.Changed(forwardComparison);
        }

        if ((inverseComparison.IsIdentical == null) || (inverseComparison.CodesAreIdentical == null))
        {
            inverseComparison.IsIdentical = forwardComparison.IsIdentical;
            inverseComparison.CodesAreIdentical = forwardComparison.CodesAreIdentical;

            vsComparisons.Changed(inverseComparison);
        }

        if (aggregateValueSetRelationships(forwardComparison, sourceVs, targetVs))
        {
            vsComparisons.Changed(forwardComparison);
        }

        if (aggregateValueSetRelationships(inverseComparison, targetVs, sourceVs))
        {
            vsComparisons.Changed(inverseComparison);
        }
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
        DbValueSet targetVs)
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

        DbComparisonCache<DbValueSetConceptComparison> changes = new();

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
                changes.CacheAdd(computedInverse);

                if (forwardConceptComparison.InverseComparisonKey != computedInverse.Key)
                {
                    forwardConceptComparison.InverseComparisonKey = computedInverse.Key;
                    changes.CacheUpdate(forwardConceptComparison);
                }

                continue;
            }

            usedInverseKeys.Add(existingInverse.Key);

            // check to see if the inverse comparison has the same relationship
            if (existingInverse.Relationship != computedInverse.Relationship)
            {
                existingInverse.Relationship = computedInverse.Relationship;
                changes.CacheUpdate(existingInverse);
            }
        }

        // flag we are deleting any inverse comparisons that are not used
        foreach ((int key, DbValueSetConceptComparison existing) in existingInverseConceptComparisons)
        {
            if (usedInverseKeys.Contains(key))
            {
                continue;
            }
            changes.CacheDelete(existing);
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
        changes.ComparisonsToAdd.Insert(_db);
        changes.ComparisonsToUpdate.Update(_db);
        changes.ComparisonsToDelete.Delete(_db);

        // return the comparison in case the caller needs it
        return (inverseComparsion, changes.Count != 0);
    }

    public void ApplyVsConceptChanges(
        List<DbGraphVs.DbVsConceptRow> originalProjection,
        IEnumerable<DbGraphVs.DbVsConceptRow> updatedProjection,
        int sourceColumnIndex,
        bool isComparingRight,
        string? reviewer)
    {
        DbComparisonCache<DbValueSetConceptComparison> changes = new();

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
                sourceToTargetComparison.Key = _comparisonDb.GetConceptComparisonKey();

                // cache the addition
                changes.CacheAdd(sourceToTargetComparison);
            }
            else
            {
                // cache as an update
                changes.CacheUpdate(sourceToTargetComparison);
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
                changes.CacheDelete(sourceToTargetComparison);
            }
        }

        // apply the changes to the concept comparisons
        changes.ComparisonsToAdd.Insert(_db);
        changes.ComparisonsToUpdate.Update(_db);
        changes.ComparisonsToDelete.Delete(_db);
    }

    private void doValueSetConceptComparisons(
        DbComparisonCache<DbValueSetConceptComparison> conceptComparisons,
        DbFhirPackage sourcePackage,
        DbValueSet sourceVs,
        DbFhirPackage targetPackage,
        DbValueSet targetVs,
        DbValueSetComparison forwardComparison,
        DbValueSetComparison reverseComparison,
        DbFhirPackageComparisonPair forwardPair,
        DbFhirPackageComparisonPair reversePair)
    {
        // select only active and concrete concepts
        List<DbValueSetConcept> sourceConcepts = DbValueSetConcept.SelectList(
            _db,
            ValueSetKey: sourceVs.Key,
            Inactive: false,
            Abstract: false);

        foreach (DbValueSetConcept sourceConcept in sourceConcepts)
        {
            // look in our cache for existing comparisons
            List<DbValueSetConceptComparison> comparisons = conceptComparisons.ForSource(sourceConcept.Key)
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
                        IsIdentical = isIdentical,
                        CodesAreIdentical = codeIsIdentical,
                    };

                    conceptComparisons.CacheAdd(comp);
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
                    IsIdentical = null,
                    CodesAreIdentical = null,
                };

                // insert into the database
                conceptComparisons.CacheAdd(noMap);

                // nothing else to do on this pass
                continue;
            }

            // iterate over the comparisons to check relationships
            foreach (DbValueSetConceptComparison conceptComparison in comparisons)
            {
                DbValueSetConcept? targetConcept = (conceptComparison.TargetConceptKey == null)
                    ? null
                    : DbValueSetConcept.SelectSingle(_db, Key: conceptComparison.TargetConceptKey);

                // if there is no target, there is nothing to check
                if (targetConcept == null)
                {
                    continue;
                }

                // get the list of all comparisons that source this ValueSet and target the same concept
                List<DbValueSetConceptComparison> targetComparisons = conceptComparisons.ForSource(targetConcept.Key)
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
                    inverseComparison = conceptComparisons.Get((int)conceptComparison.InverseComparisonKey) ??
                         DbValueSetConceptComparison.SelectSingle(_db, Key: conceptComparison.InverseComparisonKey);
                }

                if (inverseComparison == null)
                {
                    inverseComparison = conceptComparisons.Get(targetConcept.Key, conceptComparison.SourceContentKey)
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
                    conceptComparisons.CacheAdd(inverseComparison);

                    conceptComparison.InverseComparisonKey = inverseComparison.Key;
                    conceptComparisons.CacheUpdate(conceptComparison);
                }

                // do basic checks if this has not been reviewed
                if (conceptComparison.LastReviewedOn == null)
                {
                    // check for missing no-map value
                    if ((conceptComparison.TargetConceptKey == null) &&
                        (conceptComparison.NoMap != true))
                    {
                        conceptComparisons.CacheUpdate(conceptComparison);
                        conceptComparison.NoMap = true;
                    }

                    // check for incorrectly-flagged-as-equivalent escape-value code mappings
                    if ((targetConcept != null) &&
                        (conceptComparison.Relationship == CMR.Equivalent) &&
                        XVerProcessor._escapeValveCodes.Contains(sourceConcept.Code) &&
                        (sourceVs.ActiveConcreteConceptCount != targetVs.ActiveConcreteConceptCount))
                    {
                        conceptComparisons.CacheUpdate(conceptComparison);
                        conceptComparison.Relationship = RelationshipForCounts(sourceVs.ActiveConcreteConceptCount, targetVs.ActiveConcreteConceptCount);
                        conceptComparison.IsGenerated = true;
                        conceptComparison.Message = conceptComparison.Message +
                            $" Escape-valve code `{sourceConcept.Code}` maps to `{targetConcept.Code}`, but represent different concept domains (different number of codes).";
                    }

                    // check for a single source with multiple targets and any that map as equivalent
                    if ((conceptComparison.Relationship == CMR.Equivalent) &&
                        (targetComparisons.Count > 1))
                    {
                        conceptComparisons.CacheUpdate(conceptComparison);

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
                    if ((inverseComparison.LastReviewedOn == null) &&
                        (inverseComparison.Relationship != expected))
                    {
                        inverseComparison.Message = inverseComparison.Message +
                            $" Updated relationship from `{inverseComparison.Relationship}` to `{expected}`" +
                            $" based on the inverse comparsion {conceptComparison.Key}, which has a relationship" +
                            $" of `{conceptComparison.Relationship}`.";
                        inverseComparison.Relationship = expected;
                        conceptComparisons.CacheUpdate(inverseComparison);
                    }
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
                IsIdentical = other.IsIdentical,
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
            IsIdentical = other.IsIdentical,
        };
    }

    private DbElementComparison invert(
        DbElementComparison other,
        DbElement otherSourceElement,
        DbElement otherTargetElement,
        DbStructureComparison reverseCanonicalComparison,
        DbFhirPackageComparisonPair reversePair,
        DbComparisonCache<DbElementTypeComparison>? elementTypeComparisons = null)
    {
        int? inverseTypeComparisonKey = elementTypeComparisons?.Get(otherTargetElement.Key, otherSourceElement.Key)?.Key
            ?? DbElementTypeComparison.SelectSingle(_db, SourceElementKey: otherTargetElement.Key, TargetElementKey: otherSourceElement.Key)?.Key;
        int? boundValueSetComparisonKey = (otherSourceElement.BindingValueSetKey == null) || (otherTargetElement.BindingValueSetKey == null)
            ? null
            : _vsComparisons.Get((int)otherTargetElement.BindingValueSetKey, otherSourceElement.BindingValueSetKey)?.Key ??
                DbValueSetComparison.SelectSingle(
                    _db,
                    PackageComparisonKey: reversePair.Key,
                    SourceFhirPackageKey: other.TargetFhirPackageKey,
                    SourceValueSetKey: otherTargetElement.BindingValueSetKey,
                    TargetFhirPackageKey: other.SourceFhirPackageKey,
                    TargetValueSetKey: otherSourceElement.BindingValueSetKey)?.Key;

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
            ElementTypeComparisonKey = inverseTypeComparisonKey,
            BoundValueSetComparisonKey = boundValueSetComparisonKey,
            Relationship = invert(other.Relationship),
            ConceptDomainRelationship = invert(other.ConceptDomainRelationship),
            ValueDomainRelationship = invert(other.ValueDomainRelationship),
            NoMap = false,
            IsGenerated = true,
            LastReviewedBy = null,
            LastReviewedOn = null,
            Message = $"Mapping was inverted from Element comparison {other.Key}" +
                $" of `{otherSourceElement.Id}` -> `{otherTargetElement.Id}`",
            IsIdentical = other.IsIdentical,
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
            IsIdentical = other.IsIdentical,
            CodesAreIdentical = other.CodesAreIdentical,
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
            IsIdentical = other.IsIdentical,
            CodesAreIdentical = other.CodesAreIdentical,
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
