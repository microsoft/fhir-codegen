using System.Data;
using System.Diagnostics.CodeAnalysis;
using Fhir.CodeGen.SQLiteGenerator;

namespace Fhir.CodeGen.Comparison.Models;

public enum OutcomeValueSetActionCodes
{
    /// <summary>
    /// The value set is used with the same name in the target version.
    /// </summary>
    UseValueSetSameName,

    /// <summary>
    /// The value set is used but has been renamed in the target version.
    /// </summary>
    UseValueSetRenamed,

    /// <summary>
    /// The value set has no valid mapping to the target version, so a cross-version defined value set must be used.
    /// </summary>
    UseCrossVersionDefinition,

    /// <summary>
    /// The value set exists in the target version with the same name, but additional concepts are required from a cross-version defined value set.
    /// </summary>
    UseSameNameAndCrossVersion,

    /// <summary>
    /// The value set has been renamed in the target version, and additional concepts are required from a cross-version defined value set.
    /// </summary>
    UseRenamedAndCrossVersion,

    /// <summary>
    /// The value set has other mappings that account for all concepts, but this target is explicitly not mapped.
    /// </summary>
    UseOtherValueSets,

    /// <summary>
    /// The value set has other mappings, but this target is explicitly not mapped and there are concepts not covered.
    /// </summary>
    UseOtherAndCrossVersion,
}

public enum OutcomeValueSetConceptActionCodes
{
    /// <summary>
    /// The concept is used with the same code in the target version.
    /// </summary>
    UseConceptSameCode,

    /// <summary>
    /// The concept is used but has been changed to a different code in the target version.
    /// </summary>
    UseConceptChangedCode,

    /// <summary>
    /// The concept has no valid mapping to the target version.
    /// </summary>
    UnmappedConcept,

    /// <summary>
    /// The concept has no valid mapping to the target version, so a cross-version defined concept must be used.
    /// </summary>
    UseCrossVersionDefinition,

    /// <summary>
    /// The concept is mapped to a different value set in the target version.
    /// </summary>
    MappedElsewhere,

    UseCodeAndCrossVersion,
    UseOneOfMultipleCodes,
}

public enum OutcomeStructureActionCodes
{
    /// <summary>
    /// The structure is used with the same name in the target version.
    /// </summary>
    UseStructureSameName,
    /// <summary>
    /// The structure is used but has been renamed in the target version.
    /// </summary>
    UseStructureRenamed,
    /// <summary>
    /// The structure has no valid mapping to the target version, so a Basic resource must be used.
    /// </summary>
    UseBasicResource,
    /// <summary>
    /// The structure has no valid mapping to the target version, so a generated datatype extension must be used.
    /// </summary>
    UseDatatypeExtension,
    ///// <summary>
    ///// The structure is mapped to one of several possible structures in the target version.
    ///// </summary>
    //UseOneOf,
}

public enum OutcomeElementActionCodes
{
    /// <summary>
    /// The element is used with the same name in the target version.
    /// </summary>
    UseElementSameName,
    /// <summary>
    /// The element is used but has been renamed in the target version.
    /// </summary>
    UseElementRenamed,
    /// <summary>
    /// The element is represented as an extension in the target version.
    /// </summary>
    UseExtension,
    /// <summary>
    /// The element is represented as an extension inherited from an ancestor.
    /// </summary>
    UseExtensionFromAncestor,
    /// <summary>
    /// The element is mapped to a basic element in the target version.
    /// </summary>
    UseBasicElement,
    ///// <summary>
    ///// The element is mapped to one of several possible elements and possibly an extension in the target version.
    ///// </summary>
    //UseOneOf,
    /// <summary>
    /// The element has no valid mapping and cannot have an extension.
    /// </summary>
    Unresolved,
    /// <summary>
    /// The element represents an extension in the source version, so nothing is generated.
    /// </summary>
    IsExtension,
    /// <summary>
    /// The element represents an Element.id, so nothing is generated.
    /// </summary>
    IsElementId,
    /// <summary>
    /// The concept is mapped to a different element in the target version.
    /// </summary>
    MappedElsewhere,
}

public static class DbOutcomeClasses
{
    public static void LoadIndices(IDbConnection db)
    {
        DbValueSetOutcome.LoadMaxKey(db);
        DbValueSetConceptOutcome.LoadMaxKey(db);

        DbStructureOutcome.LoadMaxKey(db);
        DbElementOutcome.LoadMaxKey(db);
    }

    public static void DropTables(
        IDbConnection db,
        bool forValueSets = true,
        bool forStructures = true)
    {
        if (forValueSets)
        {
            DbValueSetOutcome.DropTable(db);
            DbValueSetConceptOutcome.DropTable(db);
        }

        if (forStructures)
        {
            DbStructureOutcome.DropTable(db);
            DbElementOutcome.DropTable(db);
        }
    }

    public static void CreateTables(
        IDbConnection db,
        bool forValueSets = true,
        bool forStructures = true)
    {
        if (forValueSets)
        {
            DbValueSetOutcome.CreateTable(db);
            DbValueSetConceptOutcome.CreateTable(db);
        }

        if (forStructures)
        {
            DbStructureOutcome.CreateTable(db);
            DbElementOutcome.CreateTable(db);
        }
    }
}

[CgSQLiteBaseClass]
public abstract class DbOutcomeBase : DbRecordBase
{
    [CgSQLiteForeignKey(referenceTable: "FhirPackages", referenceColumn: nameof(DbFhirPackage.Key))]
    public required int SourceFhirPackageKey { get; set; }
    public required Fhir.CodeGen.Common.Packaging.FhirReleases.FhirSequenceCodes SourceFhirSequence { get; set; }

    [CgSQLiteForeignKey(referenceTable: "FhirPackages", referenceColumn: nameof(DbFhirPackage.Key))]
    public required int TargetFhirPackageKey { get; set; }
    public required Fhir.CodeGen.Common.Packaging.FhirReleases.FhirSequenceCodes TargetFhirSequence { get; set; }

    /// <summary>
    /// Number of artifacts that this artifact maps *to*:
    /// - 0: no target (has no equivalent artifact)
    /// - 1: single target (e.g., R4.Patient -> R5.Patient)
    /// - 2: two targets (e.g., R2.Practitioner -> R3.Practitioner + R3.PractitionerRole)
    /// </summary>
    public required int TotalTargetCount { get; set; }

    /// <summary>
    /// Number of artifacts that map *to* this artifact:
    /// - 0: no source (has no equivalent artifact)
    /// - 1: single source (e.g., R4.Patient -> R5.Patient)
    /// - 2: two sources (e.g., R3.Practitioner + R3.PractitionerRole -> R2.Practitioner)
    /// </summary>
    public required int TotalSourceCount { get; set; }

    /// <summary>
    /// If the mapping is an explicitly-tracked rename
    /// </summary>
    public required bool IsRenamed { get; set; }

    public required bool IsUnmapped { get; set; }
    public required bool IsIdentical { get; set; }
    public required bool IsEquivalent { get; set; }
    public required bool IsBroaderThanTarget { get; set; }
    public required bool IsNarrowerThanTarget { get; set; }

    public required bool FullyMapsToThisTarget { get; set; }
    public required bool FullyMapsAcrossAllTargets { get; set; }

    public required string Comments { get; set; }


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
            switch (value.Length)
            {
                case 0:
                    ContentKeyR2 = null;
                    ContentKeyR3 = null;
                    ContentKeyR4 = null;
                    ContentKeyR4B = null;
                    ContentKeyR5 = null;
                    ContentKeyR6 = null;
                    return;

                case 5:
                    ContentKeyR2 = value[0];
                    ContentKeyR3 = value[1];
                    ContentKeyR4 = value[2];
                    ContentKeyR4B = value[3];
                    ContentKeyR5 = value[4];
                    ContentKeyR6 = null;
                    return;

                case 6:
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
    public virtual int ComparisonKey { get; set; }

    [CgSQLiteIgnore]
    public virtual int SourceContentKey { get; set; }

    [CgSQLiteIgnore]
    public virtual int? TargetContentKey { get; set; }
}

[CgSQLiteBaseClass]
public abstract class DbArtifactOutcomeBase : DbOutcomeBase
{
    public required string? PotentialGenResourceType { get; set; }
    public required string? PotentialGenLongId { get; set; }
    public required string? PotentialGenShortId { get; set; }
    public required string? PotentialGenUrl { get; set; }

    public required string SourceCanonicalVersioned { get; set; }
    public required string SourceCanonicalUnversioned { get; set; }
    public required string SourceId { get; set; }
    public required string SourceName { get; set; }
    public required string SourceVersion { get; set; }

    public required string? TargetCanonicalVersioned { get; set; }
    public required string? TargetCanonicalUnversioned { get; set; }
    public required string? TargetId { get; set; }
    public required string? TargetName { get; set; }
    public required string? TargetVersion { get; set; }
}


[CgSQLiteTable(tableName: "ValueSetOutcomes")]
public partial class DbValueSetOutcome : DbArtifactOutcomeBase
{
    [CgSQLiteForeignKey(referenceTable: "ValueSetComparisons", referenceColumn: nameof(DbValueSetComparison.Key))]
    public required int ValueSetComparisonKey { get; set; }
    [CgSQLiteIgnore]
    public override int ComparisonKey { get => this.ValueSetComparisonKey; set => this.ValueSetComparisonKey = value; }


    [CgSQLiteForeignKey(referenceTable: "ValueSets", referenceColumn: nameof(DbValueSet.Key))]
    public required int SourceValueSetKey { get; set; }
    [CgSQLiteIgnore]
    public override int SourceContentKey { get => this.SourceValueSetKey; set => this.SourceValueSetKey = value; }


    [CgSQLiteForeignKey(referenceTable: "ValueSets", referenceColumn: nameof(DbValueSet.Key))]
    public required int? TargetValueSetKey { get; set; }
    [CgSQLiteIgnore]
    public override int? TargetContentKey { get => this.TargetValueSetKey; set => this.TargetValueSetKey = value; }



    public required OutcomeValueSetActionCodes? OutcomeAction { get; set; }

    [CgSQLiteIgnore]
    public Hl7.Fhir.Model.ValueSet? GeneratedValueSet { get; set; } = null;
}

[CgSQLiteTable(tableName: "ValueSetConceptOutcomes")]
public partial class DbValueSetConceptOutcome : DbOutcomeBase
{
    [CgSQLiteForeignKey(referenceTable: "ValueSetOutcomes", referenceColumn: nameof(DbValueSetOutcome.Key))]
    public required int ValueSetOutcomeKey { get; set; }

    [CgSQLiteForeignKey(referenceTable: "ValueSetConceptComparisons", referenceColumn: nameof(DbValueSetConceptComparison.Key))]
    public required int ValueSetConceptComparisonKey { get; set; }
    [CgSQLiteIgnore]
    public override int ComparisonKey { get => this.ValueSetConceptComparisonKey; set => this.ValueSetConceptComparisonKey = value; }


    [CgSQLiteForeignKey(referenceTable: "ValueSets", referenceColumn: nameof(DbValueSet.Key))]
    public required int SourceValueSetKey { get; set; }

    [CgSQLiteForeignKey(referenceTable: "ValueSetConcepts", referenceColumn: nameof(DbValueSetConcept.Key))]
    public required int SourceValueSetConceptKey { get; set; }
    [CgSQLiteIgnore]
    public override int SourceContentKey { get => this.SourceValueSetConceptKey; set => this.SourceValueSetConceptKey = value; }

    public required string SourceSystem { get; set; }
    public required string SourceCode { get; set; }

    [CgSQLiteForeignKey(referenceTable: "ValueSets", referenceColumn: nameof(DbValueSet.Key))]
    public required int? TargetValueSetKey { get; set; }

    [CgSQLiteForeignKey(referenceTable: "ValueSetConcepts", referenceColumn: nameof(DbValueSetConcept.Key))]
    public required int? TargetValueSetConceptKey { get; set; }
    [CgSQLiteIgnore]
    public override int? TargetContentKey { get => this.TargetValueSetConceptKey; set => this.TargetValueSetConceptKey = value; }

    public required string? TargetSystem { get; set; }
    public required string? TargetCode { get; set; }

    public required bool CodeLiteralsMatch { get; set; }
    public required bool SourceCodeTreatedAsEscapeValve { get; set; }
    public required bool? TargetCodeTreatedAsEscapeValve { get; set; }

    public required OutcomeValueSetConceptActionCodes? OutcomeAction { get; set; }
}



[CgSQLiteTable(tableName: "StructureOutcomes")]
public partial class DbStructureOutcome : DbArtifactOutcomeBase
{
    [CgSQLiteForeignKey(referenceTable: "StructureComparisons", referenceColumn: nameof(DbStructureComparison.Key))]
    public required int StructureComparisonKey { get; set; }
    [CgSQLiteIgnore]
    public override int ComparisonKey { get => this.StructureComparisonKey; set => this.StructureComparisonKey = value; }


    [CgSQLiteForeignKey(referenceTable: "Structures", referenceColumn: nameof(DbStructureDefinition.Key))]
    public required int SourceStructureKey { get; set; }
    [CgSQLiteIgnore]
    public override int SourceContentKey { get => this.SourceStructureKey; set => this.SourceStructureKey = value; }

    public required Fhir.CodeGen.Common.Models.FhirArtifactClassEnum SourceArtifactClass { get; set; } = Fhir.CodeGen.Common.Models.FhirArtifactClassEnum.Unknown;

    [CgSQLiteForeignKey(referenceTable: "Structures", referenceColumn: nameof(DbStructureDefinition.Key))]
    public required int? TargetStructureKey { get; set; }

    public required Fhir.CodeGen.Common.Models.FhirArtifactClassEnum? TargetArtifactClass { get; set; } = null;


    public required Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship? ConceptDomainRelationship { get; set; }
    public required Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship? ValueDomainRelationship { get; set; }


    public required OutcomeStructureActionCodes? OutcomeAction { get; set; }

    [CgSQLiteIgnore]
    public Hl7.Fhir.Model.StructureDefinition? GeneratedProfile { get; set; } = null;
}

[CgSQLiteTable(tableName: "ElementOutcomes")]
[CgSQLiteIndex(nameof(SourceFhirPackageKey), nameof(SourceStructureKey), nameof(SourceElementKey), nameof(TargetFhirPackageKey))]
[CgSQLiteIndex(nameof(SourceElementKey), nameof(TargetFhirPackageKey))]
[CgSQLiteIndex(nameof(SourceFhirPackageKey), nameof(SourceStructureKey), nameof(SourceResourceOrder))]
[CgSQLiteIndex(nameof(StructureOutcomeKey), nameof(OutcomeAction), nameof(RelatedAncestorOutcomeKey), nameof(SourceResourceOrder))]
public partial class DbElementOutcome : DbArtifactOutcomeBase
{
    [CgSQLiteForeignKey(referenceTable: "StructureOutcomes", referenceColumn: nameof(DbStructureOutcome.Key), modelTypeName: nameof(DbStructureOutcome))]
    public required int StructureOutcomeKey { get; set; }

    [CgSQLiteForeignKey(referenceTable: "ElementComparisons", referenceColumn: nameof(DbElementComparison.Key))]
    public required int ElementComparisonKey { get; set; }
    [CgSQLiteIgnore]
    public override int ComparisonKey { get => this.ElementComparisonKey; set => this.ElementComparisonKey = value; }


    [CgSQLiteForeignKey(referenceTable: "Structures", referenceColumn: nameof(DbStructureDefinition.Key), modelTypeName: nameof(DbStructureDefinition))]
    public required int SourceStructureKey { get; set; }

    [CgSQLiteForeignKey(referenceTable: "Elements", referenceColumn: nameof(DbElement.Key), modelTypeName: nameof(DbElement))]
    public required int SourceElementKey { get; set; }
    [CgSQLiteIgnore]
    public override int SourceContentKey { get => this.SourceElementKey; set => this.SourceElementKey = value; }


    public required int SourceResourceOrder { get; set; }
    public required int SourceComponentOrder { get; set; }


    [CgSQLiteForeignKey(referenceTable: "Structures", referenceColumn: nameof(DbStructureDefinition.Key), modelTypeName: nameof(DbStructureDefinition))]
    public required int TargetStructureKey { get; set; }

    [CgSQLiteForeignKey(referenceTable: "Elements", referenceColumn: nameof(DbElement.Key), modelTypeName: nameof(DbElement))]
    public required int? TargetElementKey { get; set; }
    [CgSQLiteIgnore]
    public override int? TargetContentKey { get => this.TargetElementKey; set => this.TargetElementKey = value; }


    public required int? TargetResourceOrder { get; set; }
    public required int? TargetComponentOrder { get; set; }


    [CgSQLiteForeignKey(referenceTable: "ExtensionSubstituions", referenceColumn: nameof(DbExtensionSubstitution.Key), modelTypeName: nameof(DbExtensionSubstitution))]
    public required int? ExtensionSubstitutionKey { get; set; }

    [CgSQLiteForeignKey(referenceTable: "ElementOutcomes", referenceColumn: nameof(Key), modelTypeName: nameof(DbElementOutcome))]
    public required int? RelatedAncestorOutcomeKey { get; set; }


    public required Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship? ConceptDomainRelationship { get; set; }
    public required Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship? ValueDomainRelationship { get; set; }



    public required OutcomeElementActionCodes? OutcomeAction { get; set; }

    [CgSQLiteIgnore]
    public Hl7.Fhir.Model.StructureDefinition? GeneratedExtension { get; set; } = null;
}

