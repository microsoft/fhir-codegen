// <copyright file="ConfigSql.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Health.Fhir.CodeGen.Extensions;

namespace Microsoft.Health.Fhir.CodeGen.Configuration;

public class ConfigSql : ConfigRoot
{
    [ConfigOption(
    ArgName = "--view-definition-directory",
    EnvName = "View_Definition_Directory",
    ArgArity = "0..1",
    Description = "Local path to a source for view definitions to load.")]
    public string ViewDefinitionDirectory { get; set; } = "";

    private static ConfigurationOption ViewDefinitionDirectoryParameter => new()
    {
        Name = "View_Definition_Directory",
        DefaultValue = "",
        CliOption = new System.CommandLine.Option<string>("--view-definition-directory", "Local path to a source for view definitions to load.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };

    [ConfigOption(
    ArgName = "--export-db-name",
    EnvName = "Export_Database_Name",
    ArgArity = "0..1",
    Description = "Name of the database to generate.")]
    public string ExportDatabaseName { get; set; } = "";

    private static ConfigurationOption ExportDatabaseNameParameter => new()
    {
        Name = "Export_Database_Name",
        DefaultValue = "export.db",
        CliOption = new System.CommandLine.Option<string>("--export-db-name", "Name of the database to generate.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };


    private static readonly ConfigurationOption[] _options =
    [
        ViewDefinitionDirectoryParameter,
        ExportDatabaseNameParameter,
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
                case "View_Definition_Directory":
                    {
                        string dir = GetOpt(parseResult, opt, ViewDefinitionDirectory);

                        if (string.IsNullOrEmpty(dir))
                        {
                            dir = FindRelativeDir(string.Empty, ".");
                        }
                        else if (!Path.IsPathRooted(dir))
                        {
                            dir = FindRelativeDir(string.Empty, dir);
                        }

                        ViewDefinitionDirectory = dir;

                    }
                    break;
                case "Export_Database_Name":
                    ExportDatabaseName = GetOpt(parseResult, opt, ExportDatabaseName);
                    break;
            }
        }
    }

}
