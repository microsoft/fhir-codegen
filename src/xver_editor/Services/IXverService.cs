using System.Data;
using Fhir.CodeGen.Comparison.CompareTool;
using Fhir.CodeGen.Comparison.Models;

namespace xver_editor.Services;

public interface IXverService : IHostedService
{
    IDbConnection DbConnection { get; }
    ComparisonDatabase ComparisonDb { get; }
    FhirDbComparer Comparer { get; }

    IQueryable<DbFhirPackage> Packages { get; }
    bool IsOpen { get; }

    (bool success, string? message) Init(string? path = null);

    void CloseDb();

    Task WriteDocsFromDatabase(string outputDirectory);
    Task WriteFhirFromDatabase(string outputDirectory, string version);
}
