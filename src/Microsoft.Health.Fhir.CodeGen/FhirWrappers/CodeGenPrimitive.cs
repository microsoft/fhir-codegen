// <copyright file="IGenPrimitive.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>


using Hl7.Fhir.Model;
using Microsoft.Health.Fhir.CodeGen.Utils;
using Microsoft.Health.Fhir.CodeGenCommon.Extensions;
using Microsoft.Health.Fhir.CodeGenCommon.Models;

namespace Microsoft.Health.Fhir.CodeGen.FhirWrappers;

/// <summary>A code generate primitive.</summary>
public class CodeGenPrimitive : StructureDefinition
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CodeGenPrimitive"/> class.
    /// </summary>
    /// <param name="sd">The SD.</param>
    public CodeGenPrimitive(StructureDefinition sd)
    {
        // copy the contents
        sd.CopyTo(this);
    }

    /// <summary>Gets the artifact class.</summary>
    public FhirArtifactClassEnum ArtifactClass => FhirArtifactClassEnum.PrimitiveType;

    /// <summary>A StructureDefinition extension method that cg artifact status.</summary>
    /// <param name="sd">The SD to act on.</param>
    /// <returns>A string.</returns>
    public string ArtifactStatus => Status?.ToString() ?? PublicationStatus.Unknown.ToString();

    /// <summary>A StructureDefinition extension method that cg standard status.</summary>
    /// <param name="sd">The SD to act on.</param>
    /// <returns>A string.</returns>
    public string StandardStatus => this.GetExtensionValue<Code>(CommonDefinitions.ExtUrlStandardStatus)?.ToString() ?? string.Empty;

    /// <summary>A StructureDefinition extension method that cg maturity level.</summary>
    /// <param name="sd">The SD to act on.</param>
    /// <returns>An int.</returns>
    public int MaturityLevel => this.GetExtensionValue<Integer>(CommonDefinitions.ExtUrlFmm)?.Value ?? 0;

    /// <summary>A StructureDefinition extension method that cg is experimental.</summary>
    /// <param name="sd">The SD to act on.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public bool IsExperimental => Experimental ?? false;

    /// <summary>
    /// Get the Short definition of a Primitive datatype (may differ from Description).
    /// </summary>
    public string Short => Differential.Element.Any() ? Differential.Element[0].Short : Description;

    /// <summary>Get the Definition of a primitive datatype (may differ from Purpose).</summary>
    public string Definition => Differential.Element.Any() ? Differential.Element[0].Definition : Purpose;

    /// <summary>Get the comment for a primitive datatype.</summary>
    public string Comment => Differential.Element.Any() ? Differential.Element[0].Comment : string.Empty;

    /// <summary>Get the system type for a primitive datatype.</summary>
    public string SystemType => Differential.Element.Count > 1
        ? FhirTypeUtils.SystemToFhirType(Differential.Element[1].Type.FirstOrDefault()?.Code ?? string.Empty)
        : string.Empty;

    /// <summary>Get the FHIR type for a primitive datatype.</summary>
    public string FhirType => Differential.Element.Count > 1
        ? Differential.Element[1].Type.FirstOrDefault()?.GetExtensionValue<FhirUrl>(CommonDefinitions.ExtUrlFhirType).ToString() ?? string.Empty
        : string.Empty;

    /// <summary>Get the base type name for a primitive datatype.</summary>
    public string BaseTypeName => Differential.Element.Count > 1
        ? Differential.Element[1].Type.FirstOrDefault()?.GetExtensionValue<FhirUrl>(CommonDefinitions.ExtUrlFhirType).ToString() ?? string.Empty
        : Name;

    /// <summary>Get the validation regex string for a primitive datatype.</summary>
    public string ValidationRegEx => Differential.Element.Count > 1 && Differential.Element[1].Type.Any()
        ? Differential.Element[1].Type.First().GetExtensionValue<FhirString>(CommonDefinitions.ExtUrlSdRegex)?.ToString()
            ?? Differential.Element[1].Type.First().GetExtensionValue<FhirString>(CommonDefinitions.ExtUrlSdRegex2)?.ToString()
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
    public string TypeForExport(
        FhirNameConventionExtensions.NamingConvention convention,
        Dictionary<string, string> primitiveTypeMap,
        bool concatenatePath = false,
        string concatenationDelimiter = "",
        HashSet<string>? reservedWords = null)
    {
        string baseType = BaseTypeName;

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
            Id,
            convention,
            concatenatePath,
            concatenationDelimiter,
            reservedWords);

        return type;
    }
}
