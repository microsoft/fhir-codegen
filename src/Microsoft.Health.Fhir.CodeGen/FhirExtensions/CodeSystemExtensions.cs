using System;
using System.Collections.Generic;
using System.Text;
using Hl7.Fhir.ElementModel.Types;
using Hl7.Fhir.Model;
using Microsoft.Health.Fhir.CodeGenCommon.FhirExtensions;

namespace Microsoft.Health.Fhir.CodeGen.FhirExtensions;

public static class CodeSystemExtensions
{
    /// <summary>
    /// Gets the standards status of this definition (e.g., trial-use, normative).
    /// </summary>
    /// <param name="cs">The CodeSystem to act on.</param>
    /// <returns>A string representing the standards status.</returns>
    public static string cgStandardStatus(this CodeSystem cs) => cs.GetExtensionValue<Hl7.Fhir.Model.Code>(CommonDefinitions.ExtUrlStandardStatus)?.ToString() ?? string.Empty;

    /// <summary>
    /// Gets the Work Group responsible for this definition.
    /// </summary>
    /// <param name="cs">The CodeSystem to act on.</param>
    /// <returns>A string representing the Work Group.</returns>
    public static string cgWorkGroup(this CodeSystem cs) => cs.GetExtensionValue<FhirString>(CommonDefinitions.ExtUrlWorkGroup)?.Value ?? string.Empty;

    /// <summary>
    /// Gets the FHIR Maturity Model (FMM) level of this definition, or 0 if not specified.
    /// </summary>
    /// <param name="cs">The CodeSystem to act on.</param>
    /// <returns>An int representing the FMM level.</returns>
    public static int? cgMaturityLevel(this CodeSystem cs) => cs.GetExtensionValue<Hl7.Fhir.Model.Integer>(CommonDefinitions.ExtUrlFmm)?.Value;



    public static IEnumerable<CodeSystem.ConceptDefinitionComponent> cgGetFlat(this IEnumerable<CodeSystem.ConceptDefinitionComponent> concepts)
    {
        foreach (CodeSystem.ConceptDefinitionComponent concept in concepts)
        {
            processConcept(concept);
        }

        yield break;

        IEnumerable<CodeSystem.ConceptDefinitionComponent> processConcept(CodeSystem.ConceptDefinitionComponent concept)
        {
            foreach (CodeSystem.ConceptDefinitionComponent nestedConcept in concept.Concept)
            {
                if (nestedConcept.Concept?.Count > 0)
                {
                    processConcept(nestedConcept);
                }

                yield return nestedConcept;
            }

            yield return concept;
        }
    }
}
