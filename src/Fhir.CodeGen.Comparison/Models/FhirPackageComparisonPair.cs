using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fhir.CodeGen.Comparison.Models;

public class FhirPackageComparisonPair
{
    public DbFhirPackage SourcePackage { get; init; }

    public int SourcePackageKey => SourcePackage.Key;
    public string SourcePackageShortName => SourcePackage.ShortName;
    public Fhir.CodeGen.Common.Packaging.FhirReleases.FhirSequenceCodes SourceFhirSequence => SourcePackage.DefinitionFhirSequence;

    public DbFhirPackage TargetPackage { get; init; }

    public int TargetPackageKey => TargetPackage.Key;
    public string TargetPackageShortName => TargetPackage.ShortName;
    public Fhir.CodeGen.Common.Packaging.FhirReleases.FhirSequenceCodes TargetFhirSequence => TargetPackage.DefinitionFhirSequence;

    public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;

    public DateTimeOffset? LastReviewedOn { get; set; } = null;
    public string? LastReviewedBy { get; set; } = null;

    public int SortKey => (SourcePackageKey * 100) + TargetPackageKey;

    [SetsRequiredMembers]
    public FhirPackageComparisonPair(DbFhirPackage sourcePackage, DbFhirPackage targetPackage)
    {
        SourcePackage = sourcePackage;
        TargetPackage = targetPackage;
    }

    public int Distance => Math.Abs(SourceFhirSequence - TargetFhirSequence);

    public static List<FhirPackageComparisonPair> GetPairs(List<DbFhirPackage> packages)
    {
        List<FhirPackageComparisonPair> pairs = [];
        for (int i = 0; i < packages.Count - 1; i++)
        {
            DbFhirPackage sourcePackage = packages[i];
            for (int j = i + 1; j < packages.Count; j++)
            {
                DbFhirPackage targetPackage = packages[j];
                FhirPackageComparisonPair pair = new(
                    sourcePackage: sourcePackage,
                    targetPackage: targetPackage);
                pairs.Add(pair);
            }
        }

        return pairs;
    }
}
