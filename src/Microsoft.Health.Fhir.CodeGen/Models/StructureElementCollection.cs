// <copyright file="StructureElementCollection.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>


namespace Microsoft.Health.Fhir.CodeGen.Models;

/// <summary>A container for a structure and some subset of elements from it.</summary>
public class StructureElementCollection
{
    /// <summary>Gets or initializes the structure.</summary>
    public required Hl7.Fhir.Model.StructureDefinition Structure { get; init; }

    /// <summary>Gets or initializes the elements.</summary>
    public required List<Hl7.Fhir.Model.ElementDefinition> Elements { get; init; }
}
