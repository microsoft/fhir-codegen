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
using Hl7.Fhir.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Health.Fhir.CodeGen.Utils;
using Microsoft.Health.Fhir.CodeGenCommon.Models;
using Microsoft.Health.Fhir.CodeGenCommon.Utils;


namespace Microsoft.Health.Fhir.Comparison.Models;


public class DbFhirPackageComparisonPair
{
    [Key]
    public int Key { get; set; }

    public int SourcePackageKey { get; set; }
    public required DbFhirPackage SourcePackage { get; set; } = null!;

    public int TargetPackageKey { get; set; }
    public required DbFhirPackage TargetPackage { get; set; } = null!;

    public DateTime ProccessedAt { get; set; } = DateTime.UtcNow;

    public ICollection<DbValueSetComparison> ValueSetComparisons { get; set; } = [];
    public ICollection<DbValueSetConceptComparison> ValueSetConceptComparisons { get; set; } = [];
    public ICollection<DbInvalidConceptComparison> InvalidImportedConceptComparisons { get; set; } = [];
}

public abstract class DbPackageComparisonContent
{
    [Key]
    public int Key { get; set; }

    public int PackageComparisonKey { get; set; } = -1;
    public required DbFhirPackageComparisonPair PackageComparison { get; set; } = null!;

    public int SourceFhirPackageKey { get; set; }
    public required DbFhirPackage SourceFhirPackage { get; init; } = null!;
    public int TargetFhirPackageKey { get; set; }
    public required DbFhirPackage TargetFhirPackage { get; init; } = null!;

    public required ConceptMap.ConceptMapRelationship? Relationship { get; set; } = null;
    public required string? Message { get; set; } = null;
    public required bool? IsGenerated { get; set; } = null;
    public required string? LastReviewedBy { get; set; } = null;
    public required DateTime? LastReviewedOn { get; set; } = null;
}



public class DbValueSetComparison : DbPackageComparisonContent
{
    public int SourceKey { get; set; } = -1;
    public required DbValueSet? Source { get; set; } = null;
    public required string SourceCanonicalVersioned { get; set; } = null!;
    public required string SourceCanonicalUnversioned { get; set; } = null!;
    public required string SourceName { get; set; } = null!;
    public required string SourceVersion { get; set; } = null!;

    public int? TargetKey { get; set; } = null;
    public required DbValueSet? Target { get; set; } = null;
    public required string? TargetCanonicalVersioned { get; set; } = null;
    public required string? TargetCanonicalUnversioned { get; set; } = null;
    public required string? TargetName { get; set; } = null;
    public required string? TargetVersion { get; set; } = null;

    public required string CompositeName { get; set; } = null!;
    public required string? SourceConceptMapUrl { get; set; } = null;
    public required string? SourceConceptMapAdditionalUrls { get; set; } = null;

    public ICollection<DbValueSetConceptComparison> ComponentComparisons { get; set; } = [];
    public ICollection<DbInvalidConceptComparison> InvalidImportedComparisons { get; set; } = [];
}

public class DbValueSetConceptComparison : DbPackageComparisonContent
{
    public int CanonicalComparisonKey { get; set; } = -1;
    public required DbValueSetComparison CanonicalComparison { get; set; } = null!;

    public int SourceCanonicalKey { get; set; } = -1;
    public required DbValueSet SourceCanonical { get; set; } = null!;

    public int? TargetCanonicalKey { get; set; } = null;
    public required DbValueSet? TargetCanonical { get; set; } = null;

    public int SourceKey { get; set; } = -1;
    public required DbValueSetConcept Source { get; set; } = null!;

    public int TargetKey { get; set; } = -1;
    public required DbValueSetConcept? Target { get; set; } = null;

    public required bool? NoMap { get; set; } = null;
}

public class DbInvalidConceptComparison : DbPackageComparisonContent
{
    public int CanonicalComparisonKey { get; set; } = -1;
    public required DbValueSetComparison CanonicalComparison { get; set; } = null!;

    public int SourceCanonicalKey { get; set; } = -1;
    public required DbValueSet? SourceCanonical { get; set; } = null;

    public int? TargetCanonicalKey { get; set; } = null;
    public required DbValueSet? TargetCanonical { get; set; } = null;

    public required string ConceptMapId { get; set; } = null!;
    public required string ConceptMapUrl { get; set; } = null!;


    public required bool SourceExists { get; set; } = false;
    public required string SourceSystem { get; set; } = null!;
    public required string SourceCode { get; set; } = null!;
    public required string? SourceDisplay { get; set; } = null;

    public required bool? TargetExists { get; set; } = null;
    public required string? TargetSystem { get; set; } = null;
    public required string? TargetCode { get; set; } = null;
    public required string? TargetDisplay { get; set; } = null;

    public required bool? NoMap { get; set; } = null;
}
