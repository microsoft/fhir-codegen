using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using fhir_codegen.SQLiteGenerator;
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
