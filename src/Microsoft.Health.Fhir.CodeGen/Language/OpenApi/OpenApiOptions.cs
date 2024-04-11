// <copyright file="OpenApiOptions.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Microsoft.Health.Fhir.CodeGen.Configuration;
using Microsoft.Health.Fhir.CodeGen.Extensions;
using static Microsoft.Health.Fhir.CodeGen.Language.OpenApi.OpenApiCommon;

namespace Microsoft.Health.Fhir.CodeGen.Language.OpenApi;

/// <summary>An open API options.</summary>
public class OpenApiOptions : ConfigGenerate
{
    /// <summary>Gets or sets the open API version.</summary>
    [ConfigOption(
        ArgName = "--oas-version",
        Description = "Open API version to use.")]
    public OaVersion OpenApiVersion { get; set; } = OaVersion.v2;

    private static ConfigurationOption OpenApiVersionParameter { get; } = new()
    {
        Name = "Version",
        DefaultValue = OaVersion.v2,
        CliOption = new System.CommandLine.Option<OaVersion>("--oas-version", "Open API version to export as.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };

    /// <summary>Gets or sets the file format.</summary>
    [ConfigOption(
        ArgName = "--format",
        Description = "File format to export.")]
    public OaFileFormat FileFormat { get; set; } = OaFileFormat.Json;

    private static ConfigurationOption FileFormatParameter { get; } = new()
    {
        Name = "FileFormat",
        DefaultValue = OaFileFormat.Json,
        CliOption = new System.CommandLine.Option<OaFileFormat>("--format", "File format to export.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };

    /// <summary>Gets or sets the FHIR structures to export, default is all.</summary>
    [ConfigOption(
        ArgName = "--extension-support",
        Description = "The level of extensions to include.")]
    public ExtensionSupportLevel ExtensionSupport { get; set; } = ExtensionSupportLevel.NonPrimitive;

    private static ConfigurationOption ExtensionSupportParameter { get; } = new()
    {
        Name = "ExtensionSupport",
        DefaultValue = ExtensionSupportLevel.NonPrimitive,
        CliOption = new System.CommandLine.Option<ExtensionSupportLevel>("--extension-support", "The level of extensions to include.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };

    private static readonly ConfigurationOption[] _options =
    [
        OpenApiVersionParameter,
        FileFormatParameter,
        ExtensionSupportParameter,
    ];


    /// <summary>
    /// Gets the configuration options for the current instance and its base class.
    /// </summary>
    /// <returns>An array of configuration options.</returns>
    public override ConfigurationOption[] GetOptions()
    {
        return [.. base.GetOptions(), .. _options];
    }

    public override void Parse(System.CommandLine.Parsing.ParseResult parseResult)
    {
        // parse base properties
        base.Parse(parseResult);

        // iterate over options for ones we are interested in
        foreach (ConfigurationOption opt in _options)
        {
            switch (opt.Name)
            {
                case "Version":
                    OpenApiVersion = GetOpt(parseResult, opt.CliOption, OpenApiVersion);
                    break;
                case "FileFormat":
                    FileFormat = GetOpt(parseResult, opt.CliOption, FileFormat);
                    break;
                case "ExtensionSupport":
                    ExtensionSupport = GetOpt(parseResult, opt.CliOption, ExtensionSupport);
                    break;
            }
        }
    }
}
