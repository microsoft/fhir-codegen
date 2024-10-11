// <copyright file="CqlOptions.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Health.Fhir.CodeGen.Configuration;
using Microsoft.Health.Fhir.CodeGen.Extensions;

namespace Microsoft.Health.Fhir.CodeGen.Language.Cql;

public class CqlOptions : ConfigGenerate
{
    /// <summary>Gets or sets the CQL support files directory.</summary>
    [ConfigOption(
        ArgName = "--cql-support-dir",
        Description = "Directory containing CQL support files (R5 ConceptMaps and Parameters).")]
    public string? CqlSupportDir { get; set; } = null;

    private static ConfigurationOption CqlSupportDirParameter { get; } = new()
    {
        Name = "CqlSupportDir",
        DefaultValue = string.Empty,
        CliOption = new System.CommandLine.Option<string?>("--cql-support-dir", "Directory containing CQL support files (R5 ConceptMaps and Parameters).")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };

    /// <summary>(Immutable) Options for controlling the operation.</summary>
    private static readonly ConfigurationOption[] _options = [
        CqlSupportDirParameter,
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
                case "CqlSupportDir":
                    {
                        string? dir = GetOpt(parseResult, opt.CliOption, CqlSupportDir);
                        if (string.IsNullOrEmpty(dir))
                        {
                            dir = FindRelativeDir(string.Empty, "Language/Cql/_Definitional/fsh-generated");
                        }
                        else if (!Path.IsPathRooted(dir))
                        {
                            dir = FindRelativeDir(string.Empty, dir!);
                        }

                        CqlSupportDir = dir;
                    }
                    break;
            }
        }
    }
}
