// <copyright file="ComponentDefinition.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>


using Hl7.Fhir.Model;

namespace Microsoft.Health.Fhir.CodeGen.Models;

/// <summary>A component definition.</summary>
public record class ComponentDefinition
{
    public required StructureDefinition Structure { get; init; }

    public required ElementDefinition Element { get; init; }

    /// <summary>
    /// Gets or initializes a value indicating whether this object is root of structure.
    /// </summary>
    public required bool IsRootOfStructure { get; init; }
}
