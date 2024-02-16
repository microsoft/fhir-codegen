// <copyright file="ElementDefConstraintExtensions.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Hl7.Fhir.Model;

namespace Microsoft.Health.Fhir.CodeGen.FhirExtensions;

public static class ElementDefConstraintExtensions
{
    /// <summary>
    /// Gets whether this constraint is flagged as a best practice.
    /// </summary>
    /// <param name="c">A ConstraintComponent to process.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public static bool cgIsBestPractice(this ElementDefinition.ConstraintComponent c) =>
        c.GetExtensionValue<FhirBoolean>(CommonDefinitions.ExtUrlBestPractice)?.Value
        ?? false;

    /// <summary>
    /// Gets the explanation of why this constraint is flagged as a best practice, or an empty string
    /// if not flagged.
    /// </summary>
    /// <param name="c">A ConstraintComponent to process.</param>
    /// <returns>A string.</returns>
    public static string cgBestPracticeDescription(this ElementDefinition.ConstraintComponent c) =>
        c.GetExtensionValue<Markdown>(CommonDefinitions.ExtUrlBestPracticeExplanation)?.ToString()
        ?? c.GetExtensionValue<FhirString>(CommonDefinitions.ExtUrlBestPracticeExplanation)?.ToString()
        ?? string.Empty;

    /// <summary>
    /// Test if this constraint is inherited or newly defined in the current StructureDefinition.
    /// </summary>
    /// <param name="c"> A ConstraintComponent to process.</param>
    /// <param name="sd">The SD.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public static bool cgIsInherited(this ElementDefinition.ConstraintComponent c, StructureDefinition sd) =>
        !(c.Source?.Equals(sd.Url, StringComparison.Ordinal) ?? true);
}
