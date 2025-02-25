// <copyright file="ValueSetExtensions.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.Runtime.CompilerServices;
using Hl7.Fhir.Model;
using Microsoft.Health.Fhir.CodeGen.Models;
using Microsoft.Health.Fhir.CodeGen.Utils;
using Microsoft.Health.Fhir.CodeGenCommon.FhirExtensions;
using Microsoft.Health.Fhir.CodeGenCommon.Utils;
using CSDC = Hl7.Fhir.Model.CodeSystem.ConceptDefinitionComponent;

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
    /// Gets the versioned URL of the ValueSet.
    /// </summary>
    /// <param name="vs">The ValueSet to act on.</param>
    /// <returns>A string representing the versioned URL.</returns>
    public static string cgVersionedUrl(this ValueSet vs) => vs.Url + "|" + vs.Version;

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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        vs.Expansion?.Contains.Count > 0
        ? vs.Expansion.Contains.Select(c => c.System).Distinct()
        : vs.Compose?.Include?.Select(i => i.System).Distinct()
        ?? [];

    /// <summary>
    /// Determines whether the DefinitionCollection has a required binding for the specified ValueSet URLs.
    /// </summary>
    /// <param name="dc">The DefinitionCollection to act on.</param>
    /// <param name="versionedUrl">The versioned URL of the ValueSet.</param>
    /// <param name="unversionedUrl">The unversioned URL of the ValueSet.</param>
    /// <returns>True if the DefinitionCollection has a required binding, false otherwise.</returns>
    public static bool cgHasRequiredBinding(
        this DefinitionCollection dc,
        string versionedUrl,
        string unversionedUrl)
    {
        IEnumerable<StructureElementCollection> coreBindingsUnversioned = dc.CoreBindingsForVs(unversionedUrl);
        if (dc.StrongestBinding(coreBindingsUnversioned) == BindingStrength.Required)
        {
            return true;
        }

        IEnumerable<StructureElementCollection> coreBindingsVersioned = dc.CoreBindingsForVs(versionedUrl);
        if (dc.StrongestBinding(coreBindingsVersioned) == BindingStrength.Required)
        {
            return true;
        }

        return false;
    }

    public static bool? cgHasCode(this ValueSet vs, HashSet<string> codes)
    {
        if ((vs.Expansion == null) || (vs.Expansion.Contains.Count == 0))
        {
            return null;
        }

        return containsCode(vs.Expansion.Contains);

        bool containsCode(IEnumerable<ValueSet.ContainsComponent> cc)
        {
            foreach (ValueSet.ContainsComponent c in cc)
            {
                if (codes.Contains(cgKey(c)))
                {
                    return true;
                }
                if (c.Contains.Count != 0)
                {
                    return containsCode(c.Contains);
                }
            }

            return false;
        }
    }

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

        List<FhirConcept> results = [];

        foreach (ValueSet.ContainsComponent c in recurseContains(dc, vs, vs.Expansion.Contains))
        {
            if (!string.IsNullOrEmpty(c.Code))
            {
                yield return conceptForContainsComponent(c, vs);
            }
        }

        yield break;

        /// <summary>
        /// Recursively gets the FhirConcepts from the ContainsComponent.
        /// </summary>
        /// <param name="cc">The ContainsComponent to act on.</param>
        /// <returns>An enumerable of FhirConcept representing the concepts.</returns>
        IEnumerable<ValueSet.ContainsComponent> recurseContains(
            DefinitionCollection dc,
            ValueSet vs,
            IEnumerable<ValueSet.ContainsComponent> components,
            int depth = 0)
        {
            foreach (ValueSet.ContainsComponent c in components)
            {
                if (c.Contains.Count != 0)
                {
                    foreach (ValueSet.ContainsComponent nested in recurseContains(dc, vs, c.Contains, depth + 1))
                    {
                        yield return nested;
                    }
                }

                yield return c;
            }
        }

        FhirConcept conceptForContainsComponent(ValueSet.ContainsComponent c, ValueSet vs)
        {
            // TODO(ginoc): pulling the version from the VS is not correct, but it works in the cases we are concerned about right now
            return new FhirConcept
            {
                System = c.System,
                Version = c.Version ?? dc.GetCanonicalVersion(c.System) ?? vs.Version,
                Code = c.Code,
                Display = c.Display,
                Definition = dc.ConceptDefinition(c.System, c.Code, c.Display),
                IsAbstract = c.Abstract,
                IsInactive = c.Inactive,
                Properties = c.Property.Select(p => new FhirConcept.ConceptProperty { Code = p.Code, Value = p.Value.ToString() ?? string.Empty }).ToArray(),
            };
        }
    }


    internal static CSDC? FindCode(this IEnumerable<CSDC> concepts, string code)
    {
        return concepts.findCodeByPredicate(c => c.Code == code);
    }

    private static CSDC? findCodeByPredicate(this IEnumerable<CSDC> concepts, Predicate<CSDC> predicate)
    {
        foreach (var concept in concepts)
        {
            var result = concept.findCodeByPredicate(predicate);
            if (result != null) return result;
        }
        return null;
    }

    private static CSDC? findCodeByPredicate(this CSDC concept, Predicate<CSDC> predicate)
    {
        // Direct hit
        if (predicate(concept))
            return concept;

        // Not in this node, but this node may have child nodes to check
        if (concept.Concept?.Any() == true)
            return concept.Concept.findCodeByPredicate(predicate);
        else
            return null;
    }

    internal static List<CSDC> RemoveCode(this ICollection<CSDC> concepts, string code)
    {
        return concepts.getNonMatchingCodes(c => c.Code == code);
    }

    private static List<CSDC> getNonMatchingCodes(this ICollection<CSDC> concepts, Predicate<CSDC> predicate)
    {
        List<CSDC> filtered = concepts.Where(c => !predicate(c)).ToList();

        foreach (CSDC? concept in filtered.Where(concept => concept.Concept?.Count is > 0))
        {
            concept.Concept = concept.Concept.getNonMatchingCodes(predicate).ToList();
        }

        return filtered;
    }

    /// <summary>Gets the flat list of Contains Components from the ValueSet Expansion.</summary>
    /// <param name="vs">          The ValueSet to act on.</param>
    /// <param name="dc">          The device-context.</param>
    /// <param name="includeEmpty">(Optional) True to include, false to exclude the empty.</param>
    /// <returns>An enumerable of FhirConcept representing the flat list of concepts.</returns>
    public static IEnumerable<ValueSet.ContainsComponent> cgGetFlatContains(
        this ValueSet vs,
        bool includeEmpty = false)
    {
        if ((vs.Expansion == null) || (vs.Expansion.Contains.Count == 0))
        {
            yield break;
        }

        foreach (ValueSet.ContainsComponent fc in RecurseContains(vs, vs.Expansion.Contains))
        {
            yield return fc;
        }

        yield break;

        /// <summary>
        /// Recursively gets the FhirConcepts from the ContainsComponent.
        /// </summary>
        /// <param name="cc">The ContainsComponent to act on.</param>
        /// <returns>An enumerable of FhirConcept representing the concepts.</returns>
        IEnumerable<ValueSet.ContainsComponent> RecurseContains(ValueSet vs, IEnumerable<ValueSet.ContainsComponent> cc)
        {
            foreach (ValueSet.ContainsComponent c in cc)
            {
                if (includeEmpty || !string.IsNullOrEmpty(c.Code))
                {
                    yield return c;
                }

                if (c.Contains.Count != 0)
                {
                    foreach (ValueSet.ContainsComponent fc in RecurseContains(vs, c.Contains))
                    {
                        yield return fc;
                    }
                }
            }
        }
    }
}
