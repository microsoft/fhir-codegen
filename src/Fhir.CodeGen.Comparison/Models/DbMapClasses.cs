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

    public bool? IsIdentical { get; set; } = null;
    public bool? IsEquivalent { get; set; } = null;
    public bool? IsBroaderThanTarget { get; set; } = null;
    public bool? IsNarrowerThanTarget { get; set; } = null;

    public bool? IsRenamed { get; set; } = null;

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

    public string? IdLong { get; set; } = null;
    public string? IdShort { get; set; } = null;
    public string? Url { get; set; } = null;
    public string? Name { get; set; } = null;
    public string? Title { get; set; } = null;

    public string? LastReviewedBy { get; set; } = null;
    public DateTime? LastReviewedOn { get; set; } = null;

    public bool? HasContentUnmapped { get; set; } = null;
    public bool? HasContentIdentical { get; set; } = null;
    public bool? HasContentEquivalent { get; set; } = null;
    public bool? HasContentBroader { get; set; } = null;
    public bool? HasContentNarrower { get; set; } = null;
}

[CgSQLiteTable(tableName: "ValueSetMappings")]
[CgSQLiteIndex(nameof(IdLong))]
public partial class DbValueSetMapRecord : DbMapArtifactRecordBase      //, IDbMapArtifactRecord
{
    [CgSQLiteForeignKey(referenceTable: "ValueSets", referenceColumn: nameof(DbValueSet.Key))]
    public required int SourceValueSetKey { get; set; }

    [CgSQLiteForeignKey(referenceTable: "ValueSets", referenceColumn: nameof(DbValueSet.Key))]
    public required int? TargetValueSetKey { get; set; }


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


}

[CgSQLiteTable(tableName: "ValueSetConceptMappings")]
[CgSQLiteIndex(nameof(ValueSetMapKey), nameof(SourceValueSetConceptKey), nameof(TargetValueSetConceptKey))]
public partial class DbValueSetConceptMapRecord : DbMapRecordBase
{
    [CgSQLiteForeignKey(referenceTable: "ValueSetMappings", referenceColumn: nameof(DbValueSetMapRecord.Key))]
    public required int ValueSetMapKey { get; set; }

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


    public required bool? CodesAreIdentical { get; set; }
}


[CgSQLiteTable(tableName: "StructureMappings")]
[CgSQLiteIndex(nameof(SourceFhirPackageKey), nameof(SourceStructureKey), nameof(TargetFhirPackageKey), nameof(TargetStructureKey))]
[CgSQLiteIndex(nameof(SourceFhirPackageKey), nameof(SourceStructureId), nameof(TargetFhirPackageKey), nameof(TargetStructureId))]
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



    public required Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship? ConceptDomainRelationship { get; set; }
    public required Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship? ValueDomainRelationship { get; set; }


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
public partial class DbElementMappingRecord : DbMapRecordBase
{
    [CgSQLiteForeignKey(referenceTable: "StructureMappings", referenceColumn: nameof(DbStructureMappingRecord.Key))]
    public required int ResourceMapKey { get; set; }

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

    public string? OriginatingConceptMapUrlsLiteral { get; set; } = null;
    [CgSQLiteIgnore]
    public List<string>? OriginatingConceptMapUrls
    {
        get => OriginatingConceptMapUrlsLiteral?.Split(", ").ToList();
        set => OriginatingConceptMapUrlsLiteral = value is null ? null : string.Join(", ", value);
    }


    public required Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship? ConceptDomainRelationship { get; set; }
    public required Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship? ValueDomainRelationship { get; set; }

    public required Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship? ComputedRelationship { get; set; }


    public required ChangeIndicationCodes? ElementTypeChange { get; set; }

    public required string? TypesAddedLiteral { get; set; }
    [CgSQLiteIgnore]
    public List<string>? TypesAdded
    {
        get => TypesAddedLiteral?.Split(", ").ToList();
        set => TypesAddedLiteral = value is null ? null : string.Join(", ", value);
    }

    public required string? TypesRemovedLiteral { get; set; }
    [CgSQLiteIgnore]
    public List<string>? TypesRemoved
    {
        get => TypesRemovedLiteral?.Split(", ").ToList();
        set => TypesRemovedLiteral = value is null ? null : string.Join(", ", value);
    }

    public required string? TypesIdenticalLiteral { get; set; }
    [CgSQLiteIgnore]
    public List<string>? TypesIdentical
    {
        get => TypesIdenticalLiteral?.Split(", ").ToList();
        set => TypesIdenticalLiteral = value is null ? null : string.Join(", ", value);
    }

    public required string? TypesMappedLiteral { get; set; }
    [CgSQLiteIgnore]
    public List<string>? TypesMapped
    {
        get => TypesMappedLiteral?.Split(", ").ToList();
        set => TypesMappedLiteral = value is null ? null : string.Join(", ", value);
    }

    public required ChangeIndicationCodes? ReferenceTargetChange { get; set; }

    public required string? ReferenceTargetsAddedLiteral { get; set; }
    [CgSQLiteIgnore]
    public List<string>? ReferenceTargetsAdded
    {
        get => ReferenceTargetsAddedLiteral?.Split(", ").ToList();
        set => ReferenceTargetsAddedLiteral = value is null ? null : string.Join(", ", value);
    }

    public required string? ReferenceTargetsRemovedLiteral { get; set; }
    [CgSQLiteIgnore]
    public List<string>? ReferenceTargetsRemoved
    {
        get => ReferenceTargetsRemovedLiteral?.Split(", ").ToList();
        set => ReferenceTargetsRemovedLiteral = value is null ? null : string.Join(", ", value);
    }

    public required string? ReferenceTargetsIdenticalLiteral { get; set; }
    [CgSQLiteIgnore]
    public List<string>? ReferenceTargetsIdentical
    {
        get => ReferenceTargetsIdenticalLiteral?.Split(", ").ToList();
        set => ReferenceTargetsIdenticalLiteral = value is null ? null : string.Join(", ", value);
    }

    public required string? ReferenceTargetsMappedLiteral { get; set; }
    [CgSQLiteIgnore]
    public List<string>? ReferenceTargetsMapped
    {
        get => ReferenceTargetsMappedLiteral?.Split(", ").ToList();
        set => ReferenceTargetsMappedLiteral = value is null ? null : string.Join(", ", value);
    }

    public required ChangeIndicationCodes? BindingStrengthChange { get; set; }
    public required bool? BindingBecameRequired { get; set; }
    public required bool? BindingNoLongerRequired { get; set; }

    public required ChangeIndicationCodes? BindingTargetChange { get; set; }

    [CgSQLiteForeignKey(referenceTable: "ValueSetMappings", referenceColumn: nameof(DbValueSetMapRecord.Key))]
    public required int? BoundValueSetMapKey { get; set; }

    public required ChangeIndicationCodes? MaxCardinalityChange { get; set; }
    public required bool? BecameProhibited { get; set; } // maxCardinality changed to 0
    public required bool? BecameMandatory { get; set; }
    public required bool? BecameOptional { get; set; }
    public required bool? BecameArray { get; set; }
    public required bool? BecameScalar { get; set; }
}
