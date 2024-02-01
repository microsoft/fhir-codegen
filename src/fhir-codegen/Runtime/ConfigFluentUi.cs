// <copyright file="ConfigFluentUi.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

namespace fhir_codegen.Runtime;

/// <summary>Configuration settings when using the Fluent web user interface.</summary>
public class ConfigFluentUi : ConfigBase
{
    /// <summary>(Immutable) The default user interface listen port.</summary>
    private const int _defaultUiListenPort = 0;

    /// <summary>Gets or sets the listen port for the UI.</summary>
    public int UiListenPort { get; set; } = _defaultUiListenPort;

    /// <summary>Gets or sets a value indicating whether the browser should be opened at launch.</summary>
    public bool OpenBrowser { get; set; } = true;
}
