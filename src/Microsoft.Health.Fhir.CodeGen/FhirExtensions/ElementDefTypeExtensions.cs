// <copyright file="ElementDefTypeExtensions.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>


using Hl7.Fhir.Model;
using Microsoft.Health.Fhir.CodeGen.Utils;

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
            return tr.Code.Substring(lastSlash + 1);
        }
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
    public static string cgFiveWs(this ElementDefinition ed) => ed.Mapping.Where(m => m.Identity.Equals("w5")).FirstOrDefault()?.Map ?? string.Empty;
}
