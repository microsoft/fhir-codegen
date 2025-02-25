using System;
using System.Collections.Generic;
using System.Text;
using Hl7.Fhir.ElementModel.Types;
using Hl7.Fhir.Model;

namespace Microsoft.Health.Fhir.CodeGen.FhirExtensions;

public static class CodeSystemExtensions
{
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
