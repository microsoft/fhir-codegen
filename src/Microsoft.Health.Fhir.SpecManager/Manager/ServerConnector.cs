// <copyright file="ServerConnector.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Health.Fhir.SpecManager.Converters;
using Microsoft.Health.Fhir.SpecManager.Models;

namespace Microsoft.Health.Fhir.SpecManager.Manager;

/// <summary>A FHIR server connector.</summary>
public static class ServerConnector
{
    /// <summary>
    /// Attempts to get server information a FhirServerInfo from the given string.
    /// </summary>
    /// <param name="serverUrl"> URL of the server.</param>
    /// <param name="serverInfo">[out] Information describing the server.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public static bool TryGetServerInfo(
        string serverUrl,
        out FhirServerInfo serverInfo)
    {
        if (string.IsNullOrEmpty(serverUrl))
        {
            serverInfo = null;
            return false;
        }

        HttpClient client = null;
        HttpRequestMessage request = null;

        try
        {
            Uri serverUri = new Uri(serverUrl);

            client = new HttpClient();

            request = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(serverUri, "metadata"),
                Headers =
                {
                    Accept =
                    {
                        new MediaTypeWithQualityHeaderValue("application/fhir+json"),
                    },
                },
            };

            Console.WriteLine($"Requesting metadata from {request.RequestUri}...");

            HttpResponseMessage response = client.SendAsync(request).Result;

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine($"Request to {request.RequestUri} failed! {response.StatusCode}");
                serverInfo = null;
                return false;
            }

            string content = response.Content.ReadAsStringAsync().Result;

            if (string.IsNullOrEmpty(content))
            {
                Console.WriteLine($"Request to {request.RequestUri} returned empty body!");
                serverInfo = null;
                return false;
            }

            string fhirVersion;
            using (JsonDocument jdoc = JsonDocument.Parse(content))
            {
                fhirVersion = jdoc.RootElement.GetProperty("fhirVersion").GetString();
            }

            if (string.IsNullOrEmpty(fhirVersion))
            {
                Console.WriteLine($"Could not determine the FHIR version for {serverUrl}!");
                serverInfo = null;
                return false;
            }

            Console.WriteLine($"Connected to {serverUrl}, FHIR version: {fhirVersion}");

            IFhirConverter fhirConverter = ConverterHelper.ConverterForVersion(fhirVersion);

            object metadata = fhirConverter.ParseResource(content);

            fhirConverter.ProcessMetadata(metadata, serverUrl, out serverInfo);

            if (serverInfo != null)
            {
                Console.WriteLine($"Server Information from {serverUrl}:");
                Console.WriteLine($"\t    FHIR Version: {serverInfo.FhirVersion}");
                Console.WriteLine($"\t   Software Name: {serverInfo.SoftwareName}");
                Console.WriteLine($"\tSoftware Version: {serverInfo.SoftwareVersion}");
                Console.WriteLine($"\t    Release Date: {serverInfo.SoftwareReleaseDate}");
                Console.WriteLine($"\t     Description: {serverInfo.ImplementationDescription}");
                Console.WriteLine($"\t       Resources: {serverInfo.ResourceInteractions.Count}");

                serverInfo.TryResolveServerPackages();

                return true;
            }
        }
#pragma warning disable CA1031 // Do not catch general exception types
        catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
        {
            Console.WriteLine($"Failed to get server info from: {serverUrl}, {ex.Message}");
            serverInfo = null;
            return false;
        }
        finally
        {
            if (request != null)
            {
                request.Dispose();
            }

            if (client != null)
            {
                client.Dispose();
            }
        }

        serverInfo = null;
        return false;
    }

    /// <summary>
    /// Attempt to download an instance from the specified FULL url.
    /// </summary>
    /// <param name="instanceUrl">Full URL for the instance content.</param>
    /// <param name="fhirJson">Downloaded JSON or null if download fails.</param>
    /// <returns></returns>

    public static bool TryDownloadResource(
        string instanceUrl,
        out string fhirJson)
    {
        if (string.IsNullOrEmpty(instanceUrl))
        {
            fhirJson = null;
            return false;
        }

        HttpClient client = null;
        HttpRequestMessage request = null;

        try
        {
            Uri instanceUri = new Uri(instanceUrl);

            client = new HttpClient();

            request = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = instanceUri,
                Headers =
                {
                    Accept =
                    {
                        new MediaTypeWithQualityHeaderValue("application/fhir+json"),
                    },
                },
            };

            Console.WriteLine($"Requesting {request.RequestUri}...");

            HttpResponseMessage response = client.SendAsync(request).Result;

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine($"Request to {request.RequestUri} failed! {response.StatusCode}");
                fhirJson = null;
                return false;
            }

            fhirJson = response.Content.ReadAsStringAsync().Result;

            if (string.IsNullOrEmpty(fhirJson))
            {
                Console.WriteLine($"Request to {request.RequestUri} returned empty body!");
                fhirJson = null;
                return false;
            }

            return true;
        }
#pragma warning disable CA1031 // Do not catch general exception types
        catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
        {
            Console.WriteLine($"Failed to get resource: {instanceUrl}, {ex.Message}");
            fhirJson = null;
            return false;
        }
        finally
        {
            if (request != null)
            {
                request.Dispose();
            }

            if (client != null)
            {
                client.Dispose();
            }
        }

        fhirJson = null;
        return false;
    }

}
