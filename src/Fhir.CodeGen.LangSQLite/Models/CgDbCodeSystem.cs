using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Fhir.CodeGen.SQLiteGenerator;
using Hl7.Fhir.Model;

namespace Fhir.CodeGen.LangSQLite.Models;

[CgSQLiteTable(tableName: "CodeSystems")]
[CgSQLiteIndex(nameof(PackageKey), nameof(UnversionedUrl))]
[CgSQLiteIndex(nameof(PackageKey), nameof(Name))]
[CgSQLiteIndex(nameof(PackageKey), nameof(Id))]
public partial class CgDbCodeSystem : CgDbMetadataResourceBase
{
    public required bool? IsCaseSensitive { get; set; }
    public required string? ValueSetVersioned { get; set; }
    public required string? ValueSetUnversioned { get; set; }
    public required Hl7.Fhir.Model.CodeSystem.CodeSystemHierarchyMeaning? HierarchyMeaning { get; set; }
    public required bool? IsCompositional { get; set; }
    public required bool? VersionNeeded { get; set; }
    public required Hl7.Fhir.Model.CodeSystemContentMode? Content { get; set; }
    public required string? SupplementsVersioned { get; set; }
    public required string? SupplementsUnversioned { get; set; }
    public required int? Count { get; set; }

    [CgSQLiteIgnore]
    public string UiDisplay
    {
        get
        {
            if (string.IsNullOrEmpty(Name))
            {
                return "-";
            }
            return $"{Name}: {VersionedUrl}";
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
            return $"{Name}: {VersionedUrl}, Concepts: {Count}" +
                (string.IsNullOrEmpty(Description) ? string.Empty : " - " + Description);
        }
    }
}
