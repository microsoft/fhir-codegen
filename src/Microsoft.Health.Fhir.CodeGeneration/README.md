# Microsoft.Health.Fhir.CodeGeneration

This package adds code generation for creating Fhir classes for custom resources.

## Basic Usage

Install the nuget package and create a partial class with the `GeneratedFhirAttribute`.

```csharp
[GeneratedFhir("Models/Pokemon.StructureDefinition.json",
   TerminologyResources = new[] { "Models/PokemonType.CodeSystem.json", "Models/PokemonType.ValueSet.json" })]
public partial class Pokemon
{
}
```

