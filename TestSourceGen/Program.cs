// See https://aka.ms/new-console-template for more information

using Microsoft.CodeAnalysis;
using Microsoft.Health.Fhir.SourceGenerator;
using Microsoft.Health.Fhir.SourceGenerator.Parsing;

var resourceClass = new ResourcePartialClass(Location.None, typeof(Program).Namespace!, "Patient", "Patient.StructureDefinition.json", Array.Empty<string>());

var emitter = new Emitter(resourceClass, diag => Console.Error.WriteLine(diag.GetMessage()));

var code = emitter.Emit();

Console.WriteLine(code);
