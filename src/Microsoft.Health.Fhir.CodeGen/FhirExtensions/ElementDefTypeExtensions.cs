// <copyright file="ElementDefTypeExtensions.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>


using Hl7.Fhir.Model;
using Microsoft.Health.Fhir.CodeGenCommon.Utils;
using Microsoft.Health.Fhir.CodeGenCommon.FhirExtensions;

namespace Microsoft.Health.Fhir.CodeGen.FhirExtensions;

/// <summary>An element definition type extensions.</summary>
public static class ElementDefTypeExtensions
{
    /// <summary>An type name.</summary>
    /// <param name="tr">The TypeRefComponent.</param>
    /// <returns>A string.</returns>
    public static string cgName(this ElementDefinition.TypeRefComponent tr)
    {
        string name = tr.GetExtensionValue<FhirUrl>(CommonDefinitions.ExtUrlFhirType)?.ToString()
            ?? string.Empty;

        if (!string.IsNullOrEmpty(name))
        {
            return name;
        }

        if (FhirTypeUtils.IsFhirPathType(tr.Code, out name))
        {
            return name;
        }

        if (FhirTypeUtils.IsXmlType(tr.Code, out name))
        {
            return name;
        }

        // check for URL
        int lastSlash = tr.Code.LastIndexOf('/');
        if (lastSlash == -1)
        {
            return tr.Code;
        }
        else
        {
            return tr.Code[(lastSlash + 1)..];
        }
    }

    public static ElementDefinition.TypeRefComponent cgAsR5(this ElementDefinition.TypeRefComponent tr)
    {
        // check for already having a primitive type
        if (!tr.Code.Contains('.'))
        {
            return tr;
        }

        string typeExt = tr.GetExtensionValue<FhirUrl>(CommonDefinitions.ExtUrlFhirType)?.ToString()
            ?? tr.GetExtensionValue<FhirString>(CommonDefinitions.ExtUrlFhirType)?.ToString()
            ?? string.Empty;

        string typeKey = tr.Code + "#" + typeExt;

        switch (typeKey)
        {
            // R4 Element.id
            // R4 Resource.id
            // R5 Element.id
            case "http://hl7.org/fhirpath/System.String#string":
            // R4B Element.id
            // R4B Resource.id
            // R5 Resource.id
            case "http://hl7.org/fhirpath/System.String#id":
                return BuildType("id");

            // R4 Extension.url
            // R4B Extension.url
            // R5 Extension.url
            case "http://hl7.org/fhirpath/System.String#uri":
                return BuildType("uri");
        }

        return tr;

        ElementDefinition.TypeRefComponent BuildType(string fhirType) => new ElementDefinition.TypeRefComponent()
        {
            Code = "http://hl7.org/fhirpath/System.String",
            Extension = [new Extension
                {
                    Url = CommonDefinitions.ExtUrlFhirType,
                    Value = new FhirUrl(fhirType),
                }],
        };
    }

    public static ElementDefinition.TypeRefComponent cgExtCompatible(this ElementDefinition.TypeRefComponent tr)
    {
        // check for already having a primitive type
        if (!tr.Code.Contains('.'))
        {
            return tr;
        }

        string typeExt = tr.GetExtensionValue<FhirUrl>(CommonDefinitions.ExtUrlFhirType)?.ToString()
            ?? tr.GetExtensionValue<FhirString>(CommonDefinitions.ExtUrlFhirType)?.ToString()
            ?? string.Empty;

        string typeKey = tr.Code + "#" + typeExt;

        switch (typeKey)
        {
            // R4 Element.id
            // R4 Resource.id
            // R5 Element.id
            case "http://hl7.org/fhirpath/System.String#string":
            // R4B Element.id
            // R4B Resource.id
            // R5 Resource.id
            case "http://hl7.org/fhirpath/System.String#id":
                return BuildType("id");

            // R4 Extension.url
            // R4B Extension.url
            // R5 Extension.url
            case "http://hl7.org/fhirpath/System.String#uri":
                return BuildType("uri");
        }

        return tr;

        ElementDefinition.TypeRefComponent BuildType(string fhirType) => new ElementDefinition.TypeRefComponent()
        {
            Code = fhirType,
        };
    }

    /// <summary>Gets the validation regex specific to this type.</summary>
    /// <param name="tr">The TypeRefComponent.</param>
    /// <returns>A string.</returns>
    public static string cgRegex(this ElementDefinition.TypeRefComponent tr) =>
        tr.GetExtensionValue<FhirString>(CommonDefinitions.ExtUrlSdRegex)?.ToString()
        ?? tr.GetExtensionValue<FhirString>(CommonDefinitions.ExtUrlSdRegex2)?.ToString()
        ?? string.Empty;

    /// <summary>Gets a value indicating whether this object is simple type.</summary>
    /// <param name="tr">The TypeRefComponent.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public static bool cgIsSimpleType(this ElementDefinition.TypeRefComponent tr) =>
        !string.IsNullOrEmpty(
            tr.GetExtensionValue<FhirUrl>(CommonDefinitions.ExtUrlFhirType)?.ToString()
            ?? tr.GetExtensionValue<FhirString>(CommonDefinitions.ExtUrlFhirType)?.ToString()
            ?? string.Empty);

    /// <summary>Gets the type profiles (StructureDefinition or IG) - one must apply.</summary>
    /// <remarks>
    /// Identifies a profile structure or implementation Guide that applies to the datatype this
    /// element refers to.If any profiles are specified, then the content must conform to at least
    /// one of them.The URL can be a local reference - to a contained StructureDefinition, or a
    /// reference to another StructureDefinition or Implementation Guide by a canonical URL.When an
    /// implementation guide is specified, the type SHALL conform to at least one profile defined in
    /// the implementation guide.
    /// 
    /// 
    ///    It is possible to profile backbone element(e.g.part of a resource), using the
    ///    http://hl7.org/fhir/StructureDefinition/elementdefinition-profile-element extension.
    /// </remarks>
    public static IReadOnlyDictionary<string, string> cgTypeProfiles(this ElementDefinition.TypeRefComponent tr) => tr.Profile.Distinct().ToDictionary(p => new Uri(p).Segments.Last(), p => p);

    /// <summary>
    /// Gets the target profiles (StructureDefinition or IG) on the Reference/canonical target - one
    /// must apply.
    /// </summary>
    /// <remarks>
    /// Used when the type is "Reference" or "canonical", and identifies a profile structure or
    /// implementation Guide that applies to the target of the reference this element refers to. If
    /// any profiles are specified, then the content must conform to at least one of them. The URL
    /// can be a local reference - to a contained StructureDefinition, or a reference to another
    /// StructureDefinition or Implementation Guide by a canonical URL. When an implementation guide
    /// is specified, the target resource SHALL conform to at least one profile defined in the
    /// implementation guide.
    /// </remarks>
    public static IReadOnlyDictionary<string, string> cgTargetProfiles(this ElementDefinition.TypeRefComponent tr) => tr.TargetProfile.Distinct().ToDictionary(p => new Uri(p).Segments.Last(), p => p);

    /// <summary>Gets the mappings defined for this element.</summary>
    /// <param name="ed">The ed to act on.</param>
    /// <returns>An IReadOnlyDictionary&lt;string,string&gt;</returns>
    public static IReadOnlyDictionary<string, string> cgMappings(this ElementDefinition ed) => ed.Mapping.ToDictionary(m => m.Identity, m => m.Map);

    /// <summary>Gets the 5Ws mappings for this element, or an empty string if none are defined.</summary>
    /// <param name="ed">The ed to act on.</param>
    /// <returns>A string.</returns>
    public static string cgFiveWs(this ElementDefinition ed)
    {
        string source = ed.Mapping.Where(m => m.Identity == "w5").FirstOrDefault()?.Map ?? string.Empty;

        if (string.IsNullOrEmpty(source))
        {
            return string.Empty;
        }

        // need to change "FiveWs.subject[x]" to "FiveWs.subject", but beware of duplicates
#if NET8_0_OR_GREATER
        HashSet<string> hash = [.. source.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)];
#else
        HashSet<string> hash = [.. source.Split(',').Where(v => !string.IsNullOrEmpty(v)).Select(v => v.Trim())];
#endif
        if (hash.Contains("FiveWs.subject[x]"))
        {
            hash.Remove("FiveWs.subject[x]");
            hash.Add("FiveWs.subject");
        }

        return string.Join(",", hash);
    }
}
