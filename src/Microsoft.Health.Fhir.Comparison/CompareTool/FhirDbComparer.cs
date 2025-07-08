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
        public required List<DbElementType> UnderlyingElementTypes { get; init; }

        public CollatedType() { }

        [SetsRequiredMembers]
        public CollatedType(IEnumerable<DbElementType> types)
        {
            UnderlyingElementTypes = types is List<DbElementType> tl ? tl : types.ToList();
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

    private DbComparisonCache<DbValueSetComparison> _vsComparisonCache = new();
    private DbComparisonCache<DbValueSetConceptComparison> _conceptComparisonCache = new();

    private DbComparisonCache<DbStructureComparison> _sdComparisonCache = new();
    private DbComparisonCache<DbElementComparison> _edComparisonCache = new();
    private DbComparisonCache<DbCollatedTypeComparison> _collatedTypeComparisonCache = new();
    private DbComparisonCache<DbElementTypeComparison> _typeComparisonCache = new();

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
        HashSet<int>? comparisonPairFilterSet = null,
        bool allowUpdates = true)
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
                _vsComparisonCache.Clear();
                _conceptComparisonCache.Clear();

                List<DbValueSet> valueSets = DbValueSet.SelectList(_db, FhirPackageKey: sourcePackage.Key);
                _logger.LogInformation($" <<< processing {sourcePackage.FhirVersionShort} ValueSets, count: {valueSets.Count}");

                // iterate over value sets in the package
                foreach (DbValueSet sourceVs in valueSets)
                {
                    // if we are not allowing updates, we need to see if this value set has been compared before
                    if (allowUpdates == false)
                    {
                        int matchCount = DbValueSetComparison.SelectCount(
                            _db,
                            SourceFhirPackageKey: sourcePackage.Key,
                            SourceValueSetKey: sourceVs.Key);

                        if (matchCount != 0)
                        {
                            continue;
                        }
                    }

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
                _vsComparisonCache.ComparisonsToAdd.Insert(_db);
                _vsComparisonCache.ComparisonsToUpdate.Update(_db);
                _conceptComparisonCache.ComparisonsToAdd.Insert(_db);
                _conceptComparisonCache.ComparisonsToUpdate.Update(_db);
            }

            // any structure triggers all of them
            if ((artifactFilter == null) ||
                (artifactFilter == FhirArtifactClassEnum.PrimitiveType) ||
                (artifactFilter == FhirArtifactClassEnum.ComplexType) ||
                (artifactFilter == FhirArtifactClassEnum.Resource) ||
                (artifactFilter == FhirArtifactClassEnum.Profile) ||
                (artifactFilter == FhirArtifactClassEnum.LogicalModel))
            {
                _sdComparisonCache.Clear();
                _edComparisonCache.Clear();
                _collatedTypeComparisonCache.Clear();
                _typeComparisonCache.Clear();

                // iterate over our artifact types
                foreach (FhirArtifactClassEnum artifactClass in getArtifactClassSequence())
                {
                    List<DbStructureDefinition> structures = DbStructureDefinition.SelectList(_db, FhirPackageKey: sourcePackage.Key, ArtifactClass: artifactClass);
                    _logger.LogInformation($" <<< processing Structures:{artifactClass}, count: {structures.Count}");

                    // iterate over the structures in the package
                    foreach (DbStructureDefinition sourceSd in structures)
                    {
                        // if we are not allowing updates, we need to see if this structure has been compared before
                        if (allowUpdates == false)
                        {
                            int matchCount = DbStructureComparison.SelectCount(
                                _db,
                                SourceFhirPackageKey: sourcePackage.Key,
                                SourceStructureKey: sourceSd.Key);

                            if (matchCount != 0)
                            {
                                continue;
                            }
                        }

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
                _sdComparisonCache.ComparisonsToAdd.Insert(_db);
                _sdComparisonCache.ComparisonsToUpdate.Update(_db);
                _edComparisonCache.ComparisonsToAdd.Insert(_db);
                _edComparisonCache.ComparisonsToUpdate.Update(_db);
                _collatedTypeComparisonCache.ComparisonsToAdd.Insert(_db);
                _collatedTypeComparisonCache.ComparisonsToUpdate.Update(_db);
                _typeComparisonCache.ComparisonsToAdd.Insert(_db);
                _typeComparisonCache.ComparisonsToUpdate.Update(_db);
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
