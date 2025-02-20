using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Health.Fhir.Comparison.Models;

public class DbFhirPackage
{
    [Key]
    public int Key { get; set; }

    public required string Name { get; set; } = null!;
    public required string PackageId { get; set; } = null!;
    public required string PackageVersion { get; set; } = null!;
    public required string CanonicalUrl { get; set; } = null!;

    public ICollection<DbValueSet> ValueSets { get; init; } = null!;
    public ICollection<DbStructureDefinition> Structures { get; init; } = null!;

    public ICollection<DbFhirPackageComparisonPair> SourceDiffs { get; init; } = null!;

    public ICollection<DbFhirPackageComparisonPair> TargetDiffs { get; init; } = null!;
}

