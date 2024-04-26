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
    private static ConfigurationOption ComparePackagesParameter { get; } = new()
    {
        Name = "Compare_Package",
        DefaultValue = Array.Empty<string>(),
        CliOption = new System.CommandLine.Option<string[]>(["--compare", "--compare-package", "-c"], "Comparison package to load, either as directive ([name]#[version/literal]) or URL.")
        {
            Arity = System.CommandLine.ArgumentArity.OneOrMore,
            IsRequired = false,
        },
    };

    private static readonly ConfigurationOption[] _options =
    [
        ComparePackagesParameter,
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
            }
        }
    }

}
