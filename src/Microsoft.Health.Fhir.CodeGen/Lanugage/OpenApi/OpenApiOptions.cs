// <copyright file="OpenApiOptions.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Microsoft.Health.Fhir.CodeGen.Extensions;
using static Microsoft.Health.Fhir.CodeGen.Lanugage.OpenApi.OpenApiCommon;

namespace Microsoft.Health.Fhir.CodeGen.Lanugage.OpenApi;

/// <summary>An open API options.</summary>
public class OpenApiOptions
{
    /// <summary>Gets or sets the open API version.</summary>
    [ConfigOption(
        ArgName = "--version",
        Description = "Open API version to use.")]
    public OaVersion OpenApiVersion { get; set; } = OaVersion.v2;

    /// <summary>Gets or sets the file format.</summary>
    [ConfigOption(
        ArgName = "--format",
        Description = "File format to export.")]
    public OaFileFormat FileFormat { get; set; } = OaFileFormat.Json;

    /// <summary>Gets or sets the FHIR structures to export, default is all.</summary>
    [ConfigOption(
        ArgName = "--extension-support",
        Description = "The level of extensions to include.")]
    public ExtensionSupportLevel ExtensionSupport { get; set; } = ExtensionSupportLevel.NonPrimitive;
}
