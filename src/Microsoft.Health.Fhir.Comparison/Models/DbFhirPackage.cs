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
    public required string ShortName { get; set; } = null!;

    public ICollection<DbValueSet> ValueSets { get; set; } = [];
    public ICollection<DbValueSetConcept> ValueSetConcepts { get; set; } = [];

    public ICollection<DbStructureDefinition> Structures { get; set; } = [];
    public ICollection<DbElement> Elements { get; set; } = [];

    public ICollection<DbFhirPackageComparisonPair> ComparisonsAsSource { get; set; } = [];

    public ICollection<DbFhirPackageComparisonPair> ComparisonsAsTarget { get; set; } = [];
}

