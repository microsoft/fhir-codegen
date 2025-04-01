using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using fhir_codegen.SQLiteGenerator;
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
public partial class DbValueSetConcept : DbPackageContent
{
    [CgSQLiteForeignKey(referenceTable: "ValueSets", referenceColumn: nameof(DbValueSet.Key))]
    public required int ValueSetKey { get; set; }
    public required string System { get; set; }
    public required string Code { get; set; }
    public required string? Display { get; set; }
    public required bool Inactive { get; set; }
    public required bool Abstract { get; set; }
    public required string? Properties { get; set; }

    [CgSQLiteIgnore]
    public string FhirKey => $"{System}#{Code}";
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

    public required string? SliceName { get; set; }

    public required string CollatedTypeLiteral { get; set; }

    public required Hl7.Fhir.Model.BindingStrength? ValueSetBindingStrength { get; init; }
    public required string? BindingValueSet { get; set; }
    public required int? BindingValueSetKey { get; set; }
    public required int AdditionalBindingCount { get; set; }

    public required bool IsInherited { get; set; }
    public required string? BasePath { get; set; }
    public required bool IsSimpleType { get; set; }
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
