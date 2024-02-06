// <copyright file="ConfigRoot.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.CommandLine;
using Microsoft.Health.Fhir.CodeGen.Extensions;
using Microsoft.Health.Fhir.CodeGenCommon.Models;

namespace Microsoft.Health.Fhir.CodeGen.Configuration;

/// <summary>Root configuration with common settings.</summary>
public class ConfigRoot
{
    /// <summary>Gets or sets the pathname of the FHIR cache directory.</summary>
    [ConfigOption(
        ArgName = "--fhir-cache",
        EnvName = "Fhir_Cache",
        Description = "Location of the FHIR cache.")]
    public string FhirCacheDirectory { get; set; } = "~/.fhir";

    /// <summary>Gets or sets the pathname of the output directory.</summary>
    [ConfigOption(
        ArgAliases = new[] { "--output-path", "--output-directory", "--output-dir" },
        EnvName = "Output_Path",
        Description = "File or directory to write output.")]
    public string OutputDirectory { get; set; } = "./generated";

    /// <summary>Gets or sets the packages to load.</summary>
    [ConfigOption(
        ArgAliases = new[] { "--package", "--load-package", "-p" },
        EnvName = "Load_Package",
        ArgArity = "0..*",
        Description = "Package to load, either as directive ([name]#[version/literal]) or URL.")]
    public string[] Packages { get; set; } = Array.Empty<string>();

    /// <summary>Gets or sets a value indicating whether the offline mode.</summary>
    [ConfigOption(
        ArgName = "--offline",
        EnvName = "Offline",
        Description = "Offline mode (will not download missing packages).")]
    public bool OfflineMode { get; set; } = false;
}
