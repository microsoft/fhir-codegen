using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fhir.CodeGen.SQLiteGenerator;

namespace Fhir.CodeGen.LangSQLite.Models;

[CgSQLiteTable(tableName: "CodeSystemFilters")]
[CgSQLiteIndex(nameof(CodeSystemKey))]
public partial class CgDbCodeSystemFilter : CgDbPackageContentBase
{
    [CgSQLiteForeignKey(referenceTable: "CodeSystems", referenceColumn: nameof(CgDbCodeSystem.Key))]
    public required int CodeSystemKey { get; set; }

    public required string Code { get; set; }
    public required string? Description { get; set; }
    public required string Operators { get; set; }
    public required string Value { get; set; }
}
