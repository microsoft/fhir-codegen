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
    public required string FhirVersionShort { get; set; } = null!;
    public required string CanonicalUrl { get; set; } = null!;
    public required string ShortName { get; set; } = null!;

    public required Microsoft.Health.Fhir.CodeGenCommon.Packaging.FhirReleases.FhirSequenceCodes DefinitionFhirSequence { get; set; } = Microsoft.Health.Fhir.CodeGenCommon.Packaging.FhirReleases.FhirSequenceCodes.Unknown;

    [CgSQLiteIgnore]
    public string NpmId => (string.IsNullOrEmpty(PackageId) && string.IsNullOrEmpty(PackageVersion))
    ? string.Empty
    : $"{PackageId}@{PackageVersion}";

}

