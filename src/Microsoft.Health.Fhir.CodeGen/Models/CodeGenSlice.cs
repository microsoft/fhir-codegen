// <copyright file="CodeGenSlicing.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>


using Hl7.Fhir.Model;

namespace Microsoft.Health.Fhir.CodeGen.Models;

/// <summary>A code generate slicing.</summary>
public record class CodeGenSlice
{
    /// <summary>Gets or initializes the name of the slice.</summary>
    public required string Name { get; init; }

    /// <summary>Gets or initializes the slice order.</summary>
    public required int SliceOrder { get; init; }

    /// <summary>Gets or initializes the type of the computed.</summary>
    public required ElementDefinition.DiscriminatorType? ComputedType { get; init; }

    /// <summary>Gets or initializes the full pathname of the computed file.</summary>
    public required string ComputedPath { get; init; }

    /// <summary>Gets or initializes the computed value.</summary>
    public required string ComputedValue { get; init; }

    /// <summary>Gets or initializes the fixed element.</summary>
    public required ElementDefinition? FixedElement { get; init; }

    /// <summary>Gets or initializes the pattern element.</summary>
    public required ElementDefinition? PatternElement { get; init; }

    /// <summary>Gets or initializes the profile element.</summary>
    public required ElementDefinition? ProfileElement { get; init; }

}
