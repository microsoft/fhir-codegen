using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fhir.CodeGen.SQLiteGenerator;

namespace Fhir.CodeGen.LangSQLite.Models;

[CgSQLiteBaseClass]
public abstract class CgDbBase
{
    [CgSQLiteKey]
    public int Key { get; set; } = -1;

    //internal static int _indexValue = 0;
    //public static int GetIndex() => Interlocked.Increment(ref _indexValue);
}
