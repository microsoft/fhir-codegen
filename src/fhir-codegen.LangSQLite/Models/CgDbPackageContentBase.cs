using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fhir_codegen.SQLiteGenerator;

namespace fhir_codegen.LangSQLite.Models;

[CgSQLiteBaseClass]
public abstract class CgDbPackageContentBase : CgDbBase
{
    [CgSQLiteForeignKey(referenceTable: "Packages", referenceColumn: nameof(CgDbPackage.Key))]
    public required int PackageKey { get; set; }
}
