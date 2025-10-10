using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fhir.CodeGen.SQLiteGenerator;

namespace Fhir.CodeGen.LangSQLite.Models;

[CgSQLiteTable(tableName: "SearchParameterComponents")]
[CgSQLiteIndex(nameof(PackageKey), nameof(DefinitionCanonical))]
[CgSQLiteIndex(nameof(PackageKey), nameof(SearchParameterKey))]
[CgSQLiteIndex(nameof(SearchParameterKey))]
public partial class CgDbSearchParameterComponent : CgDbPackageContentBase
{
    [CgSQLiteForeignKey(referenceTable: "SearchParameters", referenceColumn: nameof(CgDbSearchParameter.Key))]
    public required int SearchParameterKey { get; set; }

    public required string DefinitionCanonical { get; set; }

    public required string Expression { get; set; }
}
