// <copyright file="ConfigCli.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Microsoft.Health.Fhir.CodeGen.Extensions;
using Microsoft.Health.Fhir.CodeGenCommon.Models;

namespace Microsoft.Health.Fhir.CodeGen.Configuration;

/// <summary>Configuration settings when performing a single-pass generation.</summary>
public class ConfigGenerate : ConfigRoot
{
    /// <summary>Gets or sets the FHIR structures to export, default is all.</summary>
    [ConfigOption(
        ArgName = "--structures",
        EnvName = "Export_Structures",
        Description = "Types of FHIR structures to export, default is all.",
        ArgArity = "0..*")]
    public FhirArtifactClassEnum[] ExportStructures { get; set; } = Array.Empty<FhirArtifactClassEnum>();
    //public HashSet<FhirArtifactClassEnum> ExportStructures { get; set; } = new();

    /// <summary>Gets or sets the export keys.</summary>
    [ConfigOption(
        ArgName = "--export-keys",
        EnvName = "Export_Keys",
        Description = "Keys of FHIR structures to export (e.g., Patient), empty means all.",
        ArgArity = "0..*")]
    public HashSet<string> ExportKeys { get; set; } = new();

    /// <summary>Gets a value indicating whether the experimental should be included.</summary>
    [ConfigOption(
        ArgAliases = new[] { "--include-experimental", "--experimental" },
        EnvName = "Include_Experimental",
        Description = "If the output should include structures marked experimental.")]
    public bool IncludeExperimental { get; set; } = false;
}
