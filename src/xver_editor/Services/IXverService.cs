using System.Data;
using Microsoft.Health.Fhir.Comparison.CompareTool;
using Microsoft.Health.Fhir.Comparison.Models;

namespace xver_editor.Services;

public interface IXverService : IHostedService
{
    IDbConnection DbConnection { get; }
    ComparisonDatabase ComparisonDb { get; }
    FhirDbComparer Comparer { get; }

    Task WriteDocsFromDatabase(string outputDirectory);
}
