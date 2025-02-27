using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fhir_codegen.SQLiteGenerator;


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
public partial class DbValueSet : DbCanonicalResource
{
    public required bool CanExpand { get; set; }
    public required bool? HasEscapeValveCode { get; set; }
    public required string? Message { get; set; }
    public required bool IsExcluded { get; set; } = false;

    public required int ConceptCount { get; set; }
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
public partial class DbValueSetConcept : DbPackageContent
{
    [CgSQLiteForeignKey(referenceTable: "ValueSets", referenceColumn: nameof(DbValueSet.Key))]
    public required int ValueSetKey { get; set; }

    public required string System { get; set; }
    public required string Code { get; set; }
    public required string? Display { get; set; }
}

[CgSQLiteTable(tableName: "Structures")]
public partial class DbStructureDefinition : DbCanonicalResource
{
    public required string? Comment { get; set; }
    public required string? Message { get; set; }

    public required Microsoft.Health.Fhir.CodeGenCommon.Models.FhirArtifactClassEnum ArtifactClass { get; set; } = Microsoft.Health.Fhir.CodeGenCommon.Models.FhirArtifactClassEnum.Unknown;
}


[CgSQLiteTable(tableName: "Elements")]
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

    public required string? TypeName { get; set; }
    public required string? TypeProfile { get; set; }
    public required string? TargetProfile { get; set; }

    public required Hl7.Fhir.Model.BindingStrength? ValueSetBindingStrength { get; init; }
    public required string? BindingValueSet { get; set; }
}
