// <copyright file="ConfigFluentUi.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Microsoft.Health.Fhir.CodeGen.Extensions;

namespace fhir_codegen.Runtime;

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

    /// <summary>Gets or sets a value indicating whether the browser should be opened at launch.</summary>
    [ConfigOption(
        ArgAliases = new[] { "--open-browser" },
        EnvName = "Open_Browser",
        Description = "Open a browser once the server starts.")]
    public bool OpenBrowser { get; set; } = true;
}
