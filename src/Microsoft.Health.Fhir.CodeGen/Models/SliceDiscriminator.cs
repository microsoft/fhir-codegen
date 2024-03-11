// <copyright file="SliceDiscriminator.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>


using Hl7.Fhir.Model;

namespace Microsoft.Health.Fhir.CodeGen.Models;

/// <summary>A slice discriminator.</summary>
public record class SliceDiscriminator
{
    /// <summary>Gets or initializes the type.</summary>
    public required string Type { get; init; }

    /// <summary>Gets or initializes the type of the discriminator.</summary>
    public required Hl7.Fhir.Model.ElementDefinition.DiscriminatorType DiscriminatorType { get; init; }

    /// <summary>Gets or initializes the full pathname of the file.</summary>
    public required string Path { get; init; }

    /// <summary>Gets or initializes the full pathname of the post resovle file.</summary>
    public required string PostResovlePath { get; init; }

    /// <summary>Gets or initializes the identifier.</summary>
    public required string Id { get; init; }

    /// <summary>Gets or initializes the value.</summary>
    public required DataType Value { get; init; }

    /// <summary>Gets or initializes a value indicating whether this object is binding.</summary>
    public bool IsBinding { get; init; } = false;

    /// <summary>Gets or initializes the name of the binding.</summary>
    public string BindingName { get; init; } = string.Empty;
}
