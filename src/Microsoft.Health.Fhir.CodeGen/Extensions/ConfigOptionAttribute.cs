// <copyright file="ConfigOptionAttribute.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.CommandLine;

namespace Microsoft.Health.Fhir.CodeGen.Extensions;

/// <summary>Attribute for configuration option.</summary>
public class ConfigOptionAttribute : Attribute
{
    /// <summary>Gets or sets the name of the environment variable name.</summary>
    public string EnvName { get; set; } = string.Empty;

    /// <summary>Gets or sets the name of the argument.</summary>
    public string ArgName { get; set; } = string.Empty;

    /// <summary>Gets or sets the argument aliases.</summary>
    public string[] ArgAliases { get; set; } = Array.Empty<string>();

    /// <summary>Gets or sets the description.</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>Gets or sets the arity, specified as a FHIR Cardinality string.</summary>
    public string ArgArity { get; set; } = "0..1";
}
