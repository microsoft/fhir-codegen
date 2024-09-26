// <copyright file="StructureDefPrimitive.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>


using Hl7.Fhir.Model;
using Microsoft.Health.Fhir.CodeGenCommon.Utils;
using Microsoft.Health.Fhir.CodeGenCommon.Extensions;
using Microsoft.Health.Fhir.CodeGenCommon.FhirExtensions;
using System.Xml.Linq;

namespace Microsoft.Health.Fhir.CodeGen.FhirExtensions;

public static class StructureDefPrimitive
{
    /// <summary>
    /// Get the Short definition of a Primitive datatype (may differ from Description).
    /// </summary>
    public static string cgpShort(this StructureDefinition sd) => (sd.Snapshot != null) && (sd.Snapshot.Element.Count != 0) 
        ? sd.Snapshot.Element[0].Short 
        : sd.Differential.Element.Count != 0 ? sd.Differential.Element[0].Short : sd.Description;

    /// <summary>Get the Definition of a primitive datatype (may differ from Purpose).</summary>
    public static string cgpDefinition(this StructureDefinition sd) => (sd.Snapshot != null) && (sd.Snapshot.Element.Count != 0) 
        ? sd.Snapshot.Element[0].Definition 
        : sd.Differential.Element.Count != 0 ? sd.Differential.Element[0].Definition : sd.Purpose;

    /// <summary>Get the comment for a primitive datatype.</summary>
    public static string cgpComment(this StructureDefinition sd) => (sd.Snapshot != null) && (sd.Snapshot.Element.Count != 0) 
        ? sd.Snapshot.Element[0].Comment 
        : sd.Differential.Element.Count != 0 ? sd.Differential.Element[0].Comment : string.Empty;

    /// <summary>Get the system type for a primitive datatype.</summary>
    public static string cgpSystemType(this StructureDefinition sd) => sd.Differential.Element.Count > 1
        ? FhirTypeUtils.SystemToFhirType(sd.Differential.Element[1].Type.FirstOrDefault()?.Code ?? string.Empty)
        : string.Empty;

    /// <summary>Get the FHIR type for a primitive datatype.</summary>
    public static string cgpFhirType(this StructureDefinition sd) => sd.Differential.Element.Count > 1
        ? sd.Differential.Element[1].Type.FirstOrDefault()?.GetExtensionValue<FhirUrl>(CommonDefinitions.ExtUrlFhirType).ToString() ?? string.Empty
        : string.Empty;

    /// <summary>Get the base type name for a primitive datatype.</summary>
    public static string cgpBaseTypeName(this StructureDefinition sd) => sd.Differential.Element.Count > 1
        ? sd.Differential.Element[1].Type.FirstOrDefault()?.GetExtensionValue<FhirUrl>(CommonDefinitions.ExtUrlFhirType)?.ToString() ?? string.Empty
        : sd.Name;

    /// <summary>Get the validation regex string for a primitive datatype.</summary>
    public static string cgpValidationRegEx(this StructureDefinition sd) => sd.Differential.Element.Count > 1 && sd.Differential.Element[1].Type.Count != 0
        ? sd.Differential.Element[1].Type.First().GetExtensionValue<FhirString>(CommonDefinitions.ExtUrlSdRegex)?.ToString()
            ?? sd.Differential.Element[1].Type.First().GetExtensionValue<FhirString>(CommonDefinitions.ExtUrlSdRegex2)?.ToString()
            ?? string.Empty
        : string.Empty;

    /// <summary>Get the export type name for a primitive datatype.</summary>
    /// <param name="sd">                    The SD to act on.</param>
    /// <param name="convention">            The convention.</param>
    /// <param name="primitiveTypeMap">      The primitive type map.</param>
    /// <param name="concatenatePath">       (Optional) True to concatenate path.</param>
    /// <param name="concatenationDelimiter">(Optional) The concatenation delimiter.</param>
    /// <param name="reservedWords">         (Optional) The reserved words.</param>
    /// <returns>A string.</returns>
    public static string cgpTypeForExport(
        this StructureDefinition sd,
        FhirNameConventionExtensions.NamingConvention convention,
        Dictionary<string, string> primitiveTypeMap,
        bool concatenatePath = false,
        string concatenationDelimiter = "",
        HashSet<string>? reservedWords = null)
    {
        string baseType = sd.cgpBaseTypeName();

        if (primitiveTypeMap.TryGetValue(baseType, out string? val) && !string.IsNullOrEmpty(val))
        {
            return val;
        }

        // Resources cannot inherit patterns, but they are listed that way today
        // see https://chat.fhir.org/#narrow/stream/179177-conformance/topic/Inheritance.20and.20Cardinality.20Changes
        baseType = baseType switch
        {
            "CanonicalResource" or "MetadataResource" => "DomainResource",
            _ => baseType,
        };

        string type = FhirSanitizationUtils.ToConvention(
            baseType,
            sd.Id,
            convention,
            concatenatePath,
            concatenationDelimiter,
            reservedWords);

        return type;
    }
}
