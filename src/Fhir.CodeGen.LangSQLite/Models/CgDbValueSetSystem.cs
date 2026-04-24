using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fhir.CodeGen.SQLiteGenerator;


namespace Fhir.CodeGen.LangSQLite.Models;

[CgSQLiteTable(tableName: "ValueSetSystems")]
[CgSQLiteIndex(nameof(ValueSetKey), nameof(System), nameof(Version))]
[CgSQLiteIndex(nameof(CodeSystemKey))]
public partial class CgDbValueSetSystem : CgDbPackageContentBase, IEquatable<CgDbValueSetSystem>
{
    [CgSQLiteForeignKey(referenceTable: "ValueSets", referenceColumn: nameof(CgDbValueSet.Key))]
    public required int ValueSetKey { get; set; }
    public required string System { get; set; }
    public required string? Version { get; set; }

    [CgSQLiteForeignKey(referenceTable: "CodeSystems", referenceColumn: nameof(CgDbCodeSystem.Key))]
    public required int? CodeSystemKey { get; set; }

    [CgSQLiteIgnore]
    public bool IsEmpty => Key == -1 && string.IsNullOrEmpty(System) && string.IsNullOrEmpty(Version);

    private static CgDbValueSetSystem _empty = EmptyCopy;

    [CgSQLiteIgnore]
    public static CgDbValueSetSystem Empty => _empty;

    [CgSQLiteIgnore]
    public static CgDbValueSetSystem EmptyCopy => new()
    {
        Key = -1,
        PackageKey = -1,
        ValueSetKey = -1,
        System = string.Empty,
        Version = string.Empty,
        CodeSystemKey = null
    };


    bool IEquatable<CgDbValueSetSystem>.Equals(CgDbValueSetSystem? other)
    {
        if (other == null)
        {
            return false;
        }

        if (Key == -1)
        {
            return (System == other.System) && (Version == other.Version);
        }

        return Key == other.Key;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not CgDbValueSetSystem other)
        {
            return false;
        }

        return ((IEquatable<CgDbValueSetSystem>)this).Equals(other);
    }

    public override int GetHashCode()
    {
        if (Key == -1)
        {
            return HashCode.Combine(System, Version);
        }
        else
        {
            return Key;
        }
    }

}
