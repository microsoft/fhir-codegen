using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using fhir_codegen.SQLiteGenerator;
using Microsoft.Health.Fhir.CodeGenCommon.Models;
using CMR = Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship;

namespace Microsoft.Health.Fhir.Comparison.Models;


[CgSQLiteTable]
public partial class ComparisonInfo
{
    [CgSQLiteKey]
    public required long Id { get; set; }

    public required string SourcePackageId { get; init; }
    public required string SourcePackageVersion { get; init; }

    public required string TargetPackageId { get; init; }
    public required string TargetPackageVersion { get; init; }

}

/// <summary>
/// Represents a value set mapping record.
/// </summary>
/// <remarks>
/// These records appear in a table named ValueSetMappings.
/// </remarks>
[CgSQLiteTable]
public partial class ValueSetMapping
{
    [CgSQLiteKey]
    public required long Id { get; set; }

    public required string SourceCanonical { get; set; }
    public required string SourceName { get; set; }

    public required string? TargetCanonical { get; set; }
    public required string? TargetName { get; set; }

    public required string? CompositeName { get; set; }

    public required CMR? AggregateRelationship { get; set; }
    public required string? Comment { get; set; }

    public required string? LastReviewedBy { get; set; }
    public required DateTime? LastReviewedOn { get; set; }
}


/// <summary>
/// Represents a value set code mapping record.
/// </summary>
/// <remarks>
/// These records appear in tables named "ValueSet-" + ValueSetMapping.CompositeName.
/// </remarks>
[CgSQLiteTable]
public partial class ValueSetCodeMapping
{
    [CgSQLiteKey]
    public required int Id { get; set; }

    [CgSQLiteForeignKey("ValueSetMappings", "Id")]
    public required int ValueSetMappingId { get; set; }

    public required string SourceSystem { get; set; }
    public required string SourceCode { get; set; }
    public required string SourceDisplay { get; set; }

    public required bool IsNotMapped { get; set; }

    public required string? TargetSystem { get; set; }
    public required string? TargetCode { get; set; }
    public required string? TargetDisplay { get; set; }

    public required CMR? Relationship { get; set; }
    public required string? Comment { get; set; }
    public required bool IsGenerated { get; set; }
    public bool HasBeenReviewed { get; set; } = false;
}

[CgSQLiteTable]
public partial class StructureMapping
{
    [CgSQLiteKey]
    public required int Id { get; set; }

    public required string SourceCanonical { get; set; }
    public required string SourceName { get; set; }
    public required FhirArtifactClassEnum SourceArtifactClass { get; set; }

    public required string TargetCanonical { get; set; }
    public required string TargetName { get; set; }
    public required FhirArtifactClassEnum TargetArtifactClass { get; set; }

    public required string CompositeName { get; set; }

    public required CMR? AggregateRelationship { get; set; }
    public required string? Comment { get; set; }

    public required string? LastReviewedBy { get; set; }
    public required DateTimeOffset? LastReviewedOn { get; set; }
}


/// <summary>
/// Represents a Structure Element mapping record.
/// </summary>
/// <remarks>
/// These records appear in tables named "Structure-" + StructureMappingRec.CompositeName.
/// </remarks>
[CgSQLiteTable]
public partial class StructureElementMapping
{
    [CgSQLiteKey]
    public required int Id { get; set; }

    [CgSQLiteForeignKey("StructureMapping", "Id")]
    public required int StructureMappingId { get; set; }

    public required string SourceResourceId { get; set; }
    public required string SourcePath { get; set; }
    public required string? SourceType { get; set; }

    public required bool IsNotMapped { get; set; }

    public required string? TargetResourceId { get; set; }
    public required string? TargetPath { get; set; }
    public required string? TargetType { get; set; }

    public required CMR? Relationship { get; set; }
    public required CMR? ConceptDomainRelationship { get; set; }
    public required CMR? ValueDomainRelationship { get; set; }
    public required string? Comment { get; set; }
    public required bool IsGenerated { get; set; }
    public bool HasBeenReviewed { get; set; } = false;
}
