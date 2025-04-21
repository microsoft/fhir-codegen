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

public class ElementRecForUi
{
    public required DbElement Element {get; set;}
    public required string Text { get; set; }
    public required IQueryable<ElementProjectionForUi> ElementProjections {get; set;}
}

public class ElementProjectionForUi
{
    public required DbElementRow Row { get; set; }
    public required DbElementCell SourceCell { get; set; }
    public required DbElementCell TargetCell { get; set; }
    public required DbElementComparison Comparison { get; set; }

    public string RowKey => Row.RowNumber.ToString();
    public string DelKey => $"del-{Row.RowNumber}";
    public string RelKey => $"rel-{Row.RowNumber}";
    public string CdKey => $"rel-cd-{Row.RowNumber}";
    public string VdKey => $"rel-vd-{Row.RowNumber}";
    public string TargetKey => $"target-vd-{Row.RowNumber}";
}
