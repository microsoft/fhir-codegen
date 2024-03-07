// <copyright file="ValueSetExtensions.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Hl7.Fhir.Model;
using Microsoft.Health.Fhir.CodeGenCommon.FhirExtensions;

namespace Microsoft.Health.Fhir.CodeGen.FhirExtensions;

public static class ValueSetExtensions
{
    /// <summary>Gets the standards status of this definition (e.g., trial-use, normative).</summary>
    /// <param name="vs">The SD to act on.</param>
    /// <returns>A string.</returns>
    public static string cgStandardStatus(this ValueSet vs) => vs.GetExtensionValue<Code>(CommonDefinitions.ExtUrlStandardStatus)?.ToString() ?? string.Empty;

    /// <summary>Gets the FHIR Maturity Model (FMM) level of this definition, or 0 if not specified.</summary>
    /// <param name="vs">The SD to act on.</param>
    /// <returns>An int.</returns>
    public static int? cgMaturityLevel(this ValueSet vs) => vs.GetExtensionValue<Integer>(CommonDefinitions.ExtUrlFmm)?.Value;

    /// <summary>Gets a flag indicating if this definition is experimental.</summary>
    /// <param name="vs">The SD to act on.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public static bool cgIsExperimental(this ValueSet vs) => vs.Experimental ?? false;

    /// <summary>Gets the Work Group responsible for this definition.</summary>
    /// <param name="vs">The SD to act on.</param>
    /// <returns>A string.</returns>
    public static string cgWorkGroup(this ValueSet vs) => vs.GetExtensionValue<FhirString>(CommonDefinitions.ExtUrlWorkGroup)?.Value ?? string.Empty;

    /// <summary>A ValueSet extension method that query if 'vs' is limited expansion.</summary>
    /// <param name="vs">The SD to act on.</param>
    /// <returns>True if limited expansion, false if not.</returns>
    public static bool IsLimitedExpansion(this ValueSet vs)
    {
        if (vs.Expansion.Parameter == null)
        {
            return false;
        }

        ValueSet.ParameterComponent? vspc = vs.Expansion.Parameter.Where(pc => pc.Name.Equals("limitedExpansion", StringComparison.Ordinal)).FirstOrDefault();

        if (vspc == null)
        {
            return false;
        }

        switch (vspc.Value)
        {
            case FhirBoolean fb:
                return (fb.Value == true);
            case FhirString fs:
                return fs.Value.Equals("true", StringComparison.OrdinalIgnoreCase) || fs.Value.Equals("-1", StringComparison.Ordinal);
            case Integer i:
                return i.Value == -1;
            default:
                return false;
        }
    }
}
