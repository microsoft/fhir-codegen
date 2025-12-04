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
    MappedInOtherValueSet,
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
}

[CgSQLiteBaseClass]
public abstract class DbOutcomeBase : DbRecordBase
{
    [CgSQLiteForeignKey(referenceTable: "FhirPackages", referenceColumn: nameof(DbFhirPackage.Key))]
    public required int SourceFhirPackageKey { get; set; }

    [CgSQLiteForeignKey(referenceTable: "FhirPackages", referenceColumn: nameof(DbFhirPackage.Key))]
    public required int TargetFhirPackageKey { get; set; }

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

    public required Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship? ConceptDomainRelationship { get; set; }
    public required Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship? ValueDomainRelationship { get; set; }


    public required bool FullyMapsToThisTarget { get; set; }
    public required bool FullyMapsAcrossAllTargets { get; set; }

    public required string Comments { get; set; }
}

[CgSQLiteBaseClass]
public abstract class DbArtifactOutcomeBase : DbOutcomeBase
{
    public required string? PotentialGenResourceType { get; set; }
    public required string? PotentialGenLongId { get; set; }
    public required string? PotentialGenShortId { get; set; }
    public required string? PotentialGenUrl { get; set; }
}

[CgSQLiteTable(tableName: "StructureOutcomes")]
[CgSQLiteIndex(nameof(SourceFhirPackageKey), nameof(SourceStructureKey), nameof(TargetFhirPackageKey))]
public partial class DbStructureOutcome : DbArtifactOutcomeBase
{
    [CgSQLiteForeignKey(referenceTable: "Structures", referenceColumn: nameof(DbStructureDefinition.Key))]
    public required int SourceStructureKey { get; set; }
    public required string SourceStructureName { get; set; }
    public required Fhir.CodeGen.Common.Models.FhirArtifactClassEnum SourceArtifactClass { get; set; } = Fhir.CodeGen.Common.Models.FhirArtifactClassEnum.Unknown;

    [CgSQLiteForeignKey(referenceTable: "Structures", referenceColumn: nameof(DbStructureDefinition.Key))]
    public required int? TargetStructureKey { get; set; }
    public required string? TargetStructureName { get; set; }

    public required OutcomeStructureActionCodes? OutcomeAction { get; set; }

    [CgSQLiteIgnore]
    public Hl7.Fhir.Model.StructureDefinition? PotentialGenProfile { get; set; } = null;
}

[CgSQLiteTable(tableName: "ElementOutcomes")]
[CgSQLiteIndex(nameof(SourceFhirPackageKey), nameof(SourceStructureKey), nameof(SourceElementKey), nameof(TargetFhirPackageKey))]
[CgSQLiteIndex(nameof(SourceElementKey), nameof(TargetFhirPackageKey))]
[CgSQLiteIndex(nameof(SourceFhirPackageKey), nameof(SourceStructureKey), nameof(SourceElementResourceOrder))]
public partial class DbElementOutcome : DbArtifactOutcomeBase
{
    [CgSQLiteForeignKey(referenceTable: "StructureOutcomes", referenceColumn: nameof(DbStructureOutcome.Key), modelTypeName: nameof(DbStructureOutcome))]
    public required int StructureOutcomeKey { get; set; }

    [CgSQLiteForeignKey(referenceTable: "Structures", referenceColumn: nameof(DbStructureDefinition.Key), modelTypeName: nameof(DbStructureDefinition))]
    public required int SourceStructureKey { get; set; }

    [CgSQLiteForeignKey(referenceTable: "Elements", referenceColumn: nameof(DbElement.Key), modelTypeName: nameof(DbElement))]
    public required int SourceElementKey { get; set; }

    public required string SourceElementId { get; set; }

    public required int SourceElementResourceOrder { get; set; }
    public required int SourceElementComponentOrder { get; set; }

    [CgSQLiteForeignKey(referenceTable: "Structures", referenceColumn: nameof(DbStructureDefinition.Key), modelTypeName: nameof(DbStructureDefinition))]
    public required int TargetStructureKey { get; set; }

    [CgSQLiteForeignKey(referenceTable: "Elements", referenceColumn: nameof(DbElement.Key), modelTypeName: nameof(DbElement))]
    public required int? TargetElementKey { get; set; }

    public required string? TargetElementId { get; set; }

    public required int? TargetElementResourceOrder { get; set; }
    public required int? TargetElementComponentOrder { get; set; }


    [CgSQLiteForeignKey(referenceTable: "ExtensionSubstituions", referenceColumn: nameof(DbExtensionSubstitution.Key), modelTypeName: nameof(DbExtensionSubstitution))]
    public required int? ExtensionSubstitutionKey { get; set; }

    [CgSQLiteForeignKey(referenceTable: "ElementOutcomes", referenceColumn: nameof(Key), modelTypeName: nameof(DbElementOutcome))]
    public required int? RelatedAncestorOutcomeKey { get; set; }

    public required OutcomeElementActionCodes? OutcomeAction { get; set; }

    [CgSQLiteIgnore]
    public Hl7.Fhir.Model.StructureDefinition? PotentialGenExtension { get; set; } = null;
}


[CgSQLiteTable(tableName: "ValueSetOutcomes")]
[CgSQLiteIndex(nameof(SourceFhirPackageKey), nameof(SourceValueSetKey), nameof(TargetFhirPackageKey))]
[CgSQLiteIndex(nameof(SourceValueSetKey), nameof(TargetFhirPackageKey))]
public partial class DbValueSetOutcome : DbArtifactOutcomeBase
{
    [CgSQLiteForeignKey(referenceTable: "ValueSets", referenceColumn: nameof(DbValueSet.Key))]
    public required int SourceValueSetKey { get; set; }
    public required string SourceValueSetUnversionedUrl { get; set; }
    public required string SourceValueSetVersion { get; set; }

    [CgSQLiteForeignKey(referenceTable: "ValueSets", referenceColumn: nameof(DbValueSet.Key))]
    public required int? TargetValueSetKey { get; set; }
    public required string? TargetValueSetUnversionedUrl { get; set; }
    public required string? TargetValueSetVersion { get; set; }

    public required OutcomeValueSetActionCodes? OutcomeAction { get; set; }

    [CgSQLiteIgnore]
    public Hl7.Fhir.Model.ValueSet? PotentialGenValueSet { get; set; } = null;
}

[CgSQLiteTable(tableName: "ValueSetConceptOutcomes")]
[CgSQLiteIndex(nameof(SourceFhirPackageKey), nameof(SourceValueSetKey), nameof(SourceValueSetConceptKey), nameof(TargetFhirPackageKey))]
public partial class DbValueSetConceptOutcome : DbOutcomeBase
{
    [CgSQLiteForeignKey(referenceTable: "ValueSetOutcomes", referenceColumn: nameof(DbValueSetOutcome.Key))]
    public required int ValueSetOutcomeKey { get; set; }

    [CgSQLiteForeignKey(referenceTable: "ValueSets", referenceColumn: nameof(DbValueSet.Key))]
    public required int SourceValueSetKey { get; set; }

    [CgSQLiteForeignKey(referenceTable: "ValueSetConcepts", referenceColumn: nameof(DbValueSetConcept.Key))]
    public required int SourceValueSetConceptKey { get; set; }

    public required string SourceValueSetConceptSystem { get; set; }
    public required string SourceValueSetConceptCode { get; set; }

    [CgSQLiteForeignKey(referenceTable: "ValueSets", referenceColumn: nameof(DbValueSet.Key))]
    public required int? TargetValueSetKey { get; set; }

    [CgSQLiteForeignKey(referenceTable: "ValueSetConcepts", referenceColumn: nameof(DbValueSetConcept.Key))]
    public required int? TargetValueSetConceptKey { get; set; }

    public required string? TargetValueSetConceptSystem { get; set; }
    public required string? TargetValueSetConceptCode { get; set; }

    public required OutcomeValueSetConceptActionCodes? OutcomeAction { get; set; }
}
