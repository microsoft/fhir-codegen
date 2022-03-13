// <copyright file="ClientArtifactService.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.Net.Http.Json;
using FhirCodeGenWeb.Client.Models;
using Microsoft.Health.Fhir.CodeGenCommon.Models;

namespace FhirCodeGenWeb.Client.Services;

/// <summary>A service for accessing client artifacts information.</summary>
public class ClientArtifactService : IClientArtifactService
{
    /// <summary>The artifacts by class by package.</summary>
    private Dictionary<string, Dictionary<FhirArtifactClassEnum, IEnumerable<FhirArtifactRecord>>> _artifactsByClassByPackage;

    /// <summary>The HTTP client.</summary>
    private HttpClient _httpClient;

    /// <summary>The package service.</summary>
    private IClientPackageService _packageService;

    /// <summary>Occurs when the artifact index for a package has changed.</summary>
    public event EventHandler<ArtifactIndexChangedEventArgs>? OnChanged;

    /// <summary>
    /// Initializes a new instance of the <see cref="ClientArtifactService"/> class.
    /// </summary>
    /// <param name="http">          The HTTP.</param>
    /// <param name="packageService">The package service.</param>
    public ClientArtifactService(HttpClient http, IClientPackageService packageService)
    {
        _httpClient = http;
        _artifactsByClassByPackage = new();
        _packageService = packageService;
    }

    /// <summary>Gets or sets the package records.</summary>
    /// <param name="packageName">Name of the package.</param>
    /// <param name="version">    The version.</param>
    /// <returns>
    /// A Dictionary&lt;FhirArtifactClassEnum,IEnumerable&lt;FhirArtifactRecord&gt;&gt;
    /// </returns>
    public Dictionary<FhirArtifactClassEnum, IEnumerable<FhirArtifactRecord>> ArtifactsForPackage(string packageName, string version)
    {
        string directive = packageName + "#" + version;

        if (!_artifactsByClassByPackage.ContainsKey(directive))
        {
            return new();
        }

        return _artifactsByClassByPackage[directive];
    }

    /// <summary>Updates the artifacts asynchronous.</summary>
    /// <param name="packageName">Name of the package.</param>
    /// <param name="version">    The version.</param>
    /// <returns>An asynchronous result.</returns>
    public async Task UpdateArtifactsAsync(string packageName, string version)
    {
        string directive = packageName + "#" + version;

        if (!_packageService.PackageRecords.ContainsKey(directive))
        {
            Console.WriteLine($"ClientArtifactService <<< unresolved package: {packageName}#{version}");
            return;
        }

        Dictionary<FhirArtifactClassEnum, IEnumerable<FhirArtifactRecord>>? returnedArtifacts =
            await _httpClient.GetFromJsonAsync<Dictionary<FhirArtifactClassEnum, IEnumerable<FhirArtifactRecord>>>(
                $"api/package/{packageName}/{version}/artifactIndex");

        if (returnedArtifacts != null)
        {
            _artifactsByClassByPackage[directive] = returnedArtifacts;
            StateHasChanged(packageName, version);
        }
    }

    /// <summary>State has changed.</summary>
    /// <param name="packageName">Name of the package.</param>
    /// <param name="version">    The version.</param>
    public void StateHasChanged(string packageName, string version)
    {
        EventHandler<ArtifactIndexChangedEventArgs>? handler = OnChanged;

        if (handler != null)
        {
            handler(this, new() { PackageName = packageName, Version = version });
        }
    }
}
