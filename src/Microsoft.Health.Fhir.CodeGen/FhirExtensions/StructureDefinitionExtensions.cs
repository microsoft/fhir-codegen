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

    ///// <summary>A StructureDefinition extension method that cg artifact status.</summary>
    ///// <param name="sd">The SD to act on.</param>
    ///// <returns>A string.</returns>
    //public static string cgArtifactStatus(this StructureDefinition sd) => sd.Status?.ToString() ?? PublicationStatus.Unknown.ToString();

    ///// <summary>A StructureDefinition extension method that cg standard status.</summary>
    ///// <param name="sd">The SD to act on.</param>
    ///// <returns>A string.</returns>
    //public static string cgStandardStatus(this StructureDefinition sd) => sd.GetExtensionValue<Code>(CommonDefinitions.ExtUrlStandardStatus)?.ToString() ?? string.Empty;

    ///// <summary>A StructureDefinition extension method that cg maturity level.</summary>
    ///// <param name="sd">The SD to act on.</param>
    ///// <returns>An int.</returns>
    //public static int cgMaturityLevel(this StructureDefinition sd) => sd.GetExtensionValue<Integer>(CommonDefinitions.ExtUrlFmm)?.Value ?? 0;

    ///// <summary>A StructureDefinition extension method that cg is experimental.</summary>
    ///// <param name="sd">The SD to act on.</param>
    ///// <returns>True if it succeeds, false if it fails.</returns>
    //public static bool cgIsExperimental(this StructureDefinition sd) => sd.Experimental ?? false;

}
