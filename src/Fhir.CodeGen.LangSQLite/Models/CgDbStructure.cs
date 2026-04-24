using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Fhir.CodeGen.SQLiteGenerator;
using Hl7.Fhir.Model;

namespace Fhir.CodeGen.LangSQLite.Models;

[CgSQLiteTable(tableName: "Structures")]
[CgSQLiteIndex(nameof(PackageKey), nameof(ArtifactClass))]
[CgSQLiteIndex(nameof(PackageKey), nameof(Name))]
[CgSQLiteIndex(nameof(PackageKey), nameof(UnversionedUrl))]
[CgSQLiteIndex(nameof(PackageKey), nameof(Id))]
public partial class CgDbStructure : CgDbMetadataResourceBase
{
    public required string? Comment { get; set; }
    public required string? Message { get; set; }

    public required Fhir.CodeGen.Common.Models.FhirArtifactClassEnum ArtifactClass { get; set; } = Fhir.CodeGen.Common.Models.FhirArtifactClassEnum.Unknown;

    public required int SnapshotCount { get; set; }
    public required int DifferentialCount { get; set; }

    public required string? Implements { get; set; }

    public required string? Kind { get; set; }
    public bool? IsAbstract { get; set; }
    public required string? FhirType { get; set; }
    public required string? BaseDefinition { get; set; }
    public required string? BaseDefinitionShort { get; set; }
    public required string? Derivation { get; set; }

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
