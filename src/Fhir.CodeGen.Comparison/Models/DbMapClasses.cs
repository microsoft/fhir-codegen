using Fhir.CodeGen.SQLiteGenerator;
using System.Diagnostics.CodeAnalysis;
using System.Data;


namespace Fhir.CodeGen.Comparison.Models;


[CgSQLiteBaseClass]
public abstract class DbMapRecordBase : DbRecordBase
{
    [CgSQLiteForeignKey(referenceTable: "FhirPackages", referenceColumn: nameof(DbFhirPackage.Key))]
    public required int SourceFhirPackageKey { get; set; }

    public required Fhir.CodeGen.Common.Packaging.FhirReleases.FhirSequenceCodes SourceFhirSequence { get; set; }

    [CgSQLiteForeignKey(referenceTable: "FhirPackages", referenceColumn: nameof(DbFhirPackage.Key))]
    public required int TargetFhirPackageKey { get; set; }

    public required Fhir.CodeGen.Common.Packaging.FhirReleases.FhirSequenceCodes TargetFhirSequence { get; set; }

    public required int? PreviousStepMapRecordKey { get; set; }
    public required int Steps { get; set; }

    public required Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship? Relationship { get; set; }

    [CgSQLiteIgnore]
    public bool IsNotMapped => Relationship is null;

    public required bool ExplicitNoMap { get; set; }

    public string? Comments { get; set; } = null;
    public string? TechnicalNotes { get; set; } = null;


    public int? ContentKeyR2 { get; set; } = null;
    public int? ContentKeyR3 { get; set; } = null;
    public int? ContentKeyR4 { get; set; } = null;
    public int? ContentKeyR4B { get; set; } = null;
    public int? ContentKeyR5 { get; set; } = null;
    public int? ContentKeyR6 { get; set; } = null;

    [CgSQLiteIgnore]
    public int?[] ContentKeys
    {
        get => [ContentKeyR2, ContentKeyR3, ContentKeyR4, ContentKeyR4B, ContentKeyR5, ContentKeyR6];
        set
        {
            if (value.Length == 5)
            {
                ContentKeyR2 = value[0];
                ContentKeyR3 = value[1];
                ContentKeyR4 = value[2];
                ContentKeyR4B = value[3];
                ContentKeyR5 = value[4];
                ContentKeyR6 = null;
                return;
            }
            if (value.Length == 6)
            {
                ContentKeyR2 = value[0];
                ContentKeyR3 = value[1];
                ContentKeyR4 = value[2];
                ContentKeyR4B = value[3];
                ContentKeyR5 = value[4];
                ContentKeyR6 = value[5];
                return;
            }
            throw new ArgumentException($"Invalid number of keys: {value.Length}. Expected 5 or 6.");
        }
    }

    [CgSQLiteIgnore]
    public int?[] ContentKeysInverted
    {
        get => [ContentKeyR6, ContentKeyR5, ContentKeyR4B, ContentKeyR4, ContentKeyR3, ContentKeyR2];
    }

    public int? GetContentKeyFromSource(int offsetFromSource)
    {
        int targetIndex = (int)SourceFhirSequence - 1;

        int index = (SourceFhirSequence > TargetFhirSequence)
            ? targetIndex + offsetFromSource
            : targetIndex - offsetFromSource;

        switch (index)
        {
            case 0: return ContentKeyR2;
            case 1: return ContentKeyR3;
            case 2: return ContentKeyR4;
            case 3: return ContentKeyR4B;
            case 4: return ContentKeyR5;
            case 5: return ContentKeyR6;
            default: return null;
        }
    }

    public int? GetContentKeyFromTarget(int offsetFromTarget)
    {
        int targetIndex = (int)TargetFhirSequence - 1;

        int index = (SourceFhirSequence > TargetFhirSequence)
            ? targetIndex + offsetFromTarget
            : targetIndex - offsetFromTarget;

        switch (index)
        {
            case 0: return ContentKeyR2;
            case 1: return ContentKeyR3;
            case 2: return ContentKeyR4;
            case 3: return ContentKeyR4B;
            case 4: return ContentKeyR5;
            case 5: return ContentKeyR6;
            default: return null;
        }
    }

    [CgSQLiteIgnore]
    public int? PriorContentKeyFromArray => GetContentKeyFromTarget(1);

    [CgSQLiteIgnore]
    public int? PriorContentKeyFromArrayInverted => GetContentKeyFromSource(1);


    //[CgSQLiteIgnore]
    //public virtual int? SourceKey { get; set; }

    //[CgSQLiteIgnore]
    //public virtual int? TargetKey { get; set; }
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
[CgSQLiteIndex(nameof(SourceFhirPackageKey), nameof(SourceValueSetKey), nameof(TargetFhirPackageKey))]
[CgSQLiteIndex(nameof(SourceFhirPackageKey), nameof(TargetValueSetKey))]
public partial class DbValueSetMappingRecord : DbMapArtifactRecordBase      //, IDbMapArtifactRecord
{
    [CgSQLiteForeignKey(referenceTable: "ValueSets", referenceColumn: nameof(DbValueSet.Key))]
    public required int SourceValueSetKey { get; set; }
    public required string SourceValueSetId { get; set; }

    [CgSQLiteForeignKey(referenceTable: "ValueSets", referenceColumn: nameof(DbValueSet.Key))]
    public required int? TargetValueSetKey { get; set; }
    public required string? TargetValueSetId { get; set; }

    //[CgSQLiteIgnore]
    //public override int? SourceKey { get => this.SourceValueSetKey; set => this.SourceValueSetKey = value ?? throw new Exception("Source Value Set key cannot be null!"); }

    //[CgSQLiteIgnore]
    //public override int? TargetKey { get => this.TargetValueSetKey; set => this.TargetValueSetKey = value; }
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


    //[CgSQLiteIgnore]
    //public override int? SourceKey { get => this.SourceValueSetConceptKey; set => this.SourceValueSetConceptKey = value ?? throw new Exception("Source concept key cannot be null"); }

    //[CgSQLiteIgnore]
    //public override int? TargetKey { get => this.TargetValueSetConceptKey; set => this.TargetValueSetConceptKey = value; }

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

    //[CgSQLiteIgnore]
    //public override int? SourceKey { get => this.SourceStructureKey; set => this.SourceStructureKey = value ?? throw new Exception("Source structure key cannot be null!"); }

    //[CgSQLiteIgnore]
    //public override int? TargetKey { get => this.TargetStructureKey; set => this.TargetStructureKey = value; }
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

    //[CgSQLiteIgnore]
    //public override int? SourceKey { get => this.SourceElementKey; set => this.SourceElementKey = value; }

    //[CgSQLiteIgnore]
    //public override int? TargetKey { get => this.TargetElementKey; set => this.TargetElementKey = value; }

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
