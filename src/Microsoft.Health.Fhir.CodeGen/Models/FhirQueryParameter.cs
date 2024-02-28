// <copyright file="SearchResultParameter.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Hl7.Fhir.Model;

namespace Microsoft.Health.Fhir.CodeGen.Models;

public record class FhirQueryParameter
{
    /// <summary>Gets or initializes the name.</summary>
    public required string Name { get; init; }

    /// <summary>Gets or initializes URL of the document.</summary>
    public required string Url { get; init; }

    /// <summary>Gets or initializes the description.</summary>
    public required string Description { get; init; }

    /// <summary>Gets or initializes the type of the parameter.</summary>
    public required SearchParamType ParamType { get; init; }

    /// <summary>Gets or initializes the allowed values.</summary>
    public IEnumerable<string> AllowedValues { get; init; } = Enumerable.Empty<string>();
}
