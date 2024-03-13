// <copyright file="ConfigurationOption.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>


namespace Microsoft.Health.Fhir.CodeGen.Configuration;

public record class ConfigurationOption
{
    /// <summary>Gets or initializes the name.</summary>
    public required string Name { get; init; }

    /// <summary>Gets or sets the name of the environment variable name.</summary>
    public string EnvVarName { get; init; } = string.Empty;

    /// <summary>Gets or sets the default value.</summary>
    public required object DefaultValue { get; init; }

    /// <summary>Gets or initializes the CLI option.</summary>
    public required System.CommandLine.Option CliOption { get; init; }
}
