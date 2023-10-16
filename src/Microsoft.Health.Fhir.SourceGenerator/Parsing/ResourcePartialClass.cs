namespace Microsoft.Health.Fhir.SourceGenerator.Parsing;

internal record class ResourcePartialClass(string Namespace, string Name, string StructureDefinitionPath, string[] TerminologyResourcePaths);
