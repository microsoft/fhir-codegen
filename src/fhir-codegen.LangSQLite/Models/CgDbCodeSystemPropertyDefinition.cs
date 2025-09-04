using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fhir_codegen.SQLiteGenerator;

namespace fhir_codegen.LangSQLite.Models;

[CgSQLiteTable(tableName: "CodeSystemPropertyDefinitions")]
[CgSQLiteIndex(nameof(CodeSystemKey))]
public partial class CgDbCodeSystemPropertyDefinition : CgDbPackageContentBase
{
    [CgSQLiteForeignKey(referenceTable: "CodeSystems", referenceColumn: nameof(CgDbCodeSystem.Key))]
    public required int CodeSystemKey { get; set; }

    public required string Code { get; set; }
    public required string? Uri { get; set; }
    public required string? Description { get; set; }
    public required Hl7.Fhir.Model.CodeSystem.PropertyType Type { get; set; }
}
