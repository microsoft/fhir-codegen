using Microsoft.Health.Fhir.Comparison.Models;
using System.Diagnostics.CodeAnalysis;

namespace xver_editor.Models;

public record class SdRecForUi
{
    public required DbGraphSd Graph { get; init; }
    public required DbStructureDefinition Sd { get; init; }
    public required List<DbSdRow> Projection { get; init; }
    public required Dictionary<int, bool> MapsTo { get; init; }
    public required bool HasUnreviewed { get; init; }
    public required bool AllIdentical { get; init; }

    [SetsRequiredMembers]
    public SdRecForUi(System.Data.IDbConnection db, DbStructureDefinition sd, List<DbFhirPackage> packages)
    {
        Sd = sd;
        Graph = new()
        {
            DB = db,
            Packages = packages,
            KeySd = sd,
        };
        Projection = Graph.Project();

        bool hasUnreviewed = false;
        bool allIdentical = true;

        Dictionary<int, bool> mapsTo = [];
        for (int i = 0; i < packages.Count; i++)
        {
            if (packages[i].Key == sd.FhirPackageKey)
            {
                if (hasUnreviewed == true)
                {
                    continue;
                }

                if ((i > 0) &&
                    Projection.Any(r => (r[i]?.LeftComparison != null) && (r[i]!.LeftComparison!.LastReviewedOn == null)))
                {
                    hasUnreviewed = true;
                }

                if ((i < packages.Count - 1) &&
                    Projection.Any(r => (r[i]?.RightComparison != null) && (r[i]!.RightComparison!.LastReviewedOn == null)))
                {
                    hasUnreviewed = true;
                }

                if ((i > 0) &&
                    Projection.Any(r => (r[i]?.LeftComparison != null) && (r[i]!.LeftComparison!.IsIdentical != true)))
                {
                    allIdentical = false;
                }

                if ((i < packages.Count - 1) &&
                    Projection.Any(r => (r[i]?.RightComparison != null) && (r[i]!.RightComparison!.IsIdentical != true)))
                {
                    allIdentical = false;
                }


                continue;
            }

            mapsTo.Add(packages[i].Key, Projection.Any(r => r[i] != null) ? true : false);
        }

        MapsTo = mapsTo;
        HasUnreviewed = hasUnreviewed;
        AllIdentical = allIdentical;
    }
}
