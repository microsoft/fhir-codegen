// <copyright file="ConfigBase.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.CommandLine;

namespace fhir_codegen.Runtime;

/// <summary>Configuration base with common settings.</summary>
public abstract class ConfigBase
{
    /// <summary>Gets or sets the pathname of the FHIR cache directory.</summary>
    public string FhirCacheDirectory { get; set; } = string.Empty;

    /// <summary>Gets or sets the pathname of the output directory.</summary>
    public string OutputDirectory { get; set; } = string.Empty;

    /// <summary>Gets or sets the packages.</summary>
    public string[] Packages { get; set; } = Array.Empty<string>();
}
