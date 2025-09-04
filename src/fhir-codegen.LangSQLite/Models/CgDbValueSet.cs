using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Hl7.Fhir.Model;
using fhir_codegen.SQLiteGenerator;

namespace fhir_codegen.LangSQLite.Models;


[CgSQLiteTable(tableName: "ValueSets")]
[CgSQLiteIndex(nameof(PackageKey), nameof(StrongestBindingCore))]
[CgSQLiteIndex(nameof(PackageKey), nameof(UnversionedUrl))]
[CgSQLiteIndex(nameof(PackageKey), nameof(Name))]
[CgSQLiteIndex(nameof(PackageKey), nameof(Id))]
public partial class CgDbValueSet : CgDbMetadataResourceBase
{
    public required bool CanExpand { get; set; }
    public required bool? HasEscapeValveCode { get; set; }
    public required string? Message { get; set; }
    public required bool IsExcluded { get; set; } = false;

    public required int ConceptCount { get; set; }
    public required int ActiveConcreteConceptCount { get; set; }
    public required string? ReferencedSystems { get; set; }

    public required int BindingCountCore { get; set; }
    public required Hl7.Fhir.Model.BindingStrength? StrongestBindingCore { get; set; }
    public required Hl7.Fhir.Model.BindingStrength? StrongestBindingCoreCode { get; set; }
    public required Hl7.Fhir.Model.BindingStrength? StrongestBindingCoreCoding { get; set; }

    public required int BindingCountExtended { get; set; }
    public required Hl7.Fhir.Model.BindingStrength? StrongestBindingExtended { get; set; }
    public required Hl7.Fhir.Model.BindingStrength? StrongestBindingExtendedCode { get; set; }
    public required Hl7.Fhir.Model.BindingStrength? StrongestBindingExtendedCoding { get; set; }

    public required Hl7.Fhir.Model.ValueSet.ComposeComponent? Compose { get; set; }

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

            return $"{Name}: {VersionedUrl}, Concepts: {ConceptCount}" +
                (string.IsNullOrEmpty(Description) ? string.Empty : " - " + Description);
        }
    }
}
