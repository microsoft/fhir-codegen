// <copyright file="StructureDefinitionGenCommon.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>


using Hl7.Fhir.Model;
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
                        else if (!sd.Type.Equals(sd.Id))
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
                        else if (!sd.Type.Equals(sd.Id))
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
    public static int cgMaturityLevel(this StructureDefinition sd) => sd.GetExtensionValue<Integer>(CommonDefinitions.ExtUrlFmm)?.Value ?? 0;

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

    /// <summary>Gets the information about mappings defined on this artifact.</summary>
    /// <param name="sd">The SD to act on.</param>
    /// <returns>An IReadOnlyDictionary&lt;string,StructureDefinition.MappingComponent&gt;</returns>
    public static IReadOnlyDictionary<string, StructureDefinition.MappingComponent> cgDefinedMappings(this StructureDefinition sd) => sd.Mapping.ToDictionary(m => m.Identity, m => m);

    /// <summary>Get the constraints defined by this structure.</summary>
    /// <param name="sd">The SD to act on.</param>
    /// <returns>
    /// An enumerator that allows foreach to be used to process cg constraints in this collection.
    /// </returns>
    public static IEnumerable<ElementDefinition.ConstraintComponent> cgConstraints(this StructureDefinition sd)
    {
        if (sd.Snapshot.Element.Any())
        {
            return sd.Snapshot.Element.SelectMany(e => e.Constraint)
                .Where(e => e.Source.Equals(sd.Url, StringComparison.Ordinal))
                .GroupBy(e => e.Key)
                .Select(e => e.First())
                .OrderBy(e => e.Key);
        }

        return sd.Differential.Element.SelectMany(e => e.Constraint)
            .Where(e => e.Source.Equals(sd.Url, StringComparison.Ordinal))
            .GroupBy(e => e.Key)
            .Select(e => e.First())
            .OrderBy(e => e.Key);
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
                    return sd.Type ?? sd.Name ?? string.Empty;
                }
        }
    }

    /// <summary>A StructureDefinition extension method that cg root element.</summary>
    /// <param name="sd">The SD to act on.</param>
    /// <returns>An ElementDefinition?</returns>
    public static ElementDefinition? cgRootElement(this StructureDefinition sd)
    {
        if (sd.Snapshot.Any())
        {
            return sd.Snapshot.Element.FirstOrDefault();
        }

        if (sd.Differential.Element.Any())
        {
            if (sd.Differential.Element.First().ElementId.Equals(sd.Id))
            {
                // first element is the root
                return sd.Differential.Element.First();
            }
        }

        return null;
    }
}
