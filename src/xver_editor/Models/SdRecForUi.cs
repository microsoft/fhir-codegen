using Microsoft.Health.Fhir.Comparison.Models;
using System.Diagnostics.CodeAnalysis;

namespace xver_editor.Models;

public record class SdRecForUi
{
    public required DbGraphSd Graph { get; init; }
    public required DbStructureDefinition Sd { get; init; }
    public required List<DbGraphSd.DbSdRow> Projection { get; init; }
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
        Projection = Graph.BuildProjection();

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

    public string GetFhirUrl()
    {
        DbFhirPackage package = Graph.Packages.First(p => p.Key == Sd.FhirPackageKey);

        string fhirUrlRoot = package.FhirVersionShort switch
        {
            "1.0" => "http://hl7.org/fhir/DSTU2/",
            "3.0" => "http://hl7.org/fhir/STU3/",
            "4.0" => "http://hl7.org/fhir/R4/",
            "4.3" => "http://hl7.org/fhir/R4B/",
            "5.0" => "http://hl7.org/fhir/R5/",
            "6.0" => "http://build.fhir.org/",
            _ => throw new NotImplementedException($"Fhir version {package.FhirVersionShort} not implemented"),
        };

        switch (Sd.ArtifactClass)
        {
            case Microsoft.Health.Fhir.CodeGenCommon.Models.FhirArtifactClassEnum.PrimitiveType:
                {
                    return fhirUrlRoot + "datatypes.html#" + Sd.Name;
                }

            case Microsoft.Health.Fhir.CodeGenCommon.Models.FhirArtifactClassEnum.ComplexType:
                {
                    return fhirUrlRoot + "metadatatypes.html#" + Sd.Name;
                }

            case Microsoft.Health.Fhir.CodeGenCommon.Models.FhirArtifactClassEnum.Resource:
                {
                    return fhirUrlRoot + Sd.Name + ".html";
                }
        }

        return Sd.UnversionedUrl.Replace("http://hl7.org/fhir/", fhirUrlRoot);
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
    public required DbGraphSd.DbElementRow Row { get; set; }
    public required DbGraphSd.DbElementCell SourceCell { get; set; }
    public required DbGraphSd.DbElementCell TargetCell { get; set; }
    public required DbElementComparison Comparison { get; set; }

    public string RowKey => Row.RowId.ToString();
    public string DelKey => $"del-{Row.RowId}";
    public string RelKey => $"rel-{Row.RowId}";
    public string CdKey => $"rel-cd-{Row.RowId}";
    public string VdKey => $"rel-vd-{Row.RowId}";
    public string TargetKey => $"target-vd-{Row.RowId}";
}
