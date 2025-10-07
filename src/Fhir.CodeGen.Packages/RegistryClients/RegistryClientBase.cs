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
    internal HttpClient _httpClient = null!;
    internal PackageRegistryRecord _registryRecord = null!;

    public virtual bool SupportsFindByName => false;

    public virtual bool SupportsFindByCanonical => false;

    public virtual bool SupportsFindByFhirVersion => false;

    public virtual List<RegistryCatalogRecord>? Find(
        string? name = null,
        string? packageCanonical = null,
        string? canonical = null,
        string? fhirVersion = null) => throw new NotImplementedException();
    public virtual FullPackageManifest? GetFullManifest(string packageId) => throw new NotImplementedException();

    internal (string? result, HttpStatusCode status) doJsonGet(Uri requestUri)
    {
        HttpRequestMessage request = new()
        {
            Method = HttpMethod.Get,
            RequestUri = requestUri,
        };

        if (_registryRecord.AuthHeaderValue is not null)
        {
            request.Headers.Add("Authorization", _registryRecord.AuthHeaderValue);
        }

        if (_registryRecord.CustomHeaders is not null)
        {
            foreach ((string headerName, string headerValue) in _registryRecord.CustomHeaders)
            {
                request.Headers.Add(headerName, headerValue);
            }
        }

        request.Headers.Add("Accept", "application/json");

        if (_registryRecord.UserAgent is not null)
        {
            request.Headers.Add("User-Agent", _registryRecord.UserAgent);
        }
        else
        {
            request.Headers.Add("User-Agent", "Fhir.CodeGen.Packages");
        }

        HttpResponseMessage response = _httpClient.Send(request);
        if (response.IsSuccessStatusCode)
        {
            return (response.Content.ReadAsStringAsync().Result, response.StatusCode);
        }

        return (null, response.StatusCode);
    }
}
