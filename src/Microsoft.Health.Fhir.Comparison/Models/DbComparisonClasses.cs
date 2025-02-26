using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Health.Fhir.CodeGen.Utils;
using Microsoft.Health.Fhir.CodeGenCommon.Models;
using Microsoft.Health.Fhir.CodeGenCommon.Utils;
using fhir_codegen.SQLiteGenerator;


namespace Microsoft.Health.Fhir.Comparison.Models;


[CgSQLiteTable(tableName: "PackageComparisonPairs")]
public partial class DbFhirPackageComparisonPair
{
    [CgSQLiteKey]
    public int Key { get; set; } = -1;

    [CgSQLiteForeignKey(referenceTable: "FhirPackages", referenceColumn: nameof(DbFhirPackage.Key))]
    public required int SourcePackageKey { get; set; }

    [CgSQLiteForeignKey(referenceTable: "FhirPackages", referenceColumn: nameof(DbFhirPackage.Key))]
    public required int TargetPackageKey { get; set; }

    public DateTime ProccessedAt { get; set; } = DateTime.UtcNow;
}

[CgSQLiteBaseClass]
public abstract class DbPackageComparisonContent
{
    [CgSQLiteKey]
    public int Key { get; set; } = -1;

    [CgSQLiteForeignKey(referenceTable: "PackageComparisonPairs", referenceColumn: nameof(DbFhirPackageComparisonPair.Key))]
    public required int PackageComparisonKey { get; set; }

    [CgSQLiteForeignKey(referenceTable: "FhirPackages", referenceColumn: nameof(DbFhirPackage.Key))]
    public required int SourceFhirPackageKey { get; set; }

    [CgSQLiteForeignKey(referenceTable: "FhirPackages", referenceColumn: nameof(DbFhirPackage.Key))]
    public required int TargetFhirPackageKey { get; set; }

    public required Hl7.Fhir.Model.ConceptMap.ConceptMapRelationship? Relationship { get; set; }
    public required string? Message { get; set; }
    public required bool? IsGenerated { get; set; }
    public required string? LastReviewedBy { get; set; }
    public required DateTime? LastReviewedOn { get; set; }
}


[CgSQLiteTable(tableName: "ValueSetComparisons")]
public partial class DbValueSetComparison : DbPackageComparisonContent
{
    [CgSQLiteForeignKey(referenceTable: "ValueSets", referenceColumn: nameof(DbValueSet.Key))]
    public required int SourceKey { get; set; }

    public required string SourceCanonicalVersioned { get; set; }
    public required string SourceCanonicalUnversioned { get; set; }
    public required string SourceName { get; set; }
    public required string SourceVersion { get; set; }

    [CgSQLiteForeignKey(referenceTable: "ValueSets", referenceColumn: nameof(DbValueSet.Key))]
    public required int? TargetKey { get; set; }
    public required string? TargetCanonicalVersioned { get; set; }
    public required string? TargetCanonicalUnversioned { get; set; }
    public required string? TargetName { get; set; }
    public required string? TargetVersion { get; set; }

    public required string CompositeName { get; set; }
    public required string? SourceConceptMapUrl { get; set; }
    public required string? SourceConceptMapAdditionalUrls { get; set; }
}

[CgSQLiteTable(tableName: "ConceptComparisons")]
public partial class DbValueSetConceptComparison : DbPackageComparisonContent
{
    [CgSQLiteForeignKey(referenceTable: "ValueSetComparisons", referenceColumn: nameof(DbValueSetComparison.Key))]
    public required int CanonicalComparisonKey { get; set; }

    [CgSQLiteForeignKey(referenceTable: "ValueSets", referenceColumn: nameof(DbValueSet.Key))]
    public required int SourceCanonicalKey { get; set; }

    [CgSQLiteForeignKey(referenceTable: "ValueSets", referenceColumn: nameof(DbValueSet.Key))]
    public required int? TargetCanonicalKey { get; set; }

    [CgSQLiteForeignKey(referenceTable: "Concepts", referenceColumn: nameof(DbValueSetConcept.Key))]
    public required int SourceKey { get; set; }

    [CgSQLiteForeignKey(referenceTable: "Concepts", referenceColumn: nameof(DbValueSetConcept.Key))]
    public required int? TargetKey { get; set; }

    public required bool? NoMap { get; set; }
}

[CgSQLiteTable(tableName: "UnresolvedConceptComparisons")]
public partial class DbUnresolvedConceptComparisons : DbPackageComparisonContent
{
    [CgSQLiteForeignKey(referenceTable: "ValueSetComparisons", referenceColumn: nameof(DbValueSetComparison.Key))]
    public int CanonicalComparisonKey { get; set; }

    [CgSQLiteForeignKey(referenceTable: "ValueSets", referenceColumn: nameof(DbValueSet.Key))]
    public required int SourceCanonicalKey { get; set; }

    [CgSQLiteForeignKey(referenceTable: "ValueSets", referenceColumn: nameof(DbValueSet.Key))]
    public required int? TargetCanonicalKey { get; set; }


    public required string ConceptMapId { get; set; }
    public required string ConceptMapUrl { get; set; }


    [CgSQLiteForeignKey(referenceTable: "Concepts", referenceColumn: nameof(DbValueSetConcept.Key))]
    public required int? SourceConceptKey { get; set; }
    public required bool SourceConceptExists { get; set; } = false;
    public required string SourceSystem { get; set; }
    public required string SourceCode { get; set; }
    public required string? SourceDisplay { get; set; }

    [CgSQLiteForeignKey(referenceTable: "Concepts", referenceColumn: nameof(DbValueSetConcept.Key))]
    public required int? TargetConceptKey { get; set; }
    public required bool? TargetConceptExists { get; set; }
    public required string? TargetSystem { get; set; }
    public required string? TargetCode { get; set; }
    public required string? TargetDisplay { get; set; }

    public required bool? NoMap { get; set; }
}
