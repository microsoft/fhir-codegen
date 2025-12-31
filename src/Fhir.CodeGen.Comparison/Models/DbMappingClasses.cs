using Fhir.CodeGen.SQLiteGenerator;
using System.Diagnostics.CodeAnalysis;
using System.Data;


namespace Fhir.CodeGen.Comparison.Models;

public static class DbMappingClasses
{
    public static void LoadIndices(IDbConnection db)
    {
        DbMappingSourceFile.LoadMaxKey(db);
        DbValueSetMappingRecord.LoadMaxKey(db);
        DbValueSetConceptMappingRecord.LoadMaxKey(db);
        DbStructureMappingRecord.LoadMaxKey(db);
        DbElementMappingRecord.LoadMaxKey(db);
    }

    public static void DropTables(IDbConnection db)
    {
        DbMappingSourceFile.DropTable(db);
        DbValueSetMappingRecord.DropTable(db);
        DbValueSetConceptMappingRecord.DropTable(db);
        DbStructureMappingRecord.DropTable(db);
        DbElementMappingRecord.DropTable(db);
    }

    public static void CreateTables(IDbConnection db)
    {
        DbMappingSourceFile.CreateTable(db);
        DbValueSetMappingRecord.CreateTable(db);
        DbValueSetConceptMappingRecord.CreateTable(db);
        DbStructureMappingRecord.CreateTable(db);
        DbElementMappingRecord.CreateTable(db);
    }
}

[CgSQLiteTable(tableName: "MappingSourceFiles")]
[CgSQLiteIndex(nameof(SourceRelativeFilename))]
public partial class DbMappingSourceFile : DbRecordBase
{
    public required string Filename { get; set; }
    public required string SourceRelativeFilename { get; set; }
    public required bool IsConceptMap { get; set; }
    public required bool IsFml { get; set; }

    public required string Url { get; set; }
}


[CgSQLiteBaseClass]
public abstract class DbMappingRecordBase : DbRecordBase
{
    [CgSQLiteForeignKey(referenceTable: "FhirPackages", referenceColumn: nameof(DbFhirPackage.Key))]
    public required int SourceFhirPackageKey { get; set; }

    public required Fhir.CodeGen.Common.Packaging.FhirReleases.FhirSequenceCodes SourceFhirSequence { get; set; }

    [CgSQLiteForeignKey(referenceTable: "FhirPackages", referenceColumn: nameof(DbFhirPackage.Key))]
    public required int TargetFhirPackageKey { get; set; }

    public required Fhir.CodeGen.Common.Packaging.FhirReleases.FhirSequenceCodes TargetFhirSequence { get; set; }

    //public required int? PreviousStepMapRecordKey { get; set; }
    //public required int Steps { get; set; }

    public required Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship? Relationship { get; set; }

    public required bool ExplicitNoMap { get; set; }

    public string? Comments { get; set; } = null;
    public string? TechnicalNotes { get; set; } = null;


    //public int? ContentKeyR2 { get; set; } = null;
    //public int? ContentKeyR3 { get; set; } = null;
    //public int? ContentKeyR4 { get; set; } = null;
    //public int? ContentKeyR4B { get; set; } = null;
    //public int? ContentKeyR5 { get; set; } = null;
    //public int? ContentKeyR6 { get; set; } = null;

    //[CgSQLiteIgnore]
    //public int?[] ContentKeys
    //{
    //    get => [ContentKeyR2, ContentKeyR3, ContentKeyR4, ContentKeyR4B, ContentKeyR5, ContentKeyR6];
    //    set
    //    {
    //        if (value.Length == 5)
    //        {
    //            ContentKeyR2 = value[0];
    //            ContentKeyR3 = value[1];
    //            ContentKeyR4 = value[2];
    //            ContentKeyR4B = value[3];
    //            ContentKeyR5 = value[4];
    //            ContentKeyR6 = null;
    //            return;
    //        }
    //        if (value.Length == 6)
    //        {
    //            ContentKeyR2 = value[0];
    //            ContentKeyR3 = value[1];
    //            ContentKeyR4 = value[2];
    //            ContentKeyR4B = value[3];
    //            ContentKeyR5 = value[4];
    //            ContentKeyR6 = value[5];
    //            return;
    //        }
    //        throw new ArgumentException($"Invalid number of keys: {value.Length}. Expected 5 or 6.");
    //    }
    //}

    //[CgSQLiteIgnore]
    //public int?[] ContentKeysInverted
    //{
    //    get => [ContentKeyR6, ContentKeyR5, ContentKeyR4B, ContentKeyR4, ContentKeyR3, ContentKeyR2];
    //}

    //public int? GetContentKeyFromSource(int offsetFromSource)
    //{
    //    int targetIndex = (int)SourceFhirSequence - 1;

    //    int index = (SourceFhirSequence > TargetFhirSequence)
    //        ? targetIndex + offsetFromSource
    //        : targetIndex - offsetFromSource;

    //    switch (index)
    //    {
    //        case 0: return ContentKeyR2;
    //        case 1: return ContentKeyR3;
    //        case 2: return ContentKeyR4;
    //        case 3: return ContentKeyR4B;
    //        case 4: return ContentKeyR5;
    //        case 5: return ContentKeyR6;
    //        default: return null;
    //    }
    //}

    //public int? GetContentKeyFromTarget(int offsetFromTarget)
    //{
    //    int targetIndex = (int)TargetFhirSequence - 1;

    //    int index = (SourceFhirSequence > TargetFhirSequence)
    //        ? targetIndex + offsetFromTarget
    //        : targetIndex - offsetFromTarget;

    //    switch (index)
    //    {
    //        case 0: return ContentKeyR2;
    //        case 1: return ContentKeyR3;
    //        case 2: return ContentKeyR4;
    //        case 3: return ContentKeyR4B;
    //        case 4: return ContentKeyR5;
    //        case 5: return ContentKeyR6;
    //        default: return null;
    //    }
    //}

    //[CgSQLiteIgnore]
    //public int? PriorContentKeyFromArray => GetContentKeyFromTarget(1);

    //[CgSQLiteIgnore]
    //public int? PriorContentKeyFromArrayInverted => GetContentKeyFromSource(1);


    //[CgSQLiteIgnore]
    //public virtual int? SourceKey { get; set; }

    //[CgSQLiteIgnore]
    //public virtual int? TargetKey { get; set; }
}

[CgSQLiteBaseClass]
public abstract class DbMappingArtifactRecordBase : DbMappingRecordBase
{
    [CgSQLiteForeignKey(referenceTable: "MappingSourceFiles", referenceColumn: nameof(DbMappingSourceFile.Key))]
    public required int? ConceptMapSourceKey { get; set; } = null;


    //public string IdLong { get; set; } = string.Empty;
    //public string? IdShort { get; set; } = null;
    //public string? Url { get; set; } = null;
    //public string? Name { get; set; } = null;
    //public string? Title { get; set; } = null;
}

[CgSQLiteTable(tableName: "ValueSetMappings")]
//[CgSQLiteIndex(nameof(IdLong))]
[CgSQLiteIndex(nameof(SourceFhirPackageKey), nameof(SourceValueSetKey), nameof(TargetFhirPackageKey))]
[CgSQLiteIndex(nameof(SourceFhirPackageKey), nameof(TargetValueSetKey), nameof(Key))]
[CgSQLiteIndex(nameof(SourceValueSetKey), nameof(TargetValueSetKey))]
public partial class DbValueSetMappingRecord : DbMappingArtifactRecordBase      //, IDbMapArtifactRecord
{
    [CgSQLiteForeignKey(referenceTable: "ValueSets", referenceColumn: nameof(DbValueSet.Key))]
    public required int SourceValueSetKey { get; set; }
    public required string SourceValueSetId { get; set; }
    public required string SourceValueSetUrl { get; set; }
    public required string SourceValueSetVersion { get; set; }

    [CgSQLiteForeignKey(referenceTable: "ValueSets", referenceColumn: nameof(DbValueSet.Key))]
    public required int? TargetValueSetKey { get; set; }
    public required string? TargetValueSetId { get; set; }
    public required string? TargetValueSetUrl { get; set; }
    public required string? TargetValueSetVersion { get; set; }

    //[CgSQLiteIgnore]
    //public override int? SourceKey { get => this.SourceValueSetKey; set => this.SourceValueSetKey = value ?? throw new Exception("Source Value Set key cannot be null!"); }

    //[CgSQLiteIgnore]
    //public override int? TargetKey { get => this.TargetValueSetKey; set => this.TargetValueSetKey = value; }
}

[CgSQLiteTable(tableName: "ValueSetConceptMappings")]
[CgSQLiteIndex(nameof(ValueSetMappingKey), nameof(SourceValueSetConceptKey), nameof(TargetValueSetConceptKey))]
[CgSQLiteIndex(nameof(SourceFhirPackageKey), nameof(TargetValueSetConceptKey), nameof(Key))]
public partial class DbValueSetConceptMappingRecord : DbMappingRecordBase
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
public partial class DbStructureMappingRecord : DbMappingArtifactRecordBase
{
    [CgSQLiteForeignKey(referenceTable: "Structures", referenceColumn: nameof(DbStructureDefinition.Key))]
    public required int SourceStructureKey { get; set; }
    public required string SourceStructureId { get; set; }

    [CgSQLiteForeignKey(referenceTable: "Structures", referenceColumn: nameof(DbStructureDefinition.Key))]
    public required int? TargetStructureKey { get; set; }
    public required string? TargetStructureId { get; set; }


    [CgSQLiteForeignKey(referenceTable: "MappingSourceFiles", referenceColumn: nameof(DbMappingSourceFile.Key))]
    public required int? FmlSourceKey { get; set; } = null;

    public required Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship? ConceptDomainRelationship { get; set; }
    public required Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship? ValueDomainRelationship { get; set; }


    //[CgSQLiteIgnore]
    //public override int? SourceKey { get => this.SourceStructureKey; set => this.SourceStructureKey = value ?? throw new Exception("Source structure key cannot be null!"); }

    //[CgSQLiteIgnore]
    //public override int? TargetKey { get => this.TargetStructureKey; set => this.TargetStructureKey = value; }
}


[CgSQLiteTable(tableName: "ElementMappings")]
[CgSQLiteIndex(nameof(StructureMappingKey), nameof(SourceElementKey), nameof(TargetElementKey))]
[CgSQLiteIndex(nameof(SourceFhirPackageKey), nameof(TargetFhirPackageKey), nameof(SourceElementId), nameof(TargetElementId))]
public partial class DbElementMappingRecord : DbMappingRecordBase
{
    [CgSQLiteForeignKey(referenceTable: "StructureMappings", referenceColumn: nameof(DbStructureMappingRecord.Key))]
    public required int? StructureMappingKey { get; set; }

    [CgSQLiteForeignKey(referenceTable: "Elements", referenceColumn: nameof(DbElement.Key))]
    public required int? SourceElementKey { get; set; }

    public required string SourceElementId { get; set; }

    [CgSQLiteForeignKey(referenceTable: "Elements", referenceColumn: nameof(DbElement.Key))]
    public required int? TargetElementKey { get; set; }

    public required string? TargetElementId { get; set; }

    [CgSQLiteForeignKey(referenceTable: "MappingSourceFiles", referenceColumn: nameof(DbMappingSourceFile.Key))]
    public required int? ConceptMapSourceKey { get; set; } = null;

    [CgSQLiteForeignKey(referenceTable: "MappingSourceFiles", referenceColumn: nameof(DbMappingSourceFile.Key))]
    public required int? FmlSourceKey { get; set; } = null;

    public required bool? FmlIsSimpleCopy { get; set; } = null;

    public required Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship? ConceptDomainRelationship { get; set; }
    public required Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship? ValueDomainRelationship { get; set; }


    //[CgSQLiteIgnore]
    //public override int? SourceKey { get => this.SourceElementKey; set => this.SourceElementKey = value; }

    //[CgSQLiteIgnore]
    //public override int? TargetKey { get => this.TargetElementKey; set => this.TargetElementKey = value; }

}

