using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fhir.CodeGen.SQLiteGenerator;

namespace Fhir.CodeGen.LangSQLite.Models;

[CgSQLiteTable(tableName: "ElementTypes")]
[CgSQLiteIndex(nameof(ElementKey))]
[CgSQLiteIndex(nameof(ElementKey), nameof(TypeName))]
[CgSQLiteIndex(nameof(ElementKey), nameof(TypeName), nameof(TypeProfile), nameof(TargetProfile))]
[CgSQLiteIndex(nameof(TypeName))]
[CgSQLiteIndex(nameof(TypeName), nameof(TypeProfile), nameof(TargetProfile))]
public partial class CgDbElementType : CgDbPackageContentBase
{
    [CgSQLiteForeignKey(referenceTable: "Structures", referenceColumn: nameof(CgDbStructure.Key))]
    public required int StructureKey { get; set; }

    [CgSQLiteForeignKey(referenceTable: "Elements", referenceColumn: nameof(CgDbElement.Key))]
    public required int ElementKey { get; set; }

    [CgSQLiteForeignKey(referenceTable: "CollatedTypes", referenceColumn: nameof(CgDbElementCollatedType.Key))]
    public required int CollatedTypeKey { get; set; }

    public required string? TypeName { get; set; }
    public required string? TypeProfile { get; set; }
    public required string? TargetProfile { get; set; }

    [CgSQLiteForeignKey(referenceTable: "Structures", referenceColumn: nameof(CgDbStructure.Key))]
    public required int? TypeStructureKey { get; set; }

    [CgSQLiteIgnore]
    public string Literal =>
        (string.IsNullOrEmpty(TypeName) ? string.Empty : TypeName) +
        (string.IsNullOrEmpty(TypeProfile) ? string.Empty : $"[{TypeProfile}]") +
        (string.IsNullOrEmpty(TargetProfile) ? string.Empty : $"({TargetProfile})");

}
