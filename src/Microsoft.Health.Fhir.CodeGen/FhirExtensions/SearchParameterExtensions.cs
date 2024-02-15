// <copyright file="SearchParameterExtensions.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>


using Hl7.Fhir.Model;

namespace Microsoft.Health.Fhir.CodeGen.FhirExtensions;

public static class SearchParameterExtensions
{
    /// <summary>Gets the standards status of this definition (e.g., trial-use, normative).</summary>
    /// <param name="sd">The SD to act on.</param>
    /// <returns>A string.</returns>
    public static string cgStandardStatus(this SearchParameter sd) => sd.GetExtensionValue<Code>(CommonDefinitions.ExtUrlStandardStatus)?.ToString() ?? string.Empty;

    /// <summary>Gets the FHIR Maturity Model (FMM) level of this definition, or 0 if not specified.</summary>
    /// <param name="sd">The SD to act on.</param>
    /// <returns>An int.</returns>
    public static int? cgMaturityLevel(this SearchParameter sd) => sd.GetExtensionValue<Integer>(CommonDefinitions.ExtUrlFmm)?.Value;

    /// <summary>Gets a flag indicating if this definition is experimental.</summary>
    /// <param name="sd">The SD to act on.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public static bool cgIsExperimental(this SearchParameter sd) => sd.Experimental ?? false;

    ///// <summary>Gets the Work Group responsible for this definition.</summary>
    ///// <param name="sd">The SD to act on.</param>
    ///// <returns>A string.</returns>
    //public static string cgWorkGroup(this StructureDefinition sd) => sd.GetExtensionValue<FhirString>(CommonDefinitions.ExtUrlWorkGroup)?.Value ?? string.Empty;

    ///// <summary>Gets the FHIR category this defintion belongs to (e.g., Foundation.Other, Specialized.Evidence-Based Medicine).</summary>
    ///// <param name="sd">The SD to act on.</param>
    ///// <returns>A string.</returns>
    //public static string cgDefinitionCategory(this StructureDefinition sd) => sd.GetExtensionValue<FhirString>(CommonDefinitions.ExtUrlCategory)?.Value ?? string.Empty;

    /// <summary>Gets whether all components of a composite search parameter resolve in the provided set.</summary>
    /// <param name="sp">                 The sp to act on.</param>
    /// <param name="searchParameterUrls">The search parameter urls.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public static bool cgCompositeResolves(this SearchParameter sp, IEnumerable<string> searchParameterUrls) =>
        sp.Component.All(sp => searchParameterUrls.Contains(sp.Definition));
}
