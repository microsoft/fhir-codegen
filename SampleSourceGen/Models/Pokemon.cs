using Synapxe.Fhir.CodeGeneration;

namespace SampleSourceGen.Models;

[GeneratedFhir(
    "Models/Pokemon.StructureDefinition.json",
    TerminologyResources = new[] { "Models/PokemonType.CodeSystem.json", "Models/PokemonType.ValueSet.json" })]
public partial class Pokemon
{
}
