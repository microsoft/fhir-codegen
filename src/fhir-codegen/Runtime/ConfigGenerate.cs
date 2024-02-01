// <copyright file="ConfigCli.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Microsoft.Health.Fhir.CodeGenCommon.Models;

namespace fhir_codegen.Runtime;

/// <summary>Configuration settings when performing a single-pass generation.</summary>
public class ConfigGenerate : ConfigBase
{
    /// <summary>Gets or sets the FHIR structures to export, default is all.</summary>
    public HashSet<FhirArtifactClassEnum> ExportStructures { get; set; } = new();
}
