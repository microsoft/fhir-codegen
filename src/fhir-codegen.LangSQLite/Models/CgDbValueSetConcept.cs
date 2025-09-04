using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fhir_codegen.SQLiteGenerator;

namespace fhir_codegen.LangSQLite.Models;

[CgSQLiteTable(tableName: "ValueSetConcepts")]
[CgSQLiteIndex(nameof(ValueSetKey), nameof(Inactive), nameof(Abstract))]
[CgSQLiteIndex(nameof(ValueSetKey), nameof(System), nameof(Code))]
public partial class CgDbValueSetConcept : CgDbPackageContentBase, IEquatable<CgDbValueSetConcept>
{
    [CgSQLiteForeignKey(referenceTable: "ValueSets", referenceColumn: nameof(CgDbValueSet.Key))]
    public required int ValueSetKey { get; set; }
    public required string System { get; set; }
    public required string SystemVersion { get; set; }
    public required string Code { get; set; }
    public required string? Display { get; set; }
    public required bool Inactive { get; set; }
    public required bool Abstract { get; set; }
    public required string? Properties { get; set; }

    [CgSQLiteIgnore]
    public string FhirKey => $"{System}#{Code}";

    [CgSQLiteIgnore]
    public string UiDisplay => (string.IsNullOrEmpty(Code) && string.IsNullOrEmpty(System))
        ? "-"
        : $"{Code} ({System})";

    [CgSQLiteIgnore]
    public string UiDisplayLong
    {
        get
        {
            if (string.IsNullOrEmpty(Code) && string.IsNullOrEmpty(System))
            {
                return "-";
            }

            return $"{Code} ({System}): {Display}" +
                (Inactive ? " *Inactive*" : string.Empty) +
                (Abstract ? " *Abstract*" : string.Empty);
        }
    }

    private static CgDbValueSetConcept _empty = EmptyCopy;

    [CgSQLiteIgnore]
    public bool IsEmpty => Key == -1 && string.IsNullOrEmpty(System) && string.IsNullOrEmpty(Code) && string.IsNullOrEmpty(Display);

    [CgSQLiteIgnore]
    public static CgDbValueSetConcept Empty => _empty;

    [CgSQLiteIgnore]
    public static CgDbValueSetConcept EmptyCopy => new()
    {
        Key = -1,
        PackageKey = -1,
        ValueSetKey = -1,
        System = string.Empty,
        SystemVersion = string.Empty,
        Code = string.Empty,
        Display = "No Concept Selected",
        Inactive = false,
        Abstract = false,
        Properties = string.Empty,
    };

    bool IEquatable<CgDbValueSetConcept>.Equals(CgDbValueSetConcept? other)
    {
        if (other == null)
        {
            return false;
        }

        if (Key == -1)
        {
            return (System == other.System) && (Code == other.Code);
        }

        return Key == other.Key;
    }

    public override bool Equals(object? obj)
    {
        if (obj is not CgDbValueSetConcept other)
        {
            return false;
        }

        return ((IEquatable<CgDbValueSetConcept>)this).Equals(other);
    }

    public override int GetHashCode()
    {
        if (Key == -1)
        {
            return HashCode.Combine(System, Code);
        }
        else
        {
            return Key;
        }
    }
}
