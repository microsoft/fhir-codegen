using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fhir.CodeGen.SQLiteGenerator;

namespace Fhir.CodeGen.LangSQLite.Models;

[CgSQLiteTable(tableName: "CodeSystemConcepts")]
[CgSQLiteIndex(nameof(CodeSystemKey))]
[CgSQLiteIndex(nameof(CodeSystemKey), nameof(FlatOrder))]
public partial class CgDbCodeSystemConcept : CgDbPackageContentBase
{
    [CgSQLiteForeignKey(referenceTable: "CodeSystems", referenceColumn: nameof(CgDbCodeSystem.Key))]
    public required int CodeSystemKey { get; set; }

    public required int FlatOrder { get; set; }
    public required int RelativeOrder { get; set; }
    public required string Code { get; set; }
    public required string? Display { get; set; }
    public required string? Definition { get; set; }
    public required List<Hl7.Fhir.Model.CodeSystem.DesignationComponent> Designations { get; set; }
    public required List<Hl7.Fhir.Model.CodeSystem.ConceptPropertyComponent> Properties { get; set; }
    public required int? ParentConceptKey { get; set; }
    public required int ChildConceptCount { get; set; }
}
