using System.Data;
using System.Diagnostics.CodeAnalysis;
using Fhir.CodeGen.SQLiteGenerator;

namespace Fhir.CodeGen.Comparison.Models;

public static class DbContentClasses
{
    public static void LoadIndices(IDbConnection db)
    {
        DbCodeSystem.LoadMaxKey(db);
        DbCodeSystemConcept.LoadMaxKey(db);
        DbCodeSystemConceptProperty.LoadMaxKey(db);
        DbCodeSystemPropertyDefinition.LoadMaxKey(db);
        DbCodeSystemFilter.LoadMaxKey(db);

        DbValueSet.LoadMaxKey(db);
        DbValueSetConcept.LoadMaxKey(db);

        DbStructureDefinition.LoadMaxKey(db);
        DbElement.LoadMaxKey(db);
        DbElementAdditionalBinding.LoadMaxKey(db);
        DbElementType.LoadMaxKey(db);

        DbExtensionSubstitution.LoadMaxKey(db);
        DbExternalInclusion.LoadMaxKey(db);
    }

    public static void DropTables(
        IDbConnection db,
        bool forTerminologies = true,
        bool forStructures = true)
    {
        if (forTerminologies)
        {
            DbCodeSystem.DropTable(db);
            DbCodeSystemConcept.DropTable(db);
            DbCodeSystemConceptProperty.DropTable(db);
            DbCodeSystemPropertyDefinition.DropTable(db);
            DbCodeSystemFilter.DropTable(db);

            DbValueSet.DropTable(db);
            DbValueSetConcept.DropTable(db);
        }

        if (forStructures)
        {
            DbStructureDefinition.DropTable(db);
            DbElement.DropTable(db);
            DbElementAdditionalBinding.DropTable(db);
            DbElementType.DropTable(db);

            DbExtensionSubstitution.DropTable(db);
            DbExternalInclusion.DropTable(db);
        }
    }

    public static void CreateTables(
        IDbConnection db,
        bool forTerminologies = true,
        bool forStructures = true)
    {
        if (forTerminologies)
        {
            DbCodeSystem.CreateTable(db);
            DbCodeSystemConcept.CreateTable(db);
            DbCodeSystemConceptProperty.CreateTable(db);
            DbCodeSystemPropertyDefinition.CreateTable(db);
            DbCodeSystemFilter.CreateTable(db);

            DbValueSet.CreateTable(db);
            DbValueSetConcept.CreateTable(db);
        }

        if (forStructures)
        {
            DbStructureDefinition.CreateTable(db);
            DbElement.CreateTable(db);
            DbElementAdditionalBinding.CreateTable(db);
            DbElementType.CreateTable(db);

            DbExtensionSubstitution.CreateTable(db);
            DbExternalInclusion.CreateTable(db);
        }
    }
}


[CgSQLiteTable(tableName: "ExtensionSubstitutions")]
[CgSQLiteIndex(nameof(SourceElementId))]
public partial class DbExtensionSubstitution : DbRecordBase
{
    public required string ReplacementUrl { get; set; }
    public required string SourceElementId { get; set; }
    public required Fhir.CodeGen.Common.Packaging.FhirReleases.FhirSequenceCodes SourceVersion { get; set; }
    public required string Context { get; set; }
}

[CgSQLiteTable(tableName: "ExternalInclusions")]
[CgSQLiteIndex(nameof(ResourceType))]
public partial class  DbExternalInclusion : DbRecordBase, IDbContentWithId
{
    public required Hl7.Fhir.Model.FHIRAllTypes ResourceType { get; set; }
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required string Version { get; set; }
    public required bool OverrideVersion { get; set; }
    public required string UnversionedUrl { get; set; }
    public required string VersionedUrl { get; set; }
    public required string Reason { get; set; }
    public required string? IncludeInPackages { get; set; }
    public required string Json { get; set; }

    public List<string> GetIncludeInPackagesList() => IncludeInPackages?
        .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
        .ToList()
        ?? [];

    public void SetIncludeInPackagesList(List<string> value)
    {
        if (value == null || value.Count == 0)
        {
            IncludeInPackages = null;
        }
        else
        {
            IncludeInPackages = string.Join(',', value);
        }
    }
}

[CgSQLiteTable(tableName: "ValueSets")]
[CgSQLiteIndex(nameof(FhirPackageKey), nameof(StrongestBindingCore))]
[CgSQLiteIndex(nameof(FhirPackageKey), nameof(UnversionedUrl))]
[CgSQLiteIndex(nameof(FhirPackageKey), nameof(Name))]
[CgSQLiteIndex(nameof(FhirPackageKey), nameof(Id))]
public partial class DbValueSet : DbMetadataResource, IDbContentWithId
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

    public required Hl7.Fhir.Model.ValueSet.ComposeComponent? Compose { get; set; }

    [CgSQLiteIgnore]
    public string UiDisplay
    {
        get
        {
            if (string.IsNullOrEmpty(Name))
            {
                return "-";
            }

            return $"{Name}: {VersionedUrl}";
        }
    }

    [CgSQLiteIgnore]
    public string UiDisplayLong
    {
        get
        {
            if (string.IsNullOrEmpty(Name))
            {
                return "-";
            }

            return $"{Name}: {VersionedUrl}, Concepts: {ConceptCount}" +
                (string.IsNullOrEmpty(Description) ? string.Empty : " - " + Description);
        }
    }
}

[CgSQLiteTable(tableName: "ValueSetConcepts")]
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

    public int? CodeSystemConceptKey { get; set; } = null;

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
public partial class DbStructureDefinition : DbMetadataResource, IDbContentWithId
{
    public required string? Comment { get; set; }
    public required string? Message { get; set; }

    public required Fhir.CodeGen.Common.Models.FhirArtifactClassEnum ArtifactClass { get; set; } = Fhir.CodeGen.Common.Models.FhirArtifactClassEnum.Unknown;

    public required int SnapshotCount { get; set; }
    public required int DifferentialCount { get; set; }

    public required string? Implements { get; set; }

    [CgSQLiteIgnore]
    public string UiDisplay
    {
        get
        {
            if (string.IsNullOrEmpty(Name))
            {
                return "-";
            }

            return $"{Name}" +
                (string.IsNullOrEmpty(Title) ? string.Empty : " - " + Title);
        }
    }

    [CgSQLiteIgnore]
    public string UiDisplayLong
    {
        get
        {
            if (string.IsNullOrEmpty(Name))
            {
                return "-";
            }

            return $"{Name}, Snapshot: {SnapshotCount}, Diff: {DifferentialCount}" +
                (string.IsNullOrEmpty(Title) ? string.Empty : " - " + Title) +
                (string.IsNullOrEmpty(Description) ? string.Empty : " - " + Description);
        }
    }
}


[CgSQLiteTable(tableName: "Elements")]
[CgSQLiteIndex(nameof(StructureKey))]
[CgSQLiteIndex(nameof(StructureKey), nameof(Id))]
[CgSQLiteIndex(nameof(StructureKey), nameof(Path))]
[CgSQLiteIndex(nameof(StructureKey), nameof(ResourceFieldOrder))]
[CgSQLiteIndex(nameof(ParentElementKey), nameof(ResourceFieldOrder))]
[CgSQLiteIndex(nameof(BindingValueSetKey))]
[CgSQLiteIndex(nameof(FhirPackageKey), nameof(Id))]
public partial class DbElement : DbPackageContent, IDbContentWithId
{
    [CgSQLiteForeignKey(referenceTable: "Structures", referenceColumn: nameof(DbStructureDefinition.Key))]
    public int StructureKey { get; set; }

    public required int? ParentElementKey { get; set; }

    public required int ResourceFieldOrder { get; set; }
    public required int ComponentFieldOrder { get; set; }
    public required string Id { get; set; }

    [CgSQLiteIgnore]
    public string StructureName => Id.Split('.').First();

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
    //public required string? FullCollatedReferenceTypesLiteral { get; set; }

    public required Hl7.Fhir.Model.BindingStrength? ValueSetBindingStrength { get; init; }
    public required string? BindingValueSet { get; set; }
    public required int? BindingValueSetKey { get; set; }
    public required int AdditionalBindingCount { get; set; }
    public required string? BindingDescription { get; set; }

    public required bool IsInherited { get; set; }
    public required string? BasePath { get; set; }

    [CgSQLiteForeignKey(referenceTable: "Elements", referenceColumn: nameof(DbElement.Key))]
    public required int? BaseElementKey { get; set; }

    [CgSQLiteForeignKey(referenceTable: "Structures", referenceColumn: nameof(DbStructureDefinition.Key))]
    public required int? BaseStructureKey { get; set; }

    public required bool DefinedAsContentReference { get; set; }
    public required bool IsSimpleType { get; set; }
    public required bool IsModifier { get; set; }
    public required string? IsModifierReason { get; set; }


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
        FullCollatedTypeLiteral = string.Empty,
        //FullCollatedReferenceTypesLiteral = null,
        ValueSetBindingStrength = null,
        BindingValueSet = null,
        BindingValueSetKey = null,
        BindingDescription = null,
        AdditionalBindingCount = 0,
        IsInherited = false,
        BasePath = null,
        BaseElementKey = null,
        BaseStructureKey = null,
        DefinedAsContentReference = false,
        IsSimpleType = false,
        IsModifier = false,
        IsModifierReason = null,
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

//[CgSQLiteTable(tableName: "CollatedTypes")]
//[CgSQLiteIndex(nameof(ElementKey))]
//[CgSQLiteIndex(nameof(ElementKey), nameof(TypeName))]
//public partial class DbCollatedType : DbPackageContent
//{
//    [CgSQLiteForeignKey(referenceTable: "Structures", referenceColumn: nameof(DbStructureDefinition.Key))]
//    public required int StructureKey { get; set; }

//    [CgSQLiteForeignKey(referenceTable: "Elements", referenceColumn: nameof(DbElement.Key))]
//    public required int ElementKey { get; set; }
//    public required string CollatedLiteral { get; set; }

//    [CgSQLiteForeignKey(referenceTable: "Structures", referenceColumn: nameof(DbStructureDefinition.Key))]
//    public required int? TypeStructureKey { get; set; }
//    public required string TypeName { get; set; }
//}

[CgSQLiteTable(tableName: "ElementTypes")]
[CgSQLiteIndex(nameof(ElementKey))]
//[CgSQLiteIndex(nameof(CollatedTypeKey))]
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

    //[CgSQLiteForeignKey(referenceTable: "CollatedTypes", referenceColumn: nameof(DbCollatedType.Key))]
    //public required int CollatedTypeKey { get; set; }

    public required string? TypeName { get; set; }
    [CgSQLiteForeignKey(referenceTable: "Structures", referenceColumn: nameof(DbStructureDefinition.Key))]
    public required int? TypeStructureKey { get; set; }


    public required string? TypeProfile { get; set; }
    [CgSQLiteForeignKey(referenceTable: "Structures", referenceColumn: nameof(DbStructureDefinition.Key))]
    public required int? TypeProfileStructureKey { get; set; }

    public required string? TargetProfile { get; set; }
    [CgSQLiteForeignKey(referenceTable: "Structures", referenceColumn: nameof(DbStructureDefinition.Key))]
    public required int? TargetProfileStructureKey { get; set; }

    [CgSQLiteIgnore]
    public string Literal =>
        (string.IsNullOrEmpty(TypeName) ? string.Empty : TypeName) +
        (string.IsNullOrEmpty(TypeProfile) ? string.Empty : $"[{TypeProfile}]") +
        (string.IsNullOrEmpty(TargetProfile) ? string.Empty : $"({TargetProfile})");

}

[CgSQLiteTable(tableName: "CodeSystems")]
[CgSQLiteIndex(nameof(FhirPackageKey), nameof(UnversionedUrl))]
[CgSQLiteIndex(nameof(FhirPackageKey), nameof(Name))]
[CgSQLiteIndex(nameof(FhirPackageKey), nameof(Id))]
public partial class DbCodeSystem : DbMetadataResource, IDbContentWithId
{
    public required bool? IsCaseSensitive { get; set; }
    public required string? ValueSetVersioned { get; set; }
    public required string? ValueSetUnversioned { get; set; }
    public required Hl7.Fhir.Model.CodeSystem.CodeSystemHierarchyMeaning? HierarchyMeaning { get; set; }
    public required bool? IsCompositional { get; set; }
    public required bool? VersionNeeded { get; set; }
    public required Hl7.Fhir.Model.CodeSystemContentMode? Content { get; set; }
    public required string? SupplementsVersioned { get; set; }
    public required string? SupplementsUnversioned { get; set; }
    public required int? Count { get; set; }

    [CgSQLiteIgnore]
    public string UiDisplay
    {
        get
        {
            if (string.IsNullOrEmpty(Name))
            {
                return "-";
            }
            return $"{Name}: {VersionedUrl}";
        }
    }
    [CgSQLiteIgnore]
    public string UiDisplayLong
    {
        get
        {
            if (string.IsNullOrEmpty(Name))
            {
                return "-";
            }
            return $"{Name}: {VersionedUrl}, Concepts: {Count}" +
                (string.IsNullOrEmpty(Description) ? string.Empty : " - " + Description);
        }
    }
}

[CgSQLiteTable(tableName: "CodeSystemConcepts")]
[CgSQLiteIndex(nameof(CodeSystemKey))]
[CgSQLiteIndex(nameof(CodeSystemKey), nameof(FlatOrder))]
public partial class DbCodeSystemConcept : DbPackageContent
{
    [CgSQLiteForeignKey(referenceTable: "CodeSystems", referenceColumn: nameof(DbCodeSystem.Key))]
    public required int CodeSystemKey { get; set; }
    public required int FlatOrder { get; set; }
    public required int RelativeOrder { get; set; }
    public required string Code { get; set; }
    public required string? Display { get; set; }
    public required string? Definition { get; set; }
    public required List<Hl7.Fhir.Model.CodeSystem.DesignationComponent> Designations { get; set; }
    public required List<Hl7.Fhir.Model.CodeSystem.ConceptPropertyComponent> Properties { get; set; }
    public required int? ParentConceptKey { get; set; }
    public required int ChildConceptCount { get; set; }
}

[CgSQLiteTable(tableName: "CodeSystemCodeProperties")]
[CgSQLiteIndex(nameof(CodeSystemPropertyDefinitionKey))]
public partial class DbCodeSystemConceptProperty : DbPackageContent
{
    [CgSQLiteForeignKey(referenceTable: "CodeSystemConcepts", referenceColumn: nameof(DbCodeSystemConcept.Key))]
    public required int CodeSystemConceptKey { get; set; }

    [CgSQLiteForeignKey(referenceTable: "CodeSystemPropertyDefinitions", referenceColumn: nameof(DbCodeSystemPropertyDefinition.Key))]
    public required int CodeSystemPropertyDefinitionKey { get; set; }

    public required string Code { get; set; }
    public required Hl7.Fhir.Model.CodeSystem.PropertyType Type { get; set; }
    public required string Value { get; set; }
}

[CgSQLiteTable(tableName: "CodeSystemPropertyDefinitions")]
[CgSQLiteIndex(nameof(CodeSystemKey))]
public partial class DbCodeSystemPropertyDefinition : DbPackageContent
{
    [CgSQLiteForeignKey(referenceTable: "CodeSystems", referenceColumn: nameof(DbCodeSystem.Key))]
    public required int CodeSystemKey { get; set; }
    public required string Code { get; set; }
    public required string? Uri { get; set; }
    public required string? Description { get; set; }
    public required Hl7.Fhir.Model.CodeSystem.PropertyType Type { get; set; }
}

[CgSQLiteTable(tableName: "CodeSystemFilters")]
[CgSQLiteIndex(nameof(CodeSystemKey))]
public partial class DbCodeSystemFilter : DbPackageContent
{
    [CgSQLiteForeignKey(referenceTable: "CodeSystems", referenceColumn: nameof(DbCodeSystem.Key))]
    public required int CodeSystemKey { get; set; }
    public required string Code { get; set; }
    public required string? Description { get; set; }
    public required string Operators { get; set; }
    public required string Value { get; set; }
}

public interface IDbContentWithId
{
    int Key { get; set; }
    string Id { get; set; }
}

public class DbContentWithIdCache<T>
        where T : IDbContentWithId
{
    private readonly Dictionary<int, T> _byKey = [];
    private readonly Dictionary<string, T> _byId = [];

    private readonly Dictionary<int, T> _toAdd = [];
    private readonly Dictionary<int, T> _toUpdate = [];
    private readonly Dictionary<int, T> _toDelete = [];

    public bool TryGet(int key, [NotNullWhen(true)] out T? value) => _byKey.TryGetValue(key, out value);
    public bool TryGet(string id, [NotNullWhen(true)] out T? value) => _byId.TryGetValue(id, out value);

    public T? Get(int key) => _byKey.TryGetValue(key, out T? value) ? value : default(T);
    public T? Get(string id) => _byId.TryGetValue(id, out T? value) ? value : default(T);

    public IEnumerable<T> Values => _byKey.Values;
    public int ValueCount => _byKey.Count;

    public IEnumerable<T> ToAdd => _toAdd.Values;
    public int ToAddCount => _toAdd.Count;
    public IEnumerable<T> ToUpdate => _toUpdate.Values;
    public int ToUpdateCount => _toUpdate.Count;
    public IEnumerable<T> ToDelete => _toDelete.Values;
    public int ToDeleteCount => _toDelete.Count;

    public void Clear()
    {
        _byKey.Clear();
        _byId.Clear();
        _toAdd.Clear();
        _toUpdate.Clear();
        _toDelete.Clear();
    }

    public void CacheAdd(T item)
    {
        if (item.Id is null)
        {
            throw new ArgumentException("Cannot cache item with null Id.");
        }

        _byKey[item.Key] = item;
        _byId[item.Id] = item;
        _toAdd[item.Key] = item;
    }

    public void CacheUpdate(T item)
    {
        _byKey[item.Key] = item;
        _byId[item.Id] = item;
        _toUpdate[item.Key] = item;
    }

    public void CacheDelete(T item)
    {
        _byKey[item.Key] = item;
        _byId[item.Id] = item;
        _toDelete[item.Key] = item;
    }

    public void Changed(T item)
    {
        if (!_byKey.ContainsKey(item.Key))
        {
            _byKey[item.Key] = item;
        }

        if (!_byId.ContainsKey(item.Id))
        {
            _byId[item.Id] = item;
        }

        if (_toDelete.ContainsKey(item.Key))
        {
            _toDelete.Remove(item.Key);
        }

        if (_toUpdate.ContainsKey(item.Key))
        {
            _toUpdate[item.Key] = item;
            return;
        }

        _toAdd[item.Key] = item;
    }

    public int Count => _byKey.Count;
}
