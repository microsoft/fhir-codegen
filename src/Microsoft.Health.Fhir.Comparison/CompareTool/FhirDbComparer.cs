using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Health.Fhir.Comparison.Models;

namespace Microsoft.Health.Fhir.Comparison.CompareTool;

public class FhirDbComparer
{
    private readonly ComparisonDatabase _comparisonDb;
    private readonly IDbConnection _db;

    public FhirDbComparer(ComparisonDatabase db)
    {
        _comparisonDb = db;
        _db = db.DbConnection;
    }

    public void RunAllComparisons()
    {
        HashSet<int> completedPairKeys = [];

        // iterate over all the comparison pairs
        foreach (DbFhirPackageComparisonPair dbPackagePair in DbFhirPackageComparisonPair.SelectList(_db))
        {
            // check to see if this package has already been processed
            if (completedPairKeys.Contains(dbPackagePair.Key))
            {
                continue;
            }

            completedPairKeys.Add(dbPackagePair.Key);

            // look for an inverse
            DbFhirPackageComparisonPair? dbInversePair = DbFhirPackageComparisonPair.SelectSingle(
                _db,
                SourcePackageKey: dbPackagePair.TargetPackageKey,
                TargetPackageKey: dbPackagePair.SourcePackageKey);

            // make sure the inverse exists
            if (dbInversePair == null)
            {
                dbInversePair = new()
                {
                    SourcePackageKey = dbPackagePair.TargetPackageKey,
                    TargetPackageKey = dbPackagePair.SourcePackageKey,
                };

                dbInversePair.Insert(_db);
            }

            completedPairKeys.Add(dbInversePair.Key);

            // grab our packages
            DbFhirPackage dbLeft = DbFhirPackage.SelectSingle(_db, Key: dbPackagePair.SourcePackageKey)
                ?? throw new Exception($"Inconsistent database, could not resolve FHIR Package {dbPackagePair.SourcePackageKey}");

            DbFhirPackage dbRight = DbFhirPackage.SelectSingle(_db, Key: dbPackagePair.TargetPackageKey)
                ?? throw new Exception($"Inconsistent database, could not resolve FHIR Package {dbPackagePair.TargetPackageKey}");

            // run this comparison
            compare(dbLeft, dbRight, dbPackagePair, dbInversePair);
        }
    }

    private void compare(
        DbFhirPackage rightDb,
        DbFhirPackage leftDb,
        DbFhirPackageComparisonPair pairLtoR,
        DbFhirPackageComparisonPair pairRtoL)
    {
        compareValueSets(rightDb, leftDb, pairLtoR, pairRtoL);
    }

    private void compareValueSets(
        DbFhirPackage leftDb,
        DbFhirPackage rightDb,
        DbFhirPackageComparisonPair pairLtoR,
        DbFhirPackageComparisonPair pairRtoL)
    {
        HashSet<int> completedComparisons = [];

        foreach ((List<DbValueSetComparison> comparisons, DbFhirPackage sourceDb, DbFhirPackage targetDb, DbFhirPackageComparisonPair pairStoT, DbFhirPackageComparisonPair pairTtoS) in getContents())
        {
            foreach (DbValueSetComparison comparisonForward in DbValueSetComparison.SelectList(_db, PackageComparisonKey: pairLtoR.Key))
            {
                // need two sides for a valid comparison
                if (comparisonForward.TargetValueSetKey == null)
                {
                    continue;
                }

                if (completedComparisons.Contains(comparisonForward.Key))
                {
                    continue;
                }

                completedComparisons.Add(comparisonForward.Key);

                DbValueSet forwardDbVs = DbValueSet.SelectSingle(
                    _db,
                    FhirPackageKey: comparisonForward.SourceFhirPackageKey,
                    Key: comparisonForward.SourceValueSetKey)
                    ?? throw new Exception($"Could not find ValueSet with Key: {comparisonForward.SourceValueSetKey} in package {comparisonForward.SourceFhirPackageKey}");

                DbValueSet reverseDbVs = DbValueSet.SelectSingle(
                    _db,
                    FhirPackageKey: comparisonForward.TargetFhirPackageKey,
                    Key: comparisonForward.TargetValueSetKey)
                    ?? throw new Exception($"Could not find ValueSet with Key: {comparisonForward.TargetValueSetKey} in package {comparisonForward.TargetFhirPackageKey}");

                // check for an inverse comparison
                DbValueSetComparison? comparisonReverse = DbValueSetComparison.SelectSingle(
                    _db,
                    PackageComparisonKey: pairTtoS.Key,
                    SourceValueSetKey: comparisonForward.TargetValueSetKey,
                    TargetValueSetKey: comparisonForward.SourceValueSetKey);

                if (comparisonReverse == null)
                {
                    comparisonReverse = invert(
                        pairStoT,
                        sourceDb,
                        forwardDbVs,
                        pairTtoS,
                        targetDb,
                        reverseDbVs);

                    comparisonReverse.Insert(_db);
                }

                completedComparisons.Add(comparisonReverse.Key);
            }
        }

        return;

        IEnumerable<(List<DbValueSetComparison> comparisons, DbFhirPackage sourceDb, DbFhirPackage targetDb, DbFhirPackageComparisonPair pairStoT, DbFhirPackageComparisonPair pairTtoS)> getContents() => [
            (DbValueSetComparison.SelectList(_db, PackageComparisonKey: pairLtoR.Key), leftDb, rightDb, pairLtoR, pairRtoL),
            (DbValueSetComparison.SelectList(_db, PackageComparisonKey: pairRtoL.Key), rightDb, leftDb, pairRtoL, pairLtoR),
            ];

        DbValueSetComparison invert(
            DbFhirPackageComparisonPair sourcePair,
            DbFhirPackage sourcePackage,
            DbValueSet sourceDbVs,
            DbFhirPackageComparisonPair targetPair,
            DbFhirPackage targetPackage,
            DbValueSet targetDbVs)
        {
            return new()
            {
                PackageComparisonKey = targetPair.Key,
                SourceFhirPackageKey = targetPair.SourcePackageKey,
                TargetFhirPackageKey = targetPair.TargetPackageKey,
                SourceValueSetKey = targetDbVs.Key,
                SourceCanonicalVersioned = targetDbVs.VersionedUrl,
                SourceCanonicalUnversioned = targetDbVs.UnversionedUrl,
                SourceVersion = targetDbVs.Version,
                SourceName = targetDbVs.Name,
                TargetValueSetKey = sourceDbVs.Key,
                TargetCanonicalVersioned = sourceDbVs.VersionedUrl,
                TargetCanonicalUnversioned = sourceDbVs.UnversionedUrl,
                TargetVersion = sourceDbVs.Version,
                TargetName = sourceDbVs.Name,
                CompositeName = ComparisonDatabase.GetCompositeName(targetPackage, targetDbVs, sourcePackage, sourceDbVs),
                SourceConceptMapUrl = null,
                SourceConceptMapAdditionalUrls = null,
                Relationship = null,
                IsGenerated = true,
                LastReviewedBy = null,
                LastReviewedOn = null,
                Message = $"Created by inverting existing ValueSet comparison {sourcePair.Key} of {sourceDbVs.VersionedUrl} -> {targetDbVs.VersionedUrl}",
            };
        }
    }
}
