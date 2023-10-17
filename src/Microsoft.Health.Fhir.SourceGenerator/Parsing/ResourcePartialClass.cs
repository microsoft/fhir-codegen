using Microsoft.CodeAnalysis;

namespace Microsoft.Health.Fhir.SourceGenerator.Parsing;

internal record class ResourcePartialClass(Location Location, string Namespace, string Name, string StructureDefinitionPath, string[] TerminologyResourcePaths);
