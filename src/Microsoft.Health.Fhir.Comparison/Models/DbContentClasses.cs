using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using fhir_codegen.SQLiteGenerator;
using Hl7.Fhir.ElementModel.Types;
using Microsoft.Data.Sqlite;


namespace Microsoft.Health.Fhir.Comparison.Models;

[CgSQLiteBaseClass]
public abstract class DbPackageContent
{
    [CgSQLiteKey]
    public int Key { get; set; } = -1;

    [CgSQLiteForeignKey(referenceTable: "FhirPackages", referenceColumn: nameof(DbFhirPackage.Key))]
    public required int FhirPackageKey { get; set; }
}

[CgSQLiteBaseClass]
public abstract class DbCanonicalResource : DbPackageContent
{
    public required string Id { get; set; }
    public required string VersionedUrl { get; set; }
    public required string UnversionedUrl { get; set; }
    public required string Name { get; set; }
    public required string Version { get; set; }
    public required Hl7.Fhir.Model.PublicationStatus? Status { get; set; }
    public required string? Title { get; set; }
    public required string? Description { get; set; }
    public required string? Purpose { get; set; }
}


[CgSQLiteTable(tableName: "ValueSets")]
[CgSQLiteIndex(nameof(FhirPackageKey), nameof(StrongestBindingCore))]
[CgSQLiteIndex(nameof(FhirPackageKey), nameof(UnversionedUrl))]
[CgSQLiteIndex(nameof(FhirPackageKey), nameof(Name))]
[CgSQLiteIndex(nameof(FhirPackageKey), nameof(Id))]
public partial class DbValueSet : DbCanonicalResource
{
    public required bool CanExpand { get; set; }
    public required bool? HasEscapeValveCode { get; set; }
    public required string? Message { get; set; }
    public required bool IsExcluded { get; set; } = false;

    public required int ConceptCount { get; set; }
    public required int ActiveConcreteConceptCount { get; set; }
    public required string? ReferencedSystems { get; set; }

    public required int BindingCountCore { get; set; }
    public required Hl7.Fhir.Model.BindingStrength? StrongestBindingCore { get; set; }
    public required Hl7.Fhir.Model.BindingStrength? StrongestBindingCoreCode { get; set; }
    public required Hl7.Fhir.Model.BindingStrength? StrongestBindingCoreCoding { get; set; }

    public required int BindingCountExtended { get; set; }
    public required Hl7.Fhir.Model.BindingStrength? StrongestBindingExtended { get; set; }
    public required Hl7.Fhir.Model.BindingStrength? StrongestBindingExtendedCode { get; set; }
    public required Hl7.Fhir.Model.BindingStrength? StrongestBindingExtendedCoding { get; set; }
}

[CgSQLiteTable(tableName: "Concepts")]
[CgSQLiteIndex(nameof(ValueSetKey), nameof(Inactive), nameof(Abstract))]
[CgSQLiteIndex(nameof(ValueSetKey), nameof(System), nameof(Code))]
public partial class DbValueSetConcept : DbPackageContent, IEquatable<DbValueSetConcept>
{
    [CgSQLiteForeignKey(referenceTable: "ValueSets", referenceColumn: nameof(DbValueSet.Key))]
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

            return $"{Code} ({System}) - {Display}" +
                (Inactive ? " *Inactive*" : string.Empty) +
                (Abstract ? " *Abstract*" : string.Empty);
        }
    }

    private static DbValueSetConcept _empty = EmptyCopy;

    [CgSQLiteIgnore]
    public bool IsEmpty => Key == -1 && string.IsNullOrEmpty(System) && string.IsNullOrEmpty(Code) && string.IsNullOrEmpty(Display);

    [CgSQLiteIgnore]
    public static DbValueSetConcept Empty => _empty;

    [CgSQLiteIgnore]
    public static DbValueSetConcept EmptyCopy => new()
    {
        Key = -1,
        FhirPackageKey = -1,
        ValueSetKey = -1,
        System = string.Empty,
        SystemVersion = string.Empty,
        Code = string.Empty,
        Display = "No Concept Selected",
        Inactive = false,
        Abstract = false,
        Properties = string.Empty,
    };

    bool IEquatable<DbValueSetConcept>.Equals(DbValueSetConcept? other)
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
        if (obj is not DbValueSetConcept other)
        {
            return false;
        }

        return ((IEquatable<DbValueSetConcept>)this).Equals(other);
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

[CgSQLiteTable(tableName: "Structures")]
[CgSQLiteIndex(nameof(FhirPackageKey), nameof(ArtifactClass))]
[CgSQLiteIndex(nameof(FhirPackageKey), nameof(Name))]
[CgSQLiteIndex(nameof(FhirPackageKey), nameof(UnversionedUrl))]
[CgSQLiteIndex(nameof(FhirPackageKey), nameof(Id))]
public partial class DbStructureDefinition : DbCanonicalResource
{
    public required string? Comment { get; set; }
    public required string? Message { get; set; }

    public required Microsoft.Health.Fhir.CodeGenCommon.Models.FhirArtifactClassEnum ArtifactClass { get; set; } = Microsoft.Health.Fhir.CodeGenCommon.Models.FhirArtifactClassEnum.Unknown;

    public required int SnapshotCount { get; set; }
    public required int DifferentialCount { get; set; }
}


[CgSQLiteTable(tableName: "Elements")]
[CgSQLiteIndex(nameof(StructureKey))]
[CgSQLiteIndex(nameof(StructureKey), nameof(Id))]
[CgSQLiteIndex(nameof(StructureKey), nameof(Path))]
[CgSQLiteIndex(nameof(BindingValueSetKey))]
public partial class DbElement : DbPackageContent
{
    [CgSQLiteForeignKey(referenceTable: "Structures", referenceColumn: nameof(DbStructureDefinition.Key))]
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

    public required string CollatedTypeLiteral { get; set; }

    public required Hl7.Fhir.Model.BindingStrength? ValueSetBindingStrength { get; init; }
    public required string? BindingValueSet { get; set; }
    public required int? BindingValueSetKey { get; set; }
    public required int AdditionalBindingCount { get; set; }

    public required bool IsInherited { get; set; }
    public required string? BasePath { get; set; }
    public required bool IsSimpleType { get; set; }


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

            return $"{Id} ({MinCardinality}..{MaxCardinalityString}, {CollatedTypeLiteral})";
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

            return $"{Id} ({MinCardinality}..{MaxCardinalityString}, {CollatedTypeLiteral})" +
                (string.IsNullOrEmpty(Short) ? string.Empty : " - " + Short);
        }
    }

    private static DbElement _empty = EmptyCopy;

    [CgSQLiteIgnore]
    public bool IsEmpty => Key == -1 && string.IsNullOrEmpty(Id) && string.IsNullOrEmpty(Path);

    [CgSQLiteIgnore]
    public static DbElement Empty => _empty;

    [CgSQLiteIgnore]
    public static DbElement EmptyCopy => new()
    {
        Key = -1,
        FhirPackageKey = -1,
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
        CollatedTypeLiteral = string.Empty,
        ValueSetBindingStrength = null,
        BindingValueSet = null,
        BindingValueSetKey = null,
        AdditionalBindingCount = 0,
        IsInherited = false,
        BasePath = null,
        IsSimpleType = false,
    };
}

[CgSQLiteTable(tableName: "ElementAdditionalBindings")]
[CgSQLiteIndex(nameof(ElementKey))]
public partial class DbElementAdditionalBinding : DbPackageContent
{
    [CgSQLiteForeignKey(referenceTable: "Structures", referenceColumn: nameof(DbStructureDefinition.Key))]
    public int StructureKey { get; set; }

    [CgSQLiteForeignKey(referenceTable: "Elements", referenceColumn: nameof(DbElement.Key))]
    public required int ElementKey { get; set; }

    public required string? FhirKey { get; set; }
    public required Hl7.Fhir.Model.ElementDefinition.AdditionalBindingPurposeVS? Purpose { get; set; }
    public required string? BindingValueSet { get; set; }
    public required int? BindingValueSetKey { get; set; }
    public required string? Documentation { get; set; }
    public required string? ShortDocumentation { get; set; }
    public required string? CollatedUsageContexts { get; set; }
    public required bool? SatisfiedBySingleRepetition { get; set; }
}

[CgSQLiteTable(tableName: "ElementTypes")]
[CgSQLiteIndex(nameof(ElementKey))]
[CgSQLiteIndex(nameof(ElementKey), nameof(TypeName))]
[CgSQLiteIndex(nameof(ElementKey), nameof(TypeName), nameof(TypeProfile), nameof(TargetProfile))]
[CgSQLiteIndex(nameof(TypeName))]
[CgSQLiteIndex(nameof(TypeName), nameof(TypeProfile), nameof(TargetProfile))]
public partial class DbElementType : DbPackageContent
{
    [CgSQLiteForeignKey(referenceTable: "Structures", referenceColumn: nameof(DbStructureDefinition.Key))]
    public required int StructureKey { get; set; }

    [CgSQLiteForeignKey(referenceTable: "Elements", referenceColumn: nameof(DbElement.Key))]
    public required int ElementKey { get; set; }

    public required string? TypeName { get; set; }
    public required string? TypeProfile { get; set; }
    public required string? TargetProfile { get; set; }

    [CgSQLiteForeignKey(referenceTable: "Structures", referenceColumn: nameof(DbStructureDefinition.Key))]
    public required int? TypeStructureKey { get; set; }

    [CgSQLiteIgnore]
    public string Literal =>
        (string.IsNullOrEmpty(TypeName) ? string.Empty : TypeName) +
        (string.IsNullOrEmpty(TypeProfile) ? string.Empty : $"[{TypeProfile}]") +
        (string.IsNullOrEmpty(TargetProfile) ? string.Empty : $"({TargetProfile})");
}
