using Microsoft.CodeAnalysis;

namespace Microsoft.Health.Fhir.SourceGenerator;

internal static class DiagnosticDescriptors
{
    public static readonly DiagnosticDescriptor FailedArtifactDef = new(
        "FHIRGEN001",
        "Invalid artifact file",
        "Unable to parse {1} file '{0}'. StructureDefinition files must have a snapshot.",
        "FhirSourceGenerator",
        DiagnosticSeverity.Warning,
        true);

    public static readonly DiagnosticDescriptor TypeLoaderException = new(
        "FHIRGEN002",
        "Fatal FhirCode Generation error",
        "Type loader exception '{0}'.",
        "FhirSourceGenerator",
        DiagnosticSeverity.Error,
        true);

    public static readonly DiagnosticDescriptor UnhandledException = new(
        "FHIRGEN003",
        "Fatal FhirCode Generation error",
        "Unhandled exception '{0}'.",
        "FhirSourceGenerator",
        DiagnosticSeverity.Error,
        true);

    public static readonly DiagnosticDescriptor FailedToGenerate = new(
        "FHIRGEN004",
        "Invalid .StructureDefinition.json file",
        "Failed to generate code for json file [{0}]{1}.",
        "FhirSourceGenerator",
        DiagnosticSeverity.Error,
        true);

    public static readonly DiagnosticDescriptor ProcessSuccess = new(
        "FHIRGEN005",
        "Parsing success",
        "JsonPath: {0} Canonical: {1}, Artifact Type: {2}, ResourceCount: {3}",
        "FhirSourceGenerator",
        DiagnosticSeverity.Info,
        true);
}
