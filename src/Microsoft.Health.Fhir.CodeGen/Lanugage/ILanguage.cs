// <copyright file="ILanguage.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>


namespace Microsoft.Health.Fhir.CodeGen.Lanugage;

/// <summary>Interface for code generation languages.</summary>
public interface ILanguage
{
    /// <summary>Gets the language name.</summary>
    string Name { get; }

    /// <summary>Gets the FHIR primitive type map.</summary>
    Dictionary<string, string> FhirPrimitiveTypeMap { get; }

    /// <summary>Gets options for controlling the language.</summary>
    Dictionary<string, System.CommandLine.Option> LanguageOptions { get; }
}
