using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fhir_codegen.SQLiteGenerator;

namespace fhir_codegen.LangSQLite.Models;

[CgSQLiteTable(tableName: "ElementCollatedTypes")]
[CgSQLiteIndex(nameof(ElementKey))]
[CgSQLiteIndex(nameof(ElementKey), nameof(TypeName))]
public partial class CgDbElementCollatedType : CgDbPackageContentBase
{
    [CgSQLiteForeignKey(referenceTable: "Structures", referenceColumn: nameof(CgDbStructure.Key))]
    public required int StructureKey { get; set; }

    [CgSQLiteForeignKey(referenceTable: "Elements", referenceColumn: nameof(CgDbElement.Key))]
    public required int ElementKey { get; set; }
    public required string CollatedLiteral { get; set; }

    [CgSQLiteForeignKey(referenceTable: "Structures", referenceColumn: nameof(CgDbStructure.Key))]
    public required int? TypeStructureKey { get; set; }
    public required string TypeName { get; set; }
}
