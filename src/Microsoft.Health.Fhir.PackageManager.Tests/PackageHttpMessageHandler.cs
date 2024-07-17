// <copyright file="PackageHttpMessageHandler.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation. All rights reserved.
//     Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// </copyright>

using System.Net;
using Xunit.Abstractions;

namespace Microsoft.Health.Fhir.PackageManager.Tests;

/// <summary>A package HTTP message handler.</summary>
public class PackageHttpMessageHandler : HttpMessageHandler
{
    public ITestOutputHelper? _testOutputHelper = null;

    /// <summary>Send an HTTP request as an asynchronous operation.</summary>
    /// <param name="request">          The HTTP request message to send.</param>
    /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
    /// <returns>The task object representing the asynchronous operation.</returns>
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        switch (request.RequestUri?.AbsoluteUri)
        {
            // r4 core
            case "http://packages.fhir.org/hl7.fhir.r4.core":
                {
                    return Task.FromResult(JsonFile("data/package-info-r4-core-primary.json"));
                }
            case "http://packages2.fhir.org/packages/hl7.fhir.r4.core":
                {
                    return Task.FromResult(JsonFile("data/package-info-r4-core-secondary.json"));
                }
            case "http://packages.fhir.org/catalog?op=find&name=hl7.fhir.r4&pkgcanonical=&canonical=&fhirversion=":
                {
                    return Task.FromResult(JsonFile("data/catalog-r4-primary.json"));
                }
            case "http://packages2.fhir.org/packages/catalog?op=find&name=hl7.fhir.r4&pkgcanonical=&canonical=&fhirversion=":
                {
                    return Task.FromResult(JsonFile("data/catalog-r4-secondary.json"));
                }
            case "http://packages.fhir.org/catalog?op=find&name=hl7.fhir.r4.core&pkgcanonical=&canonical=&fhirversion=":
                {
                    return Task.FromResult(JsonFile("data/catalog-r4-core-primary.json"));
                }
            case "http://packages2.fhir.org/packages/catalog?op=find&name=hl7.fhir.r4.core&pkgcanonical=&canonical=&fhirversion=":
                {
                    return Task.FromResult(JsonFile("data/catalog-r4-core-secondary.json"));
                }

            // us core
            case "http://packages.fhir.org/hl7.fhir.us.core":
                {
                    return Task.FromResult(JsonFile("data/package-info-us-core-primary.json"));
                }
            case "http://packages2.fhir.org/packages/hl7.fhir.us.core":
                {
                    return Task.FromResult(JsonFile("data/package-info-us-core-secondary.json"));
                }
            case "http://packages.fhir.org/catalog?op=find&name=hl7.fhir.us.core&pkgcanonical=&canonical=&fhirversion=":
                {
                    return Task.FromResult(JsonFile("data/catalog-us-core-primary.json"));
                }
            case "http://packages2.fhir.org/packages/catalog?op=find&name=hl7.fhir.us.core&pkgcanonical=&canonical=&fhirversion=":
                {
                    return Task.FromResult(JsonFile("data/catalog-us-core-secondary.json"));
                }

            // backport IG
            case "http://packages.fhir.org/hl7.fhir.uv.subscriptions-backport":
                {
                    return Task.FromResult(JsonFile("data/package-info-backport-primary.json"));
                }
            case "http://packages.fhir.org/hl7.fhir.uv.subscriptions-backport.r4":
                {
                    return Task.FromResult(JsonFile("data/package-info-backport-primary-r4.json"));
                }
            case "http://packages.fhir.org/hl7.fhir.uv.subscriptions-backport.r4b":
                {
                    return Task.FromResult(JsonFile("data/package-info-backport-primary-r4b.json"));
                }
            case "http://packages2.fhir.org/packages/hl7.fhir.uv.subscriptions-backport":
                {
                    return Task.FromResult(JsonFile("data/package-info-backport-secondary.json"));
                }
            case "http://packages2.fhir.org/packages/hl7.fhir.uv.subscriptions-backport.r4":
                {
                    return Task.FromResult(JsonFile("data/package-info-backport-secondary-r4.json"));
                }
            case "http://packages2.fhir.org/packages/hl7.fhir.uv.subscriptions-backport.r4b":
                {
                    return Task.FromResult(JsonFile("data/package-info-backport-secondary-r4b.json"));
                }
            case "http://build.fhir.org/ig/HL7/subscriptions-backport/package.manifest.json":
            case "https://build.fhir.org/ig/HL7/subscriptions-backport/package.manifest.json":
            case "http://build.fhir.org/ig/HL7/subscriptions-backport/branches/a-branch/package.manifest.json":
            case "https://build.fhir.org/ig/HL7/subscriptions-backport/branches/a-branch/package.manifest.json":
                {
                    return Task.FromResult(JsonFile("data/manifest-backport.json"));
                }
            case "http://build.fhir.org/ig/HL7/subscriptions-backport/package.r4.manifest.json":
            case "https://build.fhir.org/ig/HL7/subscriptions-backport/package.r4.manifest.json":
            case "http://build.fhir.org/ig/HL7/subscriptions-backport/branches/a-branch/package.r4.manifest.json":
            case "https://build.fhir.org/ig/HL7/subscriptions-backport/branches/a-branch/package.r4.manifest.json":
                {
                    return Task.FromResult(JsonFile("data/manifest-backport-r4.json"));
                }
            case "http://build.fhir.org/ig/HL7/subscriptions-backport/package.r4b.manifest.json":
            case "https://build.fhir.org/ig/HL7/subscriptions-backport/package.r4b.manifest.json":
            case "http://build.fhir.org/ig/HL7/subscriptions-backport/branches/a-branch/package.r4b.manifest.json":
            case "https://build.fhir.org/ig/HL7/subscriptions-backport/branches/a-branch/package.r4b.manifest.json":
                {
                    return Task.FromResult(JsonFile("data/manifest-backport-r4b.json"));
                }
            case "http://packages.fhir.org/catalog?op=find&name=hl7.fhir.uv.subscriptions-backport&pkgcanonical=&canonical=&fhirversion=":
                {
                    return Task.FromResult(JsonFile("data/catalog-backport-primary.json"));
                }
            case "http://packages2.fhir.org/packages/catalog?op=find&name=hl7.fhir.uv.subscriptions-backport&pkgcanonical=&canonical=&fhirversion=":
                {
                    return Task.FromResult(JsonFile("data/catalog-backport-secondary.json"));
                }
            case "http://packages.fhir.org/catalog?op=find&name=hl7.fhir.uv.subscriptions-backport.r4&pkgcanonical=&canonical=&fhirversion=":
                {
                    return Task.FromResult(JsonFile("data/catalog-backport-primary-r4.json"));
                }
            case "http://packages2.fhir.org/packages/catalog?op=find&name=hl7.fhir.uv.subscriptions-backport.r4&pkgcanonical=&canonical=&fhirversion=":
                {
                    return Task.FromResult(JsonFile("data/catalog-backport-secondary-r4.json"));
                }
            case "http://packages.fhir.org/catalog?op=find&name=hl7.fhir.uv.subscriptions-backport.r4b&pkgcanonical=&canonical=&fhirversion=":
                {
                    return Task.FromResult(JsonFile("data/catalog-backport-primary-r4b.json"));
                }
            case "http://packages2.fhir.org/packages/catalog?op=find&name=hl7.fhir.uv.subscriptions-backport.r4b&pkgcanonical=&canonical=&fhirversion=":
                {
                    return Task.FromResult(JsonFile("data/catalog-backport-secondary-r4b.json"));
                }

            // IHE PDQM
            case "https://profiles.ihe.net/ITI/PDQm/package.manifest.json":
                {
                    return Task.FromResult(JsonFile("data/manifest-ihe-pdqm.json"));
                }

            // qas.json
            case "http://build.fhir.org/ig/qas.json":
            case "https://build.fhir.org/ig/qas.json":
                {
                    return Task.FromResult(JsonFile("data/qas-full.json"));
                }

            // ci core versions
            case "http://build.fhir.org/version.info":
            case "https://build.fhir.org/version.info":
            case "http://build.fhir.org/branches/branch/version.info":
            case "https://build.fhir.org/branches/branch/version.info":
                {
                    return Task.FromResult(IniFile("data/version.info"));
                }

            // ci backport manifests

            //// URL-based directive resolution
            //case "https://hl7.org/fhir/uv/subscriptions-backport/version.info":
            //    {

            //    }
            //    break;

            default:
                {
                    _testOutputHelper?.WriteLine($"{request.Method} {request.RequestUri?.AbsoluteUri} is not implemented.");
                    //throw new NotImplementedException($"The request URI {request.RequestUri?.AbsoluteUri} is not implemented.");
                    return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));
                }
        }
    }

    /// <summary>Creates a JSON response message based on content.</summary>
    /// <param name="content">   The content.</param>
    /// <param name="statusCode">(Optional) The status code.</param>
    /// <returns>A HttpResponseMessage.</returns>
    internal static HttpResponseMessage JsonFile(
            string filename,
            HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        return new HttpResponseMessage(statusCode)
        {
            Content = new StringContent(File.ReadAllText(filename), new System.Net.Http.Headers.MediaTypeHeaderValue("application/json")),
        };
    }

    /// <summary>Creates an INI response message based on content.</summary>
    /// <param name="filename">  Filename of the file.</param>
    /// <param name="statusCode">(Optional) The status code.</param>
    /// <returns>A HttpResponseMessage.</returns>
    internal static HttpResponseMessage IniFile(
        string filename,
        HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        return new HttpResponseMessage(statusCode)
        {
            Content = new StringContent(File.ReadAllText(filename), new System.Net.Http.Headers.MediaTypeHeaderValue("text/plain")),
        };
    }

    /// <summary>Creates a JSON response message based on content.</summary>
    /// <param name="content">   The content.</param>
    /// <param name="statusCode">(Optional) The status code.</param>
    /// <returns>A HttpResponseMessage.</returns>
    internal static HttpResponseMessage JsonContent(
            string content,
            HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        return new HttpResponseMessage(statusCode)
        {
            Content = new StringContent(content, new System.Net.Http.Headers.MediaTypeHeaderValue("application/json")),
        };
    }
}
