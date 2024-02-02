// <copyright file="ILanguage.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>


using Microsoft.Health.Fhir.CodeGen.Lanugage.OpenApi;

namespace Microsoft.Health.Fhir.CodeGen.Lanugage;

/// <summary>Interface for code generation languages.</summary>
public interface ILanguage
{
    /// <summary>Gets the language name.</summary>
    string Name { get; }

    /// <summary>Gets the FHIR primitive type map.</summary>
    Dictionary<string, string> FhirPrimitiveTypeMap { get; }
}

/// <summary>Interface for code generation languages.</summary>
public interface ILanguage<T> : ILanguage
    where T : class, new()
{
    /// <summary>Exports the given configuration.</summary>
    /// <param name="config">The configuration.</param>
    void Export(T config);
}

