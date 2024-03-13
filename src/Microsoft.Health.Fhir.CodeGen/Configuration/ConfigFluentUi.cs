// <copyright file="ConfigFluentUi.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Microsoft.Health.Fhir.CodeGen.Extensions;

namespace Microsoft.Health.Fhir.CodeGen.Configuration;

/// <summary>Configuration settings when using the Fluent web user interface.</summary>
public class ConfigFluentUi : ConfigRoot
{
    /// <summary>(Immutable) The default user interface listen port.</summary>
    private const int _defaultUiListenPort = 0;

    /// <summary>Gets or sets the listen port for the UI.</summary>
    [ConfigOption(
        ArgAliases = new[] { "--port", "--listen-port" },
        EnvName = "Listen_Port",
        Description = "Listen port for the web server.")]
    public int UiListenPort { get; set; } = _defaultUiListenPort;

    private static ConfigurationOption UiListenPortParameter { get; } = new()
    {
        Name = "ListenPort",
        EnvVarName = "Listen_Port",
        DefaultValue = _defaultUiListenPort,
        CliOption = new System.CommandLine.Option<int>(["--port", "--listen-port"], "Listen port for the web server.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };


    /// <summary>Gets or sets a value indicating whether the browser should be opened at launch.</summary>
    [ConfigOption(
        ArgAliases = new[] { "--open-browser" },
        EnvName = "Open_Browser",
        Description = "Open a browser once the server starts.")]
    public bool OpenBrowser { get; set; } = true;

    private static ConfigurationOption OpenBrowserParameter { get; } = new()
    {
        Name = "OpenBrowser",
        EnvVarName = "Open_Browser",
        DefaultValue = true,
        CliOption = new System.CommandLine.Option<bool>("--open-browser", "Flag to open a browser once the server starts.")
        {
            Arity = System.CommandLine.ArgumentArity.ZeroOrOne,
            IsRequired = false,
        },
    };

    private static readonly ConfigurationOption[] _options = new ConfigurationOption[]
    {
        UiListenPortParameter,
        OpenBrowserParameter,
    };

    /// <summary>
    /// Gets the configuration options for the current instance and its base class.
    /// </summary>
    /// <returns>An array of configuration options.</returns>
    public override ConfigurationOption[] GetOptions()
    {
        return base.GetOptions().Concat(_options).ToArray();
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
                case "ListenPort":
                    UiListenPort = GetOpt(parseResult, opt.CliOption, UiListenPort);
                    break;
                case "OpenBrowser":
                    OpenBrowser = GetOpt(parseResult, opt.CliOption, OpenBrowser);
                    break;
            }
        }
    }

}
