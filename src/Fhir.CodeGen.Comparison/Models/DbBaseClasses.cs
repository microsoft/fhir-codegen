using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fhir.CodeGen.SQLiteGenerator;

namespace Fhir.CodeGen.Comparison.Models;

[CgSQLiteBaseClass]
public abstract class DbRecordBase
{
    [CgSQLiteKey]
    public int Key { get; set; } = -1;

    //internal static int _indexValue = 0;
    //public static int GetIndex() => Interlocked.Increment(ref _indexValue);
}


[CgSQLiteBaseClass]
public abstract class DbPackageContent : DbRecordBase
{
    [CgSQLiteForeignKey(referenceTable: "FhirPackages", referenceColumn: nameof(DbFhirPackage.Key))]
    public required int FhirPackageKey { get; set; }
}

[CgSQLiteBaseClass]
public abstract class DbResourceRelatedContentBase : DbPackageContent
{
    public required int ResourceKey { get; set; }
    public required int Order { get; set; }
}
