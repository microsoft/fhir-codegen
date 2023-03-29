// <copyright file="IServerConnectorService.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using Microsoft.Health.Fhir.CodeGenCommon.Models;

namespace FhirCodeGenBlazor.Services;

public interface IServerConnectorService
{
    /// <summary>Initializes this object.</summary>
    void Init();

    /// <summary>Attempts to get server information.</summary>
    /// <param name="serverUrl">      URL of the server.</param>
    /// <param name="resolveExternal">True to resolve external.</param>
    /// <param name="headers">        The headers.</param>
    /// <param name="json">           [out] The JSON.</param>
    /// <param name="serverInfo">     [out] Information describing the server.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    bool TryGetServerInfo(
        string serverUrl,
        bool resolveExternal,
        Dictionary<string, IEnumerable<string>> headers,
        out string json,
        out FhirCapabiltyStatement serverInfo);

    /// <summary>Parse capability JSON.</summary>
    /// <param name="json">The JSON.</param>
    /// <returns>A FhirCapabiltyStatement.</returns>
    FhirCapabiltyStatement ParseCapabilityJson(string json);

}
