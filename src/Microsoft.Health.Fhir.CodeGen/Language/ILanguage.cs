// <copyright file="ILanguage.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>


using Microsoft.Health.Fhir.CodeGen.Models;

namespace Microsoft.Health.Fhir.CodeGen.Language;

/// <summary>Interface for code generation languages.</summary>
public interface ILanguage
{
    /// <summary>Gets the language name.</summary>
    string Name { get; }

    /// <summary>Gets the FHIR primitive type map.</summary>
    Dictionary<string, string> FhirPrimitiveTypeMap { get; }

    /// <summary>Gets a value indicating whether this language is idempotent.</summary>
    bool IsIdempotent { get; }

    /// <summary>Gets the type of the configuration.</summary>
    Type ConfigType { get; }

    /// <summary>Exports the given configuration.</summary>
    /// <param name="config">     The configuration.</param>
    /// <param name="definitions">The definitions to export.</param>
    /// <param name="writeStream">(Optional) Stream to write data to.</param>
    void Export(
        object config,
        DefinitionCollection definitions);
}
