// <copyright file="ValueSetExtensions.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Hl7.Fhir.Model;
using Microsoft.Health.Fhir.CodeGen.Models;
using Microsoft.Health.Fhir.CodeGen.Utils;
using Microsoft.Health.Fhir.CodeGenCommon.FhirExtensions;

namespace Microsoft.Health.Fhir.CodeGen.FhirExtensions;

public static class ValueSetExtensions
{
    /// <summary>
    /// Gets the standards status of this definition (e.g., trial-use, normative).
    /// </summary>
    /// <param name="vs">The ValueSet to act on.</param>
    /// <returns>A string representing the standards status.</returns>
    public static string cgStandardStatus(this ValueSet vs) => vs.GetExtensionValue<Code>(CommonDefinitions.ExtUrlStandardStatus)?.ToString() ?? string.Empty;

    /// <summary>
    /// Gets the FHIR Maturity Model (FMM) level of this definition, or 0 if not specified.
    /// </summary>
    /// <param name="vs">The ValueSet to act on.</param>
    /// <returns>An int representing the FMM level.</returns>
    public static int? cgMaturityLevel(this ValueSet vs) => vs.GetExtensionValue<Integer>(CommonDefinitions.ExtUrlFmm)?.Value;

    /// <summary>
    /// Gets a flag indicating if this definition is experimental.
    /// </summary>
    /// <param name="vs">The ValueSet to act on.</param>
    /// <returns>True if the definition is experimental, false otherwise.</returns>
    public static bool cgIsExperimental(this ValueSet vs) => vs.Experimental ?? false;

    /// <summary>
    /// Gets the Work Group responsible for this definition.
    /// </summary>
    /// <param name="vs">The ValueSet to act on.</param>
    /// <returns>A string representing the Work Group.</returns>
    public static string cgWorkGroup(this ValueSet vs) => vs.GetExtensionValue<FhirString>(CommonDefinitions.ExtUrlWorkGroup)?.Value ?? string.Empty;

    /// <summary>
    /// Determines whether the ValueSet is limited expansion.
    /// </summary>
    /// <param name="vs">The ValueSet to act on.</param>
    /// <returns>True if the ValueSet is limited expansion, false otherwise.</returns>
    public static bool IsLimitedExpansion(this ValueSet vs)
    {
        if (vs.Expansion?.Parameter == null)
        {
            return false;
        }

        ValueSet.ParameterComponent? vspc = vs.Expansion.Parameter.Where(pc => pc.Name == "limitedExpansion").FirstOrDefault();

        if (vspc == null)
        {
            return false;
        }

        return vspc.Value switch
        {
            FhirBoolean fb => (fb.Value == true),
            FhirString fs => fs.Value.Equals("true", StringComparison.OrdinalIgnoreCase) || fs.Value.Equals("-1", StringComparison.Ordinal),
            Integer i => i.Value == -1,
            _ => false,
        };
    }

    /// <summary>
    /// Gets the key for the ValueSet.ContainsComponent.
    /// </summary>
    /// <param name="c">The ContainsComponent to act on.</param>
    /// <returns>A string representing the key.</returns>
    public static string cgKey(this ValueSet.ContainsComponent c) => c.System + "#" + c.Code;

    /// <summary>
    /// Gets the local name of the system in the ValueSet.ContainsComponent.
    /// </summary>
    /// <param name="c">The ContainsComponent to act on.</param>
    /// <returns>A string representing the local name of the system.</returns>
    public static string cgSystemLocalName(this ValueSet.ContainsComponent c)
    {
        if (string.IsNullOrEmpty(c.System))
        {
            return string.Empty;
        }

        if (c.System.StartsWith(CommonDefinitions.FhirUrlPrefix, StringComparison.Ordinal))
        {
            return FhirSanitizationUtils.SanitizeForProperty(c.System.Substring(20));
        }

        if (c.System.StartsWith(CommonDefinitions.THOCsUrlPrefix, StringComparison.Ordinal))
        {
            return FhirSanitizationUtils.SanitizeForProperty(c.System.Substring(38));
        }

        return FhirSanitizationUtils.SanitizeForProperty(c.System);
    }

    /// <summary>
    /// Gets the referenced code systems in the ValueSet.
    /// </summary>
    /// <param name="vs">The ValueSet to act on.</param>
    /// <returns>An enumerable of string representing the referenced code systems.</returns>
    public static IEnumerable<string> cgReferencedCodeSystems(this ValueSet vs) =>
        vs.Expansion?.Contains?.Select(c => c.System).Distinct() ?? vs.Compose?.Include?.Select(i => i.System).Distinct() ?? [];

    /// <summary>Gets the flat list of FhirConcepts from the ValueSet.</summary>
    /// <param name="vs">The ValueSet to act on.</param>
    /// <param name="dc">The device-context.</param>
    /// <returns>An enumerable of FhirConcept representing the flat list of concepts.</returns>
    public static IEnumerable<FhirConcept> cgGetFlatConcepts(this ValueSet vs, DefinitionCollection dc)
    {
        if ((vs.Expansion == null) || (vs.Expansion.Contains.Count == 0))
        {
            yield break;
        }

        foreach (FhirConcept fc in RecurseContains(vs.Expansion.Contains))
        {
            yield return fc;
        }

        yield break;

        /// <summary>
        /// Recursively gets the FhirConcepts from the ContainsComponent.
        /// </summary>
        /// <param name="cc">The ContainsComponent to act on.</param>
        /// <returns>An enumerable of FhirConcept representing the concepts.</returns>
        IEnumerable<FhirConcept> RecurseContains(IEnumerable<ValueSet.ContainsComponent> cc)
        {
            foreach (ValueSet.ContainsComponent c in cc)
            {
                if (!string.IsNullOrEmpty(c.Code))
                {
                    yield return new FhirConcept
                    {
                        System = c.System,
                        Code = c.Code,
                        Display = c.Display,
                        Definition = dc.ConceptDefinition(c.System, c.Code, c.Display),
                        IsAbstract = c.Abstract,
                        IsInactive = c.Inactive,
                        Properties = c.Property.Select(p => new FhirConcept.ConceptProperty { Code = p.Code, Value = p.Value.ToString() ?? string.Empty }).ToArray(),
                    };
                }

                if (c.Contains.Count != 0)
                {
                    foreach (FhirConcept fc in RecurseContains(c.Contains))
                    {
                        yield return fc;
                    }
                }
            }
        }
    }
}
