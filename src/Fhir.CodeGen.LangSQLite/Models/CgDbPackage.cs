using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fhir.CodeGen.SQLiteGenerator;

namespace Fhir.CodeGen.LangSQLite.Models;



[CgSQLiteTable(tableName: "Packages")]
public partial class CgDbPackage : CgDbBase
{
    public required string Name { get; set; }
    public required string PackageId { get; set; }
    public required string PackageVersion { get; set; }
    public required string FhirVersionShort { get; set; }
    public required string CanonicalUrl { get; set; }
    public required string? WebUrl { get; set; }
    public required string ShortName { get; set; }
    public required string? Title { get; set; }
    public required string? Description { get; set; }

    public required string? Dependencies { get; set; }

    public required Fhir.CodeGen.Common.Packaging.FhirReleases.FhirSequenceCodes DefinitionFhirSequence { get; set; } = Fhir.CodeGen.Common.Packaging.FhirReleases.FhirSequenceCodes.Unknown;

    public required DateTimeOffset? ProcessDate { get; set; }

    [CgSQLiteIgnore]
    public string NpmId => (string.IsNullOrEmpty(PackageId) && string.IsNullOrEmpty(PackageVersion))
        ? string.Empty
        : $"{PackageId}@{PackageVersion}";

    [CgSQLiteIgnore]
    public string CacheFolderName => (string.IsNullOrEmpty(PackageId) && string.IsNullOrEmpty(PackageVersion))
        ? string.Empty
        : $"{PackageId}#{PackageVersion}";
}

