// <copyright file="ConfigXVer.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Health.Fhir.CodeGen.Extensions;

namespace Microsoft.Health.Fhir.CodeGen.Configuration;

public class ConfigXVer : ConfigRoot
{
    [ConfigOption(
        ArgAliases = ["--compare", "--compare-package", "-c"],
        EnvName = "Compare_Package",
        ArgArity = "0..*",
        Description = "Comparison packages to load, as directives ([name][#|@][version/literal])")]
    public string[] ComparePackages { get; set; } = [];

    /// <summary>
    /// Gets or sets the configuration option for the packages to load.
    /// </summary>
    private static ConfigurationOption ComparePackagesParameter => new()
    {
        Name = "Compare_Package",
        DefaultValue = Array.Empty<string>(),
        CliOption = new System.CommandLine.Option<string[]>(["--compare", "--compare-package", "-c"], "Comparison packages to load, as directives ([name][#|@][version/literal])")
        {
            Arity = System.CommandLine.ArgumentArity.OneOrMore,
            IsRequired = false,
        },
    };

    [ConfigOption(
        ArgName = "--no-output",
        EnvName = "No_Output",
        ArgArity = "0..1",
        Description = "Do not output the comparison result.")]
    public bool NoOutput { get; set; } = false;

    private static ConfigurationOption NoOutputParameter => new()
    {
        Name = "No_Output",
        DefaultValue = false,
        CliOption = new System.CommandLine.Option<bool>("--no-output", "Do not output the comparison result.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };

    [ConfigOption(
               ArgName = "--save-comparison-result",
               EnvName = "Save_Comparison_Result",
               ArgArity = "0..1",
               Description = "Save the comparison result to a file.")]
    public bool SaveComparisonResult { get; set; } = false;
    private static ConfigurationOption SaveComparisonResultParameter => new()
    {
        Name = "Save_Comparison_Result",
        DefaultValue = false,
        CliOption = new System.CommandLine.Option<bool>("--save-comparison-result", "Save the comparison result to a file.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };

    [ConfigOption(
        ArgName = "--map-source-path",
        EnvName = "Map_Source_Path",
        ArgArity = "0..1",
        Description = "Path to FHIR maps to load (e.g., clone of HL7/fhir-cross-version).")]
    public string CrossVersionMapSourcePath { get; set; } = string.Empty;

    private static ConfigurationOption CrossVersionMapSourcePathParameter => new()
    {
        Name = "Map_Source_Path",
        DefaultValue = string.Empty,
        CliOption = new System.CommandLine.Option<string>("--map-source-path", "Path to FHIR maps to load (e.g., clone of HL7/fhir-cross-version).")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };

    [ConfigOption(
        ArgName = "--map-destination-path",
        EnvName = "Map_Destination_Path",
        ArgArity = "0..1",
        Description = "Path to directory to save FHIR maps to (e.g., clone of HL7/fhir-cross-version).")]
    public string CrossVersionMapDestinationPath { get; set; } = string.Empty;

    private static ConfigurationOption CrossVersionMapDestinationPathParameter => new()
    {
        Name = "Map_Destination_Path",
        DefaultValue = string.Empty,
        CliOption = new System.CommandLine.Option<string>("--map-destination-path", "Path to directory to save FHIR maps to (e.g., clone of HL7/fhir-cross-version).")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };


    private static readonly ConfigurationOption[] _options =
    [
        ComparePackagesParameter,
        NoOutputParameter,
        SaveComparisonResultParameter,
        CrossVersionMapSourcePathParameter,
        CrossVersionMapDestinationPathParameter,
    ];

    /// <summary>Gets the array of configuration options.</summary>
    /// <returns>An array of configuration option.</returns>
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
                case "Compare_Package":
                    ComparePackages = GetOptArray(parseResult, opt.CliOption, ComparePackages);
                    break;
                case "No_Output":
                    NoOutput = GetOpt(parseResult, opt.CliOption, NoOutput);
                    break;
                case "Save_Comparison_Result":
                    SaveComparisonResult = GetOpt(parseResult, opt.CliOption, SaveComparisonResult);
                    break;
                case "Map_Source_Path":
                    CrossVersionMapSourcePath = GetOpt(parseResult, opt.CliOption, CrossVersionMapSourcePath);
                    break;
                case "Map_Destination_Path":
                    CrossVersionMapDestinationPath = GetOpt(parseResult, opt.CliOption, CrossVersionMapDestinationPath);
                    break;
            }
        }
    }

}
