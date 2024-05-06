// <copyright file="ConfigCompare.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>


using Microsoft.Health.Fhir.CodeGen.Extensions;

namespace Microsoft.Health.Fhir.CodeGen.Configuration;

public class ConfigCompare : ConfigRoot
{
    [ConfigOption(
        ArgAliases = ["--compare", "--compare-package", "-c"],
        EnvName = "Compare_Package",
        ArgArity = "0..*",
        Description = "Comparison package to load, either as directive ([name]#[version/literal]) or URL.")]
    public string[] ComparePackages { get; set; } = [];

    /// <summary>
    /// Gets or sets the configuration option for the packages to load.
    /// </summary>
    private static ConfigurationOption ComparePackagesParameter => new()
    {
        Name = "Compare_Package",
        DefaultValue = Array.Empty<string>(),
        CliOption = new System.CommandLine.Option<string[]>(["--compare", "--compare-package", "-c"], "Comparison package to load, either as directive ([name]#[version/literal]) or URL.")
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
        ArgName = "--cross-version-v1-repo-path",
        EnvName = "Cross_Version_V1_Repo_Path",
        ArgArity = "0..1",
        Description = "Path to a local v1 HL7/fhir-cross-version clone.")]
    public string CrossVersionRepoPathV1 { get; set; } = string.Empty;

    private static ConfigurationOption CrossVersionRepoPathV1Parameter => new()
    {
        Name = "Cross_Version_V1_Repo_Path",
        DefaultValue = string.Empty,
        CliOption = new System.CommandLine.Option<string>("--cross-version-v1-repo-path", "Path to a local v1 HL7/fhir-cross-version clone.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };

    [ConfigOption(
        ArgName = "--cross-version-v2-repo-path",
        EnvName = "Cross_Version_V2_Repo_Path",
        ArgArity = "0..1",
        Description = "Path to a local v2 HL7/fhir-cross-version clone.")]
    public string CrossVersionRepoPathV2 { get; set; } = string.Empty;

    private static ConfigurationOption CrossVersionRepoPathV2Parameter => new()
    {
        Name = "Cross_Version_V2_Repo_Path",
        DefaultValue = string.Empty,
        CliOption = new System.CommandLine.Option<string>("--cross-version-v2-repo-path", "Path to a local v2 HL7/fhir-cross-version clone.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };


    //[ConfigOption(
    //    ArgName = "--known-change-path",
    //    EnvName = "Known_Change_Path",
    //    ArgArity = "0..1",
    //    Description = "Source path for known version changes.")]
    //public string KnownChangePath { get; set; } = "./renames";

    //private static ConfigurationOption KnownChangePathParameter => new()
    //{
    //    Name = "Known_Change_Path",
    //    DefaultValue = "./known-changes",
    //    CliOption = new System.CommandLine.Option<string[]>("--known-change-path", "Source path for known version changes.")
    //    {
    //        Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
    //        IsRequired = false,
    //    },
    //};

    //[ConfigOption(
    //    ArgName = "--ollama-url",
    //    EnvName = "Ollama_Url",
    //    ArgArity = "0..1",
    //    Description = "Base URL for Ollama evaluation.")]
    public string OllamaUrl { get; set; } = string.Empty;

    //private static ConfigurationOption OllamaUrlParameter => new()
    //{
    //    Name = "Ollama_Url",
    //    DefaultValue = string.Empty,
    //    CliOption = new System.CommandLine.Option<string>("--ollama-url", "Base URL for Ollama evaluation.")
    //    {
    //        Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
    //        IsRequired = false,
    //    },
    //};

    //[ConfigOption(
    //    ArgName = "--ollama-model",
    //    EnvName = "Ollama_Model",
    //    ArgArity = "0..1",
    //    Description = "Model name for Ollama evaluation.")]
    public string OllamaModel { get; set; } = string.Empty;
    //private static ConfigurationOption OllamaModelParameter => new()
    //{
    //    Name = "Ollama_Model",
    //    DefaultValue = string.Empty,
    //    CliOption = new System.CommandLine.Option<string>("--ollama-model", "Model name for Ollama evaluation.")
    //    {
    //        Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
    //        IsRequired = false,
    //    },
    //};


    private static readonly ConfigurationOption[] _options =
    [
        ComparePackagesParameter,
        NoOutputParameter,
        SaveComparisonResultParameter,
        CrossVersionRepoPathV1Parameter,
        CrossVersionRepoPathV2Parameter,
        //KnownChangePathParameter,
        //OllamaUrlParameter,
        //OllamaModelParameter,
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
                case "Cross_Version_V1_Repo_Path":
                    CrossVersionRepoPathV1 = GetOpt(parseResult, opt.CliOption, CrossVersionRepoPathV1);
                    break;
                case "Cross_Version_V2_Repo_Path":
                    CrossVersionRepoPathV2 = GetOpt(parseResult, opt.CliOption, CrossVersionRepoPathV2);
                    break;
                //case "Known_Change_Path":
                //    KnownChangePath = GetOpt(parseResult, opt.CliOption, KnownChangePath);
                //    break;
                //case "Ollama_Url":
                //    OllamaUrl = GetOpt(parseResult, opt.CliOption, OllamaUrl);
                //    break;
                //case "Ollama_Model":
                //    OllamaModel = GetOpt(parseResult, opt.CliOption, OllamaModel);
                //    break;
            }
        }
    }

}
