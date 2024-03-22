// <copyright file="StructureDefinitionGenCommon.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>


using System.Diagnostics.CodeAnalysis;
using Hl7.Fhir.Model;
using Hl7.Fhir.Specification.Navigation;
using Microsoft.Health.Fhir.CodeGen.Models;
using Microsoft.Health.Fhir.CodeGenCommon.FhirExtensions;
using Microsoft.Health.Fhir.CodeGenCommon.Models;

namespace Microsoft.Health.Fhir.CodeGen.FhirExtensions;

public static class StructureDefinitionExtensions
{
    /// <summary>A StructureDefinition extension method that cg artifact class.</summary>
    /// <param name="sd">The SD to act on.</param>
    /// <returns>A FhirArtifactClassEnum.</returns>
    public static FhirArtifactClassEnum cgArtifactClass(this StructureDefinition sd)
    {
        // determine the type of structure definition
        switch (sd.Kind)
        {
            case StructureDefinition.StructureDefinitionKind.PrimitiveType:
                {
                    return FhirArtifactClassEnum.PrimitiveType;
                }

            case StructureDefinition.StructureDefinitionKind.Logical:
                {
                    return FhirArtifactClassEnum.LogicalModel;
                }

            case StructureDefinition.StructureDefinitionKind.ComplexType:
                {
                    // determine type of definition
                    if (sd.Derivation == StructureDefinition.TypeDerivationRule.Constraint)
                    {
                        if (sd.Type == "Extension")
                        {
                            return FhirArtifactClassEnum.Extension;
                        }
                        // check for DSTU2 Quantity
                        else if ((sd.Version == "1.0.2") && (sd.Type == "Quantity"))
                        {
                            return FhirArtifactClassEnum.ComplexType;
                        }
                        else if (sd.Type != sd.Id)
                        {
                            return FhirArtifactClassEnum.Profile;
                        }
                    }

                    return FhirArtifactClassEnum.ComplexType;
                }

            case StructureDefinition.StructureDefinitionKind.Resource:
                {
                    // determine type of definition
                    if (sd.Derivation == StructureDefinition.TypeDerivationRule.Constraint)
                    {
                        if (sd.Type == "Extension")
                        {
                            return FhirArtifactClassEnum.Extension;
                        }
                        else if (sd.Type != sd.Id)
                        {
                            return FhirArtifactClassEnum.Profile;
                        }
                    }

                    return FhirArtifactClassEnum.Resource;
                }
        }

        return FhirArtifactClassEnum.Unknown;
    }

    /// <summary>Gets the standards status of this definition (e.g., trial-use, normative).</summary>
    /// <param name="sd">The SD to act on.</param>
    /// <returns>A string.</returns>
    public static string cgStandardStatus(this StructureDefinition sd) => sd.GetExtensionValue<Code>(CommonDefinitions.ExtUrlStandardStatus)?.ToString() ?? string.Empty;

    /// <summary>Gets the FHIR Maturity Model (FMM) level of this definition, or 0 if not specified.</summary>
    /// <param name="sd">The SD to act on.</param>
    /// <returns>An int.</returns>
    public static int? cgMaturityLevel(this StructureDefinition sd) => sd.GetExtensionValue<Integer>(CommonDefinitions.ExtUrlFmm)?.Value;

    /// <summary>Gets a flag indicating if this definition is experimental.</summary>
    /// <param name="sd">The SD to act on.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public static bool cgIsExperimental(this StructureDefinition sd) => sd.Experimental ?? false;

    /// <summary>Gets the Work Group responsible for this definition.</summary>
    /// <param name="sd">The SD to act on.</param>
    /// <returns>A string.</returns>
    public static string cgWorkGroup(this StructureDefinition sd) => sd.GetExtensionValue<FhirString>(CommonDefinitions.ExtUrlWorkGroup)?.Value ?? string.Empty;

    /// <summary>Gets the FHIR category this defintion belongs to (e.g., Foundation.Other, Specialized.Evidence-Based Medicine).</summary>
    /// <param name="sd">The SD to act on.</param>
    /// <returns>A string.</returns>
    public static string cgDefinitionCategory(this StructureDefinition sd) => sd.GetExtensionValue<FhirString>(CommonDefinitions.ExtUrlCategory)?.Value ?? string.Empty;

    /// <summary>Gets the base type name for the given ElementDefinition.</summary>
    /// <param name="sd">     The ElementDefinition.</param>
    /// <param name="typeMap">(Optional) The dictionary containing type mappings.</param>
    /// <returns>The base type name.</returns>
    public static string cgBaseTypeName(this StructureDefinition sd, DefinitionCollection dc, Dictionary<string, string>? typeMap = null)
    {
        string value;
        string? mapped;

        if (!string.IsNullOrEmpty(sd.BaseDefinition))
        {
            value = sd.BaseDefinition.Split('/').Last();
            return (typeMap?.TryGetValue(value, out mapped) ?? false)
                ? mapped : value;
        }

        if (!string.IsNullOrEmpty(sd.Type))
        {
            value = sd.Type;
            return (typeMap?.TryGetValue(value, out mapped) ?? false)
                ? mapped : value;
        }

        // if we have no type on the structure, we need to look at the first element
        return sd.cgRootElement()?.cgBaseTypeName(dc, false, typeMap) ?? string.Empty;
    }

    /// <summary>
    /// Enumerates property elements in this structure - skips the root and slices.
    /// </summary>
    /// <param name="sd">             The SD to act on.</param>
    /// <param name="forBackbonePath">(Optional) Full pathname of for backbone file.</param>
    /// <param name="topLevelOnly">   (Optional) True to return only top level elements.</param>
    /// <param name="includeRoot">    (Optional) True to include, false to exclude the root.</param>
    /// <returns>
    /// An enumerator that allows foreach to be used to process cg elements in this collection.
    /// </returns>
    public static IEnumerable<ElementDefinition> cgElements(
        this StructureDefinition sd,
        string forBackbonePath = "",
        bool topLevelOnly = false,
        bool includeRoot = true)
    {
        IEnumerable<ElementDefinition> source;

        int dotCount = string.IsNullOrEmpty(forBackbonePath) ? 0 : forBackbonePath.Count(c => c == '.');

        // filter based on the backbone path we want
        if (string.IsNullOrEmpty(forBackbonePath))
        {
            // get the correct list of elements (snapshot or differential)
            source = (sd.Snapshot?.Element.Any() ?? false)
                ? sd.Snapshot.Element.Skip(includeRoot ? 0 : 1)
                : sd.Differential.Element.Skip(includeRoot ? 0 : 1);
        }
        else
        {
            // we want child elements of the requested path, so append an additional dot
            source = (sd.Snapshot?.Element.Any() ?? false)
                ? sd.Snapshot.Element.Where(e => e.Path.StartsWith(forBackbonePath + ".", StringComparison.Ordinal))
                : sd.Differential.Element.Where(e => e.Path.StartsWith(forBackbonePath + ".", StringComparison.Ordinal));

            // this will filter out the requested path itself, so check if we need the root
            if (includeRoot)
            {
                yield return (sd.Snapshot?.Element.Any() ?? false)
                    ? sd.Snapshot.Element.First(e => e.Path == forBackbonePath)
                    : sd.Differential.Element.First(e => e.Path == forBackbonePath);
            }
        }

        // traverse our filtered elements
        foreach (ElementDefinition e in source)
        {
            // skip slices and their children
            if (e.ElementId.Contains(':'))
            {
                continue;
            }

            // if top level only, we need exactly one dot more than the forBackbonePath
            if (topLevelOnly)
            {
                int count = e.Path.Count(c => c == '.');
                if (count != dotCount + 1)
                {
                    continue;
                }
            }

            yield return e;
        }
    }

    /// <summary>
    /// A StructureDefinition extension method that attempts to get element by path.
    /// </summary>
    /// <param name="sd">     The SD to act on.</param>
    /// <param name="path">   Full pathname of the file.</param>
    /// <param name="element">[out] The element.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public static bool cgTryGetElementByPath(this StructureDefinition sd, string path, [NotNullWhen(true)] out ElementDefinition? element)
    {
        if (sd.Snapshot?.Element.Any() ?? false)
        {
            element = sd.Snapshot.Element.FirstOrDefault(e => e.Path == path);
            return element != null;
        }

        if (sd.Differential?.Element.Any() ?? false)
        {
            element = sd.Differential.Element.FirstOrDefault(e => e.Path == path);
            return element != null;
        }

        element = null;
        return false;
    }

    /// <summary>
    /// A StructureDefinition extension method that attempts to get element by identifier.
    /// </summary>
    /// <param name="sd">     The SD to act on.</param>
    /// <param name="id">     The identifier.</param>
    /// <param name="element">[out] The element.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public static bool cgTryGetElementById(this StructureDefinition sd, string id, [NotNullWhen(true)] out ElementDefinition? element)
    {
        if (sd.Snapshot?.Element.Any() ?? false)
        {
            element = sd.Snapshot.Element.FirstOrDefault(e => e.ElementId == id);
            return element != null;
        }

        if (sd.Differential?.Element.Any() ?? false)
        {
            element = sd.Differential.Element.FirstOrDefault(e => e.ElementId == id);
            return element != null;
        }

        element = null;
        return false;
    }

    /// <summary>Gets the information about mappings defined on this artifact.</summary>
    /// <param name="sd">The SD to act on.</param>
    /// <returns>An IReadOnlyDictionary&lt;string,StructureDefinition.MappingComponent&gt;</returns>
    public static IReadOnlyDictionary<string, StructureDefinition.MappingComponent> cgDefinedMappings(this StructureDefinition sd) => sd.Mapping.ToDictionary(m => m.Identity, m => m);

    /// <summary>Get the constraints defined by this structure.</summary>
    /// <param name="sd">              The SD to act on.</param>
    /// <param name="includeInherited">(Optional) True to include, false to exclude the inherited.</param>
    /// <returns>
    /// An enumerator that allows foreach to be used to process cg constraints in this collection.
    /// </returns>
    public static IEnumerable<ElementDefinition.ConstraintComponent> cgConstraints(this StructureDefinition sd, bool includeInherited = false)
    {
        if (includeInherited)
        {
            if (sd.Snapshot?.Element.Any() ?? false)
            {
                return sd.Snapshot.Element.SelectMany(e => e.Constraint)
                    .GroupBy(e => e.Key)
                    .Select(e => e.First())
                    .OrderBy(e => e.Key, NaturalComparer.Instance);
            }

            return sd.Differential.Element.SelectMany(e => e.Constraint)
                .GroupBy(e => e.Key)
                .Select(e => e.First())
                .OrderBy(e => e.Key, NaturalComparer.Instance);
        }

        if (sd.Snapshot?.Element.Any() ?? false)
        {
            return sd.Snapshot.Element.SelectMany(e => e.Constraint)
                .Where(e => e.Source == sd.Url)
                .GroupBy(e => e.Key)
                .Select(e => e.First())
                .OrderBy(e => e.Key, NaturalComparer.Instance);
        }

        return sd.Differential.Element.SelectMany(e => e.Constraint)
            .Where(e => e.Source == sd.Url)
            .GroupBy(e => e.Key)
            .Select(e => e.First())
            .OrderBy(e => e.Key, NaturalComparer.Instance);
    }

    /// <summary>Get the Base Type Name for this structure.</summary>
    /// <param name="sd">The SD to act on.</param>
    /// <returns>A string.</returns>
    public static string cgBaseTypeName(this StructureDefinition sd)
    {
        switch (sd.Kind)
        {
            case StructureDefinition.StructureDefinitionKind.PrimitiveType:
                {
                    return sd.Differential.Element.Count > 1
                        ? sd.Differential.Element[1].Type.FirstOrDefault()?.GetExtensionValue<FhirUrl>(CommonDefinitions.ExtUrlFhirType).ToString() ?? string.Empty
                        : sd.Name;
                }

            case StructureDefinition.StructureDefinitionKind.Logical:
            case StructureDefinition.StructureDefinitionKind.ComplexType:
            case StructureDefinition.StructureDefinitionKind.Resource:
            default:
                {
                    if (!string.IsNullOrEmpty(sd.BaseDefinition))
                    {
                        return sd.BaseDefinition.Split('/').Last();
                    }

                    return sd.Type ?? sd.Name ?? string.Empty;
                }
        }
    }

    /// <summary>A StructureDefinition extension method that cg root element.</summary>
    /// <param name="sd">The SD to act on.</param>
    /// <returns>An ElementDefinition?</returns>
    public static ElementDefinition? cgRootElement(this StructureDefinition sd)
    {
        if (sd.Snapshot?.Element.Any() ?? false)
        {
            return sd.Snapshot.Element.FirstOrDefault();
        }

        if (sd.Differential?.Element.Any() ?? false)
        {
            if (sd.Differential.Element.First().ElementId == sd.Id)
            {
                // first element is the root
                return sd.Differential.Element.First();
            }
        }

        return null;
    }
}
