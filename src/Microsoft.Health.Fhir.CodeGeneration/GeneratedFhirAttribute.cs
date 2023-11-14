using System;

namespace Synapxe.Fhir.CodeGeneration;

[AttributeUsage(AttributeTargets.Class)]
public class GeneratedFhirAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GeneratedFhirAttribute"/> class.
    /// </summary>
    public GeneratedFhirAttribute(string structureDefinitionPath)
    {
        StructureDefinitionPath = structureDefinitionPath;
    }

    /// <summary>
    /// The relative path to the structure definition resource json file.
    /// </summary>
    public string StructureDefinitionPath { get; }

    /// <summary>
    /// The relative paths to the terminology resource (CodeSystem/ValueSet) json files.
    /// </summary>
    public string[] TerminologyResources { get; set; } = Array.Empty<string>();
}
