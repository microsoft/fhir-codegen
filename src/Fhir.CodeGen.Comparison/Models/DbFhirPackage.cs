using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fhir.CodeGen.SQLiteGenerator;

namespace Fhir.CodeGen.Comparison.Models;

[CgSQLiteTable(tableName: "FhirPackages")]
public partial class DbFhirPackage : DbRecordBase
{
    public required string Name { get; set; }
    public required string PackageId { get; set; }
    public required string PackageVersion { get; set; }
    public required string FhirVersionShort { get; set; }
    public required string CanonicalUrl { get; set; }
    public required string ShortName { get; set; }

    public required string? Dependencies { get; set; }

    public required Fhir.CodeGen.Common.Packaging.FhirReleases.FhirSequenceCodes DefinitionFhirSequence { get; set; } = Fhir.CodeGen.Common.Packaging.FhirReleases.FhirSequenceCodes.Unknown;

    [CgSQLiteIgnore]
    public string NpmId => (string.IsNullOrEmpty(PackageId) && string.IsNullOrEmpty(PackageVersion))
        ? string.Empty
        : $"{PackageId}@{PackageVersion}";

    [CgSQLiteIgnore]
    public string CacheFolderName => (string.IsNullOrEmpty(PackageId) && string.IsNullOrEmpty(PackageVersion))
        ? string.Empty
        : $"{PackageId}#{PackageVersion}";
}

