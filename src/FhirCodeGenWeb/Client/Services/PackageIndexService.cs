// <copyright file="PackageIndexService.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.Net.Http.Json;
using Microsoft.Health.Fhir.CodeGenCommon.Models;

namespace FhirCodeGenWeb.Client.Services;

/// <summary>A service for accessing package indexes information.</summary>
public class PackageIndexService : IPackageIndexService
{
    /// <summary>The package records.</summary>
    private Dictionary<string, PackageCacheRecord> _packageRecords = new();

    /// <summary>Gets or sets the package records.</summary>
    public Dictionary<string, PackageCacheRecord> PackageRecords { get => _packageRecords; set => _packageRecords = value; }

    /// <summary>Occurs when On Changed.</summary>
    public event EventHandler<EventArgs>? OnChanged;

    /// <summary>The HTTP client.</summary>
    private HttpClient _httpClient;

    /// <summary>The timer.</summary>
    private System.Threading.Timer? timer = null;

    /// <summary>
    /// Initializes a new instance of the <see cref="PackageIndexService"/> class.
    /// </summary>
    /// <param name="http">The HTTP.</param>
    public PackageIndexService(HttpClient http)
    {
        _httpClient = http;
    }

    /// <summary>Updates the packages and status.</summary>
    /// <returns>An asynchronous result.</returns>
    public async Task UpdatePackagesAndStatusAsync()
    {
        IEnumerable<PackageCacheRecord>? packages = await _httpClient.GetFromJsonAsync<IEnumerable<PackageCacheRecord>>("api/FhirManager/package");
        if (packages != null)
        {
            foreach (PackageCacheRecord package in packages)
            {
                if (!_packageRecords.ContainsKey(package.CacheDirective))
                {
                    _packageRecords.Add(package.CacheDirective, package);
                    continue;
                }

                if (_packageRecords[package.CacheDirective].PackageState != package.PackageState)
                {
                    _packageRecords[package.CacheDirective] = package;
                    continue;
                }
            }

            // manually flag state has changed in case we are in a callback / async context
            StateHasChanged();
        }
    }

    /// <summary>Loads a package.</summary>
    /// <param name="directive">The directive.</param>
    public async void LoadPackageAsync(string directive)
    {
        _packageRecords[directive] = _packageRecords[directive] with
        {
            PackageState = PackageLoadStateEnum.Queued,
        };

        StateHasChanged();

        HttpResponseMessage response = await _httpClient.PostAsJsonAsync("api/FhirManager/package/load", directive);

        if (response.IsSuccessStatusCode)
        {
            PackageLoadStateEnum? state = await response.Content.ReadFromJsonAsync<PackageLoadStateEnum>();

            if ((state != null) && (_packageRecords.ContainsKey(directive)))
            {
                _packageRecords[directive] = _packageRecords[directive] with
                {
                    PackageState = (PackageLoadStateEnum)state,
                };

                StateHasChanged();
            }

            if (timer == null)
            {
                timer = new(async _ =>
                {
                    await UpdatePackagesAndStatusAsync();

                    bool shouldContinue = false;

                    foreach (PackageCacheRecord packageRecord in _packageRecords.Values)
                    {
                        if ((packageRecord.PackageState == PackageLoadStateEnum.Unknown) ||
                            (packageRecord.PackageState == PackageLoadStateEnum.Queued) ||
                            (packageRecord.PackageState == PackageLoadStateEnum.InProgress))
                        {
                            shouldContinue = true;
                            break;
                        }
                    }

                    if (!shouldContinue)
                    {
                        timer?.Dispose();
                        timer = null;
                    }
                }, null, 0, 1000);
            }
        }
    }

    /// <summary>State has changed.</summary>
    public void StateHasChanged()
    {
        EventHandler<EventArgs>? handler = OnChanged;

        if (handler != null)
        {
            handler(this, new());
        }
    }
}
