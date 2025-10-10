using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fhir.CodeGen.SQLiteGenerator;

namespace Fhir.CodeGen.LangSQLite.Models;

[CgSQLiteTable(tableName: "Elements")]
[CgSQLiteIndex(nameof(StructureKey))]
[CgSQLiteIndex(nameof(StructureKey), nameof(Id))]
[CgSQLiteIndex(nameof(StructureKey), nameof(Path))]
[CgSQLiteIndex(nameof(StructureKey), nameof(ResourceFieldOrder))]
[CgSQLiteIndex(nameof(ParentElementKey), nameof(ResourceFieldOrder))]
[CgSQLiteIndex(nameof(BindingValueSetKey))]
public partial class CgDbElement : CgDbPackageContentBase
{
    [CgSQLiteForeignKey(referenceTable: "Structures", referenceColumn: nameof(CgDbStructure.Key))]
    public int StructureKey { get; set; }

    public required int? ParentElementKey { get; set; }

    public required int ResourceFieldOrder { get; set; }
    public required int ComponentFieldOrder { get; set; }
    public required string Id { get; set; }
    public required string Path { get; set; }
    public required int ChildElementCount { get; set; }
    public required string Name { get; set; }
    public required string? Short { get; set; }
    public required string? Definition { get; set; }
    public required int MinCardinality { get; set; }
    public required int MaxCardinality { get; set; }
    public required string MaxCardinalityString { get; set; }

    [CgSQLiteIgnore]
    public string FhirCardinalityString => $"{MinCardinality}..{MaxCardinalityString}";

    public required string? SliceName { get; set; }

    public required string FullCollatedTypeLiteral { get; set; }

    public required Hl7.Fhir.Model.BindingStrength? ValueSetBindingStrength { get; init; }
    public required string? BindingValueSet { get; set; }
    public required int? BindingValueSetKey { get; set; }
    public required int AdditionalBindingCount { get; set; }
    public required string? BindingDescription { get; set; }

    public required bool IsInherited { get; set; }
    public required string? BasePath { get; set; }
    [CgSQLiteForeignKey(referenceTable: "Elements", referenceColumn: nameof(CgDbElement.Key))]
    public required int? BaseElementKey { get; set; }
    [CgSQLiteForeignKey(referenceTable: "Structures", referenceColumn: nameof(CgDbStructure.Key))]
    public required int? BaseStructureKey { get; set; }

    public required bool IsSimpleType { get; set; }
    public required bool IsModifier { get; set; }
    public required string? IsModifierReason { get; set; }

    public required string? StandardStatus { get; set; }


    [CgSQLiteIgnore]
    public string UiDisplay
    {
        get
        {
            if (string.IsNullOrEmpty(Id))
            {
                return "-";
            }

            return $"{Id}" +
                (string.IsNullOrEmpty(Short) ? string.Empty : " - " + Short);
        }
    }

    [CgSQLiteIgnore]
    public string UiDisplayWithType
    {
        get
        {
            if (string.IsNullOrEmpty(Id))
            {
                return "-";
            }

            return $"{Id} ({MinCardinality}..{MaxCardinalityString}, {FullCollatedTypeLiteral.Replace("http://hl7.org/fhir/StructureDefinition/", string.Empty)})";
        }
    }

    [CgSQLiteIgnore]
    public string UiDisplayLong
    {
        get
        {
            if (string.IsNullOrEmpty(Id))
            {
                return "-";
            }

            return $"{Id} ({MinCardinality}..{MaxCardinalityString}, {FullCollatedTypeLiteral.Replace("http://hl7.org/fhir/StructureDefinition/", string.Empty)})" +
                (string.IsNullOrEmpty(Short) ? string.Empty : " - " + Short);
        }
    }

    private static CgDbElement _empty = EmptyCopy;

    [CgSQLiteIgnore]
    public bool IsEmpty => Key == -1 && string.IsNullOrEmpty(Id) && string.IsNullOrEmpty(Path);

    [CgSQLiteIgnore]
    public static CgDbElement Empty => _empty;

    [CgSQLiteIgnore]
    public static CgDbElement EmptyCopy => new()
    {
        Key = -1,
        PackageKey = -1,
        StructureKey = -1,
        ParentElementKey = null,
        ResourceFieldOrder = -1,
        ComponentFieldOrder = -1,
        Id = string.Empty,
        Path = string.Empty,
        ChildElementCount = 0,
        Name = string.Empty,
        Short = null,
        Definition = null,
        MinCardinality = 0,
        MaxCardinality = 0,
        MaxCardinalityString = string.Empty,
        SliceName = null,
        FullCollatedTypeLiteral = string.Empty,
        ValueSetBindingStrength = null,
        BindingValueSet = null,
        BindingValueSetKey = null,
        BindingDescription = null,
        AdditionalBindingCount = 0,
        IsInherited = false,
        BasePath = null,
        BaseElementKey = null,
        BaseStructureKey = null,
        IsSimpleType = false,
        IsModifier = false,
        IsModifierReason = null,
        StandardStatus = null,
    };
}
