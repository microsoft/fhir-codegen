using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using fhir_codegen.SQLiteGenerator;

namespace Microsoft.Health.Fhir.Comparison.Models;

[CgSQLiteTable(tableName: "FhirPackages")]
public partial class DbFhirPackage
{
    [CgSQLiteKey]
    public int Key { get; set; } = -1;

    public required string Name { get; set; } = null!;
    public required string PackageId { get; set; } = null!;
    public required string PackageVersion { get; set; } = null!;
    public required string CanonicalUrl { get; set; } = null!;
    public required string ShortName { get; set; } = null!;
}

