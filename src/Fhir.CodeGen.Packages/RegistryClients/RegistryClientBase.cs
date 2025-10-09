using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Fhir.CodeGen.Packages.Models;

namespace Fhir.CodeGen.Packages.RegistryClients;

public abstract class RegistryClientBase
{
    protected HttpClient _httpClient = null!;
    protected RegistryEndpointRecord _registryEndpoint = null!;

    public virtual bool SupportsFindByName => false;

    public virtual bool SupportsFindByCanonical => false;

    public virtual bool SupportsFindByFhirVersion => false;

    public virtual List<RegistryCatalogRecord>? Find(
        string? name = null,
        string? packageCanonical = null,
        string? canonical = null,
        string? fhirVersion = null) => throw new NotImplementedException();
    public virtual FullPackageManifest? GetFullManifest(string packageId) => throw new NotImplementedException();

    public (HttpStatusCode status, string? result) GetJsonContent(Uri requestUri) => GetContent(requestUri, "application/json");

    public (HttpStatusCode status, string? result) GetContent(Uri requestUri, string acceptMimeType)
    {
        HttpRequestMessage request = new()
        {
            Method = HttpMethod.Get,
            RequestUri = requestUri,
        };

        if (_registryEndpoint.AuthHeaderValue is not null)
        {
            request.Headers.Add("Authorization", _registryEndpoint.AuthHeaderValue);
        }

        if (_registryEndpoint.CustomHeaders is not null)
        {
            foreach ((string headerName, string headerValue) in _registryEndpoint.CustomHeaders)
            {
                request.Headers.Add(headerName, headerValue);
            }
        }

        request.Headers.Add("Accept", acceptMimeType);

        if (_registryEndpoint.UserAgent is not null)
        {
            request.Headers.Add("User-Agent", _registryEndpoint.UserAgent);
        }
        else
        {
            request.Headers.Add("User-Agent", "Fhir.CodeGen.Packages");
        }

        HttpResponseMessage response = _httpClient.Send(request);
        if (response.IsSuccessStatusCode)
        {
            return (response.StatusCode, response.Content.ReadAsStringAsync().Result);
        }

        return (response.StatusCode, null);
    }

    public (HttpStatusCode status, Stream? content) GetHttpStream(Uri requestUri)
    {
        HttpRequestMessage request = new()
        {
            Method = HttpMethod.Get,
            RequestUri = requestUri,
        };

        if (_registryEndpoint.AuthHeaderValue is not null)
        {
            request.Headers.Add("Authorization", _registryEndpoint.AuthHeaderValue);
        }

        if (_registryEndpoint.CustomHeaders is not null)
        {
            foreach ((string headerName, string headerValue) in _registryEndpoint.CustomHeaders)
            {
                request.Headers.Add(headerName, headerValue);
            }
        }

        request.Headers.Add("Accept", "application/json");

        if (_registryEndpoint.UserAgent is not null)
        {
            request.Headers.Add("User-Agent", _registryEndpoint.UserAgent);
        }
        else
        {
            request.Headers.Add("User-Agent", "Fhir.CodeGen.Packages");
        }

        HttpResponseMessage response = _httpClient.Send(request);
        if (response.IsSuccessStatusCode)
        {
            return (response.StatusCode, response.Content.ReadAsStream());
        }

        return (response.StatusCode, null);
    }
}
