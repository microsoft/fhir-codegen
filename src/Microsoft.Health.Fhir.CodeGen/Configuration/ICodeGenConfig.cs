// <copyright file="ICodeGenConfig.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

namespace Microsoft.Health.Fhir.CodeGen.Configuration;

public interface ICodeGenConfig
{
    /// <summary>Gets options for controlling the operation.</summary>
    ConfigurationOption[] GetOptions();

    /// <summary>Parses the given parse result.</summary>
    /// <param name="parseResult">The parse result.</param>
    void Parse(System.CommandLine.Parsing.ParseResult parseResult);
}
