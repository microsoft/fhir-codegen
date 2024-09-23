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
    /// <summary>(Immutable) Options for controlling the operation.</summary>
    private static readonly ConfigurationOption[] _options = [];

    /// <summary>
    /// Gets the configuration options for the current instance and its base class.
    /// </summary>
    /// <returns>An array of configuration options.</returns>
    public override ConfigurationOption[] GetOptions()
    {
        return [.. base.GetOptions(), .. _options];
    }

    //public override void Parse(System.CommandLine.Parsing.ParseResult parseResult)
    //{
    //    // parse base properties
    //    base.Parse(parseResult);

    //    // iterate over options for ones we are interested in
    //    foreach (ConfigurationOption opt in _options)
    //    {
    //        switch (opt.Name)
    //        {
    //            case "Module":
    //                Module = GetOpt(parseResult, opt.CliOption, Module);
    //                break;
    //        }
    //    }
    //}
}
