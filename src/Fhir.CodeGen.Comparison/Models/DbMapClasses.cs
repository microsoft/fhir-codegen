using Fhir.CodeGen.SQLiteGenerator;
using System.Diagnostics.CodeAnalysis;
using System.Data;


namespace Fhir.CodeGen.Comparison.Models;


[CgSQLiteBaseClass]
public abstract class DbMapRecordBase : DbRecordBase
{
    [CgSQLiteForeignKey(referenceTable: "FhirPackages", referenceColumn: nameof(DbFhirPackage.Key))]
    public required int SourceFhirPackageKey { get; set; }

    [CgSQLiteForeignKey(referenceTable: "FhirPackages", referenceColumn: nameof(DbFhirPackage.Key))]
    public required int TargetFhirPackageKey { get; set; }


    public required Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship? Relationship { get; set; }

    [CgSQLiteIgnore]
    public bool IsNotMapped => Relationship is null;

    public required bool ExplicitNoMap { get; set; }

    public string? Comments { get; set; } = null;
    public string? TechnicalNotes { get; set; } = null;
}

[CgSQLiteBaseClass]
public abstract class DbMapArtifactRecordBase : DbMapRecordBase
{
    public string? OriginatingConceptMapUrlsLiteral { get; set; } = null;
    [CgSQLiteIgnore]
    public List<string>? OriginatingConceptMapUrls
    {
        get => OriginatingConceptMapUrlsLiteral?.Split(", ").ToList();
        set => OriginatingConceptMapUrlsLiteral = value is null ? null : string.Join(", ", value);
    }

    public string IdLong { get; set; } = string.Empty;
    public string? IdShort { get; set; } = null;
    public string? Url { get; set; } = null;
    public string? Name { get; set; } = null;
    public string? Title { get; set; } = null;
}

[CgSQLiteTable(tableName: "ValueSetMappings")]
[CgSQLiteIndex(nameof(IdLong))]
public partial class DbValueSetMappingRecord : DbMapArtifactRecordBase      //, IDbMapArtifactRecord
{
    [CgSQLiteForeignKey(referenceTable: "ValueSets", referenceColumn: nameof(DbValueSet.Key))]
    public required int SourceValueSetKey { get; set; }
    public required string SourceValueSetId { get; set; }

    [CgSQLiteForeignKey(referenceTable: "ValueSets", referenceColumn: nameof(DbValueSet.Key))]
    public required int? TargetValueSetKey { get; set; }
    public required string? TargetValueSetId { get; set; }


    [CgSQLiteForeignKey(referenceTable: "ValueSets", referenceColumn: nameof(DbValueSet.Key))]
    public int? ValueSetKeyR2 { get; set; } = null;

    [CgSQLiteForeignKey(referenceTable: "ValueSets", referenceColumn: nameof(DbValueSet.Key))]
    public int? ValueSetKeyR3 { get; set; } = null;

    [CgSQLiteForeignKey(referenceTable: "ValueSets", referenceColumn: nameof(DbValueSet.Key))]
    public int? ValueSetKeyR4 { get; set; } = null;

    [CgSQLiteForeignKey(referenceTable: "ValueSets", referenceColumn: nameof(DbValueSet.Key))]
    public int? ValueSetKeyR4B { get; set; } = null;

    [CgSQLiteForeignKey(referenceTable: "ValueSets", referenceColumn: nameof(DbValueSet.Key))]
    public int? ValueSetKeyR5 { get; set; } = null;

    [CgSQLiteForeignKey(referenceTable: "ValueSets", referenceColumn: nameof(DbValueSet.Key))]
    public int? ValueSetKeyR6 { get; set; } = null;

    [CgSQLiteIgnore]
    public int?[] ValueSetKeys
    {
        get => [ValueSetKeyR2, ValueSetKeyR3, ValueSetKeyR4, ValueSetKeyR4B, ValueSetKeyR5, ValueSetKeyR6];
        set
        {
            if (value.Length == 5)
            {
                ValueSetKeyR2 = value[0];
                ValueSetKeyR3 = value[1];
                ValueSetKeyR4 = value[2];
                ValueSetKeyR4B = value[3];
                ValueSetKeyR5 = value[4];
                ValueSetKeyR6 = null;
                return;
            }
            if (value.Length == 6)
            {
                ValueSetKeyR2 = value[0];
                ValueSetKeyR3 = value[1];
                ValueSetKeyR4 = value[2];
                ValueSetKeyR4B = value[3];
                ValueSetKeyR5 = value[4];
                ValueSetKeyR6 = value[5];
                return;
            }
            throw new ArgumentException($"Invalid number of ValueSet keys: {value.Length}. Expected 5 or 6.");
        }
    }
}

[CgSQLiteTable(tableName: "ValueSetConceptMappings")]
[CgSQLiteIndex(nameof(ValueSetMappingKey), nameof(SourceValueSetConceptKey), nameof(TargetValueSetConceptKey))]
public partial class DbValueSetConceptMappingRecord : DbMapRecordBase
{
    [CgSQLiteForeignKey(referenceTable: "ValueSetMappings", referenceColumn: nameof(DbValueSetMappingRecord.Key))]
    public required int ValueSetMappingKey { get; set; }

    [CgSQLiteForeignKey(referenceTable: "ValueSetConcepts", referenceColumn: nameof(DbValueSetConcept.Key))]
    public required int SourceValueSetConceptKey { get; set; }

    [CgSQLiteForeignKey(referenceTable: "ValueSetConcepts", referenceColumn: nameof(DbValueSetConcept.Key))]
    public required int? TargetValueSetConceptKey { get; set; }


    [CgSQLiteForeignKey(referenceTable: "ValueSetConcepts", referenceColumn: nameof(DbValueSetConcept.Key))]
    public int? ValueSetConceptKeyR2 { get; set; } = null;

    [CgSQLiteForeignKey(referenceTable: "ValueSetConcepts", referenceColumn: nameof(DbValueSetConcept.Key))]
    public int? ValueSetConceptKeyR3 { get; set; } = null;

    [CgSQLiteForeignKey(referenceTable: "ValueSetConcepts", referenceColumn: nameof(DbValueSetConcept.Key))]
    public int? ValueSetConceptKeyR4 { get; set; } = null;

    [CgSQLiteForeignKey(referenceTable: "ValueSetConcepts", referenceColumn: nameof(DbValueSetConcept.Key))]
    public int? ValueSetConceptKeyR4B { get; set; } = null;

    [CgSQLiteForeignKey(referenceTable: "ValueSetConcepts", referenceColumn: nameof(DbValueSetConcept.Key))]
    public int? ValueSetConceptKeyR5 { get; set; } = null;

    [CgSQLiteForeignKey(referenceTable: "ValueSetConcepts", referenceColumn: nameof(DbValueSetConcept.Key))]
    public int? ValueSetConceptKeyR6 { get; set; } = null;

    [CgSQLiteIgnore]
    public int?[] ValueSetConceptKeys
    {
        get => [ValueSetConceptKeyR2, ValueSetConceptKeyR3, ValueSetConceptKeyR4, ValueSetConceptKeyR4B, ValueSetConceptKeyR5, ValueSetConceptKeyR6];
        set
        {
            if (value.Length == 5)
            {
                ValueSetConceptKeyR2 = value[0];
                ValueSetConceptKeyR3 = value[1];
                ValueSetConceptKeyR4 = value[2];
                ValueSetConceptKeyR4B = value[3];
                ValueSetConceptKeyR5 = value[4];
                ValueSetConceptKeyR6 = null;
                return;
            }
            if (value.Length == 6)
            {
                ValueSetConceptKeyR2 = value[0];
                ValueSetConceptKeyR3 = value[1];
                ValueSetConceptKeyR4 = value[2];
                ValueSetConceptKeyR4B = value[3];
                ValueSetConceptKeyR5 = value[4];
                ValueSetConceptKeyR6 = value[5];
                return;
            }
            throw new ArgumentException($"Invalid number of ValueSet concept keys: {value.Length}. Expected 5 or 6.");
        }
    }
}


[CgSQLiteTable(tableName: "StructureMappings")]
[CgSQLiteIndex(nameof(SourceStructureKey), nameof(TargetStructureKey))]
[CgSQLiteIndex(nameof(SourceFhirPackageKey), nameof(SourceStructureKey), nameof(TargetFhirPackageKey), nameof(TargetStructureKey))]
[CgSQLiteIndex(nameof(SourceFhirPackageKey), nameof(SourceStructureKey), nameof(TargetFhirPackageKey), nameof(TargetStructureId))]
[CgSQLiteIndex(nameof(SourceFhirPackageKey), nameof(SourceStructureId), nameof(TargetFhirPackageKey), nameof(TargetStructureId))]
[CgSQLiteIndex(nameof(IdLong))]
public partial class DbStructureMappingRecord : DbMapArtifactRecordBase
{
    [CgSQLiteForeignKey(referenceTable: "Structures", referenceColumn: nameof(DbStructureDefinition.Key))]
    public required int SourceStructureKey { get; set; }
    public required string SourceStructureId { get; set; }

    [CgSQLiteForeignKey(referenceTable: "Structures", referenceColumn: nameof(DbStructureDefinition.Key))]
    public required int? TargetStructureKey { get; set; }
    public required string? TargetStructureId { get; set; }

    public required bool? FmlExists { get; set; }
    public required string? FmlUrl { get; set; }
    public required string? FmlFilename { get; set; }


    [CgSQLiteForeignKey(referenceTable: "Structures", referenceColumn: nameof(DbStructureDefinition.Key))]
    public int? StructureKeyR2 { get; set; } = null;

    [CgSQLiteForeignKey(referenceTable: "Structures", referenceColumn: nameof(DbStructureDefinition.Key))]
    public int? StructureKeyR3 { get; set; } = null;

    [CgSQLiteForeignKey(referenceTable: "Structures", referenceColumn: nameof(DbStructureDefinition.Key))]
    public int? StructureKeyR4 { get; set; } = null;

    [CgSQLiteForeignKey(referenceTable: "Structures", referenceColumn: nameof(DbStructureDefinition.Key))]
    public int? StructureKeyR4B { get; set; } = null;

    [CgSQLiteForeignKey(referenceTable: "Structures", referenceColumn: nameof(DbStructureDefinition.Key))]
    public int? StructureKeyR5 { get; set; } = null;

    [CgSQLiteForeignKey(referenceTable: "Structures", referenceColumn: nameof(DbStructureDefinition.Key))]
    public int? StructureKeyR6 { get; set; } = null;

    [CgSQLiteIgnore]
    public int?[] StructureKeys
    {
        get => [StructureKeyR2, StructureKeyR3, StructureKeyR4, StructureKeyR4B, StructureKeyR5, StructureKeyR6];
        set
        {
            if (value.Length == 5)
            {
                StructureKeyR2 = value[0];
                StructureKeyR3 = value[1];
                StructureKeyR4 = value[2];
                StructureKeyR4B = value[3];
                StructureKeyR5 = value[4];
                StructureKeyR6 = null;
                return;
            }

            if (value.Length == 6)
            {
                StructureKeyR2 = value[0];
                StructureKeyR3 = value[1];
                StructureKeyR4 = value[2];
                StructureKeyR4B = value[3];
                StructureKeyR5 = value[4];
                StructureKeyR6 = value[5];
                return;
            }

            throw new ArgumentException($"Invalid number of structure keys: {value.Length}. Expected 5 or 6.");
        }
    }


    //[CgSQLiteIgnore]
    //public int SourceArtifactKey => SourceStructureKey;
    //[CgSQLiteIgnore]
    //public int? TargetArtifactKey => TargetStructureKey;
}

public enum ChangeIndicationCodes : int
{
    Narrowed = -1,
    NoChange = 0,
    Broadened = 1,
}

[CgSQLiteTable(tableName: "ElementMappings")]
[CgSQLiteIndex(nameof(StructureMappingKey), nameof(SourceElementKey), nameof(TargetElementKey))]
[CgSQLiteIndex(nameof(SourceFhirPackageKey), nameof(TargetFhirPackageKey), nameof(SourceElementId), nameof(TargetElementId))]
public partial class DbElementMappingRecord : DbMapRecordBase
{
    [CgSQLiteForeignKey(referenceTable: "StructureMappings", referenceColumn: nameof(DbStructureMappingRecord.Key))]
    public required int StructureMappingKey { get; set; }

    [CgSQLiteForeignKey(referenceTable: "Elements", referenceColumn: nameof(DbElement.Key))]
    public required int? SourceElementKey { get; set; }

    public required string SourceElementId { get; set; }

    [CgSQLiteForeignKey(referenceTable: "Elements", referenceColumn: nameof(DbElement.Key))]
    public required int? TargetElementKey { get; set; }

    public required string? TargetElementId { get; set; }


    [CgSQLiteForeignKey(referenceTable: "Elements", referenceColumn: nameof(DbElement.Key))]
    public int? ElementKeyR2 { get; set; } = null;

    [CgSQLiteForeignKey(referenceTable: "Elements", referenceColumn: nameof(DbElement.Key))]
    public int? ElementKeyR3 { get; set; } = null;

    [CgSQLiteForeignKey(referenceTable: "Elements", referenceColumn: nameof(DbElement.Key))]
    public int? ElementKeyR4 { get; set; } = null;

    [CgSQLiteForeignKey(referenceTable: "Elements", referenceColumn: nameof(DbElement.Key))]
    public int? ElementKeyR4B { get; set; } = null;

    [CgSQLiteForeignKey(referenceTable: "Elements", referenceColumn: nameof(DbElement.Key))]
    public int? ElementKeyR5 { get; set; } = null;

    [CgSQLiteForeignKey(referenceTable: "Elements", referenceColumn: nameof(DbElement.Key))]
    public int? ElementKeyR6 { get; set; } = null;

    [CgSQLiteIgnore]
    public int?[] ElementKeys
    {
        get => [ElementKeyR2, ElementKeyR3, ElementKeyR4, ElementKeyR4B, ElementKeyR5, ElementKeyR6];
        set
        {
            if (value.Length == 5)
            {
                ElementKeyR2 = value[0];
                ElementKeyR3 = value[1];
                ElementKeyR4 = value[2];
                ElementKeyR4B = value[3];
                ElementKeyR5 = value[4];
                ElementKeyR6 = null;
                return;
            }
            if (value.Length == 6)
            {
                ElementKeyR2 = value[0];
                ElementKeyR3 = value[1];
                ElementKeyR4 = value[2];
                ElementKeyR4B = value[3];
                ElementKeyR5 = value[4];
                ElementKeyR6 = value[5];
                return;
            }
            throw new ArgumentException($"Invalid number of element keys: {value.Length}. Expected 5 or 6.");
        }
    }

    public string? OriginatingConceptMapUrlsLiteral { get; set; } = null;
    [CgSQLiteIgnore]
    public List<string>? OriginatingConceptMapUrls
    {
        get => OriginatingConceptMapUrlsLiteral?.Split(", ").ToList();
        set => OriginatingConceptMapUrlsLiteral = value is null ? null : string.Join(", ", value);
    }

    public string? OriginatingFmlUrlsLiteral { get; set; } = null;
    [CgSQLiteIgnore]
    public List<string>? OriginatingFmlUrls
    {
        get => OriginatingFmlUrlsLiteral?.Split(", ").ToList();
        set => OriginatingFmlUrlsLiteral = value is null ? null : string.Join(", ", value);
    }
}


public class DbArtifactMapCache<T>
        where T : DbMapArtifactRecordBase
{
    private readonly Dictionary<int, T> _byKey = [];
    private readonly Dictionary<string, T> _byIdLong = [];

    private readonly Dictionary<int, T> _toAdd = [];
    private readonly Dictionary<int, T> _toUpdate = [];
    private readonly Dictionary<int, T> _toDelete = [];

    public bool TryGet(int key, [NotNullWhen(true)] out T? value) => _byKey.TryGetValue(key, out value);
    public bool TryGet(string idLong, [NotNullWhen(true)] out T? value) => _byIdLong.TryGetValue(idLong, out value);

    public T? Get(int key) => _byKey.TryGetValue(key, out T? value) ? value : default(T);
    public T? Get(string idLong) => _byIdLong.TryGetValue(idLong, out T? value) ? value : default(T);

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
        _byIdLong.Clear();
        _toAdd.Clear();
        _toUpdate.Clear();
        _toDelete.Clear();
    }

    public void CacheAdd(T item)
    {
        if (item.IdLong is null)
        {
            throw new ArgumentException("Cannot cache item with null IdLong.");
        }

        _byKey[item.Key] = item;
        _byIdLong[item.IdLong] = item;
        _toAdd[item.Key] = item;
    }

    public void CacheUpdate(T item)
    {
        _byKey[item.Key] = item;
        _byIdLong[item.IdLong] = item;
        _toUpdate[item.Key] = item;
    }

    public void CacheDelete(T item)
    {
        _byKey[item.Key] = item;
        _byIdLong[item.IdLong] = item;
        _toDelete[item.Key] = item;
    }

    public void Changed(T item)
    {
        if (!_byKey.ContainsKey(item.Key))
        {
            _byKey[item.Key] = item;
        }

        if (!_byIdLong.ContainsKey(item.IdLong))
        {
            _byIdLong[item.IdLong] = item;
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
