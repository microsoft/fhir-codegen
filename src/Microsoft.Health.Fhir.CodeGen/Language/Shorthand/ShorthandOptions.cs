// <copyright file="ShorthandOptions.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Health.Fhir.CodeGen.Configuration;

namespace Microsoft.Health.Fhir.CodeGen.Language.Shorthand;

public class ShorthandOptions : ConfigGenerate
{

    private static readonly ConfigurationOption[] _options =
    [
    ];

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
            }
        }
    }

}
