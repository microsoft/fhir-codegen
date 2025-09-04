using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using fhir_codegen.SQLiteGenerator;
using Hl7.Fhir.Model;

namespace fhir_codegen.LangSQLite.Models;

[CgSQLiteTable(tableName: "Structures")]
[CgSQLiteIndex(nameof(PackageKey), nameof(ArtifactClass))]
[CgSQLiteIndex(nameof(PackageKey), nameof(Name))]
[CgSQLiteIndex(nameof(PackageKey), nameof(UnversionedUrl))]
[CgSQLiteIndex(nameof(PackageKey), nameof(Id))]
public partial class CgDbStructure : CgDbMetadataResourceBase
{
    public required string? Comment { get; set; }
    public required string? Message { get; set; }

    public required Microsoft.Health.Fhir.CodeGenCommon.Models.FhirArtifactClassEnum ArtifactClass { get; set; } = Microsoft.Health.Fhir.CodeGenCommon.Models.FhirArtifactClassEnum.Unknown;

    public required int SnapshotCount { get; set; }
    public required int DifferentialCount { get; set; }

    public required string? Implements { get; set; }

    [CgSQLiteIgnore]
    public string UiDisplay
    {
        get
        {
            if (string.IsNullOrEmpty(Name))
            {
                return "-";
            }

            return $"{Name}" +
                (string.IsNullOrEmpty(Title) ? string.Empty : " - " + Title);
        }
    }

    [CgSQLiteIgnore]
    public string UiDisplayLong
    {
        get
        {
            if (string.IsNullOrEmpty(Name))
            {
                return "-";
            }

            return $"{Name}, Snapshot: {SnapshotCount}, Diff: {DifferentialCount}" +
                (string.IsNullOrEmpty(Title) ? string.Empty : " - " + Title) +
                (string.IsNullOrEmpty(Description) ? string.Empty : " - " + Description);
        }
    }
}
