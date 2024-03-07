// <copyright file="OperationDefinitionExtensions.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Hl7.Fhir.Model;
using Microsoft.Health.Fhir.CodeGenCommon.FhirExtensions;
using Microsoft.Health.Fhir.CodeGenCommon.Models;

namespace Microsoft.Health.Fhir.CodeGen.FhirExtensions;

public static class OperationDefinitionExtensions
{
    /// <summary>An OperationDefinition extension method that cg artifact class.</summary>
    /// <param name="op">The op to act on.</param>
    /// <returns>A FhirArtifactClassEnum.</returns>
    public static FhirArtifactClassEnum cgArtifactClass(this OperationDefinition op) => FhirArtifactClassEnum.Operation;

    /// <summary>Gets the standards status of this definition (e.g., trial-use, normative).</summary>
    /// <param name="op">The op to act on.</param>
    /// <returns>A string.</returns>
    public static string cgStandardStatus(this OperationDefinition op) => op.GetExtensionValue<Code>(CommonDefinitions.ExtUrlStandardStatus)?.ToString() ?? string.Empty;

    /// <summary>
    /// Gets the FHIR Maturity Model (FMM) level of this definition, or 0 if not specified.
    /// </summary>
    /// <param name="op">The op to act on.</param>
    /// <returns>An int.</returns>
    public static int cgMaturityLevel(this OperationDefinition op) => op.GetExtensionValue<Integer>(CommonDefinitions.ExtUrlFmm)?.Value ?? 0;

    /// <summary>Gets a flag indicating if this definition is experimental.</summary>
    /// <param name="sd">The SD to act on.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public static bool cgIsExperimental(this OperationDefinition sd) => sd.Experimental ?? false;

    /// <summary>Gets the Work Group responsible for this definition.</summary>
    /// <param name="op">The op to act on.</param>
    /// <returns>A string.</returns>
    public static string cgWorkGroup(this OperationDefinition op) => op.GetExtensionValue<FhirString>(CommonDefinitions.ExtUrlWorkGroup)?.Value ?? string.Empty;

}
