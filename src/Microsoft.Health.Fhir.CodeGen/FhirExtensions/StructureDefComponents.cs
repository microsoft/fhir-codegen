// <copyright file="StructureDefComponents.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>


using Hl7.Fhir.Model;
using Microsoft.Health.Fhir.CodeGen.Models;

namespace Microsoft.Health.Fhir.CodeGen.FhirExtensions;

public static class StructureDefComponents
{
    /// <summary>Get a ComponentDefinition for a StructureDefinition</summary>
    /// <param name="sd">The StructureDefinition.</param>
    /// <returns>A ComponentDefinition.</returns>
    public static ComponentDefinition cgComponent(this StructureDefinition sd)
    {
        return new()
        {
            Structure = sd,
            Element = sd.cgRootElement() ?? sd.Snapshot?.Element.FirstOrDefault() ?? sd.Differential.Element.First(),
            IsRootOfStructure = true,
        };
    }

    /// <summary>Generates the component definitions for a StructureDefinition.</summary>
    /// <param name="sd">              The StructureDefinition.</param>
    /// <param name="dc">              The DefinitionCollection.</param>
    /// <param name="includeStartComponent">     (Optional) Flag indicating whether to include the root
    ///  component.</param>
    /// <param name="currentLevelOnly">(Optional) True to current level only.</param>
    /// <returns>An enumerable collection of ComponentDefinition.</returns>
    public static IEnumerable<ComponentDefinition> cgComponents(
        this StructureDefinition sd,
        DefinitionCollection dc,
        ElementDefinition? startingElement = null,
        bool includeStartComponent = false,
        bool currentLevelOnly = false)
    {
        // start with root
        if (includeStartComponent)
        {
            if (startingElement == null)
            {
                yield return new()
                {
                    Structure = sd,
                    Element = sd.cgRootElement() ?? sd.Snapshot?.Element?.FirstOrDefault() ?? sd.Differential.Element.First(),
                    IsRootOfStructure = true,
                };
            }
            else
            {
                yield return new()
                {
                    Structure = sd,
                    Element = startingElement,
                    IsRootOfStructure = false,
                };
            }
        }

        // check all elements for being a backbone element
        foreach (ElementDefinition ed in sd.cgElements(startingElement?.Path ?? string.Empty, currentLevelOnly, false))
        {
            if (ed.IsBackboneElement())
            {
                yield return new()
                {
                    Structure = sd,
                    Element = ed,
                    IsRootOfStructure = false,
                };
            }
        }
    }
}
