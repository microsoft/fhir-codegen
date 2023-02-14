// <copyright file="ServerConnector.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Health.Fhir.SpecManager.Converters;

namespace Microsoft.Health.Fhir.SpecManager.Manager;

/// <summary>A FHIR server connector.</summary>
public static class ServerConnector
{
    /// <summary>Attempts to get server information a FhirServerInfo from the given string.</summary>
    /// <param name="serverUrl">      URL of the server.</param>
    /// <param name="resolveExternal">True to resolve external references.</param>
    /// <param name="serverInfo">     [out] Information describing the server.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public static bool TryGetServerInfo(
        string serverUrl,
        bool resolveExternal,
        out FhirCapabiltyStatement serverInfo)
    {
        return TryGetServerInfo(serverUrl, resolveExternal, out _, out serverInfo);
    }

    /// <summary>Attempts to get server information a FhirServerInfo from the given string.</summary>
    /// <param name="serverUrl">      URL of the server.</param>
    /// <param name="resolveExternal">True to resolve external references.</param>
    /// <param name="json">           [out] The JSON.</param>
    /// <param name="serverInfo">     [out] Information describing the server.</param>
    /// <returns>True if it succeeds, false if it fails.</returns>
    public static bool TryGetServerInfo(
        string serverUrl,
        bool resolveExternal,
        out string json,
        out FhirCapabiltyStatement serverInfo)

    {
        if (string.IsNullOrEmpty(serverUrl))
        {
            json = string.Empty;
            serverInfo = null;
            return false;
        }

        HttpClient client = null;
        HttpRequestMessage request = null;

        try
        {
            Uri requestUri;

            if (serverUrl.EndsWith("metadata", StringComparison.OrdinalIgnoreCase))
            {
                requestUri = new Uri(serverUrl);
            }
            else if (serverUrl.EndsWith("/"))
            {
                Uri serverUri = new Uri(serverUrl);
                requestUri = new Uri(serverUri, "metadata");
            }
            else
            {
                requestUri = new Uri(serverUrl + "/metadata");
            }

            client = new HttpClient();

            request = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = requestUri,
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
                Console.WriteLine($"Request to {request.RequestUri} failed! Returned: {response.StatusCode}");
                json = string.Empty;
                serverInfo = null;
                return false;
            }

            json = response.Content.ReadAsStringAsync().Result;

            if (string.IsNullOrEmpty(json))
            {
                Console.WriteLine($"Request to {request.RequestUri} returned empty body!");
                json = string.Empty;
                serverInfo = null;
                return false;
            }

            string fhirVersion;
            using (JsonDocument jdoc = JsonDocument.Parse(json))
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

            if (!fhirConverter.TryParseResource(json, out var metadata, out string rt))
            {
                serverInfo = null;
                return false;
            }

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

                FhirManager.Current.TryResolveCanonicals(serverInfo, resolveExternal);

                return true;
            }
        }
#pragma warning disable CA1031 // Do not catch general exception types
        catch (Exception ex)
#pragma warning restore CA1031 // Do not catch general exception types
        {
            Console.WriteLine($"Failed to get server info from: {serverUrl}, {ex.Message}");
            json = string.Empty;
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

    /// <summary>Parse capability JSON.</summary>
    /// <param name="json">[out] The JSON.</param>
    /// <returns>A FhirCapabiltyStatement.</returns>
    public static FhirCapabiltyStatement ParseCapabilityJson(string json)
    {
        string url;
        string fhirVersion;
        using (JsonDocument jdoc = JsonDocument.Parse(json))
        {
            fhirVersion = jdoc.RootElement.GetProperty("fhirVersion").GetString();
            url = jdoc.RootElement.GetProperty("url").GetString() ?? string.Empty;
        }

        if (string.IsNullOrEmpty(fhirVersion))
        {
            Console.WriteLine($"ParseCapabilityJson <<< could not determine the FHIR version");
            return null;
        }

        IFhirConverter fhirConverter = ConverterHelper.ConverterForVersion(fhirVersion);

        if (!fhirConverter.TryParseResource(json, out var metadata, out string rt))
        {
            return null;
        }

        fhirConverter.ProcessMetadata(metadata, url, out FhirCapabiltyStatement serverInfo);

        return serverInfo;
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
                Console.WriteLine($"Request to {request.RequestUri} failed! Returned: {response.StatusCode}");
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
    }

}
