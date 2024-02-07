// <copyright file="IGenPrimitive.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>


using System.Runtime.CompilerServices;
using Hl7.Fhir.Model;
using Microsoft.Health.Fhir.CodeGen.Utils;
using static Microsoft.Health.Fhir.CodeGenCommon.Extensions.FhirNameConventionExtensions;

namespace Microsoft.Health.Fhir.CodeGen.FhirExtensions;

/// <summary>Interface for a code-gen primitive.</summary>
public interface IGenPrimitive
{
    /// <summary>Gets or sets the identifier.</summary>
    string Id { get; }

    /// <summary>Gets the name.</summary>
    string Name { get; }

    /// <summary>Gets the short.</summary>
    string Short { get; }

    /// <summary>Gets the definition.</summary>
    string Definition { get; }

    /// <summary>Gets the comment.</summary>
    string Comment { get; }

    /// <summary>Gets the base type this primitive extends</summary>
    string SystemType { get; }

    /// <summary>Gets the type of the FHIR.</summary>
    string FhirType { get; }

    /// <summary>Gets the name of the base type.</summary>
    string BaseTypeName { get; }

    /// <summary>Gets the base type canonical.</summary>
    string BaseTypeCanonical { get; }

    /// <summary>Gets the version.</summary>
    string Version { get; }

    /// <summary>Gets URL of the document.</summary>
    string Url { get; }

    /// <summary>Gets the publication status.</summary>
    PublicationStatus ArtifactStatus { get; }

    /// <summary>Gets the standard status.</summary>
    string StandardStatus { get; }

    /// <summary>Gets the maturity level.</summary>
    int MaturityLevel { get; }

    /// <summary>Gets a value indicating whether this object is experimental.</summary>
    bool IsExperimental { get; }

    /// <summary>Gets the validation RegEx.</summary>
    string ValidationRegEx { get; }

    /// <summary>Type for export.</summary>
    /// <param name="convention">            The convention.</param>
    /// <param name="primitiveTypeMap">      The primitive type map.</param>
    /// <param name="concatenatePath">       (Optional) True to concatenate path.</param>
    /// <param name="concatenationDelimiter">(Optional) The concatenation delimiter.</param>
    /// <param name="reservedWords">         (Optional) The reserved words.</param>
    /// <returns>A string.</returns>
    public string TypeForExport(
        NamingConvention convention,
        Dictionary<string, string> primitiveTypeMap,
        bool concatenatePath = false,
        string concatenationDelimiter = "",
        HashSet<string>? reservedWords = null);

}

/// <summary>A primitive type extensions.</summary>
public static class IPrimitiveTypeExtensions
{
    /// <summary>
    /// A StructureDefinition extension method that views a StructureDefinition as a primitive type.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown when one or more required arguments are null.</exception>
    /// <exception cref="ArgumentException">    Thrown when one or more arguments have unsupported or
    ///  illegal values.</exception>
    /// <param name="sd">The SD to act on.</param>
    /// <returns>An IGenPrimitive.</returns>
    public static IGenPrimitive AsPrimitive(this StructureDefinition sd)
    {
        if (sd == null)
        {
            throw new ArgumentNullException(nameof(sd), "StructureDefinition cannot be null");
        }

        if (sd.Kind != StructureDefinition.StructureDefinitionKind.PrimitiveType)
        {
            throw new ArgumentException("StructureDefinition must be a primitive type", nameof(sd));
        }

        return (IGenPrimitive)new GenPrimitive(sd);
    }
}

/// <summary>A code-gen primitive.</summary>
public class GenPrimitive : IGenPrimitive
{
    private StructureDefinition _sd;

    public GenPrimitive(StructureDefinition sd)
    {
        _sd = sd;
    }

    /// <summary>Gets or sets the identifier.</summary>
    public string Id => _sd.Id;

    /// <summary>Gets the name.</summary>
    public string Name => _sd.Name;

    /// <summary>Gets the version.</summary>
    public string Version => _sd.Version;

    /// <summary>Gets URL of the document.</summary>
    public string Url => _sd.Url;

    /// <summary>Gets the short.</summary>
    /// <remarks>right now, differential is generally 'more correct' than snapshot for primitives, see FHIR-37465</remarks>
    public string Short => _sd.Differential.Element.Any() ? _sd.Differential.Element[0].Short : _sd.Description;

    /// <summary>Gets the definition.</summary>
    /// <remarks>right now, differential is generally 'more correct' than snapshot for primitives, see FHIR-37465</remarks>
    public string Definition => _sd.Differential.Element.Any() ? _sd.Differential.Element[0].Definition : _sd.Purpose;

    /// <summary>Gets the comment.</summary>
    /// <remarks>right now, differential is generally 'more correct' than snapshot for primitives, see FHIR-37465</remarks>
    public string Comment => _sd.Differential.Element.Any() ? _sd.Differential.Element[0].Comment : string.Empty;

    /// <summary>Gets the base type this primitive extends.</summary>
    public string SystemType => _sd.Differential.Element.Count > 1
        ? FhirTypeUtils.SystemToFhirType(_sd.Differential.Element[1].Type.FirstOrDefault()?.Code ?? string.Empty)
        : string.Empty;

    /// <summary>Gets the type of the FHIR.</summary>
    public string FhirType => _sd.Differential.Element.Count > 1
        ? _sd.Differential.Element[1].Type.FirstOrDefault()?.GetExtensionValue<FhirUrl>(CommonDefinitions.ExtUrlFhirType).ToString() ?? string.Empty
        : string.Empty;

    /// <summary>Gets the base definition.</summary>
    /// <remarks>For primitives, the base type name is either the FHIR type (reported) or the name of the primitive.</remarks>
    public string BaseTypeName => _sd.Differential.Element.Count > 1
        ? _sd.Differential.Element[1].Type.FirstOrDefault()?.GetExtensionValue<FhirUrl>(CommonDefinitions.ExtUrlFhirType).ToString() ?? string.Empty
        : _sd.Name;

    /// <summary>Gets the base type canonical.</summary>
    public string BaseTypeCanonical => _sd.BaseDefinition;

    /// <summary>Gets the publication status.</summary>
    public PublicationStatus ArtifactStatus => _sd.Status ?? PublicationStatus.Unknown;

    /// <summary>Gets the standard status.</summary>
    public string StandardStatus => _sd.GetExtensionValue<Code>(CommonDefinitions.ExtUrlStandardStatus)?.ToString() ?? string.Empty;

    /// <summary>Gets the maturity level.</summary>
    public int MaturityLevel => _sd.GetExtensionValue<Integer>(CommonDefinitions.ExtUrlFmm)?.Value ?? 0;

    /// <summary>Gets a value indicating whether this object is experimental.</summary>
    public bool IsExperimental => _sd.Experimental ?? false;

    /// <summary>Gets the validation RegEx.</summary>
    public string ValidationRegEx => (_sd.Differential.Element.Count > 1) && _sd.Differential.Element[1].Type.Any()
        ? _sd.Differential.Element[1].Type.First().GetExtensionValue<FhirString>(CommonDefinitions.ExtUrlSdRegex)?.ToString() ?? string.Empty
        : string.Empty;

    /// <summary>Type for export.</summary>
    /// <param name="convention">            The convention.</param>
    /// <param name="primitiveTypeMap">      The primitive type map.</param>
    /// <param name="concatenatePath">       (Optional) True to concatenate path.</param>
    /// <param name="concatenationDelimiter">(Optional) The concatenation delimiter.</param>
    /// <param name="reservedWords">         (Optional) The reserved words.</param>
    /// <returns>A string.</returns>
    public string TypeForExport(
        NamingConvention convention,
        Dictionary<string, string> primitiveTypeMap,
        bool concatenatePath = false,
        string concatenationDelimiter = "",
        HashSet<string>? reservedWords = null)
    {
        string baseType = BaseTypeName;

        if (primitiveTypeMap.TryGetValue(baseType, out string? val) && (!string.IsNullOrEmpty(val)))
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
